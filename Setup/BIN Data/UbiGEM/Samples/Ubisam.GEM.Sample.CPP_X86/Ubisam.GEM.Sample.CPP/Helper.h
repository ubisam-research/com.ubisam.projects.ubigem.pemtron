#pragma once

#ifndef __HELPER_HEADER
#define __HELPER_HEADER

#define HELPER_CEID_MaterialRemoved _bstr_t("10") // Material Removed
#define HELPER_CEID_AlarmSetBase _bstr_t("10000") // Alarm Set
#define HELPER_CEID_OnlineRemote _bstr_t("10001") // Online Remote
#define HELPER_CEID_OfflineChangeRequest _bstr_t("10002") // Offline Change Request
#define HELPER_CEID_Offline _bstr_t("10003") // Offline
#define HELPER_CEID_MessageRecognition _bstr_t("10004") // Message Recognition
#define HELPER_CEID_EquipmentConstantChanged _bstr_t("10005") // Equipment Constant Changed
#define HELPER_CEID_ModelChangeStateChangedReport _bstr_t("10006") // Model Change State Changed Report
#define HELPER_CEID_MaterialInputRequest _bstr_t("10007") // Material Input Request
#define HELPER_CEID_MaterialInputCompletedReport _bstr_t("10008") // Material Input Completed Report
#define HELPER_CEID_MaterialIDReport _bstr_t("10009") // Material ID Report
#define HELPER_CEID_ProcessProgramChanged _bstr_t("10010") // Process Program Changed
#define HELPER_CEID_ProcessStateChanged _bstr_t("10011") // Process State Changed
#define HELPER_CEID_ProcessProgramSeleted _bstr_t("10012") // Process Program Seleted
#define HELPER_CEID_MaterialProcessStartedReport _bstr_t("10013") // Material Process Started Report
#define HELPER_CEID_ArrayMaterialProcessCompletedReport _bstr_t("10014") // Array Material Process Completed Report 
#define HELPER_CEID_FeedForwardDataSendCompletedReport _bstr_t("10015") // FeedForward Data Send Completed Report
#define HELPER_CEID_FeedBackDataSendCompletedReport _bstr_t("10016") // FeedBack Data Send Completed Report
#define HELPER_CEID_MaterialProcessCompletedReport _bstr_t("10017") // Material Process Completed Report
#define HELPER_CEID_MaterialActualProcessDataReport _bstr_t("10018") // Material Actual Process Data Report
#define HELPER_CEID_NGBufferMaterialInputCompletedReport _bstr_t("10019") // NG Buffer Material Input Completed Report
#define HELPER_CEID_NGBufferMaterialProcessCompletedReport _bstr_t("10020") // NG Buffer Material Process Completed Report
#define HELPER_CEID_NGBufferMaterialActualProcessDataReport _bstr_t("10021") // NG Buffer Material Actual Process Data Report
#define HELPER_CEID_MaterialOutputRequest _bstr_t("10022") // Material Output Request
#define HELPER_CEID_MaterialOutputCompletedReport _bstr_t("10023") // Material Output Completed Report
#define HELPER_CEID_PortStateChangedReport _bstr_t("10024") // Port State Changed Report
#define HELPER_CEID_PermittedReport _bstr_t("10025") // Permitted Report
#define HELPER_CEID_EquipmentOperationModeChangedReport _bstr_t("10026") // Equipment Operation Mode Changed Report
#define HELPER_CEID_CanceledReport _bstr_t("10027") // Canceled Report 
#define HELPER_CEID_PausedReport _bstr_t("10028") // Paused Report
#define HELPER_CEID_ResumedReport _bstr_t("10029") // Resumed Report
#define HELPER_CEID_AbortedReport _bstr_t("10030") // Aborted Report
#define HELPER_CEID_ScrappedReport _bstr_t("10031") // Scrapped Report
#define HELPER_CEID_MaterialManualOutputReport _bstr_t("10032") // Material Manual Output Report
#define HELPER_CEID_MaterialManualInputVerificationRequest _bstr_t("10033") // Material Manual Input Verification Request
#define HELPER_CEID_MaterialManualInputReport _bstr_t("10034") // Material Manual Input Report
#define HELPER_CEID_SubMaterialVerificationRequest _bstr_t("10035") // Sub Material Verification Request
#define HELPER_CEID_SubMaterialExchangedReport _bstr_t("10036") // Sub Material Exchanged Report
#define HELPER_CEID_CalibrationCompletedReport _bstr_t("10037") // Calibration Completed Report
#define HELPER_CEID_IdleReasonReport _bstr_t("10038") // Idle Reason Report
#define HELPER_CEID_StageStateChangedReport _bstr_t("10039") // Stage State Changed Report
#define HELPER_CEID_SpoolActivated _bstr_t("11") // Spool Activated
#define HELPER_CEID_SpoolDeactivated _bstr_t("12") // Spool Deactivated
#define HELPER_CEID_SpoolTransmitFailure _bstr_t("13") // Spool Transmit Failure
#define HELPER_CEID_AlarmSet _bstr_t("16") // Alarm Set
#define HELPER_CEID_AlarmClear _bstr_t("17") // Alarm Clear
#define HELPER_CEID_OnlineLocal _bstr_t("2") // Online Local
#define HELPER_CEID_AlarmClearBase _bstr_t("20000") // Alarm Clear
#define HELPER_CEID_LimitMonitoringBase _bstr_t("30000") // Limit Monitoring
#define HELPER_CEID_ControlStateChanged _bstr_t("4") // Equipment Constant Changed
#define HELPER_CEID_EquipmentConstantChangedByHost _bstr_t("6") // Equipment Constant Changed(HOST-S2F15)
#define HELPER_CEID_MaterialReceived _bstr_t("9") // Material Received

#define HELPER_ECID_EstablishCommunicationsTimeout _bstr_t("101") // Establish communication delay (timeout). 
#define HELPER_ECID_HeartBeat _bstr_t("102") // Heartbeat Rate
#define HELPER_ECID_InitCommunicationState _bstr_t("103") // Default Communication State at system initialization.
#define HELPER_ECID_InitControlState _bstr_t("104") // Default Control State at system initialization.
#define HELPER_ECID_OffLineSubState _bstr_t("105") // Default Offline substate
#define HELPER_ECID_OnLineFailState _bstr_t("106") // Default offline state after Attempt Online fails
#define HELPER_ECID_TimeFormat _bstr_t("107") // TimeFormat: Set time format.
#define HELPER_ECID_OnLineSubState _bstr_t("108") // Default Online Sub State
#define HELPER_ECID_CTTimeoutCount _bstr_t("109") // Conversation Timeout Count
#define HELPER_ECID_AreYouThereTimeout _bstr_t("130")
#define HELPER_ECID_IdleReasonReportUsage _bstr_t("201") // IDLE Reason Report Usage
#define HELPER_ECID_IdleReasonTimeInterval _bstr_t("202") // EQ Process Idle Status - report Setup Time
#define HELPER_ECID_OnlineLocalModeUsage _bstr_t("203") // Online Local Mode Usage
#define HELPER_ECID_EuipmentID _bstr_t("204") // Euipment ID(managed by MES)
#define HELPER_ECID_Lane_1_ProcessCode _bstr_t("205") // the Process Code of Lane #1(managed by MES)
#define HELPER_ECID_Lane_2_ProcessCode _bstr_t("206") // the Process Code of Lane #2(managed by MES)
#define HELPER_ECID_HostCommandStreamFunction _bstr_t("207") // Host Command Send Stream Function Set
#define HELPER_ECID_FFUsage _bstr_t("208") // Feed Forward function Usage(EQ using F/F Function)
#define HELPER_ECID_FFMode _bstr_t("209") // Feed Forward Operation Mode(EQ using F/F & F/B Function)
#define HELPER_ECID_FFToEuipmentID_1 _bstr_t("210") // To Euipment ID of Feed Forward data(managed by MES)
#define HELPER_ECID_FFToEuipmentID_2 _bstr_t("211") // To Euipment ID of Feed Forward data(managed by MES)
#define HELPER_ECID_FFToEuipmentID_3 _bstr_t("212") // To Euipment ID of Feed Forward data(managed by MES)
#define HELPER_ECID_FFToEuipmentID_4 _bstr_t("213") // To Euipment ID of Feed Forward data(managed by MES)
#define HELPER_ECID_FBUsage _bstr_t("214") // Feed back function(EQ using F/B Function)
#define HELPER_ECID_FBMode _bstr_t("215") // Feed Back Operation Mode(EQ using F/B Function)
#define HELPER_ECID_FBToEuipmentID_1 _bstr_t("216") // To Euipment ID of Feed back data(managed by MES)
#define HELPER_ECID_FBToEuipmentID_2 _bstr_t("217") // To Euipment ID of Feed back data(managed by MES)
#define HELPER_ECID_FBToEuipmentID_3 _bstr_t("218") // To Euipment ID of Feed back data(managed by MES)
#define HELPER_ECID_FBToEuipmentID_4 _bstr_t("219") // To Euipment ID of Feed back data(managed by MES)
#define HELPER_ECID_ProfileDataSamplingPeriod _bstr_t("220") // Profile Data Sampling Period
#define HELPER_ECID_T3Timeout _bstr_t("901")
#define HELPER_ECID_T5Timeout _bstr_t("902")
#define HELPER_ECID_T6Timeout _bstr_t("903")
#define HELPER_ECID_T7Timeout _bstr_t("904")
#define HELPER_ECID_T8Timeout _bstr_t("905")
#define HELPER_ECID_DeviceID _bstr_t("906")
#define HELPER_ECID_IPAddress _bstr_t("907")
#define HELPER_ECID_PortNumber _bstr_t("908")
#define HELPER_ECID_ActiveMode _bstr_t("909")
#define HELPER_ECID_LinkTestInterval _bstr_t("910")
#define HELPER_ECID_MaxSpoolTransmit _bstr_t("911")
#define HELPER_ECID_OverWriteSpool _bstr_t("912")
#define HELPER_ECID_EnableSpooling _bstr_t("913")
#define HELPER_ECID_Maker _bstr_t("914")
#define HELPER_ECID_MaxSpoolMsg _bstr_t("915")
#define HELPER_ECID_RetryLimit _bstr_t("916")

