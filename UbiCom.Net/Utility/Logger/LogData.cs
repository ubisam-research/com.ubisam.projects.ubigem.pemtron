using System;
using System.Collections.Generic;
using System.Text;

namespace UbiCom.Net.Utility.Logger
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
        SECS1,
        SECS2,
        Driver
    }

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

        private const string EMPTY_STRING_START = "                               ";
        private const string EMPTY_STRING_SECS1 = "        ";

        private readonly string _text;
        private readonly byte[] _rawData;
        private readonly Structure.SECSMessage _message;
        private readonly string _loggingTime;

        public DateTime Time;
        public LogType LogType;
        public LogLevel Level;

        public LogData(LogType logType, DateTime time, string loggingTime, LogLevel level, string logText)
        {
            this.LogType = logType;
            this._text = logText;

            this.Time = new DateTime(time.Ticks);
            this._loggingTime = loggingTime;
            this.Level = level;
        }

        public LogData(LogType logType, DateTime time, string loggingTime, LogLevel level, byte[] rawData)
        {
            this.LogType = logType;
            this._rawData = rawData;

            this.Time = new DateTime(time.Ticks);
            this._loggingTime = loggingTime;
            this.Level = level;
        }

        public LogData(LogType logType, DateTime time, string loggingTime, LogLevel level, Structure.SECSMessage message)
        {
            this.LogType = logType;
            this._message = message;

            this.Time = new DateTime(time.Ticks);
            this._loggingTime = loggingTime;
            this.Level = level;
        }

        public LogData(LogType logType, DateTime time, string loggingTime, string className, string methodName, Exception ex)
        {
            this.LogType = logType;

            if (string.IsNullOrEmpty(className) == false)
            {
                this._text = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            }
            else
            {
                this._text = string.Format("{0}({1}) - {2}{3}{4}", className, methodName, ex.Message, Environment.NewLine, ex.StackTrace);
            }

            this.Time = new DateTime(time.Ticks);
            this._loggingTime = loggingTime;
            this.Level = LogLevel.Error;
        }

        public string GetLogText()
        {
            StringBuilder sb = new StringBuilder(1000);

            sb.AppendFormat("[{0}] ", this._loggingTime);

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

            if (this.LogType == LogType.SECS1)
            {
                MakeSECS1Log(sb);
            }
            else if (this.LogType == LogType.SECS2)
            {
                if (this._message != null)
                {
                    MakeSECS2Log(sb);
                }
                else
                {
                    sb.AppendLine(this._text);
                }
            }
            else
            {
                sb.AppendLine(this._text);
            }

            return sb.ToString();
        }

        private void MakeSECS1Log(StringBuilder sb)
        {
            int stream;
            byte[] systemBytes;
            int totalLength;

            if (this._rawData == null)
            {
                sb.Append(this._text);
                sb.AppendLine();
            }
            else if (this._rawData[SECS1HeaderSchema.INDEX_S_TYPE] != Structure.ControlMessageType.DataMessage.GetHashCode())
            {
                for (int i = 0; i < SECS1HeaderSchema.LENGTH_TOTAL; i++)
                {
                    sb.AppendFormat("{0:X2} ", this._rawData[i]);
                }

                sb.AppendFormat("{0}Length={1}{2}({3})",
                    EMPTY_STRING_SECS1,
                    this._rawData.Length.ToString().PadRight(5),
                    EMPTY_STRING_SECS1,
                    ((Structure.ControlMessageType)this._rawData[SECS1HeaderSchema.INDEX_S_TYPE]).ToString().PadRight(16));

                sb.AppendLine();
            }
            else
            {
                systemBytes = new byte[SECS1HeaderSchema.LENGTH_SYSTEM_BYTES];

                Array.Copy(this._rawData, SECS1HeaderSchema.INDEX_SYSTEMBYTES, systemBytes, 0, SECS1HeaderSchema.LENGTH_SYSTEM_BYTES);

                for (int i = 0; i < SECS1HeaderSchema.LENGTH_TOTAL; i++)
                {
                    sb.AppendFormat("{0:X2} ", this._rawData[i]);
                }

                stream = this._rawData[SECS1HeaderSchema.INDEX_STREAM] & 0xff;

                if (stream > 0x80)
                {
                    stream -= 0x80;

                    sb.AppendFormat("{0}Length={1}{2}(S{3}F{4}W) [SB={5:X8}]",
                        EMPTY_STRING_SECS1,
                        this._rawData.Length.ToString().PadRight(5),
                        EMPTY_STRING_SECS1,
                        stream,
                        this._rawData[SECS1HeaderSchema.INDEX_FUNCTION] & 0xff,
                        Utility.Converter.ConvertBytes2Int(systemBytes));
                }
                else
                {
                    sb.AppendFormat("{0}Length={1}{2}(S{3}F{4}) [SB={5:X8}]",
                        EMPTY_STRING_SECS1,
                        this._rawData.Length.ToString().PadRight(5),
                        EMPTY_STRING_SECS1,
                        stream,
                        this._rawData[SECS1HeaderSchema.INDEX_FUNCTION] & 0xff,
                        Utility.Converter.ConvertBytes2Int(systemBytes));
                }

                if (this._rawData.Length > SECS1HeaderSchema.LENGTH_TOTAL)
                {
                    totalLength = this._rawData.Length;

                    for (int i = SECS1HeaderSchema.LENGTH_TOTAL; i < totalLength; i++)
                    {
                        if (i % 20 == 10)
                        {
                            sb.AppendFormat("{0}{1}", Environment.NewLine, EMPTY_STRING_START);
                        }

                        sb.AppendFormat("{0:X2} ", this._rawData[i]);
                    }
                }

                sb.AppendLine();
            }
        }

        private string MakeSECS2Log(StringBuilder sb)
        {
            int itemCount;
            int itemIndex;
            int itemLevel;
            string makeResult;

            if (string.IsNullOrEmpty(this._message.Name) == true)
            {
                sb.AppendFormat("S{0}F{1} ", this._message.Stream, this._message.Function);
            }
            else
            {
                sb.AppendFormat("S{0}F{1}:{2} ", this._message.Stream, this._message.Function, this._message.Name);
            }

            if (this._message.WaitBit == true)
            {
                sb.Append("W ");
            }

            sb.AppendLine(string.Format("SystemBytes={0:X8}", this._message.SystemBytes));

            if (this._message.Body != null)
            {
                itemCount = this._message.Body.Count;
                itemIndex = 0;
                itemLevel = 0;

                while (itemIndex < itemCount)
                {
                    if (this._message.Body.AsList[itemIndex].Format == Structure.SECSItemFormat.L)
                    {
                        makeResult = MakeSECS2List(sb, this._message.Body.AsList, ref itemIndex, ref itemLevel);

                        if (string.IsNullOrEmpty(makeResult) == false)
                        {
                            sb.AppendLine(string.Format("Make Failed={0}", makeResult));

                            break;
                        }
                    }
                    else
                    {
                        MakeSECS2Item(sb, this._message.Body.AsList[itemIndex], ref itemIndex, itemLevel);
                    }
                }
            }

            return sb.ToString();
        }

        private string MakeSECS2List(StringBuilder sb, List<Structure.SECSItem> item, ref int itemIndex, ref int itemLevel)
        {
            string result;
            int itemLength;

            result = string.Empty;

            try
            {
                if (item[itemIndex].Length == 0)
                {
                    if (string.IsNullOrEmpty(item[itemIndex].Name) == true)
                    {
                        sb.AppendLine(string.Format("{0}<L,0>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4)));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("{0}<L,0 [{1}]>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item[itemIndex].Name));
                    }

                    itemIndex++;
                }
                else
                {
                    if (string.IsNullOrEmpty(item[itemIndex].Name) == true)
                    {
                        sb.AppendLine(string.Format("{0}<L,{1}", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item[itemIndex].Length));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("{0}<L,{1} [{2}]", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item[itemIndex].Length, item[itemIndex].Name));
                    }

                    itemLength = item[itemIndex].Length;
                    itemLevel++;
                    itemIndex++;

                    for (int i = 0; i < itemLength; i++)
                    {
                        if (item.Count <= itemIndex)
                        {
                            result = "index error";

                            break;
                        }
                        else
                        {
                            if (item[itemIndex].Format == Structure.SECSItemFormat.L)
                            {
                                result = MakeSECS2List(sb, item, ref itemIndex, ref itemLevel);

                                if (string.IsNullOrEmpty(result) == false)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                MakeSECS2Item(sb, item[itemIndex], ref itemIndex, itemLevel);
                            }
                        }
                    }

                    itemLevel--;

                    sb.AppendLine(string.Format("{0}>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4)));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(string.Format("### Log Write Exception(Write-COM) : {0} [{1}]\r\n{2}", ex.GetType(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), ex.Message));
            }

            return result;
        }

        private static void MakeSECS2Item(StringBuilder sb, Structure.SECSItem item, ref int itemIndex, int itemLevel)
        {
            if (string.IsNullOrEmpty(item.Name) == true)
            {
                if (item.Format == Structure.SECSItemFormat.A)
                {
                    int length;
                    int byteLength = Encoding.Default.GetByteCount(item.Value);
                    int padLength;

                    if (item.Length > 0)
                    {
                        length = item.Length;
                        padLength = item.Value.ToString().Length + length - byteLength;
                    }
                    else
                    {
                        length = byteLength;
                        padLength = length;
                    }

                    if (padLength < 0)
                    {
                        padLength = 0;
                    }

                    sb.AppendLine(string.Format("{0}<{1},{2} '{3}'>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, length, item.Value.ToString().PadRight(padLength)));
                }
                else
                {
                    if (item.Value != null && item.Value.GetValue() != null)
                    {
                        sb.AppendLine(string.Format("{0}<{1},{2} '{3}'>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, item.Value.Length, item.Value.ToString().PadRight(item.Length)));
                    }
                    else
                    {
                        if (item.Length > 0)
                        {
                            sb.AppendLine(string.Format("{0}<{1},{2} '{3}'>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, item.Length, string.Empty));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("{0}<{1},{2} '{3}'>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, 0, string.Empty));
                        }
                    }
                }
            }
            else
            {
                if (item.Format == Structure.SECSItemFormat.A)
                {
                    int length;
                    int byteLength = Encoding.Default.GetByteCount(item.Value);
                    int padLength;

                    if (item.Length > 0)
                    {
                        length = item.Length;
                        padLength = item.Value.ToString().Length + length - byteLength;
                    }
                    else
                    {
                        length = byteLength;
                        padLength = length;
                    }

                    if (padLength < 0)
                    {
                        padLength = 0;
                    }

                    sb.AppendLine(string.Format("{0}<{1},{2} '{3}' [{4}]>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, length, item.Value.ToString().PadRight(padLength), item.Name));
                }
                else
                {
                    if (item.Value != null && item.Value.GetValue() != null)
                    {
                        if (item.Length > 0)
                        {
                            sb.AppendLine(string.Format("{0}<{1},{2} '{3}' [{4}]>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, item.Length, item.Value.ToString().PadRight(item.Length), item.Name));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("{0}<{1},{2} '{3}' [{4}]>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, item.Value.Length, item.Value.ToString().PadRight(item.Length), item.Name));
                        }
                    }
                    else
                    {
                        if (item.Length > 0)
                        {
                            sb.AppendLine(string.Format("{0}<{1},{2} '{3}' [{4}]>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, item.Length, string.Empty, item.Name));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("{0}<{1},{2} '{3}' [{4}]>", EMPTY_STRING_START.PadRight(EMPTY_STRING_START.Length + itemLevel * 4), item.Format, 0, string.Empty, item.Name));
                        }
                    }
                }
            }

            itemIndex++;
        }
    }
}