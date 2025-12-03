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
    public partial class SettingAlarmWindowForS5F3 : Window
    {
        #region Enum
        #region AlarmSelectType
        public enum AlarmSelectType
        {
            S5F3,
            S5F5
        }
        #endregion
        #endregion
        #region DisplayData
        private class DisplayData : INotifyPropertyChanged
        {
            #region Event
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
            #region MemberVariable
            private bool _isSelected;
            private bool _isEnabled;
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

            public bool IsEnabled
            {
                get
                {
                    return this._isEnabled;
                }
                set
                {
                    this._isEnabled = value;

                    NotifyPropertyChanged("IsEnabled");
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
        public delegate void AlarmSelectedEventHandler(List<long> selectedItems, List<long> enabledItems);

        public event AlarmSelectedEventHandler OnAlarmSelected;
        #endregion

        #region MemberVariable
        private ObservableCollection<DisplayData> _displayData;
        #endregion

        #region Constructor
        public SettingAlarmWindowForS5F3()
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
            List<long> enabledItems;

            selectedItems = new List<long>();
            enabledItems = new List<long>();

            selectedItems.AddRange(this._displayData.Where(t => t.IsSelected == true).Select(t => t.ID));
            enabledItems.AddRange(this._displayData.Where(t => t.IsEnabled == true).Select(t => t.ID));

            if (this.OnAlarmSelected != null)
            {
                this.OnAlarmSelected(selectedItems, enabledItems);
            }

            Close();
        }
        #endregion

        // CheckBox Event
        #region chkAllSelect_Click
        private void chkAllSelect_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked;

            isChecked = (sender as CheckBox).IsChecked.GetValueOrDefault();

            foreach (DisplayData tempItem in this._displayData)
            {
                tempItem.IsSelected = isChecked;
            }
        }
        #endregion
        #region chkAllEnable_Click
        private void chkAllEnable_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked;

            isChecked = (sender as CheckBox).IsChecked.GetValueOrDefault();

            foreach (DisplayData tempItem in this._displayData)
            {
                tempItem.IsEnabled = isChecked;
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(List<long> selecteItems, List<long> enabledItems, Structure.AlarmCollection alarmCollection)
        {
            string title;

            title = "Enable or Disable Alarm Send(S5F3)";

            grbTitle.Header = title;

            this._displayData = new ObservableCollection<DisplayData>();

            foreach (var tempAlarmInfo in alarmCollection.Items.OrderBy(t => t.ID))
            {
                this._displayData.Add(new DisplayData()
                {
                    IsSelected = selecteItems.Contains(tempAlarmInfo.ID),
                    ID = tempAlarmInfo.ID,
                    Code = tempAlarmInfo.Code,
                    IsEnabled = enabledItems.Contains(tempAlarmInfo.ID),
                    Description = tempAlarmInfo.Description
                });
            }
        }
        #endregion
    }
}