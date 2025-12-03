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

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingAlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingAlarmWindow : Window
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
            #region Property
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

            public long ID { get; set; }

            public int Code { get; set; }

            public string Description { get; set; }
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
        }
        #endregion

        #region Delegate
        public delegate void AlarmSelectedEventHandler(List<long> selectedItems);

        public event AlarmSelectedEventHandler OnAlarmSelected;
        #endregion

        #region MemberVariable
        private ObservableCollection<DisplayData> _displayData;
        #endregion

        #region Constructor
        public SettingAlarmWindow()
        {
            InitializeComponent();
            this._displayData = new ObservableCollection<DisplayData>();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

            dgrAlarm.ItemsSource = this._displayData;
        }
        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            List<long> selectedItems;

            selectedItems = new List<long>();

            selectedItems.AddRange(this._displayData.Where(t => t.IsSelected == true).Select(t => t.ID));

            if (this.OnAlarmSelected != null)
            {
                this.OnAlarmSelected(selectedItems);
            }

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
        public void Initialize(List<long> selecteItems, Structure.AlarmCollection alarmCollection)
        {
            string title;

            title = "List Alarms Request(S5F5)";

            grbTitle.Header = title;

            this._displayData = new ObservableCollection<DisplayData>();

            foreach (var tempAlarmInfo in alarmCollection.Items.OrderBy(t => t.ID))
            {
                this._displayData.Add(new DisplayData()
                {
                    IsSelected = selecteItems.Contains(tempAlarmInfo.ID),
                    ID = tempAlarmInfo.ID,
                    Code = tempAlarmInfo.Code,
                    Description = tempAlarmInfo.Description
                });
            }

        }
        #endregion
    }
}