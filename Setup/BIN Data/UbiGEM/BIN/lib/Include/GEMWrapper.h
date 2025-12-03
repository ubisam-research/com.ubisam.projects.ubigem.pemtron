#pragma once

#ifndef UBISAM_GEM_API
#define UBISAM_GEM_API __declspec(dllimport)
#else
#undef UBISAM_GEM_API
#define UBISAM_GEM_API __declspec(dllexport)
#endif

#include <tchar.h>

#include "GEMStructure.h"

namespace UbiGEMWrapper
{
    class GEMAdapter;

    class IGEMDriverEvents
    {
        // Connection and State Changed Events
    public:
        virtual void OnGEMConnected(LPCTSTR strIpAddress, int nPortNo) {};
        virtual void OnGEMDisconnected(LPCTSTR strIpAddress, int nPortNo) {};
        virtual void OnGEMSelected(LPCTSTR strIpAddress, int nPortNo) {};
        virtual void OnGEMDeselected(LPCTSTR strIpAddress, int nPortNo) {};

        virtual void OnSECSTimeout(UbiGEMWrapper::Structure::SECSTimeoutType timeoutType) {};
        virtual void OnSECST3Timeout(UbiGEMWrapper::Structure::SECSMessage* message) {};

        virtual void OnCommunicationStateChanged(UbiGEMWrapper::Structure::GEMCommunicationState communicationState) {};
        virtual void OnControlStateChanged(UbiGEMWrapper::Structure::GEMControlState controlState) {};
        virtual void OnControlStateOnlineChangeFailed() {};
        virtual void OnEquipmentProcessState(uint8_t equipmentProcessState) {};
        virtual void OnSpoolStateChanged(UbiGEMWrapper::Structure::GEMSpoolState spoolState) {};

