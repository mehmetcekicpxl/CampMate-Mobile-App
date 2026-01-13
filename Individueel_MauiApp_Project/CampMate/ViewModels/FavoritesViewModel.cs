using CampMate.Messages;
using CampMate.Models;
using CampMate.Services;
using CampMate.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace CampMate.ViewModels
{
    public partial class FavoritesViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private bool _isBusy;

        // Lijst met favoriete kampen
        public ObservableCollection<CampSite> FavoriteCamps { get; } = new();


        public FavoritesViewModel(AuthService authService, HttpClient httpClient)
        {
            _authService = authService;
            _httpClient = httpClient;
        }

        public async Task LoadFavoritesAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                FavoriteCamps.Clear();

                int userId = Preferences.Get("UserId", 0);

                // URL CORRECTIE: "api/campsites" met userId
                var dtos = await _httpClient.GetFromJsonAsync<List<CampSiteDto>>($"campsites?userId={userId}");

                if (dtos != null)
                {
                    // Filter alleen de favorieten
                    var favorites = dtos.Where(c => c.IsFavorite).ToList();
                    var localImages = new List<string> { "kampa.jpg", "kampb.jpg", "bg.jpg", "campmatelogo.png" };

                    foreach (var dto in favorites)
                    {
                        if (!string.IsNullOrEmpty(dto.Image) && !dto.Image.StartsWith("http") && !localImages.Contains(dto.Image))
                        {
                            dto.Image = $"http://10.0.2.2:5225/images/{dto.Image}";
                        }
                        FavoriteCamps.Add(new CampSite(dto));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task RemoveFavoriteAsync(CampSite campToRemove)
        {
            if (campToRemove == null) return;
            FavoriteCamps.Remove(campToRemove);

            try
            {
                int userId = Preferences.Get("UserId", 0);

                // Verzoek sturen met userId parameter
                var response = await _httpClient.PostAsync($"campsites/toggle-favorite/{campToRemove.Data.Id}?userId={userId}", null);

                if (response.IsSuccessStatusCode)
                {
                    WeakReferenceMessenger.Default.Send(new FavoritesUpdatedMessage(true));
                }
                else
                {
                    FavoriteCamps.Add(campToRemove); // Terugzetten bij fout
                }
            }
            catch
            {
                FavoriteCamps.Add(campToRemove);
            }
        }

    }

       
        
       
}