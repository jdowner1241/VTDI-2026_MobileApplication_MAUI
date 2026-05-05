
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly HomeViewModel _homeViewModel;
        private readonly DBManager<TaskList_RepeatList> _repeatListRepo;
        private readonly DBManager<TaskList_RepeatTag> _repeatTagRepo;
        private readonly DBManager<Attachments> _attachmentsRepo;
       // private readonly DBManager<TaskList> _taskListRepo;

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
        [ObservableProperty]
        private bool isCompletedDisplay;

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
            HomeViewModel homeViewModel,
            TaskList taskEntity, 
            IEnumerable<TaskList> alltasks, 
            IEnumerable<TaskList> grouptasks,
            DBManager<TaskList_RepeatList> repeatListRepo,
            DBManager<TaskList_RepeatTag> repeatTagRepo,
            DBManager<Attachments> attachmentsRepo
            )
        {
            TaskEntity = taskEntity;
            _homeViewModel = homeViewModel;
            _alltasks = alltasks;
            _grouptasks = grouptasks;
            _repeatListRepo = repeatListRepo;
            _repeatTagRepo = repeatTagRepo;
            _attachmentsRepo = attachmentsRepo;

            attachments = new ObservableCollection<Attachments>(taskEntity.Attachments ?? new List<Attachments>());

            // Initialize the generated IsCompleted property backing field to reflect the entity state
            // completed == !IsActive
            isCompletedDisplay = !TaskEntity.IsActive;

            //ToggleCompletionCommand = new RelayCommand<bool>(async (newValue) =>
            //{
            //    await _homeViewModel.ToggleTaskCompletionAsync(TaskEntity.ID, newValue);
            //});


        }

        //[RelayCommand]
        //public async Task ToggleCompletion(bool newValue)
        //{
        //    await _homeViewModel.ToggleTaskCompletionAsync(TaskEntity.ID, newValue);

        //    // Notify UI after persistence
        //    OnPropertyChanged(nameof(IsActive));
        //    OnPropertyChanged(nameof(IsCompletedDisplay));

        //}

        // This partial method is generated hook called when the [ObservableProperty] 'isCompleted' changes.
        // It updates the underlying entity, persists the change and notifies dependent properties.
        partial void OnIsCompletedDisplayChanged(bool value)
        {
            // value == true means the task is completed, so IsActive should be false
            TaskEntity.IsActive = !value;

            // Persist the change (fire-and-forget; the VM method handles async persistence)
            _ = _homeViewModel.ToggleTaskCompletionAsync(TaskEntity.ID, value);

            // Notify dependent properties
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(IsCompletedDisplay));
        }

        //public bool IsCompletedDisplay
        //{
        //    get => !TaskEntity.IsActive;
        //    set
        //    {
        //        if (value != TaskEntity.IsActive) // value true means completed
        //        {
        //            TaskEntity.IsActive = !value; // Set IsActive to false if completed, true if not completed

        //            // Notify property changes
        //            OnPropertyChanged(nameof(IsCompletedDisplay));
        //            OnPropertyChanged(nameof(IsActive));


        //            // Run async toggle
        //            _ = _homeViewModel.ToggleTaskCompletionAsync(TaskEntity.ID, value);
        //        }
        //    }
        //}




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
