using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.db;


namespace Mystic_ToDo_MAUI_
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("VampireWars.ttf", "VampireWars");
                    fonts.AddFont("VampireWarsItalic.ttf", "VampireWarsItalic");
                });

            // Services
            builder.Services.AddSingleton<Services.ThemeHelpers.ThemeSwitcher>();
            builder.Services.AddSingleton<Services.db.DBInitializer>();
            builder.Services.AddSingleton<Services.AppInitializer>();
            builder.Services.AddSingleton<Services.Alarm.TaskRepo>();
            builder.Services.AddSingleton<Services.Alarm.AlarmTracker>();
            builder.Services.AddSingleton<IDispatcherTimer>(sp =>
            {
                var timer = Dispatcher.GetForCurrentThread().CreateTimer();
                return timer;
            });

            // DB
            builder.Services.AddSingleton<DBManager<GroupList>>();
            builder.Services.AddSingleton<DBManager<TaskList>>();
            builder.Services.AddSingleton<DBManager<TaskList_RepeatTag>>();
            builder.Services.AddSingleton<DBManager<TaskList_RepeatList>>();
            builder.Services.AddSingleton<DBManager<Attachments>>();

            // ViewModels
            builder.Services.AddTransient<ViewModel.HomeViewModel>();
            builder.Services.AddTransient<ViewModel.HomeVM.EditorVM>();
            builder.Services.AddTransient<ViewModel.HomeVM.TaskListVM>();
            builder.Services.AddTransient<ViewModel.HomeVM.GroupListViewModel>();


            //  Repositories

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
