using CampMate.Services;
using CampMate.Views; 

namespace CampMate.Views;

public partial class SettingsPage : ContentPage
{
    private double currentFontSize = 16;
    private bool isPasswordFormVisible = false;

    
    private readonly AuthService _authService;

    
    public SettingsPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = AppTheme.Light;
        }
    }

    private void ChangePassword_Clicked(object sender, EventArgs e)
    {
        isPasswordFormVisible = !isPasswordFormVisible;
        PasswordChangeForm.IsVisible = isPasswordFormVisible;
    }

    private async void SavePassword_Clicked(object sender, EventArgs e)
    {
        var currentPassword = CurrentPasswordEntry.Text;
        var newPassword = NewPasswordEntry.Text;
        var confirmPassword = ConfirmNewPasswordEntry.Text;

        // Validaties
        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            await DisplayAlert("Fout", "Voer uw huidige wachtwoord in.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
        {
            await DisplayAlert("Fout", "Nieuw wachtwoord komt niet overeen.", "OK");
            return;
        }

       
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            await DisplayAlert("Fout", "Gebruiker niet ingelogd.", "OK");
            return;
        }

        // API-OPROEP: Wachtwoord wijzigen op de server
        bool success = await _authService.ChangePasswordAsync(currentUser.Email, currentPassword, newPassword);

        if (!success)
        {
            await DisplayAlert("Fout", "Huidig wachtwoord is onjuist of er is een fout opgetreden.", "OK");
            return;
        }

        
        PasswordChangeForm.IsVisible = false;
        isPasswordFormVisible = false;

        CurrentPasswordEntry.Text = string.Empty;
        NewPasswordEntry.Text = string.Empty;
        ConfirmNewPasswordEntry.Text = string.Empty;

        await DisplayAlert("Succes", "Je wachtwoord is succesvol gewijzigd.", "OK");
    }

    private void CancelPasswordChange_Clicked(object sender, EventArgs e)
    {
        PasswordChangeForm.IsVisible = false;
        isPasswordFormVisible = false;
    }

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");

        if (!confirm)
            return;

       
        _authService.Logout();

      
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

   
    }

    private async void GoToFavorites_Clicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new FavoritesPage(_authService));
    }
}