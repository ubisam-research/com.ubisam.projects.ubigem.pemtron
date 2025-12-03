using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info
{
    #region GEMObject
    public class SupervisedGEMObject
    {
        #region Property
        public string OBJSPEC
        {
            get; set;
        }
        public uint OBJTOKEN
        {
            get; set;
        }
        public GEMObjectAttributeCollection GEMObjectAttributeCollection
        {
            get; set;
        }
        #endregion
        #region Constructor
        public SupervisedGEMObject()
        {
            this.OBJSPEC = string.Empty;
            this.OBJTOKEN = 0;
            this.GEMObjectAttributeCollection = new GEMObjectAttributeCollection();
        }
        #endregion
        #region Clone
        public SupervisedGEMObject Clone()
        {
            SupervisedGEMObject result;

            result = new SupervisedGEMObject();

            result.OBJSPEC = this.OBJSPEC;
            result.OBJTOKEN = this.OBJTOKEN;

            result.GEMObjectAttributeCollection = this.GEMObjectAttributeCollection.Clone();
            
            return result;
        }
        #endregion
        #region ToString
        public new string ToString()
        {
            return string.Format("[OBJSPEC={0},OBJTOKEN={1}]", this.OBJSPEC, this.OBJTOKEN);
        }
        #endregion
        #region Validate
        public bool Validate(out string errorText)
        {
            bool result;

            result = true;
            errorText = string.Empty;

            if (string.IsNullOrEmpty(this.OBJSPEC) == true)
            {
                result = false;
                errorText = "OBJSPEC is empty";
            }

            return result;
        }
        #endregion
    }
    #endregion
    #region SupervisedGEMObjectCollection
    public class SupervisedGEMObjectCollection
    {
        #region Properties
        public List<SupervisedGEMObject> Items { get; set; }
        public SupervisedGEMObject this[string OBJSPEC, uint OBJTOKEN]
        {
            get
            {
                return this.Items.FirstOrDefault(t => t.OBJSPEC == OBJSPEC && t.OBJTOKEN == OBJTOKEN);
            }
        }
        #endregion
        #region Constructor
        public SupervisedGEMObjectCollection()
        {
            this.Items = new List<SupervisedGEMObject>();
        }
        #endregion
        #region Clone
        public SupervisedGEMObjectCollection Clone()
        {
            SupervisedGEMObjectCollection result;

            result = new SupervisedGEMObjectCollection();

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
        public void Add(SupervisedGEMObject info)
        {
            this.Items.Add(info);
        }
        #endregion
        #region Remove
        public void Remove(SupervisedGEMObject info)
        {
            if (info != null)
            {
                var varInfo = (from SupervisedGEMObject tempInfo in this.Items
                               where tempInfo.OBJSPEC == info.OBJSPEC && tempInfo.OBJTOKEN == info.OBJTOKEN
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
        public bool Validate(out string errorText, out SupervisedGEMObject invalidObject)
        {
            bool result;

            result = true;
            errorText = string.Empty;
            invalidObject = null;

            foreach (SupervisedGEMObject gemObject in this.Items)
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
