using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventSearchModel
    {
        public string? SearchTerm { get; set; }

        public int? EventTypeId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public bool? VenueAvailability { get; set; }

        public int? MinCapacity { get; set; }

        public int? MaxCapacity { get; set; }

        public string? Location { get; set; }        public SearchResult SearchResults { get; set; } = new();
        public List<EventType> EventTypes { get; set; } = new();
    }
}
