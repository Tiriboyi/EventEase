using Azure.Storage.Blobs;
using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Models;

namespace EventEase.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseContext _context;
        private readonly BlobContainerClient _blobContainerClient;

        public VenueController(EventEaseContext context, BlobContainerClient blobContainerClient)
        {
            _context = context;
            _blobContainerClient = blobContainerClient;
        }

        private async Task<string?> GenerateImageSasUrlAsync(string? blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl)) return null;

            var uri = new Uri(blobUrl);
            var blobClient = _blobContainerClient.GetBlobClient(Path.GetFileName(uri.LocalPath));

            // Generate SAS token valid for 1 hour
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _blobContainerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate SAS token
            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }        // GET: Venue
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            foreach (var venue in venues)
            {
                venue.ImageUrl = await GenerateImageSasUrlAsync(venue.ImageUrl);
            }
            return View(venues);
        }

        // GET: Venue/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            venue.ImageUrl = await GenerateImageSasUrlAsync(venue.ImageUrl);
            return View(venue);
        }

        // GET: Venue/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,Availability")] Venue venue, IFormFile imageFile)
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
                    venue.ImageUrl = blobClient.Uri.ToString();
                }
                else
                {
                    venue.ImageUrl = null;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // POST: Venue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,Availability,ImageUrl")] Venue venue, IFormFile imageFile)
        {
            if (id != venue.VenueId) return NotFound();

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
                        venue.ImageUrl = blobClient.Uri.ToString();
                    }
                    else
                    {
                        var existingVenue = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);
                        venue.ImageUrl = existingVenue?.ImageUrl;
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venues.Any(v => v.VenueId == venue.VenueId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            if (_context.Bookings.Any(b => b.VenueId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has associated bookings.";
            }

            return View(venue);
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            if (_context.Bookings.Any(b => b.VenueId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has associated bookings.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            if (!string.IsNullOrEmpty(venue.ImageUrl))
            {
                var blobClient = _blobContainerClient.GetBlobClient(Path.GetFileName(new Uri(venue.ImageUrl).LocalPath));
                await blobClient.DeleteIfExistsAsync();
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}