
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services
{

    public class AppInitializer
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly ThemeSwitcher _themeSwitcher;
        private readonly DBInitializer _dbInitializer;


        public AppInitializer(HomeViewModel homeViewModel, ThemeSwitcher themeSwitcher, DBInitializer dbInitializer)
        {
            _themeSwitcher = themeSwitcher;
            _dbInitializer = dbInitializer;
            _homeViewModel = homeViewModel;
        }

        public async Task InitializeAsync()
        {
            // Load theme
            _themeSwitcher.setTheme("Dark");

            // Initialize database components and seed data
            await _dbInitializer.DBInitializerAsync();

            // Initialize ViewModels (if needed)
            await _homeViewModel.LoadDataAsync();
        }

    }
}
