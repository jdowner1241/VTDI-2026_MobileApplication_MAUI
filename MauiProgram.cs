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
                });

            // Services
            builder.Services.AddSingleton<Services.AppInitializer>();
            builder.Services.AddSingleton<Services.ThemeSwitcher>();
            builder.Services.AddSingleton<Services.db.DBInitializer>();
            builder.Services.AddSingleton<Services.db.SeededData>();

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
