using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedRemoteCommandValueSetInfo
    {
        #region Property
        public string Name { get; set; }

        public List<ExpandedRemoteCommandParameterInfo> ParameterItems
        {
            get; private set;
        }
        #endregion

        #region Constructor
        public ExpandedRemoteCommandValueSetInfo()
        {
            this.Name = string.Empty;
            this.ParameterItems = new List<ExpandedRemoteCommandParameterInfo>();
        }
        #endregion
        #region AddParameterItems
        public void AddParameterItems(List<ExpandedRemoteCommandParameterInfo> items)
        {
            this.ParameterItems.AddRange(items);
        }
        #endregion
        #region AddParameterItem
        public void AddParameterItem(ExpandedRemoteCommandParameterInfo item)
        {
            this.ParameterItems.Add(item);
        }
        #endregion
        #region Clone
        public ExpandedRemoteCommandValueSetInfo Clone()
        {
            ExpandedRemoteCommandValueSetInfo result;

            result = new ExpandedRemoteCommandValueSetInfo()
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

    public class ExpandedRemoteCommandValueSetCollection
    {
        #region MemberVariable
        private readonly Dictionary<string, ExpandedRemoteCommandValueSetInfo> _items;
        #endregion
        #region Property
        public Dictionary<string, ExpandedRemoteCommandValueSetInfo> Items
        {
            get
            {
                return this._items;
            }
        }
        public ExpandedRemoteCommandValueSetInfo this[string name]
        {
            set
            {
                this._items[name] = value;
            }
            get
            {
                ExpandedRemoteCommandValueSetInfo result;

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
        public ExpandedRemoteCommandValueSetCollection()
        {
            this._items = new Dictionary<string, ExpandedRemoteCommandValueSetInfo>
            {
                ["Default"] = new ExpandedRemoteCommandValueSetInfo()
                {
                    Name = "Default"
                }
            };
        }
        #endregion
        #region Clone
        public ExpandedRemoteCommandValueSetCollection Clone()
        {
            ExpandedRemoteCommandValueSetCollection result;

            result = new ExpandedRemoteCommandValueSetCollection();

            foreach (ExpandedRemoteCommandValueSetInfo info in this._items.Values)
            {
                result.Add(info.Clone());
            }

            return result;
        }
        #endregion
        #region Add
        public void Add(ExpandedRemoteCommandValueSetInfo info)
        {
            if (info != null && string.IsNullOrEmpty(info.Name) == false)
            {
                this._items[info.Name] = info;
            }
        }
        #endregion
    }
}