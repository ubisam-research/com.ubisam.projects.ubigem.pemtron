using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Tool
{
    internal class TimerManager : IDisposable
    {
        #region [TraceTimerObject - Class]
        internal class TraceTimerObject : System.Timers.Timer
        {
            public long Cycle { get; set; }

            public Structure.TraceInfo TraceInfo { get; set; }

            public TraceTimerObject(long cycle, Structure.TraceInfo traceInfo)
            {
                this.Cycle = cycle;
                this.TraceInfo = traceInfo;
            }
        }
        #endregion

        internal delegate void ReportTraceDataEventHandler(Structure.TraceInfo traceInfo);

        internal event ReportTraceDataEventHandler OnReportTraceData;

        private ConcurrentDictionary<string, TraceTimerObject> _traceTimer;

        public TimerManager()
        {
            this._traceTimer = new ConcurrentDictionary<string, TraceTimerObject>();
        }

        void Trace_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TraceTimerObject timer;

            try
            {
                timer = sender as TraceTimerObject;

                if (timer != null)
                {
                    this.OnReportTraceData?.Invoke(timer.TraceInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                if (this._traceTimer != null)
                {
                    foreach (TraceTimerObject temp in this._traceTimer.Values)
                    {
                        temp.Elapsed -= new System.Timers.ElapsedEventHandler(Trace_Elapsed);
                        temp.Enabled = false;
                    }
                }

                this._traceTimer = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        public void StartTrace(Structure.TraceInfo traceInfo)
        {
            TraceTimerObject traceTimerObject;

            traceTimerObject = new TraceTimerObject(traceInfo.TotalMillisecond, traceInfo)
            {
                Interval = traceInfo.TotalMillisecond
            };

            traceTimerObject.Elapsed += new System.Timers.ElapsedEventHandler(Trace_Elapsed);

            traceTimerObject.Enabled = true;

            this._traceTimer[traceInfo.TraceID] = traceTimerObject;
        }

        public void StopTrace()
        {
            foreach (TraceTimerObject tempTimer in this._traceTimer.Values)
            {
                tempTimer.Elapsed -= new System.Timers.ElapsedEventHandler(Trace_Elapsed);
                tempTimer.Enabled = false;
            }

            this._traceTimer.Clear();
        }

        public void StopTrace(Structure.TraceInfo traceInfo)
        {
            if (this._traceTimer.ContainsKey(traceInfo.TraceID) == true)
            {
                this._traceTimer[traceInfo.TraceID].Elapsed -= new System.Timers.ElapsedEventHandler(Trace_Elapsed);
                this._traceTimer[traceInfo.TraceID].Enabled = false;

                this._traceTimer.TryRemove(traceInfo.TraceID, out _);
            }
        }
    }
}