        // Primary Message Received Events
    public:
        virtual int OnReceivedEstablishCommunicationsRequest(LPCTSTR strMDLN, LPCTSTR strSOFTREV) { return 0; };
        virtual void OnReceivedRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo) {};
        virtual void OnReceivedEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo) {};
        virtual void OnReceivedNewECVSend(uint32_t systemBytes, UbiGEMWrapper::Structure::VariableCollection* newEcInfo) {};
        virtual void OnReceivedLoopback(UbiGEMWrapper::Structure::List<uint8_t>* receiveData) {};
        virtual void OnReceivedTerminalMessage(uint32_t systemBytes, int tid, LPCTSTR strTerminalMessage) {};
        virtual void OnReceivedTerminalMultiMessage(uint32_t systemBytes, int tid, UbiGEMWrapper::Structure::List<LPCTSTR>* strTerminalMessages) {};
        virtual void OnReceivedRequestOffline(uint32_t systemBytes) {};
        virtual void OnReceivedRequestOnline(uint32_t systemBytes) {};
        virtual void OnReceivedDefineReport() {};
        virtual void OnReceivedLinkEventReport() {};
        virtual void OnReceivedEnableDisableEventReport() {};
        virtual void OnReceivedEnableDisableAlarmSend() {};
        virtual void OnReceivedPPLoadInquire(uint32_t systemBytes, LPCTSTR strPPID, int length) {};
        virtual void OnReceivedPPSend(uint32_t systemBytes, LPCTSTR strPPID, UbiGEMWrapper::Structure::List<uint8_t>* ppbody) {};
        virtual void OnReceivedFmtPPSend(uint32_t systemBytes, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection) {};
        virtual void OnReceivedDateTimeRequest(uint32_t systemBytes, UbiGEMWrapper::Structure::DateTime* timeData) {};
        virtual void OnReceivedDateTimeSetRequest(uint32_t systemBytes, UbiGEMWrapper::Structure::DateTime* timeData) {};
        virtual void OnReceivedPPRequest(uint32_t systemBytes, LPCTSTR strPPID) {};
        virtual void OnReceivedFmtPPRequest(uint32_t systemBytes, LPCTSTR strPPID) {};
        virtual void OnReceivedDeletePPSend(uint32_t systemBytes, UbiGEMWrapper::Structure::List<LPCTSTR>* ppids) {};
        virtual void OnReceivedCurrentEPPDRequest(uint32_t systemBytes) {};
        virtual void OnUserPrimaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* message) {};

        // Secondary Message Received Events
    public:
        virtual bool OnResponseDateTimeRequest(UbiGEMWrapper::Structure::DateTime* timeData) { return true; };
        virtual void OnResponseLoopback(UbiGEMWrapper::Structure::List<uint8_t>* receiveData, UbiGEMWrapper::Structure::List<uint8_t>* sendData) {};
        virtual void OnResponseEventReportAcknowledge(LPCTSTR strCEID, int ack) {};
        virtual void OnResponsePPLoadInquire(int ppgnt, LPCTSTR strPPID) {};
        virtual void OnResponsePPSend(int ack, LPCTSTR strPPID) {};
        virtual void OnResponsePPRequest(LPCTSTR strPPID, UbiGEMWrapper::Structure::List<uint8_t>* ppbody) {};
        virtual void OnResponseFmtPPSend(int ack, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection) {};
        virtual void OnResponseFmtPPRequest(UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection) {};
        virtual void OnResponseFmtPPVerification(UbiGEMWrapper::Structure::FmtPPVerificationCollection* fmtPPVerificationCollection) {};
        virtual void OnResponseTerminalRequest(int ack) {};
        virtual void OnUserSecondaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* primaryMessage, UbiGEMWrapper::Structure::SECSMessage* secondaryMessage) {};

        // Variable Value Update Events
    public:
        virtual void OnVariableUpdateRequest(UbiGEMWrapper::Structure::GEMVariableUpdateType updateType, UbiGEMWrapper::Structure::List<LPCTSTR>* strVIDs) {};
        virtual void OnTraceDataUpdateRequest(UbiGEMWrapper::Structure::List<LPCTSTR>* strVIDs) {};

        // ETC Events
    public:
        virtual void OnUserGEMMessageUpdateRequest(UbiGEMWrapper::Structure::SECSMessage* message) {};

        // Invalid or Unknown Message Receive Events
    public:
        virtual void OnInvalidMessageReceived(UbiGEMWrapper::Structure::GEMMessageValidationError error, UbiGEMWrapper::Structure::SECSMessage* message) {};
        virtual void OnReceivedUnknownMessage(UbiGEMWrapper::Structure::SECSMessage* message) {};
        virtual void OnReceivedInvalidRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo) {};
        virtual void OnReceivedInvalidEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo) {};
    };

    UBISAM_GEM_API LPCTSTR SECSItemFormatAsString(Structure::GEMSECSFormat format);

    class GEMWrapper
    {
    public:
        UBISAM_GEM_API GEMWrapper();
        UBISAM_GEM_API ~GEMWrapper();

        // Initialize, Start and Stop
    public:
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult Initialize(LPCTSTR strUgcFileName);
        UBISAM_GEM_API LPCTSTR GetInitializeError();
        UBISAM_GEM_API void Terminate();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult Start();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult Stop();

        // Get State of Driver
    public:
        UBISAM_GEM_API bool GetConnected();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMCommunicationState GetCommunicationState();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMControlState GetControlState();

        // Send Primary Message
    public:
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult EstablishCommunication();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestAreYouThere();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestOffline();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestOnlineLocal();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestOnlineRemote();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestHostOffline();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestLoopback(UbiGEMWrapper::Structure::List<uint8_t>* abs);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestDateTime();
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestPPLoadInquire(LPCTSTR strPPID, int length);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestPPSend(LPCTSTR strPPID, LPCTSTR strFileName);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestPPSend(LPCTSTR strPPID, UbiGEMWrapper::Structure::List<uint8_t>* ppbody);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestPPRequest(LPCTSTR strPPID);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestPPChanged(int64_t changeStatus, LPCTSTR strPPID);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestFmtPPSend(UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestFmtPPSendWithoutValue(UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestFmtPPRequest(LPCTSTR strPPID);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestFmtPPVerificationSend(UbiGEMWrapper::Structure::FmtPPVerificationCollection* fmtPPVerificationCollection);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult RequestFmtPPChanged(int64_t changeStatus, LPCTSTR strPPID);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportEquipmentProcessingState(int equipmentProcessState);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportCollectionEvent(LPCTSTR strCEID, bool bUseRaiseEvent = true);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportCollectionEvent(LPCTSTR strCEID, UbiGEMWrapper::Structure::List<LPCTSTR>* vids, UbiGEMWrapper::Structure::List<LPCTSTR>* values, bool bUseRaiseEvent = true);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportCollectionEvent(LPCTSTR strCEID, UbiGEMWrapper::Structure::List<UbiGEMWrapper::Structure::VariableInfo *>* variables, bool bUseRaiseEvent = true);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportCollectionEvent(LPCTSTR strCEID, UbiGEMWrapper::Structure::SECSItemCollection* itemCollection);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportCollectionEvent(UbiGEMWrapper::Structure::CollectionEventInfo* collectionEventInfo, bool bUseRaiseEvent = true);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportAlarmSet(int64_t alarmId);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportAlarmSet(int64_t alarmId, int alarmCode, LPCTSTR alarmText);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportAlarmClear(int64_t alarmId);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportAlarmClear(int64_t alarmId, int alarmCode, LPCTSTR alarmText);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportTerminalMessage(int tid, LPCTSTR strText);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReportConversationTimeout(LPCTSTR strMexp, LPCTSTR strEdid);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SendSECSMessage(UbiGEMWrapper::Structure::SECSMessage* message);

        // Send Secondary Message
    public:
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyRemoteCommandAck(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo, UbiGEMWrapper::Structure::RemoteCommandResult* remoteCommandResult);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyEnhancedRemoteCommandAck(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo, UbiGEMWrapper::Structure::RemoteCommandResult* remoteCommandResult);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyRequestOfflineAck(uint32_t systemBytes, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyRequestOnlineAck(uint32_t systemBytes, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyNewEquipmentConstantSend(uint32_t systemBytes, UbiGEMWrapper::Structure::VariableCollection* newVariableCollection, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyDateTimeRequest(uint32_t systemBytes, UbiGEMWrapper::Structure::DateTime* timeData);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyDateTimeSetRequest(uint32_t systemBytes, int ack, UbiGEMWrapper::Structure::DateTime* targetTime);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyPPLoadInquireAck(uint32_t systemBytes, int processProgramGrantStatus);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyPPSendAck(uint32_t systemBytes, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyPPRequestAck(uint32_t systemBytes, LPCTSTR strPPID, UbiGEMWrapper::Structure::List<uint8_t>* ppbody, bool ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyPPDeleteAck(uint32_t systemBytes, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyCurrentEPPDRequestAck(uint32_t systemBytes, UbiGEMWrapper::Structure::List<LPCTSTR>* strPPIDs, bool ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyFmtPPSendAck(uint32_t systemBytes, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyFmtPPRequestAck(uint32_t systemBytes, LPCTSTR strPPID, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection, bool ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyTerminalMessageAck(uint32_t systemBytes, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplyTerminalMultiMessageAck(uint32_t systemBytes, int ack);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult ReplySECSMessage(UbiGEMWrapper::Structure::SECSMessage* primaryMessage, UbiGEMWrapper::Structure::SECSMessage* secondaryMessage);

        // Create and Delete Instance
    public:
        UBISAM_GEM_API UbiGEMWrapper::Structure::List<bool>* CreateBoolList();
        UBISAM_GEM_API void DeleteBoolList(UbiGEMWrapper::Structure::List<bool>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<int8_t>* CreateI1List();
        UBISAM_GEM_API void DeleteI1List(UbiGEMWrapper::Structure::List<int8_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<int16_t>* CreateI2List();
        UBISAM_GEM_API void DeleteI2List(UbiGEMWrapper::Structure::List<int16_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<int32_t>* CreateI4List();
        UBISAM_GEM_API void DeleteI4List(UbiGEMWrapper::Structure::List<int32_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<int64_t>* CreateI8List();
        UBISAM_GEM_API void DeleteI8List(UbiGEMWrapper::Structure::List<int64_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<uint8_t>* CreateU1List();
        UBISAM_GEM_API void DeleteU1List(UbiGEMWrapper::Structure::List<uint8_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<uint16_t>* CreateU2List();
        UBISAM_GEM_API void DeleteU2List(UbiGEMWrapper::Structure::List<uint16_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<uint32_t>* CreateU4List();
        UBISAM_GEM_API void DeleteU4List(UbiGEMWrapper::Structure::List<uint32_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<uint64_t>* CreateU8List();
        UBISAM_GEM_API void DeleteU8List(UbiGEMWrapper::Structure::List<uint64_t>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<float>* CreateF4List();
        UBISAM_GEM_API void DeleteF4List(UbiGEMWrapper::Structure::List<float>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<double>* CreateF8List();
        UBISAM_GEM_API void DeleteF8List(UbiGEMWrapper::Structure::List<double>* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::List<LPCTSTR>* CreateStringList();
        UBISAM_GEM_API void DeleteStringList(UbiGEMWrapper::Structure::List<LPCTSTR>* p, bool bItemDelete = true);

        UBISAM_GEM_API UbiGEMWrapper::Structure::VariableCollection* GetAllVariable(bool bRebuild = false);

        UBISAM_GEM_API UbiGEMWrapper::Structure::VariableInfo* CreateVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::GEMSECSFormat format, LPCTSTR strName = nullptr, UbiGEMWrapper::Structure::GEMVariableType vidType = UbiGEMWrapper::Structure::GEMVariable_SV, LPCTSTR strDescription = nullptr);
        UBISAM_GEM_API UbiGEMWrapper::Structure::VariableInfo* GetVariable(LPCTSTR strVID);
        UBISAM_GEM_API void DeleteVariableInfo(UbiGEMWrapper::Structure::VariableInfo* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::CollectionEventCollection* GetAllCollectionEventInfo(bool bRebuild = false);

        UBISAM_GEM_API UbiGEMWrapper::Structure::CollectionEventInfo* CreateCollectionEventInfo(LPCTSTR strCEID);
        UBISAM_GEM_API UbiGEMWrapper::Structure::CollectionEventInfo* GetCollectionEventInfo(LPCTSTR strCEID);
        UBISAM_GEM_API void DeleteCollectionEventInfo(UbiGEMWrapper::Structure::CollectionEventInfo* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::ReportInfo* CreateReportInfo(LPCTSTR strRPTID);
        UBISAM_GEM_API UbiGEMWrapper::Structure::ReportInfo* GetReportInfo(LPCTSTR strRPTID);
        UBISAM_GEM_API void DeleteReportInfo(UbiGEMWrapper::Structure::ReportInfo* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::SECSMessageCollection* GetAllUserMessage(bool bRebuild = false);

        UBISAM_GEM_API UbiGEMWrapper::Structure::SECSMessage* CreateUserMessage(LPCTSTR strName, int stream, int function, bool bWaitBit = true, bool bToHost = true);
        UBISAM_GEM_API UbiGEMWrapper::Structure::SECSMessage* GetUserMessage(int stream, int function, bool bToHost = true);
        UBISAM_GEM_API void DeleteSECSMessage(UbiGEMWrapper::Structure::SECSMessage* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::FmtPPCollection* CreateFmtPPCollection(LPCTSTR strPPID, LPCTSTR strMDLN = nullptr, LPCTSTR strSOFTREV = nullptr);
        UBISAM_GEM_API void DeleteFmtPPCollection(UbiGEMWrapper::Structure::FmtPPCollection* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::FmtPPCCodeInfo* CreateFmtPPCCodeInfo(LPCTSTR strCommandCode);
        UBISAM_GEM_API void DeleteFmtPPCCodeInfo(UbiGEMWrapper::Structure::FmtPPCCodeInfo* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::FmtPPItem* CreateFmtPPItem(LPCTSTR strPPName, LPCTSTR strPPValue, UbiGEMWrapper::Structure::GEMSECSFormat eFormat);
        UBISAM_GEM_API void DeleteFmtPPItem(UbiGEMWrapper::Structure::FmtPPItem* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::FmtPPVerificationInfo* CreateFmtPPVerificationInfo(int nACK, int nSeqNum, LPCTSTR strErrW7);
        UBISAM_GEM_API void DeleteFmtPPVerificationInfo(UbiGEMWrapper::Structure::FmtPPVerificationInfo* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::FmtPPVerificationCollection* CreateFmtPPVerificationCollection(LPCTSTR strPPID);
        UBISAM_GEM_API void DeleteFmtPPVerificationCollection(UbiGEMWrapper::Structure::FmtPPVerificationCollection* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::RemoteCommandResult* CreateRemoteCommandResult(int nHostCommandAck = 0);
        UBISAM_GEM_API void DeleteRemoteCommandResult(UbiGEMWrapper::Structure::RemoteCommandResult* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::RemoteCommandParameterResult* CreateRemoteCommandParameterResult(LPCTSTR strCPName, int nParameterAck);
        UBISAM_GEM_API void DeleteRemoteCommandParameterResult(UbiGEMWrapper::Structure::RemoteCommandParameterResult* p);

        UBISAM_GEM_API UbiGEMWrapper::Structure::CustomVariableInfo* CreateCustomVariableInfo();
        UBISAM_GEM_API void DeleteCustomVariableInfo(UbiGEMWrapper::Structure::CustomVariableInfo* customVariableInfo);

        // Set Variable and Equipment Constant Value
    public:
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, bool newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, uint8_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, uint16_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, uint32_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, uint64_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, int8_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, int16_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, int32_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, int64_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, float newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, double newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, LPCTSTR newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<bool>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<uint8_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<uint16_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<uint32_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<uint64_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<int8_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<int16_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<int32_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<int64_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<float>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::List<double>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(UbiGEMWrapper::Structure::VariableInfo* variableInfo);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(UbiGEMWrapper::Structure::VariableCollection* variableInfos);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetVariable(LPCTSTR strVID, UbiGEMWrapper::Structure::CustomVariableInfo* customVariableInfo);

        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, bool newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, uint8_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, uint16_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, uint32_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, uint64_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, int8_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, int16_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, int32_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, int64_t newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, float newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, double newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, LPCTSTR newValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<bool>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<uint8_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<uint16_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<uint32_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<uint64_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<int8_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<int16_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<int32_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<int64_t>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<float>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(LPCTSTR strECID, UbiGEMWrapper::Structure::List<double>* arrayValue);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(UbiGEMWrapper::Structure::VariableInfo* variableInfo);
        UBISAM_GEM_API UbiGEMWrapper::Structure::GEMResult SetEquipmentConstant(UbiGEMWrapper::Structure::VariableCollection* variableInfos);

        // Subscribe Events
    public:
        UBISAM_GEM_API void SubscribeInitilaize(IGEMDriverEvents* eventReceiver);

		// Connection and State Changed Events
	public:
		UBISAM_GEM_API void SubscribeGEMConnected();
        UBISAM_GEM_API void SubscribeGEMDisconnected();
        UBISAM_GEM_API void SubscribeGEMSelected();
        UBISAM_GEM_API void SubscribeGEMDeselected();

        UBISAM_GEM_API void SubscribeSECSTimeout();
        UBISAM_GEM_API void SubscribeSECST3Timeout();

        UBISAM_GEM_API void SubscribeCommunicationStateChanged();
        UBISAM_GEM_API void SubscribeControlStateChanged();
        UBISAM_GEM_API void SubscribeControlStateOnlineChangeFailed();
        UBISAM_GEM_API void SubscribeEquipmentProcessState();
        UBISAM_GEM_API void SubscribeSpoolStateChanged();

        // Primary Message Received Events
    public:
        UBISAM_GEM_API void SubscribeReceivedEstablishCommunicationsRequest();
        UBISAM_GEM_API void SubscribeReceivedRemoteCommand();
        UBISAM_GEM_API void SubscribeReceivedEnhancedRemoteCommand();
        UBISAM_GEM_API void SubscribeReceivedNewECVSend();
        UBISAM_GEM_API void SubscribeReceivedLoopback();
        UBISAM_GEM_API void SubscribeReceivedTerminalMessage();
        UBISAM_GEM_API void SubscribeReceivedTerminalMultiMessage();
        UBISAM_GEM_API void SubscribeReceivedRequestOffline();
        UBISAM_GEM_API void SubscribeReceivedRequestOnline();
        UBISAM_GEM_API void SubscribeReceivedDefineReport();
        UBISAM_GEM_API void SubscribeReceivedLinkEventReport();
        UBISAM_GEM_API void SubscribeReceivedEnableDisableEventReport();
        UBISAM_GEM_API void SubscribeReceivedEnableDisableAlarmSend();
        UBISAM_GEM_API void SubscribeReceivedPPLoadInquire();
        UBISAM_GEM_API void SubscribeReceivedPPSend();
        UBISAM_GEM_API void SubscribeReceivedFmtPPSend();
        UBISAM_GEM_API void SubscribeReceivedDateTimeRequest();
        UBISAM_GEM_API void SubscribeReceivedDateTimeSetRequest();
        UBISAM_GEM_API void SubscribeReceivedPPRequest();
        UBISAM_GEM_API void SubscribeReceivedFmtPPRequest();
        UBISAM_GEM_API void SubscribeReceivedDeletePPSend();
        UBISAM_GEM_API void SubscribeReceivedCurrentEPPDRequest();
        UBISAM_GEM_API void SubscribeUserPrimaryMessageReceived();

        // Secondary Message Received Events
    public:
        UBISAM_GEM_API void SubscribeResponseDateTimeRequest();
        UBISAM_GEM_API void SubscribeResponseLoopback();
        UBISAM_GEM_API void SubscribeResponseEventReportAcknowledge();
        UBISAM_GEM_API void SubscribeResponsePPLoadInquire();
        UBISAM_GEM_API void SubscribeResponsePPSend();
        UBISAM_GEM_API void SubscribeResponsePPRequest();
        UBISAM_GEM_API void SubscribeResponseFmtPPSend();
        UBISAM_GEM_API void SubscribeResponseFmtPPRequest();
        UBISAM_GEM_API void SubscribeResponseFmtPPVerification();
        UBISAM_GEM_API void SubscribeResponseTerminalRequest();
        UBISAM_GEM_API void SubscribeUserSecondaryMessageReceived();

        // Variable Value Update Events
    public:
        UBISAM_GEM_API void SubscribeVariableUpdateRequest();
        UBISAM_GEM_API void SubscribeTraceDataUpdateRequest();

        // ETC Events
    public:
        UBISAM_GEM_API void SubscribeUserGEMMessageUpdateRequest();

        // Invalid or Unknown Message Receive Events
    public:
        UBISAM_GEM_API void SubscribeInvalidMessageReceived();
        UBISAM_GEM_API void SubscribeReceivedUnknownMessage();
        UBISAM_GEM_API void SubscribeReceivedInvalidRemoteCommand();
        UBISAM_GEM_API void SubscribeReceivedInvalidEnhancedRemoteCommand();

    private:
        void OnGEMConnected(LPCTSTR strIpAddress, int nPortNo);
        void OnGEMDisconnected(LPCTSTR strIpAddress, int nPortNo);
        void OnGEMSelected(LPCTSTR strIpAddress, int nPortNo);
        void OnGEMDeselected(LPCTSTR strIpAddress, int nPortNo);

        void OnSECSTimeout(UbiGEMWrapper::Structure::SECSTimeoutType timeoutType);
        void OnSECST3Timeout(UbiGEMWrapper::Structure::SECSMessage* message);

        void OnCommunicationStateChanged(UbiGEMWrapper::Structure::GEMCommunicationState communicationState);
        void OnControlStateChanged(UbiGEMWrapper::Structure::GEMControlState controlState);
        void OnControlStateOnlineChangeFailed();
        void OnEquipmentProcessState(uint8_t equipmentProcessState);
        void OnSpoolStateChanged(UbiGEMWrapper::Structure::GEMSpoolState spoolState);

	private:
        int OnReceivedEstablishCommunicationsRequest(LPCTSTR strMdln, LPCTSTR strSofRev);
        void OnReceivedRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo);
        void OnReceivedEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo);
        void OnReceivedNewECVSend(uint32_t systemBytes, UbiGEMWrapper::Structure::VariableCollection* newEcInfo);
        void OnReceivedLoopback(UbiGEMWrapper::Structure::List<uint8_t>* receiveData);
        void OnReceivedTerminalMessage(uint32_t systemBytes, int tid, LPCTSTR strTerminalMessage);
        void OnReceivedTerminalMultiMessage(uint32_t systemBytes, int tid, UbiGEMWrapper::Structure::List<LPCTSTR>* strTerminalMessages);
        void OnReceivedRequestOffline(uint32_t systemBytes);
        void OnReceivedRequestOnline(uint32_t systemBytes);
        void OnReceivedDefineReport();
        void OnReceivedLinkEventReport();
        void OnReceivedEnableDisableEventReport();
        void OnReceivedEnableDisableAlarmSend();
        void OnReceivedPPLoadInquire(uint32_t systemBytes, LPCTSTR strPPID, int length);
        void OnReceivedPPSend(uint32_t systemBytes, LPCTSTR strPPID, UbiGEMWrapper::Structure::List<uint8_t>* ppbody);
        void OnReceivedFmtPPSend(uint32_t systemBytes, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
        void OnReceivedDateTimeRequest(uint32_t systemBytes, UbiGEMWrapper::Structure::DateTime* timeData);
        void OnReceivedDateTimeSetRequest(uint32_t systemBytes, UbiGEMWrapper::Structure::DateTime* timeData);
        void OnReceivedPPRequest(uint32_t systemBytes, LPCTSTR strPPID);
        void OnReceivedFmtPPRequest(uint32_t systemBytes, LPCTSTR strPPID);
        void OnReceivedDeletePPSend(uint32_t systemBytes, UbiGEMWrapper::Structure::List<LPCTSTR>* ppids);
        void OnReceivedCurrentEPPDRequest(uint32_t systemBytes);
        void OnUserPrimaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* message);

	private:
        bool OnResponseDateTimeRequest(UbiGEMWrapper::Structure::DateTime* timeData);
        void OnResponseLoopback(UbiGEMWrapper::Structure::List<uint8_t>* receiveData, UbiGEMWrapper::Structure::List<uint8_t>* sendData);
        void OnResponseEventReportAcknowledge(LPCTSTR strCEID, int ack);
        void OnResponsePPLoadInquire(int ppgnt, LPCTSTR strPPID);
        void OnResponsePPSend(int ack, LPCTSTR strPPID);
        void OnResponsePPRequest(LPCTSTR strPPID, UbiGEMWrapper::Structure::List<uint8_t>* ppbody);
        void OnResponseFmtPPSend(int ack, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
        void OnResponseFmtPPRequest(UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
        void OnResponseFmtPPVerification(UbiGEMWrapper::Structure::FmtPPVerificationCollection* fmtPPVerificationCollection);
        void OnResponseTerminalRequest(int ack);
        void OnUserSecondaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* primaryMessage, UbiGEMWrapper::Structure::SECSMessage* secondaryMessage);

	private:
        void OnVariableUpdateRequest(UbiGEMWrapper::Structure::GEMVariableUpdateType updateType, UbiGEMWrapper::Structure::List<LPCTSTR>* vids);
        void OnTraceDataUpdateRequest(UbiGEMWrapper::Structure::List<LPCTSTR>* vids);

	private:
        void OnUserGEMMessageUpdateRequest(UbiGEMWrapper::Structure::SECSMessage* message);

	private:
        void OnInvalidMessageReceived(UbiGEMWrapper::Structure::GEMMessageValidationError error, UbiGEMWrapper::Structure::SECSMessage* message);
        void OnReceivedUnknownMessage(UbiGEMWrapper::Structure::SECSMessage* message);
        void OnReceivedInvalidRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo);
        void OnReceivedInvalidEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo);

    private:
        IGEMDriverEvents* m_pGEMEvent;
        UbiGEMWrapper::GEMAdapter* m_pAdapter;
        LPCTSTR m_strInitializeError;
        UbiGEMWrapper::Structure::VariableCollection* m_pAllVariables;
        UbiGEMWrapper::Structure::CollectionEventCollection* m_pAllCollectionEvents;
        UbiGEMWrapper::Structure::SECSMessageCollection* m_pAllUserMessages;
        void ClearData();
    };
}