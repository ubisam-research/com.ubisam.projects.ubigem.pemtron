namespace UbiCom.Net.Driver
{
    /// <summary>
    /// Connected 상태 변경 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ipAddress">연관된 IP Address입니다.</param>
    /// <param name="portNo">연관된 Port no입니다.</param>
    public delegate void SECSConnectedEventHandler(object sender, string ipAddress, int portNo);
    /// <summary>
    /// Disconnected 상태 변경 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ipAddress">연관된 IP Address입니다.</param>
    /// <param name="portNo">연관된 Port no입니다.</param>
    public delegate void SECSDisconnectedEventHandler(object sender, string ipAddress, int portNo);
    /// <summary>
    /// Selected 상태 변경 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ipAddress">연관된 IP Address입니다.</param>
    /// <param name="portNo">연관된 Port no입니다.</param>
    public delegate void SECSSelectedEventHandler(object sender, string ipAddress, int portNo);
    /// <summary>
    /// Deselected 상태 변경 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ipAddress">연관된 IP Address입니다.</param>
    /// <param name="portNo">연관된 Port no입니다.</param>
    public delegate void SECSDeselectedEventHandler(object sender, string ipAddress, int portNo);
    /// <summary>
    /// Timeout 발생 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="timeoutType">발생한 Timeout 종류입니다.</param>
    public delegate void TimeoutEventHandler(object sender, Structure.TimeoutType timeoutType);
    /// <summary>
    /// T3 Timeout 발생 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message">T3 Timeout이 발생한 Primary message입니다.</param>
    public delegate void T3TimeoutEventHandler(object sender, Structure.SECSMessage message);
    /// <summary>
    /// Unknown message 수신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message">수신한 message입니다.</param>
    public delegate void ReceivedUnknownMessageEventHandler(object sender, Structure.SECSMessage message);
    /// <summary>
    /// Invalid primary message 수신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="reason">Invalid 사유입니다.</param>
    /// <param name="message">수신한 message입니다.</param>
    public delegate void ReceivedInvalidPrimaryMessageEventHandler(object sender, Structure.MessageValidationError reason, Structure.SECSMessage message);
    /// <summary>
    /// Invalid secondary message 수신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="reason">Invalid 사유입니다.</param>
    /// <param name="primaryMessage">연관된 Primary message입니다.</param>
    /// <param name="secondaryMessage">수신한 Secondary message입니다.</param>
    public delegate void ReceivedInvalidSecondaryMessageEventHandler(object sender, Structure.MessageValidationError reason, Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage);
    /// <summary>
    /// Control message 송신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message">송신한 message입니다.</param>
    public delegate void SentControlMessageEventHandler(object sender, Structure.SECSMessage message);
    /// <summary>
    /// Data message 송신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message">송신한 message입니다.</param>
    public delegate void SentSECSMessageEventHandler(object sender, Structure.SECSMessage message);
    /// <summary>
    /// Control message 수신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message">수신한 message입니다.</param>
    public delegate void ReceivedControlMessageEventHandler(object sender, Structure.SECSMessage message);
    /// <summary>
    /// Data primary message 수신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message">수신한 message입니다.</param>
    public delegate void ReceivedPrimaryMessageEventHandler(object sender, Structure.SECSMessage message);
    /// <summary>
    /// Data secondary message 수신 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="primaryMessage">연관된 Primary message입니다.</param>
    /// <param name="secondaryMessage">수신한 Secondary message입니다.</param>
    public delegate void ReceivedSecondaryMessageEventHandler(object sender, Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage);

    /// <summary>
    /// SECS Driver Interface
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Connected 상태 변경 이벤트입니다.
        /// </summary>
        event SECSConnectedEventHandler OnSECSConnected;
        /// <summary>
        /// Disconnected 상태 변경 이벤트입니다.
        /// </summary>
        event SECSDisconnectedEventHandler OnSECSDisconnected;
        /// <summary>
        /// Selected 상태 변경 이벤트입니다.
        /// </summary>
        event SECSSelectedEventHandler OnSECSSelected;
        /// <summary>
        /// Deselected 상태 변경 이벤트입니다.
        /// </summary>
        event SECSDeselectedEventHandler OnSECSDeselected;
        /// <summary>
        /// Timeout 발생 이벤트입니다.
        /// </summary>
        event TimeoutEventHandler OnTimeout;
        /// <summary>
        /// T3 Timeout 발생 이벤트입니다.
        /// </summary>
        event T3TimeoutEventHandler OnT3Timeout;
        /// <summary>
        /// Unknown message 수신 이벤트입니다.
        /// </summary>
        event ReceivedUnknownMessageEventHandler OnReceivedUnknownMessage;
        /// <summary>
        /// Invalid primary message 수신 이벤트입니다.
        /// </summary>
        event ReceivedInvalidPrimaryMessageEventHandler OnReceivedInvalidPrimaryMessage;
        /// <summary>
        /// Invalid secondary message 수신 이벤트입니다.
        /// </summary>
        event ReceivedInvalidSecondaryMessageEventHandler OnReceivedInvalidSecondaryMessage;
        /// <summary>
        /// Control message 송신 이벤트입니다.
        /// </summary>
        event SentControlMessageEventHandler OnSentControlMessage;
        /// <summary>
        /// Data message 송신 이벤트입니다.
        /// </summary>
        event SentSECSMessageEventHandler OnSentSECSMessage;
        /// <summary>
        /// Control message 수신 이벤트입니다.
        /// </summary>
        event ReceivedControlMessageEventHandler OnReceivedControlMessage;
        /// <summary>
        /// Data primary message 수신 이벤트입니다.
        /// </summary>
        event ReceivedPrimaryMessageEventHandler OnReceivedPrimaryMessage;
        /// <summary>
        /// Data secondary message 수신 이벤트입니다.
        /// </summary>
        event ReceivedSecondaryMessageEventHandler OnReceivedSecondaryMessage;
        /// <summary>
        /// SECS-I Log write 발생 이벤트입니다.
        /// </summary>
        event Utility.Logger.LogWriteEventHandler OnSECS1WriteLog;
        /// <summary>
        /// SECS-II Log write 발생 이벤트입니다.
        /// </summary>
        event Utility.Logger.LogWriteEventHandler OnSECS2WriteLog;

        /// <summary>
        /// Current Connection State를 가져오거나 설정합니다.
        /// </summary>
        UbiCom.Net.Structure.ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// Driver를 초기화합니다.
        /// </summary>
        /// <param name="configurationFileName">환경 설정 File입니다.</param>
        /// <param name="driverName">SECS Driver Name입니다.</param>
        /// <param name="errorText">초기화 실패 시 실패 사유입니다.(단, OK 시 string.Empty)</param>
        /// <returns>실패 사유입니다.</returns>
        Structure.DriverError Initialize(string configurationFileName, string driverName, out string errorText);
        /// <summary>
        /// Driver를 초기화합니다.
        /// </summary>
        /// <param name="configuration">환경 설정 값입니다.</param>
        /// <param name="errorText">초기화 실패 시 실패 사유입니다.(단, OK 시 string.Empty)</param>
        /// <returns>실패 사유입니다.</returns>
        Structure.DriverError Initialize(Structure.Configurtion configuration, out string errorText);
        /// <summary>
        /// Driver를 terminate합니다.
        /// </summary>
        void Terminate();
        /// <summary>
        /// Driver를 Open합니다.
        /// </summary>
        /// <returns>실패 사유입니다.</returns>
        Structure.DriverError Open();
        /// <summary>
        /// Driver를 Open합니다.
        /// </summary>
        /// <param name="configuration">환경 설정 값입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        Structure.DriverError Open(Structure.Configurtion configuration);
        /// <summary>
        /// Driver를 Close합니다.
        /// </summary>
        /// <returns>실패 사유입니다.</returns>
        Structure.DriverError Close();
        /// <summary>
        /// SECS Message를 송신합니다.
        /// </summary>
        /// <param name="primaryMessage">송신 할 Primary message입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        Structure.MessageError SendSECSMessage(Structure.SECSMessage primaryMessage);
        /// <summary>
        /// SECS Message를 Reply합니다.
        /// </summary>
        /// <param name="primaryMessage">연관된 Primary message입니다.</param>
        /// <param name="secondaryMessage">송신 할 Secondary message입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        Structure.MessageError ReplySECSMessage(Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage);
        /// <summary>
        /// SECS Message를 Reply합니다.
        /// </summary>
        /// <param name="primarySystemBytes">연관된 Primary message의 System Bytes입니다.</param>
        /// <param name="secondaryMessage">송신 할 Secondary message입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        Structure.MessageError ReplySECSMessage(uint primarySystemBytes, Structure.SECSMessage secondaryMessage);
    }
}