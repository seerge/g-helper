using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHelper.Helpers
{
    internal class StopAsusApps
    {
        public StopAsusApps() 
        {
            int servicesCount = OptimizationService.GetRunningCount();
            if (!IsStopAsusAppsEnabled())
            {
                return;
            }
            else if (servicesCount > 0)
            {
                Extra keyb = new();
                if (ProcessHelper.IsUserAdministrator() &&
                    OptimizationService.IsRunning())
                    keyb.ServicesToggle();
                else
                    ProcessHelper.RunAsAdmin("services");
            }
        }
        private bool IsStopAsusAppsEnabled()
        {
            return AppConfig.Is("toggle_stopasusapps_mode");
        }
    }
}
