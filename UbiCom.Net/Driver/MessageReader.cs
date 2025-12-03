using System;
using System.IO;
using System.Threading;

namespace UbiCom.Net.Driver
{
    internal class MessageReader : IDisposable
    {
        public delegate void ReceivedSECSMessageEventHandler(object sender, Structure.SECSMessage message);

        public event ReceivedSECSMessageEventHandler OnReceivedMessage;

        public event Utility.TimerManager.TimeoutEventHandler OnTimeout;

        private const int LENGTH_HEADER = 10;
        private const int INDEX_S_TYPE = 5;

        private const int THREAD_SLEEP_INTERVAL_PREPARE_SOCKET = 1000;
        private const int THREAD_SLEEP_INTERVAL_READ_STREAM = 100;
        private const int THREAD_SLEEP_INTERVAL_MESSAGE = 50;
        private const int THREAD_STOP_INTERVAL = 1000;
        private const string CLASS_NAME = "MessageReader";

        private HSMSDriver _driver;

        private readonly System.Collections.Queue _queReceived;
        private Thread _receiverThread;
        private Thread _queueThread;

        private Utility.TimerManager _timer;
        private Utility.Logger.Logger _logger;

        private bool _threadFlag;
        private BinaryReader _socketReader = null;
        private bool _disposed;


        public void SelfReciveBlockCom(byte[] receivedData)
        {
            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SelfRecive3", "Self Recive");

            RawData rawData = new RawData()
            {
                Header = new byte[LENGTH_HEADER],
                Body = new byte[receivedData.Length - LENGTH_HEADER],
                Length = receivedData.Length - LENGTH_HEADER
            };

            Array.Copy(receivedData, rawData.Header, LENGTH_HEADER);
            Array.Copy(receivedData, LENGTH_HEADER, rawData.Body, 0, rawData.Body.Length);

            Structure.SECSMessage message = this.MakeMessage(rawData);

            if (message == null)
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "SelfRecive3", "Message is null");
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "SelfRecive3",
                    string.Format("Control Message Type={0} ({1:X8})", message.ControlMessageType, message.SystemBytes));

