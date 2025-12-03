using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.XPath;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedRemoteCommandParameterInfo : Structure.CommandParameterInfo, INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Property
        public int Count
        {
            get
            {
                int result;

                if (this.Format != SECSItemFormat.A)
                {
                    result = this._count;
                }
                else
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

                return result;
            }
            set
            {
                if (this._count != value)
                {
                    if (this.Format != SECSItemFormat.A)
                    {
                        this._count = value;
                    }
                    else
                    {
                        this._count = 0;
                    }

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
                    result = base.Value.ToString();
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
                    if (base.Value.ToString() != value)
                    {
                        base.Value.SetValue(value);
                        NotifyPropertyChange("Value");
                    }
                }
            }
        }

        public string GenerateRule
        {
            get;
            set;
        }
        #endregion
        #region Member Variable
        private int _count;

        #endregion
        #region Constructor
        public ExpandedRemoteCommandParameterInfo()
        {
            this._count = 0;
            this.GenerateRule = string.Empty;
        }
        #endregion
        #region NofityPropertyChange
        private void NotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.BeginInvoke(this, new PropertyChangedEventArgs(propertyName), null, null);
            }
        }
        #endregion
        #region Clone
        public ExpandedRemoteCommandParameterInfo Clone()
        {
            ExpandedRemoteCommandParameterInfo result;

            result = new ExpandedRemoteCommandParameterInfo()
            {
                Name = this.Name,
                Format = this.Format,
                Count = this.Count,
                Value = this.Value,
                GenerateRule = this.GenerateRule,
            };

            return result;
        }
        #endregion
        #region CopyTo
        public new ExpandedRemoteCommandParameterInfo CopyTo()
        {
            return Clone();
        }
        #endregion
    }
}
