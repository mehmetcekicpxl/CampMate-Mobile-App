using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Shared.DTOs
{
    public class UserUpdateDto
    {
      
        public string Name { get; set; } = string.Empty;

        // Het nieuwe e-mailadres (moet uniek zijn)
        public string Email { get; set; } = string.Empty;

        
        public string PhoneNumber { get; set; } = string.Empty;

       
        public string Location { get; set; } = string.Empty;
    }
}
