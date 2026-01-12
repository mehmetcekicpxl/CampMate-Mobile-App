using CampMate.Services;
using CampMate.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampMate.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        // Wachtwoord formulier zichtbaarheid
        [ObservableProperty]
        private bool _isPasswordFormVisible;

        // Wachtwoord velden
        [ObservableProperty]
        private string _currentPassword;

        [ObservableProperty]
        private string _newPassword;

        [ObservableProperty]
        private string _confirmNewPassword;

        // Dark Mode status
        [ObservableProperty]
        private bool _isDarkMode;

        public SettingsViewModel(AuthService authService)
        {
            _authService = authService;

            // Huidige thema status ophalen
            IsDarkMode = Application.Current?.UserAppTheme == AppTheme.Dark;
        }

        // --- DARK MODE COMMAND ---
        // Switch toggled event yerine bu command'i kullanabiliriz veya PropertyChanged
        partial void OnIsDarkModeChanged(bool value)
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
            }
        }

        // --- COMMANDS ---

        [RelayCommand]
        void TogglePasswordForm()
        {
            IsPasswordFormVisible = !IsPasswordFormVisible;
        }

        [RelayCommand]
        void CancelPasswordChange()
        {
            IsPasswordFormVisible = false;
            ClearPasswordFields();
        }

        [RelayCommand]
        async Task SavePasswordAsync()
        {
            // Validaties
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                await Shell.Current.DisplayAlert("Fout", "Voer uw huidige wachtwoord in.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmNewPassword)
            {
                await Shell.Current.DisplayAlert("Fout", "Nieuw wachtwoord komt niet overeen.", "OK");
                return;
            }

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                await Shell.Current.DisplayAlert("Fout", "Gebruiker niet ingelogd.", "OK");
                return;
            }

            // API aanroep
            bool success = await _authService.ChangePasswordAsync(currentUser.Email, CurrentPassword, NewPassword);

            if (success)
            {
                await Shell.Current.DisplayAlert("Succes", "Je wachtwoord is succesvol gewijzigd.", "OK");
                IsPasswordFormVisible = false;
                ClearPasswordFields();
            }
            else
            {
                await Shell.Current.DisplayAlert("Fout", "Huidig wachtwoord is onjuist of er is een fout opgetreden.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToFavoritesAsync()
        {
            // Navigeren naar Favorieten pagina
            var viewModel = new FavoritesViewModel(_authService);
            await Application.Current.MainPage.Navigation.PushAsync(new FavoritesPage(viewModel));
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert("Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");

            if (!confirm) return;

            _authService.Logout();

            // BELANGRIJK: Logout parameter meesturen!
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}?Logout=true");
        }

        private void ClearPasswordFields()
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
        }
    }
}