using CampMate.Services;
using CampMate.ViewModels;
using CampMate.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System.Net.Http;


namespace CampMate
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MDI");
                });

            // HttpClient register
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                return new HttpClient()
                {
                    BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                                          ? "http://10.0.2.2:5225/api/" // Android emulator
                                          : "https://localhost:7011/api/"), // Windows/iOS
                    Timeout = TimeSpan.FromSeconds(10)
                };
            });

            builder.Services.AddSingleton(sp => new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:5225/api/")
            });


            // AuthService registreren
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<CampService>();


            //Views
            // HomePage register (DI)
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<SettingsPage>();
            
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ProfilePage>();

            builder.Services.AddTransient<CampDetailPage>();   
            builder.Services.AddTransient<CampDetailViewModel>();
            builder.Services.AddTransient<AddCampPage>();

            // ViewModels
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<AddCampViewModel>();
            builder.Services.AddTransient<FavoritesViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