#define HELPER_VID_ArrayMaterialList _bstr_t("")
#define HELPER_VID_CarrierSubPositionList _bstr_t("")
#define HELPER_VID_SubMaterialCount _bstr_t("") // Sub Material Count
#define HELPER_VID_SubMaterialInformationCount _bstr_t("") // Sub Material Information Count
#define HELPER_VID_StageStateListValue _bstr_t("")
#define HELPER_VID_AlarmSetListValue _bstr_t("")
#define HELPER_VID_CarrierSubPositionListValue _bstr_t("")
#define HELPER_VID_Ports _bstr_t("")
#define HELPER_VID_Clock _bstr_t("1")
#define HELPER_VID_ControlState _bstr_t("10001") // Control State
#define HELPER_VID_ControlStateChangeReasonCode _bstr_t("10002") // Control State Change Reason Code from Onlone Remote/Local to Offline
#define HELPER_VID_ControlStateChangeReasonText _bstr_t("10003") // Control State Change Reason Code Description from Onlone Remote/Local to Offline
#define HELPER_VID_OperatorID _bstr_t("10004") // Operator Employee Number
#define HELPER_VID_TerminalID _bstr_t("10005") // Terminal Number
#define HELPER_VID_TerminalText _bstr_t("10006") // Terminal Message
#define HELPER_VID_LaneID _bstr_t("10007") // Front / Rear Lane ID
#define HELPER_VID_ProcessModelID _bstr_t("10008") // Process Model ID
#define HELPER_VID_ChangedECList _bstr_t("10009") // L2(ECID,ECV) List
#define HELPER_VID_OrderType _bstr_t("10010") // Command Subject
#define HELPER_VID_ProcessProgramID _bstr_t("10011") // Process Program ID
#define HELPER_VID_ProcessModelIDtoChange _bstr_t("10012") // The process model ID to change
#define HELPER_VID_ProcessProgramIDtoChange _bstr_t("10013") // The Process Program ID to Change
#define HELPER_VID_ModelChangeState _bstr_t("10014") // Model Change State
#define HELPER_VID_ModelChangeType _bstr_t("10015") // Model Change Type
#define HELPER_VID_MaterialInputType _bstr_t("10016") // Material Input Type
#define HELPER_VID_MaterialInputState _bstr_t("10017") // Material Input State
#define HELPER_VID_PPChangeStatus _bstr_t("10018")
#define HELPER_VID_UnitID _bstr_t("10019") // UnitID
#define HELPER_VID_ProcessState _bstr_t("10020") // Current Process State
#define HELPER_VID_EndType _bstr_t("10021") // Process Completed Type
#define HELPER_VID_FFFBDataType _bstr_t("10022") // Feed Forward / Feed Back Data Type
#define HELPER_VID_FFFBActionType _bstr_t("10023") // Feed Forward / Feed Back Action Type
#define HELPER_VID_FFFBActionText _bstr_t("10024") // Message to be displayed on the equipment screen by FFFBACTIONTYPE
#define HELPER_VID_FFFBFromEqpID _bstr_t("10025") // Front Equipment ID of F/F, F/B Data
#define HELPER_VID_FFFBToEqpID _bstr_t("10026") // To Equipment ID of F/F, F/B Data
#define HELPER_VID_FilePath _bstr_t("10027") // Log File Path
#define HELPER_VID_FileName _bstr_t("10028") // Log File Name 
#define HELPER_VID_MaterialOutputType _bstr_t("10029") // Material Output Type
#define HELPER_VID_MaterialOutputState _bstr_t("10030") // Material Output State
#define HELPER_VID_Capacity _bstr_t("10031") // Quantity of magazine currently loaded in the port
#define HELPER_VID_PortState _bstr_t("10032") // Port State
#define HELPER_VID_CarrierID _bstr_t("10033") // CarrierID
#define HELPER_VID_CarrierMainPosition _bstr_t("10034") // Carrier Main Position  (=Port ID)
#define HELPER_VID_CarrierSubPosition _bstr_t("10035")
#define HELPER_VID_CarrierSide _bstr_t("10036") // Carrier Side Information
#define HELPER_VID_CommandActionType _bstr_t("10037") // Host Remote Command Action Type
#define HELPER_VID_PermitCode _bstr_t("10038") // Permit Reason Code
#define HELPER_VID_PermitText _bstr_t("10039") // Permit Reason Code Description
#define HELPER_VID_EqpOperationMode _bstr_t("10040") // Equipment Operation Mode
#define HELPER_VID_CancelCode _bstr_t("10041") // Cancel Reason Code
#define HELPER_VID_CancelText _bstr_t("10042") // Cancel Reason Code Description
#define HELPER_VID_CancelNote _bstr_t("10043") // Cancel Note (The value keyined by operator)
#define HELPER_VID_PauseCode _bstr_t("10044") // Pause Reason Code
#define HELPER_VID_PauseText _bstr_t("10045") // Pause Reason Text
#define HELPER_VID_AbortCode _bstr_t("10046") // Abort Reason Code
#define HELPER_VID_AbortText _bstr_t("10047") // Abort Reason Text
#define HELPER_VID_ScrapType _bstr_t("10048") // Scrap Type
#define HELPER_VID_ScrapCode _bstr_t("10049") // Scrap Code
#define HELPER_VID_ScrapText _bstr_t("10050") // Scrap Code Description
#define HELPER_VID_ScrapNote _bstr_t("10051") // Scrap Note (The value keyined by operator)
#define HELPER_VID_ManualOutputCode _bstr_t("10052") // Manual Output Reason Code
#define HELPER_VID_ManualOutputText _bstr_t("10053") // Manual Output Reason Code Description
#define HELPER_VID_ManualInputCode _bstr_t("10054") // Manual Input Reason Code
#define HELPER_VID_ManualInputText _bstr_t("10055") // Manual Input Reason Code Description
#define HELPER_VID_SubMaterialExchangeCode _bstr_t("10056") // Sub Material Exchange Reason Code
#define HELPER_VID_SubMaterialExchangeText _bstr_t("10057") // Sub Material Exchange Reason Code Description
#define HELPER_VID_SubMaterialExchangeNote _bstr_t("10058") // Sub Material Exchange Note (The value keyined by operator)
#define HELPER_VID_IdleCode _bstr_t("10059") // Idle Reason Code
#define HELPER_VID_IdleText _bstr_t("10060") // Idle Reason Code Description
#define HELPER_VID_IdleStartTime _bstr_t("10061") // Idle Start Time
#define HELPER_VID_IdleEndTime _bstr_t("10062") // Idle End Time
#define HELPER_VID_IdleNote _bstr_t("10063") // Idle Note (The value keyined by operator)
#define HELPER_VID_StageID _bstr_t("10064") // Stage ID
#define HELPER_VID_StageState _bstr_t("10065") // Inspection / Ionizer Stage State
#define HELPER_VID_MaterialInformation _bstr_t("10066") // L4(MATERIALID,MATERIALTYPE,MATERIALSIDE,MATERIALJUDGE)
#define HELPER_VID_ArrayMaterialInformation _bstr_t("10067") // L,n   L5(ARRAYMATERIALID,ARRAYMATERIALTYPE,ARRAYMATERIALPOSITION,ARRAYMATERIALSIDE,ARRAYMATERIALJUDGE)
#define HELPER_VID_CurrnetCarrierInformation _bstr_t("10068")
#define HELPER_VID_PreviousCarrierInformation _bstr_t("10069")
#define HELPER_VID_NGBufferCarrierInformation _bstr_t("10070")
#define HELPER_VID_CurrentCarrierSubMaterialMapData _bstr_t("10071")
#define HELPER_VID_SubMaterialInformation _bstr_t("10072")
#define HELPER_VID_StageStateList _bstr_t("10073")
#define HELPER_VID_PortStateList _bstr_t("10074")
#define HELPER_VID_CurrentMaterialList _bstr_t("10075")
#define HELPER_VID_CurrentCarrierList _bstr_t("10076")
#define HELPER_VID_CurrentSubMaterialList _bstr_t("10077")
#define HELPER_VID_AlarmSet _bstr_t("10078")
#define HELPER_VID_ChangedECID _bstr_t("10079")
#define HELPER_VID_ChangedECV _bstr_t("10080")
#define HELPER_VID_SpoolCountActual _bstr_t("11")
#define HELPER_VID_SpoolCountTotal _bstr_t("12")
#define HELPER_VID_SpoolFullTime _bstr_t("13")
#define HELPER_VID_MDLN _bstr_t("14")
#define HELPER_VID_SOFTREV _bstr_t("15")
#define HELPER_VID_SpoolStartTime _bstr_t("16")
#define HELPER_VID_SpoolStatus _bstr_t("17")
#define HELPER_VID_SpoolFull _bstr_t("18")
#define HELPER_VID_ALCD _bstr_t("2")
#define HELPER_VID_ArrayMaterialAPD_VIDList _bstr_t("20001")
#define HELPER_VID_FeedForwardDataList _bstr_t("20002") // of FFFB Data Parameter List
#define HELPER_VID_FeedBackDataList _bstr_t("20003") // of FFFB Data Parameter List
#define HELPER_VID_MaterialAPD_VIDList _bstr_t("20004")
#define HELPER_VID_CalibrationResultDataList _bstr_t("20005") // MPS Underfill EQ Only
#define HELPER_VID_TraceDataList _bstr_t("20006") // For the equipment using "Trace Data Report"
#define HELPER_VID_AlarmID _bstr_t("24")
#define HELPER_VID_EventLimit _bstr_t("28")
#define HELPER_VID_LimitVariable _bstr_t("29")
#define HELPER_VID_ALID _bstr_t("3")
#define HELPER_VID_OperatorCommand _bstr_t("30")
#define HELPER_VID_MaterialID _bstr_t("30002") // Material ID
#define HELPER_VID_MaterialTYPE _bstr_t("30003") // Material Type (=Material)
#define HELPER_VID_MaterialSIDE _bstr_t("30004") // Material Side (=Top / Bottom)
#define HELPER_VID_MaterialJudge _bstr_t("30005") // Material Judgement
#define HELPER_VID_ArrayMaterialID _bstr_t("30006") // Array Material ID
#define HELPER_VID_ArrayMaterialType _bstr_t("30007") // Array Material Type (=Array Material)
#define HELPER_VID_ArrayMaterialPosition _bstr_t("30008") // Array Material Position (=Array Position)
#define HELPER_VID_ArrayMaterialSide _bstr_t("30009") // Array Material Side (=Top / Bottom)
#define HELPER_VID_ArrayMaterialJudge _bstr_t("30010") // Array Material Judgement (Inspection Result)
#define HELPER_VID_CurrentCarrierID _bstr_t("30011") // Current Carrier ID (=Magazine ID)
#define HELPER_VID_CurrentCarrierType _bstr_t("30012") // Current Carrier Type (=Magazine)
#define HELPER_VID_CurrentCarrierMainPosition _bstr_t("30013") // Current Carrier Main Position (=Port ID)
#define HELPER_VID_CurrentCarrierSubPosition _bstr_t("30014") // Current Carrier Sub Position (=Slot ID)
#define HELPER_VID_CurrentCarrierSide _bstr_t("30015") // Current Carrier Side (=Previous or Current)
#define HELPER_VID_PreviousCarrierID _bstr_t("30016") // Previous Carrier ID (=Carrier ID)
#define HELPER_VID_PreviousCarrierType _bstr_t("30017") // Previous Carrier Type (=Equipment/Line Carrier)
#define HELPER_VID_PreviousCarrierMainPosition _bstr_t("30018") // Previous Carrier Main Position (=Port ID)
#define HELPER_VID_PreviousCarrierSubPosition _bstr_t("30019") // Previous Carrier Sub Position (=Slot ID)
#define HELPER_VID_PreviousCarrierSide _bstr_t("30020") // Previous Carrier Side (=Previous or Current)
#define HELPER_VID_NGBuffCarrierID _bstr_t("30021") // NG Buff Carrier ID (=NG Buffer Magazine ID)
#define HELPER_VID_NGBuffCarrierType _bstr_t("30022") // NG Buff Carrier Type (=Magazine)
#define HELPER_VID_NGBuffCarrierMainPosition _bstr_t("30023") // NG Buff Carrier Main Position (=NG Buffer Port ID)
#define HELPER_VID_NGBuffCarrierSubPosition _bstr_t("30024") // NG Buff Carrier Sub Position (=NG Buffer Magazine Slot ID)
#define HELPER_VID_NGBuffCarrierSide _bstr_t("30025") // NG Buff Carrier Side (=Previous or Current)
#define HELPER_VID_SubMaterialID _bstr_t("30026") // Sub Material ID
#define HELPER_VID_SubMaterialType _bstr_t("30027") // Sub Material Type
#define HELPER_VID_SubMaterialJudge _bstr_t("30028") // Sub Material Judgement (MES Code)
#define HELPER_VID_SubMaterialCarrierID _bstr_t("30029") // Sub Material Carrier ID
#define HELPER_VID_SubMaterialCarrierType _bstr_t("30030") // Sub Material Carrier Type
#define HELPER_VID_SubMaterialCarrierMainPosition _bstr_t("30031") // Sub Material Carrier Main Position (=Port ID)
#define HELPER_VID_SubMaterialCarrierSubPosition _bstr_t("30032") // Sub Material Carrier Sub Position (=Pocket ID)
#define HELPER_VID_SubMaterialMainPosition _bstr_t("30033") // Sub Material Main Position (=Unit ID)
#define HELPER_VID_SubMaterialSubPosition_1 _bstr_t("30034") // Sub Material Sub Position #1 (=Slot ID)
#define HELPER_VID_SubMaterialSubPosition_2 _bstr_t("30035") // Sub Material Sub Position #2 (=Feeder ID) -> Mount EQ Only
#define HELPER_VID_StateID _bstr_t("30036")
#define HELPER_VID_PortID _bstr_t("30037")
#define HELPER_VID_PPChangeName _bstr_t("31")
#define HELPER_VID_PPError _bstr_t("33")
#define HELPER_VID_TransitionType _bstr_t("34")
#define HELPER_VID_PreviousControlState _bstr_t("36")
#define HELPER_VID_CommState _bstr_t("37")
#define HELPER_VID_PreviousCommState _bstr_t("38")
#define HELPER_VID_ALTX _bstr_t("4")
#define HELPER_VID_EventsEnabled _bstr_t("5")
#define HELPER_VID_PreviousProcessState _bstr_t("7")
#define HELPER_VID_AlarmsEnabled _bstr_t("8")

