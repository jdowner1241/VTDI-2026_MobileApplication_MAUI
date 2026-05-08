using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Calendar.v3;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Services;
using Mystic_ToDo_MAUI_.Services.Alarm;
using Mystic_ToDo_MAUI_.Services.API;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.ViewModel.CalendarVM;
using Mystic_ToDo_MAUI_.ViewModel.HomeVM;
using Plugin.Maui.Calendar.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;


namespace Mystic_ToDo_MAUI_.ViewModel
{
    public partial class CalendarViewModel : BaseViewModel
    {
        // -----------------------------
        // Repositories for database access
        // -----------------------------
        private readonly AppState _appState;
        private readonly DBManager<GroupList> _groupListRepo;
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatTag> _taskList_RepeatTagRepo;
        private readonly DBManager<TaskList_RepeatList> _taskList_RepeatListRepo;

        private readonly GoogleCalendarAPIHelper _googleHelper;
        private readonly GoogleTaskSyncService _syncService;

        private CancellationTokenSource _cts;


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
        [ObservableProperty]
        bool googleSyncEnabled = false;

        private SemaphoreSlim _syncLock = new(1, 1);

        // -----------------------------
        // Constructor
        // -----------------------------

        public CalendarViewModel
            (
            AppState appState,
            DBManager<TaskList> taskRepo,
            DBManager<GroupList> groupListRepo,
            DBManager<TaskList_RepeatTag> repeatTagRepo,
            DBManager<TaskList_RepeatList> taskList_RepeatListRepo,
            GoogleCalendarAPIHelper googleHelper,
            GoogleTaskSyncService syncService
            )
        {
            Title = "Calendar";
            _appState = appState;
            _groupListRepo = groupListRepo;
            _taskListRepo = taskRepo;
            _taskList_RepeatTagRepo = repeatTagRepo;
            _taskList_RepeatListRepo = taskList_RepeatListRepo;

            _googleHelper = googleHelper;
            _syncService = syncService;

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


        [RelayCommand]
        public async Task SyncGoogleCalendar()
        {
            if (GoogleSyncEnabled) return;

            try
            {
                GoogleSyncEnabled = true;

                await LoadGoogleEventsAsync();
                //var service = await GoogleCalendarAPIHelper.GetCalendarServiceAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error syncing Google Calendar: {ex.Message}");
            }
            finally
            {
                GoogleSyncEnabled = false;
            }
        }

        // Google API integration for Fetching Calendar Events
        public async Task LoadGoogleEventsAsync()
        {
      
            await _syncLock.WaitAsync();
            _cts = new CancellationTokenSource();

            try
            {

                var service = await _googleHelper.GetCalendarServiceAsync();

                var request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.TimeMax = DateTime.Now.AddMonths(1);
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                var eventsResp = await request.ExecuteAsync();

                if (eventsResp.Items != null && eventsResp.Items.Count > 0)
                {
                    foreach (var ev in eventsResp.Items)
                    {
                        // Sync Events from Google Calendar to local database as Tasks with a special "Google Events" group
                        await _syncService.SaveGoogleEventAsync(ev);
                    }


                    // Reload tasks to include any new Google Events
                    await Task.Delay(1000);
                    await LoadGroupTasksAsync();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Sync cancelled.");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to Sync Google Events.", "OK");
            }
            finally
            {
                _syncLock.Release();
            }


        }


        public void CancelSync()
        {
            _cts?.Cancel();
        }


        // -----------------------------
        // Async Method for Loading Data Automatically 
        // -----------------------------
        public async Task LoadDataAsync()
        {
            await _appState.WaitUntilReady();

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

