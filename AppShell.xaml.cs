using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Mystic_ToDo_MAUI_.Services;

namespace Mystic_ToDo_MAUI_
{
    public partial class AppShell : Shell
    {

        private readonly AppInitializer _initializer;

        public AppShell(IServiceProvider services)
        {
            InitializeComponent();

            // Resolve AppInitializer from DI (use GetRequiredService to guarantee non-null assignment)
            _initializer = services.GetRequiredService<AppInitializer>();

            // Kick off initialization once Shell is ready
            Loaded += async (s, e) =>
            {
                if (_initializer != null)
                {
                    try
                    {
                        await _initializer.InitializeAsync();
                        Debug.WriteLine("App initialization completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"AppInitializer failed: {ex}");
                    }
                }
            };

        }

        private void customIntial()
        {
            

        }

    }
}
