using PlcInterfaceApp.Models;
using PlcInterfaceApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PlcInterfaceApp.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm && e.NewValue is TagItem tag)
            {
                // Optionally handle tag selection logic here
                // vm.SelectedTag = tag;
            }
        }
    }
}
