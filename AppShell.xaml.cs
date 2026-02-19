using Mystic_ToDo_MAUI_.Resources.SharedResources.Color;

namespace Mystic_ToDo_MAUI_
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            customIntial();
        }

        private void customIntial()
        {
            LoadTheme();
        }

        private static void LoadTheme()
        {
            var CurrentTheme = new ThemeSwitcher();
            CurrentTheme.setTheme("Light");
        }
    }
}
