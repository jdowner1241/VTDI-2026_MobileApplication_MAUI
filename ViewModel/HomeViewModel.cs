using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.View.Editor;
using Mystic_ToDo_MAUI_.ViewModel.GroupListVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;


namespace Mystic_ToDo_MAUI_.ViewModel
{
    public partial class HomeViewModel : BaseViewModel
    {
        // -----------------------------
        // Repositories for database access
        // -----------------------------
        private readonly DBManager<GroupList> _groupListRepo;
        private readonly DBManager<TaskList> _taskListRepo;
        private readonly DBManager<TaskList_RepeatTag> _taskList_RepeatTagRepo;
        private readonly DBManager<Attachments> _attachmentsRepo;

        [ObservableProperty]
        private ObservableCollection<GroupListViewModel> groupList = new();
        public ObservableCollection<TaskList> TaskList { get; private set; } = new();
        public ObservableCollection<TaskList_RepeatTag> TaskList_RepeatTag { get; private set; } = new();



        // -----------------------------
        // GroupListViewModel properties and supporting structures
        // -----------------------------
        // ObservableCollections for data binding

        //public ObservableCollection<GroupListViewModel> GroupList { get; private set; } = new();
        // Constant properties
        public const int RootParentId = 1;

        // Supporting methods and properties
        public record MoveGroupArgs(GroupListViewModel Group, GroupListViewModel NewParent);

        [ObservableProperty]
        private GroupListViewModel selectedGroup;

        private Dictionary<int, GroupListViewModel> _lookup = new();

        [ObservableProperty]
        private bool isGroupListExpanded;
        [ObservableProperty]
        private ImageSource groupExpanderIcon;

        // -----------------------------
        // Editor properties and supporting structures
        // -----------------------------
        [ObservableProperty]
        private int editorSlideIndex = 0;
        [ObservableProperty]
        private int repeatListTagIndex = 0;

        [ObservableProperty]
        private ObservableCollection<Attachments> editorAttachmentList = new();
        [ObservableProperty]
        private int editorAttachmentSelection = 0;

     


        // -----------------------------
        // Constructor
        // -----------------------------
        public HomeViewModel() 
        { 
            Title = "Home";
            this._groupListRepo = new DBManager<GroupList>();
            this._taskListRepo = new DBManager<TaskList>();
            this._taskList_RepeatTagRepo = new DBManager<TaskList_RepeatTag>();
            this.selectedGroup = SelectedGroup;
          
            this.IsGroupListExpanded = true;
            this.GroupExpanderIcon = "arrow_right.png";


            this.EditorAttachmentList = new ObservableCollection<Attachments>
            {
                new Attachments { AttachmentName = "Report.pdf", AttachmentType = "PDF" },
                new Attachments { AttachmentName = "Image1.png", AttachmentType = "Image" },
                new Attachments { AttachmentName = "Notes.txt", AttachmentType = "Text" },
                new Attachments { AttachmentName = "Notes.txt", AttachmentType = "Text" },
                new Attachments { AttachmentName = "Notes.txt", AttachmentType = "Text" },
                new Attachments { AttachmentName = "Notes.txt", AttachmentType = "Text" },
                new Attachments { AttachmentName = "Notes.txt", AttachmentType = "Text" },
                new Attachments { AttachmentName = "Notes.txt", AttachmentType = "Text" }
            };
            



        }



        // -----------------------------
        // GroupListing Commands and Task
        // -----------------------------
        [RelayCommand]
        async Task GetGroupList()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                GroupList.Clear();

                var groupListing = await _groupListRepo.GetAllAsync();

        
                _lookup = groupListing.ToDictionary(
                    g => g.ID,
                    g => new GroupListViewModel(g, groupListing)
                );


                

                // Build hierarchy
                foreach (var vm in _lookup.Values)
                {
                    Debug.WriteLine($"Processing group: {vm.GroupName} (ID: {vm.Id}, ParentID: {vm.ParentId})");


                    if(vm.Id != RootParentId) 
                    {
                        if (vm.GroupEntity.ParentId is int parentId && _lookup.ContainsKey(parentId))
                        {
                            _lookup[parentId].SubGroups.Add(vm);
                            Debug.WriteLine($"Processing Sub group: {vm.GroupName} (ID: {vm.Id}, ParentID: {vm.ParentId})");
                        }
                    }
           
                }

                // -----------------------------
                // EXPAND/COLLAPSE ROOT LOAD
                // (Disabled for now)
                // -----------------------------
                /*
                foreach (var root in _lookup.Values
                             .Where(g => g.GroupEntity.ParentId == null)
                             .OrderBy(g => g.SortOrder))
                {
                    root.Level = 0;
                    GroupList.Add(root);
                }
                */

                // Assign the ToggleExpand command to each ViewModel
                /*
                foreach (var vm in _lookup.Values)
                {
                    vm.ExpandAction = ToggleExpand;
                }
                */

                // -----------------------------
                // FLATTEN TREE (All expanded)
                // -----------------------------
                var flatList = new ObservableCollection<GroupListViewModel>();

                foreach (var root in _lookup.Values
                         .Where(g => g.GroupEntity.ParentId == 1)
                         .OrderBy(g => g.SortOrder))
                {
                    FlattenGroups(root, 0, flatList);
                }

                GroupList = flatList;
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


        //Toggle Expand
        public void ToggleExpand(GroupListViewModel item)
        {
            if (item.IsExpanded)
                Collapse(item);
            else
                Expand(item);

            item.IsExpanded = !item.IsExpanded;
        }

