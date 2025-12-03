using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UbiGEM.Net.Simulator.Host.Info
{
    public class AckInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region MemberVariable
        private bool _isUse;
        #endregion
        #region Property
        public string Name
        {
            get
            {
                return $"S{this.Stream}F{this.Function} {this.DataDictinary}";
            }
        }
        public int Stream { set; get; }
        public int Function { set; get; }
        public DataDictinaryList DataDictinary { get; set; }
        public bool Use
        {
            get
            {
                return this._isUse;
            }
            set
            {
                if (this._isUse != value)
                {
                    this._isUse = value;
                    NotifyPropertyChanged("Use");
                }
            }
        }
        public byte Value { get; set; }
        #endregion
        #region Constructor
        public AckInfo()
        {
            this.Stream = -1;
            this.Function = -1;
            this._isUse = false;
            this.Value = 0;
        }
        #endregion
        #region NotifyPropertyChanged
        protected void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class AckCollection
    {
        public List<AckInfo> Items
        {
            get
            {
                List<AckInfo> result = new List<AckInfo>();

                lock (this._items)
                {
                    foreach (AckInfo info in this._items)
                    {
                        result.Add(info);
                    }
                }

                return result;
            }
        }
        public AckInfo this[int stream, int function]
        {
            get
            {
                AckInfo result;

                lock (this._items)
                {
                    result = this._items.FirstOrDefault(t => t.Stream == stream && t.Function == function);
                }

                return result;
            }
        }
        private readonly List<AckInfo> _items;
        public AckCollection()
        {
            this._items = new List<AckInfo>();
        }
        public void Add(AckInfo info)
        {
            if (info != null)
            {
                lock (this._items)
                {
                    if (this._items.Exists(t => t.Stream == info.Stream && t.Function == info.Function) == false)
                    {
                        this._items.Add(info);
                    }
                }
            }
        }
        public void Clear()
        {
            lock (this._items)
            {
                this._items.Clear();
            }
        }
    }
}
