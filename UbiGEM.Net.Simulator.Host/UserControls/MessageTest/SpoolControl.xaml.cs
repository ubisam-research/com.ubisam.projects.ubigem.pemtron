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
    public partial class SpoolControl : UserControl
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
        public SpoolControl()
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
        #region btnS2F43_Click
        private void btnS2F43_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS2F43();

                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S2F43 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S2F43 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F43Edit_Click
        private void btnS2F43Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingSpoolWindow settingSpoolWindow;

            settingSpoolWindow = new Popup.SettingSpoolWindow();
            settingSpoolWindow.Initialize(this._messageProcessor, this._messageProcessor.HSMSDriverMessages, this._messageProcessor.CurrentSetting.S2F43ResetSpoolingStreamsAndFunctions);

            if (this._parentWindow != null)
            {
                settingSpoolWindow.Owner = this._parentWindow;
                settingSpoolWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingSpoolWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingSpoolWindow.OnSpoolSelected += settingSpoolWindow_OnSpoolSelected;

            settingSpoolWindow.ShowDialog();

            settingSpoolWindow.OnSpoolSelected -= settingSpoolWindow_OnSpoolSelected;

            settingSpoolWindow = null;
        }
        #endregion
        #region settingSpoolWindow_OnSpoolSelected
        private void settingSpoolWindow_OnSpoolSelected(List<string> selectedItems)
        {
            this._messageProcessor.CurrentSetting.S2F43ResetSpoolingStreamsAndFunctions = selectedItems;
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