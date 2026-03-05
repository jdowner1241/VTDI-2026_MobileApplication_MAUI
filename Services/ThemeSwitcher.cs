using Microsoft.Maui.Controls;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor.Themes;
using Mystic_ToDo_MAUI_.Services.ThemeHelpers;
using System;
using System.Diagnostics;

namespace Mystic_ToDo_MAUI_.Services
{
    public class ThemeSwitcher
    {


        public ThemeSwitcher()
        {
          
        }


        //public void SetTheme(string themeName)
        //{
        //    try 
        //    {

        //        var merged = Application.Current?.Resources?.MergedDictionaries;
        //        if (merged == null)
        //            return;

        //        // Remove existing theme dictionary
        //        var toRemove = merged.Where(md => md is Dark || md is Light).ToList();
        //        foreach (var md in toRemove)
        //            merged.Remove(md);

        //        // Add the new theme
        //        if (string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase))
        //            merged.Add(new Dark());

        //        else
        //            merged.Add(new Light());

        //        merged.Add(new CustomStyles());
        //        merged.Add(new BrushOnlyStyles());
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Theme apply failed: {ex}");
        //    }
        //}


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
                    var oldTheme = merged.Where(md => md is Dark || md is Light).ToList();
                    foreach (var md in oldTheme) merged.Remove(md);

                    // Remove previously added CustomStyles / BrushOnlyStyles so rebinds occur
                    var oldStyles = merged
                        .Where(md => md.GetType().Name.IndexOf("CustomStyles", StringComparison.OrdinalIgnoreCase) >= 0
                                  || md.GetType().Name.IndexOf("BrushOnlyStyles", StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                    foreach (var md in oldStyles) merged.Remove(md);

                    // Add the new theme (colors only)
                    if (string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase))
                        merged.Add(new Dark());
                    else
                        merged.Add(new Light());

                    // Add CustomStyles (color-only styles)
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
