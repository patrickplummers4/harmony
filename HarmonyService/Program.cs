using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;
using System.ServiceProcess;
using System.Diagnostics;
using HarmonyHub;
using System.IO;
using System.Web.Script.Serialization;
using VoxCommando;

namespace HarmonyService
{
    class Program : ServiceBase
    {
        public static ServiceHost myserviceHost = null;
        public static bool console = false;
        public static bool IsRunning = true;
        public static string ExecutableDirectory = "";
        public static bool IsTerminated = false;
        public static EventLog MainEventLog;
        public const int harmonyPort = 5222;

        public static List<HarmonyHubDefinition> HubList = new List<HarmonyHubDefinition>();
        //public static List<HarmonyClient> clients = new List<HarmonyClient>();        
        //public static HarmonyConfigResult harmonyConfig = null;
        //public static List<HarmonyActivityCommand> harmonyActivityCommand = new List<HarmonyActivityCommand>();
        //public static List<HarmonyDeviceCommand> harmonyDeviceCommand = new List<HarmonyDeviceCommand>();

        public static string PrefixPhrase;
        public static string CastleOSPrefixPhrase;

        public static string CastleOSToken;
        public static string CastleOSUser;
        public static string CastleOSPassword;

        public static CastleOSWrapper cWrapper;

        internal System.Timers.Timer timerKeepAlive;
        public static DateTime dtLastConnect = DateTime.Now;
        public static DateTime dtLastConnectCastleOS = DateTime.Now;

        static void Main(string[] args)
        {

            //run as service
            if (!args.Contains("console"))
            {
                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[] 
		        { 
                    new Program(false),
		        };
                ServiceBase.Run(ServicesToRun);
            }
            //run as console app
            else
            {
                Program svcHarmony = new Program(true);
            }
        }

        public Program(bool bconsole)
        {
            console = bconsole;
            IsRunning = true;

            //string ipAddress = ConfigurationManager.AppSettings["harmonyIP"].ToString();
            List<string> ipAddresses = new List<string>(ConfigurationManager.AppSettings["harmonyIP"].Split(new char[] { ',' }));
            List<string> hubNames = new List<string>(ConfigurationManager.AppSettings["harmonyName"].Split(new char[] { ',' }));
            string username = ConfigurationManager.AppSettings["harmonyUser"].ToString();
            string password = ConfigurationManager.AppSettings["harmonyPassword"].ToString();
            PrefixPhrase = ConfigurationManager.AppSettings["harmonyPrefixPhrase"].ToString();
            CastleOSPrefixPhrase = ConfigurationManager.AppSettings["CastleOSPrefixPhrase"].ToString();

            //set castleos connection castleos
            cWrapper = new CastleOSWrapper();
            CastleOSUser = ConfigurationManager.AppSettings["CastleOSName"].ToString();
            CastleOSPassword =  ConfigurationManager.AppSettings["CastleOSPassword"].ToString();

            try
            {
                if (CastleOSUser != "")
                {
                    CastleOSToken = cWrapper.AuthenticateUser(CastleOSUser, CastleOSPassword);
                }
            }
            catch { }

            for (int i = 0; i<ipAddresses.Count; i++)
            {
                string ipAddress = ipAddresses[i];
                string hubName = hubNames[i];
                HarmonyHubDefinition newhub;
                HarmonyClient client = null;

                string sessionToken;

                if (File.Exists("SessionToken_" + ipAddress))
                {
                    sessionToken = File.ReadAllText("SessionToken_" + ipAddress);
                    Console.WriteLine("Reusing token: {0}", sessionToken);
                }
                else
                {
                    sessionToken = HarmonyLogin.LoginToLogitech(username, password, ipAddress, harmonyPort);
                }

                client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);

                HarmonyConfigResult harmonyConfig;
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

                newhub = new HarmonyHubDefinition(client, harmonyConfig, ipAddress, hubName);

                CreateActivityCommandList(harmonyConfig, newhub.harmonyActivityCommand);

                CreateDeviceCommandList(harmonyConfig, newhub.harmonyDeviceCommand);

                HubList.Add(newhub);
            }
            

            Thread thread = new Thread(ProgramActions);
            thread.Start();

            dtLastConnect = DateTime.Now;
            dtLastConnectCastleOS = DateTime.Now;
            timerKeepAlive = new System.Timers.Timer(5000);
            timerKeepAlive.Elapsed += new System.Timers.ElapsedEventHandler(timerKeepAliveElapsed);

            if (CastleOSUser != "")
            {
                timerKeepAlive.Enabled = true;
            }

        }

        private void timerKeepAliveElapsed(object sender, EventArgs e)
        {
            try
            {
                //keep alive for CastleOS
                if (CastleOSUser != "")
                {
                    if (dtLastConnectCastleOS.AddMinutes(2) < DateTime.Now)
                    {
                        Program.cWrapper.GetAppVersion();
                        dtLastConnectCastleOS = DateTime.Now;
                    }
                }
                else
                {
                    timerKeepAlive.Enabled = false;
                    timerKeepAlive.Stop();
                }
            }
            catch 
            {  //ignore because this means the service isn't there
                timerKeepAlive.Enabled = false;
                timerKeepAlive.Stop();

            }
        }

