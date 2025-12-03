using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info
{
    #region GEMObjectID
    public class GEMObjectID : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region MemberVariable
        private bool _isChecked;
        #endregion
        #region Property
        public string OBJID
        {
            get; set;
        }
        public bool IsSelected
        {
            get
            {
                return this._isChecked;
            }
            set
            {
                if (this._isChecked != value)
                {
                    this._isChecked = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }
        public GEMObjectAttributeCollection ObjectAttributeCollection
        {
            get; private set;
        }
        #endregion
        #region Constructor
        public GEMObjectID()
        {
            this.OBJID = string.Empty;
            this._isChecked = false;

            this.ObjectAttributeCollection = new GEMObjectAttributeCollection();
        }
        #endregion
        #region Clone
        public GEMObjectID Clone()
        {
            GEMObjectID result;

            result = new GEMObjectID();

            result.OBJID = this.OBJID;
            result._isChecked = this._isChecked;

            result.ObjectAttributeCollection = this.ObjectAttributeCollection.Clone();

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
            return string.Format("[OBJID={0}]", this.OBJID);
        }
        #endregion
        #region Validate
        public bool Validate(out string errorText)
        {
            bool result;

            result = true;
            errorText = string.Empty;

            if (string.IsNullOrEmpty(this.OBJID) == true)
            {
                result = false;
                errorText = "OBJID is empty";
            }

            return result;
        }
        #endregion
    }
    #endregion
    #region GEMObjectIDCollection
    public class GEMObjectIDCollection
    {
        #region Properties
        public List<GEMObjectID> Items { get; set; }
        public GEMObjectID this[string OBJID]
        {
            get
            {
                return this.Items.FirstOrDefault(t => t.OBJID == OBJID);
            }
        }
        #endregion
        #region Constructor
        public GEMObjectIDCollection()
        {
            this.Items = new List<GEMObjectID>();
        }
        #endregion
        #region Clone
        public GEMObjectIDCollection Clone()
        {
            GEMObjectIDCollection result;

            result = new GEMObjectIDCollection();

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
        public void Add(GEMObjectID info)
        {
            this.Items.Add(info);
        }
        #endregion
        #region Remove
        public void Remove(GEMObjectID info)
        {
            if (info != null)
            {
                var varInfo = (from GEMObjectID tempInfo in this.Items
                               where tempInfo.OBJID == info.OBJID
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
        public bool Validate(out string errorText, out GEMObjectID invalidObject)
        {
            bool result;

            result = true;
            errorText = string.Empty;
            invalidObject = null;

            foreach (GEMObjectID gemObjectID in this.Items)
            {
                if (result == true)
                {
                    if (gemObjectID.Validate(out errorText) == false)
                    {
                        result = false;
                        invalidObject = gemObjectID;
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
