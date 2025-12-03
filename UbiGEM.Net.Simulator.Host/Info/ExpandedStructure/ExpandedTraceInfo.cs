using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    #region TraceInfo
    public class ExpandedTraceInfo
    {
        #region Properties
        public string TraceID { get; set; }
        public bool AutoSend { get; set; }
        public bool AutoStop { get; set; }
        public string Dsper { get; set; }
        public long TotalSample { get; set; }
        public long ReportGroupSize { get; set; }
        public List<string> Variables { get; set; }
        public AutoSendTriggerCollection SendTriggerCollection { get; set; }
        public AutoSendTriggerCollection StopTriggerCollection { get; set; }
        #endregion

        #region Constructor
        public ExpandedTraceInfo()
        {
            this.TraceID = string.Empty;
            this.AutoSend = false;
            this.AutoStop = false;
            this.Dsper = string.Empty;
            this.TotalSample = 0;
            this.ReportGroupSize = 0;
            this.Variables = new List<string>();
            this.SendTriggerCollection = new AutoSendTriggerCollection();
            this.StopTriggerCollection = new AutoSendTriggerCollection();
        }
        #endregion
        #region Clone
        public ExpandedTraceInfo Clone()
        {
            ExpandedTraceInfo result;

            result = new ExpandedTraceInfo();

            result.TraceID = this.TraceID;
            result.AutoSend = this.AutoSend;
            result.AutoStop = this.AutoStop;
            result.Dsper = this.Dsper;
            result.TotalSample = this.TotalSample;
            result.ReportGroupSize = this.ReportGroupSize;

            if (this.Variables != null)
            {
                foreach(var item in this.Variables)
                {
                    result.Variables.Add(item);
                }
            }

            if (this.SendTriggerCollection != null)
            {
                result.SendTriggerCollection = this.SendTriggerCollection.Clone();
            }
            if (this.StopTriggerCollection != null)
            {
                result.StopTriggerCollection = this.StopTriggerCollection.Clone();
            }

            return result;
        }
        #endregion
    }
    #endregion
    #region TraceCollection
    public class ExpandedTraceCollection
    {
        #region Properties
        public List<ExpandedTraceInfo> Items { get; set; }

        public ExpandedTraceInfo this[string traceID]
        {
            get { return this.Items.FirstOrDefault(t => t.TraceID == traceID); }
        }
        #endregion
        #region Constructor
        public ExpandedTraceCollection()
        {
            this.Items = new List<ExpandedTraceInfo>();
        }
        #endregion
        #region Clone
        public ExpandedTraceCollection Clone()
        {
            ExpandedTraceCollection result;

            result = new ExpandedTraceCollection();

            if (this.Items != null)
            {
                foreach (var item in this.Items)
                {
                    result.Items.Add(item.Clone());
                }
            }

            return result;
        }
        #endregion
        #region Add
        public void Add(ExpandedTraceInfo traceInfo)
        {
            this.Items.Add(traceInfo);
        }
        #endregion
        #region Remove
        public void Remove(ExpandedTraceInfo traceInfo)
        {
            if (traceInfo != null)
            {
                var varInfo = (from ExpandedTraceInfo tempTraceInfo in this.Items
                               where tempTraceInfo.TraceID == traceInfo.TraceID
                               select tempTraceInfo).FirstOrDefault();

                if (varInfo != null)
                {
                    this.Items.Remove(varInfo);
                }
            }
        }
        #endregion
    }
    #endregion
}