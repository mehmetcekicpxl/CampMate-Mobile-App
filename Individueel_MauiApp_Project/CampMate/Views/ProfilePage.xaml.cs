using CampMate.ViewModels;

namespace CampMate.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

   
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel; // Koppel ViewModel met XAML
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        
        if (BindingContext is ProfileViewModel vm)
        {
           
            await vm.LoadUserDataAsync();
        }
    }
}