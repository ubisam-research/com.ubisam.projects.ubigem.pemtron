using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedVariableInfo : Structure.VariableInfo, INotifyPropertyChanged
    {
        #region MemberVariable
        private List<ExpandedVariableInfo> _childVariables;
        private string _value;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Property
        public new int Length
        {
            set
            {
                if (base.Length != value)
                {
                    base.Length = value;
                    NotifyPropertyChanged("Length");
                }
            }
            get
            {
                int result;

                if (this.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    result = this.ChildVariables.Count;
                }
                else
                {
                    result = base.Length;
                }

                return result;
            }
        }
        public new string Value
        {
            get
            {
                string result;

                if (this._value == null)
                {
                    result = string.Empty;
                }
                else
                {
                    result = this._value.ToString();
                }

                return result;
            }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public string ReceivedValue;

        public bool IsInheritance { get; set; }

        public new List<ExpandedVariableInfo> ChildVariables
        {
            get
            {
                return this._childVariables;
            }
        }
        public string DisplayStringForComboBox
        {
            get
            {
                string result;

                if (this.VID == this.Name)
                {
                    result = string.Format("{0}({1}) {2}", this.Format, this.Length, this.Name);
                }
                else
                {
                    result = string.Format("{0}({1}) {2}: {3}", this.Format, this.Length, this.VID, this.Name);
                }

                return result;
            }
        }
        #endregion
        #region Constructor
        public ExpandedVariableInfo(ExpandedVariableInfo parent = null)
        {
            this._value = string.Empty;
            this.IsInheritance = false;
            this.PreDefined = false;
            this._childVariables = new List<ExpandedVariableInfo>();
        }
        #endregion

        // Public Method
        public ExpandedVariableInfo Clone()
        {
            ExpandedVariableInfo result;

            result = new ExpandedVariableInfo();

            result.VID = this.VID;
            result.VIDType = this.VIDType;
            result.IsInheritance = this.IsInheritance;
            result.Description = this.Description;
            result.Length = this.Length;
            result.Name = this.Name;
            result.Format = this.Format;
            result.Value = this.Value;

            if (this._childVariables != null)
            {
                foreach (var child in this._childVariables)
                {
                    result.ChildVariables.Add(child.Clone());
                }
            }

            return result;
        }
        #region Equals
        public override bool Equals(object obj)
        {
            bool result;
            ExpandedVariableInfo operand;

            if (obj == null || !(obj is ExpandedVariableInfo))
            {
                result = false;
            }
            else
            {
                operand = obj as ExpandedVariableInfo;
                if (this.VID == this.Name)
                {
                    result = this.Name == operand.Name;
                }
                else
                {
                    result = this.VID == operand.VID;

                }
            }

            return result;
        }
        #endregion
        #region GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
        // Private Method
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
}