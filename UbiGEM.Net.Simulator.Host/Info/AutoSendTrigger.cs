using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info
{
    #region Enum
    public enum TriggerMode
    {
        NotUse,
        CollectionEvent,
        Variable
    }
    #endregion
    #region AutoSendTrigger
    public class AutoSendTrigger : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region MemberVariable
        private TriggerMode _triggerMode;
        private ExpandedStructure.ExpandedCollectionEventInfo _collectionEventInfo;
        private ExpandedStructure.ExpandedReportInfo _reportInfo;
        private string _variableValue;
        #endregion
        #region Property
        public int TriggerID { get; set; }
        public TriggerMode TriggerMode
        {
            get
            {
                return this._triggerMode;
            }
            set
            {
                if(this._triggerMode != value)
                {
                    this._triggerMode = value;
                    NotifyPropertyChanged("TriggerMode");
                }
            }
        }
        public ExpandedStructure.ExpandedCollectionEventInfo CollectionEventInfo
        {
            get
            {
                return this._collectionEventInfo;
            }
            set
            {
                if (this._collectionEventInfo != value)
                {
                    this._collectionEventInfo = value;
                    NotifyPropertyChanged("CollectionEventInfo");
                }
            }
        }
        public ExpandedStructure.ExpandedReportInfo ReportInfo
        {
            get
            {
                return this._reportInfo;
            }
            set
            {
                if (this._reportInfo != value)
                {
                    this._reportInfo = value;
                    NotifyPropertyChanged("ReportInfo");
                }
            }
        }
        public string VariableValue
        {
            get
            {
                return this._variableValue;
            }
            set
            {
                if (this._variableValue != value)
                {
                    this._variableValue = value;
                    NotifyPropertyChanged("VariableValue");
                }
            }
        }

        public ExpandedStructure.ExpandedVariableInfo TopVariable
        {
            get
            {
                ExpandedStructure.ExpandedVariableInfo result;

                if (this.VariableStack.Count == 0)
                {
                    result = null;
                }
                else
                {
                    result = this.VariableStack[0];
                }

                return result;
            }
            set
            {
                if (this.VariableStack.Count != 0)
                {
                    this.VariableStack.Clear();
                }

                this.VariableStack.Add(value);
                NotifyPropertyChanged("TopVariable");
                NotifyPropertyChanged("VariableValue");
            }
        }

        public List<ExpandedStructure.ExpandedVariableInfo> VariableStack { set; get; }
        public List<ExpandedStructure.ExpandedReportInfo> CandidateReports
        {
            get
            {
                List<ExpandedStructure.ExpandedReportInfo> result = new List<ExpandedStructure.ExpandedReportInfo>();

                if (this._collectionEventInfo != null)
                {
                    foreach (var reportInfo in this._collectionEventInfo.Reports.Items.Values)
                    {
                        result.Add(reportInfo as ExpandedStructure.ExpandedReportInfo);
                    }
                }

                return result;
            }
        }
        public List<ExpandedStructure.ExpandedVariableInfo> CandidateVariables
        {
            get
            {
                List<ExpandedStructure.ExpandedVariableInfo> result = new List<ExpandedStructure.ExpandedVariableInfo>();

                if (this._reportInfo != null)
                {
                    foreach (var variableInfo in this._reportInfo.Variables)
                    {
                        result.Add(variableInfo as ExpandedStructure.ExpandedVariableInfo);
                    }
                }

                return result;
            }
        }
        #endregion
        #region Constructor
        public AutoSendTrigger()
        {
            this.TriggerID = -1;
            this._triggerMode = TriggerMode.NotUse;
            this._collectionEventInfo = null;
            this._reportInfo = null;
            this._variableValue = string.Empty;
            this.VariableStack = new List<ExpandedStructure.ExpandedVariableInfo>();
        }
        #endregion
        #region Clone
        public AutoSendTrigger Clone()
        {
            AutoSendTrigger result;

            result = new AutoSendTrigger();
            result.TriggerID = this.TriggerID;
            result._triggerMode = this.TriggerMode;
            result._collectionEventInfo = this.CollectionEventInfo;
            result._reportInfo = this.ReportInfo;
            result._variableValue = this.VariableValue;

            foreach (ExpandedStructure.ExpandedVariableInfo child in this.VariableStack)
            {
                result.VariableStack.Add(child.Clone());
            }

            return result;
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
        #region ToString
        public new string ToString()
        {
            return string.Format("[TriggerID={0},TriggerMode={1}]", this.TriggerID, this.TriggerMode);
        }
        #endregion
        #region ValidateTrigger
        public bool ValidateTrigger(out string errorText)
        {
            bool result;

            result = true;
            errorText = string.Empty;

            switch (this.TriggerMode)
            {
                case TriggerMode.NotUse:
                    break;
                case TriggerMode.CollectionEvent:
                    if (this.CollectionEventInfo == null)
                    {
                        result = false;
                        errorText = "CollectionEvent is not selected";
                    }
                    break;
                default:
                    if (this.CollectionEventInfo == null)
                    {
                        result = false;
                        errorText = "CollectionEvent is not selected";
                    }
                    else
                    {
                        if (this.ReportInfo == null)
                        {
                            result = false;
                            errorText = "Report is not selected";
                        }
                        else
                        {
                            if (this.VariableStack.Count == 0)
                            {
                                result = false;
                                errorText = "Variable is not selected";
                            }
                            else if(this.VariableStack.Last().Format == UbiCom.Net.Structure.SECSItemFormat.L)
                            {
                                result = false;
                                errorText = "List Format is not supported";
                            }
                        }
                    }
                    break;
            }

            return result;
        }
        #endregion
    }
    #endregion
    #region AutoSendTriggerCollection
    public class AutoSendTriggerCollection
    {
        #region Properties
        public List<AutoSendTrigger> Items { get; set; }
        public AutoSendTrigger this[int triggerID]
        {
            get
            {
                return Items.FirstOrDefault(t => t.TriggerID == triggerID);
            }
        }
        #endregion
        #region Constructor
        public AutoSendTriggerCollection()
        {
            this.Items = new List<AutoSendTrigger>();
        }
        // 구버전 지원용 TriggerCEID 로 생성
        public AutoSendTriggerCollection(ExpandedStructure.ExpandedCollectionEventInfo ceInfo)
        {
            this.Items = new List<AutoSendTrigger>();
            if (ceInfo != null)
            {
                this.Items.Add(new AutoSendTrigger()
                {
                    TriggerID = 1,
                    TriggerMode = TriggerMode.CollectionEvent,
                    CollectionEventInfo = ceInfo
                });
            }
        }
        #endregion
        #region Clone
        public AutoSendTriggerCollection Clone()
        {
            AutoSendTriggerCollection result;

            result = new AutoSendTriggerCollection();

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
        public void Add(AutoSendTrigger triggerInfo)
        {
            this.Items.Add(triggerInfo);
        }
        #endregion
        #region Remove
        public void Remove(AutoSendTrigger triggerInfo)
        {
            if (triggerInfo != null)
            {
                var varInfo = (from AutoSendTrigger tempTriggerInfo in this.Items
                               where tempTriggerInfo.TriggerID == triggerInfo.TriggerID
                               select tempTriggerInfo).FirstOrDefault();

                if (varInfo != null)
                {
                    this.Items.Remove(varInfo);
                }
            }
        }
        #endregion
        #region ValidateTriggers
        public bool ValidateTriggers(out string errorText, out AutoSendTrigger invalidTrigger)
        {
            bool result;

            result = true;
            errorText = string.Empty;
            invalidTrigger = null;

            foreach (AutoSendTrigger trigger in this.Items)
            {
                if (result == true)
                {
                    if (trigger.ValidateTrigger(out errorText) == false)
                    {
                        result = false;
                        invalidTrigger = trigger;
                    }
                }
            }

            return result;
        }
        #endregion
    }
    #endregion
}
