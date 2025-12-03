using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingTraceDataWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TraceDataWindow : Window
    {
        #region MemberVariable
        private ObservableCollection<ExpandedVariableInfo> _displayVariableCollection;
        private ObservableCollection<ExpandedTraceInfo> _displayInfo;
        private ObservableCollection<ExpandedVariableInfo> _displayVariableInfo;

        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public TraceDataWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;
            ExpandedTraceInfo traceInfo;

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

            this._displayInfo = new ObservableCollection<ExpandedTraceInfo>();
            this._displayVariableCollection = new ObservableCollection<ExpandedVariableInfo>();

            if (this._messageProcessor != null)
            {
                foreach (var traceItem in this._messageProcessor.TraceCollection.Items.OrderBy(t => t.TraceID))
                {
                    traceInfo = traceItem.Clone();
                    this._displayInfo.Add(traceInfo);
                }

                foreach (var item in this._messageProcessor.VariableCollection.SV.Items.OrderBy(t => t.VID))
                {
                    expandedVariableInfo = item as ExpandedVariableInfo;

                    if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                    {
                        this._displayVariableCollection.Add(expandedVariableInfo);
                    }
                }

                foreach (var item in this._messageProcessor.VariableCollection.DVVal.Items.OrderBy(t => t.VID))
                {
                    expandedVariableInfo = item as ExpandedVariableInfo;

                    if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                    {
                        this._displayVariableCollection.Add(expandedVariableInfo);
                    }
                }

                foreach (var item in this._messageProcessor.VariableCollection.ECV.Items.OrderBy(t => t.VID))
                {
                    expandedVariableInfo = item as ExpandedVariableInfo;

                    if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                    {
                        this._displayVariableCollection.Add(expandedVariableInfo);
                    }
                }

            }

            dgrTrace.ItemsSource = this._displayInfo;
            dgrSelected.ItemsSource = null;
            dgrVariable.ItemsSource = this._displayVariableCollection;
        }
        #endregion

        // DataGridEvent
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;

            traceInfo = new ExpandedTraceInfo
            {
                TraceID = "-1"
            };

            this._displayInfo.Add(traceInfo);
            dgrTrace.SelectedItem = traceInfo;
            dgrTrace.ScrollIntoView(traceInfo);
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;

            traceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;

            if (traceInfo != null)
            {
                this._displayInfo.Remove(traceInfo);
            }
        }
        #endregion
        #region dgrTrace_SelectionChanged
        private void dgrTrace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedTraceInfo selectedItem;
            ExpandedVariableInfo variableInfo;

            selectedItem = dgrTrace.SelectedItem as ExpandedTraceInfo;

            if (selectedItem != null && this._messageProcessor != null)
            {
                this._displayVariableInfo = new ObservableCollection<ExpandedVariableInfo>();

                foreach (string tempVid in selectedItem.Variables)
                {
                    if (this._messageProcessor.VariableCollection.Items.FirstOrDefault(t => t.VID == tempVid) != null)
                    {
                        variableInfo = this._messageProcessor.VariableCollection[tempVid] as ExpandedVariableInfo;
                        this._displayVariableInfo.Add(variableInfo);
                    }
                }

                dgrSelected.ItemsSource = this._displayVariableInfo;
            }
        }
        #endregion

        // UserControl - Select, Unselect Event
        #region UCSelectUnSelectButton_OnSelect
        private void UCSelectUnSelectButton_OnSelect(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;
            ExpandedVariableInfo variableInfo;

            traceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;
            variableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

            if (traceInfo != null && variableInfo != null)
            {
                if (traceInfo.Variables.Contains(variableInfo.VID) == false)
                {
                    traceInfo.Variables.Add(variableInfo.VID);

                    this._displayVariableInfo.Add(variableInfo);
                    dgrSelected.SelectedItem = variableInfo;
                    dgrSelected.ScrollIntoView(variableInfo);
                }
            }
        }
        #endregion
        #region UCSelectUnSelectButton_OnUnselect
        private void UCSelectUnSelectButton_OnUnselect(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;
            ExpandedVariableInfo variableInfo;

            traceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (traceInfo != null && variableInfo != null)
            {
                traceInfo.Variables.Remove(variableInfo.VID);

                this._displayVariableInfo.Remove(variableInfo);
            }
        }
        #endregion

        // UserControl - Up, Down Event
        #region UCUpDownButton_OnFirst
        private void UCUpDownButton_OnFirst(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            traceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;
            selectedIndex = dgrSelected.SelectedIndex;

            if (traceInfo != null && variableInfo != null && selectedIndex > 0)
            {
                this._displayVariableInfo.Move(selectedIndex, 0);

                traceInfo.Variables.Remove(variableInfo.VID);
                traceInfo.Variables.Insert(0, variableInfo.VID);
            }
        }
        #endregion
        #region UCUpDownButton_OnUp
        private void UCUpDownButton_OnUp(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;
            int selectedIndex;

            traceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;
            selectedIndex = dgrSelected.SelectedIndex;

            if (traceInfo != null && selectedIndex > 0)
            {
                this._displayVariableInfo.Move(selectedIndex, selectedIndex - 1);
                traceInfo.Variables.Reverse(selectedIndex - 1, 2);
            }
        }
        #endregion
        #region UCUpDownButton_OnDown
        private void UCUpDownButton_OnDown(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;
            int selectedIndex;

            traceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;
            selectedIndex = dgrSelected.SelectedIndex;

            if (traceInfo != null && selectedIndex >= 0 && selectedIndex < this._displayVariableInfo.Count - 1)
            {
                this._displayVariableInfo.Move(selectedIndex, selectedIndex + 1);
                traceInfo.Variables.Reverse(selectedIndex, 2);
            }
        }
        #endregion
        #region UCUpDownButton_OnLast
        private void UCUpDownButton_OnLast(object sender, EventArgs e)
        {
            ExpandedTraceInfo traceInfo;
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            traceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;
            selectedIndex = dgrSelected.SelectedIndex;

            if (traceInfo != null && variableInfo != null && selectedIndex >= 0)
            {
                this._displayVariableInfo.Move(selectedIndex, this._displayVariableInfo.Count - 1);

                traceInfo.Variables.Remove(variableInfo.VID);
                traceInfo.Variables.Add(variableInfo.VID);
            }
        }
        #endregion

        // Button Event
        #region btnSendTriggerEdit_Click
        private void btnSendTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Settings.TriggerWindow triggerWindow;
            ExpandedTraceInfo expandedTraceInfo;

            expandedTraceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;

            if (expandedTraceInfo != null)
            {
                triggerWindow = new Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Trace Send", expandedTraceInfo.TraceID, expandedTraceInfo.SendTriggerCollection);
                triggerWindow.Owner = this;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
            }
        }
        #endregion
        #region btnStopTriggerEdit_Click
        private void btnStopTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Settings.TriggerWindow triggerWindow;
            ExpandedTraceInfo expandedTraceInfo;

            expandedTraceInfo = dgrTrace.SelectedItem as ExpandedTraceInfo;

            if (expandedTraceInfo != null)
            {
                triggerWindow = new Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Trace Stop", expandedTraceInfo.TraceID, expandedTraceInfo.StopTriggerCollection);
                triggerWindow.Owner = this;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
            }
        }
        #endregion
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Structure.GemDriverError error;

            if (IsValid() == true)
            {
                this._messageProcessor.TraceCollection.Items.Clear();
                this._messageProcessor.TraceCollection.Items.AddRange(this._displayInfo);

                if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == false)
                {
                    error = this._messageProcessor.SaveConfigFile(out string errorText);

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

        // Private Method
        #region IsValid
        private bool IsValid()
        {
            bool result;
            dynamic converted;
            string stringValue;
            string errorMsg;
            List<string> usedIDs;

            SECSItemFormat tridFormat;
            Structure.DataDictionaryInfo dataDictionaryInfo;


            usedIDs = new List<string>();
            errorMsg = string.Empty;

            dataDictionaryInfo = this._messageProcessor.DataDictionaryCollection[DataDictinaryList.TRID.ToString()];
            tridFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

            foreach (var traceInfo in _displayInfo)
            {
                if (string.IsNullOrEmpty(errorMsg) == true)
                {
                    if (tridFormat == SECSItemFormat.A || tridFormat == SECSItemFormat.J)
                    {
                        if (string.IsNullOrEmpty(traceInfo.TraceID) == true)
                        {
                            errorMsg = string.Format(" TRID: {0} is invalid", traceInfo.TraceID);
                        }
                    }
                    else
                    {
                        converted = this._messageProcessor.ConvertValue(tridFormat, traceInfo.TraceID);

                        if (converted == null || traceInfo.TraceID != converted.ToString())
                        {
                            errorMsg = string.Format(" TRID: {0} is invalid", traceInfo.TraceID);
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorMsg) == true && traceInfo.AutoSend == true)
                {
                    if( traceInfo.SendTriggerCollection.Items.Count == 0)
                    {
                        errorMsg = string.Format(" TRID: {0} \n\n AutoSend is selected, but Trigger is not selected", traceInfo.TraceID);
                    }
                }

                if (string.IsNullOrEmpty(errorMsg) == true && traceInfo.SendTriggerCollection.ValidateTriggers(out errorMsg, out AutoSendTrigger invalidTrigger) == false)
                {
                    errorMsg = string.Format(" TRID: {0} \n\n Send trigger invalid, \n\n {1}", traceInfo.TraceID, invalidTrigger.ToString());
                }

                if (string.IsNullOrEmpty(errorMsg) == true && traceInfo.AutoStop == true)
                {
                    if (traceInfo.StopTriggerCollection.Items.Count == 0)
                    {
                        errorMsg = string.Format(" TRID: {0} \n\n AutoStop is selected, but Trigger is not selected", traceInfo.TraceID);
                    }
                }

                if (string.IsNullOrEmpty(errorMsg) == true && traceInfo.StopTriggerCollection.ValidateTriggers(out errorMsg, out invalidTrigger) == false)
                {
                    errorMsg = string.Format(" TRID: {0} \n\n Stop trigger invalid, \n\n {1}", traceInfo.TraceID, invalidTrigger.ToString());
                }

                if (string.IsNullOrEmpty(errorMsg) == true)
                {
                    stringValue = traceInfo.Dsper;

                    if (int.TryParse(stringValue, out int intValue) == false)
                    {
                        errorMsg = string.Format(" TRID: {0} \n\n DSPER must be integer", traceInfo.TraceID);
                    }
                    else if (stringValue.Length != 6 && stringValue.Length != 8)
                    {
                        errorMsg = string.Format(" TRID: {0} \n\n invalid length(6 or 8) of DSPER", traceInfo.TraceID);
                    }
                }

                if (string.IsNullOrEmpty(errorMsg) == true && usedIDs.Contains(traceInfo.TraceID) == true)
                {
                    errorMsg = string.Format(" TRID: {0} is dupelicated", traceInfo.TraceID);
                }
                else
                {
                    usedIDs.Add(traceInfo.TraceID);
                }
            }

            if (string.IsNullOrEmpty(errorMsg) == false)
            {
                MessageBox.Show(errorMsg);
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
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