        // Group Expanded 
        private void Expand(GroupListViewModel item)
        {
            int index = GroupList.IndexOf(item);

            foreach (var child in item.SubGroups.OrderBy(c => c.SortOrder))
            {
                child.Level = item.Level + 1;

                index++;
                GroupList.Insert(index, child);

                if (child.IsExpanded)
                    Expand(child);
            }
        }

        // Group Collapsed 
        private void Collapse(GroupListViewModel item)
        {
            foreach (var child in item.SubGroups.ToList())
            {
                RemoveRecursive(child);
            }
        }

        private void RemoveRecursive(GroupListViewModel item)
        {
            GroupList.Remove(item);

            foreach (var child in item.SubGroups)
                RemoveRecursive(child);
        }

        private void FlattenGroups(GroupListViewModel vm, int level, ObservableCollection<GroupListViewModel> flatList)
        {
            vm.Level = level;
            flatList.Add(vm);

            foreach (var child in vm.SubGroups.OrderBy(c => c.SortOrder))
                FlattenGroups(child, level + 1, flatList);
        }

        // GroupList Section Expanded/Collapsed and Selection Commands
        [RelayCommand]
        private void ExpanderForGroup() 
        {

            IsGroupListExpanded = !IsGroupListExpanded;
            if (IsGroupListExpanded)
            {
                Debug.WriteLine("Group list expanded.");
                GroupExpanderIcon = "arrow_right.png";
                 
            }
            else
            {
                Debug.WriteLine("Group list collapsed.");
                GroupExpanderIcon = "arrow_left.png";
            }
            
        }



        [RelayCommand]
        void SelectGroup(GroupListViewModel selectedGroup) 
        {
            if (selectedGroup == null) return;

            // Clear previours selection
            ClearSelection(GroupList);

            // Mark the new selection
            Debug.WriteLine($"Selected group: {selectedGroup.GroupName}");
            selectedGroup.IsSelected = true;
            SelectedGroup = selectedGroup;


        }

        private void ClearSelection(IEnumerable<GroupListViewModel> groups)
        {
            foreach (var g in groups)
            {
                g.IsSelected = false;
                if (g.SubGroups.Any())
                    ClearSelection(g.SubGroups);
            }
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
                
                // Validate input
                if (string.IsNullOrWhiteSpace(name))
                {
                    await Shell.Current.DisplayAlertAsync("Invalid Name", "Group name cannot be empty.", "OK");
                    return; // user cancelled or entered empty name
                }

                // Check for duplicate names (case-insensitive)
                if (GroupList.Any(g => g.GroupName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    await Shell.Current.DisplayAlertAsync("Duplicate Name", "A group with this name already exists.", "OK");
                    return; // duplicate name
                }

                // Create new GroupList entity
                var newGroup = new GroupList
                {
                    GroupName = name,
                    ParentId = selectedGroup?.Id,
                    ColorHex = $"{BaseColors.blue300}",
                    IconPath = "dotnet_bot.png",
                    IsDefault = false
                };

                // Save the new group to the database
                await _groupListRepo.InsertAsync(newGroup);
         

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
                if(groupVm.IsDefault == true) 
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Default groups cannot be removed.", "OK");
                    return;
                }

                // Confirm deletion with the user
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Confirm Deletion", 
                    $"Are you sure you want to delete the group '{groupVm.GroupName}'?", 
                    "Delete", 
                    "Cancel");
                if (confirm) 
                {
                    await _groupListRepo.DeleteAsync(groupVm.GroupEntity);
                    Debug.WriteLine($"Group '{groupVm.GroupName}' deleted successfully.");
                    await Shell.Current.DisplayAlertAsync("Info", $"Group '{groupVm.GroupName}' has been deleted.", "OK");
                    
                }
                

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
                // Confirms that its not a default Group
                if (!args.NewParent.IsDefault) 
                {
                    args.Group.GroupEntity.ParentId = args.NewParent.GroupEntity.ID;
                    await _groupListRepo.UpdateAsync(args.Group.GroupEntity);
                    Debug.WriteLine($"Group '{args.Group.GroupName}' moved to new parent '{args.NewParent.GroupName}' successfully.");
                } 
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Cannot move group to a default group.", "OK");
                    Debug.WriteLine($"Cannot move group '{args.Group.GroupName}' to default parent '{args.NewParent.GroupName}'.");
                }
          

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
                    await _groupListRepo.UpdateAsync(newOrder[i].GroupEntity);
                }
                await GetGroupList(); // Refresh the GroupList
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to reorder groups. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to reorder groups.", "OK");
            }
        }


        // -----------------------------
        // Editor Commands and Task
        // -----------------------------
        [RelayCommand]
        private void NextEditorSlide() 
        {
            // 3 Slides
            if (EditorSlideIndex < 2) 
            {
                EditorSlideIndex++;
            }
        }

        [RelayCommand]
        private void PreviousEditorSlide()
        {
            // 3 Slides
            if (EditorSlideIndex > 0)
            {
                EditorSlideIndex--;
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


        [RelayCommand]
        async Task EditorAttachmentGet() 
        {

        }

        [RelayCommand]
        private void EditorAttachmentAdd() 
        {

        }

        [RelayCommand]
        private void EditorAttachmentSelect(Attachments selectedAttachment) 
        {
            if (selectedAttachment != null)
            {
                // For testing, just log or debug
                Console.WriteLine($"Selected: {selectedAttachment.AttachmentName} ({selectedAttachment.AttachmentType})");
            }

        }





        // -----------------------------
        // TaskListing Commands and Task
        // -----------------------------
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



        // -----------------------------
        // Async Method for Loading Data Automatically 
        // -----------------------------
        public async Task LoadDataAsync() 
        {
            await GetGroupList();
            //await EditorAttachmentGet();
            //await GetTaskList_RepeatList_Tag();
            //await GetTaskList();


        }


    }
}