                this.OnReceivedMessage?.Invoke(this, message);
            }
        }

        public MessageReader()
        {
            this._queReceived = new System.Collections.Queue();

            this._threadFlag = false;

            this._timer = null;
            this._disposed = false;
        }

        ~MessageReader()
        {
            Dispose(false);
        }

        void Timer_OnT8Timeout(object sender, Structure.TimeoutType timeoutType)
        {
            if (timeoutType == Structure.TimeoutType.T8 && this.OnTimeout != null)
                this.OnTimeout(this, timeoutType);
        }

        public void Initialize(HSMSDriver driver)
        {
            this._queReceived.Clear();

            this._driver = driver;

            if (this._timer != null)
            {
                this._timer.OnTimeout -= Timer_OnT8Timeout;
            }

            this._timer = driver._timerMgr;

            this._timer.OnTimeout += Timer_OnT8Timeout;

            this._socketReader = new System.IO.BinaryReader(driver._socket.GetStream());
            this._logger = driver._logger;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "Message Reader Initialize");

            this._threadFlag = true;

            this._receiverThread = new Thread(ProcRunByReceivedHSMS)
            {
                Name = "Message Reader"
            };

            this._receiverThread.Start();

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Initialize", "Message Reader Thread Start(HSMS)");

            this._queueThread = new Thread(ProcRunByMessageHSMS)
            {
                Name = "Data Message Queue"
            };

            this._queueThread.Start();
        }

        public void Terminate()
        {
            this._threadFlag = false;

            lock (this)
            {
                if (this._receiverThread != null)
                {
                    this._receiverThread.Join(new TimeSpan(THREAD_STOP_INTERVAL * 10));
                }
            }

            lock (this)
            {
                if (this._queueThread != null)
                {
                    this._queueThread.Join(new TimeSpan(THREAD_STOP_INTERVAL * 10));
                }
            }

            if (this._socketReader != null)
            {
                this._socketReader.Close();
                this._socketReader = null;
            }

            if (this._timer != null)
            {
                this._timer.Stop();
            }

            lock (this._queReceived)
            {
                this._queReceived.Clear();
            }

            if (this._logger != null)
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Terminate", "Reader Terminate");
            }
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

                    lock (this._queReceived)
                    {
                        this._queReceived.Clear();
                    }

                    if (this._timer != null)
                    {
                        this._timer.Stop();
                        this._timer = null;
                    }

                    if (this._socketReader != null)
                    {
                        this._socketReader.Close();
                        this._socketReader.Dispose();
                        this._socketReader = null;
                    }

                    if (this._logger != null)
                        this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Dispose", "Message Reader Dispose");
                }

                this._disposed = true;
            }
        }

        private void ProcRunByReceivedHSMS()
        {
            Structure.SECSMessage message;
            int totalLength;
            byte[] receivedData;
            RawData rawData;

            while (this._threadFlag)
            {
                try
                {
                    if (this._socketReader != null)
                    {
                        totalLength = System.Net.IPAddress.NetworkToHostOrder(this._socketReader.ReadInt32());

                        this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Receive, CLASS_NAME, "ProcRunByReceivedHSMS", string.Format("Read Length={0}", totalLength));

                        receivedData = ReadStream(totalLength);

                        rawData = new RawData()
                        {
                            Header = new byte[LENGTH_HEADER],
                            Body = new byte[receivedData.Length - LENGTH_HEADER],
                            Length = receivedData.Length - LENGTH_HEADER
                        };

                        Array.Copy(receivedData, rawData.Header, LENGTH_HEADER);
                        Array.Copy(receivedData, LENGTH_HEADER, rawData.Body, 0, rawData.Body.Length);

                        this._logger.WriteSECS1(DateTime.Now, Utility.Logger.LogLevel.Receive, receivedData);

                        if (rawData.Header[INDEX_S_TYPE] == Structure.ControlMessageType.DataMessage.GetHashCode())
                        {
                            lock (this._queReceived)
                            {
                                this._queReceived.Enqueue(rawData);
                            }
                        }
                        else
                        {
                            message = MakeMessage(rawData);

                            if (message == null)
                            {
                                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "ProcRunByReceivedHSMS", "Message is null");
                            }
                            else
                            {
                                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRunByReceivedHSMS",
                                    string.Format("Control Message Type={0} ({1:X8})", message.ControlMessageType, message.SystemBytes));

                                this.OnReceivedMessage?.Invoke(this, message);
                            }

                        }

                        receivedData = null;
                        rawData = null;
                    }
                    else
                    {
                        this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "ProcRunByReceivedHSMS", "Binary Reader is null");

                        System.Threading.Thread.Sleep(THREAD_SLEEP_INTERVAL_PREPARE_SOCKET);
                    }
                }
                catch (Exception ex)
                {
                    this._logger.WriteException(DateTime.Now, CLASS_NAME, "ProcRunByReceivedHSMS", ex);

                    this._threadFlag = false;

                    if (this._driver != null)
                    {
                        this._driver.SetDisconnected(true);
                    }

                    System.Diagnostics.Debug.Print(ex.Message);
                }

                continue;
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRunByReceivedHSMS", "Reader Thread Exit");
        }

        private void ProcRunByMessageHSMS()
        {
            RawData rawData;
            Structure.SECSMessage message;

            while (this._threadFlag)
            {
                try
                {
                    try
                    {
                        if (this._queReceived.Count > 0)
                        {
                            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRunByMessageHSMS",
                                string.Format("Receive Queue Count={0}", this._queReceived.Count));

                            lock (this._queReceived)
                            {
                                rawData = this._queReceived.Dequeue() as RawData;
                            }

                            if (rawData != null)
                            {
                                message = MakeMessage(rawData);

                                if (message != null)
                                {
                                    if (this._driver._config.LogEnabledSECS2 != Structure.LogMode.None)
                                    {
                                        this._logger.WriteSECS2(DateTime.Now, Utility.Logger.LogLevel.Receive, message);
                                    }

                                    this.OnReceivedMessage?.Invoke(this, message);
                                }
                                else
                                {
                                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Warning, CLASS_NAME, "ProcRunByMessageHSMS", "Message is null");
                                }
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        this._logger.WriteException(DateTime.Now, CLASS_NAME, "ProcRunByMessageHSMS", ex);
                    }
                    catch (Exception ex)
                    {
                        this._logger.WriteException(DateTime.Now, CLASS_NAME, "ProcRunByMessageHSMS", ex);

                        this._threadFlag = false;

                        if (this._driver != null)
                        {
                            this._driver.SetDisconnected(true);
                        }
                    }

                    continue;
                }
                finally
                {
                    Thread.Sleep(THREAD_SLEEP_INTERVAL_MESSAGE);
                }
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ProcRunByMessageHSMS", "Message Queue Thread Exit");
        }

        private byte[] ReadStream(int totalLength)
        {
            byte[] result;
            int index;
            int readCount;

            result = new byte[totalLength];

            index = 0;

            this._timer.Start(Structure.TimeoutType.T8);

            while (index < totalLength)
            {
                readCount = this._socketReader.Read(result, index, totalLength - index);

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ReadStream",
                    string.Format("Total Length={0} Index={1} Read Length={2}", totalLength, index, readCount));

                if (readCount == 0)
                {
                    System.Threading.Thread.Sleep(THREAD_SLEEP_INTERVAL_READ_STREAM);
                }

                index += readCount;

                this._timer.Restart(Structure.TimeoutType.T8);
            }

            this._timer.Stop(Structure.TimeoutType.T8);

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ReadStream",
                string.Format("Total Length={0} Total Read Length={1}", totalLength, result.Length));

            return result;
        }

        private Structure.SECSMessage MakeMessage(RawData rawData)
        {
            Structure.SECSMessage result;
            Structure.MessageError resultCode;

            result = new Structure.SECSMessage();

            using (MessageDecoder decoder = new MessageDecoder(this._driver, rawData))
            {
                resultCode = decoder.GetDecodingData(result);
            }

            if (resultCode != Structure.MessageError.Ok)
            {
                result = null;

                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeMessage",
                    string.Format("Message Make Result={0} (Failed)", result));
            }

            return result;
        }
    }
}