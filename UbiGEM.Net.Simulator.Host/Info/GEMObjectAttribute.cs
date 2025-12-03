using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info
{
    #region GEMObjectAttribute
    public class GEMObjectAttribute : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region MemberVariable
        private UbiCom.Net.Structure.SECSItemFormat _format;
        private bool _isSelected;
        #endregion
        #region Property
        public string ATTRID
        {
            get; set;
        }
        public UbiCom.Net.Structure.SECSItemFormat Format
        {
            get
            {
                return this._format;
            }
            set
            {
                this._format = value;
                NotifyPropertyChanged("Format");
            }
        }
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                if (this._isSelected != value)
                {
                    this._isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }
        public string ATTRDATA
        {
            get; set;
        }
        public GEMObjectAttributeCollection ChildAttributes
        {
            get; set;
        }
        #endregion
        #region Constructor
        public GEMObjectAttribute()
        {
            this.ATTRID = string.Empty;
            this._format = UbiCom.Net.Structure.SECSItemFormat.A;
            this._isSelected = false;
            this.ATTRDATA = string.Empty;

            this.ChildAttributes = new GEMObjectAttributeCollection();
        }
        #endregion
        #region Clone
        public GEMObjectAttribute Clone()
        {
            GEMObjectAttribute result;

            result = new GEMObjectAttribute();

            result.ATTRID = this.ATTRID;
            result._format = this._format;
            result._isSelected = this._isSelected;
            result.ATTRDATA = this.ATTRDATA;

            if (this.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                result.ChildAttributes = this.ChildAttributes.Clone();
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
            string result;

            if (this.Format != UbiCom.Net.Structure.SECSItemFormat.L)
            {
                result = string.Format("[ATTRID={0},Format={1},ATTRDATA={2}]", this.ATTRID, this.Format, this.ATTRDATA);
            }
            else
            {
                result = string.Format("[ATTRID={0},Format={1},Child Count={2}]", this.ATTRID, this.Format, this.ChildAttributes.Items.Count);
            }
            return result;
        }
        #endregion
        #region Validate
        public bool Validate(out string errorText)
        {
            bool result;

            result = true;
            errorText = string.Empty;

            if (string.IsNullOrEmpty(this.ATTRID) == true)
            {
                result = false;
                errorText = "ATTRID is empty";
            }

            return result;
        }
        #endregion
    }
    #endregion
    #region GEMObjectAttributeCollection
    public class GEMObjectAttributeCollection
    {
        #region Properties
        public List<GEMObjectAttribute> Items { get; set; }
        public GEMObjectAttribute this[string ATTRID]
        {
            get
            {
                return this.Items.FirstOrDefault(t => t.ATTRID == ATTRID);
            }
        }
        #endregion
        #region Constructor
        public GEMObjectAttributeCollection()
        {
            this.Items = new List<GEMObjectAttribute>();
        }
        #endregion
        #region Clone
        public GEMObjectAttributeCollection Clone()
        {
            GEMObjectAttributeCollection result;

            result = new GEMObjectAttributeCollection();

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
        public void Add(GEMObjectAttribute info)
        {
            this.Items.Add(info);
        }
        #endregion
        #region Remove
        public void Remove(GEMObjectAttribute info)
        {
            if (info != null)
            {
                var varInfo = (from GEMObjectAttribute tempInfo in this.Items
                               where tempInfo.ATTRID == info.ATTRID
                               select tempInfo).FirstOrDefault();

                if (varInfo != null)
                {
                    this.Items.Remove(varInfo);
                }
            }
        }
        #endregion
        #region Clear
        public void Clear()
        {
            this.Items.Clear();
        }
        #endregion
        #region Validate
        public bool Validate(out string errorText, out GEMObjectAttribute invalidAttribute)
        {
            bool result;

            result = true;
            errorText = string.Empty;
            invalidAttribute = null;

            foreach (GEMObjectAttribute attr in this.Items)
            {
                if (result == true)
                {
                    if (attr.Validate(out errorText) == false)
                    {
                        result = false;
                        invalidAttribute = attr;
                        break;
                    }
                }
            }

            return result;
        }
        #endregion
    }
    #endregion
}
