using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UbiCom.Net.Driver
{
    internal class ConnectManager : IDisposable
    {
        public delegate void SocketConnectedEventHandler(object sender, TcpClient client);

        public event SocketConnectedEventHandler OnSocketConnected;
        public event Utility.TimerManager.TimeoutEventHandler OnTimeout;

        private const int THREAD_SLEEP_INTERVAL = 100;
        private const int THREAD_STOP_INTERVAL = 1000;
        private const string CLASS_NAME = "ConnectManager";

        private Structure.Configurtion _config;
        private Structure.HSMSMode _hsmsMode;
        private string _ipAddress;
        private int _portNo;

        private System.Threading.Thread _connectThread = null;

        private TcpListener _listener;
        private bool _runningThread;
        private readonly Utility.Logger.Logger _logger;

        private bool _disposed;

        public ConnectManager(HSMSDriver driver)
        {
            this._disposed = false;
            this._listener = null;
            this._logger = driver._logger;
        }

        ~ConnectManager()
        {
            Dispose(false);
        }

        public void Initialize(Structure.Configurtion configuration)
        {
            this._config = configuration;

            this._hsmsMode = configuration.HSMSModeConfig.HSMSMode;

            if (this._hsmsMode == Structure.HSMSMode.Active)
            {
                this._ipAddress = configuration.HSMSModeConfig.RemoteIPAddress;
                this._portNo = configuration.HSMSModeConfig.RemotePortNo;
            }
            else
            {
                this._ipAddress = configuration.HSMSModeConfig.LocalIPAddress;
                this._portNo = configuration.HSMSModeConfig.LocalPortNo;
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "Connect Manager Initialize");
        }

        public void Terminate()
        {
            this._runningThread = false;

            if (this._listener != null)
            {
                this._listener.Stop();
                this._listener = null;
            }

            lock (this)
            {
                if (this._connectThread != null)
                {
                    this._connectThread.Join(new TimeSpan(THREAD_STOP_INTERVAL * 10));
                }
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Terminate", "Connect Manager Terminate");
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    this._runningThread = false;

                    if (this._listener != null)
                    {
                        this._listener.Stop();
                        this._listener = null;
                    }

                    lock (this)
                    {
                        if (this._connectThread != null)
                        {
                            this._connectThread.Join(new TimeSpan(THREAD_STOP_INTERVAL * 10));
                        }
                    }

                    if (this._config != null)
                    {
                        this._config = null;
                    }

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Dispose", "Connect Manager Dispose");
                }

                this._disposed = true;
            }
        }

        public void Start()
        {
            if (this._runningThread == false)
            {
                this._runningThread = true;

                this._connectThread = new System.Threading.Thread(ProcRun)
                {
                    Name = "Connect Manager : ProcRun",
                    IsBackground = true
                };

                this._connectThread.Start();

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Start", "Connect Manager Thread Start");
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Start", "Connect Manager Thread Already Started");
            }
        }

        private void ProcRun()
        {
            TcpClient client;
            int retryCount = this._config.HSMSModeConfig.T5 * 1000;
            bool t5Ocurrence = false;
            StringBuilder logText = new StringBuilder();

            while (this._runningThread == true)
            {
                try
                {
                    if (retryCount >= (this._config.HSMSModeConfig.T5 * 1000))
                    {
                        retryCount = 0;

                        if (this._config.HSMSModeConfig.HSMSMode == Structure.HSMSMode.Active)
                        {
                            if (t5Ocurrence == true)
                            {
                                this.OnTimeout?.Invoke(this, Structure.TimeoutType.T5);
                            }
                            else
                            {
                                t5Ocurrence = true;
                            }

                            client = new TcpClient();

                            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this._config.HSMSModeConfig.RemoteIPAddress), this._config.HSMSModeConfig.RemotePortNo);

                            client.Connect(endPoint);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(this._config.HSMSModeConfig.LocalIPAddress) == true)
                            {
                                this._listener = new TcpListener(IPAddress.Parse("0.0.0.0"), this._config.HSMSModeConfig.LocalPortNo);
                            }
                            else
                            {
                                this._listener = new TcpListener(IPAddress.Parse(this._config.HSMSModeConfig.LocalIPAddress), this._config.HSMSModeConfig.LocalPortNo);
                            }

                            this._listener.Start();

                            client = this._listener.AcceptTcpClient();

                            this._listener.Stop();
                        }

                        this.OnSocketConnected?.Invoke(this, client);

                        this._runningThread = false;
                    }
                }
                catch (SocketException ex)
                {
                    logText.Clear();

                    if (this._hsmsMode == Structure.HSMSMode.Active)
                    {
                        logText.AppendFormat("Connect Error (Active) ({0}:{1}) Error Code={2}, {3}",
                                              this._ipAddress,
                                              this._portNo,
                                              ex.ErrorCode,
                                              ex.Message);
                    }
                    else
                    {
                        logText.AppendFormat("Listen Error (Passive) ({0}:{1}) Error Code={2}, {3}",
                                              this._ipAddress,
                                              this._portNo,
                                              ex.ErrorCode,
                                              ex.Message);
                    }

                    this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning, logText.ToString());
                }
                catch (Exception ex)
                {
                    logText.Clear();

                    if (this._hsmsMode == Structure.HSMSMode.Active)
                    {
                        logText.AppendFormat("Connect Error (Active) ({0}:{1}) Error Message={2}",
                                              this._ipAddress,
                                              this._portNo,
                                              ex.Message);
                    }
                    else
                    {
                        logText.AppendFormat("Listen Error (Passive) ({0}:{1}) Error Message={2}",
                                              this._ipAddress,
                                              this._portNo,
                                              ex.Message);
                    }

                    this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning, logText.ToString());
                }

                retryCount += THREAD_SLEEP_INTERVAL;
                System.Threading.Thread.Sleep(THREAD_SLEEP_INTERVAL);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRun", "Connect Manager Thread Exit");
        }
    }
}