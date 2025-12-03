using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;

namespace UbiGEM.Net.Tool
{
    internal class ConfigFileManager
    {
        [Flags]
        internal enum ConfigType
        {
            All = 0x0001,
            GEMDriver = 0x0002,
            SECSDriver = 0x0004,
            DataDictionary = 0x0008,
            Variables = 0x0010,
            EquipmentConstants = 0x0020,
            Reports = 0x0040,
            CollectionEvents = 0x0080,
            Alarms = 0x0100,
            RemoteCommands = 0x0200,
            TraceDatas = 0x0400
        }

        private const int MAX_BACKUP_FILE = 10;

        private readonly object _saveLock;

        public string ConfigFileName;
        public GEMConfiguration GEMConfiguration;
        public Configurtion Configurtion;
        public CollectionEventCollection CollectionEventCollection;
        public ReportCollection ReportCollection;
        public VariableCollection VariableCollection;
        public DataDictionaryCollection DataDictionaryCollection;
        public TraceCollection TraceCollection;
        public AlarmCollection AlarmCollection;
        public RemoteCommandCollection RemoteCommandCollection;

        public ConfigFileManager()
        {
            this.ConfigFileName = string.Empty;
            this.GEMConfiguration = new GEMConfiguration();
            this.Configurtion = new Configurtion();
            this.CollectionEventCollection = new CollectionEventCollection();
            this.ReportCollection = new ReportCollection();
            this.VariableCollection = new VariableCollection();
            this.DataDictionaryCollection = new DataDictionaryCollection();
            this.TraceCollection = new TraceCollection();
            this.AlarmCollection = new AlarmCollection();
            this.RemoteCommandCollection = new RemoteCommandCollection();

            this._saveLock = new object();
        }

