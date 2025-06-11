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

        public async Task<SearchResult> SearchAsync(EventSearchModel searchModel)
        {
            var searchResult = new SearchResult { SearchTerm = searchModel.SearchTerm };
            var cacheKey = $"search_{searchModel.SearchTerm}_{searchModel.EventTypeId}_{searchModel.StartDate}_{searchModel.EndDate}_{searchModel.VenueAvailability}_{searchModel.MinCapacity}_{searchModel.MaxCapacity}_{searchModel.Location}";

            if (_cache.TryGetValue<SearchResult>(cacheKey, out var cachedResults))
            {
                return cachedResults;
            }

            // Search Events
            var eventsQuery = _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Venue)
                .AsQueryable();

            // Apply text search filter for events
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var searchTerm = searchModel.SearchTerm.ToLower();
                // Use CONTAINS for full-text search on EventName and Description
                eventsQuery = eventsQuery.Where(e => 
                    EF.Functions.Contains(e.EventName, searchModel.SearchTerm) ||
                    EF.Functions.Contains(e.Description, searchModel.SearchTerm) ||
                    EF.Functions.Contains(e.Venue.VenueName, searchModel.SearchTerm) ||
                    EF.Functions.Contains(e.Venue.Location, searchModel.SearchTerm));
            }

            // Apply other event filters
            if (searchModel.EventTypeId.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventTypeId == searchModel.EventTypeId.Value);
            }

            if (searchModel.StartDate.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventDate.Date >= searchModel.StartDate.Value.Date);
            }

            if (searchModel.EndDate.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventDate.Date <= searchModel.EndDate.Value.Date);
            }

            // Apply venue filters
            if (searchModel.VenueAvailability.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.Venue.Availability == searchModel.VenueAvailability.Value);
            }

            if (searchModel.MinCapacity.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.Venue.Capacity >= searchModel.MinCapacity.Value);
            }

            if (searchModel.MaxCapacity.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.Venue.Capacity <= searchModel.MaxCapacity.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Location))
            {
                eventsQuery = eventsQuery.Where(e => EF.Functions.Contains(e.Venue.Location, searchModel.Location));
            }

            // Order events by date
            eventsQuery = eventsQuery.OrderByDescending(e => e.EventDate);
            searchResult.Events = await eventsQuery.ToListAsync();

            // Search Venues
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var venuesQuery = _context.Venues.AsQueryable();

                // Apply full-text search to venues
                venuesQuery = venuesQuery.Where(v => 
                    EF.Functions.Contains(v.VenueName, searchModel.SearchTerm) ||
                    EF.Functions.Contains(v.Location, searchModel.SearchTerm));

                // Apply venue filters
                if (searchModel.VenueAvailability.HasValue)
                {
                    venuesQuery = venuesQuery.Where(v => v.Availability == searchModel.VenueAvailability.Value);
                }

                if (searchModel.MinCapacity.HasValue)
                {
                    venuesQuery = venuesQuery.Where(v => v.Capacity >= searchModel.MinCapacity.Value);
                }

                if (searchModel.MaxCapacity.HasValue)
                {
                    venuesQuery = venuesQuery.Where(v => v.Capacity <= searchModel.MaxCapacity.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchModel.Location))
                {
                    venuesQuery = venuesQuery.Where(v => EF.Functions.Contains(v.Location, searchModel.Location));
                }

                // Get venues that aren't already included in the events results
                var eventVenueIds = searchResult.Events.Select(e => e.VenueId).ToList();
                venuesQuery = venuesQuery.Where(v => !eventVenueIds.Contains(v.VenueId));

                searchResult.Venues = await venuesQuery.ToListAsync();
            }

            _cache.Set(cacheKey, searchResult, TimeSpan.FromMinutes(CacheExpirationMinutes));
            return searchResult;
        }
    }
}
