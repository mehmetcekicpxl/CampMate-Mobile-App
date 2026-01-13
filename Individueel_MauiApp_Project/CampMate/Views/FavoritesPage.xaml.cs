
using CampMate.ViewModels;

namespace CampMate.Views;

public partial class FavoritesPage : ContentPage
{
    public FavoritesPage(FavoritesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Roep de methode in ViewModel aan om de lijst bij elke paginaweergave te vernieuwen.
        if (BindingContext is FavoritesViewModel vm)
        {
            await vm.LoadFavoritesAsync();
        }
    }
}