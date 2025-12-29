using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CampMate.Shared.DTOs
{
    public class ChangePasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
