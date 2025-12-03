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
using System.ComponentModel;

namespace UbiGEM.Net.Simulator.Host.Popup.Settings
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TriggerListWindow : Window
    {
        #region DisplayData
        public class DisplayData
        {
            #region Property
            public ExpandedVariableInfo SelectedVariableInfo { get; set; }
            public List<ExpandedVariableInfo> CandidateVariables { get; set; }
            #endregion
            #region Constructor
            public DisplayData()
            {
                this.CandidateVariables = new List<ExpandedVariableInfo>();
            }
            #endregion
        }
        #endregion
        #region MemberVariable
        private AutoSendTrigger _selectedTrigger;
        private ObservableCollection<DisplayData> _variableStack;
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public TriggerListWindow()
        {
            InitializeComponent();

            this._selectedTrigger = null;
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayData displayData;
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

            if (this._messageProcessor == null || this._selectedTrigger == null || this._selectedTrigger.ReportInfo == null || this._selectedTrigger.VariableStack.Count == 0)
            {
                MessageBox.Show("Trigger List Window initializing is not corret");
                Close();
            }
            else
            {
                this._variableStack = new ObservableCollection<DisplayData>();

                lblTriggerID.Content = this._selectedTrigger.TriggerID;
                lblTriggerMode.Content = this._selectedTrigger.TriggerMode.ToString().Replace("_", "__");
                lblCE.Content = this._selectedTrigger.CollectionEventInfo.DisplayStringForComboBox.Replace("_", "__");
                lblReport.Content = this._selectedTrigger.ReportInfo.DisplayStringForComboBox.Replace("_", "__");
                txtValue.Text = this._selectedTrigger.VariableValue;

                dgrVariable.ItemsSource = this._variableStack;

                // add varaibles of reportinfo
                displayData = new DisplayData();

                foreach (var child in this._selectedTrigger.ReportInfo.Variables.Items)
                {
                    expandedVariableInfo = child as ExpandedVariableInfo;
                    displayData.CandidateVariables.Add(expandedVariableInfo.Clone());
                }

                this._variableStack.Add(displayData);

                // add selected variable stack
                foreach (var variable in this._selectedTrigger.VariableStack)
                {
                    displayData.SelectedVariableInfo = variable;

                    if (variable.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                    {
                        displayData = new DisplayData();

                        foreach (var child in variable.ChildVariables)
                        {
                            displayData.CandidateVariables.Add(child.Clone());
                        }

                        this._variableStack.Add(displayData);
                    }
                }
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            ExpandedVariableInfo lastSelectedVariableInfo;
            int variableStackCount;

            errorText = string.Empty;
            lastSelectedVariableInfo = null;

            variableStackCount = this._variableStack.Count;

            for (int i = variableStackCount - 1; i >= 0; i--)
            {
                lastSelectedVariableInfo = this._variableStack.ElementAt(i).SelectedVariableInfo;

                if (lastSelectedVariableInfo != null)
                {
                    break;
                }
            }

            if (lastSelectedVariableInfo != null)
            {
                if (lastSelectedVariableInfo.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    errorText = "invalid last variable(List Type)";
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                this._selectedTrigger.VariableStack.Clear();

                foreach (var variable in this._variableStack)
                {
                    this._selectedTrigger.VariableStack.Add(variable.SelectedVariableInfo);
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

        // DataGrid Event
        #region dgcVariable_SelectionChanged
        private void dgcVariable_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox combobox;
            SelectionChangedEventArgs selectionChangedEventArgs;
            ExpandedVariableInfo selectedVariable;
            DisplayData displayData;
            int dgrVariableSelectedIndex;
            int dgrVariableCount;
            string errorText;

            errorText = string.Empty;

            if (e.Source != null)
            {
                selectionChangedEventArgs = e as SelectionChangedEventArgs;
                combobox = e.Source as ComboBox;
                dgrVariableSelectedIndex = dgrVariable.SelectedIndex;

                if (combobox != null && combobox.SelectedItem != null && selectionChangedEventArgs.RemovedItems.Count > 0 && dgrVariableSelectedIndex >= 0)
                {
                    dgrVariableCount = this._variableStack.Count;

                    for (int i = dgrVariableCount - 1; i > dgrVariableSelectedIndex; i--)
                    {
                        this._variableStack.RemoveAt(i);
                    }

                    selectedVariable = combobox.SelectedItem as ExpandedVariableInfo;

                    if (selectedVariable != null)
                    {
                        displayData = dgrVariable.SelectedItem as DisplayData;
                        displayData.SelectedVariableInfo = selectedVariable;

                        if (selectedVariable.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                        {
                            if (selectedVariable.Length == 0)
                            {
                                errorText = "invalid variable: no children";

                                displayData.SelectedVariableInfo = selectionChangedEventArgs.RemovedItems[0] as ExpandedVariableInfo;
                            }
                            else
                            {
                                displayData = new DisplayData();

                                foreach (var child in selectedVariable.ChildVariables)
                                {
                                    displayData.CandidateVariables.Add(child.Clone());
                                }

                                this._variableStack.Add(displayData);
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, AutoSendTrigger selectedTrigger)
        {
            this._messageProcessor = messageProcessor;

            this._selectedTrigger = selectedTrigger;
        }
        #endregion
    }
}