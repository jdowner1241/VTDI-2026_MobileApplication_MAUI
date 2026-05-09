using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class DBInitializer
    {
        private readonly DBService _dbService;

        private DBManager<GroupList>? _groupListRepo;
        private DBManager<TaskList>? _taskListRepo;
        private DBManager<TaskList_RepeatList>? _taskListRepeatListRepo;
        private DBManager<TaskList_RepeatTag>? _repeatTagRepo;
        private DBManager<Attachments>? _attachmentsRepo;

        public DBInitializer
            (
                DBService dbService,
                DBManager<GroupList> groupListRepo,
                DBManager<TaskList> taskListRepo,
                DBManager<TaskList_RepeatList> taskListRepeatListRepo,
                DBManager<TaskList_RepeatTag> repeatTagRepo,
                DBManager<Attachments> attachmentsRepo
            )
        {
            _dbService = dbService;
            _groupListRepo = groupListRepo;
            _taskListRepo = taskListRepo;
            _taskListRepeatListRepo = taskListRepeatListRepo;
            _repeatTagRepo = repeatTagRepo;
            _attachmentsRepo = attachmentsRepo;
        }

        private async Task<bool> IsDatabaseEmpty()
        {
            var count = await _groupListRepo.GetAllAsync();
            return count.Count == 0;
        }

        public async Task DBInitializerAsync()
        {
            // Ensure DB connection exists
            await _dbService.InitializeAsync();

            // Create tables via DBManager (CORRECT PLACE)
            await _groupListRepo.InitializeTableAsync();
            await _taskListRepo.InitializeTableAsync();
            await _taskListRepeatListRepo.InitializeTableAsync();
            await _repeatTagRepo.InitializeTableAsync();
            await _attachmentsRepo.InitializeTableAsync();

            // Seed data
            

            // Seed for GroupList and TaskList_RepeatTag
            await _groupListRepo.SeedDefaultsAsync(new List<GroupList>
            {
                new GroupList {
                                GroupName = "Root", ParentId = null,
                                ColorHex=BaseColors.blue300, IconPath="list.png", IsDefault = true
                                },
                new GroupList {
                                GroupName = "Default", ParentId = 1,
                                ColorHex=BaseColors.blue300, IconPath="list.png", IsDefault = true
                                },
                new GroupList {
                                GroupName = "Google Events", ParentId = 1,
                                ColorHex=BaseColors.purple300, IconPath="notification_important.png", IsDefault = true
                                },
                new GroupList {
                                GroupName = "Important", ParentId = 1,
                                ColorHex=BaseColors.red300, IconPath="notification_important.png", IsDefault = true
                                },
                new GroupList {
                                GroupName = "Completed", ParentId = 1,
                                ColorHex=BaseColors.gray300, IconPath="task_alt.png", IsDefault = true
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
                new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 1}, // Daily
                new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 2}, // Weekly
                new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 3}, // Monthly
                new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 4}, // Yearly
            });

            await _taskListRepo.SeedDefaultsAsync(new List<TaskList>
            {
                new TaskList { Title = "Buy groceries", Notes = "Milk, Bread, Eggs", GroupID = 2, CreatedDate = DateTime.Now, IsGoogleEvent = false, GoogleEventID = "" },
                new TaskList { Title = "Finish project report", Notes = "Due by end of the week", GroupID = 2, CreatedDate = DateTime.Now, IsGoogleEvent = false, GoogleEventID = "" },
                new TaskList { Title = "Call plumber", Notes = "Fix the kitchen sink", GroupID = 2, CreatedDate = DateTime.Now, IsGoogleEvent = false, GoogleEventID = "" },
                new TaskList { Title = "Plan weekend trip", Notes = "", GroupID = 2, CreatedDate = DateTime.Now, Task_RepeatListID=2, IsGoogleEvent = false, GoogleEventID = "" }
            });


            await _attachmentsRepo.SeedDefaultsAsync(new List<Attachments>
            {
                new Attachments { AttachmentName = "Report", AttachmentType = "PDF", AttachmentPath = "/files/report.pdf", TaskListId = 1 },
                new Attachments { AttachmentName = "Image1", AttachmentType = "Image", AttachmentPath = "/files/image1.png", TaskListId = 1 },
                new Attachments { AttachmentName = "Notes", AttachmentType = "Text", AttachmentPath = "/files/notes.txt", TaskListId = 2 },
                new Attachments { AttachmentName = "Notes2", AttachmentType = "Text", AttachmentPath = "/files/notes2.txt", TaskListId = 1 },

            });

        }


        //public async Task DBInitializerAsync()
        //{
      
        //        //_groupListRepo = new DBManager<GroupList>();
        //        //_repeatTagRepo = new DBManager<TaskList_RepeatTag>();
        //        //_taskListRepeatListRepo = new DBManager<TaskList_RepeatList>();
        //        //_taskListRepo = new DBManager<TaskList>();
        //        //_attachmentsRepo = new DBManager<Attachments>();

        //        // InitializeAsync() on each DBManager
        //        //await _groupListRepo.InitializeAsync();
        //        //await _repeatTagRepo.InitializeAsync();
        //        //await _taskListRepeatListRepo.InitializeAsync();
        //        //await _taskListRepo.InitializeAsync();
        //        //await _attachmentsRepo.InitializeAsync();

        //    // Seed for GroupList and TaskList_RepeatTag
        //    await _groupListRepo.SeedDefaultsAsync(new List<GroupList>
        //    {
        //        new GroupList {
        //                        GroupName = "Root", ParentId = null,
        //                        ColorHex=BaseColors.blue300, IconPath="list.png", IsDefault = true
        //                        },
        //        new GroupList {
        //                        GroupName = "Default", ParentId = 1,
        //                        ColorHex=BaseColors.blue300, IconPath="list.png", IsDefault = true
        //                        },
        //        new GroupList {
        //                        GroupName = "Google Events", ParentId = 1,
        //                        ColorHex=BaseColors.purple300, IconPath="notification_important.png", IsDefault = true
        //                        },
        //        new GroupList {
        //                        GroupName = "Important", ParentId = 1,
        //                        ColorHex=BaseColors.red300, IconPath="notification_important.png", IsDefault = true
        //                        },
        //        new GroupList {
        //                        GroupName = "Completed", ParentId = 1,
        //                        ColorHex=BaseColors.gray300, IconPath="task_alt.png", IsDefault = true
        //                        }
        //    });


        //    await _repeatTagRepo.SeedDefaultsAsync(new List<TaskList_RepeatTag>
        //    {
        //        new TaskList_RepeatTag { ID=0, RepeatTagName = "NotSet" },
        //        new TaskList_RepeatTag { ID=1, RepeatTagName = "Daily" },
        //        new TaskList_RepeatTag { ID=2, RepeatTagName = "Weekly" },
        //        new TaskList_RepeatTag { ID=3, RepeatTagName = "Monthly" },
        //        new TaskList_RepeatTag { ID=4, RepeatTagName = "Yearly" }
        //    });

        //    await _taskListRepeatListRepo.SeedDefaultsAsync(new List<TaskList_RepeatList>
        //    {
        //        new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 1}, // Daily
        //        new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 2}, // Weekly
        //        new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 3}, // Monthly
        //        new TaskList_RepeatList { CurrentDate=DateTime.Now, DueDate=DateTime.Parse("2026.5.26"), RepeatTagID = 4}, // Yearly
        //    });

        //    await _taskListRepo.SeedDefaultsAsync(new List<TaskList>
        //    {
        //        new TaskList { Title = "Buy groceries", Notes = "Milk, Bread, Eggs", GroupID = 2, CreatedDate = DateTime.Now, IsGoogleEvent = false, GoogleEventID = "" },
        //        new TaskList { Title = "Finish project report", Notes = "Due by end of the week", GroupID = 2, CreatedDate = DateTime.Now, IsGoogleEvent = false, GoogleEventID = "" },
        //        new TaskList { Title = "Call plumber", Notes = "Fix the kitchen sink", GroupID = 2, CreatedDate = DateTime.Now, IsGoogleEvent = false, GoogleEventID = "" },
        //        new TaskList { Title = "Plan weekend trip", Notes = "", GroupID = 2, CreatedDate = DateTime.Now, Task_RepeatListID=2, IsGoogleEvent = false, GoogleEventID = "" }
        //    });


        //    await _attachmentsRepo.SeedDefaultsAsync(new List<Attachments>
        //    {
        //        new Attachments { AttachmentName = "Report", AttachmentType = "PDF", AttachmentPath = "/files/report.pdf", TaskListId = 1 },
        //        new Attachments { AttachmentName = "Image1", AttachmentType = "Image", AttachmentPath = "/files/image1.png", TaskListId = 1 },
        //        new Attachments { AttachmentName = "Notes", AttachmentType = "Text", AttachmentPath = "/files/notes.txt", TaskListId = 2 },
        //        new Attachments { AttachmentName = "Notes2", AttachmentType = "Text", AttachmentPath = "/files/notes2.txt", TaskListId = 1 },

        //    });




        //}

    }
}
