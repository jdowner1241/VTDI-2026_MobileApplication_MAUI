using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class GroupList : BaseDB
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Color? Color { get; set; }
        public bool IsDefault { get; set; } = false;
    
        public GroupList() 
        {

        }
    }
}
