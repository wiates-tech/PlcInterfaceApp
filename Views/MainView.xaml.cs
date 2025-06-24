using PlcInterfaceApp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PlcInterfaceApp.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(App.PlcService, App.DialogService);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm)
            {
                switch (e.NewValue)
                {
                    case Group group:
                        vm.SelectedGroup = group;
                        vm.SelectedSubGroup = null;
                        vm.SelectedTag = null;
                        break;

                    case SubGroup subgroup:
                        vm.SelectedSubGroup = subgroup;
                        vm.SelectedGroup = vm.Groups.FirstOrDefault(g => g.SubGroups.Contains(subgroup));
                        vm.SelectedTag = null;
                        break;

                    case TagItem tag:
                        vm.SelectedTag = tag;

                        foreach (var group in vm.Groups)
                        {
                            foreach (var sub in group.SubGroups)
                            {
                                if (sub.Tags.Contains(tag))
                                {
                                    vm.SelectedGroup = group;
                                    vm.SelectedSubGroup = sub;
                                    return;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
