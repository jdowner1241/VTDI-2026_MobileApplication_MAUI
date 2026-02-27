using Mystic_ToDo_MAUI_.Model.db.tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class DBInitializer
    {
        private DBManager<GroupList> _groupListRepo = new ();
        private DBManager<TaskList> _taskListRepo = new ();
        private DBManager<TaskList_AddedInfo> _taskListAddedInfoRepo = new ();
        private DBManager<TaskList_RepeatList> _taskListRepeatListRepo = new ();
        private DBManager<TaskList_RepeatTag> _repeatTagRepo = new ();

        public DBInitializer()
        {

        }

        public async Task DBInitializerAsync()
        {
            await Task.Run(() =>
            {
                _groupListRepo = new DBManager<GroupList>();
                _taskListRepo = new DBManager<TaskList>();
                _taskListAddedInfoRepo = new DBManager<TaskList_AddedInfo>();
                _taskListRepeatListRepo = new DBManager<TaskList_RepeatList>();
                _repeatTagRepo = new DBManager<TaskList_RepeatTag>();
            });
        }

    }
}
