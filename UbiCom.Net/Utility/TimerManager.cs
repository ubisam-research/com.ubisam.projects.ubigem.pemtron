using System;
using System.Collections.Concurrent;

namespace UbiCom.Net.Utility
{
    internal class TimerManager : IDisposable
    {
        #region [T3TimerObject - Class]
        internal class T3TimerObject : System.Timers.Timer
        {
            public Structure.SECSMessage Message { get; set; }

            public T3TimerObject()
            {
                this.Message = null;
            }

            public T3TimerObject(Structure.SECSMessage message)
            {
                this.Message = message;
            }
        }
        #endregion

        internal delegate void TimeoutEventHandler(object sender, Structure.TimeoutType timeoutType);
        internal delegate void T3TimeoutEventHandler(object sender, Structure.SECSMessage message);

        internal event TimeoutEventHandler OnTimeout;
        internal event T3TimeoutEventHandler OnT3Timeout;

        private Structure.Configurtion _config;
        private ConcurrentDictionary<uint, T3TimerObject> _t3Timer;

        private System.Timers.Timer _t1;
        private System.Timers.Timer _t2;
        private System.Timers.Timer _t4;
        private System.Timers.Timer _t5;
        private System.Timers.Timer _t6;
        private System.Timers.Timer _t7;
        private System.Timers.Timer _t8;
        private System.Timers.Timer _linkTest;

        private int _t3Interval;
        private bool _disposed;

        public TimerManager()
        {
            this._config = new Structure.Configurtion();

            this._t3Timer = null;
            this._t1 = null;
            this._t2 = null;
            this._t4 = null;
            this._t5 = null;
            this._t6 = null;
            this._t7 = null;
            this._t8 = null;
            this._linkTest = null;

            this._disposed = false;
        }

        ~TimerManager()
        {
            Dispose(false);
        }

        void T3_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            T3TimerObject timer;

            try
            {
                timer = sender as T3TimerObject;

                if (timer != null)
                {
                    if (this._config.SECSMode == Structure.SECSMode.HSMS)
                    {
                        this.OnT3Timeout?.Invoke(this, timer.Message);
                    }

                    timer.Enabled = false;
                    timer.Elapsed -= new System.Timers.ElapsedEventHandler(T3_Elapsed);

                    if (this._config.SECSMode == Structure.SECSMode.HSMS)
                    {
                        _ = this._t3Timer.TryRemove(timer.Message.SystemBytes, out T3TimerObject temp);
                    }

                    timer.Dispose();
                    timer = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        void T5_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stop(Structure.TimeoutType.T5);

            this.OnTimeout?.Invoke(this, Structure.TimeoutType.T5);
        }

        void T6_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stop(Structure.TimeoutType.T6);

            this.OnTimeout?.Invoke(this, Structure.TimeoutType.T6);
        }

        void T7_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stop(Structure.TimeoutType.T7);

            this.OnTimeout?.Invoke(this, Structure.TimeoutType.T7);
        }

        void T8_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stop(Structure.TimeoutType.T8);

