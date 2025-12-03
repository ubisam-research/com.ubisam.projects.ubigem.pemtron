using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    #region GemDriverError
    /// <summary>
    /// GEM driver 처리 결과입니다.
    /// </summary>
    public enum GemDriverError
    {
        /// <summary>
        /// 정상 상태입니다.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 알 수 없는 상태입니다.
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// Driver name이 존재하지 않습니다.
        /// </summary>
        NotExistDriverName = 2,
        /// <summary>
        /// 환경 설정 파일이 존재하지 않습니다.
        /// </summary>
        NotExistFile = 3,
        /// <summary>
        /// 환경 설정 load에 실패했습니다.
        /// </summary>
        FileLoadFailed = 4,
        /// <summary>
        /// 환경 설정 저장에 실패했습니다.
        /// </summary>
        FileSaveFailed = 5,
        /// <summary>
        /// 올바르지 않은 환경 설정 값입니다.
        /// </summary>
        InvalidConfiguration = 6,
        /// <summary>
        /// 이미 Connected 상태입니다.
        /// </summary>
        AlreadyConnected = 7,
        /// <summary>
        /// Socket 에러 상태입니다.
        /// </summary>
        SocketException = 8,
        /// <summary>
        /// License 검증 실패입니다.
        /// </summary>
        LicenseVerificationFailed = 9,
        /// <summary>
        /// 연결 해지 상태입니다.
        /// </summary>
        Disconnected = 11,
        /// <summary>
        /// HOST offline 모드입니다.
        /// </summary>
        ControlStateIsOffline = 12,
        /// <summary>
        /// 이전과 동일한 상태입니다.
        /// </summary>
        SameState = 13,
        /// <summary>
        /// 정의되지 않은 상태입니다.
        /// </summary>
        Undefined = 14,
        /// <summary>
        /// Disable 상태입니다.
        /// </summary>
        Disabled = 15,
        /// <summary>
        /// HSMS driver 에러입니다.
        /// </summary>
        HSMSDriverError = 16,
        /// <summary>
        /// HSMS driver disconnected 상태입니다.
        /// </summary>
        HSMSDriverDisconnected = 17,
        /// <summary>
        /// Not communicating 상태입니다.
        /// </summary>
        NotCommunicating = 18,
        /// <summary>
        /// Message 생성 실패입니다.
        /// </summary>
        MessageMakeFailed = 19,
        /// <summary>
        /// 예외 발생 상태입니다.
        /// </summary>
        Exception = 20,
        /// <summary>
        /// Mismatch 발생 상태입니다.
        /// </summary>
        Mismatch = 21,
        /// <summary>
        /// HOST에서 Denied를 수신했습니다.
        /// </summary>
        HostDenied = 22,
        /// <summary>
        /// Format과 Value가 맞지 않습니다.
        /// </summary>
        InvalidFormat = 23
    }
    #endregion
    #region MessageMakeError
    /// <summary>
    /// Message 생성 결과입니다.
    /// </summary>
    public enum MessageMakeError
    {
        /// <summary>
        /// 정상 상태입니다.
        /// </summary>
        Ok,
        /// <summary>
        /// 예외 발생 상태입니다.
        /// </summary>
        Exception,
        /// <summary>
        /// 정의 되지 않은 Message입니다.
        /// </summary>
        NoDefined,
        /// <summary>
        /// 
        /// </summary>
        Disabled,
        /// <summary>
        /// Format과 Value가 맞지 않습니다.
        /// </summary>
        InvalidFormat
    }
    #endregion
    #region AnalyzeMessageError
    /// <summary>
    /// Message 분석 결과입니다.
    /// </summary>
    public enum AnalyzeMessageError
    {
        /// <summary>
        /// 정상 상태입니다.
        /// </summary>
        Ok,
        /// <summary>
        /// 정의 되지 않은 상태입니다.
        /// </summary>
        Undefined,
        /// <summary>
        /// 알 수 없는 상태입니다.
        /// </summary>
        Unknown,
        /// <summary>
        /// 예외 발생 상태입니다.
        /// </summary>
        Exception
    }
    #endregion
    #region VariableType
    /// <summary>
    /// Variable의 종류입니다.
    /// </summary>
    public enum VariableType
    {
        /// <summary>
        /// Equipment Constant입니다.
        /// </summary>
        ECV = 0,
        /// <summary>
        /// Status Variable입니다.
        /// </summary>
        SV = 1,
        /// <summary>
        /// Data Variable입니다.
        /// </summary>
        DVVAL = 2
    }
    #endregion
    #region CommunicationState
    /// <summary>
    /// Communication state입니다.
    /// </summary>
    public enum CommunicationState
    {
        /// <summary>
        /// 
        /// </summary>
        Disabled = 1,
        /// <summary>
        /// 
        /// </summary>
        Enabled = 2,
        /// <summary>
        /// 
        /// </summary>
        NotCommunication = 3,
        /// <summary>
        /// 
        /// </summary>
        WaitCRFromHost = 4,
        /// <summary>
        /// 
        /// </summary>
        WaitDelay = 5,
        /// <summary>
        /// 
        /// </summary>
        WaitCRA = 6,
        /// <summary>
        /// 
        /// </summary>
        Communicating = 7
    }
    #endregion
    #region ControlState
    /// <summary>
    /// Control state입니다.
    /// </summary>
    public enum ControlState
    {
        /// <summary>
        /// 
        /// </summary>
        EquipmentOffline = 1,
        /// <summary>
        /// 
        /// </summary>
        AttemptOnline = 2,
        /// <summary>
        /// 
        /// </summary>
        HostOffline = 3,
        /// <summary>
        /// 
        /// </summary>
        OnlineLocal = 4,
        /// <summary>
        /// 
        /// </summary>
        OnlineRemote = 5
    }
    #endregion
    #region SpoolState
    /// <summary>
    /// Spool State입니다.
    /// </summary>
    public enum SpoolState
    {
        /// <summary>
        /// Default입니다. Spool active 후 Spooling message가 없을 경우 Inactive 상태가 됩니다.
        /// </summary>
        Inactive = 0,
        /// <summary>
        /// 
        /// </summary>
        ActievLoadNotFull = 1,
        /// <summary>
        /// 
        /// </summary>
        ActievLoadFull = 2,
        /// <summary>
        /// 
        /// </summary>
        ActiveUnloadPurge = 3,
        /// <summary>
        /// 
        /// </summary>
        ActiveUnloadTransmit = 4,
        /// <summary>
        /// 
        /// </summary>
        ActiveUnloadNoOutput = 5
    }
    #endregion

    #region COMMACK
    /// <summary>
    /// Establish Communications Acknowledge Code, 1 byte.
    /// </summary>
    public enum COMMACK
    {
        /// <summary>
        /// Accepted
        /// </summary>
        Accepted = 0,
        /// <summary>
        /// Denied. Try Again
        /// </summary>
        Denied_TryAgain = 1
    }
    #endregion
    #region EAC
    /// <summary>
    /// Equipment acknowledge code, 1 byte
    /// </summary>
    public enum EAC
    {
        /// <summary>
        /// Acknowledge
        /// </summary>
        Acknowledge = 0,
        /// <summary>
        /// Denied. At least one constant does not exist
        /// </summary>
        Denied_DoesNotExist = 1,
        /// <summary>
        /// Denied. Busy
        /// </summary>
        Denied_Busy = 2,
        /// <summary>
        /// Denied. At least one constant out of range
        /// </summary>
        Denied_OutOfRange = 3,
        /// <summary>
        /// Other equipment-specific error
        /// </summary>
        Denied_Etc = 4
    }
    #endregion
    #region PPGNT
    /// <summary>
    /// Process program grant status
    /// </summary>
    public enum PPGNT
    {
        /// <summary>
        /// OK
        /// </summary>
        OK = 0,
        /// <summary>
        /// Already have
        /// </summary>
        AlreadyHave = 1,
        /// <summary>
        /// No space
        /// </summary>
        NoSpace = 2,
        /// <summary>
        /// Invalid PPID
        /// </summary>
        InvalidPPID = 3,
        /// <summary>
        /// Busy, try later
        /// </summary>
        BusyTryLater = 4,
        /// <summary>
        /// Will not accept
        /// </summary>
        WillNotAccept = 5
    }
    #endregion
    #region ACKC6
    /// <summary>
    /// Acknowledge code
    /// </summary>
    public enum ACKC6
    {
        /// <summary>
        /// Accepted
        /// </summary>
        Accepted = 0,
        /// <summary>
        /// Error, not accepted
        /// </summary>
        ErrorNotAccepted = 1
    }
    #endregion
    #region ACKC7
    /// <summary>
    /// Acknowledge code
    /// </summary>
    public enum ACKC7
    {
        /// <summary>
        /// Accepted
        /// </summary>
        Accepted = 0,
        /// <summary>
        /// Permission not granted
        /// </summary>
        PermissionNotGranted = 1,
        /// <summary>
        /// Length error
        /// </summary>
        LengthError = 2,
        /// <summary>
        /// Matrix overflow
        /// </summary>
        MatrixOverflow = 3,
        /// <summary>
        /// PPID not found
        /// </summary>
        PPIDNotFound = 4,
        /// <summary>
        /// Mode unsupported
        /// </summary>
        ModeUnsupported = 5,
        /// <summary>
        /// Command will be performed with completion signaled later
        /// </summary>
        CommandWillBePerformedWithCompletionSignaledLater = 6
    }
    #endregion
    #region ACKC7A
    /// <summary>
    /// Acknowledge Code
    /// </summary>
    public enum ACKC7A
    {
        /// <summary>
        /// Accepted
        /// </summary>
        Accepted = 0,
        /// <summary>
        ///  MDLN is inconsistent
        /// </summary>
        MDLNIsInconsistent = 1,
        /// <summary>
        /// SOFTREV is inconsistent
        /// </summary>
        SOFTREVIsInconsistent = 2,
        /// <summary>
        /// Invalid CCODE
        /// </summary>
        InvalidCCODE = 3,
        /// <summary>
        /// Invalid PPARM value
        /// </summary>
        InvalidPPARMValue = 4,
        /// <summary>
        /// Other error(described by ERRW7)
        /// </summary>
        OtherError = 5
    }
    #endregion
    #region ACKC10
    /// <summary>
    /// Acknowledge code
    /// </summary>
    public enum ACKC10
    {
        /// <summary>
        /// Accepted for display 
        /// </summary>
        AcceptedForDisplay = 0,
        /// <summary>
        /// Message will not be displayed
        /// </summary>
        MessageWillNotBeDisplayed = 1,
        /// <summary>
        /// Terminal not available
        /// </summary>
        TerminalNotAvailable = 2
    }
    #endregion
    #region DRACK 
    /// <summary>
    /// Define Report Acknowledge Code, 1 byte.
    /// </summary>
    public enum DRACK
    {
        /// <summary>
        /// Accept
        /// </summary>
        Accept = 0,
        /// <summary>
        /// Denied. Insufficient space
        /// </summary>
        Denied_InsufficientSpace = 1,
        /// <summary>
        /// Denied. Invalid format
        /// </summary>
        Denied_InvalidFormat = 2,
        /// <summary>
        /// Denied. At least one RPTID already defined
        /// </summary>
        Denied_AtLeastOneRPTIDAlreadyDefined = 3,
        /// <summary>
        /// Denied. At least VID does not exist
        /// </summary>
        Denied_AtLeastVIDDoesNotExist = 4
    }
    #endregion
    #region LRACK 
    /// <summary>
    /// Link Report Acknowledge Code, 1 byte.
    /// </summary>
    public enum LRACK
    {
        /// <summary>
        /// Accept
        /// </summary>
        Accept = 0,
        /// <summary>
        /// Denied. Insufficient space
        /// </summary>
        Denied_InsufficientSpace = 1,
        /// <summary>
        /// Denied. Invalid format
        /// </summary>
        Denied_InvalidFormat = 2,
        /// <summary>
        /// Denied. At least one CEID link already defined
        /// </summary>
        Denied_AtLeastOneCEIDLinkAlreadyDefined = 3,
        /// <summary>
        /// Denied. At least CEID does not exist
        /// </summary>
        Denied_AtLeastCEIDDoesNotExist = 4,
        /// <summary>
        /// Denied. At least RPTID does not exist
        /// </summary>
        Denied_AtLeastRPTIDDoesNotExist = 5
    }
    #endregion
    #region ERACK
    /// <summary>
    /// Enable/Disable Event Report. Acknowledge Code, 1 byte.
    /// </summary>
    public enum ERACK
    {
        /// <summary>
        /// Accept
        /// </summary>
        Accept = 0,
        /// <summary>
        /// Denied. Insufficient space
        /// </summary>
        Denied_AtLeastCEIDDoesNotExist = 1
    }
    #endregion
    #region GRNT1
    /// <summary>
    /// Grant code, 1 byte.
    /// </summary>
    public enum GRNT1
    {
        /// <summary>
        /// Positive response, transfer ok
        /// </summary>
        PositiveResponse_TransferOk = 0,
        /// <summary>
        /// Busy, try again
        /// </summary>
        Busy_TryAgain = 1,
        /// <summary>
        /// No space
        /// </summary>
        NoSpace = 2,
        /// <summary>
        /// Map too large
        /// </summary>
        MapTooLarge = 3,
        /// <summary>
        /// Duplicate ID
        /// </summary>
        DuplicateID = 4,
        /// <summary>
        /// Material ID not found
        /// </summary>
        MaterialIDNotFound = 5,
        /// <summary>
        /// Unknown map format
        /// </summary>
        UnknownMapFormat = 6
    }
    #endregion
    #region MDACK
    /// <summary>
    /// Map data acknowledge.
    /// </summary>
    public enum MDACK
    {
        /// <summary>
        /// Map received
        /// </summary>
        MapReceived = 0,
        /// <summary>
        /// Format error
        /// </summary>
        FormatError = 1,
        /// <summary>
        /// No ID match
        /// </summary>
        NoIDMatch = 2,
        /// <summary>
        ///  Abort/discard map
        /// </summary>
        AbortDiscardMap = 3
    }
    #endregion
    #region TIAACK
    /// <summary>
    /// Equipment acknowledgement code, 1 byte. 
    /// </summary>
    public enum TIAACK
    {
        /// <summary>
        /// Everything correct
        /// </summary>
        EverythingCorrect = 0,
        /// <summary>
        ///  Too many SVIDs 
        /// </summary>
        TooManySVIDs = 1,
        /// <summary>
        /// No more traces allowed 
        /// </summary>
        NoMoreTracesAllowed = 2,
        /// <summary>
        /// Invalid period
        /// </summary>
        InvalidPeriod = 3,
        /// <summary>
        ///  Unknown SVID specified
        /// </summary>
        UnknownSVIDSpecified = 4,
        /// <summary>
        /// Invalid REPGSZ 
        /// </summary>
        InvalidREPGSZ = 5
    }
    #endregion
    #region TIACK
    /// <summary>
    /// Time Acknowledge Code입니다.
    /// </summary>
    public enum TIACK
    {
        /// <summary>
        /// OK입니다.
        /// </summary>
        OK = 0,
        /// <summary>
        /// Error, not done입니다.
        /// </summary>
        Error = 1
    }
    #endregion
    #region OFLACK
    /// <summary>
    /// Acknowledge code for OFF-LINE request
    /// </summary>
    public enum OFLACK
    {
        /// <summary>
        /// OFF-LINE Acknowledge
        /// </summary>
        OfflineAcknowledge = 0
    }
    #endregion
    #region ONLACK
    /// <summary>
    /// Acknowledge code for ONLINE request.
    /// </summary>
    public enum ONLACK
    {
        /// <summary>
        /// ON-LINE Accepted.
        /// </summary>
        OnlineAccepted = 0,
        /// <summary>
        /// ON-LINE Not Allowed.
        /// </summary>
        OnlineNotAllowed = 1,
        /// <summary>
        /// Equipment Already ONLINE.
        /// </summary>
        EquipmentAlreadyOnline = 2
    }
    #endregion
    #region OBJACK
    /// <summary>
    /// Acknowledge code.
    /// </summary>
    public enum OBJACK
    {
        /// <summary>
        /// Successful completion of requested data
        /// </summary>
        Successful = 0,
        /// <summary>
        /// Error
        /// </summary>
        Error = 1
    }
    #endregion
    #region SDACK
    /// <summary>
    /// Acknowledge code.
    /// </summary>
    public enum SDACK
    {
        /// <summary>
        /// Received data
        /// </summary>
        Successful = 0,
        /// <summary>
        /// Error
        /// </summary>
        Error = 1
    }
    #endregion
    #region PreDefinedCE
    /// <summary>
    /// GEM driver에 기본 등록 된 collection event입니다.
    /// </summary>
    public enum PreDefinedCE
    {
        /// <summary>
        /// Operator actuates OFF-LINE switch.
        /// </summary>
        Offline,
        /// <summary>
        /// Equipment accepts 'Set OFF-LINE' message from host (S1, F15).
        /// </summary>
        OfflineOnHost,
        /// <summary>
        /// Operator sets front panel switch to LOCAL.
        /// </summary>
        OnlineLocal,
        /// <summary>
        /// Operator sets front panel switch to REMOTE.
        /// </summary>
        OnlineRemote,
        /// <summary>
        /// 
        /// </summary>
        ControlStateChanged,
        /// <summary>
        /// 
        /// </summary>
        EquipmentConstantChanged,
        /// <summary>
        /// 
        /// </summary>
        EquipmentConstantChangedByHost,
        /// <summary>
        /// 
        /// </summary>
        ProcessProgramChanged,
        /// <summary>
        /// 
        /// </summary>
        ProcessStateChanged,
        /// <summary>
        /// 
        /// </summary>
        AlarmSet,
        /// <summary>
        /// 
        /// </summary>
        AlarmClear,
        /// <summary>
        /// 
        /// </summary>
        LimitMonitoring




        ///// <summary>
        ///// 
        ///// </summary>
        //ProcessProgramSeleted,
        ///// <summary>
        ///// 
        ///// </summary>
        //MaterialReceived,
        ///// <summary>
        ///// 
        ///// </summary>
        //MaterialRemoved,
        ///// <summary>
        ///// 
        ///// </summary>
        //SpoolActivated,
        ///// <summary>
        ///// 
        ///// </summary>
        //SpoolDeactivated,
        ///// <summary>
        ///// 
        ///// </summary>
        //SpoolTransmitFailure,
        ///// <summary>
        ///// 
        ///// </summary>
        //MessageRecognition,
        ///// <summary>
        ///// 
        ///// </summary>
        //AlarmSetBase,
        ///// <summary>
        ///// 
        ///// </summary>
        //AlarmClearBase,
        ///// <summary>
        ///// 
        ///// </summary>
        //LimitMonitoringBase
    }
    #endregion
    #region PreDefinedECV
    /// <summary>
    /// GEM driver에 기본 등록 된 equipment constant입니다.
    /// </summary>
    public enum PreDefinedECV
    {
        /// <summary>
        /// Initial communications state when the system start up.
        /// </summary>
        InitCommunicationState,
        /// <summary>
        /// The length of time, in seconds, of the interval between attempts to send S1F13 when establishing communications.
        /// </summary>
        EstablishCommunicationsTimeout,
        /// <summary>
        /// 
        /// </summary>
        HeartbeatRate,
        /// <summary>
        /// 
        /// </summary>
        AreYouThereTimeout,
        /// <summary>
        /// The setting of this ECV controls whether the equipment shall use use the variable item CLOCK and the data items STIME, TIMESTAMP, and TIME in 12-byte, 16-byte.(0=12byte format, 1=16byte format)
        /// </summary>
        TimeFormat,
        /// <summary>
        /// Initial control state when the system start up.
        /// </summary>
        InitControlState,
        /// <summary>
        /// 
        /// </summary>
        OffLineSubState,
        /// <summary>
        /// 
        /// </summary>
        OnLineFailState,
        /// <summary>
        /// 
        /// </summary>
        OnLineSubState,
        /// <summary>
        /// 
        /// </summary>
        DeviceID,
        /// <summary>
        /// 
        /// </summary>
        IPAddress,
        /// <summary>
        /// 
        /// </summary>
        PortNumber,
        /// <summary>
        /// 
        /// </summary>
        ActiveMode,
        /// <summary>
        /// 
        /// </summary>
        T3Timeout,
        /// <summary>
        /// 
        /// </summary>
        T5Timeout,
        /// <summary>
        /// 
        /// </summary>
        T6Timeout,
        /// <summary>
        /// 
        /// </summary>
        T7Timeout,
        /// <summary>
        /// 
        /// </summary>
        T8Timeout,
        /// <summary>
        /// 
        /// </summary>
        LinkTestInterval,


        //MaxSpoolTransmit,
        //OverWriteSpool,
        //EnableSpooling,
        //Maker,
        //MaxSpoolMsg,
        //RetryLimit,
    }
    #endregion
    #region HCACK
    /// <summary>
    /// Host command ack입니다.
    /// </summary>
    public enum HCACK
    {
        /// <summary>
        /// Acknowledge, command has been performed
        /// </summary>
        Acknowledge = 0,
        /// <summary>
        /// Command does not exist
        /// </summary>
        CommandDoesNotExist = 1,
        /// <summary>
        /// Cannot perform now
        /// </summary>
        CannotPerformNow = 2,
        /// <summary>
        /// At least one parameter is invalid
        /// </summary>
        ParameterIsInvalid = 3,
        /// <summary>
        /// Acknowledge, command will be performed with completion signaled later by an event
        /// </summary>
        Acknowledge_PerformedWithCompletionSignal = 4,
        /// <summary>
        /// Rejected, Already in Desired Condition
        /// </summary>
        Rejected = 5,
        /// <summary>
        /// No such object exists
        /// </summary>
        NoSuchObjectExists = 6
    }
    #endregion
    #region CPACK
    /// <summary>
    /// Host command parameter ack입니다.
    /// </summary>
    public enum CPACK
    {
        /// <summary>
        /// No error:S2F50 only
        /// </summary>
        NoError = 0,
        /// <summary>
        /// Parameter Name(CPNAME) does not exist
        /// </summary>
        ParameterNameDoesNotExist = 1,
        /// <summary>
        /// Illegal Value specified for CPVAL
        /// </summary>
        IllegalValueSpecifiedForCPVAL = 2,
        /// <summary>
        /// Illegal Format specified for CPVAL
        /// </summary>
        IllegalFormatSpecifiedForCPVAL = 3,
        /// <summary>
        /// Other equipment-specific error
        /// </summary>
        OtherEquipmentspecificError = 4
    }
    #endregion
    #region VariableUpdateType
    /// <summary>
    /// Variable Update 발생 Type입니다.
    /// </summary>
    public enum VariableUpdateType
    {
        /// <summary>
        /// Selected Equipment Status Request(S1F3) Reply 전 Update입니다.
        /// </summary>
        S1F3SelectedEquipmentStatusRequest = 0,
        /// <summary>
        /// Event Report Send(S6F11) 전 Update입니다.
        /// </summary>
        S6F11EventReportSend = 1,
        /// <summary>
        /// Individual Report Request(S6F19) Reply 전 Update입니다.
        /// </summary>
        S6F19IndividualReportRequest = 2
    }
    #endregion

    #region PreDefinedDataDictinary
    /// <summary>
    /// GEM driver에 기본 등록 된 data dictionary입니다.
    /// </summary>
    public enum PreDefinedDataDictinary
    {
        /// <summary>
        /// 
        /// </summary>
        ABS,
        /// <summary>
        /// 
        /// </summary>
        ACCESSMODE,
        /// <summary>
        /// 
        /// </summary>
        ACDS,
        /// <summary>
        /// 
        /// </summary>
        ACKA,
        /// <summary>
        /// 
        /// </summary>
        ACKC10,
        /// <summary>
        /// 
        /// </summary>
        ACKC5,
        /// <summary>
        /// 
        /// </summary>
        ACKC6,
        /// <summary>
        /// 
        /// </summary>
        ACKC7,
        /// <summary>
        /// 
        /// </summary>
        ACKC7A,
        /// <summary>
        /// 
        /// </summary>
        ALCD,
        /// <summary>
        /// 
        /// </summary>
        ALED,
        /// <summary>
        /// 
        /// </summary>
        ALID,
        /// <summary>
        /// 
        /// </summary>
        ALTX,
        /// <summary>
        /// 
        /// </summary>
        ATTRDATA,
        /// <summary>
        /// 
        /// </summary>
        ATTRID,
        /// <summary>
        /// 
        /// </summary>
        ATTRRELN,
        /// <summary>
        /// 
        /// </summary>
        BATCHLOCID,
        /// <summary>
        /// 
        /// </summary>
        BATCHLOCIDVALUE,
        /// <summary>
        /// 
        /// </summary>
        BCEQU,
        /// <summary>
        /// 
        /// </summary>
        BINLT,
        /// <summary>
        /// 
        /// </summary>
        CAACK,
        /// <summary>
        /// 
        /// </summary>
        CAPACITY,
        /// <summary>
        /// 
        /// </summary>
        CARRIERACCESSINGSTATUS,
        /// <summary>
        /// 
        /// </summary>
        CARRIERACTION,
        /// <summary>
        /// 
        /// </summary>
        CARRIERID,
        /// <summary>
        /// 
        /// </summary>
        CARRIERIDSTATUS,
        /// <summary>
        /// 
        /// </summary>
        CARRIERINPUTID,
        /// <summary>
        /// 
        /// </summary>
        CARRIERINPUTSPEC,
        /// <summary>
        /// 
        /// </summary>
        CARRIERSPEC,
        /// <summary>
        /// 
        /// </summary>
        CATTRDATA,
        /// <summary>
        /// 
        /// </summary>
        CATTRID,
        /// <summary>
        /// 
        /// </summary>
        CCODE,
        /// <summary>
        /// 
        /// </summary>
        CEED,
        /// <summary>
        /// 
        /// </summary>
        CEID,
        /// <summary>
        /// 
        /// </summary>
        CENAME,
        /// <summary>
        /// 
        /// </summary>
        CEPACK,
        /// <summary>
        /// 
        /// </summary>
        COMMACK,
        /// <summary>
        /// 
        /// </summary>
        COLCT,
        /// <summary>
        /// 
        /// </summary>
        CPACK,
        /// <summary>
        /// 
        /// </summary>
        CPNAME,
        /// <summary>
        /// 
        /// </summary>
        CPVAL,
        /// <summary>
        /// 
        /// </summary>
        CTLJOBCMD,
        /// <summary>
        /// 
        /// </summary>
        CTLJOBID,
        /// <summary>
        /// 
        /// </summary>
        DATA,
        /// <summary>
        /// 
        /// </summary>
        DATACOLLECTIONPLAN,
        /// <summary>
        /// 
        /// </summary>
        DATAID,
        /// <summary>
        /// 
        /// </summary>
        DATALENGTH,
        /// <summary>
        /// 
        /// </summary>
        DATASEG,
        /// <summary>
        /// 
        /// </summary>
        DATLC,
        /// <summary>
        /// 
        /// </summary>
        DESTCARRIERID,
        /// <summary>
        /// 
        /// </summary>
        DISABLEEVENTS,
        /// <summary>
        /// 
        /// </summary>
        DRACK,
        /// <summary>
        /// 
        /// </summary>
        DSID,
        /// <summary>
        /// 
        /// </summary>
        DSPER,
        /// <summary>
        /// 
        /// </summary>
        DUTMS,
        /// <summary>
        /// 
        /// </summary>
        DVVAL,
        /// <summary>
        /// 
        /// </summary>
        DVVALNAME,
        /// <summary>
        /// 
        /// </summary>
        EAC,
        /// <summary>
        /// 
        /// </summary>
        ECDEF,
        /// <summary>
        /// 
        /// </summary>
        ECID,
        /// <summary>
        /// 
        /// </summary>
        ECMAX,
        /// <summary>
        /// 
        /// </summary>
        ECMIN,
        /// <summary>
        /// 
        /// </summary>
        ECNAME,
        /// <summary>
        /// 
        /// </summary>
        ECV,
        /// <summary>
        /// 
        /// </summary>
        EDID,
        /// <summary>
        /// 
        /// </summary>
        ERACK,
        /// <summary>
        /// 
        /// </summary>
        ERRCODE,
        /// <summary>
        /// 
        /// </summary>
        ERRTEXT,
        /// <summary>
        /// 
        /// </summary>
        ERRW7,
        /// <summary>
        /// 
        /// </summary>
        FCNID,
        /// <summary>
        /// 
        /// </summary>
        FFROT,
        /// <summary>
        /// 
        /// </summary>
        FNLOC,
        /// <summary>
        /// 
        /// </summary>
        GRANT,
        /// <summary>
        /// 
        /// </summary>
        GRANT6,
        /// <summary>
        /// 
        /// </summary>
        HCACK,
        /// <summary>
        /// 
        /// </summary>
        IDTYP,
        /// <summary>
        /// 
        /// </summary>
        LENGTH,
        /// <summary>
        /// 
        /// </summary>
        LIMITACK,
        /// <summary>
        /// 
        /// </summary>
        LIMITID,
        /// <summary>
        /// 
        /// </summary>
        LIMITMAX,
        /// <summary>
        /// 
        /// </summary>
        LIMITMIN,
        /// <summary>
        /// 
        /// </summary>
        LINKID,
        /// <summary>
        /// 
        /// </summary>
        LOCID,
        /// <summary>
        /// 
        /// </summary>
        LOTID,
        /// <summary>
        /// 
        /// </summary>
        LOWERDB,
        /// <summary>
        /// 
        /// </summary>
        LRACK,
        /// <summary>
        /// 
        /// </summary>
        LVACK,
        /// <summary>
        /// 
        /// </summary>
        MAPER,
        /// <summary>
        /// 
        /// </summary>
        MAPFT,
        /// <summary>
        /// 
        /// </summary>
        MATERIALSTATUS,
        /// <summary>
        /// 
        /// </summary>
        MDLN,
        /// <summary>
        /// 
        /// </summary>
        MEXP,
        /// <summary>
        /// 
        /// </summary>
        MF,
        /// <summary>
        /// 
        /// </summary>
        MHEAD,
        /// <summary>
        /// 
        /// </summary>
        MID,
        /// <summary>
        /// 
        /// </summary>
        MLCL,
        /// <summary>
        /// 
        /// </summary>
        MTRLOUTSPEC,
        /// <summary>
        /// 
        /// </summary>
        NULBC,
        /// <summary>
        /// 
        /// </summary>
        OBJACK,
        /// <summary>
        /// 
        /// </summary>
        OBJCMD,
        /// <summary>
        /// 
        /// </summary>
        OBJID,
        /// <summary>
        /// 
        /// </summary>
        OBJSPEC,
        /// <summary>
        /// 
        /// </summary>
        OBJTOKEN,
        /// <summary>
        /// 
        /// </summary>
        OBJTYPE,
        /// <summary>
        /// 
        /// </summary>
        OFLACK,
        /// <summary>
        /// 
        /// </summary>
        ONLACK,
        /// <summary>
        /// 
        /// </summary>
        OPID,
        /// <summary>
        /// 
        /// </summary>
        ORDERVALUE,
        /// <summary>
        /// 
        /// </summary>
        ORLOC,
        /// <summary>
        /// 
        /// </summary>
        OUTPUTRULEVALUE,
        /// <summary>
        /// 
        /// </summary>
        PARAMNAME,
        /// <summary>
        /// 
        /// </summary>
        PARAMVAL,
        /// <summary>
        /// 
        /// </summary>
        PAUSEEVENT,
        /// <summary>
        /// 
        /// </summary>
        PFCD,
        /// <summary>
        /// 
        /// </summary>
        PORTACTION,
        /// <summary>
        /// 
        /// </summary>
        PPARM,
        /// <summary>
        /// 
        /// </summary>
        PPBODY,
        /// <summary>
        /// 
        /// </summary>
        PPGNT,
        /// <summary>
        /// 
        /// </summary>
        PPID,
        /// <summary>
        /// 
        /// </summary>
        PPNAME,
        /// <summary>
        /// 
        /// </summary>
        PPVALUE,
        /// <summary>
        /// 
        /// </summary>
        PRCMDNAME,
        /// <summary>
        /// 
        /// </summary>
        PREVENTID,
        /// <summary>
        /// 
        /// </summary>
        PRJOBID,
        /// <summary>
        /// 
        /// </summary>
        PRJOBMILESTONE,
        /// <summary>
        /// 
        /// </summary>
        PRJOBSPACE,
        /// <summary>
        /// 
        /// </summary>
        PRMTRLORDER,
        /// <summary>
        /// 
        /// </summary>
        PRAXI,
        /// <summary>
        /// 
        /// </summary>
        PRDCT,
        /// <summary>
        /// 
        /// </summary>
        PROCESSINGCTRLSPEC,
        /// <summary>
        /// 
        /// </summary>
        PROCESSINGJOBID,
        /// <summary>
        /// 
        /// </summary>
        PROCESSORDERMGMT,
        /// <summary>
        /// 
        /// </summary>
        PRPROCESSSTART,
        /// <summary>
        /// 
        /// </summary>
        PRRECIPEMETHOD,
        /// <summary>
        /// 
        /// </summary>
        PRSTATE,
        /// <summary>
        /// 
        /// </summary>
        PTN,
        /// <summary>
        /// 
        /// </summary>
        RCMD,
        /// <summary>
        /// 
        /// </summary>
        RCPPARNM,
        /// <summary>
        /// 
        /// </summary>
        RCPPARVAL,
        /// <summary>
        /// 
        /// </summary>
        RCPSPEC,
        /// <summary>
        /// 
        /// </summary>
        RECID,
        /// <summary>
        /// 
        /// </summary>
        REFP,
        /// <summary>
        /// 
        /// </summary>
        REPGSZ,
        /// <summary>
        /// 
        /// </summary>
        RPSEL,
        /// <summary>
        /// 
        /// </summary>
        RPTID,
        /// <summary>
        /// 
        /// </summary>
        ROWCT,
        /// <summary>
        /// 
        /// </summary>
        RSDA,
        /// <summary>
        /// 
        /// </summary>
        RSDC,
        /// <summary>
        /// 
        /// </summary>
        RSINF,
        /// <summary>
        /// 
        /// </summary>
        RSPACK,
        /// <summary>
        /// 
        /// </summary>
        RULENAME,
        /// <summary>
        /// 
        /// </summary>
        RULEVALUE,
        /// <summary>
        /// 
        /// </summary>
        SDBIN,
        /// <summary>
        /// 
        /// </summary>
        SEQNUM,
        /// <summary>
        /// 
        /// </summary>
        SHEAD,
        /// <summary>
        /// 
        /// </summary>
        SLOTID,
        /// <summary>
        /// 
        /// </summary>
        SLOTMAP,
        /// <summary>
        /// 
        /// </summary>
        SLOTMAPSTATUS,
        /// <summary>
        /// 
        /// </summary>
        SMPLN,
        /// <summary>
        /// 
        /// </summary>
        SOFTREV,
        /// <summary>
        /// 
        /// </summary>
        SPNAME,
        /// <summary>
        /// 
        /// </summary>
        SPVAL,
        /// <summary>
        /// 
        /// </summary>
        SRCCARRIERID,
        /// <summary>
        /// 
        /// </summary>
        SSACK,
        /// <summary>
        /// 
        /// </summary>
        STARTMETHOD,
        /// <summary>
        /// 
        /// </summary>
        STARTMETHODVALUE,
        /// <summary>
        /// 
        /// </summary>
        STATE,
        /// <summary>
        /// 
        /// </summary>
        STATUS,
        /// <summary>
        /// 
        /// </summary>
        STIME,
        /// <summary>
        /// 
        /// </summary>
        STRACK,
        /// <summary>
        /// 
        /// </summary>
        STRID,
        /// <summary>
        /// 
        /// </summary>
        STRP,
        /// <summary>
        /// 
        /// </summary>
        SUBSTID,
        /// <summary>
        /// 
        /// </summary>
        SUBSTLOCID,
        /// <summary>
        /// 
        /// </summary>
        SUBSTPOSINBATCH,
        /// <summary>
        /// 
        /// </summary>
        SUBSTPROCESSINGSTATE,
        /// <summary>
        /// 
        /// </summary>
        SUBSTRATECOUNT,
        /// <summary>
        /// 
        /// </summary>
        SUBSTSTATE,
        /// <summary>
        /// 
        /// </summary>
        SUBSTTYPE,
        /// <summary>
        /// 
        /// </summary>
        SUBSTUSAGE,
        /// <summary>
        /// 
        /// </summary>
        SV,
        /// <summary>
        /// 
        /// </summary>
        SVCACK,
        /// <summary>
        /// 
        /// </summary>
        SVCNAME,
        /// <summary>
        /// 
        /// </summary>
        SVID,
        /// <summary>
        /// 
        /// </summary>
        SVNAME,
        /// <summary>
        /// 
        /// </summary>
        TARGETSPEC,
        /// <summary>
        /// 
        /// </summary>
        TEXT,
        /// <summary>
        /// 
        /// </summary>
        TIAACK,
        /// <summary>
        /// 
        /// </summary>
        TIACK,
        /// <summary>
        /// 
        /// </summary>
        TID,
        /// <summary>
        /// 
        /// </summary>
        TIME,
        /// <summary>
        /// 
        /// </summary>
        TIMESTAMP,
        /// <summary>
        /// 
        /// </summary>
        TOTSMP,
        /// <summary>
        /// 
        /// </summary>
        TRID,
        /// <summary>
        /// 
        /// </summary>
        UNITS,
        /// <summary>
        /// 
        /// </summary>
        UPPERDB,
        /// <summary>
        /// 
        /// </summary>
        USAGE,
        /// <summary>
        /// 
        /// </summary>
        V,
        /// <summary>
        /// 
        /// </summary>
        VID,
        /// <summary>
        /// 
        /// </summary>
        VLAACK,
        /// <summary>
        /// 
        /// </summary>
        XDIES,
        /// <summary>
        /// 
        /// </summary>
        XYPOS,
        /// <summary>
        /// 
        /// </summary>
        YDIES
    }
    #endregion
    #region PreDefinedV
    /// <summary>
    /// GEM driver에 기본 등록 된 SV/DVVAL입니다.
    /// </summary>
    public enum PreDefinedV
    {
        /// <summary>
        /// Value of internal clock in 12, 16 bytes, or Extended format as specified by the TimeFormat equipment constant value setting.
        /// </summary>
        Clock,
        /// <summary>
        ///  This status variable contains the code which identifies the current control state of the equipment.
        ///  When reported related to a control state transition, its value should represent the state current after the transition.
        /// </summary>
        ControlState,
        /// <summary>
        /// 
        /// </summary>
        PreviousControlState,
        /// <summary>
        /// The current processing state of the equipment.
        /// </summary>
        ProcessState,
        /// <summary>
        /// 
        /// </summary>
        PreviousProcessState,
        /// <summary>
        /// 
        /// </summary>
        ChangedECID,
        /// <summary>
        /// 
        /// </summary>
        ChangedECV,
        /// <summary>
        /// 
        /// </summary>
        ChangedECList,
        /// <summary>
        /// Equipment Model Type, 20 bytes max.
        /// </summary>
        MDLN,
        /// <summary>
        /// Software revision code 20 bytes maximum.
        /// </summary>
        SOFTREV,
        /// <summary>
        /// Alarm code byte.
        /// </summary>
        ALCD,
        /// <summary>
        /// Alarm identification.
        /// </summary>
        ALID,
        /// <summary>
        /// Alarm text limited to 120 characters.
        /// </summary>
        ALTX,
        /// <summary>
        /// 
        /// </summary>
        PPChangeName,
        /// <summary>
        /// 
        /// </summary>
        PPChangeStatus,



        //AlarmSet,
        //EventsEnabled,
        //AlarmsEnabled,
        //SpoolCountActual,
        //SpoolCountTotal,
        //SpoolFullTime,
        //SpoolStartTime,
        //SpoolStatus,
        //SpoolFull,
        //EventLimit,
        //LimitVariable,
        //OperatorCommand,
        //PPError,
        //TransitionType,
        //PortID,
        //CommState,
        //PreviousCommState
    }
    #endregion

    /// <summary>
    /// Command parameter type입니다.
    /// </summary>
    public enum CPType
    {
        /// <summary>
        /// Value로 구성된 command parameter입니다.
        /// </summary>
        A,
        /// <summary>
        /// Name(Fixed)/Value로 구성된 command parameter입니다.
        /// </summary>
        B,
        /// <summary>
        /// Name(Unfixed)/Value로 구성된 command parameter입니다.
        /// </summary>
        C,
    }
}