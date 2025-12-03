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
    public partial class LimitMonitoringValueEditWindow : Window
    {
        #region MemberVariable
        private ExpandedLimitMonitoringInfo _monitoringInfo;

        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public LimitMonitoringValueEditWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string variableName;

            variableName = string.Empty;

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

            if (this._monitoringInfo == null)
            {
                MessageBox.Show("monitoringInfo is not initialized");
                Close();
            }
            else
            {
                lblVID.Content = this._monitoringInfo.Variable.VID;
                lblVariableName.Content = this._monitoringInfo.Variable.Name.Replace("_", "__");

                dgrLimitItem.ItemsSource = this._monitoringInfo.Items;
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            Structure.GemDriverError error;

            List<byte> usedLimitIDs;
            int upper;
            int lower;
            ExpandedLimitMonitoringInfo limitMonitoringInfo;

            errorText = string.Empty;
            usedLimitIDs = new List<byte>();

            if (string.IsNullOrEmpty(errorText) == true)
            {
                foreach (var item in this._monitoringInfo.Items)
                {
                    if (usedLimitIDs.Contains(item.LimitID) == true)
                    {
                        errorText = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n dupelicated", _monitoringInfo.Variable.VID, item.LimitID);
                    }
                    else
                    {
                        usedLimitIDs.Add(item.LimitID);

                        if (int.TryParse(item.Upper, out upper) == false)
                        {
                            errorText = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n UPPERDB must be integer", _monitoringInfo.Variable.VID, item.LimitID);
                        }
                        else
                        {
                            if (int.TryParse(item.Lower, out lower) == false)
                            {
                                errorText = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n LOWERRDB must be integer", _monitoringInfo.Variable.VID, item.LimitID);
                            }
                            else
                            {
                                if (upper < lower)
                                {
                                    errorText = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n LOWERRDB is bigger than UPPERDB", _monitoringInfo.Variable.VID, item.LimitID);
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(errorText) == false)
                    {
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                limitMonitoringInfo = this._messageProcessor.LimitMonitoringCollection.Items.FirstOrDefault(t => t.Variable.VID == this._monitoringInfo.Variable.VID);

                if (limitMonitoringInfo != null)
                {
                    limitMonitoringInfo.Items.Clear();
                    limitMonitoringInfo.Items.AddRange(this._monitoringInfo.Items);
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
        public void Initialize(MessageProcessor messageProcessor, ExpandedLimitMonitoringInfo monitoringInfo)
        {
            this._messageProcessor = messageProcessor;

            if (monitoringInfo != null)
            {
                this._monitoringInfo = monitoringInfo.Clone();
            }
        }
        #endregion
    }
}