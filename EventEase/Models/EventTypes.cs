using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventTypes
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required(ErrorMessage = "Event type name is required")]
        public string EventTypeName { get; set; }

        public List<Events> Events { get; set; } = new();
    }
}
