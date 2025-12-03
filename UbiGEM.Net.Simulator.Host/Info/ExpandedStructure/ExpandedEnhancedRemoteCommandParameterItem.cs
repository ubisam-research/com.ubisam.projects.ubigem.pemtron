using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedEnhancedRemoteCommandParameterItem : Structure.EnhancedCommandParameterItem, INotifyPropertyChanged
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
                    result = this.ChildParameterItem.Count;
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

                        /*
                        if (this.ListType == EnhancedParameterListType.B)
                        {
                            result = (uint)Encoding.Default.GetByteCount(base.Value.ToString());
                        }
                        else
                        {
                            result = (uint)this.Value.Count(t => t == ' ') + 1;
                        }
                        */
                    }
                }
                else
                {
                    result = this.Value.Count(t => t == ' ') + 1;

                    /*
                    if (this.ListType == EnhancedParameterListType.B)
                    {
                        result = this._count;
                    }
                    else
                    {
                        result = (uint)this.Value.Count(t => t == ' ') + 1;
                    }
                    */
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
                    NotifyPropertyChange("Count");
                }
                else
                {
                    if (base.Value != value)
                    {
                        base.Value.SetValue(value);

                        NotifyPropertyChange("Value");
                        NotifyPropertyChange("Count");
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
                    result = string.Format("ChildCount: {0}", this.ChildParameterItem.Count);
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
                    [true] = string.Format("ChildCount: {0}", this.ChildParameterItem.Count),
                    [false] = string.Format("ChildCount: 0")
                };

                return result;
            }
        }

        public string GenerateRule
        {
            get;
            set;
        }
        public new List<ExpandedEnhancedRemoteCommandParameterItem> ChildParameterItem
        {
            get;
            private set;
        }
        #endregion
        #region Member Variable
        private int _count;
        #endregion
        #region Constructor
        public ExpandedEnhancedRemoteCommandParameterItem()
        {
            //this.ListType = EnhancedParameterListType.B;
            this.GenerateRule = string.Empty;
            this._count = 0;
            this.UseChildLength = true;
            this.ChildParameterItem = new List<ExpandedEnhancedRemoteCommandParameterItem>();
        }
        #endregion
        #region Clone
        public ExpandedEnhancedRemoteCommandParameterItem Clone()
        {
            ExpandedEnhancedRemoteCommandParameterItem result;

            result = new ExpandedEnhancedRemoteCommandParameterItem()
            {
                //ListType = this.ListType,
                Name = this.Name,
                Format = this.Format,
                Count = this.Count,
                Value = this.Value,
                UseChildLength = this.UseChildLength,
                GenerateRule = this.GenerateRule,
            };

            if (this.ChildParameterItem != null)
            {
                foreach (ExpandedEnhancedRemoteCommandParameterItem child in this.ChildParameterItem)
                {
                    result.ChildParameterItem.Add(child.Clone());
                }
            }

            return result;
        }
        #endregion
        #region CopyTo
        public new ExpandedEnhancedRemoteCommandParameterItem CopyTo()
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