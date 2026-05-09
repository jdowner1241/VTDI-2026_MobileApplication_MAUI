using Microsoft.Maui.Controls;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor.Themes;
using System;
using System.Diagnostics;

namespace Mystic_ToDo_MAUI_.Services.ThemeHelpers
{
    public class ThemeSwitcher
    {


        public ThemeSwitcher()
        {
          
        }

        public void SetTheme(string themeName)
        {
            var app = Application.Current;
            var merged = app?.Resources?.MergedDictionaries;
            if (merged == null)
                return;

            void ApplyTheme()
            {
                try
                {
                    // Remove existing theme dictionaries (Dark/Light)
                    var oldTheme = merged.Where(md => md is DarkTheme || md is LightTheme).ToList();
                    foreach (var md in oldTheme) merged.Remove(md);

                    // Remove previously added Brushes, CustomStyles and BrushOnlyStyles so rebinds occur
                    var oldStyles = merged
                        .Where(md => md.GetType().Name.IndexOf("ThemeBrushes", StringComparison.OrdinalIgnoreCase) >= 0
                                  || md.GetType().Name.IndexOf("CustomStyles", StringComparison.OrdinalIgnoreCase) >= 0
                                  || md.GetType().Name.IndexOf("BrushOnlyStyles", StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                    foreach (var md in oldStyles) merged.Remove(md);

                    // Add the new theme (colors only)
                    if (string.Equals(themeName, "DarkTheme", StringComparison.OrdinalIgnoreCase)) 
                    {
                        merged.Add(new DarkTheme());
                    }
                    else
                    {
                        merged.Add(new LightTheme());
                    }
                        

                    // Add CustomStyles (color-only styles)
                    merged.Add(new ThemeBrushes());
                    merged.Add(new CustomStyles());

                    // Add BrushOnlyStyles AFTER CustomStyles so brush-backed setters augment existing styles
                    //merged.Add(new BrushOnlyStyles());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Theme apply failed: {ex}");
                }
            }

            if (app?.Dispatcher != null)
                app.Dispatcher.Dispatch(ApplyTheme);
            else
                ApplyTheme();
        }




    }

}
