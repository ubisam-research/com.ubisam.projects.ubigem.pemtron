using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using UbiCom.Net.Driver;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;
using UbiGEM.Net.Structure;
using UbiSam.GEM.Configration.Utility;
using UbiCom.Net.Automata.SECS2;

namespace UbiGEM.Net.Simulator.Host.Common
{
    public class MessageProcessor
    {
        #region Delegate
        public delegate void SchedulerActivated();
        public delegate void SchedulerDeactivated();
        public delegate void DriverConnectionStateChanged(object sender, string ipAddress, int portNo);
        public delegate void DriverLogAdded1(object sender, DriverLogType logType, string logText);
        public delegate void DriverLogAdded2(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText);
        public delegate void CommunicationStateChanged(CommunicationState communicationState);
        public delegate void ControlStateChanged(ControlState controlState);
        public delegate void GEMObjectDataChanged();

        public event SchedulerActivated OnSchedulerActivated;
        public event SchedulerDeactivated OnSchedulerDeactivated;
        public event DriverConnectionStateChanged OnDriverConnected;
        public event DriverConnectionStateChanged OnDriverDisconnected;
        public event DriverConnectionStateChanged OnDriverSelected;
        public event DriverConnectionStateChanged OnDriverDeselected;
        public event DriverLogAdded1 OnDriverLogAdded1;
        public event DriverLogAdded2 OnDriverLogAdded2;
        public event DriverLogAdded2 OnSECS2LogAdded;
        public event CommunicationStateChanged OnCommunicationStateChanged;
        public event ControlStateChanged OnControlStateChanged;

        public event GEMObjectDataChanged OnGEMObjectDataChanged;
        #endregion

        #region const strings
        private const string TAG_MESSAGE = "SECSMessage";
        private const string TAG_XML_STRUCTURE = "XMLStructure";
        private const string TAG_USER_XML_MESSAGE = "UserXMLMessage";
        private const string TAG_USER_MESSAGE = "UserMessage";
        private const string TAG_USER_CUSTOM_MESSAGE = "UserCustomMessage";
        #endregion

        #region Enum
        public enum DefineReportSequence
        {
            Start,
            SendAlarmDisable,
            WaitAlarmDisable,
            SendS2F37Disable,
            WaitS2F38Disable,
            SendS2F35Disable,
            WaitS2F36Disable,
            SendS2F33Disable,
            WaitS2F34Disable,
            SendS2F33Enable,
            WaitS2F34Enable,
            SendS2F35Enable,
            WaitS2F36Enable,
            SendS2F37Enable,
            WaitS2F38Enable,
            SendAlarmEnable,
            WaitAlarmEnable,
            End
        }
        #region DriverLogType
        public enum DriverLogType
        {
            INFO,
            WARN,
            SEND,
            RECV,
            TIME
        }
        #endregion
        #region VariableIDList
        public enum VariableIDList
        {
            Clock = 1,
            ALCD = 2,
            AlarmSet = 3,
            ControlState = 4,
            EventsEnabled = 5,
            ProcessState = 6,
            PreviousProcessState = 7,
            AlarmsEnabled = 8,
            SpoolCountActual = 11,
            SpoolCountTotal = 12,
            SpoolFullTime = 13,
            MDLN = 14,
            SOFTREV = 15,
            SpoolStartTime = 16,
            SpoolStatus = 17,
            SpoolFull = 18,
            AlarmID = 24,
            ChangedECID = 25,
            EventLimit = 26,
            LimitVariable = 27,
            OperatorCommand = 28,
            PPChangeName = 29,
            PPChangeStatus = 30,
            PPError = 31,
            TransitionType = 32,
            PortID = 33,
            PreviousControlState = 34,
            CommState = 35,
            PreviousCommState = 36,
            EquipmentInitiatedConnected = 101,
            EstablishCommunicationsTimeout = 102,
            MaxSpoolTransmit = 103,
            OverWriteSpool = 104,
            EnableSpooling = 105,
            TimeFormat = 106,
            T3Timeout = 109,
            T5Timeout = 110,
            T6Timeout = 111,
            T7Timeout = 112,
            T8Timeout = 113,
            InitControlState = 117,
            OffLineSubState = 118,
            OnLineFailState = 120,
            Maker = 121,
            OnLineSubState = 122,
            MaxSpoolMsg = 123,
            DeviceID = 124,
            IPAddress = 125,
            PortNumber = 126,
            ActiveMode = 127,
            LinkTestInterval = 128,
            RetryLimit = 129
        }
        #endregion
        #region GRNT1
        public enum GRNT1
        {
            OK = 0,
            Busy = 1,
            NoSpace = 2,
            TooLarge = 3,
            DupelicatedID = 4,
            NotFound = 5,
            Unknown = 6,
        }
        public enum MDACK
        {
            Received = 0,
            FormatError = 1,
            NoIDMatch = 2,
            AbortDiscardMap = 3,
        }
        #endregion
        #endregion

        #region Properties
        public bool S7F23ExtChecked { get; set; }

        public bool Activated
        {
            get; private set;
        }

        public CommunicationState CommunicationStateBefore
        {
            get; private set;
        }

        public CommunicationState CommunicationState
        {
            get; private set;
        }

        public ControlState ControlState
        {
            get; private set;
        }

        public ControlState ControlStateBefore
        {
            get; private set;
        }

        public bool HSMSDriverConnected
        {
            get
            {
                bool result;

                if (this._driver == null)
                {
                    result = false;
                }
                else
                {
                    result = this._driver.Connected;
                }

                return result;
            }
        }

        public bool IsDirty
        {
            get; set;
        }

        public Configurtion Configuration
        {
            get; set;
        }

        public Configurtion UGCConfiguration
        {
            get; set;
        }

        public AlarmCollection AlarmCollection
        {
            get; private set;
        }
        public CollectionEventCollection CollectionEventCollection
        {
            get; private set;
        }

        public DataDictionaryCollection DataDictionaryCollection
        {
            get; private set;
        }

        public RemoteCommandCollection RemoteCommandCollection
        {
            get; private set;
        }

        public ReportCollection ReportCollection
        {
            get; private set;
        }

        public VariableCollection VariableCollection
        {
            get; private set;
        }
        public CurrentSetting CurrentSetting
        {
            get; private set;
        }

        public ExpandedTraceCollection TraceCollection
        {
            get; private set;
        }

        public ExpandedLimitMonitoringCollection LimitMonitoringCollection
        {
            get; private set;
        }

        public SECSMessageCollection UserGEMMessage
        {
            get; private set;
        }

        public SECSMessageCollection UserMessage
        {
            get; private set;
        }

        public SECSMessageCollection GEMDriverMessages
        {
            get
            {
                SECSMessageCollection result = new SECSMessageCollection();
                return result;
            }
        }

        public SECSMessageCollection HSMSDriverMessages
        {
            get
            {
                return this._driver.MessageManager.Messages;
            }
        }

        public Dictionary<string, UserMessage> UserMessageData
        {
            get; private set;
        }

        public FormattedProcessProgramCollection FormattedProcessProgramCollection
        {
            get; private set;
        }

        public GEMObjectCollection GEMObjectCollection
        {
            get; private set;
        }

        public SupervisedGEMObjectCollection SupervisedGEMObjectCollection
        {
            get; private set;
        }

        public string ConfigFilepath { get; set; }

        public string UGCFilepath { get; set; }

        public List<MapSetupData> MapSetupDataCollection
        {
            get; private set;
        }
        public List<MapDataType1> MapDataType1Collection
        {
            get; private set;
        }
        public List<MapDataType2> MapDataType2Collection
        {
            get; private set;
        }
        public List<MapDataType3> MapDataType3Collection
        {
            get; private set;
        }
        #endregion

        #region MemberVariable
        private readonly long _dataId = 0;
        private readonly HSMSDriver _driver;
        private bool _hsmsDriverInitialized;
        private readonly Random _random;
        private readonly Automata _secs2Automata;
        private uint _lastSystemBytesS1F3ForControlState;

        private bool _isProcessAutoSendOnlineRunning;
        private bool _isProcessAutoSendOnlineStop;
        private DefineReportSequence CurrentDefineReportSequence;
        private DefineReportSequence BeforeDefineReportSequence;

        private uint _lastS5F3DisableSystemByte;
        private uint _lastS2F37DisableSystemByte;
        private uint _lastS2F35DisableSystemByte;
        private uint _lastS2F33DisableSystemByte;
        private uint _lastS2F33EnableSystemByte;
        private uint _lastS2F35EnableSystemByte;
        private uint _lastS2F37EnableSystemByte;
        private uint _lastS5F3EnableSystemByte;

        private string _grantedMaterialIDForS12F7;

        private string _grantedMaterialIDForS12F9;

        private string _grantedMaterialIDForS12F11;
        #endregion

        #region Constructor
        public MessageProcessor()
        {
            this.CommunicationStateBefore = CommunicationState.Disabled;
            this.CommunicationState = CommunicationState.Disabled;
            this.ControlState = ControlState.EquipmentOffline;
            this.ControlStateBefore = ControlState.EquipmentOffline;
            this.IsDirty = false;

            this._driver = new HSMSDriver();
            this.Configuration = new Configurtion();

            this.S7F23ExtChecked = true;

            this.DataDictionaryCollection = new DataDictionaryCollection();
            this.VariableCollection = new VariableCollection();
            this.ReportCollection = new ReportCollection();
            this.AlarmCollection = new AlarmCollection();
            this.CollectionEventCollection = new CollectionEventCollection();
            this.RemoteCommandCollection = new RemoteCommandCollection();

            this.CurrentSetting = new CurrentSetting();
            this.TraceCollection = new ExpandedTraceCollection();
            this.LimitMonitoringCollection = new ExpandedLimitMonitoringCollection();

            this.UserGEMMessage = new SECSMessageCollection();
            this.UserMessage = new SECSMessageCollection();

            this.UserMessageData = new Dictionary<string, UserMessage>();

            this._hsmsDriverInitialized = false;

            this._driver.OnSECSConnected += driver_OnSECSConnected;
            this._driver.OnSECSDisconnected += driver_OnSECSDisconnected;
            this._driver.OnSECSSelected += driver_OnSECSSelected;
            this._driver.OnSECSDeselected += driver_OnSECSDeselected;
            this._driver.OnReceivedPrimaryMessage += driver_OnReceivedPrimaryMessage;
            this._driver.OnReceivedSecondaryMessage += driver_OnReceivedSecondryMessage;
            this._driver.OnTimeout += driver_OnTimeout;
            this._driver.OnT3Timeout += driver_OnT3Timeout;
            this._driver.OnReceivedUnknownMessage += driver_OnReceivedUnknownMessage;
            this._driver.OnReceivedInvalidPrimaryMessage += driver_OnReceivedInvalidPrimaryMessage;
            this._driver.OnReceivedInvalidSecondaryMessage += driver_OnReceivedInvalidSecondaryMessage;
            this._driver.OnSECS1WriteLog += driver_OnSECS1WriteLog;
            this._driver.OnSECS2WriteLog += driver_OnSECS2WriteLog;

            this._random = new Random(Guid.NewGuid().GetHashCode());

            this._secs2Automata = new Automata();
            this._lastSystemBytesS1F3ForControlState = 0;

            this._isProcessAutoSendOnlineRunning = false;
            this._isProcessAutoSendOnlineStop = false;
            this.CurrentDefineReportSequence = DefineReportSequence.End;
            this.BeforeDefineReportSequence = DefineReportSequence.End;

            this.FormattedProcessProgramCollection = new FormattedProcessProgramCollection();

            this.GEMObjectCollection = new GEMObjectCollection();
            this.SupervisedGEMObjectCollection = new SupervisedGEMObjectCollection();
            this.MapSetupDataCollection = new List<MapSetupData>();
            this.MapDataType1Collection = new List<MapDataType1>();
            this.MapDataType2Collection = new List<MapDataType2>();
            this.MapDataType3Collection = new List<MapDataType3>();

            this._grantedMaterialIDForS12F7 = string.Empty;

            this._grantedMaterialIDForS12F9 = string.Empty;

            this._grantedMaterialIDForS12F11 = string.Empty;

            NewProject();
        }
        #endregion
        #region Descructor
        ~MessageProcessor()
        {
            StopScheduler();
        }
        #endregion

        // Driver Event
        #region driver_OnSECSConnected
        private void driver_OnSECSConnected(object sender, string ipAddress, int portNo)
        {
            RaiseDriverConnected(sender, ipAddress, portNo);
        }
        #endregion
        #region driver_OnSECSDisconnected
        private void driver_OnSECSDisconnected(object sender, string ipAddress, int portNo)
        {
            this.CommunicationStateBefore = this.CommunicationState;
            this.CommunicationState = CommunicationState.Disabled;

            RaiseCommunicationStateChanged(this.CommunicationState);
            RaiseDriverDisconnected(sender, ipAddress, portNo);

            if (this._isProcessAutoSendOnlineRunning == true)
            {
                this._isProcessAutoSendOnlineStop = true;
            }
        }
        #endregion
        #region driver_OnSECSSelected
        private void driver_OnSECSSelected(object sender, string ipAddress, int portNo)
        {
            if (this.CurrentSetting.AutoSendS1F13 == true)
            {
                SendS1F13();
            }

            RaiseDriverSelected(sender, ipAddress, portNo);
        }
        #endregion
        #region driver_OnSECSDeselected
        private void driver_OnSECSDeselected(object sender, string ipAddress, int portNo)
        {
            RaiseDriverDeselected(sender, ipAddress, portNo);
        }
        #endregion
        #region driver_OnReceivedPrimaryMessage
        private void driver_OnReceivedPrimaryMessage(object sender, SECSMessage message)
        {
            AnalyzePrimaryMessage(message);
        }
        #endregion
        #region driver_OnReceivedSecondryMessage
        private void driver_OnReceivedSecondryMessage(object sender, SECSMessage primaryMessage, SECSMessage secondryMessage)
        {
            AnalyzeSecondaryMessage(primaryMessage, secondryMessage);
        }
        #endregion
        #region driver_OnTimeout
        private void driver_OnTimeout(object sender, TimeoutType timeoutType)
        {
        }
        #endregion
        #region driver_OnT3Timeout
        private void driver_OnT3Timeout(object sender, SECSMessage message)
        {
            string errorText;

            errorText = $"T3 Timeout Occurs: S{message.Stream}F{message.Function} [SystemBytes={message.SystemBytes:X8}]";
            RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
        }
        #endregion
        #region driver_OnReceivedUnknownMessage
        private void driver_OnReceivedUnknownMessage(object sender, SECSMessage message)
        {
            RaiseDriverLogAdded1(sender, DriverLogType.INFO, string.Format("ReceivedUnknownMessage: [S{0}F{1}]", message.Stream, message.Function));
        }
        #endregion
        #region driver_OnReceivedInvalidPrimaryMessage
        private void driver_OnReceivedInvalidPrimaryMessage(object sender, MessageValidationError reason, SECSMessage message)
        {
            RaiseDriverLogAdded1(sender, DriverLogType.INFO, string.Format("ReceivedInvalidPrimaryMessage: [Reason: {0}, S{1}F{2}]", reason.ToString(), message.Stream, message.Function));
        }
        #endregion
        #region driver_OnReceivedInvalidSecondaryMessage
        private void driver_OnReceivedInvalidSecondaryMessage(object sender, MessageValidationError reason, SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            RaiseDriverLogAdded1(sender, DriverLogType.INFO, string.Format("ReceivedInvalidSecondaryMessage: [Reason: {0}, S{1}F{2}]", reason.ToString(), secondaryMessage.Stream, secondaryMessage.Function));
        }
        #endregion
        #region driver_OnSECS1WriteLog
        private void driver_OnSECS1WriteLog(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
        }
        #endregion
        #region driver_OnSECS2WriteLog
        private void driver_OnSECS2WriteLog(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            RaiseSECS2LogAdded(sender, logLevel, logText);
        }
        #endregion

        // Public Method
        #region StartScheduler
        public bool StartScheduler(out string errorText)
        {
            bool result;
            DriverError error;

            errorText = string.Empty;
            result = true;

            error = DriverError.Ok;

            if (this.Activated == false)
            {
                if (this.UGCFilepath == null || System.IO.File.Exists(this.UGCFilepath) == false)
                {
                    result = false;
                    errorText = "GEM Driver is null or UGC file name is null or not exists";
                }

                #region HSMS Driver Initialize
                if (result == true)
                {
                    if (this._hsmsDriverInitialized == false)
                    {
                        this.Configuration.UMDFileName = "UbiGEM.Net.Simulator.Host.umd";

                        error = this._driver.Initialize(this.Configuration, out errorText);

                        if (error != DriverError.Ok)
                        {
                            result = false;
                            errorText = string.Format("SECS Driver Initialize Error: {0}", errorText);
                        }
                    }

                    if (error == DriverError.Ok)
                    {
                        try
                        {
                            XElement standardMessage = XElement.Load(this.Configuration.UMDFileName);

                            this._driver.MessageManager.Load(standardMessage);
                        }
                        catch { }

                        foreach (var customMessage in this.UserMessage.MessageInfo)
                        {
                            this._driver.AddUserDefinedMessage(customMessage.Value);
                        }
                    }
                }
                #endregion
            }

            if (result == true)
            {
                this._driver.UseAutoReply = false;

                _ = this._driver.Open();

                this.Activated = true;

                RaiseSchedulerActivated();
            }

            return result;
        }
        public void InitializeHSMSDriver()
        {
            this.Configuration.UMDFileName = "UbiGEM.Net.Simulator.Host.umd";

            DriverError error = this._driver.Initialize(this.Configuration, out _);

            if (error == DriverError.Ok)
            {
                try
                {
                    XElement standardMessage = XElement.Load(this.Configuration.UMDFileName);

                    this._driver.MessageManager.Load(standardMessage);
                }
                catch { }

                foreach (var customMessage in this.UserMessage.MessageInfo)
                {
                    this._driver.AddUserDefinedMessage(customMessage.Value);
                }
            }
        }
        #endregion
        #region StopScheduler
        public void StopScheduler()
        {
            if (_isProcessAutoSendOnlineRunning == true)
            {
                _isProcessAutoSendOnlineStop = true;
            }

            if (this._driver != null)
            {
                this._driver.Close();
            }

            this.Activated = false;

            RaiseSchedulerDeactivated();
        }
        #endregion
        #region LoadConfigFile
        public GemDriverError LoadConfigFile(string configFileName, out string errorText)
        {
            GemDriverError result;
            XElement root;
            XElement element;
            XElement subElement;
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            ExpandedRemoteCommandValueSetCollection tempRCMDValueSetCollection;
            ExpandedEnhancedRemoteCommandValueSetCollection tempERCMDValueSetCollection;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            ExpandedCollectionEventInfo collectionEventInfo;
            ExpandedAlarmInfo alarmInfo;
            ExpandedTraceInfo traceInfo;
            ExpandedVariableInfo expandedVariableInfo;
            ExpandedReportInfo reportInfo;
            ExpandedLimitMonitoringInfo expandedLimitMonitoringInfo;
            //ExpandedLimitMonitoringItem expandedLimitMonitoringItem;
            UserMessage userMessage;
            SECSItemFormat parameterFormat;
            string readAttribute;
            long longValue;
            int intValue;
            byte byteValue;
            bool boolValue;
            object convertedValue;
            AckInfo ackInfo;
            UseReplyMessage replyItem;
            string tempUGCFileName;
            DirectoryInfo recipeDirectory;
            string ppid;
            FormattedProcessProgramInfo fmtPPInfo;
            GEMObject gemObject;
            GEMObjectID gemObjectID;
            GEMObjectAttribute gemObjectAttribute;
            string tempAttrData;
            SECSItemFormat tempAttrFormat;
            Stack<string> parentNameStack;
            string objSpec;
            KeyForSelectedObjectList keyForSelectedObjectList;
            GEMObjectAttributeFilterInfo gemObjectAttributeFilterInfo;

            SupervisedGEMObject supervisedGEMObject;
            uint objToken;

            MapSetupData mapSetupData;
            MapDataType1 mapDataType1;
            MapDataType2 mapDataType2;
            MapDataType3 mapDataType3;
            string materialID;
            ReferencePointItem rpItem;
            ReferenceStartingInfo rsInfo;
            XYPosInfo xyPosInfo;

            errorText = string.Empty;
            tempUGCFileName = string.Empty;
            parentNameStack = new Stack<string>();

            if (string.IsNullOrEmpty(configFileName) == true)
            {
                result = GemDriverError.FileLoadFailed;
                errorText = "filepath is empty";
            }
            else if (System.IO.File.Exists(configFileName) == false)
            {
                result = GemDriverError.FileLoadFailed;
                errorText = string.Format("not exists file: {0}", configFileName);
            }
            else
            {
                try
                {
                    result = GemDriverError.Ok;

                    root = XElement.Load(configFileName);

                    #region [Driver Configuration]
                    element = root.Element("Driver");

                    if (element != null)
                    {
                        this.Configuration = new Configurtion()
                        {
                            DriverName = element.Element("Name").Value,
                            DeviceType = DeviceType.Host,
                            DeviceID = int.Parse(element.Element("DeviceID").Value),
                            SECSMode = SECSMode.HSMS,
                            LogEnabledSECS1 = (LogMode)Enum.Parse(typeof(LogMode), element.Element("LogEnabledSECS1").Value),
                            LogEnabledSECS2 = (LogMode)Enum.Parse(typeof(LogMode), element.Element("LogEnabledSECS2").Value),
                            LogEnabledSystem = (LogMode)Enum.Parse(typeof(LogMode), element.Element("LogEnabledSystem").Value),
                            LogExpirationDay = int.Parse(element.Element("LogExpirationDay").Value),
                            LogPath = element.Element("LogPath").Value,
                            MaxMessageSize = int.Parse(element.Element("MaxMessageSize").Value),
                            HSMSModeConfig = new Configurtion.HSMS()
                            {
                                HSMSMode = (HSMSMode)Enum.Parse(typeof(HSMSMode), element.Element("HSMS").Element("HSMSMode").Value),
                                RemoteIPAddress = element.Element("HSMS").Element("RemoteIPAddress").Value,
                                RemotePortNo = int.Parse(element.Element("HSMS").Element("RemotePortNo").Value),
                                LocalIPAddress = element.Element("HSMS").Element("LocalIPAddress").Value,
                                LocalPortNo = int.Parse(element.Element("HSMS").Element("LocalPortNo").Value),
                            }
                        };
                    }
                    #region [UGCFile]
                    element = root.Element("UGCFileName");
                    if (element != null)
                    {
                        if (string.IsNullOrEmpty(element.Value) == false)
                        {
                            tempUGCFileName = element.Value;
                            if (tempUGCFileName.IndexOf("[MyDocuments]") < 0)
                            {
                                this.UGCFilepath = tempUGCFileName;
                            }
                            else if (tempUGCFileName.IndexOf("[MyDocuments]") == 0)
                            {
                                tempUGCFileName = tempUGCFileName.Substring("[MyDocuments]".Length);
                                tempUGCFileName = string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), tempUGCFileName);
                                this.UGCFilepath = tempUGCFileName;
                            }
                            else
                            {
                                this.UGCFilepath = string.Empty;
                            }
                        }
                    }
                    #endregion
                    #endregion

                    #region Initialize GEM Driver
                    if (string.IsNullOrEmpty(this.UGCFilepath) == false && System.IO.File.Exists(this.UGCFilepath) == true)
                    {
                        result = LoadUGCFile(this.UGCFilepath, out errorText);

                        if (result != GemDriverError.Ok)
                        {
                            errorText = string.Format(" UGC file verification failed \n\n {0}", errorText);
                        }
                        else
                        {
                            this._grantedMaterialIDForS12F7 = string.Empty;

                            this._grantedMaterialIDForS12F9 = string.Empty;

                            this._grantedMaterialIDForS12F11 = string.Empty;
                        }
                    }
                    else
                    {
                        this.UGCFilepath = string.Empty;
                        errorText = "Invalid ugc file path";
                        result = GemDriverError.InvalidConfiguration;
                    }
                    #endregion

                    #region [EquipmentConstants]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("EquipmentConstants") != null)
                    {
                        foreach (XElement tempEcid in root.Element("EquipmentConstants").Elements("ECID"))
                        {
                            if (tempEcid.Attribute("ID") != null && tempEcid.Attribute("Name") != null)
                            {
                                if (tempEcid.Attribute("ID").Value != string.Empty)
                                {
                                    expandedVariableInfo = this.VariableCollection[tempEcid.Attribute("ID").Value] as ExpandedVariableInfo;
                                }
                                else
                                {
                                    expandedVariableInfo = this.VariableCollection.Items.FirstOrDefault(t => t.Name == tempEcid.Attribute("Name").Value) as ExpandedVariableInfo;
                                }

                                if (expandedVariableInfo == null)
                                {
                                    expandedVariableInfo = new ExpandedVariableInfo()
                                    {
                                        IsInheritance = false,
                                        VID = tempEcid.Attribute("ID").Value,
                                        Name = tempEcid.Attribute("Name").Value,
                                        VIDType = VariableType.ECV
                                    };

                                    if (expandedVariableInfo.VID == string.Empty)
                                    {
                                        expandedVariableInfo.VID = expandedVariableInfo.Name;
                                    }

                                    this.VariableCollection.Add(expandedVariableInfo);
                                }

                                if (tempEcid.Attribute("Format") != null && Enum.TryParse(tempEcid.Attribute("Format").Value, out parameterFormat) == true)
                                {
                                    expandedVariableInfo.Format = parameterFormat;
                                }

                                if (tempEcid.Attribute("Length") != null && int.TryParse(tempEcid.Attribute("Length").Value, out intValue) == true)
                                {
                                    expandedVariableInfo.Length = intValue;
                                }

                                if (tempEcid.Attribute("Use") != null && bool.TryParse(tempEcid.Attribute("Use").Value, out boolValue) == true)
                                {
                                    expandedVariableInfo.IsUse = boolValue;
                                }

                                if (tempEcid.Attribute("Description") != null)
                                {
                                    expandedVariableInfo.Description = tempEcid.Attribute("Description").Value;
                                }

                                if (tempEcid.Attribute("Value") != null)
                                {
                                    expandedVariableInfo.Value = tempEcid.Attribute("Value").Value;
                                }
                            }
                        }
                    }
                    #endregion
                    #region [Variables]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("Variables") != null)
                    {
                        foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                        {
                            if (tempVariable.Attribute("ID") != null && tempVariable.Attribute("Name") != null)
                            {
                                if (tempVariable.Attribute("ID").Value != string.Empty)
                                {
                                    expandedVariableInfo = this.VariableCollection[tempVariable.Attribute("ID").Value] as ExpandedVariableInfo;
                                }
                                else
                                {
                                    expandedVariableInfo = this.VariableCollection.Items.FirstOrDefault(t => t.Name == tempVariable.Attribute("Name").Value) as ExpandedVariableInfo;
                                }


                                if (expandedVariableInfo == null)
                                {
                                    expandedVariableInfo = new ExpandedVariableInfo()
                                    {
                                        IsInheritance = false,
                                        VID = tempVariable.Attribute("ID").Value,
                                        Name = tempVariable.Attribute("Name").Value,
                                        VIDType = VariableType.DVVAL
                                    };

                                    if (tempVariable.Attribute("Class") != null)
                                    {
                                        if (tempVariable.Attribute("Class").Value == VariableType.DVVAL.ToString())
                                        {
                                            expandedVariableInfo.VIDType = VariableType.DVVAL;
                                        }
                                        else if (tempVariable.Attribute("Class").Value == VariableType.SV.ToString())
                                        {
                                            expandedVariableInfo.VIDType = VariableType.SV;
                                        }
                                    }

                                    if (expandedVariableInfo.VID == string.Empty)
                                    {
                                        expandedVariableInfo.VID = expandedVariableInfo.Name;
                                    }

                                    this.VariableCollection.Add(expandedVariableInfo);
                                }

                                if (tempVariable.Attribute("Format") != null && Enum.TryParse(tempVariable.Attribute("Format").Value, out parameterFormat) == true)
                                {
                                    expandedVariableInfo.Format = parameterFormat;
                                }

                                if (tempVariable.Attribute("Length") != null && int.TryParse(tempVariable.Attribute("Length").Value, out intValue) == true)
                                {
                                    expandedVariableInfo.Length = intValue;
                                }

                                if (tempVariable.Attribute("Use") != null && bool.TryParse(tempVariable.Attribute("Use").Value, out boolValue) == true)
                                {
                                    expandedVariableInfo.IsUse = boolValue;
                                }

                                if (tempVariable.Attribute("Description") != null)
                                {
                                    expandedVariableInfo.Description = tempVariable.Attribute("Description").Value;
                                }

                                if (tempVariable.Attribute("Value") != null)
                                {
                                    convertedValue = ConvertValue(expandedVariableInfo.Format, tempVariable.Attribute("Value").Value);

                                    if (convertedValue != null)
                                    {
                                        expandedVariableInfo.Value = convertedValue.ToString();
                                    }
                                    else
                                    {
                                        expandedVariableInfo.Value = string.Empty;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region [Equipment Constants-Childs]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("EquipmentConstants") != null)
                    {
                        foreach (XElement tempVariable in root.Element("EquipmentConstants").Elements("ECID"))
                        {
                            if (string.IsNullOrEmpty(errorText) == true && tempVariable.Attribute("ID") != null && tempVariable.Attribute("Name") != null)
                            {
                                if (string.IsNullOrEmpty(tempVariable.Attribute("ID").Value) == false)
                                {
                                    expandedVariableInfo = this.VariableCollection[tempVariable.Attribute("ID").Value] as ExpandedVariableInfo;
                                }
                                else
                                {
                                    expandedVariableInfo = this.VariableCollection.Items.FirstOrDefault(t => t.Name == tempVariable.Attribute("Name").Value) as ExpandedVariableInfo;
                                }

                                if (expandedVariableInfo != null && tempVariable.Element("Childs") != null)
                                {
                                    parentNameStack.Clear();

                                    parentNameStack.Push(expandedVariableInfo.Name);
                                    GetChildVariableInfo(this.VariableCollection, expandedVariableInfo, tempVariable.Element("Childs"), parentNameStack, out errorText);
                                    parentNameStack.Pop();
                                }
                            }
                        }
                    }
                    #endregion
                    #region [Variables-Childs]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("Variables") != null)
                    {
                        foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                        {
                            if (string.IsNullOrEmpty(errorText) == true && tempVariable.Attribute("ID") != null && tempVariable.Attribute("Name") != null)
                            {
                                if (tempVariable.Attribute("ID").Value != string.Empty)
                                {
                                    expandedVariableInfo = this.VariableCollection[tempVariable.Attribute("ID").Value] as ExpandedVariableInfo;
                                }
                                else
                                {
                                    expandedVariableInfo = this.VariableCollection.Items.FirstOrDefault(t => t.Name == tempVariable.Attribute("Name").Value) as ExpandedVariableInfo;
                                }

                                if (expandedVariableInfo != null && tempVariable.Element("Childs") != null)
                                {
                                    parentNameStack.Clear();

                                    parentNameStack.Push(expandedVariableInfo.Name);
                                    GetChildVariableInfo(this.VariableCollection, expandedVariableInfo, tempVariable.Element("Childs"), parentNameStack, out errorText);
                                    parentNameStack.Pop();
                                }
                            }
                        }
                    }
                    #endregion
                    #region [Reports]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("Reports") != null)
                    {
                        foreach (XElement tempReport in root.Element("Reports").Elements("Report"))
                        {
                            if (tempReport.Attribute("ID") != null)
                            {
                                reportInfo = this.ReportCollection[tempReport.Attribute("ID").Value] as ExpandedReportInfo;

                                if (reportInfo == null)
                                {
                                    reportInfo = new ExpandedReportInfo()
                                    {
                                        IsInheritance = false,
                                        ReportID = tempReport.Attribute("ID").Value,
                                    };

                                    this.ReportCollection.Add(reportInfo);
                                }

                                if (tempReport.Attribute("Description") != null)
                                {
                                    reportInfo.Description = tempReport.Attribute("Description").Value;
                                }

                                readAttribute = string.Empty;

                                if (tempReport.Attribute("IDList") != null && reportInfo.Variables.Items.Count == 0)
                                {
                                    readAttribute = tempReport.Attribute("IDList").Value;

                                    foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                    {
                                        expandedVariableInfo = this.VariableCollection[tempItem] as ExpandedVariableInfo;

                                        if (expandedVariableInfo != null)
                                        {
                                            reportInfo.Variables.Add(expandedVariableInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region [Collection Events]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("CollectionEvents") != null)
                    {
                        foreach (XElement tempCollectionEvent in root.Element("CollectionEvents").Elements("CEID"))
                        {
                            if (tempCollectionEvent.Attribute("ID") != null)
                            {
                                collectionEventInfo = this.CollectionEventCollection.Items.Values.FirstOrDefault(t => t.CEID == tempCollectionEvent.Attribute("ID").Value) as ExpandedCollectionEventInfo;

                                if (collectionEventInfo == null)
                                {
                                    collectionEventInfo = new ExpandedCollectionEventInfo
                                    {
                                        IsInheritance = false,
                                        CEID = tempCollectionEvent.Attribute("ID").Value,
                                        IsBase = false
                                    };
                                    this.CollectionEventCollection.Items[collectionEventInfo.CEID] = collectionEventInfo;
                                }

                                if (tempCollectionEvent.Attribute("Name") != null)
                                {
                                    collectionEventInfo.Name = tempCollectionEvent.Attribute("Name").Value;
                                }

                                if (tempCollectionEvent.Attribute("Description") != null)
                                {
                                    collectionEventInfo.Description = tempCollectionEvent.Attribute("Description").Value;
                                }

                                if (tempCollectionEvent.Attribute("Enabled") != null && bool.TryParse(tempCollectionEvent.Attribute("Enabled").Value, out boolValue) == true)
                                {
                                    collectionEventInfo.Enabled = boolValue;
                                }

                                if (tempCollectionEvent.Attribute("Use") != null && bool.TryParse(tempCollectionEvent.Attribute("Use").Value, out boolValue) == true)
                                {
                                    collectionEventInfo.IsUse = boolValue;
                                }

                                if (tempCollectionEvent.Attribute("PreDefined") != null && bool.TryParse(tempCollectionEvent.Attribute("PreDefined").Value, out boolValue) == true)
                                {
                                    collectionEventInfo.PreDefined = boolValue;
                                }

                                if (tempCollectionEvent.Attribute("IDList") != null)
                                {
                                    readAttribute = tempCollectionEvent.Attribute("IDList").Value;

                                    foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                    {
                                        reportInfo = this.ReportCollection[tempItem] as ExpandedReportInfo;

                                        if (reportInfo != null)
                                        {
                                            collectionEventInfo.Reports.Add(reportInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region [Alarms]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("Alarms") != null)
                    {
                        foreach (XElement tempAlarmElement in root.Element("Alarms").Elements("Alarm"))
                        {
                            if (tempAlarmElement.Attribute("ID") != null && long.TryParse(tempAlarmElement.Attribute("ID").Value, out longValue) == true)
                            {
                                alarmInfo = this.AlarmCollection.Items.FirstOrDefault(t => t.ID == longValue) as ExpandedAlarmInfo;

                                if (alarmInfo == null)
                                {
                                    alarmInfo = new ExpandedAlarmInfo()
                                    {
                                        IsInheritance = false,
                                        ID = longValue
                                    };

                                    this.AlarmCollection.Items.Add(alarmInfo);
                                }

                                if (tempAlarmElement.Attribute("Code") != null && int.TryParse(tempAlarmElement.Attribute("Code").Value, out intValue) == true)
                                {
                                    alarmInfo.Code = intValue;
                                }

                                if (tempAlarmElement.Attribute("Description") != null)
                                {
                                    alarmInfo.Description = tempAlarmElement.Attribute("Description").Value;
                                }
                            }
                        }
                    }
                    #endregion
                    #region [Remote Commands]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("RemoteCommands") != null)
                    {
                        #region RemoteCommand
                        foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("RemoteCommand"))
                        {
                            if (tempRemoteCommand.Attribute("Command") != null)
                            {
                                expandedRemoteCommandInfo = this.RemoteCommandCollection.RemoteCommandItems.FirstOrDefault(t => t.RemoteCommand == tempRemoteCommand.Attribute("Command").Value) as ExpandedRemoteCommandInfo;

                                if (expandedRemoteCommandInfo == null)
                                {
                                    expandedRemoteCommandInfo = new ExpandedRemoteCommandInfo()
                                    {
                                        RemoteCommand = tempRemoteCommand.Attribute("Command").Value,
                                        IsInheritance = false,
                                    };

                                    if (tempRemoteCommand.Attribute("Description") != null)
                                    {
                                        expandedRemoteCommandInfo.Description = tempRemoteCommand.Attribute("Description").Value;
                                    }
                                    this.RemoteCommandCollection.Add(expandedRemoteCommandInfo);
                                }

                                if (tempRemoteCommand.Attribute("AutoSend") != null)
                                {
                                    if (bool.TryParse(tempRemoteCommand.Attribute("AutoSend").Value, out boolValue) == true)
                                    {
                                        expandedRemoteCommandInfo.AutoSend = boolValue;
                                    }
                                }

                                if (tempRemoteCommand.Attribute("TriggerCEID") != null)
                                {
                                    collectionEventInfo = this.CollectionEventCollection.Items.Values.FirstOrDefault(t => t.CEID == tempRemoteCommand.Attribute("TriggerCEID").Value) as ExpandedCollectionEventInfo;

                                    if (collectionEventInfo != null)
                                    {
                                        expandedRemoteCommandInfo.TriggerCollection = new AutoSendTriggerCollection(collectionEventInfo);
                                    }
                                }

                                if (tempRemoteCommand.Element("Triggers") != null)
                                {
                                    expandedRemoteCommandInfo.TriggerCollection = MakeTriggerCollectionFromXElement("Triggers", tempRemoteCommand);
                                }

                                #region load parameter for old version
                                if (tempRemoteCommand.Element("Parameters") != null)
                                {
                                    if (expandedRemoteCommandInfo.ValueSetCollection["Default"] == null)
                                    {
                                        expandedRemoteCommandInfo.ValueSetCollection.Add(new ExpandedRemoteCommandValueSetInfo()
                                        {
                                            Name = "Default"
                                        });
                                    }

                                    expandedRemoteCommandInfo.ValueSetCollection["Default"].ParameterItems.Clear();
                                    expandedRemoteCommandInfo.ValueSetCollection["Default"].AddParameterItems(MakeExpandedRemoteCommandParameterListFromXElement(tempRemoteCommand));
                                }
                                #endregion
                                #region load parameter for new version
                                if (tempRemoteCommand.Element("ValueSetCollection") != null)
                                {
                                    tempRCMDValueSetCollection = MakeExpandedRemoteCommandValueSetCollectionFromXElement(tempRemoteCommand);

                                    if (tempRCMDValueSetCollection["Default"] != null)
                                    {
                                        expandedRemoteCommandInfo.ValueSetCollection = tempRCMDValueSetCollection;
                                    }
                                    else
                                    {
                                        tempRCMDValueSetCollection.Add(expandedRemoteCommandInfo.ValueSetCollection["Default"]);
                                        expandedRemoteCommandInfo.ValueSetCollection = tempRCMDValueSetCollection;
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion
                        #region EnhancedRemoteCommand
                        foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("EnhancedRemoteCommand"))
                        {
                            if (tempRemoteCommand.Attribute("Command") != null)
                            {
                                expandedEnhancedRemoteCommandInfo = this.RemoteCommandCollection.EnhancedRemoteCommandItems.FirstOrDefault(t => t.RemoteCommand == tempRemoteCommand.Attribute("Command").Value) as ExpandedEnhancedRemoteCommandInfo;

                                if (expandedEnhancedRemoteCommandInfo == null)
                                {
                                    expandedEnhancedRemoteCommandInfo = new ExpandedEnhancedRemoteCommandInfo()
                                    {
                                        RemoteCommand = tempRemoteCommand.Attribute("Command").Value,
                                        IsInheritance = false,
                                    };

                                    if (tempRemoteCommand.Attribute("Description") != null)
                                    {
                                        expandedEnhancedRemoteCommandInfo.Description = tempRemoteCommand.Attribute("Description").Value;
                                    }
                                    else
                                    {
                                        expandedEnhancedRemoteCommandInfo.Description = string.Empty;
                                    }

                                    this.RemoteCommandCollection.Add(expandedEnhancedRemoteCommandInfo);
                                }

                                if (tempRemoteCommand.Attribute("DataID") != null)
                                {
                                    expandedEnhancedRemoteCommandInfo.DataID = tempRemoteCommand.Attribute("DataID").Value;
                                }

                                if (tempRemoteCommand.Attribute("ObjSpec") != null)
                                {
                                    expandedEnhancedRemoteCommandInfo.ObjSpec = tempRemoteCommand.Attribute("ObjSpec").Value;
                                }


                                if (tempRemoteCommand.Attribute("AutoSend") != null)
                                {
                                    if (bool.TryParse(tempRemoteCommand.Attribute("AutoSend").Value, out boolValue) == true)
                                    {
                                        expandedEnhancedRemoteCommandInfo.AutoSend = boolValue;
                                    }
                                }

                                if (tempRemoteCommand.Attribute("TriggerCEID") != null)
                                {
                                    collectionEventInfo = this.CollectionEventCollection.Items.Values.FirstOrDefault(t => t.CEID == tempRemoteCommand.Attribute("TriggerCEID").Value) as ExpandedCollectionEventInfo;

                                    if (collectionEventInfo != null)
                                    {
                                        expandedEnhancedRemoteCommandInfo.TriggerCollection = new AutoSendTriggerCollection(collectionEventInfo);
                                    }
                                }

                                if (tempRemoteCommand.Element("Triggers") != null)
                                {
                                    expandedEnhancedRemoteCommandInfo.TriggerCollection = MakeTriggerCollectionFromXElement("Triggers", tempRemoteCommand);
                                }

                                #region load parameter for old version
                                if (tempRemoteCommand.Element("Parameters") != null)
                                {
                                    if (expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"] == null)
                                    {
                                        expandedEnhancedRemoteCommandInfo.ValueSetCollection.Add(new ExpandedEnhancedRemoteCommandValueSetInfo()
                                        {
                                            Name = "Default"
                                        });
                                    }

                                    expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"].ParameterItems.Clear();
                                    expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"].AddParameterItems(MakeExpandedEnhancedRemoteCommandParameterListFromXElement(tempRemoteCommand));
                                }
                                #endregion
                                #region load parameter for new version
                                if (tempRemoteCommand.Element("ValueSetCollection") != null)
                                {
                                    tempERCMDValueSetCollection = MakeExpandedEnhancedRemoteCommandValueSetCollectionFromXElement(tempRemoteCommand);

                                    if (tempERCMDValueSetCollection["Default"] != null)
                                    {
                                        expandedEnhancedRemoteCommandInfo.ValueSetCollection = tempERCMDValueSetCollection;
                                    }
                                    else
                                    {
                                        tempERCMDValueSetCollection.Add(expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"]);
                                        expandedEnhancedRemoteCommandInfo.ValueSetCollection = tempERCMDValueSetCollection;
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region [FormattedProcessProgramCollection]
                    this.FormattedProcessProgramCollection.Clear();

                    if (string.IsNullOrEmpty(errorText) == true && root.Element("FormattedProcessProgramCollection") != null)
                    {
                        foreach (XElement tempFormattedProcessProgramInfo in root.Element("FormattedProcessProgramCollection").Elements("FormattedProcessProgramInfo"))
                        {
                            if (tempFormattedProcessProgramInfo.Attribute("PPID") != null && tempFormattedProcessProgramInfo.Attribute("MDLN") != null && tempFormattedProcessProgramInfo.Attribute("SOFTREV") != null)
                            {
                                fmtPPInfo = new FormattedProcessProgramInfo()
                                {
                                    PPID = tempFormattedProcessProgramInfo.Attribute("PPID").Value,
                                    MDLN = tempFormattedProcessProgramInfo.Attribute("MDLN").Value,
                                    SOFTREV = tempFormattedProcessProgramInfo.Attribute("SOFTREV").Value,
                                };

                                if (tempFormattedProcessProgramInfo.Attribute("AutoSend") != null)
                                {
                                    if (bool.TryParse(tempFormattedProcessProgramInfo.Attribute("AutoSend").Value, out boolValue) == true)
                                    {
                                        fmtPPInfo.AutoSend = boolValue;
                                    }
                                }
                                this.FormattedProcessProgramCollection.Add(fmtPPInfo);

                                fmtPPInfo.TriggerCollection = MakeTriggerCollectionFromXElement("Triggers", tempFormattedProcessProgramInfo);

                            }
                        }
                    }
                    #endregion
                    #region [GEMObject]
                    this.GEMObjectCollection.Clear();

                    if (string.IsNullOrEmpty(errorText) == true && root.Element("GEMObjectCollection") != null)
                    {
                        foreach (XElement tempGEMObjectElement in root.Element("GEMObjectCollection").Elements("GEMObject"))
                        {
                            if (tempGEMObjectElement.Attribute("OBJSPEC") != null && tempGEMObjectElement.Attribute("OBJTYPE") != null)
                            {
                                gemObject = new GEMObject()
                                {
                                    OBJSPEC = tempGEMObjectElement.Attribute("OBJSPEC").Value,
                                    OBJTYPE = tempGEMObjectElement.Attribute("OBJTYPE").Value,
                                };

                                this.GEMObjectCollection.Add(gemObject);

                                #region [AttributeCollection]
                                if (string.IsNullOrEmpty(errorText) == true && tempGEMObjectElement.Element("AttributeCollection") != null)
                                {
                                    foreach (XElement tempAttributeElement in tempGEMObjectElement.Element("AttributeCollection").Elements("Attribute"))
                                    {
                                        if (tempAttributeElement.Attribute("ATTRID") != null && tempAttributeElement.Attribute("Format") != null)
                                        {
                                            tempAttrData = string.Empty;

                                            if (Enum.TryParse(tempAttributeElement.Attribute("Format").Value, out tempAttrFormat) == true)
                                            {
                                                gemObjectAttribute = new GEMObjectAttribute()
                                                {
                                                    ATTRID = tempAttributeElement.Attribute("ATTRID").Value,
                                                    Format = tempAttrFormat,
                                                };

                                                boolValue = true;

                                                if (tempAttributeElement.Attribute("IsSelected") == null || bool.TryParse(tempAttributeElement.Attribute("IsSelected").Value, out boolValue) == false)
                                                {
                                                    boolValue = false;
                                                }

                                                gemObjectAttribute.IsSelected = boolValue;

                                                if (tempAttrFormat == SECSItemFormat.L)
                                                {
                                                    gemObjectAttribute.ChildAttributes = MakeGEMObjectAttributesFromXElement(tempAttributeElement);
                                                }
                                                else
                                                {
                                                    if (tempAttributeElement.Attribute("ATTRDATA") != null)
                                                    {
                                                        gemObjectAttribute.ATTRDATA = tempAttributeElement.Attribute("ATTRDATA").Value;
                                                    }
                                                }

                                                gemObject.AttributeCollection.Add(gemObjectAttribute);
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region [GEMObjectID]
                                if (string.IsNullOrEmpty(errorText) == true && tempGEMObjectElement.Element("GEMObjectIDCollection") != null)
                                {
                                    foreach (XElement tempGEMObjectIDElement in tempGEMObjectElement.Element("GEMObjectIDCollection").Elements("GEMObjectID"))
                                    {
                                        if (tempGEMObjectIDElement.Attribute("OBJID") != null)
                                        {
                                            gemObjectID = new GEMObjectID()
                                            {
                                                OBJID = tempGEMObjectIDElement.Attribute("OBJID").Value
                                            };

                                            boolValue = true;

                                            if (tempGEMObjectIDElement.Attribute("IsSelected") == null || bool.TryParse(tempGEMObjectIDElement.Attribute("IsSelected").Value, out boolValue) == false)
                                            {
                                                boolValue = false;
                                            }
                                            gemObjectID.IsSelected = boolValue;

                                            gemObject.ObjectIDCollection.Add(gemObjectID);

                                            #region [Attributes]
                                            if (string.IsNullOrEmpty(errorText) == true && tempGEMObjectIDElement.Element("GEMObjectAttributeCollection") != null)
                                            {
                                                foreach (XElement tempAttributeElement in tempGEMObjectIDElement.Element("GEMObjectAttributeCollection").Elements("GEMObjectAttribute"))
                                                {
                                                    if (tempAttributeElement.Attribute("ATTRID") != null && tempAttributeElement.Attribute("Format") != null)
                                                    {
                                                        tempAttrData = string.Empty;

                                                        if (Enum.TryParse(tempAttributeElement.Attribute("Format").Value, out tempAttrFormat) == true)
                                                        {
                                                            gemObjectAttribute = new GEMObjectAttribute()
                                                            {
                                                                ATTRID = tempAttributeElement.Attribute("ATTRID").Value,
                                                                Format = tempAttrFormat,
                                                            };

                                                            boolValue = true;

                                                            if (tempAttributeElement.Attribute("IsSelected") == null || bool.TryParse(tempAttributeElement.Attribute("IsSelected").Value, out boolValue) == false)
                                                            {
                                                                boolValue = false;
                                                            }

                                                            gemObjectAttribute.IsSelected = boolValue;

                                                            if (tempAttrFormat == SECSItemFormat.L)
                                                            {
                                                                gemObjectAttribute.ChildAttributes = MakeGEMObjectAttributesFromXElement(tempAttributeElement);
                                                            }
                                                            else
                                                            {
                                                                if (tempAttributeElement.Attribute("ATTRDATA") != null)
                                                                {
                                                                    gemObjectAttribute.ATTRDATA = tempAttributeElement.Attribute("ATTRDATA").Value;
                                                                }
                                                            }

                                                            gemObjectID.ObjectAttributeCollection.Add(gemObjectAttribute);
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #region [SupervisedGEMObjectCollection]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("SupervisedGEMObjectCollection") != null)
                    {
                        this.SupervisedGEMObjectCollection.Clear();

                        foreach (XElement tempGEMObjectElement in root.Element("SupervisedGEMObjectCollection").Elements("SupervisedGEMObject"))
                        {
                            if (tempGEMObjectElement.Attribute("OBJSPEC") != null && tempGEMObjectElement.Attribute("OBJTOKEN") != null)
                            {
                                supervisedGEMObject = new SupervisedGEMObject()
                                {
                                    OBJSPEC = tempGEMObjectElement.Attribute("OBJSPEC").Value,
                                };

                                objToken = 0;

                                if (uint.TryParse(tempGEMObjectElement.Attribute("OBJTOKEN").Value, out objToken) == true)
                                {
                                    supervisedGEMObject.OBJTOKEN = objToken;
                                }

                                this.SupervisedGEMObjectCollection.Add(supervisedGEMObject);

                                #region [Attributes]
                                if (string.IsNullOrEmpty(errorText) == true && tempGEMObjectElement.Element("GEMObjectAttributeCollection") != null)
                                {
                                    foreach (XElement tempAttributeElement in tempGEMObjectElement.Element("GEMObjectAttributeCollection").Elements("GEMObjectAttribute"))
                                    {
                                        if (tempAttributeElement.Attribute("ATTRID") != null && tempAttributeElement.Attribute("Format") != null)
                                        {
                                            tempAttrData = string.Empty;

                                            if (Enum.TryParse(tempAttributeElement.Attribute("Format").Value, out tempAttrFormat) == true)
                                            {
                                                gemObjectAttribute = new GEMObjectAttribute()
                                                {
                                                    ATTRID = tempAttributeElement.Attribute("ATTRID").Value,
                                                    Format = tempAttrFormat,
                                                };

                                                boolValue = true;

                                                if (tempAttributeElement.Attribute("IsSelected") == null || bool.TryParse(tempAttributeElement.Attribute("IsSelected").Value, out boolValue) == false)
                                                {
                                                    boolValue = false;
                                                }

                                                gemObjectAttribute.IsSelected = boolValue;

                                                if (tempAttrFormat == SECSItemFormat.L)
                                                {
                                                    gemObjectAttribute.ChildAttributes = MakeGEMObjectAttributesFromXElement(tempAttributeElement);
                                                }
                                                else
                                                {
                                                    if (tempAttributeElement.Attribute("ATTRDATA") != null)
                                                    {
                                                        gemObjectAttribute.ATTRDATA = tempAttributeElement.Attribute("ATTRDATA").Value;
                                                    }
                                                }

                                                supervisedGEMObject.GEMObjectAttributeCollection.Add(gemObjectAttribute);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #region [MapSetupDataCollection]

                    this.MapSetupDataCollection.Clear();

                    if (string.IsNullOrEmpty(errorText) && root.Element("MapSetupDataCollection") != null)
                    {
                        foreach (XElement tempMapSetupDataElement in root.Element("MapSetupDataCollection").Elements("MapSetupData"))
                        {
                            if (tempMapSetupDataElement.Attribute("MID") != null)
                            {
                                materialID = tempMapSetupDataElement.Attribute("MID").Value;

                                mapSetupData = new MapSetupData()
                                {
                                    MaterialID = materialID
                                };

                                if (tempMapSetupDataElement.Attribute("IDTYP") != null && byte.TryParse(tempMapSetupDataElement.Attribute("IDTYP").Value, out byteValue) == true)
                                {
                                    mapSetupData.IDType = byteValue;
                                }

                                if (tempMapSetupDataElement.Attribute("FNLOC") != null)
                                {
                                    mapSetupData.FlatNotchLocation = tempMapSetupDataElement.Attribute("FNLOC").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("FFROT") != null)
                                {
                                    mapSetupData.FilmFrameRotation = tempMapSetupDataElement.Attribute("FFROT").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("ORLOC") != null)
                                {
                                    mapSetupData.OriginLocation = tempMapSetupDataElement.Attribute("ORLOC").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("RPSEL") != null && byte.TryParse(tempMapSetupDataElement.Attribute("RPSEL").Value, out byteValue) == true)
                                {
                                    mapSetupData.ReferencePointSelect = byteValue;
                                }

                                if (tempMapSetupDataElement.Attribute("DUTMS") != null)
                                {
                                    mapSetupData.DieUnitsOfMeasure = tempMapSetupDataElement.Attribute("DUTMS").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("XDIES") != null)
                                {
                                    mapSetupData.XAxisDieSize = tempMapSetupDataElement.Attribute("XDIES").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("YDIES") != null)
                                {
                                    mapSetupData.YAxisDieSize = tempMapSetupDataElement.Attribute("YDIES").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("ROWCT") != null && ulong.TryParse(tempMapSetupDataElement.Attribute("ROWCT").Value, out ulong ulongValue) == true)
                                {
                                    mapSetupData.RowCount = ulongValue;
                                }

                                if (tempMapSetupDataElement.Attribute("COLCT") != null && ulong.TryParse(tempMapSetupDataElement.Attribute("COLCT").Value, out ulongValue) == true)
                                {
                                    mapSetupData.ColumnCount = ulongValue;
                                }

                                if (tempMapSetupDataElement.Attribute("NULBC") != null)
                                {
                                    mapSetupData.NullBinCodeValue = tempMapSetupDataElement.Attribute("NULBC").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("BCEQU") != null)
                                {
                                    mapSetupData.BinCodeEquivalent = tempMapSetupDataElement.Attribute("BCEQU").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("PRDCT") != null)
                                {
                                    mapSetupData.ProcessDieCount = tempMapSetupDataElement.Attribute("PRDCT").Value;
                                }

                                if (tempMapSetupDataElement.Attribute("PRAXI") != null && byte.TryParse(tempMapSetupDataElement.Attribute("PRAXI").Value, out byteValue) == true)
                                {
                                    mapSetupData.ProcessAxis = byteValue;
                                }

                                if (tempMapSetupDataElement.Attribute("MLCL") != null && ulong.TryParse(tempMapSetupDataElement.Attribute("MLCL").Value, out ulongValue) == true)
                                {
                                    mapSetupData.MessageLength = ulongValue;
                                }

                                if (tempMapSetupDataElement.Element("ReferencePointCollection") != null)
                                {
                                    foreach (XElement rpElement in tempMapSetupDataElement.Element("ReferencePointCollection").Elements("ReferencePointItem"))
                                    {

                                        if (rpElement.Attribute("X") != null && rpElement.Attribute("Y") != null)
                                        {
                                            rpItem = new ReferencePointItem();

                                            if (long.TryParse(rpElement.Attribute("X").Value, out longValue) == true)
                                            {
                                                rpItem.X = longValue;
                                            }

                                            if (long.TryParse(rpElement.Attribute("Y").Value, out longValue) == true)
                                            {
                                                rpItem.Y = longValue;
                                            }

                                            if (mapSetupData.ReferencePoint.FirstOrDefault(t => t.X == rpItem.X && t.Y == rpItem.Y) == null)
                                            {
                                                mapSetupData.ReferencePoint.Add(rpItem);
                                            }
                                        }
                                    }
                                }

                                this.MapSetupDataCollection.Add(mapSetupData);
                            }
                        }
                    }
                    #endregion
                    #region [MapDataType1Collection]

                    this.MapDataType1Collection.Clear();

                    if (string.IsNullOrEmpty(errorText) && root.Element("MapDataType1Collection") != null)
                    {
                        foreach (XElement tempMapSetupDataElement in root.Element("MapDataType1Collection").Elements("MapDataType1"))
                        {
                            if (tempMapSetupDataElement.Attribute("MID") != null)
                            {
                                materialID = tempMapSetupDataElement.Attribute("MID").Value;

                                mapDataType1 = new MapDataType1()
                                {
                                    MaterialID = materialID
                                };

                                if (tempMapSetupDataElement.Attribute("IDTYP") != null && byte.TryParse(tempMapSetupDataElement.Attribute("IDTYP").Value, out byteValue) == true)
                                {
                                    mapDataType1.IDType = byteValue;
                                }
                                if (tempMapSetupDataElement.Element("ReferenceStartingCollection") != null)
                                {
                                    foreach (XElement rpElement in tempMapSetupDataElement.Element("ReferenceStartingCollection").Elements("ReferenceStartingInfo"))
                                    {
                                        if (rpElement.Attribute("X") != null && rpElement.Attribute("Y") != null)
                                        {
                                            rsInfo = new ReferenceStartingInfo();

                                            if (long.TryParse(rpElement.Attribute("X").Value, out longValue) == true)
                                            {
                                                rsInfo.X = longValue;
                                            }

                                            if (long.TryParse(rpElement.Attribute("Y").Value, out longValue) == true)
                                            {
                                                rsInfo.Y = longValue;
                                            }

                                            if (long.TryParse(rpElement.Attribute("Direction").Value, out longValue) == true)
                                            {
                                                rsInfo.Direction = longValue;
                                            }

                                            if (rpElement.Attribute("BINLT") != null)
                                            {
                                                rsInfo.BinList = rpElement.Attribute("BINLT").Value;
                                            }

                                            if (mapDataType1.ReferenceStartingList.FirstOrDefault(t => t.X == rsInfo.X && t.Y == rsInfo.Y) == null)
                                            {
                                                mapDataType1.ReferenceStartingList.Add(rsInfo);
                                            }
                                        }
                                    }
                                }

                                this.MapDataType1Collection.Add(mapDataType1);
                            }
                        }
                    }
                    #endregion
                    #region [MapDataType2Collection]

                    this.MapDataType2Collection.Clear();

                    if (string.IsNullOrEmpty(errorText) && root.Element("MapDataType2Collection") != null)
                    {
                        foreach (XElement tempMapSetupDataElement in root.Element("MapDataType2Collection").Elements("MapDataType2"))
                        {
                            if (tempMapSetupDataElement.Attribute("MID") != null)
                            {
                                materialID = tempMapSetupDataElement.Attribute("MID").Value;

                                mapDataType2 = new MapDataType2()
                                {
                                    MaterialID = materialID
                                };

                                if (tempMapSetupDataElement.Attribute("IDTYP") != null && byte.TryParse(tempMapSetupDataElement.Attribute("IDTYP").Value, out byteValue) == true)
                                {
                                    mapDataType2.IDType = byteValue;
                                }

                                if (tempMapSetupDataElement.Attribute("STRPx") != null && long.TryParse(tempMapSetupDataElement.Attribute("STRPx").Value, out longValue) == true)
                                {
                                    mapDataType2.StartPointX = longValue;
                                }

                                if (tempMapSetupDataElement.Attribute("STRPy") != null && long.TryParse(tempMapSetupDataElement.Attribute("STRPy").Value, out longValue) == true)
                                {
                                    mapDataType2.StartPointY = longValue;
                                }

                                if (tempMapSetupDataElement.Attribute("BINLT") != null)
                                {
                                    mapDataType2.BinList = tempMapSetupDataElement.Attribute("BINLT").Value;
                                }

                                this.MapDataType2Collection.Add(mapDataType2);
                            }
                        }
                    }
                    #endregion
                    #region [MapDataType3Collection]

                    this.MapDataType3Collection.Clear();

                    if (string.IsNullOrEmpty(errorText) && root.Element("MapDataType3Collection") != null)
                    {
                        foreach (XElement tempMapSetupDataElement in root.Element("MapDataType3Collection").Elements("MapDataType3"))
                        {
                            if (tempMapSetupDataElement.Attribute("MID") != null)
                            {
                                materialID = tempMapSetupDataElement.Attribute("MID").Value;

                                mapDataType3 = new MapDataType3()
                                {
                                    MaterialID = materialID
                                };

                                if (tempMapSetupDataElement.Attribute("IDTYP") != null && byte.TryParse(tempMapSetupDataElement.Attribute("IDTYP").Value, out byteValue) == true)
                                {
                                    mapDataType3.IDType = byteValue;
                                }
                                if (tempMapSetupDataElement.Element("XYPOSCollection") != null)
                                {
                                    foreach (XElement rpElement in tempMapSetupDataElement.Element("XYPOSCollection").Elements("XYPOSInfo"))
                                    {
                                        if (rpElement.Attribute("X") != null && rpElement.Attribute("Y") != null)
                                        {
                                            xyPosInfo = new XYPosInfo();

                                            if (long.TryParse(rpElement.Attribute("X").Value, out longValue) == true)
                                            {
                                                xyPosInfo.X = longValue;
                                            }

                                            if (long.TryParse(rpElement.Attribute("Y").Value, out longValue) == true)
                                            {
                                                xyPosInfo.Y = longValue;
                                            }

                                            if (rpElement.Attribute("BINLT") != null)
                                            {
                                                xyPosInfo.BinList = rpElement.Attribute("BINLT").Value;
                                            }

                                            if (mapDataType3.XYPOSList.FirstOrDefault(t => t.X == xyPosInfo.X && t.Y == xyPosInfo.Y) == null)
                                            {
                                                mapDataType3.XYPOSList.Add(xyPosInfo);
                                            }
                                        }
                                    }
                                }

                                this.MapDataType3Collection.Add(mapDataType3);
                            }
                        }
                    }
                    #endregion
                    #region [GEM Setting]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("GEMSetting") != null)
                    {
                        element = root.Element("GEMSetting");

                        #region [Stream 1]
                        subElement = element.Element("S1F3");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S1F3SelectedEquipmentStatus.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S1F3SelectedEquipmentStatus.Add(tempItem);
                                }
                            }
                        }

                        subElement = element.Element("S1F11");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S1F11StatusVariableNamelist.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S1F11StatusVariableNamelist.Add(tempItem);
                                }
                            }
                        }

                        subElement = element.Element("S1F21");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S1F21DataVariableNamelist.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S1F21DataVariableNamelist.Add(tempItem);
                                }
                            }
                        }

                        subElement = element.Element("S1F23");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S1F23CollectionEventList.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S1F23CollectionEventList.Add(tempItem);
                                }
                            }
                        }
                        #endregion
                        #region [Stream 2]
                        subElement = element.Element("S2F13");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S2F13EquipmentConstant.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S2F13EquipmentConstant.Add(tempItem);
                                }
                            }
                        }

                        subElement = element.Element("S2F15");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S2F15NewEquipmentConstant.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S2F15NewEquipmentConstant.Add(tempItem);
                                }
                            }
                        }

                        subElement = element.Element("S2F25");

                        if (subElement != null)
                        {
                            this.CurrentSetting.LoopbackDiagnostic.Clear();

                            if (subElement.Attribute("ABS") != null)
                            {
                                readAttribute = subElement.Attribute("ABS").Value;

                                foreach (string tempItem in readAttribute.Split(' ').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.LoopbackDiagnostic.Add(byte.Parse(tempItem));
                                }
                            }
                        }

                        subElement = element.Element("S2F29");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S2F29EquipmentConstantNamelist.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S2F29EquipmentConstantNamelist.Add(tempItem);
                                }
                            }
                        }

                        subElement = element.Element("S2F43");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S2F43ResetSpoolingStreamsAndFunctions.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S2F43ResetSpoolingStreamsAndFunctions.Add(tempItem);
                                }
                            }
                        }

                        subElement = element.Element("S2F47");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S2F47VariableLimitAttributeRequest.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    this.CurrentSetting.S2F47VariableLimitAttributeRequest.Add(tempItem);
                                }
                            }
                        }
                        #endregion
                        #region [Stream 5]
                        subElement = element.Element("S5F3Selected");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S5F3SelectedAlarmSend.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    if (long.TryParse(tempItem, out longValue) == true)
                                    {
                                        this.CurrentSetting.S5F3SelectedAlarmSend.Add(longValue);
                                    }
                                }
                            }
                        }

                        subElement = element.Element("S5F3Enabled");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S5F3EnabledAlarmSend.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    if (long.TryParse(tempItem, out longValue) == true)
                                    {
                                        this.CurrentSetting.S5F3EnabledAlarmSend.Add(longValue);
                                    }
                                }
                            }
                        }

                        subElement = element.Element("S5F5");

                        if (subElement != null)
                        {
                            this.CurrentSetting.S5F5ListAlarmsRequest.Clear();

                            if (subElement.Attribute("Use") != null)
                            {
                                readAttribute = subElement.Attribute("Use").Value;

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    if (long.TryParse(tempItem, out longValue) == true)
                                    {
                                        this.CurrentSetting.S5F5ListAlarmsRequest.Add(longValue);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region [Stream 10]
                        subElement = element.Element("S10F3");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("TID") != null)
                            {
                                this.CurrentSetting.TerminalMessage.S10F3TID = subElement.Attribute("TID").Value;
                                this.CurrentSetting.TerminalMessage.S10F3TerminalMessage = subElement.Attribute("TEXT").Value;
                            }
                        }

                        subElement = element.Element("S10F5");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("TID") != null)
                            {
                                this.CurrentSetting.TerminalMessage.S10F5TID = subElement.Attribute("TID").Value;
                            }

                            if (subElement.Element("TEXTs").Elements("TEXT") != null)
                            {
                                this.CurrentSetting.TerminalMessage.S10F5TerminalMessages.Clear();

                                foreach (XElement tempTerminalMessage in subElement.Element("TEXTs").Elements("TEXT"))
                                {
                                    this.CurrentSetting.TerminalMessage.S10F5TerminalMessages.Add(tempTerminalMessage.Attribute("TEXT").Value);
                                }
                            }
                        }
                        #endregion
                        #region [Stream12]
                        subElement = element.Element("S12F19");
                        this.CurrentSetting.SelectedMapError = 0;
                        this.CurrentSetting.DATLC = 0;
                        if (subElement != null)
                        {
                            if (subElement.Attribute("MAPER") != null)
                            {
                                if (byte.TryParse(subElement.Attribute("MAPER").Value, out byteValue) == true)
                                {
                                    this.CurrentSetting.SelectedMapError = byteValue;
                                }
                            }

                            if (subElement.Attribute("DATLC") != null)
                            {
                                if (byte.TryParse(subElement.Attribute("DATLC").Value, out byteValue) == true)
                                {
                                    this.CurrentSetting.DATLC = byteValue;
                                }
                            }
                        }
                        #endregion
                        #region [Stream14]

                        this.CurrentSetting.SelectedObjectSpecifierForS14F1 = string.Empty;
                        this.CurrentSetting.SelectedObjectTypeForS14F1 = string.Empty;
                        this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1.Clear();

                        this.CurrentSetting.SelectedObjectSpecifierForS14F3 = string.Empty;
                        this.CurrentSetting.SelectedObjectTypeForS14F3 = string.Empty;
                        this.CurrentSetting.SelectedObjectSpecifierForS14F5 = string.Empty;
                        this.CurrentSetting.SelectedObjectSpecifierForS14F7 = string.Empty;
                        this.CurrentSetting.SelectedObjectSpecifierForS14F9 = string.Empty;
                        this.CurrentSetting.SelectedObjectTypeForS14F9 = string.Empty;
                        this.CurrentSetting.SelectedObjectSpecifierForS14F11 = string.Empty;
                        this.CurrentSetting.SelectedObjectSpecifierForS14F13 = string.Empty;
                        this.CurrentSetting.SelectedObjectSpecifierForS14F15 = string.Empty;
                        this.CurrentSetting.SelectedAttachedObjectActionListForS14F15.Clear();

                        this.CurrentSetting.SelectedObjectSpecifierForS14F17 = string.Empty;
                        this.CurrentSetting.SelectedAttachedObjectActionListForS14F17.Clear();

                        subElement = element.Element("S14F1");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null && subElement.Attribute("SelectedObjectSpecifier").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F1 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }

                            if (subElement.Attribute("SelectedObjectType") != null && subElement.Attribute("SelectedObjectType").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectTypeForS14F1 = subElement.Attribute("SelectedObjectType").Value;
                            }
                            foreach (XElement selectedObjectAttributeIDElement in subElement.Elements("SelectedObjectAttributeFilterList"))
                            {
                                if (selectedObjectAttributeIDElement.Attribute("OBJSPEC") != null && selectedObjectAttributeIDElement.Attribute("OBJTYPE") != null)
                                {
                                    keyForSelectedObjectList = this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1.Keys.FirstOrDefault(t => t.OBJSPEC == selectedObjectAttributeIDElement.Attribute("OBJSPEC").Value && t.OBJTYPE == selectedObjectAttributeIDElement.Attribute("OBJTYPE").Value);

                                    if (keyForSelectedObjectList == null)
                                    {
                                        keyForSelectedObjectList = new KeyForSelectedObjectList()
                                        {
                                            OBJSPEC = selectedObjectAttributeIDElement.Attribute("OBJSPEC").Value,
                                            OBJTYPE = selectedObjectAttributeIDElement.Attribute("OBJTYPE").Value
                                        };

                                        this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1[keyForSelectedObjectList] = new List<GEMObjectAttributeFilterInfo>();
                                    }

                                    if (this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1[keyForSelectedObjectList] == null)
                                    {
                                        this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1[keyForSelectedObjectList] = new List<GEMObjectAttributeFilterInfo>();
                                    }

                                    if (selectedObjectAttributeIDElement.Element("AttributeFilter") != null)
                                    {
                                        foreach (var attributeFilterElement in selectedObjectAttributeIDElement.Elements("AttributeFilter"))
                                        {
                                            gemObjectAttributeFilterInfo = new GEMObjectAttributeFilterInfo();

                                            if (attributeFilterElement.Attribute("IsSelected") != null && bool.TryParse(attributeFilterElement.Attribute("IsSelected").Value, out boolValue) == true)
                                            {
                                                gemObjectAttributeFilterInfo.IsSelected = boolValue;
                                            }

                                            if (attributeFilterElement.Attribute("ObjectID") != null)
                                            {
                                                gemObjectAttributeFilterInfo.ObjectID = attributeFilterElement.Attribute("ObjectID").Value;
                                            }

                                            if (attributeFilterElement.Attribute("ATTRID") != null)
                                            {
                                                gemObjectAttributeFilterInfo.ATTRID = attributeFilterElement.Attribute("ATTRID").Value;
                                            }

                                            if (attributeFilterElement.Attribute("ATTRDATA") != null)
                                            {
                                                gemObjectAttributeFilterInfo.ATTRDATA = attributeFilterElement.Attribute("ATTRDATA").Value;
                                            }

                                            if (attributeFilterElement.Attribute("ATTRRELN") != null)
                                            {
                                                gemObjectAttributeFilterInfo.ATTRRELN = attributeFilterElement.Attribute("ATTRRELN").Value;
                                            }

                                            if (string.IsNullOrEmpty(gemObjectAttributeFilterInfo.ObjectID) == false && string.IsNullOrEmpty(gemObjectAttributeFilterInfo.ATTRID) == false)
                                            {
                                                this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1[keyForSelectedObjectList].Add(gemObjectAttributeFilterInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        subElement = element.Element("S14F3");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null && subElement.Attribute("SelectedObjectSpecifier").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F3 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }

                            if (subElement.Attribute("SelectedObjectType") != null && subElement.Attribute("SelectedObjectType").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectTypeForS14F3 = subElement.Attribute("SelectedObjectType").Value;
                            }
                        }

                        subElement = element.Element("S14F5");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null && subElement.Attribute("SelectedObjectSpecifier").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F3 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }
                        }

                        subElement = element.Element("S14F7");

                        if (subElement != null )
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F7 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }

                            if (subElement.Element("SelectedType") != null)
                            {
                                foreach (var selectedTypeElement in subElement.Elements("SelectedType"))
                                {
                                    objSpec = string.Empty;

                                    if (selectedTypeElement.Attribute("OBJSPEC") != null)
                                    {
                                        objSpec = selectedTypeElement.Attribute("OBJSPEC").Value;

                                        if (string.IsNullOrEmpty(objSpec) == false)
                                        {
                                            if (this.CurrentSetting.SelectedObjectTypeListForS14F7.ContainsKey(objSpec) == false)
                                            {
                                                this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec] = new List<string>();
                                            }

                                            if (this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec] == null)
                                            {
                                                this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec] = new List<string>();
                                            }

                                            if (selectedTypeElement.Attribute("SelectedTypes") != null)
                                            {
                                                readAttribute = selectedTypeElement.Attribute("SelectedTypes").Value;

                                                foreach (string objType in readAttribute.Split(','))
                                                {
                                                    if (this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec].Contains(objType) == false)
                                                    {
                                                        this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec].Add(objType);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        subElement = element.Element("S14F9");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null && subElement.Attribute("SelectedObjectSpecifier").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F9 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }

                            if (subElement.Attribute("SelectedObjectType") != null && subElement.Attribute("SelectedObjectType").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectTypeForS14F9 = subElement.Attribute("SelectedObjectType").Value;
                            }
                        }

                        subElement = element.Element("S14F13");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null && subElement.Attribute("SelectedObjectSpecifier").Value != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F13 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }
                        }

                        subElement = element.Element("S14F15");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F15 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }

                            if (subElement.Element("AttachedObjectActionInfo") != null)
                            {
                                foreach (var attachedObjectActionElement in subElement.Elements("AttachedObjectActionInfo"))
                                {
                                    objSpec = string.Empty;

                                    if (attachedObjectActionElement.Attribute("OBJSPEC") != null)
                                    {
                                        objSpec = attachedObjectActionElement.Attribute("OBJSPEC").Value;
                                    }

                                    if (string.IsNullOrEmpty(objSpec) == false)
                                    {
                                        if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F15.ContainsKey(objSpec) == false)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[objSpec] = new AttachedObjectActionInfo();
                                        }

                                        if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[objSpec] == null)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[objSpec] = new AttachedObjectActionInfo();
                                        }

                                        if (attachedObjectActionElement.Attribute("OBJCMD") != null && byte.TryParse(attachedObjectActionElement.Attribute("OBJCMD").Value, out byteValue) == true)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[objSpec].OBJCMD = byteValue;
                                        }

                                        if (attachedObjectActionElement.Attribute("OBJTOKEN") != null && uint.TryParse(attachedObjectActionElement.Attribute("OBJTOKEN").Value, out uint uintValue) == true)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[objSpec].OBJTOKEN = uintValue;
                                        }
                                    }
                                }
                            }
                        }

                        subElement = element.Element("S14F17");

                        if (subElement != null)
                        {
                            if (subElement.Attribute("SelectedObjectSpecifier") != null)
                            {
                                this.CurrentSetting.SelectedObjectSpecifierForS14F17 = subElement.Attribute("SelectedObjectSpecifier").Value;
                            }

                            if (subElement.Element("AttachedObjectActionInfo") != null)
                            {
                                foreach (var attachedObjectActionElement in subElement.Elements("AttachedObjectActionInfo"))
                                {
                                    objSpec = string.Empty;

                                    if (attachedObjectActionElement.Attribute("OBJSPEC") != null)
                                    {
                                        objSpec = attachedObjectActionElement.Attribute("OBJSPEC").Value;
                                    }

                                    if (string.IsNullOrEmpty(objSpec) == false)
                                    {
                                        if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F17.ContainsKey(objSpec) == false)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[objSpec] = new AttachedObjectActionInfo();
                                        }

                                        if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[objSpec] == null)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[objSpec] = new AttachedObjectActionInfo();
                                        }

                                        if (attachedObjectActionElement.Attribute("OBJCMD") != null && byte.TryParse(attachedObjectActionElement.Attribute("OBJCMD").Value, out byteValue) == true)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[objSpec].OBJCMD = byteValue;
                                        }

                                        if (attachedObjectActionElement.Attribute("TARGETSPEC") != null)
                                        {
                                            this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[objSpec].TARGETSPEC = attachedObjectActionElement.Attribute("TARGETSPEC").Value;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region [ACK]
                        subElement = element.Element("ACK");

                        if (subElement != null)
                        {
                            foreach (XElement ackItem in subElement.Elements("ACKItem"))
                            {
                                int ackStream = -1;
                                int ackFunction = -1;
                                bool ackUse = false;
                                byte ackValue = 0;

                                if (ackItem.Attribute("Stream") != null && string.IsNullOrEmpty(ackItem.Attribute("Stream").Value) == false)
                                {
                                    if (int.TryParse(ackItem.Attribute("Stream").Value, out intValue) == true)
                                    {
                                        ackStream = intValue;
                                    }
                                    else
                                    {
                                        ackStream = -1;
                                    }
                                }

                                if (ackItem.Attribute("Function") != null && string.IsNullOrEmpty(ackItem.Attribute("Function").Value) == false)
                                {
                                    if (int.TryParse(ackItem.Attribute("Function").Value, out intValue) == true)
                                    {
                                        ackFunction = intValue;
                                    }
                                    else
                                    {
                                        ackFunction = -1;
                                    }
                                }

                                if (ackItem.Attribute("Use") != null && string.IsNullOrEmpty(ackItem.Attribute("Use").Value) == false)
                                {
                                    if (bool.TryParse(ackItem.Attribute("Use").Value, out boolValue) == true)
                                    {
                                        ackUse = boolValue;
                                    }
                                    else
                                    {
                                        ackUse = false;
                                    }
                                }

                                if (ackItem.Attribute("Value") != null && string.IsNullOrEmpty(ackItem.Attribute("Value").Value) == false)
                                {
                                    if (byte.TryParse(ackItem.Attribute("Value").Value, out byteValue) == true)
                                    {
                                        ackValue = byteValue;
                                    }
                                    else
                                    {
                                        ackValue = 0;
                                    }
                                }

                                if (ackStream != -1 && ackFunction != -1)
                                {
                                    ackInfo = this.CurrentSetting.AckCollection[ackStream, ackFunction];

                                    if (ackInfo == null)
                                    {
                                        ackInfo = new AckInfo()
                                        {
                                            Stream = ackStream,
                                            Function = ackFunction,
                                            Use = ackUse,
                                            Value = ackValue,
                                        };
                                        this.CurrentSetting.AckCollection.Add(ackInfo);
                                    }
                                    else
                                    {
                                        ackInfo.Use = ackUse;
                                        ackInfo.Value = ackValue;
                                    }
                                }
                            }
                        }
                        #endregion
                        #region [UseReply]
                        subElement = element.Element("UseReply");

                        if (subElement != null)
                        {
                            foreach (var replyElement in subElement.Elements("ReplyItem"))
                            {
                                if (replyElement.Attribute("Stream") != null && replyElement.Attribute("Function") != null)
                                {
                                    if (int.TryParse(replyElement.Attribute("Stream").Value, out int stream) == true && int.TryParse(replyElement.Attribute("Function").Value, out int function) == true)
                                    {
                                        replyItem = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == stream && t.Function == function);

                                        if (replyItem != null)
                                        {
                                            if (replyElement.Attribute("Reply") != null)
                                            {
                                                if (bool.TryParse(replyElement.Attribute("Reply").Value, out boolValue) == true)
                                                {
                                                    replyItem.SendReply = boolValue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region [ETC]
                        subElement = element.Element("ETC");

                        if (subElement != null)
                        {
                            if (subElement.Element("ProcessProgramFileS7F1") != null && subElement.Element("ProcessProgramFileS7F1").Attribute("FileName") != null)
                            {
                                this.CurrentSetting.ProcessProgramFileS7F1 = subElement.Element("ProcessProgramFileS7F1").Attribute("FileName").Value;
                            }
                            else
                            {
                                this.CurrentSetting.ProcessProgramFileS7F1 = string.Empty;
                            }

                            if (subElement.Element("ProcessProgramFileS7F3") != null && subElement.Element("ProcessProgramFileS7F3").Attribute("FileName") != null)
                            {
                                this.CurrentSetting.ProcessProgramFileS7F3 = subElement.Element("ProcessProgramFileS7F3").Attribute("FileName").Value;
                            }
                            else
                            {
                                this.CurrentSetting.ProcessProgramFileS7F3 = string.Empty;
                            }

                            if (subElement.Element("ProcessProgramIDS7F5") != null && subElement.Element("ProcessProgramIDS7F5").Attribute("PPID") != null)
                            {
                                this.CurrentSetting.ProcessProgramIDS7F5 = subElement.Element("ProcessProgramIDS7F5").Attribute("PPID").Value;
                            }
                            else
                            {
                                this.CurrentSetting.ProcessProgramIDS7F5 = string.Empty;
                            }

                            if (subElement.Element("ProcessProgramIDS7F23") != null && subElement.Element("ProcessProgramIDS7F23").Attribute("PPID") != null)
                            {
                                this.CurrentSetting.ProcessProgramIDS7F23 = subElement.Element("ProcessProgramIDS7F23").Attribute("PPID").Value;
                            }
                            else
                            {
                                this.CurrentSetting.ProcessProgramIDS7F23 = string.Empty;
                            }

                            if (subElement.Element("ProcessProgramIDS7F25") != null && subElement.Element("ProcessProgramIDS7F25").Attribute("PPID") != null)
                            {
                                this.CurrentSetting.ProcessProgramIDS7F25 = subElement.Element("ProcessProgramIDS7F25").Attribute("PPID").Value;
                            }
                            else
                            {
                                this.CurrentSetting.ProcessProgramIDS7F25 = string.Empty;
                            }

                            if (subElement.Element("ProcessProgramDelete") != null && subElement.Element("ProcessProgramDelete").Attribute("IDList") != null)
                            {
                                this.CurrentSetting.ProcessProgramDelete.Clear();

                                readAttribute = subElement.Element("ProcessProgramDelete").Attribute("IDList").Value;

                                foreach (string pid in readAttribute.Split(','))
                                {
                                    this.CurrentSetting.ProcessProgramDelete.Add(pid);
                                }
                            }

                            if (subElement.Element("AutoSendDefineReport") != null && subElement.Element("AutoSendDefineReport").Attribute("Value") != null)
                            {
                                if (bool.TryParse(subElement.Element("AutoSendDefineReport").Attribute("Value").Value, out boolValue) == true)
                                {
                                    this.CurrentSetting.AutoSendDefineReport = boolValue;
                                }
                            }

                            if (subElement.Element("IsSaveRecipeReceived") != null && subElement.Element("IsSaveRecipeReceived").Attribute("Value") != null)
                            {
                                if (bool.TryParse(subElement.Element("IsSaveRecipeReceived").Attribute("Value").Value, out boolValue) == true)
                                {
                                    this.CurrentSetting.IsSaveRecipeReceived = boolValue;
                                }
                            }

                            if (subElement.Element("AutoSendS1F13") != null && subElement.Element("AutoSendS1F13").Attribute("Value") != null)
                            {
                                if (bool.TryParse(subElement.Element("AutoSendS1F13").Attribute("Value").Value, out boolValue) == true)
                                {
                                    this.CurrentSetting.AutoSendS1F13 = boolValue;
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region [Trace Data]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("TraceDatas") != null)
                    {
                        this.TraceCollection.Items.Clear();

                        foreach (XElement tempTraceData in root.Element("TraceDatas").Elements("TraceData"))
                        {
                            if (tempTraceData.Attribute("ID") != null)
                            {
                                traceInfo = new ExpandedTraceInfo()
                                {
                                    TraceID = tempTraceData.Attribute("ID").Value
                                };

                                if (tempTraceData.Attribute("Auto") != null && bool.TryParse(tempTraceData.Attribute("Auto").Value, out boolValue) == true)
                                {
                                    traceInfo.AutoSend = boolValue;
                                }

                                if (tempTraceData.Attribute("TriggerCEID") != null)
                                {
                                    collectionEventInfo = this.CollectionEventCollection.Items.Values.FirstOrDefault(t => t.CEID == tempTraceData.Attribute("TriggerCEID").Value) as ExpandedCollectionEventInfo;

                                    if (collectionEventInfo != null)
                                    {
                                        traceInfo.SendTriggerCollection = new AutoSendTriggerCollection(collectionEventInfo);
                                    }
                                }

                                if (tempTraceData.Element("SendTriggers") != null)
                                {
                                    traceInfo.SendTriggerCollection = MakeTriggerCollectionFromXElement("SendTriggers", tempTraceData);
                                }

                                if (tempTraceData.Attribute("AutoStop") != null && bool.TryParse(tempTraceData.Attribute("AutoStop").Value, out boolValue) == true)
                                {
                                    traceInfo.AutoStop = boolValue;
                                }

                                if (tempTraceData.Attribute("StopTriggerCEID") != null)
                                {
                                    collectionEventInfo = this.CollectionEventCollection.Items.Values.FirstOrDefault(t => t.CEID == tempTraceData.Attribute("StopTriggerCEID").Value) as ExpandedCollectionEventInfo;

                                    if (collectionEventInfo != null)
                                    {
                                        traceInfo.StopTriggerCollection = new AutoSendTriggerCollection(collectionEventInfo);
                                    }
                                }
                                if (tempTraceData.Element("StopTriggers") != null)
                                {
                                    traceInfo.StopTriggerCollection = MakeTriggerCollectionFromXElement("StopTriggers", tempTraceData);
                                }

                                if (tempTraceData.Attribute("Period") != null)
                                {
                                    traceInfo.Dsper = tempTraceData.Attribute("Period").Value;
                                }

                                if (tempTraceData.Attribute("TotalNumber") != null && long.TryParse(tempTraceData.Attribute("TotalNumber").Value, out longValue) == true)
                                {
                                    traceInfo.TotalSample = longValue;
                                }

                                if (tempTraceData.Attribute("GroupSize") != null && long.TryParse(tempTraceData.Attribute("GroupSize").Value, out longValue) == true)
                                {
                                    traceInfo.ReportGroupSize = longValue;
                                }

                                readAttribute = string.Empty;

                                if (tempTraceData.Attribute("IDList") != null)
                                {
                                    readAttribute = tempTraceData.Attribute("IDList").Value;
                                }

                                foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                                {
                                    if (long.TryParse(tempItem, out long id) == true)
                                    {
                                        if (this.VariableCollection.Items.FirstOrDefault(t => t.VID == tempItem) != null)
                                        {
                                            traceInfo.Variables.Add(tempItem);
                                        }
                                    }
                                }

                                this.TraceCollection.Add(traceInfo);
                            }
                        }
                    }
                    #endregion
                    #region [Limit Monitoring]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("LimitMonitorings") != null)
                    {
                        this.LimitMonitoringCollection.Items.Clear();

                        foreach (XElement tempLimitMonitoring in root.Element("LimitMonitorings").Elements("LimitMonitoring"))
                        {
                            expandedVariableInfo = null;

                            if (tempLimitMonitoring.Attribute("ID") != null)
                            {
                                if (this.VariableCollection.Variables.Items.FirstOrDefault(t => t.VID == tempLimitMonitoring.Attribute("ID").Value) != null)
                                {
                                    expandedVariableInfo = this.VariableCollection.Variables[tempLimitMonitoring.Attribute("ID").Value] as ExpandedVariableInfo;

                                    expandedLimitMonitoringInfo = new ExpandedLimitMonitoringInfo()
                                    {
                                        Variable = expandedVariableInfo
                                    };

                                    if (tempLimitMonitoring.Attribute("Auto") != null && bool.TryParse(tempLimitMonitoring.Attribute("Auto").Value, out boolValue) == true)
                                    {
                                        expandedLimitMonitoringInfo.AutoSend = boolValue;
                                    }

                                    if (tempLimitMonitoring.Attribute("TriggerCEID") != null)
                                    {
                                        collectionEventInfo = this.CollectionEventCollection[tempLimitMonitoring.Attribute("TriggerCEID").Value] as ExpandedCollectionEventInfo;

                                        if (collectionEventInfo != null)
                                        {
                                            expandedLimitMonitoringInfo.TriggerCollection = new AutoSendTriggerCollection(collectionEventInfo);
                                        }
                                    }

                                    if (tempLimitMonitoring.Element("Triggers") != null)
                                    {
                                        expandedLimitMonitoringInfo.TriggerCollection = MakeTriggerCollectionFromXElement("Triggers", tempLimitMonitoring);
                                    }

                                    foreach (XElement tempVariable in tempLimitMonitoring.Element("Limits").Elements("Limit"))
                                    {
                                        if (tempVariable.Attribute("ID") != null && tempVariable.Attribute("Upper") != null && tempVariable.Attribute("Lower") != null)
                                        {
                                            if (byte.TryParse(tempVariable.Attribute("ID").Value, out byteValue) == true)
                                            {
                                                expandedLimitMonitoringInfo.Add(new ExpandedLimitMonitoringItem
                                                {
                                                    LimitID = byteValue,
                                                    Upper = tempVariable.Attribute("Upper").Value,
                                                    Lower = tempVariable.Attribute("Lower").Value
                                                });
                                            }
                                        }
                                    }

                                    this.LimitMonitoringCollection.Add(expandedLimitMonitoringInfo);
                                }
                            }
                        }
                    }
                    #endregion
                    #region [User GEM Message]
                    if (string.IsNullOrEmpty(errorText) == true && root.Element("SECSMessages") != null)
                    {
                        this.UserMessageData.Clear();

                        foreach (XElement tempMessage in root.Element("SECSMessages").Elements("SECSMessage"))
                        {
                            if (tempMessage.Attribute("Name") != null)
                            {
                                readAttribute = tempMessage.Attribute("Name").Value;

                                userMessage = new UserMessage()
                                {
                                    Name = tempMessage.Attribute("Name").Value
                                };

                                if (tempMessage.Attribute("Direction") != null)
                                {
                                    if (Enum.TryParse(tempMessage.Attribute("Direction").Value, out SECSMessageDirection direction) == true)
                                    {
                                        userMessage.Direction = direction;
                                    }
                                }

                                if (tempMessage.Attribute("Stream") != null)
                                {
                                    if (int.TryParse(tempMessage.Attribute("Stream").Value, out intValue) == true)
                                    {
                                        userMessage.Stream = intValue;
                                    }
                                }

                                if (tempMessage.Attribute("Function") != null)
                                {
                                    if (int.TryParse(tempMessage.Attribute("Function").Value, out intValue) == true)
                                    {
                                        userMessage.Function = intValue;
                                    }
                                }

                                if (tempMessage.Attribute("WaitBit") != null)
                                {
                                    if (bool.TryParse(tempMessage.Attribute("WaitBit").Value, out boolValue) == true)
                                    {
                                        userMessage.WaitBit = boolValue;
                                    }
                                }

                                if (tempMessage.Value != null)
                                {
                                    intValue = tempMessage.Value.Count();

                                    if (intValue > 2)
                                    {
                                        userMessage.Data = tempMessage.Value.Substring(1, tempMessage.Value.Count() - 2);
                                    }
                                    else
                                    {
                                        userMessage.Data = tempMessage.Value;
                                    }
                                }

                                this.UserMessageData[userMessage.Name] = userMessage;
                            }
                        }

                        foreach (var message in this.UserMessage.MessageInfo)
                        {
                            if (this.UserMessageData.ContainsKey(message.Key) == false)
                            {
                                userMessage = new UserMessage()
                                {
                                    Name = message.Key,
                                    Stream = message.Value.Stream,
                                    Function = message.Value.Function,
                                    Direction = message.Value.Direction,
                                    WaitBit = message.Value.WaitBit,
                                    Data = MakeSECS2Log(message.Value.Body)
                                };

                                this.UserMessageData[userMessage.Name] = userMessage;
                            }
                        }
                    }
                    #endregion

                    #region [Initialize HSMS Driver]
                    this.Configuration.UMDFileName = "UbiGEM.Net.Simulator.Host.umd";

                    DriverError error = this._driver.Initialize(this.Configuration, out errorText);

                    if (error != DriverError.Ok)
                    {
                        result = GemDriverError.InvalidConfiguration;
                        errorText = string.Format("SECS Driver Initialize Error: {0}", errorText);
                    }
                    else
                    {
                        XElement standardMessage = XElement.Load(this.Configuration.UMDFileName);

                        this._driver.MessageManager.Load(standardMessage);

                        foreach (var customMessage in this.UserMessage.MessageInfo)
                        {
                            this._driver.AddUserDefinedMessage(customMessage.Value);
                        }

                        this._hsmsDriverInitialized = true;

                        this.CurrentSetting.UpdateAckCollection(this._driver.MessageManager.Messages);
                        this.CurrentSetting.UpdateReplyCollection(this._driver.MessageManager.Messages);
                    }
                    #endregion

                    #region [RegistRecipe From RecipeDirectory]
                    recipeDirectory = new DirectoryInfo(this.CurrentSetting.FormattedRecipeDirectory);

                    if (recipeDirectory != null && recipeDirectory.Exists == true)
                    {
                        foreach (var fileInfo in recipeDirectory.GetFiles())
                        {
                            ppid = fileInfo.Name;

                            if (ppid.LastIndexOf(".rcp") >= 0)
                            {
                                ppid = ppid.Substring(0, ppid.LastIndexOf(".rcp"));

                                if(this.FormattedProcessProgramCollection.Items.Exists(t => t.PPID == ppid) == false)
                                {
                                    this.FormattedProcessProgramCollection.Add(new FormattedProcessProgramInfo
                                    {
                                        PPID = ppid
                                    });
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    result = GemDriverError.Unknown;
                    errorText = string.Format("XML Parsing Error: {0}", ex.Message);
                }
                finally
                {
                    root = null;
                    element = null;
                    subElement = null;
                }
            }

            if (result == GemDriverError.Ok && string.IsNullOrEmpty(errorText) == false)
            {
                result = GemDriverError.Mismatch;
            }

            return result;
        }
        #endregion
        #region SendSECSMessage
        public MessageError SendSECSMessage(SECSMessage message)
        {
            return this._driver.SendSECSMessage(message);
        }
        #endregion
        #region GetChildVariableInfo
        private void GetChildVariableInfo(VariableCollection variableCollection, ExpandedVariableInfo variableInfo, XElement element, Stack<string> parentNameStack, out string errorText)
        {
            VariableInfo childVariableInfo;
            ExpandedVariableInfo expandedChildVariableInfo;

            errorText = string.Empty;

            foreach (XElement tempChildVariable in element.Elements("Child"))
            {
                expandedChildVariableInfo = null;

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (tempChildVariable.Attribute("ID") != null && tempChildVariable.Attribute("Name") != null)
                    {
                        if (tempChildVariable.Attribute("ID").Value != string.Empty)
                        {
                            childVariableInfo = variableCollection[tempChildVariable.Attribute("ID").Value];
                        }
                        else
                        {
                            childVariableInfo = variableCollection.Items.FirstOrDefault(t => t.Name == tempChildVariable.Attribute("Name").Value);
                        }

                        if (childVariableInfo != null && parentNameStack.Contains(childVariableInfo.Name) == false)
                        {
                            expandedChildVariableInfo = childVariableInfo as ExpandedVariableInfo;
                            expandedChildVariableInfo.IsInheritance = true;

                            if (variableInfo.ChildVariables.FirstOrDefault(t => t.VID == expandedChildVariableInfo.VID) == null)
                            {
                                variableInfo.ChildVariables.Add(expandedChildVariableInfo);
                            }
                        }
                        else
                        {
                            errorText = string.Format("Variable name recursively duplicate: {0}", childVariableInfo.Name);
                        }
                    }
                    else
                    {
                        if (tempChildVariable.Attribute("Format") != null &&
                            tempChildVariable.Attribute("Format").Value == SECSItemFormat.L.ToString() &&
                            tempChildVariable.Element("Childs") != null &&
                            tempChildVariable.Element("Childs").HasElements == true)
                        {
                            expandedChildVariableInfo = new ExpandedVariableInfo()
                            {
                                Format = SECSItemFormat.L,
                                Name = tempChildVariable.Attribute("Name") != null ? tempChildVariable.Attribute("Name").Value : string.Empty,
                                Description = tempChildVariable.Attribute("Description") != null ? tempChildVariable.Attribute("Description").Value : string.Empty
                            };

                            if (tempChildVariable.Attribute("ID") != null)
                            {
                                parentNameStack.Push(expandedChildVariableInfo.Name);
                                GetChildVariableInfo(variableCollection, expandedChildVariableInfo, tempChildVariable.Element("Childs"), parentNameStack, out errorText);
                                parentNameStack.Pop();
                            }

                            if (variableInfo.ChildVariables.FirstOrDefault(t => t.VID == expandedChildVariableInfo.VID) == null)
                            {
                                variableInfo.ChildVariables.Add(expandedChildVariableInfo);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region LoadUGCFile
        public GemDriverError LoadUGCFile(string ugcFileName, out string errorText)
        {
            GemDriverError result;
            XElement root;
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo expandedReportInfo;
            VariableInfo tempVariableInfo;
            ExpandedVariableInfo expandedVariableInfo;
            List<ExpandedRemoteCommandParameterInfo> expandedCommandParameterItems;
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            List<ExpandedEnhancedRemoteCommandParameterInfo> expandedEnhancedCommandParameterItems;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            XElement enhancedParameterListTypeElement;
            string readAttribute;
            string id;
            List<SECSItemFormat> allowableFormats;
            SECSItemFormat itemFormat;
            Stack<string> parentNameStack;

            errorText = string.Empty;
            parentNameStack = new Stack<string>();

            try
            {
                result = GemDriverError.Ok;
                root = XElement.Load(ugcFileName);

                #region [Data Dictionary]
                this.DataDictionaryCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true && root.Element("DataDictionary") != null)
                {
                    foreach (XElement tempDataItem in root.Element("DataDictionary").Elements("DataItem"))
                    {
                        allowableFormats = new List<SECSItemFormat>();

                        foreach (string tempFormat in tempDataItem.Attribute("Format").Value.Split(','))
                        {
                            if (string.IsNullOrEmpty(tempFormat) == false)
                            {
                                allowableFormats.Add((SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempFormat));
                            }
                        }

                        this.DataDictionaryCollection.Add(new DataDictionaryInfo()
                        {
                            Name = string.IsNullOrEmpty(tempDataItem.Attribute("Name").Value) == false ? tempDataItem.Attribute("Name").Value : string.Empty,
                            Format = allowableFormats ?? null,
                            Length = int.Parse(tempDataItem.Attribute("Length").Value),
                            Description = string.IsNullOrEmpty(tempDataItem.Attribute("Description").Value) == false ? tempDataItem.Attribute("Description").Value : string.Empty,
                            PreDefined = bool.Parse(tempDataItem.Attribute("PreDefined").Value)
                        });
                    }
                }
                #endregion

                this.VariableCollection.Items.Clear();
                #region [Equipment Constants]
                if (string.IsNullOrEmpty(errorText) == true && root.Element("Variables") != null)
                {
                    foreach (XElement tempEcid in root.Element("EquipmentConstants").Elements("ECID"))
                    {
                        expandedVariableInfo = new ExpandedVariableInfo
                        {
                            IsInheritance = true,
                            VID = string.IsNullOrEmpty(tempEcid.Attribute("ID").Value) == false ? tempEcid.Attribute("ID").Value : string.Empty,
                            Name = string.IsNullOrEmpty(tempEcid.Attribute("Name").Value) == false ? tempEcid.Attribute("Name").Value : string.Empty,
                            VIDType = VariableType.ECV,
                            Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempEcid.Attribute("Format").Value),
                            Length = int.Parse(tempEcid.Attribute("Length").Value),
                            Units = string.IsNullOrEmpty(tempEcid.Attribute("Unit").Value) == false ? tempEcid.Attribute("Unit").Value : string.Empty,
                            Min = string.IsNullOrEmpty(tempEcid.Attribute("Min").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Min").Value) : null,
                            Max = string.IsNullOrEmpty(tempEcid.Attribute("Max").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Max").Value) : null,
                            Value = string.IsNullOrEmpty(tempEcid.Attribute("Value").Value) == false ? tempEcid.Attribute("Value").Value : string.Empty,
                            Default = string.IsNullOrEmpty(tempEcid.Attribute("Default").Value) == false ? tempEcid.Attribute("Default").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempEcid.Attribute("Description").Value) == false ? tempEcid.Attribute("Description").Value : string.Empty,
                            IsUse = bool.Parse(tempEcid.Attribute("Use").Value),
                            PreDefined = bool.Parse(tempEcid.Attribute("PreDefined").Value)
                        };

                        if (expandedVariableInfo.VID == string.Empty)
                        {
                            expandedVariableInfo.VID = expandedVariableInfo.Name;
                        }

                        this.VariableCollection.Add(expandedVariableInfo);
                    }
                }
                #endregion
                #region [Variables]
                if (string.IsNullOrEmpty(errorText) == true && root.Element("Variables") != null)
                {
                    foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                    {
                        expandedVariableInfo = new ExpandedVariableInfo
                        {
                            IsInheritance = true,
                            VID = string.IsNullOrEmpty(tempVariable.Attribute("ID").Value) == false ? tempVariable.Attribute("ID").Value : string.Empty,
                            Name = string.IsNullOrEmpty(tempVariable.Attribute("Name").Value) == false ? tempVariable.Attribute("Name").Value : string.Empty,
                            VIDType = tempVariable.Attribute("Class").Value == "SV" ? VariableType.SV : VariableType.DVVAL,
                            Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempVariable.Attribute("Format").Value),
                            Length = int.Parse(tempVariable.Attribute("Length").Value),
                            Units = string.IsNullOrEmpty(tempVariable.Attribute("Unit").Value) == false ? tempVariable.Attribute("Unit").Value : string.Empty,
                            Min = string.IsNullOrEmpty(tempVariable.Attribute("Min").Value) == false ? (double?)double.Parse(tempVariable.Attribute("Min").Value) : null,
                            Max = string.IsNullOrEmpty(tempVariable.Attribute("Max").Value) == false ? (double?)double.Parse(tempVariable.Attribute("Max").Value) : null,
                            Default = string.IsNullOrEmpty(tempVariable.Attribute("Default").Value) == false ? tempVariable.Attribute("Default").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempVariable.Attribute("Description").Value) == false ? tempVariable.Attribute("Description").Value : string.Empty,
                            IsUse = bool.Parse(tempVariable.Attribute("Use").Value),
                            PreDefined = bool.Parse(tempVariable.Attribute("PreDefined").Value)
                        };

                        if (expandedVariableInfo.VID == string.Empty)
                        {
                            expandedVariableInfo.VID = expandedVariableInfo.Name;
                        }

                        this.VariableCollection.Add(expandedVariableInfo);
                    }
                }
                #endregion
                #region [Equipment Constants-Childs]
                expandedVariableInfo = null;

                if (string.IsNullOrEmpty(errorText) == true && root.Element("EquipmentConstants")!= null)
                {
                    foreach (XElement tempVariable in root.Element("EquipmentConstants").Elements("ECID"))
                    {
                        if (tempVariable.Attribute("ID") != null && tempVariable.Attribute("Name") != null)
                        {
                            id = tempVariable.Attribute("ID").Value;

                            if (string.IsNullOrEmpty(id) == false)
                            {
                                expandedVariableInfo = this.VariableCollection.ECV[id] as ExpandedVariableInfo;
                            }
                            else
                            {
                                tempVariableInfo = this.VariableCollection.ECV.Items.FirstOrDefault(t => t.Name == tempVariable.Attribute("Name").Value);

                                if (tempVariableInfo != null)
                                {
                                    expandedVariableInfo = tempVariableInfo as ExpandedVariableInfo;
                                }
                            }

                            if (expandedVariableInfo != null && tempVariable.Element("Childs") != null)
                            {
                                parentNameStack.Clear();
                                parentNameStack.Push(expandedVariableInfo.Name);
                                GetChildVariableInfo(this.VariableCollection, expandedVariableInfo, tempVariable.Element("Childs"), parentNameStack, out errorText);
                                parentNameStack.Pop();
                            }
                        }
                    }
                }
                #endregion
                #region [Variables-Childs]
                expandedVariableInfo = null;

                if (string.IsNullOrEmpty(errorText) == true && root.Element("Variables") != null)
                {
                    foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                    {
                        if (string.IsNullOrEmpty(errorText) == true && tempVariable.Attribute("ID") != null && tempVariable.Attribute("Name") != null)
                        {
                            id = tempVariable.Attribute("ID").Value;

                            if (string.IsNullOrEmpty(id) == true)
                            {
                                tempVariableInfo = this.VariableCollection.Variables.Items.FirstOrDefault(t => t.Name == tempVariable.Attribute("Name").Value);

                                if (tempVariableInfo != null)
                                {
                                    expandedVariableInfo = tempVariableInfo as ExpandedVariableInfo;
                                }
                            }
                            else
                            {
                                expandedVariableInfo = this.VariableCollection[id] as ExpandedVariableInfo;
                            }

                            if (expandedVariableInfo != null && tempVariable.Element("Childs") != null)
                            {
                                parentNameStack.Clear();
                                parentNameStack.Push(expandedVariableInfo.Name);
                                GetChildVariableInfo(this.VariableCollection, expandedVariableInfo, tempVariable.Element("Childs"), parentNameStack, out errorText);
                                parentNameStack.Pop();
                            }
                        }
                    }
                }
                #endregion
                #region [Reports]
                this.ReportCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (XElement tempReport in root.Element("Reports").Elements("Report"))
                    {
                        expandedReportInfo = new ExpandedReportInfo()
                        {
                            IsInheritance = true,
                            ReportID = string.IsNullOrEmpty(tempReport.Attribute("ID").Value) == false ? tempReport.Attribute("ID").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempReport.Attribute("Description").Value) == false ? tempReport.Attribute("Description").Value : string.Empty
                        };

                        readAttribute = tempReport.Attribute("IDList").Value;

                        foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                        {
                            expandedVariableInfo = this.VariableCollection[tempItem] as ExpandedVariableInfo;

                            if (expandedVariableInfo != null)
                            {
                                expandedReportInfo.Variables.Add(expandedVariableInfo);
                            }
                            else
                            {
                                result = GemDriverError.Unknown;
                                errorText = string.Format("Variable not exist in Report # {0}", tempItem);
                                break;
                            }
                        }

                        if (result == GemDriverError.Ok)
                        {
                            this.ReportCollection.Add(expandedReportInfo);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                #endregion
                #region [Collection Events]
                this.CollectionEventCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (XElement tempCollectionEvent in root.Element("CollectionEvents").Elements("CEID"))
                    {
                        expandedCollectionEventInfo = new ExpandedCollectionEventInfo()
                        {
                            IsInheritance = true,
                            CEID = string.IsNullOrEmpty(tempCollectionEvent.Attribute("ID").Value) == false ? tempCollectionEvent.Attribute("ID").Value : string.Empty,
                            Name = string.IsNullOrEmpty(tempCollectionEvent.Attribute("Name").Value) == false ? tempCollectionEvent.Attribute("Name").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempCollectionEvent.Attribute("Description").Value) == false ? tempCollectionEvent.Attribute("Description").Value : string.Empty,
                            Enabled = bool.Parse(tempCollectionEvent.Attribute("Enabled").Value),
                            IsBase = bool.Parse(tempCollectionEvent.Attribute("IsBase").Value),
                            IsUse = bool.Parse(tempCollectionEvent.Attribute("Use").Value),
                            PreDefined = bool.Parse(tempCollectionEvent.Attribute("PreDefined").Value)
                        };

                        readAttribute = string.IsNullOrEmpty(tempCollectionEvent.Attribute("IDList").Value) == false ? tempCollectionEvent.Attribute("IDList").Value : string.Empty;

                        foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                        {
                            expandedReportInfo = this.ReportCollection[tempItem] as ExpandedReportInfo;

                            if (expandedReportInfo != null)
                            {
                                expandedCollectionEventInfo.Reports.Add(expandedReportInfo);
                            }
                            else
                            {
                                result = GemDriverError.Unknown;
                                errorText = string.Format("Report not exist in Collection Event # {0}", tempItem);
                                break;
                            }
                        }

                        if (result == GemDriverError.Ok)
                        {
                            this.CollectionEventCollection.Add(expandedCollectionEventInfo);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                #endregion
                #region [Alarms]
                this.AlarmCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (XElement tempAlarm in root.Element("Alarms").Elements("Alarm"))
                    {
                        this.AlarmCollection.Add(new ExpandedAlarmInfo
                        {
                            IsInheritance = true,
                            ID = long.Parse(tempAlarm.Attribute("ID").Value),
                            Code = int.Parse(tempAlarm.Attribute("Code").Value),
                            Enabled = bool.Parse(tempAlarm.Attribute("Enabled").Value),
                            Description = string.IsNullOrEmpty(tempAlarm.Attribute("Description").Value) == false ? tempAlarm.Attribute("Description").Value : string.Empty
                        });
                    }
                }
                #endregion
                #region [Remote Commands]
                this.RemoteCommandCollection.RemoteCommandItems.Clear();
                this.RemoteCommandCollection.EnhancedRemoteCommandItems.Clear();

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("RemoteCommand"))
                    {
                        expandedCommandParameterItems = new List<ExpandedRemoteCommandParameterInfo>();

                        foreach (XElement tempParameter in tempRemoteCommand.Element("Parameters").Elements("Parameter"))
                        {
                            expandedCommandParameterItems.Add(new ExpandedRemoteCommandParameterInfo()
                            {
                                Name = string.IsNullOrEmpty(tempParameter.Attribute("Name").Value) == false ? tempParameter.Attribute("Name").Value : string.Empty,
                                Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value)
                            });
                        }

                        expandedRemoteCommandInfo = new ExpandedRemoteCommandInfo
                        {
                            IsInheritance = true,
                            RemoteCommand = string.IsNullOrEmpty(tempRemoteCommand.Attribute("Command").Value) == false ? tempRemoteCommand.Attribute("Command").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempRemoteCommand.Attribute("Description").Value) == false ? tempRemoteCommand.Attribute("Description").Value : string.Empty
                        };

                        expandedRemoteCommandInfo.ValueSetCollection.Add(new ExpandedRemoteCommandValueSetInfo()
                        {
                            Name = "Default"
                        });
                        expandedRemoteCommandInfo.ValueSetCollection["Default"].AddParameterItems(expandedCommandParameterItems);

                        FillDefault(expandedRemoteCommandInfo);
                        this.RemoteCommandCollection.Add(expandedRemoteCommandInfo);
                    }

                    foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("EnhancedRemoteCommand"))
                    {
                        expandedEnhancedCommandParameterItems = new List<ExpandedEnhancedRemoteCommandParameterInfo>();

                        expandedEnhancedRemoteCommandInfo = new ExpandedEnhancedRemoteCommandInfo()
                        {
                            IsInheritance = true,
                            RemoteCommand = tempRemoteCommand.Attribute("Command") != null ? tempRemoteCommand.Attribute("Command").Value : string.Empty,
                            Description = tempRemoteCommand.Attribute("Description") != null ? tempRemoteCommand.Attribute("Description").Value : string.Empty
                        };

                        foreach (XElement tempParameter in tempRemoteCommand.Element("Parameters").Elements("Parameter"))
                        {
                            itemFormat = tempParameter.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A;

                            if (itemFormat == SECSItemFormat.L)
                            {
                                expandedEnhancedCommandParameterInfo = new ExpandedEnhancedRemoteCommandParameterInfo()
                                {
                                    Name = tempParameter.Attribute("Name") != null ? tempParameter.Attribute("Name").Value : string.Empty,
                                    Format = tempParameter.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A,
                                    Value = string.Empty
                                };

                                if (tempParameter.Element("Value") != null)
                                {
                                    enhancedParameterListTypeElement = tempParameter.Element("Value");

                                    expandedEnhancedCommandParameterInfo.ValueItems.Add(new ExpandedEnhancedRemoteCommandParameterItem()
                                    {
                                        Format = enhancedParameterListTypeElement.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), enhancedParameterListTypeElement.Attribute("Format").Value) : SECSItemFormat.A,
                                        Value = string.Empty
                                    });
                                }
                                else if (tempParameter.Element("Values") != null)
                                {
                                    foreach (XElement tempParameterValue in tempParameter.Element("Values").Elements("Value"))
                                    {
                                        var valueItem = new ExpandedEnhancedRemoteCommandParameterItem()
                                        {
                                            Name = tempParameterValue.Attribute("Name") != null ? tempParameterValue.Attribute("Name").Value : string.Empty,
                                            Format = tempParameterValue.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameterValue.Attribute("Format").Value) : SECSItemFormat.A,
                                            Value = string.Empty
                                        };
                                        valueItem.ChildParameterItem.AddRange(MakeExpandedRemoteCommandValueListFromXElement(tempParameterValue));
                                        expandedEnhancedCommandParameterInfo.ValueItems.Add(valueItem);
                                    }
                                }

                                expandedEnhancedCommandParameterItems.Add(expandedEnhancedCommandParameterInfo);
                            }
                            else
                            {
                                expandedEnhancedCommandParameterInfo = new ExpandedEnhancedRemoteCommandParameterInfo()
                                {
                                    Name = tempParameter.Attribute("Name") != null ? tempParameter.Attribute("Name").Value : string.Empty,
                                    Format = tempParameter.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A,
                                    Value = string.Empty
                                };
                                expandedEnhancedCommandParameterItems.Add(expandedEnhancedCommandParameterInfo);
                            }

                        }

                        expandedEnhancedRemoteCommandInfo.ValueSetCollection.Add(new ExpandedEnhancedRemoteCommandValueSetInfo()
                        {
                            Name = "Default"
                        });

                        expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"].AddParameterItems(expandedEnhancedCommandParameterItems);
                        FillDefault(expandedEnhancedRemoteCommandInfo);
                        this.RemoteCommandCollection.Add(expandedEnhancedRemoteCommandInfo);
                    }
                }
                #endregion
                #region [Message]
                if (string.IsNullOrEmpty(errorText) == true && root.Element(TAG_MESSAGE) != null)
                {
                    if (root.Element(TAG_MESSAGE) != null && root.Element(TAG_MESSAGE).Element(TAG_USER_CUSTOM_MESSAGE) != null)
                    {
                        using (SECSMessageManager secsMessageManager = new SECSMessageManager())
                        {
                            secsMessageManager.Load(root.Element(TAG_MESSAGE).Element(TAG_USER_CUSTOM_MESSAGE));

                            if (secsMessageManager != null && secsMessageManager.Messages != null && secsMessageManager.Messages.MessageInfo.Count() > 0)
                            {
                                this.UserMessage = secsMessageManager.Messages;
                            }
                        }

                        foreach (var message in this.UserMessage.MessageInfo)
                        {
                            if (this.UserMessageData.ContainsKey(message.Key) == false)
                            {
                                UserMessage userMessage = new UserMessage()
                                {
                                    Name = message.Key,
                                    Stream = message.Value.Stream,
                                    Function = message.Value.Function,
                                    Direction = message.Value.Direction,
                                    WaitBit = message.Value.WaitBit,
                                    Data = MakeSECS2Log(message.Value.Body)
                                };

                                this.UserMessageData[userMessage.Name] = userMessage;
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }
            finally
            {
                root = null;
                expandedCollectionEventInfo = null;
                expandedReportInfo = null;
                expandedVariableInfo = null;
            }

            if (result == GemDriverError.Ok && string.IsNullOrEmpty(errorText) == false)
            {
                result = GemDriverError.Mismatch;
            }

            return result;
        }
        #endregion
        #region TestUGCFile
        public GemDriverError TestUGCFile(string ugcFileName, out string errorText)
        {
            GemDriverError result;
            XElement root;
            XElement element;
            Configurtion ugcConfiguration;
            int intValue;
            LogMode logMode;
            DataDictionaryCollection dataDictionaryCollection;
            VariableCollection variableCollection;
            ReportCollection reportCollection;
            CollectionEventCollection collectionEventCollection;
            AlarmCollection alarmCollection;
            RemoteCommandCollection remoteCommandCollection;
            SECSMessageCollection userGEMMessage;
            SECSMessageCollection userMessage;
            CollectionEventInfo collectionEventInfo;
            ReportInfo reportInfo;
            ExpandedVariableInfo expandedVariableInfo;

            List<ExpandedRemoteCommandParameterInfo> expandedCommandParameterItems;
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            List<ExpandedEnhancedRemoteCommandParameterInfo> expandedEnhancedCommandParameterItems;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            XElement enhancedParameterListTypeElement;

            Stack<string> parentNameStack;

            string readAttribute;
            string id;
            List<SECSItemFormat> formats;
            SECSItemFormat itemFormat;
            parentNameStack = new Stack<string>();

            ugcConfiguration = new Configurtion()
            {
                DriverName = string.Empty,
                DeviceType = DeviceType.Equipment,
                DeviceID = 0,
                SECSMode = SECSMode.HSMS,
                MaxMessageSize = 2097152,
                LogEnabledSECS1 = LogMode.Hour,
                LogEnabledSECS2 = LogMode.Hour,
                LogEnabledSystem = LogMode.None,
                LogExpirationDay = 30,
                LogPath = @"C:\Logs",
                HSMSModeConfig = new Configurtion.HSMS()
                {
                    HSMSMode = HSMSMode.Passive,
                    RemoteIPAddress = string.Empty,
                    RemotePortNo = 0,
                    LocalIPAddress = string.Empty,
                    LocalPortNo = 0,
                    T3 = 45,
                    T5 = 10,
                    T6 = 5,
                    T7 = 10,
                    T8 = 5,
                    LinkTest = 120,
                }
            };
            dataDictionaryCollection = new DataDictionaryCollection();
            variableCollection = new VariableCollection();
            reportCollection = new ReportCollection();
            collectionEventCollection = new CollectionEventCollection();
            alarmCollection = new AlarmCollection();

            remoteCommandCollection = new RemoteCommandCollection();
            userGEMMessage = new SECSMessageCollection();
            userMessage = new SECSMessageCollection();

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;
                root = XElement.Load(ugcFileName);

                #region [SECSDriver]
                
                element = root.Element("SECSDriver");

                if (element != null)
                {
                    if (element.Element("Name") != null)
                    {
                        ugcConfiguration.DriverName = string.Format("{0}_HOST", element.Element("Name").Value);
                    }

                    if (element.Element("Type") != null)
                    {
                        if (Enum.TryParse(element.Element("Type").Value, out DeviceType deviceType) == true)
                        {
                            ugcConfiguration.DeviceType = deviceType;
                        }
                    }

                    if (element.Element("IsAsyncMode") != null)
                    {
                        if (bool.TryParse(element.Element("IsAsyncMode").Value, out bool boolValue) == true)
                        {
                            ugcConfiguration.IsAsyncMode = boolValue;
                        }
                    }

                    if (element.Element("DeviceID") != null)
                    {
                        if (int.TryParse(element.Element("DeviceID").Value, out intValue) == true)
                        {
                            ugcConfiguration.DeviceID = intValue;
                        }
                    }

                    if (element.Element("Mode") != null)
                    {
                        if (Enum.TryParse(element.Element("Mode").Value, out SECSMode secsMode) == true)
                        {
                            ugcConfiguration.SECSMode = secsMode;
                        }
                    }

                    if (element.Element("MaxMessageSize") != null)
                    {
                        if (double.TryParse(element.Element("MaxMessageSize").Value, out double doubleValue) == true)
                        {
                            ugcConfiguration.MaxMessageSize = doubleValue;
                        }
                    }

                    if (element.Element("LogEnabledSECS1") != null)
                    {
                        if (Enum.TryParse(element.Element("LogEnabledSECS1").Value, out logMode) == true)
                        {
                            ugcConfiguration.LogEnabledSECS1 = logMode;
                        }
                    }

                    if (element.Element("LogEnabledSECS2") != null)
                    {
                        if (Enum.TryParse(element.Element("LogEnabledSECS2").Value, out logMode) == true)
                        {
                            ugcConfiguration.LogEnabledSECS2 = logMode;
                        }
                    }

                    if (element.Element("LogEnabledSystem") != null)
                    {
                        if (Enum.TryParse(element.Element("LogEnabledSystem").Value, out logMode) == true)
                        {
                            ugcConfiguration.LogEnabledSystem = logMode;
                        }
                    }

                    if (element.Element("LogExpirationDay") != null)
                    {
                        if (int.TryParse(element.Element("LogExpirationDay").Value, out intValue) == true)
                        {
                            ugcConfiguration.LogExpirationDay = intValue;
                        }
                    }

                    if (element.Element("LogPath") != null)
                    {
                        ugcConfiguration.LogPath = element.Element("LogPath").Value;
                    }

                    #region [SECSDriver - HSMS]
                    if (element.Element("HSMS") != null)
                    {
                        if (element.Element("HSMS").Element("HSMSMode") != null)
                        {
                            if (Enum.TryParse(element.Element("HSMS").Element("HSMSMode").Value, out HSMSMode hsmsMode) == true)
                            {
                                ugcConfiguration.HSMSModeConfig.HSMSMode = hsmsMode;
                            }
                        }

                        if (element.Element("HSMS").Element("RemoteIPAddress") != null)
                        {
                            ugcConfiguration.HSMSModeConfig.RemoteIPAddress = element.Element("HSMS").Element("RemoteIPAddress").Value;
                        }

                        if (element.Element("HSMS").Element("RemotePortNo") != null)
                        {
                            if (int.TryParse(element.Element("HSMS").Element("RemotePortNo").Value, out intValue) == true)
                            {
                                ugcConfiguration.HSMSModeConfig.RemotePortNo = intValue;
                            }
                        }

                        if (element.Element("HSMS").Element("LocalIPAddress") != null)
                        {
                            ugcConfiguration.HSMSModeConfig.LocalIPAddress = element.Element("HSMS").Element("LocalIPAddress").Value;
                        }

                        if (element.Element("HSMS").Element("LocalPortNo") != null)
                        {
                            if (int.TryParse(element.Element("HSMS").Element("LocalPortNo").Value, out intValue) == true)
                            {
                                ugcConfiguration.HSMSModeConfig.LocalPortNo = intValue;
                            }
                        }

                        #region [SECSDriver - HSMS - Timeout]
                        if (element.Element("HSMS").Element("Timeout") != null)
                        {
                            if (element.Element("HSMS").Element("Timeout").Element("T3") != null)
                            {
                                if (int.TryParse(element.Element("HSMS").Element("Timeout").Element("T3").Value, out intValue) == true)
                                {
                                    ugcConfiguration.HSMSModeConfig.T3 = intValue;
                                }
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T5") != null)
                            {
                                if (int.TryParse(element.Element("HSMS").Element("Timeout").Element("T5").Value, out intValue) == true)
                                {
                                    ugcConfiguration.HSMSModeConfig.T5 = intValue;
                                }
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T6") != null)
                            {
                                if (int.TryParse(element.Element("HSMS").Element("Timeout").Element("T6").Value, out intValue) == true)
                                {
                                    ugcConfiguration.HSMSModeConfig.T6 = intValue;
                                }
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T7") != null)
                            {
                                if (int.TryParse(element.Element("HSMS").Element("Timeout").Element("T7").Value, out intValue) == true)
                                {
                                    ugcConfiguration.HSMSModeConfig.T7 = intValue;
                                }
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T8") != null)
                            {
                                if (int.TryParse(element.Element("HSMS").Element("Timeout").Element("T8").Value, out intValue) == true)
                                {
                                    ugcConfiguration.HSMSModeConfig.T8 = intValue;
                                }
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("LinkTest") != null)
                            {
                                if (int.TryParse(element.Element("HSMS").Element("Timeout").Element("LinkTest").Value, out intValue) == true)
                                {
                                    ugcConfiguration.HSMSModeConfig.LinkTest = intValue;
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
                #region [Data Dictionary]
                dataDictionaryCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true && root.Element("DataDictionary") != null)
                {
                    foreach (XElement tempDataItem in root.Element("DataDictionary").Elements("DataItem"))
                    {
                        formats = new List<SECSItemFormat>();

                        foreach (string tempFormat in tempDataItem.Attribute("Format").Value.Split(','))
                        {
                            if (string.IsNullOrEmpty(tempFormat) == false)
                            {
                                formats.Add((SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempFormat));
                            }
                        }

                        dataDictionaryCollection.Add(new DataDictionaryInfo()
                        {
                            Name = string.IsNullOrEmpty(tempDataItem.Attribute("Name").Value) == false ? tempDataItem.Attribute("Name").Value : string.Empty,
                            Format = formats ?? null,
                            Length = int.Parse(tempDataItem.Attribute("Length").Value),
                            Description = string.IsNullOrEmpty(tempDataItem.Attribute("Description").Value) == false ? tempDataItem.Attribute("Description").Value : string.Empty,
                            PreDefined = bool.Parse(tempDataItem.Attribute("PreDefined").Value)
                        });
                    }
                }
                #endregion

                variableCollection.Items.Clear();

                #region [Equipment Constants]
                if (string.IsNullOrEmpty(errorText) == true && root.Element("EquipmentConstants") != null)
                {
                    foreach (XElement tempEcid in root.Element("EquipmentConstants").Elements("ECID"))
                    {
                        variableCollection.Add(new ExpandedVariableInfo
                        {
                            IsInheritance = true,
                            VID = string.IsNullOrEmpty(tempEcid.Attribute("ID").Value) == false ? tempEcid.Attribute("ID").Value : string.Empty,
                            Name = string.IsNullOrEmpty(tempEcid.Attribute("Name").Value) == false ? tempEcid.Attribute("Name").Value : string.Empty,
                            VIDType = VariableType.ECV,
                            Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempEcid.Attribute("Format").Value),
                            Length = int.Parse(tempEcid.Attribute("Length").Value),
                            Units = string.IsNullOrEmpty(tempEcid.Attribute("Unit").Value) == false ? tempEcid.Attribute("Unit").Value : string.Empty,
                            Min = string.IsNullOrEmpty(tempEcid.Attribute("Min").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Min").Value) : null,
                            Max = string.IsNullOrEmpty(tempEcid.Attribute("Max").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Max").Value) : null,
                            Value = string.IsNullOrEmpty(tempEcid.Attribute("Value").Value) == false ? tempEcid.Attribute("Value").Value : string.Empty,
                            Default = string.IsNullOrEmpty(tempEcid.Attribute("Default").Value) == false ? tempEcid.Attribute("Default").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempEcid.Attribute("Description").Value) == false ? tempEcid.Attribute("Description").Value : string.Empty,
                            IsUse = true,
                            PreDefined = bool.Parse(tempEcid.Attribute("PreDefined").Value)
                        });
                    }
                }
                #endregion
                #region [Variables]
                if (string.IsNullOrEmpty(errorText) == true && root.Element("Variables") != null)
                {
                    foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                    {
                        variableCollection.Add(new ExpandedVariableInfo
                        {
                            IsInheritance = true,
                            VID = string.IsNullOrEmpty(tempVariable.Attribute("ID").Value) == false ? tempVariable.Attribute("ID").Value : string.Empty,
                            Name = string.IsNullOrEmpty(tempVariable.Attribute("Name").Value) == false ? tempVariable.Attribute("Name").Value : string.Empty,
                            VIDType = tempVariable.Attribute("Class").Value == "SV" ? VariableType.SV : VariableType.DVVAL,
                            Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempVariable.Attribute("Format").Value),
                            Length = int.Parse(tempVariable.Attribute("Length").Value),
                            Units = string.IsNullOrEmpty(tempVariable.Attribute("Unit").Value) == false ? tempVariable.Attribute("Unit").Value : string.Empty,
                            Min = string.IsNullOrEmpty(tempVariable.Attribute("Min").Value) == false ? (double?)double.Parse(tempVariable.Attribute("Min").Value) : null,
                            Max = string.IsNullOrEmpty(tempVariable.Attribute("Max").Value) == false ? (double?)double.Parse(tempVariable.Attribute("Max").Value) : null,
                            Default = string.IsNullOrEmpty(tempVariable.Attribute("Default").Value) == false ? tempVariable.Attribute("Default").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempVariable.Attribute("Description").Value) == false ? tempVariable.Attribute("Description").Value : string.Empty,
                            IsUse = true,
                            PreDefined = bool.Parse(tempVariable.Attribute("PreDefined").Value)
                        });
                    }
                }
                #endregion
                #region [Equipment Constants-Childs]
                if (string.IsNullOrEmpty(errorText) == true && root.Element("EquipmentConstants") != null)
                {
                    foreach (XElement tempVariable in root.Element("EquipmentConstants").Elements("ECID"))
                    {
                        if (string.IsNullOrEmpty(errorText) == true && tempVariable.Attribute("ID") != null)
                        {
                            id = tempVariable.Attribute("ID").Value;

                            expandedVariableInfo = variableCollection.ECV[id] as ExpandedVariableInfo;

                            if (expandedVariableInfo != null && tempVariable.Element("Childs") != null)
                            {
                                parentNameStack.Clear();
                                parentNameStack.Push(expandedVariableInfo.Name);
                                GetChildVariableInfo(variableCollection, expandedVariableInfo, tempVariable.Element("Childs"), parentNameStack, out errorText);
                                parentNameStack.Pop();
                            }
                        }
                    }
                }
                #endregion
                #region [Variables-Childs]
                if (string.IsNullOrEmpty(errorText) == true && root.Element("Variables") != null)
                {
                    foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                    {
                        if (string.IsNullOrEmpty(errorText) == true && tempVariable.Attribute("ID") != null)
                        {
                            id = tempVariable.Attribute("ID").Value;

                            expandedVariableInfo = variableCollection[id] as ExpandedVariableInfo;

                            if (expandedVariableInfo != null && tempVariable.Element("Childs") != null)
                            {
                                parentNameStack.Clear();
                                parentNameStack.Push(expandedVariableInfo.Name);
                                GetChildVariableInfo(variableCollection, expandedVariableInfo, tempVariable.Element("Childs"), parentNameStack, out errorText);
                                parentNameStack.Pop();
                            }
                        }
                    }
                }
                #endregion
                #region [Reports]
                reportCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true && root.Element("Reports") != null)
                {
                    foreach (XElement tempReport in root.Element("Reports").Elements("Report"))
                    {
                        reportInfo = new ReportInfo()
                        {
                            ReportID = string.IsNullOrEmpty(tempReport.Attribute("ID").Value) == false ? tempReport.Attribute("ID").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempReport.Attribute("Description").Value) == false ? tempReport.Attribute("Description").Value : string.Empty
                        };

                        readAttribute = tempReport.Attribute("IDList").Value;

                        foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                        {
                            expandedVariableInfo = variableCollection[tempItem] as ExpandedVariableInfo;

                            if (expandedVariableInfo != null)
                            {
                                reportInfo.Variables.Add(expandedVariableInfo);
                            }
                            else
                            {
                                result = GemDriverError.Unknown;
                                errorText = string.Format("Variable not exist in Report # {0}", tempItem);
                                break;
                            }
                        }

                        if (result == GemDriverError.Ok)
                        {
                            reportCollection.Add(reportInfo);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                #endregion
                #region [Collection Events]
                collectionEventCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true && root.Element("CollectionEvents") != null)
                {
                    foreach (XElement tempCollectionEvent in root.Element("CollectionEvents").Elements("CEID"))
                    {
                        collectionEventInfo = new CollectionEventInfo()
                        {
                            CEID = string.IsNullOrEmpty(tempCollectionEvent.Attribute("ID").Value) == false ? tempCollectionEvent.Attribute("ID").Value : string.Empty,
                            Name = string.IsNullOrEmpty(tempCollectionEvent.Attribute("Name").Value) == false ? tempCollectionEvent.Attribute("Name").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempCollectionEvent.Attribute("Description").Value) == false ? tempCollectionEvent.Attribute("Description").Value : string.Empty,
                            Enabled = bool.Parse(tempCollectionEvent.Attribute("Enabled").Value),
                            IsBase = bool.Parse(tempCollectionEvent.Attribute("IsBase").Value),
                            IsUse = bool.Parse(tempCollectionEvent.Attribute("Use").Value),
                            PreDefined = bool.Parse(tempCollectionEvent.Attribute("PreDefined").Value)
                        };

                        readAttribute = string.IsNullOrEmpty(tempCollectionEvent.Attribute("IDList").Value) == false ? tempCollectionEvent.Attribute("IDList").Value : string.Empty;

                        foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                        {
                            reportInfo = reportCollection[tempItem];

                            if (reportInfo != null)
                            {
                                collectionEventInfo.Reports.Add(reportInfo);
                            }
                            else
                            {
                                result = GemDriverError.Unknown;
                                errorText = string.Format("Report not exist in Collection Event # {0}", tempItem);
                                break;
                            }
                        }

                        if (result == GemDriverError.Ok)
                        {
                            collectionEventCollection.Add(collectionEventInfo);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                #endregion
                #region [Alarms]
                alarmCollection.Items.Clear();

                if (string.IsNullOrEmpty(errorText) == true && root.Element("Alarms") != null)
                {
                    foreach (XElement tempAlarm in root.Element("Alarms").Elements("Alarm"))
                    {
                        alarmCollection.Add(new ExpandedAlarmInfo
                        {
                            IsInheritance = true,
                            ID = long.Parse(tempAlarm.Attribute("ID").Value),
                            Code = int.Parse(tempAlarm.Attribute("Code").Value),
                            Enabled = bool.Parse(tempAlarm.Attribute("Enabled").Value),
                            Description = string.IsNullOrEmpty(tempAlarm.Attribute("Description").Value) == false ? tempAlarm.Attribute("Description").Value : string.Empty
                        });
                    }
                }
                #endregion
                #region [Remote Commands]
                remoteCommandCollection.RemoteCommandItems.Clear();
                remoteCommandCollection.EnhancedRemoteCommandItems.Clear();

                if (string.IsNullOrEmpty(errorText) == true && root.Element("RemoteCommands") != null)
                {
                    foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("RemoteCommand"))
                    {
                        expandedCommandParameterItems = new List<ExpandedRemoteCommandParameterInfo>();

                        foreach (XElement tempParameter in tempRemoteCommand.Element("Parameters").Elements("Parameter"))
                        {
                            expandedCommandParameterItems.Add(new ExpandedRemoteCommandParameterInfo()
                            {
                                Name = string.IsNullOrEmpty(tempParameter.Attribute("Name").Value) == false ? tempParameter.Attribute("Name").Value : string.Empty,
                                Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value)
                            });
                        }

                        expandedRemoteCommandInfo = new ExpandedRemoteCommandInfo
                        {
                            IsInheritance = true,
                            RemoteCommand = string.IsNullOrEmpty(tempRemoteCommand.Attribute("Command").Value) == false ? tempRemoteCommand.Attribute("Command").Value : string.Empty,
                            Description = string.IsNullOrEmpty(tempRemoteCommand.Attribute("Description").Value) == false ? tempRemoteCommand.Attribute("Description").Value : string.Empty
                        };

                        expandedRemoteCommandInfo.ValueSetCollection["Default"].AddParameterItems(expandedCommandParameterItems);
                        remoteCommandCollection.Add(expandedRemoteCommandInfo);
                    }

                    foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("EnhancedRemoteCommand"))
                    {
                        expandedEnhancedCommandParameterItems = new List<ExpandedEnhancedRemoteCommandParameterInfo>();
                        expandedEnhancedRemoteCommandInfo = new ExpandedEnhancedRemoteCommandInfo()
                        {
                            IsInheritance = true,
                            RemoteCommand = tempRemoteCommand.Attribute("Command") != null ? tempRemoteCommand.Attribute("Command").Value : string.Empty,
                            Description = tempRemoteCommand.Attribute("Description") != null ? tempRemoteCommand.Attribute("Description").Value : string.Empty
                        };

                        foreach (XElement tempParameter in tempRemoteCommand.Element("Parameters").Elements("Parameter"))
                        {
                            itemFormat = tempParameter.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A;

                            if (itemFormat == SECSItemFormat.L)
                            {
                                expandedEnhancedCommandParameterInfo = new ExpandedEnhancedRemoteCommandParameterInfo()
                                {
                                    Name = tempParameter.Attribute("Name") != null ? tempParameter.Attribute("Name").Value : string.Empty,
                                    Format = tempParameter.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A,
                                    Value = string.Empty
                                };

                                if (tempParameter.Element("Value") != null)
                                {
                                    enhancedParameterListTypeElement = tempParameter.Element("Value");
                                    expandedEnhancedCommandParameterInfo.ValueItems.Add(new ExpandedEnhancedRemoteCommandParameterItem()
                                    {
                                        Format = enhancedParameterListTypeElement.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), enhancedParameterListTypeElement.Attribute("Format").Value) : SECSItemFormat.A,
                                        Value = string.Empty
                                    });
                                }
                                else if (tempParameter.Element("Values") != null)
                                {
                                    foreach (XElement tempParameterValue in tempParameter.Element("Values").Elements("Value"))
                                    {
                                        var valueItem = new ExpandedEnhancedRemoteCommandParameterItem()
                                        {
                                            Name = tempParameterValue.Attribute("Name") != null ? tempParameterValue.Attribute("Name").Value : string.Empty,
                                            Format = tempParameterValue.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameterValue.Attribute("Format").Value) : SECSItemFormat.A,
                                            Value = string.Empty
                                        };
                                        valueItem.ChildParameterItem.AddRange(MakeExpandedRemoteCommandValueListFromXElement(tempParameterValue));
                                        expandedEnhancedCommandParameterInfo.ValueItems.Add(valueItem);
                                    }
                                }

                                expandedEnhancedCommandParameterItems.Add(expandedEnhancedCommandParameterInfo);
                            }
                            else
                            {
                                expandedEnhancedCommandParameterInfo = new ExpandedEnhancedRemoteCommandParameterInfo()
                                {
                                    Name = tempParameter.Attribute("Name") != null ? tempParameter.Attribute("Name").Value : string.Empty,
                                    Format = tempParameter.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A,
                                    Value = string.Empty
                                };
                                expandedEnhancedCommandParameterItems.Add(expandedEnhancedCommandParameterInfo);
                            }
                        }

                        expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"].AddParameterItems(expandedEnhancedCommandParameterItems);
                        remoteCommandCollection.Add(expandedEnhancedRemoteCommandInfo);
                    }
                }
                #endregion
                #region [Message]
                if (string.IsNullOrEmpty(errorText) == true && root.Element(TAG_MESSAGE) != null)
                {
                    if (root.Element(TAG_MESSAGE) != null && root.Element(TAG_MESSAGE).Element(TAG_USER_MESSAGE) != null)
                    {
                        using (UmdLoader loader = new UmdLoader())
                        {
                            errorText = loader.Load(root.Element(TAG_MESSAGE).Element(TAG_USER_MESSAGE));

                            if (string.IsNullOrEmpty(errorText) == true)
                            {
                                userGEMMessage = loader.SECSMessage;
                            }
                        }
                    }

                    if (root.Element(TAG_MESSAGE) != null && root.Element(TAG_MESSAGE).Element(TAG_USER_CUSTOM_MESSAGE) != null)
                    {
                        using (UmdLoader loader = new UmdLoader())
                        {
                            errorText = loader.Load(root.Element(TAG_MESSAGE).Element(TAG_USER_CUSTOM_MESSAGE));

                            if (string.IsNullOrEmpty(errorText) == true)
                            {
                                userMessage = loader.SECSMessage;
                            }
                        }
                    }
                }
                #endregion

                this.UGCConfiguration = ugcConfiguration;
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }
            finally
            {
                root = null;
                collectionEventInfo = null;
                reportInfo = null;
                expandedVariableInfo = null;
            }

            if (result == GemDriverError.Ok && string.IsNullOrEmpty(errorText) == false)
            {
                result = GemDriverError.Mismatch;
            }
            return result;
        }
        #endregion
        #region SaveConfigFile
        public GemDriverError SaveConfigFile(out string errorText)
        {
            GemDriverError result;

            if (string.IsNullOrEmpty(this.ConfigFilepath) == false)
            {
                result = SaveConfigFile(this.ConfigFilepath, out errorText);
            }
            else
            {
                errorText = "Config file path is empty";
                result = GemDriverError.FileSaveFailed;
            }

            return result;
        }

        public GemDriverError SaveConfigFile(string fileName, out string errorText)
        {
            GemDriverError result;
            XElement root;
            XElement element;
            XElement subElement;
            XElement subElement2;
            XElement subElement3;
            XElement subElement4;
            XElement subElement5;
            XElement triggerElement;
            XElement sendTriggerElement;
            XElement stopTriggerElement;
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo expandedReportInfo;
            ExpandedVariableInfo expandedVariableInfo;
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            string vid;
            Stack<string> parentNameStack;

            string writeAttribute;

            errorText = string.Empty;
            parentNameStack = new Stack<string>();

            try
            {
                root = new XElement("UbiGEM");

                #region [Driver]
                element = new XElement("Driver",
                                       new XElement("Name", this.Configuration.DriverName),
                                       new XElement("Type", this.Configuration.DeviceType),
                                       new XElement("IsAsyncMode", this.Configuration.IsAsyncMode),
                                       new XElement("DeviceID", this.Configuration.DeviceID),
                                       new XElement("Mode", this.Configuration.SECSMode),
                                       new XElement("MaxMessageSize", this.Configuration.MaxMessageSize),
                                       new XElement("LogEnabledSECS1", this.Configuration.LogEnabledSECS1),
                                       new XElement("LogEnabledSECS2", this.Configuration.LogEnabledSECS2),
                                       new XElement("LogEnabledSystem", this.Configuration.LogEnabledSystem),
                                       new XElement("LogExpirationDay", this.Configuration.LogExpirationDay),
                                       new XElement("LogPath", this.Configuration.LogPath),
                                       new XElement("HSMS",
                                                    new XElement("HSMSMode", this.Configuration.HSMSModeConfig.HSMSMode),
                                                    new XElement("RemoteIPAddress", this.Configuration.HSMSModeConfig.RemoteIPAddress),
                                                    new XElement("RemotePortNo", this.Configuration.HSMSModeConfig.RemotePortNo),
                                                    new XElement("LocalIPAddress", this.Configuration.HSMSModeConfig.LocalIPAddress),
                                                    new XElement("LocalPortNo", this.Configuration.HSMSModeConfig.LocalPortNo),
                                                    new XElement("Timeout",
                                                                 new XElement("T3", this.Configuration.HSMSModeConfig.T3),
                                                                 new XElement("T5", this.Configuration.HSMSModeConfig.T5),
                                                                 new XElement("T6", this.Configuration.HSMSModeConfig.T6),
                                                                 new XElement("T7", this.Configuration.HSMSModeConfig.T7),
                                                                 new XElement("T8", this.Configuration.HSMSModeConfig.T8),
                                                                 new XElement("LinkTest", this.Configuration.HSMSModeConfig.LinkTest))));

                root.Add(element);

                element = new XElement("UGCFileName", this.UGCFilepath);
                root.Add(element);
                #endregion

                #region [EquipmentConstants]
                element = new XElement("EquipmentConstants");

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (VariableInfo tempItem in this.VariableCollection.ECV.Items)
                    {
                        expandedVariableInfo = tempItem as ExpandedVariableInfo;

                        if (expandedVariableInfo.VID == expandedVariableInfo.Name)
                        {
                            vid = string.Empty;
                        }
                        else
                        {
                            vid = expandedVariableInfo.VID;
                        }

                        if (expandedVariableInfo.IsInheritance == true)
                        {
                            subElement = new XElement("ECID",
                                                 new XAttribute("ID", vid),
                                                 new XAttribute("Name", expandedVariableInfo.Name),
                                                 new XAttribute("Format", expandedVariableInfo.Format),
                                                 new XAttribute("Use", expandedVariableInfo.IsUse),
                                                 new XAttribute("Value", expandedVariableInfo.Value));
                        }
                        else
                        {
                            subElement = new XElement("ECID",
                                                 new XAttribute("ID", vid),
                                                 new XAttribute("Name", expandedVariableInfo.Name),
                                                 new XAttribute("Format", expandedVariableInfo.Format),
                                                 new XAttribute("Length", expandedVariableInfo.Length),
                                                 new XAttribute("Use", expandedVariableInfo.IsUse),
                                                 new XAttribute("Value", expandedVariableInfo.Value),
                                                 new XAttribute("Description", expandedVariableInfo.Description));
                        }

                        if (tempItem.Format == SECSItemFormat.L && expandedVariableInfo.ChildVariables != null)
                        {
                            subElement2 = new XElement("Childs");

                            parentNameStack.Clear();
                            parentNameStack.Push(expandedVariableInfo.Name);

                            foreach (ExpandedVariableInfo tempChildItem in expandedVariableInfo.ChildVariables)
                            {
                                if (parentNameStack.Contains(tempChildItem.Name) == false)
                                {
                                    result = SetChildVariableInfo(tempChildItem, subElement2, parentNameStack, out errorText);
                                }
                                else
                                {
                                    errorText = string.Format("Variable name recursively duplicate: {0}", tempChildItem.Name);
                                }
                            }
                            parentNameStack.Pop();

                            subElement.Add(subElement2);
                        }

                        if (string.IsNullOrEmpty(errorText) == false)
                        {
                            break;
                        }
                        else
                        {
                            element.Add(subElement);
                        }
                    }
                }
                root.Add(element);
                #endregion
                #region [Variables]
                element = new XElement("Variables");

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (VariableInfo tempItem in this.VariableCollection.Variables.Items.Where(t => t.VIDType != VariableType.ECV))
                    {
                        expandedVariableInfo = tempItem as ExpandedVariableInfo;

                        if (expandedVariableInfo.VID == expandedVariableInfo.Name)
                        {
                            vid = string.Empty;
                        }
                        else
                        {
                            vid = expandedVariableInfo.VID;
                        }

                        if (expandedVariableInfo.IsInheritance == true)
                        {
                            subElement = new XElement("VID",
                                                      new XAttribute("ID", vid),
                                                      new XAttribute("Name", expandedVariableInfo.Name),
                                                      new XAttribute("Format", expandedVariableInfo.Format),
                                                      new XAttribute("Use", expandedVariableInfo.IsUse),
                                                      new XAttribute("Value", expandedVariableInfo.Value));
                        }
                        else
                        {
                            subElement = new XElement("VID",
                                                      new XAttribute("ID", vid),
                                                      new XAttribute("Name", expandedVariableInfo.Name),
                                                      new XAttribute("Format", expandedVariableInfo.Format),
                                                      new XAttribute("Length", expandedVariableInfo.Length),
                                                      new XAttribute("Value", expandedVariableInfo.Value),
                                                      new XAttribute("Use", expandedVariableInfo.IsUse),
                                                      new XAttribute("Description", expandedVariableInfo.Description));
                        }

                        if (expandedVariableInfo.Format == SECSItemFormat.L && expandedVariableInfo.ChildVariables != null)
                        {
                            subElement2 = new XElement("Childs");

                            parentNameStack.Clear();
                            parentNameStack.Push(expandedVariableInfo.Name);

                            foreach (ExpandedVariableInfo tempChildItem in expandedVariableInfo.ChildVariables)
                            {
                                if (parentNameStack.Contains(tempChildItem.Name) == false)
                                {
                                    result = SetChildVariableInfo(tempChildItem, subElement2, parentNameStack, out errorText);
                                }
                                else
                                {
                                    errorText = string.Format("Variable name recursively duplicate: {0}", tempChildItem.Name);
                                }
                            }

                            parentNameStack.Pop();

                            subElement.Add(subElement2);
                        }

                        if (string.IsNullOrEmpty(errorText) == false)
                        {
                            break;
                        }
                        else
                        {
                            element.Add(subElement);
                        }
                    }
                }
                root.Add(element);
                #endregion
                #region [Reports]
                element = new XElement("Reports");

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (ReportInfo tempItem in this.ReportCollection.Items.Values)
                    {
                        expandedReportInfo = tempItem as ExpandedReportInfo;

                        writeAttribute = string.Empty;

                        foreach (VariableInfo tempSubItem in tempItem.Variables.Items)
                        {
                            writeAttribute += tempSubItem.VID + ",";
                        }

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        if (expandedReportInfo.IsInheritance == true)
                        {
                            element.Add(new XElement("Report",
                                                     new XAttribute("ID", tempItem.ReportID),
                                                     new XAttribute("IDList", writeAttribute)));
                        }
                        else
                        {
                            element.Add(new XElement("Report",
                                                     new XAttribute("ID", tempItem.ReportID),
                                                     new XAttribute("IDList", writeAttribute),
                                                     new XAttribute("Description", tempItem.Description)));

                        }
                    }
                }
                root.Add(element);
                #endregion
                #region [Collection Events]
                element = new XElement("CollectionEvents");
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (CollectionEventInfo tempItem in this.CollectionEventCollection.Items.Values)
                    {
                        expandedCollectionEventInfo = tempItem as ExpandedCollectionEventInfo;

                        writeAttribute = string.Empty;

                        foreach (ReportInfo tempSubItem in expandedCollectionEventInfo.Reports.Items.Values)
                        {
                            writeAttribute += tempSubItem.ReportID.ToString() + ",";
                        }

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        if (expandedCollectionEventInfo.IsInheritance == true)
                        {
                            element.Add(new XElement("CEID",
                                                     new XAttribute("ID", tempItem.CEID),
                                                     new XAttribute("Enabled", tempItem.Enabled),
                                                     new XAttribute("IDList", writeAttribute)));
                        }
                        else
                        {
                            element.Add(new XElement("CEID",
                                                     new XAttribute("ID", tempItem.CEID),
                                                     new XAttribute("Name", tempItem.Name),
                                                     new XAttribute("PreDefined", tempItem.PreDefined),
                                                     new XAttribute("Use", tempItem.IsUse),
                                                     new XAttribute("Enabled", tempItem.Enabled),
                                                     new XAttribute("IDList", writeAttribute),
                                                     new XAttribute("Description", tempItem.Description)));
                        }
                    }
                }

                root.Add(element);
                #endregion
                #region [Alarms]
                element = new XElement("Alarms");
                foreach (AlarmInfo tempItem in this.AlarmCollection.Items)
                {
                    element.Add(new XElement("Alarm",
                        new XAttribute("ID", tempItem.ID),
                        new XAttribute("Code", tempItem.Code),
                        new XAttribute("Description", tempItem.Description)));
                }
                root.Add(element);
                #endregion
                #region [Remote Commands]
                element = new XElement("RemoteCommands");

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    #region [Remote Command]
                    foreach (RemoteCommandInfo tempItem in this.RemoteCommandCollection.RemoteCommandItems)
                    {
                        expandedRemoteCommandInfo = tempItem as ExpandedRemoteCommandInfo;

                        subElement = MakeExpandedRemoteCommandValueSetCollectionToXElement(expandedRemoteCommandInfo.ValueSetCollection);
                        triggerElement = MakeTriggerCollectionToXElement("Triggers", expandedRemoteCommandInfo.TriggerCollection);

                        element.Add(new XElement("RemoteCommand",
                                                 new XAttribute("Command", expandedRemoteCommandInfo.RemoteCommand),
                                                 new XAttribute("AutoSend", expandedRemoteCommandInfo.AutoSend),
                                                 new XAttribute("Description", tempItem.Description),
                                                 subElement, triggerElement));
                    }
                    #endregion

                    #region [Enhanced Remote Command]
                    foreach (EnhancedRemoteCommandInfo tempItem in this.RemoteCommandCollection.EnhancedRemoteCommandItems)
                    {
                        expandedEnhancedRemoteCommandInfo = tempItem as ExpandedEnhancedRemoteCommandInfo;

                        subElement = MakeExpandedEnhancedRemoteCommandValueSetCollectionToXElement(expandedEnhancedRemoteCommandInfo.ValueSetCollection);

                        triggerElement = MakeTriggerCollectionToXElement("Triggers", expandedEnhancedRemoteCommandInfo.TriggerCollection);

                        element.Add(new XElement("EnhancedRemoteCommand",
                                                 new XAttribute("Command", expandedEnhancedRemoteCommandInfo.RemoteCommand),
                                                 new XAttribute("AutoSend", expandedEnhancedRemoteCommandInfo.AutoSend),
                                                 new XAttribute("DataID", expandedEnhancedRemoteCommandInfo.DataID ?? string.Empty),
                                                 new XAttribute("ObjSpec", expandedEnhancedRemoteCommandInfo.ObjSpec),
                                                 new XAttribute("Description", tempItem.Description),
                                                 subElement, triggerElement));
                    }
                    #endregion
                }

                root.Add(element);
                #endregion

                #region [FormattedProcessProgramCollection]
                element = new XElement("FormattedProcessProgramCollection");

                foreach (FormattedProcessProgramInfo info in this.FormattedProcessProgramCollection.Items)
                {
                    subElement = new XElement("FormattedProcessProgramInfo",
                            new XAttribute("PPID", info.PPID ?? string.Empty),
                            new XAttribute("AutoSend", info.AutoSend),
                            new XAttribute("MDLN", info.MDLN ?? string.Empty),
                            new XAttribute("SOFTREV", info.SOFTREV ?? string.Empty));
                    triggerElement = MakeTriggerCollectionToXElement("Triggers", info.TriggerCollection);
                    subElement.Add(triggerElement);
                    element.Add(subElement);
                }

                root.Add(element);
                #endregion

                #region [GEMObject]
                element = new XElement("GEMObjectCollection");
                foreach (GEMObject gemObject in this.GEMObjectCollection.Items)
                {
                    subElement = new XElement("GEMObject", new XAttribute("OBJSPEC", gemObject.OBJSPEC),
                                                           new XAttribute("OBJTYPE", gemObject.OBJTYPE)
                                                           );
                    element.Add(subElement);

                    subElement2 = new XElement("AttributeCollection");
                    subElement.Add(subElement2);

                    foreach (GEMObjectAttribute attr in gemObject.AttributeCollection.Items)
                    {
                        subElement3 = new XElement("Attribute", new XAttribute("ATTRID", attr.ATTRID),
                                                                         new XAttribute("Format", attr.Format),
                                                                         new XAttribute("IsSelected", attr.IsSelected));

                        if (attr.Format == SECSItemFormat.L)
                        {
                            subElement3.Add(MakeGEMObjectAttributesToXElement(attr.ChildAttributes));
                        }
                        else
                        {
                            subElement3.Add(new XAttribute("ATTRDATA", attr.ATTRDATA));
                        }

                        subElement2.Add(subElement3);
                    }

                    subElement2 = new XElement("GEMObjectIDCollection");
                    subElement.Add(subElement2);

                    foreach (GEMObjectID gemObjectID in gemObject.ObjectIDCollection.Items)
                    {
                        subElement3 = new XElement("GEMObjectID", new XAttribute("OBJID", gemObjectID.OBJID),
                                                                  new XAttribute("IsSelected", gemObjectID.IsSelected)
                                                                  );
                        subElement2.Add(subElement3);

                        subElement4 = new XElement("GEMObjectAttributeCollection");
                        subElement3.Add(subElement4);

                        foreach (GEMObjectAttribute attr in gemObjectID.ObjectAttributeCollection.Items)
                        {
                            subElement5 = new XElement("GEMObjectAttribute", new XAttribute("ATTRID", attr.ATTRID),
                                                                             new XAttribute("Format", attr.Format),
                                                                             new XAttribute("IsSelected", attr.IsSelected));

                            if (attr.Format == SECSItemFormat.L)
                            {
                                subElement5.Add(MakeGEMObjectAttributesToXElement(attr.ChildAttributes));
                            }
                            else
                            {
                                subElement5.Add(new XAttribute("ATTRDATA", attr.ATTRDATA));
                            }

                            subElement4.Add(subElement5);
                        }
                    }
                }
                root.Add(element);
                #endregion

                #region [SupervisedGEMObject]
                element = new XElement("SupervisedGEMObjectCollection");
                foreach (SupervisedGEMObject gemObject in this.SupervisedGEMObjectCollection.Items)
                {
                    subElement = new XElement("SupervisedGEMObject", new XAttribute("OBJSPEC", gemObject.OBJSPEC),
                                                           new XAttribute("OBJTOKEN", gemObject.OBJTOKEN)
                                                           );
                    element.Add(subElement);

                    subElement2 = new XElement("GEMObjectAttributeCollection");
                    subElement.Add(subElement2);

                    foreach (GEMObjectAttribute attr in gemObject.GEMObjectAttributeCollection.Items)
                    {
                        subElement3 = new XElement("GEMObjectAttribute", new XAttribute("ATTRID", attr.ATTRID),
                                                                            new XAttribute("Format", attr.Format),
                                                                            new XAttribute("IsSelected", attr.IsSelected));

                        if (attr.Format == SECSItemFormat.L)
                        {
                            subElement3.Add(MakeGEMObjectAttributesToXElement(attr.ChildAttributes));
                        }
                        else
                        {
                            subElement3.Add(new XAttribute("ATTRDATA", attr.ATTRDATA));
                        }

                        subElement2.Add(subElement3);
                    }
                }
                root.Add(element);
                #endregion

                #region [MapSetupDataCollection]
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    element = new XElement("MapSetupDataCollection");

                    foreach (MapSetupData mapSetupData in this.MapSetupDataCollection)
                    {
                        subElement = new XElement("MapSetupData"
                                            , new XAttribute("MID", mapSetupData.MaterialID)
                                            , new XAttribute("IDTYP", mapSetupData.IDType)
                                            , new XAttribute("FNLOC", mapSetupData.FlatNotchLocation)
                                            , new XAttribute("FFROT", mapSetupData.FilmFrameRotation)
                                            , new XAttribute("ORLOC", mapSetupData.OriginLocation)
                                            , new XAttribute("RPSEL", mapSetupData.ReferencePointSelect)
                                            , new XAttribute("DUTMS", mapSetupData.DieUnitsOfMeasure)
                                            , new XAttribute("XDIES", mapSetupData.XAxisDieSize)
                                            , new XAttribute("YDIES", mapSetupData.YAxisDieSize)
                                            , new XAttribute("ROWCT", mapSetupData.RowCount)
                                            , new XAttribute("COLCT", mapSetupData.ColumnCount)
                                            , new XAttribute("NULBC", mapSetupData.NullBinCodeValue)
                                            , new XAttribute("BCEQU", mapSetupData.BinCodeEquivalent)
                                            , new XAttribute("PRDCT", mapSetupData.ProcessDieCount)
                                            , new XAttribute("PRAXI", mapSetupData.ProcessAxis)
                                            , new XAttribute("MLCL", mapSetupData.MessageLength)
                                            );

                        subElement2 = new XElement("ReferencePointCollection");
                        subElement.Add(subElement2);

                        foreach (ReferencePointItem rpItem in mapSetupData.ReferencePoint)
                        {
                            subElement2.Add(new XElement("ReferencePointItem"
                                            , new XAttribute("X", rpItem.X)
                                            , new XAttribute("Y", rpItem.Y)));
                        }

                        element.Add(subElement);
                    }

                    root.Add(element);
                }
                #endregion

                #region [MapDataType1Collection]
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    element = new XElement("MapDataType1Collection");

                    foreach (MapDataType1 mapDataType1 in this.MapDataType1Collection)
                    {
                        subElement = new XElement("MapDataType1"
                                            , new XAttribute("MID", mapDataType1.MaterialID)
                                            , new XAttribute("IDTYP", mapDataType1.IDType)
                                            );

                        subElement2 = new XElement("ReferenceStartingCollection");
                        subElement.Add(subElement2);

                        foreach (ReferenceStartingInfo rsInfo in mapDataType1.ReferenceStartingList)
                        {
                            subElement2.Add(new XElement("ReferenceStartingInfo"
                                            , new XAttribute("X", rsInfo.X)
                                            , new XAttribute("Y", rsInfo.Y)
                                            , new XAttribute("Direction", rsInfo.Direction)
                                            , new XAttribute("BINLT", rsInfo.BinList)));
                        }

                        element.Add(subElement);
                    }

                    root.Add(element);
                }
                #endregion

                #region [MapDataType2Collection]
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    element = new XElement("MapDataType2Collection");

                    foreach (MapDataType2 mapDataType2 in this.MapDataType2Collection)
                    {
                        subElement = new XElement("MapDataType2"
                                            , new XAttribute("MID", mapDataType2.MaterialID)
                                            , new XAttribute("IDTYP", mapDataType2.IDType)
                                            , new XAttribute("STRPx", mapDataType2.StartPointX)
                                            , new XAttribute("STRPy", mapDataType2.StartPointY)
                                            , new XAttribute("BINLT", mapDataType2.BinList)
                                            );
                        element.Add(subElement);
                    }

                    root.Add(element);
                }
                #endregion
                #region [MapDataType3Collection]
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    element = new XElement("MapDataType3Collection");

                    foreach (MapDataType3 mapDataType3 in this.MapDataType3Collection)
                    {
                        subElement = new XElement("MapDataType3"
                                            , new XAttribute("MID", mapDataType3.MaterialID)
                                            , new XAttribute("IDTYP", mapDataType3.IDType)
                                            );

                        subElement2 = new XElement("XYPOSCollection");
                        subElement.Add(subElement2);

                        foreach (XYPosInfo xyPosInfo in mapDataType3.XYPOSList)
                        {
                            subElement2.Add(new XElement("XYPOSInfo"
                                            , new XAttribute("X", xyPosInfo.X)
                                            , new XAttribute("Y", xyPosInfo.Y)
                                            , new XAttribute("BINLT", xyPosInfo.BinList)));
                        }

                        element.Add(subElement);
                    }

                    root.Add(element);
                }
                #endregion

                #region [GEM Setting]
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    element = new XElement("GEMSetting");

                    #region [Stream 1]
                    if (this.CurrentSetting.S1F3SelectedEquipmentStatus.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S1F3SelectedEquipmentStatus.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S1F3", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S1F11StatusVariableNamelist.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S1F11StatusVariableNamelist.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S1F11", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S1F21DataVariableNamelist.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S1F21DataVariableNamelist.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S1F21", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S1F23CollectionEventList.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S1F23CollectionEventList.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S1F23", new XAttribute("Use", writeAttribute)));
                    }
                    #endregion
                    #region [Stream 2]
                    if (this.CurrentSetting.S2F13EquipmentConstant.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S2F13EquipmentConstant.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S2F13", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S2F15NewEquipmentConstant.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S2F15NewEquipmentConstant.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S2F15", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.LoopbackDiagnostic.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.LoopbackDiagnostic.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + " ";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S2F25", new XAttribute("ABS", writeAttribute)));
                    }

                    if (this.CurrentSetting.S2F29EquipmentConstantNamelist.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S2F29EquipmentConstantNamelist.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S2F29", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S2F43ResetSpoolingStreamsAndFunctions.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S2F43ResetSpoolingStreamsAndFunctions.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S2F43", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S2F47VariableLimitAttributeRequest.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S2F47VariableLimitAttributeRequest.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S2F47", new XAttribute("Use", writeAttribute)));
                    }
                    #endregion
                    #region [Stream 5]
                    if (this.CurrentSetting.S5F3SelectedAlarmSend.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S5F3SelectedAlarmSend.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S5F3Selected", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S5F3EnabledAlarmSend.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S5F3EnabledAlarmSend.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S5F3Enabled", new XAttribute("Use", writeAttribute)));
                    }

                    if (this.CurrentSetting.S5F5ListAlarmsRequest.Count > 0)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.S5F5ListAlarmsRequest.ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        element.Add(new XElement("S5F5", new XAttribute("Use", writeAttribute)));
                    }
                    #endregion
                    #region [Stream 10]
                    element.Add(new XElement("S10F3",
                                             new XAttribute("TID", this.CurrentSetting.TerminalMessage.S10F3TID),
                                             new XAttribute("TEXT", this.CurrentSetting.TerminalMessage.S10F3TerminalMessage)));

                    subElement = new XElement("TEXTs");

                    this.CurrentSetting.TerminalMessage.S10F5TerminalMessages.ForEach(t =>
                    {
                        subElement.Add(new XElement("TEXT", new XAttribute("TEXT", t)));
                    });

                    element.Add(new XElement("S10F5",
                                             new XAttribute("TID", this.CurrentSetting.TerminalMessage.S10F5TID),
                                             subElement));
                    #endregion
                    #region [Stream 12]
                    element.Add(new XElement("S12F19",
                                             new XAttribute("MAOER", this.CurrentSetting.SelectedMapError),
                                             new XAttribute("DATLC", this.CurrentSetting.DATLC)));
                    #endregion
                    #region [Stream 14]
                    subElement = new XElement("S14F1"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F1)
                                , new XAttribute("SelectedObjectType", this.CurrentSetting.SelectedObjectTypeForS14F1));

                    element.Add(subElement);

                    foreach (var keyForSelectedObjectList in this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1.Keys)
                    {
                        subElement2 = new XElement("SelectedObjectAttributeFilterList"
                                    , new XAttribute("OBJSPEC", keyForSelectedObjectList.OBJSPEC)
                                    , new XAttribute("OBJTYPE", keyForSelectedObjectList.OBJTYPE));

                        foreach (var gemObjectAttributeFilter in this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1[keyForSelectedObjectList])
                        {
                            subElement2.Add(new XElement("AttributeFilter"
                                            , new XAttribute("IsSelected", gemObjectAttributeFilter.IsSelected)
                                            , new XAttribute("ObjectID", gemObjectAttributeFilter.ObjectID)
                                            , new XAttribute("ATTRID", gemObjectAttributeFilter.ATTRID)
                                            , new XAttribute("ATTRDATA", gemObjectAttributeFilter.ATTRDATA)
                                            , new XAttribute("ATTRRELN", gemObjectAttributeFilter.ATTRRELN)));
                        }

                        subElement.Add(subElement2);
                    }

                    subElement = new XElement("S14F3"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F3)
                                , new XAttribute("SelectedObjectType", this.CurrentSetting.SelectedObjectTypeForS14F3));

                    element.Add(subElement);

                    subElement = new XElement("S14F5"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F5));

                    element.Add(subElement);


                    subElement = new XElement("S14F7"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F7));

                    foreach (string objSpec in this.CurrentSetting.SelectedObjectTypeListForS14F7.Keys)
                    {
                        writeAttribute = string.Empty;

                        this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec].ForEach(t =>
                        {
                            writeAttribute += t.ToString() + ",";
                        });

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        subElement.Add(new XElement("SelectedType"
                                    , new XAttribute("OBJSPEC", objSpec)
                                    , new XAttribute("SelectedTypes", writeAttribute)));
                    }

                    element.Add(subElement);

                    subElement = new XElement("S14F9"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F9)
                                , new XAttribute("SelectedObjectType", this.CurrentSetting.SelectedObjectTypeForS14F9));

                    element.Add(subElement);

                    subElement = new XElement("S14F11"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F11));

                    element.Add(subElement);

                    subElement = new XElement("S14F13"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F13));

                    element.Add(subElement);

                    subElement = new XElement("S14F15"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F15));

                    foreach (string tempObjectSpecifier in this.CurrentSetting.SelectedAttachedObjectActionListForS14F15.Keys)
                    {
                        if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[tempObjectSpecifier] != null)
                        {
                            subElement.Add(new XElement("AttachedObjectActionInfo"
                                                , new XAttribute("OBJSPEC", tempObjectSpecifier)
                                                , new XAttribute("OBJCMD", this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[tempObjectSpecifier].OBJCMD)
                                                , new XAttribute("OBJTOKEN", this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[tempObjectSpecifier].OBJTOKEN)));
                        }
                    }

                    subElement = new XElement("S14F17"
                                , new XAttribute("SelectedObjectSpecifier", this.CurrentSetting.SelectedObjectSpecifierForS14F17));

                    foreach (string tempObjectSpecifier in this.CurrentSetting.SelectedAttachedObjectActionListForS14F17.Keys)
                    {
                        if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[tempObjectSpecifier] != null)
                        {
                            subElement.Add(new XElement("AttachedObjectActionInfo"
                                                , new XAttribute("OBJSPEC", tempObjectSpecifier)
                                                , new XAttribute("OBJCMD", this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[tempObjectSpecifier].OBJCMD)
                                                , new XAttribute("TARGETSPEC", this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[tempObjectSpecifier].TARGETSPEC)));
                        }
                    }

                    element.Add(subElement);

                    #endregion
                    #region [ACK]
                    subElement = new XElement("ACK");

                    foreach (var ackItem in this.CurrentSetting.AckCollection.Items)
                    {
                        subElement.Add(new XElement("ACKItem",
                                                    new XAttribute("Stream", ackItem.Stream),
                                                    new XAttribute("Function", ackItem.Function),
                                                    new XAttribute("Use", ackItem.Use),
                                                    new XAttribute("Value", ackItem.Value)));
                    }

                    element.Add(subElement);
                    #endregion
                    #region [UseReply]
                    subElement = new XElement("UseReply");

                    foreach (var replyItem in this.CurrentSetting.UseReplyCollection)
                    {
                        subElement.Add(new XElement("ReplyItem",
                                                    new XAttribute("Stream", replyItem.Stream),
                                                    new XAttribute("Function", replyItem.Function),
                                                    new XAttribute("Reply", replyItem.SendReply)));
                    }

                    element.Add(subElement);
                    #endregion
                    #region [ETC]
                    subElement = new XElement("ETC");

                    if (string.IsNullOrEmpty(this.CurrentSetting.ProcessProgramFileS7F1) == false)
                    {
                        subElement.Add(new XElement("ProcessProgramFileS7F1", new XAttribute("FileName", this.CurrentSetting.ProcessProgramFileS7F1)));
                    }

                    if (string.IsNullOrEmpty(this.CurrentSetting.ProcessProgramFileS7F3) == false)
                    {
                        subElement.Add(new XElement("ProcessProgramFileS7F3", new XAttribute("FileName", this.CurrentSetting.ProcessProgramFileS7F3)));
                    }

                    if (string.IsNullOrEmpty(this.CurrentSetting.ProcessProgramIDS7F5) == false)
                    {
                        subElement.Add(new XElement("ProcessProgramIDS7F5", new XAttribute("PPID", this.CurrentSetting.ProcessProgramIDS7F5)));
                    }

                    if (string.IsNullOrEmpty(this.CurrentSetting.ProcessProgramIDS7F23) == false)
                    {
                        subElement.Add(new XElement("ProcessProgramIDS7F23", new XAttribute("PPID", this.CurrentSetting.ProcessProgramIDS7F23)));
                    }

                    if (string.IsNullOrEmpty(this.CurrentSetting.ProcessProgramIDS7F25) == false)
                    {
                        subElement.Add(new XElement("ProcessProgramIDS7F25", new XAttribute("PPID", this.CurrentSetting.ProcessProgramIDS7F25)));
                    }

                    writeAttribute = string.Empty;

                    foreach (string ppid in this.CurrentSetting.ProcessProgramDelete)
                    {
                        writeAttribute += "," + ppid;
                    }

                    if (writeAttribute.Length > 0)
                    {
                        writeAttribute = writeAttribute.Substring(1);
                    }

                    if (string.IsNullOrEmpty(writeAttribute) == false)
                    {
                        subElement.Add(new XElement("ProcessProgramDelete", new XAttribute("IDList", writeAttribute)));
                    }

                    if (this.CurrentSetting.AutoSendDefineReport == true)
                    {
                        subElement.Add(new XElement("AutoSendDefineReport", new XAttribute("Value", this.CurrentSetting.AutoSendDefineReport)));
                    }

                    if (this.CurrentSetting.IsSaveRecipeReceived == true)
                    {
                        subElement.Add(new XElement("IsSaveRecipeReceived", new XAttribute("Value", this.CurrentSetting.IsSaveRecipeReceived)));
                    }

                    if (this.CurrentSetting.AutoSendS1F13 == true)
                    {
                        subElement.Add(new XElement("AutoSendS1F13", new XAttribute("Value", this.CurrentSetting.AutoSendS1F13)));
                    }

                    element.Add(subElement);
                }
                #endregion

                root.Add(element);
                #endregion
                #region [Trace Data]
                element = new XElement("TraceDatas");

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (ExpandedTraceInfo tempTraceInfo in this.TraceCollection.Items)
                    {
                        writeAttribute = string.Empty;

                        foreach (string tempVID in tempTraceInfo.Variables)
                        {
                            writeAttribute += tempVID + ",";
                        }

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);
                        }

                        sendTriggerElement = MakeTriggerCollectionToXElement("SendTriggers", tempTraceInfo.SendTriggerCollection);

                        stopTriggerElement = MakeTriggerCollectionToXElement("StopTriggers", tempTraceInfo.StopTriggerCollection);

                        element.Add(new XElement("TraceData",
                                                 new XAttribute("ID", tempTraceInfo.TraceID),
                                                 new XAttribute("Auto", tempTraceInfo.AutoSend),
                                                 new XAttribute("AutoStop", tempTraceInfo.AutoStop),
                                                 new XAttribute("Period", tempTraceInfo.Dsper),
                                                 new XAttribute("TotalNumber", tempTraceInfo.TotalSample),
                                                 new XAttribute("GroupSize", tempTraceInfo.ReportGroupSize),
                                                 new XAttribute("IDList", writeAttribute),
                                                 sendTriggerElement, stopTriggerElement
                                                 ));
                    }
                }

                root.Add(element);
                #endregion
                #region [Limit Monitoring]
                element = new XElement("LimitMonitorings");
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (ExpandedLimitMonitoringInfo tempLimitMonitoringInfo in this.LimitMonitoringCollection.Items)
                    {
                        subElement = new XElement("Limits");

                        foreach (ExpandedLimitMonitoringItem tempLimitMonitoringItem in tempLimitMonitoringInfo.Items)
                        {
                            subElement.Add(new XElement("Limit",
                                                    new XAttribute("ID", tempLimitMonitoringItem.LimitID),
                                                    new XAttribute("Upper", tempLimitMonitoringItem.Upper),
                                                    new XAttribute("Lower", tempLimitMonitoringItem.Lower)));
                        }

                        triggerElement = MakeTriggerCollectionToXElement("Triggers", tempLimitMonitoringInfo.TriggerCollection);

                        element.Add(new XElement("LimitMonitoring",
                                                 new XAttribute("ID", tempLimitMonitoringInfo.Variable.VID),
                                                 new XAttribute("Auto", tempLimitMonitoringInfo.AutoSend),
                                                 subElement, triggerElement));
                    }
                }

                root.Add(element);
                #endregion
                #region [Message]
                element = new XElement("SECSMessages");
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    foreach (var message in this.UserMessageData.Values)
                    {
                        subElement = new XElement("SECSMessage",
                                                  new XAttribute("Name", message.Name),
                                                  new XAttribute("Direction", message.Direction),
                                                  new XAttribute("Stream", message.Stream),
                                                  new XAttribute("Function", message.Function),
                                                  new XAttribute("WaitBit", message.WaitBit));

                        subElement.Add(new XCData(string.Format("\n{0}\n", message.Data)));
                        element.Add(subElement);
                    }
                }

                root.Add(element);
                #endregion

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    root.Save(Path.GetTempFileName());

                    root.Save(fileName);
                    result = GemDriverError.Ok;
                    this.ConfigFilepath = fileName;
                }
                else
                {
                    result = GemDriverError.FileSaveFailed;
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = string.Format("XML Parsing Error: {0}", ex.Message);
            }
            finally
            {
                root = null;
                element = null;
                subElement = null;
            }

            if (result == GemDriverError.Ok && string.IsNullOrEmpty(errorText) == false)
            {
                result = GemDriverError.Mismatch;
            }

            return result;
        }
        #endregion
        #region NewProject
        public void NewProject()
        {
            this.ConfigFilepath = string.Empty;
            this.UGCFilepath = string.Empty;

            this.DataDictionaryCollection = new DataDictionaryCollection();
            this.VariableCollection = new VariableCollection();
            this.ReportCollection = new ReportCollection();
            this.CollectionEventCollection = new CollectionEventCollection();
            this.AlarmCollection = new AlarmCollection();

            this.RemoteCommandCollection.RemoteCommandItems.Clear();
            this.RemoteCommandCollection.EnhancedRemoteCommandItems.Clear();
            this.TraceCollection.Items.Clear();
            this.LimitMonitoringCollection.Items.Clear();

            this.FormattedProcessProgramCollection.Clear();

            this.UserMessage.Clear();
            this.UserMessageData.Clear();

            this.Configuration = new Configurtion
            {
                LogPath = @"C:\Logs",

                DeviceType = this._driver.Config.DeviceType,
                UMDFileName = "[{StandardMessageSet}]"
            };

            this.CurrentSetting.Clear();
        }
        #endregion
        #region SetChildVariableInfo
        private GemDriverError SetChildVariableInfo(ExpandedVariableInfo variableInfo, XElement element, Stack<string> parentNameStack, out string errorText)
        {
            GemDriverError result;
            XElement childRootElement;
            XElement childElement;
            ExpandedVariableInfo childVariableInfo;
            string vid;

            errorText = string.Empty;

            try
            {

                if (variableInfo.VID == variableInfo.Name)
                {
                    vid = string.Empty;
                }
                else
                {
                    vid = variableInfo.VID;
                }

                result = GemDriverError.Ok;

                if (variableInfo.ChildVariables != null && variableInfo.ChildVariables.Count > 0)
                {
                    childRootElement = new XElement("Child",
                                                    new XAttribute("ID", vid),
                                                    new XAttribute("Name", variableInfo.Name),
                                                    new XAttribute("Format", variableInfo.Format),
                                                    new XAttribute("Length", variableInfo.Length),
                                                    new XAttribute("Description", variableInfo.Description));

                    childElement = new XElement("Childs");

                    parentNameStack.Push(variableInfo.Name);

                    foreach (VariableInfo tempChildItem in variableInfo.ChildVariables)
                    {
                        childVariableInfo = tempChildItem as ExpandedVariableInfo;

                        if (childVariableInfo.ChildVariables != null && childVariableInfo.ChildVariables.Count > 0)
                        {
                            if (parentNameStack.Contains(tempChildItem.Name) == false)
                            {
                                result = SetChildVariableInfo(childVariableInfo, childElement, parentNameStack, out errorText);
                            }
                            else
                            {
                                errorText = string.Format("Variable name recursively duplicate: {0}", tempChildItem.Name);
                            }

                            if (string.IsNullOrEmpty(errorText) == false)
                            {
                                break;
                            }
                        }
                        else
                        {
                            childElement.Add(new XElement("Child",
                                                          new XAttribute("ID", vid),
                                                          new XAttribute("Name", childVariableInfo.Name),
                                                          new XAttribute("Format", childVariableInfo.Format),
                                                          new XAttribute("Length", childVariableInfo.Length),
                                                          new XAttribute("Value", childVariableInfo.Value),
                                                          new XAttribute("Description", childVariableInfo.Description)));
                        }
                    }

                    childRootElement.Add(childElement);
                    element.Add(childRootElement);

                    parentNameStack.Pop();
                }
                else
                {
                    element.Add(new XElement("Child",
                                             new XAttribute("ID", vid),
                                             new XAttribute("Name", variableInfo.Name),
                                             new XAttribute("Format", variableInfo.Format),
                                             new XAttribute("Length", variableInfo.Length),
                                             new XAttribute("Value", variableInfo.Value),
                                             new XAttribute("Description", variableInfo.Description)));
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            if (result == GemDriverError.Ok && string.IsNullOrEmpty(errorText) == false)
            {
                result = GemDriverError.Mismatch;
            }

            return result;
        }
        #endregion
        #region SendS1F1
        public MessageError SendS1F1()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(1, 1, this._driver.Config.DeviceType);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS1F3
        public MessageError SendS1F3()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat svidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    svidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(1, 3, this._driver.Config.DeviceType);

                    if (this.VariableCollection.Variables.Items.Count(t => t.VID != t.Name) == this.CurrentSetting.S1F3SelectedEquipmentStatus.Count)
                    {
                        message.Body.Add("SVIDCOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("SVIDCOUNT", SECSItemFormat.L, this.CurrentSetting.S1F3SelectedEquipmentStatus.Count, null);

                        this.CurrentSetting.S1F3SelectedEquipmentStatus.ForEach(t =>
                        {
                            variableInfo = this.VariableCollection[t] as ExpandedVariableInfo;

                            if (svidFormat == SECSItemFormat.J || svidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(variableInfo.Name, svidFormat, Encoding.Default.GetByteCount(t), t);
                            }
                            else
                            {
                                converted = ConvertValue(svidFormat, t);

                                if (converted != null)
                                {
                                    message.Body.Add(variableInfo.Name, svidFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add(variableInfo.Name, svidFormat, 0, string.Empty);
                                }
                            }
                        });
                    }

                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS1F3ForControlState
        public MessageError SendS1F3ForControlState()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat svidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    svidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(1, 3, this._driver.Config.DeviceType);

                    message.Body.Add("SVIDCOUNT", SECSItemFormat.L, 1, null);

                    variableInfo = this.VariableCollection.Items.FirstOrDefault(t => t.PreDefined == true && t.Name == PreDefinedV.ControlState.ToString()) as ExpandedVariableInfo;

                    if (variableInfo != null)
                    {
                        if (svidFormat == SECSItemFormat.A || svidFormat == SECSItemFormat.J)
                        {
                            message.Body.Add(variableInfo.Name, svidFormat, Encoding.Default.GetByteCount(variableInfo.VID), variableInfo.VID);
                        }
                        else
                        {
                            converted = ConvertValue(svidFormat, variableInfo.VID);

                            if (converted != null)
                            {
                                message.Body.Add(variableInfo.Name, svidFormat, 1, converted);
                            }
                            else
                            {
                                message.Body.Add(variableInfo.Name, svidFormat, 0, string.Empty);
                            }
                        }

                        driverResult = this._driver.SendSECSMessage(message);

                        if (driverResult == MessageError.Ok)
                        {
                            this._lastSystemBytesS1F3ForControlState = message.SystemBytes;
                        }

                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS1F11
        public MessageError SendS1F11()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat svidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    svidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(1, 11, this._driver.Config.DeviceType);

                    if (this.VariableCollection.SV.Items.Count(t => t.VID != t.Name) == this.CurrentSetting.S1F11StatusVariableNamelist.Count)
                    {
                        message.Body.Add("SVIDCOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("SVIDCOUNT", SECSItemFormat.L, this.CurrentSetting.S1F11StatusVariableNamelist.Count, null);

                        this.CurrentSetting.S1F11StatusVariableNamelist.ForEach(t =>
                        {
                            variableInfo = this.VariableCollection[t] as ExpandedVariableInfo;

                            if (svidFormat == SECSItemFormat.A || svidFormat == SECSItemFormat.J)
                            {
                                message.Body.Add(variableInfo.Name, svidFormat, Encoding.Default.GetByteCount(t), t);
                            }
                            else
                            {
                                converted = ConvertValue(svidFormat, t);

                                if (converted != null)
                                {
                                    message.Body.Add(variableInfo.Name, svidFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add(variableInfo.Name, svidFormat, 0, string.Empty);
                                }
                            }
                        });
                    }

                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS1F13
        public MessageError SendS1F13()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(1, 13, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 0, null);

                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    this.CommunicationStateBefore = this.CommunicationState;
                    this.CommunicationState = CommunicationState.WaitCRFromHost;

                    if (this.CommunicationStateBefore != this.CommunicationState)
                    {
                        RaiseCommunicationStateChanged(this.CommunicationState);
                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS1F15
        public MessageError SendS1F15()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(1, 15, this._driver.Config.DeviceType);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS1F17
        public MessageError SendS1F17()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(1, 17, this._driver.Config.DeviceType);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS1F21
        public MessageError SendS1F21()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat svidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;
           
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    svidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(1, 21, this._driver.Config.DeviceType);

                    if (this.VariableCollection.DVVal.Items.Count == this.CurrentSetting.S1F21DataVariableNamelist.Count)
                    {
                        message.Body.Add("VIDCOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("VIDCOUNT", SECSItemFormat.L, this.CurrentSetting.S1F21DataVariableNamelist.Count, null);

                        this.CurrentSetting.S1F21DataVariableNamelist.ForEach(t =>
                        {
                            variableInfo = this.VariableCollection[t] as ExpandedVariableInfo;

                            if (svidFormat == SECSItemFormat.A || svidFormat == SECSItemFormat.J)
                            {
                                message.Body.Add(variableInfo.Name, svidFormat, Encoding.Default.GetByteCount(t), t);
                            }
                            else
                            {
                                converted = ConvertValue(svidFormat, t);

                                if (converted != null)
                                {
                                    message.Body.Add(variableInfo.Name, svidFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add(variableInfo.Name, svidFormat, 0, string.Empty);
                                }
                            }
                        });
                    }

                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS1F23
        public MessageError SendS1F23()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat ceidFormat;
            ExpandedCollectionEventInfo ceInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.CEID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    ceidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(1, 23, this._driver.Config.DeviceType);

                    if (this.CollectionEventCollection.Items.Count(t => t.Value.Enabled == true) == this.CurrentSetting.S1F23CollectionEventList.Count)
                    {
                        message.Body.Add("CEIDCOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("CEIDCOUNT", SECSItemFormat.L, this.CurrentSetting.S1F23CollectionEventList.Count, null);

                        this.CurrentSetting.S1F23CollectionEventList.ForEach(t =>
                        {
                            ceInfo = this.CollectionEventCollection[t] as ExpandedCollectionEventInfo;

                            if (ceidFormat == SECSItemFormat.A || ceidFormat == SECSItemFormat.J)
                            {
                                message.Body.Add(ceInfo.Name, ceidFormat, Encoding.Default.GetByteCount(t), t);
                            }
                            else
                            {
                                converted = ConvertValue(ceidFormat, t);

                                if (converted != null)
                                {
                                    message.Body.Add(ceInfo.Name, ceidFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add(ceInfo.Name, ceidFormat, 0, string.Empty);
                                }
                            }
                        });
                    }

                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F13
        public MessageError SendS2F13()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat ecidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.ECID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    ecidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(2, 13, this._driver.Config.DeviceType);

                    if (this.VariableCollection.ECV.Items.Count(t => t.VID != t.Name) == this.CurrentSetting.S2F13EquipmentConstant.Count)
                    {
                        message.Body.Add("ECIDCOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("ECIDCOUNT", SECSItemFormat.L, this.CurrentSetting.S2F13EquipmentConstant.Count, null);

                        this.CurrentSetting.S2F13EquipmentConstant.ForEach(t =>
                        {
                            variableInfo = this.VariableCollection[t] as ExpandedVariableInfo;

                            if (ecidFormat == SECSItemFormat.A || ecidFormat == SECSItemFormat.J)
                            {
                                message.Body.Add(variableInfo.Name, ecidFormat, Encoding.Default.GetByteCount(t), t);
                            }
                            else
                            {
                                converted = ConvertValue(ecidFormat, t);

                                if (converted != null)
                                {
                                    message.Body.Add(variableInfo.Name, ecidFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add(variableInfo.Name, ecidFormat, 0, string.Empty);
                                }
                            }
                        });
                    }

                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F15
        public MessageError SendS2F15()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat svidFormat;
            ExpandedVariableInfo expandedVariableInfo;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    svidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(2, 15, this._driver.Config.DeviceType);

                    message.Body.Add("ECIDCOUNT", SECSItemFormat.L, this.CurrentSetting.S2F15NewEquipmentConstant.Count, null);

                    this.CurrentSetting.S2F15NewEquipmentConstant.ForEach(t =>
                    {
                        expandedVariableInfo = this.VariableCollection[t] as ExpandedVariableInfo;
                        AddEquipmentConstantsChild(message, expandedVariableInfo);
                    });

                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F17
        public MessageError SendS2F17()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(2, 17, this._driver.Config.DeviceType);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS2F23
        public MessageError SendS2F23(ExpandedTraceInfo traceInfo)
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat tridFormat;
            SECSItemFormat totsmpFormat;
            SECSItemFormat repgszFormat;
            SECSItemFormat svidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.TRID.ToString()];
                tridFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.TOTSMP.ToString()];
                totsmpFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.REPGSZ.ToString()];
                repgszFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];
                svidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                message = this._driver.Messages.GetMessageHeader(2, 23, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 5, null);

                if (tridFormat == SECSItemFormat.A || tridFormat == SECSItemFormat.J)
                {
                    message.Body.Add("TRID", tridFormat, Encoding.Default.GetByteCount(traceInfo.TraceID), traceInfo.TraceID);
                }
                else
                {
                    message.Body.Add("TRID", tridFormat, 1, traceInfo.TraceID);
                }

                message.Body.Add("DSPER", SECSItemFormat.A, Encoding.Default.GetByteCount(traceInfo.Dsper), traceInfo.Dsper);

                if (totsmpFormat == SECSItemFormat.A || totsmpFormat == SECSItemFormat.J)
                {
                    message.Body.Add("TOTSMP", totsmpFormat, Encoding.Default.GetByteCount(traceInfo.TotalSample.ToString()), traceInfo.TotalSample.ToString());
                }
                else
                {
                    message.Body.Add("TOTSMP", totsmpFormat, 1, traceInfo.TotalSample);
                }

                if (repgszFormat == SECSItemFormat.A || totsmpFormat == SECSItemFormat.J)
                {
                    message.Body.Add("REPGSZ", repgszFormat, Encoding.Default.GetByteCount(traceInfo.ReportGroupSize.ToString()), traceInfo.ReportGroupSize.ToString());
                }
                else
                {
                    message.Body.Add("REPGSZ", repgszFormat, 1, traceInfo.ReportGroupSize);
                }
                
                message.Body.Add("SVIDCOUNT", SECSItemFormat.L, traceInfo.Variables.Count, null);

                traceInfo.Variables.ForEach(t =>
                {
                    variableInfo = this.VariableCollection[t] as ExpandedVariableInfo;

                    if (svidFormat == SECSItemFormat.A || svidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add(variableInfo.Name, svidFormat, Encoding.Default.GetByteCount(t), t);
                    }
                    else
                    {
                        converted = ConvertValue(svidFormat, t);

                        if (converted != null)
                        {
                            message.Body.Add(variableInfo.Name, svidFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add(variableInfo.Name, svidFormat, 0, string.Empty);
                        }
                    }
                });

                driverResult = this._driver.SendSECSMessage(message);

                RaiseDriverLogAdded1(this, DriverLogType.INFO, string.Format("Result={0}, TRID={1}", driverResult, traceInfo.TraceID));
            }

            return driverResult;
        }
        #endregion
        #region SendS2F23
        public MessageError SendS2F23Stop(ExpandedTraceInfo traceInfo)
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat tridFormat;
            SECSItemFormat totsmpFormat;
            SECSItemFormat repgszFormat;
            SECSItemFormat svidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.TRID.ToString()];
                tridFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.TOTSMP.ToString()];
                totsmpFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.REPGSZ.ToString()];
                repgszFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];
                svidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                message = this._driver.Messages.GetMessageHeader(2, 23, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 5, null);
                if (tridFormat == SECSItemFormat.A || tridFormat == SECSItemFormat.J)
                {
                    message.Body.Add("TRID", tridFormat, Encoding.Default.GetByteCount(traceInfo.TraceID), traceInfo.TraceID);
                }
                else
                {
                    message.Body.Add("TRID", tridFormat, 1, traceInfo.TraceID);
                }

                message.Body.Add("DSPER", SECSItemFormat.A, Encoding.Default.GetByteCount(traceInfo.Dsper), traceInfo.Dsper);

                if (totsmpFormat == SECSItemFormat.A || totsmpFormat == SECSItemFormat.J)
                {
                    message.Body.Add("TOTSMP", totsmpFormat, Encoding.Default.GetByteCount("0"), "0");
                }
                else
                {
                    message.Body.Add("TOTSMP", totsmpFormat, 1, 0);
                }

                if (repgszFormat == SECSItemFormat.A || totsmpFormat == SECSItemFormat.J)
                {
                    message.Body.Add("REPGSZ", repgszFormat, Encoding.Default.GetByteCount(traceInfo.ReportGroupSize.ToString()), traceInfo.ReportGroupSize.ToString());
                }
                else
                {
                    message.Body.Add("REPGSZ", repgszFormat, 1, traceInfo.ReportGroupSize);
                }

                message.Body.Add("SVIDCOUNT", SECSItemFormat.L, traceInfo.Variables.Count, null);

                traceInfo.Variables.ForEach(t =>
                {
                    variableInfo = this.VariableCollection[t] as ExpandedVariableInfo;

                    if (svidFormat == SECSItemFormat.A || svidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add(variableInfo.Name, svidFormat, Encoding.Default.GetByteCount(t), t);
                    }
                    else
                    {
                        converted = ConvertValue(svidFormat, t);

                        if (converted != null)
                        {
                            message.Body.Add(variableInfo.Name, svidFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add(variableInfo.Name, svidFormat, 0, string.Empty);
                        }
                    }
                });

                driverResult = this._driver.SendSECSMessage(message);

                RaiseDriverLogAdded1(this, DriverLogType.INFO, string.Format("Result={0}, TRID={1}", driverResult, traceInfo.TraceID));
            }

            return driverResult;
        }
        #endregion
        #region SendS2F25
        public MessageError SendS2F25()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(2, 25, this._driver.Config.DeviceType);

                if (this.CurrentSetting.LoopbackDiagnostic == null || this.CurrentSetting.LoopbackDiagnostic.Count == 0)
                {
                    message.Body.Add("ABS", GetSECSFormat(DataDictinaryList.ABS, SECSItemFormat.B), 0, string.Empty);
                }
                else if (this.CurrentSetting.LoopbackDiagnostic.Count == 1)
                {
                    message.Body.Add("ABS", GetSECSFormat(DataDictinaryList.ABS, SECSItemFormat.B), 1, this.CurrentSetting.LoopbackDiagnostic[0]);
                }
                else
                {
                    message.Body.Add("ABS", GetSECSFormat(DataDictinaryList.ABS, SECSItemFormat.B), this.CurrentSetting.LoopbackDiagnostic.Count, this.CurrentSetting.LoopbackDiagnostic.ToArray());
                }

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS2F29
        public MessageError SendS2F29()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat ecidFormat;
            ExpandedVariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.ECID.ToString()];

                if (dataDictionaryInfo != null)
                {
                    ecidFormat = dataDictionaryInfo.Format.First();

                    message = this._driver.Messages.GetMessageHeader(2, 29, this._driver.Config.DeviceType);

                    if (this.VariableCollection.ECV.Items.Count == this.CurrentSetting.S2F29EquipmentConstantNamelist.Count)
                    {
                        message.Body.Add("ECIDCOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("ECIDCOUNT", SECSItemFormat.L, this.CurrentSetting.S2F29EquipmentConstantNamelist.Count, null);

                        this.CurrentSetting.S2F29EquipmentConstantNamelist.ForEach(t =>
                        {
                            variableInfo = this.VariableCollection[t] as ExpandedVariableInfo;

                            if (ecidFormat == SECSItemFormat.A || ecidFormat == SECSItemFormat.J)
                            {
                                message.Body.Add(variableInfo.Name, ecidFormat, Encoding.Default.GetByteCount(t), t);
                            }
                            else
                            {
                                converted = ConvertValue(ecidFormat, t);

                                if (converted != null)
                                {
                                    message.Body.Add(variableInfo.Name, ecidFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add(variableInfo.Name, ecidFormat, 0, string.Empty);
                                }
                            }
                        });
                    }

                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F31
        public MessageError SendS2F31()
        {
            SECSMessage message;
            ExpandedVariableInfo variableInfo;
            int length;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                variableInfo = this.VariableCollection.GetVariableInfo(VariableIDList.TimeFormat.ToString()) as ExpandedVariableInfo;

                if (variableInfo != null && int.TryParse(variableInfo.Value, out int timeFormat) == true)
                {
                    if (timeFormat == 0)
                    {
                        length = 12;
                    }
                    else
                    {
                        length = 16;
                    }
                }
                else
                {
                    length = 16;
                }

                message = this._driver.Messages.GetMessageHeader(2, 31, this._driver.Config.DeviceType);

                if (length == 12)
                {
                    message.Body.Add("TIMEDATE", GetSECSFormat(DataDictinaryList.TIME, SECSItemFormat.A), 12, DateTime.Now.ToString("yyMMddHHmmss"));
                }
                else
                {
                    message.Body.Add("TIMEDATE", GetSECSFormat(DataDictinaryList.TIME, SECSItemFormat.A), 16, DateTime.Now.ToString("yyyyMMddHHmmssff"));
                }

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS2F33
        public MessageError SendS2F33()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat dataIdFormat;
            SECSItemFormat reportIdFormat;
            SECSItemFormat svidFormat;
            MessageError driverResult;

            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.DATAID.ToString()];
                dataIdFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.RPTID.ToString()];
                reportIdFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.SVID.ToString()];
                svidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                message = this._driver.Messages.GetMessageHeader(2, 33, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("DATAID", dataIdFormat, 1, this._dataId);

                message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, this.ReportCollection.Items.Count, null);

                foreach (ReportInfo tempReportInfo in this.ReportCollection.Items.Values)
                {
                    message.Body.Add(SECSItemFormat.L, 2, null);

                    if (reportIdFormat == SECSItemFormat.A || reportIdFormat == SECSItemFormat.J)
                    {
                        message.Body.Add("RPTID", reportIdFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                    }
                    else
                    {
                        converted = ConvertValue(reportIdFormat, tempReportInfo.ReportID);

                        if (converted != null)
                        {
                            message.Body.Add("RPTID", reportIdFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add("RPTID", reportIdFormat, 0, string.Empty);
                        }
                    }

                    message.Body.Add("VIDCOUNT", SECSItemFormat.L, tempReportInfo.Variables.Items.Count, null);

                    foreach (VariableInfo tempVariableInfo in tempReportInfo.Variables.Items)
                    {
                        if (svidFormat == SECSItemFormat.A || svidFormat == SECSItemFormat.J)
                        {
                            message.Body.Add(tempVariableInfo.Name, svidFormat, Encoding.Default.GetByteCount(tempVariableInfo.VID), tempVariableInfo.VID);
                        }
                        else
                        {
                            converted = ConvertValue(svidFormat, tempVariableInfo.VID);

                            if (converted != null)
                            {
                                message.Body.Add(tempVariableInfo.Name, svidFormat, 1, converted);
                            }
                            else
                            {
                                message.Body.Add(tempVariableInfo.Name, svidFormat, 0, string.Empty);
                            }
                        }
                    }
                }

                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    this._lastS2F33EnableSystemByte = message.SystemBytes;
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F33Disable
        public MessageError SendS2F33Disable()
        {
            SECSMessage message;
            MessageError driverResult;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat dataIdFormat;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(2, 33, this._driver.Config.DeviceType);

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.DATAID.ToString()];
                dataIdFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("DATAID", dataIdFormat, 1, this._dataId);
                message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, 0, null);

                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    this._lastS2F33DisableSystemByte = message.SystemBytes;
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F35
        public MessageError SendS2F35()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat dataIdFormat;
            SECSItemFormat ceidFormat;
            SECSItemFormat reportIdFormat;
            MessageError driverResult;

            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.DATAID.ToString()];
                dataIdFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.CEID.ToString()];
                ceidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.RPTID.ToString()];
                reportIdFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                message = this._driver.Messages.GetMessageHeader(2, 35, this._driver.Config.DeviceType);

                var varCollectionEvent = from CollectionEventInfo tempCollectionEventInfo in this.CollectionEventCollection.Items.Values
                                         where tempCollectionEventInfo.IsUse == true &&
                                               tempCollectionEventInfo.Enabled == true
                                         select tempCollectionEventInfo;

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("DATAID", dataIdFormat, 1, this._dataId);
                message.Body.Add("CEIDCOUNT", SECSItemFormat.L, varCollectionEvent.Count(), null);

                foreach (CollectionEventInfo tempCollectionEventInfo in varCollectionEvent)
                {
                    message.Body.Add(SECSItemFormat.L, 2, null);

                    if (ceidFormat == SECSItemFormat.A || ceidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, Encoding.Default.GetByteCount(tempCollectionEventInfo.CEID), tempCollectionEventInfo.CEID);
                    }
                    else
                    {
                        converted = ConvertValue(ceidFormat, tempCollectionEventInfo.CEID);

                        if (converted != null)
                        {
                            message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, 0, string.Empty);
                        }
                    }

                    message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, tempCollectionEventInfo.Reports.Items.Count, null);

                    foreach (ReportInfo tempReportInfoin in tempCollectionEventInfo.Reports.Items.Values)
                    {
                        if (reportIdFormat == SECSItemFormat.A || reportIdFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("RPTID", reportIdFormat, Encoding.Default.GetByteCount(tempReportInfoin.ReportID), tempReportInfoin.ReportID);
                        }
                        else
                        {
                            converted = ConvertValue(reportIdFormat, tempReportInfoin.ReportID);

                            if (converted != null)
                            {
                                message.Body.Add("RPTID", reportIdFormat, 1, converted);
                            }
                            else
                            {
                                message.Body.Add("RPTID", reportIdFormat, 0, string.Empty);
                            }
                        }
                    }
                }

                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    this._lastS2F35EnableSystemByte = message.SystemBytes;
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F35Disable
        public MessageError SendS2F35Disable()
        {
            SECSMessage message;
            MessageError driverResult;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat dataIdFormat;
            SECSItemFormat ceidFormat;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.DATAID.ToString()];
                dataIdFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;
                ceidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                message = this._driver.Messages.GetMessageHeader(2, 35, this._driver.Config.DeviceType);

                var varCollectionEvent = from CollectionEventInfo tempCollectionEventInfo in this.CollectionEventCollection.Items.Values
                                         where tempCollectionEventInfo.IsUse == true &&
                                               tempCollectionEventInfo.Enabled == true
                                         select tempCollectionEventInfo;

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("DATAID", dataIdFormat, 1, this._dataId);
                message.Body.Add("CEIDCOUNT", SECSItemFormat.L, varCollectionEvent.Count(), null);

                foreach (CollectionEventInfo tempCollectionEventInfo in varCollectionEvent)
                {
                    message.Body.Add(SECSItemFormat.L, 2, null);

                    if (ceidFormat == SECSItemFormat.A || ceidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, Encoding.Default.GetByteCount(tempCollectionEventInfo.CEID), tempCollectionEventInfo.CEID);
                    }
                    else
                    {
                        converted = ConvertValue(ceidFormat, tempCollectionEventInfo.CEID);

                        if (converted != null)
                        {
                            message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, 0, string.Empty);
                        }
                    }

                    message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, 0, null);
                }
                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    this._lastS2F35DisableSystemByte = message.SystemBytes;
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F37Disable
        public MessageError SendS2F37Disable()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(2, 37, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("CEED", SECSItemFormat.Boolean, 1, false);
                message.Body.Add("CEIDCOUNT", SECSItemFormat.L, 0, null);

                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    this._lastS2F37DisableSystemByte = message.SystemBytes;
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F37Enable
        public MessageError SendS2F37Enable()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat ceidFormat;
            MessageError driverResult;

            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.CEID.ToString()];
                ceidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                message = this._driver.Messages.GetMessageHeader(2, 37, this._driver.Config.DeviceType);

                var varCollectionEvent = from CollectionEventInfo tempCollectionEventInfo in this.CollectionEventCollection.Items.Values
                                         where tempCollectionEventInfo.IsUse == true &&
                                               tempCollectionEventInfo.Enabled == true
                                         select tempCollectionEventInfo;

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("CEED", SECSItemFormat.Boolean, 1, true);
                message.Body.Add("CEIDCOUNT", SECSItemFormat.L, varCollectionEvent.Count(), null);

                foreach (CollectionEventInfo tempCollectionEventInfo in varCollectionEvent)
                {
                    if (ceidFormat == SECSItemFormat.A || ceidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, Encoding.Default.GetByteCount(tempCollectionEventInfo.CEID), tempCollectionEventInfo.CEID);
                    }
                    else
                    {
                        converted = ConvertValue(ceidFormat, tempCollectionEventInfo.CEID);

                        if (converted != null)
                        {
                            message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add(tempCollectionEventInfo.Name, ceidFormat, 0, string.Empty);
                        }
                    }
                }

                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    this._lastS2F37EnableSystemByte = message.SystemBytes;
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F41
        public MessageError SendS2F41(ExpandedRemoteCommandInfo expandedRmoteCommandInfo, ExpandedRemoteCommandValueSetInfo valueSet)
        {
            SECSMessage message;
            dynamic converted;
            SECSItemFormat cmdFormat;
            SECSItemFormat cpNameFormat;

            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (expandedRmoteCommandInfo != null)
                {
                    message = this._driver.Messages.GetMessageHeader(2, 41, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    cmdFormat = GetSECSFormat(DataDictinaryList.RCMD, SECSItemFormat.A);
                    cpNameFormat = GetSECSFormat(DataDictinaryList.CPNAME, SECSItemFormat.A);

                    if (cmdFormat == SECSItemFormat.A || cmdFormat == SECSItemFormat.J)
                    {
                        message.Body.Add("RCMD", cmdFormat, Encoding.Default.GetByteCount(expandedRmoteCommandInfo.RemoteCommand), expandedRmoteCommandInfo.RemoteCommand);
                    }
                    else
                    {
                        converted = ConvertValue(cmdFormat, expandedRmoteCommandInfo.RemoteCommand);

                        if (converted != null)
                        {
                            message.Body.Add("RCMD", cmdFormat, 1, converted);
                        }
                        else
                        {
                            if (expandedRmoteCommandInfo.RemoteCommand == "")
                            {
                                message.Body.Add("RCMD", cmdFormat, 0, string.Empty);
                            }
                            else
                            {
                                driverResult = MessageError.InvalidMessageStructure;
                            }
                        }
                    }

                    if(driverResult == MessageError.Ok)
                    {
                        message.Body.Add("CPCOUNT", SECSItemFormat.L, valueSet.ParameterItems.Count, null);

                        foreach (ExpandedRemoteCommandParameterInfo tempCommandParameterInfo in valueSet.ParameterItems)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            if (cpNameFormat == SECSItemFormat.A || cpNameFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("CPNAME", cpNameFormat, Encoding.Default.GetByteCount(tempCommandParameterInfo.Name), tempCommandParameterInfo.Name);
                            }
                            else
                            {
                                converted = ConvertValue(cpNameFormat, tempCommandParameterInfo.Name);

                                if (converted != null)
                                {
                                    message.Body.Add("CPNAME", cpNameFormat, 1, converted);
                                }
                                else
                                {
                                    if (tempCommandParameterInfo.Name == string.Empty)
                                    {
                                        message.Body.Add("CPNAME", cpNameFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        driverResult = MessageError.InvalidMessageStructure;
                                    }
                                }
                            }

                            if (driverResult == MessageError.Ok)
                            {
                                if (string.IsNullOrEmpty(tempCommandParameterInfo.Value) == true && string.IsNullOrEmpty(tempCommandParameterInfo.GenerateRule) == false)
                                {
                                    if (tempCommandParameterInfo.Format == SECSItemFormat.A || tempCommandParameterInfo.Format == SECSItemFormat.J)
                                    {
                                        converted = GenerateValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.GenerateRule, 0);
                                        message.Body.Add("CPVAL", tempCommandParameterInfo.Format, Encoding.Default.GetByteCount(converted.ToString()), converted.ToString());
                                    }
                                    else
                                    {
                                        if (tempCommandParameterInfo.Count == 0)
                                        {
                                            message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                        }
                                        else if (tempCommandParameterInfo.Count == 1)
                                        {
                                            converted = GenerateValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.GenerateRule, 0);

                                            if (converted != null)
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 1, converted);
                                            }
                                            else
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                            }
                                        }
                                        else
                                        {
                                            converted = GenerateList(tempCommandParameterInfo.Format, tempCommandParameterInfo.Count, tempCommandParameterInfo.GenerateRule);

                                            if (converted != null)
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, converted.Count, converted.ToArray());
                                            }
                                            else
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (tempCommandParameterInfo.Format == SECSItemFormat.A || tempCommandParameterInfo.Format == SECSItemFormat.J)
                                    {
                                        message.Body.Add("CPVAL", tempCommandParameterInfo.Format, Encoding.Default.GetByteCount(tempCommandParameterInfo.Value), tempCommandParameterInfo.Value);
                                    }
                                    else
                                    {
                                        if (tempCommandParameterInfo.Count == 0)
                                        {
                                            message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                        }
                                        else if (tempCommandParameterInfo.Count == 1)
                                        {
                                            converted = ConvertValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.Value);

                                            if (converted != null)
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 1, converted);
                                            }
                                            else
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                            }
                                        }
                                        else
                                        {
                                            converted = ConvertValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.Count, tempCommandParameterInfo.Value);

                                            if (converted != null)
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, converted.Count, converted.ToArray());
                                            }
                                            else
                                            {
                                                message.Body.Add("CPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (driverResult == MessageError.Ok)
                        {
                            driverResult = this._driver.SendSECSMessage(message);
                        }
                    }

                }
            }

            return driverResult;
        }
        #endregion
        #region SendS2F43
        public MessageError SendS2F43()
        {
            SECSMessage message;
            Dictionary<string, List<string>> selected;
            string[] splitData;
            SECSItemFormat stridFormat;
            SECSItemFormat fcnidFormat;
            MessageError driverResult;
            byte convertedStream;
            byte convertedFunction;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                selected = new Dictionary<string, List<string>>();

                foreach (string tempItem in this.CurrentSetting.S2F43ResetSpoolingStreamsAndFunctions)
                {
                    splitData = tempItem.Split('S', 'F');

                    if (splitData != null && splitData.Length >= 2)
                    {
                        if (selected.ContainsKey(splitData[1]) == false)
                        {
                            selected[splitData[1]] = new List<string>();
                        }

                        selected[splitData[1]].Add(splitData[2]);
                    }
                }

                message = this._driver.Messages.GetMessageHeader(2, 43, this._driver.Config.DeviceType);

                if (selected.Count > 0)
                {
                    stridFormat = GetSECSFormat(DataDictinaryList.STRID, SECSItemFormat.U1);
                    fcnidFormat = GetSECSFormat(DataDictinaryList.FCNID, SECSItemFormat.U1);

                    message.Body.Add("STRIDCOUNT", SECSItemFormat.L, selected.Count, null);

                    foreach (KeyValuePair<string, List<string>> tempStream in selected.OrderBy(t => t.Key))
                    {
                        message.Body.Add(SECSItemFormat.L, 2, null);

                        convertedStream = byte.Parse(tempStream.Key);

                        message.Body.Add("STRID", stridFormat, 1, convertedStream);

                        message.Body.Add("FCNIDCOUNT", SECSItemFormat.L, tempStream.Value.Count, null);

                        foreach (string tempFunction in tempStream.Value.OrderBy(t => t))
                        {
                            convertedFunction = byte.Parse(tempFunction);
                            message.Body.Add("FCNID", fcnidFormat, 1, convertedFunction);
                        }
                    }
                }
                else
                {
                    message.Body.Add("STRIDCOUNT", SECSItemFormat.L, 0, null);
                }

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS2F45
        public MessageError SendS2F45()
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;

            SECSItemFormat vidFormat;
            SECSItemFormat limitidFormat;
            SECSItemFormat upperFormat;
            SECSItemFormat lowerFormat;
            VariableInfo variableInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.VID.ToString()];
                vidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.LIMITID.ToString()];
                limitidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.UPPERDB.ToString()];
                upperFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.A;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.LOWERDB.ToString()];
                lowerFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.A;

                message = this._driver.Messages.GetMessageHeader(2, 45, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("DATAID", GetSECSFormat(DataDictinaryList.DATAID, SECSItemFormat.U4), 1, this._dataId);

                message.Body.Add("VARCOUNT", SECSItemFormat.L, this.LimitMonitoringCollection.Items.Count, null);

                foreach (ExpandedLimitMonitoringInfo tempLimitMonitoringInfo in this.LimitMonitoringCollection.Items)
                {
                    variableInfo = this.VariableCollection[tempLimitMonitoringInfo.Variable.VID];

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    if (vidFormat == SECSItemFormat.A || vidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add(variableInfo.Name, vidFormat, Encoding.Default.GetByteCount(tempLimitMonitoringInfo.Variable.VID), tempLimitMonitoringInfo.Variable.VID);
                    }
                    else
                    {
                        converted = ConvertValue(vidFormat, tempLimitMonitoringInfo.Variable.VID);

                        if (converted != null)
                        {
                            message.Body.Add(variableInfo.Name, vidFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add(variableInfo.Name, vidFormat, 0, string.Empty);
                        }
                    }

                    message.Body.Add("LIMITCOUNT", SECSItemFormat.L, tempLimitMonitoringInfo.Items.Count, null);

                    foreach (ExpandedLimitMonitoringItem tempLimitMonitoringItem in tempLimitMonitoringInfo.Items)
                    {
                        message.Body.Add(SECSItemFormat.L, 2, null);

                        if (limitidFormat == SECSItemFormat.A || limitidFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("LIMITID", limitidFormat, Encoding.Default.GetByteCount(tempLimitMonitoringItem.LimitID.ToString()), tempLimitMonitoringItem.LimitID.ToString());
                        }
                        else
                        {
                            converted = ConvertValue(limitidFormat, tempLimitMonitoringItem.LimitID.ToString());

                            if (converted != null)
                            {
                                message.Body.Add("LIMITID", limitidFormat, 1, converted);
                            }
                            else
                            {
                                message.Body.Add("LIMITID", limitidFormat, 0, string.Empty);
                            }
                        }

                        if (string.IsNullOrEmpty(tempLimitMonitoringItem.Upper) == false || string.IsNullOrEmpty(tempLimitMonitoringItem.Lower) == false)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            if (upperFormat == SECSItemFormat.A || upperFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("UPPERDB", upperFormat, Encoding.Default.GetByteCount(tempLimitMonitoringItem.Upper), tempLimitMonitoringItem.Upper);
                            }
                            else
                            {
                                converted = ConvertValue(upperFormat, tempLimitMonitoringItem.Upper);

                                if (converted != null)
                                {
                                    message.Body.Add("UPPERDB", upperFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add("UPPERDB", upperFormat, 0, string.Empty);
                                }
                            }

                            if (lowerFormat == SECSItemFormat.A || lowerFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("LOWERDB", lowerFormat, Encoding.Default.GetByteCount(tempLimitMonitoringItem.Lower), tempLimitMonitoringItem.Lower);
                            }
                            else
                            {
                                converted = ConvertValue(lowerFormat, tempLimitMonitoringItem.Lower);

                                if (converted != null)
                                {
                                    message.Body.Add("LOWERDB", lowerFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add("LOWERDB", lowerFormat, 0, string.Empty);
                                }
                            }
                        }
                        else
                        {
                            message.Body.Add(SECSItemFormat.L, 0, null);
                        }
                    }
                }

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS2F45
        public MessageError SendS2F45(ExpandedLimitMonitoringInfo monitoringInfo)
        {
            SECSMessage message;
            DataDictionaryInfo dataDictionaryInfo;

            SECSItemFormat vidFormat;
            SECSItemFormat limitidFormat;
            SECSItemFormat upperFormat;
            SECSItemFormat lowerFormat;
            VariableInfo variableInfo;
            MessageError driverResult;

            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.VID.ToString()];
                vidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.LIMITID.ToString()];
                limitidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.B;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.UPPERDB.ToString()];
                upperFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.A;

                dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.LOWERDB.ToString()];
                lowerFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.A;

                message = this._driver.Messages.GetMessageHeader(2, 45, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("DATAID", GetSECSFormat(DataDictinaryList.DATAID, SECSItemFormat.U4), 1, this._dataId);

                message.Body.Add("VARCOUNT", SECSItemFormat.L, 1, null);

                variableInfo = this.VariableCollection[monitoringInfo.Variable.VID];

                message.Body.Add(SECSItemFormat.L, 2, null);

                converted = ConvertValue(vidFormat, monitoringInfo.Variable.VID);

                if (converted != null)
                {
                    message.Body.Add(variableInfo.Name, vidFormat, 1, converted);
                }
                else
                {
                    message.Body.Add(variableInfo.Name, vidFormat, 0, string.Empty);
                }

                message.Body.Add("LIMITCOUNT", SECSItemFormat.L, monitoringInfo.Items.Count, null);

                foreach (ExpandedLimitMonitoringItem tempLimitMonitoringItem in monitoringInfo.Items)
                {
                    message.Body.Add(SECSItemFormat.L, 2, null);

                    if (limitidFormat == SECSItemFormat.A || limitidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add("LIMITID", limitidFormat, Encoding.Default.GetByteCount(tempLimitMonitoringItem.LimitID.ToString()), tempLimitMonitoringItem.LimitID.ToString());
                    }
                    else
                    {
                        converted = ConvertValue(limitidFormat, tempLimitMonitoringItem.LimitID.ToString());

                        if (converted != null)
                        {
                            message.Body.Add("LIMITID", limitidFormat, 1, converted);
                        }
                        else
                        {
                            message.Body.Add("LIMITID", limitidFormat, 0, string.Empty);
                        }
                    }

                    if (string.IsNullOrEmpty(tempLimitMonitoringItem.Upper) == false || string.IsNullOrEmpty(tempLimitMonitoringItem.Lower) == false)
                    {
                        message.Body.Add(SECSItemFormat.L, 2, null);

                        if (upperFormat == SECSItemFormat.A || upperFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("UPPERDB", upperFormat, Encoding.Default.GetByteCount(tempLimitMonitoringItem.Upper), tempLimitMonitoringItem.Upper);
                        }
                        else
                        {
                            converted = ConvertValue(upperFormat, tempLimitMonitoringItem.Upper);

                            if (converted != null)
                            {
                                message.Body.Add("UPPERDB", upperFormat, 1, converted);
                            }
                            else
                            {
                                message.Body.Add("UPPERDB", upperFormat, 0, string.Empty);
                            }
                        }

                        if (lowerFormat == SECSItemFormat.A || lowerFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("LOWERDB", lowerFormat, Encoding.Default.GetByteCount(tempLimitMonitoringItem.Lower), tempLimitMonitoringItem.Lower);
                        }
                        else
                        {
                            converted = ConvertValue(lowerFormat, tempLimitMonitoringItem.Lower);

                            if (converted != null)
                            {
                                message.Body.Add("LOWERDB", lowerFormat, 1, converted);
                            }
                            else
                            {
                                message.Body.Add("LOWERDB", lowerFormat, 0, string.Empty);
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add(SECSItemFormat.L, 0, null);
                    }
                }

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS2F49
        public MessageError SendS2F49(ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo, ExpandedEnhancedRemoteCommandValueSetInfo valueSet)
        {
            SECSMessage message;
            dynamic converted;
            SECSItemFormat dataIDFormat;
            SECSItemFormat objSpecFormat;
            SECSItemFormat cmdFormat;
            SECSItemFormat cpNameFormat;
            MessageError driverResult;

            bool useNameValuePair;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                dataIDFormat = GetSECSFormat(DataDictinaryList.DATAID, SECSItemFormat.U4);
                objSpecFormat = GetSECSFormat(DataDictinaryList.OBJSPEC, SECSItemFormat.A);

                if (expandedEnhancedRemoteCommandInfo != null )
                {
                    message = this._driver.Messages.GetMessageHeader(2, 49, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 4, null);

                    converted = ConvertValue(dataIDFormat, expandedEnhancedRemoteCommandInfo.DataID);

                    if (converted != null)
                    {
                        message.Body.Add("DATAID", dataIDFormat, 1, converted);
                    }
                    else
                    {
                        if (expandedEnhancedRemoteCommandInfo.DataID == string.Empty)
                        {
                            message.Body.Add("DATAID", dataIDFormat, 0, string.Empty);
                        }
                        else
                        {
                            driverResult = MessageError.InvalidMessageStructure;
                        }                        
                    }

                    if (driverResult == MessageError.Ok)
                    {
                        if (expandedEnhancedRemoteCommandInfo.ObjSpec != null)
                        {
                            if (objSpecFormat == SECSItemFormat.A || objSpecFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("OBJSPEC", objSpecFormat, Encoding.Default.GetByteCount(expandedEnhancedRemoteCommandInfo.ObjSpec), expandedEnhancedRemoteCommandInfo.ObjSpec);
                            }
                            else
                            {
                                converted = ConvertValue(objSpecFormat, expandedEnhancedRemoteCommandInfo.ObjSpec);

                                if (converted != null)
                                {
                                    message.Body.Add("OBJSPEC", objSpecFormat, 1, converted);
                                }
                                else
                                {
                                    if (expandedEnhancedRemoteCommandInfo.ObjSpec == string.Empty)
                                    {
                                        message.Body.Add("OBJSPEC", objSpecFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        driverResult = MessageError.InvalidMessageStructure;
                                    }
                                }
                            }
                        }
                        else
                        {
                            message.Body.Add("OBJSPEC", objSpecFormat, 0, string.Empty);
                        }
                    }

                    if (driverResult == MessageError.Ok)
                    {
                        cmdFormat = GetSECSFormat(DataDictinaryList.RCMD, SECSItemFormat.A);

                        if (cmdFormat == SECSItemFormat.A || cmdFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("RCMD", cmdFormat, Encoding.Default.GetByteCount(expandedEnhancedRemoteCommandInfo.RemoteCommand), expandedEnhancedRemoteCommandInfo.RemoteCommand);
                        }
                        else
                        {
                            converted = ConvertValue(cmdFormat, expandedEnhancedRemoteCommandInfo.RemoteCommand);

                            if (converted != null)
                            {
                                message.Body.Add("RCMD", cmdFormat, 1, converted);
                            }
                            else
                            {
                                if (expandedEnhancedRemoteCommandInfo.RemoteCommand == string.Empty)
                                {
                                    message.Body.Add("RCMD", cmdFormat, 0, string.Empty);
                                }
                                else
                                {
                                    driverResult = MessageError.InvalidMessageStructure;
                                }
                            }
                        }
                    }

                    if (driverResult == MessageError.Ok)
                    {
                        cpNameFormat = GetSECSFormat(DataDictinaryList.CPNAME, SECSItemFormat.A);
                        message.Body.Add("CPCOUNT", SECSItemFormat.L, valueSet.ParameterItems.Count, null);

                        foreach (ExpandedEnhancedRemoteCommandParameterInfo tempCommandParameterInfo in valueSet.ParameterItems)
                        {
                            useNameValuePair = true;

                            if (string.IsNullOrEmpty(tempCommandParameterInfo.Name) == true && tempCommandParameterInfo.Format != SECSItemFormat.L)
                            {
                                useNameValuePair = false;
                            }

                            if (useNameValuePair == true)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (cpNameFormat == SECSItemFormat.A || cpNameFormat == SECSItemFormat.J)
                                {
                                    message.Body.Add("CPNAME", cpNameFormat, Encoding.Default.GetByteCount(tempCommandParameterInfo.Name), tempCommandParameterInfo.Name);
                                }
                                else
                                {
                                    converted = ConvertValue(cpNameFormat, tempCommandParameterInfo.Name);

                                    if (converted != null)
                                    {
                                        message.Body.Add("CPNAME", cpNameFormat, 1, converted);
                                    }
                                    else
                                    {
                                        if (tempCommandParameterInfo.Name == string.Empty)
                                        {
                                            message.Body.Add("CPNAME", cpNameFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            driverResult = MessageError.InvalidMessageStructure;
                                        }
                                    }
                                }
                            }

                            if (driverResult == MessageError.Ok)
                            {
                                if (tempCommandParameterInfo.Format == SECSItemFormat.L)
                                {
                                    #region tempCommandParameterInfo.Format L
                                    if (tempCommandParameterInfo.ValueItems.Count == 0 || tempCommandParameterInfo.UseChildLength == false)
                                    {
                                        message.Body.Add(SECSItemFormat.L, 0, null);
                                    }
                                    else
                                    {
                                        message.Body.Add(SECSItemFormat.L, tempCommandParameterInfo.ValueItems.Count, null);

                                        foreach (ExpandedEnhancedRemoteCommandParameterItem tempValueItem in tempCommandParameterInfo.ValueItems)
                                        {
                                            useNameValuePair = true;

                                            if (string.IsNullOrEmpty(tempValueItem.Name) == true && tempValueItem.Format != SECSItemFormat.L)
                                            {
                                                useNameValuePair = false;
                                            }

                                            if (useNameValuePair == true)
                                            {

                                                message.Body.Add(SECSItemFormat.L, 2, null);

                                                if (cpNameFormat == SECSItemFormat.A || cpNameFormat == SECSItemFormat.J)
                                                {
                                                    message.Body.Add("CPNAME", cpNameFormat, Encoding.Default.GetByteCount(tempValueItem.Name), tempValueItem.Name);
                                                }
                                                else
                                                {
                                                    converted = ConvertValue(cpNameFormat, tempValueItem.Name);

                                                    if (converted != null)
                                                    {
                                                        message.Body.Add("CPNAME", cpNameFormat, 1, converted);
                                                    }
                                                    else
                                                    {
                                                        if (tempValueItem.Name == string.Empty)
                                                        {
                                                            message.Body.Add("CPNAME", cpNameFormat, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            driverResult = MessageError.InvalidMessageStructure;
                                                        }
                                                    }
                                                }
                                            }

                                            if (MakeS2F49Child(message, tempValueItem) == false)
                                            {
                                                driverResult = MessageError.InvalidMessageStructure;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region tempCommandParameterInfo.Format != L
                                    if (string.IsNullOrEmpty(tempCommandParameterInfo.Value) == true && string.IsNullOrEmpty(tempCommandParameterInfo.GenerateRule) == false)
                                    {
                                        if (tempCommandParameterInfo.Format == SECSItemFormat.A || tempCommandParameterInfo.Format == SECSItemFormat.J)
                                        {
                                            converted = GenerateValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.GenerateRule, 0);

                                            if (converted != null)
                                            {
                                                message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, Encoding.Default.GetByteCount(converted.ToString()), converted.ToString());
                                            }
                                            else
                                            {
                                                message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                            }
                                        }
                                        else
                                        {
                                            if (tempCommandParameterInfo.Count == 0)
                                            {
                                                message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                            }
                                            else if (tempCommandParameterInfo.Count == 1)
                                            {
                                                converted = GenerateValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.GenerateRule, 0);

                                                if (converted != null)
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 1, converted);
                                                }
                                                else
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                                }
                                            }
                                            else
                                            {
                                                converted = GenerateList(tempCommandParameterInfo.Format, tempCommandParameterInfo.Count, tempCommandParameterInfo.GenerateRule);

                                                if (converted != null)
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, converted.Count, converted.ToArray());
                                                }
                                                else
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (tempCommandParameterInfo.Format == SECSItemFormat.A || tempCommandParameterInfo.Format == SECSItemFormat.J)
                                        {
                                            message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, Encoding.Default.GetByteCount(tempCommandParameterInfo.Value), tempCommandParameterInfo.Value);
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(tempCommandParameterInfo.Value) == true)
                                            {
                                                message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                            }
                                            else if (tempCommandParameterInfo.Count == 1)
                                            {
                                                converted = ConvertValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.Value);

                                                if (converted != null)
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 1, converted);
                                                }
                                                else
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(tempCommandParameterInfo.Format, tempCommandParameterInfo.Count, tempCommandParameterInfo.Value);

                                                if (converted != null)
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, converted.Count, converted.ToArray());
                                                }
                                                else
                                                {
                                                    message.Body.Add("CEPVAL", tempCommandParameterInfo.Format, 0, string.Empty);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }

                    if (driverResult == MessageError.Ok)
                    {
                        driverResult = this._driver.SendSECSMessage(message);
                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS5F3
        public void SendS5F3()
        {
            SECSMessage message;
            SECSItemFormat aledFormat;
            SECSItemFormat alidFormat;
            MessageError driverResult;

            bool enable;

            if (this._driver.Connected == true)
            {
                aledFormat = GetSECSFormat(DataDictinaryList.ALED, SECSItemFormat.Boolean);
                alidFormat = GetSECSFormat(DataDictinaryList.ALID, SECSItemFormat.B);

                int alarmTotalCount = this.AlarmCollection.Items.Count;
                int selectedTotalCount = this.CurrentSetting.S5F3SelectedAlarmSend.Count;
                int enabledTotalCount = this.CurrentSetting.S5F3EnabledAlarmSend.Count;

                if (alarmTotalCount == selectedTotalCount && enabledTotalCount == 0)
                {
                    // all disable
                    message = this._driver.Messages.GetMessageHeader(5, 3, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    message.Body.Add("ALED", SECSItemFormat.B, 1, 0);

                    message.Body.Add("ALID", alidFormat, 0, string.Empty);

                    driverResult = this._driver.SendSECSMessage(message);

                    OnDriverLogAdded1.BeginInvoke(null, DriverLogType.SEND, string.Format("Result={0}, All alarm disabled", driverResult), null, null);
                }
                else if (alarmTotalCount == selectedTotalCount && alarmTotalCount == enabledTotalCount)
                {
                    // all enable
                    message = this._driver.Messages.GetMessageHeader(5, 3, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    message.Body.Add("ALED", SECSItemFormat.B, 1, 0x80);

                    message.Body.Add("ALID", alidFormat, 0, string.Empty);

                    driverResult = this._driver.SendSECSMessage(message);

                    OnDriverLogAdded1.BeginInvoke(null, DriverLogType.SEND, string.Format("Result={0}, All alarm enabled", driverResult), null, null);
                }
                else
                {
                    // individual enable
                    foreach (long alarmID in this.CurrentSetting.S5F3SelectedAlarmSend)
                    {
                        message = this._driver.Messages.GetMessageHeader(5, 3, this._driver.Config.DeviceType);

                        enable = this.CurrentSetting.S5F3EnabledAlarmSend.Contains(alarmID);

                        message.Body.Add(SECSItemFormat.L, 2, null);

                        if (enable == true)
                        {
                            message.Body.Add("ALED", SECSItemFormat.B, 1, 0x80);
                        }
                        else
                        {
                            message.Body.Add("ALED", SECSItemFormat.B, 1, 0);
                        }

                        message.Body.Add("ALID", alidFormat, 1, alarmID);

                        driverResult = this._driver.SendSECSMessage(message);
                        OnDriverLogAdded1.BeginInvoke(null, DriverLogType.SEND, string.Format("Result={0}, ALID={1}, enabled={2}", driverResult, alarmID, enable), null, null);

                        if (driverResult != MessageError.Ok)
                        {
                            break;
                        }
                    }
                }
            }
        }
        #endregion
        #region SendS5F3EnableDisable
        public void SendS5F3(bool enable)
        {
            SECSMessage message;
            SECSItemFormat aledFormat;
            SECSItemFormat alidFormat;
            MessageError driverResult;

            if (this._driver.Connected == true)
            {
                aledFormat = GetSECSFormat(DataDictinaryList.ALED, SECSItemFormat.B);
                alidFormat = GetSECSFormat(DataDictinaryList.ALID, SECSItemFormat.B);

                // all disable
                message = this._driver.Messages.GetMessageHeader(5, 3, this._driver.Config.DeviceType);
                message.Body.Add(SECSItemFormat.L, 2, null);

                if (enable == true)
                {
                    message.Body.Add("ALED", SECSItemFormat.B, 1, 0x80);
                }
                else
                {
                    message.Body.Add("ALED", SECSItemFormat.B, 1, 0);
                }

                message.Body.Add("ALID", alidFormat, 0, string.Empty);
                driverResult = this._driver.SendSECSMessage(message);

                if (driverResult == MessageError.Ok)
                {
                    if (enable == false)
                    {
                        OnDriverLogAdded1.BeginInvoke(null, DriverLogType.SEND, string.Format("Result={0}, All alarm disabled", driverResult), null, null);
                        this._lastS5F3DisableSystemByte = message.SystemBytes;
                    }
                    else
                    {
                        OnDriverLogAdded1.BeginInvoke(null, DriverLogType.SEND, string.Format("Result={0}, All alarm enabled", driverResult), null, null);
                        this._lastS5F3EnableSystemByte = message.SystemBytes;
                    }
                }
            }
        }
        #endregion
        #region SendS5F5
        public MessageError SendS5F5()
        {
            SECSMessage message;
            SECSItemFormat alidFormat;
            MessageError driverResult;

            dynamic value;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                alidFormat = GetSECSFormat(DataDictinaryList.ALID, SECSItemFormat.B);

                message = this._driver.Messages.GetMessageHeader(5, 5, this._driver.Config.DeviceType);

                if (this.CurrentSetting.S5F5ListAlarmsRequest == null || this.CurrentSetting.S5F5ListAlarmsRequest.Count == 0)
                {
                    message.Body.Add("ALID", alidFormat, 0, "");
                }
                else
                {
                    value = ConvertValue(alidFormat, this.CurrentSetting.S5F5ListAlarmsRequest.Count, string.Join(" ", this.CurrentSetting.S5F5ListAlarmsRequest));
                    if (this.CurrentSetting.S5F5ListAlarmsRequest.Count == 1)
                    {
                        message.Body.Add("ALID", alidFormat, this.CurrentSetting.S5F5ListAlarmsRequest.Count, value);
                    }
                    else
                    {
                        message.Body.Add("ALID", alidFormat, this.CurrentSetting.S5F5ListAlarmsRequest.Count, value.ToArray());
                    }
                }

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS5F7
        public MessageError SendS5F7()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(5, 7, this._driver.Config.DeviceType);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS6F15
        public MessageError SendS6F15(string collectionEventID)
        {
            SECSMessage message;
            SECSItemFormat ceidFormat;
            string[] splitData;
            string ceid;
            CollectionEventInfo collectionEventInfo;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(collectionEventID) == false)
                {
                    splitData = collectionEventID.Split(':');

                    if (splitData != null && splitData.Length > 0)
                    {
                        ceid = splitData[0].Trim();
                        collectionEventInfo = this.CollectionEventCollection[ceid];
                        ceidFormat = GetSECSFormat(DataDictinaryList.CEID, SECSItemFormat.U2);

                        if (collectionEventInfo != null)
                        {
                            message = this._driver.Messages.GetMessageHeader(6, 15, this._driver.Config.DeviceType);

                            if (ceidFormat == SECSItemFormat.A || ceidFormat == SECSItemFormat.J)
                            {
                                message.Body.Add(collectionEventInfo.Name, ceidFormat, Encoding.Default.GetByteCount(ceid), ceid);
                            }
                            else
                            {
                                converted = ConvertValue(ceidFormat, ceid);

                                if (converted != null)
                                {
                                    message.Body.Add(collectionEventInfo.Name, ceidFormat, 1, converted);
                                }
                                else
                                {
                                    message.Body.Add(collectionEventInfo.Name, ceidFormat, 0, string.Empty);
                                }
                            }

                            driverResult = this._driver.SendSECSMessage(message);
                        }
                        else
                        {
                            driverResult = MessageError.DataIsNull;
                            RaiseDriverLogAdded1(this, DriverLogType.INFO, string.Format("Collection Event ID: {0} can not find(disabled or not exist)", ceid));
                        }
                    }
                    else
                    {
                        driverResult = MessageError.DataIsNull;
                        RaiseDriverLogAdded1(this, DriverLogType.INFO, "Collection Event ID is wrong");
                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS6F19
        public MessageError SendS6F19(string report)
        {
            SECSMessage message;
            SECSItemFormat reportIdFormat;
            string[] splitData;
            string reportId;
            MessageError driverResult;
            dynamic converted;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(report) == false)
                {
                    splitData = report.Split(':');

                    if (splitData != null && splitData.Length > 0 && splitData[0].Length > 0)
                    {
                        reportId = splitData[0].Trim();
                        reportIdFormat = GetSECSFormat(DataDictinaryList.RPTID, SECSItemFormat.U2);

                        message = this._driver.Messages.GetMessageHeader(6, 19, this._driver.Config.DeviceType);

                        if (reportIdFormat == SECSItemFormat.A || reportIdFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("RPTID", reportIdFormat, Encoding.Default.GetByteCount(reportId), reportId);
                        }
                        else
                        {
                            converted = ConvertValue(reportIdFormat, reportId);

                            if (converted != null)
                            {
                                message.Body.Add("RPTID", reportIdFormat, 1, converted);
                            }
                            else
                            {
                                message.Body.Add("RPTID", reportIdFormat, 0, string.Empty);
                            }
                        }

                        driverResult = this._driver.SendSECSMessage(message);
                    }
                    else
                    {
                        driverResult = MessageError.DataIsNull;
                        RaiseDriverLogAdded1(this, DriverLogType.INFO, "Report ID is wrong");
                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS7F1
        public MessageError SendS7F1(string path)
        {
            SECSMessage message;
            string ppid;
            FileInfo file;
            int lastIndexOfExt;
            int count;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                file = new FileInfo(path);
                lastIndexOfExt = file.Name.LastIndexOf(".");
                if (lastIndexOfExt < 0)
                {
                    ppid = file.Name;
                }
                else
                {
                    ppid = file.Name.Substring(0, lastIndexOfExt);
                }
                count = (int)file.Length;

                message = this._driver.Messages.GetMessageHeader(7, 1, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("PPID", GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A), Encoding.Default.GetByteCount(ppid), ppid);
                message.Body.Add("LENGTH", GetSECSFormat(DataDictinaryList.LENGTH, SECSItemFormat.U2), 1, count);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS7F3
        public MessageError SendS7F3(string path)
        {
            SECSMessage message;
            string ppid;
            FileInfo file;
            int lastIndexOfExt;
            int count;
            byte[] ppbody;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                file = new FileInfo(path);
                lastIndexOfExt = file.Name.LastIndexOf(".");
                if (lastIndexOfExt < 0)
                {
                    ppid = file.Name;
                }
                else
                {
                    ppid = file.Name.Substring(0, lastIndexOfExt);
                }

                ppbody = File.ReadAllBytes(path);
                count = ppbody.Length;

                message = this._driver.Messages.GetMessageHeader(7, 3, this._driver.Config.DeviceType);

                message.Body.Add(SECSItemFormat.L, 2, null);
                message.Body.Add("PPID", GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A), Encoding.Default.GetByteCount(ppid), ppid);
                message.Body.Add("PPBODY", GetSECSFormat(DataDictinaryList.PPBODY, SECSItemFormat.B), count, ppbody);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS7F5
        public MessageError SendS7F5(string ppid)
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(7, 5, this._driver.Config.DeviceType);

                message.Body.Add("PPID", GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A), Encoding.Default.GetByteCount(ppid), ppid);
                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS7F17
        public MessageError SendS7F17()
        {
            SECSMessage message;
            MessageError driverResult;
            int count;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                count = this.CurrentSetting.ProcessProgramDelete.Count;
                message = this._driver.Messages.GetMessageHeader(7, 17, this._driver.Config.DeviceType);

                message.Body.Add("PPIDCOUNT", SECSItemFormat.L, count, null);

                foreach (string ppid in this.CurrentSetting.ProcessProgramDelete)
                {
                    message.Body.Add("PPID", GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A), Encoding.Default.GetByteCount(ppid), ppid);
                }

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS7F19
        public MessageError SendS7F19()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(7, 19, this._driver.Config.DeviceType);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS7F23
        public MessageError SendS7F23(string ppid)
        {
            SECSMessage message;
            FormattedProcessProgramInfo fmtPPInfo;
            RecipeManager recipeManager;
            List<FmtPPCCodeInfo> ccodeCollection;
            string errorText;
            MessageError driverResult;
            SECSItemFormat ccodeFormat;
            SECSItemFormat ppNameFormat;
            dynamic converted;

            driverResult = MessageError.Ok;
            errorText = string.Empty;

            if (this._driver.Connected == true)
            {
                ccodeFormat = GetSECSFormat(DataDictinaryList.CCODE, SECSItemFormat.A);
                ppNameFormat = GetSECSFormat(DataDictinaryList.PARAMNAME, SECSItemFormat.A);

                fmtPPInfo = this.FormattedProcessProgramCollection.Items.FirstOrDefault(t => t.PPID == ppid);

                if (fmtPPInfo != null)
                {
                    ccodeCollection = null;

                    if (fmtPPInfo.FmtPPCollection.Items == null || fmtPPInfo.FmtPPCollection.Items.Count == 0)
                    {
                        recipeManager = new RecipeManager(fmtPPInfo.PPID)
                        {
                            RecipeDirectory = this.CurrentSetting.FormattedRecipeDirectory
                        };

                        ccodeCollection = recipeManager.Load(out errorText);
                        fmtPPInfo.IsLoaded = true;
                        fmtPPInfo.FmtPPCollection.Items = ccodeCollection;
                    }

                    if (fmtPPInfo.FmtPPCollection.Items != null)
                    {
                        message = this._driver.Messages.GetMessageHeader(7, 23, this._driver.Config.DeviceType);

                        message.Body.Add(SECSItemFormat.L, 4, null);
                        message.Body.Add("PPID", GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A), Encoding.Default.GetByteCount(fmtPPInfo.PPID), fmtPPInfo.PPID);
                        message.Body.Add("MDLN", SECSItemFormat.A, Encoding.Default.GetByteCount(fmtPPInfo.MDLN), fmtPPInfo.MDLN);
                        message.Body.Add("SOFTREV", SECSItemFormat.A, Encoding.Default.GetByteCount(fmtPPInfo.SOFTREV), fmtPPInfo.SOFTREV);

                        message.Body.Add("COMMANDCOUNT", SECSItemFormat.L, fmtPPInfo.FmtPPCollection.Items.Count, null);

                        foreach (FmtPPCCodeInfo tempFmtPPCCodeInfo in fmtPPInfo.FmtPPCollection.Items)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            if (ccodeFormat == SECSItemFormat.A || ccodeFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("CCODE", ccodeFormat, Encoding.Default.GetByteCount(tempFmtPPCCodeInfo.CommandCode), tempFmtPPCCodeInfo.CommandCode);
                            }
                            else
                            {
                                converted = ConvertValue(ccodeFormat, tempFmtPPCCodeInfo.CommandCode);

                                if (converted != null)
                                {
                                    message.Body.Add("CCODE", ccodeFormat, 1, new SECSValue(converted));
                                }
                                else
                                {
                                    message.Body.Add("CCODE", ccodeFormat, 0, string.Empty);
                                }
                            }

                            message.Body.Add("PPARMCOUNT", SECSItemFormat.L, tempFmtPPCCodeInfo.Items.Count, null);

                            foreach (FmtPPItem tempFmtPPItem in tempFmtPPCCodeInfo.Items)
                            {
                                if (S7F23ExtChecked == true)
                                {
                                    message.Body.Add(string.Empty, SECSItemFormat.L, 2, null);
                                    message.Body.Add("PPNAME", SECSItemFormat.A, tempFmtPPItem.PPName.Length, tempFmtPPItem.PPName);

                                    if (tempFmtPPItem.Format == SECSItemFormat.A || tempFmtPPItem.Format == SECSItemFormat.J)
                                    {
                                        message.Body.Add("PPVALUE", tempFmtPPItem.Format, Encoding.Default.GetByteCount(tempFmtPPItem.PPValue), tempFmtPPItem.PPValue);
                                    }
                                    else
                                    {
                                        if (tempFmtPPItem.PPValue.IndexOf(" ") >= 0)
                                        {
                                            AddListToMessage(message, tempFmtPPItem.Format, "PPVALUE", 0, false, tempFmtPPItem.PPValue, out errorText);
                                        }
                                        else
                                        {
                                            converted = ConvertValue(tempFmtPPItem.Format, tempFmtPPItem.PPValue);

                                            if (converted != null)
                                            {
                                                message.Body.Add("PPVALUE", tempFmtPPItem.Format, 1, converted);
                                            }
                                            else
                                            {
                                                message.Body.Add("PPVALUE", tempFmtPPItem.Format, 0, string.Empty);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (tempFmtPPItem.Format == SECSItemFormat.A || tempFmtPPItem.Format == SECSItemFormat.J)
                                    {
                                        message.Body.Add("PPVALUE", tempFmtPPItem.Format, Encoding.Default.GetByteCount(tempFmtPPItem.PPValue), tempFmtPPItem.PPValue);
                                    }
                                    else
                                    {
                                        if (tempFmtPPItem.PPValue.IndexOf(" ") >= 0)
                                        {
                                            AddListToMessage(message, tempFmtPPItem.Format, "PPVALUE", 0, false, tempFmtPPItem.PPValue, out errorText);
                                        }
                                        else
                                        {
                                            converted = ConvertValue(tempFmtPPItem.Format, tempFmtPPItem.PPValue);

                                            if (converted != null)
                                            {
                                                message.Body.Add("PPVALUE", tempFmtPPItem.Format, 1, converted);
                                            }
                                            else
                                            {
                                                message.Body.Add("PPVALUE", tempFmtPPItem.Format, 0, string.Empty);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        driverResult = this._driver.SendSECSMessage(message);
                    }
                    else
                    {
                        RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format("Load INI Fail: {0}", errorText));
                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS7F25
        public MessageError SendS7F25(string ppid)
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(7, 25, this._driver.Config.DeviceType);

                message.Body.Add("PPID", GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A), Encoding.Default.GetByteCount(ppid), ppid);

                driverResult = this._driver.SendSECSMessage(message);
            }

            return driverResult;
        }
        #endregion
        #region SendS9F3
        public MessageError SendS9F3()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(9, 3, this._driver.Config.DeviceType);

                if (message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS9F5
        public MessageError SendS9F5()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(9, 5, this._driver.Config.DeviceType);

                if (message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS9F7
        public MessageError SendS9F7()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(9, 7, this._driver.Config.DeviceType);

                if (message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS9F11
        public MessageError SendS9F11()
        {
            SECSMessage message;
            MessageError driverResult;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(9, 11, this._driver.Config.DeviceType);

                if (message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS10F3
        public MessageError SendS10F3()
        {
            SECSMessage message;
            MessageError driverResult;
            dynamic converted;
            SECSItemFormat tidFormat;
            SECSItemFormat textFormat;
            string compact;
            string errorText;

            errorText = string.Empty;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(this.CurrentSetting.TerminalMessage.S10F3TID) == false)
                {
                    tidFormat = GetSECSFormat(DataDictinaryList.TID, SECSItemFormat.B);
                    textFormat = GetSECSFormat(DataDictinaryList.TEXT, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(10, 3, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    if (tidFormat == SECSItemFormat.A || tidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add("TID", tidFormat, Encoding.Default.GetByteCount(this.CurrentSetting.TerminalMessage.S10F3TID), this.CurrentSetting.TerminalMessage.S10F3TID);
                    }
                    else
                    {
                        converted = ConvertValue(tidFormat, this.CurrentSetting.TerminalMessage.S10F3TID);

                        if (converted == null)
                        {
                            message.Body.Add("TID", tidFormat, 0, string.Empty);
                        }
                        else
                        {
                            message.Body.Add("TID", tidFormat, 1, converted);
                        }
                    }

                    if (textFormat == SECSItemFormat.A || textFormat == SECSItemFormat.J)
                    {
                        message.Body.Add("TEXT", textFormat, Encoding.Default.GetByteCount(this.CurrentSetting.TerminalMessage.S10F3TerminalMessage), this.CurrentSetting.TerminalMessage.S10F3TerminalMessage);
                    }
                    else
                    {
                        if (this.CurrentSetting.TerminalMessage.S10F3TerminalMessage.Count(t => t == ' ') > 0)
                        {
                            compact = this.CurrentSetting.TerminalMessage.S10F3TerminalMessage;
                            AddListToMessage(message, textFormat, "TEXT", 0, false, compact, out errorText);
                        }
                    }

                    if (string.IsNullOrEmpty(errorText) == true)
                    {
                        driverResult = this._driver.SendSECSMessage(message);
                    }
                    else
                    {
                        MessageBox.Show(errorText);
                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region SendS10F5
        public MessageError SendS10F5()
        {
            SECSMessage message;
            SECSItemFormat textFormat;
            MessageError driverResult;
            dynamic converted;
            SECSItemFormat tidFormat;
            string compact;
            string errorText;

            errorText = string.Empty;

            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(this.CurrentSetting.TerminalMessage.S10F5TID) == false)
                {
                    tidFormat = GetSECSFormat(DataDictinaryList.TID, SECSItemFormat.B);
                    textFormat = GetSECSFormat(DataDictinaryList.TEXT, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(10, 5, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    if (tidFormat == SECSItemFormat.A || tidFormat == SECSItemFormat.J)
                    {
                        message.Body.Add("TID", tidFormat, Encoding.Default.GetByteCount(this.CurrentSetting.TerminalMessage.S10F5TID), this.CurrentSetting.TerminalMessage.S10F5TID);
                    }
                    else
                    {
                        converted = ConvertValue(tidFormat, this.CurrentSetting.TerminalMessage.S10F5TID);

                        if (converted == null)
                        {
                            message.Body.Add("TID", tidFormat, 0, string.Empty);
                        }
                        else
                        {
                            message.Body.Add("TID", tidFormat, 1, converted);
                        }
                    }

                    message.Body.Add("TEXTCOUNT", SECSItemFormat.L, this.CurrentSetting.TerminalMessage.S10F5TerminalMessages.Count, null);

                    foreach (string tempTerminalMessage in this.CurrentSetting.TerminalMessage.S10F5TerminalMessages)
                    {
                        if (textFormat == SECSItemFormat.A || textFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("TEXT", textFormat, Encoding.Default.GetByteCount(tempTerminalMessage), tempTerminalMessage);
                        }
                        else
                        {
                            if (tempTerminalMessage.Count(t => t == ' ') > 0)
                            {
                                compact = tempTerminalMessage;

                                if (AddListToMessage(message, textFormat, "TEXT", 0, false, compact, out errorText) == false)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(errorText) == true)
                    {
                        driverResult = this._driver.SendSECSMessage(message);
                    }
                    else
                    {
                        MessageBox.Show(errorText);
                    }
                }
            }

            return driverResult;
        }
        #endregion
        #region ReplyMessageS12F2
        private AnalyzeMessageError ReplyMessageS12F2(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;
            AckInfo ack;
            string materialID;
            MapSetupData mapSetupData;
            SECSItemCollection mapSetupDataCollection;
            SECSItemCollection referencePointCollection;
            string rpItemValue;
            string[] rpItemValueSplitted;
            ReferencePointItem rpItem;

            SECSMessage replyMessage;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 15)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 1);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(12, 2, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[12, 2];

                    if (ack != null && ack.Use == true && ack.Value != 0)
                    {
                        replyMessage.Body.Add("SDACK", SECSItemFormat.B, 1, ack.Value);
                    }
                    else
                    {
                        mapSetupDataCollection = primary.Body.Item.Items[0].SubItem;
                        materialID = mapSetupDataCollection.Items[0].Value.ToString();
                        referencePointCollection = mapSetupDataCollection.Items[6].SubItem;

                        if (string.IsNullOrEmpty(materialID) == false)
                        {
                            mapSetupData = this.MapSetupDataCollection.FirstOrDefault(t => t.MaterialID == materialID);

                            if (mapSetupData == null)
                            {
                                mapSetupData = new MapSetupData()
                                {
                                    MaterialID = materialID
                                };

                                this.MapSetupDataCollection.Add(mapSetupData);
                            }

                            mapSetupData.IDType = mapSetupDataCollection.Items[1].Value;
                            mapSetupData.FlatNotchLocation = mapSetupDataCollection.Items[2].Value.ToString();
                            mapSetupData.FilmFrameRotation = mapSetupDataCollection.Items[3].Value.ToString();
                            mapSetupData.OriginLocation = mapSetupDataCollection.Items[4].Value.ToString();
                            mapSetupData.ReferencePointSelect = mapSetupDataCollection.Items[5].Value;

                            mapSetupData.ReferencePoint.Clear();

                            for (int i = 0; i < referencePointCollection.Count; i++)
                            {
                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    rpItemValue = referencePointCollection.Items[i].Value.ToString();
                                    rpItemValueSplitted = rpItemValue.Split(' ');

                                    rpItem = new ReferencePointItem()
                                    {
                                        X = long.Parse(rpItemValueSplitted[0]),
                                        Y = long.Parse(rpItemValueSplitted[1]),
                                    };

                                    if (mapSetupData.ReferencePoint.FirstOrDefault(t => t.X == rpItem.X && t.Y == rpItem.Y) == null)
                                    {
                                        mapSetupData.ReferencePoint.Add(rpItem);
                                    }
                                    else
                                    {
                                        errorText = string.Format("Reference Point ({0},{1}) is dupelicated", rpItem.X, rpItem.Y);
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(errorText) == true)
                            {
                                mapSetupData.DieUnitsOfMeasure = mapSetupDataCollection.Items[7].Value.ToString();
                                mapSetupData.XAxisDieSize = mapSetupDataCollection.Items[8].Value.ToString();
                                mapSetupData.YAxisDieSize = mapSetupDataCollection.Items[9].Value.ToString();
                                mapSetupData.RowCount = mapSetupDataCollection.Items[10].Value;

                                mapSetupData.ColumnCount = mapSetupDataCollection.Items[11].Value;
                                mapSetupData.NullBinCodeValue = mapSetupDataCollection.Items[12].Value.ToString();
                                mapSetupData.ProcessDieCount = mapSetupDataCollection.Items[13].Value;
                                mapSetupData.ProcessAxis = mapSetupDataCollection.Items[14].Value;

                                replyMessage.Body.Add("SDACK", SECSItemFormat.B, 1, 0);
                            }
                            else
                            {
                                replyMessage.Body.Add("SDACK", SECSItemFormat.B, 1, 1);
                            }
                        }
                        else
                        {
                            replyMessage.Body.Add("SDACK", SECSItemFormat.B, 1, 1);
                            errorText = "MID is blank";
                        }

                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F4
        private AnalyzeMessageError ReplyMessageS12F4(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;
            string materialID;
            MapSetupData mapSetupData;

            SECSMessage replyMessage;

            SECSItemFormat midFormat;
            SECSItemFormat referencePointFormat;
            SECSItemFormat processDieCountFormat;
            SECSItemFormat bcequFormat;
            SECSItemFormat nulbcFormat;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 9)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 3);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(12, 4, this._driver.Config.DeviceType);

                    materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                    mapSetupData = this.MapSetupDataCollection.FirstOrDefault(t => t.MaterialID == materialID);

                    if (string.IsNullOrEmpty(materialID) == false && mapSetupData != null)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 15, null);

                        midFormat = GetSECSFormat(DataDictinaryList.MID, SECSItemFormat.A);

                        if (midFormat == SECSItemFormat.A)
                        {
                            replyMessage.Body.Add("MID", SECSItemFormat.A, Encoding.Default.GetByteCount(mapSetupData.MaterialID), mapSetupData.MaterialID);
                        }
                        else
                        {
                            AddListToMessage(replyMessage, midFormat, "MID", 0, false, mapSetupData.MaterialID, out errorText);
                        }
                        
                        replyMessage.Body.Add("IDTYP", SECSItemFormat.B, 1, mapSetupData.IDType);

                        if (string.IsNullOrEmpty(mapSetupData.FlatNotchLocation) == true)
                        {
                            replyMessage.Body.Add("FNLOC", SECSItemFormat.U2, 0, string.Empty);
                        }
                        else
                        {
                            ConvertAndAddToMessageForLength1Item(replyMessage, "FNLOC", SECSItemFormat.U2, mapSetupData.FlatNotchLocation.Trim(), out errorText);
                        }

                        if (string.IsNullOrEmpty(mapSetupData.OriginLocation) == true)
                        {
                            replyMessage.Body.Add("ORLOC", SECSItemFormat.B, 0, string.Empty);
                        }
                        else
                        {
                            ConvertAndAddToMessageForLength1Item(replyMessage, "ORLOC", SECSItemFormat.B, mapSetupData.OriginLocation.Trim(), out errorText);
                        }

                        replyMessage.Body.Add("RPSEL", SECSItemFormat.U1, 1, mapSetupData.ReferencePointSelect);

                        referencePointFormat = GetSECSFormat(DataDictinaryList.REFP, SECSItemFormat.I2);

                        replyMessage.Body.Add("POINTCOUNT", SECSItemFormat.L, mapSetupData.ReferencePoint.Count, null);

                        foreach (ReferencePointItem rpItem in mapSetupData.ReferencePoint)
                        {
                            AddListToMessage(replyMessage, referencePointFormat, "REFP", 2, true, rpItem.DataString(), out errorText);
                        }

                        replyMessage.Body.Add("DUTMS", SECSItemFormat.A, Encoding.Default.GetByteCount(mapSetupData.DieUnitsOfMeasure), mapSetupData.DieUnitsOfMeasure);

                        ConvertAndAddToMessageForLength1Item(replyMessage, "XDIES", GetSECSFormat(DataDictinaryList.XDIES, SECSItemFormat.U2), mapSetupData.XAxisDieSize, out errorText);
                        ConvertAndAddToMessageForLength1Item(replyMessage, "YDIES", GetSECSFormat(DataDictinaryList.YDIES, SECSItemFormat.U2), mapSetupData.YAxisDieSize, out errorText);

                        replyMessage.Body.Add("ROWCT", GetSECSFormat(DataDictinaryList.ROWCT, SECSItemFormat.U2), 1, mapSetupData.RowCount);
                        replyMessage.Body.Add("COLCT", GetSECSFormat(DataDictinaryList.COLCT, SECSItemFormat.U2), 1, mapSetupData.ColumnCount);

                        processDieCountFormat = GetSECSFormat(DataDictinaryList.PRDCT, SECSItemFormat.U2);

                        if (string.IsNullOrEmpty(mapSetupData.ProcessDieCount) == true)
                        {
                            replyMessage.Body.Add("PRDCT", processDieCountFormat, 0, string.Empty);
                        }
                        else
                        {
                            ConvertAndAddToMessageForLength1Item(replyMessage, "PRDCT", processDieCountFormat, mapSetupData.ProcessDieCount.Trim(), out errorText);
                        }

                        bcequFormat = GetSECSFormat(DataDictinaryList.BCEQU, SECSItemFormat.A);

                        if (bcequFormat == SECSItemFormat.A)
                        {
                            replyMessage.Body.Add("BCEQU", SECSItemFormat.A, Encoding.Default.GetByteCount(mapSetupData.BinCodeEquivalent), mapSetupData.BinCodeEquivalent);
                        }
                        else
                        {
                            AddListToMessage(replyMessage, bcequFormat, "BCEQU", 0, false, mapSetupData.BinCodeEquivalent.Trim(), out errorText);
                        }

                        nulbcFormat = GetSECSFormat(DataDictinaryList.NULBC, SECSItemFormat.A);

                        if (nulbcFormat == SECSItemFormat.A)
                        {
                            replyMessage.Body.Add("NULBC", SECSItemFormat.A, Encoding.Default.GetByteCount(mapSetupData.NullBinCodeValue), mapSetupData.NullBinCodeValue);
                        }
                        else
                        {
                            AddListToMessage(replyMessage, nulbcFormat, "NULBC", 0, false, mapSetupData.NullBinCodeValue.Trim(), out errorText);
                        }

                        replyMessage.Body.Add("MLCL", GetSECSFormat(DataDictinaryList.MLCL, SECSItemFormat.U2), 1, mapSetupData.MessageLength);
                    }
                    else if (string.IsNullOrEmpty(materialID) == true)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        errorText = "MID is blank";
                    }
                    else if (mapSetupData == null)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        errorText = string.Format("MapSetupData is not exists with MID: {0}", materialID);
                    }
                    else
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        errorText = "unknwon error";
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F6
        private AnalyzeMessageError ReplyMessageS12F6(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;
            AckInfo ack;

            string materialID;
            byte idType;
            byte mapFT;
            ulong messageLegnth;
            MapDataType1 mapDataType1;
            MapDataType2 mapDataType2;
            MapDataType3 mapDataType3;

            SECSMessage replyMessage;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 4)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 5);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(12, 6, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[12, 6];

                    if (ack != null && ack.Use == true && ack.Value != 0)
                    {
                        replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, ack.Value);
                    }
                    else
                    {
                        materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                        idType = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                        mapFT = primary.Body.Item.Items[0].SubItem.Items[2].Value;
                        messageLegnth = primary.Body.Item.Items[0].SubItem.Items[3].Value;

                        if (string.IsNullOrEmpty(materialID) == true)
                        {
                            replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, GRNT1.NotFound.GetHashCode());
                            errorText = "MID is blank";
                        }
                        else
                        {
                            if (mapFT == 0)
                            {
                                mapDataType1 = this.MapDataType1Collection.FirstOrDefault(t => t.MaterialID == materialID);

                                if (mapDataType1 != null)
                                {
                                    replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, GRNT1.DupelicatedID.GetHashCode());
                                    errorText = string.Format("MapDataType1 is exists with MID: {0}", materialID);

                                    this._grantedMaterialIDForS12F7 = string.Empty;
                                }
                                else
                                {
                                    replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, GRNT1.OK.GetHashCode());

                                    this._grantedMaterialIDForS12F7 = materialID;
                                }
                            }
                            else if (mapFT == 1)
                            {
                                mapDataType2 = this.MapDataType2Collection.FirstOrDefault(t => t.MaterialID == materialID);

                                if (mapDataType2 != null)
                                {
                                    replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, GRNT1.DupelicatedID.GetHashCode());
                                    errorText = string.Format("MapDataType2 is exists with MID: {0}", materialID);

                                    this._grantedMaterialIDForS12F9 = string.Empty;
                                }
                                else
                                {
                                    replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, GRNT1.OK.GetHashCode());

                                    this._grantedMaterialIDForS12F9 = materialID;
                                }
                            }
                            else if (mapFT == 2)
                            {
                                mapDataType3 = this.MapDataType3Collection.FirstOrDefault(t => t.MaterialID == materialID);

                                if (mapDataType3 != null)
                                {
                                    replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, GRNT1.DupelicatedID.GetHashCode());
                                    errorText = string.Format("MapDataType3 is exists with MID: {0}", materialID);

                                    this._grantedMaterialIDForS12F11 = string.Empty;
                                }
                                else
                                {
                                    replyMessage.Body.Add("GRNT1", SECSItemFormat.B, 1, GRNT1.OK.GetHashCode());

                                    this._grantedMaterialIDForS12F11 = materialID;
                                }
                            }
                        }
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F8
        private AnalyzeMessageError ReplyMessageS12F8(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;
            AckInfo ack;

            string materialID;
            byte idType;
            MapDataType1 mapDataType1;
            ReferenceStartingInfo rsInfo;

            SECSMessage replyMessage;
            SECSItemCollection rsInfoCollection;
            string rsInfoValue;
            string[] rsInfoValueSplitted;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 3 || primary.Body.Item.Items[0].SubItem.Items[2].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 7);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(12, 8, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[12, 8];

                    if (ack != null && ack.Use == true && ack.Value != 0)
                    {
                        replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, ack.Value);
                    }
                    else
                    {
                        materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                        idType = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                        rsInfoCollection = primary.Body.Item.Items[0].SubItem.Items[2].SubItem;

                        if (string.IsNullOrEmpty(materialID) == true)
                        {
                            replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, MDACK.NoIDMatch.GetHashCode());
                            errorText = "MID is blank";
                        }
                        else
                        {
                            mapDataType1 = this.MapDataType1Collection.FirstOrDefault(t => t.MaterialID == materialID);

                            if (mapDataType1 == null)
                            {
                                mapDataType1 = new MapDataType1()
                                {
                                    MaterialID = materialID,
                                    IDType = idType,
                                };

                                this.MapDataType1Collection.Add(mapDataType1);
                            }

                            if (this._grantedMaterialIDForS12F7 != materialID)
                            {
                                mapDataType1.ReferenceStartingList.Clear();
                            }

                            for (int i = 0; i < rsInfoCollection.Count; i++)
                            {
                                rsInfoValue = rsInfoCollection.Items[i].SubItem.Items[0].Value.ToString();
                                rsInfoValueSplitted = rsInfoValue.Split(' ');

                                if (rsInfoValueSplitted.Length == 3)
                                {
                                    rsInfo = new ReferenceStartingInfo()
                                    {
                                        X = long.Parse(rsInfoValueSplitted[0]),
                                        Y = long.Parse(rsInfoValueSplitted[1]),
                                        Direction = long.Parse(rsInfoValueSplitted[2]),
                                        BinList = rsInfoCollection.Items[i].SubItem.Items[1].Value.ToString()
                                    };
                                    if (mapDataType1.ReferenceStartingList.FirstOrDefault(t => t.X == rsInfo.X && t.Y == rsInfo.Y) == null)
                                    {
                                        mapDataType1.ReferenceStartingList.Add(rsInfo);
                                    }
                                }
                            }

                            replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, MDACK.Received.GetHashCode());
                        }
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F10
        private AnalyzeMessageError ReplyMessageS12F10(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;
            AckInfo ack;

            string materialID;
            byte idType;
            MapDataType2 mapDataType2;

            SECSMessage replyMessage;
            string startPointValue;
            string[] startPointValueSplitted;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 4)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 9);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(12, 10, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[12, 10];

                    if (ack != null && ack.Use == true && ack.Value != 0)
                    {
                        replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, ack.Value);
                    }
                    else
                    {
                        materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                        idType = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                        startPointValue = primary.Body.Item.Items[0].SubItem.Items[2].Value.ToString();
                        startPointValueSplitted = startPointValue.Split(' ');

                        if (string.IsNullOrEmpty(materialID) == true)
                        {
                            replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, MDACK.NoIDMatch.GetHashCode());
                            errorText = "MID is blank";
                        }
                        else
                        {
                            mapDataType2 = this.MapDataType2Collection.FirstOrDefault(t => t.MaterialID == materialID);

                            if (mapDataType2 == null)
                            {
                                mapDataType2 = new MapDataType2()
                                {
                                    MaterialID = materialID,
                                    IDType = idType,
                                };

                                this.MapDataType2Collection.Add(mapDataType2);
                            }

                            mapDataType2.StartPointX = long.Parse(startPointValueSplitted[0]);
                            mapDataType2.StartPointY = long.Parse(startPointValueSplitted[1]);
                            mapDataType2.BinList = primary.Body.Item.Items[0].SubItem.Items[3].Value.ToString();

                            replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, MDACK.Received.GetHashCode());
                        }
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F12
        private AnalyzeMessageError ReplyMessageS12F12(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;
            AckInfo ack;

            string materialID;
            byte idType;
            MapDataType3 mapDataType3;
            XYPosInfo xyPosInfo;

            SECSMessage replyMessage;
            SECSItemCollection xyposInfoCollection;
            string xyPosValue;
            string[] xyPosSplitted;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 3 || primary.Body.Item.Items[0].SubItem.Items[2].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 11);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(12, 12, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[12, 12];

                    if (ack != null && ack.Use == true && ack.Value != 0)
                    {
                        replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, ack.Value);
                    }
                    else
                    {
                        materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                        idType = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                        xyposInfoCollection = primary.Body.Item.Items[0].SubItem.Items[2].SubItem;

                        if (string.IsNullOrEmpty(materialID) == true)
                        {
                            replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, MDACK.NoIDMatch.GetHashCode());
                            errorText = "MID is blank";
                        }
                        else
                        {
                            mapDataType3 = this.MapDataType3Collection.FirstOrDefault(t => t.MaterialID == materialID);

                            if (mapDataType3 == null)
                            {
                                mapDataType3 = new MapDataType3()
                                {
                                    MaterialID = materialID,
                                    IDType = idType,
                                };

                                this.MapDataType3Collection.Add(mapDataType3);
                            }

                            if (this._grantedMaterialIDForS12F11 != materialID)
                            {
                                mapDataType3.XYPOSList.Clear();
                            }

                            for (int i = 0; i < xyposInfoCollection.Count; i++)
                            {
                                xyPosValue = xyposInfoCollection.Items[i].SubItem.Items[0].Value.ToString();
                                xyPosSplitted = xyPosValue.Split(' ');

                                xyPosInfo = new XYPosInfo()
                                {
                                    X = long.Parse(xyPosSplitted[0]),
                                    Y = long.Parse(xyPosSplitted[1]),
                                    BinList = xyposInfoCollection.Items[i].SubItem.Items[1].Value.ToString()
                                };

                                if (mapDataType3.XYPOSList.FirstOrDefault(t => t.X == xyPosInfo.X && t.Y == xyPosInfo.Y) == null)
                                {
                                    mapDataType3.XYPOSList.Add(xyPosInfo);
                                }
                            }

                            replyMessage.Body.Add("MDACK", SECSItemFormat.B, 1, MDACK.Received.GetHashCode());
                        }
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F14
        private AnalyzeMessageError ReplyMessageS12F14(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;

            string materialID;
            byte idType;
            MapDataType1 mapDataType1;

            SECSMessage replyMessage;

            string errorText;

            SECSItemFormat midFormat;
            SECSItemFormat rsInfoFormat;
            SECSItemFormat binLTFormat;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 2)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 13);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                    idType = primary.Body.Item.Items[0].SubItem.Items[1].Value;

                    if (string.IsNullOrEmpty(materialID) == true)
                    {
                        errorText = "MID is blank";
                    }
                    else
                    {
                        mapDataType1 = this.MapDataType1Collection.FirstOrDefault(t => t.MaterialID == materialID);

                        if (mapDataType1 == null)
                        {
                            errorText = string.Format("MapDataType1 is not exists with MID: {0}", materialID);
                        }
                        else
                        {

                            replyMessage = this._driver.Messages.GetMessageHeader(12, 14, this._driver.Config.DeviceType);

                            replyMessage.Body.Add("", SECSItemFormat.L, 3, null);

                            midFormat = GetSECSFormat(DataDictinaryList.MID, SECSItemFormat.A);

                            if (midFormat == SECSItemFormat.A)
                            {
                                replyMessage.Body.Add("MID", SECSItemFormat.A, Encoding.Default.GetByteCount(materialID), materialID);
                            }
                            else
                            {
                                AddListToMessage(replyMessage, midFormat, "MID", 0, false, materialID, out errorText);
                            }

                            replyMessage.Body.Add("IDTYP", SECSItemFormat.B, 1, idType);

                            replyMessage.Body.Add("MAPDATACOUNT", SECSItemFormat.L, mapDataType1.ReferenceStartingList.Count, null);

                            rsInfoFormat = GetSECSFormat(DataDictinaryList.RSINF, SECSItemFormat.I2);

                            foreach (var rsInfo in mapDataType1.ReferenceStartingList)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                AddListToMessage(replyMessage, rsInfoFormat, "RSINF", 3, true, rsInfo.DataString(), out errorText);

                                binLTFormat = GetSECSFormat(DataDictinaryList.BINLT, SECSItemFormat.A);

                                if (binLTFormat == SECSItemFormat.A)
                                {
                                    replyMessage.Body.Add("BINLT", SECSItemFormat.A, Encoding.Default.GetByteCount(rsInfo.BinList), rsInfo.BinList);
                                }
                                else
                                {
                                    AddListToMessage(replyMessage, binLTFormat, "BINLT", 0, false, rsInfo.BinList.Trim(), out errorText);
                                }
                            }

                            this._driver.ReplySECSMessage(primary, replyMessage);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F16
        private AnalyzeMessageError ReplyMessageS12F16(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;

            string materialID;
            byte idType;
            MapDataType2 mapDataType2;

            SECSMessage replyMessage;

            string errorText;

            SECSItemFormat midFormat;
            SECSItemFormat strpFormat;
            SECSItemFormat binLTFormat;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 2)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 15);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                    idType = primary.Body.Item.Items[0].SubItem.Items[1].Value;

                    replyMessage = this._driver.Messages.GetMessageHeader(12, 16, this._driver.Config.DeviceType);

                    if (string.IsNullOrEmpty(materialID) == true)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        errorText = "MID is blank";
                    }
                    else
                    {
                        mapDataType2 = this.MapDataType2Collection.FirstOrDefault(t => t.MaterialID == materialID);

                        if (mapDataType2 == null)
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                            errorText = string.Format("MapDataType2 is not exists with MID: {0}", materialID);
                        }
                        else
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 4, null);
                            midFormat = GetSECSFormat(DataDictinaryList.MID, SECSItemFormat.A);

                            if (midFormat == SECSItemFormat.A)
                            {
                                replyMessage.Body.Add("MID", SECSItemFormat.A, Encoding.Default.GetByteCount(materialID), materialID);
                            }
                            else
                            {
                                AddListToMessage(replyMessage, midFormat, "MID", 0, false, materialID, out errorText);
                            }

                            replyMessage.Body.Add("IDTYP", SECSItemFormat.B, 1, idType);

                            strpFormat = GetSECSFormat(DataDictinaryList.STRP, SECSItemFormat.I2);

                            AddListToMessage(replyMessage, strpFormat, "STRP", 2, true, mapDataType2.StrpString(), out errorText);

                            binLTFormat = GetSECSFormat(DataDictinaryList.BINLT, SECSItemFormat.A);

                            if (binLTFormat == SECSItemFormat.A)
                            {
                                replyMessage.Body.Add("BINLT", SECSItemFormat.A, Encoding.Default.GetByteCount(mapDataType2.BinList), mapDataType2.BinList);
                            }
                            else
                            {
                                AddListToMessage(replyMessage, binLTFormat, "BINLT", 0, false, mapDataType2.BinList.Trim(), out errorText);
                            }
                        }
                    }
                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region ReplyMessageS12F18
        private AnalyzeMessageError ReplyMessageS12F18(SECSMessage primary)
        {
            AnalyzeMessageError result;

            UseReplyMessage reply;

            string materialID;
            byte idType;
            byte sdbin;
            MapDataType3 mapDataType3;

            SECSMessage replyMessage;

            string errorText;

            SECSItemFormat midFormat;
            SECSItemFormat xyPosFormst;
            SECSItemFormat binLTFormat;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 3)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 12 && t.Function == 17);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    materialID = primary.Body.Item.Items[0].SubItem.Items[0].Value.ToString();
                    idType = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                    sdbin = primary.Body.Item.Items[0].SubItem.Items[2].Value;

                    replyMessage = this._driver.Messages.GetMessageHeader(12, 18, this._driver.Config.DeviceType);

                    if (string.IsNullOrEmpty(materialID) == true)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        errorText = "MID is blank";
                    }
                    else
                    {
                        mapDataType3 = this.MapDataType3Collection.FirstOrDefault(t => t.MaterialID == materialID);

                        if (mapDataType3 == null)
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                            errorText = string.Format("MapDataType3 is not exists with MID: {0}", materialID);
                        }
                        else
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 3, null);
                            midFormat = GetSECSFormat(DataDictinaryList.MID, SECSItemFormat.A);

                            if (midFormat == SECSItemFormat.A)
                            {
                                replyMessage.Body.Add("MID", SECSItemFormat.A, Encoding.Default.GetByteCount(materialID), materialID);
                            }
                            else
                            {
                                AddListToMessage(replyMessage, midFormat, "MID", 0, false, materialID, out errorText);
                            }

                            replyMessage.Body.Add("IDTYP", SECSItemFormat.B, 1, idType);

                            replyMessage.Body.Add("MAPDATACOUNT", SECSItemFormat.L, mapDataType3.XYPOSList.Count, null);

                            xyPosFormst = GetSECSFormat(DataDictinaryList.XYPOS, SECSItemFormat.I2);

                            foreach (var xyPosInfo in mapDataType3.XYPOSList)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                AddListToMessage(replyMessage, xyPosFormst, "XYPOS", 2, true, xyPosInfo.DataString(), out errorText);

                                binLTFormat = GetSECSFormat(DataDictinaryList.BINLT, SECSItemFormat.A);

                                if (sdbin == 0)
                                {
                                    if (binLTFormat == SECSItemFormat.A)
                                    {
                                        replyMessage.Body.Add("BINLT", SECSItemFormat.A, Encoding.Default.GetByteCount(xyPosInfo.BinList), xyPosInfo.BinList);
                                    }
                                    else
                                    {
                                        AddListToMessage(replyMessage, binLTFormat, "BINLT", 0, false, xyPosInfo.BinList.Trim(), out errorText);
                                    }
                                }
                                else
                                {
                                    replyMessage.Body.Add("BINLT", SECSItemFormat.A, 0, string.Empty);
                                }
                            }
                        }
                    }
                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }

            return result;
        }
        #endregion
        #region SendS12F19
        public void SendS12F19()
        {
            SECSMessage message;

            if (this._driver.Connected == true)
            {
                message = this._driver.Messages.GetMessageHeader(12, 19, this._driver.Config.DeviceType);
                message.Body.Add("", SECSItemFormat.L, 2, null);
                message.Body.Add("MAPER", SECSItemFormat.B, 1, this.CurrentSetting.SelectedMapError);
                message.Body.Add("DATLC", SECSItemFormat.U1, 1, this.CurrentSetting.DATLC);

                this._driver.SendSECSMessage(message);

            }
        }
        #endregion
        #region SendS14F1
        public MessageError SendS14F1()
        {
            MessageError driverResult;
            SECSMessage message;
            SECSItemFormat typeFormat;
            SECSItemFormat objIDFormat;
            SECSItemFormat attrIDFormat;

            string selectedObjectSpecifier;
            string selectedObjectType;

            GEMObject gemObject;
            GEMObjectID gemObjectID;
            GEMObjectAttribute gemAttr;

            KeyForSelectedObjectList keyForSelectedObjectList;

            string compact;
            dynamic converted;
            string errorText;

            List<string> selectedObjectIDList;
            List<GEMObjectAttributeFilterInfo> selectedFilterList;
            List<GEMObjectAttribute> selectedAttributeList;
            Dictionary<string, SECSItemFormat> formatForSelectedFilter;

            errorText = string.Empty;

            driverResult = MessageError.Ok;

            selectedObjectSpecifier = this.CurrentSetting.SelectedObjectSpecifierForS14F1;
            selectedObjectType = this.CurrentSetting.SelectedObjectTypeForS14F1;

            message = null;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false && string.IsNullOrEmpty(selectedObjectType) == false)
                {
                    gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType);

                    selectedObjectIDList = new List<string>();

                    if (gemObject != null)
                    {
                        selectedObjectIDList.AddRange(gemObject.ObjectIDCollection.Items.Where(t => t.IsSelected == true).Select(t => t.OBJID));

                        typeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);
                        objIDFormat = GetSECSFormat(DataDictinaryList.OBJID, SECSItemFormat.A);
                        attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                        message = this._driver.Messages.GetMessageHeader(14, 1, this._driver.Config.DeviceType);

                        message.Body.Add(SECSItemFormat.L, 5, null);
                        message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(selectedObjectSpecifier), selectedObjectSpecifier);

                        if (typeFormat == SECSItemFormat.A || typeFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("OBJTYPE", typeFormat, Encoding.Default.GetByteCount(selectedObjectType), selectedObjectType);
                        }
                        else
                        {
                            compact = selectedObjectType;

                            if (compact.IndexOf(" ") > -1)
                            {
                                if (AddListToMessage(message, typeFormat, "OBJTYPE", 0, false, compact, out errorText) == false)
                                {
                                    driverResult = MessageError.InvalidLength;
                                }
                            }
                            else
                            {
                                converted = ConvertValue(typeFormat, compact);

                                if (converted == null)
                                {
                                    message.Body.Add("OBJTYPE", typeFormat, 0, string.Empty);
                                }
                                else
                                {
                                    message.Body.Add("OBJTYPE", typeFormat, 1, converted);
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            message.Body.Add("OBJCOUNT", SECSItemFormat.L, selectedObjectIDList.Count, null);

                            foreach (var tempGEMObjectID in selectedObjectIDList)
                            {
                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    if (objIDFormat == SECSItemFormat.A || objIDFormat == SECSItemFormat.J)
                                    {
                                        message.Body.Add("OBJID", objIDFormat, Encoding.Default.GetByteCount(tempGEMObjectID), tempGEMObjectID);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(tempGEMObjectID) == true)
                                        {
                                            message.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = tempGEMObjectID;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(message, objIDFormat, "OBJID", 0, false, compact, out errorText) == false)
                                                {
                                                    driverResult = MessageError.InvalidLength;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(objIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("OBJID", objIDFormat, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            // relation(filter)
                            keyForSelectedObjectList = this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1.Keys.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType);

                            if (keyForSelectedObjectList == null || this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1[keyForSelectedObjectList] == null)
                            {
                                message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                            }
                            else
                            {
                                selectedFilterList = new List<GEMObjectAttributeFilterInfo>();
                                formatForSelectedFilter = new Dictionary<string, SECSItemFormat>();

                                foreach (var info in this.CurrentSetting.SelectedObjectAttributeFilterListForS14F1[keyForSelectedObjectList].Where(t => t.IsSelected == true))
                                {
                                    if (selectedObjectIDList.Count == 0)
                                    {
                                        if (selectedFilterList.FirstOrDefault(t => t.ATTRID == info.ATTRID) == null)
                                        {
                                            selectedFilterList.Add(info);

                                            gemObjectID = gemObject.ObjectIDCollection.Items.FirstOrDefault(t => t.OBJID == info.ObjectID);

                                            if (gemObjectID != null)
                                            {
                                                gemAttr = gemObjectID.ObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == info.ATTRID);

                                                if (gemAttr != null && formatForSelectedFilter.ContainsKey(info.ATTRID) == false)
                                                {
                                                    formatForSelectedFilter[info.ObjectID] = gemAttr.Format;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (selectedObjectIDList.Contains(info.ObjectID) == true && selectedFilterList.FirstOrDefault(t => t.ATTRID == info.ATTRID) == null)
                                        {
                                            selectedFilterList.Add(info);
                                        }
                                    }
                                }

                                message.Body.Add("ATTRCOUNT", SECSItemFormat.L, selectedFilterList.Count, null);

                                foreach (var info in selectedFilterList)
                                {
                                    if (string.IsNullOrEmpty(errorText) == true)
                                    {
                                        message.Body.Add("QUALIFIER", SECSItemFormat.L, 3, null);

                                        if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(info.ATTRID), info.ATTRID);
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(info.ATTRID) == true)
                                            {
                                                message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                            }
                                            else
                                            {
                                                compact = info.ATTRID.Trim();

                                                if (compact.IndexOf(" ") > -1)
                                                {
                                                    if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                    {
                                                        driverResult = MessageError.InvalidLength;
                                                    }
                                                }
                                                else
                                                {
                                                    converted = ConvertValue(attrIDFormat, compact);

                                                    if (converted == null)
                                                    {
                                                        message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                    }
                                                    else
                                                    {
                                                        message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                    }
                                                }
                                            }
                                        }

                                        if (formatForSelectedFilter.ContainsKey(info.ATTRID) == false)
                                        {
                                            message.Body.Add("ATTRDATA", SECSItemFormat.A, Encoding.Default.GetByteCount(info.ATTRDATA), info.ATTRDATA);
                                        }
                                        else if (formatForSelectedFilter[info.ATTRID] == SECSItemFormat.A || formatForSelectedFilter[info.ATTRID] == SECSItemFormat.J)
                                        {
                                            message.Body.Add("ATTRDATA", formatForSelectedFilter[info.ATTRID], Encoding.Default.GetByteCount(info.ATTRDATA), info.ATTRDATA);
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(info.ATTRDATA) == true)
                                            {
                                                message.Body.Add("ATTRDATA", formatForSelectedFilter[info.ATTRID], 0, string.Empty);
                                            }
                                            else
                                            {
                                                compact = info.ATTRDATA.Trim();

                                                if (compact.IndexOf(" ") > -1)
                                                {
                                                    AddListToMessage(message, formatForSelectedFilter[info.ATTRID], "ATTRDATA", 0, false, compact, out errorText);
                                                }
                                                else
                                                {
                                                    converted = ConvertValue(formatForSelectedFilter[info.ATTRID], compact);

                                                    if (converted == null)
                                                    {
                                                        message.Body.Add("ATTRDATA", formatForSelectedFilter[info.ATTRID], 0, string.Empty);
                                                    }
                                                    else
                                                    {
                                                        message.Body.Add("ATTRDATA", formatForSelectedFilter[info.ATTRID], 1, converted);
                                                    }
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(info.ATTRRELN) == true)
                                        {
                                            message.Body.Add("ATTRRELN", SECSItemFormat.U1, 1, 0);
                                        }
                                        else
                                        {
                                            compact = info.ATTRRELN.Trim();

                                            converted = ConvertValue(SECSItemFormat.U1, compact);

                                            if (converted == null)
                                            {
                                                message.Body.Add("ATTRRELN", SECSItemFormat.U1, 1, 0);
                                            }
                                            else
                                            {
                                                message.Body.Add("ATTRRELN", SECSItemFormat.U1, 1, converted);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            // Attribute
                            selectedAttributeList = new List<GEMObjectAttribute>();

                            foreach (GEMObjectID tempGEMObjectID in gemObject.ObjectIDCollection.Items)
                            {
                                foreach (GEMObjectAttribute attr in tempGEMObjectID.ObjectAttributeCollection.Items.Where(t => t.IsSelected == true))
                                {
                                    if (selectedObjectIDList.Count == 0)
                                    {
                                        if (selectedAttributeList.FirstOrDefault(t => t.ATTRID == attr.ATTRID) == null)
                                        {
                                            selectedAttributeList.Add(attr);
                                        }
                                    }
                                    else
                                    {
                                        if (selectedObjectIDList.Contains(tempGEMObjectID.OBJID) == true && selectedAttributeList.FirstOrDefault(t => t.ATTRID == attr.ATTRID) == null)
                                        {
                                            selectedAttributeList.Add(attr);
                                        }
                                    }
                                }
                            }

                            message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, selectedAttributeList.Count, null);

                            foreach (GEMObjectAttribute attr in selectedAttributeList)
                            {
                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                    {
                                        message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attr.ATTRID), attr.ATTRID);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(attr.ATTRID) == true)
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = attr.ATTRID;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                {
                                                    driverResult = MessageError.InvalidLength;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attrIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        errorText = string.Format("Can not find GEMObject with [OBJSPEC={0},OBJTYPE={1}]", selectedObjectSpecifier, selectedObjectType);
                    }
                }
                else
                {
                    errorText = "OBJSPEC or OBJTYPE is empty";
                }
                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }

            return driverResult;
        }
        #endregion
        #region AnalyzeSecondaryMessageS14F2
        private void AnalyzeSecondaryMessageS14F2(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            SECSItemCollection objCollection;
            SECSItemCollection ackCollection;
            SECSItemCollection attrCollection;

            GEMObject gemObject;
            GEMObjectID gemObjectID;
            GEMObjectAttribute gemObjectAttr;

            string objSpec;
            string objType;

            SECSItemFormat objIDFormat;
            SECSItemFormat attrIDFormat;
            string objID;
            string attrID;

            string errorText;

            objIDFormat = GetSECSFormat(DataDictinaryList.OBJID, SECSItemFormat.A);
            attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

            errorText = string.Empty;

            if (secondaryMessage.Body.Item.Items.Length == 1 && secondaryMessage.Body.Item.Items[0].SubItem.Items.Length == 2)
            {
                ackCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[1].SubItem;

                if (ackCollection.Count == 2 && ackCollection.Items[0].Format == SECSItemFormat.U1 && ackCollection.Items[0].Value == 0)
                {
                    objSpec = primaryMessage.Body.AsList[1].Value;
                    objType = primaryMessage.Body.AsList[2].Value;

                    gemObject = this.GEMObjectCollection[objSpec, objType];

                    if (gemObject == null)
                    {
                        gemObject = new GEMObject()
                        {
                            OBJSPEC = objSpec,
                            OBJTYPE = objType,
                        };

                        this.GEMObjectCollection.Add(gemObject);
                        this.IsDirty = true;
                    }

                    objCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[0].SubItem;

                    for (int i = 0; i < objCollection.Count; i++)
                    {
                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            if (objCollection.Items[i].SubItem.Count == 2 && objCollection.Items[i].SubItem.Items[0].Format == objIDFormat && objCollection.Items[i].SubItem.Items[1].Format == SECSItemFormat.L)
                            {
                                objID = objCollection.Items[i].SubItem.Items[0].Value.ToString();

                                if (string.IsNullOrEmpty(objID) == true)
                                {
                                    errorText = "Exist empty OBJID";
                                }
                                else
                                {
                                    gemObjectID = gemObject.ObjectIDCollection.Items.FirstOrDefault(t => t.OBJID == objID);

                                    if (gemObjectID == null)
                                    {
                                        gemObjectID = new GEMObjectID()
                                        {
                                            OBJID = objID
                                        };
                                        gemObject.ObjectIDCollection.Add(gemObjectID);
                                        this.IsDirty = true;
                                    }

                                    // Extract AttrID
                                    attrCollection = objCollection.Items[i].SubItem.Items[1].SubItem;

                                    for (int j = 0; j < attrCollection.Count; j++)
                                    {
                                        if (string.IsNullOrEmpty(errorText) == true)
                                        {
                                            if (attrCollection[j].SubItem.Count == 2 && attrCollection[j].SubItem[0].Format == attrIDFormat)
                                            {
                                                attrID = attrCollection[j].SubItem.Items[0].Value.ToString();

                                                gemObjectAttr = gemObjectID.ObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == attrID);

                                                if (gemObjectAttr == null)
                                                {
                                                    gemObjectAttr = new GEMObjectAttribute()
                                                    {
                                                        ATTRID = attrID,
                                                    };
                                                    gemObjectID.ObjectAttributeCollection.Add(gemObjectAttr);
                                                }

                                                SetATTRDATA(attrCollection[j].SubItem.Items[1], gemObjectAttr);
                                                this.IsDirty = true;
                                            }
                                            else
                                            {
                                                errorText = "Can not analyze message. Item count or format mismatch";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                errorText = "Can not analyze message. Item count or format mismatch";
                            }
                        }
                    }
                }
                else
                {
                    errorText = "Can not analyze message. Item count mismatch";
                }
            }
            else
            {
                errorText = "Can not analyze message. Item count mismatch";
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }
        }
        #endregion
        #region ReplyMessageS14F2
        private AnalyzeMessageError ReplyMessageS14F2(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;
            string objType;
            SECSItemCollection objIDCollection;
            SECSItemCollection attrIDCollection;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;

            GEMObject gemObject;
            string objID;
            GEMObjectID gemObjectID;
            string attrID;
            GEMObjectAttribute gemObjectAttr;
            SECSItemFormat objIDFormat;
            SECSItemFormat attrIDFormat;
            string compact;
            dynamic converted;
            SECSItemFormat errCodeFormat;
            ErrCode errCode;
            string errText;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            errCode = ErrCode.NoError;
            errText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].SubItem.Items.Length != 5)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;
                objType = primary.Body.Item.Items[0].SubItem.Items[1].Value;

                gemObject = this.GEMObjectCollection[objSpec, objType];

                if (gemObject == null)
                {
                    gemObject = new GEMObject()
                    {
                        OBJSPEC = objSpec,
                        OBJTYPE = objType,
                    };
                    this.GEMObjectCollection.Add(gemObject);
                    this.IsDirty = true;
                }

                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 1);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(14, 2, this._driver.Config.DeviceType);

                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                    ack = this.CurrentSetting.AckCollection[14, 2];

                    if (ack != null && ack.Use == true)
                    {
                        replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        objIDFormat = GetSECSFormat(DataDictinaryList.OBJID, SECSItemFormat.A);
                        attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                        objIDCollection = primary.Body.Item.Items[0].SubItem.Items[2].SubItem;
                        attrIDCollection = primary.Body.Item.Items[0].SubItem.Items[4].SubItem;

                        replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, objIDCollection.Count, null);

                        for (int i = 0; i < objIDCollection.Count; i++)
                        {
                            if (string.IsNullOrEmpty(errorText) == true)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                objID = objIDCollection.Items[i].Value;

                                if (gemObject.ObjectIDCollection.Items.FirstOrDefault(t => t.OBJID == objID) == null)
                                {
                                    errCode = ErrCode.UnknownObjectInstance;
                                    errText = string.Format("{0}:{1}", ErrCode.UnknownObjectInstance.ToString(), objID);
                                    errorText = string.Format("{0}:{1}", ErrCode.UnknownObjectInstance.ToString(), objID);
                                }
                                else
                                {
                                    #region OBJID
                                    if (objIDFormat == SECSItemFormat.A || objIDFormat == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("OBJID", objIDFormat, Encoding.Default.GetByteCount(objID), objID);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(objIDCollection.Items[i].Value) == true)
                                        {
                                            replyMessage.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = objIDCollection.Items[i].Value.ToString();

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(replyMessage, objIDFormat, "OBJID", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(objIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("OBJID", objIDFormat, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    gemObjectID = gemObject.ObjectIDCollection[objID];
                                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, attrIDCollection.Count, null);

                                    #region ATTR
                                    for (int j = 0; j < attrIDCollection.Items.Length; j++)
                                    {
                                        if (string.IsNullOrEmpty(errorText) == true && attrIDCollection.Items[j].Value != null)
                                        {
                                            attrID = attrIDCollection.Items[j].Value.ToString();
                                            gemObjectAttr = gemObjectID.ObjectAttributeCollection[attrID];

                                            if (gemObjectAttr == null)
                                            {
                                                errCode = ErrCode.UnknownAttributeName;
                                                errText = string.Format("{0}:{1}", ErrCode.UnknownAttributeName.ToString(), attrID);
                                                errorText = string.Format("{0}:{1}", ErrCode.UnknownAttributeName.ToString(), attrID);
                                            }
                                            else
                                            {
                                                replyMessage.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                                                if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(gemObjectAttr.ATTRID), gemObjectAttr.ATTRID);
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(gemObjectAttr.ATTRID) == true)
                                                    {
                                                        replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                    }
                                                    else
                                                    {
                                                        compact = gemObjectAttr.ATTRID;

                                                        if (compact.IndexOf(" ") > -1)
                                                        {
                                                            AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText);
                                                        }
                                                        else
                                                        {
                                                            converted = ConvertValue(attrIDFormat, compact);

                                                            if (converted == null)
                                                            {
                                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                            }
                                                            else
                                                            {
                                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                            }
                                                        }
                                                    }
                                                }

                                                if (gemObjectAttr.Format == SECSItemFormat.L)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, gemObjectAttr.ChildAttributes.Items.Count, string.Empty);
                                                    MakeS14F3Child(replyMessage, gemObjectAttr, out errorText);
                                                }
                                                else
                                                {
                                                    if (gemObjectAttr.Format == SECSItemFormat.A || gemObjectAttr.Format == SECSItemFormat.J)
                                                    {
                                                        replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, Encoding.Default.GetByteCount(gemObjectAttr.ATTRDATA), gemObjectAttr.ATTRDATA);
                                                    }
                                                    else
                                                    {
                                                        if (string.IsNullOrEmpty(gemObjectAttr.ATTRDATA) == true)
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            compact = gemObjectAttr.ATTRDATA;

                                                            if (compact.IndexOf(" ") > -1)
                                                            {
                                                                AddListToMessage(replyMessage, gemObjectAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                            }
                                                            else
                                                            {
                                                                converted = ConvertValue(gemObjectAttr.Format, compact);

                                                                if (converted == null)
                                                                {
                                                                    replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                                }
                                                                else
                                                                {
                                                                    replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, 1, converted);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }

                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, 0);
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                    }

                    if (errCode != ErrCode.NoError)
                    {
                        replyMessage.Body.Clear();

                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, 1);
                        replyMessage.Body.Add("", SECSItemFormat.L, 1, null);
                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                        errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U2);

                        replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, errCode.GetHashCode());
                        replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(errText), errText);

                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                    else
                    {
                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                }
            }

            return result;
        }
        #endregion
        #region SendS14F3
        public MessageError SendS14F3()
        {
            MessageError driverResult;
            SECSMessage message;
            SECSItemFormat typeFormat;
            SECSItemFormat objIDFormat;
            SECSItemFormat attrIDFormat;

            string selectedObjectSpecifier;
            string selectedObjectType;

            GEMObject gemObject;
            int objIDCount;
            int attrCount;
            string compact;
            dynamic converted;
            string errorText;
            List<GEMObjectAttribute> selectedAttributeList;
            List<string> selectedObjectIDList;
            GEMObjectAttribute gemObjectAttr;

            errorText = string.Empty;

            selectedObjectSpecifier = this.CurrentSetting.SelectedObjectSpecifierForS14F3;
            selectedObjectType = this.CurrentSetting.SelectedObjectTypeForS14F3;

            message = null;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false && string.IsNullOrEmpty(selectedObjectType) == false)
                {
                    gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType);

                    if (gemObject != null)
                    {
                        typeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);
                        objIDFormat = GetSECSFormat(DataDictinaryList.OBJID, SECSItemFormat.A);
                        attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                        message = this._driver.Messages.GetMessageHeader(14, 3, this._driver.Config.DeviceType);

                        message.Body.Add(SECSItemFormat.L, 4, null);
                        message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(gemObject.OBJSPEC), gemObject.OBJSPEC);

                        if (typeFormat == SECSItemFormat.A || typeFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("OBJTYPE", typeFormat, Encoding.Default.GetByteCount(gemObject.OBJTYPE), gemObject.OBJTYPE);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(gemObject.OBJTYPE) == true)
                            {
                                message.Body.Add("OBJTYPE", typeFormat, 0, string.Empty);
                            }
                            else
                            {
                                compact = gemObject.OBJTYPE;

                                if (compact.IndexOf(" ") > -1)
                                {
                                    if (AddListToMessage(message, typeFormat, "OBJTYPE", 0, false, compact, out errorText) == false)
                                    {
                                        driverResult = MessageError.InvalidLength;
                                    }
                                }
                                else
                                {
                                    converted = ConvertValue(typeFormat, compact);

                                    if (converted == null)
                                    {
                                        message.Body.Add("OBJTYPE", typeFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        message.Body.Add("OBJTYPE", typeFormat, 1, converted);
                                    }
                                }
                            }
                        }

                        selectedObjectIDList = new List<string>();

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            objIDCount = gemObject.ObjectIDCollection.Items.Count(t => t.IsSelected == true);
                            message.Body.Add("OBJCOUNT", SECSItemFormat.L, objIDCount, null);

                            foreach (GEMObjectID gemObjectID in gemObject.ObjectIDCollection.Items.Where(t => t.IsSelected == true))
                            {
                                selectedObjectIDList.Add(gemObjectID.OBJID);

                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    if (objIDFormat == SECSItemFormat.A || objIDFormat == SECSItemFormat.J)
                                    {
                                        message.Body.Add("OBJID", objIDFormat, Encoding.Default.GetByteCount(gemObjectID.OBJID), gemObjectID.OBJID);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(gemObjectID.OBJID) == true)
                                        {
                                            message.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = gemObjectID.OBJID;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(message, objIDFormat, "OBJID", 0, false, compact, out errorText) == false)
                                                {
                                                    driverResult = MessageError.InvalidLength;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(objIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("OBJID", objIDFormat, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            // Attribute
                            selectedAttributeList = new List<GEMObjectAttribute>();

                            foreach (GEMObjectID tempGEMObjectID in gemObject.ObjectIDCollection.Items)
                            {
                                foreach (GEMObjectAttribute attr in tempGEMObjectID.ObjectAttributeCollection.Items.Where(t => t.IsSelected == true))
                                {
                                    if (selectedObjectIDList.Count == 0)
                                    {
                                        if (selectedAttributeList.FirstOrDefault(t => t.ATTRID == attr.ATTRID) == null)
                                        {
                                            selectedAttributeList.Add(attr);
                                        }
                                    }
                                    else
                                    {
                                        if (selectedObjectIDList.Contains(tempGEMObjectID.OBJID) == true && selectedAttributeList.FirstOrDefault(t => t.ATTRID == attr.ATTRID) == null)
                                        {
                                            selectedAttributeList.Add(attr);
                                        }
                                    }
                                }
                            }

                            attrCount = selectedAttributeList.Count;

                            message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, attrCount, null);

                            foreach (GEMObjectAttribute attr in selectedAttributeList)
                            {
                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    gemObjectAttr = attr;
                                    message.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                                    if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                    {
                                        message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attr.ATTRID), attr.ATTRID);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(attr.ATTRID) == true)
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = attr.ATTRID;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                {
                                                    driverResult = MessageError.InvalidLength;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attrIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                }
                                            }
                                        }
                                    }

                                    if (gemObjectAttr == null)
                                    {
                                        message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                                    }
                                    else
                                    {
                                        if (attr.Format == SECSItemFormat.L)
                                        {
                                            message.Body.Add("ATTRDATA", attr.Format, gemObjectAttr.ChildAttributes.Items.Count, string.Empty);
                                            MakeS14F3Child(message, gemObjectAttr, out errorText);
                                        }
                                        else
                                        {
                                            if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                                            {
                                                message.Body.Add("ATTRDATA", gemObjectAttr.Format, Encoding.Default.GetByteCount(gemObjectAttr.ATTRDATA), gemObjectAttr.ATTRDATA);
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(gemObjectAttr.ATTRDATA) == true)
                                                {
                                                    message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    compact = gemObjectAttr.ATTRDATA;

                                                    if (compact.IndexOf(" ") > -1)
                                                    {
                                                        AddListToMessage(message, gemObjectAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                    }
                                                    else
                                                    {
                                                        converted = ConvertValue(gemObjectAttr.Format, compact);

                                                        if (converted == null)
                                                        {
                                                            message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            message.Body.Add("ATTRDATA", gemObjectAttr.Format, 1, converted);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        errorText = string.Format("Can not find GEMObject with [OBJSPEC={0},OBJTYPE={1}]", selectedObjectSpecifier, selectedObjectType);
                    }
                }
                else
                {
                    errorText = "OBJSPEC or OBJTYPE is null]";
                }

                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }
            return driverResult;
        }
        #endregion
        #region AnalyzeSecondaryMessageS14F4
        private void AnalyzeSecondaryMessageS14F4(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            SECSItemCollection objCollection;
            SECSItemCollection ackCollection;
            SECSItemCollection attrCollection;

            GEMObject gemObject;
            GEMObjectID gemObjectID;
            GEMObjectAttribute gemObjectAttr;

            string objSpec;
            string objType;

            SECSItemFormat objIDFormat;
            SECSItemFormat attrIDFormat;
            string objID;
            string attrID;

            string errorText;

            objIDFormat = GetSECSFormat(DataDictinaryList.OBJID, SECSItemFormat.A);
            attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

            errorText = string.Empty;

            if (secondaryMessage.Body.Item.Items.Length == 1 && secondaryMessage.Body.Item.Items[0].SubItem.Items.Length == 2)
            {
                ackCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[1].SubItem;

                if (ackCollection.Count == 2 && ackCollection.Items[0].Format == SECSItemFormat.U1 && ackCollection.Items[0].Value == 0)
                {
                    objSpec = primaryMessage.Body.AsList[1].Value;
                    objType = primaryMessage.Body.AsList[2].Value;

                    gemObject = this.GEMObjectCollection[objSpec, objType];

                    if (gemObject == null)
                    {
                        gemObject = new GEMObject()
                        {
                            OBJSPEC = objSpec,
                            OBJTYPE = objType,
                        };

                        this.GEMObjectCollection.Add(gemObject);
                        this.IsDirty = true;
                    }

                    objCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[0].SubItem;

                    for (int i = 0; i < objCollection.Count; i++)
                    {
                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            if (objCollection.Items[i].SubItem.Count == 2 && objCollection.Items[i].SubItem.Items[0].Format == objIDFormat && objCollection.Items[i].SubItem.Items[1].Format == SECSItemFormat.L)
                            {
                                objID = objCollection.Items[i].SubItem.Items[0].Value.ToString();

                                if (string.IsNullOrEmpty(objID) == true)
                                {
                                    errorText = "Exist empty OBJID";
                                }
                                else
                                {
                                    gemObjectID = gemObject.ObjectIDCollection.Items.FirstOrDefault(t => t.OBJID == objID);

                                    if (gemObjectID == null)
                                    {
                                        gemObjectID = new GEMObjectID()
                                        {
                                            OBJID = objID
                                        };
                                        gemObject.ObjectIDCollection.Add(gemObjectID);
                                        this.IsDirty = true;
                                    }

                                    // Extract AttrID
                                    attrCollection = objCollection.Items[i].SubItem.Items[1].SubItem;

                                    for (int j = 0; j < attrCollection.Count; j++)
                                    {
                                        if (string.IsNullOrEmpty(errorText) == true)
                                        {
                                            if (attrCollection[j].SubItem.Count == 2 && attrCollection[j].SubItem[0].Format == attrIDFormat)
                                            {
                                                attrID = attrCollection[j].SubItem.Items[0].Value.ToString();

                                                gemObjectAttr = gemObjectID.ObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == attrID);

                                                if (gemObjectAttr == null)
                                                {
                                                    gemObjectAttr = new GEMObjectAttribute()
                                                    {
                                                        ATTRID = attrID,
                                                    };
                                                    gemObjectID.ObjectAttributeCollection.Add(gemObjectAttr);
                                                }

                                                SetATTRDATA(attrCollection[j].SubItem.Items[1], gemObjectAttr);
                                                this.IsDirty = true;
                                            }
                                            else
                                            {
                                                errorText = "Can not analyze message. Item count or format mismatch";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                errorText = "Can not analyze message. Item count or format mismatch";
                            }
                        }
                    }
                }
                else
                {
                    errorText = "Can not analyze message. Item count mismatch";
                }
            }
            else
            {
                errorText = "Can not analyze message. Item count mismatch";
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }
        }
        #endregion
        #region ReplyMessageS14F4
        private AnalyzeMessageError ReplyMessageS14F4(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;
            string objType;
            SECSItemCollection objIDCollection;
            SECSItemCollection attrListCollection;
            SECSItemCollection attrCollection;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;

            string objID;
            List<string> objIDList;
            Dictionary<string, List<string>> attrIDList;
            string attrID;

            GEMObject gemObject;
            GEMObjectID gemObjectID;
            GEMObjectAttribute gemObjectAttr;

            SECSItemFormat objIDFormat;
            SECSItemFormat attrIDFormat;
            string compact;
            dynamic converted;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1 || primary.Body.Item.Items[0].Length != 4)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;
                objType = primary.Body.Item.Items[0].SubItem.Items[1].Value;

                gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTYPE == objType);

                if (gemObject == null)
                {
                    gemObject = new GEMObject()
                    {
                        OBJSPEC = objSpec,
                        OBJTYPE = objType,
                    };

                    this.GEMObjectCollection.Add(gemObject);
                }

                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 3);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(14, 4, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[14, 4];

                    if (ack != null && ack.Use == true)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        objIDFormat = GetSECSFormat(DataDictinaryList.OBJID, SECSItemFormat.A);
                        attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                        objIDCollection = primary.Body.Item.Items[0].SubItem.Items[2].SubItem;
                        attrListCollection = primary.Body.Item.Items[0].SubItem.Items[3].SubItem;

                        objIDList = new List<string>();
                        attrIDList = new Dictionary<string, List<string>>();

                        for (int i = 0; i < objIDCollection.Count; i++)
                        {
                            if (string.IsNullOrEmpty(errorText) == true)
                            {
                                if (objIDCollection.Items[i].Format != objIDFormat)
                                {
                                    errorText = "OBJID Format mismatch";
                                }

                                objID = objIDCollection.Items[i].Value.ToString();

                                objIDList.Add(objID);

                                if (attrIDList.ContainsKey(objID) == false)
                                {
                                    attrIDList[objID] = new List<string>();
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            for (int j = 0; j < attrListCollection.Count; j++)
                            {
                                if (attrListCollection.Items[j].Format == SECSItemFormat.L && attrListCollection.Items[j].Length == 2)
                                {
                                    attrCollection = attrListCollection.Items[j].SubItem;

                                    if (attrIDFormat != attrCollection.Items[0].Format)
                                    {
                                        errorText = "ATTRID Format mismatch";
                                    }
                                    else
                                    {
                                        attrID = attrCollection.Items[0].Value.ToString();

                                        foreach (string tempObjID in objIDList)
                                        {
                                            gemObjectID = gemObject.ObjectIDCollection[tempObjID];

                                            if (gemObjectID == null)
                                            {
                                                gemObjectID = new GEMObjectID()
                                                {
                                                    OBJID = tempObjID,
                                                };
                                                gemObject.ObjectIDCollection.Add(gemObjectID);
                                                this.IsDirty = true;
                                            }

                                            gemObjectAttr = gemObjectID.ObjectAttributeCollection[attrID];

                                            if (gemObjectAttr == null)
                                            {
                                                gemObjectAttr = new GEMObjectAttribute()
                                                {
                                                    ATTRID = attrID
                                                };

                                                gemObjectID.ObjectAttributeCollection.Add(gemObjectAttr);
                                            }

                                            SetATTRDATA(attrCollection.Items[1], gemObjectAttr);
                                            if (attrIDList.ContainsKey(tempObjID) == true && attrIDList[tempObjID] != null && attrIDList[tempObjID].Contains(attrID) == false)
                                            {
                                                attrIDList[tempObjID].Add(attrID);
                                            }

                                            this.IsDirty = true;
                                        }
                                    }
                                }
                                else
                                {
                                    errorText = "Invalid Format or Length in Attributes";
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, objIDList.Count, null);

                            foreach(string tempObjID in objIDList)
                            {
                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                    #region OBJID
                                    if (objIDFormat == SECSItemFormat.A || objIDFormat == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("OBJID", objIDFormat, Encoding.Default.GetByteCount(tempObjID), tempObjID);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(tempObjID) == true)
                                        {
                                            replyMessage.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = tempObjID;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(replyMessage, objIDFormat, "OBJID", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(objIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("OBJID", objIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("OBJID", objIDFormat, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    gemObjectID = gemObject.ObjectIDCollection[tempObjID];

                                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, attrIDList[tempObjID].Count, null);

                                    #region ATTR
                                    foreach (string tempAttrID in attrIDList[tempObjID])
                                    {
                                        if (string.IsNullOrEmpty(errorText) == true)
                                        {
                                            gemObjectAttr = gemObjectID.ObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == tempAttrID);
                                            if (gemObjectAttr == null)
                                            {
                                                replyMessage.Body.Add("ATTRIBUTE", SECSItemFormat.L, 0, null);
                                            }
                                            else
                                            {
                                                replyMessage.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                                                if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(gemObjectAttr.ATTRID), gemObjectAttr.ATTRID);
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(gemObjectAttr.ATTRID) == true)
                                                    {
                                                        replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                    }
                                                    else
                                                    {
                                                        compact = gemObjectAttr.ATTRID;

                                                        if (compact.IndexOf(" ") > -1)
                                                        {
                                                            AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText);
                                                        }
                                                        else
                                                        {
                                                            converted = ConvertValue(attrIDFormat, compact);

                                                            if (converted == null)
                                                            {
                                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                            }
                                                            else
                                                            {
                                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                            }
                                                        }
                                                    }
                                                }

                                                if (gemObjectAttr.Format == SECSItemFormat.L)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, gemObjectAttr.ChildAttributes.Items.Count, string.Empty);
                                                    MakeS14F3Child(replyMessage, gemObjectAttr, out errorText);
                                                }
                                                else
                                                {
                                                    if (gemObjectAttr.Format == SECSItemFormat.A || gemObjectAttr.Format == SECSItemFormat.J)
                                                    {
                                                        replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, Encoding.Default.GetByteCount(gemObjectAttr.ATTRDATA), gemObjectAttr.ATTRDATA);
                                                    }
                                                    else
                                                    {
                                                        if (string.IsNullOrEmpty(gemObjectAttr.ATTRDATA) == true)
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            compact = gemObjectAttr.ATTRDATA;

                                                            if (compact.IndexOf(" ") > -1)
                                                            {
                                                                AddListToMessage(replyMessage, gemObjectAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                            }
                                                            else
                                                            {
                                                                converted = ConvertValue(gemObjectAttr.Format, compact);

                                                                if (converted == null)
                                                                {
                                                                    replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                                }
                                                                else
                                                                {
                                                                    replyMessage.Body.Add("ATTRDATA", gemObjectAttr.Format, 1, converted);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }

                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, 0);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }
                        else
                        {
                            RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
                        }
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            return result;
        }
        #endregion
        #region SendS14F5
        public MessageError SendS14F5()
        {
            MessageError driverResult;
            SECSMessage message;
            string errorText;

            string objSpec = this.CurrentSetting.SelectedObjectSpecifierForS14F5;

            errorText = string.Empty;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (objSpec != null)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 5, this._driver.Config.DeviceType);

                    message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(objSpec), objSpec);

                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    errorText = "OBJSPEC is null";
                }
            }
            else
            {
                errorText = "Disconnected";
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }

            return driverResult;
        }
        #endregion
        #region AnalyzeSecondaryMessageS14F6
        private void AnalyzeSecondaryMessageS14F6(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            SECSItemCollection typesCollection;
            SECSItemCollection ackCollection;

            SECSItem typeItem;
            GEMObject gemObject;

            string objSpec;
            string objType;
            

            string errorText;

            errorText = string.Empty;

            if (secondaryMessage.Body.Item.Items.Length == 1 && secondaryMessage.Body.Item.Items[0].SubItem.Items.Length == 2)
            {
                ackCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[1].SubItem;

                if (ackCollection.Count == 2 && ackCollection.Items[0].Format == SECSItemFormat.U1 && ackCollection.Items[0].Value == 0)
                {
                    objSpec = primaryMessage.Body.AsList[0].Value;

                    typesCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[0].SubItem;

                    for (int i = 0; i < typesCollection.Count; i++)
                    {
                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            typeItem = typesCollection.Items[i];

                            if (typeItem.Value != null)
                            {
                                objType = typeItem.Value.ToString();

                                if (string.IsNullOrEmpty(objType) == false)
                                {
                                    gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTYPE == objType);

                                    if (gemObject == null)
                                    {
                                        gemObject = new GEMObject()
                                        {
                                            OBJSPEC = objSpec,
                                            OBJTYPE = objType
                                        };

                                        this.GEMObjectCollection.Add(gemObject);

                                        this.IsDirty = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorText = "Can not analyze message. Item count mismatch";
                }
            }
            else
            {
                errorText = "Can not analyze message. Item count mismatch";
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }
        }
        #endregion
        #region ReplyMessageS14F6
        private AnalyzeMessageError ReplyMessageS14F6(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;

            int typeCount;

            SECSItemFormat objectTypeFormat;
            string compact;
            dynamic converted;

            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items.Length != 1)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].Value;

                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 5);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(14, 6, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[14, 6];

                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                    if (ack != null && ack.Use == true)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        objectTypeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);

                        typeCount = this.GEMObjectCollection.Items.Count(t => t.OBJSPEC == objSpec);

                        replyMessage.Body.Add("OBJTYPECount", SECSItemFormat.L, typeCount, null);

                        foreach (GEMObject gemObject in this.GEMObjectCollection.Items.Where(t => t.OBJSPEC == objSpec))
                        {
                            compact = gemObject.OBJTYPE;

                            if (objectTypeFormat == SECSItemFormat.A || objectTypeFormat == SECSItemFormat.J)
                            {
                                replyMessage.Body.Add("OBJTYPE", objectTypeFormat, Encoding.Default.GetByteCount(compact), compact);
                            }
                            else
                            {
                                compact = gemObject.OBJTYPE.Trim();

                                if (compact.IndexOf(" ") > -1)
                                {
                                    if (AddListToMessage(replyMessage, objectTypeFormat, "OBJTYPE", 0, false, compact, out errorText) == false)
                                    {
                                        result = AnalyzeMessageError.Unknown;
                                    }
                                }
                                else
                                {
                                    converted = ConvertValue(objectTypeFormat, compact);

                                    if (converted == null)
                                    {
                                        replyMessage.Body.Add("OBJTYPE", objectTypeFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        replyMessage.Body.Add("OBJTYPE", objectTypeFormat, 1, converted);
                                    }
                                }
                            }
                        }

                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            return result;
        }
        #endregion
        #region SendS14F7
        public MessageError SendS14F7()
        {
            MessageError driverResult;
            SECSMessage message;
            string errorText;
            SECSItemFormat typeFormat;
            string compact;
            dynamic converted;

            string objSpec;

            objSpec = this.CurrentSetting.SelectedObjectSpecifierForS14F7;

            errorText = string.Empty;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(objSpec) == false)
                {
                    if (this.CurrentSetting.SelectedObjectTypeListForS14F7.ContainsKey(objSpec) == false)
                    {
                        this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec] = new List<string>();
                    }

                    if (this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec] == null)
                    {
                        this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec] = new List<string>();
                    }

                    message = this._driver.Messages.GetMessageHeader(14, 7, this._driver.Config.DeviceType);

                    message.Body.Add("L2", SECSItemFormat.L, 2, null);

                    message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(objSpec), objSpec);

                    message.Body.Add("OBJTYPECount", SECSItemFormat.L, this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec].Count, null);

                    typeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);

                    foreach (string objType in this.CurrentSetting.SelectedObjectTypeListForS14F7[objSpec])
                    {
                        if (typeFormat == SECSItemFormat.A || typeFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("OBJTYPE", typeFormat, Encoding.Default.GetByteCount(objType), objType);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(objType) == true)
                            {
                                message.Body.Add("OBJTYPE", typeFormat, 0, string.Empty);
                            }
                            else
                            {
                                compact = objType.Trim();

                                if (compact.IndexOf(" ") > -1)
                                {
                                    if (AddListToMessage(message, typeFormat, "OBJTYPE", 0, false, compact, out errorText) == false)
                                    {
                                        driverResult = MessageError.InvalidLength;
                                    }
                                }
                                else
                                {
                                    converted = ConvertValue(typeFormat, compact);

                                    if (converted == null)
                                    {
                                        message.Body.Add("OBJTYPE", typeFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        message.Body.Add("OBJTYPE", typeFormat, 1, converted);
                                    }
                                }
                            }
                        }
                    }

                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    errorText = "OBJSPEC is null";
                }
            }
            else
            {
                errorText = "Disconnected";
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }

            return driverResult;
        }
        #endregion
        #region AnalyzeSecondaryMessageS14F8
        private void AnalyzeSecondaryMessageS14F8(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            SECSItemCollection typesCollection;
            SECSItemCollection ackCollection;

            SECSItemCollection typeItemCollection;
            SECSItemFormat objTypeFormat;
            SECSItemFormat attrIDFormat;
            GEMObject gemObject;
            GEMObjectAttribute attr;

            string objSpec;
            string objType;


            string errorText;

            errorText = string.Empty;

            objTypeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);
            attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

            if (secondaryMessage.Body.Item.Items.Length == 1 && secondaryMessage.Body.Item.Items[0].SubItem.Items.Length == 2)
            {
                ackCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[1].SubItem;

                if (ackCollection.Count == 2 && ackCollection.Items[0].Format == SECSItemFormat.U1 && ackCollection.Items[0].Value == 0)
                {
                    objSpec = primaryMessage.Body.AsList[1].Value;

                    typesCollection = secondaryMessage.Body.Item.Items[0].SubItem.Items[0].SubItem;

                    for (int i = 0; i < typesCollection.Count; i++)
                    {
                        if (string.IsNullOrEmpty(errorText) == true && typesCollection.Items[i].Format == SECSItemFormat.L)
                        {
                            typeItemCollection = typesCollection.Items[i].SubItem;

                            if (typeItemCollection.Count == 2 && typeItemCollection.Items[0].Format == objTypeFormat && typeItemCollection.Items[1].Format == SECSItemFormat.L)
                            {
                                objType = typeItemCollection.Items[0].Value.ToString();

                                if (string.IsNullOrEmpty(objType) == false)
                                {
                                    gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTYPE == objType);

                                    if (gemObject == null)
                                    {
                                        gemObject = new GEMObject()
                                        {
                                            OBJSPEC = objSpec,
                                            OBJTYPE = objType
                                        };

                                        this.GEMObjectCollection.Add(gemObject);
                                    }

                                    for (int j = 0; j < typeItemCollection.Items[1].SubItem.Count; j++)
                                    {
                                        if (typeItemCollection.Items[1].SubItem.Items[j].Format == attrIDFormat)
                                        {
                                            attr = gemObject.AttributeCollection.Items.FirstOrDefault(t => t.ATTRID == typeItemCollection.Items[1].SubItem.Items[j].Value.ToString());

                                            if (attr == null)
                                            {
                                                attr = new GEMObjectAttribute()
                                                {
                                                    ATTRID = typeItemCollection.Items[1].SubItem.Items[j].Value,
                                                };

                                                gemObject.AttributeCollection.Add(attr);
                                            }
                                        }
                                    }

                                    this.IsDirty = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorText = "Can not analyze message. Item count mismatch";
                }
            }
            else
            {
                errorText = "Can not analyze message. Item count mismatch";
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, errorText);
            }
        }
        #endregion
        #region ReplyMessageS14F8
        private AnalyzeMessageError ReplyMessageS14F8(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;
            GEMObject gemObject;

            List<string> objectTypes;
            Dictionary<string, List<string>> gemAttributes;
            int typeCount;

            SECSItemFormat objectTypeFormat;
            SECSItemFormat attrIDFormat;
            SECSItemFormat errCodeFormat;
            string compact;
            dynamic converted;

            List<string> errText;
            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            errText = new List<string>();

            if (primary.Body.Item.Items[0].SubItem.Count != 2 || primary.Body.Item.Items[0].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;

                objectTypes = new List<string>();

                for (int i = 0; i < primary.Body.Item.Items[0].SubItem.Items[1].SubItem.Count; i++)
                {
                    objectTypes.Add(primary.Body.Item.Items[0].SubItem.Items[1].SubItem.Items[i].Value.ToString());
                }

                gemAttributes = new Dictionary<string, List<string>>();

                if (string.IsNullOrEmpty(objSpec) == true)
                {
                    foreach (string type in objectTypes)
                    {
                        gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJTYPE == type);

                        if (gemObject == null)
                        {
                            errText.Add(type);
                        }
                        else
                        {
                            if (gemAttributes.ContainsKey(type) == false)
                            {
                                gemAttributes[type] = new List<string>();

                                foreach (GEMObjectAttribute gemObjectAttribute in gemObject.AttributeCollection.Items)
                                {
                                    if (gemAttributes[gemObject.OBJTYPE].Contains(gemObjectAttribute.ATTRID) == false)
                                    {
                                        gemAttributes[gemObject.OBJTYPE].Add(gemObjectAttribute.ATTRID);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (string type in objectTypes)
                    {
                        gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTYPE == type);

                        if (gemObject == null)
                        {
                            errText.Add(type);
                        }
                        else
                        {
                            if (gemAttributes.ContainsKey(type) == false)
                            {
                                gemAttributes[type] = new List<string>();

                                foreach (GEMObjectAttribute gemObjectAttribute in gemObject.AttributeCollection.Items)
                                {
                                    if (gemAttributes[gemObject.OBJTYPE].Contains(gemObjectAttribute.ATTRID) == false)
                                    {
                                        gemAttributes[gemObject.OBJTYPE].Add(gemObjectAttribute.ATTRID);
                                    }
                                }
                            }
                        }
                    }
                }

                reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 7);

                if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(14, 8, this._driver.Config.DeviceType);

                    ack = this.CurrentSetting.AckCollection[14, 8];

                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                    if (ack != null && ack.Use == true && ack.Value != 0)
                    {
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                        replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        objectTypeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);
                        attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                        if (errText.Count > 0)
                        {
                            replyMessage.Body.Add("OBJTYPECount", SECSItemFormat.L, 0, null);
                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, 1);
                            replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, errText.Count, null);

                            errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U1);

                            foreach (string text in errText)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, ErrCode.UnknownObjectType.GetHashCode());
                                replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(text), text);
                            }
                        }
                        else
                        {
                            typeCount = gemAttributes.Count;

                            replyMessage.Body.Add("OBJTYPECount", SECSItemFormat.L, typeCount, null);

                            foreach (string objType in gemAttributes.Keys)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                compact = objType;

                                if (objectTypeFormat == SECSItemFormat.A || objectTypeFormat == SECSItemFormat.J)
                                {
                                    replyMessage.Body.Add("OBJTYPE", objectTypeFormat, Encoding.Default.GetByteCount(compact), compact);
                                }
                                else
                                {
                                    compact = objType.Trim();

                                    if (compact.IndexOf(" ") > -1)
                                    {
                                        if (AddListToMessage(replyMessage, objectTypeFormat, "OBJTYPE", 0, false, compact, out errorText) == false)
                                        {
                                            result = AnalyzeMessageError.Unknown;
                                        }
                                    }
                                    else
                                    {
                                        converted = ConvertValue(objectTypeFormat, compact);

                                        if (converted == null)
                                        {
                                            replyMessage.Body.Add("OBJTYPE", objectTypeFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            replyMessage.Body.Add("OBJTYPE", objectTypeFormat, 1, converted);
                                        }
                                    }
                                }

                                replyMessage.Body.Add("ATTRIDCount", SECSItemFormat.L, gemAttributes[objType].Count, null);

                                foreach (string attrID in gemAttributes[objType])
                                {
                                    compact = attrID;

                                    if (objectTypeFormat == SECSItemFormat.A || objectTypeFormat == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(compact), compact);
                                    }
                                    else
                                    {
                                        compact = attrID.Trim();

                                        if (compact.IndexOf(" ") > -1)
                                        {
                                            if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                            {
                                                result = AnalyzeMessageError.Unknown;
                                            }
                                        }
                                        else
                                        {
                                            converted = ConvertValue(attrIDFormat, compact);

                                            if (converted == null)
                                            {
                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                            }
                                            else
                                            {
                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                            }
                                        }
                                    }
                                }
                            }

                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }
                    }

                    this._driver.ReplySECSMessage(primary, replyMessage);
                }
            }

            return result;
        }
        #endregion
        #region SendS14F9
        public MessageError SendS14F9()
        {
            MessageError driverResult;
            SECSMessage message;
            SECSItemFormat typeFormat;
            SECSItemFormat attrIDFormat;

            string selectedObjectSpecifier;
            string selectedObjectType;

            GEMObject gemObject;
            string compact;
            dynamic converted;
            string errorText;
            List<GEMObjectAttribute> selectedAttributeList;
            GEMObjectAttribute gemObjectAttr;

            errorText = string.Empty;

            selectedObjectSpecifier = this.CurrentSetting.SelectedObjectSpecifierForS14F9;
            selectedObjectType = this.CurrentSetting.SelectedObjectTypeForS14F9;

            message = null;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false && string.IsNullOrEmpty(selectedObjectType) == false)
                {
                    gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType);

                    if (gemObject != null)
                    {
                        typeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);
                        attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                        message = this._driver.Messages.GetMessageHeader(14, 9, this._driver.Config.DeviceType);

                        message.Body.Add(SECSItemFormat.L, 3, null);
                        message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(gemObject.OBJSPEC), gemObject.OBJSPEC);

                        if (typeFormat == SECSItemFormat.A || typeFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("OBJTYPE", typeFormat, Encoding.Default.GetByteCount(gemObject.OBJTYPE), gemObject.OBJTYPE);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(gemObject.OBJTYPE) == true)
                            {
                                message.Body.Add("OBJTYPE", typeFormat, 0, string.Empty);
                            }
                            else
                            {
                                compact = gemObject.OBJTYPE;

                                if (compact.IndexOf(" ") > -1)
                                {
                                    if (AddListToMessage(message, typeFormat, "OBJTYPE", 0, false, compact, out errorText) == false)
                                    {
                                        driverResult = MessageError.InvalidLength;
                                    }
                                }
                                else
                                {
                                    converted = ConvertValue(typeFormat, compact);

                                    if (converted == null)
                                    {
                                        message.Body.Add("OBJTYPE", typeFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        message.Body.Add("OBJTYPE", typeFormat, 1, converted);
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            // Attribute
                            selectedAttributeList = new List<GEMObjectAttribute>();

                            foreach (GEMObjectAttribute attr in gemObject.AttributeCollection.Items.Where(t => t.IsSelected == true))
                            {
                                if (selectedAttributeList.FirstOrDefault(t => t.ATTRID == attr.ATTRID) == null)
                                {
                                    selectedAttributeList.Add(attr);
                                }
                            }

                            message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, selectedAttributeList.Count, null);

                            foreach (GEMObjectAttribute attr in selectedAttributeList)
                            {
                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    gemObjectAttr = attr;
                                    message.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                                    if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                    {
                                        message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attr.ATTRID), attr.ATTRID);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(attr.ATTRID) == true)
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = attr.ATTRID;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                {
                                                    driverResult = MessageError.InvalidLength;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attrIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                }
                                            }
                                        }
                                    }

                                    if (gemObjectAttr == null)
                                    {
                                        message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                                    }
                                    else
                                    {
                                        if (attr.Format == SECSItemFormat.L)
                                        {
                                            message.Body.Add("ATTRDATA", attr.Format, gemObjectAttr.ChildAttributes.Items.Count, string.Empty);
                                            MakeS14F3Child(message, gemObjectAttr, out errorText);
                                        }
                                        else
                                        {
                                            if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                                            {
                                                message.Body.Add("ATTRDATA", gemObjectAttr.Format, Encoding.Default.GetByteCount(gemObjectAttr.ATTRDATA), gemObjectAttr.ATTRDATA);
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(gemObjectAttr.ATTRDATA) == true)
                                                {
                                                    message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    compact = gemObjectAttr.ATTRDATA;

                                                    if (compact.IndexOf(" ") > -1)
                                                    {
                                                        AddListToMessage(message, gemObjectAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                    }
                                                    else
                                                    {
                                                        converted = ConvertValue(gemObjectAttr.Format, compact);

                                                        if (converted == null)
                                                        {
                                                            message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            message.Body.Add("ATTRDATA", gemObjectAttr.Format, 1, converted);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        errorText = string.Format("Can not find GEMObject with [OBJSPEC={0},OBJTYPE={1}]", selectedObjectSpecifier, selectedObjectType);
                    }
                }
                else
                {
                    errorText = "OBJSPEC or OBJTYPE is null]";
                }

                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }
            return driverResult;
        }
        #endregion
        #region ReplyMessageS14F10
        private AnalyzeMessageError ReplyMessageS14F10(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;
            string objType;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;

            SECSItemCollection attributeCollection;
            SECSItemFormat objectTypeFormat;
            SECSItemFormat attrIDFormat;
            string compact;
            dynamic converted;

            GEMObject gemObject;
            string attrID;
            GEMObjectAttribute gemObjAttr;
            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items[0].SubItem.Count != 3 || primary.Body.Item.Items[0].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;
                objType = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                attributeCollection = primary.Body.Item.Items[0].SubItem.Items[2].SubItem;

                gemObject = null;

                if (string.IsNullOrEmpty(objSpec) == false && string.IsNullOrEmpty(objType) == false)
                {
                    gemObject = this.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTYPE == objType);

                    if (gemObject == null)
                    {
                        gemObject = new GEMObject()
                        {
                            OBJSPEC = objSpec,
                            OBJTYPE = objType,
                        };

                        this.GEMObjectCollection.Add(gemObject);
                    }
                }

                if (gemObject != null)
                {
                    reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 9);

                    if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 10, this._driver.Config.DeviceType);

                        ack = this.CurrentSetting.AckCollection[14, 10];

                        replyMessage.Body.Add("", SECSItemFormat.L, 3, null);

                        if (ack != null && ack.Use == true)
                        {
                            replyMessage.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(objSpec), objSpec);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }
                        else
                        {
                            objectTypeFormat = GetSECSFormat(DataDictinaryList.OBJTYPE, SECSItemFormat.A);
                            attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                            replyMessage.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(objSpec), objSpec);

                            replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, attributeCollection.Count, null);

                            for (int i = 0; i < attributeCollection.Count; i++)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);


                                attrID = string.Empty;

                                if (attributeCollection.Items[i].Value != null)
                                {
                                    attrID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString();
                                }

                                gemObjAttr = gemObject.AttributeCollection[attrID];

                                if (gemObjAttr == null)
                                {
                                    gemObjAttr = new GEMObjectAttribute()
                                    {
                                        ATTRID = attrID,
                                        Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                    };
                                    gemObject.AttributeCollection.Add(gemObjAttr);
                                }

                                if (gemObjAttr.Format == SECSItemFormat.L)
                                {
                                    SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                }
                                else
                                {
                                    gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                }

                                if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                {
                                    replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attrID), attrID);
                                }
                                else
                                {
                                    compact = attrID.Trim();

                                    if (compact.IndexOf(" ") > -1)
                                    {
                                        if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                        {
                                            result = AnalyzeMessageError.Unknown;
                                        }
                                    }
                                    else
                                    {
                                        converted = ConvertValue(attrIDFormat, compact);

                                        if (converted == null)
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                        }
                                    }
                                }

                                if (gemObjAttr.Format == SECSItemFormat.L)
                                {
                                    replyMessage.Body.Add("ATTRDATA", gemObjAttr.Format, gemObjAttr.ChildAttributes.Items.Count, string.Empty);
                                    MakeS14F3Child(replyMessage, gemObjAttr, out errorText);
                                }
                                else
                                {
                                    if (gemObjAttr.Format == SECSItemFormat.A || gemObjAttr.Format == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("ATTRDATA", gemObjAttr.Format, Encoding.Default.GetByteCount(gemObjAttr.ATTRDATA), gemObjAttr.ATTRDATA);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(gemObjAttr.ATTRDATA) == true)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", gemObjAttr.Format, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = gemObjAttr.ATTRDATA;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(replyMessage, gemObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(gemObjAttr.Format, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", gemObjAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", gemObjAttr.Format, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }

                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                }

            }

            return result;
        }
        #endregion
        #region SendS14F11
        public MessageError SendS14F11()
        {
            MessageError driverResult;
            SECSMessage message;
            SECSItemFormat attrIDFormat;

            string selectedObjectSpecifier;

            string compact;
            dynamic converted;
            string errorText;
            List<GEMObjectAttribute> selectedAttributeList;
            GEMObjectAttribute gemObjectAttr;

            errorText = string.Empty;

            selectedObjectSpecifier = this.CurrentSetting.SelectedObjectSpecifierForS14F11;

            message = null;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(14, 11, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(selectedObjectSpecifier), selectedObjectSpecifier);

                    selectedAttributeList = new List<GEMObjectAttribute>();

                    foreach (var gemObject in this.GEMObjectCollection.Items.Where(t => t.OBJSPEC == selectedObjectSpecifier))
                    {
                        foreach (GEMObjectID tempGEMObjectID in gemObject.ObjectIDCollection.Items)
                        {
                            foreach (GEMObjectAttribute attr in tempGEMObjectID.ObjectAttributeCollection.Items.Where(t => t.IsSelected == true))
                            {
                                if (selectedAttributeList.FirstOrDefault(t => t.ATTRID == attr.ATTRID) == null)
                                {
                                    selectedAttributeList.Add(attr);
                                }
                            }
                        }
                    }
                    message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, selectedAttributeList.Count, null);

                    foreach (GEMObjectAttribute attr in selectedAttributeList)
                    {
                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            gemObjectAttr = attr;
                            message.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                            if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attr.ATTRID), attr.ATTRID);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(attr.ATTRID) == true)
                                {
                                    message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                }
                                else
                                {
                                    compact = attr.ATTRID;

                                    if (compact.IndexOf(" ") > -1)
                                    {
                                        if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                        {
                                            driverResult = MessageError.InvalidLength;
                                        }
                                    }
                                    else
                                    {
                                        converted = ConvertValue(attrIDFormat, compact);

                                        if (converted == null)
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                        }
                                    }
                                }
                            }

                            if (gemObjectAttr == null)
                            {
                                message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                            }
                            else
                            {
                                if (attr.Format == SECSItemFormat.L)
                                {
                                    message.Body.Add("ATTRDATA", attr.Format, gemObjectAttr.ChildAttributes.Items.Count, string.Empty);
                                    MakeS14F3Child(message, gemObjectAttr, out errorText);
                                }
                                else
                                {
                                    if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                                    {
                                        message.Body.Add("ATTRDATA", gemObjectAttr.Format, Encoding.Default.GetByteCount(gemObjectAttr.ATTRDATA), gemObjectAttr.ATTRDATA);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(gemObjectAttr.ATTRDATA) == true)
                                        {
                                            message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = gemObjectAttr.ATTRDATA;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(message, gemObjectAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(gemObjectAttr.Format, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("ATTRDATA", gemObjectAttr.Format, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorText = "OBJSPEC is null]";
                }

                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }
            return driverResult;
        }
        #endregion
        #region ReplyMessageS14F12
        private AnalyzeMessageError ReplyMessageS14F12(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;

            SECSItemCollection attributeCollection;
            SECSItemFormat attrIDFormat;
            string compact;
            dynamic converted;

            List<GEMObjectAttribute> deletedAttributeList;
            List<GEMObjectAttribute> tempDeletingAttributeList;
            GEMObjectAttribute gemObjAttr;
            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items[0].SubItem.Count != 2 || primary.Body.Item.Items[0].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;
                attributeCollection = primary.Body.Item.Items[0].SubItem.Items[1].SubItem;

                if (string.IsNullOrEmpty(objSpec) == false)
                {
                    reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 11);

                    if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 12, this._driver.Config.DeviceType);

                        ack = this.CurrentSetting.AckCollection[14, 12];

                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                        if (ack != null && ack.Use == true)
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }
                        else
                        {
                            deletedAttributeList = new List<GEMObjectAttribute>();
                            tempDeletingAttributeList = new List<GEMObjectAttribute>();

                            for (int i = 0; i < attributeCollection.Count; i++)
                            {
                                gemObjAttr = new GEMObjectAttribute()
                                {
                                    ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                    Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                };

                                if (gemObjAttr.Format == SECSItemFormat.L)
                                {
                                    SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                }
                                else
                                {
                                    gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                }

                                deletedAttributeList.Add(gemObjAttr);
                            }

                            foreach (var gemObject in this.GEMObjectCollection.Items.Where(t => t.OBJSPEC == objSpec))
                            {
                                tempDeletingAttributeList.Clear();

                                foreach (var tempObjAttr in gemObject.AttributeCollection.Items)
                                {
                                    if (tempObjAttr.Format != SECSItemFormat.L)
                                    {
                                        gemObjAttr = deletedAttributeList.FirstOrDefault(t => t.ATTRID == tempObjAttr.ATTRID && t.Format == tempObjAttr.Format && t.ATTRDATA == tempObjAttr.ATTRDATA);

                                        if (gemObjAttr != null)
                                        {
                                            tempDeletingAttributeList.Add(gemObjAttr);
                                        }
                                    }
                                }

                                foreach (var tempObjAttr in tempDeletingAttributeList)
                                {
                                    gemObject.AttributeCollection.Items.Remove(tempObjAttr);
                                }
                            }

                            attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                            replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, attributeCollection.Count, null);

                            foreach (var tempObjAttr in deletedAttributeList)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                {
                                    replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                }
                                else
                                {
                                    compact = tempObjAttr.ATTRID.Trim();

                                    if (compact.IndexOf(" ") > -1)
                                    {
                                        if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                        {
                                            result = AnalyzeMessageError.Unknown;
                                        }
                                    }
                                    else
                                    {
                                        converted = ConvertValue(attrIDFormat, compact);

                                        if (converted == null)
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                        }
                                    }
                                }

                                if (tempObjAttr.Format == SECSItemFormat.L)
                                {
                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                    MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                }
                                else
                                {
                                    if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = tempObjAttr.ATTRDATA;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(tempObjAttr.Format, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }

                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                }
                else
                {
                    errorText = string.Format("OBJSPEC is empty");
                    RaiseDriverLogAdded1(this, DriverLogType.INFO, errorText);
                }
            }
            return result;

        }
        #endregion
        #region SendS14F13
        public MessageError SendS14F13()
        {
            MessageError driverResult;
            SECSMessage message;
            SECSItemFormat attrIDFormat;

            string selectedObjectSpecifier;

            string compact;
            dynamic converted;
            string errorText;
            List<GEMObjectAttribute> selectedAttributeList;
            GEMObjectAttribute gemObjectAttr;

            errorText = string.Empty;

            selectedObjectSpecifier = this.CurrentSetting.SelectedObjectSpecifierForS14F13;

            message = null;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(14, 13, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(selectedObjectSpecifier), selectedObjectSpecifier);

                    selectedAttributeList = new List<GEMObjectAttribute>();

                    foreach (var tempGEMObject in this.SupervisedGEMObjectCollection.Items.Where(t => t.OBJSPEC == selectedObjectSpecifier))
                    {
                        foreach (GEMObjectAttribute attr in tempGEMObject.GEMObjectAttributeCollection.Items.Where(t => t.IsSelected == true))
                        {
                            if (selectedAttributeList.FirstOrDefault(t => t.ATTRID == attr.ATTRID) == null)
                            {
                                selectedAttributeList.Add(attr);
                            }
                        }
                    }
                    message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, selectedAttributeList.Count, null);

                    foreach (GEMObjectAttribute attr in selectedAttributeList)
                    {
                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            gemObjectAttr = attr;
                            message.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                            if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attr.ATTRID), attr.ATTRID);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(attr.ATTRID) == true)
                                {
                                    message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                }
                                else
                                {
                                    compact = attr.ATTRID;

                                    if (compact.IndexOf(" ") > -1)
                                    {
                                        if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                        {
                                            driverResult = MessageError.InvalidLength;
                                        }
                                    }
                                    else
                                    {
                                        converted = ConvertValue(attrIDFormat, compact);

                                        if (converted == null)
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                        }
                                    }
                                }
                            }

                            if (gemObjectAttr == null)
                            {
                                message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                            }
                            else
                            {
                                if (attr.Format == SECSItemFormat.L)
                                {
                                    message.Body.Add("ATTRDATA", attr.Format, gemObjectAttr.ChildAttributes.Items.Count, string.Empty);
                                    MakeS14F3Child(message, gemObjectAttr, out errorText);
                                }
                                else
                                {
                                    if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                                    {
                                        message.Body.Add("ATTRDATA", gemObjectAttr.Format, Encoding.Default.GetByteCount(gemObjectAttr.ATTRDATA), gemObjectAttr.ATTRDATA);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(gemObjectAttr.ATTRDATA) == true)
                                        {
                                            message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = gemObjectAttr.ATTRDATA;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(message, gemObjectAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(gemObjectAttr.Format, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("ATTRDATA", gemObjectAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("ATTRDATA", gemObjectAttr.Format, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorText = "OBJSPEC is null]";
                }

                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }
            return driverResult;
        }
        #endregion
        #region ReplyMessageS14F14
        private AnalyzeMessageError ReplyMessageS14F14(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;
            uint objToken;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;

            SECSItemCollection attributeCollection;
            SECSItemFormat attrIDFormat;
            string compact;
            dynamic converted;

            SupervisedGEMObject gemObject;
            GEMObjectAttribute gemObjAttr;
            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items[0].SubItem.Count != 2 || primary.Body.Item.Items[0].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;
                attributeCollection = primary.Body.Item.Items[0].SubItem.Items[1].SubItem;

                if (string.IsNullOrEmpty(objSpec) == false)
                {
                    reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 13);

                    if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 14, this._driver.Config.DeviceType);

                        ack = this.CurrentSetting.AckCollection[14, 14];

                        replyMessage.Body.Add("", SECSItemFormat.L, 3, null);

                        if (ack != null && ack.Use == true)
                        {
                            replyMessage.Body.Add("OBJTOKEN", SECSItemFormat.U4, 1, (uint)0);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }
                        else
                        {
                            objToken = (uint)this._random.Next();
                            gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTOKEN == objToken);

                            while (gemObject != null)
                            {
                                objToken = (uint)this._random.Next();
                                gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTOKEN == objToken);
                            }

                            gemObject = new SupervisedGEMObject()
                            {
                                OBJSPEC = objSpec,
                                OBJTOKEN = objToken,
                            };

                            this.SupervisedGEMObjectCollection.Add(gemObject);

                            for (int i = 0; i < attributeCollection.Count; i++)
                            {
                                gemObjAttr = new GEMObjectAttribute()
                                {
                                    ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                    Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                };

                                if (gemObjAttr.Format == SECSItemFormat.L)
                                {
                                    SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                }
                                else
                                {
                                    gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                }

                                gemObject.GEMObjectAttributeCollection.Add(gemObjAttr);
                            }

                            attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                            replyMessage.Body.Add("OBJTOKEN", SECSItemFormat.U4, 1, objToken);

                            replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, attributeCollection.Count, null);

                            foreach (var tempObjAttr in gemObject.GEMObjectAttributeCollection.Items)
                            {
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                {
                                    replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                }
                                else
                                {
                                    compact = tempObjAttr.ATTRID.Trim();

                                    if (compact.IndexOf(" ") > -1)
                                    {
                                        if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                        {
                                            result = AnalyzeMessageError.Unknown;
                                        }
                                    }
                                    else
                                    {
                                        converted = ConvertValue(attrIDFormat, compact);

                                        if (converted == null)
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                        }
                                        else
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                        }
                                    }
                                }

                                if (tempObjAttr.Format == SECSItemFormat.L)
                                {
                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                    MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                }
                                else
                                {
                                    if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = tempObjAttr.ATTRDATA;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(tempObjAttr.Format, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }

                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                }
                else
                {
                    errorText = string.Format("OBJSPEC is empty");
                    RaiseDriverLogAdded1(this, DriverLogType.INFO, errorText);
                }
            }
            return result;

        }
        #endregion
        #region SendS14F15
        public MessageError SendS14F15()
        {
            MessageError driverResult;
            SECSMessage message;
            SECSItemFormat attrIDFormat;

            string selectedObjectSpecifier;

            string compact;
            dynamic converted;
            string errorText;
            AttachedObjectActionInfo actionInfo;
            SupervisedGEMObject selectedGEMObject;

            errorText = string.Empty;

            selectedObjectSpecifier = this.CurrentSetting.SelectedObjectSpecifierForS14F15;

            message = null;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    selectedGEMObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier);

                    if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F15.ContainsKey(selectedObjectSpecifier) == false || this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] == null)
                    {
                        this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                    }

                    actionInfo = this.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier];

                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(14, 15, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 4, null);
                    message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(selectedObjectSpecifier), selectedObjectSpecifier);
                    message.Body.Add("OBJCMD", SECSItemFormat.U1, 1, actionInfo.OBJCMD);
                    message.Body.Add("OBJTOKEN", SECSItemFormat.U4, 1, actionInfo.OBJTOKEN);

                    if (selectedGEMObject == null)
                    {
                        message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, selectedGEMObject.GEMObjectAttributeCollection.Items.Count(t => t.IsSelected == true), null);

                        foreach (GEMObjectAttribute attr in selectedGEMObject.GEMObjectAttributeCollection.Items.Where(t => t.IsSelected == true))
                        {
                            if (string.IsNullOrEmpty(errorText) == true)
                            {
                                message.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                                if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                {
                                    message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attr.ATTRID), attr.ATTRID);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(attr.ATTRID) == true)
                                    {
                                        message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        compact = attr.ATTRID;

                                        if (compact.IndexOf(" ") > -1)
                                        {
                                            if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                            {
                                                driverResult = MessageError.InvalidLength;
                                            }
                                        }
                                        else
                                        {
                                            converted = ConvertValue(attrIDFormat, compact);

                                            if (converted == null)
                                            {
                                                message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                            }
                                            else
                                            {
                                                message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                            }
                                        }
                                    }
                                }

                                if (attr.Format == SECSItemFormat.L)
                                {
                                    message.Body.Add("ATTRDATA", attr.Format, attr.ChildAttributes.Items.Count, string.Empty);
                                    MakeS14F3Child(message, attr, out errorText);
                                }
                                else
                                {
                                    if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                                    {
                                        message.Body.Add("ATTRDATA", attr.Format, Encoding.Default.GetByteCount(attr.ATTRDATA), attr.ATTRDATA);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(attr.ATTRDATA) == true)
                                        {
                                            message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = attr.ATTRDATA;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(message, attr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attr.Format, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("ATTRDATA", attr.Format, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                else
                {
                    errorText = "OBJSPEC is null";
                }

                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }
            return driverResult;
        }
        #endregion
        #region ReplyMessageS14F16
        private AnalyzeMessageError ReplyMessageS14F16(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;
            byte objCmd;
            uint objToken;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;


            SECSItemFormat errCodeFormat;
            SECSItemCollection attributeCollection;
            SECSItemFormat attrIDFormat;
            string compact;
            dynamic converted;

            SupervisedGEMObject gemObject;
            GEMObjectAttribute gemObjAttr;
            List<GEMObjectAttribute> detachingAttributes;
            List<GEMObjectAttribute> detachedAttributes;
            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items[0].SubItem.Count != 4 || primary.Body.Item.Items[0].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;
                objCmd = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                objToken = primary.Body.Item.Items[0].SubItem.Items[2].Value;
                attributeCollection = primary.Body.Item.Items[0].SubItem.Items[3].SubItem;

                if (string.IsNullOrEmpty(objSpec) == false)
                {
                    reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 15);

                    if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 16, this._driver.Config.DeviceType);

                        ack = this.CurrentSetting.AckCollection[14, 16];

                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                        if (ack != null && ack.Use == true)
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }
                        else
                        {
                            if (objCmd == (byte)1 || objCmd == (byte)3)
                            {
                                #region Attach or reattach
                                gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTOKEN == objToken);

                                if (gemObject == null)
                                {
                                    gemObject = new SupervisedGEMObject()
                                    {
                                        OBJSPEC = objSpec,
                                        OBJTOKEN = objToken
                                    };

                                    this.SupervisedGEMObjectCollection.Add(gemObject);

                                }

                                for (int i = 0; i < attributeCollection.Count; i++)
                                {
                                    gemObjAttr = new GEMObjectAttribute()
                                    {
                                        ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                        Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                    };

                                    if (gemObjAttr.Format == SECSItemFormat.L)
                                    {
                                        SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                    }
                                    else
                                    {
                                        gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                    }


                                    if (gemObject.GEMObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == gemObjAttr.ATTRID) == null)
                                    {
                                        gemObject.GEMObjectAttributeCollection.Add(gemObjAttr);
                                    }
                                }

                                attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                                replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, gemObject.GEMObjectAttributeCollection.Items.Count, null);

                                foreach (var tempObjAttr in gemObject.GEMObjectAttributeCollection.Items)
                                {
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                    if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                    }
                                    else
                                    {
                                        compact = tempObjAttr.ATTRID.Trim();

                                        if (compact.IndexOf(" ") > -1)
                                        {
                                            if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                            {
                                                result = AnalyzeMessageError.Unknown;
                                            }
                                        }
                                        else
                                        {
                                            converted = ConvertValue(attrIDFormat, compact);

                                            if (converted == null)
                                            {
                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                            }
                                            else
                                            {
                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                            }
                                        }
                                    }

                                    if (tempObjAttr.Format == SECSItemFormat.L)
                                    {
                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                        MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                    }
                                    else
                                    {
                                        if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                            {
                                                replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                            }
                                            else
                                            {
                                                compact = tempObjAttr.ATTRDATA;

                                                if (compact.IndexOf(" ") > -1)
                                                {
                                                    AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                }
                                                else
                                                {
                                                    converted = ConvertValue(tempObjAttr.Format, compact);

                                                    if (converted == null)
                                                    {
                                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                    }
                                                    else
                                                    {
                                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                #endregion
                            }

                            else if (objCmd == (byte)2)
                            {
                                #region detach
                                gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTOKEN == objToken);

                                if (gemObject == null)
                                {
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                    errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U2);

                                    converted = ConvertValue(errCodeFormat, ErrCode.VerificationError.GetHashCode().ToString());
                                    replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, converted);
                                    replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(ErrCode.VerificationError.ToString()), ErrCode.VerificationError.ToString());

                                }
                                else
                                {
                                    detachingAttributes = new List<GEMObjectAttribute>();

                                    for (int i = 0; i < attributeCollection.Count; i++)
                                    {
                                        gemObjAttr = new GEMObjectAttribute()
                                        {
                                            ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                            Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                        };

                                        if (gemObjAttr.Format == SECSItemFormat.L)
                                        {
                                            SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                        }
                                        else
                                        {
                                            gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                        }

                                        if (detachingAttributes.FirstOrDefault(t => t.ATTRID == gemObjAttr.ATTRID) == null)
                                        {
                                            detachingAttributes.Add(gemObjAttr);
                                        }
                                    }

                                    detachedAttributes = new List<GEMObjectAttribute>();

                                    foreach (var tempObjAttr in detachingAttributes)
                                    {
                                        gemObjAttr = gemObject.GEMObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == tempObjAttr.ATTRID && t.Format == tempObjAttr.Format && t.ATTRDATA == tempObjAttr.ATTRDATA);

                                        if (gemObject != null)
                                        {
                                            detachedAttributes.Add(gemObjAttr);
                                            gemObject.GEMObjectAttributeCollection.Items.Remove(gemObjAttr);
                                        }
                                    }

                                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                                    replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, detachedAttributes.Count, null);
                                    foreach (var tempObjAttr in detachedAttributes)
                                    {
                                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                        if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                        }
                                        else
                                        {
                                            compact = tempObjAttr.ATTRID.Trim();

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                {
                                                    result = AnalyzeMessageError.Unknown;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attrIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                }
                                            }
                                        }

                                        if (tempObjAttr.Format == SECSItemFormat.L)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                            MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                        }
                                        else
                                        {
                                            if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                            {
                                                replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    compact = tempObjAttr.ATTRDATA;

                                                    if (compact.IndexOf(" ") > -1)
                                                    {
                                                        AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                    }
                                                    else
                                                    {
                                                        converted = ConvertValue(tempObjAttr.Format, compact);

                                                        if (converted == null)
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                }
                                #endregion
                            }
                            else if (objCmd == (byte)4)
                            {
                                #region set attribute
                                gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec && t.OBJTOKEN == objToken);

                                if (gemObject == null)
                                {
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                    errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U2);

                                    converted = ConvertValue(errCodeFormat, ErrCode.VerificationError.GetHashCode().ToString());
                                    replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, converted);
                                    replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(ErrCode.VerificationError.ToString()), ErrCode.VerificationError.ToString());

                                }
                                else
                                {
                                    for (int i = 0; i < attributeCollection.Count; i++)
                                    {
                                        gemObjAttr = new GEMObjectAttribute()
                                        {
                                            ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                            Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                        };

                                        if (gemObjAttr.Format == SECSItemFormat.L)
                                        {
                                            SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                        }
                                        else
                                        {
                                            gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                        }

                                        if (gemObject.GEMObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == gemObjAttr.ATTRID) == null)
                                        {
                                            gemObject.GEMObjectAttributeCollection.Add(gemObjAttr);
                                        }
                                    }

                                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                                    replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, gemObject.GEMObjectAttributeCollection.Items.Count, null);

                                    foreach (var tempObjAttr in gemObject.GEMObjectAttributeCollection.Items)
                                    {
                                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                        if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                        }
                                        else
                                        {
                                            compact = tempObjAttr.ATTRID.Trim();

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                {
                                                    result = AnalyzeMessageError.Unknown;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attrIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                }
                                            }
                                        }

                                        if (tempObjAttr.Format == SECSItemFormat.L)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                            MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                        }
                                        else
                                        {
                                            if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                            {
                                                replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    compact = tempObjAttr.ATTRDATA;

                                                    if (compact.IndexOf(" ") > -1)
                                                    {
                                                        AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                    }
                                                    else
                                                    {
                                                        converted = ConvertValue(tempObjAttr.Format, compact);

                                                        if (converted == null)
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                }
                                #endregion
                            }
                            else
                            {
                                #region cmd not verified
                                replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U2);

                                converted = ConvertValue(errCodeFormat, ErrCode.ValidationError.GetHashCode().ToString());
                                replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, converted);
                                replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(ErrCode.ValidationError.ToString()), ErrCode.ValidationError.ToString());
                                #endregion
                            }

                        }

                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                }
                else
                {
                    errorText = string.Format("OBJSPEC is empty");
                    RaiseDriverLogAdded1(this, DriverLogType.INFO, errorText);
                }
            }
            return result;

        }
        #endregion
        #region SendS14F17
        public MessageError SendS14F17()
        {
            MessageError driverResult;
            SECSMessage message;
            SECSItemFormat attrIDFormat;

            string selectedObjectSpecifier;

            string compact;
            dynamic converted;
            string errorText;
            AttachedObjectActionInfo actionInfo;
            SupervisedGEMObject selectedGEMObject;

            errorText = string.Empty;

            selectedObjectSpecifier = this.CurrentSetting.SelectedObjectSpecifierForS14F17;

            message = null;
            driverResult = MessageError.Ok;

            if (this._driver.Connected == true)
            {
                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    selectedGEMObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier);

                    if (this.CurrentSetting.SelectedAttachedObjectActionListForS14F17.ContainsKey(selectedObjectSpecifier) == false || this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[selectedObjectSpecifier] == null)
                    {
                        this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                    }

                    actionInfo = this.CurrentSetting.SelectedAttachedObjectActionListForS14F17[selectedObjectSpecifier];

                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(14, 17, this._driver.Config.DeviceType);

                    message.Body.Add(SECSItemFormat.L, 4, null);
                    message.Body.Add("OBJSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(selectedObjectSpecifier), selectedObjectSpecifier);
                    message.Body.Add("OBJCMD", SECSItemFormat.U1, 1, actionInfo.OBJCMD);
                    message.Body.Add("TARGETSPEC", SECSItemFormat.A, Encoding.Default.GetByteCount(actionInfo.TARGETSPEC), actionInfo.TARGETSPEC);

                    if (selectedGEMObject == null)
                    {
                        message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, 0, null);
                    }
                    else
                    {
                        message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, selectedGEMObject.GEMObjectAttributeCollection.Items.Count(t => t.IsSelected == true), null);

                        foreach (GEMObjectAttribute attr in selectedGEMObject.GEMObjectAttributeCollection.Items.Where(t => t.IsSelected == true))
                        {
                            if (string.IsNullOrEmpty(errorText) == true)
                            {
                                message.Body.Add("ATTRIBUTE", SECSItemFormat.L, 2, null);

                                if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                {
                                    message.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(attr.ATTRID), attr.ATTRID);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(attr.ATTRID) == true)
                                    {
                                        message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        compact = attr.ATTRID;

                                        if (compact.IndexOf(" ") > -1)
                                        {
                                            if (AddListToMessage(message, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                            {
                                                driverResult = MessageError.InvalidLength;
                                            }
                                        }
                                        else
                                        {
                                            converted = ConvertValue(attrIDFormat, compact);

                                            if (converted == null)
                                            {
                                                message.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                            }
                                            else
                                            {
                                                message.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                            }
                                        }
                                    }
                                }

                                if (attr.Format == SECSItemFormat.L)
                                {
                                    message.Body.Add("ATTRDATA", attr.Format, attr.ChildAttributes.Items.Count, string.Empty);
                                    MakeS14F3Child(message, attr, out errorText);
                                }
                                else
                                {
                                    if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                                    {
                                        message.Body.Add("ATTRDATA", attr.Format, Encoding.Default.GetByteCount(attr.ATTRDATA), attr.ATTRDATA);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(attr.ATTRDATA) == true)
                                        {
                                            message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                                        }
                                        else
                                        {
                                            compact = attr.ATTRDATA;

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                AddListToMessage(message, attr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attr.Format, compact);

                                                if (converted == null)
                                                {
                                                    message.Body.Add("ATTRDATA", attr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    message.Body.Add("ATTRDATA", attr.Format, 1, converted);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                else
                {
                    errorText = "OBJSPEC is null";
                }

                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    driverResult = this._driver.SendSECSMessage(message);
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }
            return driverResult;
        }
        #endregion
        #region ReplyMessageS14F18
        private AnalyzeMessageError ReplyMessageS14F18(SECSMessage primary)
        {
            AnalyzeMessageError result;
            string objSpec;
            byte objCmd;
            string targetSpec;

            UseReplyMessage reply;
            AckInfo ack;

            SECSMessage replyMessage;


            SECSItemFormat errCodeFormat;
            SECSItemCollection attributeCollection;
            SECSItemFormat attrIDFormat;
            string compact;
            dynamic converted;

            SupervisedGEMObject gemObject;
            SupervisedGEMObject targetObject;
            GEMObjectAttribute gemObjAttr;
            List<GEMObjectAttribute> detachingAttributes;
            List<GEMObjectAttribute> detachedAttributes;
            string errorText;

            result = AnalyzeMessageError.Ok;
            errorText = string.Empty;

            if (primary.Body.Item.Items[0].SubItem.Count != 4 || primary.Body.Item.Items[0].Format != SECSItemFormat.L)
            {
                result = AnalyzeMessageError.Unknown;
            }
            else
            {
                objSpec = primary.Body.Item.Items[0].SubItem.Items[0].Value;
                objCmd = primary.Body.Item.Items[0].SubItem.Items[1].Value;
                targetSpec = primary.Body.Item.Items[0].SubItem.Items[2].Value;
                attributeCollection = primary.Body.Item.Items[0].SubItem.Items[3].SubItem;

                if (string.IsNullOrEmpty(objSpec) == false && string.IsNullOrEmpty(targetSpec) == false)
                {
                    reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 14 && t.Function == 17);

                    if (reply != null && reply.SendReply == true && primary.WaitBit == true)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 18, this._driver.Config.DeviceType);

                        ack = this.CurrentSetting.AckCollection[14, 18];

                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                        if (ack != null && ack.Use == true && ack.Value != 0)
                        {
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                            replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                            replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, ack.Value);
                            replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                        }
                        else
                        {
                            if (objCmd == (byte)1 || objCmd == (byte)3)
                            {
                                #region Attach or reattach
                                gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec);
                                targetObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == targetSpec);

                                if (gemObject == null)
                                {
                                    gemObject = new SupervisedGEMObject()
                                    {
                                        OBJSPEC = objSpec,
                                    };

                                    this.SupervisedGEMObjectCollection.Add(gemObject);
                                }

                                if (targetObject == null)
                                {
                                    targetObject = new SupervisedGEMObject()
                                    {
                                        OBJSPEC = targetSpec
                                    };

                                    this.SupervisedGEMObjectCollection.Items.Add(targetObject);
                                }

                                for (int i = 0; i < attributeCollection.Count; i++)
                                {
                                    gemObjAttr = new GEMObjectAttribute()
                                    {
                                        ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                        Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                    };

                                    if (gemObjAttr.Format == SECSItemFormat.L)
                                    {
                                        SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                    }
                                    else
                                    {
                                        gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                    }

                                    if (targetObject.GEMObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == gemObjAttr.ATTRID) == null)
                                    {
                                        targetObject.GEMObjectAttributeCollection.Add(gemObjAttr);
                                    }
                                }

                                attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                                replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, targetObject.GEMObjectAttributeCollection.Items.Count, null);

                                foreach (var tempObjAttr in targetObject.GEMObjectAttributeCollection.Items)
                                {
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                    if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                    }
                                    else
                                    {
                                        compact = tempObjAttr.ATTRID.Trim();

                                        if (compact.IndexOf(" ") > -1)
                                        {
                                            if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                            {
                                                result = AnalyzeMessageError.Unknown;
                                            }
                                        }
                                        else
                                        {
                                            converted = ConvertValue(attrIDFormat, compact);

                                            if (converted == null)
                                            {
                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                            }
                                            else
                                            {
                                                replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                            }
                                        }
                                    }

                                    if (tempObjAttr.Format == SECSItemFormat.L)
                                    {
                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                        MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                    }
                                    else
                                    {
                                        if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                            {
                                                replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                            }
                                            else
                                            {
                                                compact = tempObjAttr.ATTRDATA;

                                                if (compact.IndexOf(" ") > -1)
                                                {
                                                    AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                }
                                                else
                                                {
                                                    converted = ConvertValue(tempObjAttr.Format, compact);

                                                    if (converted == null)
                                                    {
                                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                    }
                                                    else
                                                    {
                                                        replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                #endregion
                            }

                            else if (objCmd == (byte)2)
                            {
                                #region detach
                                gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec);
                                targetObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == targetSpec);

                                if (gemObject == null || targetObject == null)
                                {
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                    errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U2);

                                    converted = ConvertValue(errCodeFormat, ErrCode.VerificationError.GetHashCode().ToString());
                                    replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, converted);
                                    replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(ErrCode.VerificationError.ToString()), ErrCode.VerificationError.ToString());

                                }
                                else
                                {
                                    detachingAttributes = new List<GEMObjectAttribute>();

                                    for (int i = 0; i < attributeCollection.Count; i++)
                                    {
                                        gemObjAttr = new GEMObjectAttribute()
                                        {
                                            ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                            Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                        };

                                        if (gemObjAttr.Format == SECSItemFormat.L)
                                        {
                                            SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                        }
                                        else
                                        {
                                            gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                        }

                                        if (detachingAttributes.FirstOrDefault(t => t.ATTRID == gemObjAttr.ATTRID) == null)
                                        {
                                            detachingAttributes.Add(gemObjAttr);
                                        }
                                    }

                                    detachedAttributes = new List<GEMObjectAttribute>();

                                    foreach (var tempObjAttr in detachingAttributes)
                                    {
                                        gemObjAttr = targetObject.GEMObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == tempObjAttr.ATTRID && t.Format == tempObjAttr.Format && t.ATTRDATA == tempObjAttr.ATTRDATA);

                                        if (gemObject != null)
                                        {
                                            detachedAttributes.Add(gemObjAttr);
                                            gemObject.GEMObjectAttributeCollection.Items.Remove(gemObjAttr);
                                        }
                                    }

                                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                                    replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, detachedAttributes.Count, null);

                                    foreach (var tempObjAttr in detachedAttributes)
                                    {
                                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                        if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                        }
                                        else
                                        {
                                            compact = tempObjAttr.ATTRID.Trim();

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                {
                                                    result = AnalyzeMessageError.Unknown;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attrIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                }
                                            }
                                        }

                                        if (tempObjAttr.Format == SECSItemFormat.L)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                            MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                        }
                                        else
                                        {
                                            if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                            {
                                                replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    compact = tempObjAttr.ATTRDATA;

                                                    if (compact.IndexOf(" ") > -1)
                                                    {
                                                        AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                    }
                                                    else
                                                    {
                                                        converted = ConvertValue(tempObjAttr.Format, compact);

                                                        if (converted == null)
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                }
                                #endregion
                            }
                            else if (objCmd == (byte)4)
                            {
                                #region set attribute
                                gemObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == objSpec);
                                targetObject = this.SupervisedGEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == targetSpec);

                                if (gemObject == null || targetObject == null)
                                {
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                    errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U2);

                                    converted = ConvertValue(errCodeFormat, ErrCode.VerificationError.GetHashCode().ToString());
                                    replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, converted);
                                    replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(ErrCode.VerificationError.ToString()), ErrCode.VerificationError.ToString());

                                }
                                else
                                {
                                    for (int i = 0; i < attributeCollection.Count; i++)
                                    {
                                        gemObjAttr = new GEMObjectAttribute()
                                        {
                                            ATTRID = attributeCollection.Items[i].SubItem.Items[0].Value.ToString(),
                                            Format = attributeCollection.Items[i].SubItem.Items[1].Format,
                                        };

                                        if (gemObjAttr.Format == SECSItemFormat.L)
                                        {
                                            SetATTRDATA(attributeCollection.Items[i].SubItem.Items[1], gemObjAttr);
                                        }
                                        else
                                        {
                                            gemObjAttr.ATTRDATA = attributeCollection.Items[i].SubItem.Items[1].Value.ToString();
                                        }

                                        if (targetObject.GEMObjectAttributeCollection.Items.FirstOrDefault(t => t.ATTRID == gemObjAttr.ATTRID) == null)
                                        {
                                            targetObject.GEMObjectAttributeCollection.Add(gemObjAttr);
                                        }
                                    }

                                    attrIDFormat = GetSECSFormat(DataDictinaryList.ATTRID, SECSItemFormat.A);

                                    replyMessage.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, targetObject.GEMObjectAttributeCollection.Items.Count, null);

                                    foreach (var tempObjAttr in targetObject.GEMObjectAttributeCollection.Items)
                                    {
                                        replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                        if (attrIDFormat == SECSItemFormat.A || attrIDFormat == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("ATTRID", attrIDFormat, Encoding.Default.GetByteCount(tempObjAttr.ATTRID), tempObjAttr.ATTRID);
                                        }
                                        else
                                        {
                                            compact = tempObjAttr.ATTRID.Trim();

                                            if (compact.IndexOf(" ") > -1)
                                            {
                                                if (AddListToMessage(replyMessage, attrIDFormat, "ATTRID", 0, false, compact, out errorText) == false)
                                                {
                                                    result = AnalyzeMessageError.Unknown;
                                                }
                                            }
                                            else
                                            {
                                                converted = ConvertValue(attrIDFormat, compact);

                                                if (converted == null)
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    replyMessage.Body.Add("ATTRID", attrIDFormat, 1, converted);
                                                }
                                            }
                                        }

                                        if (tempObjAttr.Format == SECSItemFormat.L)
                                        {
                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, tempObjAttr.ChildAttributes.Items.Count, string.Empty);
                                            MakeS14F3Child(replyMessage, tempObjAttr, out errorText);
                                        }
                                        else
                                        {
                                            if (tempObjAttr.Format == SECSItemFormat.A || tempObjAttr.Format == SECSItemFormat.J)
                                            {
                                                replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, Encoding.Default.GetByteCount(tempObjAttr.ATTRDATA), tempObjAttr.ATTRDATA);
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(tempObjAttr.ATTRDATA) == true)
                                                {
                                                    replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                }
                                                else
                                                {
                                                    compact = tempObjAttr.ATTRDATA;

                                                    if (compact.IndexOf(" ") > -1)
                                                    {
                                                        AddListToMessage(replyMessage, tempObjAttr.Format, "ATTRDATA", 0, false, compact, out errorText);
                                                    }
                                                    else
                                                    {
                                                        converted = ConvertValue(tempObjAttr.Format, compact);

                                                        if (converted == null)
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 0, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            replyMessage.Body.Add("ATTRDATA", tempObjAttr.Format, 1, converted);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                    replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                }
                                #endregion
                            }
                            else
                            {
                                #region cmd not verified
                                replyMessage.Body.Add("", SECSItemFormat.L, 0, null);
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("OBJACK", SECSItemFormat.U1, 1, (byte)0);
                                replyMessage.Body.Add("", SECSItemFormat.L, 2, null);

                                errCodeFormat = GetSECSFormat(DataDictinaryList.ERRCODE, SECSItemFormat.U2);

                                converted = ConvertValue(errCodeFormat, ErrCode.ValidationError.GetHashCode().ToString());
                                replyMessage.Body.Add("ERRCODE", errCodeFormat, 1, converted);
                                replyMessage.Body.Add("ERRTEXT", SECSItemFormat.A, Encoding.Default.GetByteCount(ErrCode.ValidationError.ToString()), ErrCode.ValidationError.ToString());
                                #endregion
                            }

                        }

                        this._driver.ReplySECSMessage(primary, replyMessage);
                    }
                }
                else
                {
                    errorText = string.Format("OBJSPEC is empty");
                    RaiseDriverLogAdded1(this, DriverLogType.INFO, errorText);
                }
            }
            return result;

        }
        #endregion
        #region Initialize
        public void Initialize()
        {
            ExpandedVariableInfo initControlStateVariableInfo;
            initControlStateVariableInfo = this.VariableCollection.Items.FirstOrDefault(t => t.PreDefined == true && t.Name == PreDefinedECV.InitControlState.ToString()) as ExpandedVariableInfo;

            if (initControlStateVariableInfo != null)
            {
                if (Enum.TryParse(initControlStateVariableInfo.Value, out ControlState initControlState) == true)
                {
                    this.ControlStateBefore = this.ControlState;

                    if (this.ControlState != ControlState.HostOffline)
                    {
                        this.ControlState = ControlState.EquipmentOffline;
                    }

                    RaiseControlStateChanged(this.ControlState);
                }
            }

        }
        #endregion

        // Private Method For Analyze Message
        #region AnalyzeSecondaryMessage
        private void AnalyzeSecondaryMessage(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError analyzeResult;

            switch (secondaryMessage.Stream)
            {
                case 1:
                    analyzeResult = AnalyzeSecondaryMessageStream1(primaryMessage, secondaryMessage);
                    break;
                case 2:
                    analyzeResult = AnalyzeSecondaryMessageStream2(primaryMessage, secondaryMessage);
                    break;
                case 5:
                    analyzeResult = AnalyzeSecondaryMessageStream5(primaryMessage, secondaryMessage);
                    break;
                case 7:
                    analyzeResult = AnalyzeSecondaryMessageStream7(secondaryMessage);
                    break;
                case 14:
                    analyzeResult = AnalyzeSecondaryMessageStream14(primaryMessage, secondaryMessage);
                    break;
                default:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
            }

            if (analyzeResult != AnalyzeMessageError.Ok)
            {
                RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format("Analyze Secondary Message : Message={0}, Result={1}", secondaryMessage, analyzeResult));
            }
        }
        #endregion
        #region AnalyzeSecondaryMessageStream1
        private AnalyzeMessageError AnalyzeSecondaryMessageStream1(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError analyzeResult;
            bool resultGetValue;
            object value;
            ControlState receivedControlState;

            switch (secondaryMessage.Function)
            {
                #region S1F0
                case 0:
                    analyzeResult = AnalyzeMessageError.Ok;

                    AnalyzeSecondaryS1F0(primaryMessage, secondaryMessage);
                    break;
                #endregion
                #region S1F2
                case 2:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S1F4
                case 4:
                    analyzeResult = AnalyzeMessageError.Ok;
                    resultGetValue = GetVValueByS1F4(primaryMessage, secondaryMessage, PreDefinedV.ControlState, out value, out _);

                    if (resultGetValue == true)
                    {
                        receivedControlState = (ControlState)int.Parse(value.ToString());

                        if (this.ControlState != receivedControlState)
                        {
                            this.ControlStateBefore = this.ControlState;
                            this.ControlState = receivedControlState;

                            RaiseControlStateChanged(this.ControlState);

                            // Offline > Online 전환 시
                            if ((this.ControlStateBefore == ControlState.EquipmentOffline || this.ControlStateBefore == ControlState.HostOffline)
                                && (this.ControlState == ControlState.OnlineLocal || this.ControlState == ControlState.OnlineRemote))
                            {
                                if (this.CurrentSetting.AutoSendDefineReport == true && this._isProcessAutoSendOnlineRunning == false)
                                {
                                    this._isProcessAutoSendOnlineRunning = true;
                                    this._isProcessAutoSendOnlineStop = false;
                                    this.CurrentDefineReportSequence = DefineReportSequence.Start;
                                    ProcessAutoSendOnline();
                                }
                            }
                        }
                    }

                    break;
                #endregion
                #region S1F12
                case 12:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S1F14
                case 14:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (this.CommunicationState == CommunicationState.WaitCRFromHost)
                    {
                        this.CommunicationStateBefore = this.CommunicationState;
                        this.CommunicationState = CommunicationState.Communicating;

                        if (this.CommunicationStateBefore != this.CommunicationState)
                        {
                            RaiseCommunicationStateChanged(this.CommunicationState);
                            SendS1F3ForControlState();
                        }
                    }

                    break;
                #endregion
                #region S1F16
                case 16:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (secondaryMessage.Body.AsList.Count == 1
                        && secondaryMessage.Body.AsList[0].Format == SECSItemFormat.B
                        && secondaryMessage.Body.AsList[0].Length == 1
                        && secondaryMessage.Body.AsList[0].Value == 0)
                    {
                        this.ControlStateBefore = this.ControlState;
                        this.ControlState = ControlState.HostOffline;

                        RaiseControlStateChanged(this.ControlState);
                    }
                    break;
                #endregion
                #region S1F18
                case 18:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S1F22
                case 22:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S1F24
                case 24:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                default:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
            }

            return analyzeResult;
        }
        #endregion
        #region AnalyzeSecondaryS1F0
        private void AnalyzeSecondaryS1F0(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            if (primaryMessage.Stream == 1 && primaryMessage.Function == 3 && secondaryMessage.SystemBytes == this._lastSystemBytesS1F3ForControlState)
            {
                this.ControlStateBefore = this.ControlState;

                if (this.ControlState != ControlState.HostOffline)
                {
                    this.ControlState = ControlState.EquipmentOffline;
                }

                if (this.ControlStateBefore != this.ControlState)
                {
                    RaiseControlStateChanged(this.ControlState);
                }
            }

        }
        #endregion
        #region AnalyzeSecondaryMessageStream2
        private AnalyzeMessageError AnalyzeSecondaryMessageStream2(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError analyzeResult;
            byte ack;

            switch (secondaryMessage.Function)
            {
                #region S2F34
                case 34:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (this._isProcessAutoSendOnlineRunning == true && secondaryMessage.Body.AsList.Count == 1)
                    {
                        ack = secondaryMessage.Body.AsList[0].Value;

                        if (ack == 0)
                        {
                            if (primaryMessage.SystemBytes == _lastS2F33DisableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitS2F34Disable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.SendS2F33Enable;
                            }
                            else if (primaryMessage.SystemBytes == _lastS2F33EnableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitS2F34Enable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.SendS2F35Enable;
                            }
                        }
                        else
                        {
                            this.CurrentDefineReportSequence = DefineReportSequence.End;
                        }
                    }

                    break;
                #endregion
                #region S2F36
                case 36:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (this._isProcessAutoSendOnlineRunning == true && secondaryMessage.Body.AsList.Count == 1)
                    {
                        ack = secondaryMessage.Body.AsList[0].Value;

                        if (ack == 0)
                        {
                            if (primaryMessage.SystemBytes == _lastS2F35DisableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitS2F36Disable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.SendS2F33Disable;
                            }
                            else if (primaryMessage.SystemBytes == _lastS2F35EnableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitS2F36Enable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.SendS2F37Enable;
                            }
                        }
                        else
                        {
                            this.CurrentDefineReportSequence = DefineReportSequence.End;
                        }
                    }

                    break;
                #endregion
                #region S2F38
                case 38:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (this._isProcessAutoSendOnlineRunning == true && secondaryMessage.Body.AsList.Count == 1)
                    {
                        ack = secondaryMessage.Body.AsList[0].Value;

                        if (ack == 0)
                        {
                            if (primaryMessage.SystemBytes == _lastS2F37DisableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitS2F38Disable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.SendS2F35Disable;
                            }
                            else if (primaryMessage.SystemBytes == _lastS2F37EnableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitS2F38Enable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.SendAlarmEnable;
                            }
                        }
                        else
                        {
                            this.CurrentDefineReportSequence = DefineReportSequence.End;
                        }
                    }

                    break;
                #endregion
                default:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
            }

            return analyzeResult;
        }
        #endregion
        #region AnalyzeSecondaryMessageStream5
        private AnalyzeMessageError AnalyzeSecondaryMessageStream5(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError analyzeResult;
            byte ack;

            switch (secondaryMessage.Function)
            {
                #region S5F4
                case 4:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (this._isProcessAutoSendOnlineRunning == true && secondaryMessage.Body.AsList.Count == 1)
                    {
                        ack = secondaryMessage.Body.AsList[0].Value;

                        if (ack == 0)
                        {
                            if (primaryMessage.SystemBytes == this._lastS5F3DisableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitAlarmDisable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.SendS2F37Disable;
                            }
                            else if (primaryMessage.SystemBytes == _lastS5F3EnableSystemByte && this.CurrentDefineReportSequence == DefineReportSequence.WaitAlarmEnable)
                            {
                                this.CurrentDefineReportSequence = DefineReportSequence.End;
                            }
                        }
                        else
                        {
                            this.CurrentDefineReportSequence = DefineReportSequence.End;
                        }
                    }

                    break;
                #endregion
                default:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
            }

            return analyzeResult;
        }
        #endregion
        #region AnalyzeSecondaryMessageStream7
        private AnalyzeMessageError AnalyzeSecondaryMessageStream7(SECSMessage secondaryMessage)
        {
            AnalyzeMessageError analyzeResult;
            string ppid;
            string path;
            byte[] readData;
            string mdln;
            string softRev;
            FormattedProcessProgramInfo fmtPPInfo;
            FmtPPCCodeInfo ccodeInfo;
            int ccodeCount;
            string ccode;
            FmtPPItem ppItem;
            int ppcount;
            string ppName;
            string ppValue;
            SECSItem secsItemCCode;
            SECSItem secsItemPP;
            RecipeManager recipeManager;

            switch (secondaryMessage.Function)
            {
                #region [S7F6]
                case 6:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (this.CurrentSetting.IsSaveRecipeReceived == true)
                    {
                        ppid = secondaryMessage.Body.AsList[1].Value;
                        path = string.Format(@"{0}\{1}.rcp", this.CurrentSetting.RecipeDirectory, ppid);
                        readData = secondaryMessage.Body.AsList[2].Value;
                        File.WriteAllBytes(path, readData);
                    }
                    break;
                #endregion
                #region [S7F26]
                case 26:
                    analyzeResult = AnalyzeMessageError.Ok;

                    if (this.CurrentSetting.IsSaveRecipeReceived == true)
                    {
                        ppid = secondaryMessage.Body.AsList[1].Value;
                        mdln = secondaryMessage.Body.AsList[2].Value;
                        softRev = secondaryMessage.Body.AsList[3].Value;

                        fmtPPInfo = this.FormattedProcessProgramCollection.Items.FirstOrDefault(t => t.PPID == ppid);

                        if (fmtPPInfo == null)
                        {
                            fmtPPInfo = new FormattedProcessProgramInfo
                            {
                                PPID = ppid
                            };
                            this.FormattedProcessProgramCollection.Add(fmtPPInfo);
                        }

                        fmtPPInfo.MDLN = mdln;
                        fmtPPInfo.SOFTREV = softRev;

                        fmtPPInfo.FmtPPCollection.Items.Clear();

                        secsItemCCode = secsItemCCode = secondaryMessage.Body.Item.Items[0].SubItem.Items[3];
                        ccodeCount = secsItemCCode.Length;

                        for (int i = 0; i < ccodeCount; i++)
                        {
                            ccode = secsItemCCode.SubItem[i].SubItem[0].Value;
                            secsItemPP = secsItemCCode.SubItem[i].SubItem[1];

                            ccodeInfo = new FmtPPCCodeInfo
                            {
                                CommandCode = ccode
                            };

                            fmtPPInfo.FmtPPCollection.Items.Add(ccodeInfo);
                            ppcount = secsItemPP.Length;

                            for (int j = 0; j < ppcount; j++)
                            {
                                if (secsItemPP.SubItem[j].SubItem.Count == 2)
                                {
                                    ppName = secsItemPP.SubItem[j].SubItem[0].Value;
                                    ppValue = secsItemPP.SubItem[j].SubItem[1].Value;

                                    ppItem = new FmtPPItem
                                    {
                                        PPName = ppName,
                                        PPValue = ppValue,
                                        Format = secsItemPP.SubItem[j].SubItem[1].Format
                                    };

                                    ccodeInfo.Items.Add(ppItem);
                                }
                            }
                        }
                        recipeManager = new RecipeManager(fmtPPInfo.PPID)
                        {
                            RecipeDirectory = this.CurrentSetting.FormattedRecipeDirectory
                        };

                        recipeManager.Save(true, fmtPPInfo.FmtPPCollection.Items, out string errorText);
                    }
                    break;
                #endregion
                default:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
            }

            return analyzeResult;
        }
        #endregion
        #region AnalyzeSecondaryMessageStream14
        private AnalyzeMessageError AnalyzeSecondaryMessageStream14(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError analyzeResult;

            switch (secondaryMessage.Function)
            {
                #region S14F2
                case 2:
                    analyzeResult = AnalyzeMessageError.Ok;
                    AnalyzeSecondaryMessageS14F2(primaryMessage, secondaryMessage);
                    break;
                #endregion
                #region S14F4
                case 4:
                    analyzeResult = AnalyzeMessageError.Ok;
                    AnalyzeSecondaryMessageS14F4(primaryMessage, secondaryMessage);
                    break;
                #endregion
                #region S14F6
                case 6:
                    analyzeResult = AnalyzeMessageError.Ok;
                    AnalyzeSecondaryMessageS14F6(primaryMessage, secondaryMessage);
                    break;
                #endregion
                #region S14F8
                case 8:
                    analyzeResult = AnalyzeMessageError.Ok;
                    AnalyzeSecondaryMessageS14F8(primaryMessage, secondaryMessage);
                    break;
                #endregion
                #region S14F10
                case 10:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S14F12
                case 12:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S14F14
                case 14:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S14F16
                case 16:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                #region S14F18
                case 18:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
                #endregion
                default:
                    analyzeResult = AnalyzeMessageError.Ok;
                    break;
            }

            if (this.OnGEMObjectDataChanged != null)
            {
                this.OnGEMObjectDataChanged.BeginInvoke(null, null);
            }

            return analyzeResult;
        }
        #endregion
        #region AnalyzePrimaryMessage
        private void AnalyzePrimaryMessage(SECSMessage message)
        {
            AnalyzeMessageError analyzeResult;

            int stream;
            int function;
            int replyFunction;
            bool wait;

            SECSMessage userPrimaryMessage;
            UserMessage userMessage;
            SECSMessage replyMessage;

            bool isUserMessage = false;
            bool isUserSecondaryMessage = false;

            // 유저메시지 먼저 검사
            #region UserMessage
            stream = message.Stream;
            function = message.Function;
            replyFunction = function + 1;
            wait = message.WaitBit;

            userPrimaryMessage = this.UserMessage.GetMessageHeader(stream, function, DeviceType.Host);
            replyMessage = this.UserMessage.GetMessageHeader(stream, replyFunction, DeviceType.Host);

            if (userPrimaryMessage != null)
            {
                isUserMessage = true;
                RaiseDriverLogAdded1(this, DriverLogType.INFO, string.Format(" User defined primary message received. [S{0}F{1}{2}]", stream, function, wait == true ? "W" : string.Empty));
            }

            if(replyMessage != null)
            {
                isUserSecondaryMessage = true;

                UseReplyMessage reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == stream && t.Function == function);

                if (function != 0 && replyFunction % 2 == 0)
                {
                    if (wait == false || reply.SendReply == false)
                    {
                        RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format(" sencondary message reply aborted. WaitBit={0}, Reply={1}", wait, reply.SendReply));
                    }
                    else
                    {
                        userMessage = this.UserMessageData.Values.FirstOrDefault(t => t.Stream == stream && t.Function == replyFunction);

                        if (userMessage == null)
                        {
                            RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format(" Can not find user message. [S{0}F{1}]", stream, replyFunction));
                        }
                        else
                        {
                            replyMessage = _secs2Automata.MakeSECSMessageUsingAutomata(userMessage.Name, userMessage.Direction, stream, function + 1, false, userMessage.Data, out string errorLine, out string errorText);

                            if (string.IsNullOrEmpty(errorText) == false)
                            {
                                RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format(" sencondary message create fail. {0} \n {1}", errorText, errorLine));
                            }
                            else
                            {
                                if (replyMessage == null)
                                {
                                    RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format(" sencondary message create fail. {0}", errorText));
                                }
                                else
                                {
                                    if (replyMessage.Direction == SECSMessageDirection.ToHost)
                                    {
                                        RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format(" Can not reply. secondary message Direction is E→H"));
                                    }
                                    else
                                    {
                                        if (replyMessage.Body.AsList.Count(t => t.IsFixed == false) == 0)
                                        {
                                            _driver.ReplySECSMessage(message, replyMessage);
                                        }
                                        else
                                        {
                                            RaiseDriverLogAdded1(this, DriverLogType.WARN, string.Format(" Can not reply. secondary message has not fixed length item"));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            if (isUserMessage == false)
            {
                wait = message.WaitBit;
                message.WaitBit = message.WaitBit == true && isUserSecondaryMessage == false;
                switch (message.Stream)
                {
                    case 1:
                        analyzeResult = AnalyzePrimaryMessageStream1(message);
                        break;
                    case 2:
                        analyzeResult = AnalyzePrimaryMessageStream2(message);
                        break;
                    case 3:
                        analyzeResult = AnalyzePrimaryMessageStream3(message);
                        break;
                    case 4:
                        analyzeResult = AnalyzePrimaryMessageStream4(message);
                        break;
                    case 5:
                        analyzeResult = AnalyzePrimaryMessageStream5(message);
                        break;
                    case 6:
                        analyzeResult = AnalyzePrimaryMessageStream6(message);
                        break;
                    case 7:
                        analyzeResult = AnalyzePrimaryMessageStream7(message);
                        break;
                    case 8:
                        analyzeResult = AnalyzePrimaryMessageStream8(message);
                        break;
                    case 9:
                        analyzeResult = AnalyzePrimaryMessageStream9(message);
                        break;
                    case 10:
                        analyzeResult = AnalyzePrimaryMessageStream10(message);
                        break;
                    case 11:
                        analyzeResult = AnalyzePrimaryMessageStream11(message);
                        break;
                    case 12:
                        analyzeResult = AnalyzePrimaryMessageStream12(message);
                        break;
                    case 13:
                        analyzeResult = AnalyzePrimaryMessageStream13(message);
                        break;
                    case 14:
                        analyzeResult = AnalyzePrimaryMessageStream14(message);
                        break;
                    case 15:
                        analyzeResult = AnalyzePrimaryMessageStream15(message);
                        break;
                    case 16:
                        analyzeResult = AnalyzePrimaryMessageStream16(message);
                        break;
                    case 17:
                        analyzeResult = AnalyzePrimaryMessageStream17(message);
                        break;
                    default:
                        analyzeResult = AnalyzeMessageError.Undefined;
                        break;
                }
                message.WaitBit = wait;
            }
        }
        #endregion
        #region AnalyzePrimaryMessageStream1
        private AnalyzeMessageError AnalyzePrimaryMessageStream1(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            AckInfo ack;
            byte s1f14ack;

            UseReplyMessage reply;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S1F1]
                case 1:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 1 && t.Function == 1);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(1, 2, this._driver.Config.DeviceType);

                                replyMessage.Body.Add(SECSItemFormat.L, 0, null);

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                #region [S1F13]
                case 13:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            this.CommunicationStateBefore = this.CommunicationState;
                            this.CommunicationState = CommunicationState.WaitCRA;

                            if (this.CommunicationStateBefore != this.CommunicationState)
                            {
                                RaiseCommunicationStateChanged(CommunicationState);
                            }

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 1 && t.Function == 13);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(1, 14, this._driver.Config.DeviceType);

                                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                                ack = this.CurrentSetting.AckCollection[1, 14];

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("COMMACK", SECSItemFormat.B, 1, ack.Value);
                                    s1f14ack = (byte)ack.Value;
                                }
                                else
                                {
                                    replyMessage.Body.Add("COMMACK", SECSItemFormat.B, 1, 0);
                                    s1f14ack = 0;
                                }

                                replyMessage.Body.Add(SECSItemFormat.L, 0, null);

                                this._driver.ReplySECSMessage(message, replyMessage);

                                if (s1f14ack == 0)
                                {
                                    this.CommunicationStateBefore = this.CommunicationState;
                                    this.CommunicationState = CommunicationState.Communicating;

                                    if (this.CommunicationStateBefore != this.CommunicationState)
                                    {
                                        RaiseCommunicationStateChanged(CommunicationState);
                                        SendS1F3ForControlState();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            replyMessage = null;

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream2
        private AnalyzeMessageError AnalyzePrimaryMessageStream2(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            ExpandedVariableInfo variableInfo;

            UseReplyMessage reply;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S2F17]
                case 17:
                    {
                        string timeData;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 2 && t.Function == 17);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                variableInfo = this.VariableCollection.GetVariableInfo(PreDefinedECV.TimeFormat.ToString()) as ExpandedVariableInfo;

                                replyMessage = this._driver.Messages.GetMessageHeader(2, 18, this._driver.Config.DeviceType);

                                if (variableInfo != null && int.TryParse(variableInfo.Value, out int timeFormat) == true)
                                {
                                    if (timeFormat == 0)
                                    {
                                        timeData = DateTime.Now.ToString("yyMMddHHmmss");
                                    }
                                    else
                                    {
                                        timeData = DateTime.Now.ToString("yyyyMMddHHmmssff");
                                    }
                                }
                                else
                                {
                                    timeData = DateTime.Now.ToString("yyyyMMddHHmmssff");
                                }

                                replyMessage.Body.Add("TIMEDATE", GetSECSFormat(DataDictinaryList.TIME, SECSItemFormat.A), timeData.Length, timeData);

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                #region [S2F25]
                case 25:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 2 && t.Function == 25);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(2, 26, this._driver.Config.DeviceType);

                                replyMessage.Body.Add(message.Body.Item[0]);

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            replyMessage = null;

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream3
        private AnalyzeMessageError AnalyzePrimaryMessageStream3(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream4
        private AnalyzeMessageError AnalyzePrimaryMessageStream4(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream5
        private AnalyzeMessageError AnalyzePrimaryMessageStream5(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            AckInfo ack;

            UseReplyMessage reply;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S5F1]
                case 1:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 5 && t.Function == 1);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(5, 2, this._driver.Config.DeviceType);

                                ack = this.CurrentSetting.AckCollection[5, 2];

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("ACKC5", GetSECSFormat(DataDictinaryList.ACKC5, SECSItemFormat.B), 1, ack.Value);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ACKC5", GetSECSFormat(DataDictinaryList.ACKC5, SECSItemFormat.B), 1, 0);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            replyMessage = null;

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream6
        private AnalyzeMessageError AnalyzePrimaryMessageStream6(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            AckInfo ack;
            GemDriverError error;

            UseReplyMessage reply;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S6F1]
                case 1:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 6 && t.Function == 1);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(6, 2, this._driver.Config.DeviceType);

                                ack = this.CurrentSetting.AckCollection[6, 2];

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("ACKC6", GetSECSFormat(DataDictinaryList.ACKC6, SECSItemFormat.B), 1, ack.Value);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ACKC6", GetSECSFormat(DataDictinaryList.ACKC6, SECSItemFormat.B), 1, 0);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                #region [S6F11]
                case 11:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 6 && t.Function == 11);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(6, 12, this._driver.Config.DeviceType);

                                ack = this.CurrentSetting.AckCollection[6, 12];

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("ACKC6", GetSECSFormat(DataDictinaryList.ACKC6, SECSItemFormat.B), 1, ack.Value);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ACKC6", GetSECSFormat(DataDictinaryList.ACKC6, SECSItemFormat.B), 1, 0);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }

                            // update recevied value for variables
                            UpdateReceivedVariableValue(message);

                            if (this.IsDirty == true)
                            {
                                error = this.SaveConfigFile(out string errorText);

                                if (string.IsNullOrEmpty(errorText) == false)
                                {
                                    RaiseDriverLogAdded1(this, DriverLogType.WARN, "Equipment Constant is changed. Config file save occurs error");
                                }
                                else
                                {
                                    this.IsDirty = false;
                                }
                            }

                            // update control state and process auto send
                            AnalyzeCEID(message);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            replyMessage = null;

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream7
        private AnalyzeMessageError AnalyzePrimaryMessageStream7(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            AckInfo ack;
            int count;
            byte[] readData;
            int intRand;
            string ppid;
            string mdln;
            string softRev;
            dynamic converted;
            FormattedProcessProgramInfo fmtPPInfo;
            FmtPPCCodeInfo ccodeInfo;
            int ccodeCount;
            string ccode;
            FmtPPItem ppItem;
            int ppcount;
            string ppName;
            string ppValue;
            SECSItem secsItemCCode;
            SECSItem secsItemPP;
            SECSItemFormat ccodeFormat;
            SECSItemFormat ppNameFormat;
            RecipeManager recipeManager;
            string errorText;

            UseReplyMessage reply;
            string path;

            errorText = string.Empty;
            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S7F1]
                case 1:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 1);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(7, 2, this._driver.Config.DeviceType);

                                ack = this.CurrentSetting.AckCollection[7, 2];

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("PPGNT", GetSECSFormat(DataDictinaryList.PPGNT, SECSItemFormat.B), 1, ack.Value);
                                }
                                else
                                {
                                    replyMessage.Body.Add("PPGNT", GetSECSFormat(DataDictinaryList.PPGNT, SECSItemFormat.B), 1, 0);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                #region [S7F3]
                case 3:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 3);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                ack = this.CurrentSetting.AckCollection[7, 4];

                                replyMessage = this._driver.Messages.GetMessageHeader(7, 4, this._driver.Config.DeviceType);

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("ACKC7", GetSECSFormat(DataDictinaryList.ACKC7, SECSItemFormat.B), 1, ack.Value);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ACKC7", GetSECSFormat(DataDictinaryList.ACKC7, SECSItemFormat.B), 1, 0);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }

                            if (this.CurrentSetting.IsSaveRecipeReceived == true)
                            {
                                ppid = message.Body.AsList[1].Value;
                                path = string.Format(@"{0}\{1}.rcp", this.CurrentSetting.RecipeDirectory, ppid);
                                readData = message.Body.AsList[2].Value;
                                File.WriteAllBytes(path, readData);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                #region [S7F5]
                case 5:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 5);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(7, 6, this._driver.Config.DeviceType);

                                ppid = message.Body.Item[0].Value;
                                path = string.Format(@"{0}\{1}.rcp", this.CurrentSetting.RecipeDirectory, ppid);

                                if (File.Exists(path) == true)
                                {
                                    readData = File.ReadAllBytes(path);
                                    count = readData.Length;

                                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                                    replyMessage.Body.Add("PPID", GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A), Encoding.Default.GetByteCount(message.Body.Item[0].Value), message.Body.Item[0].Value);
                                    replyMessage.Body.Add("PPBODY", GetSECSFormat(DataDictinaryList.PPBODY, SECSItemFormat.B), count, readData);
                                }
                                else
                                {
                                    replyMessage.Body.Add(SECSItemFormat.L, 0, null);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);

                                readData = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                #region [S7F19]
                case 19:
                    {
                        result = AnalyzeMessageError.Ok;
                        reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 19);

                        if (reply != null && reply.SendReply == true && message.WaitBit == true)
                        {
                            replyMessage = this._driver.Messages.GetMessageHeader(7, 20, this._driver.Config.DeviceType);

                            count = this._random.Next(1, 20);

                            replyMessage.Body.Add(SECSItemFormat.L, count, null);

                            for (int i = 0; i < count; i++)
                            {
                                intRand = this._random.Next(1, 100);
                                ppid = string.Format("PPID_{0}", intRand);
                                replyMessage.Body.Add(SECSItemFormat.A, Encoding.Default.GetByteCount(ppid), ppid);
                            }

                            this._driver.ReplySECSMessage(message, replyMessage);
                        }
                    }

                    break;
                #endregion
                #region [S7F23]
                case 23:
                    {
                        result = AnalyzeMessageError.Ok;

                        reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 23);

                        if (reply != null && reply.SendReply == true && message.WaitBit == true)
                        {
                            replyMessage = this._driver.Messages.GetMessageHeader(7, 24, this._driver.Config.DeviceType);

                            ack = this.CurrentSetting.AckCollection[7, 24];

                            if (ack != null && ack.Use == true)
                            {
                                replyMessage.Body.Add("ACKC7", GetSECSFormat(DataDictinaryList.ACKC7, SECSItemFormat.B), 1, ack.Value);
                            }
                            else
                            {
                                replyMessage.Body.Add("ACKC7", GetSECSFormat(DataDictinaryList.ACKC7, SECSItemFormat.B), 1, 0);
                            }

                            this._driver.ReplySECSMessage(message, replyMessage);
                        }

                        if (this.CurrentSetting.IsSaveRecipeReceived == true)
                        {
                            ppid = message.Body.AsList[1].Value;
                            mdln = message.Body.AsList[2].Value;
                            softRev = message.Body.AsList[3].Value;

                            fmtPPInfo = this.FormattedProcessProgramCollection.Items.FirstOrDefault(t => t.PPID == ppid);

                            if (fmtPPInfo == null)
                            {
                                fmtPPInfo = new FormattedProcessProgramInfo
                                {
                                    PPID = ppid
                                };
                                this.FormattedProcessProgramCollection.Add(fmtPPInfo);
                            }

                            fmtPPInfo.MDLN = mdln;
                            fmtPPInfo.SOFTREV = softRev;

                            fmtPPInfo.FmtPPCollection.Items.Clear();

                            secsItemCCode = secsItemCCode = message.Body.Item.Items[0].SubItem.Items[3];
                            ccodeCount = secsItemCCode.Length;

                            for (int i = 0; i < ccodeCount; i++)
                            {
                                ccode = secsItemCCode.SubItem[i].SubItem[0].Value;
                                secsItemPP = secsItemCCode.SubItem[i].SubItem[1];
                                ccodeInfo = new FmtPPCCodeInfo
                                {
                                    CommandCode = ccode
                                };
                                fmtPPInfo.FmtPPCollection.Items.Add(ccodeInfo);
                                ppcount = secsItemPP.Length;

                                for (int j = 0; j < ppcount; j++)
                                {
                                    if (secsItemPP.SubItem[j].SubItem.Count == 2)
                                    {
                                        ppName = secsItemPP.SubItem[j].SubItem[0].Value;
                                        ppValue = secsItemPP.SubItem[j].SubItem[1].Value;

                                        ppItem = new FmtPPItem
                                        {
                                            PPName = ppName,
                                            Format = secsItemPP.SubItem[j].SubItem[1].Format
                                        };

                                        if (ppItem.Format == SECSItemFormat.Boolean)
                                        {
                                            if (ppValue == "1")
                                            {
                                                ppItem.PPValue = true.ToString();
                                            }
                                            else
                                            {
                                                ppItem.PPValue = false.ToString();
                                            }
                                        }
                                        else
                                        {
                                            ppItem.PPValue = ppValue;
                                        }

                                        ccodeInfo.Items.Add(ppItem);
                                    }
                                }
                            }
                            recipeManager = new RecipeManager(fmtPPInfo.PPID)
                            {
                                RecipeDirectory = this.CurrentSetting.FormattedRecipeDirectory
                            };
                            recipeManager.Save(true, fmtPPInfo.FmtPPCollection.Items, out errorText);
                        }
                    }

                    break;
                #endregion
                #region [S7F25]
                case 25:
                    {
                        result = AnalyzeMessageError.Ok;

                        reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 25);

                        if (reply != null && reply.SendReply == true && message.WaitBit == true)
                        {
                            ccodeFormat = GetSECSFormat(DataDictinaryList.CCODE, SECSItemFormat.B);
                            ppNameFormat = GetSECSFormat(DataDictinaryList.PARAMNAME, SECSItemFormat.A);

                            ppid = message.Body.Item[0].Value;

                            fmtPPInfo = this.FormattedProcessProgramCollection[ppid];

                            if (fmtPPInfo == null)
                            {
                                fmtPPInfo = new FormattedProcessProgramInfo
                                {
                                    PPID = ppid
                                };
                            }

                            if (fmtPPInfo.FmtPPCollection.Items.Count == 0)
                            {
                                recipeManager = new RecipeManager(ppid)
                                {
                                    RecipeDirectory = this.CurrentSetting.FormattedRecipeDirectory
                                };
                                fmtPPInfo.FmtPPCollection.Items = recipeManager.Load(out errorText);
                                fmtPPInfo.IsLoaded = true;
                            }

                            replyMessage = this._driver.Messages.GetMessageHeader(7, 26, this._driver.Config.DeviceType);

                            replyMessage.Body.Add(SECSItemFormat.L, 4, null);
                            replyMessage.Body.Add("PPID", SECSItemFormat.A, Encoding.Default.GetByteCount(fmtPPInfo.PPID), fmtPPInfo.PPID);
                            replyMessage.Body.Add("MDLN", SECSItemFormat.A, Encoding.Default.GetByteCount(fmtPPInfo.MDLN), fmtPPInfo.MDLN);
                            replyMessage.Body.Add("SOFTREV", SECSItemFormat.A, Encoding.Default.GetByteCount(fmtPPInfo.SOFTREV), fmtPPInfo.SOFTREV);

                            if (fmtPPInfo.FmtPPCollection.Items == null)
                            {
                                replyMessage.Body.Add("COMMANDCOUNT", SECSItemFormat.L, 0, null);
                            }
                            else
                            {
                                replyMessage.Body.Add("COMMANDCOUNT", SECSItemFormat.L, fmtPPInfo.FmtPPCollection.Items.Count, null);

                                foreach (FmtPPCCodeInfo tempFmtPPCCodeInfo in fmtPPInfo.FmtPPCollection.Items)
                                {
                                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                                    if (ccodeFormat == SECSItemFormat.A || ccodeFormat == SECSItemFormat.J)
                                    {
                                        replyMessage.Body.Add("CCODE", ccodeFormat, Encoding.Default.GetByteCount(tempFmtPPCCodeInfo.CommandCode), tempFmtPPCCodeInfo.CommandCode);
                                    }
                                    else
                                    {
                                        converted = ConvertValue(ccodeFormat, tempFmtPPCCodeInfo.CommandCode);

                                        if (converted != null)
                                        {
                                            replyMessage.Body.Add("CCODE", ccodeFormat, 1, new SECSValue(converted));
                                        }
                                        else
                                        {
                                            replyMessage.Body.Add("CCODE", ccodeFormat, 0, string.Empty);
                                        }
                                    }

                                    replyMessage.Body.Add("PPARMCOUNT", SECSItemFormat.L, tempFmtPPCCodeInfo.Items.Count, null);

                                    foreach (FmtPPItem tempFmtPPItem in tempFmtPPCCodeInfo.Items)
                                    {
                                        if (S7F23ExtChecked == true)
                                        {
                                            replyMessage.Body.Add(string.Empty, SECSItemFormat.L, 2, null);
                                            replyMessage.Body.Add("PPNAME", SECSItemFormat.A, tempFmtPPItem.PPName.Length, tempFmtPPItem.PPName);

                                            if (tempFmtPPItem.Format == SECSItemFormat.A || tempFmtPPItem.Format == SECSItemFormat.J)
                                            {
                                                replyMessage.Body.Add("PPVALUE", tempFmtPPItem.Format, Encoding.Default.GetByteCount(tempFmtPPItem.PPValue), tempFmtPPItem.PPValue);
                                            }
                                            else
                                            {
                                                if (tempFmtPPItem.PPValue.IndexOf(" ") >= 0)
                                                {
                                                    AddListToMessage(replyMessage, tempFmtPPItem.Format, "PPVALUE", 0, false, tempFmtPPItem.PPValue, out errorText);
                                                }
                                                else
                                                {
                                                    converted = ConvertValue(tempFmtPPItem.Format, tempFmtPPItem.PPValue);

                                                    if (converted != null)
                                                    {
                                                        replyMessage.Body.Add("PPVALUE", tempFmtPPItem.Format, 1, converted);
                                                    }
                                                    else
                                                    {
                                                        replyMessage.Body.Add("PPVALUE", tempFmtPPItem.Format, 0, string.Empty);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (tempFmtPPItem.Format == SECSItemFormat.A || tempFmtPPItem.Format == SECSItemFormat.J)
                                            {
                                                replyMessage.Body.Add("PPVALUE", tempFmtPPItem.Format, Encoding.Default.GetByteCount(tempFmtPPItem.PPValue), tempFmtPPItem.PPValue);
                                            }
                                            else
                                            {
                                                if (tempFmtPPItem.PPValue.IndexOf(" ") >= 0)
                                                {
                                                    AddListToMessage(replyMessage, tempFmtPPItem.Format, "PPVALUE", 0, false, tempFmtPPItem.PPValue, out errorText);
                                                }
                                                else
                                                {
                                                    converted = ConvertValue(tempFmtPPItem.Format, tempFmtPPItem.PPValue);

                                                    if (converted != null)
                                                    {
                                                        replyMessage.Body.Add("PPVALUE", tempFmtPPItem.Format, 1, converted);
                                                    }
                                                    else
                                                    {
                                                        replyMessage.Body.Add("PPVALUE", tempFmtPPItem.Format, 0, string.Empty);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            result = AnalyzeMessageError.Ok;

                            this._driver.ReplySECSMessage(message, replyMessage);
                        }
                    }

                    break;
                #endregion
                #region [S7F27]
                case 27:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 27);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(7, 28, this._driver.Config.DeviceType);

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                #region [S7F29]
                case 29:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 7 && t.Function == 29);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(7, 30, this._driver.Config.DeviceType);

                                ack = this.CurrentSetting.AckCollection[7, 30];

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("PPGNT", GetSECSFormat(DataDictinaryList.PPGNT, SECSItemFormat.B), 1, ack.Value);
                                }
                                else
                                {
                                    replyMessage.Body.Add("PPGNT", GetSECSFormat(DataDictinaryList.PPGNT, SECSItemFormat.B), 1, 0);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            replyMessage = null;

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream8
        private AnalyzeMessageError AnalyzePrimaryMessageStream8(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream9
        private AnalyzeMessageError AnalyzePrimaryMessageStream9(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 9:
                case 11:
                case 13:
                    result = AnalyzeMessageError.Ok;
                    break;
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream10
        private AnalyzeMessageError AnalyzePrimaryMessageStream10(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            AckInfo ack;

            UseReplyMessage reply;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S10F1]
                case 1:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            reply = this.CurrentSetting.UseReplyCollection.FirstOrDefault(t => t.Stream == 10 && t.Function == 1);

                            if (reply != null && reply.SendReply == true && message.WaitBit == true)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(10, 2, this._driver.Config.DeviceType);

                                ack = this.CurrentSetting.AckCollection[10, 2];

                                if (ack != null && ack.Use == true)
                                {
                                    replyMessage.Body.Add("ACKC10", GetSECSFormat(DataDictinaryList.ACKC10, SECSItemFormat.B), 1, ack.Value);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ACKC10", GetSECSFormat(DataDictinaryList.ACKC10, SECSItemFormat.B), 1, 0);
                                }

                                this._driver.ReplySECSMessage(message, replyMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    break;
                #endregion
                case 7:
                    result = AnalyzeMessageError.Ok;
                    break;
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            replyMessage = null;

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream11
        private AnalyzeMessageError AnalyzePrimaryMessageStream11(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream12
        private AnalyzeMessageError AnalyzePrimaryMessageStream12(SECSMessage message)
        {
            AnalyzeMessageError result;

            SECSMessage replyMessage;

            switch (message.Function)
            {
                #region [S12F1]
                case 1:
                    result = ReplyMessageS12F2(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F3]
                case 3:
                    result = ReplyMessageS12F4(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F5]
                case 5:
                    result = ReplyMessageS12F6(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F7]
                case 7:
                    result = ReplyMessageS12F8(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F9]
                case 9:
                    result = ReplyMessageS12F10(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F11]
                case 11:
                    result = ReplyMessageS12F12(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F13]
                case 13:
                    result = ReplyMessageS12F14(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F15]
                case 15:
                    result = ReplyMessageS12F16(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F17]
                case 17:
                    result = ReplyMessageS12F18(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(12, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S12F19]
                case 19:
                    result = AnalyzeMessageError.Ok;
                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream13
        private AnalyzeMessageError AnalyzePrimaryMessageStream13(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream14
        private AnalyzeMessageError AnalyzePrimaryMessageStream14(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;

            switch (message.Function)
            {
                #region [S14F1]
                case 1:
                    result = ReplyMessageS14F2(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F3]
                case 3:
                    this.IsDirty = true;

                    result = ReplyMessageS14F4(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F5]
                case 5:
                    result = ReplyMessageS14F6(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F7]
                case 7:
                    result = ReplyMessageS14F8(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F9]
                case 9:
                    result = ReplyMessageS14F10(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F11]
                case 11:
                    result = ReplyMessageS14F12(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F13]
                case 13:
                    result = ReplyMessageS14F14(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F15]
                case 15:
                    result = ReplyMessageS14F16(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                #region [S14F17]
                case 17:
                    result = ReplyMessageS14F18(message);

                    if (result == AnalyzeMessageError.Unknown)
                    {
                        replyMessage = this._driver.Messages.GetMessageHeader(14, 0, this._driver.Config.DeviceType);
                        replyMessage.SystemBytes = message.SystemBytes;

                        this._driver.SendSECSMessage(replyMessage);
                    }
                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            if (this.OnGEMObjectDataChanged != null)
            {
                this.OnGEMObjectDataChanged.BeginInvoke(null, null);
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream15
        private AnalyzeMessageError AnalyzePrimaryMessageStream15(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream16
        private AnalyzeMessageError AnalyzePrimaryMessageStream16(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzePrimaryMessageStream17
        private AnalyzeMessageError AnalyzePrimaryMessageStream17(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }
        #endregion
        #region AnalyzeCEID
        private void AnalyzeCEID(SECSMessage message)
        {
            CollectionEventInfo collectionEventInfo;
            string ceid;
            ControlState receivedControlState;

            if (message.Body.Item != null && message.Body.AsList.Count > 0)
            {
                ceid = message.Body.Item.Items[0].SubItem[1].Value;

                collectionEventInfo = this.CollectionEventCollection[ceid];

                if (collectionEventInfo != null)
                {
                    #region Check ControlState
                    switch (collectionEventInfo.Name)
                    {
                        case "Offline":
                        case "OfflineOnHost":
                        case "OnlineLocal":
                        case "OnlineRemote":
                        case "ControlStateChanged":
                            #region ControlState
                            {
                                if (collectionEventInfo.Name == "Offline")
                                {
                                    receivedControlState = ControlState.EquipmentOffline;
                                }
                                else if (collectionEventInfo.Name == "OfflineOnHost")
                                {
                                    receivedControlState = ControlState.HostOffline;
                                }
                                else if (collectionEventInfo.Name == "OnlineLocal")
                                {
                                    receivedControlState = ControlState.OnlineLocal;
                                }
                                else if (collectionEventInfo.Name == "OnlineRemote")
                                {
                                    receivedControlState = ControlState.OnlineRemote;
                                }
                                else
                                {
                                    bool resultGetValue;

                                    resultGetValue = GetVValueByS6F11(message, collectionEventInfo, null, PreDefinedV.ControlState.ToString(), out object value, out string errorText);

                                    if (resultGetValue == true)
                                    {
                                        receivedControlState = (ControlState)int.Parse(value.ToString());
                                    }
                                    else
                                    {
                                        receivedControlState = ControlState.EquipmentOffline;

                                        RaiseDriverLogAdded1(this, DriverLogType.INFO, errorText);
                                    }
                                }

                                if (this.ControlState != receivedControlState)
                                {
                                    this.ControlStateBefore = this.ControlState;
                                    this.ControlState = receivedControlState;

                                    RaiseControlStateChanged(this.ControlState);

                                    // Offline > Online 전환 시
                                    if ((this.ControlStateBefore == ControlState.EquipmentOffline || this.ControlStateBefore == ControlState.HostOffline)
                                        && (this.ControlState == ControlState.OnlineLocal || this.ControlState == ControlState.OnlineRemote))
                                    {
                                        if (this.CurrentSetting.AutoSendDefineReport == true && this._isProcessAutoSendOnlineRunning == false)
                                        {
                                            this._isProcessAutoSendOnlineRunning = true;
                                            this._isProcessAutoSendOnlineStop = false;
                                            this.CurrentDefineReportSequence = DefineReportSequence.Start;
                                            ProcessAutoSendOnline();
                                        }
                                    }
                                }

                            }
                            #endregion
                            break;
                    }
                    #endregion

                    ProcessAutoSend(message);
                }
            }
        }
        #endregion

        // Private Method
        #region GetSECSFormat
        public SECSItemFormat GetSECSFormat(DataDictinaryList dataDictinary, SECSItemFormat defaultFormat = SECSItemFormat.U2)
        {
            SECSItemFormat result;
            DataDictionaryInfo dataDictionaryInfo;

            dataDictionaryInfo = this.DataDictionaryCollection[dataDictinary.ToString()];
            result = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : defaultFormat;

            return result;
        }
        #endregion
        #region ProcessAuto
        private void ProcessAutoSend(SECSMessage message)
        {
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;

            bool autoSend;

            #region Auto Send Remote Command
            foreach (var commandItem in this.RemoteCommandCollection.RemoteCommandItems)
            {
                expandedRemoteCommandInfo = commandItem as ExpandedRemoteCommandInfo;

                if (expandedRemoteCommandInfo != null)
                {
                    foreach (AutoSendTrigger trigger in expandedRemoteCommandInfo.TriggerCollection.Items.OrderBy(t => t.TriggerID))
                    {
                        autoSend = CheckAutoSend(message, trigger);

                        if (expandedRemoteCommandInfo.AutoSend == true && autoSend == true && expandedRemoteCommandInfo.ValueSetCollection["Default"] != null)
                        {
                            SendS2F41(expandedRemoteCommandInfo, expandedRemoteCommandInfo.ValueSetCollection["Default"]);
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
            #endregion

            #region Auto Send Enhanced Remote Command
            foreach (var enhancedCommandItem in this.RemoteCommandCollection.EnhancedRemoteCommandItems)
            {
                expandedEnhancedRemoteCommandInfo = enhancedCommandItem as ExpandedEnhancedRemoteCommandInfo;

                if (expandedEnhancedRemoteCommandInfo != null)
                {
                    foreach (AutoSendTrigger trigger in expandedEnhancedRemoteCommandInfo.TriggerCollection.Items.OrderBy(t => t.TriggerID))
                    {
                        autoSend = CheckAutoSend(message, trigger);

                        if (expandedEnhancedRemoteCommandInfo.AutoSend == true && autoSend == true && expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"] != null)
                        {
                            SendS2F49(expandedEnhancedRemoteCommandInfo, expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"]);
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
            #endregion

            #region TraceData
            foreach (var traceInfo in this.TraceCollection.Items)
            {
                foreach (AutoSendTrigger trigger in traceInfo.SendTriggerCollection.Items.OrderBy(t => t.TriggerID))
                {
                    autoSend = CheckAutoSend(message, trigger);

                    if (traceInfo.AutoSend == true && autoSend == true)
                    {
                        SendS2F23(traceInfo);
                        System.Threading.Thread.Sleep(100);
                    }
                }

                foreach (AutoSendTrigger trigger in traceInfo.StopTriggerCollection.Items.OrderBy(t => t.TriggerID))
                {
                    autoSend = CheckAutoSend(message, trigger);

                    if (traceInfo.AutoStop == true && autoSend == true)
                    {
                        SendS2F23Stop(traceInfo);
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            #endregion

            #region LimitMonitoring
            foreach (var monitoringInfo in this.LimitMonitoringCollection.Items)
            {
                foreach (AutoSendTrigger trigger in monitoringInfo.TriggerCollection.Items.OrderBy(t => t.TriggerID))
                {
                    autoSend = CheckAutoSend(message, trigger);

                    if (monitoringInfo.AutoSend == true && autoSend == true)
                    {
                        SendS2F45(monitoringInfo);
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            #endregion

            #region Process Program
            foreach (var fmtPPInfo in this.FormattedProcessProgramCollection.Items)
            {
                foreach (AutoSendTrigger trigger in fmtPPInfo.TriggerCollection.Items.OrderBy(t => t.TriggerID))
                {
                    autoSend = CheckAutoSend(message, trigger);

                    if (fmtPPInfo.AutoSend == true && autoSend == true)
                    {
                        SendS7F23(fmtPPInfo.PPID);
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            #endregion
        }
        #endregion
        #region CheckAutoSend
        private bool CheckAutoSend(SECSMessage message, AutoSendTrigger trigger)
        {
            bool result;

            string ceid;

            result = false;
            bool resultGetValue;

            if (message.Body.Item.Items[0].SubItem.Count == 3)
            {
                ceid = message.Body.Item.Items[0].SubItem[1].Value;

                if (trigger.TriggerMode == TriggerMode.CollectionEvent)
                {
                    if (trigger.CollectionEventInfo != null && ceid == trigger.CollectionEventInfo.CEID)
                    {
                        result = true;
                    }
                }
                else if (trigger.TriggerMode == TriggerMode.Variable)
                {
                    if (trigger.CollectionEventInfo != null && ceid == trigger.CollectionEventInfo.CEID && trigger.ReportInfo != null && trigger.VariableStack.Last() != null)
                    {
                        resultGetValue = GetVValueByS6F11(message, trigger.CollectionEventInfo, trigger.ReportInfo.ReportID, trigger.VariableStack.Last().Name, out object value, out _);
                        if (resultGetValue == true)
                        {
                            if (value.ToString() == trigger.VariableValue)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region ProcessAutoSendOnline
        private void ProcessAutoSendOnline()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                while (true)
                {
                    if (this._isProcessAutoSendOnlineStop == true || this.CurrentDefineReportSequence == DefineReportSequence.End)
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(100);

                    this.BeforeDefineReportSequence = this.CurrentDefineReportSequence;

                    switch (this.CurrentDefineReportSequence)
                    {
                        case DefineReportSequence.Start:
                            this.CurrentDefineReportSequence = DefineReportSequence.SendAlarmDisable;
                            break;

                        case DefineReportSequence.SendAlarmDisable:
                            SendS5F3(false);
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitAlarmDisable;
                            break;

                        case DefineReportSequence.WaitAlarmDisable:
                            break;

                        case DefineReportSequence.SendS2F37Disable:
                            SendS2F37Disable();
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitS2F38Disable;
                            break;

                        case DefineReportSequence.WaitS2F38Disable:
                            break;

                        case DefineReportSequence.SendS2F35Disable:
                            SendS2F35Disable();
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitS2F36Disable;
                            break;

                        case DefineReportSequence.WaitS2F36Disable:
                            break;

                        case DefineReportSequence.SendS2F33Disable:
                            SendS2F33Disable();
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitS2F34Disable;
                            break;

                        case DefineReportSequence.WaitS2F34Disable:
                            break;

                        case DefineReportSequence.SendS2F33Enable:
                            SendS2F33();
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitS2F34Enable;
                            break;

                        case DefineReportSequence.WaitS2F34Enable:
                            break;

                        case DefineReportSequence.SendS2F35Enable:
                            SendS2F35();
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitS2F36Enable;
                            break;

                        case DefineReportSequence.WaitS2F36Enable:
                            break;

                        case DefineReportSequence.SendS2F37Enable:
                            SendS2F37Enable();
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitS2F38Enable;
                            break;

                        case DefineReportSequence.WaitS2F38Enable:
                            break;

                        case DefineReportSequence.SendAlarmEnable:
                            SendS5F3(true);
                            this.CurrentDefineReportSequence = DefineReportSequence.WaitAlarmEnable;
                            break;

                        case DefineReportSequence.WaitAlarmEnable:
                            break;
                    }
                }

                this._isProcessAutoSendOnlineRunning = false;
                this._isProcessAutoSendOnlineStop = false;

                this.CurrentDefineReportSequence = DefineReportSequence.End;
            })).Start();
        }
        #endregion
        #region IsValidGenerateRule
        public bool IsValidGenerateRule(SECSItemFormat format, string data)
        {
            bool result;

            string[] split;
            string stringValue;
            string generateType;
            string id;
            ExpandedVariableInfo variableInfo;

            result = true;

            List<int> openBraceIndices;
            List<int> closeBraceIndices;

            int currentOpenIndex;
            int currentCloseIndex;
            int lastCloseIndex;
            int rulePartLength;
            string ruleForCheck;

            if (string.IsNullOrEmpty(data) == false)
            {
                openBraceIndices = new List<int>();
                closeBraceIndices = new List<int>();

                openBraceIndices.Add(data.IndexOf("{"));
                closeBraceIndices.Add(data.IndexOf("}"));

                if (openBraceIndices[0] == -1 || closeBraceIndices[0] == -1)
                {
                    result = false;
                }

                if (result == true)
                {
                    while (openBraceIndices.Count(t => t == -1) < 1)
                    {
                        openBraceIndices.Add(data.IndexOf("{", openBraceIndices.Last() + 1));
                    }

                    while (closeBraceIndices.Count(t => t == -1) < 1)
                    {
                        closeBraceIndices.Add(data.IndexOf("}", closeBraceIndices.Last() + 1));
                    }
                }

                if (openBraceIndices.Count != closeBraceIndices.Count)
                {
                    result = false;
                }

                if (result == true)
                {
                    if (format != SECSItemFormat.A && openBraceIndices.Count > 2)
                    {
                        result = false;
                    }
                }

                if (result == true)
                {
                    for (int i = 0; i < openBraceIndices.Count - 1; ++i)
                    {
                        if (openBraceIndices[i] >= closeBraceIndices[i] || (i < openBraceIndices.Count - 2 && openBraceIndices[i + 1] <= closeBraceIndices[i]))
                        {
                            result = false;
                            break;
                        }
                    }
                }

                if (result == true)
                {
                    lastCloseIndex = 0;

                    for (int i = 0; i < openBraceIndices.Count - 1; ++i)
                    {
                        currentOpenIndex = openBraceIndices[i];
                        currentCloseIndex = closeBraceIndices[i];

                        rulePartLength = currentCloseIndex - currentOpenIndex;
                        ruleForCheck = data.Substring(currentOpenIndex + 1, rulePartLength - 1);

                        if (result == true)
                        {
                            split = ruleForCheck.Split(':');

                            if (split.Count() == 1)
                            {
                                result = false;
                            }
                            else
                            {
                                generateType = split[0].ToUpper();

                                // check VID and EC
                                if (generateType == "VID" || generateType == "EC")
                                {
                                    #region check VID and EC
                                    if (split.Count() != 2)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        id = split[1];

                                        if (string.IsNullOrEmpty(id) == true)
                                        {
                                            result = false;
                                        }
                                        else
                                        {
                                            if (generateType == "VID")
                                            {
                                                variableInfo = this.VariableCollection.Variables[id] as ExpandedVariableInfo;

                                                if (variableInfo == null || variableInfo.Format == SECSItemFormat.L)
                                                {
                                                    result = false;
                                                }
                                            }
                                            else if (generateType == "EC")
                                            {
                                                variableInfo = this.VariableCollection.ECV[id] as ExpandedVariableInfo;

                                                if (variableInfo == null)
                                                {
                                                    result = false;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else if (generateType == "RAND")
                                {
                                    #region check RAND
                                    if (split.Count() != 3)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        stringValue = split[1];

                                        if (int.TryParse(stringValue, out int min) == false)
                                        {
                                            result = false;
                                        }
                                        else
                                        {
                                            stringValue = split[2];

                                            if (int.TryParse(stringValue, out int max) == false)
                                            {
                                                result = false;
                                            }
                                            else
                                            {
                                                if (min > max || min == max)
                                                {
                                                    result = false;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else if (generateType == "INC")
                                {
                                    #region check INC
                                    if (split.Count() != 3)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        stringValue = split[1];

                                        if (int.TryParse(stringValue, out int start) == false)
                                        {
                                            result = false;
                                        }
                                        else
                                        {
                                            stringValue = split[2];
                                            if (int.TryParse(stringValue, out int step) == false)
                                            {
                                                result = false;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                        }

                        lastCloseIndex = currentCloseIndex + 1;
                    }
                }
            }

            return result;
        }
        #endregion
        #region GenerateList
        private dynamic GenerateList(SECSItemFormat format, int count, string generateRule)
        {
            dynamic result;
            dynamic converted;
            int generatedCount;
            long lastData;

            result = null;

            lastData = 0;

            switch (format)
            {
                case SECSItemFormat.B:
                case SECSItemFormat.U1:
                    result = new List<byte>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((byte)0);
                        }
                    }

                    break;
                case SECSItemFormat.U2:
                    result = new List<ushort>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((ushort)0);
                        }
                    }

                    break;
                case SECSItemFormat.U4:
                    result = new List<uint>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((uint)0);
                        }
                    }

                    break;
                case SECSItemFormat.U8:
                    result = new List<ulong>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((ulong)0);
                        }
                    }

                    break;
                case SECSItemFormat.I1:
                    result = new List<sbyte>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((sbyte)0);
                        }
                    }

                    break;
                case SECSItemFormat.I2:
                    result = new List<short>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((short)0);
                        }
                    }

                    break;
                case SECSItemFormat.I4:
                    result = new List<int>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((int)0);
                        }
                    }

                    break;
                case SECSItemFormat.I8:
                    result = new List<long>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((long)0);
                        }
                    }

                    break;
                case SECSItemFormat.F4:
                    result = new List<float>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((float)0);
                        }
                    }

                    break;
                case SECSItemFormat.F8:
                    result = new List<double>();

                    while (true)
                    {
                        generatedCount = result.Count;

                        if (generatedCount == count)
                        {
                            break;
                        }

                        converted = GenerateValue(format, generateRule, lastData);

                        if (converted != null)
                        {
                            result.Add(converted);
                            lastData = converted;
                        }
                        else
                        {
                            result.Add((double)0);
                        }
                    }

                    break;
            }

            return result;
        }
        #endregion
        #region GenerateValue
        private object GenerateValue(SECSItemFormat format, string generateRule, long lastData)
        {
            object result;
            string[] split;
            string generateType;
            string id;
            int min;
            int max;
            int randValue;
            long incValue;
            ExpandedVariableInfo variableInfo;

            bool continueGenerate;

            string ruleForCheck;
            string rule;
            string patternBefore;
            string patternAfter;

            result = null;

            patternBefore = string.Empty;
            patternAfter = string.Empty;

            continueGenerate = true;

            List<int> openBraceIndices;
            List<int> closeBraceIndices;

            int lastCloseIndex;
            int currentOpenIndex;
            int currentCloseIndex;

            List<string> generatedStrings;
            string generated;
            int rulePartLength;

            if (format == SECSItemFormat.L)
            {
                result = ConvertValue(format, string.Empty);
            }
            else
            {
                generatedStrings = new List<string>();
                if (string.IsNullOrEmpty(generateRule) == true)
                {
                    result = ConvertValue(format, string.Empty);
                }
                else
                {
                    openBraceIndices = new List<int>();
                    closeBraceIndices = new List<int>();

                    openBraceIndices.Add(generateRule.IndexOf("{"));
                    closeBraceIndices.Add(generateRule.IndexOf("}"));

                    if (openBraceIndices[0] == -1 || closeBraceIndices[0] == -1)
                    {
                        result = ConvertValue(format, string.Empty);
                        continueGenerate = false;
                    }

                    if (continueGenerate == true)
                    {
                        while (openBraceIndices.Count(t => t == -1) < 1)
                        {
                            openBraceIndices.Add(generateRule.IndexOf("{", openBraceIndices.Last() + 1));
                        }

                        while (closeBraceIndices.Count(t => t == -1) < 1)
                        {
                            closeBraceIndices.Add(generateRule.IndexOf("}", closeBraceIndices.Last() + 1));
                        }
                    }

                    if (openBraceIndices.Count != closeBraceIndices.Count)
                    {
                        result = ConvertValue(format, string.Empty);
                        continueGenerate = false;
                    }

                    if (continueGenerate == true)
                    {
                        if (format != SECSItemFormat.A && openBraceIndices.Count > 2)
                        {
                            result = ConvertValue(format, string.Empty);
                            continueGenerate = false;
                        }
                    }

                    if (continueGenerate == true)
                    {
                        for (int i = 0; i < openBraceIndices.Count - 1; ++i)
                        {
                            if (openBraceIndices[i] >= closeBraceIndices[i] || (i < openBraceIndices.Count - 2 && openBraceIndices[i + 1] <= closeBraceIndices[i]))
                            {
                                result = ConvertValue(format, string.Empty);
                                continueGenerate = false;
                                break;
                            }
                        }
                    }

                    if (continueGenerate == true)
                    {
                        lastCloseIndex = 0;

                        for (int i = 0; i < openBraceIndices.Count - 1; ++i)
                        {
                            currentOpenIndex = openBraceIndices[i];
                            currentCloseIndex = closeBraceIndices[i];

                            if (currentOpenIndex - lastCloseIndex > 0)
                            {
                                generatedStrings.Add(generateRule.Substring(lastCloseIndex, currentOpenIndex - lastCloseIndex));
                            }

                            rulePartLength = currentCloseIndex - currentOpenIndex;
                            rule = generateRule.Substring(currentOpenIndex, rulePartLength + 1);
                            ruleForCheck = generateRule.Substring(currentOpenIndex + 1, rulePartLength - 1);

                            if (IsValidGenerateRule(format, rule) == false)
                            {
                                result = ConvertValue(format, string.Empty);
                                break;
                            }
                            else
                            {
                                split = ruleForCheck.Split(':');

                                generateType = split[0].ToUpper();

                                if (generateType == "VID" || generateType == "EC")
                                {
                                    #region Generate VID and EC
                                    id = split[1];

                                    if (generateType == "VID")
                                    {
                                        variableInfo = this.VariableCollection.Variables[id] as ExpandedVariableInfo;

                                        if (variableInfo != null)
                                        {
                                            generated = ConvertValue(format, variableInfo.ReceivedValue).ToString();
                                            if (generated == null)
                                            {
                                                result = ConvertValue(format, string.Empty);
                                                break;
                                            }
                                            else
                                            {
                                                generatedStrings.Add(generated);
                                            }
                                        }
                                        else
                                        {
                                            result = ConvertValue(format, string.Empty);
                                            break;
                                        }
                                    }
                                    else if (generateType == "EC")
                                    {
                                        variableInfo = this.VariableCollection.ECV[id] as ExpandedVariableInfo;

                                        if (variableInfo != null)
                                        {
                                            generated = ConvertValue(format, variableInfo.Value).ToString();

                                            if (generated == null)
                                            {
                                                result = ConvertValue(format, string.Empty);
                                                break;
                                            }
                                            else
                                            {
                                                generatedStrings.Add(generated);
                                            }
                                        }
                                        else
                                        {
                                            result = ConvertValue(format, string.Empty);
                                            break;
                                        }
                                    }
                                    #endregion
                                }
                                else if (generateType == "RAND")
                                {
                                    #region Generate RAND
                                    min = int.Parse(split[1]);
                                    max = int.Parse(split[2]);
                                    randValue = this._random.Next(min, max + 1);

                                    switch (format)
                                    {
                                        case SECSItemFormat.U1:
                                        case SECSItemFormat.B:
                                            randValue %= byte.MaxValue;
                                            break;
                                        case SECSItemFormat.U2:
                                            randValue %= ushort.MaxValue;
                                            break;
                                        case SECSItemFormat.I1:
                                            randValue %= sbyte.MaxValue;
                                            break;
                                        case SECSItemFormat.I2:
                                            randValue %= short.MaxValue;
                                            break;
                                    }

                                    generated = ConvertValue(format, randValue.ToString()).ToString();

                                    if (generated == null)
                                    {
                                        result = ConvertValue(format, string.Empty);
                                        break;
                                    }
                                    else
                                    {
                                        generatedStrings.Add(generated);
                                    }

                                    #endregion
                                }
                                else if (generateType == "INC")
                                {
                                    #region Generate INC
                                    if (long.TryParse(split[1], out long start) == false || int.TryParse(split[2], out int step) == false)
                                    {
                                        result = ConvertValue(format, string.Empty);
                                    }
                                    else
                                    {
                                        incValue = lastData;

                                        if (incValue == 0)
                                        {
                                            incValue = start;
                                        }
                                        else
                                        {
                                            incValue = lastData + step;
                                        }

                                        switch (format)
                                        {
                                            case SECSItemFormat.U1:
                                            case SECSItemFormat.B:
                                                incValue %= byte.MaxValue;
                                                break;
                                            case SECSItemFormat.U2:
                                                incValue %= ushort.MaxValue;
                                                break;
                                            case SECSItemFormat.U4:
                                                incValue &= uint.MaxValue;
                                                break;
                                            case SECSItemFormat.I1:
                                                incValue %= sbyte.MaxValue;
                                                break;
                                            case SECSItemFormat.I2:
                                                incValue %= short.MaxValue;
                                                break;
                                            case SECSItemFormat.I4:
                                                incValue &= int.MaxValue;
                                                break;
                                        }

                                        generated = ConvertValue(format, incValue.ToString()).ToString();

                                        if (generated == null)
                                        {
                                            result = ConvertValue(format, string.Empty);
                                            break;
                                        }
                                        else
                                        {
                                            generatedStrings.Add(generated.ToString());
                                        }

                                    }
                                    #endregion
                                }
                                else
                                {
                                    result = ConvertValue(format, string.Empty);
                                    break;
                                }
                            }

                            lastCloseIndex = currentCloseIndex + 1;
                        }

                        
                    }
                }

                if (result == null)
                {
                    result = string.Join("", generatedStrings);

                    if (format != SECSItemFormat.A)
                    {
                        result = ConvertValue(format, result.ToString());
                    }
                }
            }

            return result;
        }
        #endregion
        #region GetVValueByS1F4
        private bool GetVValueByS1F4(SECSMessage primaryMessage, SECSMessage secondaryMessage, PreDefinedV preDefineV, out object value, out string errorText)
        {
            bool result;
            ExpandedVariableInfo preDefinedVInfo;
            List<SECSItem> primaryBodyItems;
            List<SECSItem> secondaryBodyItems;
            SECSItem primaryItem;
            SECSItem secondaryItem;

            result = false;
            value = null;
            errorText = string.Empty;

            if (primaryMessage == null)
            {
                errorText = "primary message is null";
            }

            if (string.IsNullOrEmpty(errorText) == true)
            {
                if (secondaryMessage == null)
                {
                    errorText = "secondary message is null";
                }
            }

            if (string.IsNullOrEmpty(errorText) == true)
            {
                preDefinedVInfo = this.VariableCollection.Items.FirstOrDefault(t => t.PreDefined == true && t.Name == preDefineV.ToString()) as ExpandedVariableInfo;

                if (preDefinedVInfo != null)
                {
                    primaryBodyItems = primaryMessage.Body.AsList;
                    secondaryBodyItems = secondaryMessage.Body.AsList;

                    if (primaryBodyItems.Count == secondaryBodyItems.Count)
                    {
                        for (int i = 0; i < primaryBodyItems.Count; i++)
                        {
                            primaryItem = primaryBodyItems[i];
                            secondaryItem = secondaryBodyItems[i];

                            if (primaryItem.Value != null && secondaryItem.Value != null && primaryItem.Value.ToString() == preDefinedVInfo.VID)
                            {
                                result = true;
                                value = secondaryItem.Value.GetValue();
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region UpdateReceivedVariableValue
        private void UpdateReceivedVariableValue(SECSMessage message)
        {
            CollectionEventInfo collectionEventInfo;
            string ceid;
            string logText;

            if (message.Body.Item != null && message.Body.AsList.Count > 0)
            {
                ceid = message.Body.Item.Items[0].SubItem[1].Value;

                logText = string.Format("Received CEID={0}", ceid);

                collectionEventInfo = this.CollectionEventCollection[ceid];

                if (collectionEventInfo != null)
                {
                    logText = string.Format("{0}, Name={1}", logText, collectionEventInfo.Name);
                    RaiseDriverLogAdded1(this, DriverLogType.RECV, logText);

                    UpdateVValueByS6F11(message, collectionEventInfo);

                }
                else
                {
                    logText = string.Format("{0}, Collection Event Not found", logText);
                    RaiseDriverLogAdded1(this, DriverLogType.RECV, logText);
                }
            }
        }
        #endregion
        #region UpdateVValueByS6F11
        private void UpdateVValueByS6F11(SECSMessage secsMessage, CollectionEventInfo collectionEventInfo)
        {
            SECSItem reports;
            ReportInfo reportInfo;
            string reportId;

            try
            {
                reports = secsMessage.Body.Item[0].SubItem[2];

                if (reports != null && reports.Format == SECSItemFormat.L && reports.Length > 0)
                {
                    if (collectionEventInfo.Reports.Items.Count == reports.Length)
                    {
                        foreach (SECSItem tempSECSItem in reports.SubItem.Items)
                        {
                            reportId = tempSECSItem.SubItem[0].Value;
                            reportInfo = collectionEventInfo.Reports[reportId];

                            if (reportInfo != null)
                            {
                                UpdateVValueByReportInfo(tempSECSItem.SubItem[1], reportInfo);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
        #region UpdateVValueByReportInfo
        private void UpdateVValueByReportInfo(SECSItem report, ReportInfo reportInfo)
        {
            ExpandedVariableInfo updateVariableInfo;
            SECSItem changedECIDItem;
            SECSItem changedECVItem;

            changedECIDItem = null;
            changedECVItem = null;

            try
            {
                if (report != null && report.Format == SECSItemFormat.L && report.Length > 0 && report.Length == reportInfo.Variables.Items.Count)
                {
                    for (int i = 0; i < report.Length; i++)
                    {
                        if (report.SubItem[i].Format == SECSItemFormat.L)
                        {
                            if (reportInfo.Variables.Items[i].Name == PreDefinedV.ChangedECList.ToString())
                            {
                                UpdateReceivedECList(report.SubItem[i]);
                            }
                            else if (reportInfo.Variables.Items[i].Name == PreDefinedV.ChangedECID.ToString())
                            {
                                changedECIDItem = report.SubItem[i];
                            }
                            else if (reportInfo.Variables.Items[i].Name == PreDefinedV.ChangedECID.ToString())
                            {
                                changedECVItem = report.SubItem[i];
                            }
                            else
                            {
                                UpdateVValueByVariableInfo(report.SubItem[i], reportInfo.Variables.Items[i] as ExpandedVariableInfo);
                            }
                        }
                        else
                        {
                            updateVariableInfo = this.VariableCollection[reportInfo.Variables.Items[i].VID] as ExpandedVariableInfo;

                            if (updateVariableInfo != null)
                            {
                                if (report.SubItem[i].Value.GetValue() != null)
                                {
                                    updateVariableInfo.ReceivedValue = report.SubItem[i].Value.GetValue().ToString();

                                    if (updateVariableInfo.VIDType == VariableType.ECV)
                                    {
                                        updateVariableInfo.Value = report.SubItem[i].Value.GetValue().ToString();

                                        UpdateSystemConfig(updateVariableInfo.Name, report.SubItem[i].Value.GetValue().ToString());
                                    }
                                }
                            }
                        }
                    }

                    // Update EC
                    if (changedECIDItem != null && changedECVItem != null && changedECIDItem.Length == changedECVItem.Length)
                    {
                        UpdateReceivedEC(changedECIDItem, changedECVItem);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
        #region UpdateVValueByVariableInfo
        private void UpdateVValueByVariableInfo(SECSItem variable, ExpandedVariableInfo variableInfo)
        {
            ExpandedVariableInfo updateVariableInfo;
            SECSItem changedECIDItem;
            SECSItem changedECVItem;

            changedECIDItem = null;
            changedECVItem = null;

            try
            {
                if (variable != null && variable.Format == SECSItemFormat.L && variable.Length > 0 && variable.Length == variableInfo.ChildVariables.Count)
                {
                    for (int i = 0; i < variable.Length; i++)
                    {
                        if (variable.SubItem[i].Format == SECSItemFormat.L)
                        {
                            if (variableInfo.ChildVariables[i].Name == PreDefinedV.ChangedECList.ToString())
                            {
                                UpdateReceivedECList(variable.SubItem[i]);
                            }
                            else if (variableInfo.ChildVariables[i].Name == PreDefinedV.ChangedECID.ToString())
                            {
                                changedECIDItem = variable.SubItem[i];
                            }
                            else if (variableInfo.ChildVariables[i].Name == PreDefinedV.ChangedECID.ToString())
                            {
                                changedECVItem = variable.SubItem[i];
                            }
                            else
                            {
                                UpdateVValueByVariableInfo(variable.SubItem[i], variableInfo.ChildVariables[i]);
                            }
                        }
                        else
                        {
                            updateVariableInfo = this.VariableCollection[variableInfo.ChildVariables[i].VID] as ExpandedVariableInfo;

                            if (updateVariableInfo != null)
                            {
                                if (variable.SubItem[i].Value.GetValue() != null)
                                {
                                    updateVariableInfo.ReceivedValue = variable.SubItem[i].Value.GetValue().ToString();

                                    if (updateVariableInfo.VIDType == VariableType.ECV)
                                    {
                                        updateVariableInfo.Value = variable.SubItem[i].Value.GetValue().ToString();

                                        UpdateSystemConfig(updateVariableInfo.Name, variable.SubItem[i].Value.GetValue().ToString());
                                    }
                                }
                            }
                        }
                    }

                    // Update EC
                    if (changedECIDItem != null && changedECVItem != null && changedECIDItem.Length == changedECVItem.Length)
                    {
                        UpdateReceivedEC(changedECIDItem, changedECVItem);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
        #region UpdateReceivedEC
        private void UpdateReceivedEC(SECSItem changedECIDItem, SECSItem changedECVItem)
        {
            ExpandedVariableInfo updateVariableInfo;
            int changedECCount;
            string changedECID;
            string changedECValue;

            changedECCount = changedECIDItem.Length;

            for (int i = 0; i < changedECCount; i++)
            {
                if (changedECIDItem.SubItem[i].Format != SECSItemFormat.L && changedECVItem.SubItem[i].Format != SECSItemFormat.L)
                {
                    changedECID = changedECIDItem.SubItem[i].Value.ToString();
                    changedECValue = changedECVItem.SubItem[i].Value.ToString();

                    updateVariableInfo = this.VariableCollection.ECV[changedECID] as ExpandedVariableInfo;

                    if (updateVariableInfo != null)
                    {
                        updateVariableInfo.Value = changedECValue;

                        UpdateSystemConfig(updateVariableInfo.Name, changedECValue);
                    }
                }
            }
        }
        #endregion
        #region UpdateReceivedECList
        private void UpdateReceivedECList(SECSItem variable)
        {
            ExpandedVariableInfo updateVariableInfo;
            SECSItem changedECItem;
            int changedECCount;
            string changedECID;
            string changedECValue;

            changedECCount = variable.Length;

            for (int i = 0; i < changedECCount; i++)
            {
                changedECItem = variable.SubItem[i];

                if (changedECItem.Format == SECSItemFormat.L && changedECItem.Length == 2 && changedECItem.SubItem[0].Format != SECSItemFormat.L && changedECItem.SubItem[1].Format != SECSItemFormat.L)
                {
                    changedECID = changedECItem.SubItem[0].Value;
                    changedECValue = changedECItem.SubItem[1].Value;

                    updateVariableInfo = this.VariableCollection.ECV[changedECID] as ExpandedVariableInfo;

                    if (updateVariableInfo != null)
                    {
                        updateVariableInfo.Value = changedECValue;
                        UpdateSystemConfig(updateVariableInfo.Name, changedECValue);
                    }
                }
                else if (changedECItem.Format == SECSItemFormat.L && changedECItem.Length > 0 && changedECItem.SubItem[0].Format == SECSItemFormat.L)
                {
                    UpdateReceivedECList(changedECItem);
                }
            }
        }
        #endregion
        #region UpdateSystemConfig
        public void UpdateSystemConfig(string targetVariableName, string newValue)
        {
            int intValue;

            switch (targetVariableName)
            {
                case "T3Timeout":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.HSMSModeConfig.T3 = intValue;
                        this.IsDirty = true;
                    }
                    break;
                case "T5Timeout":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.HSMSModeConfig.T5 = intValue;
                        this.IsDirty = true;
                    }
                    break;
                case "T6Timeout":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.HSMSModeConfig.T6 = intValue;
                        this.IsDirty = true;
                    }
                    break;
                case "T7Timeout":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.HSMSModeConfig.T7 = intValue;
                        this.IsDirty = true;
                    }
                    break;
                case "T8Timeout":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.HSMSModeConfig.T8 = intValue;
                        this.IsDirty = true;
                    }
                    break;
                case "DeviceID":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.DeviceID = intValue;
                        this.IsDirty = true;
                    }
                    break;
                case "IPAddress":
                    this.Configuration.HSMSModeConfig.LocalIPAddress = newValue;
                    this.Configuration.HSMSModeConfig.RemoteIPAddress = newValue;
                    this.IsDirty = true;
                    break;
                case "PortNumber":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.HSMSModeConfig.LocalPortNo = intValue;
                        this.Configuration.HSMSModeConfig.RemotePortNo = intValue;
                        this.IsDirty = true;
                    }
                    break;
                case "ActiveMode":
                    if (bool.TryParse(newValue, out bool boolValue) == true)
                    {
                        if (boolValue == true)
                        {
                            this.Configuration.HSMSModeConfig.HSMSMode = HSMSMode.Passive;
                        }
                        else
                        {
                            this.Configuration.HSMSModeConfig.HSMSMode = HSMSMode.Active;
                        }
                        this.IsDirty = true;
                    }
                    break;
                case "LinkTestInterval":
                    if (int.TryParse(newValue, out intValue) == true)
                    {
                        this.Configuration.HSMSModeConfig.LinkTest = intValue;
                        this.IsDirty = true;
                    }
                    break;
            }
        }
        #endregion
        #region GetVValueByS6F11
        private bool GetVValueByS6F11(SECSMessage secsMessage, CollectionEventInfo collectionEventInfo, string reportId, string variableName, out object value, out string errorText)
        {
            bool result;
            SECSItem reports;
            ReportInfo reportInfo;

            result = false;
            value = null;
            errorText = string.Empty;

            try
            {
                reports = secsMessage.Body.Item[0].SubItem[2];

                if (reports != null && reports.Format == SECSItemFormat.L && reports.Length > 0)
                {
                    if (collectionEventInfo.Reports.Items.Count == reports.Length)
                    {
                        foreach (SECSItem tempSECSItem in reports.SubItem.Items)
                        {
                            reportInfo = collectionEventInfo.Reports[tempSECSItem.SubItem[0].Value];

                            if (reportInfo != null)
                            {
                                if (string.IsNullOrEmpty(reportId) == false)
                                {
                                    if (reportInfo.ReportID == reportId)
                                    {
                                        result = GetVValueByReportInfo(tempSECSItem.SubItem[1], reportInfo, variableName, out value, out errorText);
                                    }
                                }
                                else
                                {
                                    result = GetVValueByReportInfo(tempSECSItem.SubItem[1], reportInfo, variableName, out value, out errorText);
                                }


                                if (result == true || string.IsNullOrEmpty(errorText) == false)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                result = false;
                                errorText = string.Format("Report is not defined:RPTID={0}", tempSECSItem.SubItem[0].Value);
                                break;
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        errorText = string.Format("Report count mismatch:Define={0}, Message={1}", collectionEventInfo.Reports.Items.Count, reports.Length);
                    }
                }
                else
                {
                    result = false;
                    errorText = "Report is empty";
                }
            }
            catch (Exception ex)
            {
                result = false;
                errorText = ex.Message;
            }

            return result;
        }
        #endregion
        #region GetVValueByReportInfo
        private bool GetVValueByReportInfo(SECSItem report, ReportInfo reportInfo, string variableName, out object value, out string errorText)
        {
            bool result;

            value = null;
            errorText = string.Empty;

            try
            {
                result = false;

                if (report != null && report.Format == SECSItemFormat.L && report.Length > 0)
                {
                    if (report.Length == reportInfo.Variables.Items.Count)
                    {
                        for (int i = 0; i < report.Length; i++)
                        {
                            if (report.SubItem[i].Format == SECSItemFormat.L)
                            {
                                result = GetVValueByVariableInfo(null, reportInfo.Variables.Items[i] as ExpandedVariableInfo, variableName, out value, out errorText);

                                if (result == true || string.IsNullOrEmpty(errorText) == false)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (variableName == reportInfo.Variables.Items[i].Name)
                                {
                                    value = report.SubItem[i].Value.GetValue();

                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        errorText = string.Format("Report count mismatch:RPTID={0}, Define={1}, Message={2}", reportInfo.ReportID, reportInfo.Variables.Items.Count, report.Length);
                    }
                }
                else
                {
                    result = false;
                    errorText = "Report is empty";
                }
            }
            catch (Exception ex)
            {
                result = false;
                errorText = ex.Message;
            }

            return result;
        }
        #endregion
        #region GetVValueByVariableInfo
        private bool GetVValueByVariableInfo(SECSItem variable, ExpandedVariableInfo variableInfo, string variableName, out object value, out string errorText)
        {
            bool result;

            value = null;
            errorText = string.Empty;

            try
            {
                result = true;

                if (variable != null && variable.Format == SECSItemFormat.L && variable.Length > 0)
                {
                    if (variable.Length == variableInfo.ChildVariables.Count)
                    {
                        for (int i = 0; i < variable.Length; i++)
                        {
                            if (variable.SubItem[i].Format == SECSItemFormat.L)
                            {
                                result = GetVValueByVariableInfo(null, variableInfo.ChildVariables[i], variableName, out value, out errorText);

                                if (result == true || string.IsNullOrEmpty(errorText) == false)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (variableName == variableInfo.ChildVariables[i].Name)
                                {
                                    value = variable.SubItem[i].Value.GetValue();

                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        errorText = string.Format("Child V count mismatch:VID={0}, Define={1}, Message={2}", variableInfo.VID, variableInfo.ChildVariables.Count, variable.Length);
                    }
                }
                else
                {
                    result = false;
                    errorText = "Child variables is empty";
                }
            }
            catch (Exception ex)
            {
                result = false;
                errorText = ex.Message;
            }

            return result;
        }
        #endregion

        // Make SECS2Log Methods
        #region MakeSECS2Log
        public string MakeSECS2Log(SECSBody body)
        {
            StringBuilder builder;
            int depth;
            int subCount;
            List<SECSItem> list;
            SECSItem item;

            depth = 0;
            builder = new StringBuilder();

            if (body.AsList.Count > 0)
            {
                list = body.AsList;

                for (int i = 0; i < list.Count; i++)
                {
                    item = list[i];
                    builder.AppendLine();

                    if (item.Format == SECSItemFormat.L)
                    {
                        builder.AppendFormat("<L,{0}", item.Length);

                        if (string.IsNullOrEmpty(item.Name) == false)
                        {
                            builder.AppendFormat(" [{0}]", item.Name);
                        }

                        subCount = MakeSECS2Log(builder, depth + 1, list, i + 1);
                        i += subCount;

                        builder.AppendLine();
                        builder.Append(">");
                    }
                    else
                    {
                        builder.AppendFormat("<{0},{1} ", item.Format, item.Length);

                        if (item.Length == 1)
                        {
                            if (item.Value == null)
                            {
                                builder.AppendFormat("''");
                            }
                            else
                            {
                                builder.AppendFormat("'{0}'", item.Value.ToString());
                            }

                            if (string.IsNullOrEmpty(item.Name) == false)
                            {
                                builder.AppendFormat(" [{0}]", item.Name);
                            }

                            builder.AppendFormat(">");
                        }
                        else
                        {
                            builder.AppendFormat("'");

                            if (item.Value != null)
                            {
                                if (item.Value.GetValue() is List<object> ll)
                                {
                                    foreach (var t in ll)
                                    {
                                        builder.AppendFormat("{0} ", t.ToString());
                                    }
                                    builder.Remove(builder.Length - 1, 1);
                                }
                            }

                            builder.AppendFormat("'");

                            if (string.IsNullOrEmpty(item.Name) == false)
                            {
                                builder.AppendFormat(" [{0}]", item.Name);
                            }

                            builder.AppendFormat(">");
                        }
                    }
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
        #endregion
        #region MakeSECS2Log
        private static int MakeSECS2Log(StringBuilder builder, int depth, List<SECSItem> list, int itemIndex)
        {
            int itemCount;
            int subCount;
            SECSItem item;

            itemCount = 0;

            if (list != null)
            {
                for (int i = itemIndex; i < list.Count; i++)
                {
                    item = list[i];

                    builder.AppendLine();

                    if (item.Format == SECSItemFormat.L)
                    {
                        for (int j = 0; j < depth; j++)
                        {
                            builder.Append("  ");
                        }

                        builder.AppendFormat("<L,{0}", item.Length);

                        if (string.IsNullOrEmpty(item.Name) == false)
                        {
                            builder.AppendFormat(" [{0}]", item.Name);
                        }

                        subCount = MakeSECS2Log(builder, depth + 1, list, i + 1);
                        i += subCount;
                        itemCount += subCount + 1;

                        builder.AppendLine();

                        for (int j = 0; j < depth; j++)
                        {
                            builder.Append("  ");
                        }

                        builder.Append(">");
                    }
                    else
                    {
                        for (int j = 0; j < depth; j++)
                        {
                            builder.Append("  ");
                        }

                        builder.AppendFormat("<{0},{1} ", item.Format, item.Length);

                        if (item.Length == 1)
                        {
                            if (item.Value == null)
                            {
                                builder.AppendFormat("''");
                            }
                            else
                            {
                                builder.AppendFormat("'{0}'", item.Value.ToString());
                            }

                            if (string.IsNullOrEmpty(item.Name) == false)
                            {
                                builder.AppendFormat(" [{0}]", item.Name);
                            }

                            builder.AppendFormat(">");
                        }
                        else
                        {
                            builder.AppendFormat("'");

                            if (item.Value != null && item.Value.GetValue() != null)
                            {
                                foreach (var t in item.Value.GetValue() as List<object>)
                                {
                                    builder.AppendFormat("{0} ", t.ToString());
                                }

                                builder.Remove(builder.Length - 1, 1);
                            }

                            builder.AppendFormat("'");

                            if (string.IsNullOrEmpty(item.Name) == false)
                            {
                                builder.AppendFormat(" [{0}]", item.Name);
                            }

                            builder.AppendFormat(">");
                        }

                        itemCount++;
                    }
                }
            }

            return itemCount;
        }
        #endregion

        // ETC Methods
        #region MakeExpandedRemoteCommandValueSetCollectionToXElement
        private XElement MakeExpandedRemoteCommandValueSetCollectionToXElement(ExpandedRemoteCommandValueSetCollection collection)
        {
            XElement result;
            XElement valueSetInfoElement;
            XElement parametersElement;

            result = new XElement("ValueSetCollection");

            foreach (ExpandedRemoteCommandValueSetInfo info in collection.Items.Values)
            {
                valueSetInfoElement = new XElement("ValueSet", 
                                                new XAttribute("Name", info.Name));
                result.Add(valueSetInfoElement);

                parametersElement = new XElement("Parameters");
                valueSetInfoElement.Add(parametersElement);

                foreach (ExpandedRemoteCommandParameterInfo param in info.ParameterItems)
                {
                    parametersElement.Add(new XElement("Parameter",
                                                 new XAttribute("Name", param.Name),
                                                 new XAttribute("Format", param.Format),
                                                 new XAttribute("Count", param.Count),
                                                 new XAttribute("GenerateRule", param.GenerateRule),
                                                 new XAttribute("Value", param.Value)));
                }
            }
            return result;
        }
        #endregion
        #region MakeExpandedRemoteCommandValueSetCollectionFromXElement
        private ExpandedRemoteCommandValueSetCollection MakeExpandedRemoteCommandValueSetCollectionFromXElement(XElement element)
        {
            ExpandedRemoteCommandValueSetCollection result;
            ExpandedRemoteCommandValueSetInfo info;

            result = new ExpandedRemoteCommandValueSetCollection();

            if (element != null && element.Element("ValueSetCollection") != null)
            {
                foreach (var valueSetElement in element.Element("ValueSetCollection").Elements("ValueSet"))
                {
                    if (valueSetElement.Attribute("Name") != null && string.IsNullOrEmpty(valueSetElement.Attribute("Name").Value) == false)
                    {
                        info = new ExpandedRemoteCommandValueSetInfo()
                        {
                            Name = valueSetElement.Attribute("Name").Value
                        };

                        if (valueSetElement.Element("Parameters") != null)
                        {
                            info.AddParameterItems(MakeExpandedRemoteCommandParameterListFromXElement(valueSetElement));
                        }

                        result.Add(info);
                    }
                }
            }
            return result;
        }
        #endregion
        #region MakeExpandedEnhancedRemoteCommandValueSetCollectionToXElement
        private XElement MakeExpandedEnhancedRemoteCommandValueSetCollectionToXElement(ExpandedEnhancedRemoteCommandValueSetCollection collection)
        {
            XElement result;
            XElement valueSetInfoElement;
            XElement parametersElement;
            XElement parameterElement;

            result = new XElement("ValueSetCollection");

            foreach (ExpandedEnhancedRemoteCommandValueSetInfo info in collection.Items.Values)
            {
                valueSetInfoElement = new XElement("ValueSet",
                                                new XAttribute("Name", info.Name));
                result.Add(valueSetInfoElement);

                parametersElement = new XElement("Parameters");
                valueSetInfoElement.Add(parametersElement);

                foreach (ExpandedEnhancedRemoteCommandParameterInfo param in info.ParameterItems)
                {
                    if (param.Format == SECSItemFormat.L)
                    {
                        parameterElement = new XElement("Parameter",
                                                     new XAttribute("Name", param.Name),
                                                     new XAttribute("Format", param.Format),
                                                     new XAttribute("Count", param.Count),
                                                     new XAttribute("UseChildItem", param.UseChildLength));
                        parameterElement.Add(MakeExpandedEnhancedRemoteCommandParameterInfoToXElement(param));
                    }
                    else
                    {
                        parameterElement = new XElement("Parameter",
                                                     new XAttribute("Name", param.Name),
                                                     new XAttribute("Format", param.Format),
                                                     new XAttribute("Count", param.Count),
                                                     new XAttribute("GenerateRule", param.GenerateRule),
                                                     new XAttribute("Value", param.Value));

                    }

                    parametersElement.Add(parameterElement);
                }
            }
            return result;
        }
        #endregion
        #region MakeExpandedEnhancedRemoteCommandParameterInfoToXElement
        private XElement MakeExpandedEnhancedRemoteCommandParameterInfoToXElement(ExpandedEnhancedRemoteCommandParameterInfo parameterItem)
        {
            XElement result;
            XElement subElement;

            result = null;

            if (parameterItem != null && parameterItem.Format == SECSItemFormat.L)
            {
                    result = new XElement("Values");

                foreach (ExpandedEnhancedRemoteCommandParameterItem tempValueItem in parameterItem.ValueItems)
                {
                    if (tempValueItem.Format == SECSItemFormat.L)
                    {
                        subElement = new XElement("Value",
                                    new XAttribute("Name", tempValueItem.Name),
                                    new XAttribute("Format", tempValueItem.Format),
                                    new XAttribute("Count", tempValueItem.Count),
                                    new XAttribute("UseChildItem", tempValueItem.UseChildLength));

                        subElement.Add(MakeExpandedEnhancedRemoteCommandParameterItemToXElement(tempValueItem));

                        result.Add(subElement);
                    }
                    else
                    {
                        result.Add(new XElement("Value",
                                    new XAttribute("Name", tempValueItem.Name),
                                    new XAttribute("Format", tempValueItem.Format),
                                    new XAttribute("Count", tempValueItem.Count),
                                    new XAttribute("GenerateRule", tempValueItem.GenerateRule),
                                    new XAttribute("Value", tempValueItem.Value)));
                    }
                }
            }

            return result;

        }
        #endregion
        #region MakeExpandedEnhancedRemoteCommandParameterItemToXElement
        private XElement MakeExpandedEnhancedRemoteCommandParameterItemToXElement(ExpandedEnhancedRemoteCommandParameterItem valueItem)
        {
            XElement result;
            XElement subElement;

            result = null;

            if (valueItem != null && valueItem.Format == SECSItemFormat.L)
            {
                    result = new XElement("Values");

                foreach (ExpandedEnhancedRemoteCommandParameterItem tempValueItem in valueItem.ChildParameterItem)
                {
                    if (tempValueItem.Format == SECSItemFormat.L)
                    {
                        subElement = new XElement("Value",
                                    new XAttribute("Name", tempValueItem.Name),
                                    new XAttribute("Count", tempValueItem.Count),
                                    new XAttribute("Format", tempValueItem.Format));

                        subElement.Add(MakeExpandedEnhancedRemoteCommandParameterItemToXElement(tempValueItem));

                        result.Add(subElement);
                    }
                    else
                    {
                        result.Add(new XElement("Value",
                                    new XAttribute("Name", tempValueItem.Name),
                                    new XAttribute("Format", tempValueItem.Format),
                                    new XAttribute("Count", tempValueItem.Count),
                                    new XAttribute("GenerateRule", tempValueItem.GenerateRule),
                                    new XAttribute("Value", tempValueItem.Value)));
                    }
                }
                
            }

            return result;

        }
        #endregion
        #region MakeExpandedRemoteCommandParameterListFromXElement
        private List<ExpandedRemoteCommandParameterInfo> MakeExpandedRemoteCommandParameterListFromXElement(XElement element)
        {
            List<ExpandedRemoteCommandParameterInfo> result;
            ExpandedRemoteCommandParameterInfo info;

            result = new List<ExpandedRemoteCommandParameterInfo>();

            if (element.Element("Parameters") != null)
            {
                foreach(XElement parameterElement in element.Element("Parameters").Elements("Parameter"))
                {
                    if (parameterElement.Attribute("Name") != null && parameterElement.Attribute("Format") != null 
                        && Enum.TryParse(parameterElement.Attribute("Format").Value, out SECSItemFormat format) == true)
                    {
                        info = new ExpandedRemoteCommandParameterInfo
                        {
                            Name = parameterElement.Attribute("Name").Value,
                            Format = format,
                            Count = 1
                        };

                        if (parameterElement.Attribute("Count") != null && int.TryParse(parameterElement.Attribute("Count").Value, out int count) == true)
                        {
                            info.Count = count;
                        }

                        result.Add(info);

                        if (parameterElement.Attribute("GenerateRule") != null)
                        {
                            if (IsValidGenerateRule(format, parameterElement.Attribute("GenerateRule").Value) == true)
                            {
                                info.GenerateRule = parameterElement.Attribute("GenerateRule").Value;
                            }
                        }

                        if (parameterElement.Attribute("Value") != null)
                        {
                            info.Value = parameterElement.Attribute("Value").Value;
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region MakeExpandedEnhancedRemoteCommandValueSetCollectionFromXElement
        private ExpandedEnhancedRemoteCommandValueSetCollection MakeExpandedEnhancedRemoteCommandValueSetCollectionFromXElement(XElement element)
        {
            ExpandedEnhancedRemoteCommandValueSetCollection result;
            ExpandedEnhancedRemoteCommandValueSetInfo info;

            result = new ExpandedEnhancedRemoteCommandValueSetCollection();

            if (element != null && element.Element("ValueSetCollection") != null)
            {
                foreach (var valueSetElement in element.Element("ValueSetCollection").Elements("ValueSet"))
                {
                    if (valueSetElement.Attribute("Name") != null && string.IsNullOrEmpty(valueSetElement.Attribute("Name").Value) == false)
                    {
                        info = new ExpandedEnhancedRemoteCommandValueSetInfo()
                        {
                            Name = valueSetElement.Attribute("Name").Value
                        };

                        if (valueSetElement.Element("Parameters") != null)
                        {
                            info.AddParameterItems(MakeExpandedEnhancedRemoteCommandParameterListFromXElement(valueSetElement));
                        }

                        result.Add(info);
                    }
                }
            }
            return result;
        }
        #endregion
        #region MakeExpandedEnhancedRemoteCommandParameterListFromXElement
        private List<ExpandedEnhancedRemoteCommandParameterInfo> MakeExpandedEnhancedRemoteCommandParameterListFromXElement(XElement element)
        {
            List<ExpandedEnhancedRemoteCommandParameterInfo> result;
            ExpandedEnhancedRemoteCommandParameterInfo info;

            result = new List<ExpandedEnhancedRemoteCommandParameterInfo>();

            if (element.Element("Parameters") != null)
            {
                foreach (XElement parameterElement in element.Element("Parameters").Elements("Parameter"))
                {
                    if (parameterElement.Attribute("Name") != null && parameterElement.Attribute("Format") != null
                        && Enum.TryParse(parameterElement.Attribute("Format").Value, out SECSItemFormat format) == true)
                    {
                        info = new ExpandedEnhancedRemoteCommandParameterInfo
                        {
                            Name = parameterElement.Attribute("Name").Value,
                            Format = format,
                            Count = 1
                        };

                        if (parameterElement.Attribute("Count") != null && int.TryParse(parameterElement.Attribute("Count").Value, out int count) == true)
                        {
                            info.Count = count;
                        }

                        if (parameterElement.Attribute("UseChildItem") != null && bool.TryParse(parameterElement.Attribute("UseChildItem").Value, out bool boolValue) == true)
                        {
                            info.UseChildLength = boolValue;
                        }

                        result.Add(info);

                        if (format == SECSItemFormat.L)
                        {
                            info.ValueItems.AddRange(MakeExpandedRemoteCommandValueListFromXElement(parameterElement));
                        }
                        else
                        {
                            if (parameterElement.Attribute("GenerateRule") != null)
                            {
                                if (IsValidGenerateRule(format, parameterElement.Attribute("GenerateRule").Value) == true)
                                {
                                    info.GenerateRule = parameterElement.Attribute("GenerateRule").Value;
                                }
                            }

                            if (parameterElement.Attribute("Value") != null)
                            {
                                info.Value = parameterElement.Attribute("Value").Value;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region MakeExpandedRemoteCommandValueListFromXElement
        private List<ExpandedEnhancedRemoteCommandParameterItem> MakeExpandedRemoteCommandValueListFromXElement(XElement element)
        {
            List<ExpandedEnhancedRemoteCommandParameterItem> result;
            ExpandedEnhancedRemoteCommandParameterItem item;
            bool boolValue;
            XElement valueElement;

            result = new List<ExpandedEnhancedRemoteCommandParameterItem>();

            if (element.Element("Value") != null && element.Element("Value").Attribute("Format") != null 
                && Enum.TryParse(element.Element("Value").Attribute("Format").Value, out SECSItemFormat format) == true)
            {
                #region ListType A
                {
                    valueElement = element.Element("Value");
                    item = new ExpandedEnhancedRemoteCommandParameterItem
                    {
                        Name = string.Empty,
                        Format = format,
                        Count = 1
                    };

                    if (valueElement.Attribute("Count") != null && int.TryParse(valueElement.Attribute("Count").Value, out int count) == true)
                    {
                        item.Count = count;
                    }

                    if (valueElement.Attribute("UseChildItem") != null && bool.TryParse(valueElement.Attribute("UseChildItem").Value, out boolValue) == true)
                    {
                        item.UseChildLength = boolValue;
                    }

                    if (valueElement.Attribute("GenerateRule") != null && IsValidGenerateRule(format, valueElement.Attribute("GenerateRule").Value) == true)
                    {
                        item.GenerateRule = valueElement.Attribute("GenerateRule").Value;
                    }

                    if (valueElement.Attribute("Value") != null)
                    {
                        item.Value = valueElement.Attribute("Value").Value;
                    }

                    result.Add(item);
                }
                #endregion
            }
            else if (element.Element("Values") != null)
            {
                #region ListType B
                foreach (XElement tempValueElement in element.Element("Values").Elements("Value"))
                {
                    if (tempValueElement.Attribute("Name") != null && tempValueElement.Attribute("Format") != null
                        && Enum.TryParse(tempValueElement.Attribute("Format").Value, out format) == true)
                    {
                        item = new ExpandedEnhancedRemoteCommandParameterItem
                        {
                            Name = tempValueElement.Attribute("Name").Value,
                            Format = format,
                            Count = 1
                        };

                        if (tempValueElement.Attribute("Count") != null && int.TryParse(tempValueElement.Attribute("Count").Value, out int count) == true)
                        {
                            item.Count = count;
                        }

                        if (tempValueElement.Attribute("UseChildItem") != null && bool.TryParse(tempValueElement.Attribute("UseChildItem").Value, out boolValue) == true)
                        {
                            item.UseChildLength = boolValue;
                        }

                        if (format == SECSItemFormat.L)
                        {
                            item.ChildParameterItem.AddRange(MakeExpandedRemoteCommandValueListFromXElement(tempValueElement));
                        }

                        if (tempValueElement.Attribute("GenerateRule") != null && IsValidGenerateRule(format, tempValueElement.Attribute("GenerateRule").Value) == true)
                        {
                            item.GenerateRule = tempValueElement.Attribute("GenerateRule").Value;
                        }

                        if (tempValueElement.Attribute("Value") != null)
                        {
                            item.Value = tempValueElement.Attribute("Value").Value;
                        }

                        result.Add(item);
                    }
                }
                #endregion
            }

            return result;
        }
        #endregion
        #region MakeTriggerCollectionFromXElement
        private AutoSendTriggerCollection MakeTriggerCollectionFromXElement(string elementName, XElement element)
        {
            AutoSendTriggerCollection result;
            AutoSendTrigger trigger;
            ExpandedCollectionEventInfo ceInfo;
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;
            string stringValue;
            string[] split;

            result = new AutoSendTriggerCollection();

            if (element != null && element.Element(elementName) != null)
            {
                foreach (var triggerElement in element.Element(elementName).Elements("TriggerInfo"))
                {
                    if (triggerElement.Attribute("TriggerID") != null && int.TryParse(triggerElement.Attribute("TriggerID").Value, out int intValue) == true)
                    {
                        trigger = new AutoSendTrigger()
                        {
                            TriggerID = intValue
                        };

                        if (triggerElement.Attribute("TriggerMode") != null && Enum.TryParse(triggerElement.Attribute("TriggerMode").Value, out TriggerMode triggerMode) == true)
                        {
                            trigger.TriggerMode = triggerMode;
                        }
                        else
                        {
                            trigger.TriggerMode = TriggerMode.NotUse;
                        }

                        if (triggerElement.Attribute("CollectionEventID") != null)
                        {
                            ceInfo = this.CollectionEventCollection[triggerElement.Attribute("CollectionEventID").Value] as ExpandedCollectionEventInfo;

                            if (ceInfo != null)
                            {
                                trigger.CollectionEventInfo = ceInfo;
                            }
                        }

                        if (triggerElement.Attribute("ReportID") != null)
                        {
                            reportInfo = this.ReportCollection[triggerElement.Attribute("ReportID").Value] as ExpandedReportInfo;

                            if (reportInfo != null)
                            {
                                trigger.ReportInfo = reportInfo;
                            }
                        }

                        if (triggerElement.Attribute("VariableID") != null)
                        {
                            variableInfo = this.VariableCollection[triggerElement.Attribute("VariableID").Value] as ExpandedVariableInfo;

                            if (variableInfo != null && variableInfo.VID != variableInfo.Name)
                            {
                                trigger.VariableStack.Add(variableInfo);
                            }
                        }
                        else if (triggerElement.Attribute("VariableStack") != null)
                        {
                            stringValue = triggerElement.Attribute("VariableStack").Value;
                            split = stringValue.Split(',');
                            foreach (string s in split)
                            {
                                variableInfo = this.VariableCollection[s] as ExpandedVariableInfo;
                                if (variableInfo != null)
                                {
                                    if (variableInfo.VID != variableInfo.Name)
                                    {
                                        trigger.VariableStack.Add(variableInfo);
                                    }

                                    if (variableInfo.Format != SECSItemFormat.L)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (triggerElement.Attribute("VariableValue") != null)
                        {
                            trigger.VariableValue = triggerElement.Attribute("VariableValue").Value;
                        }

                        result.Add(trigger);
                    }
                }
            }
            return result;
        }
        #endregion
        #region MakeTriggerCollectionToXElement
        private XElement MakeTriggerCollectionToXElement(string elementName, AutoSendTriggerCollection triggerCollection)
        {
            XElement result;
            string writeAttribute;

            result = null;

            if (elementName != null)
            {
                result = new XElement(elementName);

                if (triggerCollection != null)
                {
                    foreach (var trigger in triggerCollection.Items)
                    {
                        writeAttribute = string.Empty;

                        foreach (var t in trigger.VariableStack)
                        {
                            writeAttribute += string.Format(",{0}", t.VID);
                        }

                        if (writeAttribute.Length > 0)
                        {
                            writeAttribute = writeAttribute.Substring(1);
                        }

                        result.Add(new XElement("TriggerInfo",
                                    new XAttribute("TriggerID", trigger.TriggerID),
                                    new XAttribute("TriggerMode", trigger.TriggerMode),
                                    new XAttribute("CollectionEventID", trigger.CollectionEventInfo == null ? string.Empty : trigger.CollectionEventInfo.CEID),
                                    new XAttribute("ReportID", trigger.ReportInfo == null ? string.Empty : trigger.ReportInfo.ReportID),
                                    new XAttribute("VariableStack", writeAttribute),
                                    new XAttribute("VariableValue", trigger.VariableValue)
                            ));
                    }
                }
            }

            return result;
        }
        #endregion
        #region MakeExpandedCommandParameterItemToXElement
        private XElement MakeExpandedEnhancedCommandParameterItemToXElement(ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem)
        {
            XElement result;
            XElement subElement;
            string value;

            if (expandedEnhancedCommandParameterItem != null)
            {
                result = new XElement("Values");

                foreach (ExpandedEnhancedRemoteCommandParameterItem tempEnhancedCommandParameterItem in expandedEnhancedCommandParameterItem.ChildParameterItem)
                {
                    subElement = null;

                    if (tempEnhancedCommandParameterItem.Format == SECSItemFormat.L)
                    {
                        subElement = new XElement("Value",
                                                        new XAttribute("Name", tempEnhancedCommandParameterItem.Name),
                                                        new XAttribute("Format", tempEnhancedCommandParameterItem.Format));
                        subElement.Add(MakeExpandedEnhancedCommandParameterItemToXElement(tempEnhancedCommandParameterItem));
                    }
                    else
                    {
                        if (tempEnhancedCommandParameterItem.Value != null)
                        {
                            value = tempEnhancedCommandParameterItem.Value;
                        }
                        else
                        {
                            value = string.Empty;
                        }

                        subElement = new XElement("Value",
                                                        new XAttribute("Name", tempEnhancedCommandParameterItem.Name),
                                                        new XAttribute("Format", tempEnhancedCommandParameterItem.Format),
                                                        new XAttribute("GenerateRule", tempEnhancedCommandParameterItem.GenerateRule),
                                                        new XAttribute("Value", value));
                    }
                    result.Add(subElement);
                }
            }
            else
            {

                var tempEnhancedCommandParameterItem = expandedEnhancedCommandParameterItem.ChildParameterItem[0];

                if (tempEnhancedCommandParameterItem.Value != null)
                {
                    result = new XElement("Value",
                                                    new XAttribute("Name", tempEnhancedCommandParameterItem.Name),
                                                    new XAttribute("Format", tempEnhancedCommandParameterItem.Format),
                                                    new XAttribute("GenerateRule", tempEnhancedCommandParameterItem.GenerateRule),
                                                    new XAttribute("Value", tempEnhancedCommandParameterItem.Value));
                }
                else
                {
                    result = new XElement("Value",
                                                    new XAttribute("Name", tempEnhancedCommandParameterItem.Name),
                                                    new XAttribute("Format", tempEnhancedCommandParameterItem.Format),
                                                    new XAttribute("GenerateRule", tempEnhancedCommandParameterItem.GenerateRule),
                                                    new XAttribute("Value", string.Empty));
                }
            }

            return result;
        }
        #endregion
        #region MakeExpandedRemoteCommandParameterItemFromXElement
        private ExpandedEnhancedRemoteCommandParameterItem MakeExpandedRemoteCommandParameterItemFromXElement(XElement element)
        {
            ExpandedEnhancedRemoteCommandParameterItem result;
            ExpandedEnhancedRemoteCommandParameterItem child;
            XElement enhancedParameterListTypeElement;
            string rule;

            if (element.Attribute("Format") == null)
            {
                result = null;
            }
            else
            {
                result = new ExpandedEnhancedRemoteCommandParameterItem()
                {
                    Name = element.Attribute("Name") == null ? string.Empty : element.Attribute("Name").Value,
                    Format = element.Attribute("Format") != null ? (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), element.Attribute("Format").Value) : SECSItemFormat.A,
                };

                if (result.Format != SECSItemFormat.L)
                {
                    if (element.Attribute("Value") != null)
                    {
                        result.Value = element.Attribute("Value").Value;
                    }

                    if (element.Attribute("GenerateRule") != null)
                    {
                        rule = element.Attribute("GenerateRule").Value;

                        if (IsValidGenerateRule(result.Format, rule) == true)
                        {
                            result.GenerateRule = rule;
                        }
                    }
                }
                else
                {
                    if (element.Element("Value") != null)
                    {
                        enhancedParameterListTypeElement = element.Element("Value");

                        child = MakeExpandedRemoteCommandParameterItemFromXElement(enhancedParameterListTypeElement);
                        result.ChildParameterItem.Add(child);
                    }
                    else if (element.Element("Values") != null)
                    {
                        foreach (XElement tempValue in element.Element("Values").Elements())
                        {
                            if (tempValue.Attribute("Name") != null)
                            {
                                child = MakeExpandedRemoteCommandParameterItemFromXElement(tempValue);
                                result.ChildParameterItem.Add(child);
                            }
                        }
                    }

                }
            }
            return result;
        }
        #endregion
        #region MakeGEMObjectAttributesFromXElement
        private GEMObjectAttributeCollection MakeGEMObjectAttributesFromXElement(XElement element)
        {
            GEMObjectAttributeCollection result;
            GEMObjectAttribute attr;
            string tempValue;

            result = new GEMObjectAttributeCollection();

            if (element.Element("Childs") != null && element.Element("Childs").Element("Child") != null)
            {
                foreach (XElement childElement in element.Element("Childs").Elements("Child"))
                {
                    if (childElement.Attribute("Name") != null && childElement.Attribute("Format") != null)
                    {
                        tempValue = string.Empty;

                        if (Enum.TryParse(childElement.Attribute("Format").Value, out SECSItemFormat tempFormat) == true)
                        {
                            attr = new GEMObjectAttribute()
                            {
                                ATTRID = childElement.Attribute("Name").Value,
                                Format = tempFormat,
                            };

                            if (tempFormat == SECSItemFormat.L)
                            {
                                attr.ChildAttributes = MakeGEMObjectAttributesFromXElement(childElement);
                            }
                            else
                            {
                                if (childElement.Attribute("ATTRDATA") != null)
                                {
                                    attr.ATTRDATA = childElement.Attribute("ATTRDATA").Value;
                                }
                            }

                            result.Add(attr);
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region MakeGEMObjectAttributesToXElement
        private XElement MakeGEMObjectAttributesToXElement(GEMObjectAttributeCollection attrCollection)
        {
            XElement result;
            XElement child;

            result = new XElement("Childs");

            foreach (GEMObjectAttribute attr in attrCollection.Items)
            {
                child = new XElement("Child", new XAttribute("Name", attr.ATTRID),
                                              new XAttribute("Format", attr.Format)
                                     );
                if (attr.Format == SECSItemFormat.L)
                {
                    child.Add(MakeGEMObjectAttributesToXElement(attr.ChildAttributes));
                }
                else
                {
                    child.Add(new XAttribute("Value", attr.ATTRDATA));
                }

                result.Add(child);
            }

            return result;
        }
        #endregion
        #region MakeList
        private List<T> MakeList<T>(SECSItemFormat format, int count, bool isFixed, string data)
        {
            List<T> list;
            string[] splitData;
            dynamic converted;
            T value;

            list = null;

            splitData = data.Split(' ');

            if (count == 0 || (isFixed == true && count == splitData.Length) || (isFixed == false && count <= splitData.Length))
            {
                list = new List<T>();

                foreach (string temp in splitData)
                {
                    converted = ConvertValue(format, temp);

                    if (converted == null)
                    {
                        list = null;
                        break;
                    }
                    else
                    {
                        value = converted;
                        list.Add(value);
                    }
                }
            }

            return list;
        }
        #endregion
        #region AddListToMessage
        public bool AddListToMessage(SECSMessage message, SECSItemFormat format, string name, int count, bool isFixed, string dataString, out string errorText)
        {
            bool result;
            List<sbyte> sbyteList;
            List<byte> byteList;
            List<short> shortList;
            List<ushort> ushortList;
            List<int> intList;
            List<uint> uintList;
            List<long> longList;
            List<ulong> ulongList;
            List<float> floatList;
            List<double> doubleList;
            List<bool> boolList;

            errorText = string.Empty;
            result = true;

            switch (format)
            {
                case SECSItemFormat.Boolean:
                    boolList = MakeList<bool>(format, count, isFixed, dataString);

                    if (boolList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, boolList.Count, boolList.ToArray());
                    }

                    break;
                case SECSItemFormat.B:
                case SECSItemFormat.U1:
                    byteList = MakeList<byte>(format, count, isFixed, dataString);

                    if (byteList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, byteList.Count, byteList.ToArray());
                    }

                    break;
                case SECSItemFormat.U2:
                    ushortList = MakeList<ushort>(format, count, isFixed, dataString);

                    if (ushortList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, ushortList.Count, ushortList.ToArray());
                    }

                    break;
                case SECSItemFormat.U4:
                    uintList = MakeList<uint>(format, count, isFixed, dataString);

                    if (uintList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, uintList.Count, uintList.ToArray());
                    }

                    break;
                case SECSItemFormat.U8:
                    ulongList = MakeList<ulong>(format, count, isFixed, dataString);

                    if (ulongList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, ulongList.Count, ulongList.ToArray());
                    }

                    break;
                case SECSItemFormat.I1:
                    sbyteList = MakeList<sbyte>(format, count, isFixed, dataString);

                    if (sbyteList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, sbyteList.Count, sbyteList.ToArray());
                    }

                    break;
                case SECSItemFormat.I2:
                    shortList = MakeList<short>(format, count, isFixed, dataString);

                    if (shortList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, shortList.Count, shortList.ToArray());
                    }

                    break;
                case SECSItemFormat.I4:
                    intList = MakeList<int>(format, count, isFixed, dataString);

                    if (intList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, intList.Count, intList.ToArray());
                    }

                    break;
                case SECSItemFormat.I8:
                    longList = MakeList<long>(format, count, isFixed, dataString);

                    if (longList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, longList.Count, longList.ToArray());
                    }

                    break;
                case SECSItemFormat.F4:
                    floatList = MakeList<float>(format, count, isFixed, dataString);

                    if (floatList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, floatList.Count, floatList.ToArray());
                    }

                    break;
                case SECSItemFormat.F8:
                    doubleList = MakeList<double>(format, count, isFixed, dataString);

                    if (doubleList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, doubleList.Count, doubleList.ToArray());
                    }

                    break;
                default:
                    result = false;
                    break;
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                result = false;
            }

            return result;
        }
        #endregion
        #region ConvertAndAddToMessage
        private void ConvertAndAddToMessageForLength1Item(SECSMessage message, string name, SECSItemFormat format, string data, out string errorText)
        {
            string compact;
            dynamic converted;

            errorText = string.Empty;

            if (string.IsNullOrEmpty(data) == true)
            {
                message.Body.Add(name, format, 0, string.Empty);
            }
            else
            {
                compact = data.Trim();
                converted = ConvertValue(format, compact);

                if (converted == null)
                {
                    message.Body.Add(name, format, 0, string.Empty);
                }
                else
                {
                    message.Body.Add(name, format, 1, converted);
                }
            }
        }
        #endregion

        #region ConvertValue
        public dynamic ConvertValue(SECSItemFormat format, string value)
        {
            dynamic result;

            if (value == null)
            {
                result = string.Empty;
            }
            else
            {
                switch (format)
                {
                    case SECSItemFormat.L:
                        result = null;
                        break;
                    case SECSItemFormat.Boolean:
                        if (bool.TryParse(value, out bool boolValue) == true)
                        {
                            result = boolValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.B:
                    case SECSItemFormat.U1:
                        if (byte.TryParse(value, out byte byteValue) == true)
                        {
                            result = byteValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.U2:
                        if (ushort.TryParse(value, out ushort ushortValue) == true)
                        {
                            result = ushortValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.U4:
                        if (uint.TryParse(value, out uint uintValue) == true)
                        {
                            result = uintValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.U8:
                        if (ulong.TryParse(value, out ulong ulongValue) == true)
                        {
                            result = ulongValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I1:
                        if (sbyte.TryParse(value, out sbyte sbyteValue) == true)
                        {
                            result = sbyteValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I2:
                        if (short.TryParse(value, out short shortValue) == true)
                        {
                            result = shortValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I4:
                        if (int.TryParse(value, out int intValue) == true)
                        {
                            result = intValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I8:
                        if (long.TryParse(value, out long longValue) == true)
                        {
                            result = longValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.F4:
                        if (float.TryParse(value, out float floatValue) == true)
                        {
                            result = floatValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.F8:
                        if (double.TryParse(value, out double doubleValue) == true)
                        {
                            result = doubleValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.A:
                    case SECSItemFormat.J:
                    default:
                        result = value;
                        break;
                }
            }

            return result;
        }
        public dynamic ConvertValue(SECSItemFormat format, int count, string value)
        {
            dynamic result;
            string[] splitted;

            result = null;

            if (string.IsNullOrEmpty(value) == true)
            {
                result = null;
            }
            else if (format == SECSItemFormat.A || format == SECSItemFormat.J)
            {
                result = value;
            }
            else
            {
                switch (format)
                {
                    case SECSItemFormat.Boolean:
                        if (count == 1)
                        {
                            if (bool.TryParse(value, out bool converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<bool>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (bool.TryParse(s, out bool converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add(false);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.B:
                    case SECSItemFormat.U1:
                        if (count == 1)
                        {
                            if (byte.TryParse(value, out byte converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<byte>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (byte.TryParse(s, out byte converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((byte)0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }

                        }

                        break;

                    case SECSItemFormat.U2:
                        if (count == 1)
                        {
                            if (ushort.TryParse(value, out ushort converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<ushort>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (ushort.TryParse(s, out ushort converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((ushort)0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.U4:
                        if (count == 1)
                        {
                            if (uint.TryParse(value, out uint converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<uint>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (uint.TryParse(s, out uint converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((uint)0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.U8:
                        if (count == 1)
                        {
                            if (ulong.TryParse(value, out ulong converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<ulong>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (ulong.TryParse(s, out ulong converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((ulong)0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.I1:
                        if (count == 1)
                        {
                            if (sbyte.TryParse(value, out sbyte converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<sbyte>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (sbyte.TryParse(s, out sbyte converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((sbyte)0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.I2:
                        if (count == 1)
                        {
                            if (short.TryParse(value, out short converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<short>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (short.TryParse(s, out short converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((short)0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.I4:
                        if (count == 1)
                        {
                            if (int.TryParse(value, out int converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<int>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (int.TryParse(s, out int converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add(0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.I8:
                        if (count == 1)
                        {
                            if (long.TryParse(value, out long converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<long>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (long.TryParse(s, out long converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((long)0);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.F4:
                        if (count == 1)
                        {
                            if (float.TryParse(value, out float converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<float>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (float.TryParse(s, out float converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add(0.0f);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    case SECSItemFormat.F8:
                        if (count == 1)
                        {
                            if (double.TryParse(value, out double converted) == true)
                            {
                                result = converted;
                            }
                        }
                        else if (count > 1)
                        {
                            result = new List<double>();

                            splitted = value.Split(' ');

                            foreach (var s in splitted)
                            {
                                if (double.TryParse(s, out double converted) == true)
                                {
                                    result.Add(converted);
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                while (result.Count < count)
                                {
                                    result.Add((double)0.0f);
                                }

                                while (result.Count > count)
                                {
                                    result.RemoveAt(result.Count - 1);
                                }
                            }
                        }

                        break;

                    default:
                        result = null;
                        break;
                }
            }

            return result;
        }
        #endregion

        // RaiseEvent
        #region RaiseEvent
        private void RaiseSchedulerActivated()
        {
            this.OnSchedulerActivated?.BeginInvoke(null, null);
        }
        private void RaiseSchedulerDeactivated()
        {
            this.OnSchedulerDeactivated?.BeginInvoke(null, null);
        }
        private void RaiseDriverConnected(object sender, string ipAddress, int portNo)
        {
            this.OnDriverConnected?.BeginInvoke(sender, ipAddress, portNo, null, null);
        }
        private void RaiseDriverDisconnected(object sender, string ipAddress, int portNo)
        {
            this.OnDriverDisconnected?.BeginInvoke(sender, ipAddress, portNo, null, null);
        }
        private void RaiseDriverSelected(object sender, string ipAddress, int portNo)
        {
            this.OnDriverSelected?.BeginInvoke(sender, ipAddress, portNo, null, null);
        }
        private void RaiseDriverDeselected(object sender, string ipAddress, int portNo)
        {
            this.OnDriverDeselected?.BeginInvoke(sender, ipAddress, portNo, null, null);
        }
        private void RaiseDriverLogAdded1(object sender, DriverLogType logType, string logText)
        {
            this.OnDriverLogAdded1?.BeginInvoke(sender, logType, logText, null, null);
        }
        private void RaiseDriverLogAdded2(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            this.OnDriverLogAdded2?.BeginInvoke(sender, logLevel, logText, null, null);
        }
        private void RaiseSECS2LogAdded(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            this.OnSECS2LogAdded?.BeginInvoke(sender, logLevel, logText, null, null);
        }
        private void RaiseCommunicationStateChanged(CommunicationState state)
        {
            this.OnCommunicationStateChanged?.Invoke(state);
        }
        private void RaiseControlStateChanged(ControlState state)
        {
            this.OnControlStateChanged?.Invoke(state);
        }
        #endregion
        #region AddEquipmentConstantsChild
        private void AddEquipmentConstantsChild(SECSMessage message, ExpandedVariableInfo variableInfo)
        {
            DataDictionaryInfo dataDictionaryInfo;
            SECSItemFormat ecidFormat;
            dynamic converted;
            ExpandedVariableInfo childVariable;

            dataDictionaryInfo = this.DataDictionaryCollection[DataDictinaryList.ECID.ToString()];

            if (dataDictionaryInfo != null)
            {
                ecidFormat = dataDictionaryInfo.Format.First();
            }
            else
            {
                ecidFormat = SECSItemFormat.U2;
            }

            message.Body.Add(SECSItemFormat.L, 2, null);

            if (ecidFormat == SECSItemFormat.J || ecidFormat == SECSItemFormat.A)
            {
                message.Body.Add(variableInfo.Name, ecidFormat, Encoding.Default.GetByteCount(variableInfo.VID), variableInfo.VID);
            }
            else
            {
                converted = ConvertValue(ecidFormat, variableInfo.VID);

                if (converted != null)
                {
                    message.Body.Add(variableInfo.Name, ecidFormat, 1, converted);
                }
                else
                {
                    message.Body.Add(variableInfo.Name, ecidFormat, 0, string.Empty);
                }
            }

            if (variableInfo.Format == SECSItemFormat.L)
            {
                message.Body.Add(variableInfo.Format, variableInfo.Length, null);

                foreach (VariableInfo child in variableInfo.ChildVariables)
                {
                    childVariable = child as ExpandedVariableInfo;
                    AddEquipmentConstantsChild(message, childVariable);
                }
            }
            else if (variableInfo.Format == SECSItemFormat.A || variableInfo.Format == SECSItemFormat.J)
            {
                message.Body.Add(variableInfo.Format, Encoding.Default.GetByteCount(variableInfo.Value), variableInfo.Value);
            }
            else
            {
                if (variableInfo.Length < 1)
                {
                    if (variableInfo.Value != null)
                    {
                        string[] token = variableInfo.Value.Split(' ');

                        converted = ConvertValue(variableInfo.Format, token.Length, variableInfo.Value);

                        if (converted != null)
                        {
                            if (token.Length == 1)
                            {
                                message.Body.Add(variableInfo.Format, token.Length, converted);
                            }
                            else
                            {
                                message.Body.Add(variableInfo.Format, token.Length, converted.ToArray());
                            }
                        }
                        else
                        {
                            message.Body.Add(variableInfo.Format, 0, string.Empty);
                        }
                    }
                    else
                    {
                        message.Body.Add(variableInfo.Format, 0, string.Empty);
                    }
                }
                else if (variableInfo.Length == 1)
                {
                    converted = ConvertValue(variableInfo.Format, variableInfo.Value);

                    if (converted != null)
                    {
                        message.Body.Add(variableInfo.Format, 1, converted);
                    }
                    else
                    {
                        message.Body.Add(variableInfo.Format, 0, string.Empty);
                    }
                }
                else if (variableInfo.Length > 1)
                {
                    converted = ConvertValue(variableInfo.Format, variableInfo.Length, variableInfo.Value);

                    if (converted != null)
                    {
                        message.Body.Add(variableInfo.Format, variableInfo.Length, converted.ToArray());
                    }
                    else
                    {
                        message.Body.Add(variableInfo.Format, 0, string.Empty);
                    }
                }
            }
        }
        #endregion
        #region MakeS2F49Child
        private bool MakeS2F49Child(SECSMessage message, ExpandedEnhancedRemoteCommandParameterItem item)
        {
            SECSItemFormat cpNameFormat;
            dynamic converted;
            bool result;

            bool useNameValuePair;
            

            cpNameFormat = GetSECSFormat(DataDictinaryList.CPNAME, SECSItemFormat.A);

            result = true;

            if (item.Format != SECSItemFormat.L)
            {
                if (string.IsNullOrEmpty(item.Value) == true && string.IsNullOrEmpty(item.GenerateRule) == false)
                {
                    if (item.Format == SECSItemFormat.A || item.Format == SECSItemFormat.J)
                    {
                        converted = GenerateValue(item.Format, item.GenerateRule, 0);

                        if (converted != null)
                        {
                            message.Body.Add("CEPVAL", item.Format, Encoding.Default.GetByteCount(converted.ToString()), converted.ToString());
                        }
                        else
                        {
                            message.Body.Add("CEPVAL", item.Format, Encoding.Default.GetByteCount(string.Empty), string.Empty);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.Value) == false)
                        {
                            message.Body.Add("CEPVAL", item.Format, 0, string.Empty);
                        }
                        else if (item.Count == 1)
                        {
                            converted = GenerateValue(item.Format, item.GenerateRule, 0);

                            if (converted != null)
                            {
                                message.Body.Add("CEPVAL", item.Format, 1, converted);
                            }
                            else
                            {
                                message.Body.Add("CEPVAL", item.Format, 0, string.Empty);
                            }
                        }
                        else
                        {
                            converted = GenerateList(item.Format, item.Count, item.GenerateRule);

                            if (converted != null)
                            {
                                message.Body.Add("CEPVAL", item.Format, (int)item.Count, converted.ToArray());
                            }
                            else
                            {
                                message.Body.Add("CEPVAL", item.Format, 0, string.Empty);
                            }
                        }
                    }
                }
                else
                {
                    if (item.Format == SECSItemFormat.A || item.Format == SECSItemFormat.J)
                    {
                        message.Body.Add("CEPVAL", item.Format, Encoding.Default.GetByteCount(item.Value.ToString()), item.Value.ToString());
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.Value) == false)
                        {
                            message.Body.Add("CEPVAL", item.Format, 0, string.Empty);
                        }
                        else if (item.Count == 1)
                        {
                            converted = ConvertValue(item.Format, item.Count, item.Value);

                            if (converted != null)
                            {
                                message.Body.Add("CEPVAL", item.Format, 1, converted);
                            }
                            else
                            {
                                message.Body.Add("CEPVAL", item.Format, 0, string.Empty);
                            }
                        }
                        else
                        {
                            converted = ConvertValue(item.Format, item.Count, item.Value);

                            if (converted != null)
                            {
                                message.Body.Add("CEPVAL", item.Format, converted.Count, converted.ToArray());
                            }
                            else
                            {
                                message.Body.Add("CEPVAL", item.Format, 0, string.Empty);
                            }
                        }
                    }
                }
            }
            else
            {
                if (item.ChildParameterItem.Count == 0 || item.UseChildLength == false)
                {
                    message.Body.Add(SECSItemFormat.L, 0, null);
                }
                else
                {
                    message.Body.Add(SECSItemFormat.L, item.ChildParameterItem.Count, null);

                    foreach (ExpandedEnhancedRemoteCommandParameterItem child in item.ChildParameterItem)
                    {
                        useNameValuePair = true;

                        if (string.IsNullOrEmpty(child.Name) == true && child.Format != SECSItemFormat.L)
                        {
                            useNameValuePair = false;
                        }

                        if (useNameValuePair == true)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            if (cpNameFormat == SECSItemFormat.A || cpNameFormat == SECSItemFormat.J)
                            {
                                message.Body.Add("CPNAME", cpNameFormat, Encoding.Default.GetByteCount(child.Name), child.Name);
                            }
                            else
                            {
                                converted = ConvertValue(cpNameFormat, child.Name);

                                if (converted != null)
                                {
                                    message.Body.Add("CPNAME", cpNameFormat, 1, converted);
                                }
                                else
                                {
                                    if (child.Name == string.Empty)
                                    {
                                        message.Body.Add("CPNAME", cpNameFormat, 0, string.Empty);
                                    }
                                    else
                                    {
                                        result = false;
                                        break;
                                    }
                                }
                            }
                        }

                        result = MakeS2F49Child(message, child);
                    }
                }
            }
            return result;
        }
        #endregion
        #region FillDefault
        private void FillDefault(ExpandedRemoteCommandInfo cmdInfo)
        {
            if (cmdInfo != null)
            {
                foreach (ExpandedRemoteCommandValueSetInfo valueSetInfo in cmdInfo.ValueSetCollection.Items.Values)
                {
                    foreach (ExpandedRemoteCommandParameterInfo parameterInfo in valueSetInfo.ParameterItems)
                    {
                        switch (parameterInfo.Format)
                        {
                            case SECSItemFormat.None:
                            case SECSItemFormat.A:
                            case SECSItemFormat.J:
                            case SECSItemFormat.L:
                            case SECSItemFormat.X:
                                break;
                            case SECSItemFormat.Boolean:
                                if (string.IsNullOrEmpty(parameterInfo.Value) == true)
                                {
                                    parameterInfo.Value = false.ToString();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        #endregion
        #region FillDefault
        private void FillDefault(ExpandedEnhancedRemoteCommandInfo cmdInfo)
        {
            SECSItemFormat dataIDFormat;
            dynamic converted;

            dataIDFormat = GetSECSFormat(DataDictinaryList.DATAID, SECSItemFormat.U4);

            if (cmdInfo != null)
            {
                if (string.IsNullOrEmpty(cmdInfo.DataID) == true)
                {
                    if (dataIDFormat != SECSItemFormat.L && dataIDFormat != SECSItemFormat.A && dataIDFormat != SECSItemFormat.J)
                    {
                        converted = ConvertValue(dataIDFormat, cmdInfo.DataID);

                        if (converted != null)
                        {
                            cmdInfo.DataID = converted.ToString();
                        }
                        else
                        {
                            cmdInfo.DataID = 0.ToString();
                        }
                    }
                }

                foreach (ExpandedEnhancedRemoteCommandValueSetInfo valueSetInfo in cmdInfo.ValueSetCollection.Items.Values)
                {
                    foreach (ExpandedEnhancedRemoteCommandParameterInfo parameterInfo in valueSetInfo.ParameterItems)
                    {
                        switch (parameterInfo.Format)
                        {
                            case SECSItemFormat.None:
                            case SECSItemFormat.A:
                            case SECSItemFormat.J:
                            case SECSItemFormat.X:
                                break;
                            case SECSItemFormat.Boolean:
                                if (string.IsNullOrEmpty(parameterInfo.Value) == true)
                                {
                                    parameterInfo.Value = false.ToString();
                                }
                                break;
                            case SECSItemFormat.L:
                                foreach (ExpandedEnhancedRemoteCommandParameterItem parameterItem in parameterInfo.ValueItems)
                                {
                                    FillDefault(parameterItem);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        #endregion
        #region FillDefault
        private void FillDefault(ExpandedEnhancedRemoteCommandParameterItem item)
        {
            if (item != null)
            {
                switch (item.Format)
                {
                    case SECSItemFormat.None:
                    case SECSItemFormat.A:
                    case SECSItemFormat.J:
                    case SECSItemFormat.X:
                        break;
                    case SECSItemFormat.Boolean:
                        break;
                    case SECSItemFormat.L:
                        foreach (ExpandedEnhancedRemoteCommandParameterItem childItem in item.ChildParameterItem)
                        {
                            FillDefault(childItem);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        #region MakeS14F3Child
        private void MakeS14F3Child(SECSMessage message, GEMObjectAttribute parentAttr, out string errorText)
        {
            string compact;
            dynamic converted;

            errorText = string.Empty;

            foreach (GEMObjectAttribute attr in parentAttr.ChildAttributes.Items)
            {
                if(string.IsNullOrEmpty(errorText) == true)
                {
                    if (attr.Format == SECSItemFormat.L)
                    {
                        message.Body.Add(attr.ATTRID, attr.Format, attr.ChildAttributes.Items.Count, string.Empty);
                        MakeS14F3Child(message, attr, out errorText);
                    }
                    else
                    {
                        if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                        {
                            message.Body.Add(attr.ATTRID, attr.Format, Encoding.Default.GetByteCount(attr.ATTRDATA), attr.ATTRDATA);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(attr.ATTRDATA) == true)
                            {
                                message.Body.Add(attr.ATTRID, attr.Format, 0, string.Empty);
                            }
                            else
                            {
                                compact = attr.ATTRDATA;

                                if (compact.IndexOf(" ") > -1)
                                {
                                    AddListToMessage(message, attr.Format, attr.ATTRID, 0, false, compact, out errorText);
                                }
                                else
                                {
                                    converted = ConvertValue(attr.Format, compact);

                                    if (converted == null)
                                    {
                                        message.Body.Add(attr.ATTRID, attr.Format, 0, string.Empty);
                                    }
                                    else
                                    {
                                        message.Body.Add(attr.ATTRID, attr.Format, 1, converted);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region SetATTRDATA
        private void SetATTRDATA(SECSItem secsItem, GEMObjectAttribute attr)
        {
            attr.Format = secsItem.Format;

            if (secsItem.Format == SECSItemFormat.L)
            {
                SetATTRDATA(secsItem.SubItem, attr.ChildAttributes);
            }
            else
            {
                attr.ATTRDATA = secsItem.Value.ToString();
            }
        }
        private void SetATTRDATA(SECSItemCollection secsItemCollection, GEMObjectAttributeCollection attrCollection)
        {
            GEMObjectAttribute attr;
            string attrID;

            for (int i = 0; i < secsItemCollection.Count; i++)
            {
                if (secsItemCollection.Items[i].Format == SECSItemFormat.L)
                {
                    attrID = string.Format("LIST_{0}", (i + 1));
                }
                else
                {
                    attrID = string.Format("DATA_{0}", (i + 1));
                }

                attr = attrCollection[attrID];
                if (attr == null)
                {
                    attr = new GEMObjectAttribute()
                    {
                        ATTRID = attrID,
                    };
                    attrCollection.Add(attr);
                }

                SetATTRDATA(secsItemCollection.Items[i], attr);
            }
        }
        #endregion
        public void UpdateAckAndReply()
        {
            this.CurrentSetting.UpdateAckCollection(this._driver.MessageManager.Messages);
            this.CurrentSetting.UpdateReplyCollection(this._driver.MessageManager.Messages);
        }
    }
}