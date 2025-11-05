namespace CampMate.Views;

public partial class LoginPage : ContentPage
{
    // Test gebruiker
    private const string TEST_EMAIL = "test@campmate.com";
    private const string TEST_PASSWORD = "password123";

    
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }

    public LoginPage()
    {
        InitializeComponent();
        this.BindingContext = this;
    }

    private void OnLoginClicked(object sender, EventArgs e)
    {
        
        if (!string.IsNullOrWhiteSpace(UserEmail) &&
            !string.IsNullOrWhiteSpace(UserPassword) &&
            UserEmail == TEST_EMAIL &&
            UserPassword == TEST_PASSWORD)
        {
           
            AppShell.LoadMainContent();
        }
        else
        {
            
            DisplayAlert("inloggen mislukt",
                         $"je heeft ingevoerd: {UserEmail ?? "NULL"} / {UserPassword ?? "NULL"}\n" +
                         $"verwacht: {TEST_EMAIL} / {TEST_PASSWORD}",
                         "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
    }
}