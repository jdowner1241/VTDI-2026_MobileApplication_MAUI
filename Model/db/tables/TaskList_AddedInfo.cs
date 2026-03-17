using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList_AddedInfo : BaseDB
    {
        public string? Notes { get; set; }
        public bool IsDefault { get; set; } = false;

        // Foreign Key for Attachments
        public int? AttachmentID { get; set; } = 0;
        [Ignore]
        public Attachments? Attachment { get; set; }


        public TaskList_AddedInfo()
        {

        }
    }
}
