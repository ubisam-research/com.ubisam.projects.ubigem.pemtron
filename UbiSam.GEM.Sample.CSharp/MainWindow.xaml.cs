using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;
using UbiGEM.Net.Utility.Logger;

namespace UbiSam.GEM.Sample.CSharp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region [Constant]
        private readonly string DATETIME_TEXT_FORMAT = "{0} [{1}] {2}" + Environment.NewLine;
        private const string PROGRAM_TITLE_FORMAT = "{0} - {1}";
        private const string PROGRAM_STATUS_FORMAT = "{0} - {1}:{2}";

        private const string UGC_FILE_FILTER = "UbiGEM Configuration File (*.ugc)|*.ugc|All files (*.*)|*.*";
        private const string PROGRAM_DEFAULT_TITLE = "UbiSam.GEM.Sample.CSharp";

        private const int LOG_LINE_MAX_COUNT = 100;
        #endregion
        #region [Variable]
        private UbiGEM.Net.Driver.GemDriver _gemDriver;
        private List<long> _setAlarmList;
        private int _ack;
        private string _ugcFileName;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Initialize();
            UpdateTitle();
        }

        #region [Initialize]
        private void Initialize()
        {
            _gemDriver = new UbiGEM.Net.Driver.GemDriver();
            _setAlarmList = new List<long>();

            #region [Communication Event]
            _gemDriver.OnCommunicationStateChanged += GemDriver_OnCommunicationStateChanged;
            _gemDriver.OnControlStateChanged += GemDriver_OnControlStateChanged;
            _gemDriver.OnEquipmentProcessState += GemDriver_OnEquipmentProcessState;
            _gemDriver.OnGEMConnected += GemDriver_OnGEMConnected;
            _gemDriver.OnGEMSelected += GemDriver_OnGEMSelected;
            _gemDriver.OnGEMDeselected += GemDriver_OnGEMDeselected;
            _gemDriver.OnGEMDisconnected += GemDriver_OnGEMDisconnected;
            _gemDriver.OnControlStateOnlineChangeFailed += GemDriver_OnControlStateOnlineChangeFailed;
            #endregion

            #region [Received Message Event]
            _gemDriver.OnReceivedRequestOffline += GemDriver_OnReceivedRequestOffline;
            _gemDriver.OnReceivedRequestOnline += GemDriver_OnReceivedRequestOnline;
            _gemDriver.OnReceivedDefineReport += GemDriver_OnReceivedDefineReport;
            _gemDriver.OnReceivedLinkEventReport += GemDriver_OnReceivedLinkEventReport;
            _gemDriver.OnReceivedEnableDisableEventReport += GemDriver_OnReceivedEnableDisableEventReport;
            _gemDriver.OnReceivedRemoteCommand += GemDriver_OnReceivedRemoteCommand;
            _gemDriver.OnReceivedEnhancedRemoteCommand += GemDriver_OnReceivedEnhancedRemoteCommand;
            _gemDriver.OnReceivedNewECVSend += GemDriver_OnReceivedNewECVSend;
            _gemDriver.OnReceivedEnableDisableAlarmSend += GemDriver_OnReceivedEnableDisableAlarmSend;
            _gemDriver.OnReceivedTerminalMessage += GemDriver_OnReceivedTerminalMessage;
            _gemDriver.OnReceivedTerminalMultiMessage += GemDriver_OnReceivedTerminalMultiMessage;
            _gemDriver.OnReceivedPPRequest += GemDriver_OnReceivedPPRequest;
            _gemDriver.OnReceivedPPSend += GemDriver_OnReceivedPPSend;
            _gemDriver.OnReceivedPPLoadInquire += GemDriver_OnReceivedPPLoadInquire;
            _gemDriver.OnReceivedDeletePPSend += GemDriver_OnReceivedDeletePPSend;
            _gemDriver.OnReceivedFmtPPRequest += GemDriver_OnReceivedFmtPPRequest;
            _gemDriver.OnReceivedFmtPPSend += GemDriver_OnReceivedFmtPPSend;
            _gemDriver.OnReceivedCurrentEPPDRequest += GemDriver_OnReceivedCurrentEPPDRequest;
            _gemDriver.OnReceivedDateTimeRequest += GemDriver_OnReceivedDateTimeRequest;
            _gemDriver.OnReceivedDateTimeSetRequest += GemDriver_OnReceivedDateTimeSetRequest;
            _gemDriver.OnReceivedLoopback += GemDriver_OnReceivedLoopback;
            _gemDriver.OnReceivedEstablishCommunicationsRequest += GemDriver_OnReceivedEstablishCommunicationsRequest;
            _gemDriver.OnUserPrimaryMessageReceived += GemDriver_OnUserPrimaryMessageReceived;
            _gemDriver.OnUserSecondaryMessageReceived += GemDriver_OnUserSecondaryMessageReceived;
            _gemDriver.OnReceivedUnknownMessage += GemDriver_OnReceivedUnknownMessage;
            _gemDriver.OnInvalidMessageReceived += GemDriver_OnInvalidMessageReceived;
            _gemDriver.OnReceivedInvalidRemoteCommand += GemDriver_OnReceivedInvalidRemoteCommand;
            _gemDriver.OnReceivedInvalidEnhancedRemoteCommand += GemDriver_OnReceivedInvalidEnhancedRemoteCommand;
            #endregion

            #region [Response Message Event]
            _gemDriver.OnResponseTerminalRequest += GemDriver_OnResponseTerminalRequest;
            _gemDriver.OnResponsePPRequest += GemDriver_OnResponsePPRequest;
            _gemDriver.OnResponsePPSend += GemDriver_OnResponsePPSend;
            _gemDriver.OnResponsePPLoadInquire += GemDriver_OnResponsePPLoadInquire;
            _gemDriver.OnResponseFmtPPRequest += GemDriver_OnResponseFmtPPRequest;
            _gemDriver.OnResponseFmtPPSend += GemDriver_OnResponseFmtPPSend;
            _gemDriver.OnResponseFmtPPVerification += GemDriver_OnResponseFmtPPVerification;
            _gemDriver.OnResponseDateTimeRequest += GemDriver_OnResponseDateTimeRequest;
            _gemDriver.OnResponseLoopback += GemDriver_OnResponseLoopback;
            _gemDriver.OnResponseEventReportAcknowledge += GemDriver_OnResponseEventReportAcknowledge;
            #endregion

            #region [Request Message Event]
            _gemDriver.OnVariableUpdateRequest += GemDriver_OnVariableUpdateRequest;
            _gemDriver.OnUserGEMMessageUpdateRequest += GemDriver_OnUserGEMMessageUpdateRequest;
            _gemDriver.OnTraceDataUpdateRequest += GemDriver_OnTraceDataUpdateRequest;
            #endregion

            #region [Log Event]
            _gemDriver.OnWriteLog += GemDriver_OnWriteLog;
            _gemDriver.OnSECS1Log += GemDriver_OnSECS1Log;
            _gemDriver.OnSECS2Log += GemDriver_OnSECS2Log;
            #endregion
        }

        #region [Communication Event]
        /// <summary>
        /// Communication State가 변경될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="communicationState"></param>
        private void GemDriver_OnCommunicationStateChanged(CommunicationState communicationState)
        {
            WriteLog(LogLevel.Information, $"OnCommunicationStateChanged - {communicationState}");
        }

        /// <summary>
        /// Control State가 변경될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="controlState"></param>
        private void GemDriver_OnControlStateChanged(ControlState controlState)
        {
            WriteLog(LogLevel.Information, $"OnControlStateChanged - {controlState}");
        }

        /// <summary>
        /// EquipmentProcess State가 변경될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="equipmentProcessState"></param>
        private void GemDriver_OnEquipmentProcessState(byte equipmentProcessState)
        {
            WriteLog(LogLevel.Information, $"OnEquipmentProcessState - {equipmentProcessState}");
        }

        private void GemDriver_OnGEMConnected(string ipAddress, int portNo)
        {
            UpdateTitle("Connected", ipAddress, portNo);
        }

        private void GemDriver_OnGEMSelected(string ipAddress, int portNo)
        {
            UpdateTitle("Selected", ipAddress, portNo);
        }

        private void GemDriver_OnGEMDeselected(string ipAddress, int portNo)
        {
            UpdateTitle("Deselected", ipAddress, portNo);
        }

        private void GemDriver_OnGEMDisconnected(string ipAddress, int portNo)
        {
            UpdateTitle("Disconnected", ipAddress, portNo);
        }

        private void GemDriver_OnControlStateOnlineChangeFailed()
        {
            WriteLog(LogLevel.Error, "OnControlStateOnlineChangeFailed");
        }
        #endregion

        #region [Received Message Event]
        /// <summary>
        /// Host에서 S1F15(Offline Request)가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="systemBytes"></param>
        private void GemDriver_OnReceivedRequestOffline(uint systemBytes)
        {
            WriteLog(LogLevel.Information, "Received Request Offline");

            _gemDriver.ReplyRequestOfflineAck(systemBytes, _ack);
        }

        /// <summary>
        /// Host에서 S1F17(Online Request)가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        private void GemDriver_OnReceivedRequestOnline(uint systemBytes)
        {
            WriteLog(LogLevel.Information, "Received Request Online");

            _gemDriver.ReplyRequestOnlineAck(systemBytes, _ack);
        }

        /// <summary>
        /// S2F33(Define Report)가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        private void GemDriver_OnReceivedDefineReport()
        {
            WriteLog(LogLevel.Information, "Received Define Report");
        }

        /// <summary>
        /// S2F35(Link Event Report)가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        private void GemDriver_OnReceivedLinkEventReport()
        {
            WriteLog(LogLevel.Information, "Received LinkEvent Report");
        }

        /// <summary>
        /// S2F37(Event Report Enable/Disable)이 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        private void GemDriver_OnReceivedEnableDisableEventReport()
        {
            WriteLog(LogLevel.Information, "Received Enable Disable Event Send");
        }

        /// <summary>
        /// Host에서 S2F41(Remote Command)가 수신될 때 발생하는 이벤트입니다.
        /// RemoteCommandInfo 의 아이템을 순회하는 코드입니다.
        /// RemoteCommandParameterResult 에 IRemoteCommandParameterResult 를 추가하여 parameter 별 ack를 구성할 수 있습니다.
        /// </summary>
        /// <param name="remoteCommandInfo"></param>
        private void GemDriver_OnReceivedRemoteCommand(RemoteCommandInfo remoteCommandInfo)
        {
            RemoteCommandResult result = new RemoteCommandResult();
            RemoteCommandParameterResult paramResult;
            string logText;

            result.HostCommandAck = _ack;

            foreach (CommandParameterInfo paramInfo in remoteCommandInfo.CommandParameter.Items)
            {
                paramResult = new RemoteCommandParameterResult(paramInfo.Name, (int)CPACK.IllegalFormatSpecifiedForCPVAL);
                result.Items.Add(paramResult);
            }

            logText = $"[RemoteCommand={remoteCommandInfo.RemoteCommand}]{Environment.NewLine}";

            foreach (CommandParameterInfo paramInfo in remoteCommandInfo.CommandParameter.Items)
            {
                logText += $": [CPNAME={paramInfo.Name},Format={paramInfo.Format},CPVAL={paramInfo.Value}]{Environment.NewLine}";
            }

            if (logText.Length > 0)
            {
                logText = logText.Substring(0, logText.Length - Environment.NewLine.Length);
            }

            WriteLog(LogLevel.Information, $"OnReceivedRemoteCommand : {logText}");

            //S2F42 Reply이전 호스트에서 받은 S2F41(EnhancedRemoteCommand)에 대한 Validation Check가 필요합니다.

            _gemDriver.ReplyRemoteCommandAck(remoteCommandInfo, result);

            //S2F42 Reply이후 관련된 Logic을 넣으면 됩니다.
        }

        /// <summary>
        /// Host에서 S2F49(Enhanced Remote Command)가 수신될 때 발생하는 이벤트입니다.
        /// EnhancedRemoteCommandInfo 의 아이템을 순회하는 코드입니다.
        /// RemoteCommandResult 에 RemoteCommandParameterResult 를 추가하여 parameter 별 ack를 구성할 수 있습니다.
        /// </summary>
        /// <param name="remoteCommandInfo"></param>
        private void GemDriver_OnReceivedEnhancedRemoteCommand(EnhancedRemoteCommandInfo remoteCommandInfo)
        {
            RemoteCommandResult result = new RemoteCommandResult();
            RemoteCommandParameterResult paramResult;
            string logText;

            result.HostCommandAck = _ack;

            logText = $"[RemoteCommand={remoteCommandInfo.RemoteCommand}]{Environment.NewLine}";

            foreach (EnhancedCommandParameterInfo paramInfo in remoteCommandInfo.EnhancedCommandParameter.Items)
            {
                if (paramInfo.Format == SECSItemFormat.L)
                {
                    logText += $": [CPNAME={paramInfo.Name},Format={paramInfo.Format},Count={paramInfo.Items.Count}]{Environment.NewLine}";
                    paramResult = new RemoteCommandParameterResult(paramInfo.Name);

                    foreach (EnhancedCommandParameterItem item in paramInfo.Items)
                    {
                        logText += CheckValidationParameterItem(1, item, paramResult);
                    }
                }
                else
                {
                    logText += $": [CPNAME={paramInfo.Name},Format={paramInfo.Format},CPVAL={paramInfo.Value}]{Environment.NewLine}";
                    paramResult = new RemoteCommandParameterResult(paramInfo.Name, (int)CPACK.IllegalFormatSpecifiedForCPVAL);
                }

                result.Items.Add(paramResult);
            }

            if (logText.Length > 0)
            {
                logText = logText.Substring(0, logText.Length - Environment.NewLine.Length);
            }

            WriteLog(LogLevel.Information, $"OnReceivedEnhancedRemoteCommand : {logText}");

            //S2F50 Reply이전 호스트에서 받은 S2F49(EnhancedRemoteCommand)에 대한 Validation Check가 필요합니다.

            _gemDriver.ReplyEnhancedRemoteCommandAck(remoteCommandInfo, result);

            //S2F50 Reply이후 관련된 Logic을 넣으면 됩니다.
        }

        /// <summary>
        /// Host에서 S2F15(New ECV Send)가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="newEcInfo"></param>
        private void GemDriver_OnReceivedNewECVSend(uint systemBytes, VariableCollection newEcInfo)
        {
            WriteLog(LogLevel.Information, "Received New ECV Send");

            _gemDriver.ReplyNewEquipmentConstantSend(systemBytes, newEcInfo, _ack);
        }

        /// <summary>
        /// S5F3(Alarm Enable/Disable Send)가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        private void GemDriver_OnReceivedEnableDisableAlarmSend()
        {
            WriteLog(LogLevel.Information, "Received Enable Disable Alarm Send");
        }

        /// <summary>
        /// Host에서 S10F3(Terminal Message Single)이 수신될 때 발생하는 이벤트입니다
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="tid"></param>
        /// <param name="terminalMessage"></param>
        private void GemDriver_OnReceivedTerminalMessage(uint systemBytes, int tid, string terminalMessage)
        {
            WriteLog(LogLevel.Information, $"Received Terminal Message: TID={tid}, Text={terminalMessage ?? string.Empty}");

            _gemDriver.ReplyTerminalMessageAck(systemBytes, _ack);
        }

        /// <summary>
        /// Host에서 S10F5(Terminal Message Multi)가 수신될 때 발생하는 이벤트입니다
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="tid"></param>
        /// <param name="terminalMessages"></param>
        private void GemDriver_OnReceivedTerminalMultiMessage(uint systemBytes, int tid, List<string> terminalMessages)
        {
            string logText;

            logText = string.Empty;

            foreach (string terminalMessage in terminalMessages)
            {
                logText += terminalMessage + Environment.NewLine;
            }

            if (logText.Length > 0)
            {
                logText = logText.Substring(0, logText.Length - Environment.NewLine.Length);
            }

            WriteLog(LogLevel.Information, $"Received Terminal Multi Message: TID={tid}{Environment.NewLine}{logText}");

            _gemDriver.ReplyTerminalMultiMessageAck(systemBytes, _ack);
        }

        /// <summary>
        /// S7F5(PP Reqeust)가 수신될 경우 발생하는 이벤트입니다
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="ppid"></param>
        private void GemDriver_OnReceivedPPRequest(uint systemBytes, string ppid)
        {
            bool result;
            List<byte> ppbody;
            result = MakePPBody(out ppbody);

            WriteLog(LogLevel.Information, "Received PP Request");

            _gemDriver.ReplyPPRequestAck(systemBytes, ppid, ppbody, result);
        }

        /// <summary>
        /// Host에서 S7F3(PP Send)가 수신될 때 발생하는 이벤트입니다
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="ppid"></param>
        /// <param name="ppbody"></param>
        private void GemDriver_OnReceivedPPSend(uint systemBytes, string ppid, List<byte> ppbody)
        {
            WriteLog(LogLevel.Information, "Received PP Send");

            _gemDriver.ReplyPPSendAck(systemBytes, _ack);
        }

        /// <summary>
        /// Host에서 S7F1(PP Load Inquire)가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="ppid"></param>
        /// <param name="length"></param>
        private void GemDriver_OnReceivedPPLoadInquire(uint systemBytes, string ppid, int length)
        {
            WriteLog(LogLevel.Information, "Received PP Load Inquire");

            _gemDriver.ReplyPPLoadInquireAck(systemBytes, _ack);
        }

        /// <summary>
        /// S7F17(Delete PP Send)가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="ppids"></param>
        private void GemDriver_OnReceivedDeletePPSend(uint systemBytes, List<string> ppids)
        {
            string logText;

            logText = string.Empty;

            if (ppids.Count == 0)
            {
                //Delete All PPIDs
            }
            else
            {
                foreach (string ppid in ppids)
                {
                    //Delete Existing PPID.

                    logText += ppid + ",";
                }

                if (logText.Length > 0)
                {
                    logText = logText.Substring(0, logText.Length - 1);
                }
            }

            WriteLog(LogLevel.Information, $"Received Delette PP Send: ppids={logText}");

            _gemDriver.ReplyPPDeleteAck(systemBytes, _ack);
        }

        /// <summary>
        /// S7F25(Formatted PP Reqeust)가 수신될 경우 발생하는 이벤트입니다
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="ppid"></param>
        private void GemDriver_OnReceivedFmtPPRequest(uint systemBytes, string ppid)
        {
            FmtPPCollection fmtPPCollection;
            bool result = ProcessProgramParsing(ppid, false, out fmtPPCollection);
            WriteLog(LogLevel.Information, "Received FMT PP Request");

            _gemDriver.ReplyFmtPPRequestAck(systemBytes, ppid, fmtPPCollection, result);
        }

        /// <summary>
        /// Host에서 S7F23(Formatted PP Send)가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="fmtPPCollection"></param>
        private void GemDriver_OnReceivedFmtPPSend(uint systemBytes, FmtPPCollection fmtPPCollection)
        {
            string lotText = string.Empty;

            lotText += $"[PPID={fmtPPCollection.PPID}]{Environment.NewLine}";

            foreach (FmtPPCCodeInfo ppcodeInfo in fmtPPCollection.Items)
            {
                lotText += $": [CCODE={ppcodeInfo.CommandCode}]{Environment.NewLine}";

                foreach (FmtPPItem ppitem in ppcodeInfo.Items)
                {
                    lotText += $":    [PPNAME={ppitem.PPName},FORMAT={ppitem.Format}]{Environment.NewLine},PPVALUE={ppitem.PPValue}";
                }
            }

            if (lotText.Length > 0)
            {
                lotText = lotText.Substring(0, lotText.Length - Environment.NewLine.Length);
            }

            WriteLog(LogLevel.Information, $"OnReceivedFmtPPSend : {lotText}");

            _gemDriver.ReplyFmtPPSendAck(systemBytes, _ack);
        }

        private void GemDriver_OnReceivedCurrentEPPDRequest(uint systemBytes)
        {
            List<string> ppids = new List<string>();

            //Add PP List to ppids

            WriteLog(LogLevel.Information, "Received Current EPPD Request");

            _gemDriver.ReplyCurrentEPPDRequestAck(systemBytes, ppids, true);
        }

        /// <summary>
        /// S2F17(Date Time Reqeust)가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="systemBytes"></param>
        private void GemDriver_OnReceivedDateTimeRequest(uint systemBytes)
        {
            DateTime timeData = DateTime.Now;
            WriteLog(LogLevel.Information, "Received Date Time Request");

            _gemDriver.ReplyDateTimeRequest(systemBytes, timeData);
        }

        /// <summary>
        /// S2F31(Date Time Set Request)가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="systemBytes"></param>
        /// <param name="timeData"></param>
        private void GemDriver_OnReceivedDateTimeSetRequest(uint systemBytes, DateTime timeData)
        {
            WriteLog(LogLevel.Information, $"Received Date Time Set Request DateTime={timeData:yyyy-MM-dd HH:mm:ss.fff}");

            _gemDriver.ReplyDateTimeSetRequest(systemBytes, _ack, timeData);
        }

        /// <summary>
        /// Host에서 S2F25(Loopback)이 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="receiveData"></param>
        private void GemDriver_OnReceivedLoopback(List<byte> receiveData)
        {
            string strReceiveData = string.Empty;

            foreach (byte data in receiveData)
            {
                strReceiveData += data + " ";
            }

            if(strReceiveData.Length > 0)
            {
                strReceiveData = strReceiveData.Substring(0, strReceiveData.Length - 1);
            }

            WriteLog(LogLevel.Information, $"Received Loopback Data={strReceiveData}");
        }

        /// <summary>
        /// Host에서 S1F13(Establish Communication)이 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="mdln"></param>
        /// <param name="sofRev"></param>
        /// <returns></returns>
        private int GemDriver_OnReceivedEstablishCommunicationsRequest(string mdln, string sofRev)
        {
            WriteLog(LogLevel.Information, "Received Establish Communication Request");

            return _ack;
        }

        /// <summary>
        /// 사용자 정의 Message로 등록한 Stream, Function 중 Primary 메시지가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="message"></param>
        private void GemDriver_OnUserPrimaryMessageReceived(SECSMessage message)
        {
            WriteLog(LogLevel.Information, "User PrimaryMessage Received");
        }

        /// <summary>
        /// 사용자 정의 Message로 등록한 Stream, Function 중 Secondary 메시지가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="primaryMessage"></param>
        /// <param name="secondaryMessage"></param>
        private void GemDriver_OnUserSecondaryMessageReceived(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            WriteLog(LogLevel.Information, "User SecondaryMessage Received");
        }

        private void GemDriver_OnReceivedUnknownMessage(SECSMessage message)
        {
            WriteLog(LogLevel.Information, "Received Unknown Message");
        }

        private void GemDriver_OnInvalidMessageReceived(MessageValidationError error, SECSMessage message)
        {
            WriteLog(LogLevel.Information, "Received Invalid Message");
        }

        /// <summary>
        /// UbiGEM Configuration 파일에 정의되지 않은 Remote Command 가 수신될 경우 발생합니다.
        /// </summary>
        /// <param name="remoteCommandInfo"></param>
        private void GemDriver_OnReceivedInvalidRemoteCommand(RemoteCommandInfo remoteCommandInfo)
        {
            WriteLog(LogLevel.Information, "Received Invalid Remote Command");
        }

        /// <summary>
        /// UbiGEM Configuration 파일에 정의되지 않은 Enhanced Remote Command 가 수신될 경우 발생합니다.
        /// </summary>
        /// <param name="remoteCommandInfo"></param>
        private void GemDriver_OnReceivedInvalidEnhancedRemoteCommand(EnhancedRemoteCommandInfo remoteCommandInfo)
        {
            WriteLog(LogLevel.Information, "Received Invalid Enhanced Remote Command");
        }
        #endregion

        #region [Response Message Event]
        private void GemDriver_OnResponseTerminalRequest(int ack)
        {
            WriteLog(LogLevel.Information, "Response Terminal Request");
        }

        /// <summary>
        /// S7F5(PP Request)를 발송 후 Host에서 S7F6이 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="ppid"></param>
        /// <param name="ppbody"></param>
        private void GemDriver_OnResponsePPRequest(string ppid, List<byte> ppbody)
        {
            WriteLog(LogLevel.Information, "Response PP Request");
        }

        /// <summary>
        /// S7F3(PP Send)를 발송 후 Host에서 S7F4가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="ack"></param>
        /// <param name="ppid"></param>
        private void GemDriver_OnResponsePPSend(int ack, string ppid)
        {
            WriteLog(LogLevel.Information, "Response PP Send");
        }

        /// <summary>
        /// S7F1(PP Load Inquire)를 발송 후 Host에서 S7F2가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="ppgnt"></param>
        /// <param name="ppid"></param>
        private void GemDriver_OnResponsePPLoadInquire(int ppgnt, string ppid)
        {
            WriteLog(LogLevel.Information, "Response PP Load Inquire");
        }

        /// <summary>
        /// S2F23(Formatted PP Request) 발송 후 Host에서 S2F24가 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="fmtPPCollection"></param>
        private void GemDriver_OnResponseFmtPPRequest(FmtPPCollection fmtPPCollection)
        {
            WriteLog(LogLevel.Information, "Response FMT PP Request");
        }

        /// <summary>
        /// S7F25(Formatted PP Send)를 발송 후 Host에서 S7F26이 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="ack"></param>
        /// <param name="fmtPPCollection"></param>
        private void GemDriver_OnResponseFmtPPSend(int ack, FmtPPCollection fmtPPCollection)
        {
            WriteLog(LogLevel.Information, "Response FMT PP Send");
        }

        /// <summary>
        /// S2F27(Formatted PP Verification Send)를 발송 후 Host에서 S2F28이 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="fmtPPVerificationCollection"></param>
        private void GemDriver_OnResponseFmtPPVerification(FmtPPVerificationCollection fmtPPVerificationCollection)
        {
            WriteLog(LogLevel.Information, "Response FMT Verification Ack");
        }

        /// <summary>
        /// S7F17(Date Time Reqeust)를 발송 후 Host에서 S7F18이 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="timeData"></param>
        /// <returns></returns>
        private bool GemDriver_OnResponseDateTimeRequest(DateTime timeData)
        {
            bool result = true;

            WriteLog(LogLevel.Information, $"Response Date Time Request DateTime={timeData:yyyy-MM-dd HH:mm:ss.fff}");

            return result;
        }

        /// <summary>
        /// S2F25(Loopback)을 발송 후 Host에서 S2F26이 수신될 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="receiveData"></param>
        /// <param name="sendData"></param>
        private void GemDriver_OnResponseLoopback(List<byte> receiveData, List<byte> sendData)
        {
            bool result = false;

            if (receiveData.Count == sendData.Count)
            {
                int count = receiveData.Count;

                for (int i = 0; i < count; i++)
                {
                    if (receiveData[i] != sendData[i])
                    {
                        result = false;
                        break;
                    }
                }
            }

            WriteLog(LogLevel.Information, $"Response Loopback : Receive Data={string.Join(",", receiveData)} : Send Data={string.Join(",", sendData)} : Result={result}");
        }

        /// <summary>
        /// S6F11(Event Report)의 Secondary Message(S6F12)가 수신될 경우 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="ceid"></param>
        /// <param name="ack"></param>
        private void GemDriver_OnResponseEventReportAcknowledge(string ceid, int ack)
        {
            if (ceid == "10001" && ack == (int)ACKC6.Accepted)
            {
                // Collection Event의 Ack값에 따라 정의할 시나리오 작성
            }
        }
        #endregion

        #region [Request Message Event]
        /// <summary>
        /// Variable 정보의 Update가 필요할 때 발생하는 이벤트입니다.
        /// </summary>
        /// <param name="updateType"></param>
        /// <param name="variables"></param>
        private void GemDriver_OnVariableUpdateRequest(VariableUpdateType updateType, List<VariableInfo> variables)
        {
            /* <OnVariableUpdateRequest Event의 경우>
             * 1. 호스트가 S1F3 Message를 Send 하였을 때
             * 2. 호스트가 S6F19 Message를 Send 하였을 때
             * 3. ReportCollectionEvent(string) API를 사용할 경우.
             */

            // List Type Variable의 데이터 설정 방법
            // VID=2000 이고 구조가 아래와 같고, n = 5, m = 4 인 경우
            // Ln DataList
            //    L3 DataInfo
            //       A DataID
            //       U1 SubDataCount
            //       Lm SubDataList
            //          L2 SubDataInfo
            //              A SubDataID
            //              U1 SubDataNo

            /*
            VariableInfo dataList = new VariableInfo() { VID = "2000", Format = SECSItemFormat.L, Name = "DataList" };

            for (int i = 0; i < 5; i++)
            {
                int subCount = 4;

                VariableInfo dataInfo = new VariableInfo() { VID = "", Format = SECSItemFormat.L, Name = "DataInfo" };

                dataList.ChildVariables.Add(dataInfo);

        		dataInfo.ChildVariables.Add(new VariableInfo() { VID = "", Format = SECSItemFormat.A, Name = "DataID", Value = "DataID" });
                dataInfo.ChildVariables.Add(new VariableInfo() { VID = "", Format = SECSItemFormat.U1, Name = "SubDataCount", Value = subCount });
                
                VariableInfo subDataList = new VariableInfo() { VID = "", Format = SECSItemFormat.L, Name = "SubDataList" };
		        dataInfo.ChildVariables.Add(subDataList);

		        for(int j = 0; j < subCount; j++)
		        {
			        VariableInfo subDataInfo = new VariableInfo() { VID = "", Format = SECSItemFormat.L, Name = "SubDataInfo" };
			        subDataList.ChildVariables.Add(subDataInfo);

                    subDataInfo.ChildVariables.Add(new VariableInfo() { VID = "", Format = SECSItemFormat.A, Name = "SubDataID", Value = "SubDataID" });

                    subDataInfo.ChildVariables.Add(new VariableInfo() { VID = "", Format = SECSItemFormat.U1, Name = "SubDataNo", Value = j });
		        }
	        }

            _gemDriver.SetVariable(dataList);
            */

            foreach (VariableInfo variableInfo in variables)
            {
                if(variableInfo.VID == DefinedV.Alarmset)
                {
                    // Format 'L'의 ChildVariable 'n'개 값 설정 방법
                    VariableInfo alarmSet = _gemDriver.Variables[DefinedV.Alarmset];
                    VariableInfo alarmID;

                    // 상위 Variable의 ChildVariables을 Clear
                    alarmSet.ChildVariables.Clear();

                    // 하위 Variable의 개수 만큼 상위 Variable ChildVariables에 추가
                    foreach (long alid in _setAlarmList)
                    {
                        // 하위 Variable 생성 방법
                        // 1. ugc file에서 정의한 Variable의 정보를 Copy해서 생성
                        alarmID = _gemDriver.Variables[DefinedV.ALID].CopyTo();
                        alarmID.Value = alid;

                        // 2. 직접 Variable 객체를 생성
                        alarmID = new VariableInfo()
                        {
                            VID = DefinedV.ALID,
                            Name = "ALID",
                            Format = SECSItemFormat.A,
                            Length = 1,
                            Value = alid
                        };

                        alarmSet.ChildVariables.Add(alarmID);
                    }
                }
            }

            WriteLog(LogLevel.Information, "Variable Update Request");
        }

        /// <summary>
        /// 사용자 정의 GEM Message의 업데이트가 필요할 경우 발생합니다.
        /// </summary>
        /// <param name="message"></param>
        private void GemDriver_OnUserGEMMessageUpdateRequest(SECSMessage message)
        {
            WriteLog(LogLevel.Information, "GEM Message Update Request");
        }

        /// <summary>
        /// Trace Data를 발송하기 위해 Variable의 Update가 필요한 경우 발생합니다.
        /// </summary>
        /// <param name="variables"></param>
        private void GemDriver_OnTraceDataUpdateRequest(List<VariableInfo> variables)
        {
            WriteLog(LogLevel.Information, "Trace Data Update Request");
        }
        #endregion

        #region [Log Event]
        private void GemDriver_OnWriteLog(LogLevel logLevel, string logText)
        {
            // Driver Log를 남길 때
            logText = logText.Substring(30);
            logText = logText.Substring(0, logText.Length - 2);
            WriteLog(logLevel, logText);
        }

        private void GemDriver_OnSECS1Log(LogLevel logLevel, string logText)
        {
            // SECS 1 Log를 남길 때
        }

        private void GemDriver_OnSECS2Log(LogLevel logLevel, string logText)
        {
            // SECS 2 Log를 남길 때
            logText = logText.Substring(30);
            logText = logText.Substring(0, logText.Length - 2);
            WriteLog(logLevel, logText);
        }
        #endregion
        #endregion



        #region [Control State]
        private void btnOffline_Click(object sender, RoutedEventArgs e)
        {
            GemDriverError driverResult = _gemDriver.RequestOffline();
            WriteLog(LogLevel.Error, $"Request Offline Result={driverResult}");
        }

        private void btnLocalOnline_Click(object sender, RoutedEventArgs e)
        {
            GemDriverError driverResult = _gemDriver.RequestOnlineLocal();
            WriteLog(LogLevel.Error, $"Request Online Local Result={driverResult}");
        }

        private void btnRemoteOnline_Click(object sender, RoutedEventArgs e)
        {
            GemDriverError driverResult = _gemDriver.RequestOnlineRemote();

            WriteLog(LogLevel.Error, $"Request Online Remote Result={driverResult}");
        }
        #endregion

        #region [EC]
        private void cbbECID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox;
            VariableInfo variable;

            comboBox = sender as ComboBox;

            if (sender != null)
            {
                variable = comboBox.SelectedItem as VariableInfo;
                if (variable != null)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        txtECIDValue.Text = _gemDriver.Variables[variable.VID].Value;
                    }));
                }
            }
        }

        private void btnSetECIDValue_Click(object sender, RoutedEventArgs e)
        {
            // SetEquipmentConstant(string, object) API의 사용은 ECV의 Value 변경되었음을 보고할 경우입니다.
            VariableInfo varInfo;

            if (cbbECID.SelectedItem != null)
            {
                varInfo = cbbECID.SelectedItem as VariableInfo;

                if (varInfo != null)
                {
                    GemDriverError driverResult = _gemDriver.SetEquipmentConstant(varInfo.VID, txtECIDValue.Text);

                    WriteLog(LogLevel.Information, $"Set ECID : {varInfo.VID}, Value : {varInfo.Value}, Result : {driverResult}");
                }
            }
        }

        private void btnSetECIDListValue_Click(object sender, RoutedEventArgs e)
        {
            // SetEquipmentConstant(List<string>, List<object>) 사용은 ECV의 Value 변경되었음을 List로 보고 할 경우입니다.

            List<string> ecids = new List<string>();
            List<object> values = new List<object>();

            // 변경된 ECV의 ID와 값을 쌍으로 구성하여 보고

            GemDriverError driverResult = _gemDriver.SetEquipmentConstant(ecids, values);

            WriteLog(LogLevel.Error, $"Set ECID Value List Result={driverResult}");
        }
        #endregion

        #region [SV/DVval]
        private void cbbVID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox;
            VariableInfo variable;

            comboBox = sender as ComboBox;

            if (comboBox != null)
            {
                variable = comboBox.SelectedItem as VariableInfo;

                if (variable != null)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        txtVIDValue.Text = _gemDriver.Variables[variable.VID].Value;
                    }));
                }
            }
        }

        private void btnSetVIDValue_Click(object sender, RoutedEventArgs e)
        {
            // SetVariable(string, object) API사용은 DVVAL/SV의 Value 변경되었음을 보고 할 경우입니다.
            VariableInfo varInfo;

            varInfo = cbbVID.SelectedItem as VariableInfo;

            if (varInfo != null)
            {
                GemDriverError driverResult = _gemDriver.SetVariable(varInfo.VID, txtVIDValue.Text);

                WriteLog(LogLevel.Information, $"Set VID : {varInfo.VID}, Value : {varInfo.Value}, Result : {driverResult}");
            }
        }

        private void btnSetVariableList_Click(object sender, RoutedEventArgs e)
        {
            // SetVariable(List<string>, List<object>) API사용은 DVVAL/SV의 Value 변경되었음을 List로 보고 할 경우입니다.

            List<string> vids = new List<string>();
            List<object> values = new List<object>();

            // 변경된 SV/DVVAL의 ID와 값을 쌍으로 구성하여 보고

            GemDriverError driverResult = _gemDriver.SetVariable(vids, values);

            WriteLog(LogLevel.Error, $"Set Variable Value List Result : {driverResult}");
        }
        #endregion

        #region [CE Report]
        private void btnCESendDefined_Click(object sender, RoutedEventArgs e)
        {
            // ReportCollectionEvent(string) API의 사용은 미리 정의된 Collection Event를 보고할 경우 입니다.
            // OnVariableUpdateRequest Event 발생 합니다.
            // OnVariableUpdateRequest Event 내에서 Variable의 값을 설정 하는것도 가능합니다.

            CollectionEventInfo ceInfo;
            KeyValuePair<string, CollectionEventInfo> selectedItem;

            if (cbbCE.SelectedItem != null)
            {
                selectedItem = (KeyValuePair<string, CollectionEventInfo>)cbbCE.SelectedItem;
                ceInfo = selectedItem.Value;
                // EquipmentConstantChanged 관련 Collection Event 는 직접적으로 호출하면 안됩니다.
                if (ceInfo != null && ceInfo.Name != "EquipmentConstantChanged" && ceInfo.Name != "EquipmentConstantChangedbyhost")
                {
                    // OnVariableUpdateRequest Event에서 값을 설정 하지 않고, ReportCollectionEvent(string) 호출 이전에 설정해도 됩니다.
                    _gemDriver.Variables[DefinedV.ControlState].Value = 5;

                    GemDriverError driverResult = _gemDriver.ReportCollectionEvent(ceInfo.CEID);

                    WriteLog(LogLevel.Error, $"Report Collection Event Result : {driverResult}");
                }
            }
        }

        private void btnCESend_Click(object sender, RoutedEventArgs e)
        {
            // ReportCollectionEvent(CollectionEventInfo) API는 Report 하려는 Collection Event의 구조가 복잡할 경우 사용하기 좋습니다.
            // Collection Event를 Code로 구성하여 Report 합니다.
            // ※ 호스트에서 DefineReport를 사용하는 업체는 ReportCollectionEvent(CollectionEventInfo) API를 사용하실 경우, Code 수정이 불가피 합니다.

            // Variable Value 값 설정은 두가지 방법을 제시합니다.
            // 1. new로 새로운 VariableInfo 생성
            //  ▶ new VariableInfo() { VID = "", Name = "", Format = SECSItemFormat.A, Value = "Value" }
            // ※ Name은 Log를 찍을 때 표시됩니다.

            // 2. GEM Driver 내 VariableCollection에서 해당 Variable CopyTo()
            //  ▶ GemDriver.Variables[VID].Value = "Value";
            //  ▶ ReportInfo.Variables.Add(GemDriver.Variables[VID].CopyTo());

            if (cbbCE.SelectedItem != null)
            {
                KeyValuePair<string, CollectionEventInfo> selectedItem = (KeyValuePair<string, CollectionEventInfo>)cbbCE.SelectedItem;

                // CollectionEvent를 완전히 새로 구성하기 때문에 새로운 객체를 생성합니다.
                CollectionEventInfo ceInfo = new CollectionEventInfo() { CEID = selectedItem.Value.CEID, IsUse = true, Enabled = true };

                // EquipmentConstantChanged 관련 Collection Event는 직접적으로 호출하면 안됩니다.
                if (ceInfo != null && ceInfo.Name != "EquipmentConstantChanged" && ceInfo.Name != "EquipmentConstantChangedbyhost")
                {
                    // Collection Event 구조 생성
                    ReportInfo rptInfo;

                    rptInfo = new ReportInfo() { ReportID = "1" };

                    rptInfo.Variables.Add(new VariableInfo() { Name = "DeviceID", Format = SECSItemFormat.A, Value = "0" });
                    rptInfo.Variables.Add(new VariableInfo() { Name = "ControlState", Format = SECSItemFormat.U1, Value = 5 });

                    ceInfo.Reports.Add(rptInfo);

                    GemDriverError driverResult = _gemDriver.ReportCollectionEvent(ceInfo);
                    WriteLog(LogLevel.Error, $"Report Collection Event Result : {driverResult}");
                }
            }
        }
        #endregion

        #region [Process State]
        private void btnProcessingStateChange_Click(object sender, RoutedEventArgs e)
        {
            // ReportEquipmentProcessingState(byte) API사용은 Equipment Processing States(장비 프로세싱 상태)에 변경이 있어 호스트로 보고 할 경우입니다.
            byte processState;
            if (byte.TryParse(txtEQPProcessingState.Text, out processState))
            {
                GemDriverError driverResult = _gemDriver.ReportEquipmentProcessingState(processState);
                WriteLog(LogLevel.Error, $"Report Equipment Processing State Result : {driverResult}");
            }
        }
        #endregion

        #region [Alarm]
        private void btnSetAlarm_Click(object sender, RoutedEventArgs e)
        {
            long alarmID;
            if (long.TryParse(txtAlarm.Text, out alarmID) == true)
            {
                GemDriverError driverResult = _gemDriver.ReportAlarmSet(alarmID);

                WriteLog(LogLevel.Error, $"Set Alarm Result : {driverResult}");

                if (driverResult == GemDriverError.Ok)
                {
                    UpdateSetAlarmList(alarmID, true);
                }
            }
        }

        private void btnClearAlarm_Click(object sender, RoutedEventArgs e)
        {
            long alarmID;
            if (long.TryParse(txtAlarm.Text, out alarmID) == true)
            {
                GemDriverError driverResult = _gemDriver.ReportAlarmClear(alarmID);
                WriteLog(LogLevel.Error, $"Clear Alarm Result : {driverResult}");

                if (driverResult == GemDriverError.Ok)
                {
                    UpdateSetAlarmList(alarmID, false);
                }
            }
        }

        private void UpdateSetAlarmList(long alarmID, bool isSet)
        {
            // GEM Driver는 Set Alarm List를 관리하지 않습니다.

            if (isSet == true)
            {
                _setAlarmList.Add(alarmID);
            }
            else
            {
                _setAlarmList.Remove(alarmID);
            }
        }
        #endregion

        #region [Terminal Message]
        private void btnReportTerminalMessage_Click(object sender, RoutedEventArgs e)
        {
            int tid;
            if (int.TryParse(txtTerminalTID.Text, out tid) == true)
            {
                GemDriverError driverResult = _gemDriver.ReportTerminalMessage(tid, txtTerminalMessage.Text);

                WriteLog(LogLevel.Error, $"Report Terminal Message Result : {driverResult}");
            }
        }
        #endregion

        #region [Process Program]
        private void btnRequestPPRequest_Click(object sender, RoutedEventArgs e)
        {
            string ppid;

            ppid = string.IsNullOrEmpty(txtPPID.Text) == true ? "MGL19SS06MD" : txtPPID.Text;
            GemDriverError driverResult = _gemDriver.RequestPPRequest(ppid);
            WriteLog(LogLevel.Error, $"Request PP Request Result : {driverResult}");
        }

        private void btnRequestPPSend_Click(object sender, RoutedEventArgs e)
        {
            string ppid;
            List<byte> ppbody;

            ppid = string.IsNullOrEmpty(txtPPID.Text) == true ? "MGL19SS06MD" : txtPPID.Text;

            MakePPBody(out ppbody);

            GemDriverError driverResult = _gemDriver.RequestPPSend(ppid, ppbody);

            WriteLog(LogLevel.Error, $"Request PP Send Result : {driverResult}");
        }

        private void btnRequestPPLoadInquire_Click(object sender, RoutedEventArgs e)
        {
            string ppid;
            List<byte> ppbody;

            ppid = string.IsNullOrEmpty(txtPPID.Text) == true ? "MGL19SS06MD" : txtPPID.Text;

            MakePPBody(out ppbody);

            GemDriverError driverResult = _gemDriver.RequestPPLoadInquire(ppid, ppbody.Count);
            WriteLog(LogLevel.Error, $"Request PP Load Inquire Result : {driverResult}");
        }

        private void btnRequestPPChanged_Click(object sender, RoutedEventArgs e)
        {
            string ppid = string.Empty;
            int ppstate = (int)ProcessProgramChangeState.Credited;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                ppid = string.IsNullOrEmpty(txtPPID.Text) == true ? "MGL19SS06MD" : txtPPID.Text;
            }));

            GemDriverError driverResult = _gemDriver.RequestPPChanged(ppstate, ppid);
            WriteLog(LogLevel.Error, $"Request PP Changed Result : {driverResult}");
        }

        private bool MakePPBody(out List<byte> ppbody)
        {
            bool result = true;
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            byte[] arrPPBody;
            int count;

            ppbody = new List<byte>();
            count = rand.Next(0, 1000);

            arrPPBody = new byte[count];

            rand.NextBytes(arrPPBody);

            foreach (byte data in arrPPBody)
            {
                ppbody.Add(data);
            }

            return result;
        }
        #endregion

        #region [Formatted Process Program]
        private void btnRequestFmtPPRequest_Click(object sender, RoutedEventArgs e)
        {
            string ppid = string.IsNullOrEmpty(txtFMTPPID.Text) == true ? "MGL19SS06MD" : txtFMTPPID.Text;
            GemDriverError driverResult = _gemDriver.RequestFmtPPRequest(ppid);
            WriteLog(LogLevel.Error, $"Request Fmt PP Request Result : {driverResult}");
        }

        private void btnRequestFmtPPChanged_Click(object sender, RoutedEventArgs e)
        {
            string ppid = string.Empty;
            int fmtPPstate = (int)ProcessProgramChangeState.Credited;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                ppid = string.IsNullOrEmpty(txtFMTPPID.Text) == true ? "MGL19SS06MD" : txtFMTPPID.Text;
            }));

            GemDriverError driverResult = _gemDriver.RequestFmtPPChanged(fmtPPstate, ppid);
            WriteLog(LogLevel.Error, $"Request Fmt PP Changed Result : {driverResult}");
        }

        private void btnRequestFmtPPSendWithoutValue_Click(object sender, RoutedEventArgs e)
        {
            FmtPPCollection fmtPPCollection;
            string ppid = string.IsNullOrEmpty(txtFMTPPID.Text) == true ? "MGL19SS06MD" : txtFMTPPID.Text;
            ProcessProgramParsing(ppid, true, out fmtPPCollection);

            GemDriverError driverResult = _gemDriver.RequestFmtPPSendWithoutValue(fmtPPCollection);
            WriteLog(LogLevel.Error, $"Request Fmt PP Send Without Value Result : {driverResult}");
        }

        private void btnRequestFmtPPSend_Click(object sender, RoutedEventArgs e)
        {
            FmtPPCollection fmtPPCollection;
            string ppid = string.IsNullOrEmpty(txtFMTPPID.Text) == true ? "MGL19SS06MD" : txtFMTPPID.Text;
            ProcessProgramParsing(ppid, false, out fmtPPCollection);

            GemDriverError driverResult = _gemDriver.RequestFmtPPSend(fmtPPCollection);
            WriteLog(LogLevel.Error, $"Request Fmt PP Send Result : {driverResult}");
        }

        private void btnRequestFmtPPVerificationSend_Click(object sender, RoutedEventArgs e)
        {
            string ppid = string.IsNullOrEmpty(txtFMTPPID.Text) == true ? "MGL19SS06MD" : txtFMTPPID.Text;
            FmtPPVerificationCollection fmtPPCollection = new FmtPPVerificationCollection(ppid);
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < 10; i++)
            {
                FmtPPVerificationInfo info = new FmtPPVerificationInfo
                {
                    ACK = _ack,
                    SeqNum = rand.Next(0, 1000),
                    ErrW7 = $"ERR{rand.Next(0, 1000)}",
                };

                fmtPPCollection.Items.Add(info);
            }

            GemDriverError driverResult = _gemDriver.RequestFmtPPVerificationSend(fmtPPCollection);
            WriteLog(LogLevel.Error, $"Request Fmt PP Verification Send Result : {driverResult}");
        }

        private bool ProcessProgramParsing(string ppid, bool withoutValue, out FmtPPCollection fmtPPCollection)
        {
            bool result = true;
            XElement root;
            XElement element;
            XElement subElement;
            fmtPPCollection = new FmtPPCollection(ppid);

            if (ppid == "MGL19SS06MD")
            {
                FmtPPCCodeInfo info;

                try
                {
                    root = XElement.Load(new System.IO.StringReader(Properties.Resources.MGL19SS06MD));

                    element = root.Element("CCodeInfoInfos");

                    if (element != null)
                    {
                        foreach (XElement tempCCodeInfo in element.Elements("CCodeInfo"))
                        {
                            info = new FmtPPCCodeInfo
                            {
                                CommandCode = tempCCodeInfo.Attribute("CommandCode") != null ? tempCCodeInfo.Attribute("CommandCode").Value : string.Empty
                            };

                            subElement = tempCCodeInfo.Element("PPItems");

                            if (subElement != null)
                            {
                                foreach (XElement tempPPARM in subElement.Elements("PPItem"))
                                {
                                    if (withoutValue == true)
                                    {
                                        string value;
                                        SECSItemFormat format;

                                        value = tempPPARM.Attribute("PPValue") != null ? tempPPARM.Attribute("PPValue").Value : string.Empty;
                                        format = tempPPARM.Attribute("Format").Value != null ? ((SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempPPARM.Attribute("Format").Value)) : SECSItemFormat.A;

                                        info.Add(value, format);
                                    }
                                    else
                                    {
                                        string name;
                                        string value;
                                        SECSItemFormat format;

                                        name = tempPPARM.Attribute("PPName") != null ? tempPPARM.Attribute("PPName").Value : string.Empty;
                                        value = tempPPARM.Attribute("PPValue") != null ? tempPPARM.Attribute("PPValue").Value : string.Empty;
                                        format = tempPPARM.Attribute("Format").Value != null ? ((SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), tempPPARM.Attribute("Format").Value)) : SECSItemFormat.A;

                                        info.Add(name, value, format);
                                    }
                                }
                            }

                            fmtPPCollection.Items.Add(info);
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion

        #region [ETC]
        private void btnRequestDateTime_Click(object sender, RoutedEventArgs e)
        {
            GemDriverError driverResult = _gemDriver.RequestDateTime();
            WriteLog(LogLevel.Error, $"Request Date Time Result : {driverResult}");
        }

        private void btnRequestLoopback_Click(object sender, RoutedEventArgs e)
        {
            List<byte> byteList = new List<byte>();

            if (string.IsNullOrEmpty(txtLoopbackValue.Text.Trim()) == false)
            {
                string[] temp = txtLoopbackValue.Text.Split(' ');

                foreach (string byteValue in temp)
                {
                    byteList.Add(byte.Parse(byteValue));
                }
            }

            GemDriverError driverResult = _gemDriver.RequestLoopback(byteList);
            WriteLog(LogLevel.Error, $"Request Loopback Result : {driverResult}");
        }
        #endregion

        #region [User Message]
        private void btnUserMessageSend_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, SECSMessage> selectedItem;
            SECSMessage message;

            if (cbbUserMessage.SelectedItem != null)
            {
                selectedItem = (KeyValuePair<string, SECSMessage>)cbbUserMessage.SelectedItem;

                message = selectedItem.Value;

                if (message != null)
                {
                    message.Body.Clear();

                    // 아래 형식의 User Defined Message를 송신하고자 할 경우
                    // <L, 2
                    //     <B, 1 '2'>
                    //     <A, 2 'OK'>
                    // >

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add(SECSItemFormat.B, 1, 2);
                    message.Body.Add(SECSItemFormat.A, 2, "OK");

                    GemDriverError error = _gemDriver.SendSECSMessage(message);

                    string wait = message.WaitBit == true ? "W" : string.Empty;
                    WriteLog(LogLevel.Information, $"UserMessage S{message.Stream}F{message.Function}{wait} Send. Result={error}");
                }
            }
        }
        #endregion

        #region [ACK]
        private void btnAckApply_Click(object sender, RoutedEventArgs e)
        {
            int convertValue = 0;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                int.TryParse(txtAck.Text, out convertValue);
            }));

            _ack = convertValue;
        }
        #endregion



        #region [Other]
        private void btnLogClear_Click(object sender, RoutedEventArgs e)
        {
            txtLogs.Document.Blocks.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(((Key.D0 <= e.Key) && (e.Key <= Key.D9))
                || ((Key.NumPad0 <= e.Key) && (e.Key <= Key.NumPad9))
                || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            _gemDriver.Stop();
            _gemDriver.Dispose();
        }

        private void WriteLog(LogLevel logLevel, string logText)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                int lineCount;

                if (txtLogs != null)
                {
                    lineCount = GetLineCount();

                    if (LOG_LINE_MAX_COUNT <= lineCount)
                    {
                        txtLogs.Document.Blocks.Remove(txtLogs.Document.Blocks.FirstBlock);
                    }

                    string strLogText = string.Format(DATETIME_TEXT_FORMAT, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), logLevel.ToString(), logText);

                    if( 1048576 < strLogText.Count() )
                    {
                        string result = strLogText.Substring(0, 500) + "\n...\n" + strLogText.Substring(strLogText.Length - 500) + "\n";

                        TextRange tr = new TextRange(txtLogs.Document.ContentEnd, txtLogs.Document.ContentEnd)
                        {
                            Text = result
                        };
                    }
                    else
                    {
                        TextRange tr = new TextRange(txtLogs.Document.ContentEnd, txtLogs.Document.ContentEnd)
                        {
                            Text = strLogText
                        };
                    }

                    try
                    {
                        txtLogs.ScrollToEnd();
                    }
                    catch { }
                }
            }));
        }

        private int GetLineCount()
        {
            int lineCount;

            if (string.IsNullOrWhiteSpace(GetAsText()))
            {
                return 0;
            }

            lineCount = Regex.Matches(GetAsRTF(), Regex.Escape(@"\par")).Count - 1;

            return lineCount;
        }

        private string GetAsText()
        {
            return new TextRange(txtLogs.Document.ContentStart, txtLogs.Document.ContentEnd).Text;
        }

        private string GetAsRTF()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    TextRange textRange = new TextRange(txtLogs.Document.ContentStart, txtLogs.Document.ContentEnd);
                    textRange.Save(memoryStream, DataFormats.Rtf);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }));

                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private void UpdateTitle()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                if (string.IsNullOrEmpty(_ugcFileName) == true)
                {
                    Title = PROGRAM_DEFAULT_TITLE;
                }
                else
                {
                    Title = string.Format(PROGRAM_TITLE_FORMAT, PROGRAM_DEFAULT_TITLE, _ugcFileName);
                }
            }));
        }

        private void UpdateTitle(string connectionState, string ipAddress, int portNo)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                Title = string.Format(PROGRAM_TITLE_FORMAT, PROGRAM_DEFAULT_TITLE, _ugcFileName) + " - " + string.Format(PROGRAM_STATUS_FORMAT, connectionState, ipAddress, portNo);
            }));
        }

        private string CheckValidationParameterItem(int level, EnhancedCommandParameterItem enhancedCommandParameterItem, RemoteCommandParameterResult paramResult)
        {
            string logText = ":";
            RemoteCommandParameterResult itemResult;

            for (int i = 0; i < level; i++)
            {
                logText += " ";
            }

            level++;

            if (enhancedCommandParameterItem.Format == SECSItemFormat.L)
            {
                logText += $"[CPNAME={enhancedCommandParameterItem.Name},Format={enhancedCommandParameterItem.Format},CEPVAL={enhancedCommandParameterItem.Value}]{Environment.NewLine}";

                if (string.IsNullOrEmpty(enhancedCommandParameterItem.Name) == true)
                {
                    itemResult = new RemoteCommandParameterResult((int)CPACK.IllegalFormatSpecifiedForCPVAL);
                }
                else
                {
                    itemResult = new RemoteCommandParameterResult(enhancedCommandParameterItem.Name, (int)CPACK.IllegalFormatSpecifiedForCPVAL);
                }

                foreach (EnhancedCommandParameterItem item in enhancedCommandParameterItem.ChildParameterItem.Items)
                {
                    logText += CheckValidationParameterItem(level, item, itemResult);
                }
            }
            else
            {
                logText += $"[CPNAME={enhancedCommandParameterItem.Name},Format={enhancedCommandParameterItem.Format},CEPVAL={enhancedCommandParameterItem.Value}]{Environment.NewLine}";
                itemResult = new RemoteCommandParameterResult(enhancedCommandParameterItem.Name, (int)CPACK.IllegalFormatSpecifiedForCPVAL);
            }

            paramResult.ParameterListAck.Add(itemResult);

            return logText;
        }
        #endregion

        #region [Menu]
        private void btnOepnUGC_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = UGC_FILE_FILTER
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _ugcFileName = openFileDialog.FileName;

                UpdateTitle();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            GemDriverError driverResult = _gemDriver.Initialize(_ugcFileName, out errorText);

            WriteLog(LogLevel.Error, $"Initialize Result : {driverResult}");

            if (driverResult == GemDriverError.Ok)
            {
                cbbECID.ItemsSource = _gemDriver.Variables.ECV.Items.Where(t => t.Format != SECSItemFormat.L).ToList();
                cbbVID.ItemsSource = _gemDriver.Variables.Variables.Items.Where(t => t.Format != SECSItemFormat.L).ToList();
                cbbCE.ItemsSource = _gemDriver.CollectionEvents.Items;
                cbbUserMessage.ItemsSource = _gemDriver.UserMessage.MessageInfo;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Initialize EQP Data to Driver before Connecting to Communication

            GemDriverError driverResult = _gemDriver.Start();
            WriteLog(LogLevel.Error, $"Driver Start Result : {driverResult}");

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _gemDriver.Stop();
        }
        #endregion

        private void S1F13Recv_Click(object sender, RoutedEventArgs e)
        {
            _gemDriver.SelfReciveS1F13();
        }
    }
}
