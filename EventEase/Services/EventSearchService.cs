using EventEase.Models;
using EventEase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace EventEase.Services
{
    public class EventSearchService
    {
        private readonly EventEaseContext _context;
        private readonly IMemoryCache _cache;
        private const int CacheExpirationMinutes = 15;

        public EventSearchService(EventEaseContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Event>> SearchEventsAsync(EventSearchModel searchModel)
        {
            var cacheKey = $"search_{searchModel.SearchTerm}_{searchModel.EventTypeId}_{searchModel.StartDate}_{searchModel.EndDate}_{searchModel.VenueAvailability}_{searchModel.MinCapacity}_{searchModel.MaxCapacity}_{searchModel.Location}";

            if (_cache.TryGetValue<List<Event>>(cacheKey, out var cachedResults))
            {
                return cachedResults;
            }

            var query = _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Venue)
                .AsQueryable();

            // Apply text search filter
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var searchTerm = searchModel.SearchTerm.ToLower();
                query = query.Where(e => 
                    EF.Functions.Like(e.EventName.ToLower(), $"%{searchTerm}%") || 
                    (e.Description != null && EF.Functions.Like(e.Description.ToLower(), $"%{searchTerm}%")) ||
                    EF.Functions.Like(e.EventType.TypeName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(e.Venue.VenueName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(e.Venue.Location.ToLower(), $"%{searchTerm}%"));
            }

            // Apply event type filter
            if (searchModel.EventTypeId.HasValue)
            {
                query = query.Where(e => e.EventTypeId == searchModel.EventTypeId.Value);
            }

            // Apply date range filters
            if (searchModel.StartDate.HasValue)
            {
                query = query.Where(e => e.EventDate.Date >= searchModel.StartDate.Value.Date);
            }

            if (searchModel.EndDate.HasValue)
            {
                query = query.Where(e => e.EventDate.Date <= searchModel.EndDate.Value.Date);
            }

            // Apply venue filters
            if (searchModel.VenueAvailability.HasValue)
            {
                query = query.Where(e => e.Venue.Availability == searchModel.VenueAvailability.Value);
            }

            if (searchModel.MinCapacity.HasValue)
            {
                query = query.Where(e => e.Venue.Capacity >= searchModel.MinCapacity.Value);
            }

            if (searchModel.MaxCapacity.HasValue)
            {
                query = query.Where(e => e.Venue.Capacity <= searchModel.MaxCapacity.Value);
            }

            // Apply location filter
            if (!string.IsNullOrWhiteSpace(searchModel.Location))
            {
                var location = searchModel.Location.ToLower();
                query = query.Where(e => EF.Functions.Like(e.Venue.Location.ToLower(), $"%{location}%"));
            }

            // Order by date descending by default
            query = query.OrderByDescending(e => e.EventDate);

            var results = await query.ToListAsync();
            
            _cache.Set(cacheKey, results, TimeSpan.FromMinutes(CacheExpirationMinutes));
            
            return results;
        }
    }
}
