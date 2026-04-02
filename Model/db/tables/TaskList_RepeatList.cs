using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList_RepeatList : BaseDB
    {

        public DateTime CurrentDate { get; set; }
        public DateTime DueDate { get; set; }

        // Forigen Key For TaskRepeatTag
        public int RepeatTagID { get; set; } = 0;
        [Ignore]
        public TaskList_RepeatTag RepeatTag { get; set; }

        public TaskList_RepeatList()
        {

        }

    }
}
