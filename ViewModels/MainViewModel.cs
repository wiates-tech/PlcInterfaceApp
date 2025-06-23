using PlcInterfaceApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlcInterfaceApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IPlcService _plcService = new PlcService();

        public ObservableCollection<Group> Groups { get; set; } = new();
        public ObservableCollection<string> PlcTypes { get; set; } = new() { "Modbus", "S7" };
        public string SelectedPlcType { get; set; } = "Modbus";
        public string IpAddress { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "502";
        public string Address { get; set; }
        public string SelectedDataType { get; set; } = "Int";
        public string Value { get; set; }

        public ICommand ConnectCommand { get; }
        public ICommand ReadCommand { get; }
        public ICommand WriteCommand { get; }

        public MainViewModel()
        {
            ConnectCommand = new RelayCommand(async _ => await ConnectAsync());
            ReadCommand = new RelayCommand(async _ => await ReadAsync());
            WriteCommand = new RelayCommand(async _ => await WriteAsync());
            LoadMockData();
        }

        private async Task ConnectAsync()
        {
            bool connected = await _plcService.ConnectAsync(SelectedPlcType, IpAddress, int.Parse(Port));
            MessageBox.Show(connected ? "Connected successfully." : "Connection failed.");
        }

        private async Task ReadAsync()
        {
            string result = await _plcService.ReadValueAsync(SelectedPlcType, Address, SelectedDataType);
            Value = result;
            OnPropertyChanged(nameof(Value));
        }

        private async Task WriteAsync()
        {
            bool success = await _plcService.WriteValueAsync(SelectedPlcType, Address, SelectedDataType, Value);
            MessageBox.Show(success ? "Write successful." : "Write failed.");
        }

        private void LoadMockData()
        {
            var group = new Group { Name = "Group A" };
            var subgroup = new SubGroup { Name = "Subgroup A1" };
            subgroup.Tags.Add(new TagItem { Name = "Tag1", Address = "0", DataType = "Int", Value = "123" });
            group.SubGroups.Add(subgroup);
            Groups.Add(group);
        }
    }
}
