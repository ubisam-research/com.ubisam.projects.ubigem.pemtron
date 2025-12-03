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
    public partial class TraceDataControl : UserControl
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
        public TraceDataControl()
        {
            InitializeComponent();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            InitializeTraceData();

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
        #region btnTraceDataEdit_Click
        private void btnTraceDataEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            ExpandedTraceInfo traceInfo;
            Popup.ValueEdit.TraceDataValueEditWindow editWindow;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedTraceInfo)
                {
                    traceInfo = btn.DataContext as ExpandedTraceInfo;
                    editWindow = new Popup.ValueEdit.TraceDataValueEditWindow();
                    editWindow.Initialize(this._messageProcessor, traceInfo);
                    
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
                }
            }
        }
        #endregion
        #region btnSendTriggerEdit_Click
        private void btnSendTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Popup.Settings.TriggerWindow triggerWindow;
            ExpandedTraceInfo expandedTraceInfo;

            expandedTraceInfo = dgrTraceData.SelectedItem as ExpandedTraceInfo;

            if (expandedTraceInfo != null)
            {
                triggerWindow = new Popup.Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Trace Send", expandedTraceInfo.TraceID, expandedTraceInfo.SendTriggerCollection);
                triggerWindow.Owner = this._parentWindow;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
            }
        }
        #endregion
        #region btnTraceDataSend_Click
        private void btnTraceDataSend_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            ExpandedTraceInfo traceInfo;
            MessageError error;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedTraceInfo)
                {
                    traceInfo = btn.DataContext as ExpandedTraceInfo;

                    if (this._messageProcessor.HSMSDriverConnected == true)
                    {
                        error = this._messageProcessor.SendS2F23(traceInfo);

                        RaiseWriteLog(MessageProcessor.DriverLogType.INFO, string.Format("Send S2F23: Trace ID: {0}: Result: {1}", traceInfo.TraceID, error));
                    }
                    else
                    {
                        MessageBox.Show("Send S2F23 Failed. Disconnected");
                    }
                }
            }
        }
        #endregion
        #region btnTraceDataStop_Click
        private void btnTraceDataStop_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            ExpandedTraceInfo traceInfo;
            MessageError error;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedTraceInfo)
                {
                    traceInfo = btn.DataContext as ExpandedTraceInfo;

                    if (this._messageProcessor.HSMSDriverConnected == true)
                    {
                        error = this._messageProcessor.SendS2F23Stop(traceInfo);

                        RaiseWriteLog(MessageProcessor.DriverLogType.INFO, string.Format("Send S2F23: Trace ID: {0}: Result: {1}", traceInfo.TraceID, error));
                    }
                    else
                    {
                        MessageBox.Show("Send S2F23 Failed. Disconnected");
                    }
                }
            }
        }
        #endregion
        #region btnStopTriggerEdit_Click
        private void btnStopTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Popup.Settings.TriggerWindow triggerWindow;
            ExpandedTraceInfo expandedTraceInfo;

            expandedTraceInfo = dgrTraceData.SelectedItem as ExpandedTraceInfo;

            if (expandedTraceInfo != null)
            {
                triggerWindow = new Popup.Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Trace Stop", expandedTraceInfo.TraceID, expandedTraceInfo.StopTriggerCollection);
                triggerWindow.Owner = this._parentWindow;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
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
        #region InitializeTraceData
        public void InitializeTraceData()
        {
            dgrTraceData.CancelEdit();

            if (this._messageProcessor != null)
            {
                dgrTraceData.ItemsSource = this._messageProcessor.TraceCollection.Items;
            }

            dgrTraceData.Items.Refresh();
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
