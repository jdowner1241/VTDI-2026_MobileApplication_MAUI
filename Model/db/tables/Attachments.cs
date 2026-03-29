using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class Attachments : BaseDB
    {
        public string AttachmentName { get;set;} = string.Empty;
        public string AttachmentType { get;set;} = string.Empty;
        public string AttachmentPath { get;set;} = string.Empty;

        // Foreign key back to TaskList
        public int TaskListId { get; set; }
        [Ignore]
        public TaskList TaskList { get; set; }


        public Attachments() { }
    }
}
