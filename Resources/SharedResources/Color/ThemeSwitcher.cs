using System;
using Microsoft.Maui.Controls;
using Mystic_ToDo_MAUI_.Resources.SharedResources.Color.Themes;

namespace Mystic_ToDo_MAUI_.Resources.SharedResources.Color
{
    class ThemeSwitcher
    {
        //public void setTheme(string themeName)
        //{
        //    // 1-2: Get current application and bail out if it's not available
        //    var app = Application.Current;
        //    if (app == null)
        //        return;

        //    // 3: Ensure Resources exists; create and assign if necessary
        //    var resources = app.Resources ?? (app.Resources = new ResourceDictionary());

        //    // 4: Operate on MergedDictionaries safely
        //    var merged = resources.MergedDictionaries;
        //    merged.Clear();

        //    // Always load the base colors first
        //    merged.Add(new ResourceDictionary { Source = new Uri("BaseColors.xaml", UriKind.Relative) });

        //    if (string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase))
        //        merged.Add(new Mystic_ToDo_MAUI_.Resources.SharedResources.Color.Themes.Theme_Dark());
        //    else
        //        merged.Add(new Mystic_ToDo_MAUI_.Resources.SharedResources.Color.Themes.Theme_Light());

        //    ////load theme-specific resources if a theme name was supplied
        //    //if (!string.IsNullOrWhiteSpace(themeName))
        //    //{

        //    //    // Encure themeName is valid before attempting to load theme-specific resources
        //    //    themeName = themeName.Trim(); // Normalize input
        //    //    themeName = char.ToUpper(themeName[0]) + themeName.Substring(1).ToLower(); // Capitalize first letter

        //    //    // Construct the URI for the theme resource and add it to the merged dictionaries
        //    //    merged.Add(new ResourceDictionary { Source = new Uri($"Themes/Theme_{themeName}.xaml", UriKind.Relative) });
        //    //}
        //}

        public void setTheme(string themeName) {

            // Get current application and bail out if it's not available
            var app = Application.Current;
            if (app == null)
                return;

            // Ensure Resources exists; create and assign if necessary
            var resources = app.Resources ??= new ResourceDictionary();
            var merged = resources.MergedDictionaries;

            // Clear existing merged dictionaries to avoid conflicts
            merged.Clear();

            // Add typed dictionaries
            merged.Add(new BaseColors());

            // Add theme-specific resources based on the provided theme name
            if (string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase))
                merged.Add(new Themes.Theme_Dark());
            else
                merged.Add(new Themes.Theme_Light());
        }

    }
}
