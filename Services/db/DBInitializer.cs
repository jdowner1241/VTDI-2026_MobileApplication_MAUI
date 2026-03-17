using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class DBInitializer
    {
        private DBManager<GroupList>? _groupListRepo;
        private DBManager<TaskList>? _taskListRepo;
        private DBManager<TaskList_AddedInfo>? _taskListAddedInfoRepo;
        private DBManager<TaskList_RepeatList>? _taskListRepeatListRepo;
        private DBManager<TaskList_RepeatTag>? _repeatTagRepo;
        private DBManager<Attachments>? _attachmentsRepo;

        public DBInitializer()
        {

        }

        public async Task DBInitializerAsync()
        {
      
                // Now safe to access FileSystem.AppDataDirectory
                _groupListRepo = new DBManager<GroupList>();
                _taskListRepo = new DBManager<TaskList>();
                _taskListAddedInfoRepo = new DBManager<TaskList_AddedInfo>();
                _taskListRepeatListRepo = new DBManager<TaskList_RepeatList>();
                _repeatTagRepo = new DBManager<TaskList_RepeatTag>();
                _attachmentsRepo = new DBManager<Attachments>();

                // InitializeAsync() on each DBManager
                await _groupListRepo.InitializeAsync();
                await _taskListRepo.InitializeAsync();
                await _taskListAddedInfoRepo.InitializeAsync();
                await _taskListRepeatListRepo.InitializeAsync();
                await _repeatTagRepo.InitializeAsync();
                await _attachmentsRepo.InitializeAsync();

            // Seed for GroupList and TaskList_RepeatTag
            await _groupListRepo.SeedDefaultsAsync(new List<GroupList>
            {
                new GroupList {
                                GroupName = "Root", ParentId = null,
                                ColorHex=BaseColors.blue300, IconPath="dotnet_bot.png", IsDefault = true
                                },
                new GroupList {
                                GroupName = "Default", ParentId = 1,
                                ColorHex=BaseColors.blue300, IconPath="dotnet_bot.png", IsDefault = true
                                },
                new GroupList {
                                GroupName = "Important", ParentId = 1,
                                ColorHex=BaseColors.red300, IconPath="dotnet_bot.png", IsDefault = true
                                },
                new GroupList {
                                GroupName = "Completed", ParentId = 1,
                                ColorHex=BaseColors.gray300, IconPath="dotnet_bot.png", IsDefault = true
                                }
            });


            await _repeatTagRepo.SeedDefaultsAsync(new List<TaskList_RepeatTag>
            {
                new TaskList_RepeatTag { ID=0, RepeatTagName = "NotSet" },
                new TaskList_RepeatTag { ID=1, RepeatTagName = "Daily" },
                new TaskList_RepeatTag { ID=2, RepeatTagName = "Weekly" },
                new TaskList_RepeatTag { ID=3, RepeatTagName = "Monthly" },
                new TaskList_RepeatTag { ID=4, RepeatTagName = "Yearly" }
            });

        }

    }
}
