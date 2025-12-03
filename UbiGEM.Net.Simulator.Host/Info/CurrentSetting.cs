using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Info;

namespace UbiGEM.Net.Simulator.Host
{
    public class UseReplyMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region MemberVariable
        private bool _sendReply;
        #endregion
        #region Property
        public int Stream { get; set; }
        public int Function { get; set; }
        public bool SendReply
        {
            get
            {
                return this._sendReply;
            }
            set
            {
                if (this._sendReply != value)
                {
                    this._sendReply = value;
                    NotifyPropertyChanged("SendReply");
                }
            }
        }
        #endregion
        #region Constructor
        public UseReplyMessage()
        {
            this.Stream = 1;
            this.Function = 1;
            this._sendReply = true;
        }
        #endregion
        #region NotifyPropertyChanged
        protected void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    
    public class KeyForSelectedObjectList
    {
        #region Property
        public string OBJSPEC { get; set; }
        public string OBJTYPE { get; set; }
        #endregion
        #region Constructor
        public KeyForSelectedObjectList()
        {
            this.OBJSPEC = string.Empty;
            this.OBJTYPE = string.Empty;
        }
        #endregion
        #region ToString
        public override string ToString()
        {
            return string.Format("[OBJSPEC={0},OBJTYPE={1}]", this.OBJSPEC, this.OBJTYPE);
        }
        #endregion
    }

    public class GEMObjectAttributeFilterInfo
    {
        #region Property
        public bool IsSelected { get; set; }
        public string ObjectID { get; set; }
        public string ATTRID { get; set; }
        public string ATTRDATA { get; set; }
        public string ATTRRELN { get; set; }
        #endregion
        #region Constructor
        public GEMObjectAttributeFilterInfo()
        {
            this.IsSelected = false;
            this.ObjectID = string.Empty;
            this.ATTRID = string.Empty;
            this.ATTRDATA = string.Empty;
            this.ATTRRELN = string.Empty;
        }
        #endregion
        #region ToString
        public override string ToString()
        {
            return string.Format("[IsSelected={0},ObjectID={1},ATTRID={2},ATTRDATA={3},ATTRRELN={4}]", this.IsSelected, this.ObjectID, this.ATTRID, this.ATTRDATA, this.ATTRRELN);
        }
        #endregion
    }

    public class AttachedObjectActionInfo
    {
        #region Property
        public byte OBJCMD { get; set; }
        public uint OBJTOKEN { get; set; }
        public string TARGETSPEC { get; set; }
        #endregion
        #region Constructor
        public AttachedObjectActionInfo()
        {
            this.OBJCMD = (byte)0;
            this.OBJTOKEN = (uint)0;
            this.TARGETSPEC = string.Empty;
        }
        #endregion
    }

    public class TerminalMessageInfo
    {
        public string S10F3TID { get; set; }

        public string S10F5TID { get; set; }

        public string S10F3TerminalMessage { get; set; }

        public List<string> S10F5TerminalMessages { get; set; }

        public TerminalMessageInfo()
        {
            this.S10F3TID = string.Empty;
            this.S10F5TID = string.Empty;
            this.S10F3TerminalMessage = "Terminal Message";
            this.S10F5TerminalMessages = new List<string>() { "Terminal Message" };
        }
    }

    public class CurrentSetting
    {
        public bool AutoSendDefineReport { get; set; }
        public bool AutoSendS1F13 { get; set; }

        public bool AutoSendEquipmentContants { get; set; }
        
        public List<string> S1F3SelectedEquipmentStatus { get; set; }

        public List<string> S1F11StatusVariableNamelist { get; set; }

        public List<string> S1F21DataVariableNamelist { get; set; }

        public List<string> S1F23CollectionEventList { get; set; }

        public List<string> S2F13EquipmentConstant { get; set; }

        public List<string> S2F15NewEquipmentConstant { get; set; }

        public List<string> S2F29EquipmentConstantNamelist { get; set; }

        public List<string> S2F43ResetSpoolingStreamsAndFunctions { get; set; }

        public List<string> S2F47VariableLimitAttributeRequest { get; set; }

        public List<long> S5F3SelectedAlarmSend { get; set; }

