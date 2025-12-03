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

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingLimitMonitoringWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LimitMonitoringWindow : Window
    {
        #region MemberVarialbe
        private ObservableCollection<ExpandedVariableInfo> _variableItems;
        private ObservableCollection<ExpandedLimitMonitoringItem> _displayLimitItems;
        private ObservableCollection<ExpandedLimitMonitoringInfo> _displayInfo;

        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public LimitMonitoringWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

            this._displayInfo = new ObservableCollection<ExpandedLimitMonitoringInfo>();

            foreach (var item in this._messageProcessor.LimitMonitoringCollection.Clone().Items)
            {
                this._displayInfo.Add(item);
            }

            this._variableItems = new ObservableCollection<ExpandedVariableInfo>();

            foreach (var item in this._messageProcessor.VariableCollection.Variables.Items.Where(t => t.VIDType == Structure.VariableType.SV).OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;

                if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                {
                    this._variableItems.Add(expandedVariableInfo);
                }
            }

            foreach (var item in this._messageProcessor.VariableCollection.Variables.Items.Where(t => t.VIDType == Structure.VariableType.DVVAL).OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;

                if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                {
                    this._variableItems.Add(expandedVariableInfo);
                }
            }

            dgrSelected.ItemsSource = this._displayInfo;
            dgrVariable.ItemsSource = this._variableItems;
        }
        #endregion

        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            ExpandedLimitMonitoringInfo limitMonitoringInfo;
            ExpandedLimitMonitoringItem limitMonitoringItem;

            if (dgrSelected.SelectedItem != null)
            {
                limitMonitoringInfo = dgrSelected.SelectedItem as ExpandedLimitMonitoringInfo;

                limitMonitoringItem = new ExpandedLimitMonitoringItem();

                limitMonitoringInfo.Add(limitMonitoringItem);
                this._displayLimitItems.Add(limitMonitoringItem);
                dgrLimitInfo.SelectedItem = limitMonitoringItem;
                dgrLimitInfo.ScrollIntoView(limitMonitoringItem);
            }
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            ExpandedLimitMonitoringInfo limitMonitoringInfo;
            ExpandedLimitMonitoringItem limitMonitoringItem;

            limitMonitoringInfo = dgrSelected.SelectedItem as ExpandedLimitMonitoringInfo;
            limitMonitoringItem = dgrLimitInfo.SelectedItem as ExpandedLimitMonitoringItem;

            if (limitMonitoringInfo != null && limitMonitoringItem != null)
            {
                limitMonitoringInfo.Remove(limitMonitoringItem);
                this._displayLimitItems.Remove(limitMonitoringItem);
            }
        }
        #endregion
        #region dgrSelected_SelectionChanged
        private void dgrSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedLimitMonitoringInfo limitMonitoringInfo;

            limitMonitoringInfo = dgrSelected.SelectedItem as ExpandedLimitMonitoringInfo;

            if (limitMonitoringInfo != null)
            {
                this._displayLimitItems = new ObservableCollection<ExpandedLimitMonitoringItem>(limitMonitoringInfo.Items);
                dgrLimitInfo.ItemsSource = this._displayLimitItems;
            }
        }
        #endregion

        // UserControl: Select, Unselect Event
        #region UCSelectUnSelectButton_OnSelect
        private void UCSelectUnSelectButton_OnSelect(object sender, EventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            ExpandedLimitMonitoringInfo limitMonitoringInfo;

            variableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null && this._displayInfo.FirstOrDefault(t => t.Variable.VID == variableInfo.VID) == null)
            {
                limitMonitoringInfo = new ExpandedLimitMonitoringInfo()
                {
                    Variable = variableInfo,
                };

                this._displayInfo.Add(limitMonitoringInfo);
                dgrSelected.SelectedItem = limitMonitoringInfo;
                dgrSelected.ScrollIntoView(limitMonitoringInfo);

                this._displayLimitItems = new ObservableCollection<ExpandedLimitMonitoringItem>(limitMonitoringInfo.Items);
                dgrLimitInfo.ItemsSource = this._displayLimitItems;
            }
        }
        #endregion
        #region UCSelectUnSelectButton_OnUnselect
        private void UCSelectUnSelectButton_OnUnselect(object sender, EventArgs e)
        {
            ExpandedLimitMonitoringInfo limitMonitoringInfo;

            limitMonitoringInfo = dgrSelected.SelectedItem as ExpandedLimitMonitoringInfo;

            if (limitMonitoringInfo != null)
            {
                this._displayInfo.Remove(limitMonitoringInfo);
            }
        }
        #endregion

        // Button Event
        #region btnTriggerEdit_Click
        private void btnTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Settings.TriggerWindow triggerWindow;
            ExpandedLimitMonitoringInfo expandedLimitMonitoringInfo;
            string variableInfo;

            expandedLimitMonitoringInfo = dgrSelected.SelectedItem as ExpandedLimitMonitoringInfo;

            if (expandedLimitMonitoringInfo != null)
            {
                triggerWindow = new Settings.TriggerWindow();
                variableInfo = string.Format("{0}: {1}", expandedLimitMonitoringInfo.Variable.VID, expandedLimitMonitoringInfo.Variable.Name);
                triggerWindow.Initialize(this._messageProcessor, "Limit Monitoring", variableInfo, expandedLimitMonitoringInfo.TriggerCollection);
                triggerWindow.Owner = this;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
            }
        }
        #endregion
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            Structure.GemDriverError error;


            int upper;
            int lower;
            string errorMsg;
            List<byte> usedLimitIDs;

            errorMsg = string.Empty;
            usedLimitIDs = new List<byte>();

            foreach (var tempVariableInfo in this._displayInfo)
            {
                if (tempVariableInfo.AutoSend == true)
                {
                    if (tempVariableInfo.TriggerCollection.Items.Count == 0)
                    {
                        errorMsg = string.Format(" VID: {0} \n\n AutoSend is selected, but Trigger is not selected", tempVariableInfo.Variable.VID);
                        break;
                    }
                }

                usedLimitIDs.Clear();

                foreach (var item in tempVariableInfo.Items)
                {
                    if (usedLimitIDs.Contains(item.LimitID) == true)
                    {
                        errorMsg = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n dupelicated", tempVariableInfo.Variable.VID, item.LimitID);
                    }
                    else
                    {
                        usedLimitIDs.Add(item.LimitID);

                        if (int.TryParse(item.Upper, out upper) == false)
                        {
                            errorMsg = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n UPPERDB must be integer", tempVariableInfo.Variable.VID, item.LimitID);
                        }
                        else
                        {
                            if (int.TryParse(item.Lower, out lower) == false)
                            {
                                errorMsg = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n LOWERRDB must be integer", tempVariableInfo.Variable.VID, item.LimitID);
                            }
                            else
                            {
                                if (upper < lower)
                                {
                                    errorMsg = string.Format(" VID: {0} \n\n LIMITID: {1} \n\n LOWERRDB is bigger than UPPERDB", tempVariableInfo.Variable.VID, item.LimitID);
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(errorMsg) == false)
                    {
                        break;
                    }
                }

                if (string.IsNullOrEmpty(errorMsg) == false)
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(errorMsg) == false)
            {
                MessageBox.Show(errorMsg);
            }
            else
            {
                this._messageProcessor.LimitMonitoringCollection.Items.Clear();

                foreach (ExpandedLimitMonitoringInfo tempVariableInfo in this._displayInfo)
                {
                    this._messageProcessor.LimitMonitoringCollection.Items.Add(tempVariableInfo);
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
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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