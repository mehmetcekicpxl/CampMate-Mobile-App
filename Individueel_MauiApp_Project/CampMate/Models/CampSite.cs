using CampMate.Shared.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CampMate.Models
{
    public class CampSite : INotifyPropertyChanged
    {
        public CampSiteDto Data { get; set; } = new CampSiteDto();

        public CampSite()
        {
           
        }

        
        public CampSite(CampSiteDto dto)
        {
            Data = dto;


            // We transfer the favorite information from DTO to Model's own variable.
            _isFavorite = dto.IsFavorite;
           

            LoadCommentsFromDto();
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged(nameof(IsFavorite));

                
                    if (Data != null)
                    {
                        Data.IsFavorite = value;
                    }
                }
            }
        }

        public double AverageStars => Data.RatingCount > 0
              ? (double)Data.TotalStars / Data.RatingCount
              : 0;

        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();

        public string Name => Data.Name;
        public string Location => Data.Location;
        public string Facilities => Data.Facilities;
        public decimal Price => Data.Price;
        public string ImagePath => Data.Image;

        public string? WebsiteUrl => Data.WebsiteUrl;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void RefreshUI()
        {
            OnPropertyChanged(nameof(AverageStars));
            OnPropertyChanged(nameof(Data));
        }

        public class Comment
        {
            public string UserName { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
        }

        public void LoadCommentsFromDto()
        {
            Comments.Clear();
            if (Data.Comments != null) 
            {
                foreach (var c in Data.Comments)
                    Comments.Add(new Comment { UserName = c.UserName, Text = c.Text });
            }
        }
    }
}