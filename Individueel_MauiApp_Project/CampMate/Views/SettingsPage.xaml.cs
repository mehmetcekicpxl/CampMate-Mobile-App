namespace CampMate.Views;



public partial class SettingsPage : ContentPage

{



    private double currentFontSize = 16;





    private bool isPasswordFormVisible = false;



    public SettingsPage()

    {

        InitializeComponent();




    }





    private void OnDarkModeToggled(object sender, ToggledEventArgs e)

    {

        if (e.Value)

        {

            // Donkere modus inschakelen

            Application.Current.UserAppTheme = AppTheme.Dark;

        }

        else

        {

            // Verlichtingsmodus inschakelen

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

        // 1. Huidige wachtwoordcontrole

        if (currentPassword != LoginPage.TEST_PASSWORD)

        {

            await DisplayAlert("Fout", "Huidig wachtwoord is onjuist.", "OK");

            return;

        }



        // 2. Nieuw wachtwoord en bevestigingscontrole

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)

        {

            await DisplayAlert("Fout", "Nieuw wachtwoord en bevestiging komen niet overeen of zijn leeg.", "OK");

            return;

        }

        LoginPage.TEST_PASSWORD = newPassword;

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



        if (confirm)

        {
            var loginPage = new LoginPage();

            Application.Current.MainPage = new NavigationPage(loginPage);

            await ((LoginPage)Application.Current.MainPage).DisplayLogoutMessage();

        }

    }

}