using System.IO;
using CommandLine;
using CommandLine.Text;
using HarmonyHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace HarmonyConsole
{
    class Options
    {
        [Option('i', "ip", Required = true,
            HelpText = "IP Address of Harmony Hub.")]
        public string IpAddress { get; set; }

        [Option('u', "user", Required = true,
            HelpText = "Logitech Username")]
        public string Username { get; set; }

        [Option('p', "pass", Required = true,
            HelpText = "Logitech Password")]
        public string Password { get; set; }

        [Option('d', "device", Required = false,
            HelpText = "Device ID")]
        public string DeviceId { get; set; }

        [Option('c', "command", Required = false,
            HelpText = "Command to send to device")]
        public string Command { get; set; }

        [Option('a', "activity", Required = false,
            HelpText = "Activity ID to start")]
        public string ActivityId { get; set; }

        [Option('l', "list", Required = false,
            HelpText = "List devices or activities. d = devices; a = activities; ac = commands within an activity, e.g. -l ac ")]
        public string ListType { get; set; }

        [Option('g', "get", Required = false,
            HelpText = "Get Current Activity")]
        public bool GetActivity { get; set; }

        [Option('o', "off", Required = false,
            HelpText = "Turn entire system off")]
        public bool TurnOff { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const int harmonyPort = 5222;

            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine();

                string ipAddress = options.IpAddress;
                string username = options.Username;
                string password = options.Password;

                string deviceId = options.DeviceId;
                string activityId = options.ActivityId;

                //Dns.GetHostEntry(ipAddress);

                string sessionToken;

                if (File.Exists("SessionToken_" + ipAddress))
                {
                    sessionToken = File.ReadAllText("SessionToken_" + ipAddress);
                    Console.WriteLine("Reusing token: {0}", sessionToken);
                }
                else
                {
                    sessionToken = LoginToLogitech(username, password, ipAddress, harmonyPort);
                }

                // do we need to grab the config first?
                HarmonyConfigResult harmonyConfig = null;

                HarmonyClient client = null;

                if (!string.IsNullOrEmpty(deviceId) || options.GetActivity || !string.IsNullOrEmpty(options.ListType))
                {
                    client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);

                    if (!File.Exists("HubConfig_" + ipAddress))
                    {
                        client.GetConfig();
                        while (string.IsNullOrEmpty(client.Config)) { }
                        File.WriteAllText("HubConfig_" + ipAddress, client.Config);
                        harmonyConfig = new JavaScriptSerializer().Deserialize<HarmonyConfigResult>(client.Config);
                    }
                    else
                    {
                        string hubconfig = File.ReadAllText("HubConfig_" + ipAddress);

                        harmonyConfig = new JavaScriptSerializer().Deserialize<HarmonyConfigResult>(hubconfig);
                    }                  
                }                               

                if (!string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(options.Command))
                {
                    if (null == client) client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);
                    //activityClient.PressButton("14766260", "Mute");
                    client.PressButton(deviceId, options.Command);
                }

                if (null != harmonyConfig && !string.IsNullOrEmpty(deviceId) && string.IsNullOrEmpty(options.Command))
                {
                    // just list device control options
                    foreach (var device in harmonyConfig.device.Where(device => device.id == deviceId))
                    {
                        //foreach (Dictionary<string, object> controlGroup in device.controlGroup)
                        foreach (ControlGroup controlGroup in device.controlGroup)
                        {
                            //foreach (var o in controlGroup.Where(o => o.Key == "name"))
                            //{
                            //    Console.WriteLine("\n");
                            //    Console.WriteLine("{0}:{1}", o.Key, o.Value);                                
                            //}
                            Console.WriteLine("{0}", controlGroup.name);

                            //foreach (var o2 in controlGroup.Where(o2 => o2.Key == "function"))
                            foreach (Function func in controlGroup.function)
                            {                           
                                //foreach (Dictionary<string, object> cmd in (object[])o2.Value)
                                //{
                                //    Console.WriteLine("{0}:{1}", cmd.Single(c => c.Key == "name").Value, cmd.Single(c => c.Key == "label").Value); // ((Dictionary<string, object>)((object[])o2.Value)[1]).Single(c => c.Key == "name"));
                                //}
                                Console.WriteLine("{0}:{1}", func.name, func.label);
                            }
                            
                        }
                    }
                }

                //get device/commands by activity
                //((object[])((Dictionary<string,object>)harmonyConfig.activity[0].controlGroup[0]).ToList()[1].Value)[1]
                //if (null != harmonyConfig && !string.IsNullOrEmpty(activityId) && string.IsNullOrEmpty(options.Command))
                //{                    
                    
                //}

                if (!string.IsNullOrEmpty(activityId))
                {
                    if (null == client) client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);
                    client.StartActivity(activityId);
                }

                if (null != harmonyConfig && options.GetActivity)
                {
                    client.GetCurrentActivity();
                    // now wait for it to be populated
                    while (string.IsNullOrEmpty(client.CurrentActivity)) { }
                    Console.WriteLine("Current Activity: {0}", harmonyConfig.ActivityNameFromId(client.CurrentActivity));
                }

                if (options.TurnOff)
                {
                    if (null == client) client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);
                    client.TurnOff();
                }

                if (null != harmonyConfig && !string.IsNullOrEmpty(options.ListType))
                {
                    //if (!options.ListType.Equals("d") && !options.ListType.Equals("a")) return;

                    //list the activities defined in Harmony Hub
                    if (options.ListType.Equals("a"))
                    {
                        Console.WriteLine("Activities:");
                        harmonyConfig.activity.Sort();
                        foreach (var activity in harmonyConfig.activity)
                        {
                            Console.WriteLine(" {0}:{1}", activity.id, activity.label);
                        }
                    }

                    //list the devices Harmony knows about
                    if (options.ListType.Equals("d"))
                    {
                        Console.WriteLine("Devices:");
                        harmonyConfig.device.Sort();
                        foreach (var device in harmonyConfig.device)
                        {
                            Console.WriteLine(" {0}:{1}", device.id, device.label);
                        }
                    }

                    //List the commands associated with each activity
                    if (options.ListType.Equals("ac"))
                    {
                        foreach (var activity in harmonyConfig.activity)
                        {
                            Console.WriteLine(activity.label);

                            //each activity has multiple control Groups that represent groups of commands, eg Numeric Buttons, Nav buttons (arrows), Media Control type (play, ff rwd etc)
                            //foreach (Dictionary<string, object> controlGroup in activity.controlGroup)
                            foreach (ControlGroup controlGroup in activity.controlGroup)
                            {

                                //each controlGroup name
                                //foreach (var o in controlGroup.Where(o => o.Key == "name"))
                                //{
                                //    Console.WriteLine("\n");
                                //    Console.WriteLine("{0}:{1}", o.Key, o.Value);
                                //}
                                Console.WriteLine("\n");
                                Console.WriteLine("{0}", controlGroup.name);

                                //list the functions for each controlGroup
                                //the actual command you can send back to the hub is first and a more readable label is second, eg.
                                //VolumeUp :  Volume Up
                                //foreach (var o2 in controlGroup.Where(o2 => o2.Key == "function"))
                                foreach (Function func in controlGroup.function)
                                {
                                    //foreach (Dictionary<string, object> cmd in (object[])o2.Value)
                                    //{
                                    //    //cmd.ToList()[0].Value
                                        
                                    //    Console.WriteLine("{0}:{1}", cmd.Single(c => c.Key == "name").Value, cmd.Single(c => c.Key == "label").Value); // ((Dictionary<string, object>)((object[])o2.Value)[1]).Single(c => c.Key == "name"));
                                    //}
                                    Console.WriteLine("{0}:{1}", func.name, func.label);

                                    func.harmonyAction = new JavaScriptSerializer().Deserialize<HarmonyAction>(func.action);
                                }

                            }
                        }
                    }
                }

            } // option parsing

           
        }

        public static string LoginToLogitech(string email, string password, string ipAddress, int harmonyPort)
        {
            string userAuthToken = HarmonyLogin.GetUserAuthToken(email, password);
            if (string.IsNullOrEmpty(userAuthToken))
            {
                throw new Exception("Could not get token from Logitech server.");
            }

            File.WriteAllText("UserAuthToken_" + ipAddress, userAuthToken);

            var authentication = new HarmonyAuthenticationClient(ipAddress, harmonyPort);

            string sessionToken = authentication.SwapAuthToken(userAuthToken);
            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new Exception("Could not swap token on Harmony Hub.");
            }

            File.WriteAllText("SessionToken_" + ipAddress, sessionToken);

            Console.WriteLine("Date Time : {0}", DateTime.Now);
            Console.WriteLine("User Token: {0}", userAuthToken);
            Console.WriteLine("Sess Token: {0}", sessionToken);

            return sessionToken;
        }
    }
}
