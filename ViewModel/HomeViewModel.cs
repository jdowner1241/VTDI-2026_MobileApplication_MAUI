using Mystic_ToDo_MAUI_.Model.db.tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mystic_ToDo_MAUI_.Services.db;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Mystic_ToDo_MAUI_.ViewModel
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly DBManager<GroupList> _groupListRepo;
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatTag> _taskList_RepeatTagRepo;

        public ObservableCollection<GroupList> GroupList { get; private set; } = new();
        public ObservableCollection<TaskList> TaskList { get; private set; } = new();
        public ObservableCollection<TaskList_RepeatTag> TaskList_RepeatTag { get; private set; } = new();
        public HomeViewModel() 
        { 
            Title = "Home";
            this._groupListRepo = new DBManager<GroupList>();
            this._taskListRepo = new DBManager<TaskList>();
            this._taskList_RepeatTagRepo = new DBManager<TaskList_RepeatTag>();
        }

        [RelayCommand]
        async Task GetGroupList() 
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                if (GroupList.Any()) GroupList.Clear();

                var groupListing = await _groupListRepo.GetAllAsync();
                if (groupListing != null)
                {
                    foreach (var group in groupListing) GroupList.Add(group);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: GroupList. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: GroupList.", "OK");
            }
            finally 
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        async Task GetTaskList()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                if (TaskList.Any()) TaskList.Clear();

                var taskListing = await _taskListRepo.GetAllAsync();
                if (taskListing != null)
                {
                    foreach (var task in taskListing) TaskList.Add(task);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: TaskList. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: TaskList.", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        async Task GetTaskList_RepeatList_Tag()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                if (TaskList_RepeatTag.Any()) TaskList_RepeatTag.Clear();

                var tagListing = await _taskList_RepeatTagRepo.GetAllAsync();
                if (tagListing != null)
                {
                    foreach (var tag in tagListing) TaskList_RepeatTag.Add(tag);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: RepeatTags. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: RepeatTags.", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // This is a placehold method to load data automatically on app startup.
        public async Task LoadDataAsync() 
        {

        }


    }
}
