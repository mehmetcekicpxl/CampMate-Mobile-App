using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Models
{
    public class CampSite : INotifyPropertyChanged // Om de puntenwijziging van de UI te zien
    {
        
            public string Name { get; set; }
            public string Location { get; set; }
            public int Stars { get; set; }
            public string Facilities { get; set; }
            public int Price { get; set; }
            public string Image { get; set; }
            public bool IsFavorite { get; set; } =false;



      
        public double TotalStars { get; set; } 
        public int RatingCount { get; set; }  

        
        public double AverageStars => RatingCount > 0 ? Math.Round(TotalStars / RatingCount, 1) : 0;

      
        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // De UI wordt geactiveerd wanneer het punt verandert.
        public void RefreshUI()
        {
            OnPropertyChanged(nameof(AverageStars));
            OnPropertyChanged(nameof(Stars));
        }
        public class Comment
        {
            public string UserName { get; set; }
            public string Text { get; set; }
        }

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
