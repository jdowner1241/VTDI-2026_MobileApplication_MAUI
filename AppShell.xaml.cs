using Mystic_ToDo_MAUI_.Resources.SharedResources.Color;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.ViewModel;

namespace Mystic_ToDo_MAUI_
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();

            //customIntial();
        }

        //private void customIntial()
        //{
        //    LoadTheme();
        //    LoadDBComponents();
        //    LoadViewModels();
        //}

        //private void LoadViewModels()
        //{
        //    BindingContext = new HomeViewModel();
        //     //this.BindingContext = _homeViewModel;
        //}

        //private void LoadDBComponents()
        //{
        //    // Initialize the database components and seed data
        //    SeededData _seededData = new SeededData();
        //}

        //private static void LoadTheme()
        //{
        //    var CurrentTheme = new ThemeSwitcher();
        //    CurrentTheme.setTheme("Dark");
        //}
    }
}
