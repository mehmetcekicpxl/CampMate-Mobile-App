using System.Collections.Generic;

namespace CampMate.Shared.DTOs
{
    public class CampSiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Facilities { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;

       
        public double TotalStars { get; set; }
        public int RatingCount { get; set; }
        public double AverageStars => RatingCount > 0 ? (double)TotalStars / RatingCount : 0;


        public bool IsFavorite { get; set; } = false;

        
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();

        public int UserId { get; set; }
    }
}