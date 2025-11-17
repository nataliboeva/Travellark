using System.ComponentModel.DataAnnotations;

namespace Travellark.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int DestinationId { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Destination? Destination { get; set; }
    }
}

