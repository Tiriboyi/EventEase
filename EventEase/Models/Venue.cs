using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(100, ErrorMessage = "Venue name cannot be longer than 100 characters.")]
        public string VenueName { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100, ErrorMessage = "Location cannot be longer than 100 characters.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Availability is required.")]
        public bool Availability { get; set; }        public string? ImageUrl { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public Venue()
        {
            Bookings = new List<Booking>();
            Availability = true; // Default to available
        }
    }
}