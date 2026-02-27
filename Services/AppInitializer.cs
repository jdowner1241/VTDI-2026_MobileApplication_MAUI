
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
        private readonly SeededData _seededData;
        private readonly HomeViewModel _homeViewModel;
        private readonly ThemeSwitcher _themeSwitcher;
        private readonly DBInitializer _dbInitializer;


        public AppInitializer(SeededData seededData, HomeViewModel homeViewModel, ThemeSwitcher themeSwitcher, DBInitializer dbInitializer)
        {
            _seededData = seededData;
            _homeViewModel = homeViewModel;
            _themeSwitcher = themeSwitcher;
            _dbInitializer = dbInitializer;
        }

        public async Task InitializeAsync()
        {
            // Load theme
            _themeSwitcher.setTheme("Dark");

            // Initialize database components and seed data
            await _dbInitializer.DBInitializerAsync();

            // Seed database
            await _seededData.SeedAsync();

            // Initialize ViewModels (if needed)
            await _homeViewModel.LoadDataAsync();
        }

    }
}
