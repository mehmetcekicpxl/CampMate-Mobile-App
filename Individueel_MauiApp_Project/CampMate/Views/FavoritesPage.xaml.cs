//using CampMate.Services;
//using CampMate.Shared.DTOs; 
//using CampMate.Models;
//using CampMate.Messages;
//using CommunityToolkit.Mvvm.Messaging;
//using System.Collections.ObjectModel;
//using System.Text.Json;

//namespace CampMate.Views
//{
//    public partial class FavoritesPage : ContentPage
//    {
//        public ObservableCollection<CampSite> FavoriteCamps { get; set; }
//        private readonly AuthService _authService; // Om te weten wie zich heeft aangemeld


//        public FavoritesPage(AuthService authService)
//        {
//            InitializeComponent();
//            _authService = authService;

//            FavoriteCamps = new ObservableCollection<CampSite>();
//            FavoritesCollectionView.ItemsSource = FavoriteCamps;
//        }

//        protected override async void OnAppearing()
//        {
//            base.OnAppearing();
//            await LoadFavoritesFromStorage();
//        }

//        private async Task LoadFavoritesFromStorage()
//        {
//            FavoriteCamps.Clear();

//            try
//            {
//                // 1. De gebruiker die zich aanmeldt, zoeken
//                var currentUser = await _authService.GetCurrentUserAsync();
//                if (currentUser == null) return;

//                // 2. MAAK EEN PERSOONLIJKE SLEUTEL AAN (bijv.: "Favorites_mehmet@gmail.com")
//                string storageKey = $"Favorites_{currentUser.Email}";

//                string json = await SecureStorage.GetAsync(storageKey);

//                if (!string.IsNullOrEmpty(json))
//                {

//                    var dtos = JsonSerializer.Deserialize<List<CampSiteDto>>(json);

//                    if (dtos != null)
//                    {
//                        foreach (var dto in dtos)
//                        {

//                            FavoriteCamps.Add(new CampSite(dto));
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                await DisplayAlert("Fout", "Fout bij het laden van favorieten: " + ex.Message, "OK");
//            }
//        }

//        private async void RemoveFavorite_Clicked(object sender, EventArgs e)
//        {
//            if (sender is ImageButton button && button.CommandParameter is CampSite campToRemove)
//            {
//                FavoriteCamps.Remove(campToRemove);


//                await SaveCurrentFavoritesToStorage();

//                // We waarschuwen andere pagina's (dat Favoriet is verwijderd)
//                WeakReferenceMessenger.Default.Send(new FavoritesUpdatedMessage(true));
//            }
//        }

//        private async Task SaveCurrentFavoritesToStorage()
//        {
//            var currentUser = await _authService.GetCurrentUserAsync();
//            if (currentUser == null) return;


//            string storageKey = $"Favorites_{currentUser.Email}";


//            var dtosToSave = FavoriteCamps.Select(c => c.Data).ToList();

//            string json = JsonSerializer.Serialize(dtosToSave);

//            await SecureStorage.SetAsync(storageKey, json);
//        }
//    }
//}


using CampMate.ViewModels;

namespace CampMate.Views;

public partial class FavoritesPage : ContentPage
{
    public FavoritesPage(FavoritesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Roep de methode in ViewModel aan om de lijst bij elke paginaweergave te vernieuwen.
        if (BindingContext is FavoritesViewModel vm)
        {
            vm.LoadFavoritesFromStorageAsync();
        }
    }
}