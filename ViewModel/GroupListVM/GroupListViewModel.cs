using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mystic_ToDo_MAUI_.Model.db.tables;
using Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor;
using System.Collections.ObjectModel;

namespace Mystic_ToDo_MAUI_.ViewModel.GroupListVM
{
    public partial class GroupListViewModel : ObservableObject
    {
        public GroupList GroupEntity { get; }
        private readonly IEnumerable<GroupList> _allGroups;

        public GroupListViewModel(GroupList groupEntity, IEnumerable<GroupList> allGroups)
        {
            GroupEntity = groupEntity;
            _allGroups = allGroups;
            BuildPath();
        }

        // Exposed entity properties
        public int Id => GroupEntity.ID;
        public string GroupName => GroupEntity.GroupName;
        public Color? Color => Color.FromArgb(GroupEntity.ColorHex);
        public ImageSource Icon => ImageSource.FromFile(GroupEntity.IconPath);
        public bool IsDefault => GroupEntity.IsDefault;
        public int SortOrder => GroupEntity.SortOrder;
        public int? ParentId => GroupEntity.ParentId;    

        // Constant properties
        public const int RootParentId = 1;


        // Computed properties
        [ObservableProperty]
        private string path;

        public ObservableCollection<GroupListViewModel> SubGroups { get; } = new();

        [ObservableProperty]
        private bool isSelected;

        public Color SelectionIndicatorColor =>
            IsSelected ? Color.FromArgb(BaseColors.blue300) : Colors.Transparent;

        // Tree depth
        public int Level { get; set; }

        // Indentation
        public Thickness IndentMargin => new Thickness(Level * 20, 0, 5, 5);

        // Expansion
        [ObservableProperty]
        private bool isExpanded;

        public bool HasChildren => SubGroups.Count > 0;

        public string ExpandIcon =>
            !HasChildren ? "" : IsExpanded ? "arrow_down.png" : "arrow_up.png";

        public Action<GroupListViewModel>? ExpandAction { get; set; }


        [RelayCommand]
        void ToggleExpand()
        {
            ExpandAction?.Invoke(this);
        }

        //[RelayCommand]
        //void ToggleExpand()
        //{
        //    IsExpanded = !IsExpanded;
        //}

        private void BuildPath()
        {
            var segments = new List<string>();
            var current = GroupEntity;

            while (current != null)
            {
                segments.Insert(0, current.GroupName);
                current = _allGroups.FirstOrDefault(g => g.ID == current.ParentId);
            }

            Path = string.Join(" > ", segments);
        }
    }
}