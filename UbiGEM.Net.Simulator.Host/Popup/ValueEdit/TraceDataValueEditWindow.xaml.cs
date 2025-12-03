using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup.ValueEdit
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TraceDataValueEditWindow : Window
    {
        #region MemberVariable
        private ExpandedTraceInfo _traceInfo;
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public TraceDataValueEditWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<ExpandedVariableInfo> variableCollection;
            ExpandedVariableInfo expandedVariableInfo;

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

            if (this._traceInfo == null)
            {
                MessageBox.Show("traceInfo is not initialized");
                Close();
            }
            else
            {
                lblID.Content = this._traceInfo.TraceID;
                txtPeriod.Text = this._traceInfo.Dsper;
                txtTotalNumber.Text = this._traceInfo.TotalSample.ToString();
                txtGroupSize.Text = this._traceInfo.ReportGroupSize.ToString();

                variableCollection = new List<ExpandedVariableInfo>();

                foreach (var vid in this._traceInfo.Variables)
                {
                    expandedVariableInfo = this._messageProcessor.VariableCollection[vid] as ExpandedVariableInfo;

                    if (expandedVariableInfo != null)
                    {
                        variableCollection.Add(expandedVariableInfo);
                    }
                }

                dgrVariables.ItemsSource = variableCollection;
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            Structure.GemDriverError error;

            string dsper;
            int dsperLen;
            int intValue;
            string totalNumber;
            string groupSize;
            
            errorText = string.Empty;

            dsper = txtPeriod.Text.Trim();
            dsperLen = dsper.Length;

            if (int.TryParse(dsper, out intValue) == false)
            {
                errorText = string.Format(" DSPER must be integer");
            }
            else
            {
                this._traceInfo.Dsper = dsper;
            }

            totalNumber = txtTotalNumber.Text.Trim();

            if (string.IsNullOrEmpty(errorText) == true)
            {
                if (int.TryParse(totalNumber, out intValue) == false)
                {
                    errorText = "TOTSMP must be integer";
                }
                else
                {
                    this._traceInfo.TotalSample = intValue;
                }
            }

            groupSize = txtGroupSize.Text.Trim();
            if (string.IsNullOrEmpty(errorText) == true)
            {
                if (int.TryParse(groupSize, out intValue) == false)
                {
                    errorText = "REPGSZ must be integer.";
                }
                else
                {
                    this._traceInfo.ReportGroupSize = intValue;
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
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
        #endregion
        #region btnCancel_Click
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, ExpandedTraceInfo traceInfo)
        {
            this._messageProcessor = messageProcessor;
            this._traceInfo = traceInfo;
        }
        #endregion

        // Private Method
    }
}