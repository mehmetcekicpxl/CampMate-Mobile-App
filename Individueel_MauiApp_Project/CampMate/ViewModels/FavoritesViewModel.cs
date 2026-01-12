using CampMate.Messages;
using CampMate.Models;
using CampMate.Services;
using CampMate.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace CampMate.ViewModels
{
    public partial class FavoritesViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        // Lijst met favoriete kampen
        public ObservableCollection<CampSite> FavoriteCamps { get; } = new();

        public FavoritesViewModel(AuthService authService)
        {
            _authService = authService;

           
        }

        // --- COMMANDS ---

        [RelayCommand]
        async Task RemoveFavoriteAsync(CampSite campToRemove)
        {
            if (campToRemove == null) return;

            // Verwijder uit UI lijst
            FavoriteCamps.Remove(campToRemove);

            // Opslaan in opslag
            await SaveCurrentFavoritesToStorageAsync();

            // Andere pagina's informeren
            WeakReferenceMessenger.Default.Send(new FavoritesUpdatedMessage(true));
        }

        // --- HELPERS ---

        public async void LoadFavoritesFromStorageAsync()
        {
            FavoriteCamps.Clear();

            try
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null) return;

                string storageKey = $"Favorites_{currentUser.Email}";
                string json = await SecureStorage.GetAsync(storageKey);

                if (!string.IsNullOrEmpty(json))
                {
                    var dtos = JsonSerializer.Deserialize<List<CampSiteDto>>(json);
                    if (dtos != null)
                    {
                        foreach (var dto in dtos)
                        {
                            FavoriteCamps.Add(new CampSite(dto));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Fout", "Fout bij het laden van favorieten: " + ex.Message, "OK");
            }
        }

        private async Task SaveCurrentFavoritesToStorageAsync()
        {
            try
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null) return;

                string storageKey = $"Favorites_{currentUser.Email}";
                var dtosToSave = FavoriteCamps.Select(c => c.Data).ToList();
                string json = JsonSerializer.Serialize(dtosToSave);

                await SecureStorage.SetAsync(storageKey, json);
            }
            catch { }
        }
    }
}