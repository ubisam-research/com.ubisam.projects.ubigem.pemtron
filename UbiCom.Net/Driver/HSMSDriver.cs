using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

//[assembly: System.Runtime.CompilerServices.SuppressIldasm()]
namespace UbiCom.Net.Driver
{
    /// <summary>
    /// UbiCom.Net - HSMS Driver입니다.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public partial class HSMSDriver : IDriver, IDisposable
    {
        #region [Delegate / Event Define]
        /// <summary>
        /// Socket Connected 시 발생합니다.
        /// </summary>
        public event SECSConnectedEventHandler OnSECSConnected;
        /// <summary>
        /// Socket Disconnected 시 발생합니다.
        /// </summary>
        public event SECSDisconnectedEventHandler OnSECSDisconnected;
        /// <summary>
        /// HSMS Selected 시 발생합니다.
        /// </summary>
        public event SECSSelectedEventHandler OnSECSSelected;
        /// <summary>
        /// HSMS Deselected 시 발생합니다.
        /// </summary>
        public event SECSDeselectedEventHandler OnSECSDeselected;
        /// <summary>
        /// Timeout 시 발생합니다.
        /// </summary>
        public event TimeoutEventHandler OnTimeout;
        /// <summary>
        /// T3 Timeout 시 발생합니다.
        /// </summary>
        public event T3TimeoutEventHandler OnT3Timeout;
        /// <summary>
        /// Unknown Message 수신 시 발생합니다.
        /// </summary>
        public event ReceivedUnknownMessageEventHandler OnReceivedUnknownMessage;
        /// <summary>
        /// Invalid Primary Message 수신 시 발생합니다.
        /// </summary>
        public event ReceivedInvalidPrimaryMessageEventHandler OnReceivedInvalidPrimaryMessage;
        /// <summary>
        /// Invalid Seconadry Message 수신 시 발생합니다.
        /// </summary>
        public event ReceivedInvalidSecondaryMessageEventHandler OnReceivedInvalidSecondaryMessage;
        /// <summary>
        /// Control Message 송신 시 발생합니다.
        /// </summary>
        public event SentControlMessageEventHandler OnSentControlMessage;
        /// <summary>
        /// SECS Message 송신 시 발생합니다.
        /// </summary>
        public event SentSECSMessageEventHandler OnSentSECSMessage;
        /// <summary>
        /// Control Message 수신 시 발생합니다.
        /// </summary>
        public event ReceivedControlMessageEventHandler OnReceivedControlMessage;
        /// <summary>
        /// Primary SECS Message 수신 시 발생합니다.
        /// </summary>
        public event ReceivedPrimaryMessageEventHandler OnReceivedPrimaryMessage;
        /// <summary>
        /// Secondary SECS Message 수신 시 발생합니다.
        /// </summary>
        public event ReceivedSecondaryMessageEventHandler OnReceivedSecondaryMessage;
        /// <summary>
        /// SECS-I Log Write 시 발생합니다.
        /// </summary>
        public event Utility.Logger.LogWriteEventHandler OnSECS1WriteLog;
        /// <summary>
        /// SECS-II Log Write 시 발생합니다.
        /// </summary>
        public event Utility.Logger.LogWriteEventHandler OnSECS2WriteLog;
        #endregion

        private const string CLASS_NAME = "HSMSDriver";
        private const string STANDARD_MESSAGE_SET = "[{StandardMessageSet}]";

        private const int USB_KEYLOCK_WAIT_DURATION = 3 * 60 * 60 * 1000;
        private const int USB_KEYLOCK_WARNING_DURATION = 10 * 60 * 1000;

        private static uint __systemBytes;

        internal Structure.Configurtion _config;
        internal Utility.TimerManager _timerMgr;
        internal TcpClient _socket;
        internal Utility.Logger.Logger _logger;

        private readonly ConnectManager _connectManager;
        private readonly MessageSender _messageSender;
        private readonly MessageReader _messageReader;

        private Utility.MessageLoader _messageLoader;
        private Utility.SendMessageManager _sendMessageMgr;

        private bool _licenseFailed;
        private readonly UbiSam.Net.KeyLock.LicenseChecker _licenseChecker;
        private readonly string _licenseKey;

        private readonly object _lockSystembytes;

        private bool _useSendLinkTest;
        private bool _isOpen;
        private bool _disposed;

        /// <summary>
        /// 환경 설정 값을 가져옵니다.
        /// </summary>
        public Structure.Configurtion Config
        {
            get { return this._config; }
        }

        /// <summary>
        /// Connected 상태를 가져옵니다.
        /// </summary>
        public bool Connected
        {
            get
            {
                if (this._socket != null && this._socket.Client != null && this._socket.Connected == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Connection 상태를 가져오거나 설정합니다.
        /// </summary>
        public Structure.ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// Auto reply 사용 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool UseAutoReply { get; set; }

        /// <summary>
        /// Message manager를 가져옵니다.
        /// </summary>
        public Structure.SECSMessageManager MessageManager
        {
            get { return this._messageLoader.SECSMessageMgr; }
        }

        /// <summary>
        /// SECS message를 가져옵니다.
        /// </summary>
        public Structure.SECSMessageCollection Messages
        {
            get { return this._messageLoader.SECSMessageMgr.Messages; }
        }

        /// <summary>
        /// Primary message를 가져옵니다.
        /// </summary>
        public Structure.SECSMessageCollection PrimaryMessages
        {
            get
            {
                Structure.SECSMessageCollection result;

                result = new Structure.SECSMessageCollection();

                if (this._config.DeviceType == Structure.DeviceType.Host)
                {
                    var varMessage = from KeyValuePair<string, Structure.SECSMessage> tempMessage in this._messageLoader.SECSMessageMgr.Messages.MessageInfo
                                     where tempMessage.Value.Direction != Structure.SECSMessageDirection.ToHost &&
                                           (tempMessage.Value.Function % 2) == 1
                                     select tempMessage.Value;

                    foreach (Structure.SECSMessage tempMessage in varMessage)
                    {
                        result.Add(tempMessage);
                    }
                }
                else
                {
                    var varMessage = from KeyValuePair<string, Structure.SECSMessage> tempMessage in this._messageLoader.SECSMessageMgr.Messages.MessageInfo
                                     where tempMessage.Value.Direction != Structure.SECSMessageDirection.ToEquipment &&
                                           (tempMessage.Value.Function % 2) == 1
                                     select tempMessage.Value;

                    foreach (Structure.SECSMessage tempMessage in varMessage)
                    {
                        result.Add(tempMessage);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Secondary message를 가져옵니다.
        /// </summary>
        public Structure.SECSMessageCollection SecondaryMessages
        {
            get
            {
                Structure.SECSMessageCollection result;

                result = new Structure.SECSMessageCollection();

                if (this._config.DeviceType == Structure.DeviceType.Host)
                {
                    var varMessage = from KeyValuePair<string, Structure.SECSMessage> tempMessage in this._messageLoader.SECSMessageMgr.Messages.MessageInfo
                                     where tempMessage.Value.Direction != Structure.SECSMessageDirection.ToHost &&
                                           (tempMessage.Value.Function % 2) == 0
                                     select tempMessage.Value;

                    foreach (Structure.SECSMessage tempMessage in varMessage)
                    {
                        result.Add(tempMessage);
                    }
                }
                else
                {
                    var varMessage = from KeyValuePair<string, Structure.SECSMessage> tempMessage in this._messageLoader.SECSMessageMgr.Messages.MessageInfo
                                     where tempMessage.Value.Direction != Structure.SECSMessageDirection.ToEquipment &&
                                           (tempMessage.Value.Function % 2) == 0
                                     select tempMessage.Value;

                    foreach (Structure.SECSMessage tempMessage in varMessage)
                    {
                        result.Add(tempMessage);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public HSMSDriver()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            this._logger = new Utility.Logger.Logger();
            this._config = new Structure.Configurtion();
            this._connectManager = new ConnectManager(this);
            this._timerMgr = new Utility.TimerManager();
            this._messageSender = new MessageSender();
            this._messageReader = new MessageReader();
            this._messageLoader = new Utility.MessageLoader(this);
            this._socket = null;
            this._sendMessageMgr = new Utility.SendMessageManager();

            #region [License Initialize]
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
            #endregion

            this._lockSystembytes = new object();

            this._useSendLinkTest = false;
            this._isOpen = false;
            this._disposed = false;

            #region [Event Subscribe]
            this._connectManager.OnSocketConnected += ConnectManager_OnSocketConnected;
            this._connectManager.OnTimeout += TimerMgr_OnTimeout;

            this._timerMgr.OnTimeout += TimerMgr_OnTimeout;
            this._timerMgr.OnT3Timeout += TimerMgr_OnT3Timeout;

            this._messageSender.OnSentControlMessage += MessageSender_OnSentControlMessage;
            this._messageSender.OnSentSECSPrimaryMessage += MessageSender_OnSentSECSPrimaryMessage;
            this._messageSender.OnSentSECSSecondaryMessage += MessageSender_OnSentSECSSecondaryMessage;

            this._messageReader.OnTimeout += TimerMgr_OnTimeout;
            this._messageReader.OnReceivedMessage += MessageReader_OnReceivedMessage;

            this._logger.OnSECS1WriteLog += Logger_OnSECS1WriteLog;
            this._logger.OnSECS2WriteLog += Logger_OnSECS2WriteLog;
            #endregion

            this.ConnectionState = Structure.ConnectionState.Disconnected;
            this.UseAutoReply = true;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ctor");
        }

        /// <summary>
        /// 기본 소멸자입니다.
        /// </summary>
        ~HSMSDriver()
        {
            Dispose(false);
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 애플리케이션 정의 작업을 수행합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 애플리케이션 정의 작업을 수행합니다.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (this._messageSender != null)
                    {
                        this._messageSender.Dispose();
                    }

                    if (this._messageReader != null)
                    {
                        this._messageReader.Dispose();
                    }

                    if (this._timerMgr != null)
                    {
                        this._timerMgr.Dispose();
                    }

                    if (this._connectManager != null)
                    {
                        this._connectManager.Dispose();
                    }

                    if (this._socket != null)
                    {
                        this._socket.Close();
                        this._socket = null;
                    }

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Dispose", "Dispose");

                    this._logger.Dispose();
                }

                this._disposed = true;
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                this._logger.WriteException(DateTime.Now, CLASS_NAME, "CurrentDomain_UnhandledException", e.ExceptionObject as Exception);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        private void LicenseChecker_CheckActiveEvent(object sender, out string uniqueKey, out UbiSam.Net.KeyLock.Structure.Product product, out bool isActive)
        {
            uniqueKey = this._licenseKey;
            product = UbiSam.Net.KeyLock.Structure.Product.UbiCOM;
            isActive = true;
        }

        private void LicenseChecker_LicenseCheckEvent(object sender, string uniqueKey, UbiSam.Net.KeyLock.Structure.Product product, UbiSam.Net.KeyLock.Structure.LicenseResult result)
        {
            string logText;

            if (this._licenseKey == uniqueKey)
            {
                logText = string.Format("License Status Changed:Status={0}", result);

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "LicenseChecker_LicenseCheckEvent", logText);
                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Information, logText);
                this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Information, logText);

                if (result == UbiSam.Net.KeyLock.Structure.LicenseResult.LicenseOk)
                {
                    this._licenseFailed = false;
                }
                else
                {
                    this._licenseFailed = true;

                    Close();
                }
            }
        }

        private void LicenseChecker_OnRequestLogging(string message)
        {
            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "LicenseChecker_OnRequestLogging", message);
        }

        void ConnectManager_OnSocketConnected(object sender, TcpClient client)
        {
            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ConnectManager_OnSocketConnected", "Connected");

            this._sendMessageMgr.Clear();

            SetConnected(client);
        }

        void TimerMgr_OnTimeout(object sender, Structure.TimeoutType timeoutType)
        {
            if (timeoutType != Structure.TimeoutType.Linktest)
            {
                string errorText = string.Format("Occurrence timeout:{0}", timeoutType);

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "TimerMgr_OnTimeout", errorText);
                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);
                this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);
            }

            if (timeoutType != Structure.TimeoutType.Linktest)
            {
                RaiseEventOnTimeout(this, timeoutType);
            }

            if (timeoutType == Structure.TimeoutType.Linktest)
            {
                SendControlMessage(Structure.ControlMessageType.LinktestRequest, GetNextSystemBytes());
            }
            else if (timeoutType == Structure.TimeoutType.T7)
            {
                if (this.ConnectionState != Structure.ConnectionState.Selected)
                {
                    SetDisconnected(true);
                }
            }
            else if (timeoutType == Structure.TimeoutType.T6 ||
                timeoutType == Structure.TimeoutType.T8)
            {
                SetDisconnected(true);
            }
        }

        void TimerMgr_OnT3Timeout(object sender, Structure.SECSMessage message)
        {
            string errorText = string.Format("Occurrence T3 timeout:{0}", message);

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "TimerMgr_OnT3Timeout", errorText);
            this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);
            this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);

            RaiseEventOnT3Timeout(this, message);

            SendInvalidMessage(Structure.MessageValidationError.T3Timeout, message);
        }

        void MessageSender_OnSentControlMessage(object sender, Structure.SECSMessage message)
        {
            RaiseEventOnSentControlMessage(this, message);
        }

        void MessageSender_OnSentSECSPrimaryMessage(object sender, Structure.SECSMessage message)
        {
            // modify, locketk, 2025-05-13
            // for novasoft request
            //this._sendMessageMgr.Add(message);
            if (message.WaitBit == true)
            {
                this._sendMessageMgr.Add(message);
            }

            RaiseEventOnSentSECSMessage(this, message);
        }

        void MessageSender_OnSentSECSSecondaryMessage(object sender, Structure.SECSMessage message)
        {
            RaiseEventOnSentSECSMessage(this, message);
        }

        void MessageReader_OnReceivedMessage(object sender, Structure.SECSMessage message)
        {
            Structure.MessageValidationError result;
            Structure.SECSMessage primaryMessage;
            Structure.SECSMessage secondaryMessage;
            string errorText;

            if (message != null)
            {
                if (this._useSendLinkTest == true)
                {
                    this._timerMgr.Restart(Structure.TimeoutType.Linktest);
                }

                if (message.ControlMessageType == Structure.ControlMessageType.DataMessage)
                {
                    if ((message.Function % 2) == 0)
                    {
                        primaryMessage = this._sendMessageMgr.GetMessage(message.SystemBytes);

                        if (primaryMessage == null)
                        {
                            errorText = "Received invalid systembytes " + message.ToString();

                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "reader_OnReceivedMessage", errorText);
                            this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);
                            this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);

                            RaiseEventOnReceivedUnknownMessage(this, message);
                        }
                        else
                        {
                            this._timerMgr.StopT3(message);

                            result = this._messageLoader.ValidateReceivedMessage(message);

                            message.UserData = primaryMessage.UserData;

                            if (result == Structure.MessageValidationError.Ok)
                            {
                                RaiseEventOnReceivedSecondaryMessage(this, primaryMessage, message);
                            }
                            else
                            {
                                errorText = "Received invalid secondary message " + message.ToString();

                                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "reader_OnReceivedMessage", errorText);
                                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);
                                this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);

                                RaiseEventOnReceivedInvalidSecondaryMessage(this, result, primaryMessage, message);

                                SendInvalidMessage(result, message);
                            }

