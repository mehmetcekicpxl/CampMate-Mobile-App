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



    




    



    


    // === 2. Beveiliging (Güvenlik) İşlevleri ===



    // Wachtwoord wijzigen butonuna tıklandığında formun gösterilmesi/gizlenmesi

    private void ChangePassword_Clicked(object sender, EventArgs e)

    {

        isPasswordFormVisible = !isPasswordFormVisible;

        PasswordChangeForm.IsVisible = isPasswordFormVisible;

    }



    // Şifre Kaydetme

    private async void SavePassword_Clicked(object sender, EventArgs e)

    {

        // Gerçek şifre değiştirme mantığı buraya gelir (API çağrısı vs.)

        // Şifre Giriş Alanlarına erişmek için XAML'de x:Name tanımlamaları gereklidir

        var currentPassword = CurrentPasswordEntry.Text;

        var newPassword = NewPasswordEntry.Text;

        var confirmPassword = ConfirmNewPasswordEntry.Text;

        // 1. Mevcut Şifre Kontrolü

        if (currentPassword != LoginPage.TEST_PASSWORD)

        {

            await DisplayAlert("Fout", "Huidig wachtwoord is onjuist.", "OK");

            return;

        }



        // 2. Yeni Şifre ve Onay Kontrolü

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)

        {

            await DisplayAlert("Fout", "Nieuw wachtwoord en bevestiging komen niet overeen of zijn leeg.", "OK");

            return;

        }



        // 3. Şifreyi Güncelleme

        LoginPage.TEST_PASSWORD = newPassword;



        // Başarılı olursa formu gizle ve bildirim göster

        PasswordChangeForm.IsVisible = false;

        isPasswordFormVisible = false;



        // Giriş alanlarını temizle (Kullanıcı deneyimi için önemli)

        CurrentPasswordEntry.Text = string.Empty;

        NewPasswordEntry.Text = string.Empty;

        ConfirmNewPasswordEntry.Text = string.Empty;



        await DisplayAlert("Succes", "Je wachtwoord is succesvol gewijzigd.", "OK");

    }









    // Şifre Değiştirme İptali

    private void CancelPasswordChange_Clicked(object sender, EventArgs e)

    {

        PasswordChangeForm.IsVisible = false;

        isPasswordFormVisible = false;

    }



    // === 3. Uitloggen (Çıkış Yap) İşlevi ===



    // Uitloggen butonuna tıklandığında

    private async void LogoutButton_Clicked(object sender, EventArgs e)

    {

        // 1. Kullanıcıyı uyarma

        bool confirm = await DisplayAlert("Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");



        if (confirm)

        {

            // 2. Oturumu sonlandırma (Token silme, kullanıcı verilerini temizleme vb.)

            // Gerçek dünyada burada SecureStorage.Remove("auth_token") gibi işlemler yapılır.



            // 3. Login sayfasına yönlendirme (App.Current.MainPage'i değiştirme)

            var loginPage = new LoginPage();



            // Login sayfasına, çıkış yapıldığına dair bir bilgi taşıyıcı göndermek gereklidir.

            // Bu örnekte, 'Je bent uitgelogd' yazısını göstermesi için LoginPage sınıfında bir mantık olmalı.



            Application.Current.MainPage = new NavigationPage(loginPage);



            // (Opsiyonel) Çıkış bildirimini göstermek için LoginPage'de bir metot çağrılabilir

            // Örneğin: ((LoginPage)loginPage).ShowLogoutMessage(); 



            // VEYA: LoginPage yapısı basitleştirilmiş bir şekilde şu şekilde çağrılabilir (LoginPage'in buna uygun olması gerekir):

            await ((LoginPage)Application.Current.MainPage).DisplayLogoutMessage();

        }

    }

}