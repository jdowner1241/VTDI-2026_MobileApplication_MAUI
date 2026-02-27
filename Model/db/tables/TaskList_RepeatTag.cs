using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList_RepeatTag
    {

        [PrimaryKey] // no AutoIncrement to ensure that the ID is set manually 
        public int ID { get; set; }
        public String RepeatTagName { get; set; }

        public TaskList_RepeatTag()
        {

        }

    }
}
