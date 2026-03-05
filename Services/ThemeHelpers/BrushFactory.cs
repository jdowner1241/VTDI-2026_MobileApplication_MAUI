using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Mystic_ToDo_MAUI_.Services.ThemeHelpers;
using System;

namespace Mystic_ToDo_MAUI_.Services
{
    public class BrushFactory : IBrushFactory
    {
        readonly (string BrushKey, string ColorKey)[] _map = new[]
        {
            ("App_PrimaryBrush","App_PrimaryColor"),
            ("App_SecondaryBrush","App_SecondaryColor"),
            ("App_BorderBrush","App_DividerColor"),
            ("App_GlowBrush","App_GlowColor"),
            ("App_ShadowBrush","App_ShadowColor"),
            ("App_ControlBackgroundBrush","App_ControlBackgroundColor"),
            ("App_ControlTextBrush","App_ControlTextColor")
        };

        public void EnsureBrushes()
        {
            var resources = Application.Current?.Resources;
            if (resources == null) return;

            foreach (var (brushKey, colorKey) in _map)
            {
                if (resources.ContainsKey(brushKey)) continue;
                if (!resources.ContainsKey(colorKey)) continue;
                if (!(resources[colorKey] is Color color)) continue;

                try
                {
                    // Create on UI thread to avoid WinRT/Composition errors
                    Application.Current?.Dispatcher?.Dispatch(() =>
                    {
                        if (!resources.ContainsKey(brushKey))
                            resources[brushKey] = new SolidColorBrush(color);
                    });
                }
                catch
                {
                    // Fallback: best-effort non-dispatch creation
                    if (!resources.ContainsKey(brushKey))
                        resources[brushKey] = new SolidColorBrush(color);
                }
            }
        }
    }
}