#define HELPER_DD_ABS _bstr_t("ABS")
#define HELPER_DD_ACCESSMODE _bstr_t("ACCESSMODE")
#define HELPER_DD_ACDS _bstr_t("ACDS")
#define HELPER_DD_ACKA _bstr_t("ACKA")
#define HELPER_DD_ACKC10 _bstr_t("ACKC10")
#define HELPER_DD_ACKC13 _bstr_t("ACKC13") // Return code for secondary messages 1 byte.
#define HELPER_DD_ACKC15 _bstr_t("ACKC15") // Return code for secondary messages, 1 byte.
#define HELPER_DD_ACKC3 _bstr_t("ACKC3") // Acknowledge code, 1 byte.
#define HELPER_DD_ACKC5 _bstr_t("ACKC5")
#define HELPER_DD_ACKC6 _bstr_t("ACKC6")
#define HELPER_DD_ACKC7 _bstr_t("ACKC7")
#define HELPER_DD_ACKC7A _bstr_t("ACKC7A")
#define HELPER_DD_AGENT _bstr_t("AGENT")
#define HELPER_DD_ALCD _bstr_t("ALCD")
#define HELPER_DD_ALED _bstr_t("ALED")
#define HELPER_DD_ALID _bstr_t("ALID")
#define HELPER_DD_ALTX _bstr_t("ALTX")
#define HELPER_DD_ATTRDATA _bstr_t("ATTRDATA")
#define HELPER_DD_ATTRID _bstr_t("ATTRID")
#define HELPER_DD_ATTRRELN _bstr_t("ATTRRELN") // The relationship that a specified qualifying value has to the value of an attribute of an object instance (the value of interest):
#define HELPER_DD_AUTOCLEAR _bstr_t("AUTOCLEAR") // A flag which enables or disables the Auto Clear function.
#define HELPER_DD_AUTOCLOSE _bstr_t("AUTOCLOSE") // A function that equipment closes the session automatically when operator access doesn’t occur exceeding the predefined maximum time.
#define HELPER_DD_BATCHLOCID _bstr_t("BATCHLOCID")
#define HELPER_DD_BATCHLOCIDVALUE _bstr_t("BATCHLOCIDVALUE")
#define HELPER_DD_BCDS _bstr_t("BCDS") // Before Command Codes.
#define HELPER_DD_BCEQU _bstr_t("BCEQU") // Bin code equivalents.
#define HELPER_DD_BINLT _bstr_t("BINLT") // The Bin List.
#define HELPER_DD_BLKDEF _bstr_t("BLKDEF") // Block Definition.
#define HELPER_DD_BPD _bstr_t("BPD") // Boot program Data.
#define HELPER_DD_BYTMAX _bstr_t("BYTMAX") // Byte Maximum.
#define HELPER_DD_CAACK _bstr_t("CAACK")
#define HELPER_DD_CAPACITY _bstr_t("CAPACITY")
#define HELPER_DD_CARRIERACCESSINGSTATUS _bstr_t("CARRIERACCESSINGSTATUS")
#define HELPER_DD_CARRIERACTION _bstr_t("CARRIERACTION")
#define HELPER_DD_CARRIERID _bstr_t("CARRIERID")
#define HELPER_DD_CARRIERIDSTATUS _bstr_t("CARRIERIDSTATUS")
#define HELPER_DD_CARRIERINPUTID _bstr_t("CARRIERINPUTID")
#define HELPER_DD_CARRIERINPUTSPEC _bstr_t("CARRIERINPUTSPEC")
#define HELPER_DD_CARRIERSPEC _bstr_t("CARRIERSPEC")
#define HELPER_DD_CATTRDATA _bstr_t("CATTRDATA")
#define HELPER_DD_CATTRID _bstr_t("CATTRID")
#define HELPER_DD_CCACK _bstr_t("CCACK") // Information concerning the result of the service.
#define HELPER_DD_CCODE _bstr_t("CCODE")
#define HELPER_DD_CEED _bstr_t("CEED")
#define HELPER_DD_CEID _bstr_t("CEID")
#define HELPER_DD_CENAME _bstr_t("CENAME")
#define HELPER_DD_CEPACK _bstr_t("CEPACK")
#define HELPER_DD_CEPVAL _bstr_t("CEPVAL") // Command Enhanced Parameter Value.
#define HELPER_DD_CHKINFO _bstr_t("CHKINFO") // User defined.
#define HELPER_DD_CKPNT _bstr_t("CKPNT") // Checkpoint as defined by the sending system.
#define HELPER_DD_CLSSTS _bstr_t("CLSSTS") // Information if the session is closed properly.
#define HELPER_DD_CMDA _bstr_t("CMDA") // Command acknowledge code.
#define HELPER_DD_CMDMAX _bstr_t("CMDMAX") // Command Maximum.
#define HELPER_DD_CNAME _bstr_t("CNAME") // Command Name = 16 characters.
#define HELPER_DD_COLCT _bstr_t("COLCT") // Column count in die increments.
#define HELPER_DD_COLHDR _bstr_t("COLHDR") // Text description of contents of TBLELT. 1?20 characters.
#define HELPER_DD_COMMACK _bstr_t("COMMACK")
#define HELPER_DD_COMPARISONOPERATOR _bstr_t("COMPARISONOPERATOR") // Choice of available operators that compare the supplied value to the current attribute value. Evaluated as ‘Current value XX supplied value’ where XX is one of the enumerated values  (e.g., ‘GT’).
#define HELPER_DD_CONDITION _bstr_t("CONDITION") // Provides condition information for a subsystem component. Used in the data item in the CONDITIONLIST.
#define HELPER_DD_CONDITIONLIST _bstr_t("CONDITIONLIST") // A list of CONDITION data sent in a fixed order. CONDITIONLIST has the following form:
#define HELPER_DD_CPACK _bstr_t("CPACK")
#define HELPER_DD_CPNAME _bstr_t("CPNAME")
#define HELPER_DD_CPVAL _bstr_t("CPVAL")
#define HELPER_DD_CRAACK _bstr_t("CRAACK") // Information concerning the result of the service.
#define HELPER_DD_CRAEACK _bstr_t("CRAEACK") // Information concerning the result of the event.
#define HELPER_DD_CSAACK _bstr_t("CSAACK") // Equipment Acknowledgement code,  1 byte.
#define HELPER_DD_CTLJOBCMD _bstr_t("CTLJOBCMD")
#define HELPER_DD_CTLJOBID _bstr_t("CTLJOBID")
#define HELPER_DD_DATA _bstr_t("DATA")
#define HELPER_DD_DATAACK _bstr_t("DATAACK") // Acknowledge code for data.
#define HELPER_DD_DATACOLLECTIONPLAN _bstr_t("DATACOLLECTIONPLAN")
#define HELPER_DD_DATAID _bstr_t("DATAID")
#define HELPER_DD_DATALENGTH _bstr_t("DATALENGTH")
#define HELPER_DD_DATASEG _bstr_t("DATASEG")
#define HELPER_DD_DATASRC _bstr_t("DATASRC") // Object type for Data Source Objects.
#define HELPER_DD_DATLC _bstr_t("DATLC") // Data location.
#define HELPER_DD_DELRSPSTAT _bstr_t("DELRSPSTAT") // Status response for the Delete PDE request.
#define HELPER_DD_DESTCARRIERID _bstr_t("DESTCARRIERID")
#define HELPER_DD_DIRRSPSTAT _bstr_t("DIRRSPSTAT") // Status response for the GET PDE Directory request.
#define HELPER_DD_DISABLEEVENTS _bstr_t("DISABLEEVENTS")
#define HELPER_DD_DRACK _bstr_t("DRACK")
#define HELPER_DD_DRRACK _bstr_t("DRRACK") // Information concerning the result of the service.
#define HELPER_DD_DSID _bstr_t("DSID")
#define HELPER_DD_DSNAME _bstr_t("DSNAME") // The name of the Data Set.
#define HELPER_DD_DSPER _bstr_t("DSPER")
#define HELPER_DD_DUTMS _bstr_t("DUTMS") // Die Units of Measure.
#define HELPER_DD_DVNAME _bstr_t("DVNAME") // Data value name.
#define HELPER_DD_DVVAL _bstr_t("DVVAL")
#define HELPER_DD_DVVALNAME _bstr_t("DVVALNAME")
#define HELPER_DD_EAC _bstr_t("EAC")
#define HELPER_DD_ECDEF _bstr_t("ECDEF")
#define HELPER_DD_ECID _bstr_t("ECID")
#define HELPER_DD_ECMAX _bstr_t("ECMAX")
#define HELPER_DD_ECMIN _bstr_t("ECMIN")
#define HELPER_DD_ECNAME _bstr_t("ECNAME")
#define HELPER_DD_ECV _bstr_t("ECV")
#define HELPER_DD_EDID _bstr_t("EDID")
#define HELPER_DD_EMID _bstr_t("EMID") // Equivalent material ID  (16 bytes maximum).
#define HELPER_DD_EPD _bstr_t("EPD") // Executive program data.
#define HELPER_DD_EQID _bstr_t("EQID") // Identifier that indicates equipment which the recipe is tuned for.
#define HELPER_DD_EQNAME _bstr_t("EQNAME") // A unique ASCII equipment identifier assigned by the factory to the equipment. Limited to a maximum of 80 characters.
#define HELPER_DD_ERACK _bstr_t("ERACK")
#define HELPER_DD_ERCACK _bstr_t("ERCACK") // Response component for single recipe check.
#define HELPER_DD_ERRCODE _bstr_t("ERRCODE")
#define HELPER_DD_ERRTEXT _bstr_t("ERRTEXT")
#define HELPER_DD_ERRW7 _bstr_t("ERRW7")
#define HELPER_DD_ERXACK _bstr_t("ERXACK") // Response component for single recipe transfer.
#define HELPER_DD_EVNTSRC _bstr_t("EVNTSRC") // Object type for Event Source Objects.
#define HELPER_DD_EXID _bstr_t("EXID") // Unique identifier for the exception. Maximum length of 20 characters.
#define HELPER_DD_EXMESSAGE _bstr_t("EXMESSAGE") // Text which describes the nature of the exception.
#define HELPER_DD_EXRECVRA _bstr_t("EXRECVRA") // Text which specifies a recovery action for an exception. Maximum length of 40 bytes.
#define HELPER_DD_EXTYPE _bstr_t("EXTYPE") // Text which identifies the type of an exception. It is usually a single word of text.
#define HELPER_DD_FCNID _bstr_t("FCNID")
#define HELPER_DD_FFROT _bstr_t("FFROT") // Film Frame Rotation.
#define HELPER_DD_FILDAT _bstr_t("FILDAT") // Data from the Data Set.
#define HELPER_DD_FNLOC _bstr_t("FNLOC") // Flat/Notch Location.
#define HELPER_DD_FRMLEN _bstr_t("FRMLEN") // Formatted Process Program Length.
#define HELPER_DD_GETRSPSTAT _bstr_t("GETRSPSTAT") // Status response for the Get PDE and Get PDEheader requests.
#define HELPER_DD_GRANT _bstr_t("GRANT")
#define HELPER_DD_GRANT6 _bstr_t("GRANT6")
#define HELPER_DD_GRNT1 _bstr_t("GRNT1") // Grant code, 1 byte.
#define HELPER_DD_GRXLACK _bstr_t("GRXLACK") // Information concerning the result of the service.
#define HELPER_DD_HANDLE _bstr_t("HANDLE") // Logical unit or channel.
#define HELPER_DD_HCACK _bstr_t("HCACK")
#define HELPER_DD_HOACK _bstr_t("HOACK") // Conveys whether the corresponding handoff activity succeeded (= True) or failed (= False).
#define HELPER_DD_HOCANCELACK _bstr_t("HOCANCELACK") // Tells whether the cancel ready message was accepted or rejected.
#define HELPER_DD_HOCMDNAME _bstr_t("HOCMDNAME") // Identifier for the handoff command to be executed.
#define HELPER_DD_HOHALTACK _bstr_t("HOHALTACK") // Tells whether the halt command was accepted or rejected.
#define HELPER_DD_IACDS _bstr_t("IACDS") // Immediately After Command Codes.
#define HELPER_DD_IBCDS _bstr_t("IBCDS") // Immediately Before Command Codes.
#define HELPER_DD_IDTYP _bstr_t("IDTYP") // Id type.
#define HELPER_DD_INPTN _bstr_t("INPTN") // A specialized version of PTN indicating the InputPort.
#define HELPER_DD_JOBACTION _bstr_t("JOBACTION") // Specifies the action for a ReticleTransferJob.
#define HELPER_DD_LENGTH _bstr_t("LENGTH")
#define HELPER_DD_LIMITACK _bstr_t("LIMITACK")
#define HELPER_DD_LIMITID _bstr_t("LIMITID")
#define HELPER_DD_LIMITMAX _bstr_t("LIMITMAX")
#define HELPER_DD_LIMITMIN _bstr_t("LIMITMIN")
#define HELPER_DD_LINKID _bstr_t("LINKID")
#define HELPER_DD_LLIM _bstr_t("LLIM") // Lower limit for numeric value.
#define HELPER_DD_LOC _bstr_t("LOC") // Machine material location code, 1 byte.
#define HELPER_DD_LOCID _bstr_t("LOCID")
#define HELPER_DD_LOTID _bstr_t("LOTID")
#define HELPER_DD_LOWERDB _bstr_t("LOWERDB")
#define HELPER_DD_LRACK _bstr_t("LRACK")
#define HELPER_DD_LVACK _bstr_t("LVACK")
#define HELPER_DD_MAPER _bstr_t("MAPER") // Map Error.
#define HELPER_DD_MAPFT _bstr_t("MAPFT") // Map data format type.
#define HELPER_DD_MATERIALSTATUS _bstr_t("MATERIALSTATUS")
#define HELPER_DD_MAXNUMBER _bstr_t("MAXNUMBER") // Provides MaxNumber information for each subspace. Used in the data item MAXNUMBERLIST.
#define HELPER_DD_MAXNUMBERLIST _bstr_t("MAXNUMBERLIST") // Maximum number of PEM Recipes allowed to be preserved in PRC after PJ creation. MaxNumber has a list structure so that it can be applied to each subspace. The usage of the list structure is equipment defined.
#define HELPER_DD_MAXTIME _bstr_t("MAXTIME") // Maximum time during which a PEM Recipe allowed to be in PRC after use.
#define HELPER_DD_MCINDEX _bstr_t("MCINDEX") // Identifier used to link a handoff command message with its eventual completion message. Corresponding messages carry the same value for this data item.
#define HELPER_DD_MDACK _bstr_t("MDACK") // Map data acknowledge.
#define HELPER_DD_MDLN _bstr_t("MDLN")
#define HELPER_DD_MEXP _bstr_t("MEXP")
#define HELPER_DD_MF _bstr_t("MF")
#define HELPER_DD_MHEAD _bstr_t("MHEAD")
#define HELPER_DD_MID _bstr_t("MID")
#define HELPER_DD_MIDAC _bstr_t("MIDAC") // Material ID Acknowledge Code, 1 byte.
#define HELPER_DD_MIDRA _bstr_t("MIDRA") // Material ID Acknowledge Code, 1 byte.
#define HELPER_DD_MLCL _bstr_t("MLCL") // Message length.
#define HELPER_DD_MMODE _bstr_t("MMODE") // Matrix mode select, 1 byte.
#define HELPER_DD_MTRLOUTSPEC _bstr_t("MTRLOUTSPEC")
#define HELPER_DD_NACDS _bstr_t("NACDS") // Not After Command Codes.
#define HELPER_DD_NBCDS _bstr_t("NBCDS") // Not Before Command Codes.
#define HELPER_DD_NULBC _bstr_t("NULBC") // Null bin code value.
#define HELPER_DD_OBJACK _bstr_t("OBJACK")
#define HELPER_DD_OBJCMD _bstr_t("OBJCMD") // Specifies an action to be performed by an object:
#define HELPER_DD_OBJID _bstr_t("OBJID")
#define HELPER_DD_OBJSPEC _bstr_t("OBJSPEC")
#define HELPER_DD_OBJTOKEN _bstr_t("OBJTOKEN") // Token used for authorization.
#define HELPER_DD_OBJTYPE _bstr_t("OBJTYPE")
#define HELPER_DD_OFLACK _bstr_t("OFLACK")
#define HELPER_DD_ONLACK _bstr_t("ONLACK")
#define HELPER_DD_OPID _bstr_t("OPID")
#define HELPER_DD_OPRID _bstr_t("OPRID") // Host-registered identifier of the operator who uses the Remote Access session
#define HELPER_DD_OPRPWORD _bstr_t("OPRPWORD") // Host-registered password of the operator who uses the Remote Access session.
#define HELPER_DD_ORAACK _bstr_t("ORAACK") // Information concerning the result of the service.
#define HELPER_DD_ORAEACK _bstr_t("ORAEACK") // Information concerning the result of the event.
#define HELPER_DD_ORDERVALUE _bstr_t("ORDERVALUE")
#define HELPER_DD_ORLOC _bstr_t("ORLOC") // Origin Location.
#define HELPER_DD_OUTPTN _bstr_t("OUTPTN") // A specialized version of PTN indicating the OutPutPort.
#define HELPER_DD_OUTPUTRULEVALUE _bstr_t("OUTPUTRULEVALUE")
#define HELPER_DD_PARAMNAME _bstr_t("PARAMNAME")
#define HELPER_DD_PARAMVAL _bstr_t("PARAMVAL")
#define HELPER_DD_PAUSEEVENT _bstr_t("PAUSEEVENT")
#define HELPER_DD_PDEATTRIBUTE _bstr_t("PDEATTRIBUTE") // Selection from available PDE attributes whose values could be reported.
#define HELPER_DD_PDEATTRIBUTENAME _bstr_t("PDEATTRIBUTENAME") // Selection from available PDE attributes that can be used to filter the PDE directory report.
#define HELPER_DD_PDEATTRIBUTEVALUE _bstr_t("PDEATTRIBUTEVALUE") // Contains the value of the corresponding PDEATTRIBUTE in the appropriate format.
#define HELPER_DD_PDEREF _bstr_t("PDEREF") // Contains the unique identifier of a PDE (uid) or of a PDE group (gid).
#define HELPER_DD_PDFLT _bstr_t("PDFLT") // Parameter Default Value.
#define HELPER_DD_PECACK _bstr_t("PECACK") // OK/NG response from the host to Pre-Exe Check event from equipment.
#define HELPER_DD_PECEACK _bstr_t("PECEACK") // Information concerning the result of the event.
#define HELPER_DD_PECRACK _bstr_t("PECRACK") // Response component for single recipe check.
#define HELPER_DD_PEMFLAG _bstr_t("PEMFLAG") // PEMFlag holds SecurityID to be used for PJ creation:
#define HELPER_DD_PFCD _bstr_t("PFCD")
#define HELPER_DD_PGRPACTION _bstr_t("PGRPACTION") // The action to be performed on a port group.
#define HELPER_DD_PMAX _bstr_t("PMAX") // Parameter Count Maximum.
#define HELPER_DD_PNAME _bstr_t("PNAME") // Parameter Name ≤16 characters.
#define HELPER_DD_PORTACTION _bstr_t("PORTACTION")
#define HELPER_DD_PORTGRPNAME _bstr_t("PORTGRPNAME") // The identifier of a group of ports.
#define HELPER_DD_PPARM _bstr_t("PPARM")
#define HELPER_DD_PPBODY _bstr_t("PPBODY")
#define HELPER_DD_PPGNT _bstr_t("PPGNT")
#define HELPER_DD_PPID _bstr_t("PPID")
#define HELPER_DD_PPNAME _bstr_t("PPNAME")
#define HELPER_DD_PPVALUE _bstr_t("PPVALUE")
#define HELPER_DD_PRAXI _bstr_t("PRAXI") // Process axis.
#define HELPER_DD_PRCMDNAME _bstr_t("PRCMDNAME")
#define HELPER_DD_PRCPREEXECHK _bstr_t("PRCPREEXECHK") // Enable/Disable of PreExecution Check option. This defines use of optional Pre-Execution Check.
#define HELPER_DD_PRCSWITCH _bstr_t("PRCSWITCH") // Enable/Disable of entire PRC functionalities.
#define HELPER_DD_PRDCT _bstr_t("PRDCT") // Process Die Count.
#define HELPER_DD_PREACK _bstr_t("PREACK") // Information concerning the result of the event.
#define HELPER_DD_PREVENTID _bstr_t("PREVENTID")
#define HELPER_DD_PRJOBID _bstr_t("PRJOBID")
#define HELPER_DD_PRJOBMILESTONE _bstr_t("PRJOBMILESTONE")
#define HELPER_DD_PRJOBSPACE _bstr_t("PRJOBSPACE")
#define HELPER_DD_PRMTRLORDER _bstr_t("PRMTRLORDER")
#define HELPER_DD_PROCESSINGCTRLSPEC _bstr_t("PROCESSINGCTRLSPEC")
#define HELPER_DD_PROCESSINGJOBID _bstr_t("PROCESSINGJOBID")
#define HELPER_DD_PROCESSORDERMGMT _bstr_t("PROCESSORDERMGMT")
#define HELPER_DD_PRPAUSEEVENT _bstr_t("PRPAUSEEVENT") // The list of event identifiers, which may be sent as an attribute value to a Process Job. When a Process Job encounters one of these events it will pause, until it receives the PRJobCommand RESUME.
#define HELPER_DD_PRPROCESSSTART _bstr_t("PRPROCESSSTART")
#define HELPER_DD_PRRECIPEMETHOD _bstr_t("PRRECIPEMETHOD")
#define HELPER_DD_PRSTATE _bstr_t("PRSTATE")
#define HELPER_DD_PRXACK _bstr_t("PRXACK") // Information concerning the result of the service.
#define HELPER_DD_PSRACK _bstr_t("PSRACK") // Information concerning the result of the service.
#define HELPER_DD_PTN _bstr_t("PTN")
#define HELPER_DD_QREACK _bstr_t("QREACK") // Information concerning the result of the event.
#define HELPER_DD_QRXACK _bstr_t("QRXACK") // Information concerning the result of the service.
#define HELPER_DD_QRXLEACK _bstr_t("QRXLEACK") // Information concerning the result of the event.
#define HELPER_DD_QUA _bstr_t("QUA") // Quantity in format, 1 byte.
#define HELPER_DD_RAC _bstr_t("RAC") // Reset acknowledge, 1 byte.
#define HELPER_DD_RACSWITCH _bstr_t("RACSWITCH") // Enable/Disable of entire RAC functionalities.
#define HELPER_DD_RCMD _bstr_t("RCMD")
#define HELPER_DD_RCPATTRDATA _bstr_t("RCPATTRDATA") // The contents (value) of a recipe attribute.
#define HELPER_DD_RCPATTRID _bstr_t("RCPATTRID") // The name (identifier) of a non-identifier recipe attribute.
#define HELPER_DD_RCPBODY _bstr_t("RCPBODY") // Recipe body.
#define HELPER_DD_RCPBODYA _bstr_t("RCPBODYA") // Recipe body allowed list structure.
#define HELPER_DD_RCPCLASS _bstr_t("RCPCLASS") // Recipe class.
#define HELPER_DD_RCPCMD _bstr_t("RCPCMD") // Indicates an action to be performed on a recipe.
#define HELPER_DD_RCPDEL _bstr_t("RCPDEL")
#define HELPER_DD_RCPDESCLTH _bstr_t("RCPDESCLTH") // The length in bytes of a recipe section.
#define HELPER_DD_RCPDESCNM _bstr_t("RCPDESCNM") // Identifies a type of descriptor of a recipe: ‘ASDesc’, ‘BodyDesc’, ‘GenDesc.’
#define HELPER_DD_RCPDESCTIME _bstr_t("RCPDESCTIME") // The timestamp of a recipe section, in the format ‘YYYYMMDDhhmmsscc.’
#define HELPER_DD_RCPID _bstr_t("RCPID") // Recipe identifier. Formatted text conforming to the requirements of OBJSPEC.
#define HELPER_DD_RCPNAME _bstr_t("RCPNAME") // Recipe name.
#define HELPER_DD_RCPNEWID _bstr_t("RCPNEWID") // The new recipe identifier assigned as the result of a copy or rename operation.
#define HELPER_DD_RCPOWCODE _bstr_t("RCPOWCODE") // Indicates whether any preexisting recipe is to be overwritten (= TRUE) or not (= FALSE) on download.
#define HELPER_DD_RCPPARNM _bstr_t("RCPPARNM")
#define HELPER_DD_RCPPARRULE _bstr_t("RCPPARRULE") // The restrictions applied to a recipe variable parameter setting. Maximum length of 80 characters.
#define HELPER_DD_RCPPARVAL _bstr_t("RCPPARVAL")
#define HELPER_DD_RCPRENAME _bstr_t("RCPRENAME") // Indicates whether a recipe is to be renamed (= TRUE) or copied (= FALSE).
#define HELPER_DD_RCPSECCODE _bstr_t("RCPSECCODE") // Indicates the sections of a recipe requested for transfer or being transferred:
#define HELPER_DD_RCPSECNM _bstr_t("RCPSECNM") // Recipe section name: ‘Generic’, ‘Body’, or ‘ASDS.’
#define HELPER_DD_RCPSPEC _bstr_t("RCPSPEC")
#define HELPER_DD_RCPSTAT _bstr_t("RCPSTAT") // The status of a managed recipe.
#define HELPER_DD_RCPUPDT _bstr_t("RCPUPDT") // Indicates if an existing recipe is to be updated (= True) or a new recipe is to be created (= False).
#define HELPER_DD_RCPVERS _bstr_t("RCPVERS") // Recipe version.
#define HELPER_DD_READLN _bstr_t("READLN") // Maximum length to read.
#define HELPER_DD_RECID _bstr_t("RECID")
#define HELPER_DD_RECLEN _bstr_t("RECLEN") // Maximum length of a Discrete record.
#define HELPER_DD_REFP _bstr_t("REFP") // Reference Point.
#define HELPER_DD_REPGSZ _bstr_t("REPGSZ")
#define HELPER_DD_RESC _bstr_t("RESC") // Resolution code for numeric data.
#define HELPER_DD_RESOLUTION _bstr_t("RESOLUTION") // Contains the unique identifier of a PDE (uid).
#define HELPER_DD_RESPDESTAT _bstr_t("RESPDESTAT") // Status response for the Resolve PDE request. If more than one of these conditions applies, the first value on the list that applies should be returned.
#define HELPER_DD_RESPEC _bstr_t("RESPEC") // Object specifier for the recipe executor.
#define HELPER_DD_RESV _bstr_t("RESV") // Resolution value for numeric data.
#define HELPER_DD_RETICLEID _bstr_t("RETICLEID") // The object identifier for a reticle. Conforms to OBJSPEC.
#define HELPER_DD_RETPLACEINSTR _bstr_t("RETPLACEINSTR") // Instructions to indicate which pod slots will have reticles placed. Possible values for Reticle-PlacementInstruction are:
#define HELPER_DD_RETREMOVEINSTR _bstr_t("RETREMOVEINSTR") // Instructions to indicate which pod slots will have reticles removed.
#define HELPER_DD_RIC _bstr_t("RIC") // Reset code, 1 byte.
#define HELPER_DD_RMACK _bstr_t("RMACK") // Conveys whether a requested action was successfully completed, denied, completed with errors, or will be completed with notification to the requestor.
#define HELPER_DD_RMCHGSTAT _bstr_t("RMCHGSTAT") // Indicates the change that occurred for an object.
#define HELPER_DD_RMCHGTYPE _bstr_t("RMCHGTYPE") // Indicates the type of change for a recipe.
#define HELPER_DD_RMDATASIZE _bstr_t("RMDATASIZE") // The maximum total length, in bytes, of a multi-block message, used by the receiver to determine if the anticipated message exceeds the receiver’s capacity.
#define HELPER_DD_RMGRNT _bstr_t("RMGRNT") // Grant code, used to grant or deny a request. 1 byte.
#define HELPER_DD_RMNEWNS _bstr_t("RMNEWNS") // New name (identifier) assigned to a recipe namespace.
#define HELPER_DD_RMNSCMD _bstr_t("RMNSCMD") // Action to be performed on a recipe namespace.
#define HELPER_DD_RMNSSPEC _bstr_t("RMNSSPEC") // The object specifier of a recipe namespace.
#define HELPER_DD_RMRECSPEC _bstr_t("RMRECSPEC") // The object specifier of a distributed recipe namespace recorder.
#define HELPER_DD_RMREQUESTOR _bstr_t("RMREQUESTOR") // Set to TRUE if initiator of change request was an attached segment. Set to FALSE otherwise.
#define HELPER_DD_RMSEGSPEC _bstr_t("RMSEGSPEC") // The object specifier of a distributed recipe namespace segment.
#define HELPER_DD_RMSPACE _bstr_t("RMSPACE") // The amount of storage available for at least one recipe in a recipe namespace, in bytes.
#define HELPER_DD_ROWCT _bstr_t("ROWCT") // Row count in die increments.
#define HELPER_DD_RPMACK _bstr_t("RPMACK") // Reticle Pod management service acknowledge code. 1 byte.
#define HELPER_DD_RPMDESTLOC _bstr_t("RPMDESTLOC") // The LocationID towards which a reticle must be moved. Conforms to OBJID.
#define HELPER_DD_RPMSOURLOC _bstr_t("RPMSOURLOC") // The LocationID of the location from which to pick-up a reticle for moving it to another location. Conforms to OBJID.
#define HELPER_DD_RPSEL _bstr_t("RPSEL") // Reference Point Select.
#define HELPER_DD_RPTID _bstr_t("RPTID")
#define HELPER_DD_RPTOC _bstr_t("RPTOC") // A Trace Object attribute for a flag which, if set TRUE, causes only variables which have changed during the sample period to be included in a report.
#define HELPER_DD_RQCMD _bstr_t("RQCMD") // Required Command.
#define HELPER_DD_RQPAR _bstr_t("RQPAR") // Required Parameter.
#define HELPER_DD_RRACK _bstr_t("RRACK") // Information concerning the result of the service.
#define HELPER_DD_RSACK _bstr_t("RSACK") // Ready to Send Acknowledge code, 1 byte.
#define HELPER_DD_RSDA _bstr_t("RSDA")
#define HELPER_DD_RSDC _bstr_t("RSDC")
#define HELPER_DD_RSINF _bstr_t("RSINF") // Starting location for row or column. This item consists of 3 values (x,y,direction). If direction value is negative, it equals decreasing direction. If the value is positive, it equals increasing direction. Direction must be a nonzero value.
#define HELPER_DD_RSPACK _bstr_t("RSPACK")
#define HELPER_DD_RTSRSPSTAT _bstr_t("RTSRSPSTAT") // Status response for the Ready To Send request.
#define HELPER_DD_RTYPE _bstr_t("RTYPE") // Type of record.
#define HELPER_DD_RULENAME _bstr_t("RULENAME")
#define HELPER_DD_RULEVALUE _bstr_t("RULEVALUE")
#define HELPER_DD_RXACK _bstr_t("RXACK") // Response component for a list of recipe transfer.
#define HELPER_DD_SDACK _bstr_t("SDACK") // Map set-up data acknowledge.
#define HELPER_DD_SDBIN _bstr_t("SDBIN") // Send bin information flag.
#define HELPER_DD_SECID _bstr_t("SECID") // Identifier of Security Class of the recipe.
#define HELPER_DD_SENDRESULT _bstr_t("SENDRESULT") // Reports overall success or failure of the sendPDE() request.
#define HELPER_DD_SENDRSPSTAT _bstr_t("SENDRSPSTAT") // Status response for the Send PDE request.
#define HELPER_DD_SEQNUM _bstr_t("SEQNUM")
#define HELPER_DD_SFCD _bstr_t("SFCD") // Status form code, 1 byte.
#define HELPER_DD_SHEAD _bstr_t("SHEAD")
#define HELPER_DD_SLOTID _bstr_t("SLOTID")
#define HELPER_DD_SLOTMAP _bstr_t("SLOTMAP")
#define HELPER_DD_SLOTMAPSTATUS _bstr_t("SLOTMAPSTATUS")
#define HELPER_DD_SMPLN _bstr_t("SMPLN")
#define HELPER_DD_SOFTREV _bstr_t("SOFTREV")
#define HELPER_DD_SPAACK _bstr_t("SPAACK") // Information concerning the result of the service.
#define HELPER_DD_SPD _bstr_t("SPD") // Service program data.
#define HELPER_DD_SPFACK _bstr_t("SPFACK") // Information concerning the result of the service.
#define HELPER_DD_SPID _bstr_t("SPID") // Service program ID, 6 characters.
#define HELPER_DD_SPNAME _bstr_t("SPNAME")
#define HELPER_DD_SPR _bstr_t("SPR") // Service program results.
#define HELPER_DD_SPVAL _bstr_t("SPVAL")
#define HELPER_DD_SRAACK _bstr_t("SRAACK") // Information concerning the result of the service.
#define HELPER_DD_SRCCARRIERID _bstr_t("SRCCARRIERID")
#define HELPER_DD_SRSSWITCH _bstr_t("SRSSWITCH") // Enable/Disable entire SRS functionalities. Default is Disabled.
#define HELPER_DD_SSACK _bstr_t("SSACK")
#define HELPER_DD_SSCMD _bstr_t("SSCMD") // Indicates an action to be performed by the subsystem.
#define HELPER_DD_SSSACK _bstr_t("SSSACK") // Information concerning the result of the service.
#define HELPER_DD_STARTMETHOD _bstr_t("STARTMETHOD")
#define HELPER_DD_STARTMETHODVALUE _bstr_t("STARTMETHODVALUE")
#define HELPER_DD_STATE _bstr_t("STATE")
#define HELPER_DD_STATUS _bstr_t("STATUS")
#define HELPER_DD_STATUSLIST _bstr_t("STATUSLIST") // A list of STATUS data sent in a fixed order. STATUSLIST has the following form:
#define HELPER_DD_STATUSTXT _bstr_t("STATUSTXT") // Text string describing the corresponding status response. Maximum length of 80 characters.
#define HELPER_DD_STEMP _bstr_t("STEMP") // String template. ASCII text string acceptable to equipment as a parameter value. A data string matches a template string if the data string is at least as long as the template and each character of the data string matches the corresponding character of the template. A null list indicates all user data is acceptable to the machine.
#define HELPER_DD_STIME _bstr_t("STIME")
#define HELPER_DD_STRACK _bstr_t("STRACK")
#define HELPER_DD_STRID _bstr_t("STRID")
#define HELPER_DD_STRP _bstr_t("STRP") // Starting position in die coordinate position. Must be in (X,Y) order.
#define HELPER_DD_SUBSTID _bstr_t("SUBSTID")
#define HELPER_DD_SUBSTLOCID _bstr_t("SUBSTLOCID")
#define HELPER_DD_SUBSTPOSINBATCH _bstr_t("SUBSTPOSINBATCH")
#define HELPER_DD_SUBSTPROCESSINGSTATE _bstr_t("SUBSTPROCESSINGSTATE")
#define HELPER_DD_SUBSTRATECOUNT _bstr_t("SUBSTRATECOUNT")
#define HELPER_DD_SUBSTSTATE _bstr_t("SUBSTSTATE")
#define HELPER_DD_SUBSTTYPE _bstr_t("SUBSTTYPE")
#define HELPER_DD_SUBSTUSAGE _bstr_t("SUBSTUSAGE")
#define HELPER_DD_SV _bstr_t("SV")
#define HELPER_DD_SVCACK _bstr_t("SVCACK")
#define HELPER_DD_SVCNAME _bstr_t("SVCNAME")
#define HELPER_DD_SVID _bstr_t("SVID")
#define HELPER_DD_SVNAME _bstr_t("SVNAME")
#define HELPER_DD_TARGETID _bstr_t("TARGETID") // Identifies where a request for action or data is to be applied. If text, conforms to OBJSPEC.
#define HELPER_DD_TARGETPDE _bstr_t("TARGETPDE") // Contains the unique identifier (uid) of the PDE that is the starting point for the verification process.
#define HELPER_DD_TARGETSPEC _bstr_t("TARGETSPEC") // Object specifier of target object.
#define HELPER_DD_TBLACK _bstr_t("TBLACK") // Indicates success or failure.
#define HELPER_DD_TBLCMD _bstr_t("TBLCMD") // Provides information about the table or parts of the table being transferred or requested. Enumerated:
#define HELPER_DD_TBLELT _bstr_t("TBLELT") // Table element. The first table element in a row is used to identify the row.
#define HELPER_DD_TBLID _bstr_t("TBLID") // Table identifier. Text conforming to the requirements of OBJSPEC.
#define HELPER_DD_TBLTYP _bstr_t("TBLTYP") // A reserved text string to denote the format and application of the table. Text conforming to the requirements of OBJSPEC.
#define HELPER_DD_TCID _bstr_t("TCID") // TCID is the identifier of the TransferContainer.
#define HELPER_DD_TEXT _bstr_t("TEXT")
#define HELPER_DD_TIAACK _bstr_t("TIAACK")
#define HELPER_DD_TIACK _bstr_t("TIACK")
#define HELPER_DD_TID _bstr_t("TID")
#define HELPER_DD_TIME _bstr_t("TIME")
#define HELPER_DD_TIMESTAMP _bstr_t("TIMESTAMP")
#define HELPER_DD_TOTSMP _bstr_t("TOTSMP")
#define HELPER_DD_TRACK _bstr_t("TRACK") // Tells whether the related transfer activity was successful (= True) or unsuccessful (= False).
#define HELPER_DD_TRANSFERSIZE _bstr_t("TRANSFERSIZE") // Size, in bytes, of the TransferContainer proposed for transfer.
#define HELPER_DD_TRATOMICID _bstr_t("TRATOMICID") // Equipment assigned identifier for an atomic transfer.
#define HELPER_DD_TRAUTOD _bstr_t("TRAUTOD") // A Trace Object attribute for a control flag which, if set TRUE, causes the Trace Object to delete itself when it has completed a report.
#define HELPER_DD_TRAUTOSTART _bstr_t("TRAUTOSTART") // For each atomic transfer, this data item tells the equipment if it should automatically start the handoff when ready (= TRUE) or await the host’s ‘StartHandoff’ command (= FALSE) following setup. This data item only affects the primary transfer partner.
#define HELPER_DD_TRCMDNAME _bstr_t("TRCMDNAME") // Identifier of the transfer job-related command to be executed. Possible values:
#define HELPER_DD_TRDIR _bstr_t("TRDIR") // Direction of handoff.
#define HELPER_DD_TRID _bstr_t("TRID")
#define HELPER_DD_TRJOBID _bstr_t("TRJOBID") // Equipment assigned identifier for the transfer job.
#define HELPER_DD_TRJOBMS _bstr_t("TRJOBMS") // Milestone for a transfer job (e.g., started or complete).
#define HELPER_DD_TRJOBNAME _bstr_t("TRJOBNAME") // Host assigned identifier for the transfer job. Limited to a maximum of 80 characters.
#define HELPER_DD_TRLINK _bstr_t("TRLINK") // Common identifier for the atomic transfer used by the transfer partners to confirm that they are working on the same host-defined task.
#define HELPER_DD_TRLOCATION _bstr_t("TRLOCATION") // Identifier of the material location involved with the transfer. For one transfer partner, this will represent the designated source location for the material to be sent. For the other transfer partner, it will represent the designated destination location for the material to be received.
#define HELPER_DD_TROBJNAME _bstr_t("TROBJNAME") // Identifier for the material (transfer object) to be transferred.
#define HELPER_DD_TROBJTYPE _bstr_t("TROBJTYPE") // Type of object to be transferred.
#define HELPER_DD_TRPORT _bstr_t("TRPORT") // Identifier of the equipment port to be used for the handoff.
#define HELPER_DD_TRPTNR _bstr_t("TRPTNR") // Name of the equipment which will serve as the other transfer partner for this atomic transfer. This corresponds to EQNAME.
#define HELPER_DD_TRPTPORT _bstr_t("TRPTPORT") // Identifier of the transfer partner’s port to be used for the transfer.
#define HELPER_DD_TRRCP _bstr_t("TRRCP") // Name of the transfer recipe for this handoff. Limited to a maximum of 80 characters.
#define HELPER_DD_TRROLE _bstr_t("TRROLE") // Tells whether the equipment is to be the primary or secondary transfer partner.
#define HELPER_DD_TRSPER _bstr_t("TRSPER") // A Trace Object attribute which holds the value for sampling interval time.
#define HELPER_DD_TRTYPE _bstr_t("TRTYPE") // Tells whether the equipment is to be an active or passive participant in the transfer.
#define HELPER_DD_TSIP _bstr_t("TSIP") // Transfer status of input port, 1 byte.
#define HELPER_DD_TSOP _bstr_t("TSOP") // Transfer status of output port, 1 byte.
#define HELPER_DD_TTC _bstr_t("TTC") // Time to completion.
#define HELPER_DD_TYPEID _bstr_t("TYPEID") // Identifier of the Type of the recipe.
#define HELPER_DD_UID _bstr_t("UID") // Contains a unique identifier for a PDE.
#define HELPER_DD_ULIM _bstr_t("ULIM") // Upper limit for numeric value.
#define HELPER_DD_UNFLEN _bstr_t("UNFLEN") // Unformatted Process Program Length.
#define HELPER_DD_UNITS _bstr_t("UNITS")
#define HELPER_DD_UPPERDB _bstr_t("UPPERDB")
#define HELPER_DD_USAGE _bstr_t("USAGE")
#define HELPER_DD_V _bstr_t("V")
#define HELPER_DD_VERID _bstr_t("VERID") // Optional unique identifier of recipes.
#define HELPER_DD_VERIFYDEPTH _bstr_t("VERIFYDEPTH") // Selects whether to check only the target PDE or all associated PDEs within a multi-part recipe.
#define HELPER_DD_VERIFYRSPSTAT _bstr_t("VERIFYRSPSTAT") // Verification result.
#define HELPER_DD_VERIFYSUCCESS _bstr_t("VERIFYSUCCESS") // Boolean.
#define HELPER_DD_VERIFYTYPE _bstr_t("VERIFYTYPE") // Choice of the type of verification to perform.
#define HELPER_DD_VID _bstr_t("VID")
#define HELPER_DD_VLAACK _bstr_t("VLAACK")
#define HELPER_DD_XDIES _bstr_t("XDIES") // X-axis die size (index).
#define HELPER_DD_XYPOS _bstr_t("XYPOS") // X and Y Coordinate Position. Must be in (X,Y) order.
#define HELPER_DD_YDIES _bstr_t("YDIES") // Y-axis die size (index).

#endif
