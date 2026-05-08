using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Mystic_ToDo_MAUI_.Services;
using Mystic_ToDo_MAUI_.Services.Alarm;

namespace Mystic_ToDo_MAUI_
{
    public partial class AppShell : Shell
    {

        private readonly AppState _appState;
        private readonly AppInitializer _initializer;
        private readonly AlarmTracker _alarmTracker;

        public AppShell(IServiceProvider services)
        {
            InitializeComponent();

            // Resolve AppInitializer from DI (use GetRequiredService to guarantee non-null assignment)
            _initializer = services.GetRequiredService<AppInitializer>();
            _alarmTracker = services.GetRequiredService<AlarmTracker>();
            _appState = services.GetRequiredService<AppState>();

            // Kick off initialization once Shell is ready
            Loaded += async (s, e) =>
            {
                try
                {
                    await _initializer.InitializeAsync();

                    _alarmTracker.Start();

                    //_appState.IsInitialized = true; // Startup GATE OPEN
                    _appState.MarkReady();

                    Debug.WriteLine("App fully initialized.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            };
        }

    }
}
