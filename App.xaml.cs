using Microsoft.Extensions.DependencyInjection;
using Mystic_ToDo_MAUI_.Services;
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


        //public App()
        //{
        //    try
        //    {
        //        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandled;
        //        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        //        InitializeComponent();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Startup exception: {ex}");
        //        File.AppendAllText("crash.log", $"Startup exception: {ex}");
        //    }
        //}

        //private void OnDomainUnhandled(object sender, UnhandledExceptionEventArgs e)
        //{
        //    var ex = e.ExceptionObject as Exception;
        //    Debug.WriteLine($"Domain unhandled: {ex}");
        //    File.AppendAllText("crash.log", ex?.ToString() ?? "null");

        //}

        //private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        //{
        //    Debug.WriteLine($"Task unobserved: {e.Exception}");
        //    e.SetObserved();
        //}

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(Services));
        }

    }
}