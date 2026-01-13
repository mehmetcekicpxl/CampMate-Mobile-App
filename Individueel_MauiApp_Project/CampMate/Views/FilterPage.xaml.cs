
using CampMate.Models;
using CampMate.ViewModels;

namespace CampMate.Views;

public partial class FilterPage : ContentPage
{
    // Constructor ontvangt de lijst van HomePage
    public FilterPage(List<CampSite> allCamps)
    {
        InitializeComponent();

      
        BindingContext = new FilterViewModel(allCamps);
    }
}
