using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Utility.Logger
{
    /// <summary>
    /// Log write 발생 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="logLevel">Log level입니다.</param>
    /// <param name="logText">Log 내용입니다.</param>
    public delegate void LogWriteEventHandler(LogLevel logLevel, string logText);

    internal class Logger : IDisposable
    {
        public event LogWriteEventHandler OnWriteLog;

        private const string LOG_PATH_DEFAULT = @"C:\Logs";
        private const string LOG_PATH_SYSTEM = "GEM_DRIVER";

        private const int LOG_DELETE_HOUR = 1;

        private readonly LogWriter _gemLog;

        private readonly System.Threading.Timer _deleteTimer;

        private int _logExpirationDay;

        public Logger()
        {
            //this._driverLog = new LogWriter();
            this._gemLog = new LogWriter();

            this._logExpirationDay = 30;

            this._deleteTimer = new System.Threading.Timer(DeleteLogCallBack);

            DateTime historyDeleteHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, LOG_DELETE_HOUR, 0, 0);
            if (DateTime.Now.Hour >= LOG_DELETE_HOUR)
            {
                historyDeleteHour = historyDeleteHour.AddDays(1);
            }
            TimeSpan ts = historyDeleteHour - DateTime.Now;
            this._deleteTimer.Change(ts, new TimeSpan(0));

            this._gemLog.OnWriteLog += GemLog_OnWriteLog;
        }

        private void GemLog_OnWriteLog(LogLevel logLevel, string logText)
        {
            this.OnWriteLog?.Invoke(logLevel, logText);
        }

        public void Initialize()
        {
            this._gemLog.Initialize(LOG_PATH_DEFAULT, LogMode.None, LogType.GEM);
        }

        public void Initialize(Structure.GEMConfiguration config, string secsDriverName)
        {
            this._logExpirationDay = config.LogExpirationDay;
            this._gemLog.Initialize(string.Format(@"{0}\{1}\{2}", config.LogPath, secsDriverName, LOG_PATH_SYSTEM), config.LogEnabledGEM, LogType.GEM);
        }

        public void WriteGEM(string logText)
        {
            this._gemLog.Add(LogLevel.Information, logText);
        }

        public void WriteGEM(LogLevel level, string logText)
        {
            this._gemLog.Add(level, logText);
        }

        public void WriteGEM(Exception ex)
        {
            this._gemLog.Add(ex);
        }

        public void Dispose()
        {
            this._gemLog.OnWriteLog -= GemLog_OnWriteLog;

            if (this._gemLog != null)
                this._gemLog.Dispose();
        }

        private void DeleteLogCallBack(object obj)
        {
            string deleteResult;

            DateTime historyDeleteHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, LOG_DELETE_HOUR, 0, 0);
            historyDeleteHour = historyDeleteHour.AddDays(1);

            TimeSpan ts = historyDeleteHour - DateTime.Now;

            this._deleteTimer.Change(ts, new TimeSpan(0));

            deleteResult = this._gemLog.Delete(this._logExpirationDay);
            WriteGEM(deleteResult);
        }
    }
}