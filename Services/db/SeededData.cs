using Mystic_ToDo_MAUI_.Model.db.tables;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mystic_ToDo_MAUI_.Model.db;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class SeededData 
    {
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;
      
        public SeededData()
        {
            _repeatTagRepo = new DBManager<TaskList_RepeatTag>();
        }

        // Async method to see defaults
        public async Task SeedAsync() 
        {
            // RepeatTag Defaults
            await _repeatTagRepo.SeedDefaultsAsync(new List<TaskList_RepeatTag>
            {
                new TaskList_RepeatTag { ID = 0, RepeatTagName = "NotSet" },
                new TaskList_RepeatTag { ID = 1, RepeatTagName = "Daily" },
                new TaskList_RepeatTag { ID = 2, RepeatTagName = "Weekly" },
                new TaskList_RepeatTag { ID = 3, RepeatTagName = "Monthly" },
                new TaskList_RepeatTag { ID = 4, RepeatTagName = "Yearly" }
            });
        }

        }
    }
}
