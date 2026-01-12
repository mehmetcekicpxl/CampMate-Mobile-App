using CampMate.Services;
using CampMate.Shared.DTOs;
using CampMate.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace CampMate.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        // Formuliervelden
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _phoneNumber;

        [ObservableProperty]
        private string _location;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        async Task RegisterAsync()
        {
            // Validaties
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("FOUT", "Vul ten minste de velden voor e-mailadres en wachtwoord in.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("FOUT", "De wachtwoorden komen niet overeen!", "OK");
                return;
            }

            // DTO aanmaken
            var registerDto = new RegisterDto
            {
                Name = Name,
                Email = Email,
                Password = Password,
                PhoneNumber = PhoneNumber,
                Location = Location
            };

            // API aanroep
            bool isSuccess = await _authService.RegisterAsync(registerDto);

            if (isSuccess)
            {
                await Shell.Current.DisplayAlert("Succesvol", $"Welkom {registerDto.Name}! Uw registratie is voltooid.", "OK");

                // Terug naar inlogpagina
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Fout", "Registratie mislukt. Probeer een ander e-mailadres.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}