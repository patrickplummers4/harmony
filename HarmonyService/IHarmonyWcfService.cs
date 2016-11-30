using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using HarmonyHub;
using VoxCommando;

namespace HarmonyService
{
    [ServiceContract]    
    public interface IHarmonyWcfService
    {     
        [OperationContract]
        [WebGet(UriTemplate="currentactivity/{hubName}")]
        string CurrentActivity(string hubName);

        [OperationContract]
        [WebGet(UriTemplate="devices/{hubName}")]
        List<Device> Devices(string hubName);

        [OperationContract]
        [WebGet(UriTemplate="activities/{hubName}")]
        List<Activity> Activities(string hubName);

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
        List<HarmonyActivityCommand> ActivityCommands(string hubName);
        
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

    }
}
