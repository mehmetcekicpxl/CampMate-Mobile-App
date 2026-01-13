using CampMate.Messages;
using CampMate.Models;
using CampMate.Services;
using CampMate.Shared.DTOs;
using CampMate.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace CampMate.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        // Lijst die op het scherm wordt getoond
        public ObservableCollection<CampSite> CampList { get; } = new();

        // Back-uplijst voor filteren
        private List<CampSite> _allCamps = new();

        [ObservableProperty]
        private string _searchText;

        // Wordt geactiveerd wanneer de zoektekst wijzigt
        partial void OnSearchTextChanged(string value)
        {
            FilterCamps(value);
        }

        public HomePageViewModel(AuthService authService, HttpClient httpClient)
        {
            _authService = authService;
            _httpClient = httpClient;

            RegisterMessages();
            LoadCampsFromApi();
        }

        private void RegisterMessages()
        {
            // Luister naar favorieten updates
            WeakReferenceMessenger.Default.Register<FavoritesUpdatedMessage>(this, async (r, m) =>
            {
                await RefreshFavoritesAsync();
            });

            // Luister naar filtercriteria
            WeakReferenceMessenger.Default.Register<FilterCriteriaMessage>(this, (r, m) =>
            {
                ApplyFilter(m.Value);
            });

            // Luister naar nieuwe kamp toevoegingen
            WeakReferenceMessenger.Default.Register<NewCampMessage>(this, (r, m) =>
            {
                AddNewCampToList(m.Value);
            });

            // Luister naar verwijderde kampen
            WeakReferenceMessenger.Default.Register<CampDeletedMessage>(this, (r, m) =>
            {
                RemoveCampFromList(m.Value);
            });

            // Luister naar updates (bijv. rating wijzigingen)
            WeakReferenceMessenger.Default.Register<CampUpdatedMessage>(this, (r, m) =>
            {
                UpdateCampInList(m.Value);
            });
        }

        private async void LoadCampsFromApi()
        {
            try
            {
                var dtos = await _httpClient.GetFromJsonAsync<List<CampSiteDto>>("campsites");

                if (dtos != null)
                {
                    var localFavorites = await LoadFavoritesFromStorageAsync();
                    var favoriteNames = localFavorites.Select(f => f.Name).ToHashSet();

                    _allCamps.Clear();
                    CampList.Clear();

                    var localImages = new List<string> { "kampa.jpg", "kampb.jpg", "bg.jpg", "campmatelogo.png" };

                    foreach (var dto in dtos)
                    {
                        // Afbeelding URL correctie
                        if (!string.IsNullOrEmpty(dto.Image)
                            && !dto.Image.StartsWith("http")
                            && !localImages.Contains(dto.Image))
                        {
                            dto.Image = $"http://10.0.2.2:5225/images/{dto.Image}";
                        }

                        // Favoriet status instellen
                        // eerst op false zetten om oude favorieten te wissen daarna controlleren van huide gebruiker is dat die favoriet is
                        dto.IsFavorite = false;
                        if (favoriteNames.Contains(dto.Name))
                        {
                            dto.IsFavorite = true;
                        }

                        var campModel = new CampSite(dto);
                        _allCamps.Add(campModel);
                        CampList.Add(campModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("API Fout: " + ex.Message);
                await Shell.Current.DisplayAlert("Fout", "Er is een probleem opgetreden tijdens het laden van de gegevens.", "OK");
            }
        }

        // --- COMMANDS ---

        [RelayCommand]
        async Task FilterPageAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new FilterPage(_allCamps));
        }

        [RelayCommand]
        async Task AddNewCampAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddCampPage));
        }

        [RelayCommand]
        async Task GoToDetailsAsync(CampSite selectedCamp)
        {
            if (selectedCamp == null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Camp", selectedCamp.Data }
            };

            await Shell.Current.GoToAsync(nameof(CampDetailPage), navigationParameter);
        }

        [RelayCommand]
        void ToggleFavorite(CampSite camp)
        {
            if (camp == null) return;

            camp.IsFavorite = !camp.IsFavorite;
            SaveFavoritesToStorage(_allCamps);
        }

        // --- HULPMETHODEN ---

        private void FilterCamps(string query)
        {
            CampList.Clear();
            var search = query?.ToLower() ?? "";

            var filtered = _allCamps.Where(c => c.Name.ToLower().Contains(search) ||
                                                c.Location.ToLower().Contains(search));

            foreach (var item in filtered) CampList.Add(item);
        }

        private void ApplyFilter(FilterOptions filters)
        {
            CampList.Clear();
            var filtered = _allCamps.AsEnumerable();

            if (filters.Location != null)
                filtered = filtered.Where(c => c.Location == filters.Location);

            if (filters.Facilities != null)
                filtered = filtered.Where(c => filters.Facilities.All(f => c.Facilities.Contains(f)));

            foreach (var item in filtered) CampList.Add(item);
        }

        private void AddNewCampToList(CampSiteDto newDto)
        {
            var localImages = new List<string> { "kampa.jpg", "kampb.jpg", "bg.jpg", "campmatelogo.png" };
            if (!string.IsNullOrEmpty(newDto.Image) && !newDto.Image.StartsWith("http") && !localImages.Contains(newDto.Image))
            {
                newDto.Image = $"http://10.0.2.2:5225/images/{newDto.Image}";
            }

            var campModel = new CampSite(newDto);
            _allCamps.Add(campModel);

            // Voeg toe aan de UI-lijst als het overeenkomt met de zoekopdracht
            if (campModel.Name.ToLower().Contains(SearchText?.ToLower() ?? "") ||
                campModel.Location.ToLower().Contains(SearchText?.ToLower() ?? ""))
            {
                CampList.Add(campModel);
            }
        }

        private void RemoveCampFromList(int deletedId)
        {
            var itemToRemove = CampList.FirstOrDefault(c => c.Data.Id == deletedId);
            if (itemToRemove != null) CampList.Remove(itemToRemove);

            var itemInAll = _allCamps.FirstOrDefault(c => c.Data.Id == deletedId);
            if (itemInAll != null) _allCamps.Remove(itemInAll);
        }

        private void UpdateCampInList(CampSiteDto updatedDto)
        {
            var item = _allCamps.FirstOrDefault(c => c.Data.Id == updatedDto.Id);
            if (item != null)
            {
                item.Data.TotalStars = updatedDto.TotalStars;
                item.Data.RatingCount = updatedDto.RatingCount;

                // UI verversen
                int index = CampList.IndexOf(item);
                if (index >= 0) CampList[index] = item;
            }
        }

        // --- FAVORIETEN BEHEREN (SecureStorage) ---

        private async Task<List<CampSiteDto>> LoadFavoritesFromStorageAsync()
        {
            try
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null || string.IsNullOrEmpty(currentUser.Email)) return new List<CampSiteDto>();

                string key = $"Favorites_{currentUser.Email}";
                string json = await SecureStorage.GetAsync(key);

                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<List<CampSiteDto>>(json) ?? new List<CampSiteDto>();
                }
            }
            catch { }
            return new List<CampSiteDto>();
        }

        

        // Deze methode wordt aangeroepen vanuit HomePage.xaml.cs (OnAppearing)
        public async Task RefreshFavoritesAsync()
        {
            // 1. Haal de actuele favorietenlijst op van de opslag (voor de huidige gebruiker)
            var updatedFavorites = await LoadFavoritesFromStorageAsync();
            var favoriteNames = updatedFavorites.Select(f => f.Name).ToHashSet();

            // 2. Loop door de volledige lijst (_allCamps) en reset de status. BELANGRIJK: We zetten IsFavorite op true OF false.   Dit zorgt ervoor dat oude favorieten van de vorige gebruiker worden gewist.
            
            foreach (var camp in _allCamps)
            {
                camp.IsFavorite = favoriteNames.Contains(camp.Name);
            }

            // 3. Update ook de zichtbare lijst (CampList)
            foreach (var camp in CampList)
            {
                camp.IsFavorite = favoriteNames.Contains(camp.Name);
            }
        }


        private async void SaveFavoritesToStorage(List<CampSite> camps)
        {
            try
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                var favoriteCamps = camps.Where(c => c.IsFavorite).ToList();
                var dtosToSave = favoriteCamps.Select(c => c.Data).ToList();

                string key = $"Favorites_{currentUser.Email}";
                string json = JsonSerializer.Serialize(dtosToSave);
                await SecureStorage.SetAsync(key, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Opslaan favorieten mislukt: " + ex.Message);
            }
        }
    }
}