namespace UbiCom.Net.Structure
{
    /// <summary>
    /// HSMS 모드입니다.
    /// </summary>
    public enum HSMSMode
    {
        /// <summary>
        /// Passive 모드입니다.
        /// </summary>
        Passive,
        /// <summary>
        /// Active 모드입니다.
        /// </summary>
        Active
    }

    /// <summary>
    /// SECS 모드입니다.
    /// </summary>
    public enum SECSMode
    {
        /// <summary>
        /// SECS-I 모드입니다.
        /// </summary>
        SECS1,
        /// <summary>
        /// HSMS 모드입니다.
        /// </summary>
        HSMS
    }

    /// <summary>
    /// Device type입니다.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// HOST 모드입니다.
        /// </summary>
        Host,
        /// <summary>
        /// Equipment 모드입니다.
        /// </summary>
        Equipment
    }

    /// <summary>
    /// SECS message direction입니다.
    /// </summary>
    public enum SECSMessageDirection
    {
        /// <summary>
        /// HOST -> Equipment입니다.
        /// </summary>
        ToEquipment,
        /// <summary>
        /// Equipment -> HOST 입니다.
        /// </summary>
        ToHost,
        /// <summary>
        /// Equipment &lt;-> HOST 입니다.
        /// </summary>
        Both
    }

    /// <summary>
    /// SECS item format입니다.
    /// </summary>
    public enum SECSItemFormat
    {
        /// <summary>
        /// 초기값(사용하면 안됨)
        /// </summary>
        None = 0,
        /// <summary>
        /// List type입니다.
        /// </summary>
        L = 0x01,
        /// <summary>
        /// ASCII type입니다.
        /// </summary>
        A = 0x41,
        /// <summary>
        /// Binary type입니다.
        /// </summary>
        B = 0x21,
        /// <summary>
        /// Boolean type입니다.
        /// </summary>
        Boolean = 0x25,
        /// <summary>
        /// 1-byte integer (signed) type입니다.
        /// </summary>
        I1 = 0x65,
        /// <summary>
        /// 2-byte integer (signed) type입니다.
        /// </summary>
        I2 = 0x69,
        /// <summary>
        /// 4-byte integer (signed) type입니다.
        /// </summary>
        I4 = 0x71,
        /// <summary>
        /// 8-byte integer (signed) type입니다.
        /// </summary>
        I8 = 0x61,
        /// <summary>
        /// 1-byte integer (unsigned) type입니다.
        /// </summary>
        U1 = 0xa5,
        /// <summary>
        /// 2-byte integer (unsigned) type입니다.
        /// </summary>
        U2 = 0xa9,
        /// <summary>
        /// 4-byte integer (unsigned) type입니다.
        /// </summary>
        U4 = 0xb1,
        /// <summary>
        /// 8-byte integer (unsigned) type입니다.
        /// </summary>
        U8 = 0xa1,
        /// <summary>
        /// 4-byte floating point type입니다.
        /// </summary>
        F4 = 0x91,
        /// <summary>
        /// 8-byte floating point type입니다.
        /// </summary>
        F8 = 0x81,
        /// <summary>
        /// JIS-8 type입니다.
        /// </summary>
        J = 0x45,
        /// <summary>
        /// Any type입니다.
        /// </summary>
        X = 0xff
    }

    /// <summary>
    /// 연결 상태입니다.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// 알 수 없는 상태입니다.
        /// </summary>
        Unknown,
        /// <summary>
        /// Socket conneted 상태입니다.
        /// </summary>
        Connected,
        /// <summary>
        /// HSMS selected 상태입니다.
        /// </summary>
        Selected,
        /// <summary>
        /// Socket disconneted 상태입니다.
        /// </summary>
        Disconnected,
        /// <summary>
        /// HSMS deselected 상태입니다.
        /// </summary>
        Deselected
    }

    /// <summary>
    /// Logging 모드입니다.
    /// </summary>
    public enum LogMode
    {
        /// <summary>
        /// Logging 하지 않는 모드입니다.
        /// </summary>
        None,
        /// <summary>
        /// 시간 단위로 Logging하는 모드입니다.
        /// </summary>
        Hour,
        /// <summary>
        /// 일자 단위로 Logging하는 모드입니다.
        /// </summary>
        Day
    }

    /// <summary>
    /// Timeout 종류입니다.
    /// </summary>
    public enum TimeoutType
    {
        /// <summary>
        /// 
        /// </summary>
        T1,
        /// <summary>
        /// 
        /// </summary>
        T2,
        /// <summary>
        /// Reply timeout입니다.
        /// </summary>
        T3,
        /// <summary>
        /// 
        /// </summary>
        T4,
        /// <summary>
        /// Connect separation timeout입니다.
        /// </summary>
        T5,
        /// <summary>
        /// Transaction timeout입니다.
        /// </summary>
        T6,
        /// <summary>
        /// Not selected timeout입니다.
        /// </summary>
        T7,
        /// <summary>
        /// Network intercharacter timeout입니다.
        /// </summary>
        T8,
        /// <summary>
        /// Linktest timeout입니다.
        /// </summary>
        Linktest
    }

    /// <summary>
    /// SECS Message Header S-Type Code입니다.
    /// </summary>
    public enum ControlMessageType
    {
        /// <summary>
        /// Data message입니다.
        /// </summary>
        DataMessage = 0,
        /// <summary>
        /// Select request입니다.
        /// </summary>
        SelectRequest = 1,
        /// <summary>
        /// Select response입니다.
        /// </summary>
        SelectResponse = 2,
        /// <summary>
        /// Deselect request입니다.
        /// </summary>
        DeselectRequest = 3,
        /// <summary>
        /// Deselect response입니다.
        /// </summary>
        DeselectResponse = 4,
        /// <summary>
        /// Linktest request입니다.
        /// </summary>
        LinktestRequest = 5,
        /// <summary>
        /// Linktest response입니다.
        /// </summary>
        LinktestResponse = 6,
        /// <summary>
        /// Reject request입니다.
        /// </summary>
        RejectRequest = 7,
        /// <summary>
        /// Separate request입니다.
        /// </summary>
        SeparateRequest = 9
    }

    /// <summary>
    /// Select Response 시 Select Status입니다.
    /// </summary>
    public enum SelectStatus
    {
        /// <summary>
        /// Select가 성공적으로 완료된 상태입니다.
        /// </summary>
        Succeed = 0,
        /// <summary>
        /// 통신이 이미 Active된 상태입니다.(이전의 Select가 이미 통신 설정한 상태)
        /// </summary>
        AlreadySelected = 1,
        /// <summary>
        /// Select Requests를 받을 준비가 되어 있지 않은 상태입니다.
        /// </summary>
        ConnectionNotReady = 2,
        /// <summary>
        /// Entity가 이미 TCP/IP Connection을 분리 단계에 있는 상태입니다.
        /// </summary>
        ConnectExhaust = 3
    }

    /// <summary>
    /// Deselected 상태입니다.
    /// </summary>
    public enum DeselectStatus
    {
        /// <summary>
        /// Deselect가 성공적으로 완료된 상태입니다.
        /// </summary>
        Succeed = 0,
        /// <summary>
        /// 이전의 Deselect로 이미 HSMS 통신이 종료된 상태입니다.
        /// </summary>
        AlreadyDeselected = 1,
        /// <summary>
        /// Session이 여전히 응답 Entity로 사용 중에 있어서 끊을 수 없는 상태입니다.
        /// </summary>
        ConnectionBusy = 2
    }

    /// <summary>
    /// 수신 message의 reject reason code입니다.
    /// </summary>
    public enum RejectReasonCode
    {
        /// <summary>
        /// 알 수 없는 상태입니다.(Default Value)
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// SType 값이 표준에 정의 되지 않은 Message를 수신했을 경우입니다.
        /// </summary>
        STypeNotSupported = 1,
        /// <summary>
        /// PType 값이 표준에 정의 되지 않은 Message를 수신했을 경우입니다.
        /// </summary>
        PTypeNotSupported = 2,
        /// <summary>
        /// 부합된 Request Message를 보내지 않았는데, Response Control Message를 수신했을 경우입니다.
        /// </summary>
        TransactionNotOpen = 3,
        /// <summary>
        /// Selected 상태가 아닐 때, Data Message를 수신했을 경우입니다.
        /// </summary>
        EntityNotSelected = 4
    }

    /// <summary>
    /// SECS Driver 오류 type입니다.
    /// </summary>
    public enum DriverError
    {
        /// <summary>
        /// 정상 상태입니다.
        /// </summary>
        Ok,
        /// <summary>
        /// 알수 없는 오류입니다.
        /// </summary>
        Unknown,
        /// <summary>
        /// Driver 이름을 지정하지 않았습니다.
        /// </summary>
        NotExistDriverName,
        /// <summary>
        /// 파일이 존재하지 않습니다.
        /// </summary>
        NotExistFile,
        /// <summary>
        /// 환경 설정 파일 로드를 실패했습니다.
        /// </summary>
        FileLoadFailed,
        /// <summary>
        /// 환경 설정 파일 저장을 실패했습니다.
        /// </summary>
        FileSaveFailed,
        /// <summary>
        /// 올바르지 않은 환경 설정입니다.
        /// </summary>
        InvalidConfiguration,
        /// <summary>
        /// 이미 Connected 상태입니다.
        /// </summary>
        AlreadyConnected,
        /// <summary>
        /// Socket 에러입니다.
        /// </summary>
        SocketException,
        /// <summary>
        /// License 검증 실패입니다.
        /// </summary>
        LicenseVerificationFailed,
        /// <summary>
        /// Trial version입니다.(재기동 후 정상 진행 가능)
        /// </summary>
        TrialVersion
    }

    /// <summary>
    /// SECS Message 오류 type입니다.
    /// </summary>
    public enum MessageError
    {
        /// <summary>
        /// 정상 상태입니다.
        /// </summary>
        Ok,
        /// <summary>
        /// 알수 없는 오류입니다.
        /// </summary>
        Unknown,
        /// <summary>
        /// Driver 초기화를 하지 않았습니다.
        /// </summary>
        NoInitialize,
        /// <summary>
        /// Driver Connected 상태가 아닙니다.
        /// </summary>
        NoConnected,
        /// <summary>
        /// Driver Selected 상태가 아닙니다.
        /// </summary>
        NotSelected,
        /// <summary>
        /// Message Queue가 가득 찼습니다.(MAX Queue size는 Primary/Secondary 각각 100개 임)
        /// </summary>
        MessageQueueOverflow,
        /// <summary>
        /// System Byte가 중복되었습니다.
        /// </summary>
        DuplicateSystemBytes,
        /// <summary>
        /// 
        /// </summary>
        InvalidLength,
        /// <summary>
        /// Data(message)가 존재하지 않습니다.
        /// </summary>
        DataIsNull,
        /// <summary>
        /// Socket이 정상적으로 생성되지 않았습니다.
        /// </summary>
        SocketIsNull,
        /// <summary>
        /// Message 구조가 올바르지 않습니다.
        /// </summary>
        InvalidMessageStructure
    }

    /// <summary>
    /// Message 검증 에러 type입니다.
    /// </summary>
    public enum MessageValidationError
    {
        /// <summary>
        /// 정상 상태입니다.
        /// </summary>
        Ok,
        /// <summary>
        /// Unrecognized Device ID입니다.(S9F1)
        /// </summary>
        UnrecognizedDeviceID,
        /// <summary>
        /// Unrecognized Stream Type입니다.(S9F3)
        /// </summary>
        UnrecognizedSteam,
        /// <summary>
        /// Unrecognized Function Type입니다.(S9F5)
        /// </summary>
        UnrecognizedFunction,
        /// <summary>
        /// Illegal Data입니다.(S9F7)
        /// </summary>
        IllegalDataFormat,
        /// <summary>
        /// Transaction Timer Timeout입니다.(S9F9)
        /// </summary>
        T3Timeout,
        /// <summary>
        /// Data Too Long입니다.(S9F11, Default Value = 2MB)
        /// </summary>
        DataToLong
    }
}