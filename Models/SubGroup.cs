using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcInterfaceApp.Models
{
    public class SubGroup
    {
        public string Name { get; set; }
        public ObservableCollection<TagItem> Tags { get; set; } = new();
    }
}
