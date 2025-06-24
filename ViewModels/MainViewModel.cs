using PlcInterfaceApp.Models;
using PlcInterfaceApp.Services;
using PlcInterfaceApp.Services.DialogService;
using PlcInterfaceApp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class MainViewModel : BaseViewModel
{
    // Properties for Groups, PLC Types, Data Types, and Commands
    public ObservableCollection<Group> Groups { get; set; } = new();
    public ObservableCollection<string> PlcTypes { get; set; } = new() { "Modbus", "S7" };
    public ObservableCollection<string> DataTypes { get; set; } = new() { "Int", "Real", "Bool", "Char", "String" };

    // Selected items for Group, SubGroup, and Tag

    private Group _selectedGroup;
    public Group SelectedGroup
    {
        get => _selectedGroup;
        set { _selectedGroup = value; OnPropertyChanged(nameof(SelectedGroup)); }
    }

    private SubGroup _selectedSubGroup;
    public SubGroup SelectedSubGroup
    {
        get => _selectedSubGroup;
        set { _selectedSubGroup = value; OnPropertyChanged(nameof(SelectedSubGroup)); }
    }

    private TagItem _selectedTag;
    public TagItem SelectedTag
    {
        get => _selectedTag;
        set { _selectedTag = value; OnPropertyChanged(nameof(SelectedTag)); }
    }

    // Properties for connection status and PLC type selection

    private bool _isConnecting;
    public bool IsConnecting
    {
        get => _isConnecting;
        set
        {
            _isConnecting = value;
            OnPropertyChanged(nameof(IsConnecting));
            OnPropertyChanged(nameof(IsNotConnecting)); // Optional if you use it in XAML
        }
    }
    public bool IsNotConnecting => !IsConnecting;
    private string _selectedPlcType;
    public string SelectedPlcType
    {
        get => _selectedPlcType;
        set
        {
            if (_selectedPlcType != value)
            {
                _selectedPlcType = value;
                OnPropertyChanged(nameof(SelectedPlcType));

                // Set default IP and Port based on PLC type
                if (_selectedPlcType == "Modbus")
                {
                    IpAddress = "127.0.0.1";
                    Port = "502";
                }
                else if (_selectedPlcType == "S7")
                {
                    IpAddress = "192.168.0.1";
                    Port = "102";
                }

                // Notify UI to refresh
                OnPropertyChanged(nameof(IpAddress));
                OnPropertyChanged(nameof(Port));
            }
        }
    }
    public string IpAddress { get; set; } = "192.168.0.1";
    public string Port { get; set; } = "502";
    public string SelectedDataType { get; set; }

    // Commands for Connect, Read, Write, Add Group, Delete Group, Add Sub Group, Delete Sub Group, Add Tag & Delete Tag actions
    public ICommand ConnectCommand { get; }
    public ICommand ReadCommand { get; }
    public ICommand WriteCommand { get; }
    public ICommand AddGroupCommand { get; }
    public ICommand DeleteGroupCommand { get; }
    public ICommand AddSubGroupCommand { get; }
    public ICommand DeleteSubGroupCommand { get; }
    public ICommand AddTagCommand { get; }
    public ICommand DeleteTagCommand { get; }

    // Services for PLC operations and dialog interactions

    private readonly IPlcService _plcService;
    private readonly IDialogService _dialogService;


    public MainViewModel(IPlcService plcService, IDialogService dialogService)
    {
        // Initialize mock data for testing purposes
        LoadMockData();
        SelectedPlcType = "Modbus";

        // Initialize services
        _plcService = plcService;
        _dialogService = dialogService;

        // Initialize commands
        ConnectCommand = new RelayCommand(_ => ConnectToPlc());
        ReadCommand = new RelayCommand(_ => ReadFromPlc());
        WriteCommand = new RelayCommand(_ => WriteToPlc());
        AddGroupCommand = new RelayCommand(_ => AddGroup());
        DeleteGroupCommand = new RelayCommand(_ => DeleteGroup());
        AddSubGroupCommand = new RelayCommand(_ => AddSubGroup());
        DeleteSubGroupCommand = new RelayCommand(_ => DeleteSubGroup());
        AddTagCommand = new RelayCommand(_ => AddTag());
        DeleteTagCommand = new RelayCommand(_ => DeleteTag());
    }

    /*     * Method to load mock data for initial testing.
     * This can be replaced with actual data loading logic in the future.
     */
    private void LoadMockData()
    {
        var group1 = new Group { Name = "Group A" };
        var sub1 = new SubGroup { Name = "Subgroup A1" };
        sub1.Tags.Add(new TagItem { Name = "Tag1", Address = "0", DataType = "Int", Value = "123" });
        sub1.Tags.Add(new TagItem { Name = "Tag2", Address = "1", DataType = "Bool", Value = "True" });
        group1.SubGroups.Add(sub1);
        Groups.Add(group1);
    }

    /*     * Methods for adding and deleting groups, subgroups, and tags.
     * These methods are bound to UI commands for user interaction.
     */
    private void AddGroup()
    {
        char groupLetter = (char)('A' + Groups.Count);
        Groups.Add(new Group { Name = $"Group {groupLetter}" });
    }

    private void DeleteGroup()
    {
        if (SelectedGroup != null && Groups.Contains(SelectedGroup))
        {
            Groups.Remove(SelectedGroup);
            SelectedGroup = null;
        }
        else
        {
            _dialogService.ShowWarning("Select a Group to Delete!");
        }
    }

    private void AddSubGroup()
    {
        if (SelectedGroup != null && Groups.Contains(SelectedGroup))
        {
            SelectedGroup.SubGroups.Add(new SubGroup { Name = $"Sub {SelectedGroup.SubGroups.Count + 1}" });
        }
        else
        {
            _dialogService.ShowWarning("Select a Group to Add Subgroup!");
        }
    }

    private void DeleteSubGroup()
    {
        if (SelectedGroup != null && SelectedSubGroup != null)
        {
            if (SelectedGroup.SubGroups.Contains(SelectedSubGroup))
            {
                SelectedGroup.SubGroups.Remove(SelectedSubGroup);
                SelectedSubGroup = null;
            }
        }
        else
        {
            _dialogService.ShowWarning("Select a SubGroup to Delete!");
        }
    }

    private void AddTag()
    {
        if (SelectedGroup != null && SelectedSubGroup != null)
        {
            SelectedSubGroup.Tags.Add(new TagItem
            {
                Name = $"Tag {SelectedSubGroup.Tags.Count + 1}",
                Address = "0",
                DataType = "Int",
                Value = "0"
            });
        }
        else
        {
            _dialogService.ShowWarning("Select a SubGroup to add a tag!");
        }
        
    }

    private void DeleteTag()
    {
        if (SelectedSubGroup != null && SelectedTag != null)
        {
            if (SelectedSubGroup.Tags.Contains(SelectedTag))
            {
                SelectedSubGroup.Tags.Remove(SelectedTag);
                SelectedTag = null;
            }
        }
        else
        {
            _dialogService.ShowWarning("Select a Tag to Delete!");
        }
    }

    /*     * Method to connect to the PLC using the selected PLC type, IP address, and port.
     * This method is bound to the Connect command and handles connection logic.
     */
    private async void ConnectToPlc()
    {
        if (string.IsNullOrWhiteSpace(IpAddress) || string.IsNullOrWhiteSpace(Port))
        {
            _dialogService.ShowError("IP Address or Port cannot be empty.", "Input Error");
            return;
        }

        if (!int.TryParse(Port, out int port))
        {
            _dialogService.ShowError("Invalid port number. Please enter a valid integer.", "Input Error");
            return;
        }

        IsConnecting = true;

        try
        {
            bool isConnected = await _plcService.ConnectAsync(SelectedPlcType, IpAddress, port);

            if (isConnected)
            {
                _dialogService.ShowMessage($"Connected to {SelectedPlcType} PLC at {IpAddress}:{Port}", "Connection Successful");
            }
            else
            {
                _dialogService.ShowError($"Failed to connect to {SelectedPlcType} PLC at {IpAddress}:{Port}. Please check your settings.", "Connection Failed");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Connection error: {ex.Message}", "Exception");
        }
        finally
        {
            IsConnecting = false;
        }
    }

    /*     * Method to read a value from the PLC based on the selected tag's address and data type.
     * This method is bound to the Read command and handles reading logic.
     */
    private async void ReadFromPlc()
    {
        if (SelectedTag == null)
        {
            _dialogService.ShowWarning("No tag selected for reading.");
            return;
        }

        if (!_plcService.IsConnected)
        {
            _dialogService.ShowWarning("Not connected to PLC.", "Connection Error");
            return;
        }

        try
        {
            string result = await _plcService.ReadValueAsync(SelectedPlcType, SelectedTag.Address, SelectedTag.DataType);

            if (result.StartsWith("Error"))
            {
                _dialogService.ShowError(result, "Read Error");
            }
            else
            {
                SelectedTag.Value = result;
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError("Unexpected error: " + ex.Message, "Read Error");
        }
    }

    /*     * Method to write a value to the PLC based on the selected tag's address, data type, and value.
     * This method is bound to the Write command and handles writing logic.
     */
    private async void WriteToPlc()
    {
        if (SelectedTag == null)
        {
            _dialogService.ShowWarning("No tag selected for writing.");
            return;
        }

        if (!_plcService.IsConnected)
        {
            _dialogService.ShowWarning("Not connected to PLC.");
            return;
        }

        try
        {
            bool success = await _plcService.WriteValueAsync(
                SelectedPlcType,
                SelectedTag.Address,
                SelectedTag.DataType,
                SelectedTag.Value
            );

            if (success)
            {
                _dialogService.ShowMessage("Value written successfully.", "Success");
            }
            else
            {
                _dialogService.ShowError("Failed to write value. Please check the address and data type.", "Write Error");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError("Unexpected error: " + ex.Message, "Write Error");
        }
    }
}
