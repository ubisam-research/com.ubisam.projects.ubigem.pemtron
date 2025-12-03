using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UbiGEM.Net.Structure;
using UbiCom.Net.Structure;
using System.Xml.Linq;
using System.Collections;
using UbiGEM.Net.Utility.Logger;
using UbiGEM.Net.Structure.WaferMap;
using Microsoft.Win32;

namespace UbiGEM.Net.Driver
{
    /// <summary>
    /// GEM Driver입니다.
    /// </summary>
    public partial class GemDriver : IDisposable
    {
        // Received -> Host로부터 Data 수신 시
        // Response -> Host로 처리결과 수신 시
        // Request  -> Host로 Data 요청 시
        #region [Delegate / Event]
        /// <summary>
        /// Connected 상태 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ipAddress">연관된 IP Address입니다.</param>
        /// <param name="portNo">연관된 Port no입니다.</param>
        public delegate void ConnectionStateChangedEventHandler(string ipAddress, int portNo);
        /// <summary>
        /// Invalid message 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="error">Invalid 사유입니다.</param>
        /// <param name="message">수신한 message입니다.</param>
        public delegate void InvalidMessageReceivedEventHandler(MessageValidationError error, SECSMessage message);
        /// <summary>
        /// Unknown message 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="message">수신한 message입니다.</param>
        public delegate void ReceivedUnknownMessageEventHandler(SECSMessage message);
        /// <summary>
        /// 사용자 정의 Primary message 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="message">수신한 message입니다.</param>
        public delegate void UserPrimaryMessageReceivedEventHandler(SECSMessage message);
        /// <summary>
        /// 사용자 정의 Secondary message 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="primaryMessage">연관된 message입니다.</param>
        /// <param name="secondaryMessage">수신한 message입니다.</param>
        public delegate void UserSecondaryMessageReceivedEventHandler(SECSMessage primaryMessage, SECSMessage secondaryMessage);
        /// <summary>
        /// Communication state 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="communicationState">Communication state입니다.</param>
        public delegate void CommunicationStateChangedEventHandler(CommunicationState communicationState);
        /// <summary>
        /// Constrol state 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="controlState">Control state입니다.</param>
        public delegate void ControlStateChangedEventHandler(ControlState controlState);
        /// <summary>
        /// Constrol state(Online) 변경 실패 이벤트 핸들러입니다.
        /// </summary>
        public delegate void ControlStateOnlineChangeFailedEventHandler();

        /// <summary>
        /// Equipment process state 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="equipmentProcessState">Equipment process state입니다.</param>
        public delegate void EquipmentProcessStateChangedEventHandler(byte equipmentProcessState);
        /// <summary>
        /// Spool state 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="spoolState">Spool state입니다.</param>
        public delegate void SpoolStateChangedEventHandler(SpoolState spoolState);
        /// <summary>
        /// Variable update request 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="updateType">Update type입니다.</param>
        /// <param name="variables">Update 할 variable입니다.</param>
        public delegate void VariableUpdateRequestEventHandler(VariableUpdateType updateType, List<VariableInfo> variables, string ceid);
        /// <summary>
        /// User message update request 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="message">Update 할 message입니다.</param>
        public delegate void UserMessageUpdateRequestEventHandler(SECSMessage message);
        /// <summary>
        /// Trace data update request 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="variables">Update 할 variable입니다.</param>
        public delegate void TraceDataUpdateRequestEventHandler(List<VariableInfo> variables);

        /// <summary>
        /// Establish Communications 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="mdln">Equipment Model Type입니다.</param>
        /// <param name="sofRev">Software revision입니다.</param>
        /// <returns>Establish Communications Acknowledge Code입니다.</returns>
        public delegate int ReceivedEstablishCommunicationsRequestEventHandler(string mdln, string sofRev);
        /// <summary>
        /// Remote command 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="remoteCommandInfo">수신한 remote command입니다.</param>
        public delegate void ReceivedRemoteCommandEventHandler(RemoteCommandInfo remoteCommandInfo);
        /// <summary>
        /// Remote command 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="remoteCommandInfo">수신한 remote command입니다.</param>
        public delegate void ReceivedInvalidRemoteCommandEventHandler(RemoteCommandInfo remoteCommandInfo);
        /// <summary>
        /// Enhanced remote command 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="remoteCommandInfo">수신한 remote command입니다.</param>
        public delegate void ReceivedEnhancedRemoteCommandEventHandler(EnhancedRemoteCommandInfo remoteCommandInfo);
        /// <summary>
        /// Enhanced remote command 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="remoteCommandInfo">수신한 remote command입니다.</param>
        public delegate void ReceivedInvalidEnhancedRemoteCommandEventHandler(EnhancedRemoteCommandInfo remoteCommandInfo);
        /// <summary>
        /// New equipment constants 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="newEcInfo">수신한 EC 정보입니다.</param>
        public delegate void ReceivedNewECVSendEventHandler(uint systemBytes, VariableCollection newEcInfo);
        /// <summary>
        /// Date &amp; time 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        public delegate void ReceivedDateTimeRequestEventHandler(uint systemBytes);        /// <summary>
                                                                                           /// Date &amp; time 수신 이벤트 핸들러입니다.
                                                                                           /// </summary>
                                                                                           /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
                                                                                           /// <param name="timeData">수신한 Date &amp; time입니다.</param>
        public delegate void ReceivedDateTimeSetRequestEventHandler(uint systemBytes, DateTime timeData);
        /// <summary>
        /// Loopback 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="receiveData">수신한 ABS입니다.</param>
        public delegate void ReceivedLoopbackEventHandler(List<byte> receiveData);
        /// <summary>
        /// Treminal message 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="tid">수신한 TID입니다.</param>
        /// <param name="terminalMessage">수신한 terminal message입니다.</param>
        public delegate void ReceivedTerminalMessageEventHandler(uint systemBytes, int tid, string terminalMessage);
        /// <summary>
        /// Treminal multi message 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="tid">수신한 TID입니다.</param>
        /// <param name="terminalMessages">수신한 terminal message입니다.</param>
        public delegate void ReceivedTerminalMultiMessageEventHandler(uint systemBytes, int tid, List<string> terminalMessages);
        /// <summary>
        /// Process Program load inquire 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppid">수신한 PPID입니다.</param>
        /// <param name="length">수신한 PPBODY length입니다.</param>
        public delegate void ReceivedPPLoadInquireEventHandler(uint systemBytes, string ppid, int length);
        /// <summary>
        /// Process Program send 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppid">수신한 PPID입니다.</param>
        /// <param name="ppbody">수신한 PPBODY입니다.</param>
        public delegate void ReceivedPPSendEventHandler(uint systemBytes, string ppid, List<byte> ppbody);
        /// <summary>
        /// Formatted Process Program send 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="fmtPPCollection">수신한 formatted PP 정보입니다.</param>
        public delegate void ReceivedFmtPPSendEventHandler(uint systemBytes, FmtPPCollection fmtPPCollection);
        /// <summary>
        /// Process Program request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppid">수신한 PPID입니다.</param>
        public delegate void ReceivedPPRequestEventHandler(uint systemBytes, string ppid);
        /// <summary>
        /// Formatted Process Program request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppid">수신한 PPID입니다.</param>
        public delegate void ReceivedFmtPPRequestEventHandler(uint systemBytes, string ppid);
        /// <summary>
        /// Delete Process Program send 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppids">수신한 PPID입니다.</param>
        public delegate void ReceivedDeletePPSendEventHandler(uint systemBytes, List<string> ppids);
        /// <summary>
        /// Current EPPD request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        public delegate void ReceivedCurrentEPPDRequestEventHandler(uint systemBytes);
        /// <summary>
        /// Request offline 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        public delegate void ReceivedRequestOfflineEventHandler(uint systemBytes);
        /// <summary>
        /// Request online 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        public delegate void ReceivedRequestOnlineEventHandler(uint systemBytes);
        /// <summary>
        /// Define report 수신 이벤트 핸들러입니다.
        /// </summary>
        public delegate void ReceivedDefineReportEventHandler();
        /// <summary>
        /// Link event 수신 이벤트 핸들러입니다.
        /// </summary>
        public delegate void ReceivedLinkEventReportEventHandler();
        /// <summary>
        /// Enable/Disable event report 수신 이벤트 핸들러입니다.
        /// </summary>
        public delegate void ReceivedEnableDisableEventReportEventHandler();
        /// <summary>
        /// Enable/Disable alarm send 수신 이벤트 핸들러입니다.
        /// </summary>
        public delegate void ReceivedEnableDisableAlarmSendEventHandler();
        /// <summary>
        /// Map error report send 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="mapError">Map Error.</param>
        /// <param name="dataLocation">Data location. </param>
        public delegate void ReceivedMapErrorReportSendEventHandler(int mapError, int dataLocation);

