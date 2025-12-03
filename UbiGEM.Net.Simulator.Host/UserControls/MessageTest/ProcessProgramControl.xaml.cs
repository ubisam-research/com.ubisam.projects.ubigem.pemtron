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
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Simulator.Host.Popup;

namespace UbiGEM.Net.Simulator.Host.UserControls.MessageTest
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProcessProgramControl : UserControl
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
        public ProcessProgramControl()
        {
            InitializeComponent();
        }
        #endregion

        // GroupBox Mouse Event
        #region GroupBox_MouseUp
        private void GroupBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            expander.IsExpanded = !expander.IsExpanded;
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

            InitializeComboBoxFmtPPS7F23();
        }
        #endregion

        // Button Event
        #region btnS7F1_Click
        private void btnS7F1_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (string.IsNullOrEmpty(this._messageProcessor.CurrentSetting.ProcessProgramFileS7F1) == false)
                {
                    if (System.IO.File.Exists(this._messageProcessor.CurrentSetting.ProcessProgramFileS7F1) == true)
                    {
                        driverResult = this._messageProcessor.SendS7F1(this._messageProcessor.CurrentSetting.ProcessProgramFileS7F1);

                        RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S7F1 : Result={0}", driverResult));
                    }
                    else
                    {
                        RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F1 : Fail. \r\n\r\n {0} is not exists", this._messageProcessor.CurrentSetting.ProcessProgramFileS7F1));
                    }
                }
                else
                {
                    RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F1 : Fail. ProcessProgramFile is not selected"));
                }
            }
            else
            {
                MessageBox.Show("Send S7F1 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS7F1Edit_Click
        private void btnS7F1Edit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog;

            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            openFileDialog.FileName = this._messageProcessor.CurrentSetting.ProcessProgramFileS7F1;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._messageProcessor.CurrentSetting.ProcessProgramFileS7F1 = openFileDialog.FileName;
                this._messageProcessor.IsDirty = true;
            }
        }
        #endregion
        #region btnS7F3_Click
        private void btnS7F3_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (string.IsNullOrEmpty(this._messageProcessor.CurrentSetting.ProcessProgramFileS7F3) == false)
                {
                    if (System.IO.File.Exists(this._messageProcessor.CurrentSetting.ProcessProgramFileS7F3) == true)
                    {
                        driverResult = this._messageProcessor.SendS7F3(this._messageProcessor.CurrentSetting.ProcessProgramFileS7F3);

                        RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S7F3 : Result={0}", driverResult));
                    }
                    else
                    {
                        RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F3 : Fail. \r\n\r\n {0} is not exists", this._messageProcessor.CurrentSetting.ProcessProgramFileS7F3));
                    }
                }
                else
                {
                    RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F3 : Fail. ProcessProgramFile is not selected"));
                }
            }
            else
            {
                MessageBox.Show("Send S7F3 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS7F3Edit_Click
        private void btnS7F3Edit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog;

            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            openFileDialog.FileName = this._messageProcessor.CurrentSetting.ProcessProgramFileS7F3;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._messageProcessor.CurrentSetting.ProcessProgramFileS7F3 = openFileDialog.FileName;
                this._messageProcessor.IsDirty = true;
            }
        }
        #endregion
        #region btnS7F5_Click
        private void btnS7F5_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (string.IsNullOrEmpty(this._messageProcessor.CurrentSetting.ProcessProgramIDS7F5) == false)
                {
                    driverResult = this._messageProcessor.SendS7F5(this._messageProcessor.CurrentSetting.ProcessProgramIDS7F5);

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S7F5 : Result={0}", driverResult));
                }
                else
                {
                    RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F5 : Fail. ProcessProgramID is balnk"));
                }
            }
            else
            {
                MessageBox.Show("Send S7F5 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS75Edit_Click
        private void btnS75Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingFormattedProcessProgram popup;

            popup = new SettingFormattedProcessProgram();
            popup.Initialize(this._messageProcessor, SettingFormattedProcessProgram.Type.S7F5);
            popup.OnPPIDChanged += Popup_OnPPIDChanged;
            popup.Owner = this._parentWindow;
            popup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popup.ShowDialog();
        }
        #endregion
        #region btnS7F17_Click
        private void btnS7F17_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS7F17();

                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S7F17 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S7F17 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS7F17Edit_Click
        private void btnS7F17Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingProcessProgramDelete popup;

            popup = new SettingProcessProgramDelete();
            popup.Initialize(this._messageProcessor);
            popup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popup.Owner = this._parentWindow;
            popup.ShowDialog();
        }
        #endregion
        #region btnS7F19_Click
        private void btnS7F19_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS7F19();

                RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S7F19 : Result={0}", driverResult));
            }
            else
            {
                MessageBox.Show("Send S7F19 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS7F23_Click
        private void btnS7F23_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;
            FormattedProcessProgramInfo info;
            string ppid;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (cboFmtPPS7F23.SelectedValue != null && string.IsNullOrEmpty(cboFmtPPS7F23.SelectedValue.ToString()) == false)
                {
                    ppid = cboFmtPPS7F23.SelectedValue.ToString();

                    info = this._messageProcessor.FormattedProcessProgramCollection.Items.FirstOrDefault(t => t.PPID == ppid);

                    if (info != null)
                    {
                        driverResult = this._messageProcessor.SendS7F23(ppid);

                        RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S7F23 : Result={0}", driverResult));
                    }
                    else
                    {
                        RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F23 Fail : can not find ppid : {0} ", ppid));
                    }
                }
                else
                {
                    RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F23 Fail : ppid is not selected"));
                }
            }
            else
            {
                MessageBox.Show("Send S7F23 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS7F25_Click
        private void btnS7F25_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (string.IsNullOrEmpty(this._messageProcessor.CurrentSetting.ProcessProgramIDS7F25) == false)
                {
                    driverResult = this._messageProcessor.SendS7F25(this._messageProcessor.CurrentSetting.ProcessProgramIDS7F25);

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S7F25 : Result={0}", driverResult));
                }
                else
                {
                    RaiseWriteLog(MessageProcessor.DriverLogType.WARN, string.Format("Send S7F25 Fail : ppid is empty "));
                }
            }
            else
            {
                MessageBox.Show("Send S7F25 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS7F25Edit_Click
        private void btnS7F25Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingFormattedProcessProgram popup;

            popup = new SettingFormattedProcessProgram();
            popup.Initialize(this._messageProcessor, SettingFormattedProcessProgram.Type.S7F25);
            popup.OnPPIDChanged += Popup_OnPPIDChanged;
            popup.Owner = this._parentWindow;
            popup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popup.ShowDialog();
        }
        #endregion
        #region chkS7F23Ext_Click
        private void chkS7F23Ext_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.S7F23ExtChecked = chkS7F23Ext.IsChecked.Value;
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

        #region Popup_OnPPIDChanged
        private void Popup_OnPPIDChanged(SettingFormattedProcessProgram.Type type, string ppid)
        {
            switch (type)
            {
                case SettingFormattedProcessProgram.Type.S7F5:
                    this._messageProcessor.CurrentSetting.ProcessProgramIDS7F5 = ppid;
                    break;
                case SettingFormattedProcessProgram.Type.S7F25:
                    this._messageProcessor.CurrentSetting.ProcessProgramIDS7F25 = ppid;
                    break;
            }
            this._messageProcessor.IsDirty = true;
        }
        #endregion
        #region cboFmtPPS7F23_DropDownOpened
        private void cboFmtPPS7F23_DropDownOpened(object sender, EventArgs e)
        {
            InitializeComboBoxFmtPPS7F23();
        }
        #endregion
        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion
        #region InitializeComboBoxFmtPPS7F23
        public void InitializeComboBoxFmtPPS7F23()
        {
            if (this._messageProcessor != null)
            {
                cboFmtPPS7F23.ItemsSource = this._messageProcessor.FormattedProcessProgramCollection.Items.OrderBy(t => t.PPID).Select(t => t.PPID);
            }
        }
        #endregion
    }
}