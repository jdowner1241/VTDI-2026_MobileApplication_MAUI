
using CommunityToolkit.Mvvm.ComponentModel;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Text;


namespace Mystic_ToDo_MAUI_.ViewModel.HomeVM
{
    public partial class TaskListVM : ObservableObject
    {

        // +++++++++++++++++++++
        // Task table : Objects and Properties 
        // +++++++++++++++++++++
        private readonly DBManager<TaskList_RepeatList> _repeatListRepo;
        private readonly DBManager<Attachments> _attachmentsRepo;

        public TaskList TaskEntity { get; }
        private readonly IEnumerable<TaskList> _alltasks;
        private readonly IEnumerable<TaskList> _grouptasks;


        [ObservableProperty]
        private bool isSelected;


        // Exposed Entity properties
        public int TaskID => TaskEntity.ID;
        public string TaskTitle => TaskEntity.Title;
        public DateTime TaskCreatedDate => TaskEntity.CreatedDate;
        public DateTime? TaskModifiedDate => TaskEntity.ModifiedDate;
        public bool IsActive => TaskEntity.IsActive;
        public bool IsCompletedDisplay => !IsActive;
        public string? Notes => TaskEntity?.Notes;
        public List<Attachments>? Raw_Attachments => TaskEntity?.Attachments;

        [ObservableProperty]
        public ObservableCollection<Attachments>? attachments;

        public TaskList_RepeatList? TaskRepeatInfo { get; private set; }


        public DateTime? DueDate => TaskRepeatInfo?.DueDate;
        public string DueDateDisplay => DueDate?.ToString("dd/MM/yyyy") ?? string.Empty;
        public DateTime? CurrentDate => TaskRepeatInfo?.CurrentDate;
        public int? RepeatInterval => TaskRepeatInfo?.HowOften;
        public string? RepeatFrequency => TaskRepeatInfo?.RepeatTag?.RepeatTagName;

        public bool HasAlarm => DueDate != null ? true : false;
        public ImageSource? AlarmDisplay => HasAlarm == true ? "alarm.png" : null;
        //public ImageSource? AlarmDisplay => "alarm_on.png";

        public bool HasRepeat => RepeatInterval != null && !string.IsNullOrEmpty(RepeatFrequency) ? true : false;
        public ImageSource? RepeatDisplayIcon => HasRepeat == true ? "event_repeat.png" : null;

        public string RepeatDisplay =>  RepeatInterval.HasValue && !string.IsNullOrEmpty(RepeatFrequency)
                                        ? $"Repeat: {RepeatInterval}, Every: {RepeatFrequency} Times"
                                        : string.Empty;


        // Control Properties 





        // +++++++++++++++++++++
        // Constructor
        // +++++++++++++++++++++
        public TaskListVM
            (
            TaskList taskEntity, 
            IEnumerable<TaskList> alltasks, 
            IEnumerable<TaskList> grouptasks,
            DBManager<TaskList_RepeatList> repeatListRepo,
            DBManager<Attachments> attachmentsRepo
            )
        {
            TaskEntity = taskEntity;
            _alltasks = alltasks;
            _grouptasks = grouptasks;
            _repeatListRepo = repeatListRepo;
            _attachmentsRepo = attachmentsRepo;

            attachments = new ObservableCollection<Attachments>(taskEntity.Attachments ?? new List<Attachments>());
        }


        public async Task LoadRepeatInfoAsync()
        {
            if (TaskEntity.Task_RepeatListID.HasValue)
            {
                TaskRepeatInfo = await _repeatListRepo.GetByIdAsync(TaskEntity.Task_RepeatListID.Value);
                OnPropertyChanged(nameof(TaskRepeatInfo));
                OnPropertyChanged(nameof(DueDate));
                OnPropertyChanged(nameof(HasAlarm));
                OnPropertyChanged(nameof(HasRepeat));
                OnPropertyChanged(nameof(DueDateDisplay));
                OnPropertyChanged(nameof(RepeatInterval));
                OnPropertyChanged(nameof(RepeatFrequency));
                OnPropertyChanged(nameof(RepeatDisplay));
                OnPropertyChanged(nameof(AlarmDisplay));
                OnPropertyChanged(nameof(RepeatDisplayIcon));
            }
        }


    }
}
