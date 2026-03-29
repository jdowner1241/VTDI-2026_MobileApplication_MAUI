using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList : BaseDB
    {
        public string Title { get; set; }
        public string? Notes { get; set; }
        public int GroupID { get; set; } = 0;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; } = null;
        public bool IsActive { get; set; } = true;



        //  Foreign key to TaskList_AddedInfo
        //public int? Task_AddedInfoID { get; set; } = null;
        //[Ignore]
        //public TaskList_AddedInfo? Task_AddedInfo { get; set; }

        // Foreign Key to  TaskRepeatTag
        public int? Task_RepeatListID { get; set; } = null;
        [Ignore]
        public TaskList_RepeatList? TaskList_RepeatList { get; set; }

        // One-to-many relationship: Task → Attachments
        [Ignore]
        public List<Attachments>? Attachments { get; set; } = new();


        public TaskList() 
        {

        }

    }
}