        /// <summary>
        /// Get attribute request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="objectType">Object type입니다.</param>
        /// <param name="objectIDs">Object ID 목록입니다.</param>
        /// <param name="objectQualifiers"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public delegate void ReceivedGetAttributeRequestEventHandler(
            string objectSpec,
            string objectType,
            List<string> objectIDs,
            List<ObjectQualifierInfo> objectQualifiers,
            List<string> attributes);
        /// <summary>
        /// Set attribute request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="objectType">Object type입니다.</param>
        /// <param name="objectIDs">Object ID 목록입니다.</param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public delegate void ReceivedSetAttributeRequestEventHandler(
            string objectSpec,
            string objectType,
            List<string> objectIDs,
            List<AttributeInfo> attributes);
        /// <summary>
        /// Get type request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">수신한 object spec입니다.</param>
        /// <returns></returns>
        public delegate void ReceivedGetTypeRequestEventHandler(string objectSpec);
        /// <summary>
        /// Get attribute name request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">수신한 object spec입니다.</param>
        /// <param name="objectTypes">수신한 object type 목록입니다.</param>
        /// <returns></returns>
        public delegate void ReceivedGetAttributeNameRequestEventHandler(string objectSpec, List<string> objectTypes);
        /// <summary>
        /// Create object request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="objectType">Object type입니다.</param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public delegate void ReceivedCreateObjectRequestEventHandler(
            uint systemBytes,
            string objectSpec,
            string objectType,
            List<AttributeInfo> attributes);
        /// <summary>
        /// Delete object request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public delegate void ReceivedDeleteObjectRequestEventHandler(string objectSpec, List<AttributeInfo> attributes);
        /// <summary>
        /// Object attach request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public delegate void ReceivedObjectAttachRequestEventHandler(string objectSpec, List<AttributeInfo> attributes);
        /// <summary>
        /// Attached object action request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="objectCommand">Object command입니다.</param>
        /// <param name="objectToken">Object token입니다.</param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public delegate void ReceivedAttachedObjectActionRequestEventHandler(
            string objectSpec,
            ulong objectCommand,
            ulong objectToken,
            List<AttributeInfo> attributes);
        /// <summary>
        ///  Supervised object action request 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="objectCommand">Object command입니다.</param>
        /// <param name="targetSpec">Target spec입니다.</param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public delegate void ReceivedSupervisedObjectActionRequestEventHandler(
            string objectSpec,
            int objectCommand,
            string targetSpec,
            List<AttributeInfo> attributes);

        /// <summary>
        /// Date &amp; time request 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="timeData">Date &amp; time입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public delegate bool ResponseDateTimeRequestEventHandler(DateTime timeData);
        /// <summary>
        /// Loopback 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="receiveData">수신한 ABS입니다.</param>
        /// <param name="sendData">송신할 ABS입니다.</param>
        public delegate void ResponseLoopbackEventHandler(List<byte> receiveData, List<byte> sendData);
        /// <summary>
        ///  Event Report Acknowledge 수신 이벤트 핸들러입니다.
        /// </summary>
        /// <param name = "ceid" >송신한 CEID입니다.</param >
        /// <param name="ack">수신한 ACKC6입니다.</param>
        public delegate void ResponseEventReportAcknowledgeEventHandler(string ceid, int ack);
        /// <summary>
        /// PP load inquire 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ppgnt">수신한 PPGNT입니다.</param>
        /// <param name="ppid">수신한 PPID입니다.</param>
        public delegate void ResponsePPLoadInquireEventHandler(int ppgnt, string ppid);
        /// <summary>
        /// PP send 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">수신한 ACK입니다.</param>
        /// <param name="ppid">수신한 PPID입니다.</param>
        public delegate void ResponsePPSendEventHandler(int ack, string ppid);
        /// <summary>
        /// Formatted PP send 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="fmtPPCollection">수신한 Formatted PP 정보입니다.</param>
        public delegate void ResponseFmtPPSendEventHandler(int ack, FmtPPCollection fmtPPCollection);
        /// <summary>
        /// PP request 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ppid">수신한 PPID입니다.</param>
        /// <param name="ppbody">수신한 PPBody입니다.</param>
        public delegate void ResponsePPRequestEventHandler(string ppid, List<byte> ppbody);
        /// <summary>
        /// Formatted PP request 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="fmtPPCollection">수신한 Formatted PP 정보입니다.</param>
        public delegate void ResponseFmtPPRequestEventHandler(FmtPPCollection fmtPPCollection);
        /// <summary>
        /// Formatted PP verification ack 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="fmtPPVerificationCollection">수신한 Formatted PP Verification 정보입니다.</param>
        public delegate void ResponseFmtPPVerificationEventHandler(FmtPPVerificationCollection fmtPPVerificationCollection);

        /// <summary>
        /// Terminal request ack 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        public delegate void ResponseTerminalRequestEventHandler(int ack);

        /// <summary>
        /// Map set-up data 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        public delegate void ResponseMapSetupDataAckEventHandler(int ack);
        /// <summary>
        /// Map set-up data request 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="mapSetupData"></param>
        public delegate void ResponseMapSetupDataEventHandler(MapSetupData mapSetupData);
        /// <summary>
        /// Map transmit grant 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        public delegate void ResponseMapTransmitGrantEventHandler(int ack);
        /// <summary>
        /// Map data send 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        public delegate void ResponseMapDataAckEventHandler(int ack);
        /// <summary>
        /// Map data type 1 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="mapData">수신한 map data입니다.</param>
        public delegate void ResponseMapDataType1EventHandler(MapDataType1 mapData);
        /// <summary>
        /// Map data type 2 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="mapData">수신한 map data입니다.</param>
        public delegate void ResponseMapDataType2EventHandler(MapDataType2 mapData);
        /// <summary>
        /// Map data type 3 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="mapData">수신한 map data입니다.</param>
        public delegate void ResponseMapDataType3EventHandler(MapDataType3 mapData);

        /// <summary>
        /// Get attribute data 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="objectAttributeInfos">수신한 attribute 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseGetAttributeDataEventHandler(int ack, List<ObjectAttributeInfo> objectAttributeInfos, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        /// Set attribute data 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="objectAttributeInfos">수신한 attribute 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseSetAttributeDataEventHandler(int ack, List<ObjectAttributeInfo> objectAttributeInfos, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        /// Get type data 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="objectTypes">수신한 type 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseGetTypeDataEventHandler(int ack, List<string> objectTypes, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        /// Get attribute name data 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="objectTypeInfos">수신한 type 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseGetAttributeNameDataEventHandler(int ack, List<ObjectTypeInfo> objectTypeInfos, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        ///  Create object 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectSpec">Object spec입니다.</param>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="attributeInfos">수신한 attribute 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseCreateObjectEventHandler(string objectSpec, int ack, List<AttributeInfo> attributeInfos, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        /// Delete object 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="attributeInfos">수신한 attribute 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseDeleteObjectEventHandler(int ack, List<AttributeInfo> attributeInfos, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        /// Object attach 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="objectToken">수신한 object token입니다.</param>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="attributeInfos">수신한 attribute 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseObjectAttachEventHandler(ulong objectToken, int ack, List<AttributeInfo> attributeInfos, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        /// Attached object action 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="attributeInfos">수신한 attribute 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseAttachedObjectActionEventHandler(int ack, List<AttributeInfo> attributeInfos, List<ObjectErrorItem> objectErrorItems);
        /// <summary>
        ///  Supervised object action 응답 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ack">ACK입니다.</param>
        /// <param name="attributeInfos">수신한 attribute 정보입니다.</param>
        /// <param name="objectErrorItems">수신한 error 정보입니다.</param>
        public delegate void ResponseSupervisedObjectActionEventHandler(int ack, List<AttributeInfo> attributeInfos, List<ObjectErrorItem> objectErrorItems);

        /// <summary>
        /// Equipment constants 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="ecInfo">변경한 equipment constants입니다.</param>
        /// <param name="ecv">변경한 equipment constants value입니다.</param>
        public delegate void ReportECVChangedEventHandler(List<VariableInfo> ecInfo, List<string> ecv);

        /// <summary>
        /// Log Write 시 발생합니다.
        /// </summary>
        public event Utility.Logger.LogWriteEventHandler OnWriteLog;
        /// <summary>
        /// SECS-I Log Write 시 발생합니다.
        /// </summary>
        public event Utility.Logger.LogWriteEventHandler OnSECS1Log;
        /// <summary>
        /// SECS-II Log Write 시 발생합니다.
        /// </summary>
        public event Utility.Logger.LogWriteEventHandler OnSECS2Log;
        /// <summary>
        /// Socket Connected 시 발생합니다.
        /// </summary>
        public event ConnectionStateChangedEventHandler OnGEMConnected;
        /// <summary>
        /// Socket Disconnected 시 발생합니다.
        /// </summary>
        public event ConnectionStateChangedEventHandler OnGEMDisconnected;
        /// <summary>
        /// HSMS Selected 시 발생합니다.
        /// </summary>
        public event ConnectionStateChangedEventHandler OnGEMSelected;
        /// <summary>
        /// HSMS Deselected 시 발생합니다.
        /// </summary>
        public event ConnectionStateChangedEventHandler OnGEMDeselected;
        /// <summary>
        /// Invalid Mesage 수신 시 발생합니다.
        /// </summary>
        public event InvalidMessageReceivedEventHandler OnInvalidMessageReceived;
        /// <summary>
        /// Unknown Message 수신 시 발생합니다.
        /// </summary>
        public event ReceivedUnknownMessageEventHandler OnReceivedUnknownMessage;

        /// <summary>
        /// Establish Communications 수신 시 발생합니다.(S1F13)
        /// </summary>
        public event ReceivedEstablishCommunicationsRequestEventHandler OnReceivedEstablishCommunicationsRequest;
        /// <summary>
        /// Remote Command 수신 시 발생합니다.(S2F41)
        /// </summary>
        public event ReceivedRemoteCommandEventHandler OnReceivedRemoteCommand;
        /// <summary>
        /// Invalid Remote Command 수신 시 발생합니다.(S2F41)
        /// </summary>
        public event ReceivedInvalidRemoteCommandEventHandler OnReceivedInvalidRemoteCommand;
        /// <summary>
        /// Enhanced Remote Command 수신 시 발생합니다.(S2F49)
        /// </summary>
        public event ReceivedEnhancedRemoteCommandEventHandler OnReceivedEnhancedRemoteCommand;
        /// <summary>
        /// Invalid Enhanced Remote Command 수신 시 발생합니다.(S2F49)
        /// </summary>
        public event ReceivedInvalidEnhancedRemoteCommandEventHandler OnReceivedInvalidEnhancedRemoteCommand;
        /// <summary>
        /// HOST에서 EC Change 수신 시 발생합니다.(S2F15)
        /// </summary>
        public event ReceivedNewECVSendEventHandler OnReceivedNewECVSend;

        /// <summary>
        /// HOST에서 Loopback Diagnostic Request 수신 시 발생합니다.(S2F25)
        /// </summary>
        public event ReceivedLoopbackEventHandler OnReceivedLoopback;
        /// <summary>
        /// HOST에서 Terminal Message 수신 시 발생합니다.(S10F3)
        /// </summary>
        public event ReceivedTerminalMessageEventHandler OnReceivedTerminalMessage;
        /// <summary>
        /// HOST에서 Terminal Multi Message 수신 시 발생합니다.(S10F5)
        /// </summary>
        public event ReceivedTerminalMultiMessageEventHandler OnReceivedTerminalMultiMessage;
        /// <summary>
        /// HOST에서 Offline 전환 요청 수신 시 발생합니다.(S1F15)
        /// </summary>
        public event ReceivedRequestOfflineEventHandler OnReceivedRequestOffline;
        /// <summary>
        /// HOST에서 Online 전환 요청 수신 시 발생합니다.(S1F17)
        /// </summary>
        public event ReceivedRequestOnlineEventHandler OnReceivedRequestOnline;
        /// <summary>
        /// HOST에서 Define Report 수신 시 발생합니다.(S2F33)
        /// </summary>
        public event ReceivedDefineReportEventHandler OnReceivedDefineReport;
        /// <summary>
        /// HOST에서 Link Event 수신 시 발생합니다.(S2F35)
        /// </summary>
        public event ReceivedLinkEventReportEventHandler OnReceivedLinkEventReport;
        /// <summary>
        /// HOST에서 Enable/Disable Event Report 수신 시 발생합니다.(S2F37)
        /// </summary>
        public event ReceivedEnableDisableEventReportEventHandler OnReceivedEnableDisableEventReport;
        /// <summary>
        /// HOST에서 Enable/Disable Alarm Send 수신 시 발생합니다.(S5F3)
        /// </summary>
        public event ReceivedEnableDisableAlarmSendEventHandler OnReceivedEnableDisableAlarmSend;

        /// <summary>
        /// HOST에서 Get attribute request 수신 시 발생합니다.(S14F1)
        /// </summary>
        public event ReceivedGetAttributeRequestEventHandler OnReceivedGetAttributeRequest;
        /// <summary>
        /// HOST에서 Set attribute request 수신 시 발생합니다.(S14F3)
        /// </summary>
        public event ReceivedSetAttributeRequestEventHandler OnReceivedSetAttributeRequest;
        /// <summary>
        /// HOST에서 Get type request 수신 시 발생합니다.(S14F5)
        /// </summary>
        public event ReceivedGetTypeRequestEventHandler OnReceivedGetTypeRequest;
        /// <summary>
        /// HOST에서 Get attribute name request 수신 시 발생합니다.(S14F7)
        /// </summary>
        public event ReceivedGetAttributeNameRequestEventHandler OnReceivedGetAttributeNameRequest;
        /// <summary>
        /// HOST에서 Create object request 수신 시 발생합니다.(S14F9)
        /// </summary>
        public event ReceivedCreateObjectRequestEventHandler OnReceivedCreateObjectRequest;
        /// <summary>
        /// HOST에서 Delete object request 수신 시 발생합니다.(S14F11)
        /// </summary>
        public event ReceivedDeleteObjectRequestEventHandler OnReceivedDeleteObjectRequest;
        /// <summary>
        /// HOST에서 Object attach request 수신 시 발생합니다.(S14F13)
        /// </summary>
        public event ReceivedObjectAttachRequestEventHandler OnReceivedAttachObjectRequest;
        /// <summary>
        /// HOST에서 Attached object action request 수신 시 발생합니다.(S14F15)
        /// </summary>
        public event ReceivedAttachedObjectActionRequestEventHandler OnReceivedAttachedObjectActionRequest;
        /// <summary>
        /// HOST에서 Supervised object action request 수신 시 발생합니다.(S14F17)
        /// </summary>
        public event ReceivedSupervisedObjectActionRequestEventHandler OnReceivedSupervisedObjectActionRequest;

        /// <summary>
        /// HOST에서 Time Data 수신 시 발생합니다.(S2F18)
        /// </summary>
        public event ResponseDateTimeRequestEventHandler OnResponseDateTimeRequest;
        /// <summary>
        /// HOST에서 Loopback Ack 수신 시 발생합니다.(S2F26)
        /// </summary>
        public event ResponseLoopbackEventHandler OnResponseLoopback;
        /// <summary>
        /// HOST에서 Event Report Acknowledge 수신 시 발생합니다.(S6F12)
        /// </summary>
        public event ResponseEventReportAcknowledgeEventHandler OnResponseEventReportAcknowledge;
        /// <summary>
        /// HOST에서 Process Program Load Inquire 수신 시 발생합니다.(S7F1)
        /// </summary>
        public event ReceivedPPLoadInquireEventHandler OnReceivedPPLoadInquire;
        /// <summary>
        /// HOST에서 PP Send 수신(Unformatted) 시 발생합니다.(S7F3)
        /// </summary>
        public event ReceivedPPSendEventHandler OnReceivedPPSend;
        /// <summary>
        /// HOST에서 PP Send 수신(Formatted) 시 발생합니다.(S7F23)
        /// </summary>
        public event ReceivedFmtPPSendEventHandler OnReceivedFmtPPSend;
        /// <summary>
        /// HOST에서 PP Load Inquire ACK 수신 시 발생합니다.(S7F2)
        /// </summary>
        public event ResponsePPLoadInquireEventHandler OnResponsePPLoadInquire;
        /// <summary>
        /// HOST에서 PP Send ACK 수신(Unformatted) 시 발생합니다.(S7F4)
        /// </summary>
        public event ResponsePPSendEventHandler OnResponsePPSend;
        /// <summary>
        /// HOST에서 PP Request ACK 수신(Unformatted) 시 발생합니다.(S7F6)
        /// </summary>
        public event ResponsePPRequestEventHandler OnResponsePPRequest;
        /// <summary>
        /// HOST에서 PP Send ACK 수신(Formatted) 시 발생합니다.(S7F24)
        /// </summary>
        public event ResponseFmtPPSendEventHandler OnResponseFmtPPSend;
        /// <summary>
        /// HOST에서 PP Request ACK 수신(Formatted) 시 발생합니다.(S7F26)
        /// </summary>
        public event ResponseFmtPPRequestEventHandler OnResponseFmtPPRequest;
        /// <summary>
        /// HOST에서 PP Verification ACK 수신(Formatted) 시 발생합니다.(S7F28)
        /// </summary>
        public event ResponseFmtPPVerificationEventHandler OnResponseFmtPPVerification;

        /// <summary>
        /// HOST에서  Terminal Request ACK 수신(Formatted) 시 발생합니다.(S10F2)
        /// </summary>
        public event ResponseTerminalRequestEventHandler OnResponseTerminalRequest;



        /// <summary>
        /// HOST에서 Map Set-up Data ACK 수신 시 발생합니다.(S12F2)
        /// </summary>
        public event ResponseMapSetupDataAckEventHandler OnResponseMapSetupDataAck;
        /// <summary>
        /// HOST에서 Map Set-up Data 수신 시 발생합니다.(S12F4)
        /// </summary>
        public event ResponseMapSetupDataEventHandler OnResponseMapSetupData;
        /// <summary>
        /// HOST에서 Map Transmit Grant 수신 시 발생합니다.(S12F6)
        /// </summary>
        public event ResponseMapTransmitGrantEventHandler OnResponseMapTransmitGrant;
        /// <summary>
        /// HOST에서 Map Data Acknowledge Type 1 수신 시 발생합니다.(S12F8)
        /// </summary>
        public event ResponseMapDataAckEventHandler OnResponseMapDataAckType1;
        /// <summary>
        /// HOST에서 Map Data Acknowledge Type 2 수신 시 발생합니다.(S12F10)
        /// </summary>
        public event ResponseMapDataAckEventHandler OnResponseMapDataAckType2;
        /// <summary>
        /// HOST에서 Map Data Acknowledge Type 3 수신 시 발생합니다.(S12F12)
        /// </summary>
        public event ResponseMapDataAckEventHandler OnResponseMapDataAckType3;
        /// <summary>
        /// HOST에서 Map Data Type 1 수신 시 발생합니다.(S12F14)
        /// </summary>
        public event ResponseMapDataType1EventHandler OnResponseMapDataType1;
        /// <summary>
        /// HOST에서 Map Data Type 2 수신 시 발생합니다.(S12F16)
        /// </summary>
        public event ResponseMapDataType2EventHandler OnResponseMapDataType2;
        /// <summary>
        /// HOST에서 Map Data Type 3 수신 시 발생합니다.(S12F18)
        /// </summary>
        public event ResponseMapDataType3EventHandler OnResponseMapDataType3;

        /// <summary>
        /// HOST에서 Get Attribute Data ACK 수신 시 발생합니다.(S14F2)
        /// </summary>
        public event ResponseGetAttributeDataEventHandler OnResponseGetAttributeData;
        /// <summary>
        /// HOST에서 Set Attribute Data ACK 수신 시 발생합니다.(S14F4)
        /// </summary>
        public event ResponseSetAttributeDataEventHandler OnResponseSetAttributeData;
        /// <summary>
        /// HOST에서 Get Type Data ACK 수신 시 발생합니다.(S14F6)
        /// </summary>
        public event ResponseGetTypeDataEventHandler OnResponseGetTypeData;
        /// <summary>
        /// HOST에서 Get Attribute Name Data ACK 수신 시 발생합니다.(S14F8)
        /// </summary>
        public event ResponseGetAttributeNameDataEventHandler OnResponseGetAttributeNameData;
        /// <summary>
        /// HOST에서 Create Object ACK 수신 시 발생합니다.(S14F10)
        /// </summary>
        public event ResponseCreateObjectEventHandler OnResponseCreateObject;
        /// <summary>
        /// HOST에서 Delete Object ACK 수신 시 발생합니다.(S14F12)
        /// </summary>
        public event ResponseDeleteObjectEventHandler OnResponseDeleteObject;
        /// <summary>
        /// HOST에서 Object Attach ACK 수신 시 발생합니다.(S14F14)
        /// </summary>
        public event ResponseObjectAttachEventHandler OnResponseObjectAttach;
        /// <summary>
        /// HOST에서 Attached Object Action ACK 수신 시 발생합니다.(S14F16)
        /// </summary>
        public event ResponseAttachedObjectActionEventHandler OnResponseAttachedObjectAction;
        /// <summary>
        /// HOST에서 Supervised Object Action ACK 수신 시 발생합니다.(S14F18)
        /// </summary>
        public event ResponseSupervisedObjectActionEventHandler OnResponseSupervisedObjectAction;




        /// <summary>
        /// HOST에서 Date and Time Request 수신 시 발생합니다.(S2F17)
        /// </summary>
        public event ReceivedDateTimeRequestEventHandler OnReceivedDateTimeRequest;
        /// <summary>
        /// HOST에서 Date and Time Set Request 수신 시 발생합니다.(S2F31)
        /// </summary>
        public event ReceivedDateTimeSetRequestEventHandler OnReceivedDateTimeSetRequest;
        /// <summary>
        /// HOST에서 Process Program Request 수신(Unformatted) 시 발생합니다.(S7F5)
        /// </summary>
        public event ReceivedPPRequestEventHandler OnReceivedPPRequest;
        /// <summary>
        /// HOST에서 Process Program Request 수신(Formatted) 시 발생합니다.(S7F25)
        /// </summary>
        public event ReceivedFmtPPRequestEventHandler OnReceivedFmtPPRequest;
        /// <summary>
        /// HOST에서 Process Program Delete 수신 시 발생합니다.(S7F17)
        /// </summary>
        public event ReceivedDeletePPSendEventHandler OnReceivedDeletePPSend;
        /// <summary>
        /// HOST에서 Process Program List Request 수신 시 발생합니다.(S7F19)
        /// </summary>
        public event ReceivedCurrentEPPDRequestEventHandler OnReceivedCurrentEPPDRequest;

        /// <summary>
        /// HOST에서  Map Error Report Send 수신 시 발생합니다.(S12F19)
        /// </summary>
        public event ReceivedMapErrorReportSendEventHandler OnReceivedMapErrorReportSend;



        /// <summary>
        /// 사용자 정의 Primary Message 수신 시 발생합니다.
        /// </summary>
        public event UserPrimaryMessageReceivedEventHandler OnUserPrimaryMessageReceived;
        /// <summary>
        /// 사용자 정의 Secondary Message 수신 시 발생합니다.
        /// </summary>
        public event UserSecondaryMessageReceivedEventHandler OnUserSecondaryMessageReceived;
        /// <summary>
        /// Communication State 변경 시 발생합니다.
        /// </summary>
        public event CommunicationStateChangedEventHandler OnCommunicationStateChanged;
        /// <summary>
        /// Control State 변경 시 발생합니다.
        /// </summary>
        public event ControlStateChangedEventHandler OnControlStateChanged;
        /// <summary>
        /// Control State(Online) 변경 실패 시 발생합니다.
        /// </summary>
        public event ControlStateOnlineChangeFailedEventHandler OnControlStateOnlineChangeFailed;

        /// <summary>
        /// Equipment Process State 변경 시 발생합니다.
        /// </summary>
        public event EquipmentProcessStateChangedEventHandler OnEquipmentProcessState;
        /// <summary>
        /// Spool State 변경 시 발생합니다.
        /// </summary>
        public event SpoolStateChangedEventHandler OnSpoolStateChanged;
        /// <summary>
        /// Variable을 Update해야 할 경우 발생합니다.
        /// </summary>
        public event VariableUpdateRequestEventHandler OnVariableUpdateRequest;
        /// <summary>
        /// User 정의 GEM message를 Update해야 할 경우 발생합니다.
        /// </summary>
        public event UserMessageUpdateRequestEventHandler OnUserGEMMessageUpdateRequest;
        /// <summary>
        /// Trace 대상 Variable을 Update해야 할 경우 발생합니다.(Trace data 수집 주기)
        /// </summary>
        public event TraceDataUpdateRequestEventHandler OnTraceDataUpdateRequest;
        #endregion

        private const string STANDARD_MESSAGE_SET = "[{StandardMessageSet}]";
        private const string DEFAULT_UMD_FILE_NAME = "UbiGEM.umd";

        private const string DESCRIPTION_UNKNOWN_VID = "Unknown VID";

        private const int ACK = 0;
        private const int NAK = 1;
        private const int DEFAULT_T3 = 45;
        private const int DEFAULT_T5 = 10;
        private const int DEFAULT_T6 = 5;
        private const int DEFAULT_T7 = 10;
        private const int DEFAULT_T8 = 5;
        private const int DEFAULT_LINKTEST = 120;

        private const int USB_KEYLOCK_WAIT_DURATION = 3 * 60 * 60 * 1000;
        //private const int USB_KEYLOCK_WAIT_DURATION = 60 * 1000;
        private const int USB_KEYLOCK_WARNING_DURATION = 10 * 60 * 1000;

        private UbiCom.Net.Driver.HSMSDriver _driver;

        private CommunicationState _communicationState;
        private ControlState _controlState;
        private byte _equipmentProcessState;
        private readonly SpoolState _spoolState;
        internal Utility.Logger.Logger _logger;

        private readonly CollectionEventCollection _collectionEventCollection;
        private readonly ReportCollection _reportCollection;
        private readonly VariableCollection _variableCollection;
        private readonly DataDictionaryCollection _dataDictionaryCollection;
        private readonly TraceCollection _traceCollection;
        private readonly AlarmCollection _alarmCollection;
        private readonly RemoteCommandCollection _remoteCommandCollection;
        private readonly LimitMonitoringCollection _limitMonitoringCollection;
        private readonly SpoolingCollection _spoolingCollection;


        private VariableCollection _orgVariableCollection;

        private readonly MessageMaker _messageMaker;
        public void SetDataID(long _dataId)
        {
            _messageMaker._dataId = _dataId;
        }

        private readonly Tool.TimerManager _timerManager;

        private readonly Tool.ConfigFileManager _configFileManager;

        private readonly System.Threading.Timer _timerCommDelay;
        private readonly System.Threading.Timer _timerAreYouThere;

        private SECSItemFormat _ceidFormat;
        private SECSItemFormat _rptidFormat;
        private SECSItemFormat _vidFormat;

        private readonly UbiSam.Net.KeyLock.LicenseChecker _licenseChecker;
        private readonly string _licenseKey;
        private bool _licenseFailed;
        private bool _isInitialized;
        private bool _isStarted;
        private bool _disposed;

        /// <summary>
        /// HSMS Driver를 가져옵니다.
        /// </summary>
        public UbiCom.Net.Driver.HSMSDriver HSMSDriver
        {
            get { return this._driver; }
        }

        /// <summary>
        /// Colelction Event 정보를 가져옵니다.
        /// </summary>
        public CollectionEventCollection CollectionEvents
        {
            get { return this._collectionEventCollection; }
        }

        /// <summary>
        /// Report 정보를 가져옵니다.
        /// </summary>
        public ReportCollection Reports
        {
            get { return this._reportCollection; }
        }

        /// <summary>
        /// Variable(EC, SV, DV) 정보를 가져옵니다.
        /// </summary>
        public VariableCollection Variables
        {
            get { return this._variableCollection; }
        }

        /// <summary>
        /// Alarm 정보를 가져옵니다.
        /// </summary>
        public AlarmCollection Alarms
        {
            get { return this._alarmCollection; }
        }

        /// <summary>
        /// Data Dictionary 정보를 가져옵니다.
        /// </summary>
        public DataDictionaryCollection DataDictionary
        {
            get { return this._dataDictionaryCollection; }
        }

        /// <summary>
        /// Current Communication State를 가져옵니다.
        /// </summary>
        public CommunicationState CommunicationState
        {
            get { return this._communicationState; }
        }

        /// <summary>
        /// Current Communication State를 가져옵니다.
        /// </summary>
        public ControlState ControlState
        {
            get { return this._controlState; }
            set { this._controlState = value; }
        }

        /// <summary>
        /// Current Equipment Process State를 가져옵니다.
        /// </summary>
        public byte EquipmentProcessState
        {
            get { return this._equipmentProcessState; }
            set { this._equipmentProcessState = value; }
        }

        /// <summary>
        /// Current Spool State를 가져옵니다.
        /// </summary>
        public SpoolState SpoolState
        {
            get { return this._spoolState; }
        }


        private SECSMessageCollection _standardMessageCollection;

        /// <summary>
        /// SEMI Standard Message Set을 가져옵니다.
        /// </summary>
        public SECSMessageCollection StandardMessage
        {
            get { return this._standardMessageCollection; }
        }

        private SECSMessageCollection _userMessageCollection;

        /// <summary>
        /// User Defined Message Set을 가져옵니다.
        /// </summary>
        public SECSMessageCollection UserMessage
        {
            get { return this._userMessageCollection; }
        }

        private SECSMessageCollection _userGEMMessageCollection;

        /// <summary>
        /// SEMI Standard Message Set(User Defined)을 가져옵니다. (no used)
        /// </summary>
        public SECSMessageCollection UserGEMMessage
        {
            get { return this._userGEMMessageCollection; }
        }

        /// <summary>
        /// Message Manager를 가져옵니다.
        /// </summary>
        public SECSMessageManager MessageManager
        {
            get { return this._driver.MessageManager; }
        }

        /// <summary>
        /// Remote Command 정보를 가져옵니다.
        /// </summary>
        public RemoteCommandCollection RemoteCommand
        {
            get { return this._remoteCommandCollection; }
        }

        /// <summary>
        /// UGC에 정의된 variable structure를 가져옵니다.
        /// </summary>
        public VariableCollection VariableStructure
        {
            get { return this._orgVariableCollection; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public GemDriver()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;

            CheckKeyLockDLL();

            this._driver = new UbiCom.Net.Driver.HSMSDriver();
            this._collectionEventCollection = new CollectionEventCollection();
            this._reportCollection = new ReportCollection();
            this._variableCollection = new VariableCollection();
            this._dataDictionaryCollection = new DataDictionaryCollection();
            this._traceCollection = new TraceCollection();
            this._alarmCollection = new AlarmCollection();
            this._remoteCommandCollection = new RemoteCommandCollection();
            this._limitMonitoringCollection = new LimitMonitoringCollection();
            this._spoolingCollection = new SpoolingCollection();
            this._messageMaker = new MessageMaker();
            this._configFileManager = new Tool.ConfigFileManager();
            this._logger = new Utility.Logger.Logger();

            this._timerManager = new Tool.TimerManager();

            this._communicationState = CommunicationState.Disabled;

            this._userGEMMessageCollection = new SECSMessageCollection();
            this._userMessageCollection = new SECSMessageCollection();

            #region [Event Subscribe]
            this._driver.OnSECSConnected += Driver_OnSECSConnected;
            this._driver.OnSECSDisconnected += Driver_OnSECSDisconnected;
            this._driver.OnSECSSelected += Driver_OnSECSSelected;
            this._driver.OnSECSDeselected += Driver_OnSECSDeselected;
            this._driver.OnReceivedPrimaryMessage += Driver_OnReceivedPrimaryMessage;
            this._driver.OnReceivedSecondaryMessage += Driver_OnReceivedSecondaryMessage;
            this._driver.OnReceivedInvalidPrimaryMessage += Driver_OnReceivedInvalidPrimaryMessage;
            this._driver.OnReceivedInvalidSecondaryMessage += Driver_OnReceivedInvalidSecondaryMessage;
            this._driver.OnReceivedUnknownMessage += Driver_OnReceivedUnknownMessage;
            this._driver.OnT3Timeout += Driver_OnT3Timeout;
            this._driver.OnTimeout += Driver_OnTimeout;

            this._driver.OnSECS1WriteLog += Driver_OnSECS1WriteLog;
            this._driver.OnSECS2WriteLog += Driver_OnSECS2WriteLog;

            this._timerManager.OnReportTraceData += TimerManager_OnReportTraceData;

            this._messageMaker.OnUserGEMMessageUpdateRequest += MessageMaker_OnUserGEMMessageUpdateRequest;
            this._messageMaker.OnMessageMakerLogging += MessageMaker_OnMessageMakerLogging;

            this._logger.OnWriteLog += Logger_OnWriteLog;
            #endregion

            this._configFileManager.ConfigFileName = string.Empty;
            this._configFileManager.CollectionEventCollection = this._collectionEventCollection;
            this._configFileManager.ReportCollection = this._reportCollection;
            this._configFileManager.VariableCollection = this._variableCollection;
            this._configFileManager.DataDictionaryCollection = this._dataDictionaryCollection;
            this._configFileManager.TraceCollection = this._traceCollection;
            this._configFileManager.AlarmCollection = this._alarmCollection;
            this._configFileManager.RemoteCommandCollection = this._remoteCommandCollection;

            this._configFileManager.LoadPreDefined();

            LoadStandardLibrary();

            this._timerCommDelay = new System.Threading.Timer(CallbackEstablishCommunication);
            this._timerAreYouThere = new System.Threading.Timer(CallbackAreYouThereTimeout);

            this._licenseFailed = false;
            this._licenseChecker = new UbiSam.Net.KeyLock.LicenseChecker()
            {
                ErrorMessageShow = UbiSam.Net.KeyLock.Structure.ErrorMessageShowType.None,
                USBKeyLockWarningDuration = USB_KEYLOCK_WARNING_DURATION,
                USBKeyLockWaitDuration = USB_KEYLOCK_WAIT_DURATION,
                ExitOnLicenseFail = false
            };

            this._licenseChecker.CheckActiveEvent += LicenseChecker_CheckActiveEvent;
            this._licenseChecker.LicenseCheckEvent += LicenseChecker_LicenseCheckEvent;
            this._licenseChecker.OnRequestLogging += LicenseChecker_OnRequestLogging;

            this._licenseKey = Guid.NewGuid().ToString();

            this._isInitialized = false;
            this._isStarted = false;
            this._disposed = false;
        }

        /// <summary>
        /// 기본 소멸자입니다.
        /// </summary>
        ~GemDriver()
        {
            Dispose(false);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                this._logger.WriteGEM(e.ExceptionObject as Exception);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        /// <summary>
        /// GEM Driver를 초기화합니다.
        /// </summary>
        /// <param name="configurationFileName">GEM Driver 환경 설정 파일입니다.</param>
        /// <param name="errorText">GEM Driver 초기화 실패 시 실패 사유입니다.(OK시 string.Empty)</param>
        /// <returns>GEM Driver 초기화 결과입니다.</returns>
        public GemDriverError Initialize(string configurationFileName, out string errorText)
        {
            GemDriverError result;
            XElement messageElement;
            string logText;
            System.Diagnostics.Process currentProcess;
            System.Reflection.Assembly assembly;
            Version version;
            DataDictionaryInfo dataDictionaryInfo;
            VariableInfo variableInfo;
            VariableInfo controlStateVariableInfo;
            string umdFileName;

            this._isInitialized = false;

            try
            {
                currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                logText = string.Empty;

                assembly = System.Reflection.Assembly.GetExecutingAssembly();
                version = assembly.GetName().Version;

                this._configFileManager.ConfigFileName = configurationFileName;

                result = this._configFileManager.LoadConfigFile(out errorText, out string notice);

                logText = string.Format("Initialize (PID={0}) (Version={1})", currentProcess.Id, version);

                this._configFileManager.Configurtion.UMDFileName = STANDARD_MESSAGE_SET;

                if (this._configFileManager.GEMConfiguration != null && string.IsNullOrEmpty(this._configFileManager.Configurtion.DriverName) == false)
                {
                    this._logger.Initialize(this._configFileManager.GEMConfiguration, this._configFileManager.Configurtion.DriverName);
                }
                else
                {
                    this._logger.Initialize();
                }

                if (result == GemDriverError.Ok)
                {
                    result = (GemDriverError)this._driver.Initialize(this._configFileManager.Configurtion, out errorText);

                    if (result == GemDriverError.Ok)
                    {
                        umdFileName = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, DEFAULT_UMD_FILE_NAME);

                        if (System.IO.File.Exists(umdFileName) == true)
                        {
                            errorText = this._driver.MessageManager.Load(umdFileName);

                            if (string.IsNullOrEmpty(errorText) == false)
                            {
                                this._logger.WriteGEM($"User UMD file load failure:{umdFileName}, Error Text={errorText}");

                                messageElement = XElement.Load(new System.IO.StringReader(Resources.XML.StandardMessageStructure));

                                errorText = this._driver.MessageManager.Load(messageElement);
                            }
                            else
                            {
                                this._logger.WriteGEM($"Use user UMD file:{umdFileName}");
                            }
                        }
                        else
                        {
                            messageElement = XElement.Load(new System.IO.StringReader(Resources.XML.StandardMessageStructure));

                            errorText = this._driver.MessageManager.Load(messageElement);
                        }

                        messageElement = XElement.Load(configurationFileName);

                        if (messageElement.Element("SECSMessage") != null &&
                            messageElement.Element("SECSMessage").Element("UserMessage") != null)
                        {
                            using (SECSMessageManager loader = new SECSMessageManager())
                            {
                                loader.Load(messageElement.Element("SECSMessage").Element("UserMessage"));

                                this._userGEMMessageCollection = loader.Messages;

                                foreach (KeyValuePair<string, SECSMessage> tempMessage in loader.Messages.MessageInfo)
                                {
                                    this._driver.MessageManager.AddUserDefinedMessage(tempMessage.Value);
                                }
                            }
                        }

                        if (messageElement.Element("SECSMessage") != null &&
                            messageElement.Element("SECSMessage").Element("UserCustomMessage") != null)
                        {
                            using (SECSMessageManager loader = new SECSMessageManager())
                            {
                                loader.Load(messageElement.Element("SECSMessage").Element("UserCustomMessage"));

                                this._userMessageCollection = loader.Messages;

                                foreach (KeyValuePair<string, SECSMessage> tempMessage in loader.Messages.MessageInfo)
                                {
                                    this._driver.MessageManager.AddUserDefinedMessage(tempMessage.Value);
                                }
                            }
                        }

                        if (this._driver.MessageManager == null || this._driver.MessageManager.Messages == null)
                        {
                            result = GemDriverError.FileLoadFailed;
                        }
                        else
                        {
                            this._orgVariableCollection = this._variableCollection.CopyTo();

                            this._messageMaker.Initialize(this._driver.Config.DeviceType,
                                                          this._driver.MessageManager.Messages,
                                                          this._collectionEventCollection,
                                                          this._reportCollection,
                                                          this._variableCollection,
                                                          this._dataDictionaryCollection,
                                                          this._userGEMMessageCollection,
                                                          this._userMessageCollection,
                                                          this._orgVariableCollection);

                            dataDictionaryInfo = this._dataDictionaryCollection[PreDefinedDataDictinary.CEID.ToString()];
                            this._ceidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                            dataDictionaryInfo = this._dataDictionaryCollection[PreDefinedDataDictinary.RPTID.ToString()];
                            this._rptidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                            dataDictionaryInfo = this._dataDictionaryCollection[PreDefinedDataDictinary.VID.ToString()];
                            this._vidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

                            this._isInitialized = true;
                        }

                        variableInfo = this._variableCollection.GetVariableInfo(VariableType.ECV, PreDefinedECV.InitControlState.ToString());

                        if (variableInfo != null)
                        {
                            try
                            {
                                ControlState controlState;

                                controlState = (ControlState)((int)variableInfo.Value);

                                this._controlState = controlState;
                            }
                            catch (Exception ex)
                            {
                                this._controlState = ControlState.EquipmentOffline;

                                System.Diagnostics.Debug.Print(ex.Message);
                                this._logger.WriteGEM(ex);
                            }
                        }
                        else
                        {
                            this._controlState = ControlState.EquipmentOffline;
                        }

                        controlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ControlState.ToString());

                        if (controlStateVariableInfo != null)
                        {
                            controlStateVariableInfo.Value.SetValue((int)this._controlState);
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorText) == false)
                {
                    logText += (", Error=" + errorText);

                    this._logger.WriteGEM(Utility.Logger.LogLevel.Error, logText);
                }
                else
                {
                    this._logger.WriteGEM(Utility.Logger.LogLevel.Information, logText);
                }

                if (string.IsNullOrEmpty(notice) == false)
                {
                    this._logger.WriteGEM(Utility.Logger.LogLevel.Warning, $"Notice\r\n{notice}");
                }
            }
            catch (Exception ex)
            {
                this._isInitialized = false;

                result = GemDriverError.Unknown;
                errorText = ex.Message;

                System.Diagnostics.Debug.Print(ex.Message);
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// GEM Driver를 Dispose합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// GEM Driver를 Dispose합니다.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (this._driver != null)
                    {
                        this._driver.Close();
                        this._driver.Terminate();

                        this._driver = null;
                    }

                    if (this._logger != null)
                    {
                        this._logger.Dispose();

                        this._logger = null;
                    }
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// GEM Driver 작동을 시작합니다.
        /// </summary>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError Start()
        {
            GemDriverError result;
            string logText;

            if (this._licenseFailed == true)
            {
                return GemDriverError.LicenseVerificationFailed;
            }

            if (this._configFileManager == null ||
                string.IsNullOrEmpty(this._configFileManager.ConfigFileName) == true ||
                this._isInitialized == false)
            {
                return GemDriverError.InvalidConfiguration;
            }

            this.MessageManager.SetDeviceID(this._driver.Config.DeviceID);

            this._isStarted = true;

            if (this._driver.Connected == false)
            {
                if (this._communicationState != CommunicationState.NotCommunication)
                {
                    this._communicationState = CommunicationState.NotCommunication;

                    this.OnCommunicationStateChanged?.Invoke(this._communicationState);
                }

                result = (GemDriverError)this._driver.Open();

                if (result == GemDriverError.Ok)
                {
                    this._licenseChecker.LicenseFailWaitTimerEnabled = true;

                    logText = string.Format("SECS Driver Open : Driver={0}", this._configFileManager.Configurtion.DriverName);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("SECS Driver Open Failed : Driver={0}, Result={1}", this._configFileManager.Configurtion.DriverName, result);

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            else
            {
                result = GemDriverError.AlreadyConnected;

                logText = string.Format("SECS Driver Open Skip : Driver={0}, Result={1}", this._configFileManager.Configurtion.DriverName, result);

                this._logger.WriteGEM(LogLevel.Warning, logText);
            }

            return result;
        }

        /// <summary>
        /// GEM Driver를 정지시킵니다.
        /// </summary>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError Stop()
        {
            GemDriverError result;
            string logText;

            this._isStarted = false;
            this._licenseChecker.LicenseFailWaitTimerEnabled = false;

            result = (GemDriverError)this._driver.Close();

            if (this._communicationState != CommunicationState.Disabled)
            {
                this._communicationState = CommunicationState.Disabled;

                this.OnCommunicationStateChanged?.Invoke(this._communicationState);
            }

            if (result == GemDriverError.Ok)
            {
                logText = string.Format("SECS Driver Close : Driver={0}", this._configFileManager.Configurtion.DriverName);

                this._logger.WriteGEM(LogLevel.Information, logText);
            }
            else
            {
                logText = string.Format("SECS Driver Close Failed : Driver={0}, Result={1}", this._configFileManager.Configurtion.DriverName, result);

                this._logger.WriteGEM(LogLevel.Error, logText);
            }

            return result;
        }

        /// <summary>
        /// GEM Driver의 신규 Project를 생성합니다.
        /// </summary>
        public void NewProject()
        {
            this._collectionEventCollection.Items.Clear();
            this._reportCollection.Items.Clear();
            this._variableCollection.Items.Clear();
            this._dataDictionaryCollection.Items.Clear();
            this._traceCollection.Items.Clear();
            this._alarmCollection.Items.Clear();
            this._remoteCommandCollection.RemoteCommandItems.Clear();
            this._userGEMMessageCollection.Clear();
            this._userMessageCollection.Clear();

            this._configFileManager.ConfigFileName = string.Empty;

            this._configFileManager.GEMConfiguration.LogEnabledGEM = Utility.Logger.LogMode.Hour;
            this._configFileManager.GEMConfiguration.LogExpirationDay = 30;
            this._configFileManager.GEMConfiguration.LogPath = @"c:\Log";

            this._configFileManager.Configurtion.DriverName = string.Empty;
            this._configFileManager.Configurtion.DeviceType = DeviceType.Equipment;
            this._configFileManager.Configurtion.IsAsyncMode = true;
            this._configFileManager.Configurtion.DeviceID = 0;
            this._configFileManager.Configurtion.SECSMode = SECSMode.HSMS;
            this._configFileManager.Configurtion.MaxMessageSize = 2 * 1024 * 1024;
            this._configFileManager.Configurtion.UMDFileName = string.Empty;
            this._configFileManager.Configurtion.LogPath = @"c:\Log";
            this._configFileManager.Configurtion.LogEnabledSECS1 = UbiCom.Net.Structure.LogMode.Hour;
            this._configFileManager.Configurtion.LogEnabledSECS2 = UbiCom.Net.Structure.LogMode.Hour;
            this._configFileManager.Configurtion.LogEnabledSystem = UbiCom.Net.Structure.LogMode.None;
            this._configFileManager.Configurtion.LogExpirationDay = 30;
            this._configFileManager.Configurtion.HSMSModeConfig.HSMSMode = HSMSMode.Active;
            this._configFileManager.Configurtion.HSMSModeConfig.RemoteIPAddress = string.Empty;
            this._configFileManager.Configurtion.HSMSModeConfig.RemotePortNo = 0;
            this._configFileManager.Configurtion.HSMSModeConfig.LocalIPAddress = string.Empty;
            this._configFileManager.Configurtion.HSMSModeConfig.LocalPortNo = 0;
            this._configFileManager.Configurtion.HSMSModeConfig.T3 = DEFAULT_T3;
            this._configFileManager.Configurtion.HSMSModeConfig.T5 = DEFAULT_T5;
            this._configFileManager.Configurtion.HSMSModeConfig.T6 = DEFAULT_T6;
            this._configFileManager.Configurtion.HSMSModeConfig.T7 = DEFAULT_T7;
            this._configFileManager.Configurtion.HSMSModeConfig.T8 = DEFAULT_T8;
            this._configFileManager.Configurtion.HSMSModeConfig.LinkTest = DEFAULT_LINKTEST;

            this._configFileManager.LoadPreDefined();

            this._messageMaker.Initialize(this._driver.Config.DeviceType,
                                          this._driver.MessageManager.Messages,
                                          this._collectionEventCollection,
                                          this._reportCollection,
                                          this._variableCollection,
                                          this._dataDictionaryCollection,
                                          this._userGEMMessageCollection,
                                          this._userMessageCollection,
                                          this._orgVariableCollection);
        }

        /// <summary>
        /// GEM Driver에 사용자 log를 추가합니다.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logData"></param>
        public void AddGemDriverLog(LogLevel logLevel, string logData)
        {
            this._logger.WriteGEM(logLevel, logData);
        }

        /// <summary>
        /// GEM Driver에 사용자 log를 추가합니다.
        /// </summary>
        /// <param name="ex"></param>
        public void AddGemDriverLog(Exception ex)
        {
            this._logger.WriteGEM(ex);
        }

        private void LicenseChecker_CheckActiveEvent(object sender, out string uniqueKey, out UbiSam.Net.KeyLock.Structure.Product product, out bool isActive)
        {
            uniqueKey = this._licenseKey;
            product = UbiSam.Net.KeyLock.Structure.Product.UbiGEM;
            isActive = true;
        }

        private void LicenseChecker_LicenseCheckEvent(object sender, string uniqueKey, UbiSam.Net.KeyLock.Structure.Product product, UbiSam.Net.KeyLock.Structure.LicenseResult result)
        {
            if (this._licenseKey == uniqueKey)
            {
                this._logger.WriteGEM(LogLevel.Information, $"License Status Changed:Status={result}");

                if (result == UbiSam.Net.KeyLock.Structure.LicenseResult.LicenseOk)
                {
                    this._licenseFailed = false;

                    if (this._isStarted == true)
                    {
                        Start();
                    }
                }
                else
                {
                    this._licenseFailed = true;

                    Stop();
                }
            }
        }

        private void LicenseChecker_OnRequestLogging(string message)
        {
            this._logger.WriteGEM(LogLevel.Information, $"License Logging:{message}");
        }

        private SECSItemFormat GetSECSFormat(PreDefinedDataDictinary dataDictinary, SECSItemFormat defaultFormat)
        {
            SECSItemFormat result;
            DataDictionaryInfo dataDictionaryInfo;

            dataDictionaryInfo = this._dataDictionaryCollection[dataDictinary.ToString()];
            result = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : defaultFormat;

            return result;
        }

        private static int GetLength(SECSItemFormat format, object value)
        {
            int result;

            if (format == SECSItemFormat.A || format == SECSItemFormat.J)
            {
                result = Encoding.Default.GetByteCount(value.ToString());
            }
            else
            {
                if (value == null)
                {
                    result = 0;
                }
                else if (value is string)
                {
                    string[] splitData;

                    splitData = value.ToString().Split(' ');

                    if (splitData != null)
                    {
                        result = splitData.Length;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    if (value is IList list)
                    {
                        result = list.Count;
                    }
                    else if (value.GetType().IsArray == true)
                    {
                        if (value is IEnumerable array)
                        {
                            int totalCount = 0;

                            foreach (var temp in array)
                            {
                                totalCount++;
                            }

                            result = totalCount;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                    else
                    {
                        result = 1;
                    }
                }
            }

            return result;
        }

        private static string GetValue(SECSItem secsItem)
        {
            string result;
            StringBuilder sb;

            if (secsItem.Format == SECSItemFormat.A || secsItem.Format == SECSItemFormat.J)
            {
                result = secsItem.Value;
            }
            else
            {
                if (secsItem.SubItem != null && secsItem.SubItem.Count > 0)
                {
                    sb = new StringBuilder();

                    foreach (var temp in secsItem.SubItem.Items)
                    {
                        sb.AppendFormat("{0} ", temp.ToString());
                    }

                    result = sb.ToString().Trim();
                }
                else if (secsItem.Value.GetValue() is IList list)
                {
                    sb = new StringBuilder();

                    foreach (var temp in list)
                    {
                        sb.AppendFormat("{0} ", temp.ToString());
                    }

                    result = sb.ToString().Trim();
                }
                else if (secsItem.Value.GetValue() is IEnumerable enumerable)
                {
                    sb = new StringBuilder();

                    foreach (var temp in enumerable)
                    {
                        sb.AppendFormat("{0} ", temp.ToString());
                    }

                    result = sb.ToString().Trim();
                }
                else
                {
                    result = secsItem.Value;
                }
            }

            return result;
        }

        private SECSItem GetSECSItem(string name, PreDefinedDataDictinary dataDictinary, object value, SECSItemFormat defaultFormat = SECSItemFormat.A)
        {
            SECSItem result;
            SECSItemFormat secsItemFormat;
            SECSValue secsValue;
            bool numericValueIsNull;

            secsItemFormat = GetSECSFormat(dataDictinary, defaultFormat);

            result = new SECSItem()
            {
                Name = name,
                Format = secsItemFormat
            };

            secsValue = new SECSValue();

            numericValueIsNull = false;

            if (value == null)
            {
                if (defaultFormat == SECSItemFormat.B ||
                    defaultFormat == SECSItemFormat.I1 ||
                    defaultFormat == SECSItemFormat.I2 ||
                    defaultFormat == SECSItemFormat.I4 ||
                    defaultFormat == SECSItemFormat.I8 ||
                    defaultFormat == SECSItemFormat.U1 ||
                    defaultFormat == SECSItemFormat.U2 ||
                    defaultFormat == SECSItemFormat.U4 ||
                    defaultFormat == SECSItemFormat.U8 ||
                    defaultFormat == SECSItemFormat.F4 ||
                    defaultFormat == SECSItemFormat.F8)
                {
                    numericValueIsNull = true;
                }
            }

            secsValue.SetValue(defaultFormat, value);

            result.Value = secsValue;

            if (numericValueIsNull == true)
            {
                result.Length = 0;
            }
            else
            {
                if (secsItemFormat == SECSItemFormat.A)
                {
                    result.Length = Encoding.Default.GetByteCount(value.ToString());
                }
                else
                {
                    if (value is IList list)
                    {
                        result.Length = list.Count;
                    }
                    else if (value.GetType().IsArray == true)
                    {
                        if (value is IEnumerable array)
                        {
                            int totalCount = 0;

                            foreach (var temp in array)
                            {
                                totalCount++;
                            }

                            result.Length = totalCount;
                        }
                        else
                        {
                            result.Length = 0;
                        }
                    }
                    else
                    {
                        result.Length = 1;
                    }
                }
            }

            return result;
        }

        private static string Substring(string source, int startIndex, int length)
        {
            try
            {
                if (string.IsNullOrEmpty(source) == false)
                {
                    Encoding encoding = Encoding.GetEncoding("euc-kr");
                    byte[] buffer = encoding.GetBytes(source);
                    int bufferLength = buffer.Length;

                    if (startIndex < 0)
                    {
                        startIndex = 0;
                    }
                    else if (startIndex > bufferLength)
                    {
                        startIndex = bufferLength;
                    }

                    if (length < 0)
                    {
                        length = 0;
                    }
                    else if (length > bufferLength - startIndex)
                    {
                        length = bufferLength - startIndex;
                    }

                    if (startIndex < bufferLength)
                    {
                        int nCopyStart = 0;
                        int nCopyLen = 0;

                        if (startIndex >= 1)
                        {
                            while (true)
                            {
                                if (buffer[nCopyStart] >= 0x80)
                                {
                                    nCopyStart++;
                                }

                                nCopyStart++;

                                if (nCopyStart >= startIndex)
                                {
                                    if (nCopyStart > startIndex)
                                    {
                                        length--;
                                    }

                                    break;
                                }
                            }
                        }

                        int index = 0;

                        while (index < length)
                        {
                            if (buffer[nCopyStart + index] >= 0x80)
                            {
                                index++;
                            }

                            index++;
                        }

                        nCopyLen = (index <= length) ? index : index - 2;

                        if (nCopyLen >= 1)
                        {
                            return encoding.GetString(buffer, nCopyStart, nCopyLen);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return string.Empty;
        }

        private static SECSValue MakeSECSValue(SECSItemFormat format, string stringValue)
        {
            SECSValue result;
            List<string> splitData;
            int length;
            bool success;

            result = null;

            if (string.IsNullOrEmpty(stringValue) == true)
            {
                result = new SECSValue();
            }
            else if (format == SECSItemFormat.A || format == SECSItemFormat.J)
            {
                result = new SECSValue(stringValue);
            }
            else
            {
                splitData = (from string temp in stringValue.Split(' ')
                             where string.IsNullOrEmpty(temp) == false
                             select temp).ToList();

                if (splitData != null && splitData.Count > 1)
                {
                    length = splitData.Count;
                    success = true;

                    #region [List] 
                    switch (format)
                    {
                        case SECSItemFormat.I1:
                            {
                                sbyte[] values = new sbyte[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (sbyte.TryParse(splitData[i], out sbyte value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.I2:
                            {
                                short[] values = new short[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (short.TryParse(splitData[i], out short value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.I4:
                            {
                                int[] values = new int[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (int.TryParse(splitData[i], out int value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.I8:
                            {
                                long[] values = new long[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (long.TryParse(splitData[i], out long value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.U1:
                            {
                                byte[] values = new byte[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (byte.TryParse(splitData[i], out byte value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.U2:
                            {
                                ushort[] values = new ushort[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (ushort.TryParse(splitData[i], out ushort value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.U4:
                            {
                                uint[] values = new uint[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (uint.TryParse(splitData[i], out uint value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.U8:
                            {
                                ulong[] values = new ulong[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (ulong.TryParse(splitData[i], out ulong value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.F4:
                            {
                                float[] values = new float[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (float.TryParse(splitData[i], out float value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    result = new SECSValue(values);
                                }
                            }
                            break;
                        case SECSItemFormat.F8:
                            {
                                double[] values = new double[length];
                                Array.Clear(values, 0, values.Length);

                                for (int i = 0; i < splitData.Count; i++)
                                {
                                    if (double.TryParse(splitData[i], out double value) == true)
                                    {
                                        values[i] = value;
                                    }
                                    else
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success == true)
                                {
                                    //result = new SECSValue(values);
                                    result = new SECSValue();
                                    result = values;
                                }
                            }
                            break;
                        default:
                            result = new SECSValue(stringValue);
                            break;
                    }
                    #endregion
                }
                else
                {
                    result = new SECSValue(stringValue);
                }
            }

            return result;
        }

        private void LoadStandardLibrary()
        {
            XElement messageElement;
            string errorText;

            try
            {
                messageElement = XElement.Load(new System.IO.StringReader(Resources.XML.StandardMessageStructure));

                errorText = this._driver.MessageManager.Load(messageElement);

                if (string.IsNullOrEmpty(errorText) == false)
                {
                    this._logger.WriteGEM(LogLevel.Error, errorText);
                }
                else
                {
                    this._standardMessageCollection = this._driver.MessageManager.Messages;
                }
            }
            catch (Exception ex)
            {
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }
        }

        private void CallbackEstablishCommunication(object arg)
        {
            string logText;

            EstablishCommunication();

            logText = "Establish Communication Timeout:CommDelay";

            this._logger.WriteGEM(LogLevel.Information, logText);
        }

        private void CallbackAreYouThereTimeout(object arg)
        {
            string logText;

            logText = "Are you there Timeout";

            this._logger.WriteGEM(LogLevel.Information, logText);

            FailAreYouThere();
        }

        void FireOnWriteLog(Utility.Logger.LogLevel logLevel, string logText)
        {
            this.OnWriteLog?.Invoke(logLevel, logText);
        }

        void FireOnSECS1Log(Utility.Logger.LogLevel logLevel, string logText)
        {
            this.OnSECS1Log?.Invoke(logLevel, logText);
        }

        void FireOnSECS2Log(Utility.Logger.LogLevel logLevel, string logText)
        {
            this.OnSECS2Log?.Invoke(logLevel, logText);
        }

        private void FireOnConnectionStateChanged(string ipAddress, int portNo)
        {
            this.OnGEMConnected?.Invoke(ipAddress, portNo);
        }

        private void FireOnDisconnectionStateChanged(string ipAddress, int portNo)
        {
            this.OnGEMDisconnected?.Invoke(ipAddress, portNo);
        }

        private void FireOnGEMSelected(string ipAddress, int portNo)
        {
            this.OnGEMSelected?.Invoke(ipAddress, portNo);
        }

        private void FireOnGEMDeselected(string ipAddress, int portNo)
        {
            this.OnGEMDeselected?.Invoke(ipAddress, portNo);
        }

        private void FireOnCommunicationStateChanged(CommunicationState communicationState)
        {
            this.OnCommunicationStateChanged?.Invoke(communicationState);
        }

        private static void CheckKeyLockDLL()
        {
            try
            {
                List<string> dlls = new List<string>() { "ubisam_mega32.dll", "ubisam_mega64.dll", "UbiSam.Net.KeyLock.Mega64.dll", "UbiSam.Net.KeyLock.Mega86.dll" };
                List<string> noFileList = new List<string>();

                foreach (string tempFileName in dlls)
                {
                    if (System.IO.File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}{tempFileName}") == false)
                    {
                        noFileList.Add(tempFileName);
                    }
                }

                if (noFileList.Count > 0)
                {
                    string installPath = string.Empty;
                    RegistryKey localMachine = Registry.LocalMachine;

                    RegistryKey software = localMachine.OpenSubKey(@"SOFTWARE");
                    RegistryKey manufacturer = software.OpenSubKey(@"UbiSam");

                    if (manufacturer != null)
                    {
                        RegistryKey productName = manufacturer.OpenSubKey(@"UbiGEM");

                        if (productName != null)
                        {
                            object regInstallPath = productName.GetValue("InstallPath");

                            if (regInstallPath == null || string.IsNullOrEmpty(regInstallPath.ToString()) == true)
                            {
                                return;
                            }

                            installPath = regInstallPath.ToString();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                        {
                            using (RegistryKey key = rootKey.OpenSubKey(@"SOFTWARE\UbiSam\UbiGEM", false))
                            {
                                if (key != null)
                                {
                                    object registInstallPath = key.GetValue("InstallPath");

                                    if (registInstallPath != null && string.IsNullOrEmpty(registInstallPath.ToString()) == false)
                                    {
                                        installPath = registInstallPath.ToString();
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(installPath) == false)
                    {
                        foreach (string temp in noFileList)
                        {
                            string destFile = $@"{AppDomain.CurrentDomain.BaseDirectory}{temp}";

                            if (System.IO.File.Exists(destFile) == false)
                            {
                                string sourceFile = $@"{installPath}\BIN\{temp}";

                                if (System.IO.File.Exists(sourceFile) == true)
                                {
                                    System.IO.File.Copy(sourceFile, destFile);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }
    }
}