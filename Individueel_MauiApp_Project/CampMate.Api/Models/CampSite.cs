using System.ComponentModel.DataAnnotations.Schema;

namespace CampMate.Api.Models
{
    public class CampSite
    {
        public int Id { get; set; }  // Primary key
        public string Name { get; set; }
        public string Location { get; set; }
        public int Stars { get; set; }
        public string Facilities { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public bool IsFavorite { get; set; } = false;

        // Rating
        public double TotalStars { get; set; }
        public int RatingCount { get; set; }

        // Navigation property for Comments
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public int UserId { get; set; } // FK

        [ForeignKey("UserId")] 
        public User? User { get; set; }

        public string? WebsiteUrl { get; set; }
    }
}
