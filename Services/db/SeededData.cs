using Mystic_ToDo_MAUI_.Model.db.tables;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mystic_ToDo_MAUI_.Model.db;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class SeededData 
    {
        private readonly DBManager<GroupList> _groupListRepo;
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;
      
        public SeededData()
        {
            _repeatTagRepo = new DBManager<TaskList_RepeatTag>();
            _groupListRepo = new DBManager<GroupList>();
        }

        // Async method to see defaults
        public async Task SeedAsync() 
        {
            // GroupList Defaults
            await _groupListRepo.SeedDefaultsAsync(new List<GroupList>
            {
                new GroupList { 
                                ID = 0, GroupName = "Default", ParentId = null, 
                                ColorHex=BaseColors.blue300, IconPath="dotnet_bot.png", IsDefault = true
                                },
                new GroupList {
                                ID = 0, GroupName = "Important", ParentId = null,
                                ColorHex=BaseColors.red300, IconPath="dotnet_bot.png", IsDefault = true
                                },
                new GroupList {
                                ID = 0, GroupName = "Completed", ParentId = null,
                                ColorHex=BaseColors.gray300, IconPath="dotnet_bot.png", IsDefault = true
                                }
            });

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
