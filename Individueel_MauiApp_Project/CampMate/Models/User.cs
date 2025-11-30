using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Models
{
   public  class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }

        public User()
        {
            Name="Mehmet";
            Email= "mehmetcekic@gmail.com";
            PhoneNumber= "+32 666 666 666";
            Location= "Genk";
        }
    }
}
