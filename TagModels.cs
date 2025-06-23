using System.Collections.ObjectModel;

namespace PlcInterfaceApp
{
    public class TagItem
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
    }

    public class SubGroup
    {
        public string Name { get; set; }
        public ObservableCollection<TagItem> Tags { get; set; } = new ObservableCollection<TagItem>();
    }

    public class Group
    {
        public string Name { get; set; }
        public ObservableCollection<SubGroup> SubGroups { get; set; } = new ObservableCollection<SubGroup>();
    }
}
