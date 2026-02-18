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
            new ThemeSwitcher().setTheme("Dark");
        }
    }
}
