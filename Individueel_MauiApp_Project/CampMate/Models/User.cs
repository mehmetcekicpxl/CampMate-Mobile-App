using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Models
{
   public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
       
        public string ProfileImage { get; set; } = "profile.png";

        public User()
        {
         
        }
    }
}
