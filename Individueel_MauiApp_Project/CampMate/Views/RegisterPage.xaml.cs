//using CampMate.Services;
//using CampMate.Shared.DTOs;

//namespace CampMate.Views;

//public partial class RegisterPage : ContentPage
//{
//    private readonly AuthService _authService;

//    public RegisterPage(AuthService authService)
//    {
//        InitializeComponent();
//        _authService = authService;
//    }

//    private async void OnRegisterCompleted(object sender, EventArgs e)
//    {
//        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
//        {
//            await DisplayAlert("FOUT", "Vul ten minste de velden voor e-mailadres en wachtwoord in.", "OK");
//            return;
//        }

//        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
//        {
//            await DisplayAlert("FOUT", "De wachtwoorden komen niet overeen!", "OK");
//            return;
//        }

//        // DTO aanmaken
//        var registerDto = new RegisterDto
//        {
//            Name = NameEntry.Text,
//            Email = EmailEntry.Text,
//            Password = PasswordEntry.Text,
//            PhoneNumber = PhoneEntry.Text,
//            Location = LocationEntry.Text
//        };

//        // Verstuur naar API
//        bool isSuccess = await _authService.RegisterAsync(registerDto);

//        if (isSuccess)
//        {
//            await DisplayAlert("Succesvol", $"Welkom {registerDto.Name}! Uw registratie is voltooid.", "OK");
//            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
//        }
//        else
//        {
//            await DisplayAlert("Fout", "Registratie mislukt. Probeer een ander e-mailadres.", "OK");
//        }
//    }

//    private async void OnBackToLoginClicked(object sender, EventArgs e)
//    {
//        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
//    }
//}


using CampMate.ViewModels;

namespace CampMate.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}