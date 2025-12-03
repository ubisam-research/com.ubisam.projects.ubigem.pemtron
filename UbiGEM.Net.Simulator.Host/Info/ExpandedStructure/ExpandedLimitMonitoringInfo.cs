using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    #region ExpandedLimitMonitoringItem
    public class ExpandedLimitMonitoringItem
    {
        #region Properties
        public byte LimitID { get; set; }
        public string Upper { get; set; }
        public string Lower { get; set; }
        #endregion
        #region Constructor
        public ExpandedLimitMonitoringItem()
        {
        }
        #endregion
        #region Clone
        public ExpandedLimitMonitoringItem Clone()
        {
            ExpandedLimitMonitoringItem result;

            result = new ExpandedLimitMonitoringItem();

            result.LimitID = this.LimitID;
            result.Upper = this.Upper;
            result.Lower = this.Lower;

            return result;
        }
        #endregion
    }
    #endregion
    #region ExpandedLimitMonitoringInfo
    public class ExpandedLimitMonitoringInfo : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region MemberVariable
        private bool _autoSend;
        #endregion
        #region Properties
        public ExpandedVariableInfo Variable { get; set; }
        public AutoSendTriggerCollection TriggerCollection { get; set; }
        public bool AutoSend
        {
            get
            {
                return this._autoSend;
            }
            set
            {
                this._autoSend = value;

                NotifyPropertyChanged("AutoSend");
            }
        }

        public List<ExpandedLimitMonitoringItem> Items { get; set; }
        #endregion
        #region Constructor
        public ExpandedLimitMonitoringInfo()
        {
            this._autoSend = false;
            this.Items = new List<ExpandedLimitMonitoringItem>();
            this.TriggerCollection = new AutoSendTriggerCollection();
        }
        #endregion
        #region Clone
        public ExpandedLimitMonitoringInfo Clone()
        {
            ExpandedLimitMonitoringInfo result;

            result = new ExpandedLimitMonitoringInfo();

            if (this.Variable != null)
            {
                result.Variable = this.Variable.Clone();
            }

            result.AutoSend = this.AutoSend;

            if (this.Items != null)
            {
                foreach(var item in this.Items)
                {
                    result.Items.Add(item.Clone());
                }
            }

            if (this.TriggerCollection != null)
            {
                result.TriggerCollection = this.TriggerCollection.Clone();
            }

            return result;
        }
        #endregion
        #region Add
        public void Add(ExpandedLimitMonitoringItem limitMonitoringItem)
        {
            this.Items.Add(limitMonitoringItem);
        }
        #endregion
        #region Remove
        public void Remove(ExpandedLimitMonitoringItem limitMonitoringItem)
        {
            var varInfo = (from ExpandedLimitMonitoringItem tempLimitMonitoringItem in this.Items
                           where tempLimitMonitoringItem.LimitID == limitMonitoringItem.LimitID
                           select tempLimitMonitoringItem).FirstOrDefault();

            if (varInfo != null)
            {
                this.Items.Remove(varInfo);
            }
        }
        #endregion
        #region NotifyPropertyChanged
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
    #endregion
    #region ExpandedLimitMonitoringCollection
    public class ExpandedLimitMonitoringCollection
    {
        #region Properties
        public List<ExpandedLimitMonitoringInfo> Items { get; set; }

        public ExpandedLimitMonitoringInfo this[string vid]
        {
            get { return this.Items.FirstOrDefault(t => t.Variable.VID == vid); }
        }
        #endregion
        #region Constructor
        public ExpandedLimitMonitoringCollection()
        {
            this.Items = new List<ExpandedLimitMonitoringInfo>();
        }
        #endregion
        #region Clone
        public ExpandedLimitMonitoringCollection Clone()
        {
            ExpandedLimitMonitoringCollection result;

            result = new ExpandedLimitMonitoringCollection();

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
        public void Add(ExpandedLimitMonitoringInfo limitMonitoringInfo)
        {
            this.Items.Add(limitMonitoringInfo);
        }
        #endregion
        #region Remove
        public void Remove(ExpandedLimitMonitoringInfo limitMonitoringInfo)
        {
            var varInfo = (from ExpandedLimitMonitoringInfo tempLimitMonitoringInfo in this.Items
                           where tempLimitMonitoringInfo.Variable == limitMonitoringInfo.Variable
                           select tempLimitMonitoringInfo).FirstOrDefault();

            if (varInfo != null)
            {
                this.Items.Remove(varInfo);
            }
        }
        #endregion
    }
    #endregion
}