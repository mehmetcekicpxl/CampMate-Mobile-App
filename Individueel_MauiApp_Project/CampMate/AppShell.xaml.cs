

using CampMate.Views;

namespace CampMate
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            
            Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
            Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
        }

        // als login succesvol is
        public static void LoadMainContent()
        {
            
            var shell = Shell.Current as AppShell;
            if (shell == null) return;

           
            shell.HomeFlyout.IsVisible = true;
            shell.ProfileFlyout.IsVisible = true;
            shell.SettingsFlyout.IsVisible = true;

            
            Shell.SetFlyoutBehavior(shell, FlyoutBehavior.Flyout);

            
            Shell.Current.GoToAsync("//HomeRoute/Home");
        }
    }
}
