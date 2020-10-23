﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.SAP
{
    class PluginConfig
    {
        private static string pluginname => "SAP";
        private static Config _globallocal = null;
        public static Config globallocal
        {
            get
            {
                if (_globallocal == null)
                {
                    _globallocal = Config.local;
                }
                return _globallocal;
            }
        }
        public static bool auto_launch_sap_bridge { get { return globallocal.GetProperty(pluginname, true); } set { globallocal.SetProperty(pluginname, value); } }
        public static bool record_with_get_element { get { return globallocal.GetProperty(pluginname, false); } set { globallocal.SetProperty(pluginname, value); } }
        public static int bridge_timeout_seconds { get { return globallocal.GetProperty(pluginname, 60); } set { globallocal.SetProperty(pluginname, value); } }
    }
}
