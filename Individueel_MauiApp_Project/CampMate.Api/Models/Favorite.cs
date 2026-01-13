using System.ComponentModel.DataAnnotations.Schema;

namespace CampMate.Api.Models
{
    public class Favorite
    {
        public int Id { get; set; } 

        public int UserId { get; set; } 
        public int CampSiteId { get; set; } 

       
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CampSiteId")]
        public CampSite CampSite { get; set; }
    }
}