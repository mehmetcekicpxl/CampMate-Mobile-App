using System.ComponentModel.DataAnnotations.Schema;

namespace CampMate.Api.Models
{
    public class Comment
    {
        public int Id { get; set; }  // Primary key
        public string UserName { get; set; }
        public string Text { get; set; }

        // Foreign key to CampSite
        public int CampSiteId { get; set; }

        [ForeignKey("CampSiteId")]
        public CampSite? CampSite { get; set; }

        public int UserId { get; set; } // FK

        [ForeignKey("UserId")]
        public User? User { get; set; } 
    }
}
