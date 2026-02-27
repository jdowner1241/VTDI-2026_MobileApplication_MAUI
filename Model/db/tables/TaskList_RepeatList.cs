using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList_RepeatList : BaseDB
    {
        public int RepeatTag { get; set; } = 0;
        public int HowOften { get; set; } = 0;
        public DateTime CurrentDate { get; set; }
        public DateTime DueDate { get; set; }

        public bool IsDefault { get; set; } = false;

        public TaskList_RepeatList()
        {

        }

    }
}
