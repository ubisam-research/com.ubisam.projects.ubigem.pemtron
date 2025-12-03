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
    public partial class ETCControl : UserControl
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
        public ETCControl()
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
        #region btnS2F17_Click
        private void btnS2F17_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS2F17();

                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S2F17 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S2F17 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F25_Click
        private void btnS2F25_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS2F25();

                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S2F25 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S2F25 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F25Edit_Click
        private void btnS2F25Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingLoopback settingLoopback;

            settingLoopback = new Popup.SettingLoopback();
            settingLoopback.Initialize(this._messageProcessor, this._messageProcessor.CurrentSetting.LoopbackDiagnostic);

            if (this._parentWindow != null)
            {
                settingLoopback.Owner = this._parentWindow;
                settingLoopback.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingLoopback.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingLoopback.ShowDialog();
        }
        #endregion
        #region btnS2F31_Click
        private void btnS2F31_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS2F31();

                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S2F31 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S2F31 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS10F3_Click
        private void btnS10F3_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (string.IsNullOrEmpty(this._messageProcessor.CurrentSetting.TerminalMessage.S10F3TID) == true)
            {
                MessageBox.Show("Terminal Message TID of S10F3 is empty");
            }
            else
            {
                if (this._messageProcessor.HSMSDriverConnected == true)
                {
                    driverResult = this._messageProcessor.SendS10F3();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S10F3 : Result={0}", driverResult));
                }
                else
                {
                    MessageBox.Show("Send S10F3 Failed - Disconnected");
                }
            }
        }
        #endregion
        #region btnS10F3Edit_Click
        private void btnS10F3Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingTerminalWindow settingTerminalWindow;

            settingTerminalWindow = new Popup.SettingTerminalWindow();

            if (this._parentWindow != null)
            {
                settingTerminalWindow.Owner = this._parentWindow;
                settingTerminalWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingTerminalWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingTerminalWindow.Initialize(this._messageProcessor, Popup.SettingTerminalWindow.TerminalMessageSelectType.S10F3, this._messageProcessor.CurrentSetting.TerminalMessage);

            settingTerminalWindow.OnTerminalMessageChanged += settingTerminalWindow_OnTerminalMessageChanged;

            settingTerminalWindow.ShowDialog();

            settingTerminalWindow.OnTerminalMessageChanged -= settingTerminalWindow_OnTerminalMessageChanged;

            settingTerminalWindow = null;
        }
        #endregion
        #region btnS10F5_Click
        private void btnS10F5_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (string.IsNullOrEmpty(this._messageProcessor.CurrentSetting.TerminalMessage.S10F5TID) == true)
            {
                MessageBox.Show("Terminal Message TID of S10F5 is empty");
            }
            else
            {
                if (this._messageProcessor.HSMSDriverConnected == true)
                {
                    driverResult = this._messageProcessor.SendS10F5();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S10F5 : Result={0}", driverResult));
                }
                else
                {
                    MessageBox.Show("Send S10F5 Failed - Disconnected");
                }
            }
        }
        #endregion
        #region btnS10F5Edit_Click
        private void btnS10F5Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingTerminalWindow settingTerminalWindow;

            settingTerminalWindow = new Popup.SettingTerminalWindow();

            if (this._parentWindow != null)
            {
                settingTerminalWindow.Owner = this._parentWindow;
                settingTerminalWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingTerminalWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingTerminalWindow.Initialize(this._messageProcessor, Popup.SettingTerminalWindow.TerminalMessageSelectType.S10F5, this._messageProcessor.CurrentSetting.TerminalMessage);

            settingTerminalWindow.OnTerminalMessageChanged += settingTerminalWindow_OnTerminalMessageChanged;

            settingTerminalWindow.ShowDialog();

            settingTerminalWindow.OnTerminalMessageChanged -= settingTerminalWindow_OnTerminalMessageChanged;

            settingTerminalWindow = null;
        }
        #endregion
        #region settingTerminalWindow_OnTerminalMessageChanged
        private void settingTerminalWindow_OnTerminalMessageChanged(Popup.SettingTerminalWindow.TerminalMessageSelectType selectType, TerminalMessageInfo terminalMessageInfo)
        {
            this._messageProcessor.CurrentSetting.TerminalMessage = terminalMessageInfo;
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
