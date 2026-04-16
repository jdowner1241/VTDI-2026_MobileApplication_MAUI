using CommunityToolkit.Maui;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.View;
using Mystic_ToDo_MAUI_.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.Alarm
{
    public class TaskRepo
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatList> _repeatListRepo;
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;


        public TaskRepo(HomeViewModel homeViewModel, DBManager<TaskList> taskListRepo, DBManager<TaskList_RepeatList> repeatListRepo, DBManager<TaskList_RepeatTag> repeatTagRepo)
        {
            _homeViewModel = homeViewModel;
            _taskListRepo = taskListRepo;
            _repeatListRepo = repeatListRepo;
            _repeatTagRepo = repeatTagRepo;
        }

        //public async Task<IEnumerable<TaskList>> GetAllTasksAsync(DateTime now)
        //{
        //    var allTasks = await _taskListRepo.GetAllAsync();
        //    var dueTasks = new List<TaskList>();

        //    // Filter active tasks and check their repeat info
        //    foreach (var task in allTasks.Where(t => t.IsActive)) 
        //    {
        //        // Check if the task has repeat info
        //        if (task.Task_RepeatListID != null)
        //        {
        //            var repeatInfo = await _repeatListRepo.GetByIdAsync(task.Task_RepeatListID.Value);

        //            if (repeatInfo != null)
        //            {
        //                if (repeatInfo.DueDate != null && repeatInfo.DueDate <= now)
        //                {
        //                    if(repeatInfo.RepeatTagID != null)
        //                    {
        //                        var repeatTag = await _repeatTagRepo.GetByIdAsync(repeatInfo.RepeatTagID);

        //                        if (repeatTag != null)
        //                        {
        //                            repeatInfo.RepeatTag = repeatTag;
        //                            task.Task_RepeatList = repeatInfo;
        //                        }
        //                        else
        //                        {
        //                            repeatInfo.RepeatTag = null;
        //                            task.Task_RepeatList = repeatInfo;
        //                        }
        //                    }

        //                    dueTasks.Add(task);
        //                }
        //            }
        //        }
        //    }

        //   return dueTasks;
        //}

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

            // Trigger UpdateTaskAsync in HomeViewModel to refresh the UI and update active logic
            await _homeViewModel.ToggleTaskCompletionAsync(task.ID, isCompleted);

            // Save changes to the Task entity
            await _taskListRepo.UpdateAsync(task);

            // If the task has repeat info, update that too
            if (task.Task_RepeatList != null)
            {
                await _repeatListRepo.UpdateAsync(task.Task_RepeatList);
            }
        }

        public async Task UpdateTaskRepeatAsync(TaskList task)
        {
            if (task.Task_RepeatList == null) return;

            // Update task modified date to now then update RepeatList and TaskList in the database
            task.ModifiedDate = DateTime.Now;
            await _taskListRepo.UpdateAsync(task);
            await _repeatListRepo.UpdateAsync(task.Task_RepeatList);
        }

    }
}
