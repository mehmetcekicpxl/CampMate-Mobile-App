using CampMate.Services;
using CampMate.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampMate.ViewModels
{
    // IQueryAttributable toegevoegd om 'Logout' parameter te vangen
    public partial class LoginViewModel : ObservableObject, IQueryAttributable
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _userEmail;

        [ObservableProperty]
        private string _userPassword;

        [ObservableProperty]
        private bool _isLogoutMessageVisible;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;

            // Standaardwaarden voor test
            UserEmail = "mehmet@campmate.com";
            UserPassword = "1234";
            IsLogoutMessageVisible = false;
        }

        // --- NAVIGATIE DATA ONTVANGEN ---
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // Controleer of de gebruiker is uitgelogd
            if (query.ContainsKey("Logout") && bool.TryParse(query["Logout"].ToString(), out bool isLogout) && isLogout)
            {
                ShowLogoutNotificationAsync();
            }
        }

        // --- COMMANDS ---

        [RelayCommand]
        async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(UserEmail) || string.IsNullOrWhiteSpace(UserPassword))
            {
                await Shell.Current.DisplayAlert("Fout", "Voer uw e-mailadres en wachtwoord in.", "OK");
                return;
            }

            // Inloggen via API
            var user = await _authService.LoginAsync(UserEmail, UserPassword);

            if (user != null)
            {
                // Hoofdpagina laden
                AppShell.LoadMainContent();
            }
            else
            {
                await Shell.Current.DisplayAlert("Mislukt", "E-mailadres of wachtwoord onjuist.", "OK");
            }
        }

        [RelayCommand]
        async Task RegisterAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
        }

        // Toon 'Je bent uitgelogd' bericht
        private async void ShowLogoutNotificationAsync()
        {
            IsLogoutMessageVisible = true;
            await Task.Delay(3000);
            IsLogoutMessageVisible = false;
        }
    }
}