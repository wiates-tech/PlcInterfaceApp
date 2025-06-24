using PlcInterfaceApp.Services;
using PlcInterfaceApp.Services.DialogService;
using System.Windows;

namespace PlcInterfaceApp
{
    public partial class App : Application
    {
        public static IPlcService PlcService { get; private set; }
        public static IDialogService DialogService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            PlcService = new PlcService();
            DialogService = new DialogService();
        }
    }
}
