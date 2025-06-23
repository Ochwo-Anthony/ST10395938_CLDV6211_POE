using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Events
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Event description is required")]
        public string EventDescription { get; set; }

        [Required(ErrorMessage = "Venue ID is required")]
        public int VenueId { get; set; }

        [ForeignKey("VenueId")]
        public Venues? Venues { get; set; }

        [Required(ErrorMessage = "Event type is required")]
        public int EventTypeId { get; set; }

        [ForeignKey("EventTypeId")]
        public EventTypes? EventType { get; set; }

        public List<Bookings> Bookings { get; set; } = new();
    }
}
