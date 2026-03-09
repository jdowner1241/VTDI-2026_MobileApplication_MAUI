using Microsoft.Extensions.DependencyInjection;
using Mystic_ToDo_MAUI_.Services;
using Mystic_ToDo_MAUI_.Services.debuggerHelpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Mystic_ToDo_MAUI_
{
    public partial class App : Application
    {

        public IServiceProvider Services { get; }

        public App(IServiceProvider services)
        {
            Services = services;

            InitializeComponent();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Debug.WriteLine("=== App Started ===");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Debug.WriteLine("=== App Sleeping ===");
 
        }

        protected override void OnResume()
        {
            base.OnResume();
            Debug.WriteLine("=== App Resumed ===");
        }

        protected override void CleanUp()
        {
            // This runs when the app is shutting down (WinUI/desktop)
            Debug.WriteLine("=== App Closing ===");
            //ResourceDebugger.DumpAllResources("OnExit");
            base.CleanUp();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(Services));
        }

    }
}