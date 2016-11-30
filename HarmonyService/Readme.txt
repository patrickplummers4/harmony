
This is a RESTful service implementation of the Harmony Console app by hdurdle.

Instead of a command line app, this runs as a Windows service and acts as a REST intermediary to the Harmony Hub.  I've also added support for the CastleOS Home Automation system.

The service can also be used to generate VoxCommando XML voice command files based off of your devices and activities as defined in your Harmony Hubs.   This can be imported into VoxCommando to provide immediate support for full voice control of your Hub.

Features:

* Windows Service
* Exposes REST interface to Harmony Hub, e.g. /pressbutton/dvd/Pause
* Supports multiple hubs
* Handles generic commands based on current Activity -- e.g. "Pause" may be a different device whether in the "Watch TV" vs "Watch Movie" activity mode.  The service figures outwhat device
needs to be triggered (similar to how the normal harmony remote works).
* Outputs VoxCommando XML voice command files based on your setup's specific device and activity list.
* Basic support for CastleOS home automation.  Handles lights, dimmers, color changing bulbs, and scenes.
* Outputs VoxCommando XML files for the CastleOS device list and actions.

To install it as a Windows service, run the InstallService.bat file from a command window (cmd.exe) running as Administrator.

To run the service as a console app in Visual Studio (debug mode), the easiest thing to do is run Visual Studio as an Administrator, select the HarmonyService project as the startup project and make sure that 'console' is passed as a command line argument (Project Properties -> Debug).

The reason for running as Administrator is so that the WCF service can self-register on the port defined in the app.config. If you don't wish to do this, you'll need to use the netsh windows command to assign the appropriate rights.

Open a command window as an Administrator:

Determine free ports:

netsh http show urlacl

See command line options:

netsh http add urlacl help

Common command for this service:

netsh http add urlacl "http://+:8085/HarmonyService" user=domain\user

NEXT:  Edit the app.config file to plug in your hub IP addresses and names and set other configuration.   This is documented in the config file itself.

Once you've installed the service and started it (services.msc) then you can call various commands.

Here are some "tester" commands that output a list of links that you can use as examples on how to call the REST services.

Replace {hubName} with your hub name as defined in the app.config file.

This gets all activities defined.
http://localhost:8085/HarmonyService/activitytester/{hubName}


This gets all commands by activity.
http://localhost:8085/HarmonyService/activitycommandtester/{hubName}


This gets all commands by device.
http://localhost:8085/HarmonyService/devicecommandtester/{hubName}

The following outputs VoxCommando XML files for teh Harmony Hub and CastleOS respectively.  Replace {commandStartId} with a number like 100, 200, etc. that is not already in use
by another VoxCommando Command Group.

http://localhost:8085/HarmonyService/GenerateVoxCommandoCommands/{hubName}/{commandStartID}

http://localhost:8085/HarmonyService/GenerateVoxCommandoCommandsCastleOS/{commandStartID}

Here's a list of all the operation contracts possible on the service.  The URI template item is the path to the operation and is appended to the server/port/service name.

Items in curly brackets are parameters.

E.g.:  http://localhost:8085/HarmonyService/currentactivity/LivingRoom


[OperationContract]
[WebGet(UriTemplate="currentactivity/{hubName}")]
string CurrentActivity(string hubName);

[OperationContract]
[WebGet(UriTemplate="devices/{hubName}")]
List<Device>
  Devices(string hubName);

  [OperationContract]
  [WebGet(UriTemplate="activities/{hubName}")]
  List<Activity>
    Activities(string hubName);

    [OperationContract]
    [WebGet(UriTemplate="activity/{hubName}/{activityId}")]
    void StartActivity(string hubName, string activityId);

    [OperationContract]
    [WebGet(UriTemplate="pressbutton/{hubName}/{deviceId}/{commandId}")]
    void PressButton(string hubName, string deviceId, string commandId);

    [OperationContract]
    [WebGet(UriTemplate="turnoff/{hubName}")]
    void TurnOff(string hubName);

    [OperationContract]
    [WebGet(UriTemplate = "activitycommands/{hubName}")]
    List<HarmonyActivityCommand>
      ActivityCommands(string hubName);

      [OperationContract]
      [WebGet(UriTemplate = "doactivitycommand/{hubName}/{commandId}")]
      void DoActivityCommand(string hubName, string commandId);

      [OperationContract]
      [WebGet(UriTemplate = "devicecommands/{hubName}")]
      List<HarmonyDeviceCommand> DeviceCommands(string hubName);

        [OperationContract]
        [WebGet(UriTemplate = "activitytester/{hubName}", BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream ActivityTester(string hubName);

        [OperationContract]
        [WebGet(UriTemplate = "activitycommandtester/{hubName}", BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream ActivityCommandTester(string hubName);

        [OperationContract]
        [WebGet(UriTemplate = "devicecommandtester/{hubName}", BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream DeviceCommandTester(string hubName);

        [OperationContract]
        [XmlSerializerFormat]
        [WebGet(UriTemplate = "GenerateVoxCommandoCommands/{hubName}/{commandStartID}")]
        VoiceCommands GenerateVoxCommandoCommands(string hubName,string commandStartID);

        [OperationContract]
        [XmlSerializerFormat]
        [WebGet(UriTemplate = "GenerateVoxCommandoCommandsCastleOS/{commandStartID}")]
        VoiceCommands GenerateVoxCommandoCommands_CastleOS(string commandStartID);

        [OperationContract]
        [WebGet(UriTemplate = "switch/{deviceID}/{state}")]
        void Switch(string deviceID, string state);

        [OperationContract]
        [WebGet(UriTemplate = "togglescene/{id}/{state}")]
        void ToggleScene(string id, string state);

        [OperationContract]
        [WebGet(UriTemplate = "dim/{id}/{level}")]
        void Dim(string id, string level);

        [OperationContract]
        [WebGet(UriTemplate = "setcolor/{deviceId}/{colorname}")]
        void SetColor(string deviceId, string colorname);