        public void LoadPreDefined()
        {
            XElement root;
            List<SECSItemFormat> allowableFormats;
            VariableInfo variableInfo;

            root = XElement.Load(new System.IO.StringReader(Resources.XML.PreDefined));

            this.CollectionEventCollection.Items.Clear();
            this.VariableCollection.Items.Clear();
            this.DataDictionaryCollection.Items.Clear();

            foreach (XElement tempCeid in root.Element("CEIDList").Elements("CEID"))
            {
                this.CollectionEventCollection.Add(new CollectionEventInfo()
                {
                    CEID = tempCeid.Attribute("ID").Value.ToString(),
                    Name = tempCeid.Attribute("Name").Value,
                    Description = tempCeid.Attribute("Description").Value,
                    Enabled = true,
                    IsUse = true,
                    PreDefined = true,
                    IsBase = bool.Parse(tempCeid.Attribute("IsBase").Value)
                });
            }

            foreach (XElement tempEcid in root.Element("ECIDList").Elements("ECID"))
            {
                variableInfo = new VariableInfo
                {
                    VID = tempEcid.Attribute("ID").Value.ToString(),
                    Name = tempEcid.Attribute("Name").Value,
                    VIDType = VariableType.ECV,
                    Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempEcid.Attribute("Format").Value),
                    Length = int.Parse(tempEcid.Attribute("Length").Value),
                    Units = tempEcid.Attribute("Unit").Value,
                    Min = (string.IsNullOrEmpty(tempEcid.Attribute("Min").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Min").Value) : null),
                    Max = (string.IsNullOrEmpty(tempEcid.Attribute("Max").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Max").Value) : null),
                    Default = tempEcid.Attribute("Default").Value,
                    Description = tempEcid.Attribute("Description").Value,
                    IsUse = true,
                    PreDefined = true
                };

                if (tempEcid.Attribute("Value") != null && string.IsNullOrEmpty(tempEcid.Attribute("Value").Value) == false)
                {
                    variableInfo.Value = GetSECSValue(variableInfo.Format, tempEcid.Attribute("Value").Value);
                }

                this.VariableCollection.Add(variableInfo);
            }

            foreach (XElement tempVid in root.Element("VIDList").Elements("VID"))
            {
                variableInfo = new VariableInfo
                {
                    VID = tempVid.Attribute("ID").Value.ToString(),
                    Name = tempVid.Attribute("Name").Value,
                    VIDType = (tempVid.Attribute("Class").Value == "SV" ? VariableType.SV : VariableType.DVVAL),
                    Format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempVid.Attribute("Format").Value),
                    Length = int.Parse(tempVid.Attribute("Length").Value),
                    Units = tempVid.Attribute("Unit").Value,
                    Min = (string.IsNullOrEmpty(tempVid.Attribute("Min").Value) == false ? (double?)double.Parse(tempVid.Attribute("Min").Value) : null),
                    Max = (string.IsNullOrEmpty(tempVid.Attribute("Max").Value) == false ? (double?)double.Parse(tempVid.Attribute("Max").Value) : null),
                    Default = tempVid.Attribute("Default").Value,
                    Description = tempVid.Attribute("Description").Value,
                    IsUse = true,
                    PreDefined = true
                };

                if (tempVid.Attribute("Value") != null && string.IsNullOrEmpty(tempVid.Attribute("Value").Value) == false)
                {
                    variableInfo.Value = GetSECSValue(variableInfo.Format, tempVid.Attribute("Value").Value);
                }

                this.VariableCollection.Add(variableInfo);
            }

            foreach (XElement tempDataItem in root.Element("DataItemList").Elements("DataItem"))
            {
                allowableFormats = new List<SECSItemFormat>();

                foreach (string tempFormat in tempDataItem.Attribute("AllowableFormats").Value.Split(','))
                {
                    allowableFormats.Add((SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempFormat));
                }

                this.DataDictionaryCollection.Add(new DataDictionaryInfo()
                {
                    Name = tempDataItem.Attribute("Name").Value,
                    Format = allowableFormats,
                    Length = int.Parse(tempDataItem.Attribute("Length").Value),
                    Description = tempDataItem.Attribute("Description").Value,
                    PreDefined = true
                });
            }
        }

        public GemDriverError LoadConfigFile(out string errorText, out string notice)
        {
            GemDriverError result;
            XElement root;
            XElement element;
            Configurtion.HSMS hsmsModeConfig;
            CollectionEventInfo collectionEventInfo;
            ReportInfo reportInfo;
            VariableInfo variableInfo;
            VariableInfo variableInfo2;
            RemoteCommandInfo remoteCommandInfo;
            EnhancedRemoteCommandInfo enhancedRemoteCommandInfo;
            EnhancedCommandParameterInfo enhancedCommandParameterInfo;
            EnhancedCommandParameterItem enhancedCommandParameterItem;
            EnhancedCommandParameterItem enhancedCommandParameterItem2;
            TraceInfo traceInfo;
            string id;
            string name;
            string readAttribute;
            string[] splitData;
            List<UbiCom.Net.Structure.SECSItemFormat> allowableFormats;
            List<UbiCom.Net.Structure.SECSItemFormat> formats;
            UbiCom.Net.Structure.SECSItemFormat itemFormat;

            result = GemDriverError.Ok;

            errorText = string.Empty;
            notice = string.Empty;

            if (System.IO.File.Exists(this.ConfigFileName) == false)
            {
                errorText = $"Configuration file dose not exist:{this.ConfigFileName}";

                return GemDriverError.NotExistFile;
            }

            try
            {
                result = GemDriverError.Ok;
                root = XElement.Load(this.ConfigFileName);

                this.DataDictionaryCollection.Items.Clear();
                this.VariableCollection.Items.Clear();
                this.ReportCollection.Items.Clear();
                this.CollectionEventCollection.Items.Clear();
                this.AlarmCollection.Items.Clear();
                this.RemoteCommandCollection.RemoteCommandItems.Clear();
                this.RemoteCommandCollection.EnhancedRemoteCommandItems.Clear();
                this.TraceCollection.Items.Clear();

                #region [GEM Driver Configuration]
                element = root.Element("GEMDriver");

                if (element != null)
                {
                    this.GEMConfiguration = new GEMConfiguration()
                    {
                        LogPath = (element.Element("LogPath") != null ? element.Element("LogPath").Value : "C:\\Logs"),
                        LogEnabledGEM = (element.Element("LogEnabledGEM") != null ? (Utility.Logger.LogMode)Enum.Parse(typeof(Utility.Logger.LogMode), element.Element("LogEnabledGEM").Value) : Utility.Logger.LogMode.Day),
                        LogExpirationDay = (element.Element("LogExpirationDay") != null ? int.Parse(element.Element("LogExpirationDay").Value) : 30),
                        ExtensionOption = new GEMExtensionOption()
                        {
                            UseAutoTimeSync = (element.Element("Extension").Element("AutoTimeSync") != null ? bool.Parse(element.Element("Extension").Element("AutoTimeSync").Value) : false),
                            UseFormattedPPValue = (element.Element("Extension").Element("FormattedPPValue") != null ? bool.Parse(element.Element("Extension").Element("FormattedPPValue").Value) : false),
                            UseS1F17InEQPOffline = (element.Element("Extension").Element("S1F17InEQPOffline") != null ? bool.Parse(element.Element("Extension").Element("S1F17InEQPOffline").Value) : false),
                        }
                    };
                }
                #endregion
                #region [SECS Driver Configuration]
                element = root.Element("SECSDriver");

                if (element != null)
                {
                    hsmsModeConfig = new Configurtion.HSMS();

                    this.Configurtion = new UbiCom.Net.Structure.Configurtion()
                    {
                        DriverName = (element.Element("Name") != null ? element.Element("Name").Value : "GEM_EQP"),
                        DeviceType = UbiCom.Net.Structure.DeviceType.Equipment,
                        DeviceID = (element.Element("DeviceID") != null ? int.Parse(element.Element("DeviceID").Value) : 0),
                        SECSMode = UbiCom.Net.Structure.SECSMode.HSMS,
                        MaxMessageSize = (element.Element("MaxMessageSize") != null ? double.Parse(element.Element("MaxMessageSize").Value) : 2 * 1024 * 1024),
                        UMDFileName = (element.Element("UMDFileName") != null ? element.Element("UMDFileName").Value : string.Empty),
                        LogEnabledSECS1 = (element.Element("LogEnabledSECS1") != null ? (UbiCom.Net.Structure.LogMode)Enum.Parse(typeof(UbiCom.Net.Structure.LogMode), element.Element("LogEnabledSECS1").Value) : LogMode.None),
                        LogEnabledSECS2 = (element.Element("LogEnabledSECS2") != null ? (UbiCom.Net.Structure.LogMode)Enum.Parse(typeof(UbiCom.Net.Structure.LogMode), element.Element("LogEnabledSECS2").Value) : LogMode.None),
                        LogEnabledSystem = (element.Element("LogEnabledSystem") != null ? (UbiCom.Net.Structure.LogMode)Enum.Parse(typeof(UbiCom.Net.Structure.LogMode), element.Element("LogEnabledSystem").Value) : LogMode.None),
                        LogExpirationDay = (element.Element("LogExpirationDay") != null ? int.Parse(element.Element("LogExpirationDay").Value) : 0),
                        LogPath = (element.Element("LogPath") != null ? element.Element("LogPath").Value : @"C:\Logs")
                    };

                    if (element.Element("HSMS") != null)
                    {
                        if (element.Element("HSMS").Element("HSMSMode") != null)
                        {
                            hsmsModeConfig.HSMSMode = (UbiCom.Net.Structure.HSMSMode)Enum.Parse(typeof(UbiCom.Net.Structure.HSMSMode), element.Element("HSMS").Element("HSMSMode").Value);
                        }

                        if (element.Element("HSMS").Element("RemoteIPAddress") != null)
                        {
                            hsmsModeConfig.RemoteIPAddress = element.Element("HSMS").Element("RemoteIPAddress").Value;
                        }

                        if (element.Element("HSMS").Element("RemotePortNo") != null)
                        {
                            hsmsModeConfig.RemotePortNo = int.Parse(element.Element("HSMS").Element("RemotePortNo").Value);
                        }

                        if (element.Element("HSMS").Element("LocalIPAddress") != null)
                        {
                            hsmsModeConfig.LocalIPAddress = element.Element("HSMS").Element("LocalIPAddress").Value;
                        }

                        if (element.Element("HSMS").Element("LocalPortNo") != null)
                        {
                            hsmsModeConfig.LocalPortNo = int.Parse(element.Element("HSMS").Element("LocalPortNo").Value);
                        }

                        if (element.Element("HSMS").Element("Timeout") != null)
                        {
                            if (element.Element("HSMS").Element("Timeout").Element("T3") != null)
                            {
                                hsmsModeConfig.T3 = int.Parse(element.Element("HSMS").Element("Timeout").Element("T3").Value);
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T5") != null)
                            {
                                hsmsModeConfig.T5 = int.Parse(element.Element("HSMS").Element("Timeout").Element("T5").Value);
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T6") != null)
                            {
                                hsmsModeConfig.T6 = int.Parse(element.Element("HSMS").Element("Timeout").Element("T6").Value);
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T7") != null)
                            {
                                hsmsModeConfig.T7 = int.Parse(element.Element("HSMS").Element("Timeout").Element("T7").Value);
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("T8") != null)
                            {
                                hsmsModeConfig.T8 = int.Parse(element.Element("HSMS").Element("Timeout").Element("T8").Value);
                            }

                            if (element.Element("HSMS").Element("Timeout").Element("LinkTest") != null)
                            {
                                hsmsModeConfig.LinkTest = int.Parse(element.Element("HSMS").Element("Timeout").Element("LinkTest").Value);
                            }
                        }
                    }

                    this.Configurtion.HSMSModeConfig = hsmsModeConfig;
                }
                #endregion
                #region [Data Dictionary]
                foreach (XElement tempDataItem in root.Element("DataDictionary").Elements("DataItem"))
                {
                    allowableFormats = new List<UbiCom.Net.Structure.SECSItemFormat>();

                    if (tempDataItem.Attribute("AllowableFormats") != null)
                    {
                        foreach (string tempFormat in tempDataItem.Attribute("AllowableFormats").Value.Split(','))
                        {
                            allowableFormats.Add((UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempFormat));
                        }
                    }

                    formats = new List<UbiCom.Net.Structure.SECSItemFormat>();

                    if (tempDataItem.Attribute("Format") != null)
                    {
                        foreach (string tempFormat in tempDataItem.Attribute("Format").Value.Split(','))
                        {
                            formats.Add((UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempFormat));
                        }
                    }

                    this.DataDictionaryCollection.Add(new DataDictionaryInfo()
                    {
                        Name = (tempDataItem.Attribute("Name") != null ? tempDataItem.Attribute("Name").Value : string.Empty),
                        Format = formats,
                        AllowableFormats = allowableFormats,
                        Length = (tempDataItem.Attribute("Length") != null ? int.Parse(tempDataItem.Attribute("Length").Value) : 0),
                        Description = (tempDataItem.Attribute("Description") != null ? tempDataItem.Attribute("Description").Value : string.Empty),
                        PreDefined = (tempDataItem.Attribute("PreDefined") != null ? bool.Parse(tempDataItem.Attribute("PreDefined").Value) : false),
                    });
                }
                #endregion
                #region [Variables]
                foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                {
                    variableInfo = new VariableInfo()
                    {
                        VID = (tempVariable.Attribute("ID") != null ? tempVariable.Attribute("ID").Value : string.Empty),
                        Name = (tempVariable.Attribute("Name") != null ? tempVariable.Attribute("Name").Value : string.Empty),
                        VIDType = (tempVariable.Attribute("Class").Value == VariableType.SV.ToString() ? VariableType.SV : VariableType.DVVAL),
                        Format = (tempVariable.Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempVariable.Attribute("Format").Value) : SECSItemFormat.A),
                        Length = (tempVariable.Attribute("Length") != null ? int.Parse(tempVariable.Attribute("Length").Value) : 0),
                        Units = (tempVariable.Attribute("Unit") != null ? tempVariable.Attribute("Unit").Value : string.Empty),
                        Description = (tempVariable.Attribute("Description") != null ? tempVariable.Attribute("Description").Value : string.Empty),
                        IsUse = (tempVariable.Attribute("Use") != null ? bool.Parse(tempVariable.Attribute("Use").Value) : false),
                        PreDefined = (tempVariable.Attribute("PreDefined") != null ? bool.Parse(tempVariable.Attribute("PreDefined").Value) : false),
                    };

                    if (tempVariable.Attribute("Default") != null)
                    {
                        variableInfo.Default = GetSECSValue(variableInfo.Format, tempVariable.Attribute("Default").Value);
                    }
                    else
                    {
                        variableInfo.Default = string.Empty;
                    }

                    this.VariableCollection.Add(variableInfo);
                }
                #endregion
                #region [Equipment Constants]
                foreach (XElement tempEcid in root.Element("EquipmentConstants").Elements("ECID"))
                {
                    variableInfo = new VariableInfo
                    {
                        VID = (tempEcid.Attribute("ID") != null ? tempEcid.Attribute("ID").Value : string.Empty),
                        Name = (tempEcid.Attribute("Name") != null ? tempEcid.Attribute("Name").Value : string.Empty),
                        VIDType = VariableType.ECV,
                        Format = (tempEcid.Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempEcid.Attribute("Format").Value) : SECSItemFormat.A),
                        Length = (tempEcid.Attribute("Length") != null ? int.Parse(tempEcid.Attribute("Length").Value) : 0),
                        Units = (tempEcid.Attribute("Unit") != null ? tempEcid.Attribute("Unit").Value : string.Empty),
                        Min = (string.IsNullOrEmpty(tempEcid.Attribute("Min").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Min").Value) : null),
                        Max = (string.IsNullOrEmpty(tempEcid.Attribute("Max").Value) == false ? (double?)double.Parse(tempEcid.Attribute("Max").Value) : null),
                        Description = (tempEcid.Attribute("Description") != null ? tempEcid.Attribute("Description").Value : string.Empty),
                        IsUse = (tempEcid.Attribute("Use") != null ? bool.Parse(tempEcid.Attribute("Use").Value) : false),
                        PreDefined = (tempEcid.Attribute("PreDefined") != null ? bool.Parse(tempEcid.Attribute("PreDefined").Value) : false),
                    };

                    if (tempEcid.Attribute("Default") != null)
                    {
                        variableInfo.Default = GetSECSValue(variableInfo.Format, tempEcid.Attribute("Default").Value);
                    }
                    else
                    {
                        variableInfo.Default = string.Empty;
                    }

                    if (tempEcid.Attribute("Value") != null)
                    {
                        variableInfo.Value = GetSECSValue(variableInfo.Format, tempEcid.Attribute("Value").Value);
                    }
                    else
                    {
                        variableInfo.Value = string.Empty;
                    }

                    if (tempEcid.Element("MappingValues") != null)
                    {
                        foreach (XElement tempUserEcv in tempEcid.Element("MappingValues").Elements("MappingValue"))
                        {
                            if (tempUserEcv.Attribute("Name") != null && tempUserEcv.Attribute("Value") != null)
                            {
                                variableInfo.SetCustomMapping(tempUserEcv.Attribute("Name").Value, tempUserEcv.Attribute("Value").Value);
                            }
                        }
                    }

                    this.VariableCollection.Add(variableInfo);
                }
                #endregion
                #region [Child Variables]
                foreach (XElement tempVariable in root.Element("Variables").Elements("VID"))
                {
                    if (tempVariable.Attribute("ID") != null)
                    {
                        variableInfo = null;

                        id = tempVariable.Attribute("ID").Value.ToString();

                        if (string.IsNullOrEmpty(id) == false)
                        {
                            variableInfo = this.VariableCollection[id];
                        }
                        else
                        {
                            name = tempVariable.Attribute("Name").Value.ToString();

                            if (string.IsNullOrEmpty(name) == false)
                            {
                                variableInfo = this.VariableCollection.GetVariableInfo(name);
                            }
                        }

                        if (variableInfo != null && variableInfo.Format == SECSItemFormat.L)
                        {
                            if (tempVariable.Attribute("ChildVIDList") != null && string.IsNullOrEmpty(tempVariable.Attribute("ChildVIDList").Value) == false)
                            {
                                splitData = tempVariable.Attribute("ChildVIDList").Value.Split(',');

                                if (splitData != null && splitData.Length > 0)
                                {
                                    foreach (string tempVid in splitData)
                                    {
                                        variableInfo2 = this.VariableCollection[tempVid];

                                        if (variableInfo2 != null)
                                        {
                                            variableInfo.ChildVariables.Add(variableInfo2);
                                        }
                                    }
                                }
                            }
                            else if (tempVariable.Element("Childs") != null)
                            {
                                GetChildVariableInfo(variableInfo, tempVariable.Element("Childs"));
                            }
                        }
                    }
                }
                #endregion
                #region [Reports]
                foreach (XElement tempReport in root.Element("Reports").Elements("Report"))
                {
                    reportInfo = new ReportInfo()
                    {
                        ReportID = (tempReport.Attribute("ID") != null ? tempReport.Attribute("ID").Value : string.Empty),
                        Description = (tempReport.Attribute("Description") != null ? tempReport.Attribute("Description").Value : string.Empty),
                    };

                    readAttribute = tempReport.Attribute("IDList").Value;

                    foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                    {
                        variableInfo = this.VariableCollection[tempItem];

                        if (variableInfo != null)
                        {
                            reportInfo.Variables.Add(variableInfo);
                        }
                        else
                        {
                            notice += $"(RPT-V Link)Variable dose not exist:RPTID={reportInfo.ReportID}, VID={tempItem}\r\n";

                            continue;
                        }
                    }

                    if (result == GemDriverError.Ok)
                    {
                        this.ReportCollection.Add(reportInfo);
                    }
                    else
                    {
                        break;
                    }
                }
                #endregion
                #region [Collection Events]
                if (result == GemDriverError.Ok)
                {
                    foreach (XElement tempCollectionEvent in root.Element("CollectionEvents").Elements("CEID"))
                    {
                        collectionEventInfo = new CollectionEventInfo()
                        {
                            CEID = (tempCollectionEvent.Attribute("ID") != null ? tempCollectionEvent.Attribute("ID").Value.ToString() : string.Empty),
                            Name = (tempCollectionEvent.Attribute("Name") != null ? tempCollectionEvent.Attribute("Name").Value : string.Empty),
                            Description = (tempCollectionEvent.Attribute("Description") != null ? tempCollectionEvent.Attribute("Description").Value : string.Empty),
                            Enabled = (tempCollectionEvent.Attribute("Enabled") != null ? bool.Parse(tempCollectionEvent.Attribute("Enabled").Value) : false),
                            IsUse = (tempCollectionEvent.Attribute("Use") != null ? bool.Parse(tempCollectionEvent.Attribute("Use").Value) : false),
                            PreDefined = (tempCollectionEvent.Attribute("PreDefined") != null ? bool.Parse(tempCollectionEvent.Attribute("PreDefined").Value) : false),
                            IsBase = (tempCollectionEvent.Attribute("IsBase") != null ? bool.Parse(tempCollectionEvent.Attribute("IsBase").Value) : false),
                        };

                        readAttribute = tempCollectionEvent.Attribute("IDList").Value;

                        foreach (string tempItem in readAttribute.Split(',').Where(t => string.IsNullOrEmpty(t) == false))
                        {
                            if (string.IsNullOrEmpty(tempItem) == false)
                            {
                                reportInfo = this.ReportCollection[tempItem];

                                if (reportInfo != null)
                                {
                                    collectionEventInfo.Reports.Add(reportInfo);
                                }
                                else
                                {
                                    notice += $"(CE-RPT Link)RPT dose not exist:CEID={collectionEventInfo.CEID}, RPTID={tempItem}\r\n";

                                    continue;
                                }
                            }
                            else
                            {
                                notice += $"(CE-RPT Link)RPTID is empty:CEID={collectionEventInfo.CEID}, IDList={readAttribute}\r\n";

                                continue;
                            }
                        }

                        if (result == GemDriverError.Ok)
                        {
                            this.CollectionEventCollection.Add(collectionEventInfo);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                #endregion
                #region [Alarms]
                if (result == GemDriverError.Ok)
                {
                    foreach (XElement tempAlarm in root.Element("Alarms").Elements("Alarm"))
                    {
                        this.AlarmCollection.Add(new AlarmInfo
                        {
                            ID = (tempAlarm.Attribute("ID") != null ? long.Parse(tempAlarm.Attribute("ID").Value) : 0),
                            Code = (tempAlarm.Attribute("Code") != null ? int.Parse(tempAlarm.Attribute("Code").Value) : 0),
                            Enabled = (tempAlarm.Attribute("Enabled") != null ? bool.Parse(tempAlarm.Attribute("Enabled").Value) : false),
                            Description = (tempAlarm.Attribute("Description") != null ? tempAlarm.Attribute("Description").Value : string.Empty),
                        });
                    }
                }
                #endregion
                #region [Remote Commands]
                if (result == GemDriverError.Ok)
                {
                    foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("RemoteCommand"))
                    {
                        remoteCommandInfo = new RemoteCommandInfo()
                        {
                            RemoteCommand = (tempRemoteCommand.Attribute("Command") != null ? tempRemoteCommand.Attribute("Command").Value : string.Empty),
                            Description = (tempRemoteCommand.Attribute("Description") != null ? tempRemoteCommand.Attribute("Description").Value : string.Empty)
                        };

                        foreach (XElement tempParameter in tempRemoteCommand.Element("Parameters").Elements("Parameter"))
                        {
                            remoteCommandInfo.CommandParameter.Add(new CommandParameterInfo()
                            {
                                Name = (tempParameter.Attribute("Name") != null ? tempParameter.Attribute("Name").Value : string.Empty),
                                Format = (tempParameter.Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A),
                                Value = string.Empty
                            });
                        }

                        this.RemoteCommandCollection.Add(remoteCommandInfo);
                    }

                    foreach (XElement tempRemoteCommand in root.Element("RemoteCommands").Elements("EnhancedRemoteCommand"))
                    {
                        enhancedRemoteCommandInfo = new EnhancedRemoteCommandInfo()
                        {
                            RemoteCommand = (tempRemoteCommand.Attribute("Command") != null ? tempRemoteCommand.Attribute("Command").Value : string.Empty),
                            Description = (tempRemoteCommand.Attribute("Description") != null ? tempRemoteCommand.Attribute("Description").Value : string.Empty)
                        };

                        foreach (XElement tempParameter in tempRemoteCommand.Element("Parameters").Elements("Parameter"))
                        {
                            itemFormat = (tempParameter.Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempParameter.Attribute("Format").Value) : SECSItemFormat.A);

                            if (itemFormat == SECSItemFormat.L)
                            {
                                enhancedCommandParameterInfo = new EnhancedCommandParameterInfo()
                                {
                                    Name = (tempParameter.Attribute("Name") != null ? tempParameter.Attribute("Name").Value : string.Empty),
                                    Format = itemFormat,
                                    Value = string.Empty,
                                    ParameterType = (tempParameter.Attribute("Type") != null ? (CPType)Enum.Parse(typeof(CPType), tempParameter.Attribute("Type").Value) : CPType.A)
                                };

                                if (tempParameter.Element("Values") != null)
                                {
                                    foreach (XElement tempParameterValue in tempParameter.Element("Values").Elements("Value"))
                                    {
                                        enhancedCommandParameterItem = new EnhancedCommandParameterItem()
                                        {
                                            Name = (tempParameterValue.Attribute("Name") != null ? tempParameterValue.Attribute("Name").Value : string.Empty),
                                            Format = (tempParameterValue.Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempParameterValue.Attribute("Format").Value) : SECSItemFormat.A),
                                            Value = string.Empty,
                                            ParameterType = (tempParameterValue.Attribute("Type") != null ? (CPType)Enum.Parse(typeof(CPType), tempParameterValue.Attribute("Type").Value) : CPType.A)
                                        };

                                        if (enhancedCommandParameterItem.Format == SECSItemFormat.L)
                                        {
                                            if (tempParameterValue.Element("Values") != null)
                                            {
                                                GetChildEnhancedCommandParameterItem(enhancedCommandParameterItem, tempParameterValue.Element("Values"));
                                            }
                                            else if (tempParameterValue.Element("Value") != null)
                                            {
                                                enhancedCommandParameterItem2 = new EnhancedCommandParameterItem()
                                                {
                                                    Name = (tempParameterValue.Element("Value").Attribute("Name") != null ? tempParameterValue.Element("Value").Attribute("Name").Value : string.Empty),
                                                    Format = (tempParameterValue.Element("Value").Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempParameterValue.Element("Value").Attribute("Format").Value) : SECSItemFormat.A),
                                                    Value = string.Empty,
                                                    ParameterType = (tempParameterValue.Attribute("Type") != null ? (CPType)Enum.Parse(typeof(CPType), tempParameterValue.Attribute("Type").Value) : CPType.A)
                                                };

                                                GetChildEnhancedCommandParameterItem(enhancedCommandParameterItem2, tempParameterValue.Element("Value").Element("Values"));

                                                enhancedCommandParameterItem.ChildParameterItem.Items.Add(enhancedCommandParameterItem2);
                                            }
                                        }

                                        enhancedCommandParameterInfo.Items.Add(enhancedCommandParameterItem);
                                    }
                                }
                                else if (tempParameter.Element("Value") != null)
                                {
                                    XElement parameterValue = tempParameter.Element("Value");

                                    enhancedCommandParameterItem = new EnhancedCommandParameterItem()
                                    {
                                        Name = (parameterValue.Attribute("Name") != null ? parameterValue.Attribute("Name").Value : string.Empty),
                                        Format = (parameterValue.Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), parameterValue.Attribute("Format").Value) : SECSItemFormat.A),
                                        Value = string.Empty,
                                        ParameterType = (parameterValue.Attribute("Type") != null ? (CPType)Enum.Parse(typeof(CPType), parameterValue.Attribute("Type").Value) : CPType.A)
                                    };

                                    if (enhancedCommandParameterItem.Format == SECSItemFormat.L)
                                    {
                                        if (parameterValue.Element("Values") != null)
                                        {
                                            GetChildEnhancedCommandParameterItem(enhancedCommandParameterItem, parameterValue.Element("Values"));
                                        }
                                        else if (parameterValue.Element("Value") != null)
                                        {
                                            enhancedCommandParameterItem2 = new EnhancedCommandParameterItem()
                                            {
                                                Name = (parameterValue.Element("Value").Attribute("Name") != null ? parameterValue.Element("Value").Attribute("Name").Value : string.Empty),
                                                Format = (parameterValue.Element("Value").Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), parameterValue.Element("Value").Attribute("Format").Value) : SECSItemFormat.A),
                                                Value = string.Empty,
                                                ParameterType = (parameterValue.Attribute("Type") != null ? (CPType)Enum.Parse(typeof(CPType), parameterValue.Attribute("Type").Value) : CPType.A)
                                            };

                                            GetChildEnhancedCommandParameterItem(enhancedCommandParameterItem2, parameterValue.Element("Value").Element("Values"));

                                            enhancedCommandParameterItem.ChildParameterItem.Items.Add(enhancedCommandParameterItem2);
                                        }
                                    }

                                    enhancedCommandParameterInfo.Items.Add(enhancedCommandParameterItem);
                                }

                                enhancedRemoteCommandInfo.EnhancedCommandParameter.Add(enhancedCommandParameterInfo);
                            }
                            else
                            {
                                enhancedRemoteCommandInfo.EnhancedCommandParameter.Add(new EnhancedCommandParameterInfo()
                                {
                                    Name = (tempParameter.Attribute("Name") != null ? tempParameter.Attribute("Name").Value : string.Empty),
                                    Format = itemFormat,
                                    Value = string.Empty
                                });
                            }
                        }

                        this.RemoteCommandCollection.Add(enhancedRemoteCommandInfo);
                    }
                }
                #endregion
                #region [Trace Data]
                if (result == GemDriverError.Ok && root.Element("TraceDatas") != null && root.Element("TraceDatas").Elements("TraceData") != null)
                {
                    foreach (XElement tempTraceData in root.Element("TraceDatas").Elements("TraceData"))
                    {
                        traceInfo = new TraceInfo()
                        {
                            TraceID = (tempTraceData.Attribute("TraceID") != null ? tempTraceData.Attribute("TraceID").Value : string.Empty),
                            Dsper = (tempTraceData.Attribute("Period") != null ? tempTraceData.Attribute("Period").Value : string.Empty),
                            TotalSample = (tempTraceData.Attribute("TotalNumber") != null ? long.Parse(tempTraceData.Attribute("TotalNumber").Value) : 0),
                            ReportGroupSize = (tempTraceData.Attribute("GroupSize") != null ? long.Parse(tempTraceData.Attribute("GroupSize").Value) : 0)
                        };

                        foreach (XElement tempVariable in tempTraceData.Element("Variables").Elements("VID"))
                        {
                            variableInfo = this.VariableCollection[tempVariable.Attribute("ID").Value.ToString()];

                            if (variableInfo != null)
                            {
                                traceInfo.Variables.Add(variableInfo);
                            }
                        }

                        this.TraceCollection.Add(traceInfo);
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
                element = null;
                hsmsModeConfig = null;
                collectionEventInfo = null;
                reportInfo = null;
                variableInfo = null;
                remoteCommandInfo = null;
            }

            return result;
        }

        public GemDriverError SaveConfigFile(ConfigType configType, bool useBackup, out string errorText)
        {
            GemDriverError result;
            XElement root;

            errorText = string.Empty;

            try
            {
                lock (this._saveLock)
                {
                    result = GemDriverError.Ok;

                    if (System.IO.File.Exists(this.ConfigFileName) == true)
                    {
                        root = XElement.Load(this.ConfigFileName);
                    }
                    else
                    {
                        errorText = $"Configuration file dose not exist:{this.ConfigFileName}";

                        return GemDriverError.NotExistFile;
                    }

                    if (root == null)
                    {
                        root = new XElement("UbiGEM");
                    }

                    if (configType == ConfigType.All || configType == ConfigType.GEMDriver)
                    {
                        result = SaveConfigFileByGEMDriver(root, out errorText);
                    }

                    if (configType == ConfigType.All || configType == ConfigType.SECSDriver)
                    {
                        result = SaveConfigFileBySECSDriver(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.DataDictionary))
                    {
                        result = SaveConfigFileByDataDictionary(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.Variables))
                    {
                        result = SaveConfigFileByVariables(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.EquipmentConstants))
                    {
                        result = SaveConfigFileByEquipmentConstants(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.Reports))
                    {
                        result = SaveConfigFileByReports(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.CollectionEvents))
                    {
                        result = SaveConfigFileByCollectionEvents(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.Alarms))
                    {
                        result = SaveConfigFileByAlarms(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.RemoteCommands))
                    {
                        result = SaveConfigFileByRemoteCommands(root, out errorText);
                    }

                    if (result == GemDriverError.Ok && (configType == ConfigType.All || configType == ConfigType.TraceDatas))
                    {
                        result = SaveConfigFileByTraceDatas(root, out errorText);
                    }

                    if (result == GemDriverError.Ok)
                    {
                        if (useBackup == true)
                        {
                            try
                            {
                                string fileName = System.IO.Path.GetFileNameWithoutExtension(this.ConfigFileName);
                                string backupFilePath;
                                string backupFileName;
                                System.IO.DirectoryInfo directoryInfo;
                                List<System.IO.FileInfo> fileInfos;

                                backupFilePath = string.Format(@"{0}\UbiSam\UbiGEM\Backup\UGC Files", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                                backupFileName = string.Format(@"{0}\{1}_{2}.ugc", backupFilePath, fileName, DateTime.Now.ToString("MMddHHmmss"));

                                if (System.IO.Directory.Exists(backupFilePath) == false)
                                {
                                    System.IO.Directory.CreateDirectory(backupFilePath);
                                }

                                directoryInfo = new System.IO.DirectoryInfo(backupFilePath);

                                if (directoryInfo != null)
                                {
                                    fileInfos = directoryInfo.GetFiles("*.ugc").OrderByDescending(t => t.CreationTime).ToList();

                                    if (fileInfos != null && fileInfos.Count > MAX_BACKUP_FILE)
                                    {
                                        for (int i = MAX_BACKUP_FILE - 1; i < fileInfos.Count; i++)
                                        {
                                            System.IO.File.Delete(fileInfos[i].FullName);
                                        }
                                    }

                                    System.IO.File.Copy(this.ConfigFileName, backupFileName);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.Print(ex.Message);
                            }
                        }

                        try
                        {
                            string tempSaveFileName = $"{this.ConfigFileName}_";

                            root.Save(tempSaveFileName);

                            System.IO.File.Copy(tempSaveFileName, this.ConfigFileName, true);

                            try
                            {
                                System.IO.File.Delete(tempSaveFileName);
                            }
                            finally
                            {
                            }
                        }
                        catch (Exception ex)
                        {
                            result = GemDriverError.Unknown;
                            errorText = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }
            finally
            {
                root = null;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByGEMDriver(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;
            XElement subElement;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                subElement = new XElement("Extension");

                subElement.Add(new XElement("AutoTimeSync"), this.GEMConfiguration.ExtensionOption.UseAutoTimeSync);
                subElement.Add(new XElement("FormattedPPValue"), this.GEMConfiguration.ExtensionOption.UseFormattedPPValue);

                element = root.Element("GEMDriver");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("GEMDriver",
                                       new XElement("LogEnabledGEM", this.GEMConfiguration.LogEnabledGEM),
                                       new XElement("LogExpirationDay", this.GEMConfiguration.LogExpirationDay),
                                       new XElement("LogPath", this.GEMConfiguration.LogPath),
                                       new XElement(subElement));

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileBySECSDriver(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("SECSDriver");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("SECSDriver",
                                       new XElement("Name", this.Configurtion.DriverName),
                                       new XElement("Type", this.Configurtion.DeviceType),
                                       new XElement("IsAsyncMode", this.Configurtion.IsAsyncMode),
                                       new XElement("DeviceID", this.Configurtion.DeviceID),
                                       new XElement("Mode", this.Configurtion.SECSMode),
                                       new XElement("MaxMessageSize", this.Configurtion.MaxMessageSize),
                                       new XElement("UMDFileName", this.Configurtion.UMDFileName),
                                       new XElement("LogEnabledSECS1", this.Configurtion.LogEnabledSECS1),
                                       new XElement("LogEnabledSECS2", this.Configurtion.LogEnabledSECS2),
                                       new XElement("LogEnabledSystem", this.Configurtion.LogEnabledSystem),
                                       new XElement("LogExpirationDay", this.Configurtion.LogExpirationDay),
                                       new XElement("LogPath", this.Configurtion.LogPath),
                                       new XElement("HSMS",
                                                    new XElement("HSMSMode", this.Configurtion.HSMSModeConfig.HSMSMode),
                                                    new XElement("RemoteIPAddress", this.Configurtion.HSMSModeConfig.RemoteIPAddress),
                                                    new XElement("RemotePortNo", this.Configurtion.HSMSModeConfig.RemotePortNo),
                                                    new XElement("LocalIPAddress", this.Configurtion.HSMSModeConfig.LocalIPAddress),
                                                    new XElement("LocalPortNo", this.Configurtion.HSMSModeConfig.LocalPortNo),
                                                    new XElement("Timeout",
                                                                 new XElement("T3", this.Configurtion.HSMSModeConfig.T3),
                                                                 new XElement("T5", this.Configurtion.HSMSModeConfig.T5),
                                                                 new XElement("T6", this.Configurtion.HSMSModeConfig.T6),
                                                                 new XElement("T7", this.Configurtion.HSMSModeConfig.T7),
                                                                 new XElement("T8", this.Configurtion.HSMSModeConfig.T8),
                                                                 new XElement("LinkTest", this.Configurtion.HSMSModeConfig.LinkTest))));

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByDataDictionary(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("DataDictionary");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("DataDictionary");

                foreach (DataDictionaryInfo tempItem in this.DataDictionaryCollection.Items.Values)
                {
                    element.Add(new XElement("DataItem",
                                             new XAttribute("Name", tempItem.Name),
                                             new XAttribute("PreDefined", tempItem.PreDefined),
                                             new XAttribute("Format", tempItem.FormatString),
                                             new XAttribute("AllowableFormats", tempItem.AllowableFormatString),
                                             new XAttribute("Length", tempItem.Length),
                                             new XAttribute("Description", tempItem.Description)));
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByVariables(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;
            XElement variableElement;
            XElement childElement;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("Variables");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("Variables");

                foreach (VariableInfo tempItem in this.VariableCollection.Variables.Items)
                {
                    variableElement = new XElement("VID",
                                                   new XAttribute("ID", tempItem.VID),
                                                   new XAttribute("Name", tempItem.Name),
                                                   new XAttribute("Class", tempItem.VIDType),
                                                   new XAttribute("PreDefined", tempItem.PreDefined),
                                                   new XAttribute("Use", tempItem.IsUse),
                                                   new XAttribute("Format", tempItem.Format),
                                                   new XAttribute("Length", tempItem.Length),
                                                   new XAttribute("Unit", (tempItem.Units == null ? "" : tempItem.Units)),
                                                   new XAttribute("Min", (tempItem.Min == null ? "" : tempItem.Min.GetValueOrDefault().ToString())),
                                                   new XAttribute("Max", (tempItem.Max == null ? "" : tempItem.Max.GetValueOrDefault().ToString())),
                                                   new XAttribute("Default", tempItem.Default),
                                                   new XAttribute("Description", tempItem.Description));

                    if (tempItem.ChildVariables != null && tempItem.ChildVariables.Count > 0)
                    {
                        childElement = new XElement("Childs");

                        foreach (VariableInfo tempChildItem in tempItem.ChildVariables)
                        {
                            result = SetChildVariableInfo(tempChildItem, childElement, out errorText);
                        }

                        variableElement.Add(childElement);
                    }

                    element.Add(variableElement);
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByEquipmentConstants(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;
            XElement variableElement;
            XElement childElement;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("EquipmentConstants");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("EquipmentConstants");

                foreach (VariableInfo tempItem in this.VariableCollection.ECV.Items)
                {
                    variableElement = new XElement("ECID",
                                                   new XAttribute("ID", tempItem.VID),
                                                   new XAttribute("Name", tempItem.Name),
                                                   new XAttribute("Class", tempItem.VIDType),
                                                   new XAttribute("PreDefined", tempItem.PreDefined),
                                                   new XAttribute("Use", tempItem.IsUse),
                                                   new XAttribute("Format", tempItem.Format),
                                                   new XAttribute("Length", tempItem.Length),
                                                   new XAttribute("Unit", (tempItem.Units == null ? "" : tempItem.Units)),
                                                   new XAttribute("Min", (tempItem.Min == null ? "" : tempItem.Min.GetValueOrDefault().ToString())),
                                                   new XAttribute("Max", (tempItem.Max == null ? "" : tempItem.Max.GetValueOrDefault().ToString())),
                                                   new XAttribute("Value", tempItem.Value),
                                                   new XAttribute("Default", tempItem.Default),
                                                   new XAttribute("Description", tempItem.Description));

                    if (tempItem.HasCustomMapping == true)
                    {
                        childElement = new XElement("MappingValues");

                        foreach (KeyValuePair<string, string> tempCustomMapping in tempItem.CustomMapping)
                        {
                            childElement.Add(new XElement("MappingValue",
                                                          new XAttribute("Name", tempCustomMapping.Key),
                                                          new XAttribute("Value", tempCustomMapping.Value)));
                        }

                        variableElement.Add(childElement);
                    }

                    element.Add(variableElement);
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByReports(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;
            string writeAttribute;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("Reports");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("Reports");

                foreach (ReportInfo tempItem in this.ReportCollection.Items.Values)
                {
                    writeAttribute = string.Empty;

                    foreach (VariableInfo tempSubItem in tempItem.Variables.Items)
                    {
                        writeAttribute += tempSubItem.VID.ToString() + ",";
                    }

                    if (writeAttribute.Length > 0)
                        writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);

                    element.Add(new XElement("Report",
                                             new XAttribute("ID", tempItem.ReportID),
                                             new XAttribute("IDList", writeAttribute),
                                             new XAttribute("Description", tempItem.Description)));
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByCollectionEvents(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;
            string writeAttribute;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("CollectionEvents");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("CollectionEvents");

                foreach (CollectionEventInfo tempItem in this.CollectionEventCollection.Items.Values)
                {
                    writeAttribute = string.Empty;

                    foreach (ReportInfo tempSubItem in tempItem.Reports.Items.Values)
                    {
                        writeAttribute += tempSubItem.ReportID.ToString() + ",";
                    }

                    if (writeAttribute.Length > 0)
                        writeAttribute = writeAttribute.Substring(0, writeAttribute.Length - 1);

                    element.Add(new XElement("CEID",
                                             new XAttribute("ID", tempItem.CEID),
                                             new XAttribute("Name", tempItem.Name),
                                             new XAttribute("PreDefined", tempItem.PreDefined),
                                             new XAttribute("Use", tempItem.IsUse),
                                             new XAttribute("Enabled", tempItem.Enabled),
                                             new XAttribute("IDList", writeAttribute),
                                             new XAttribute("IsBase", tempItem.IsBase),
                                             new XAttribute("Description", tempItem.Description)));
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByAlarms(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("Alarms");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("Alarms");

                foreach (AlarmInfo tempItem in this.AlarmCollection.Items)
                {
                    element.Add(new XElement("Alarm",
                                             new XAttribute("ID", tempItem.ID),
                                             new XAttribute("Code", tempItem.Code),
                                             new XAttribute("Enabled", tempItem.Enabled),
                                             new XAttribute("Description", tempItem.Description)));
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByRemoteCommands(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;
            XElement subElement;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("RemoteCommands");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("RemoteCommands");

                foreach (RemoteCommandInfo tempItem in this.RemoteCommandCollection.RemoteCommandItems)
                {
                    subElement = new XElement("Parameters");

                    foreach (CommandParameterInfo tempSubItem in tempItem.CommandParameter.Items)
                    {
                        subElement.Add(new XElement("Parameter",
                                                    new XAttribute("Name", tempSubItem.Name),
                                                    new XAttribute("Format", tempSubItem.Format),
                                                    new XAttribute("Value", tempSubItem.Value.ToString())));
                    }

                    element.Add(new XElement("RemoteCommand",
                                             new XAttribute("Command", tempItem.RemoteCommand),
                                             new XAttribute("Description", tempItem.Description),
                                             subElement));
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }

        private GemDriverError SaveConfigFileByTraceDatas(XElement root, out string errorText)
        {
            GemDriverError result;
            XElement element;
            XElement subElement;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                element = root.Element("TraceDatas");

                if (element != null)
                {
                    element.Remove();
                }

                element = new XElement("TraceDatas");

                foreach (TraceInfo tempTraceInfo in this.TraceCollection.Items)
                {
                    subElement = new XElement("Variables");

                    tempTraceInfo.Variables.ForEach(t =>
                    {
                        subElement.Add(new XElement("VID", new XAttribute("ID", t)));
                    });

                    element.Add(new XElement("TraceData",
                                             new XAttribute("ID", tempTraceInfo.TraceID),
                                             new XAttribute("Period", tempTraceInfo.Dsper),
                                             new XAttribute("TotalNumber", tempTraceInfo.TotalSample),
                                             new XAttribute("GroupSize", tempTraceInfo.ReportGroupSize),
                                             subElement));
                }

                root.Add(element);
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }
            finally
            {
                element = null;
                subElement = null;
            }

            return result;
        }

        private static SECSValue GetSECSValue(SECSItemFormat secsItemFormat, string value)
        {
            SECSValue result;

            result = new SECSValue();

            switch (secsItemFormat)
            {
                case SECSItemFormat.Boolean:
                    {
                        if (bool.TryParse(value, out bool convert) == false)
                        {
                            if (value == "1")
                            {
                                convert = true;
                            }
                            else
                            {
                                convert = false;
                            }
                        }

                        result.SetValue(convert);
                    }

                    break;
                case SECSItemFormat.I1:
                    {
                        if (sbyte.TryParse(value, out sbyte convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.I2:
                    {
                        if (short.TryParse(value, out short convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.I4:
                    {
                        if (int.TryParse(value, out int convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.I8:
                    {
                        if (long.TryParse(value, out long convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.U1:
                    {
                        if (byte.TryParse(value, out byte convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.U2:
                    {
                        if (ushort.TryParse(value, out ushort convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.U4:
                    {
                        if (uint.TryParse(value, out uint convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.U8:
                    {
                        if (ulong.TryParse(value, out ulong convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.F4:
                    {
                        if (float.TryParse(value, out float convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                case SECSItemFormat.F8:
                    {
                        if (double.TryParse(value, out double convert) == false)
                        {
                            convert = 0;
                        }

                        result.SetValue(convert);
                    }
                    break;
                default:
                    result.SetValue(value);
                    break;
            }

            return result;
        }

        private void GetChildVariableInfo(VariableInfo variableInfo, XElement element)
        {
            VariableInfo childVariableInfo;

            foreach (XElement tempChildVariable in element.Elements("Child"))
            {
                if (tempChildVariable.Attribute("ID") != null && string.IsNullOrEmpty(tempChildVariable.Attribute("ID").Value) == false)
                {
                    childVariableInfo = this.VariableCollection[tempChildVariable.Attribute("ID").Value];

                    variableInfo.ChildVariables.Add(childVariableInfo);
                }
                else
                {
                    if (tempChildVariable.Attribute("Format") != null &&
                        tempChildVariable.Attribute("Format").Value == SECSItemFormat.L.ToString() &&
                        tempChildVariable.Element("Childs") != null &&
                        tempChildVariable.Element("Childs").HasElements == true)
                    {
                        childVariableInfo = new VariableInfo()
                        {
                            Format = SECSItemFormat.L,
                            Name = (tempChildVariable.Attribute("Name") != null ? tempChildVariable.Attribute("Name").Value : string.Empty),
                            Description = (tempChildVariable.Attribute("Description") != null ? tempChildVariable.Attribute("Description").Value : string.Empty)
                        };

                        if (tempChildVariable.Attribute("ID") != null)
                        {
                            GetChildVariableInfo(childVariableInfo, tempChildVariable.Element("Childs"));
                        }

                        variableInfo.ChildVariables.Add(childVariableInfo);
                    }
                }
            }
        }

        private void GetChildEnhancedCommandParameterItem(EnhancedCommandParameterItem parameterItem, XElement element)
        {
            EnhancedCommandParameterItem childParameterItem;

            if (element != null)
            {
                foreach (XElement tempParameterItem in element.Elements())
                {
                    childParameterItem = new EnhancedCommandParameterItem()
                    {
                        Name = (tempParameterItem.Attribute("Name") != null ? tempParameterItem.Attribute("Name").Value : string.Empty),
                        Format = (tempParameterItem.Attribute("Format") != null ? (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempParameterItem.Attribute("Format").Value) : SECSItemFormat.A),
                        Value = string.Empty,
                        ParameterType = (tempParameterItem.Attribute("Type") != null ? (CPType)Enum.Parse(typeof(CPType), tempParameterItem.Attribute("Type").Value) : CPType.A)
                    };

                    if (childParameterItem.Format == SECSItemFormat.L)
                    {
                        GetChildEnhancedCommandParameterItem(childParameterItem, tempParameterItem.Element("Values"));
                    }

                    parameterItem.ChildParameterItem.Items.Add(childParameterItem);
                }
            }
        }

        private GemDriverError SetChildVariableInfo(VariableInfo variableInfo, XElement element, out string errorText)
        {
            GemDriverError result;
            XElement childRootElement;
            XElement childElement;

            errorText = string.Empty;

            try
            {
                result = GemDriverError.Ok;

                if (variableInfo.ChildVariables != null && variableInfo.ChildVariables.Count > 0)
                {
                    childRootElement = new XElement("Child",
                                                    new XAttribute("Format", variableInfo.Format),
                                                    new XAttribute("ID", variableInfo.VID),
                                                    new XAttribute("Name", variableInfo.Name),
                                                    new XAttribute("Description", variableInfo.Description));

                    childElement = new XElement("Childs");

                    foreach (VariableInfo tempChildItem in variableInfo.ChildVariables)
                    {
                        if (tempChildItem.ChildVariables != null && tempChildItem.ChildVariables.Count > 0)
                        {
                            result = SetChildVariableInfo(tempChildItem, childElement, out errorText);

                            if (result != GemDriverError.Ok)
                            {
                                break;
                            }
                        }
                        else
                        {
                            childElement.Add(new XElement("Child",
                                                          new XAttribute("Format", tempChildItem.Format),
                                                          new XAttribute("ID", tempChildItem.VID),
                                                          new XAttribute("Name", tempChildItem.Name),
                                                          new XAttribute("Description", tempChildItem.Description)));
                        }
                    }

                    childRootElement.Add(childElement);
                    element.Add(childRootElement);
                }
                else
                {
                    element.Add(new XElement("Child",
                                             new XAttribute("Format", variableInfo.Format),
                                             new XAttribute("ID", variableInfo.VID),
                                             new XAttribute("Name", variableInfo.Name),
                                             new XAttribute("Description", variableInfo.Description)));
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Unknown;
                errorText = ex.Message;
            }

            return result;
        }
    }
}