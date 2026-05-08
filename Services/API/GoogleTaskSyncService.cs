using Google.Apis.Calendar.v3.Data;
using Microsoft.VisualBasic;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.Alarm;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.ViewModel.CalendarVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Mystic_ToDo_MAUI_.Services.API
{
    public class GoogleTaskSyncService
    {

        // -----------------------------
        // Repositories for database access
        // -----------------------------
        private readonly DBInitializer _dbInitializer;
        private readonly DBManager<GroupList> _groupRepo;

        private readonly DBManager<TaskList> _taskRepo;
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;
        private readonly DBManager<TaskList_RepeatList> _repeatInfoRepo;


        // -----------------------------
        // Constructor 
        // -----------------------------
        public GoogleTaskSyncService(
           DBManager<TaskList> taskRepo,
           DBManager<GroupList> groupRepo,
           DBManager<TaskList_RepeatTag> repeatTag,
           DBManager<TaskList_RepeatList> repeatInfo,
           DBInitializer dbInitializer
           )
        {
            _dbInitializer = dbInitializer;
            _taskRepo = taskRepo;
            _groupRepo = groupRepo;
            _repeatTagRepo = repeatTag;
            _repeatInfoRepo = repeatInfo;
              
        }

        // -----------------------------
        // Custom Methods 
        // -----------------------------
        public async Task SaveGoogleEventAsync(Event googleEvent)
        {
            // Ensure the "Google Events" group exists and retrieving its ID
            var groups = await _groupRepo.GetAllAsync();

            var googleGroup = groups
                .FirstOrDefault(g => g.GroupName == "Google Events");

            if (googleGroup == null)
            {
                await _dbInitializer.DBInitializerAsync();

                var refreshedGroups = await _groupRepo.GetAllAsync();

                googleGroup = refreshedGroups
                    .FirstOrDefault(g => g.GroupName == "Google Events");
            }

            if (googleGroup != null) 
            {
                // RepeatTag properties
                var repeatTags = await _repeatTagRepo.GetAllAsync();

                var eventRepeatTag = repeatTags
                    .FirstOrDefault(rt => rt.RepeatTagName == "NotSet");

                // RepeatInfo properties
                TaskList_RepeatList? eventRepeatInfo = null;

                if (googleEvent.End != null)
                {
                    // Determine the due date based on the event's end time
                    DateTime dueDate;
                    if (googleEvent.End?.DateTimeDateTimeOffset != null)
                    {
                        dueDate = googleEvent.End.DateTimeDateTimeOffset.Value
                            .LocalDateTime;
                    }
                    else if (!string.IsNullOrEmpty(googleEvent.End?.Date))
                    {
                        dueDate = DateTime.Parse(googleEvent.End.Date)
                            .AddDays(1)
                            .AddSeconds(-1);
                    }
                    else
                    {
                        Debug.WriteLine($"Unable to determine due date for event: {googleEvent.Summary}");
                        return;
                    }

                    eventRepeatInfo = new TaskList_RepeatList
                    {
                        CurrentDate = DateTime.Now,
                        DueDate = dueDate,
                        RepeatTagID = eventRepeatTag?.ID ?? 0
                    };

                    await _repeatInfoRepo.InsertAsync(eventRepeatInfo);
                }
                // Get the ID of the inserted repeat info to link it to the task
                var repeatInfoId = eventRepeatInfo?.ID;
                int? Task_RepeatListID = repeatInfoId.HasValue ? (int?)repeatInfoId.Value : null;


                // Check if the event already exists in the database
                var tasks = await _taskRepo.GetAllAsync();

                var existingTask = tasks.FirstOrDefault(t =>
                    t.GoogleEventID == googleEvent.Id);

                if (existingTask != null)
                    return;

                // TaskList properties
                var task = new TaskList
                {
                    Title = googleEvent.Summary,
                    Notes = googleEvent.Description,
                    GroupID = googleGroup.ID,
                    CreatedDate = googleEvent.CreatedDateTimeOffset?.DateTime ?? DateTime.Now,
                    IsActive = true,
                    GoogleEventID = googleEvent.Id,
                    IsGoogleEvent = true,
                    Task_RepeatListID = Task_RepeatListID,
                };
                await _taskRepo.InsertAsync(task);

                // Get the ID of the inserted task to link attachments


                // Update the task with Attachments if they exist
                if (googleEvent.Attachments != null && googleEvent.Attachments.Count > 0)
                {
                    var insertedTask = await _taskRepo.GetByIdAsync(task.ID);
                    if (insertedTask != null)
                    {
                        insertedTask.Attachments = googleEvent.Attachments?.Select(a => new Attachments
                        {
                            AttachmentName = a.Title,
                            AttachmentPath = a.FileUrl,
                            AttachmentType = a.MimeType
                        }).ToList();


                        await _taskRepo.UpdateAsync(insertedTask);
                    }
                }

            }
        }

        // -----------------------------
        // Startup  Methods 
        // -----------------------------
    }
}
