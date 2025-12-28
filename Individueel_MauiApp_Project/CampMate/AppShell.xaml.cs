

using CampMate.Views;

namespace CampMate
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();


            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(FilterPage), typeof(FilterPage));
            Routing.RegisterRoute(nameof(AddCampPage), typeof(AddCampPage));


        }

        // als login succesvol is
        public static void LoadMainContent()
        {
            
            var shell = Shell.Current as AppShell;
            if (shell == null) return;
          


            Shell.Current.GoToAsync("//MainTabs/HomePage");
        }
    }
}