        public List<long> S5F3EnabledAlarmSend { get; set; }

        public List<long> S5F5ListAlarmsRequest { get; set; }

        public bool IsSaveRecipeReceived { get; set; }

        public TerminalMessageInfo TerminalMessage { get; set; }

        public List<byte> LoopbackDiagnostic { get; set; }

        public string SelectedObjectSpecifierForS14F1 { get; set; }
        public string SelectedObjectTypeForS14F1 { get; set; }
        public Dictionary<KeyForSelectedObjectList, List<GEMObjectAttributeFilterInfo>> SelectedObjectAttributeFilterListForS14F1 { get; private set; }

        public string SelectedObjectSpecifierForS14F3 { get; set; }
        public string SelectedObjectTypeForS14F3 { get; set; }

        public string SelectedObjectSpecifierForS14F5 { get; set; }
        public string SelectedObjectSpecifierForS14F7 { get; set; }
        public Dictionary<string,List<string>> SelectedObjectTypeListForS14F7 { get; private set; }

        public string SelectedObjectSpecifierForS14F9 { get; set; }
        public string SelectedObjectTypeForS14F9 { get; set; }

        public string SelectedObjectSpecifierForS14F11 { get; set; }
        public string SelectedObjectSpecifierForS14F13 { get; set; }

        public string SelectedObjectSpecifierForS14F15 { get; set; }
        public Dictionary<string, AttachedObjectActionInfo> SelectedAttachedObjectActionListForS14F15 { get; private set; }

        public string SelectedObjectSpecifierForS14F17 { get; set; }
        public Dictionary<string, AttachedObjectActionInfo> SelectedAttachedObjectActionListForS14F17 { get; private set; }
        public byte SelectedMapError { get; set; }
        public byte DATLC { get; set; }

        public string FormattedRecipeDirectory
        {
            get
            {
                string dirPath;
                string myDocumentPath;

                myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dirPath = string.Format(@"{0}\UbiSam\UbiGEM\UbiGEM.Net.Simulator\Recipe_Fmt", myDocumentPath);

                if (Directory.Exists(dirPath) == false)
                {
                    Directory.CreateDirectory(dirPath);
                }
                return dirPath;
            }
        }
        public string RecipeDirectory
        {
            get
            {
                string dirPath;
                string myDocumentPath;

                myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dirPath = string.Format(@"{0}\UbiSam\UbiGEM\UbiGEM.Net.Simulator\Recipe", myDocumentPath);

                if (Directory.Exists(dirPath) == false)
                {
                    Directory.CreateDirectory(dirPath);
                }
                return dirPath;
            }
        }



        public string ProcessProgramFileS7F1 { get; set; }
        public string ProcessProgramFileS7F3 { get; set; }
        public string ProcessProgramIDS7F5 { get; set; }
        
        public string ProcessProgramIDS7F23 { get; set; }
        public string ProcessProgramIDS7F25 { get; set; }

        public List<string> ProcessProgramDelete { get; set; }

        public AckCollection AckCollection { get; private set; }

        public List<UseReplyMessage> UseReplyCollection { get; set; }

