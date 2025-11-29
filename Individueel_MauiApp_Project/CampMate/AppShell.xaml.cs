

using CampMate.Views;

namespace CampMate
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();


            //Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
            //Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
            Routing.RegisterRoute(nameof(FilterPage), typeof(Views.FilterPage));

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
