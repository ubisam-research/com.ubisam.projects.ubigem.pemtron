using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Utility.Logger
{
    /// <summary>
    /// Log level입니다.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Information입니다.
        /// </summary>
        Information = 0,
        /// <summary>
        /// Driver information입니다.
        /// </summary>
        HSMS = 1,
        /// <summary>
        /// 송신 관련입니다.
        /// </summary>
        Send = 2,
        /// <summary>
        /// 수신 관련입니다.
        /// </summary>
        Receive = 3,
        /// <summary>
        /// Warning입니다.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// 동작에 영향을 미치는 error입니다.
        /// </summary>
        Error = 5
    }

    internal enum LogType
    {
        GEM,
        Driver
    }

    /// <summary>
    /// Logging 모드입니다.
    /// </summary>
    public enum LogMode
    {
        /// <summary>
        /// Logging 하지 않는 모드입니다.
        /// </summary>
        None = 0,
        /// <summary>
        /// 시간 단위로 Logging하는 모드입니다.
        /// </summary>
        Hour = 1,
        /// <summary>
        /// 일자 단위로 Logging하는 모드입니다.
        /// </summary>
        Day = 2
    }

    [ComVisible(false)]
    internal class LogData
    {
        private static class SECS1HeaderSchema
        {
            public const int LENGTH_TOTAL = 10;
            public const int LENGTH_DEVICE_ID = 2;
            public const int LENGTH_SYSTEM_BYTES = 4;

            public const int INDEX_DEVICE_ID = 0;
            public const int INDEX_STREAM = 2;
            public const int INDEX_FUNCTION = 3;
            public const int INDEX_P_TYPE = 4;
            public const int INDEX_S_TYPE = 5;
            public const int INDEX_SYSTEMBYTES = 6;
        }

        private readonly string _text;

        public DateTime Time;
        public LogType LogType;
        public LogLevel Level;

        public LogData(LogLevel level, string logText)
        {
            this.LogType = LogType.Driver;
            this._text = logText;

            this.Time = DateTime.Now;
            this.Level = level;
        }

        public LogData(Exception ex)
        {
            this.LogType = LogType.Driver;
            this._text = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);

            this.Time = DateTime.Now;
            this.Level = LogLevel.Error;
        }

        public string GetLogText()
        {
            StringBuilder sb = new StringBuilder(1000);

            sb.Append(this.Time.ToString("[yyyy-MM-dd HH:mm:ss.fff] "));

            switch (this.Level)
            {
                case LogLevel.Information:
                    sb.Append("INFO ");
                    break;
                case LogLevel.HSMS:
                    sb.Append("HSMS ");
                    break;
                case LogLevel.Receive:
                    sb.Append("RECV ");
                    break;
                case LogLevel.Send:
                    sb.Append("SEND ");
                    break;
                case LogLevel.Warning:
                    sb.Append("WARN ");
                    break;
                case LogLevel.Error:
                    sb.Append("EXCP ");
                    break;
            }

            sb.AppendLine(this._text);

            return sb.ToString();
        }
    }
}