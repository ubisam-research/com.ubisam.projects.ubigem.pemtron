using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.UserControls.MessageTest
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);
        public WriteLog OnWriteLog { get; set; }
        #endregion
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private Window _parentWindow;
        #endregion
        #region Constructor
        public AlarmControl()
        {
            InitializeComponent();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            parent = this.Parent;

            if (parent != null)
            {
                while (LogicalTreeHelper.GetParent(parent) != null && parent is Window == false)
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }

                if (parent is Window == true)
                {
                    this._parentWindow = parent as Window;
                }
            }
        }
        #endregion
        // GroupBox Mouse Event
        #region GroupBox_MouseUp
        private void GroupBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            expander.IsExpanded = !expander.IsExpanded;
        }
        #endregion
        // Button Event
        #region btnS5F3Edit_Click
        private void btnS5F3Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingAlarmWindowForS5F3 settingAlarmWindow;

            settingAlarmWindow = new Popup.SettingAlarmWindowForS5F3();

            if (this._parentWindow != null)
            {
                settingAlarmWindow.Owner = this._parentWindow;
                settingAlarmWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingAlarmWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingAlarmWindow.Initialize(this._messageProcessor.CurrentSetting.S5F3SelectedAlarmSend, this._messageProcessor.CurrentSetting.S5F3EnabledAlarmSend, this._messageProcessor.AlarmCollection);
            settingAlarmWindow.OnAlarmSelected += SettingAlarmWindow_OnAlarmSelected;

            settingAlarmWindow.ShowDialog();

            settingAlarmWindow.OnAlarmSelected -= SettingAlarmWindow_OnAlarmSelected;

            settingAlarmWindow = null;
        }
        #endregion
        #region btnS5F3_Click
        private void btnS5F3_Click(object sender, RoutedEventArgs e)
        {
            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                this._messageProcessor.SendS5F3();
            }
            else
            {
                MessageBox.Show("Send S5F3 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS5F5Edit_Click
        private void btnS5F5Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingAlarmWindow settingAlarmWindow;

            settingAlarmWindow = new Popup.SettingAlarmWindow();

            if (this._parentWindow != null)
            {
                settingAlarmWindow.Owner = this._parentWindow;
                settingAlarmWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingAlarmWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingAlarmWindow.Initialize(this._messageProcessor.CurrentSetting.S5F5ListAlarmsRequest, this._messageProcessor.AlarmCollection);

            settingAlarmWindow.OnAlarmSelected += settingAlarmWindow_OnAlarmSelected;

            settingAlarmWindow.ShowDialog();

            settingAlarmWindow.OnAlarmSelected -= settingAlarmWindow_OnAlarmSelected;

            settingAlarmWindow = null;
        }
        #endregion
        #region btnS5F5_Click
        private void btnS5F5_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS5F5();
                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S5F5 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S5F5 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS5F7_Click
        private void btnS5F7_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS5F7();

                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S5F7 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S5F7 Failed - Disconnected");
            }
        }
        #endregion
        #region settingAlarmWindow_OnAlarmSelected
        private void settingAlarmWindow_OnAlarmSelected(List<long> selectedItems)
        {
            this._messageProcessor.CurrentSetting.S5F5ListAlarmsRequest = selectedItems;
            this._messageProcessor.IsDirty = true;
        }

        private void SettingAlarmWindow_OnAlarmSelected(List<long> selectedItems, List<long> enabledItems)
        {
            this._messageProcessor.CurrentSetting.S5F3SelectedAlarmSend = selectedItems;
            this._messageProcessor.CurrentSetting.S5F3EnabledAlarmSend = enabledItems;
            this._messageProcessor.IsDirty = true;
        }
        #endregion
        #region RaiseWriteLog
        private void RaiseWriteLog(MessageProcessor.DriverLogType logType, string logText)
        {
            if (this.OnWriteLog != null)
            {
                this.OnWriteLog.BeginInvoke(logType, logText, null, null);
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion
    }
}
