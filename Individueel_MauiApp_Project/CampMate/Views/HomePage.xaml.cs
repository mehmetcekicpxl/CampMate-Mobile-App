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
            var campModel = new CampSite(m.Value);

            // We zetten r om naar het type (HomePage).
            if (r is HomePage page)
            {
                page.AllCamps.Add(campModel);
                page.CampList.Add(campModel);
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

                foreach (var dto in dtos)
                {
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



    private async void SubmitReview_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;

        // Als er geen knop of kampgegevens zijn, voer dan geen bewerking uit.
        if (button == null || button.CommandParameter is not CampSite camp)
            return;

        if (button.Parent is Grid grid)
        {
            
            var pickerBorder = grid.Children.FirstOrDefault(c => c is Border b && b.Content is Picker) as Border;
            var entryBorder = grid.Children.FirstOrDefault(c => c is Border b && b.Content is Entry) as Border;

            var picker = pickerBorder?.Content as Picker;
            var entry = entryBorder?.Content as Entry;

            bool hasRated = picker?.SelectedIndex != -1;
            bool hasCommented = !string.IsNullOrWhiteSpace(entry?.Text);

            
            if (!hasRated && !hasCommented) return;

           
            var userDto = await _authService.GetCurrentUserAsync();

           

            string currentUserName = userDto.Name;
           

       
            if (hasRated && picker.SelectedItem != null)
            {
                if (double.TryParse(picker.SelectedItem.ToString(), out double rating))
                {
                    camp.TotalStars += rating;
                    camp.RatingCount++;
                    camp.RefreshUI();
                }
            }


            if (hasCommented)
            {
                // 1. EERST API AANROEPEN (Database update)
                
                bool success = await _authService.AddCommentAsync(camp.Data.Id, currentUserName, entry.Text);

                if (success)
                {
                    // 2. Als API succesvol is, voeg dan toe aan het scherm (UI)
                    camp.Comments.Add(new CampSite.Comment
                    {
                        UserName = currentUserName,
                        Text = entry.Text
                    });

                    entry.Text = string.Empty; 
                    await DisplayAlert("Bedankt", "Je reactie is geplaatst!", "OK");
                }
                else
                {
                    await DisplayAlert("Fout", "Kan reactie niet opslaan. Controleer je verbinding.", "OK");
                }
            }
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

    // Scroll en commentaar 
    private void OnCustomScrollChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = sender as Slider;
        var grid = slider?.Parent as Grid;
        var border = grid?.Children[0] as Border;
        var collectionView = border?.Content as CollectionView;

        
        var camp = collectionView?.BindingContext as CampSite;

        if (camp != null && camp.Comments.Count > 0)
        {
            int targetIndex = (int)(e.NewValue * (camp.Comments.Count - 1));
            var targetComment = camp.Comments[targetIndex];
            collectionView.ScrollTo(targetComment, position: ScrollToPosition.Start, animate: false);
        }
    }

   
    
}