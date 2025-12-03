using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommandParameterListWindow : Window
    {
        #region MemberVariable
        private string _commandName;
        private ExpandedEnhancedRemoteCommandParameterInfo _commandParameter;
        private List<ExpandedEnhancedRemoteCommandParameterItem> _parameterStack;
        private ObservableCollection<ExpandedEnhancedRemoteCommandParameterItem> _enhancedParameterValueItems;

        private MessageProcessor _messageProcessor;

        private bool _childChanged;
        #endregion
        #region Constructor
        public CommandParameterListWindow()
        {
            InitializeComponent();

            this._childChanged = false;
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterItem parentItem;

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

            if (this._messageProcessor == null || string.IsNullOrEmpty(this._commandName) == true || this._commandParameter == null || this._parameterStack == null || this._parameterStack.Count == 0)
            {
                MessageBox.Show("Command Parameter List Setting Window is not initialized");
                Close();
            }
            else
            {
                lblCommandName.Content = this._commandName.Replace("_", "__");
                lblCPName.Content = this._commandParameter.Name.Replace("_", "__");

                dgrCPVALStack.ItemsSource = this._parameterStack;

                this._enhancedParameterValueItems = new ObservableCollection<ExpandedEnhancedRemoteCommandParameterItem>();

                parentItem = this._parameterStack.LastOrDefault();

                if (parentItem != null)
                {
                    /*
                    if (parentItem.ListType == EnhancedParameterListType.A)
                    {
                        btnListTypeA.IsChecked = true;
                        btnListTypeB.IsChecked = false;
                    }
                    else
                    {
                        btnListTypeA.IsChecked = false;
                        btnListTypeB.IsChecked = true;
                    }
                    */

                    foreach (ExpandedEnhancedRemoteCommandParameterItem child in parentItem.ChildParameterItem)
                    {
                        this._enhancedParameterValueItems.Add(child.Clone());
                    }
                }
                dgrEnhancedCommandValue.ItemsSource = this._enhancedParameterValueItems;
            }
        }
        #endregion

        // DataGrid Event
        #region EnhancedCommandValue_OnAdd
        private void EnhancedCommandValue_OnAdd(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterItem parentItem;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;

            parentItem = this._parameterStack.LastOrDefault();

            if (parentItem != null)
            {
                expandedEnhancedCommandParameterItem = new ExpandedEnhancedRemoteCommandParameterItem();
                this._enhancedParameterValueItems.Add(expandedEnhancedCommandParameterItem);
                dgrEnhancedCommandValue.SelectedItem = expandedEnhancedCommandParameterItem;
            }
        }
        #endregion
        #region EnhancedCommandValue_OnRemove
        private void EnhancedCommandValue_OnRemove(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;

            expandedEnhancedCommandParameterItem = dgrEnhancedCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;

            if (expandedEnhancedCommandParameterItem != null)
            {
                this._enhancedParameterValueItems.Remove(expandedEnhancedCommandParameterItem);
            }
        }
        #endregion
        #region dgcCPVALFormat_SelectionChanged
        private void dgcCPVALFormat_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox combobox;
            SECSItemFormat selectedFormat;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;
            combobox = e.Source as ComboBox;
            expandedEnhancedCommandParameterItem = dgrEnhancedCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;

            if (combobox != null && combobox.SelectedItem != null && expandedEnhancedCommandParameterItem != null)
            {
                /*
                selectedFormat = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), combobox.SelectedItem.ToString());

                if (expandedEnhancedCommandParameterItem.ListType == EnhancedParameterListType.A && selectedFormat == SECSItemFormat.L)
                {
                    if (selectionChangedEventArgs.RemovedItems.Count == 0)
                    {
                        selectedFormat = SECSItemFormat.A;
                    }
                    else
                    {
                        selectedFormat = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), selectionChangedEventArgs.RemovedItems[0].ToString().ToString());
                    }

                    expandedEnhancedCommandParameterItem.Format = selectedFormat;

                    MessageBox.Show("can not set Format L under List Type: A");
                }
                */
                selectedFormat = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), combobox.SelectedItem.ToString());
                expandedEnhancedCommandParameterItem.Format = selectedFormat;
            }
        }
        #endregion

        // UserControl: Up, Down Event
        #region ctlValueUpDown_OnFirst
        private void ctlValueUpDown_OnFirst(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (selectedIndex > 0)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, 0);
            }
        }
        #endregion
        #region ctlValueUpDown_OnUp
        private void ctlValueUpDown_OnUp(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (selectedIndex > 0)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, selectedIndex - 1);
            }
        }
        #endregion
        #region ctlValueUpDown_OnDown
        private void ctlValueUpDown_OnDown(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < this._enhancedParameterValueItems.Count - 1)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, selectedIndex + 1);
            }
        }
        #endregion
        #region ctlValueUpDown_OnLast
        private void ctlValueUpDown_OnLast(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < this._enhancedParameterValueItems.Count - 1)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, this._enhancedParameterValueItems.Count - 1);
            }
        }
        #endregion

        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterItem parentItem;

            parentItem = this._parameterStack.LastOrDefault();

            if (IsValid(out string errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                this._childChanged = CheckNotEquility(parentItem.ChildParameterItem, new List<ExpandedEnhancedRemoteCommandParameterItem>(this._enhancedParameterValueItems));
                parentItem.ChildParameterItem.Clear();
                parentItem.ChildParameterItem.AddRange(this._enhancedParameterValueItems);
            }
        }
        #endregion
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterItem parentItem;

            parentItem = this._parameterStack.LastOrDefault();

            if (this._childChanged == false)
            {
                this._childChanged = CheckNotEquility(parentItem.ChildParameterItem, new List<ExpandedEnhancedRemoteCommandParameterItem>(this._enhancedParameterValueItems));

                if (this._childChanged == true)
                {
                    MessageBoxResult msgResult = MessageBox.Show("Some data is changed.\n\nDo you want ignore changed data?", "Warning", MessageBoxButton.YesNo);

                    if (msgResult == MessageBoxResult.Yes)
                    {
                        this._childChanged = false;
                    }
                    else
                    {
                        parentItem.ChildParameterItem.Clear();
                        parentItem.ChildParameterItem.AddRange(this._enhancedParameterValueItems);
                    }
                }
            }

            DialogResult = this._childChanged;
            Close();
        }
        #endregion

        #region dgcValueChildLength_SelectionChanged
        private void dgcValueChildLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterItem selectedItem;
            ComboBox comboBox;

            selectedItem = dgrEnhancedCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;
            comboBox = e.Source as ComboBox;

            if (selectedItem != null && comboBox != null && comboBox.SelectedValue != null)
            {
                selectedItem.UseChildLength = bool.Parse(comboBox.SelectedValue.ToString());
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, string commandName, ExpandedEnhancedRemoteCommandParameterInfo commandParameter, List<ExpandedEnhancedRemoteCommandParameterItem> parameterStack)
        {
            this._messageProcessor = messageProcessor;
            this._commandName = commandName;
            this._commandParameter = commandParameter;
            this._parameterStack = parameterStack;
        }
        #endregion

        // Private Method
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            //bool isListTypeA;
            //bool dupelicatedValueName;
            bool formatNotSelectedValue;
            //bool valueNoName;
            bool generateRuleFailValue;
            bool convertFail;
            //List<string> usedValueNames;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;
            string[] splitted;

            result = true;
            errorText = string.Empty;

            //isListTypeA = btnListTypeA.IsChecked.Value;

            expandedEnhancedCommandParameterItem = null;
            //dupelicatedValueName = false;
            formatNotSelectedValue = false;
            //valueNoName = false;
            generateRuleFailValue = false;
            convertFail = false;

            //usedValueNames = new List<string>();

            foreach (ExpandedEnhancedRemoteCommandParameterItem valueItem in this._enhancedParameterValueItems)
            {
                if (valueItem.Format == SECSItemFormat.X || valueItem.Format == SECSItemFormat.None)
                {
                    result = false;
                    formatNotSelectedValue = true;
                    expandedEnhancedCommandParameterItem = valueItem;
                }

                if (result == true && string.IsNullOrEmpty(valueItem.GenerateRule) == false)
                {
                    if (this._messageProcessor.IsValidGenerateRule(valueItem.Format, valueItem.GenerateRule) == false)
                    {
                        result = false;
                        generateRuleFailValue = true;
                        expandedEnhancedCommandParameterItem = valueItem;
                    }
                }

                if (result == true && string.IsNullOrEmpty(valueItem.Value) == false)
                {
                    if (valueItem.Format != SECSItemFormat.A && valueItem.Format != SECSItemFormat.J)
                    {
                        splitted = valueItem.Value.Split(' ');

                        convertFail = false;

                        foreach (string s in splitted)
                        {
                            if (string.IsNullOrEmpty(s) == false && this._messageProcessor.ConvertValue(valueItem.Format, s) == null)
                            {
                                result = false;
                                expandedEnhancedCommandParameterItem = valueItem;
                                convertFail = true;
                                break;
                            }
                        }
                    }
                }

                /*
                if (result == true && string.IsNullOrEmpty(valueItem.Name) == true)
                {
                    if (isListTypeA == false)
                    {
                        result = false;
                        valueNoName = true;
                        expandedEnhancedCommandParameterItem = valueItem;
                    }
                }

                if (result == true)
                {
                    if (usedValueNames.Contains(valueItem.Name) == true)
                    {
                        result = false;
                        dupelicatedValueName = true;
                        expandedEnhancedCommandParameterItem = valueItem;
                    }
                    else
                    {
                        usedValueNames.Add(valueItem.Name);
                    }
                }
                */
            }

            if (result == false)
            {
                if (convertFail == true)
                {
                    errorText = string.Format(" Value Name: {0} Value convert fail", expandedEnhancedCommandParameterItem.Name);
                }
                else if (generateRuleFailValue == true)
                {
                    errorText = string.Format(" Value Name: {0} Rule verify fail", expandedEnhancedCommandParameterItem.Name);
                }
                /*
                else if (valueNoName == true)
                {
                    errorText = string.Format(" no name value exist");
                }
                */
                else if (formatNotSelectedValue == true)
                {
                    errorText = string.Format(" Value Name: {0} \n\n format is not selected", expandedEnhancedCommandParameterItem.Name);
                }
                /*
                else if (dupelicatedValueName == true)
                {
                    errorText = string.Format(" Value: {0} \n\n name is dupelicated", expandedEnhancedCommandParameterItem.Name);
                }
                */
            }
            return result;
        }
        #endregion
        private bool CheckNotEquility(List<ExpandedEnhancedRemoteCommandParameterItem> list1, List<ExpandedEnhancedRemoteCommandParameterItem> list2)
        {
            bool notEquals = false;

            if (list1 != null && list2 != null)
            {
                if (list1.Count == list2.Count)
                {
                    for (int i = 0, n = list1.Count; i < n; ++i)
                    {
                        if (notEquals == true)
                        {
                            break;
                        }
                        else
                        {
                            if (list1[i].Name != list2[i].Name)
                            {
                                notEquals = true;
                            }
                            else if (list1[i].Format != list2[i].Format)
                            {
                                notEquals = true;
                            }
                            else if (list1[i].Count != list2[i].Count)
                            {
                                notEquals = true;
                            }
                            else if (list1[i].Value != list2[i].Value)
                            {
                                notEquals = true;
                            }
                            else if (list1[i].GenerateRule != list2[i].GenerateRule)
                            {
                                notEquals = true;
                            }
                        }
                    }
                }
                else
                {
                    notEquals = true;
                }
            }
            else if(list1 == null && list2 != null)
            {
                notEquals = true;
            }
            else if (list1 != null && list2 == null)
            {
                notEquals = true;
            }

            return notEquals;
        }

        private void btnChildEdit_Click(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;
            CommandParameterListWindow window;
            bool? childChanged;

            expandedEnhancedCommandParameterItem = dgrEnhancedCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;

            if (expandedEnhancedCommandParameterItem != null && expandedEnhancedCommandParameterItem.Format == SECSItemFormat.L)
            {
                this._parameterStack.Add(expandedEnhancedCommandParameterItem);

                window = new CommandParameterListWindow();
                window.Initialize(this._messageProcessor, this._commandName, this._commandParameter, this._parameterStack);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = this;
                childChanged = window.ShowDialog();

                if (childChanged.HasValue == true && childChanged.Value == true)
                {
                    this._childChanged = true;
                }

                this._parameterStack.RemoveAt(this._parameterStack.Count - 1);
            }
        }
    }
}