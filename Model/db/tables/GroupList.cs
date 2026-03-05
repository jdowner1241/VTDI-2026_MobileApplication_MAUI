
using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class GroupList : BaseDB
    {
        public string GroupName { get; set; } = string.Empty;
        public int? ParentId { get; set; } // Nullable to allow root groups without a parent
        public int SortOrder { get; set; } = 0; // Default sort order
        public string ColorHex { get; set; } = string.Empty; // Store color as a hex string (e.g., "#FF5733")
        public string? IconPath { get; set; } // Store the path to the icon image
        public bool IsDefault { get; set; } = false;

        [Ignore]
        public Color? Color => string.IsNullOrEmpty(ColorHex) ? null : Color.FromArgb(ColorHex);

        [Ignore]
        public ImageSource Icon => string.IsNullOrEmpty(IconPath) ? null : ImageSource.FromFile(IconPath);

        public GroupList() 
        {

        }
    }
}
