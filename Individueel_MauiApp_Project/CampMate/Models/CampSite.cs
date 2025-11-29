using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Models
{
    public class CampSite
    {
        
            public string Name { get; set; }
            public string Location { get; set; }
            public int Stars { get; set; }
            public string Facilities { get; set; }
            public int Price { get; set; }
            public string Image { get; set; }

        // full path property voor UI binding 
        public string ImagePath
        {
            get
            {
                
                if (string.IsNullOrEmpty(Image))
                    return null;

                // Als het door de app geselecteerde foto is AppDataDirectory
                var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "images", Image);
                if (File.Exists(appDataPath))
                    return appDataPath;

                
                return Image; // dotnet_bot.png in Resources/Images 
            }
        }


    }
}
