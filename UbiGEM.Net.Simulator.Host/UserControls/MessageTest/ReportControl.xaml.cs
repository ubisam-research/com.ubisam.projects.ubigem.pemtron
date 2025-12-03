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
    public partial class ReportControl : UserControl
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
        public ReportControl()
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

            IniItializeComboBoxReport();

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

        // ComboBox Event
        #region cboReport_DropDownOpened
        private void cboReport_DropDownOpened(object sender, EventArgs e)
        {
            IniItializeComboBoxReport();
        }
        #endregion

        // Button Event
        #region btnS6F19_Click
        private void btnS6F19_Click(object sender, RoutedEventArgs e)
        {
            string report;
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                report = cboReport.Text.Trim();
                if (string.IsNullOrEmpty(report) == false)
                {
                    driverResult = this._messageProcessor.SendS6F19(report);

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S6F19 : Result={0}", driverResult));
                }
                else
                {
                    MessageBox.Show("Send S6F19 : Report ID is empty");
                }
            }
            else
            {
                MessageBox.Show("Send S6F19 Failed - Disconnected");
            }
        }
        #endregion

        // Private Method
        #region IniItializeComboBoxReport
        private void IniItializeComboBoxReport()
        {
            if (this._messageProcessor != null)
            {
                cboReport.ItemsSource = this._messageProcessor.ReportCollection.Items.OrderBy(t => t.Key).Select(t => string.Format("{0} : {1}", t.Value.ReportID, t.Value.Description));
            }
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
