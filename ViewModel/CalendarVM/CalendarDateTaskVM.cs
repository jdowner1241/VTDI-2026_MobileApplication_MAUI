using CommunityToolkit.Mvvm.ComponentModel;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Text;

namespace Mystic_ToDo_MAUI_.ViewModel.CalendarVM
{
    public partial class CalendarDateTaskVM : ObservableObject
    {

        // -----------------------------
        // Repositories for database access
        // -----------------------------
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;
        private readonly DBManager<TaskList_RepeatList> _repeatListRepo;

        public TaskList TaskEntity { get; }

        // -----------------------------
        // Exposed Entity properties
        // -----------------------------
        public int TaskID => TaskEntity.ID;
        public string TaskTitle => TaskEntity.Title;
        public string Description => TaskEntity.Notes ?? "";

        [ObservableProperty]
        private bool isCompleted;

        public DateTime? DueDate => TaskRepeatInfo?.DueDate;
        public string AlarmDueDateDisplay => DueDate?.ToString("MM/dd/yyyy") ?? string.Empty;

        public bool HasAlarm => DueDate != null;
        public string AlarmIcon => HasAlarm ? "alarm.png" : string.Empty;

        public bool HasRepeat => TaskRepeatInfo != null && TaskRepeatTag != null;
        public string RepeatDisplay => TaskRepeatTag?.RepeatTagName ?? string.Empty;
        public string RepeatIcon => HasRepeat ? "event_repeat.png" : string.Empty;


        public TaskList_RepeatList? TaskRepeatInfo { get; private set; }
        public TaskList_RepeatTag? TaskRepeatTag { get; private set; }


        // -----------------------------
        // Constructor
        // -----------------------------

        public CalendarDateTaskVM(
            TaskList entity,
            DBManager<TaskList> taskRepo,
            DBManager<TaskList_RepeatTag> repeatTagRepo,
            DBManager<TaskList_RepeatList> repeatListRepo)
        {
            TaskEntity = entity;
            _taskListRepo = taskRepo;
            _repeatTagRepo = repeatTagRepo;
            _repeatListRepo = repeatListRepo;

            isCompleted = !TaskEntity.IsActive;
     
        }


        // -----------------------------
        // Custom Methods
        // -----------------------------
        public async Task LoadInfoAsync()
        {
            if (TaskEntity.Task_RepeatListID.HasValue)
            {
                TaskRepeatInfo = await _repeatListRepo.GetByIdAsync(TaskEntity.Task_RepeatListID.Value);
                OnPropertyChanged(nameof(TaskRepeatInfo));
                OnPropertyChanged(nameof(DueDate));
                OnPropertyChanged(nameof(HasAlarm));
                OnPropertyChanged(nameof(AlarmDueDateDisplay));
            }

            if(TaskRepeatInfo != null)
            {
                if (TaskRepeatInfo?.RepeatTagID != 0)
                {
                    TaskRepeatTag = await _repeatTagRepo.GetByIdAsync(TaskRepeatInfo.RepeatTagID);
                    OnPropertyChanged(nameof(TaskRepeatTag));
                    OnPropertyChanged(nameof(HasRepeat));
                    OnPropertyChanged(nameof(RepeatDisplay));
                }
            }

        }

    }
}
