
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.View.Editor;
using Mystic_ToDo_MAUI_.ViewModel.HomeVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Mail;
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
        private readonly DBManager<TaskList_RepeatList> _taskList_RepeatListRepo;
        private readonly DBManager<Attachments> _attachmentsRepo;


        // -----------------------------
        // GroupListViewModel properties and supporting structures
        // -----------------------------
        // ObservableCollections for data binding

        // Constant properties
        public const int RootParentId = 1;

        // Supporting methods and properties
        public record MoveGroupArgs(GroupListViewModel Group, GroupListViewModel NewParent);
        private Dictionary<int, GroupListViewModel> _lookup = new();

        // Controls, Collections and Selected Items 
        [ObservableProperty]
        private ObservableCollection<GroupListViewModel> groupList = new();
        [ObservableProperty]
        private GroupListViewModel selectedGroup;

        [ObservableProperty]
        private bool isGroupListExpanded;
        [ObservableProperty]
        private ImageSource groupExpanderIcon;

        // Flags for Async loading state
        [ObservableProperty]
        bool loading_GroupList = false;



        // -----------------------------
        // Editor properties and supporting structures
        // -----------------------------

        // Constant properties


        // Supporting methods and properties
        private CancellationTokenSource _cts = new();


        // Controls, Collections and Selected Items 
        [ObservableProperty]
        private bool editorModeIsEdit = false;
        [ObservableProperty]
        private string editorActionText = "Add";
        partial void OnEditorModeIsEditChanged(bool value)
        {
            EditorActionText = value ? "Edit" : "Add";
        }

        [ObservableProperty]
        private string editorTaskTitle;

        [ObservableProperty]
        public string editorNotes;

        [ObservableProperty]
        private DateTime editorAlarmDueDate;
        [ObservableProperty]
        private bool editorAlarmIsEnabled;

        [ObservableProperty]
        public ObservableCollection<string> repeatTags = new();
        [ObservableProperty]
        private string repeatListTagSelected;
        [ObservableProperty]
        private bool repeatListTagIsEnabled = false;

        [ObservableProperty]
        private ObservableCollection<Attachments> editorAttachmentList = new();
        [ObservableProperty]
        private int editorAttachmentSelection = 0;

        [ObservableProperty]
        private ObservableCollection<string> sortTypeList = new();
        [ObservableProperty]
        private string sortTypeSelected;

        [ObservableProperty]
        private ObservableCollection<string> sortOrderList = new();
        [ObservableProperty]
        private string sortOrderSelected;

        [ObservableProperty]
        public ObservableCollection<TaskListVM> taskList = new();
        [ObservableProperty]
        private TaskListVM taskSelected;



        // Flags for Async loading state
        [ObservableProperty]
        bool loading_TaskList_RepeatList_Tag = false;
        [ObservableProperty]
        bool loading_EditorAttachment = false;
        [ObservableProperty]
        bool loading_SortType = false;
        [ObservableProperty]
        bool loading_SortOrder = false;
        [ObservableProperty]
        bool loading_TaskList = false;


        [ObservableProperty]
        private string currentLoadingStep;


        // -----------------------------
        // Constructor
        // -----------------------------
        public HomeViewModel(
            DBManager<GroupList> groupRepo,
            DBManager<TaskList> taskRepo,
            DBManager<TaskList_RepeatTag> repeatTagRepo,
            DBManager<TaskList_RepeatList> taskList_RepeatListRepo,
            DBManager<Attachments> attachmentsRepo
        ) 
        { 
            Title = "Home";
            _groupListRepo = groupRepo;
            _taskListRepo = taskRepo;
            _taskList_RepeatTagRepo = repeatTagRepo;
            _taskList_RepeatListRepo = taskList_RepeatListRepo;
            _attachmentsRepo = attachmentsRepo;
 

            this.IsGroupListExpanded = true;
            this.GroupExpanderIcon = "arrow_right.png";

            EditorAlarmIsEnabled = false;
            RepeatListTagIsEnabled = false;



        }



        // -----------------------------
        // GroupListing Commands and Task
        // -----------------------------
        [RelayCommand]
        async Task GetGroupList()
        {
            if (Loading_GroupList) return;

            try
            {
                Loading_GroupList = true;
                GroupList.Clear();

                var groupListing = await _groupListRepo.GetAllAsync();

                // Create Dictionary from DB
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
                Loading_GroupList = false;
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
        async Task SelectGroup(GroupListViewModel selectedGroup) 
        {
            if (selectedGroup == null) return;

            // Clear previours selection
            ClearSelection(GroupList);

            // Mark the new selection
            Debug.WriteLine($"Selected group: {selectedGroup.GroupName}");
            selectedGroup.IsSelected = true;
            SelectedGroup = selectedGroup;

            // Refresh TaskList based on new group selection
            await GetTaskList();
        }

        [RelayCommand]
        async Task GroupListStartupSelection() 
        {
            if (GroupList.Count > 0)
            {
                SelectGroup(GroupList[0]); // Set the Default Group
           
            }
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

        //[RelayCommand]
        async Task GetTaskList_RepeatList_Tag()
        {
            if (Loading_TaskList_RepeatList_Tag) return;

            try
            {
                Loading_TaskList_RepeatList_Tag = true;

                var raw_tagListing = await _taskList_RepeatTagRepo.GetAllAsync();

                //_cts.Token.ThrowIfCancellationRequested();


                if (raw_tagListing != null)
                {

                    RepeatTags = new ObservableCollection<string>(
                          raw_tagListing?.Select(x => x.RepeatTagName)
                          ?? Enumerable.Empty<string>()
                      );


                    // set AFTER population
                    RepeatListTagSelected = RepeatTags.FirstOrDefault();

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: RepeatTags. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: RepeatTags.", "OK");
            }
            finally
            {
                Loading_TaskList_RepeatList_Tag = false;
            }
        }

        [RelayCommand]
        async Task EditorAlarmToggle() 
        {
            if (EditorAlarmIsEnabled)
            {

                EditorAlarmDueDate = DateTime.Now;
                EditorAlarmIsEnabled = false;   // Hide DatePicker when toggled off
                RepeatListTagSelected = RepeatTags.FirstOrDefault() ?? string.Empty;
                RepeatListTagIsEnabled = false; // Disable repeat options when alarm is off

            }
            else
            {
                EditorAlarmIsEnabled = true; // Show DatePicker when toggled on
            }
        }

        [RelayCommand]
        async Task EditorRepeatToggle()
        {
            if (!EditorAlarmIsEnabled)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                                                        "Alarm must be enabled to set repeat options.", 
                                                        "OK");

                // Force repeat options off when alarm is disabled
                RepeatListTagIsEnabled = false;
                RepeatListTagSelected = RepeatTags.FirstOrDefault()
                                        ?? string.Empty;
                return;
            }

            // Alarm is enabled → toggle repeat options
            if (RepeatListTagIsEnabled)
            {
                // Currently on → turn off
                RepeatListTagIsEnabled = false;
                //RepeatListTagSelected = RepeatTags.FirstOrDefault()
                                        //?? string.Empty;
            }
            else
            {
                // Currently off → turn on
                RepeatListTagIsEnabled = true;
                RepeatListTagSelected = TaskSelected?.TaskList_RepeatTag?.RepeatTagName
                                        ?? RepeatTags.FirstOrDefault()
                                        ?? string.Empty;
            }
        }

        //[RelayCommand]
        //async Task EditorAction() 
        //{
        //    if (EditorModeIsEdit)
        //    {
        //        // Edit existing task
        //        if (TaskSelected != null)
        //        {
        //            TaskSelected.TaskTitle = EditorTaskTitle;
        //            TaskSelected.Notes = EditorNotes;
        //            TaskSelected.DueDate = EditorDueDate;
        //            await _taskListRepo.UpdateAsync(TaskSelected.TaskEntity);
        //            await GetTaskList(); // Refresh task list to reflect changes
        //        }
        //    }
        //    else
        //    {
        //        // Add new task
        //        var newTask = new TaskList
        //        {
        //            Title = EditorTaskTitle,
        //            Notes = EditorNotes,
        //            DueDate = EditorDueDate,
        //            GroupID = SelectedGroup.Id
        //        };

        //        var newRepeat = new TaskList_RepeatList
        //        {
        //            RepeatTagID = _taskList_RepeatTagRepo.GetAllAsync().Result
        //                            .FirstOrDefault(t => t.RepeatTagName == RepeatListTagSelected)?.ID
        //        };  


        //        await _taskListRepo.InsertAsync(newTask);
        //        await _taskList_RepeatListRepo.InsertAsync(new TaskList_RepeatList
        //        {
        //            TaskListID = newTask.ID,
        //            RepeatTagID = newRepeat.RepeatTagID
        //        });
        //        await GetTaskList(); // Refresh task list to show new task
        //    }

        //    // Clear editor fields after action
        //    await EditorClear();
        //}


        [RelayCommand]
        async Task EditorClear() 
        {
            EditorTaskTitle = string.Empty;
            EditorNotes = string.Empty;
            EditorAlarmDueDate = DateTime.Now;
            EditorModeIsEdit = false; // Reset to Add mode
            TaskSelected.IsSelected = false; // Clear selection
            RepeatListTagSelected = RepeatTags.FirstOrDefault() ?? string.Empty;
            EditorAttachmentList.Clear();
            EditorAttachmentSelection = 0;
            EditorAlarmDueDate = DateTime.Now;
            EditorAlarmIsEnabled = false;
            RepeatListTagSelected = RepeatTags.FirstOrDefault()
                                    ?? string.Empty;
            RepeatListTagIsEnabled = false;

        }


        [RelayCommand]
        async Task GetEditorAttachment() 
        {
            if (Loading_EditorAttachment) return;

            try
            {
                Loading_EditorAttachment = true;
                if (EditorAttachmentList.Any()) EditorAttachmentList.Clear();

                var attachments = await _attachmentsRepo.GetAllAsync();
                if (attachments != null)
                {
                    if(TaskSelected != null) 
                    {
                        // Filter tasks by selected group
                        var taskAttachments = attachments.Where(t => t.TaskListId == TaskSelected.TaskID);

                        foreach (var attachment in taskAttachments)
                        {
                            EditorAttachmentList.Add(attachment);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: Attachment List. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: Attachment List.", "OK");
            }
            finally
            {
                Loading_EditorAttachment = false;
            }
        }

        [RelayCommand]
        async Task EditorAttachmentAdd() 
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an attachment"
            });

            if (result != null)
            {
                // Full path to the file
                string filePath = result.FullPath;

                // File name only
                string fileName = Path.GetFileName(filePath);

                // File extension/type
                string fileType = Path.GetExtension(filePath);

                // Populate your model
                var attachment = new Attachments
                {
                    AttachmentName = fileName,
                    AttachmentType = fileType,
                    AttachmentPath = filePath,
                    TaskListId = TaskSelected.TaskID // link to the task
                };

                // Save to DB or add to collection
                await _attachmentsRepo.InsertAsync(attachment);
            }

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

        [RelayCommand]
        async Task LoadSortType() 
        {
            if (Loading_SortType) return;

            try
            {
                Loading_SortType = true;

                SortTypeList = new ObservableCollection<string>
                {
                   "Name",
                   "Due Date"
                };

                // set AFTER population
                SortTypeSelected = SortTypeList.FirstOrDefault();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: Sort Type List. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: Sort Type List.", "OK");
            }
            finally
            {
                Loading_SortType = false;
            }
        }

        [RelayCommand]
        async Task LoadSortOrder()
        {
            if (Loading_SortOrder) return;

            try
            {
                Loading_SortOrder = true;

                SortOrderList = new ObservableCollection<string>
                {
                   "Asen",
                   "Desen"
                };

                // set AFTER population
                SortOrderSelected = SortOrderList.FirstOrDefault();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: Sort Order List. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: Sort Order List.", "OK");
            }
            finally
            {
                Loading_SortOrder = false;
            }
        }




        // -----------------------------
        // TaskListing Commands and Task
        // -----------------------------
        [RelayCommand]
        async Task GetTaskList()
        {
            if (Loading_TaskList) return;

            try
            {
                Loading_TaskList = true;
                if (TaskList.Any()) TaskList.Clear();

                var taskListing = await _taskListRepo.GetAllAsync();
                if (taskListing != null)
                {
                    // Filter tasks by selected group
                    var groupTasks = taskListing.Where(t => t.GroupID == SelectedGroup.Id);

                    foreach (var task in groupTasks)
                    {
                        var vm = new TaskListVM(task, taskListing, groupTasks, 
                                                _taskList_RepeatListRepo, _taskList_RepeatTagRepo, _attachmentsRepo);

                        await vm.LoadInfoAsync();

                        TaskList.Add(vm);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get: TaskList. Error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Unable to get: TaskList.", "OK");
            }
            finally
            {
                Loading_TaskList = false;
            }
        }

        [RelayCommand]
        async Task TaskSelection(TaskListVM taskSelection) 
        {
            if (taskSelection == null) return;

            // Clear previours selection
            ClearSelection(GroupList);

            // Mark the new selection
            Debug.WriteLine($"Selected Task: {taskSelection.TaskTitle}");
            taskSelection.IsSelected = true;
            TaskSelected = taskSelection;

            // Populate Editor fields based on selected task
            if (TaskSelected != null)
            {
                EditorModeIsEdit = true;
                EditorTaskTitle = TaskSelected.TaskTitle;
                EditorNotes = TaskSelected.Notes ?? string.Empty;

                // Set alarm and repeat options based on task properties
                if (TaskSelected.DueDate != null) 
                {
                    EditorAlarmIsEnabled = true;
                    EditorAlarmDueDate = TaskSelected.DueDate ?? DateTime.Now;
                } 
                else
                {
                    EditorAlarmIsEnabled = false;
                    RepeatListTagIsEnabled = false;
                }
                
                if(TaskSelected.TaskList_RepeatTag != null)
                {
                    RepeatListTagIsEnabled = true;
                    RepeatListTagSelected = TaskSelected.TaskList_RepeatTag.RepeatTagName 
                                            ?? RepeatTags.FirstOrDefault() 
                                            ?? string.Empty;

                }
                else
                {
                    RepeatListTagSelected = RepeatTags.FirstOrDefault() ?? string.Empty;
                    RepeatListTagIsEnabled = false;
                }
            }
                
    

            // Load Attachments for the selected task
            await GetEditorAttachment();

        }




        // -----------------------------
        // Async Method for Loading Data Automatically 
        // -----------------------------
        public async Task LoadDataAsync() 
        {
            if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db")))
            {
                Debug.WriteLine("Database not ready yet.");
                return;
            }

            //if (!_groupListRepo.IsInitialized || 
            //    !_groupListRepo.IsInitialized || 
            //    !_taskListRepo.IsInitialized || 
            //    !_taskList_RepeatTagRepo.IsInitialized
            //    )
            //{
            //    Debug.WriteLine("Database not ready yet.");
            //    return;
            //}


            await GetGroupList();
            await GroupListStartupSelection();
            await GetTaskList_RepeatList_Tag();
            await LoadSortType();
            await LoadSortOrder();
            await GetTaskList();
            await GetEditorAttachment();
        }

        public async Task LoadDataTrackerAsync()
        {
            

            if (IsLoading) return;

            try
            {

                await RunStep("Loading groups...", GetGroupList);
                await RunStep("Selecting default group...", GroupListStartupSelection);
                await RunStep("Loading repeat tags...", GetTaskList_RepeatList_Tag);
                await RunStep("Loading sort types...", LoadSortType);
                await RunStep("Loading sort order...", LoadSortOrder);
                await RunStep("Loading tasks...", GetTaskList);
                await RunStep("Loading attachments...", GetEditorAttachment);
            }
            finally
            {
                CurrentLoadingStep = "Done";
                IsLoading = false;
            }
        }

        private async Task RunStep(string stepName, Func<Task> action)
        {
            try
            {
                CurrentLoadingStep = stepName;
                Debug.WriteLine($"[START] {stepName}");

                await action();

                Debug.WriteLine($"[SUCCESS] {stepName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] {stepName}: {ex.Message}");
                throw; // or handle it here
            }
        }


        public void Cancel()
        {
            if (!_cts.IsCancellationRequested)
                _cts.Cancel();

            _cts = new CancellationTokenSource(); // reset for reuse
        }

    }
}
