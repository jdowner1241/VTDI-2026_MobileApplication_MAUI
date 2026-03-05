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
            DumpResources("OnSleep");
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
            ResourceDebugger.DumpAllResources("OnExit");
            //DumpResources("OnExit");
            base.CleanUp();
        }

        private void DumpResources(string phase)
        {
            Debug.WriteLine($"--- Dumping resources at {phase} ---");

            // Top-level resources
            foreach (var key in Resources.Keys)
            {
                var value = Resources[key];
                Debug.WriteLine($"Top-level Resource: {key} = {value}");
            }

            // Merged dictionaries
            foreach (var dict in Resources.MergedDictionaries)
            {
                Debug.WriteLine($"Dictionary: {dict.GetType().FullName}");
                foreach (var key in dict.Keys)
                {
                    var value = dict[key];
                    Debug.WriteLine($"   Key: {key} = {value}");
                }
            }
        }


        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(Services));
        }

    }
}