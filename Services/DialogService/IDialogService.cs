using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcInterfaceApp.Services.DialogService
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "Info");
        void ShowError(string message, string title = "Error");
        void ShowWarning(string message, string title = "Warning");
        bool Confirm(string message, string title = "Confirm");
    }
}
