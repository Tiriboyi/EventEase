using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required(ErrorMessage = "Event type name is required.")]
        [StringLength(50, ErrorMessage = "Event type name cannot be longer than 50 characters.")]
        [Display(Name = "Event Type")]
        public string TypeName { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public string? Description { get; set; }

        public ICollection<Venue> Venues { get; set; }

        public EventType()
        {
            Venues = new List<Venue>();
        }
    }
}