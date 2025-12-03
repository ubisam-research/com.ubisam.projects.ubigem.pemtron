using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedEnhancedRemoteCommandValueSetInfo
    {
        #region Property
        public string Name { get; set; }

        public List<ExpandedEnhancedRemoteCommandParameterInfo> ParameterItems
        {
            get; private set;
        }
        #endregion

        #region Constructor
        public ExpandedEnhancedRemoteCommandValueSetInfo()
        {
            this.Name = string.Empty;
            this.ParameterItems = new List<ExpandedEnhancedRemoteCommandParameterInfo>();
        }
        #endregion
        #region AddParameterItems
        public void AddParameterItems(List<ExpandedEnhancedRemoteCommandParameterInfo> items)
        {
            this.ParameterItems.AddRange(items);
        }
        #endregion
        #region AddParameterItem
        public void AddParameterItem(ExpandedEnhancedRemoteCommandParameterInfo item)
        {
            this.ParameterItems.Add(item);
        }
        #endregion
        #region Clone
        public ExpandedEnhancedRemoteCommandValueSetInfo Clone()
        {
            ExpandedEnhancedRemoteCommandValueSetInfo result;

            result = new ExpandedEnhancedRemoteCommandValueSetInfo()
            {
                Name = this.Name
            };

            if (this.ParameterItems != null)
            {
                foreach (var item in this.ParameterItems)
                {
                    result.ParameterItems.Add(item.Clone());
                }
            }

            return result;
        }
        #endregion
        #region ToString
        public new string ToString()
        {
            string result;

            result = string.Format("Name={0}, ParameterItems.Count={1}", this.Name, this.ParameterItems.Count);

            return result;
        }
        #endregion
    }

    public class ExpandedEnhancedRemoteCommandValueSetCollection
    {
        #region MemberVariable
        private Dictionary<string, ExpandedEnhancedRemoteCommandValueSetInfo> _items;
        #endregion
        #region Property
        public Dictionary<string, ExpandedEnhancedRemoteCommandValueSetInfo> Items
        {
            get
            {
                return this._items;
            }
        }
        public ExpandedEnhancedRemoteCommandValueSetInfo this[string name]
        {
            set
            {
                this._items[name] = value;
            }
            get
            {
                ExpandedEnhancedRemoteCommandValueSetInfo result;

                result = null;

                if( this._items.ContainsKey(name) == true)
                {
                    result = this._items[name];
                }

                return result;
            }
        }
        #endregion
        #region Constructor
        public ExpandedEnhancedRemoteCommandValueSetCollection()
        {
            this._items = new Dictionary<string, ExpandedEnhancedRemoteCommandValueSetInfo>();
            this._items["Default"] = new ExpandedEnhancedRemoteCommandValueSetInfo()
            {
                Name = "Default"
            };
        }
        #endregion
        #region Clone
        public ExpandedEnhancedRemoteCommandValueSetCollection Clone()
        {
            ExpandedEnhancedRemoteCommandValueSetCollection result;

            result = new ExpandedEnhancedRemoteCommandValueSetCollection();

            foreach (ExpandedEnhancedRemoteCommandValueSetInfo info in this._items.Values)
            {
                result.Add(info.Clone());
            }

            return result;
        }
        #endregion
        #region Add
        public void Add(ExpandedEnhancedRemoteCommandValueSetInfo info)
        {
            if (info != null && string.IsNullOrEmpty(info.Name) == false)
            {
                this._items[info.Name] = info;
            }
        }
        #endregion
    }
}