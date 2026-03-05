using CommunityToolkit.Mvvm.ComponentModel;
using Mystic_ToDo_MAUI_.Model.db.tables;

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

        // Expose entity properties for binding
        public int Id => GroupEntity.ID;
        public string GroupName => GroupEntity.GroupName;
        public Color? Color => GroupEntity.Color;
        public ImageSource Icon => GroupEntity.Icon;
        public bool IsDefault => GroupEntity.IsDefault;

        public int SortOrder => GroupEntity.SortOrder;

        [ObservableProperty]
        private string path;


        private void BuildPath()
        {
            var segements = new List<string>();
            var current = GroupEntity;

            while  (current != null)
            {
                segements.Insert(0, current.GroupName);
                current = _allGroups.FirstOrDefault(g => g.ID == current.ParentId);
            }

            Path = string.Join(" > ", segements);
        }
    }
}