        public CurrentSetting()
        {
            this.AutoSendEquipmentContants = false;
            this.AutoSendDefineReport = false;
            this.AutoSendS1F13 = true;
            this.S1F3SelectedEquipmentStatus = new List<string>();
            this.S1F11StatusVariableNamelist = new List<string>();
            this.S1F21DataVariableNamelist = new List<string>();
            this.S1F23CollectionEventList = new List<string>();
            this.S2F13EquipmentConstant = new List<string>();
            this.S2F15NewEquipmentConstant = new List<string>();
            this.S2F29EquipmentConstantNamelist = new List<string>();
            this.S2F43ResetSpoolingStreamsAndFunctions = new List<string>();
            this.S2F47VariableLimitAttributeRequest = new List<string>();
            this.S5F3SelectedAlarmSend = new List<long>();
            this.S5F3EnabledAlarmSend = new List<long>();
            this.S5F5ListAlarmsRequest = new List<long>();
            this.LoopbackDiagnostic = new List<byte>();
            this.TerminalMessage = new TerminalMessageInfo();
            this.ProcessProgramFileS7F1 = string.Empty;
            this.ProcessProgramFileS7F3 = string.Empty;
            this.ProcessProgramIDS7F5 = string.Empty;
            this.ProcessProgramIDS7F23 = string.Empty;
            this.ProcessProgramIDS7F25 = string.Empty;
            this.ProcessProgramDelete = new List<string>();
            this.AckCollection = new AckCollection();
            this.UseReplyCollection = new List<UseReplyMessage>();

            this.SelectedObjectSpecifierForS14F1 = string.Empty;
            this.SelectedObjectTypeForS14F1 = string.Empty;
            this.SelectedObjectAttributeFilterListForS14F1 = new Dictionary<KeyForSelectedObjectList, List<GEMObjectAttributeFilterInfo>>();

            this.SelectedObjectSpecifierForS14F3 = string.Empty;
            this.SelectedObjectTypeForS14F3 = string.Empty;

            this.SelectedObjectSpecifierForS14F5 = string.Empty;

            this.SelectedObjectSpecifierForS14F7 = string.Empty;
            this.SelectedObjectTypeListForS14F7 = new Dictionary<string, List<string>>();

            this.SelectedObjectSpecifierForS14F9 = string.Empty;
            this.SelectedObjectTypeForS14F9 = string.Empty;

            this.SelectedObjectSpecifierForS14F11 = string.Empty;
            this.SelectedObjectSpecifierForS14F13 = string.Empty;

            this.SelectedObjectSpecifierForS14F15 = string.Empty;
            this.SelectedAttachedObjectActionListForS14F15 = new Dictionary<string, AttachedObjectActionInfo>();

            this.SelectedObjectSpecifierForS14F17 = string.Empty;
            this.SelectedAttachedObjectActionListForS14F17 = new Dictionary<string, AttachedObjectActionInfo>();

            this.IsSaveRecipeReceived = false;
        }

