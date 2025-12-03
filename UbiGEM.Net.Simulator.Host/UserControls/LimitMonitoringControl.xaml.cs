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
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LimitMonitoringControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);
        public WriteLog OnWriteLog { get; set; }
        #endregion

        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private List<ExpandedLimitMonitoringInfo> _monitroingCollection;
        private Window _parentWindow;
        #endregion

        #region Constructor
        public LimitMonitoringControl()
        {
            InitializeComponent();

            this._monitroingCollection = new List<ExpandedLimitMonitoringInfo>();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            InitializeLimitMonitoring();

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

        // Button Event
        #region btnTriggerEdit_Click
        private void btnTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Popup.Settings.TriggerWindow triggerWindow;
            ExpandedLimitMonitoringInfo expandedLimitMonitoringInfo;
            string variableInfo;

            expandedLimitMonitoringInfo = dgrLimitMonitoring.SelectedItem as ExpandedLimitMonitoringInfo;

            if (expandedLimitMonitoringInfo != null)
            {
                triggerWindow = new Popup.Settings.TriggerWindow();
                variableInfo = string.Format("{0}: {1}", expandedLimitMonitoringInfo.Variable.VID, expandedLimitMonitoringInfo.Variable.Name);
                triggerWindow.Initialize(this._messageProcessor, "Limit Monitoring", variableInfo, expandedLimitMonitoringInfo.TriggerCollection);
                triggerWindow.Owner = this._parentWindow;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
            }
        }
        #endregion
        #region btnLimitMonitoringEdit_Click
        private void btnLimitMonitoringEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            ExpandedLimitMonitoringInfo monitoringInfo;
            Popup.ValueEdit.LimitMonitoringValueEditWindow editWindow;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedLimitMonitoringInfo)
                {
                    monitoringInfo = btn.DataContext as ExpandedLimitMonitoringInfo;
                    editWindow = new Popup.ValueEdit.LimitMonitoringValueEditWindow();
                    editWindow.Initialize(this._messageProcessor, monitoringInfo);
                    
                    if (this._parentWindow != null)
                    {
                        editWindow.Owner = this._parentWindow;
                        editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    }
                    else
                    {
                        editWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }

                    editWindow.ShowDialog();
                    InitializeLimitMonitoring();
                }
            }
        }
        #endregion
        #region btnLimitMonitoringSend_Click
        private void btnLimitMonitoringSend_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            ExpandedLimitMonitoringInfo monitoringInfo;
            MessageError error;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedLimitMonitoringInfo)
                {
                    monitoringInfo = btn.DataContext as ExpandedLimitMonitoringInfo;

                    if (this._messageProcessor.HSMSDriverConnected == true)
                    {
                        if (monitoringInfo != null)
                        {
                            error = this._messageProcessor.SendS2F45(monitoringInfo);

                            this.RaiseWriteLog(MessageProcessor.DriverLogType.INFO, string.Format("Send S2F45: Trace ID: {0}: Result: {1}", monitoringInfo.Variable, error));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Send S2F45 Failed. Disconnected");
                    }
                }
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
        #region InitializeLimitMonitoring
        public void InitializeLimitMonitoring()
        {
            dgrLimitMonitoring.CancelEdit();

            if (this._messageProcessor != null)
            {
                dgrLimitMonitoring.ItemsSource = this._messageProcessor.LimitMonitoringCollection.Items;
            }

            dgrLimitMonitoring.Items.Refresh();
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
    }
}
