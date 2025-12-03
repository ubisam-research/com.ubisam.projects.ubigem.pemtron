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
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;
using UbiGEM.Net.Simulator.Host.Info;

namespace UbiGEM.Net.Simulator.Host.Popup.Settings
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TriggerWindow : Window
    {
        #region MemberVariable
        private string _type;
        private string _name;
        private AutoSendTriggerCollection _currentTriggerCollection;
        private ObservableCollection<AutoSendTrigger> _displayInfo;
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public TriggerWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<TriggerMode> triggerModeList;

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

            if (string.IsNullOrEmpty(this._type) == true || string.IsNullOrEmpty(this._name) == true || this._messageProcessor == null || this._currentTriggerCollection == null)
            {
                MessageBox.Show("Trigger Window initializing is not correst");
                Close();
            }
            else
            {
                if (this._type.StartsWith("Trace") == true)
                {
                    lblNameText.Content = "Trace ID";
                }
                else if (this._type == "Fmt Process Program")
                {
                    lblNameText.Content = "PPID";
                }

                lblType.Content = this._type.Replace("_", "__");
                lblName.Content = this._name.Replace("_", "__");

                triggerModeList = new List<TriggerMode>();
                triggerModeList.AddRange(Enum.GetValues(typeof(TriggerMode)).Cast<TriggerMode>());
                dgcMode.ItemsSource = triggerModeList;

                MakeComboBoxItemsForCE();

                this._displayInfo = new ObservableCollection<AutoSendTrigger>();

                foreach (AutoSendTrigger trigger in this._currentTriggerCollection.Items)
                {
                    this._displayInfo.Add(trigger.Clone());
                }

                dgrTrigger.ItemsSource = this._displayInfo;
            }
        }
        #endregion

        // Button Event
        #region btnListEdit_Click
        private void btnListEdit_Click(object sender, RoutedEventArgs e)
        {
            AutoSendTrigger selectedTrigger;
            TriggerListWindow listWindow;

            selectedTrigger = dgrTrigger.SelectedItem as AutoSendTrigger;

            if (selectedTrigger != null && selectedTrigger.TriggerMode == TriggerMode.Variable && selectedTrigger.ReportInfo != null && selectedTrigger.VariableStack.Count > 0 && selectedTrigger.VariableStack[0].Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                listWindow = new TriggerListWindow();
                listWindow.Initialize(this._messageProcessor, selectedTrigger);
                listWindow.Owner = this;
                listWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                listWindow.ShowDialog();
            }
        }
        #endregion
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;

            List<int> usedTriggerIDs;

            errorText = string.Empty;
            usedTriggerIDs = new List<int>();
            foreach (var trigger in this._displayInfo)
            {
                if (string.IsNullOrEmpty(errorText) == true && trigger.TriggerID < 1)
                {
                    errorText = string.Format(" Invalid Trigger. \r\n Trigger ID: {0} \r\n Reson: Trigger ID must unsiged", trigger.TriggerID);
                }

                if (string.IsNullOrEmpty(errorText) == true && trigger.ValidateTrigger(out errorText) == false)
                {
                    errorText = string.Format(" Invalid Trigger. \r\n Trigger ID: {0} \r\n Reson: {1}", trigger.TriggerID, errorText);
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (usedTriggerIDs.Contains(trigger.TriggerID) == true)
                    {
                        errorText = string.Format(" Invalid Trigger. \r\n Trigger ID: {0} \r\n Reson: Duplicated ID", trigger.TriggerID);
                    }
                    else
                    {
                        usedTriggerIDs.Add(trigger.TriggerID);
                    }
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                this._currentTriggerCollection.Items.Clear();

                foreach (var trigger in this._displayInfo)
                {
                    this._currentTriggerCollection.Add(trigger);
                }

                this._messageProcessor.IsDirty = true;
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

        // UserControl Event
        #region Trigger_OnAdd
        private void Trigger_OnAdd(object sender, EventArgs e)
        {
            AutoSendTrigger trigger;

            trigger = new AutoSendTrigger()
            {
                TriggerID = -1,
                TriggerMode = TriggerMode.CollectionEvent,
            };

            this._displayInfo.Add(trigger);
            dgrTrigger.SelectedItem = trigger;
        }
        #endregion
        #region Trigger_OnRemove
        private void Trigger_OnRemove(object sender, EventArgs e)
        {
            AutoSendTrigger trigger;

            trigger = dgrTrigger.SelectedItem as AutoSendTrigger;

            if (trigger != null)
            {
                this._displayInfo.Remove(trigger);
            }
        }
        #endregion

        // DataGrid Event
        #region dgcVariable_SelectionChanged
        private void dgcVariable_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox combobox;
            ExpandedVariableInfo selectedVariable;
            AutoSendTrigger selectedTrigger;

            if (e.Source != null)
            {
                combobox = e.Source as ComboBox;
                selectedTrigger = dgrTrigger.SelectedItem as AutoSendTrigger;

                if (combobox != null && combobox.SelectedItem != null && selectedTrigger != null)
                {
                    selectedVariable = combobox.SelectedItem as ExpandedVariableInfo;

                    selectedTrigger.VariableStack.Clear();
                    selectedTrigger.VariableStack.Add(selectedVariable);

                    if (selectedVariable.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                    {
                        dgcValue.IsReadOnly = true;
                    }
                    else
                    {
                        dgcValue.IsReadOnly = false;
                    }
                }
            }
        }
        #endregion
        #region dgrTrigger_SelectionChanged
        private void dgrTrigger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutoSendTrigger selectedTrigger;

            if (dgrTrigger.SelectedItem != null)
            {
                selectedTrigger = dgrTrigger.SelectedItem as AutoSendTrigger;

                if (selectedTrigger != null)
                {
                    switch (selectedTrigger.TriggerMode)
                    {
                        case TriggerMode.NotUse:
                            dgcCE.IsReadOnly = true;
                            dgcReport.IsReadOnly = true;
                            dgcVariable.IsReadOnly = true;
                            dgcValue.IsReadOnly = true;
                            break;
                        case TriggerMode.CollectionEvent:
                            dgcCE.IsReadOnly = false;
                            dgcReport.IsReadOnly = true;
                            dgcVariable.IsReadOnly = true;
                            dgcValue.IsReadOnly = true;
                            break;
                        case TriggerMode.Variable:
                            dgcCE.IsReadOnly = false;
                            dgcReport.IsReadOnly = false;
                            dgcVariable.IsReadOnly = false;
                            dgcValue.IsReadOnly = false;

                            if(selectedTrigger.VariableStack.Count > 0 && selectedTrigger.VariableStack[0].Format == UbiCom.Net.Structure.SECSItemFormat.L)
                            {
                                dgcValue.IsReadOnly = true;
                            }

                            break;
                    }
                }
            }
        }
        #endregion
        #region dgcMode_SelectionChanged
        private void dgcMode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AutoSendTrigger selectedTrigger;
            TriggerMode selectedMode;
            ComboBox comboBox;

            selectedTrigger = dgrTrigger.SelectedItem as AutoSendTrigger;

            if (e.Source != null)
            {
                comboBox = e.Source as ComboBox;

                if (comboBox.SelectedItem != null && Enum.TryParse(comboBox.SelectedItem.ToString(), out selectedMode) == true)
                {
                    if (selectedTrigger != null)
                    {
                        selectedTrigger.TriggerMode = selectedMode;
                    }

                    switch (selectedMode)
                    {
                        case TriggerMode.NotUse:
                            dgcCE.IsReadOnly = true;
                            dgcReport.IsReadOnly = true;
                            dgcVariable.IsReadOnly = true;
                            dgcValue.IsReadOnly = true;
                            break;
                        case TriggerMode.CollectionEvent:
                            dgcCE.IsReadOnly = false;
                            dgcReport.IsReadOnly = true;
                            dgcVariable.IsReadOnly = true;
                            dgcValue.IsReadOnly = true;
                            break;
                        case TriggerMode.Variable:
                            dgcCE.IsReadOnly = false;
                            dgcReport.IsReadOnly = false;
                            dgcVariable.IsReadOnly = false;
                            dgcValue.IsReadOnly = false;
                            break;
                    }

                }
            }
        }
        #endregion
        #region dgcCE_SelectionChanged
        private void dgcCE_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AutoSendTrigger selectedTrigger;
            ExpandedCollectionEventInfo selectedCE;
            ComboBox comboBox;

            if (e.Source != null)
            {
                selectedTrigger = dgrTrigger.SelectedItem as AutoSendTrigger;
                comboBox = e.Source as ComboBox;
                selectedCE = comboBox.SelectedItem as ExpandedCollectionEventInfo;

                if (selectedCE != null && selectedTrigger != null && selectedTrigger.CollectionEventInfo != selectedCE)
                {
                    selectedTrigger.CollectionEventInfo = selectedCE;
                    selectedTrigger.ReportInfo = null;
                    selectedTrigger.VariableStack.Clear();
                    selectedTrigger.VariableValue = string.Empty;
                }
            }
        }
        #endregion
        #region dgcReport_SelectionChanged
        private void dgcReport_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AutoSendTrigger selectedTrigger;
            ExpandedReportInfo selectedReport;
            ComboBox comboBox;

            if (e.Source != null)
            {
                selectedTrigger = dgrTrigger.SelectedItem as AutoSendTrigger;
                comboBox = e.Source as ComboBox;
                selectedReport = comboBox.SelectedItem as ExpandedReportInfo;

                if (selectedReport != null && selectedTrigger != null && selectedTrigger.ReportInfo != selectedReport)
                {
                    selectedTrigger.ReportInfo = selectedReport;
                    selectedTrigger.VariableStack.Clear();
                    selectedTrigger.VariableValue = string.Empty;
                }
            }
        }
        #endregion
        #region dgrTrigger_MouseDoubleClick
        private void dgrTrigger_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AutoSendTrigger selectedTrigger;

            selectedTrigger = dgrTrigger.SelectedItem as AutoSendTrigger;

            if (selectedTrigger != null)
            {
                btnListEdit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
        #endregion
        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, string type, string name, AutoSendTriggerCollection currentTriggers)
        {
            this._messageProcessor = messageProcessor;
            this._type = type;
            this._name = name;

            this._currentTriggerCollection = currentTriggers;

        }
        #endregion

        // Private Method
        #region MakeComboBoxItemsForCE
        private void MakeComboBoxItemsForCE()
        {
            ExpandedCollectionEventInfo exCEInfo;
            List<ExpandedCollectionEventInfo> ceList;

            ceList = new List<ExpandedCollectionEventInfo>();

            foreach (var ce in this._messageProcessor.CollectionEventCollection.Items.Values.OrderBy(t => t.CEID))
            {
                exCEInfo = ce as ExpandedCollectionEventInfo;
                ceList.Add(exCEInfo);
            }
            dgcCE.ItemsSource = ceList;
        }
        #endregion
    }
}