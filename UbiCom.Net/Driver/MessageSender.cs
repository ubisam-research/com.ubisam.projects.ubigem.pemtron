using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UbiCom.Net.Driver
{
    internal class MessageSender : IDisposable
    {
        internal event SentControlMessageEventHandler OnSentControlMessage;
        internal event SentSECSMessageEventHandler OnSentSECSPrimaryMessage;
        internal event SentSECSMessageEventHandler OnSentSECSSecondaryMessage;

        internal event Utility.TimerManager.T3TimeoutEventHandler OnT3Timeout;

        private const int MAX_MESSAGE_QUEUE_SIZE = 100;
        private const int THREAD_SLEEP_INTERVAL = 10;
        private const int TOTAL_LENGTH_SIZE = 4;
        private const int THREAD_STOP_INTERVAL = 1000;
        private const string CLASS_NAME = "MessageSender";

        private HSMSDriver _driver;

        private readonly Queue _quePrimary;
        private readonly Queue _queSecondary;
        private Thread _queueThread;

        private Utility.TimerManager _timer;
        private Utility.Logger.Logger _logger;

        private bool _threadFlag;
        private BinaryWriter _socketWriter = null;

        private bool _disposed;

        public int PrimaryCount
        {
            get { return this._quePrimary.Count; }
        }

        public MessageSender()
        {
            this._quePrimary = new System.Collections.Queue();
            this._queSecondary = new System.Collections.Queue();

            this._threadFlag = false;

            this._timer = null;
            this._disposed = false;
        }

        ~MessageSender()
        {
            Dispose(false);
        }

        void Timer_OnT3Timeout(object sender, Structure.SECSMessage message)
        {
            this.OnT3Timeout?.Invoke(this, message);
        }

        public void Initialize(HSMSDriver driver)
        {
            this._quePrimary.Clear();
            this._queSecondary.Clear();

            if (this._timer != null)
            {
                this._timer.OnT3Timeout -= Timer_OnT3Timeout;
            }

            this._driver = driver;
            this._timer = driver._timerMgr;

            this._timer.OnT3Timeout += Timer_OnT3Timeout;

            this._socketWriter = new BinaryWriter(driver._socket.GetStream());
            this._logger = driver._logger;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "Message Sender Initialize");

            this._threadFlag = true;

            if (driver.Config.SECSMode == Structure.SECSMode.HSMS)
            {
                this._queueThread = new Thread(ProcRun)
                {
                    Name = "Message Sender"
                };

                this._queueThread.Start();
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "Message Sender Thread Start");
        }

        public void Terminate()
        {
            this._threadFlag = false;

            lock (this)
            {
                if (this._queueThread != null)
                {
                    this._queueThread.Join(new TimeSpan(THREAD_STOP_INTERVAL * 10));
                }
            }

            if (this._socketWriter != null)
            {
                this._socketWriter.Flush();
                this._socketWriter.Close();
                this._socketWriter = null;
            }

            if (this._timer != null)
            {
                this._timer.Stop();
            }

            lock (this._quePrimary)
            {
                this._quePrimary.Clear();
            }

            lock (this._queSecondary)
            {
                this._queSecondary.Clear();
            }

            if (this._logger != null)
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Terminate", "Sender Terminate");
            }
        }

        public Structure.MessageError SendControlMessage(Structure.SECSMessage message, int reasonCode = 0)
        {
            Structure.MessageError result;
            byte[] logData;

            if (message == null)
            {
                result = Structure.MessageError.DataIsNull;

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "SendControlMessage", "Message is null");
            }
            else if (this._socketWriter == null)
            {
                result = Structure.MessageError.SocketIsNull;

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "SendControlMessage", "Binary Writer is null");
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendControlMessage",
                    string.Format("Control Message Type={0} ({1:X8})", message.ControlMessageType, message.SystemBytes));

                using (MessageEncoder encoder = new MessageEncoder(this._driver, message))
                {
                    result = encoder.GetEncodingData(out byte[] sendData);

                    if (result == Structure.MessageError.Ok)
                    {
                        try
                        {
                            if (message.ControlMessageType == Structure.ControlMessageType.SelectRequest ||
                                message.ControlMessageType == Structure.ControlMessageType.LinktestRequest)
                            {
                                this._timer.Start(Structure.TimeoutType.T6);
                                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendControlMessage", "T6 Timer Start");
                            }

                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendControlMessage", string.Format("Write Start ({0:X8})", message.SystemBytes));
                            this._socketWriter.Write(sendData);
                            this._socketWriter.Flush();
                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendControlMessage", string.Format("Write Complete ({0:X8})", message.SystemBytes));

                            if (this._driver._config.LogEnabledSECS1 != Structure.LogMode.None)
                            {
                                logData = new byte[sendData.Length - TOTAL_LENGTH_SIZE];

                                Array.Copy(sendData, TOTAL_LENGTH_SIZE, logData, 0, logData.Length);

                                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Send, logData);

                                logData = null;
                            }

                            this.OnSentControlMessage?.Invoke(this, message);
                        }
                        catch (Exception ex)
                        {
                            this._logger.WriteException(DateTime.Now, CLASS_NAME, "SendControlMessage", ex);

                            result = Structure.MessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }
                }
            }

            if (reasonCode != 0)
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendControlMessage",
                    string.Format("Control Message Type={0} ({1:X8}) Result={2} Reason Code={3}", message.ControlMessageType, message.SystemBytes, result, reasonCode));
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendControlMessage",
                    string.Format("Control Message Type={0} ({1:X8}) Result={2}", message.ControlMessageType, message.SystemBytes, result));
            }

            return result;
        }

        public Structure.MessageError SendPrmaryMessage(Structure.SECSMessage message)
        {
            Structure.MessageError result;

            if (this._quePrimary.Count >= MAX_MESSAGE_QUEUE_SIZE)
            {
                result = Structure.MessageError.MessageQueueOverflow;

                if (message.WaitBit == true)
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "SendPrmaryMessage",
                        string.Format("S{0}F{1}W ({2:X8}) Result={3} Queue Count={4}", message.Stream, message.Function, message.SystemBytes, result, this._quePrimary.Count));

                    this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning,
                        string.Format("S{0}F{1}W ({2:X8}) Result={3} Queue Count={4}", message.Stream, message.Function, message.SystemBytes, result, this._quePrimary.Count));
                }
                else
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "SendPrmaryMessage",
                        string.Format("S{0}F{1} ({2:X8}) Result={3} Queue Count={4}", message.Stream, message.Function, message.SystemBytes, result, this._quePrimary.Count));

                    this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning,
                        string.Format("S{0}F{1} ({2:X8}) Result={3} Queue Count={4}", message.Stream, message.Function, message.SystemBytes, result, this._quePrimary.Count));
                }
            }
            else
            {
                result = ValidateMessageStructure(message, out string errorText);

                if (result == Structure.MessageError.Ok)
                {
                    lock (this._quePrimary)
                    {
                        this._quePrimary.Enqueue(message);
                    }

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendPrmaryMessage",
                        string.Format("Enqueue:S{0}F{1}W ({2:X8}) Result={3} Queue Count={4}", message.Stream, message.Function, message.SystemBytes, result, this._quePrimary.Count));
                }
                else
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "SendPrmaryMessage",
                        string.Format("Invalid Message Structure:S{0}F{1} ({2:X8}), Error Text={3}", message.Stream, message.Function, message.SystemBytes, errorText));

                    this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning,
                        string.Format("Invalid Message Structure:S{0}F{1} ({2:X8}), Error Text={3}", message.Stream, message.Function, message.SystemBytes, errorText));
                }
            }

            return result;
        }

        public Structure.MessageError SendSecondaryMessage(Structure.SECSMessage message)
        {
            Structure.MessageError result;

            result = ValidateMessageStructure(message, out string errorText);

            if (result == Structure.MessageError.Ok)
            {
                lock (this._queSecondary)
                {
                    this._queSecondary.Enqueue(message);
                }

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SendSecondaryMessage",
                    string.Format("Enqueue:S{0}F{1}W ({2:X8}) Result={3} Queue Count={4}", message.Stream, message.Function, message.SystemBytes, result, this._queSecondary.Count));
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "SendSecondaryMessage",
                    string.Format("Invalid Message Structure:S{0}F{1} ({2:X8}), Error Text={3}", message.Stream, message.Function, message.SystemBytes, errorText));

                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Warning,
                    string.Format("Invalid Message Structure:S{0}F{1} ({2:X8}), Error Text={3}", message.Stream, message.Function, message.SystemBytes, errorText));
            }

            return result;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this._threadFlag = false;

            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (this._driver != null)
                        this._driver = null;

                    lock (this._quePrimary)
                    {
                        this._quePrimary.Clear();
                    }

                    lock (this._queSecondary)
                    {
                        this._queSecondary.Clear();
                    }

                    if (this._timer != null)
                    {
                        this._timer.Dispose();
                        this._timer = null;
                    }

                    if (this._socketWriter != null)
                    {
                        this._socketWriter.Close();
                        this._socketWriter.Dispose();
                        this._socketWriter = null;
                    }

                    if (this._logger != null)
                        this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Dispose", "Message Sender Dispose");
                }

                this._disposed = true;
            }
        }

        private void ProcRun()
        {
            Structure.SECSMessage message;

            while (this._threadFlag)
            {
                try
                {
                    try
                    {
                        if (this._queSecondary.Count > 0)
                        {
                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRun",
                                string.Format("Secondary Queue Count={0}", this._queSecondary.Count));

                            lock (this._queSecondary)
                            {
                                message = this._queSecondary.Dequeue() as Structure.SECSMessage;
                            }

                            WriteSecondaryMessage(message);
                        }
                        if (this._quePrimary.Count > 0)
                        {
                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRun",
                                string.Format("Primary Queue Count={0}", this._quePrimary.Count));

                            lock (this._quePrimary)
                            {
                                message = this._quePrimary.Dequeue() as Structure.SECSMessage;
                            }

                            WritePrimaryMessage(message);
                        }
                    }
                    catch (System.IO.IOException ex)
                    {
                        this._logger.WriteException(DateTime.Now, CLASS_NAME, "ProcRun", ex);

                        this._threadFlag = false;

                        if (this._driver != null)
                        {
                            this._driver.SetDisconnected(true);
                        }

                        System.Diagnostics.Debug.Print(ex.Message);

                        continue;
                    }
                    catch (Exception ex)
                    {
                        this._logger.WriteException(DateTime.Now, CLASS_NAME, "ProcRun", ex);

                        System.Diagnostics.Debug.Print(ex.Message);
                    }

                    continue;
                }
                finally
                {
                    message = null;

                    Thread.Sleep(THREAD_SLEEP_INTERVAL);
                }
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRun", "Sender Thread Exit");
        }

        private Structure.MessageError WritePrimaryMessage(Structure.SECSMessage message)
        {
            Structure.MessageError result;
            byte[] logData;
            byte[] sendData;

            if (message == null)
            {
                result = Structure.MessageError.DataIsNull;

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "WritePrimaryMessage", "Message is null");
            }
            else
            {
                if (message.WaitBit == true)
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WritePrimaryMessage",
                        string.Format("S{0}F{1}W ({2:X8})", message.Stream, message.Function, message.SystemBytes));
                }
                else
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WritePrimaryMessage",
                        string.Format("S{0}F{1} ({2:X8})", message.Stream, message.Function, message.SystemBytes));
                }

                using (MessageEncoder encoder = new MessageEncoder(this._driver, message))
                {
                    sendData = null;

                    result = encoder.GetEncodingData(out sendData);

                    if (result == Structure.MessageError.Ok)
                    {
                        if (message.WaitBit == true)
                        {
                            this._timer.StartT3(message);
                        }

                        try
                        {
                            if (this._driver._config.LogEnabledSECS1 != Structure.LogMode.None)
                            {
                                logData = new byte[sendData.Length - TOTAL_LENGTH_SIZE];

                                Array.Copy(sendData, TOTAL_LENGTH_SIZE, logData, 0, logData.Length);

                                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Send, logData);

                                logData = null;
                            }

                            if (this._driver._config.LogEnabledSECS2 != Structure.LogMode.None)
                            {
                                this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Send, message);
                            }

                            this._socketWriter.Write(sendData);

                            this._socketWriter.Flush();
                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WritePrimaryMessage", string.Format("Write Complete ({0:X8})", message.SystemBytes));

                            this.OnSentSECSPrimaryMessage?.Invoke(this, message);
                        }
                        catch (Exception ex)
                        {
                            this._logger.WriteException(DateTime.Now, CLASS_NAME, "WritePrimaryMessage", ex);

                            result = Structure.MessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    if (sendData != null)
                        sendData = null;
                }

                if (message.WaitBit == true)
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WritePrimaryMessage",
                        string.Format("S{0}F{1}W ({2:X8}) Result={3}", message.Stream, message.Function, message.SystemBytes, result));
                }
                else
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WritePrimaryMessage",
                        string.Format("S{0}F{1} ({2:X8}) Result={3}", message.Stream, message.Function, message.SystemBytes, result));
                }
            }

            return result;
        }

        private Structure.MessageError WriteSecondaryMessage(Structure.SECSMessage message)
        {
            Structure.MessageError result;
            byte[] logData;
            byte[] sendData;

            if (message == null)
            {
                result = Structure.MessageError.DataIsNull;

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "WriteSecondaryMessage", "Message is null");
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WriteSecondaryMessage",
                    string.Format("S{0}F{1} ({2:X8})", message.Stream, message.Function, message.SystemBytes));

                using (MessageEncoder encoder = new MessageEncoder(this._driver, message))
                {
                    sendData = null;

                    result = encoder.GetEncodingData(out sendData);

                    if (result == Structure.MessageError.Ok)
                    {
                        try
                        {
                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WriteSecondaryMessage", string.Format("Write Start ({0:X8})", message.SystemBytes));

                            if (this._driver._config.LogEnabledSECS1 != Structure.LogMode.None)
                            {
                                logData = new byte[sendData.Length - TOTAL_LENGTH_SIZE];

                                Array.Copy(sendData, TOTAL_LENGTH_SIZE, logData, 0, logData.Length);

                                this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Send, logData);

                                logData = null;
                            }

                            if (this._driver._config.LogEnabledSECS2 != Structure.LogMode.None)
                            {
                                this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Send, message);
                            }

                            this._socketWriter.Write(sendData);
                            this._socketWriter.Flush();
                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WriteSecondaryMessage", string.Format("Write Complete ({0:X8})", message.SystemBytes));

                            this.OnSentSECSSecondaryMessage?.Invoke(this, message);
                        }
                        catch (Exception ex)
                        {
                            this._logger.WriteException(DateTime.Now, CLASS_NAME, "WriteSecondaryMessage", ex);

                            result = Structure.MessageError.Unknown;

                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    if (sendData != null)
                        sendData = null;

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "WriteSecondaryMessage",
                        string.Format("S{0}F{1} ({2:X8}) Result={3}", message.Stream, message.Function, message.SystemBytes, result));
                }
            }

            return result;
        }

        private Structure.MessageError ValidateMessageStructure(Structure.SECSMessage message, out string errorText)
        {
            Structure.MessageError result;
            Structure.SECSItem secsItem;
            int index;

            result = Structure.MessageError.Ok;
            errorText = string.Empty;

            if (message.Body != null)
            {
                if (message.Body.Count > 0)
                {
                    secsItem = message.Body.AsList[0];

                    if (secsItem.Format == Structure.SECSItemFormat.L)
                    {
                        if (secsItem.Length > 0)
                        {
                            index = 0;

                            result = ValidateMessageStructure(message.Body.AsList, ref index, secsItem.Length, out int childItemCount, out errorText);

                            if (result == Structure.MessageError.Ok && secsItem.Length != childItemCount)
                            {
                                result = Structure.MessageError.InvalidMessageStructure;

                                errorText = string.Format("Index={0}, Item Name={1}, Item Length={2}, Child Item Count={3}", index, secsItem.Name, secsItem.Length, childItemCount);
                            }
                        }
                    }
                    else if (message.Body.AsList.Count > 1)
                    {
                        result = Structure.MessageError.InvalidMessageStructure;

                        errorText = string.Format("Body root child item count={0}", message.Body.AsList.Count);
                    }
                    else if (message.Body.AsList.Count == 1 &&
                        secsItem.Format != Structure.SECSItemFormat.A &&
                        secsItem.Format != Structure.SECSItemFormat.J)
                    {
                        if (secsItem.Value != null)
                        {
                            if (secsItem.Value.GetValue() is IList ilist)
                            {
                                if (secsItem.Length < ilist.Count)
                                {
                                }
                                else if (secsItem.Length > ilist.Count)
                                {
                                    secsItem.Length = ilist.Count;
                                }
                            }
                        }
                        else
                        {
                            result = Structure.MessageError.DataIsNull;
                            errorText = string.Format("Value is null:Item Name={0}", secsItem.Name);
                        }
                    }
                }
            }

            return result;
        }

        private Structure.MessageError ValidateMessageStructure(List<Structure.SECSItem> items, ref int index, int listCount, out int childItemCount, out string errorText)
        {
            Structure.MessageError result;
            int startIndex;
            int currentIndex;

            Structure.SECSItem secsItem;

            result = Structure.MessageError.Ok;
            childItemCount = 0;
            errorText = string.Empty;

            if (items.Count > index)
            {
                startIndex = index;

                for (int i = startIndex; i < (startIndex + listCount); i++)
                {
                    index++;
                    childItemCount++;

                    if (items.Count > index)
                    {
                        secsItem = items[index];

                        if (secsItem.Format == Structure.SECSItemFormat.L)
                        {
                            if (secsItem.Length > 0)
                            {
                                currentIndex = index;

                                result = ValidateMessageStructure(items, ref index, items[currentIndex].Length, out int subChildItemCount, out errorText);

                                if (result != Structure.MessageError.Ok)
                                {
                                    break;
                                }
                                else if (result == Structure.MessageError.Ok && items[currentIndex].Length != subChildItemCount)
                                {
                                    result = Structure.MessageError.InvalidMessageStructure;

                                    errorText = string.Format("Index={0}, Item Name={1}, Item Length={2}, Child Item Count={3}", index, items[currentIndex].Name, items[currentIndex].Length, subChildItemCount);

                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (secsItem.Format != Structure.SECSItemFormat.A && secsItem.Format != Structure.SECSItemFormat.J)
                            {
                                if (secsItem.Value != null)
                                {
                                    if (secsItem.Value.GetValue() is IList ilist)
                                    {
                                        if (secsItem.Length < ilist.Count)
                                        {
                                        }
                                        else if (secsItem.Length > ilist.Count)
                                        {
                                            secsItem.Length = ilist.Count;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result = Structure.MessageError.InvalidMessageStructure;

                        errorText = string.Format("Total Item Count={0}, Item Index={1}", items.Count, index);

                        break;
                    }
                }
            }
            else
            {
                result = Structure.MessageError.InvalidMessageStructure;

                errorText = string.Format("Total Item Count={0}, Item Index={1}", items.Count, index);
            }

            return result;
        }
    }
}