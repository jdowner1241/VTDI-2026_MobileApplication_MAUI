using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class TaskList : BaseDB
    {
        public string Title { get; set; }
        public int GroupID { get; set; } = 0;
        public int? Task_AddedInfoID { get; set; } = null;
        public int? Task_RepeatID { get; set; } = null;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; } = null;
        public bool IsActive { get; set; } = true;


        public TaskList() 
        {

        }

    }
}
