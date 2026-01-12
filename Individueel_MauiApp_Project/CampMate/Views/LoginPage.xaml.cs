//using CampMate.Services; 

//namespace CampMate.Views;

//public partial class LoginPage : ContentPage
//{
//    private readonly AuthService _authService;


//    public LoginPage(AuthService authService)
//    {
//        InitializeComponent();
//        _authService = authService;
//        this.BindingContext = this;
//        LogoutMessage.IsVisible = false;
//        UserEmail = "mehmet@campmate.com";
//        UserPassword = "123";
//    }


//    public string UserEmail { get; set; }
//    public string UserPassword { get; set; }

//    private async void OnLoginClicked(object sender, EventArgs e)
//    {
//        if (string.IsNullOrWhiteSpace(UserEmail) || string.IsNullOrWhiteSpace(UserPassword))
//        {
//            await DisplayAlert("Fout", "Voer uw e-mailadres en wachtwoord in.", "OK");
//            return;
//        }

//        // We vragen het aan de API
//        var user = await _authService.LoginAsync(UserEmail, UserPassword);

//        if (user != null)
//        {


//            AppShell.LoadMainContent();
//        }
//        else
//        {
//            await DisplayAlert("Inloggen mislukt", "E-mailadres of wachtwoord onjuist.", "OK");
//        }
//    }

//    private async void OnRegisterClicked(object sender, EventArgs e)
//    {
//        await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
//    }

//    public async Task DisplayLogoutMessage()
//    {
//        LogoutMessage.IsVisible = true;
//        await Task.Delay(3000);
//        LogoutMessage.IsVisible = false;
//    }
//}


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