using CampMate.Messages;
using CampMate.Services;
using CampMate.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace CampMate.ViewModels
{
   
    public partial class CommentItemViewModel : ObservableObject
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public bool CanDeleteComment { get; set; } 
    }

    public partial class CampDetailViewModel : ObservableObject, IQueryAttributable
    {
        private readonly AuthService _authService;
        private readonly CampService _campService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CampImageUrl))]
        private CampSiteDto _camp;

        //om te helpen bij het debuggen van machtigingen
        private string _debugInfo;
        public string DebugInfo
        {
            get => _debugInfo;
            set => SetProperty(ref _debugInfo, value);
        }

        // HEBT U DE BEVOEGDHEID OM KAMPEN TE VERWIJDEREN?
        [ObservableProperty]
        private bool _canDeleteCamp;

       
        public ObservableCollection<CommentItemViewModel> CommentsList { get; set; } = new();

        // Nieuwe beoordeling en score
        [ObservableProperty]
        private string _newCommentText;

        [ObservableProperty]
        private int _selectedRatingIndex = -1;

        public CampDetailViewModel(AuthService authService, CampService campService)
        {
            _authService = authService;
            _campService = campService;
        }

       
        public string CampImageUrl
        {
            get
            {
               
                if (Camp == null || string.IsNullOrEmpty(Camp.Image)) return "campmatelogo.png";

               
                var localImages = new List<string> { "kampa.jpg", "kampb.jpg", "bg.jpg", "campmatelogo.png" };

               
               
                foreach (var localImage in localImages)
                {
                    if (Camp.Image.EndsWith(localImage))
                    {
                        return localImage;
                    }
                }

               

                
                return $"http://10.0.2.2:5225/images/{Camp.Image}";
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // 1. Neem eerst de gegevens op die worden doorgegeven bij het overschakelen tussen pagina's (voor een snelle opening).
            var initialData = query["Camp"] as CampSiteDto;

            if (initialData != null)
            {
                Camp = initialData;
                LoadComments();

               
                var freshData = await _campService.GetCampByIdAsync(initialData.Id);

                if (freshData != null)
                {
                    
                    Camp = freshData;

                    // Bereken de rechten opnieuw (misschien is de beheerdersstatus gewijzigd, enz.)
                    CalculateCampDeletePermission();

                    // Vernieuw de lijst met reacties met die van de server!
                    LoadComments();
                }
            }
        }


        private void CalculateCampDeletePermission()
        {
            int currentUserId = Preferences.Get("UserId", -999);
            bool isAdmin = Preferences.Get("IsAdmin", false);

            // Eigenaar OF Admin?
            CanDeleteCamp = (Camp.UserId == currentUserId) || isAdmin;
            // Voor debuggen
            DebugInfo = $"Kamp-eigenaar ID: {Camp.UserId} | Mijn ID: {currentUserId} | Is Admin ?: {isAdmin}";
        }

        private void LoadComments()
        {
            CommentsList.Clear();
            if (Camp.Comments != null)
            {
                int currentUserId = Preferences.Get("UserId", -1);
                bool isAdmin = Preferences.Get("IsAdmin", false);

                foreach (var c in Camp.Comments)
                {

                    //bool canDeleteThisComment = (c.UserId == currentUserId) || isAdmin;

                   
                    bool isOwnerOrAdmin = (c.UserId == currentUserId) || isAdmin;

                   
                    bool isRatingMessage = c.Text.Contains("★");

                    
                    bool canDelete = isOwnerOrAdmin && !isRatingMessage;

                    CommentsList.Add(new CommentItemViewModel
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Text = c.Text,
                        UserId = c.UserId,
                        CanDeleteComment = canDelete
                    });
                }
            }
        }

      
        [RelayCommand]
        async Task DeleteCampAsync()
        {
            bool answer = await Shell.Current.DisplayAlert("Let op", "Wilt u dit kamp echt verwijderen?", "Ja, verwijderen", "Annuleren");
            if (!answer) return;

            bool success = await _campService.DeleteCampAsync(Camp.Id);
            if (success)
            {
                WeakReferenceMessenger.Default.Send(new CampDeletedMessage(Camp.Id));

                await Shell.Current.DisplayAlert("Succesvol", "Het kamp is verwijderd.", "OK");
                await Shell.Current.GoToAsync(".."); 
            }
            else
            {
                await Shell.Current.DisplayAlert("Fout", "Het kamp kon niet worden gewist.", "OK");
            }
        }

        [RelayCommand]
        async Task DeleteCommentAsync(CommentItemViewModel commentItem)
        {
            if (commentItem == null) return;

            bool answer = await Shell.Current.DisplayAlert("Verwijder", "Weet je zeker dat je wilt verwijderen?", "Ja", "Nee");
            if (!answer) return;

            // 1. Verwijderen uit backend
            bool success = await _campService.DeleteCommentAsync(commentItem.Id);

            if (success)
            {
                // 2. Visueel verwijderen uit de lijst
                CommentsList.Remove(commentItem);

              

                int ratingToRemove = 0;
                string text = commentItem.Text.Trim(); 

                if (text.Contains("ster") && text.Length > 0)
                {
                   
                    string firstChar = text.Substring(0, 1);

                    if (int.TryParse(firstChar, out int parsedRating))
                    {
                        ratingToRemove = parsedRating; 
                    }
                }

                // Als het verwijderde item een punt is, herstel dan het account.
                if (ratingToRemove > 0)
                {
                    
                    Camp.TotalStars -= ratingToRemove;

                    
                    if (Camp.RatingCount > 0) Camp.RatingCount--;

                    
                    if (Camp.TotalStars < 0) Camp.TotalStars = 0;

                    // Scherm bijwerken
                    OnPropertyChanged(nameof(Camp));

                    //  Meld het op de startpagina

                    WeakReferenceMessenger.Default.Send(new CampUpdatedMessage(Camp));

                    await Shell.Current.DisplayAlert("Informatie", "De punten zijn teruggenomen.", "OK");
                }
                else
                {
                    
                    await Shell.Current.DisplayAlert("Succesvol", "De reactie is verwijderd.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Fout", "Kan niet worden verwijderd.", "OK");
            }
        }

        

      
        [RelayCommand]
        async Task SubmitCommentAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCommentText)) return;

            var userDto = await _authService.GetCurrentUserAsync();
            if (userDto == null) return;

            // We geven een beoordeling van 0, omdat dit alleen een opmerking is.
            bool success = await _authService.AddCommentAsync(Camp.Id, userDto.Name, NewCommentText, 0);

            if (success)
            {
                CommentsList.Add(new CommentItemViewModel
                {
                    UserName = userDto.Name,
                    Text = NewCommentText,
                    UserId = userDto.Id,
                    CanDeleteComment = true,
                    
                });

                NewCommentText = string.Empty;
                await Shell.Current.DisplayAlert("Succesvol", "Uw opmerking is toegevoegd.", "OK");
            }
        }

       
        [RelayCommand]
        async Task SubmitRatingAsync()
        {
            if (SelectedRatingIndex == -1)
            {
                await Shell.Current.DisplayAlert("Waarschuwing", "Kies een punt.", "OK");
                return;
            }

            int ratingToSend = SelectedRatingIndex + 1; // 0 index = 1 punt
            var userDto = await _authService.GetCurrentUserAsync();

           
            bool success = await _authService.AddCommentAsync(Camp.Id, userDto.Name, "", ratingToSend);

            if (success)
            {
              
                Camp.TotalStars += ratingToSend;
                Camp.RatingCount++;

                // Scherm vernieuwen
                OnPropertyChanged(nameof(Camp));

                
                WeakReferenceMessenger.Default.Send(new CampUpdatedMessage(Camp));

              
                CommentsList.Add(new CommentItemViewModel
                {
                    UserName = userDto.Name,
                    Text = $"{ratingToSend}  ster werd toegekend ★", // Het moet hetzelfde formaat hebben als in de backend.
                    UserId = userDto.Id,
                    CanDeleteComment = false,
                  
                });

                SelectedRatingIndex = -1; // Keuze resetten
                await Shell.Current.DisplayAlert("Bedankt", $" Je hebt  {ratingToSend}  ster gegeven!", "OK");
            }
        }

        [RelayCommand]
        async Task OpenWebsiteAsync()
        {
            if (string.IsNullOrWhiteSpace(Camp?.WebsiteUrl)) return;

            try
            {
                string url = Camp.WebsiteUrl;
                
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                
                await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Fout", "De link kan niet worden geopend.", "OK");
            }
        }
    }
}