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


                
                await RefreshCampDataAsync();
            }
        }

        private async Task RefreshCampDataAsync()
        {
            if (Camp == null) return;

            var freshData = await _campService.GetCampByIdAsync(Camp.Id);
            if (freshData != null)
            {
                Camp = freshData;
                CalculateCampDeletePermission();
                LoadComments(); 
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

            
            if (commentItem.Id == 0)
            {
                await Shell.Current.DisplayAlert("Fout", "Ververs de pagina om deze reactie te verwijderen.", "OK");
                return;
            }

            bool answer = await Shell.Current.DisplayAlert("Verwijder", "Weet je zeker dat je wilt verwijderen?", "Ja", "Nee");
            if (!answer) return;

            bool success = await _campService.DeleteCommentAsync(commentItem.Id);

            if (success)
            {
                
                CommentsList.Remove(commentItem);
                await Shell.Current.DisplayAlert("Succesvol", "De reactie is verwijderd.", "OK");

                // sync met de server
                await RefreshCampDataAsync();
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

            bool success = await _authService.AddCommentAsync(Camp.Id, userDto.Name, NewCommentText, 0);

            if (success)
            {
                NewCommentText = string.Empty;
                await Shell.Current.DisplayAlert("Succesvol", "Uw opmerking is toegevoegd.", "OK");

                
                await RefreshCampDataAsync();
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

            int ratingToSend = SelectedRatingIndex + 1;
            var userDto = await _authService.GetCurrentUserAsync();

            bool success = await _authService.AddCommentAsync(Camp.Id, userDto.Name, "", ratingToSend);

            if (success)
            {
                SelectedRatingIndex = -1;
                await Shell.Current.DisplayAlert("Bedankt", $" Je hebt {ratingToSend} ster gegeven!", "OK");

                
                await RefreshCampDataAsync();

               
                WeakReferenceMessenger.Default.Send(new CampUpdatedMessage(Camp));
            }
        }


       

       [RelayCommand]
        async Task OpenWebsiteAsync()
        {
            if (string.IsNullOrWhiteSpace(Camp?.WebsiteUrl)) return;

            try
            {
                string url = Camp.WebsiteUrl;
        
       
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
                    !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "https://" + url;
                }

       
                await Launcher.Default.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Fout", $"De link kan niet worden geopend: {ex.Message}", "OK");
            }
        }
    }
}