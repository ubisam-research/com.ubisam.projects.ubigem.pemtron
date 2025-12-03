using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UbiGEM.Net.Simulator.Host
{
    public class SecsFormatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region [Variables]
        private List<UbiCom.Net.Structure.SECSItemFormat> _items;
        private List<UbiCom.Net.Structure.SECSItemFormat> _selectedItems;
        #endregion

        #region [Properties]
        public List<UbiCom.Net.Structure.SECSItemFormat> Items
        {
            get
            {
                return this._items;
            }
            set
            {
                this._items = value;

                NotifyPropertyChanged("Items");
            }
        }

        public List<UbiCom.Net.Structure.SECSItemFormat> SelectedItems
        {
            get
            {
                return this._selectedItems;
            }
            set
            {
                this._selectedItems = value;

                NotifyPropertyChanged("SelectedItems");
            }
        }
        #endregion

        #region Constructor
        public SecsFormatViewModel()
        {
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(List<UbiCom.Net.Structure.SECSItemFormat> selected)
        {
            this._items = new List<UbiCom.Net.Structure.SECSItemFormat>();
            this._selectedItems = new List<UbiCom.Net.Structure.SECSItemFormat>();

            foreach (string tempName in typeof(UbiCom.Net.Structure.SECSItemFormat).GetEnumNames())
            {
                if (tempName != UbiCom.Net.Structure.SECSItemFormat.None.ToString())
                {
                    this._items.Add((UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), tempName));
                }
            }

            selected.ForEach(t =>
            {
                this._selectedItems.Add(t);
            });
        }
        #endregion

        // Protected Method
        #region NotifyPropertyChanged
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}