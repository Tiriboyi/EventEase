using System.Collections.Generic;

namespace EventEase.Models
{
    public class SearchResult
    {
        public List<Event> Events { get; set; } = new();
        public List<Venue> Venues { get; set; } = new();
        public string SearchTerm { get; set; }
        public bool HasResults => Events.Any() || Venues.Any();
    }
}
