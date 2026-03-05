using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.debuggerHelpers
{
    public static class ResourceDebugger
    {
        public static void DumpAllResources(string phase)
        {
            System.Diagnostics.Debug.WriteLine($"=== Dumping resources at {phase} ===");

            var app = Application.Current;
            if (app == null)
            {
                System.Diagnostics.Debug.WriteLine("No Application.Current");
                return;
            }

            // Top-level resources
            foreach (var key in app.Resources.Keys)
            {
                try
                {
                    var value = app.Resources[key];
                    System.Diagnostics.Debug.WriteLine($"Top-level: {key} = {value?.GetType().FullName}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Top-level: {key} threw {ex.GetType().Name}");
                }
            }

            // Merged dictionaries
            foreach (var dict in app.Resources.MergedDictionaries)
            {
                System.Diagnostics.Debug.WriteLine($"Dictionary: {dict.GetType().FullName}");
                foreach (var key in dict.Keys)
                {
                    try
                    {
                        var value = dict[key];
                        System.Diagnostics.Debug.WriteLine($"   Key: {key} = {value?.GetType().FullName}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"   Key: {key} threw {ex.GetType().Name}");
                    }
                }
            }
        }
    }
}
