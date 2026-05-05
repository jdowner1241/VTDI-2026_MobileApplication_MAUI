using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services.Alarm;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.ViewModel.CalendarVM;
using Mystic_ToDo_MAUI_.ViewModel.HomeVM;
using Plugin.Maui.Calendar.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Linq;


namespace Mystic_ToDo_MAUI_.ViewModel
{
    public partial class CalendarViewModel : BaseViewModel
    {
        // -----------------------------
        // Repositories for database access
        // -----------------------------
        private readonly DBInitializer _dbInitializer;
        private readonly DBManager<GroupList> _groupListRepo;
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatTag> _taskList_RepeatTagRepo;
        private readonly DBManager<TaskList_RepeatList> _taskList_RepeatListRepo;


        // -----------------------------
        // properties and supporting structures
        // -----------------------------
        [ObservableProperty]
        private DateTime selectedDate;
        partial void OnSelectedDateChanged(DateTime value)
        {
            LoadTasksForDate(value);
        }


        [ObservableProperty]
        public ObservableCollection<CalendarDateTaskVM> groupTasks = new();

        [ObservableProperty]
        public ObservableCollection<CalendarDateTaskVM> tasksForSelectedDate = new();

        [ObservableProperty]
        public ObservableCollection<GroupList> allGroups = new();

        [ObservableProperty]
        public GroupList selectedGroup;

        partial void OnSelectedGroupChanged(GroupList value)
        {
            LoadGroupTasksAsync();
        }

        [ObservableProperty]
        public EventCollection events = new();


        // Flags for Async loading state
        [ObservableProperty]
        bool loading_SelectedDateTasks = false;
        [ObservableProperty]
        bool loading_GroupList = false;

        // -----------------------------
        // Constructor
        // -----------------------------

        public CalendarViewModel
            (DBInitializer dbInitializer,
            DBManager<TaskList> taskRepo,
            DBManager<GroupList> groupListRepo,
            DBManager<TaskList_RepeatTag> repeatTagRepo,
            DBManager<TaskList_RepeatList> taskList_RepeatListRepo
            )
        {
            _dbInitializer = dbInitializer;
            _groupListRepo = groupListRepo;
            _taskListRepo = taskRepo;
            _taskList_RepeatTagRepo = repeatTagRepo;
            _taskList_RepeatListRepo = taskList_RepeatListRepo;

            // Default to today
            SelectedDate = DateTime.Today;


        }

        // -----------------------------
        // Custom Methods
        // -----------------------------
        // Load all tasks for the selected group into Events
        private async Task LoadGroupTasksAsync()
        {
            Events.Clear();
            GroupTasks.Clear();

            var tasks = await _taskListRepo.GetAllAsync();

            foreach (var task in tasks)
            {
                if (SelectedGroup != null && task.GroupID != SelectedGroup.ID)
                    continue;

                var vm = new CalendarDateTaskVM(task, _taskListRepo, _taskList_RepeatTagRepo, _taskList_RepeatListRepo);
                await vm.LoadInfoAsync();
                GroupTasks.Add(vm);

                var eventDate = vm.DueDate?.Date;
                if (eventDate == null)
                    continue; // skip tasks without a due date

                if (!Events.ContainsKey(eventDate.Value))
                    Events[eventDate.Value] = new ArrayList();

                ((IList)Events[eventDate.Value]).Add(vm);
            }
        }


        private async Task LoadTasksForDate(DateTime date)
        {
            TasksForSelectedDate.Clear();

            foreach (var vm in GroupTasks)
            {
                var eventDate = vm.DueDate?.Date;
                if (eventDate == null) continue;

                if (eventDate == date.Date)
                {
                    TasksForSelectedDate.Add(vm);
                }
            }
        }



        //async Task LoadTasksForDate(DateTime date)
        //{
        //    if (Loading_SelectedDateTasks) return;

        //    try
        //    {
        //        Loading_SelectedDateTasks = true;

        //        TasksForSelectedDate.Clear();
        //        Events.Clear();

        //        var tasks = await _taskListRepo.GetAllAsync();

        //        foreach (var task in tasks)
        //        {
        //            // Skip tasks that don't match the selected group (if a group is selected)
        //            if (SelectedGroup != null && task.GroupID != SelectedGroup.ID)
        //                continue;


        //            if (task.CreatedDate.Date == date.Date ||
        //                (task.ModifiedDate.HasValue && task.ModifiedDate.Value.Date == date.Date))
        //            {
        //                var vm = new CalendarDateTaskVM(task, _taskListRepo, _taskList_RepeatTagRepo, _taskList_RepeatListRepo);
        //                await vm.LoadInfoAsync();
        //                TasksForSelectedDate.Add(vm);

        //                // Add to EventCollection using DueDate or CreatedDate
        //                var eventDate = vm.DueDate?.Date ?? task.CreatedDate.Date;

        //                if (!Events.ContainsKey(eventDate))
        //                    Events[eventDate] = new ArrayList();

        //                // EventCollection stores values as non-generic ICollection.
        //                // Cast to IList to call Add(object).
        //                ((IList)Events[eventDate]).Add(vm); // store the VM directly
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Unable to get Task for selected Date. Error: {ex.Message}");
        //        await Shell.Current.DisplayAlertAsync("Error", $"Unable to get Task for selected Date.", "OK");
        //    }
        //    finally 
        //    {
        //        Loading_SelectedDateTasks = false;
        //    }

        //}

        [RelayCommand]
        async Task GetGroupList()
        {
            if (Loading_GroupList) return;

            try
            {
                Loading_GroupList = true;
                AllGroups.Clear();

                var groupListing = await _groupListRepo.GetAllAsync();

                // Populate Groups
                foreach (var group in groupListing)
                {
                    if (group.GroupName == "Root") continue;

                    AllGroups.Add(group);
                }

                // Refresh the collection to update the UI
                await RefreshTasks();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: GroupList. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: GroupList.", "OK");
            }
            finally
            {
                Loading_GroupList = false;
            }
        }



        // RelayCommand to manually refresh
        [RelayCommand]
        async Task RefreshTasks()
        {
            await LoadGroupTasksAsync();
            await LoadTasksForDate(SelectedDate);
        }


        // -----------------------------
        // Async Method for Loading Data Automatically 
        // -----------------------------
        public async Task LoadDataAsync()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");

            if (!File.Exists(dbPath))
            {
                Debug.WriteLine("Database not ready yet. Creating schema...");
            }

            // Always ensure database is created and schema is up to date before loading data
            await _dbInitializer.DBInitializerAsync();

            // Now safe to call repo methods
            await GetGroupList();

            // Default selected group to "All Tasks" if it exists
            if (SelectedGroup == null && AllGroups != null)
            {
                SelectedGroup = AllGroups.FirstOrDefault(g => g.GroupName == "Default");
            }

            // Show Task data for the selected date after ensuring DB is ready
            await RefreshTasks();
        }


    }

} // End of CalendarViewModel.cs

