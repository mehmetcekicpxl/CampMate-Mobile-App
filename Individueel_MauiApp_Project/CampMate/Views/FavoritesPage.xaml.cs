using CampMate.Messages;
using CampMate.Models;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace CampMate.Views
{
    public partial class FavoritesPage : ContentPage
    {
     
        public ObservableCollection<CampSite> FavoriteCamps { get; set; }

        public FavoritesPage()
        {
            InitializeComponent();

            
            FavoriteCamps = new ObservableCollection<CampSite>();
            FavoritesCollectionView.ItemsSource = FavoriteCamps;
        }

        // Laad favorieten telkens wanneer de pagina wordt weergegeven
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadFavoritesFromStorage();
        }

        
        private async Task LoadFavoritesFromStorage()
        {
            FavoriteCamps.Clear(); 

            try
            {
                // Haal de JSON-string op uit SecureStorage
                string json = await SecureStorage.GetAsync("FavoriteCamps");

                if (!string.IsNullOrEmpty(json))
                {
                    // JSON naar CampSite-lijst converteren
                    var camps = JsonSerializer.Deserialize<List<CampSite>>(json);

                    if (camps != null)
                    {
                        // Toevoegen aan ObservableCollection
                        foreach (var camp in camps)
                        {
                            FavoriteCamps.Add(camp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", "Er is een probleem opgetreden tijdens het laden van de favorietenlijst.: " + ex.Message, "OK");
            }
        }

    
        private async void RemoveFavorite_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is CampSite campToRemove)
            {
               
                FavoriteCamps.Remove(campToRemove);

              
                await SaveCurrentFavoritesToStorage();

                WeakReferenceMessenger.Default.Send(new FavoritesUpdatedMessage(true));

                await DisplayAlert("Verwijderd", $"{campToRemove.Name} uit uw favorieten verwijderd.", "OK");

                
            }
        }

        
        private async Task SaveCurrentFavoritesToStorage()
        {
           
            var campsToSave = FavoriteCamps.ToList();
            string json = JsonSerializer.Serialize(campsToSave);

            await SecureStorage.SetAsync("FavoriteCamps", json);
        }
    }
}