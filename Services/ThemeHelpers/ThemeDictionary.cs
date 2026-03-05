using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor.Themes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.ThemeHelpers
{
    public class ThemeDictionary: ResourceDictionary
    {

        public void ApplyDark()
        {
            Clear();
            MergedDictionaries.Add(new BaseColors());
            MergedDictionaries.Add(new Dark());
            MergedDictionaries.Add(new CustomStyles());
        }

        public void ApplyLight()
        {
            Clear();
            MergedDictionaries.Add(new BaseColors());
            MergedDictionaries.Add(new Light());
            MergedDictionaries.Add(new CustomStyles());
        }


    }
}
