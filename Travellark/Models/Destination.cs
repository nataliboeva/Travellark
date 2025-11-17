using System.ComponentModel.DataAnnotations;
using Travellark.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Travellark.Models
{
    public class Destination
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Region")]
        public string? Region { get; set; }

        [Display(Name = "Place Type")]
        public DestinationType Type { get; set; }

        [Display(Name = "Travel Status")]
        public DestinationStatus Status { get; set; }

        [Display(Name = "Rating")]
        public int? Rating { get; set; }

        [Display(Name = "Why It Inspires You")]
        public string? InspirationText { get; set; }

        [Display(Name = "Personal Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }


        [Display(Name = "Map Link")]
        public string? MapUrl { get; set; }

        [Display(Name = "Added On")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Visited On")]
        public DateTime? VisitedAt { get; set; }
    }
}
