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
        private DBManager<TaskList_RepeatList>? _taskListRepeatListRepo;
        private DBManager<TaskList_RepeatTag>? _repeatTagRepo;
        private DBManager<Attachments>? _attachmentsRepo;

        public DBInitializer()
        {

        }

        public async Task DBInitializerAsync()
        {
      
                _groupListRepo = new DBManager<GroupList>();
                _repeatTagRepo = new DBManager<TaskList_RepeatTag>();
                _taskListRepeatListRepo = new DBManager<TaskList_RepeatList>();
                _taskListRepo = new DBManager<TaskList>();
                _attachmentsRepo = new DBManager<Attachments>();

                // InitializeAsync() on each DBManager
                await _groupListRepo.InitializeAsync();
                await _repeatTagRepo.InitializeAsync();
                await _taskListRepeatListRepo.InitializeAsync();
                await _taskListRepo.InitializeAsync();
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

            await _taskListRepeatListRepo.SeedDefaultsAsync(new List<TaskList_RepeatList>
            {
                new TaskList_RepeatList { HowOften=1, CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 1}, // Daily
                new TaskList_RepeatList { HowOften=1, CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 2}, // Weekly
                new TaskList_RepeatList { HowOften=2, CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 3}, // Monthly
                new TaskList_RepeatList { HowOften=1, CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 4}, // Yearly
            });

            await _taskListRepo.SeedDefaultsAsync(new List<TaskList>
            {
                new TaskList { Title = "Buy groceries", Notes = "Milk, Bread, Eggs", GroupID = 2, CreatedDate = DateTime.Now },
                new TaskList { Title = "Finish project report", Notes = "Due by end of the week", GroupID = 3, CreatedDate = DateTime.Now },
                new TaskList { Title = "Call plumber", Notes = "Fix the kitchen sink", GroupID = 2, CreatedDate = DateTime.Now },
                new TaskList { Title = "Plan weekend trip", Notes = "", GroupID = 2, CreatedDate = DateTime.Now, Task_RepeatListID=2 }
            });


            await _attachmentsRepo.SeedDefaultsAsync(new List<Attachments>
            {
                new Attachments { AttachmentName = "Report", AttachmentType = "PDF", AttachmentPath = "/files/report.pdf", TaskListId = 1 },
                new Attachments { AttachmentName = "Image1", AttachmentType = "Image", AttachmentPath = "/files/image1.png", TaskListId = 1 },
                new Attachments { AttachmentName = "Notes", AttachmentType = "Text", AttachmentPath = "/files/notes.txt", TaskListId = 1 },
                new Attachments { AttachmentName = "Notes2", AttachmentType = "Text", AttachmentPath = "/files/notes2.txt", TaskListId = 1 },

            });




        }

    }
}
