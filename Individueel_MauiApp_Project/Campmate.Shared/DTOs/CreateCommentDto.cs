using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Shared.DTOs
{
    public class CreateCommentDto
    {
        
        public int CampSiteId { get; set; }

      
        public string UserName { get; set; } = string.Empty;

   
        public string Text { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int Rating { get; set; }
    }
}
