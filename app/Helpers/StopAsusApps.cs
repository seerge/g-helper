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
            if (!IsStopAsusAppsEnabled())
            {
                return;
            }
            else
            {
                Extra keyb = new();
                if (ProcessHelper.IsUserAdministrator())
                    keyb.ServiesToggle();
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
