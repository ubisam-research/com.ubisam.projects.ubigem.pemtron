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
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RemoteCommandWindow : Window
    {
        #region MemberVariable
        private ObservableCollection<ExpandedRemoteCommandInfo> _displayInfo;
        private ObservableCollection<ExpandedRemoteCommandParameterInfo> _displayParameterInfo;

        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public RemoteCommandWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;

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

            lblExample1.Content = "A__{EC:15}";
            lblExample2.Content = "{VID:100}__AA";
            lblExample3.Content = "{RAND:1:9}";
            lblExample4.Content = "{INC:1:2}";

            this._displayInfo = new ObservableCollection<ExpandedRemoteCommandInfo>();

            foreach (var item in this._messageProcessor.RemoteCommandCollection.RemoteCommandItems)
            {
                if (item is ExpandedRemoteCommandInfo)
                {
                    expandedRemoteCommandInfo = item as ExpandedRemoteCommandInfo;
                    this._displayInfo.Add(expandedRemoteCommandInfo.Clone());
                }
            }

            dgrRemoteCommand.ItemsSource = this._displayInfo;

            if (this._displayInfo.Count > 0)
            {
                dgrRemoteCommand.SelectedItem = this._displayInfo.ElementAt(0);
            }
        }
        #endregion

        // DataGrid Event
        #region RemoteCommand_OnAdd
        private void RemoteCommand_OnAdd(object sender, EventArgs e)
        {
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;

            expandedRemoteCommandInfo = new ExpandedRemoteCommandInfo();

            this._displayInfo.Add(expandedRemoteCommandInfo);

            this._displayParameterInfo = new ObservableCollection<ExpandedRemoteCommandParameterInfo>(expandedRemoteCommandInfo.ValueSetCollection["Default"].ParameterItems);

            dgrRemoteCommand.SelectedItem = expandedRemoteCommandInfo;
            dgrCommandParameter.ItemsSource = this._displayParameterInfo;
        }
        #endregion
        #region RemoteCommand_OnRemove
        private void RemoteCommand_OnRemove(object sender, EventArgs e)
        {
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;

            expandedRemoteCommandInfo = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;

            if (expandedRemoteCommandInfo != null)
            {
                if(expandedRemoteCommandInfo.IsInheritance == true)
                {
                    MessageBox.Show(string.Format("RemoteCommand: {0} can not remove", expandedRemoteCommandInfo.RemoteCommand));
                }
                else
                {
                    this._displayInfo.Remove(expandedRemoteCommandInfo);

                    dgrCommandParameter.ItemsSource = null;
                }
            }
        }
        #endregion
        #region dgrRemoteCommand_SelectionChanged
        private void dgrRemoteCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;

            expandedRemoteCommandInfo = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;

            if (expandedRemoteCommandInfo != null)
            {
                if (expandedRemoteCommandInfo.IsInheritance == true)
                {
                    dgcCommand.IsReadOnly = true;
                    dgcDescription.IsReadOnly = true;

                    ctlFunctionRemoteCommand.ChangeButtonEnabled(true, false);
                }
                else
                {
                    dgcCommand.IsReadOnly = false;
                    dgcDescription.IsReadOnly = false;

                    ctlFunctionRemoteCommand.ChangeButtonEnabled(true, true);
                }

                cboValueSetName.ItemsSource = expandedRemoteCommandInfo.ValueSetCollection.Items.Values;
                cboValueSetName.SelectedItem = expandedRemoteCommandInfo.ValueSetCollection["Default"];
            }
        }
        #endregion

        #region CommandParameter_OnAdd
        private void CommandParameter_OnAdd(object sender, EventArgs e)
        {
            ExpandedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedRemoteCommandParameterInfo commandParameterInfo;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                commandParameterInfo = new ExpandedRemoteCommandParameterInfo();

                selectedValueSet.ParameterItems.Add(commandParameterInfo);
                this._displayParameterInfo.Add(commandParameterInfo);

                dgrCommandParameter.SelectedItem = commandParameterInfo;
            }
        }
        #endregion
        #region CommandParameter_OnRemove
        private void CommandParameter_OnRemove(object sender, EventArgs e)
        {
            ExpandedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedRemoteCommandParameterInfo expandedCommandParameterInfo;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedRemoteCommandValueSetInfo;
            expandedCommandParameterInfo = dgrCommandParameter.SelectedItem as ExpandedRemoteCommandParameterInfo;

            if (selectedValueSet != null && expandedCommandParameterInfo != null)
            {
                selectedValueSet.ParameterItems.Remove(expandedCommandParameterInfo);
                this._displayParameterInfo.Remove(expandedCommandParameterInfo);
            }
        }
        #endregion
        #region dgcParameterFormat_SelectionChanged
        private void dgcParameterFormat_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ExpandedRemoteCommandParameterInfo expandedCommandParameterInfo;
            ComboBox comboBox;

            expandedCommandParameterInfo = dgrCommandParameter.SelectedItem as ExpandedRemoteCommandParameterInfo;
            comboBox = e.Source as ComboBox;

            if (expandedCommandParameterInfo != null && comboBox != null && comboBox.SelectedItem != null)
            {
                if (Enum.TryParse(comboBox.SelectedItem.ToString(), out SECSItemFormat selectedFormat) == true)
                {
                    expandedCommandParameterInfo.Format = selectedFormat;
                }
            }
        }
        private void dgcParameterRule_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.GeneratingRuleWindow window;
            ExpandedRemoteCommandInfo expandedCommandInfo;
            ExpandedRemoteCommandParameterInfo expandedCommandParameterInfo;
            bool? dialogResult;
            string beforeRule;

            expandedCommandInfo = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;
            expandedCommandParameterInfo = dgrCommandParameter.SelectedItem as ExpandedRemoteCommandParameterInfo;

            if (expandedCommandInfo != null && expandedCommandParameterInfo != null)
            {
                beforeRule = expandedCommandParameterInfo.GenerateRule;

                window = new Settings.GeneratingRuleWindow();
                window.Initialize(this._messageProcessor);
                window.Owner = this;
                window.Header = $"Generating Rule Editor For RCMD={expandedCommandInfo.RemoteCommand}, CP={expandedCommandParameterInfo.Name}";
                window.GeneratingRule = beforeRule;

                dialogResult = window.ShowDialog();

                if (dialogResult != null && dialogResult.Value == true)
                {
                    expandedCommandParameterInfo.GenerateRule = window.GeneratingRule;
                }
            }
        }
        #endregion

        // UserControl: Up, Down Event
        #region ctlUpDown_OnFirst
        private void ctlUpDown_OnFirst(object sender, EventArgs e)
        {
            ExpandedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedRemoteCommandParameterInfo selectedParameterInfo;

            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrCommandParameter.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displayParameterInfo.Move(selectedIndex, 0);

                    selectedParameterInfo = selectedValueSet.ParameterItems[selectedIndex];
                    selectedValueSet.ParameterItems.Remove(selectedParameterInfo);
                    selectedValueSet.ParameterItems.Insert(0, selectedParameterInfo);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnUp
        private void ctlUpDown_OnUp(object sender, EventArgs e)
        {
            ExpandedRemoteCommandValueSetInfo selectedValueSet;

            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrCommandParameter.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displayParameterInfo.Move(selectedIndex, selectedIndex - 1);
                    selectedValueSet.ParameterItems.Reverse(selectedIndex - 1, 2);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnDown
        private void ctlUpDown_OnDown(object sender, EventArgs e)
        {
            ExpandedRemoteCommandValueSetInfo selectedValueSet;

            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrCommandParameter.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displayParameterInfo.Count - 1)
                {
                    this._displayParameterInfo.Move(selectedIndex, selectedIndex + 1);
                    selectedValueSet.ParameterItems.Reverse(selectedIndex, 2);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnLast
        private void ctlUpDown_OnLast(object sender, EventArgs e)
        {
            ExpandedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedRemoteCommandParameterInfo selectedParameterInfo;

            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrCommandParameter.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displayParameterInfo.Count - 1)
                {
                    this._displayParameterInfo.Move(selectedIndex, this._displayParameterInfo.Count - 1);

                    selectedParameterInfo = selectedValueSet.ParameterItems[selectedIndex];
                    selectedValueSet.ParameterItems.RemoveAt(selectedIndex);
                    selectedValueSet.ParameterItems.Add(selectedParameterInfo);
                }
            }
        }
        #endregion

        // Button Event
        #region btnTriggerEdit_Click
        private void btnTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Settings.TriggerWindow triggerWindow;
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;

            expandedRemoteCommandInfo = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;

            if (expandedRemoteCommandInfo != null)
            {
                triggerWindow = new Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Remote Command", expandedRemoteCommandInfo.RemoteCommand, expandedRemoteCommandInfo.TriggerCollection);
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

            if (IsValid(out string errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                this._messageProcessor.RemoteCommandCollection.RemoteCommandItems.Clear();

                foreach (var commandItem in this._displayInfo)
                {
                    this._messageProcessor.RemoteCommandCollection.RemoteCommandItems.Add(commandItem);
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
        #region btnChangeValueSet_Click
        private void btnChangeValueSet_Click(object sender, RoutedEventArgs e)
        {
            ValueSetNameEditWindow window;
            ExpandedRemoteCommandInfo selectedCMD;
            List<ValueSetNameEditWindow.DisplayData> valueSetNames;
            int valueSetNameIndex;

            selectedCMD = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;

            if (selectedCMD != null)
            {
                valueSetNames = new List<ValueSetNameEditWindow.DisplayData>();
                valueSetNameIndex = 0;

                foreach (string name in selectedCMD.ValueSetCollection.Items.Keys)
                {
                    valueSetNames.Add(new ValueSetNameEditWindow.DisplayData()
                    {
                        OriginalIndex = valueSetNameIndex,
                        Name = name
                    });
                    valueSetNameIndex++;
                }

                window = new ValueSetNameEditWindow();

                window.Initialize(valueSetNames);
                window.Owner = this;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.ValueSetNameChanged += ValueSetNameWindow_ValueSetNameChanged;
                window.ShowDialog();
            }
        }
        #endregion
        #region cboValueSetName_SelectionChanged
        private void cboValueSetName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedRemoteCommandValueSetInfo selectedValueSet;
            selectedValueSet = cboValueSetName.SelectedItem as ExpandedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                this._displayParameterInfo = new ObservableCollection<ExpandedRemoteCommandParameterInfo>(selectedValueSet.ParameterItems);

                dgrCommandParameter.ItemsSource = this._displayParameterInfo;

                if (this._displayParameterInfo.Count > 0)
                {
                    dgrCommandParameter.SelectedItem = this._displayParameterInfo.ElementAt(0);
                }
            }
        }
        #endregion
        #region ValueSetNameWindow_ValueSetNameChanged
        private void ValueSetNameWindow_ValueSetNameChanged(List<ValueSetNameEditWindow.DisplayData> changedData)
        {
            ExpandedRemoteCommandValueSetCollection newCollection;
            ExpandedRemoteCommandInfo selectedCMD;
            ExpandedRemoteCommandValueSetInfo valueSetInfo;
            int valueSetNameIndex;

            newCollection = new ExpandedRemoteCommandValueSetCollection();
            selectedCMD = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;

            if (selectedCMD != null && changedData != null && changedData.Count > 0)
            {
                foreach (ValueSetNameEditWindow.DisplayData data in changedData)
                {
                    valueSetNameIndex = data.OriginalIndex;

                    // Index 가 0은 Default 이므로 무조건 추가
                    if (valueSetNameIndex == 0)
                    {
                        valueSetInfo = selectedCMD.ValueSetCollection.Items.ElementAt(valueSetNameIndex).Value;
                    }
                    // Index 가 0보다 작은 경우 Default를 복사해서 추가
                    else if (valueSetNameIndex < 0)
                    {
                        valueSetInfo = selectedCMD.ValueSetCollection["Default"].Clone();
                        valueSetInfo.Name = data.Name;
                        FillDefault(valueSetInfo);
                    }
                    // Index가 0보다 큰 경우 Name을 수정해서 추가
                    else
                    {
                        valueSetInfo = selectedCMD.ValueSetCollection.Items.ElementAt(valueSetNameIndex).Value;
                        valueSetInfo.Name = data.Name;
                    }
                    newCollection.Add(valueSetInfo);
                }

                // combobox update
                selectedCMD.ValueSetCollection = newCollection;
                cboValueSetName.ItemsSource = newCollection.Items.Values;
                cboValueSetName.SelectedItem = newCollection.Items.First().Value;
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

        // Private Method
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            List<string> usedCommandNames;
            //List<string> usedParameterNames;
            dynamic converted;

            errorText = string.Empty;

            result = true;
            usedCommandNames = new List<string>();

            foreach (var commandItem in this._displayInfo)
            {
                if (commandItem.AutoSend == true)
                {
                    if (commandItem.TriggerCollection.Items.Count == 0)
                    {
                        result = false;
                        errorText = string.Format(" Remote Command: {0} \n\n AutoSend selected, but Trigger is not selected", commandItem.RemoteCommand);
                    }
                }

                if (result == true && commandItem.TriggerCollection.ValidateTriggers(out errorText, out AutoSendTrigger invalidTrigger) == false)
                {
                    result = false;
                    errorText = string.Format(" Remote Command: {0} \n\n Trigger invalid, \n\n {1}", commandItem.RemoteCommand, invalidTrigger.ToString());
                }

                if (result == true && string.IsNullOrEmpty(commandItem.RemoteCommand) == true)
                {
                    result = false;
                    errorText = string.Format("No name command exists");
                }

                if (result == true)
                {
                    if (usedCommandNames.Contains(commandItem.RemoteCommand) == true)
                    {
                        result = false;
                        errorText = string.Format(" Remote Command: {0} \n\n name is dupelicated", commandItem.RemoteCommand);
                    }
                    else
                    {
                        usedCommandNames.Add(commandItem.RemoteCommand);

                    }
                }
                if (result == true)
                {
                    foreach(ExpandedRemoteCommandValueSetInfo valueSet in commandItem.ValueSetCollection.Items.Values)
                    {
                        if (result == true)
                        {
                            //usedParameterNames = new List<string>();

                            foreach (var parameterItem in valueSet.ParameterItems)
                            {
                                if (parameterItem.Format == SECSItemFormat.X || parameterItem.Format == SECSItemFormat.None || parameterItem.Format == SECSItemFormat.L)
                                {
                                    result = false;
                                    errorText = string.Format(" Remote Command: {0} \n\n ValueSet Name: {1} \n\n Parameter Name: {2} \n\n format is invalid", commandItem.RemoteCommand, valueSet.Name, parameterItem.Name);
                                }

                                /*
                                if (result == true && string.IsNullOrEmpty(parameterItem.Name) == true)
                                {
                                    result = false;
                                    errorText = string.Format(" Remote Command: {0} \n\n ValueSet Name: {1} \n\n no name parameter exist", commandItem.RemoteCommand, valueSet.Name);
                                }
                                */

                                if (result == true && string.IsNullOrEmpty(parameterItem.Value) == false)
                                {
                                    if (parameterItem.Format != SECSItemFormat.A && parameterItem.Format != SECSItemFormat.J && parameterItem.Count > 0)
                                    {
                                        converted = this._messageProcessor.ConvertValue(parameterItem.Format, parameterItem.Count, parameterItem.Value);

                                        if (converted == null)
                                        {
                                            result = false;
                                            errorText = string.Format(" Remote Command: {0} \n\n ValueSet Name: {1} \n\n Parameter Name: {2} \n\n Value convert failed.", commandItem.RemoteCommand, valueSet.Name, parameterItem.Name);
                                        }
                                    }
                                }

                                if (result == true && string.IsNullOrEmpty(parameterItem.GenerateRule) == false)
                                {
                                    if (this._messageProcessor.IsValidGenerateRule(parameterItem.Format, parameterItem.GenerateRule) == false)
                                    {
                                        result = false;
                                        errorText = string.Format(" Remote Command: {0} \n\n ValueSet Name: {1} \n\n Parameter Name: {2} \n\n Rule verify fail", commandItem.RemoteCommand, valueSet.Name, parameterItem.Name);
                                    }
                                }

                                /*
                                if (result == true)
                                {
                                    if (usedParameterNames.Contains(parameterItem.Name) == true)
                                    {
                                        result = false;
                                        errorText = string.Format(" Remote Command: {0} \n\n ValueSet Name: {1} \n\n Parameter: {2} \n\n name is dupelicated", commandItem.RemoteCommand, valueSet.Name, parameterItem.Name);
                                    }
                                    else
                                    {
                                        usedParameterNames.Add(parameterItem.Name);
                                    }
                                }
                                */
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region FillDefault
        private void FillDefault(ExpandedRemoteCommandValueSetInfo valueSetInfo)
        {
            if (valueSetInfo != null)
            {
                foreach (ExpandedRemoteCommandParameterInfo parameterInfo in valueSetInfo.ParameterItems)
                {
                    switch (parameterInfo.Format)
                    {
                        case SECSItemFormat.None:
                        case SECSItemFormat.A:
                        case SECSItemFormat.J:
                        case SECSItemFormat.L:
                        case SECSItemFormat.X:
                            break;
                        case SECSItemFormat.Boolean:
                            if (string.IsNullOrEmpty(parameterInfo.Value) == true)
                            {
                                parameterInfo.Value = false.ToString();
                            }
                            break;
                        default:
                            if (string.IsNullOrEmpty(parameterInfo.Value) == true)
                            {
                                parameterInfo.Value = 0.ToString();
                            }
                            break;
                    }
                }
            }
        }
        #endregion

    }
}