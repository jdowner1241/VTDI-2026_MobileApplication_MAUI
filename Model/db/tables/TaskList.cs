using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList : BaseDB
    {
        public string Title { get; set; }
        public int GroupID { get; set; } = 0;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; } = null;
        public bool IsActive { get; set; } = true;


        //  Foreign key to TaskList_AddedInfo
        public int? Task_AddedInfoID { get; set; } = null;
        [Ignore]
        public TaskList_AddedInfo? Task_AddedInfo { get; set; }

        // Foreign Key to  TaskRepeatTag
        public int? Task_RepeatID { get; set; } = null;
        [Ignore]
        public TaskList_RepeatTag? TaskList_RepeatTag { get; set; }


        public TaskList() 
        {

        }

    }
}
