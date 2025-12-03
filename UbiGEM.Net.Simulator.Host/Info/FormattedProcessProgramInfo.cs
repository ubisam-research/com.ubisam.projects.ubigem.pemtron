using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiGEM.Net.Structure;

namespace UbiGEM.Net.Simulator.Host.Info
{
    #region FormattedProcessProgramInfo
    public class FormattedProcessProgramInfo : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Property
        public bool IsLoaded { set; get; }
        public string PPID
        {
            get
            {
                return this.FmtPPCollection.PPID;
            }
            set
            {
                if (this.FmtPPCollection.PPID != value)
                {
                    this.FmtPPCollection.PPID = value;
                    NotifyPropertyChange("PPID");
                }
            }
        }
        public bool AutoSend { get; set; }
        public AutoSendTriggerCollection TriggerCollection { get; set; }
        public string MDLN
        {
            get
            {
                return this.FmtPPCollection.MDLN;
            }
            set
            {
                if (this.FmtPPCollection.MDLN != value)
                {
                    this.FmtPPCollection.MDLN = value;
                    NotifyPropertyChange("MDLN");
                }
            }
        }
        public string SOFTREV
        {
            get
            {
                return this.FmtPPCollection.SOFTREV;
            }
            set
            {
                if (this.FmtPPCollection.SOFTREV != value)
                {
                    this.FmtPPCollection.SOFTREV = value;
                    NotifyPropertyChange("SOFTREV");
                }
            }
        }
        public FmtPPCollection FmtPPCollection { get; private set; }
        #endregion
        #region Constructor
        public FormattedProcessProgramInfo()
        {            
            this.FmtPPCollection = new FmtPPCollection(string.Empty);
            this.FmtPPCollection.MDLN = string.Empty;
            this.FmtPPCollection.SOFTREV = string.Empty;
            this.IsLoaded = false;
            this.AutoSend = false;
            this.TriggerCollection = new AutoSendTriggerCollection();
        }
        #endregion
        #region Clone
        public FormattedProcessProgramInfo Clone()
        {
            FormattedProcessProgramInfo result;
            FmtPPCCodeInfo ccodeInfo;

            result = new FormattedProcessProgramInfo();
            result.FmtPPCollection.PPID = this.FmtPPCollection.PPID;
            result.FmtPPCollection.MDLN = this.FmtPPCollection.MDLN;
            result.FmtPPCollection.SOFTREV = this.FmtPPCollection.SOFTREV;
            result.IsLoaded = this.IsLoaded;

            if (this.FmtPPCollection.Items != null)
            {
                foreach (var ccode in this.FmtPPCollection.Items)
                {
                    ccodeInfo = new FmtPPCCodeInfo();

                    ccodeInfo.CommandCode = ccode.CommandCode;

                    foreach (var pparam in ccode.Items)
                    {
                        ccodeInfo.Items.Add(new FmtPPItem() {
                            PPName = pparam.PPName,
                            PPValue = pparam.PPValue,
                            Format = pparam.Format
                        });
                    }

                    result.FmtPPCollection.AddCode(ccodeInfo);
                }
            }

            if (this.TriggerCollection != null)
            {
                result.TriggerCollection = this.TriggerCollection.Clone();
            }

            return result;
        }
        #endregion
        #region NotifyPropertyChange
        private void NotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
    #endregion
    #region FormattedProcessProgramCollection
    public class FormattedProcessProgramCollection
    {
        #region Properties
        public List<FormattedProcessProgramInfo> Items { get; private set; }
        public FormattedProcessProgramInfo this[string ppid]
        {
            get
            {
                return Items.FirstOrDefault(t => t.PPID == ppid);
            }
        }
        #endregion
        #region Constructor
        public FormattedProcessProgramCollection()
        {
            this.Items = new List<FormattedProcessProgramInfo>();
        }
        #endregion
        #region Clone
        public FormattedProcessProgramCollection Clone()
        {
            FormattedProcessProgramCollection result;

            result = new FormattedProcessProgramCollection();

            if (this.Items != null)
            {
                foreach(var item in this.Items)
                {
                    result.Add(item.Clone());
                }
            }
            return result;
        }
        #endregion
        #region Add
        public void Add(FormattedProcessProgramInfo info)
        {
            this.Items.Add(info);
        }
        #endregion
        #region Clear
        public void Clear()
        {
            this.Items.Clear();
        }
        #endregion
    }
    #endregion
}
