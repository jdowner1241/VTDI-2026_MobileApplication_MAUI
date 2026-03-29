
using CommunityToolkit.Mvvm.ComponentModel;
using Mystic_ToDo_MAUI_.Model.db.tables;
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
        //public ObservableCollection<Attachments>? Attachments
        //{
        //    get
        //    {
        //        if (_attachments == null && Raw_Attachments != null)
        //        {
        //            _attachments = new ObservableCollection<Attachments>(Raw_Attachments);
        //        }
        //        return _attachments;
        //    }
        //    set
        //    {
        //        _attachments = value;
        //    }
        //}

        public TaskList_RepeatList? TaskRepeatInfo => TaskEntity?.TaskList_RepeatList;

        public DateTime? DueDate => TaskRepeatInfo?.DueDate;
        public string DueDateDisplay => DueDate?.ToString("dd/MM/yyyy") ?? string.Empty;
        public DateTime? CurrentDate => TaskRepeatInfo?.CurrentDate;
        public int? RepeatInterval => TaskRepeatInfo?.HowOften;
        public string? RepeatFrequency => TaskRepeatInfo?.RepeatTag?.RepeatTagName;

        public bool HasAlarm => RepeatInterval != null ? true : false;
        public ImageSource? AlarmDisplay => HasAlarm == true ? "alarm_on.png" : null;

        public bool HasRepeat => RepeatInterval != null && !string.IsNullOrEmpty(RepeatFrequency) ? true : false;
        public ImageSource? RepeatDisplayIcon => HasRepeat == true ? "event_repeat.png" : null;

        public string RepeatDisplay =>  RepeatInterval.HasValue && !string.IsNullOrEmpty(RepeatFrequency)
                                        ? $"Repeat: {RepeatInterval}, Every: {RepeatFrequency} Times"
                                        : string.Empty;


        // Control Properties 





        // +++++++++++++++++++++
        // Constructor
        // +++++++++++++++++++++
        public TaskListVM(TaskList taskEntity, IEnumerable<TaskList> alltasks, IEnumerable<TaskList> grouptasks)
        {
            TaskEntity = taskEntity;
            _alltasks = alltasks;
            _grouptasks = grouptasks;

            attachments = new ObservableCollection<Attachments>(taskEntity.Attachments ?? new List<Attachments>());
        }

    }
}
