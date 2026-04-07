
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
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;
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
        public string AlarmDueDateDisplay => DueDate?.ToString("MM/dd/yyyy") ?? string.Empty;
        public DateTime? CurrentDate => TaskRepeatInfo?.CurrentDate;
        //public int? RepeatInterval => TaskRepeatInfo?.HowOften;
        //public string? RepeatFrequency => TaskRepeatInfo?.RepeatTag?.RepeatTagName;


        public int? TaskList_RepeatTagID => TaskRepeatInfo?.RepeatTagID;
        public TaskList_RepeatTag? TaskList_RepeatTag { get; private set; }

        public bool HasAlarm => DueDate != null ? true : false;
        public ImageSource? AlarmDisplayIcon => HasAlarm == true ? "alarm.png" : null;




        public bool HasRepeat => HasAlarm &&
                                 TaskList_RepeatTag != null && 
                                 TaskList_RepeatTag.ID > 0;
        public ImageSource? RepeatDisplayIcon => HasRepeat == true ? "event_repeat.png" : null;

        public string RepeatDisplay => TaskList_RepeatTag == null || TaskList_RepeatTag.ID == 0
                                       ? string.Empty
                                       : $"Repeat: {TaskList_RepeatTag.RepeatTagName}";


        // +++++++++++++++++++++
        // Constructor
        // +++++++++++++++++++++
        public TaskListVM
            (
            TaskList taskEntity, 
            IEnumerable<TaskList> alltasks, 
            IEnumerable<TaskList> grouptasks,
            DBManager<TaskList_RepeatList> repeatListRepo,
            DBManager<TaskList_RepeatTag> repeatTagRepo,
            DBManager<Attachments> attachmentsRepo
            )
        {
            TaskEntity = taskEntity;
            _alltasks = alltasks;
            _grouptasks = grouptasks;
            _repeatListRepo = repeatListRepo;
            _repeatTagRepo = repeatTagRepo;
            _attachmentsRepo = attachmentsRepo;

            attachments = new ObservableCollection<Attachments>(taskEntity.Attachments ?? new List<Attachments>());
        }


        public async Task LoadInfoAsync()
        {
            if (TaskEntity.Task_RepeatListID.HasValue)
            {
                TaskRepeatInfo = await _repeatListRepo.GetByIdAsync(TaskEntity.Task_RepeatListID.Value);
                OnPropertyChanged(nameof(TaskRepeatInfo));
                OnPropertyChanged(nameof(DueDate));
                OnPropertyChanged(nameof(HasAlarm));
                OnPropertyChanged(nameof(HasRepeat));
                OnPropertyChanged(nameof(AlarmDueDateDisplay));
                OnPropertyChanged(nameof(TaskList_RepeatTagID));
                OnPropertyChanged(nameof(RepeatDisplay));
                OnPropertyChanged(nameof(AlarmDisplayIcon));
                OnPropertyChanged(nameof(RepeatDisplayIcon));
            }

            //if (TaskEntity.Attachments != null)
            //{
            //    attachmentsList = await _attachmentsRepo.GetByIdAsync(TaskEntity.Attachments.Select(a => a.ID).ToList());
            //    Attachments = new ObservableCollection<Attachments>(attachmentsList);
            //    OnPropertyChanged(nameof(Attachments));
            //}

            if (TaskList_RepeatTagID.HasValue == true)
            {
                TaskList_RepeatTag = await _repeatTagRepo.GetByIdAsync(TaskList_RepeatTagID.Value);

                OnPropertyChanged(nameof(TaskList_RepeatTag));
                OnPropertyChanged(nameof(TaskList_RepeatTagID));
                OnPropertyChanged(nameof(HasRepeat));
                OnPropertyChanged(nameof(RepeatDisplay));
                OnPropertyChanged(nameof(RepeatDisplayIcon));
            }
        }


    }
}
