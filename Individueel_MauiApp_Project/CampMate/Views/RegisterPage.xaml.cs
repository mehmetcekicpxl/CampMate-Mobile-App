namespace CampMate.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterCompleted(object sender, EventArgs e)
    {
        await DisplayAlert("Succesvol", "Uw registratie is voltooid, log in.", "OK");
        
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}