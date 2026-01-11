using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Shared.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
       
        public string ProfileImage { get; set; } = "profile.png";

        public bool IsAdmin { get; set; }
    }
}
