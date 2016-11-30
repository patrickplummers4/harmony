using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace HarmonyService
{
    [RunInstaller(true)]
    public partial class HarmonyWindowsServiceInstaller : Installer
    {
        public HarmonyWindowsServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.DisplayName = "Harmony Hub Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "HarmonyHubService";
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }

}