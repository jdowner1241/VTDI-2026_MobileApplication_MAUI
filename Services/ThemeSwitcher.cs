using System;
using Microsoft.Maui.Controls;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor.Themes;

namespace Mystic_ToDo_MAUI_.Services
{
    public class ThemeSwitcher
    {

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

            // Start Base Colors Dictionary
            merged.Add(new BaseColors());

            // Add theme-specific resources based on the provided theme name
            if (string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase))
            {
                //merged.Add(new Themes.Theme_Dark());
                merged.Add(new Dark());
                merged.Add(new CustomStyles());
            }
            else
            {
                merged.Add(new Light());
                merged.Add(new CustomStyles());
            }

        }

    }
}
