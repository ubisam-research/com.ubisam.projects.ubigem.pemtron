using System;

namespace UbiCom.Net.Utility.Logger
{
    /// <summary>
    /// Log write 발생 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="logLevel">Log level입니다.</param>
    /// <param name="logText">Log 내용입니다.</param>
    public delegate void LogWriteEventHandler(object sender, LogLevel logLevel, string logText);

    internal class Logger : IDisposable
    {
        public event LogWriteEventHandler OnSECS1WriteLog;
        public event LogWriteEventHandler OnSECS2WriteLog;

        private const string LOG_PATH_SYSTEM = "DRIVER";
        private const string LOG_PATH_SECS_1 = "SECS-I";
        private const string LOG_PATH_SECS_2 = "SECS-II";

        private const int LOG_DELETE_HOUR = 1;

        private LogWriter _driverLog;
        private LogWriter _secs1Log;
        private LogWriter _secs2Log;

        private readonly System.Threading.Timer _deleteTimer;

        private int _logExpirationDay;
        private bool _disposed;

        public Logger()
        {
            this._driverLog = null;
            this._secs1Log = null;
            this._secs2Log = null;
            this._disposed = false;

            this._logExpirationDay = 30;

            this._deleteTimer = new System.Threading.Timer(DeleteLogCallBack);

            DateTime historyDeleteHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, LOG_DELETE_HOUR, 0, 0);
            if (DateTime.Now.Hour >= LOG_DELETE_HOUR)
            {
                historyDeleteHour = historyDeleteHour.AddDays(1);
            }
            TimeSpan ts = historyDeleteHour - DateTime.Now;
            this._deleteTimer.Change(ts, new TimeSpan(0));
        }

        ~Logger()
        {
            Dispose(false);
        }

        private void Secs1Log_OnSECS1WriteLog(object sender, LogLevel logLevel, string logText)
        {
            this.OnSECS1WriteLog?.Invoke(this, logLevel, logText);
        }

        private void Secs2Log_OnSECS2WriteLog(object sender, LogLevel logLevel, string logText)
        {
            this.OnSECS2WriteLog?.Invoke(this, logLevel, logText);
        }

        public void Initialize(Structure.Configurtion config)
        {
            this._logExpirationDay = config.LogExpirationDay;

            if (this._secs1Log != null)
            {
                this._secs1Log.OnSECS2WriteLog -= Secs1Log_OnSECS1WriteLog;
            }

            if (this._secs2Log != null)
            {
                this._secs2Log.OnSECS2WriteLog -= Secs2Log_OnSECS2WriteLog;
            }

            if( null == this._driverLog )
            {
                this._driverLog = new LogWriter();
                this._secs1Log = new LogWriter();
                this._secs2Log = new LogWriter();
            }
            else
            {
                this._driverLog.Dispose();
                this._secs1Log.Dispose();
                this._secs2Log.Dispose();
            }

            this._secs1Log.OnSECS1WriteLog += Secs1Log_OnSECS1WriteLog;
            this._secs2Log.OnSECS2WriteLog += Secs2Log_OnSECS2WriteLog;

            this._driverLog.Initialize(string.Format(@"{0}\{1}\{2}", config.LogPath, config.DriverName, LOG_PATH_SYSTEM), config.LogEnabledSystem, LogType.Driver);
            this._secs1Log.Initialize(string.Format(@"{0}\{1}\{2}", config.LogPath, config.DriverName, LOG_PATH_SECS_1), config.LogEnabledSECS1, LogType.SECS1);
            this._secs2Log.Initialize(string.Format(@"{0}\{1}\{2}", config.LogPath, config.DriverName, LOG_PATH_SECS_2), config.LogEnabledSECS2, LogType.SECS2);
        }

        public void WriteDriver(DateTime time, string logText)
        {
            if (this._driverLog != null)
            {
                this._driverLog.Add(new DateTime(time.Ticks), LogLevel.Information, logText);
            }
        }

        public void WriteDriver(DateTime time, LogLevel level, string className, string methodName)
        {
            if (this._driverLog != null)
            {
                this._driverLog.Add(new DateTime(time.Ticks), level, string.Format("{0}({1})", className, methodName));
            }
        }

        public void WriteDriver(DateTime time, LogLevel level, string className, string methodName, string logText)
        {
            if (this._driverLog != null)
            {
                this._driverLog.Add(new DateTime(time.Ticks), level, string.Format("{0}({1}) {2}", className, methodName, logText));
            }
        }

        public void WriteSECS1(DateTime time, string logText)
        {
            if (this._secs1Log != null)
            {
                this._secs1Log.Add(new DateTime(time.Ticks), LogLevel.Information, logText);
            }
        }

        public void WriteSECS1(DateTime time, LogLevel level, string logText)
        {
            if (this._secs1Log != null)
            {
                this._secs1Log.Add(new DateTime(time.Ticks), level, logText);
            }
        }

        public void WriteSECS1(DateTime time, LogLevel level, byte[] rawData)
        {
            if (this._secs1Log != null)
            {
                this._secs1Log.Add(new DateTime(time.Ticks), level, rawData);
            }
        }

        public void WriteSECS2(DateTime time, string logText)
        {
            if (this._secs2Log != null)
            {
                this._secs2Log.Add(new DateTime(time.Ticks), LogLevel.Information, logText);
            }
        }

        public void WriteSECS2(DateTime time, LogLevel level, string logText)
        {
            if (this._secs2Log != null)
            {
                this._secs2Log.Add(new DateTime(time.Ticks), level, logText);
            }
        }

        public void WriteSECS2(DateTime time, LogLevel level, Structure.SECSMessage message)
        {
            if (this._secs2Log != null && message.ControlMessageType == Structure.ControlMessageType.DataMessage)
            {
                this._secs2Log.Add(new DateTime(time.Ticks), level, message);
            }
        }

        public void WriteException(DateTime time, string className, string methodName, Exception ex)
        {
            if (this._driverLog != null)
                this._driverLog.Add(time, className, methodName, ex);

            if (this._secs1Log != null)
                this._secs1Log.Add(time, className, methodName, ex);

            if (this._secs2Log != null)
                this._secs2Log.Add(time, className, methodName, ex);
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
                    if (this._driverLog != null)
                        this._driverLog.Dispose();

                    if (this._secs1Log != null)
                        this._secs1Log.Dispose();

                    if (this._secs2Log != null)
                        this._secs2Log.Dispose();
                }

                this._disposed = true;
            }
        }

        private void DeleteLogCallBack(object obj)
        {
            string deleteResult;

            DateTime historyDeleteHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, LOG_DELETE_HOUR, 0, 0);
            historyDeleteHour = historyDeleteHour.AddDays(1);

            TimeSpan ts = historyDeleteHour - DateTime.Now;

            this._deleteTimer.Change(ts, new TimeSpan(0));

            if (this._driverLog != null)
            {
                deleteResult = this._driverLog.Delete(this._logExpirationDay);
                WriteDriver(DateTime.Now, deleteResult);
            }

            if (this._secs1Log != null)
            {
                _ = this._secs1Log.Delete(this._logExpirationDay);
            }

            if (this._secs2Log != null)
            {
                _ = this._secs2Log.Delete(this._logExpirationDay);
            }
        }
    }
}