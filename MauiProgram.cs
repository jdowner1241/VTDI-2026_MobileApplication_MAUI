using Microsoft.Extensions.Logging;

namespace Mystic_ToDo_MAUI_
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
                    fonts.AddFont("Vampire-Wars.ttf", "VampireWars");
                    fonts.AddFont("Vampire-Wars-Italic.ttf", "VampireWarsItalic");
                });

            // Services
            builder.Services.AddSingleton<Services.ThemeHelpers.ThemeSwitcher>();
            builder.Services.AddSingleton<Services.db.DBInitializer>();
            builder.Services.AddSingleton<Services.AppInitializer>();

            // ViewModels
            builder.Services.AddSingleton<ViewModel.HomeViewModel>();

            //  Repositories

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
