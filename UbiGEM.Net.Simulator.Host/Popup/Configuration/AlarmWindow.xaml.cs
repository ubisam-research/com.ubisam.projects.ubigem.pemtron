using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private ObservableCollection<ExpandedAlarmInfo> _displayInfo;
        #endregion

        #region Constructor
        public AlarmWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedAlarmInfo alarmInfo;
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

            this._displayInfo = new ObservableCollection<ExpandedAlarmInfo>();

            if (this._messageProcessor != null)
            {
                foreach (var alarmItem in this._messageProcessor.AlarmCollection.Items.OrderBy(t => t.ID))
                {
                    alarmInfo = alarmItem as ExpandedAlarmInfo;
                    this._displayInfo.Add(alarmInfo.Clone());
                }
            }
            dgrAlarm.ItemsSource = this._displayInfo;
        }
        #endregion

        // User Control Event
        #region ClientMenu_OnAdd
        private void ClientMenu_OnAdd(object sender, EventArgs e)
        {
            ExpandedAlarmInfo expandedAlarmInfo;

            expandedAlarmInfo = new ExpandedAlarmInfo()
            {
                IsInheritance = false,
                ID = -1,
            };
            this._displayInfo.Add(expandedAlarmInfo);

            dgrAlarm.SelectedItem = expandedAlarmInfo;
            dgrAlarm.ScrollIntoView(expandedAlarmInfo);
        }
        #endregion
        #region ClientMenu_OnRemove
        private void ClientMenu_OnRemove(object sender, EventArgs e)
        {
            ExpandedAlarmInfo expandedAlarmInfo;

            if (dgrAlarm.SelectedItem != null)
            {
                expandedAlarmInfo = dgrAlarm.SelectedItem as ExpandedAlarmInfo;

                if (expandedAlarmInfo != null && expandedAlarmInfo.IsInheritance == false)
                {
                    this._displayInfo.Remove(expandedAlarmInfo);
                }
            }
        }
        #endregion

        // DataGrid Event
        #region dgrAlarm_SelectionChanged
        private void dgrAlarm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedAlarmInfo expandedAlarmInfo;

            dgcALID.IsReadOnly = false;
            dgcALCD.IsReadOnly = false;
            dgcDescription.IsReadOnly = false;

            ctlAddRemove.ChangeButtonEnabled(true, true);

            if (dgrAlarm.SelectedItem != null)
            {
                expandedAlarmInfo = dgrAlarm.SelectedItem as ExpandedAlarmInfo;

                if (expandedAlarmInfo != null)
                {
                    if (expandedAlarmInfo.IsInheritance == true)
                    {
                        dgcALID.IsReadOnly = true;
                        dgcALCD.IsReadOnly = true;
                        dgcDescription.IsReadOnly = true;

                        ctlAddRemove.ChangeButtonEnabled(true, false);
                    }
                }
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            Structure.GemDriverError error;

            errorText = string.Empty;
            if (IsValid(out errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                if (this._messageProcessor != null)
                {
                    this._messageProcessor.AlarmCollection.Items.Clear();

                    foreach (var alarmItem in this._displayInfo)
                    {
                        this._messageProcessor.AlarmCollection.Add(alarmItem);
                    }

                    if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == false)
                    {
                        error = this._messageProcessor.SaveConfigFile(out errorText);

                        if (error != Structure.GemDriverError.Ok)
                        {
                            MessageBox.Show(errorText);
                        }
                        else
                        {
                            this._messageProcessor.IsDirty = false;
                        }
                    }
                    else
                    {
                        this._messageProcessor.IsDirty = true;
                    }
                }
            }
        }
        #endregion
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion

        // Private Method
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            List<long> usedIDs;

            errorText = string.Empty;
            result = true;

            usedIDs = new List<long>();

            foreach (var alarmItem in this._displayInfo)
            {
                if (alarmItem.ID <= 0)
                {
                    errorText = string.Format("Invalid ALID: {0}", alarmItem.ID);
                    result = false;
                }
                else
                {
                    if (usedIDs.Contains(alarmItem.ID) == true)
                    {
                        errorText = string.Format("Dupelicated ALID: {0}", alarmItem.ID);
                        result = false;
                    }
                    else
                    {
                        usedIDs.Add(alarmItem.ID);
                    }
                }
            }

            return result;
        }
        #endregion
    }
}