        public void Clear()
        {
            this.AutoSendEquipmentContants = false;
            this.AutoSendDefineReport = false;
            this.AutoSendS1F13 = true;
            this.S1F3SelectedEquipmentStatus.Clear();
            this.S1F11StatusVariableNamelist.Clear();
            this.S1F21DataVariableNamelist.Clear();
            this.S1F23CollectionEventList.Clear();
            this.S2F13EquipmentConstant.Clear();
            this.S2F15NewEquipmentConstant.Clear();
            this.S2F29EquipmentConstantNamelist.Clear();
            this.S2F43ResetSpoolingStreamsAndFunctions.Clear();
            this.S2F47VariableLimitAttributeRequest.Clear();
            this.S5F3SelectedAlarmSend.Clear();
            this.S5F3EnabledAlarmSend.Clear();
            this.S5F5ListAlarmsRequest.Clear();
            this.LoopbackDiagnostic.Clear();
            this.TerminalMessage.S10F3TID = string.Empty;
            this.TerminalMessage.S10F3TerminalMessage = string.Empty;
            this.TerminalMessage.S10F5TID = string.Empty;
            this.TerminalMessage.S10F5TerminalMessages.Clear();
            this.ProcessProgramFileS7F1 = string.Empty;
            this.ProcessProgramFileS7F3 = string.Empty;
            this.ProcessProgramIDS7F5 = string.Empty;
            this.ProcessProgramIDS7F23 = string.Empty;
            this.ProcessProgramIDS7F25 = string.Empty;
            this.ProcessProgramDelete.Clear();
            this.AckCollection.Clear();
            this.UseReplyCollection.Clear();

            this.SelectedObjectSpecifierForS14F1 = string.Empty;
            this.SelectedObjectTypeForS14F1 = string.Empty;
            this.SelectedObjectAttributeFilterListForS14F1.Clear();

            this.SelectedObjectSpecifierForS14F3 = string.Empty;
            this.SelectedObjectTypeForS14F3 = string.Empty;

            this.SelectedObjectSpecifierForS14F5 = string.Empty;
            this.SelectedObjectSpecifierForS14F7 = string.Empty;
            this.SelectedObjectTypeListForS14F7.Clear();

            this.SelectedObjectSpecifierForS14F9 = string.Empty;
            this.SelectedObjectTypeForS14F9 = string.Empty;

            this.SelectedObjectSpecifierForS14F11 = string.Empty;
            this.SelectedObjectSpecifierForS14F13 = string.Empty;

            this.SelectedObjectSpecifierForS14F15 = string.Empty;
            this.SelectedAttachedObjectActionListForS14F15.Clear();

            this.SelectedObjectSpecifierForS14F17 = string.Empty;
            this.SelectedAttachedObjectActionListForS14F17.Clear();

            this.IsSaveRecipeReceived = false;
        }
        public void UpdateAckCollection(SECSMessageCollection secsMessages)
        {
            SECSItem ackItem;
            AckInfo ackInfo;

            if (secsMessages != null)
            {
                foreach (KeyValuePair<string, SECSMessage> keyValuePair in secsMessages.MessageInfo)
                {
                    SECSMessage message = keyValuePair.Value;

                    if (message != null && message.Function % 2 == 0 && message.Function != 0 && (message.Direction == SECSMessageDirection.Both || message.Direction == SECSMessageDirection.ToEquipment))
                    {
                        ackItem = null;

                        if (message.Stream == 1 && message.Function == 14)
                        {
                            ackItem = message.Body.AsList[1];
                        }
                        else if (message.Body.AsList.Count == 1)
                        {
                            ackItem = message.Body.AsList[0];
                        }

                        if (ackItem != null && ackItem.Format == SECSItemFormat.B && Enum.TryParse(ackItem.Name, out DataDictinaryList dataDictinary) == true)
                        {
                            switch (dataDictinary)
                            {
                                case DataDictinaryList.COMMACK:
                                case DataDictinaryList.ACKC6:
                                case DataDictinaryList.ACKC7:
                                case DataDictinaryList.ACKC10:
                                case DataDictinaryList.SDACK:
                                case DataDictinaryList.GRNT1:
                                case DataDictinaryList.MDACK:
                                case DataDictinaryList.OBJACK:

                                    ackInfo = this.AckCollection[message.Stream, message.Function];

                                    if (ackInfo == null)
                                    {
                                        ackInfo = new AckInfo()
                                        {
                                            Stream = message.Stream,
                                            Function = message.Function,
                                            Use = false,
                                            Value = 0,
                                        };

                                        this.AckCollection.Add(ackInfo);
                                    }
                                    else
                                    {
                                        ackInfo.DataDictinary = dataDictinary;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            //this.GenerateAckCollection();
        }
        /*
        private void GenerateAckCollection()
        {
            List<Tuple<int, int, DataDictinaryList>> tuples;

            tuples = new List<Tuple<int, int, DataDictinaryList>>
            {
                new Tuple<int, int, DataDictinaryList>(1, 14, DataDictinaryList.COMMACK),
                new Tuple<int, int, DataDictinaryList>(6, 2, DataDictinaryList.ACKC6),
                new Tuple<int, int, DataDictinaryList>(6, 4, DataDictinaryList.ACKC6),
                new Tuple<int, int, DataDictinaryList>(6, 6, DataDictinaryList.ACKC6),
                new Tuple<int, int, DataDictinaryList>(6, 10, DataDictinaryList.ACKC6),
                new Tuple<int, int, DataDictinaryList>(6, 12, DataDictinaryList.ACKC6),
                new Tuple<int, int, DataDictinaryList>(6, 14, DataDictinaryList.ACKC6),
                new Tuple<int, int, DataDictinaryList>(6, 26, DataDictinaryList.ACKC6),
                new Tuple<int, int, DataDictinaryList>(7, 2, DataDictinaryList.ACKC7),
                new Tuple<int, int, DataDictinaryList>(7, 4, DataDictinaryList.ACKC7),
                new Tuple<int, int, DataDictinaryList>(7, 24, DataDictinaryList.ACKC7),
                new Tuple<int, int, DataDictinaryList>(7, 30, DataDictinaryList.ACKC7),
                new Tuple<int, int, DataDictinaryList>(10, 2, DataDictinaryList.ACKC10),
                new Tuple<int, int, DataDictinaryList>(12, 2, DataDictinaryList.SDACK),
                new Tuple<int, int, DataDictinaryList>(12, 6, DataDictinaryList.GRNT1),
                new Tuple<int, int, DataDictinaryList>(12, 8, DataDictinaryList.MDACK),
                new Tuple<int, int, DataDictinaryList>(12, 10, DataDictinaryList.MDACK),
                new Tuple<int, int, DataDictinaryList>(12, 12, DataDictinaryList.MDACK),
                new Tuple<int, int, DataDictinaryList>(14, 2, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 4, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 6, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 8, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 10, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 12, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 14, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 16, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 18, DataDictinaryList.OBJACK),
                new Tuple<int, int, DataDictinaryList>(14, 20, DataDictinaryList.OBJACK),
            };

            AckInfo ackInfo;

            foreach (Tuple<int, int, DataDictinaryList> tuple in tuples)
            {
                ackInfo = this.AckCollection[tuple.Item1, tuple.Item2];

                if (ackInfo == null)
                {
                    ackInfo = new AckInfo()
                    {
                        Stream = tuple.Item1,
                        Function = tuple.Item2,
                        DataDictinary = tuple.Item3,
                        Use = false,
                        Value = 0,
                    };

                    this.AckCollection.Add(ackInfo);
                }
                else
                {
                    ackInfo.DataDictinary = tuple.Item3;
                }
            }
        }
        */
        public void UpdateReplyCollection(SECSMessageCollection secsMessages)
        {
            if (secsMessages != null)
            {
                foreach (KeyValuePair<string, SECSMessage> keyValuePair in secsMessages.MessageInfo)
                {
                    SECSMessage message = keyValuePair.Value;

                    if (message != null && message.Function % 2 == 1 && (message.Direction == SECSMessageDirection.Both || message.Direction == SECSMessageDirection.ToHost))
                    {
                        if (this.UseReplyCollection.Exists(t => t.Stream == message.Stream && t.Function == message.Function) == false)
                        {
                            this.UseReplyCollection.Add(new UseReplyMessage()
                            {
                                Stream = message.Stream,
                                Function = message.Function,
                            });
                        }
                    }
                }
            }

            //this.GenerateUseReplyCollection();
        }
        /*
        private void GenerateUseReplyCollection()
        {
            List<Tuple<int, int>> tuples;

            tuples = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 1),
                new Tuple<int, int>(1, 13),
                new Tuple<int, int>(2, 17),
                new Tuple<int, int>(2, 25),
                new Tuple<int, int>(5, 1),
                new Tuple<int, int>(6, 1),
                new Tuple<int, int>(6, 11),
                new Tuple<int, int>(7, 1),
                new Tuple<int, int>(7, 3),
                new Tuple<int, int>(7, 5),
                new Tuple<int, int>(7, 23),
                new Tuple<int, int>(7, 25),
                new Tuple<int, int>(7, 27),
                new Tuple<int, int>(10, 1),
                new Tuple<int, int>(12, 1),
                new Tuple<int, int>(12, 3),
                new Tuple<int, int>(12, 5),
                new Tuple<int, int>(12, 7),
                new Tuple<int, int>(12, 9),
                new Tuple<int, int>(12, 11),
                new Tuple<int, int>(12, 13),
                new Tuple<int, int>(12, 15),
                new Tuple<int, int>(12, 17),
                new Tuple<int, int>(14, 1),
                new Tuple<int, int>(14, 3),
                new Tuple<int, int>(14, 5),
                new Tuple<int, int>(14, 7),
                new Tuple<int, int>(14, 9),
                new Tuple<int, int>(14, 11),
                new Tuple<int, int>(14, 13),
                new Tuple<int, int>(14, 15),
                new Tuple<int, int>(14, 17)
            };

            foreach (var tuple in tuples)
            {
                if (this.UseReplyCollection.Exists(t => t.Stream == tuple.Item1 && t.Function == tuple.Item2) == false)
                {
                    this.UseReplyCollection.Add(new UseReplyMessage()
                    {
                        Stream = tuple.Item1,
                        Function = tuple.Item2,
                    });
                }
            }
        }
        */
    }
}