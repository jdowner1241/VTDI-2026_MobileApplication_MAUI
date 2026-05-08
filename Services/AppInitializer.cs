
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


        public AppInitializer(
                                ThemeSwitcher themeSwitcher, 
                                DBInitializer dbInitializer)
        {
            _themeSwitcher = themeSwitcher;
            _dbInitializer = dbInitializer;
        }

        public async Task InitializeAsync()
        {
            // Apply default theme on UI thread
            try
            {
                if (MainThread.IsMainThread)
                {
                    _themeSwitcher.SetTheme("DarkTheme");
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                        _themeSwitcher.SetTheme("DarkTheme"));
                }
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Theme switch failed: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");

            if (!File.Exists(dbPath))
            {
                Debug.WriteLine("Database not ready yet. Creating schema...");
            }

            // Always ensure database is created and schema is up to date before loading data
            await _dbInitializer.DBInitializerAsync();

            // Initialize database components and seed data
            await _dbInitializer.DBInitializerAsync();

        }

    }
}
