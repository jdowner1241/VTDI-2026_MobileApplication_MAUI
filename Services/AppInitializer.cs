
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

        private readonly HomeViewModel _homeViewModel;
        private readonly ThemeSwitcher _themeSwitcher;
        private readonly DBInitializer _dbInitializer;


        public AppInitializer(
                                HomeViewModel homeViewModel, 
                                ThemeSwitcher themeSwitcher, 
                                DBInitializer dbInitializer)
        {
            _homeViewModel = homeViewModel;
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



            // Initialize database components and seed data
            await _dbInitializer.DBInitializerAsync();

            // Initialize ViewModels (if needed)
            await _homeViewModel.LoadDataAsync();

        }

    }
}
