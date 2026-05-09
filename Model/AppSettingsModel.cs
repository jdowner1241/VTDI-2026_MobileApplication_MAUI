using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model
{
    public class AppSettingsModel
    {
        public bool IsDarkModeEnabled { get; set; } = true;

        public bool AlarmNotificationsEnabled { get; set; } = true;

        public bool RepeatNotificationsEnabled { get; set; } = true;

        public bool GoogleSyncEnabled { get; set; } = false;

        public string GoogleAccountEmail { get; set; } = "Not Connected";
    }
}
