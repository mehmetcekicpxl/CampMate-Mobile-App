namespace CampMate.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterCompleted(object sender, EventArgs e)
    {
        
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("FOUT", "Vul ten minste de velden voor e-mailadres en wachtwoord in.", "OK");
            return;
        }

      
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("FOUT", "De wachtwoorden komen niet overeen!", "OK");
            return;
        }

       
        string name = NameEntry.Text;
        string email = EmailEntry.Text;
        string phone = PhoneEntry.Text;
        string location = LocationEntry.Text;

       
        await DisplayAlert("Succesvol", $"Welkom {name}! Uw registratie is voltooid.", "OK");

        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}