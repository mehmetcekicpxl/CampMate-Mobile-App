using CampMate.Messages;
using CampMate.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace CampMate.Views;

public partial class HomePage : ContentPage
{
    private ObservableCollection<CampSite> CampList { get; set; }
    private List<CampSite> AllCamps { get; set; }

    public HomePage()
    {
        InitializeComponent();

        AllCamps = new List<CampSite>
        {
            new CampSite { Name="Camp A", Location="Antalya", Stars=4, Facilities="zwembad,fietsverhuur,Elektrisch", Price=50, Image="kampa.jpg" },
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
    }

    private void ApplyFilter(FilterOptions filters)
    {
        
        CampList.Clear();

       // neem filtered list
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
        await Navigation.PushAsync(new FilterPage());
    }

    private void AddNewCampButton_Clicked(object sender, EventArgs e)
    {

    }
}
