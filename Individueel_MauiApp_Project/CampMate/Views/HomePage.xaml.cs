using CampMate.Messages;
using CampMate.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using System.Collections.ObjectModel;
using System.Text.Json;
using static CampMate.Models.CampSite;

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
            new CampSite { Name="City Camp Valkenburg", Location="Valkenburg,Nederlands", Facilities="zwembad,fietsverhuur,Elektrisch", Price=50, Image="kampa.jpg", IsFavorite = favorites.Any(f => f.Name == "Camp A") },
            new CampSite { Name="Camping Jungfrau", Location="Lauterbrunnen, Zwitserland", Facilities="Elektrisch", Price=30, Image="kampb.jpg" },
            new CampSite { Name="Camping Village Roma Capitol", Location="Rome, Italië", Facilities="cafe,fietsverhuur", Price=50, Image="bg.jpg" },
            new CampSite { Name="Camping De La Seine", Location="Jumièges, Frankrijk", Facilities="Elektrisch", Price=30, Image="kampa.jpg" }
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

  
    private void OnCustomScrollChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = sender as Slider;
        // Laten we CollectionView vinden via Grid waaraan Slider is gekoppeld.
        var grid = slider.Parent as Grid;
        var border = grid?.Children[0] as Border;
        var collectionView = border?.Content as CollectionView;
        var camp = collectionView?.BindingContext as CampSite;

        if (camp != null && camp.Comments.Count > 0)
        {
            // We berekenen op basis van de sliderwaarde (tussen 0 en 1) welke interpretatie we zullen gebruiken.
            int targetIndex = (int)(e.NewValue * (camp.Comments.Count - 1));
            var targetComment = camp.Comments[targetIndex];

            collectionView.ScrollTo(targetComment, position: ScrollToPosition.Start, animate: false);
        }
    }

    

    private void SubmitReview_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var camp = button?.CommandParameter as CampSite;
        if (button == null || camp == null) return;

        if (button.Parent is Grid grid)
        {
            var picker = ((Border)grid.Children[0]).Content as Picker;
            var entry = ((Border)grid.Children[1]).Content as Entry;

            bool hasRated = picker?.SelectedIndex != -1;
            bool hasCommented = !string.IsNullOrWhiteSpace(entry?.Text);

            if (!hasRated && !hasCommented) return;

         
            if (hasRated)
            {
                double rating = double.Parse(picker.SelectedItem.ToString());
                camp.TotalStars += rating;
                camp.RatingCount++;
                camp.RefreshUI(); // Gemiddelde score wordt bijgewerkt in UI
                picker.SelectedIndex = -1;
            }

            // 2. Commentaarverwerking en automatisch scrollen
            if (hasCommented)
            {
                var newComment = new Comment { UserName = "Mehmet", Text = entry.Text };
                camp.Comments.Add(newComment);
                entry.Text = string.Empty;

               
                if (grid.Parent is VerticalStackLayout parent)
                {
                    var commentGrid = parent.Children.OfType<Grid>().FirstOrDefault(g => g.HeightRequest == 150);
                    var border = commentGrid?.Children.OfType<Border>().FirstOrDefault();
                    var collectionView = border?.Content as CollectionView;

                    if (collectionView != null)
                    {
                        MainScrollView.InputTransparent = true; // Stop het scrollen
                        Dispatcher.Dispatch(async () => {
                            await Task.Delay(100);
                            collectionView.ScrollTo(newComment, position: ScrollToPosition.End, animate: true);
                            await Task.Delay(500);
                            MainScrollView.InputTransparent = false; // Terug openen
                        });
                    }
                }
            }
        }
    }
}
