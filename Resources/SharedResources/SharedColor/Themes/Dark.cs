using MauiColor = Microsoft.Maui.Graphics.Color;

namespace Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor.Themes
{
    internal class Dark : ResourceDictionary
    {
        
        public Dark()
        {
            // Safely obtain the current application and its resources
            var app = Application.Current;

            if (app != null && app.Resources != null)
            {
                // Initialize color tokens
                // Main Colors
                this["PrimaryBrush"] = (MauiColor)app.Resources["blue500"];
                this["SecondaryBrush"] = (MauiColor)app.Resources["teal200"];
                this["TertiaryBrush"] = (MauiColor)app.Resources["blue300"];
                this["BackgroundBrush"] = (MauiColor)app.Resources["black500"];
                this["SurfaceBrush"] = (MauiColor)app.Resources["gray300"];
                this["TextBrush"] = (MauiColor)app.Resources["white900"];
                this["TextHeaderBrush"] = (MauiColor)app.Resources["purple800"];
                this["TextSubHeaderBrush"] = (MauiColor)app.Resources["purple600"];
                this["MuteTextBrush"] = (MauiColor)app.Resources["gray400"];


                // Effects
                this["BorderBrush"] = (MauiColor)app.Resources["purple300"];
                this["LinkBrush"] = (MauiColor)app.Resources["blue500"];
                this["LinkHoverBrush"] = (MauiColor)app.Resources["blue700"];
                this["ShadowBrush"] = (MauiColor)app.Resources["gray200"];
                this["GlowBrush"] = (MauiColor)app.Resources["purple300"];

                // States
                this["ErrorBrush"] = (MauiColor)app.Resources["red500"];
                this["WarningBrush"] = (MauiColor)app.Resources["yellow500"];
                this["InfoBrush"] = (MauiColor)app.Resources["blue500"];
                this["SuccessBrush"] = (MauiColor)app.Resources["green500"];
                this["DisabledBrush"] = (MauiColor)app.Resources["gray500"];
                this["SelectedBrush"] = (MauiColor)app.Resources["blue500"];
                this["UnselectedBrush"] = (MauiColor)app.Resources["gray600"];


                // Inverted Color
                this["InverseTextBrush"] = (MauiColor)app.Resources["black500"];

                // Controler Colors
                this["ControlBackgroundBrush"] = (MauiColor)app.Resources["blue300"];
                this["ControlTextBrush"] = (MauiColor)app.Resources["black500"];
                this["PlaceholderTextBrush"] = (MauiColor)app.Resources["gray600"];
                this["DividerBrush"] = (MauiColor)app.Resources["purple600"];
                this["NavigationBarBrush"] = (MauiColor)app.Resources["purple400"];
                this["NavigationTextBrush"] = (MauiColor)app.Resources["black400"];
                this["TabBarBackgroundBrush"] = (MauiColor)app.Resources["gray500"];
                this["TabBarTextBrush"] = (MauiColor)app.Resources["black400"];

            }
        }
    }
}