            this.OnTimeout?.Invoke(this, Structure.TimeoutType.T8);
        }

        void LinkTest_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.OnTimeout?.Invoke(this, Structure.TimeoutType.Linktest);

            Restart(Structure.TimeoutType.Linktest);
        }

        public void Initialize(Structure.Configurtion configuration)
        {
            TimerTerminate();

            this._config = configuration;
            this._t3Timer = new ConcurrentDictionary<uint, T3TimerObject>();

            this._t1 = new System.Timers.Timer();
            this._t2 = new System.Timers.Timer();
            this._t4 = new System.Timers.Timer();
            this._t5 = new System.Timers.Timer();
            this._t6 = new System.Timers.Timer();
            this._t7 = new System.Timers.Timer();
            this._t8 = new System.Timers.Timer();
            this._linkTest = new System.Timers.Timer();

            this._t5.Elapsed += T5_Elapsed;
            this._t6.Elapsed += T6_Elapsed;
            this._t7.Elapsed += T7_Elapsed;
            this._t8.Elapsed += T8_Elapsed;
            this._linkTest.Elapsed += LinkTest_Elapsed;

            if (this._config.SECSMode == Structure.SECSMode.HSMS)
            {
                this._t5.Interval = this._config.HSMSModeConfig.T5 * 1000;
                this._t6.Interval = this._config.HSMSModeConfig.T6 * 1000;
                this._t7.Interval = this._config.HSMSModeConfig.T7 * 1000;
                this._t8.Interval = this._config.HSMSModeConfig.T8 * 1000;
                this._linkTest.Interval = this._config.HSMSModeConfig.LinkTest * 1000;

                this._t3Interval = this._config.HSMSModeConfig.T3 * 1000;
            }
            else
            {
                //this._t1 = new System.Timers.Timer();
                //this._t2 = new System.Timers.Timer();
                //this._t4 = new System.Timers.Timer();

                //this._t3Interval = this._config.SECS1ModeConfig.T3 * 1000;
            }
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
                    TimerTerminate();
                }

                this._disposed = true;
            }
        }

        public void Start(Structure.TimeoutType timeoutType)
        {
            try
            {
                switch (timeoutType)
                {
                    case Structure.TimeoutType.T1:
                        this._t1.Enabled = true;
                        break;
                    case Structure.TimeoutType.T2:
                        this._t2.Enabled = true;
                        break;
                    case Structure.TimeoutType.T4:
                        this._t4.Enabled = true;
                        break;
                    case Structure.TimeoutType.T5:
                        this._t5.Enabled = true;
                        break;
                    case Structure.TimeoutType.T6:
                        this._t6.Enabled = true;
                        break;
                    case Structure.TimeoutType.T7:
                        this._t7.Enabled = true;
                        break;
                    case Structure.TimeoutType.T8:
                        this._t8.Enabled = true;
                        break;
                    case Structure.TimeoutType.Linktest:
                        this._linkTest.Enabled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        public void Restart(Structure.TimeoutType timeoutType)
        {
            Stop(timeoutType);
            Start(timeoutType);
        }

        public void Stop()
        {
            if (this._t1 != null)
            {
                this._t1.Enabled = false;
            }

            if (this._t2 != null)
            {
                this._t2.Enabled = false;
            }

            if (this._t4 != null)
            {
                this._t4.Enabled = false;
            }

            if (this._t5 != null)
            {
                this._t5.Enabled = false;
            }

            if (this._t6 != null)
            {
                this._t6.Enabled = false;
            }

            if (this._t7 != null)
            {
                this._t7.Enabled = false;
            }

            if (this._t8 != null)
            {
                this._t8.Enabled = false;
            }

            if (this._linkTest != null)
            {
                this._linkTest.Enabled = false;
            }

            if (this._t3Timer != null)
            {
                foreach (T3TimerObject temp in this._t3Timer.Values)
                {
                    temp.Elapsed -= new System.Timers.ElapsedEventHandler(T3_Elapsed);
                    temp.Enabled = false;
                }

                this._t3Timer.Clear();
            }
        }

        public void Stop(Structure.TimeoutType timeoutType)
        {
            try
            {
                switch (timeoutType)
                {
                    case Structure.TimeoutType.T1:
                        this._t1.Enabled = false;
                        break;
                    case Structure.TimeoutType.T2:
                        this._t2.Enabled = false;
                        break;
                    case Structure.TimeoutType.T4:
                        this._t4.Enabled = false;
                        break;
                    case Structure.TimeoutType.T5:
                        this._t5.Enabled = false;
                        break;
                    case Structure.TimeoutType.T6:
                        this._t6.Enabled = false;
                        break;
                    case Structure.TimeoutType.T7:
                        this._t7.Enabled = false;
                        break;
                    case Structure.TimeoutType.T8:
                        this._t8.Enabled = false;
                        break;
                    case Structure.TimeoutType.Linktest:
                        this._linkTest.Enabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        public Structure.MessageError StartT3(Structure.SECSMessage message)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            T3TimerObject timer;

            if (this._t3Timer.ContainsKey(message.SystemBytes) == true)
            {
                result = Structure.MessageError.DuplicateSystemBytes;
            }
            else
            {
                timer = new T3TimerObject(message)
                {
                    Interval = this._t3Interval
                };

                timer.Elapsed += T3_Elapsed;

                timer.Enabled = true;

                this._t3Timer[message.SystemBytes] = timer;
            }

            return result;
        }

        public void StopT3(Structure.SECSMessage message)
        {
            if (this._t3Timer.ContainsKey(message.SystemBytes) == true)
            {
                this._t3Timer[message.SystemBytes].Elapsed -= new System.Timers.ElapsedEventHandler(T3_Elapsed);
                this._t3Timer[message.SystemBytes].Enabled = false;

                this._t3Timer.TryRemove(message.SystemBytes, out T3TimerObject timer);
            }
        }

        private void TimerTerminate()
        {
            if (this._t3Timer != null)
            {
                foreach (T3TimerObject temp in this._t3Timer.Values)
                {
                    temp.Elapsed -= new System.Timers.ElapsedEventHandler(T3_Elapsed);
                    temp.Enabled = false;
                }
            }

            this._config = null;
            this._t3Timer = null;

            if (this._t1 != null)
            {
                this._t1.Enabled = false;
            }

            if (this._t2 != null)
            {
                this._t2.Enabled = false;
            }

            if (this._t4 != null)
            {
                this._t4.Enabled = false;
            }

            if (this._t5 != null)
            {
                this._t5.Elapsed -= T5_Elapsed;
                this._t5.Enabled = false;
            }

            if (this._t6 != null)
            {
                this._t6.Elapsed -= T6_Elapsed;
                this._t6.Enabled = false;
            }

            if (this._t7 != null)
            {
                this._t7.Elapsed -= T7_Elapsed;
                this._t7.Enabled = false;
            }

            if (this._t8 != null)
            {
                this._t8.Elapsed -= T8_Elapsed;
                this._t8.Enabled = false;
            }

            if (this._linkTest != null)
            {
                this._linkTest.Elapsed -= LinkTest_Elapsed;
                this._linkTest.Enabled = false;
            }
        }
    }
}