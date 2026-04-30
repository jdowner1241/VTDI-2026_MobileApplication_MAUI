using CommunityToolkit.Maui;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.View;
using Mystic_ToDo_MAUI_.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.Alarm
{
    public class TaskRepo
    {
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatList> _repeatListRepo;
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;
        private readonly DBManager<GroupList> _groupListRepo;

        // events
        public event Action? TaskUpdated; 
  
        public TaskRepo(
            DBManager<TaskList> taskListRepo, 
            DBManager<TaskList_RepeatList> repeatListRepo,
            DBManager<TaskList_RepeatTag> repeatTagRepo,
            DBManager<GroupList> groupListRepo)
        {   
            _taskListRepo = taskListRepo;
            _repeatListRepo = repeatListRepo;
            _repeatTagRepo = repeatTagRepo;
            _groupListRepo = groupListRepo;
          
        }


        public async Task<IEnumerable<TaskList>> GetAllTasksAsync(DateTime now)
        {
            var allTasks = await _taskListRepo.GetAllAsync();
            var dueTasks = new List<TaskList>();

            // Only consider active tasks
            foreach (var task in allTasks.Where(t => t.IsActive))
            {
                // Check if this task has repeat info attached
                if (task.Task_RepeatListID != null)
                {
                    var repeatInfo = await _repeatListRepo.GetByIdAsync(task.Task_RepeatListID.Value);

                    // If repeat info exists and the due date has passed
                    if (repeatInfo != null && repeatInfo.DueDate != null && repeatInfo.DueDate <= now)
                    {
                        var repeatTag = await _repeatTagRepo.GetByIdAsync(repeatInfo.RepeatTagID);

                        if (repeatInfo.RepeatTagID != 0)
                        {
                            repeatInfo.RepeatTag = repeatTag;
                        }

                        // Update the task's repeat info with the attached tag
                        task.Task_RepeatList = repeatInfo;
                        // Add this task to the list of due tasks
                        dueTasks.Add(task);
                    }
                }
            }

            return dueTasks;
        }



        public TaskList_RepeatList? GetNextAlarm(TaskList_RepeatList repeatInfo)
        {
            // Guard clause: if no due date or no repeat tag, we cannot calculate the next alarm
            if (repeatInfo?.DueDate == null || repeatInfo.RepeatTag == null || repeatInfo.RepeatTagID == 0)
                return null;

            // Calculate the new due date based on the repeat frequency
            DateTime? newDueDate = repeatInfo.RepeatTag.RepeatTagName switch
            {
                "Daily" => repeatInfo.DueDate.AddDays(1),
                "Weekly" => repeatInfo.DueDate.AddDays(7),
                "Monthly" => repeatInfo.DueDate.AddMonths(1),
                "Yearly" => repeatInfo.DueDate.AddYears(1),
                _ => null
            };

            // If we couldn’t calculate a new date, return null
            if (newDueDate == null)
                return null;

            // Build a new repeat info object with updated dates
            var newRepeatInfo = new TaskList_RepeatList
            {
                CurrentDate = repeatInfo.DueDate,
                DueDate = newDueDate ?? repeatInfo.DueDate,
                RepeatTagID = repeatInfo.RepeatTagID,
                RepeatTag = repeatInfo.RepeatTag
            };

            return newRepeatInfo;
        }

        public async Task UpdateTaskNonRepeatAsync(TaskList task, bool isCompleted)
        {
            if (task == null) return;

            Debug.WriteLine($"UpdateTask Non-Repeat");
            Debug.WriteLine($"Before Change: Task : {task.Title} isCompleted Status: {isCompleted}");

            // Trigger UpdateTaskAsync in HomeViewModel to refresh the UI and update active logic, also save task
            await SetTaskCompletionAsync(task, isCompleted);
            

            // If the task has repeat info, update that too
            var editedTask = await _taskListRepo.GetByIdAsync(task.ID);
            if (editedTask.Task_RepeatList != null)
            {
                await _repeatListRepo.UpdateAsync(editedTask.Task_RepeatList);

                Debug.WriteLine($"After Change: Task : {editedTask.Title} isCompleted Status: {editedTask.IsActive}");
            }
        }

        public async Task UpdateTaskRepeatAsync(TaskList task)
        {
            if (task.Task_RepeatList == null) return;

            Debug.WriteLine($"UpdateTask Repeat");
            Debug.WriteLine($"Task : {task.Title}");

            // Update task modified date to now then update RepeatList and TaskList in the database
            task.ModifiedDate = DateTime.Now;
            await _taskListRepo.UpdateAsync(task);
            await _repeatListRepo.UpdateAsync(task.Task_RepeatList);
        }

        public async Task SetTaskCompletionAsync(TaskList task, bool isCompleted)
        {
            try
            {
                var GroupList = await _groupListRepo.GetAllAsync();
                if (GroupList == null) return;
                var editedTask = await _taskListRepo.GetByIdAsync(task.ID);

                if (editedTask == null) return;

                // Flip the Complete status 
                editedTask.IsActive = !isCompleted;

                // if inactive, move to Completed group
                if (!editedTask.IsActive)
                {

                    // Task is being marked complete → move to Completed folder
                    var completedFolder = GroupList.FirstOrDefault(g => g.GroupName == "Completed");

                    // Only store original group if it's not Completed
                    if (!editedTask.OrginalGroupID.HasValue && editedTask.GroupID != completedFolder?.ID)
                    {
                        editedTask.OrginalGroupID = editedTask.GroupID;
                    }

                    if (completedFolder != null)
                        editedTask.GroupID = completedFolder.ID;

                }
                else
                {
                    // Task is being reactivated → restore original group
                    if (editedTask.OrginalGroupID.HasValue)
                    {
                        editedTask.GroupID = editedTask.OrginalGroupID.Value;
                        editedTask.OrginalGroupID = null; // clear after restoring
                    }
                    else
                    {
                        // Fallback: send to Default folder if no original group stored
                        var defaultFolder = GroupList.FirstOrDefault(g => g.GroupName == "Default");
                        if (defaultFolder != null)
                        {
                            editedTask.GroupID = defaultFolder.ID;
                        }
                    }
                }

                await _taskListRepo.UpdateAsync(editedTask);

                TaskUpdated?.Invoke();// Refresh task list to reflect changes

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error togging task completion: {ex.Message}");
            }
        }

    }
}
