using CampMate.Shared.DTOs;
using CampMate.Messages;
using CampMate.Models; 
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using CampMate.Services;

namespace CampMate.Views;

public partial class HomePage : ContentPage
{
   
    private ObservableCollection<CampSite> CampList { get; set; }

    // Filter vs. back-up die de volledige lijst bevat
    private List<CampSite> AllCamps { get; set; }

    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public HomePage(HttpClient httpClient , AuthService authService)
    {
        InitializeComponent();
        _httpClient = httpClient;
        _authService = authService;

        
        CampList = new ObservableCollection<CampSite>();
        AllCamps = new List<CampSite>();

        // UI-verbinding
        CampListView.ItemsSource = CampList;

        // --- MESSENGERS ---
        WeakReferenceMessenger.Default.Register<FavoritesUpdatedMessage>(this, (r, m) =>
        {
            ((HomePage)r).UpdateCampListFavorites();
        });

        WeakReferenceMessenger.Default.Register<FilterCriteriaMessage>(this, (r, m) =>
        {
            ApplyFilter(m.Value);
        });

        WeakReferenceMessenger.Default.Register<NewCampMessage>(this, (r, m) =>
        {
            var newDto = m.Value;

            
            var localImages = new List<string> { "kampa.jpg", "kampb.jpg", "bg.jpg", "dotnet_bot.png" };

            if (!string.IsNullOrEmpty(newDto.Image)
                && !newDto.Image.StartsWith("http")
                && !localImages.Contains(newDto.Image))
            {
                newDto.Image = $"http://10.0.2.2:5225/images/{newDto.Image}";
            }

            var campModel = new CampSite(newDto);

            if (r is HomePage page)
            {
                page.AllCamps.Add(campModel);
                page.CampList.Add(campModel);
            }
        });



        // --- LUISTER NAAR HET VERWIJDERINGSBERICHT ---
        WeakReferenceMessenger.Default.Register<CampDeletedMessage>(this, (r, m) =>
        {
            // Verwijder de ID uit het bericht
            int deletedCampId = m.Value;

            if (r is HomePage page)
            {
                // 1. Zoek en verwijder uit de lijst op het scherm
                var itemToRemove = page.CampList.FirstOrDefault(c => c.Data.Id == deletedCampId);
                if (itemToRemove != null)
                {
                    page.CampList.Remove(itemToRemove);
                }

                
                var itemInAll = page.AllCamps.FirstOrDefault(c => c.Data.Id == deletedCampId);
                if (itemInAll != null)
                {
                    page.AllCamps.Remove(itemInAll);
                }
            }
        });

       

        

        // Haal de gegevens op uit de API
        LoadCampsFromApi();
    }

    private async void LoadCampsFromApi()
    {
        try
        {
            
            var dtos = await _httpClient.GetFromJsonAsync<List<CampSiteDto>>("campsites");

            if (dtos != null)
            {
                
                var localFavorites = await LoadFavoritesFromStorageAsync();

                // We plaatsen onze favoriete namen in een set (HashSet) voor snel zoeken.
                var favoriteNames = localFavorites.Select(f => f.Name).ToHashSet();

                AllCamps.Clear();
                CampList.Clear();


                
                var localImages = new List<string> { "kampa.jpg", "kampb.jpg", "bg.jpg", "dotnet_bot.png" };

                foreach (var dto in dtos)
                {
                    
                    if (!string.IsNullOrEmpty(dto.Image)
                        && !dto.Image.StartsWith("http")
                        && !localImages.Contains(dto.Image)) 
                    {
                        dto.Image = $"http://10.0.2.2:5225/images/{dto.Image}";
                    }
                
                    // Is de naam van het kamp in mijn favorieten opgenomen?
                    if (favoriteNames.Contains(dto.Name))
                    {
                        dto.IsFavorite = true; 
                    }

                   
                    var campModel = new CampSite(dto);

                    AllCamps.Add(campModel);
                    CampList.Add(campModel);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("API-gegevens kunnen niet worden opgehaald: " + ex.Message);
            await DisplayAlert("Fout", "Er is een probleem opgetreden tijdens het laden van de gegevens.", "OK");
        }
    }


    private async Task<List<CampSiteDto>> LoadFavoritesFromStorageAsync()
    {
        try
        {
            
            var currentUser = await _authService.GetCurrentUserAsync();

          
            if (currentUser == null || string.IsNullOrEmpty(currentUser.Email))
            {
                return new List<CampSiteDto>();
            }

            // gebruikersspecifieke sleutel 
            string key = $"Favorites_{currentUser.Email}";

            string json = await SecureStorage.GetAsync(key);

            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<List<CampSiteDto>>(json) ?? new List<CampSiteDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Favorieten kunnen niet worden geladen: " + ex.Message);
        }

        return new List<CampSiteDto>();
    }

    private async void UpdateCampListFavorites()
    {
       
        var updatedFavorites = await LoadFavoritesFromStorageAsync();

        var favoriteNames = updatedFavorites.Select(f => f.Name).ToHashSet();
        foreach (var camp in CampList)
            camp.IsFavorite = favoriteNames.Contains(camp.Name);
    }

    private void FavoriteButton_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is CampSite camp)
        {
            camp.IsFavorite = !camp.IsFavorite;
           
            SaveFavoritesToStorage(AllCamps);
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
            Console.WriteLine("Favorieten konden niet worden opgeslagen: " + ex.Message);
        }
    }



    

    // --- FILTEREN EN ZOEKEN ---
    private void ApplyFilter(FilterOptions filters)
    {
        CampList.Clear();
        var filtered = AllCamps.AsEnumerable();

        if (filters.Location != null)
            filtered = filtered.Where(c => c.Location == filters.Location); 

        if (filters.Facilities != null)
            filtered = filtered.Where(c => filters.Facilities.All(f => c.Facilities.Contains(f)));

        foreach (var item in filtered)
            CampList.Add(item);
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower() ?? "";
        CampList.Clear();

        var filtered = AllCamps
            .Where(c => c.Name.ToLower().Contains(searchText) ||
                        c.Location.ToLower().Contains(searchText));

        foreach (var camp in filtered)
            CampList.Add(camp);
    }

  
    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new FilterPage(AllCamps));
    }

    private async void AddNewCampButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddCampPage));
    }

    

    
    private async void OnCampSelected(object sender, SelectionChangedEventArgs e)
    {
        // LET OP:  de lijst bevat "CampSite"-objecten, geen "CampSiteDto"-objecten.
        // Daarom halen we ze op met 'as CampSite'.
        var selectedCampWrapper = e.CurrentSelection.FirstOrDefault() as CampSite;

        if (selectedCampWrapper == null) return;

        // Bij het verzenden naar de detailpagina verzenden we de pure gegevens (DTO) in de Wrapper.
        // Omdat CampDetailPage 'CampSiteDto' verwacht.
        var navigationParameter = new Dictionary<string, object>
        {
            { "Camp", selectedCampWrapper.Data } 
        };

        
        await Shell.Current.GoToAsync(nameof(CampDetailPage), navigationParameter);

        
        ((CollectionView)sender).SelectedItem = null;
    }



}