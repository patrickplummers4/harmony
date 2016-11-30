using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using HarmonyHub;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using VoxCommando;
using CastleOS_API;
using System.Drawing;
using System.Reflection;

namespace HarmonyService
{    
    public class HarmonyWcfService : IHarmonyWcfService
    {
       

        //returns the current activity by name
        public string CurrentActivity(string hubName)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);


            if (hub != null) 
            { 
                hub.client.GetCurrentActivity();
                // now wait for it to be populated
                DateTime s = DateTime.Now;
                while (string.IsNullOrEmpty(hub.client.CurrentActivity) && s.AddSeconds(10) > DateTime.Now) 
                { 
                    Thread.Sleep(50); 
                }
                return String.Format("{0}", hub.harmonyConfig.ActivityNameFromId(hub.client.CurrentActivity));
            }
            else
            {
                return String.Empty;
            }
        }

        //returns the current activity by ID
        public string CurrentActivityId(string hubName)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);


            if (hub != null)
            {
                hub.client.GetCurrentActivity();
                // now wait for it to be populated
                DateTime s = DateTime.Now;
                while (string.IsNullOrEmpty(hub.client.CurrentActivity) && s.AddSeconds(10) > DateTime.Now)
                {
                    Thread.Sleep(50);
                }
                return String.Format("{0}", hub.client.CurrentActivity);
            }
            else
            {
                return String.Empty;
            }
        }

        //return the device that should perform the passed in command given the passed in activity
        //E.g. In a user's "Watch TV" activity, the TV controls the sound so the "Mute" command plus "Watch TV" activity should return the 
        //device id of the TV
        public string DeviceIdFromActivityCommand(HarmonyHubDefinition hub, string activity, string command)
        {
            string result = String.Empty;

            if (hub.harmonyActivityCommand != null)               
            {
                result = hub.harmonyActivityCommand.FirstOrDefault(hac => hac.ActivityId == activity && hac.CommandId == command).DeviceId;                    
            }
            
            return result;
        }

        //determines the correct button press command to send based on the current hub activity
        //Sending the same params can result in different actions depending on the current activity.
        //Eg. when the "Pause" command is sent for an exmaple user's setup, if the Watch TV is active then send the pressbutton command for the DVR device.
        //For the "Watch a Movie" activity, the device to pause is the Bluray player.
        //This allows the caller to not have to care about devices, it only needs to know commands available.
        public void DoActivityCommand(string hubName, string commandId)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);
            if (hub != null)
            {
                string activityId = CurrentActivityId(hubName);
                if (activityId != "-1")
                {
                    string deviceid = DeviceIdFromActivityCommand(hub, activityId, commandId);

                    PressButton(hubName, deviceid, commandId);
                }
            }
            
        }

        //List of all devices the hub knows about
        public List<Device> Devices(string hubName)
        {
             HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);


             if (hub != null)
             {
                 return hub.harmonyConfig.device;
             }
             else
             {
                 return null;
             }
        }

        //list of all activities defined on the hub
        public List<Activity> Activities(string hubName)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);


            if (hub != null)
            {
                return hub.harmonyConfig.activity;
            }
            else
            {
                return null;
            }
        }

        //Start an activity
        public void StartActivity(string hubName, string activityId)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);
            
            if (hub != null)
            {
                hub.client.StartActivity(activityId);
            }
        }

        //execute a specific button press -- device / command
        public void PressButton(string hubName, string deviceId, string commandId)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);

            if (hub != null)
            {
                hub.client.PressButton(deviceId, commandId);
            }
        }

        //execute the turn all off button for the hub
        public void TurnOff(string hubName)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);

            if (hub != null)
            {
                hub.client.TurnOff();
            }
        }

        //returns the list of commands specifically defined for each activity (lists all activities not just the current one)
        public List<HarmonyActivityCommand> ActivityCommands(string hubName)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);

            if (hub != null)
            {
                return hub.harmonyActivityCommand;
            }
            else
            {
                return null;
            }
        }

        //returns the list of commands available for each device on the hub
        public List<HarmonyDeviceCommand> DeviceCommands(string hubName)
        {
            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);

            if (hub != null)
            {
                return hub.harmonyDeviceCommand;
            }
            else
            {
                return null;
            }
        }

        //get the base url for the server running this service
        public string GetBaseUrl()
        {
            Uri serviceBase = Program.myserviceHost.BaseAddresses[0];
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return String.Format("{0}://{1}:{2}{3}", serviceBase.Scheme, localIP, serviceBase.Port, serviceBase.PathAndQuery);
        }

        //loop through the Activity list and output some html links for handy testing of activity start/end commands
        public System.IO.Stream ActivityTester(string hubName)
        {
            string result = "";
            string url = GetBaseUrl();

            List<Activity> alist = Activities(hubName);

            if (alist != null)
            {

                if (alist.Count == 0)
                {
                    result = "<html><body>No Activities Found.</body><html>";
                }
                else
                {
                    result = "<html><body>";

                    foreach (Activity hac in alist)
                    {
                        result += String.Format("<br><a href='{0}activity/{1}/{2}'>{3}</a>", url, hubName, hac.id, hac.label);
                    }
                }

                result += "</body></html>";

                byte[] resultBytes = Encoding.UTF8.GetBytes(result);
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(resultBytes);
            }
            else
            {
                return null;
            }
        }
            

        //loop through the harmonyActivityCommand list and output some html links for handy testing of commands defined by activity
        public System.IO.Stream ActivityCommandTester(string hubName)
        {
            string result = "";
            string url = GetBaseUrl();

            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);

            if (hub != null)
            {

                if (hub.harmonyActivityCommand == null)
                {
                    result = "<html><body>No Activity Commands Found.</body><html>";
                }
                else
                {
                    result = "<html><body>";

                    foreach (HarmonyActivityCommand hac in hub.harmonyActivityCommand)
                    {
                        result += String.Format("<br><a href='{0}PressButton/{5}/{1}/{2}'>{3}-{4}</a>", url, hac.DeviceId, hac.CommandId, hac.ActivityLabel, hac.CommandLabel, hubName);
                    }
                }

                result += "</body></html>";

                byte[] resultBytes = Encoding.UTF8.GetBytes(result);
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(resultBytes);
            }
            else
            {
                return null;
            }
        }

        //loop through the devices list and output some html links for handy testing of misc button commands (all commands for all devices)
        public System.IO.Stream DeviceCommandTester(string hubName)
        {
            string result = "";           

            string url = GetBaseUrl();

            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);

            if (hub != null)
            {
                if (hub.harmonyDeviceCommand == null)
                {
                    result = "<html><body>No Device Commands Found.</body><html>";
                }
                else
                {
                    result = "<html><body>";

                    foreach (HarmonyDeviceCommand hac in hub.harmonyDeviceCommand)
                    {
                        result += String.Format("<br><a href='{0}PressButton/{5}/{1}/{2}'>{3}-{4}</a>", url, hac.DeviceId, hac.CommandToSend, hac.DeviceLabel, hac.CommandLabel, hubName);
                    }
                }

                result += "</body></html>";

                byte[] resultBytes = Encoding.UTF8.GetBytes(result);
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(resultBytes);
            }
            else
            {
                return null;
            }
        }

        //generates a VoxCommando commpatible XML file containting voice commands for various Harmony Hub control scenarios, including:
        //   *  Start activities
        //   *  Press specific device command buttons
        //   *  Do a command based on the current activity
        public VoiceCommands GenerateVoxCommandoCommands(string hubName, string commandStartID)
        {
            int startID = 1;
            int.TryParse(commandStartID, out startID);
            string url = GetBaseUrl();

            string prefixPhrase = Program.PrefixPhrase;

            HarmonyHubDefinition hub = Program.HubList.FirstOrDefault(h => h.HubName == hubName);

            if (hub != null)
            {
                //top level of the xml file
                //voicecommand attributes
                VoiceCommands vc = new VoiceCommands()
                {
                    version = "2.1.4.8",
                    space = "preserve",
                    date = "5/1/2016 12:00:00 AM"
                };

                //group collection attributes -- only one collection per hub
                vc.groupCollection = new VoiceCommandsGroupCollection()
                {
                    open = "True",
                    name = "Harmony Hub - " + hubName
                };

                //command groups 
                //we're making a command group for starting activities, a group for distinct activity commands,  plus groups for each device/command type combo
                //eg TV + numeric commands
                List<VoiceCommandsGroupCollectionCommandGroup> cg = new List<VoiceCommandsGroupCollectionCommandGroup>();

                #region Commands to Start Activities
                //activities
                VoiceCommandsGroupCollectionCommandGroup cgActivities = new VoiceCommandsGroupCollectionCommandGroup()
                {
                    open = "True",
                    name = "Harmony Activities",
                    enabled = "True",
                    prefix = "",
                    requiredProcess = "",
                    priority = 0,
                    description = ""
                };

                //do the commands
                List<VoiceCommandsGroupCollectionCommandGroupCommand> acmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();

                foreach (Activity a in hub.harmonyConfig.activity)
                {
                    VoiceCommandsGroupCollectionCommandGroupCommand acmd = new VoiceCommandsGroupCollectionCommandGroupCommand()
                        {
                            id = startID++,
                            name = "Start Activity " + a.label,
                            enabled = true,
                            alwaysOn = "False",
                            confirm = "False",
                            requiredConfidence = 0,
                            loop = "False",
                            loopDelay = 0,
                            loopMax = 0,
                            description = ""
                        };
                    VoiceCommandsGroupCollectionCommandGroupCommandAction ca = new VoiceCommandsGroupCollectionCommandGroupCommandAction();
                    ca.cmdType = "Scrape";
                    string[] p = new string[4];
                    p[0] = GetBaseUrl() + String.Format("activity/{1}/{0}", a.id, hubName);
                    ca.@params = p;
                    ca.cmdRepeat = 1;

                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph1 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                    ph1.Value = prefixPhrase;
                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph2 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                    ph2.Value = a.label;

                    acmd.Items = new object[3] { ca, ph1, ph2 };

                    acmd.ItemsElementName = new ItemsChoiceType[3] { ItemsChoiceType.action, ItemsChoiceType.phrase, ItemsChoiceType.phrase };

                    acmds.Add(acmd);
                }
                cgActivities.command = acmds.ToArray();
                cg.Add(cgActivities);
                #endregion

                #region Commands to Execute based on current activity

                //these commands are the unique command names for all activities
                //e.g. the "pause" command may be an activity command for multiple activities like Watch TV or Watch Movie
                //VoxCommando will call the service and let it figure out how this maps to a specific device.
                //This allows the voice commands to be shorter -- you can just say "Harmony Pause" rather than
                //"Press button blueray pause"

                //activities
                VoiceCommandsGroupCollectionCommandGroup cgActivityCommand = new VoiceCommandsGroupCollectionCommandGroup()
                {
                    open = "True",
                    name = "Harmony Activity Commands",
                    enabled = "True",
                    prefix = "",
                    requiredProcess = "",
                    priority = 0,
                    description = ""
                };

                //do the commands
                List<VoiceCommandsGroupCollectionCommandGroupCommand> accmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();

                foreach (string commandId in hub.harmonyActivityCommand.Select(x => x.CommandId).Distinct())
                {
                    VoiceCommandsGroupCollectionCommandGroupCommand acmd = new VoiceCommandsGroupCollectionCommandGroupCommand()
                    {
                        id = startID++,
                        name = "Do Activity Command" + commandId,
                        enabled = true,
                        alwaysOn = "False",
                        confirm = "False",
                        requiredConfidence = 0,
                        loop = "False",
                        loopDelay = 0,
                        loopMax = 0,
                        description = ""
                    };
                    VoiceCommandsGroupCollectionCommandGroupCommandAction ca = new VoiceCommandsGroupCollectionCommandGroupCommandAction();
                    ca.cmdType = "Scrape";
                    string[] p = new string[4];
                    p[0] = GetBaseUrl() + String.Format("doactivitycommand/{1}/{0}", commandId, hubName);
                    ca.@params = p;
                    ca.cmdRepeat = 1;

                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph1 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                    ph1.Value = prefixPhrase;
                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph2 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();

                    //sort of a kludge to detect number commands and convert them to works, e.g. 1 = "one"
                    int num = -1;
                    if (Regex.IsMatch(commandId, "Number[0-9]"))
                    {
                        
                            if (int.TryParse(commandId.Substring(6, 1), out num))
                            {
                                ph2.Value = NumberToWords(num);
                            }
                            else
                            {
                                ph2.Value = commandId;
                            }                                                 

                    }
                    else 
                    { 
                        ph2.Value = commandId;
                    }

                    acmd.Items = new object[3] { ca, ph1, ph2 };

                    acmd.ItemsElementName = new ItemsChoiceType[3] { ItemsChoiceType.action, ItemsChoiceType.phrase, ItemsChoiceType.phrase };

                    accmds.Add(acmd);
                }
                cgActivityCommand.command = accmds.ToArray();
                cg.Add(cgActivityCommand);

                #endregion

                #region Commands to Press Buttons by Device and Command, grouped by type of command (command group)
                //create device command groups by command types (as defined by the hub)  
                string lastgrouplabel = "";
                bool isFirst = true;
                VoiceCommandsGroupCollectionCommandGroup cgDev = new VoiceCommandsGroupCollectionCommandGroup();
                List<VoiceCommandsGroupCollectionCommandGroupCommand> dcmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();
                foreach (HarmonyDeviceCommand hac in hub.harmonyDeviceCommand)
                {
                    //create a command group for each hub controlGroup
                    //Control groups for the hub are like Numeric, Navigation, MediaControl, etc
                    if (lastgrouplabel != hac.controlGroupLabel)
                    {

                        if (!isFirst)
                        {
                            cgDev.command = dcmds.ToArray();
                            cg.Add(cgDev);
                            //reinit the commands collection
                            dcmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();
                        }
                        else
                        {
                            isFirst = false;
                        }

                        cgDev = new VoiceCommandsGroupCollectionCommandGroup()
                        {
                            open = "True",
                            name = String.Format("Harmony Dev Cmd - {1} - {0}", hac.controlGroupLabel, hac.DeviceLabel),
                            enabled = "True",
                            prefix = "",
                            requiredProcess = "",
                            priority = 0,
                            description = ""
                        };

                        lastgrouplabel = hac.controlGroupLabel;
                    }

                    //do the commands               

                    VoiceCommandsGroupCollectionCommandGroupCommand dcmd = new VoiceCommandsGroupCollectionCommandGroupCommand()
                    {
                        id = startID++,
                        name = hac.CommandLabel,
                        enabled = true,
                        alwaysOn = "False",
                        confirm = "False",
                        requiredConfidence = 0,
                        loop = "False",
                        loopDelay = 0,
                        loopMax = 0,
                        description = ""
                    };
                    VoiceCommandsGroupCollectionCommandGroupCommandAction ca = new VoiceCommandsGroupCollectionCommandGroupCommandAction();
                    ca.cmdType = "Scrape";
                    string[] p = new string[4];
                    p[0] = String.Format("{0}pressbutton/{3}/{1}/{2}", url, hac.DeviceId, hac.CommandToSend, hubName);
                    ca.@params = p;
                    ca.cmdRepeat = 1;

                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph0 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                    ph0.Value = prefixPhrase;
                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph1 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                    ph1.Value = "Press Button";
                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph2 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                    ph2.Value = hac.DeviceLabel;
                    VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph3 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                    ph3.Value = hac.CommandLabel;

                    dcmd.Items = new object[5] { ca, ph0, ph1, ph2, ph3 };
                    dcmd.ItemsElementName = new ItemsChoiceType[5] { ItemsChoiceType.action, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase };
                    dcmds.Add(dcmd);

                }

                //add last command group
                cgDev.command = dcmds.ToArray();
                cg.Add(cgDev);
                #endregion


                //assign the command group list to an array 
                vc.groupCollection.commandGroup = cg.ToArray<VoiceCommandsGroupCollectionCommandGroup>();

                return vc;
            }
            else
            {
                return null;
            }
            
        }

        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        //generates a VoxCommando commpatible XML file containting voice commands for various CastleOS control scenarios, including:
        //   *  Turn on / off lights,outlets, fans
        //   *  Set brightness (dimmers)
        //   *  Change colors on LEDs
        public VoiceCommands GenerateVoxCommandoCommands_CastleOS(string commandStartID)
        {
            int startID = 1;
            int.TryParse(commandStartID, out startID);
            string url = GetBaseUrl();

            string prefixPhrase = Program.CastleOSPrefixPhrase;

            
            //top level of the xml file
            //voicecommand attributes
            VoiceCommands vc = new VoiceCommands()
            {
                version = "2.1.4.8",
                space = "preserve",
                date = "5/1/2016 12:00:00 AM"
            };

            //group collection attributes -- only one collection per hub
            vc.groupCollection = new VoiceCommandsGroupCollection()
            {
                open = "True",
                name = "CastleOS"
            };

            //command groups 
            //we're making a command group for switches, a group for dimmers,  plus groups scenes                
            List<VoiceCommandsGroupCollectionCommandGroup> cg = new List<VoiceCommandsGroupCollectionCommandGroup>();

            #region Commands to Turn on and off stuff
               
            VoiceCommandsGroupCollectionCommandGroup cgActivities = new VoiceCommandsGroupCollectionCommandGroup()
            {
                open = "True",
                name = "CastleOS Switches On and Off",
                enabled = "True",
                prefix = "",
                requiredProcess = "",
                priority = 0,
                description = ""
            };

            //do the commands
            List<VoiceCommandsGroupCollectionCommandGroupCommand> acmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();

            List<AutomationDevice> devices = Program.cWrapper.GetDevices(Program.CastleOSUser, Program.CastleOSToken);

                
            foreach (AutomationDevice d in devices)
            {
                //turn on
                VoiceCommandsGroupCollectionCommandGroupCommand acmd = new VoiceCommandsGroupCollectionCommandGroupCommand()
                {
                    id = startID++,
                    name = "Turn on off - " + d.name,
                    enabled = true,
                    alwaysOn = "False",
                    confirm = "False",
                    requiredConfidence = 0,
                    loop = "False",
                    loopDelay = 0,
                    loopMax = 0,
                    description = ""
                };
                VoiceCommandsGroupCollectionCommandGroupCommandAction ca = new VoiceCommandsGroupCollectionCommandGroupCommandAction();
                ca.cmdType = "Scrape";
                string[] p = new string[4];
                p[0] = GetBaseUrl() + String.Format("switch/{0}/{1}", d.uniqueId, "{1}");
                ca.@params = p;
                ca.cmdRepeat = 1;

                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph1 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph1.Value = prefixPhrase;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph2 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph2.Value = "Turn";
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph3 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph3.Value = d.name;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph4 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph4.Value = "light,outlet,fan";
                ph4.optional = true;
                ph4.optionalSpecified = true;
                string ph5 = "on,off";

                acmd.Items = new object[6] { ca, ph1, ph2, ph3, ph4, ph5 };

                acmd.ItemsElementName = new ItemsChoiceType[6] { ItemsChoiceType.action, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.payloadList };

                acmds.Add(acmd);

                
            }
            cgActivities.command = acmds.ToArray();
            cg.Add(cgActivities);

            #endregion

            #region Commands for dimming capable lights

            VoiceCommandsGroupCollectionCommandGroup cgDimmers = new VoiceCommandsGroupCollectionCommandGroup()
            {
                open = "True",
                name = "CastleOS Lights Dim",
                enabled = "True",
                prefix = "",
                requiredProcess = "",
                priority = 0,
                description = ""
            };

            //do the commands
            List<VoiceCommandsGroupCollectionCommandGroupCommand> dcmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();

            //List<AutomationDevice> devices = Program.cWrapper.GetDevices(Program.CastleOSUser, Program.CastleOSToken);
            List<AutomationDevice> dimmers = devices.FindAll(d => d.dimmer == "1");

            foreach (AutomationDevice d in dimmers)
            {
                //dim
                VoiceCommandsGroupCollectionCommandGroupCommand dcmd = new VoiceCommandsGroupCollectionCommandGroupCommand()
                {
                    id = startID++,
                    name = "Dim - " + d.name,
                    enabled = true,
                    alwaysOn = "False",
                    confirm = "False",
                    requiredConfidence = 0,
                    loop = "False",
                    loopDelay = 0,
                    loopMax = 0,
                    description = ""
                };
                VoiceCommandsGroupCollectionCommandGroupCommandAction ca = new VoiceCommandsGroupCollectionCommandGroupCommandAction();
                ca.cmdType = "Scrape";
                string[] p = new string[4];
                p[0] = GetBaseUrl() + String.Format("dim/{0}/{1}", d.uniqueId, "{1}");
                ca.@params = p;
                ca.cmdRepeat = 1;

                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph1 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph1.Value = prefixPhrase;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph2 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph2.Value = "Dim,turn down,raise,brighten";
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph3 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph3.Value = d.name;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph4 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph4.Value = "to";
                ph4.optional = true;
                ph4.optionalSpecified = true;
                //payloadRange
                string ph5 = "0,99";
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph6 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph6.Value = "percent";
                ph6.optional = true;
                ph6.optionalSpecified = true;

                dcmd.Items = new object[7] { ca, ph1, ph2, ph3, ph4, ph5, ph6 };

                dcmd.ItemsElementName = new ItemsChoiceType[7] { ItemsChoiceType.action, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.payloadRange, ItemsChoiceType.phrase };

                dcmds.Add(dcmd);
            }

            cgDimmers.command = dcmds.ToArray();
            cg.Add(cgDimmers);

            #endregion

            #region Commands to Activate Scenes             

            VoiceCommandsGroupCollectionCommandGroup cgScenes = new VoiceCommandsGroupCollectionCommandGroup()
            {
                open = "True",
                name = "CastleOS Scenes",
                enabled = "True",
                prefix = "",
                requiredProcess = "",
                priority = 0,
                description = ""
            };

            //do the commands
            List<VoiceCommandsGroupCollectionCommandGroupCommand> scmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();

            List<Scene> scenes = Program.cWrapper.GetScenes(Program.CastleOSUser, Program.CastleOSToken);
            
            foreach (Scene s in scenes)
            {                
                VoiceCommandsGroupCollectionCommandGroupCommand scmd = new VoiceCommandsGroupCollectionCommandGroupCommand()
                {
                    id = startID++,
                    name = "Toggle Scene - " + s.sceneName,
                    enabled = true,
                    alwaysOn = "False",
                    confirm = "False",
                    requiredConfidence = 0,
                    loop = "False",
                    loopDelay = 0,
                    loopMax = 0,
                    description = ""
                };
                VoiceCommandsGroupCollectionCommandGroupCommandAction ca = new VoiceCommandsGroupCollectionCommandGroupCommandAction();
                ca.cmdType = "Scrape";
                string[] p = new string[4];
                p[0] = GetBaseUrl() + String.Format("togglescene/{0}/{1}", s.sceneValue, "{1}");
                ca.@params = p;
                ca.cmdRepeat = 1;

                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph1 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph1.Value = prefixPhrase;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph2 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph2.Value = "Turn";
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph3 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph3.Value = s.sceneName;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph4 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph4.Value = "to";
                ph4.optional = true;
                ph4.optionalSpecified = true;
                //payloadList
                string ph5 = "on,off";
                
                scmd.Items = new object[6] { ca, ph1, ph2, ph3, ph4, ph5 };

                scmd.ItemsElementName = new ItemsChoiceType[6] { ItemsChoiceType.action, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.payloadList };

                scmds.Add(scmd);
            }

            cgScenes.command = scmds.ToArray();
            cg.Add(cgScenes);
            #endregion

            #region Commands to Change bulb colors

            //string[] colors = Enum.GetNames(typeof(KnownColor));
            //string payloadColorNames = string.Join(",", colors);
            Type colorType = typeof(System.Drawing.Color);

            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);

            var colors = from c in propInfoList
                         select c.Name;

            string payloadColorNames = string.Join(",", colors);
        

            VoiceCommandsGroupCollectionCommandGroup cgColor = new VoiceCommandsGroupCollectionCommandGroup()
            {
                open = "True",
                name = "CastleOS Color Setting",
                enabled = "True",
                prefix = "",
                requiredProcess = "",
                priority = 0,
                description = ""
            };

            //do the commands
            List<VoiceCommandsGroupCollectionCommandGroupCommand> ccmds = new List<VoiceCommandsGroupCollectionCommandGroupCommand>();

            
            List<AutomationDevice> cdevices = devices.FindAll(d => d.canChangeColor == true);

            foreach (AutomationDevice d in cdevices)
            {
                //dim
                VoiceCommandsGroupCollectionCommandGroupCommand ccmd = new VoiceCommandsGroupCollectionCommandGroupCommand()
                {
                    id = startID++,
                    name = "Change Bulb Color - " + d.name,
                    enabled = true,
                    alwaysOn = "False",
                    confirm = "False",
                    requiredConfidence = 0,
                    loop = "False",
                    loopDelay = 0,
                    loopMax = 0,
                    description = ""
                };
                VoiceCommandsGroupCollectionCommandGroupCommandAction ca = new VoiceCommandsGroupCollectionCommandGroupCommandAction();
                ca.cmdType = "Scrape";
                string[] p = new string[4];
                p[0] = GetBaseUrl() + String.Format("setcolor/{0}/{1}", d.uniqueId, "{1}");
                ca.@params = p;
                ca.cmdRepeat = 1;

                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph1 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph1.Value = prefixPhrase;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph2 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph2.Value = "Turn,Set";
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph3 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph3.Value = d.name;
                VoiceCommandsGroupCollectionCommandGroupCommandPhrase ph4 = new VoiceCommandsGroupCollectionCommandGroupCommandPhrase();
                ph4.Value = "to";
                ph4.optional = true;
                ph4.optionalSpecified = true;
                //payloadList
                string ph5 = payloadColorNames;

                ccmd.Items = new object[6] { ca, ph1, ph2, ph3, ph4, ph5 };

                ccmd.ItemsElementName = new ItemsChoiceType[6] { ItemsChoiceType.action, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.phrase, ItemsChoiceType.payloadList };

                ccmds.Add(ccmd);
            }

            cgColor.command = ccmds.ToArray();
            cg.Add(cgColor);

            #endregion


            //assign the command group list to an array 
            vc.groupCollection.commandGroup = cg.ToArray<VoiceCommandsGroupCollectionCommandGroup>();

            return vc;
            
        }

        public void Switch(string deviceID, string state)
        {
            bool power = false;
            if (state == "on") 
            {
                power = true;
            }

            Program.cWrapper.Switch(deviceID, power, Program.CastleOSUser, Program.CastleOSPassword);
        }

        public void ToggleScene(string id, string state)
        {
            bool power = false;
            if (state == "on")
            {
                power = true;
            }

            Program.cWrapper.ToggleScene(id, power, Program.CastleOSUser, Program.CastleOSPassword);
        }

        public void Dim(string id, string level)
        {
            int ilevel = 99;
            int.TryParse(level, out ilevel);

            Program.cWrapper.Dim(id, ilevel, Program.CastleOSUser, Program.CastleOSPassword);
        }

        public void SetColor(string deviceId, string colorname) 
        {
            //get the color from the name
            Color color = Color.FromName(colorname);

            //if the RGB is 0 then the color was not found so don't do anything.
            if (!(color.R == 0 && color.G == 0 && color.B == 0))
            {
                double hue = color.GetHue();
                double saturation = color.GetSaturation();
                double brightness = color.GetBrightness();

                Program.cWrapper.SetColor(deviceId, hue, saturation, brightness, Program.CastleOSUser, Program.CastleOSPassword);

            }
        }
    }
}
