using System.Collections.ObjectModel;

namespace PlcInterfaceApp.Models
{
    public class Group
    {
        public string Name { get; set; }
        public ObservableCollection<SubGroup> SubGroups { get; set; } = new();
    }
}
