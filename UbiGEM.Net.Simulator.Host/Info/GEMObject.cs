using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info
{
    #region GEMObject
    public class GEMObject : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region MemberVariable
        #endregion
        #region Property
        public string OBJSPEC
        {
            get; set;
        }
        public string OBJTYPE
        {
            get; set;
        }
        public GEMObjectIDCollection ObjectIDCollection
        {
            get; set;
        }
        public GEMObjectAttributeCollection AttributeCollection
        {
            get; private set;
        }
        #endregion
        #region Constructor
        public GEMObject()
        {
            this.OBJSPEC = string.Empty;
            this.OBJTYPE = string.Empty;

            this.ObjectIDCollection = new GEMObjectIDCollection();
            this.AttributeCollection = new GEMObjectAttributeCollection();
        }
        #endregion
        #region Clone
        public GEMObject Clone()
        {
            GEMObject result;

            result = new GEMObject();

            result.OBJSPEC = this.OBJSPEC;
            result.OBJTYPE = this.OBJTYPE;

            result.ObjectIDCollection = this.ObjectIDCollection.Clone();
            result.AttributeCollection = this.AttributeCollection.Clone();
            
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
            return string.Format("[OBJSPEC={0},OBJTYPE={1}]", this.OBJSPEC, this.OBJTYPE);
        }
        #endregion
        #region Validate
        public bool Validate(out string errorText)
        {
            bool result;

            result = true;
            errorText = string.Empty;

            if (string.IsNullOrEmpty(this.OBJSPEC) == true || string.IsNullOrEmpty(this.OBJTYPE) == true)
            {
                result = false;
                errorText = "OBJSPEC or OBJTYPE are empty";
            }

            return result;
        }
        #endregion
    }
    #endregion
    #region GEMObjectCollection
    public class GEMObjectCollection
    {
        #region Properties
        public List<GEMObject> Items { get; set; }
        public GEMObject this[string OBJSPEC, string OBJTYPE]
        {
            get
            {
                return this.Items.FirstOrDefault(t => t.OBJSPEC == OBJSPEC && t.OBJTYPE == OBJTYPE);
            }
        }
        #endregion
        #region Constructor
        public GEMObjectCollection()
        {
            this.Items = new List<GEMObject>();
        }
        #endregion
        #region Clone
        public GEMObjectCollection Clone()
        {
            GEMObjectCollection result;

            result = new GEMObjectCollection();

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
        public void Add(GEMObject info)
        {
            this.Items.Add(info);
        }
        #endregion
        #region Remove
        public void Remove(GEMObject info)
        {
            if (info != null)
            {
                var varInfo = (from GEMObject tempInfo in this.Items
                               where tempInfo.OBJSPEC == info.OBJSPEC && tempInfo.OBJTYPE == info.OBJTYPE
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
        public bool Validate(out string errorText, out GEMObject invalidObject)
        {
            bool result;

            result = true;
            errorText = string.Empty;
            invalidObject = null;

            foreach (GEMObject gemObject in this.Items)
            {
                if (result == true)
                {
                    if (gemObject.Validate(out errorText) == false)
                    {
                        result = false;
                        invalidObject = gemObject;
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
