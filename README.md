# PLC Interface WPF Application (MVVM Architecture)

## ✨ Project Overview

This WPF application provides an intuitive interface to connect, read, and write data to Modbus and Siemens S7 PLCs. The application follows the **Model-View-ViewModel (MVVM)** architecture with strict adherence to **best coding practices**, **testability**, and **UI responsiveness**. It supports hierarchical tag grouping and dynamic PLC configuration with runtime IP/port updates.

---

## 🌐 Core Functionalities

* ✅ Connect to a PLC (Modbus or S7) using IP and Port
* ✅ Read values from PLC tags (with support for multiple data types)
* ✅ Write values to PLC tags
* ✅ Add/Delete:

  * Groups
  * Subgroups
  * Tags
* ✅ Mock data generation for initial testing
* ✅ PLC connection status indication
* ✅ Error/Warning/Message handling using a Dialog Service abstraction

---

## 🚀 MVVM Architecture Breakdown

### ■ **Model Layer**

* `Group`, `SubGroup`, `TagItem` classes
* Designed as POCOs (Plain Old CLR Objects) to represent hierarchical data
* Uses `ObservableCollection<T>` for UI binding and real-time updates

### ■ **ViewModel Layer**

* **`MainViewModel`** acts as the central logic unit
* Maintains:

  * State (Selected Group, SubGroup, Tag)
  * Data (Groups list, PLC Types, Data Types)
  * Commands (bound to UI interactions)
* Handles:

  * Validation
  * Service invocation
  * UI notification via `INotifyPropertyChanged`

### ■ **View Layer** (XAML)

* Uses `DataTemplate` for displaying nested groups and tags in `TreeView`
* Uses `Binding` to connect UI to `MainViewModel` properties/commands
* Separation of concerns strictly maintained

---

## 🚧 Best Practices Followed

### 🌟 **MVVM Pattern Adherence**

* No code-behind logic in views
* `INotifyPropertyChanged` implemented in base class
* `RelayCommand` used to implement `ICommand`

### ⚖️ **Command Abstraction**

* `ConnectCommand`, `ReadCommand`, `WriteCommand`, `Add/Delete Commands` are all exposed for XAML binding

### 📑 **Service Abstractions**

* `IPlcService` abstracts PLC connection logic for both Modbus and S7
* `IDialogService` handles all UI messages, making it mockable/testable

### ✨ **Data Binding**

* `ObservableCollection<T>` ensures real-time UI updates
* Use of `SelectedGroup`, `SelectedSubGroup`, `SelectedTag` provides full context in the UI

### ⚙️ **Error Handling**

* User is never left confused: each failure is followed by a user-friendly dialog
* All external service calls wrapped in try-catch with fallbacks

### 🔮 **Mock Data Generation**

* `LoadMockData()` method provides sample groups and tags to quickly verify UI behavior without real PLCs

---

## 🔹 Command Details

| Command                 | Description                                     |
| ----------------------- | ----------------------------------------------- |
| `ConnectCommand`        | Connects to the PLC using selected type/IP/port |
| `ReadCommand`           | Reads value of the selected tag from PLC        |
| `WriteCommand`          | Writes value of the selected tag to PLC         |
| `AddGroupCommand`       | Adds a new Group to the tree                    |
| `DeleteGroupCommand`    | Deletes the selected Group                      |
| `AddSubGroupCommand`    | Adds a SubGroup under selected Group            |
| `DeleteSubGroupCommand` | Deletes the selected SubGroup                   |
| `AddTagCommand`         | Adds a Tag under selected SubGroup              |
| `DeleteTagCommand`      | Deletes the selected Tag                        |

---

## 🌞 Supported PLC Types

* **Modbus TCP/IP**

  * Default IP: `127.0.0.1`
  * Default Port: `502`
* **Siemens S7 (S7.Net)**

  * Default IP: `192.168.0.1`
  * Default Port: `102`

---

## 🚨 Data Type Support

* `Int`
* `Real`
* `Bool`
* `Char`
* `String`

All data type parsing and casting handled within the PLC service layer.

---

## ⚒️ Technologies Used

* **WPF (.NET)**
* **MVVM Pattern**
* **RelayCommand Implementation**
* **ObservableCollection<T>** for dynamic UI binding
* **Dependency Injection (via constructor)** for service mocking/testability

---

## 📖 How to Run

1. Open the solution in Visual Studio
2. Run the WPF project
3. Use default mock data or connect to an actual PLC using the correct IP/Port