                            this._sendMessageMgr.Remove(message.SystemBytes);
                        }
                    }
                    else
                    {
                        result = this._messageLoader.ValidateReceivedMessage(message);

                        if (result == Structure.MessageValidationError.Ok)
                        {
                            RaiseEventOnReceivedPrimaryMessage(this, message);

                            if (this.UseAutoReply == true && message.AutoReply == true)
                            {
                                secondaryMessage = this._messageLoader.SECSMessageMgr[message.Stream, message.Function + 1, this._config.DeviceType];

                                if (secondaryMessage != null)
                                {
                                    ReplySECSMessage(message, secondaryMessage);
                                }
                            }
                        }
                        else
                        {
                            errorText = "Received invalid primary message " + message.ToString();

                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "reader_OnReceivedMessage", errorText);
                            this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);
                            this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Warning, errorText);

                            RaiseEventOnReceivedInvalidPrimaryMessage(this, result, message);

                            SendInvalidMessage(result, message);
                        }
                    }
                }
                else
                {
                    ReceivedControlMessage(message);
                }
            }
        }

        void Logger_OnSECS1WriteLog(object sender, Utility.Logger.LogLevel logLevel, string logText)
        {
            RaiseEventOnSECS1WriteLog(this, logLevel, logText);
        }

        void Logger_OnSECS2WriteLog(object sender, Utility.Logger.LogLevel logLevel, string logText)
        {
            RaiseEventOnSECS2WriteLog(this, logLevel, logText);
        }

        /// <summary>
        /// Driver를 초기화합니다.
        /// </summary>
        /// <param name="configurationFileName">환경 설정 File입니다.</param>
        /// <param name="driverName">SECS Driver Name입니다.</param>
        /// <param name="errorText">초기화 실패 시 실패 사유입니다.(단, OK 시 string.Empty)</param>
        /// <returns>실패 사유입니다.</returns>
        public Structure.DriverError Initialize(string configurationFileName, string driverName, out string errorText)
        {
            Structure.DriverError result;
            Structure.ConfigurtionCollection configurationInfo;

            if (this._licenseFailed == true)
            {
                errorText = "License verification failed.";

                return Structure.DriverError.LicenseVerificationFailed;
            }

            if (this.Connected == true &&
                (this.ConnectionState == Structure.ConnectionState.Connected ||
                 this.ConnectionState == Structure.ConnectionState.Selected))
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "Already Connected");

                errorText = Resources.ErrorString.AlreadyConnected;

                return Structure.DriverError.AlreadyConnected;
            }

            configurationInfo = new Structure.ConfigurtionCollection();

            result = configurationInfo.Load(configurationFileName, out errorText);

            if (result == Structure.DriverError.Ok)
            {
                if (configurationInfo.Contains(driverName) == true)
                {
                    result = Initialize(configurationInfo[driverName], out errorText);
                }
                else
                {
                    result = Structure.DriverError.NotExistDriverName;

                    errorText = Resources.ErrorString.DoseNotExistDriverName;

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Error, CLASS_NAME, "Initialize", errorText);
                    this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Error, errorText);
                    this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Error, errorText);
                }
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Error, CLASS_NAME, "Initialize", errorText);
                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Error, errorText);
                this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Error, errorText);
            }

            return result;
        }

        /// <summary>
        /// Driver를 초기화합니다.
        /// </summary>
        /// <param name="configuration">환경 설정 값입니다.</param>
        /// <param name="errorText">초기화 실패 시 실패 사유입니다.(단, OK 시 string.Empty)</param>
        /// <returns>실패 사유입니다.</returns>
        public Structure.DriverError Initialize(Structure.Configurtion configuration, out string errorText)
        {
            Structure.DriverError result;
            string logText;
            System.Diagnostics.Process currentProcess;
            System.Reflection.Assembly assembly;
            Version version;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "CurrentDirectory:" + System.IO.Directory.GetCurrentDirectory());

            if (this._licenseFailed == true)
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "License verification failed");

                errorText = "License verification failed.";

                return Structure.DriverError.LicenseVerificationFailed;
            }

            if (this.Connected == true &&
                (this.ConnectionState == Structure.ConnectionState.Connected ||
                 this.ConnectionState == Structure.ConnectionState.Selected))
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "Already Connected");

                errorText = Resources.ErrorString.AlreadyConnected;

                return Structure.DriverError.AlreadyConnected;
            }

            currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            logText = string.Empty;

            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            version = assembly.GetName().Version;

            this._logger.Initialize(configuration);

            if (configuration == null)
            {
                result = Structure.DriverError.InvalidConfiguration;

                errorText = Resources.ErrorString.ConfigurationIsNull;

                logText = string.Format("Initialize (null) (PID={0}) (Version={1}):Result={2}", currentProcess.Id, version, result);

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Error, CLASS_NAME, "Initialize", logText);
            }
            else
            {
                result = configuration.ValidateConfiguration(out errorText);

                if (result == Structure.DriverError.Ok)
                {
                    this._messageLoader = new Utility.MessageLoader(this);

                    if (string.IsNullOrEmpty(configuration.UMDFileName) == false)
                    {
                        if (configuration.UMDFileName == STANDARD_MESSAGE_SET)
                        {
                            this.MessageManager.Initialize(configuration);

                            logText = string.Format("Initialize ({0}) (PID={1}) (Version={2}):Result={3}", configuration.DriverName, currentProcess.Id, version, result);

                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", logText);
                        }
                        else
                        {
                            result = LoadUmd(configuration, out errorText);

                            logText = string.Format("Initialize ({0}) (PID={1}) (Version={2}):Load UMD={3}, Result={4}", configuration.DriverName, currentProcess.Id, version, configuration.UMDFileName, result);

                            if (string.IsNullOrEmpty(errorText) == false)
                            {
                                logText += (", Error=" + errorText);

                                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Error, CLASS_NAME, "Initialize", logText);
                            }
                            else
                            {
                                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", logText);
                            }
                        }

                        if (result == Structure.DriverError.Ok)
                        {
                            this._config = configuration;
                            this._connectManager.Initialize(this._config);
                            this._timerMgr.Initialize(this._config);

                            this._useSendLinkTest = true;

                            logText = string.Format("Initialize ({0}) (PID={1}) (Version={2}):Instance initialized", configuration.DriverName, currentProcess.Id, version);

                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", logText);
                        }
                    }

                    if (string.IsNullOrEmpty(errorText) == false)
                    {
                        logText += (", Error=" + errorText);

                        this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Error, logText);
                        this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Error, logText);
                    }
                    else
                    {
                        this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Information, logText);
                        this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Information, logText);
                    }
                }
                else
                {
                    logText += (", Error=" + errorText);

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Error, CLASS_NAME, "Initialize", logText);
                    this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Error, logText);
                    this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Error, logText);
                }
            }

            return result;
        }

        /// <summary>
        /// Driver를 terminate합니다.
        /// </summary>
        public void Terminate()
        {
            this._messageSender.Dispose();
            this._messageReader.Dispose();
            this._timerMgr.Dispose();
            this._connectManager.Dispose();

            if (this._socket != null)
            {
                this._socket.Close();
                this._socket = null;
            }

            if (this._messageLoader != null)
            {
                this._messageLoader.Dispose();
                this._messageLoader = null;
            }

            if (this._sendMessageMgr != null)
                this._sendMessageMgr = null;

            this._logger.Dispose();
        }

        /// <summary>
        /// Driver를 Open합니다.
        /// </summary>
        /// <returns>실패 사유입니다.</returns>
        public Structure.DriverError Open()
        {
            Structure.DriverError result;

            if (this._licenseFailed == true)
            {
                return Structure.DriverError.LicenseVerificationFailed;
            }

            result = Open(this._config);

            return result;
        }

        /// <summary>
        /// Driver를 Open합니다.
        /// </summary>
        /// <param name="configuration">환경 설정 값입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        public Structure.DriverError Open(Structure.Configurtion configuration)
        {
            Structure.DriverError result;

            if (this._licenseFailed == true)
            {
                return Structure.DriverError.LicenseVerificationFailed;
            }

            if (this.Connected == true &&
                (this.ConnectionState == Structure.ConnectionState.Connected ||
                 this.ConnectionState == Structure.ConnectionState.Selected))
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Open", "Already Connected");

                result = Structure.DriverError.AlreadyConnected;
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Open", "Connector Start");

                this._sendMessageMgr.Clear();
                this._connectManager.Start();

                this._isOpen = true;

                this._licenseChecker.LicenseFailWaitTimerEnabled = true;

                result = Structure.DriverError.Ok;
            }

            return result;
        }

        /// <summary>
        /// Driver를 Close합니다.
        /// </summary>
        /// <returns>실패 사유입니다.</returns>
        public Structure.DriverError Close()
        {
            Structure.DriverError result;

            try
            {
                this._licenseChecker.LicenseFailWaitTimerEnabled = false;

                this._isOpen = false;

                if (this.ConnectionState == Structure.ConnectionState.Selected)
                {
                    SendControlMessage(Structure.ControlMessageType.SeparateRequest, GetNextSystemBytes());
                }

                this._messageSender.Terminate();
                this._messageReader.Terminate();
                this._timerMgr.Stop();
                this._connectManager.Terminate();

                if (this.ConnectionState != Structure.ConnectionState.Disconnected)
                {
                    SetConnectionState(Structure.ConnectionState.Disconnected);
                }

                if (this._socket != null)
                {
                    this._socket.Close();
                    this._socket = null;
                }

                result = Structure.DriverError.Ok;
            }
            catch (Exception ex)
            {
                result = Structure.DriverError.Unknown;

                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// SECS Message를 송신합니다.
        /// </summary>
        /// <param name="message">송신 할 Primary message입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        public Structure.MessageError SendSECSMessage(Structure.SECSMessage message)
        {
            Structure.MessageError result;

            if (this.ConnectionState != Structure.ConnectionState.Selected)
            {
                result = Structure.MessageError.NotSelected;
            }
            else
            {
                message.DeviceId = this._config.DeviceID;
                message.SystemBytes = GetNextSystemBytes();

                if (this._sendMessageMgr.Exists(message.SystemBytes) == true)
                {
                    result = Structure.MessageError.DuplicateSystemBytes;
                }
                else
                {
                    result = this._messageSender.SendPrmaryMessage(message);
                }
            }

            return result;
        }

        public bool SelfReciveBlock(byte[] receivedData)
        {
            _messageReader.SelfReciveBlockCom(receivedData);
            return true;
        }
        public bool SelfReciveS1F13()
        {
            byte[] receivedData = new byte[] { 0x00, 0x63, 0x81, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x01, 0x00 };

            receivedData[1] = (byte)_config.DeviceID;

            SelfReciveBlock(receivedData);

            return true;
        }
        /// <summary>
        /// SECS Message를 Reply합니다.
        /// </summary>
        /// <param name="primaryMessage">연관된 Primary message입니다.</param>
        /// <param name="secondaryMessage">송신 할 Secondary message입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        public Structure.MessageError ReplySECSMessage(Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage)
        {
            Structure.MessageError result;

            if (this.ConnectionState != Structure.ConnectionState.Selected)
            {
                result = Structure.MessageError.NotSelected;
            }
            else
            {
                secondaryMessage.DeviceId = this._config.DeviceID;
                secondaryMessage.SystemBytes = primaryMessage.SystemBytes;

                result = this._messageSender.SendSecondaryMessage(secondaryMessage);
            }

            return result;
        }

        /// <summary>
        /// SECS Message를 Reply합니다.
        /// </summary>
        /// <param name="primarySystemBytes">연관된 Primary message의 System Bytes입니다.</param>
        /// <param name="secondaryMessage">송신 할 Secondary message입니다.</param>
        /// <returns>실패 사유입니다.</returns>
        public Structure.MessageError ReplySECSMessage(uint primarySystemBytes, Structure.SECSMessage secondaryMessage)
        {
            Structure.MessageError result;

            if (this.ConnectionState != Structure.ConnectionState.Selected)
            {
                result = Structure.MessageError.NotSelected;
            }
            else
            {
                secondaryMessage.DeviceId = this._config.DeviceID;
                secondaryMessage.SystemBytes = primarySystemBytes;

                result = this._messageSender.SendSecondaryMessage(secondaryMessage);
            }

            return result;
        }

        /// <summary>
        /// 사용자 정의 SECS Message를 추가합니다.
        /// </summary>
        /// <param name="message">추가 할 message입니다.</param>
        public void AddUserDefinedMessage(Structure.SECSMessage message)
        {
            this._messageLoader.SECSMessageMgr.AddUserDefinedMessage(message);
        }

        private uint GetNextSystemBytes()
        {
            lock (this._lockSystembytes)
            {
                if (__systemBytes >= 0xffffffff)
                {
                    __systemBytes = 0;
                }

                __systemBytes++;
            }

            return __systemBytes;
        }

        internal void SetConnected(TcpClient client)
        {
            this._socket = client;

            this._messageSender.Initialize(this);
            this._messageReader.Initialize(this);

            this._timerMgr.Stop();

            if (this._config.HSMSModeConfig.HSMSMode == Structure.HSMSMode.Active)
            {
                SetConnectionState(Structure.ConnectionState.Connected);

                SendControlMessage(Structure.ControlMessageType.SelectRequest, GetNextSystemBytes());
            }
            else
            {
                if (this.ConnectionState != Structure.ConnectionState.Selected && this.ConnectionState != Structure.ConnectionState.Disconnected)
                {
                    // Accept 후 해당 Logic 수행 전에 Select.Request 수신 되는 경우 있음
                    this._timerMgr.Start(Structure.TimeoutType.T7);
                }

                SetConnectionState(Structure.ConnectionState.Connected);
            }
        }

        internal void SetDisconnected(bool isRetry)
        {
            lock (this)
            {
                try
                {
                    this._timerMgr.Stop();

                    this._messageSender.Terminate();
                    this._messageReader.Terminate();
                    this._connectManager.Terminate();

                    SetConnectionState(Structure.ConnectionState.Disconnected);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
                finally
                {
                    try
                    {
                        if (this._socket != null)
                        {
                            this._socket.Close();
                            this._socket = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SetDisconnected", "Terminate");

                if (isRetry == true && this._isOpen == true)
                {
                    this._connectManager.Initialize(this._config);
                    this._timerMgr.Initialize(this._config);

                    this._connectManager.Start();

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SetDisconnected", "Re-connect start");
                }
                else
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SetDisconnected", "do not Re-connect start");
                }
            }
        }

        private void SetConnectionState(Structure.ConnectionState currentState)
        {
            Structure.ConnectionState previousState;

            previousState = this.ConnectionState;

            if (previousState != currentState)
            {
                if (this.ConnectionState == Structure.ConnectionState.Selected && currentState == Structure.ConnectionState.Connected)
                {
                    this.ConnectionState = Structure.ConnectionState.Selected;
                }
                else
                {
                    this.ConnectionState = currentState;
                }

                switch (currentState)
                {
                    case Structure.ConnectionState.Disconnected:
                        try
                        {
                            if (this._config.HSMSModeConfig.HSMSMode == Structure.HSMSMode.Active)
                            {
                                RaiseEventOnSECSDisconnected(this, this._config.HSMSModeConfig.RemoteIPAddress, this._config.HSMSModeConfig.RemotePortNo);
                            }
                            else
                            {
                                RaiseEventOnSECSDisconnected(this, this._config.HSMSModeConfig.LocalIPAddress, this._config.HSMSModeConfig.LocalPortNo);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseEventOnSECSDisconnected(this, string.Empty, 0);

                            System.Diagnostics.Debug.Print(ex.Message);
                        }

                        break;
                    case Structure.ConnectionState.Connected:
                        if (this._config.HSMSModeConfig.HSMSMode == Structure.HSMSMode.Active)
                        {
                            RaiseEventOnSECSConnected(this, this._config.HSMSModeConfig.RemoteIPAddress, this._config.HSMSModeConfig.RemotePortNo);
                        }
                        else
                        {
                            RaiseEventOnSECSConnected(this, this._config.HSMSModeConfig.LocalIPAddress, this._config.HSMSModeConfig.LocalPortNo);
                        }

                        break;
                    case Structure.ConnectionState.Selected:
                        if (this._config.HSMSModeConfig.HSMSMode == Structure.HSMSMode.Active)
                        {
                            RaiseEventOnSECSSelected(this, this._config.HSMSModeConfig.RemoteIPAddress, this._config.HSMSModeConfig.RemotePortNo);
                        }
                        else
                        {
                            RaiseEventOnSECSSelected(this, this._config.HSMSModeConfig.LocalIPAddress, this._config.HSMSModeConfig.LocalPortNo);
                        }

                        break;
                    case Structure.ConnectionState.Deselected:
                        if (this._config.HSMSModeConfig.HSMSMode == Structure.HSMSMode.Active)
                        {
                            RaiseEventOnSECSDeselected(this, this._config.HSMSModeConfig.RemoteIPAddress, this._config.HSMSModeConfig.RemotePortNo);
                        }
                        else
                        {
                            RaiseEventOnSECSDeselected(this, this._config.HSMSModeConfig.LocalIPAddress, this._config.HSMSModeConfig.LocalPortNo);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private Structure.MessageError SendControlMessage(Structure.ControlMessageType messageType, uint systemBytes, int reasonCode = 0)
        {
            Structure.MessageError result;
            Structure.SECSMessage message;
            message = new Structure.SECSMessage
            {
                ControlMessageType = messageType,
                DeviceId = this._config.DeviceID,
                Name = messageType.ToString(),
                SystemBytes = systemBytes
            };

            result = this._messageSender.SendControlMessage(message, reasonCode);

            return result;
        }

        private Structure.MessageError SendInvalidMessage(Structure.MessageValidationError reason, Structure.SECSMessage receiveMessage)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            Structure.SECSMessage message;

            if (receiveMessage.Stream == 9 &&
                (receiveMessage.Function == 0 || receiveMessage.Function == 1 ||
                 receiveMessage.Function == 3 || receiveMessage.Function == 5 ||
                 receiveMessage.Function == 7 || receiveMessage.Function == 9 ||
                 receiveMessage.Function == 11))
            {
            }
            else if (this._config.DeviceType == Structure.DeviceType.Equipment)
            {
                switch (reason)
                {
                    case Structure.MessageValidationError.UnrecognizedDeviceID:
                        message = this._messageLoader.GetSECSMessage(9, 1);

                        break;
                    case Structure.MessageValidationError.UnrecognizedSteam:
                        message = this._messageLoader.GetSECSMessage(9, 3);

                        break;
                    case Structure.MessageValidationError.UnrecognizedFunction:
                        message = this._messageLoader.GetSECSMessage(9, 5);

                        break;
                    case Structure.MessageValidationError.IllegalDataFormat:
                        message = this._messageLoader.GetSECSMessage(9, 7);

                        break;
                    case Structure.MessageValidationError.T3Timeout:
                        message = this._messageLoader.GetSECSMessage(9, 9);

                        break;
                    case Structure.MessageValidationError.DataToLong:
                        message = this._messageLoader.GetSECSMessage(9, 11);

                        break;
                    default:
                        message = null;
                        break;
                }

                if (message != null)
                {
                    message.DeviceId = this._config.DeviceID;
                    message.SystemBytes = GetNextSystemBytes();
                    message.WaitBit = false;

                    message.Body.Item.Clear();

                    foreach (Structure.SECSItem temp in message.Body.AsList)
                    {
                        message.Body.Item.Add(temp);
                    }

                    if (message.Body.AsList.Count == 1 && message.Body.Item.Items[0].Format == Structure.SECSItemFormat.B)
                    {
                        MakeHeader(receiveMessage, out byte[] header);

                        message.Body.Item.Items[0].Value = header;
                    }

                    result = this._messageSender.SendPrmaryMessage(message);
                }
            }

            return result;
        }

        private void ReceivedControlMessage(Structure.SECSMessage message)
        {
            Structure.MessageError result;

            RaiseEventOnReceivedControlMessage(this, message);

            switch (message.ControlMessageType)
            {
                case Structure.ControlMessageType.SelectRequest:
                    this._timerMgr.Stop(Structure.TimeoutType.T7);
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ReceivedControlMessage", "T7 Timer Stop");

                    if (this.ConnectionState == Structure.ConnectionState.Selected)
                    {
                        result = SendControlMessage(Structure.ControlMessageType.SelectResponse, message.SystemBytes, Structure.SelectStatus.AlreadySelected.GetHashCode());
                    }
                    else
                    {
                        result = SendControlMessage(Structure.ControlMessageType.SelectResponse, message.SystemBytes);
                    }

                    if (result == Structure.MessageError.Ok)
                    {
                        SetConnectionState(Structure.ConnectionState.Selected);
                    }

                    break;
                case Structure.ControlMessageType.SelectResponse:
                    this._timerMgr.Stop(Structure.TimeoutType.T6);
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ReceivedControlMessage", "T6 Timer Stop");

                    ReceivedSelectResponse(message);

                    break;
                case Structure.ControlMessageType.DeselectRequest:
                    this.ConnectionState = Structure.ConnectionState.Connected;

                    if (this.ConnectionState == Structure.ConnectionState.Deselected)
                    {
                        SendControlMessage(Structure.ControlMessageType.DeselectResponse, message.SystemBytes, Structure.DeselectStatus.AlreadyDeselected.GetHashCode());
                    }
                    else
                    {
                        SendControlMessage(Structure.ControlMessageType.DeselectResponse, message.SystemBytes);
                    }

                    break;
                case Structure.ControlMessageType.DeselectResponse:
                    this.ConnectionState = Structure.ConnectionState.Connected;

                    break;
                case Structure.ControlMessageType.LinktestRequest:
                    SendControlMessage(Structure.ControlMessageType.LinktestResponse, message.SystemBytes);

                    break;
                case Structure.ControlMessageType.LinktestResponse:
                    this._timerMgr.Stop(Structure.TimeoutType.T6);
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ReceivedControlMessage", "T6 Timer Stop");

                    break;
                case Structure.ControlMessageType.RejectRequest:
                    SendControlMessage(Structure.ControlMessageType.RejectRequest, message.SystemBytes);
                    break;
                case Structure.ControlMessageType.SeparateRequest:
                    SetDisconnected(true);

                    break;
            }
        }

        private void ReceivedSelectResponse(Structure.SECSMessage message)
        {
            switch ((Structure.SelectStatus)message.StatusCode)
            {
                case Structure.SelectStatus.Succeed:
                    SetConnectionState(Structure.ConnectionState.Selected);

                    break;
                case Structure.SelectStatus.AlreadySelected:
                case Structure.SelectStatus.ConnectionNotReady:
                case Structure.SelectStatus.ConnectExhaust:
                    SetDisconnected(false);

                    break;
            }
        }

        private Structure.DriverError LoadUmd(Structure.Configurtion configuration, out string errorText)
        {
            Structure.DriverError result;

            try
            {
                errorText = this._messageLoader.Load(configuration);

                if (string.IsNullOrEmpty(errorText) == false)
                {
                    this._messageLoader = null;

                    result = Structure.DriverError.FileLoadFailed;
                }
                else
                {
                    result = Structure.DriverError.Ok;
                }
            }
            catch (Exception ex)
            {
                result = Structure.DriverError.Unknown;
                errorText = Resources.ErrorString.FailedUMDFileLoad;

                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        private Structure.MessageError MakeHeader(Structure.SECSMessage message, out byte[] header)
        {
            Structure.MessageError result;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeHeader", string.Empty);

            header = new byte[MessageEncoder.HeaderSchema.LENGTH_TOTAL];

            try
            {
                byte[] systemBytes;
                if (message.ControlMessageType == Structure.ControlMessageType.DataMessage)
                {
                    byte[] deviceId = Utility.Converter.ConvertLong2Bytes(message.DeviceId, MessageEncoder.HeaderSchema.LENGTH_DEVICE_ID);
                    Array.Copy(deviceId, 0, header, MessageEncoder.HeaderSchema.INDEX_DEVICE_ID, MessageEncoder.HeaderSchema.LENGTH_DEVICE_ID);

                    if (message.WaitBit == true)
                    {
                        header[MessageEncoder.HeaderSchema.INDEX_STREAM] = (byte)(message.Stream - 0x80);
                    }
                    else
                    {
                        header[MessageEncoder.HeaderSchema.INDEX_STREAM] = (byte)message.Stream;
                    }

                    header[MessageEncoder.HeaderSchema.INDEX_FUNCTION] = (byte)message.Function;
                    header[MessageEncoder.HeaderSchema.INDEX_P_TYPE] = 0;
                    header[MessageEncoder.HeaderSchema.INDEX_S_TYPE] = (byte)(message.ControlMessageType.GetHashCode());

                    systemBytes = Utility.Converter.ConvertLong2Bytes(message.SystemBytes, MessageEncoder.HeaderSchema.LENGTH_SYSTEM_BYTES);
                    Array.Copy(systemBytes, 0, header, MessageEncoder.HeaderSchema.INDEX_SYSTEMBYTES, MessageEncoder.HeaderSchema.LENGTH_SYSTEM_BYTES);
                }
                else
                {
                    header[MessageEncoder.HeaderSchema.INDEX_DEVICE_ID] = 0xff;
                    header[MessageEncoder.HeaderSchema.INDEX_DEVICE_ID + 1] = 0xff;
                    header[MessageEncoder.HeaderSchema.INDEX_STREAM] = 0;
                    header[MessageEncoder.HeaderSchema.INDEX_FUNCTION] = 0;
                    header[MessageEncoder.HeaderSchema.INDEX_P_TYPE] = 0;
                    header[MessageEncoder.HeaderSchema.INDEX_S_TYPE] = (byte)(message.ControlMessageType.GetHashCode());

                    systemBytes = Utility.Converter.ConvertLong2Bytes(message.SystemBytes, MessageEncoder.HeaderSchema.LENGTH_SYSTEM_BYTES);
                    Array.Copy(systemBytes, 0, header, MessageEncoder.HeaderSchema.INDEX_SYSTEMBYTES, MessageEncoder.HeaderSchema.LENGTH_SYSTEM_BYTES);
                }

                result = Structure.MessageError.Ok;
            }
            catch (Exception ex)
            {
                header = null;
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "MakeHeader", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeHeader", string.Format("Result={0}", result));

            return result;
        }
    }
}