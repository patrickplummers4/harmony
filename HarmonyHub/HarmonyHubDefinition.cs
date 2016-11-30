using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyHub
{
    //this class holds an instance of a Harmony Hub.
    //allows support for more than one hub on your network
    public class HarmonyHubDefinition
    {
        public HarmonyClient client;
        public HarmonyConfigResult harmonyConfig;
        public List<HarmonyActivityCommand> harmonyActivityCommand = new List<HarmonyActivityCommand>();
        public List<HarmonyDeviceCommand> harmonyDeviceCommand = new List<HarmonyDeviceCommand>();
        public string IPAddress;
        public string HubName;

        public HarmonyHubDefinition(HarmonyClient c, HarmonyConfigResult conf, string ip, string name)
        {
            client = c;
            harmonyConfig = conf;
            IPAddress = ip;
            HubName = name;
        }
    }
}
