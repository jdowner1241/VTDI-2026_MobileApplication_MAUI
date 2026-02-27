using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList_AddedInfo : BaseDB
    {
        public String? Notes { get; set; }
        public String? AttachmentPath { get; set; }
        public bool IsDefault { get; set; } = false;

        public TaskList_AddedInfo()
        {

        }
    }
}
