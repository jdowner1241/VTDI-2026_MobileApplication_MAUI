using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Mystic_ToDo_MAUI_.Services;
using Mystic_ToDo_MAUI_.Services.Alarm;

namespace Mystic_ToDo_MAUI_
{
    public partial class AppShell : Shell
    {

        private readonly AppInitializer _initializer;
        private readonly AlarmTracker _alarmTracker;

        public AppShell(IServiceProvider services)
        {
            InitializeComponent();

            // Resolve AppInitializer from DI (use GetRequiredService to guarantee non-null assignment)
            _initializer = services.GetRequiredService<AppInitializer>();
            _alarmTracker = services.GetRequiredService<AlarmTracker>();

            // Kick off initialization once Shell is ready
            Loaded += async (s, e) =>
            {
                try
                {
                    await _initializer.InitializeAsync();
                    Debug.WriteLine("App initialization completed successfully.");

                    // Start the alarm service
                    _alarmTracker.Start();
                    Debug.WriteLine("AlarmTracker started.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"AppInitializer failed: {ex}");
                }
            };
        }

    }
}