        //Create a flat searchable list of commands available by activity
        //Note about how harmony hub deals with multiple commands of the same name:
        /* There can be many potential devices registered that support a given command like VolumeUp.  When a user of the regular remote hits that
         * button, the hub/remote needs to figure out what that means.  It does this based on the current Activity by default and custom mappings secondly.
         * For the REST interface it will be convenient to have the service do the same thing to simplify calls.  For example you should be able to 
         * call the service with just a command and the service will look up what device id it should send to the hub with that command based on the
         * current activity (e.g. Watch TV).   If the command is not found in the current activity, it will look for the command in it's list of devices.
         * If this ends up not being the correct behavior for your implementation then you'll want to call the /pressbutton/{deviceid}/command/{command} directly.
         * */
        public void CreateActivityCommandList(HarmonyConfigResult harmonyConfig, List<HarmonyActivityCommand> harmonyActivityCommand)
        {
            foreach (var activity in harmonyConfig.activity)
            {  
                //each activity has multiple control Groups that represent groups of commands, eg Numeric Buttons, Nav buttons (arrows), Media Control type (play, ff rwd etc)                
                foreach (ControlGroup controlGroup in activity.controlGroup)
                {
                    //list the functions for each controlGroup
                    //the actual command you can send back to the hub is buried in the HarmonyAction part of the function
                    //this is invalid json so you have to deserialize it in a second pass after the main HarmonyConfigResults deserialization that happens
                    //at the service launch
                    foreach (Function func in controlGroup.function)
                    {                  
                        func.harmonyAction = new JavaScriptSerializer().Deserialize<HarmonyAction>(func.action);

                        HarmonyActivityCommand hac = new HarmonyActivityCommand();
                        hac.controlGroupLabel = controlGroup.name;
                        hac.ActivityId = activity.id;
                        hac.ActivityLabel = activity.label;
                        hac.DeviceId = func.harmonyAction.deviceId;
                        hac.CommandLabel = func.label;
                        hac.CommandId = func.name;
                        hac.CommandToSend = func.harmonyAction.command;
                        harmonyActivityCommand.Add(hac);
                    }
                }                
            }
        }

        public void CreateDeviceCommandList(HarmonyConfigResult harmonyConfig, List<HarmonyDeviceCommand> harmonyDeviceCommand)
        {
            foreach (var device in harmonyConfig.device)
            {
                //each activity has multiple control Groups that represent groups of commands, eg Numeric Buttons, Nav buttons (arrows), Media Control type (play, ff rwd etc)                
                foreach (ControlGroup controlGroup in device.controlGroup)
                {
                    //list the functions for each controlGroup
                    //the actual command you can send back to the hub is buried in the HarmonyAction part of the function
                    //this is invalid json so you have to deserialize it in a second pass after the main HarmonyConfigResults deserialization that happens
                    //at the service launch
                    foreach (Function func in controlGroup.function)
                    {
                        func.harmonyAction = new JavaScriptSerializer().Deserialize<HarmonyAction>(func.action);

                        HarmonyDeviceCommand hc = new HarmonyDeviceCommand();
                        hc.controlGroupLabel = controlGroup.name;
                        hc.DeviceId = func.harmonyAction.deviceId;
                        hc.DeviceLabel = device.label;
                        hc.CommandLabel = func.label;
                        hc.CommandId = func.name;
                        hc.CommandToSend = func.harmonyAction.command;
                        harmonyDeviceCommand.Add(hc);
                    }
                }
            }
        }

        public void ProgramActions()
        {
            this.EventLog.Source = "HarmonyService";
            ExecutableDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            //try to write to the log -- if installed as a service this will work.  If running from Visual Studio before installing as a service, the
            //log will not be there and this will fail
            try
            {
                this.EventLog.WriteEntry("Harmony Service Starting - " + ExecutableDirectory, EventLogEntryType.Information, 101, 1);
            }
            catch { }

            MainEventLog = this.EventLog;

            try
            {                
                myserviceHost = new ServiceHost(typeof(HarmonyWcfService));
                myserviceHost.Open();                

                string key;

                

                while (IsRunning)
                {
                    if (console)
                    {
                        key = Console.ReadLine();

                        if (key.ToLower() == "quit" || key.ToLower() == "exit")
                        {                            
                            IsRunning = false;
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(5000);

                        //keep alive for Harmony Hubs
                        //if (dtLastConnect.AddSeconds(60) > DateTime.Now)
                        //{
                        //    foreach (HarmonyHubDefinition hub in Program.HubList)
                        //    {
                        //        hub.client.GetCurrentActivity();
                        //    }
                        //    dtLastConnect = DateTime.Now;
                        //}

                        
                    }
                }

                myserviceHost.Close();

            }
            catch (Exception e)
            {

                try
                {
                    this.EventLog.WriteEntry(String.Format("Harmony Service Error: {0}", e.Message), EventLogEntryType.Error, 666, 1);
                }
                catch { }
                
            }

            IsTerminated = true;

        }

        protected override void OnShutdown()
        {
            
            IsRunning = false;

            while (IsTerminated == false)
            {
                Thread.Sleep(1000);
            }

            Thread.Sleep(5000);

            base.OnShutdown();
        }

        protected override void OnStop()
        {            
            IsRunning = false;

            while (IsTerminated == false)
            {
                Thread.Sleep(1000);
            }

            Thread.Sleep(5000);

            base.OnStop();
        }

    }
}
