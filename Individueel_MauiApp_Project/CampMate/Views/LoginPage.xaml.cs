
using CampMate.ViewModels;

namespace CampMate.Views;

public partial class LoginPage : ContentPage
{
    // ViewModel injectie
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}