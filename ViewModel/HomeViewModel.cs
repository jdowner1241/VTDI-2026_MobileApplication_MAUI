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
using Mystic_ToDo_MAUI_.ViewModel.GroupListVM;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;

namespace Mystic_ToDo_MAUI_.ViewModel
{
    public partial class HomeViewModel : BaseViewModel
    {

        // Repositories for database access
        private readonly DBManager<GroupList> _groupListRepo;
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatTag> _taskList_RepeatTagRepo;

        // ObservableCollections for data binding
        public ObservableCollection<GroupListViewModel> GroupList { get; private set; } = new();
        public ObservableCollection<TaskList> TaskList { get; private set; } = new();
        public ObservableCollection<TaskList_RepeatTag> TaskList_RepeatTag { get; private set; } = new();


        // Supporting methods and properties
        public record MoveGroupArgs(GroupListViewModel Group, GroupListViewModel NewParent);
        
        [ObservableProperty]
        private GroupListViewModel selectedGroup;


        // Constructor
        public HomeViewModel() 
        { 
            Title = "Home";
            this._groupListRepo = new DBManager<GroupList>();
            this._taskListRepo = new DBManager<TaskList>();
            this._taskList_RepeatTagRepo = new DBManager<TaskList_RepeatTag>();
            this.selectedGroup = SelectedGroup;

        }

        // <GroupListing Commands and Task>
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
                    foreach (var group in groupListing)
                    { 
                        var vm = new GroupListViewModel(group, groupListing);
                        GroupList.Add(vm); 
                    } 
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
        void SelectGroup(GroupListViewModel selectedGroup) 
        {
            if (selectedGroup == null) return;
            Debug.WriteLine($"Selected group: {selectedGroup.GroupName}");
            SelectedGroup = selectedGroup;


        }

        [RelayCommand]
        void TapGroup(GroupListViewModel tappedGroup)
        {
            Debug.WriteLine($"Tapped group: {tappedGroup.GroupName}");
            SelectedGroup = tappedGroup;
        }

        [RelayCommand]
        void LongPressGroup(GroupListViewModel group)
        {
            Debug.WriteLine($"Long pressed group: {group.GroupName}");
        }


        [RelayCommand]
        async Task AddGroup() {
            try
            {
                // Prompt user for Group name
                var name = await Shell.Current.DisplayPromptAsync(
                    "New Group", 
                    "Enter group name:", 
                    "OK", 
                    "Cancel", 
                    "Group Name", 50);

                if (string.IsNullOrWhiteSpace(name))
                {
                    await Shell.Current.DisplayAlertAsync("Invalid Name", "Group name cannot be empty.", "OK");
                    return; // user cancelled or entered empty name
                }

                var newGroup = new GroupList
                {
                    GroupName = "New Group",
                    ParentId = null,
                    ColorHex = "#ff3322",
                    IconPath = "Images.dotnet_bot.png",
                    IsDefault = false
                };

                // Save the new group to the database
                await _groupListRepo.SaveAsync(newGroup);

                // Refresh the GroupList
                await GetGroupList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add group. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to add group.", "OK");
            }
        }

        [RelayCommand]
        async Task RemoveGroup(GroupListViewModel groupVm) 
        {
            if (groupVm == null) return;
            try
            {
                await _groupListRepo.DeleteAsync(groupVm.GroupEntity);

                // Refresh the GroupList
                await GetGroupList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to remove group. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to remove group.", "OK");
            }
        }

        // This method moves a group to a new parent group. It updates the ParentId of the group and saves it to the database.
        [RelayCommand]
        async Task MoveGroupToParent(MoveGroupArgs args ) 
        {
            if (args?.Group == null || args?.NewParent == null) return;

            try
            {
                args.Group.GroupEntity.ParentId = args.NewParent.GroupEntity.ID;
                await _groupListRepo.SaveAsync(args.Group.GroupEntity);

                await GetGroupList(); // Refresh the GroupList
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Unable to move group. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to move group.", "OK");
            }
        }

        // This method reorders groups based on a new order provided as a list of GroupListViewModel. It updates the SortOrder of each group and saves the changes to the database.
        [RelayCommand]
        async Task ReorderGroups(IList<GroupListViewModel> newOrder)
        {
            try 
            {
                for (int i = 0; i < newOrder.Count; i++)
                {
                    newOrder[i].GroupEntity.SortOrder = i; 
                    await _groupListRepo.SaveAsync(newOrder[i].GroupEntity);
                }
                await GetGroupList(); // Refresh the GroupList
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to reorder groups. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to reorder groups.", "OK");
            }
        }


        // <GroupListing Commands and Task>
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
