using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace VoxCommando
{

    /// <remarks/>
    /// 
   
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  //  [DataContract]
    public partial class VoiceCommands
    {

        private VoiceCommandsGroupCollection groupCollectionField;

        private string versionField;

        private string spaceField;

        private string dateField;

        /// <remarks/>
        /// 
        
        public VoiceCommandsGroupCollection groupCollection
        {
            get
            {
                return this.groupCollectionField;
            }
            set
            {
                this.groupCollectionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]       
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollection
    {

        private VoiceCommandsGroupCollectionCommandGroup[] commandGroupField;

        private string openField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("commandGroup")]
        public VoiceCommandsGroupCollectionCommandGroup[] commandGroup
        {
            get
            {
                return this.commandGroupField;
            }
            set
            {
                this.commandGroupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string open
        {
            get
            {
                return this.openField;
            }
            set
            {
                this.openField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroup
    {

        private VoiceCommandsGroupCollectionCommandGroupCommand[] commandField;

        private string openField;

        private string nameField;

        private string enabledField;

        private string prefixField;

        private int priorityField;

        private string requiredProcessField;

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("command")]
        public VoiceCommandsGroupCollectionCommandGroupCommand[] command
        {
            get
            {
                return this.commandField;
            }
            set
            {
                this.commandField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string open
        {
            get
            {
                return this.openField;
            }
            set
            {
                this.openField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string prefix
        {
            get
            {
                return this.prefixField;
            }
            set
            {
                this.prefixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int priority
        {
            get
            {
                return this.priorityField;
            }
            set
            {
                this.priorityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string requiredProcess
        {
            get
            {
                return this.requiredProcessField;
            }
            set
            {
                this.requiredProcessField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroupCommand
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        private int idField;

        private string nameField;

        private bool enabledField;

        private string alwaysOnField;

        private string confirmField;

        private int requiredConfidenceField;

        private string loopField;

        private int loopDelayField;

        private int loopMaxField;

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("action", typeof(VoiceCommandsGroupCollectionCommandGroupCommandAction))]
        [System.Xml.Serialization.XmlElementAttribute("event", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("if", typeof(VoiceCommandsGroupCollectionCommandGroupCommandIF))]
        [System.Xml.Serialization.XmlElementAttribute("payloadFromXML", typeof(VoiceCommandsGroupCollectionCommandGroupCommandPayloadFromXML))]
        [System.Xml.Serialization.XmlElementAttribute("payloadList", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("payloadRange", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("phrase", typeof(VoiceCommandsGroupCollectionCommandGroupCommandPhrase))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string alwaysOn
        {
            get
            {
                return this.alwaysOnField;
            }
            set
            {
                this.alwaysOnField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string confirm
        {
            get
            {
                return this.confirmField;
            }
            set
            {
                this.confirmField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int requiredConfidence
        {
            get
            {
                return this.requiredConfidenceField;
            }
            set
            {
                this.requiredConfidenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string loop
        {
            get
            {
                return this.loopField;
            }
            set
            {
                this.loopField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int loopDelay
        {
            get
            {
                return this.loopDelayField;
            }
            set
            {
                this.loopDelayField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int loopMax
        {
            get
            {
                return this.loopMaxField;
            }
            set
            {
                this.loopMaxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroupCommandAction
    {

        private string cmdTypeField;

        private string[] paramsField;

        private int cmdRepeatField;

        /// <remarks/>
        public string cmdType
        {
            get
            {
                return this.cmdTypeField;
            }
            set
            {
                this.cmdTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("param", IsNullable = false)]
        public string[] @params
        {
            get
            {
                return this.paramsField;
            }
            set
            {
                this.paramsField = value;
            }
        }

        /// <remarks/>
        public int cmdRepeat
        {
            get
            {
                return this.cmdRepeatField;
            }
            set
            {
                this.cmdRepeatField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroupCommandIF
    {

        private string ifTypeField;

        private string ifParamsField;

        private VoiceCommandsGroupCollectionCommandGroupCommandIFAction[] thenField;

        private VoiceCommandsGroupCollectionCommandGroupCommandIFAction2[] elseField;

        private string ifBlockDisabledField;

        private string ifNotField;

        /// <remarks/>
        public string ifType
        {
            get
            {
                return this.ifTypeField;
            }
            set
            {
                this.ifTypeField = value;
            }
        }

        /// <remarks/>
        public string ifParams
        {
            get
            {
                return this.ifParamsField;
            }
            set
            {
                this.ifParamsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("action", IsNullable = false)]
        public VoiceCommandsGroupCollectionCommandGroupCommandIFAction[] then
        {
            get
            {
                return this.thenField;
            }
            set
            {
                this.thenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("action", IsNullable = false)]
        public VoiceCommandsGroupCollectionCommandGroupCommandIFAction2[] @else
        {
            get
            {
                return this.elseField;
            }
            set
            {
                this.elseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ifBlockDisabled
        {
            get
            {
                return this.ifBlockDisabledField;
            }
            set
            {
                this.ifBlockDisabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ifNot
        {
            get
            {
                return this.ifNotField;
            }
            set
            {
                this.ifNotField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroupCommandIFAction
    {

        private string cmdTypeField;

        private string[] paramsField;

        private int cmdRepeatField;

        /// <remarks/>
        public string cmdType
        {
            get
            {
                return this.cmdTypeField;
            }
            set
            {
                this.cmdTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("param", IsNullable = false)]
        public string[] @params
        {
            get
            {
                return this.paramsField;
            }
            set
            {
                this.paramsField = value;
            }
        }

        /// <remarks/>
        public int cmdRepeat
        {
            get
            {
                return this.cmdRepeatField;
            }
            set
            {
                this.cmdRepeatField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroupCommandIFAction2
    {

        private string cmdTypeField;

        private string[] paramsField;

        private int cmdRepeatField;

        /// <remarks/>
        public string cmdType
        {
            get
            {
                return this.cmdTypeField;
            }
            set
            {
                this.cmdTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("param", IsNullable = false)]
        public string[] @params
        {
            get
            {
                return this.paramsField;
            }
            set
            {
                this.paramsField = value;
            }
        }

        /// <remarks/>
        public int cmdRepeat
        {
            get
            {
                return this.cmdRepeatField;
            }
            set
            {
                this.cmdRepeatField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroupCommandPayloadFromXML
    {

        private string phraseOnlyField;

        private string use2partPhraseField;

        private string phraseConnectorField;

        private string phrase2wildcardField;

        private string optionalField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string phraseOnly
        {
            get
            {
                return this.phraseOnlyField;
            }
            set
            {
                this.phraseOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string use2partPhrase
        {
            get
            {
                return this.use2partPhraseField;
            }
            set
            {
                this.use2partPhraseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string phraseConnector
        {
            get
            {
                return this.phraseConnectorField;
            }
            set
            {
                this.phraseConnectorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Phrase2wildcard
        {
            get
            {
                return this.phrase2wildcardField;
            }
            set
            {
                this.phrase2wildcardField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string optional
        {
            get
            {
                return this.optionalField;
            }
            set
            {
                this.optionalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VoiceCommandsGroupCollectionCommandGroupCommandPhrase
    {

        private bool optionalField;

        private bool optionalFieldSpecified;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool optional
        {
            get
            {
                return this.optionalField;
            }
            set
            {
                this.optionalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool optionalSpecified
        {
            get
            {
                return this.optionalFieldSpecified;
            }
            set
            {
                this.optionalFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        action,

        /// <remarks/>
        @event,

        /// <remarks/>
        @if,

        /// <remarks/>
        payloadFromXML,

        /// <remarks/>
        payloadList,

        /// <remarks/>
        payloadRange,

        /// <remarks/>
        phrase,
    }


}
