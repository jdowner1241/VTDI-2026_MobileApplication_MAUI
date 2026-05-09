
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.Services.debuggerHelpers;
using Mystic_ToDo_MAUI_.Services.ThemeHelpers;
using Mystic_ToDo_MAUI_.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services
{

    public class AppInitializer
    {
        private readonly ThemeSwitcher _themeSwitcher;
        private readonly DBInitializer _dbInitializer;
        private readonly AppSettingsService _settingsService;
        private readonly AppState _appState;

        public AppInitializer
            (
            DBInitializer dbInitializer,
            ThemeSwitcher themeSwitcher,
            AppSettingsService settingsService,
            AppState appState
            )
        {
            _themeSwitcher = themeSwitcher;
            _dbInitializer = dbInitializer;
            _settingsService = settingsService;
            _appState = appState;
        }

        public async Task InitializeAsync()
        {
            // Load Database
            //var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");

            //if (!File.Exists(dbPath))
            //{
            //    Debug.WriteLine("Database not ready yet. Creating schema...");
            //}

            // Always ensure database is created and schema is up to date before loading data
            await _dbInitializer.DBInitializerAsync();


            // Load App Settings
            var settings = await _settingsService.LoadSettingsAsync();

            // Apply default theme on UI thread
            try
            {
                //if (MainThread.IsMainThread)
                //{
                //    _themeSwitcher.SetTheme("DarkTheme");
                //}
                //else
                //{
                //    MainThread.BeginInvokeOnMainThread(() =>
                //        _themeSwitcher.SetTheme("DarkTheme"));
                //}
                if (settings.IsDarkModeEnabled)
                {
                    _themeSwitcher.SetTheme("DarkTheme");
                }
                else
                {
                    _themeSwitcher.SetTheme("LightTheme");
                }

            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Theme switch failed: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }

            // Set App State to ready after all initialization is complete
            _appState.MarkReady();

        }

    }
}
