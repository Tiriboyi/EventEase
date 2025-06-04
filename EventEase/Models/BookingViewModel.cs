namespace EventEase.Models
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public DateTime BookingDate { get; set; }
        public string? ImageUrl { get; set; }
        public string EventType { get; set; }
        public bool Availability { get; set; }
    }
}