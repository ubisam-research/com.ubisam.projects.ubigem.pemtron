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

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StandaloneTestControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);
        public WriteLog OnWriteLog { get; set; }
        #endregion
        #region MemberVariables
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public StandaloneTestControl()
        {
            InitializeComponent();

            ctlConnection.OnWriteLog += ctl_WriteLog;
            ctlControlState.OnWriteLog += ctl_WriteLog;
            ctlDefineEventReport.OnWriteLog += ctl_WriteLog;
            ctlVariableData.OnWriteLog += ctl_WriteLog;
            ctlReport.OnWriteLog += ctl_WriteLog;
            ctlCollectionEvent.OnWriteLog += ctl_WriteLog;
            ctlAlarm.OnWriteLog += ctl_WriteLog;
            ctlProcessProgram.OnWriteLog += ctl_WriteLog;
            ctlSpoolControl.OnWriteLog += ctl_WriteLog;
            cltETC.OnWriteLog += ctl_WriteLog;
            //ctlMapError.OnWriteLog += ctl_WriteLog;
            //ctlGEMObject.OnWriteLog += ctl_WriteLog;
        }
        #endregion
        #region ctl_WriteLog
        private void ctl_WriteLog(MessageProcessor.DriverLogType logType, string logText)
        {
            RaiseWriteLog(logType, logText);
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
            ctlConnection.Initialize(this._messageProcessor);
            ctlControlState.Initialize(this._messageProcessor);
            ctlDefineEventReport.Initialize(this._messageProcessor);
            ctlVariableData.Initialize(this._messageProcessor);
            ctlReport.Initialize(this._messageProcessor);
            ctlCollectionEvent.Initialize(this._messageProcessor);
            ctlAlarm.Initialize(this._messageProcessor);
            ctlProcessProgram.Initialize(this._messageProcessor);
            ctlSpoolControl.Initialize(this._messageProcessor);
            cltETC.Initialize(this._messageProcessor);
            ctlMapError.Initialize(this._messageProcessor);
            ctlGEMObject.Initialize(this._messageProcessor);
        }
        public void InitializeFmtPP()
        {
            ctlProcessProgram.InitializeComboBoxFmtPPS7F23();
        }
        public void InitializeGEMObject()
        {
            ctlGEMObject.InitializeGEMObject();
        }
        #endregion
    }
}
