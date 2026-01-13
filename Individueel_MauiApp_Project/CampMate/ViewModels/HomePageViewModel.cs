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

        // Geeft aan of er een filter is toegepast
        public bool IsFilterApplied = false;

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
            WeakReferenceMessenger.Default.Register<FavoritesUpdatedMessage>(this, (r, m) =>
            {
                LoadCampsFromApi();
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

        // Haal campings op van de API
        private async void LoadCampsFromApi()
        {
            try
            {
                // 1. Haal de ID van de ingelogde gebruiker op
                int userId = Preferences.Get("UserId", 0);

                // 2. Voeg userId toe aan de URL: "campsites?userId=5"
                var dtos = await _httpClient.GetFromJsonAsync<List<CampSiteDto>>($"campsites?userId={userId}");

                if (dtos != null)
                {
                    _allCamps.Clear();
                    CampList.Clear();
                    var localImages = new List<string> { "kampa.jpg", "kampb.jpg", "bg.jpg", "campmatelogo.png" };

                    foreach (var dto in dtos)
                    {
                        // Afbeelding URL correctie voor Android Emulator
                        if (!string.IsNullOrEmpty(dto.Image) && !dto.Image.StartsWith("http") && !localImages.Contains(dto.Image))
                        {
                            dto.Image = $"http://10.0.2.2:5225/images/{dto.Image}";
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
            }
        }

        [RelayCommand]
        public async Task LoadCamps()
        {
            
            IsFilterApplied = false;
           
            LoadCampsFromApi();
            await Task.CompletedTask;
        }

        // Favoriet toevoegen of verwijderen
        [RelayCommand]
        async Task ToggleFavorite(CampSite camp)
        {
            if (camp == null) return;

            // Directe UI feedback
            camp.IsFavorite = !camp.IsFavorite;

            try
            {
                int userId = Preferences.Get("UserId", 0);

                // Geen ingewikkelde headers meer, gewoon userId in de URL
                var response = await _httpClient.PostAsync($"campsites/toggle-favorite/{camp.Data.Id}?userId={userId}", null);

                if (!response.IsSuccessStatusCode)
                {
                    camp.IsFavorite = !camp.IsFavorite; 
                }
            }
            catch
            {
                camp.IsFavorite = !camp.IsFavorite; 
            }
        }
        

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
            // Filter toegepast
            IsFilterApplied = true;

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

        
    }
}