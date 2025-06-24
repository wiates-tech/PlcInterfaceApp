using System.Collections.ObjectModel;

namespace PlcInterfaceApp.Models
{
    public class SubGroup
    {
        public string Name { get; set; }
        public ObservableCollection<TagItem> Tags { get; set; } = new();
        public Group ParentGroup { get; set; }
    }
}
