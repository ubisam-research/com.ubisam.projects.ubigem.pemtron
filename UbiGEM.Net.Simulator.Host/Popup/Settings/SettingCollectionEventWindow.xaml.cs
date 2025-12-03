using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingVariableWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingCollectionEventWindow : Window
    {
        #region DisplayData
        private class DisplayData : INotifyPropertyChanged
        {
            #region Event
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
            #region MemberVariable
            private bool _isSelected;
            #endregion
            #region Properties
            public bool IsSelected
            {
                get
                {
                    return this._isSelected;
                }
                set
                {
                    this._isSelected = value;

                    NotifyPropertyChanged("IsSelected");
                }
            }

            public string CEID { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
            #endregion

            // Public Method
            #region ToString
            public override string ToString()
            {
                return string.Format("CEID={0}, Name={1}", this.CEID, this.Name);
            }
            #endregion

            // Protected Method
            #region NotifyPropertyChanged
            protected void NotifyPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion
        }
        #endregion

        #region MemberVariable
        private ObservableCollection<DisplayData> _displayData;
        private List<string> _selectedItems;
        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public SettingCollectionEventWindow()
        {
            InitializeComponent();

            this._displayData = new ObservableCollection<DisplayData>();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedCollectionEventInfo expandedCEInfo;

            this.MouseLeftButtonDown += delegate
            {
                try
                {
                    DragMove();
                }
                catch
                {
                }
            };

            if (this._messageProcessor != null && this._selectedItems != null)
            {
                foreach (Structure.CollectionEventInfo collectionEventInfo in this._messageProcessor.CollectionEventCollection.Items.Values.Where(t => t.Enabled == true))
                {
                    expandedCEInfo = collectionEventInfo as ExpandedCollectionEventInfo;

                    this._displayData.Add(new DisplayData()
                    {
                        CEID = expandedCEInfo.CEID,
                        Name = expandedCEInfo.Name,
                        Description = expandedCEInfo.Description,
                        IsSelected = this._selectedItems.Contains(expandedCEInfo.CEID)
                    });
                }
            }

            dgrCE.ItemsSource = this._displayData;
        }
        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.CurrentSetting.S1F23CollectionEventList.Clear();
            this._messageProcessor.CurrentSetting.S1F23CollectionEventList.AddRange(this._displayData.Where(t => t.IsSelected == true).Select(t => t.CEID));

            Close();
        }
        #endregion

        // CheckBox Event
        #region chkAll_Click
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked;

            isChecked = (sender as CheckBox).IsChecked.GetValueOrDefault();

            foreach (DisplayData tempItem in this._displayData)
            {
                tempItem.IsSelected = isChecked;
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, List<string> selectedItems)
        {
            this._messageProcessor = messageProcessor;
            this._displayData = new ObservableCollection<DisplayData>();
            this._selectedItems = selectedItems;
        }
        #endregion

        // Private Method
    }
}