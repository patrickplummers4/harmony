using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HarmonyHub
{
    /// <summary>
    /// Logitech HarmonyHub Configuration
    /// </summary>
    public class HarmonyConfigResult
    {
        public List<Activity> activity { get; set; }
        public List<Device> device { get; set; }
        public List<object> sequence { get; set; }
        public Content content { get; set; }
        public Global global { get; set; }

        public string ActivityNameFromId(string activityId)
        {
            return (from activityItem in activity where activityItem.id == activityId select activityItem.label).FirstOrDefault();
        }

        public string DeviceNameFromId(string deviceId)
        {
            return (from deviceItem in device where deviceItem.id == deviceId select deviceItem.label).FirstOrDefault();
        }
        
    }

    /// <summary>
    /// HarmonyHub Activity
    /// </summary>
    /// 
    [DataContract]
    public class Activity : IComparable<Activity>
    {
        [DataMember]
        public string suggestedDisplay { get; set; }
        [DataMember]
        public string label { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string activityTypeDisplayName { get; set; }
        public List<ControlGroup> controlGroup { get; set; }
        public List<object> sequences { get; set; }
        [DataMember]
        public int activityOrder { get; set; }
        [DataMember]
        public bool isTuningDefault { get; set; }
        public Dictionary<string, FixItCommand> fixit { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public string baseImageUri { get; set; }

        public int CompareTo(Activity other)
        {
            return String.Compare(label, other.label, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    /// <summary>
    /// HarmonyHub Device
    /// </summary>
    /// 
    [DataContract]
    public class Device : IComparable<Device>
    {
        [DataMember]
        public int Transport { get; set; }
        [DataMember]
        public string suggestedDisplay { get; set; }
        [DataMember]
        public string deviceTypeDisplayName { get; set; }
        [DataMember]
        public string label { get; set; }
        [DataMember]
        public string id { get; set; }
        //[DataMember]
        public List<int> Capabilities { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int DongleRFID { get; set; }
        //[DataMember]
        public List<ControlGroup> controlGroup { get; set; }
        [DataMember]
        public int ControlPort { get; set; }
        [DataMember]
        public bool IsKeyboardAssociated { get; set; }
        [DataMember]
        public string model { get; set; }
        [DataMember]
        public string deviceProfileUri { get; set; }
        [DataMember]
        public string manufacturer { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public string isManualPower { get; set; }

        public int CompareTo(Device other)
        {
            return String.Compare(label, other.label, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public class Content
    {
        public string contentUserHost { get; set; }
        public string contentDeviceHost { get; set; }
        public string contentServiceHost { get; set; }
        public string contentImageHost { get; set; }
        public string householdUserProfileUri { get; set; }
    }

    public class Global
    {
        public string timeStampHash { get; set; }
        public string locale { get; set; }
    }

    /// <summary>
    /// Power and Input states to "fix" a device
    /// </summary>
    [DataContract]
    public class FixItCommand
    {
        /// <summary>
        /// Device ID for fix settings
        /// </summary>
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public bool isRelativePower { get; set; }
        [DataMember]
        public bool isManualPower { get; set; }
        [DataMember]
        public string Power { get; set; }
        [DataMember]
        public string Input { get; set; }
    }

    /// <summary>
    /// HarmonyHub Remote Action
    /// </summary>
    public class HarmonyAction
    {
        /// <summary>
        /// Action Type (IRCommand)
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// DeviceId to receive command
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// HarmonyHub command to send to device
        /// </summary>
        public string command { get; set; }
    }

    public class ControlGroup
    {
        public string name { get; set; }

        public List<Function> function { get; set; }
    }

    public class Function
    {
        public string action { get; set; }

        public string name { get; set; }

        public string label { get; set; }

        public HarmonyAction harmonyAction { get; set; }

    }

    public class HarmonyActivityCommand
    {
        public string ActivityLabel { get; set; }
        public string ActivityId { get; set; }        
        public string DeviceLabel { get; set; }
        public string DeviceId { get; set; }
        public string CommandLabel { get; set; }
        public string CommandId { get; set; }
        public string controlGroupLabel { get; set; }
        public string CommandToSend { get; set; }
    }

    public class HarmonyDeviceCommand
    {        
        public string DeviceLabel { get; set; }
        public string DeviceId { get; set; }
        public string CommandLabel { get; set; }
        public string CommandId { get; set; }
        public string controlGroupLabel { get; set; }
        public string CommandToSend { get; set; }
    }
}
