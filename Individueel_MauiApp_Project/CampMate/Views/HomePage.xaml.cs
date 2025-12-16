using CampMate.Messages;
using CampMate.Models;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace CampMate.Views;

public partial class HomePage : ContentPage
{
    private ObservableCollection<CampSite> CampList { get; set; }
    private List<CampSite> AllCamps { get; set; }

    public HomePage()
    {
        InitializeComponent();

        // Load favorite camps from storage
        var favorites = LoadFavoritesFromStorage();

        WeakReferenceMessenger.Default.Register<FavoritesUpdatedMessage>(this, (r, m) =>
        {
            ((HomePage)r).UpdateCampListFavorites();
        });

        AllCamps = new List<CampSite>
        {
            new CampSite { Name="Camp A", Location="Antalya", Stars=4, Facilities="zwembad,fietsverhuur,Elektrisch", Price=50, Image="kampa.jpg", IsFavorite = favorites.Any(f => f.Name == "Camp A") },
            new CampSite { Name="Camp B", Location="Bodrum", Stars=3, Facilities="Elektrisch", Price=30, Image="kampb.jpg" },
            new CampSite { Name="Camp C", Location="Istanbul", Stars=4, Facilities="cafe,fietsverhuur", Price=50, Image="bg.jpg" },
            new CampSite { Name="Camp D", Location="Antalya", Stars=3, Facilities="Elektrisch", Price=30, Image="dotnet_bot.png" }
        };

        
        CampList = new ObservableCollection<CampSite>(AllCamps);
        CampListView.ItemsSource = CampList;

        

        // WeakReferenceMessenger luister de message van FilterPage!!!
        WeakReferenceMessenger.Default.Register<FilterCriteriaMessage>(this, (r, m) =>
        {
            ApplyFilter(m.Value);
        });


        WeakReferenceMessenger.Default.Register<NewCampMessage>(this, (r, m) =>
        {
            AllCamps.Add(m.Value);        
            CampList.Add(m.Value);        
        });

    }

    //  Deze methode voert de synchronisatie uit
    private async void UpdateCampListFavorites()
    {
        // 1. Haal de nieuwe, huidige favorietenlijst op uit SecureStorage
        var updatedFavorites = await LoadFavoritesFromStorageForHomePage();
        var favoriteNames = updatedFavorites.Select(f => f.Name).ToHashSet();

        
        foreach (var camp in CampList)
        {
            // Pas de IsFavorite property aan
            camp.IsFavorite = favoriteNames.Contains(camp.Name);
        }

        CampListView.ItemsSource = null;
        CampListView.ItemsSource = CampList;
    }

   
    private async Task<List<CampSite>> LoadFavoritesFromStorageForHomePage()
    {
        try
        {
            string? json = await SecureStorage.GetAsync("FavoriteCamps");
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<List<CampSite>>(json) ?? new List<CampSite>();
            }
        }
        catch { }
        return new List<CampSite>();
    }

    private void ApplyFilter(FilterOptions filters)
    {
        
        CampList.Clear();

      
        var filtered = AllCamps.AsEnumerable();

        if (filters.Location is not null)
            filtered = filtered.Where(c => c.Location == filters.Location);

        if (filters.Facilities is not null)
            filtered = filtered.Where(c => filters.Facilities.All(f => c.Facilities.Contains(f)));

        // toevgen in ObservableCollection  en  UI gaat automatisch upgedated
        foreach (var item in filtered)
            CampList.Add(item);
    }

    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilterPage(AllCamps));
    }

    private async void AddNewCampButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddCampPage));
    }

    private void FavoriteButton_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is CampSite camp)
        {
           
            camp.IsFavorite = !camp.IsFavorite;

           
            var index = CampList.IndexOf(camp);
            CampList.RemoveAt(index);
            CampList.Insert(index, camp);

            
            SaveFavoritesToStorage(AllCamps);
        }
    }
    private void SaveFavoritesToStorage(List<CampSite> camps)
    {
        
        var favoriteCamps = camps.Where(c => c.IsFavorite).ToList();

        
        string json = JsonSerializer.Serialize(favoriteCamps);

        
        SecureStorage.SetAsync("FavoriteCamps", json);
    }

    
    private List<CampSite> LoadFavoritesFromStorage()
    {
        try
        {
            string? json = SecureStorage.GetAsync("FavoriteCamps").Result;
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<List<CampSite>>(json) ?? new List<CampSite>();
            }
        }
        catch {  }

        return new List<CampSite>();
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower() ?? "";

       
        CampList.Clear();

       
        var filtered = AllCamps
            .Where(c =>
                c.Name.ToLower().Contains(searchText) ||
                c.Location.ToLower().Contains(searchText)
            );

        
        foreach (var camp in filtered)
            CampList.Add(camp);
    }

}
