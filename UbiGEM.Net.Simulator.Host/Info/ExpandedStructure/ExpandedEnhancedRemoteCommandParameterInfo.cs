using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedEnhancedRemoteCommandParameterInfo : Structure.EnhancedCommandParameterInfo, INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Property
        /*
        public EnhancedParameterListType ListType
        {
            set;
            get;
        }
        */
        public int Count
        {
            get
            {
                int result;

                if (this.Format == SECSItemFormat.L)
                {
                    result = this.ValueItems.Count;
                }
                else if (this.Format == SECSItemFormat.A || this.Format == SECSItemFormat.J)
                {
                    if (base.Value == null)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = Encoding.Default.GetByteCount(base.Value.ToString());
                    }
                }
                else
                {
                    result = this._count;
                }

                return result;
            }
            set
            {
                if (this.Format != SECSItemFormat.L)
                {
                    if (this._count != value)
                    {
                        if (this.Format == SECSItemFormat.A)
                        {
                            this._count = 0;
                        }
                        else
                        {
                            this._count = value;
                        }

                        NotifyPropertyChange("Count");
                    }
                }
                else
                {
                    NotifyPropertyChange("Count");
                }
            }
        }
        public SECSValue BaseValue
        {
            get
            {
                if (base.Value == null)
                {
                    base.Value = new SECSValue();
                }
                return base.Value;
            }
        }

        public bool IsCountEditable
        {
            get
            {
                bool isReadonly;

                isReadonly = this.Format == SECSItemFormat.L || this.Format == SECSItemFormat.A || this.Format == SECSItemFormat.J || this.Format == SECSItemFormat.X || this.Format == SECSItemFormat.None;

                return isReadonly == false;
            }
        }
        public new SECSItemFormat Format
        {
            get
            {
                return base.Format;
            }
            set
            {
                if (base.Format != value)
                {
                    base.Format = value;
                    NotifyPropertyChange("Format");
                    NotifyPropertyChange("IsCountEditable");
                }
            }
        }

        public new string Value
        {
            get
            {
                string result;

                if (base.Value == null)
                {
                    result = string.Empty;
                }
                else
                {
                    if (this.Format == SECSItemFormat.L)
                    {
                        result = string.Empty;
                    }
                    else
                    {
                        result = base.Value.ToString();
                    }
                }

                return result;
            }
            set
            {
                if (base.Value == null)
                {
                    base.Value = new SECSValue(value);
                    NotifyPropertyChange("Value");
                }
                else
                {
                    if (base.Value != value)
                    {
                        base.Value.SetValue(value);
                        NotifyPropertyChange("Value");
                    }
                }
            }
        }

        public bool UseChildLength { get; set; }

        public string SelectedChildLength
        {
            get
            {
                string result;

                if (this.UseChildLength == true)
                {
                    result = string.Format("ChildCount: {0}", this.ValueItems.Count);
                }
                else
                {
                    result = string.Format("ChildCount: 0");
                }

                return result;
            }
        }

        public Dictionary<bool, string> ChildLength
        {
            get
            {
                Dictionary<bool, string> result;

                result = new Dictionary<bool, string>
                {
                    [true] = string.Format("ChildCount: {0}", this.ValueItems.Count),
                    [false] = string.Format("ChildCount: 0")
                };

                return result;
            }
        }

        public string GenerateRule { get; set; }

        public List<ExpandedEnhancedRemoteCommandParameterItem> ValueItems
        {
            get; private set;
        }
        #endregion
        #region Member Variable
        private int _count;
        #endregion
        #region Constructor
        public ExpandedEnhancedRemoteCommandParameterInfo()
        {
            this._count = 0;
            //this.ListType = EnhancedParameterListType.B;
            this.UseChildLength = true;

            this.GenerateRule = string.Empty;
            this.ValueItems = new List<ExpandedEnhancedRemoteCommandParameterItem>();
        }
        #endregion
        #region Clone
        public ExpandedEnhancedRemoteCommandParameterInfo Clone()
        {
            ExpandedEnhancedRemoteCommandParameterInfo result;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;

            result = new ExpandedEnhancedRemoteCommandParameterInfo()
            {
                //ListType = this.ListType,
                Name = this.Name,
                Format = this.Format,
                Count = this.Count,
                Value = this.Value,
                UseChildLength = this.UseChildLength,
                GenerateRule = this.GenerateRule,
            };

            if (this.ValueItems != null)
            {
                foreach (var item in this.ValueItems)
                {
                    expandedEnhancedCommandParameterItem = item.CopyTo();
                    result.ValueItems.Add(expandedEnhancedCommandParameterItem);
                }
            }

            return result;
        }
        #endregion
        #region CopyTo
        public new ExpandedEnhancedRemoteCommandParameterInfo CopyTo()
        {
            return Clone();
        }
        #endregion
        #region NotifyPropertyChange
        private void NotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.BeginInvoke(this, new PropertyChangedEventArgs(propertyName), null, null);
            }
        }
        #endregion
    }
}