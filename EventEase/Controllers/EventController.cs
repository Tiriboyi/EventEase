using EventEase.Data;
using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEaseContext _context;
        private readonly EventSearchService _searchService;
        private readonly BlobContainerClient _blobContainerClient;

        public EventController(EventEaseContext context, EventSearchService searchService, BlobContainerClient blobContainerClient)
        {
            _context = context;
            _searchService = searchService;
            _blobContainerClient = blobContainerClient;
        }

        private async Task<string?> GenerateImageSasUrlAsync(string? blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl)) return "/images/istockphoto.jpg";

            var uri = new Uri(blobUrl);
            var blobClient = _blobContainerClient.GetBlobClient(Path.GetFileName(uri.LocalPath));

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _blobContainerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddHours(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(24)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }        // GET: Event
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .ToListAsync();

            foreach (var @event in events)
            {
                @event.ImageUrl = await GenerateImageSasUrlAsync(@event.ImageUrl);
            }
            return View(events);
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return NotFound();

            // Generate SAS URL for the image
            @event.ImageUrl = await GenerateImageSasUrlAsync(@event.ImageUrl);

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "TypeName");
            return View();
        }        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,VenueId,EventTypeId,Description")] Event @event, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var blobClient = _blobContainerClient.GetBlobClient(fileName);
                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, overwrite: true);
                    }
                    @event.ImageUrl = blobClient.Uri.ToString();
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);
            return View(@event);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);
            
            if (@event == null) return NotFound();

            // Generate SAS URL for the image
            @event.ImageUrl = await GenerateImageSasUrlAsync(@event.ImageUrl);

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);
            return View(@event);
        }        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,VenueId,EventTypeId,Description,ImageUrl")] Event @event, IFormFile? imageFile)
        {
            if (id != @event.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var blobClient = _blobContainerClient.GetBlobClient(fileName);
                        using (var stream = imageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, overwrite: true);
                        }
                        @event.ImageUrl = blobClient.Uri.ToString();
                    }
                    else
                    {
                        var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == id);
                        @event.ImageUrl = existingEvent?.ImageUrl;
                    }

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.EventId == @event.EventId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return NotFound();

            if (_context.Bookings.Any(b => b.EventId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has associated bookings.";
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            if (_context.Bookings.Any(b => b.EventId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has associated bookings.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Search
        public async Task<IActionResult> Search(EventSearchModel searchModel)
        {
            if (searchModel == null)
            {
                searchModel = new EventSearchModel();
            }

            // Populate filter options
            searchModel.EventTypes = await _context.EventTypes.ToListAsync();            // Get search results
            if (!string.IsNullOrEmpty(searchModel.SearchTerm) || 
                searchModel.EventTypeId.HasValue || 
                searchModel.StartDate.HasValue || 
                searchModel.EndDate.HasValue ||
                searchModel.VenueAvailability.HasValue ||
                searchModel.MinCapacity.HasValue ||
                searchModel.MaxCapacity.HasValue ||
                !string.IsNullOrEmpty(searchModel.Location))
            {
                searchModel.SearchResults = await _searchService.SearchAsync(searchModel);
            }

            return View(searchModel);
        }
    }
}