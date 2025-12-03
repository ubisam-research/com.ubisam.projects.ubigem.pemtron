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
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EnhancedRemoteCommandWindow : Window
    {
        #region MemberVariable
        private readonly ObservableCollection<ExpandedEnhancedRemoteCommandInfo> _enhancedCommandItems;
        private ObservableCollection<ExpandedEnhancedRemoteCommandParameterInfo> _enhancedParameterItems;
        private ObservableCollection<ExpandedEnhancedRemoteCommandParameterItem> _enhancedParameterValueItems;

        private MessageProcessor _messageProcessor;

        private bool _childChanged;
        #endregion
        #region Constructor
        public EnhancedRemoteCommandWindow()
        {
            InitializeComponent();

            this._enhancedCommandItems = new ObservableCollection<ExpandedEnhancedRemoteCommandInfo>();
            this._childChanged = false;
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;

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

            foreach (var item in this._messageProcessor.RemoteCommandCollection.EnhancedRemoteCommandItems)
            {
                expandedEnhancedRemoteCommandInfo = item as ExpandedEnhancedRemoteCommandInfo;

                this._enhancedCommandItems.Add(expandedEnhancedRemoteCommandInfo.Clone());
            }

            dgrEnhancedRemoteCommand.ItemsSource = this._enhancedCommandItems;

            if (this._enhancedCommandItems.Count > 0)
            {
                dgrEnhancedRemoteCommand.SelectedItem = this._enhancedCommandItems.ElementAt(0);
            }
        }
        #endregion

        // DataGrid Event
        #region EnhancedRemoteCommand_OnAdd
        private void EnhancedRemoteCommand_OnAdd(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;

            expandedEnhancedRemoteCommandInfo = new ExpandedEnhancedRemoteCommandInfo();

            this._enhancedCommandItems.Add(expandedEnhancedRemoteCommandInfo);
            this._enhancedParameterItems = new ObservableCollection<ExpandedEnhancedRemoteCommandParameterInfo>(expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"].ParameterItems);

            dgrEnhancedRemoteCommand.SelectedItem = expandedEnhancedRemoteCommandInfo;
            this.dgrEnhancedCommandParameter.ItemsSource = this._enhancedParameterItems;
            this.dgrEnhancedCommandValue.ItemsSource = null;
        }
        #endregion
        #region EnhancedRemoteCommand_OnRemove
        private void EnhancedRemoteCommand_OnRemove(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;

            expandedEnhancedRemoteCommandInfo = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;

            if (expandedEnhancedRemoteCommandInfo != null)
            {
                if (expandedEnhancedRemoteCommandInfo.IsInheritance == true)
                {
                    MessageBox.Show($"Remote Command: {expandedEnhancedRemoteCommandInfo.RemoteCommand} can not remove");
                }
                else
                {
                    this._enhancedCommandItems.Remove(expandedEnhancedRemoteCommandInfo);

                    this.dgrEnhancedCommandParameter.ItemsSource = null;
                    this.dgrEnhancedCommandValue.ItemsSource = null;
                }
            }
        }
        #endregion
        #region dgrEnhancedRemoteCommand_SelectionChanged
        private void dgrEnhancedRemoteCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandInfo selectedCMD;

            selectedCMD = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;

            if (selectedCMD != null)
            {
                ctlCommandParameter.ChangeButtonEnabled(true, true);
                ctlCommandValue.ChangeButtonEnabled(true, true);

                //grdListType.Visibility = Visibility.Visible;

                if (selectedCMD.IsInheritance == true)
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

                cboValueSetName.ItemsSource = selectedCMD.ValueSetCollection.Items.Values;
                cboValueSetName.SelectedItem = selectedCMD.ValueSetCollection["Default"];
            }
        }
        #endregion

        #region EnhancedCommandParameter_OnAdd
        private void EnhancedCommandParameter_OnAdd(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                expandedEnhancedCommandParameterInfo = new ExpandedEnhancedRemoteCommandParameterInfo();

                selectedValueSet.ParameterItems.Add(expandedEnhancedCommandParameterInfo);
                this._enhancedParameterItems.Add(expandedEnhancedCommandParameterInfo);
                this._enhancedParameterValueItems = new ObservableCollection<ExpandedEnhancedRemoteCommandParameterItem>(expandedEnhancedCommandParameterInfo.ValueItems);

                dgrEnhancedCommandParameter.SelectedItem = expandedEnhancedCommandParameterInfo;
                this.dgrEnhancedCommandValue.ItemsSource = this._enhancedParameterValueItems;
            }
        }
        #endregion
        #region EnhancedCommandParameter_OnRemove
        private void EnhancedCommandParameter_OnRemove(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;
            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;

            if (selectedValueSet != null)
            {
                selectedValueSet.ParameterItems.Remove(expandedEnhancedCommandParameterInfo);
                this._enhancedParameterItems.Remove(expandedEnhancedCommandParameterInfo);

                this._enhancedParameterValueItems = null;
                dgrEnhancedCommandValue.ItemsSource = null;

                ctlValueUpDown.ChangeButtonEnabledAll(false);
                ctlCommandValue.ChangeButtonEnabled(false, false);
                //grdListType.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
        #region dgrEnhancedCommandParameter_SelectionChanged
        private void dgrEnhancedCommandParameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedEnhancedRemoteCommandParameterInfo selectedItem;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;
            selectedItem = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;

            if (selectedValueSet != null)
            {
                if (selectedItem != null && selectedItem.Format == SECSItemFormat.L)
                {
                    this._enhancedParameterValueItems = new ObservableCollection<ExpandedEnhancedRemoteCommandParameterItem>(selectedItem.ValueItems);

                    dgrEnhancedCommandValue.ItemsSource = this._enhancedParameterValueItems;

                    /*
                    grdListType.Visibility = Visibility.Visible;
                    btnListTypeA.IsEnabled = false;
                    btnListTypeB.IsEnabled = false;
                    */

                    dgcValueName.IsReadOnly = false;

                    /*
                    if (selectedItem.ListType == EnhancedParameterListType.A)
                    {
                        dgcValueName.IsReadOnly = true;
                        dgcValueRule.IsReadOnly = true;

                        btnListTypeA.IsChecked = true;
                        btnListTypeB.IsChecked = false;
                    }
                    else
                    {
                        dgcValueName.IsReadOnly = false;
                        dgcValueRule.IsReadOnly = false;

                        btnListTypeA.IsChecked = false;
                        btnListTypeB.IsChecked = true;
                    }
                    */

                    ctlValueUpDown.ChangeButtonEnabledAll(true);
                    ctlCommandValue.ChangeButtonEnabled(true, true);

                    /*
                    btnListTypeA.IsEnabled = true;
                    btnListTypeB.IsEnabled = true;
                    */
                }
                else
                {
                    dgrEnhancedCommandValue.ItemsSource = null;

                    ctlValueUpDown.ChangeButtonEnabledAll(false);
                    ctlCommandValue.ChangeButtonEnabled(false, false);

                    dgcValueName.IsReadOnly = false;

                    /*
                    grdListType.Visibility = Visibility.Collapsed;
                    */
                }
            }
        }
        #endregion
        #region dgcParameterFormat_SelectionChanged
        private void dgcParameterFormat_SelectionChanged(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo selectedItem;
            ComboBox comboBox;

            comboBox = sender as ComboBox;
            selectedItem = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;

            if (selectedItem != null && comboBox != null && comboBox.SelectedItem != null)
            {
                if (Enum.TryParse(comboBox.SelectedItem.ToString(), out SECSItemFormat format) == true)
                {
                    selectedItem.Format = format;

                    if (format == SECSItemFormat.L)
                    {
                        this._enhancedParameterValueItems = new ObservableCollection<ExpandedEnhancedRemoteCommandParameterItem>(selectedItem.ValueItems);

                        dgrEnhancedCommandValue.ItemsSource = this._enhancedParameterValueItems;

                        ctlValueUpDown.ChangeButtonEnabledAll(true);
                        ctlCommandValue.ChangeButtonEnabled(true, true);

                        /*
                        grdListType.Visibility = Visibility.Visible;
                        */

                        dgcValueName.IsReadOnly = false;

                        /*
                        if (selectedItem.ListType == EnhancedParameterListType.A)
                        {
                            dgcValueName.IsReadOnly = true;
                            dgcValueRule.IsReadOnly = true;
                        }
                        else
                        {
                            dgcValueName.IsReadOnly = false;
                            dgcValueRule.IsReadOnly = false;
                        }
                        */

                        ctlCommandValue.InvalidateVisual();
                    }
                    else
                    {
                        dgrEnhancedCommandValue.ItemsSource = null;

                        ctlValueUpDown.ChangeButtonEnabledAll(false);
                        ctlCommandValue.ChangeButtonEnabled(false, false);

                        /*
                        grdListType.Visibility = Visibility.Collapsed;
                        */

                        dgcValueName.IsReadOnly = false;
                        ctlCommandValue.InvalidateVisual();
                    }
                }
            }
        }
        #endregion

        #region dgcCPVALFormat_SelectionChanged
        private void dgcCPVALFormat_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox combobox;
            SECSItemFormat selectedFormat;
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;
            combobox = e.Source as ComboBox;
            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            expandedEnhancedCommandParameterItem = dgrEnhancedCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;

            if (combobox != null && combobox.SelectedItem != null && expandedEnhancedCommandParameterInfo != null && expandedEnhancedCommandParameterItem != null)
            {
                selectedFormat = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), combobox.SelectedItem.ToString());

                expandedEnhancedCommandParameterItem.Format = selectedFormat;

                dgcValueName.IsReadOnly = false;
                ctlCommandValue.InvalidateVisual();

                /*
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
            }
        }
        #endregion

        #region EnhancedCommandValue_OnAdd
        private void EnhancedCommandValue_OnAdd(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;

            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;

            if (expandedEnhancedCommandParameterInfo != null && expandedEnhancedCommandParameterInfo.Format == SECSItemFormat.L)
            {
                expandedEnhancedCommandParameterItem = new ExpandedEnhancedRemoteCommandParameterItem();

                expandedEnhancedCommandParameterInfo.ValueItems.Add(expandedEnhancedCommandParameterItem);
                this._enhancedParameterValueItems.Add(expandedEnhancedCommandParameterItem);
                dgrEnhancedCommandValue.SelectedItem = expandedEnhancedCommandParameterItem;
            }
        }
        #endregion
        #region EnhancedCommandValue_OnRemove
        private void EnhancedCommandValue_OnRemove(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;

            expandedEnhancedRemoteCommandInfo = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;
            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            expandedEnhancedCommandParameterItem = dgrEnhancedCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;

            if (expandedEnhancedRemoteCommandInfo != null && expandedEnhancedCommandParameterInfo != null && expandedEnhancedCommandParameterItem != null)
            {
                expandedEnhancedCommandParameterInfo.ValueItems.Remove(expandedEnhancedCommandParameterItem);
                this._enhancedParameterValueItems.Remove(expandedEnhancedCommandParameterItem);
            }
        }
        #endregion

        #region dgcParameterChildLength_SelectionChanged
        private void dgcParameterChildLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo selectedInfo;
            ComboBox comboBox;

            selectedInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            comboBox = e.Source as ComboBox;

            if (selectedInfo != null && comboBox != null && comboBox.SelectedValue != null)
            {
                selectedInfo.UseChildLength = bool.Parse(comboBox.SelectedValue.ToString());
            }
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

        // UserControl: Up, Down Event
        #region ctlParameterUpDown_OnFirst
        private void ctlParameterUpDown_OnFirst(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedEnhancedRemoteCommandParameterInfo selectedParameterInfo;
            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrEnhancedCommandParameter.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._enhancedParameterItems.Move(selectedIndex, 0);

                    selectedParameterInfo = selectedValueSet.ParameterItems[selectedIndex];
                    selectedValueSet.ParameterItems.RemoveAt(selectedIndex);
                    selectedValueSet.ParameterItems.Insert(0, selectedParameterInfo);
                }
            }
        }
        #endregion
        #region ctlParameterUpDown_OnUp
        private void ctlParameterUpDown_OnUp(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrEnhancedCommandParameter.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._enhancedParameterItems.Move(selectedIndex, selectedIndex - 1);
                    selectedValueSet.ParameterItems.Reverse(selectedIndex - 1, 2);
                }
            }
        }
        #endregion
        #region ctlParameterUpDown_OnDown
        private void ctlParameterUpDown_OnDown(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrEnhancedCommandParameter.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._enhancedParameterItems.Count - 1)
                {
                    this._enhancedParameterItems.Move(selectedIndex, selectedIndex + 1);
                    selectedValueSet.ParameterItems.Reverse(selectedIndex, 2);
                }
            }
        }
        #endregion
        #region ctlParameterUpDown_OnLast
        private void ctlParameterUpDown_OnLast(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            ExpandedEnhancedRemoteCommandParameterInfo selectedParameterInfo;
            int selectedIndex;

            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                selectedIndex = dgrEnhancedCommandParameter.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._enhancedParameterItems.Count - 1)
                {
                    this._enhancedParameterItems.Move(selectedIndex, this._enhancedParameterItems.Count - 1);

                    selectedParameterInfo = selectedValueSet.ParameterItems[selectedIndex];
                    selectedValueSet.ParameterItems.RemoveAt(selectedIndex);
                    selectedValueSet.ParameterItems.Add(selectedParameterInfo);
                }
            }
        }
        #endregion

        #region ctlValueUpDown_OnFirst
        private void ctlValueUpDown_OnFirst(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            ExpandedEnhancedRemoteCommandParameterItem selectedParameterItem;

            int selectedIndex;

            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (expandedEnhancedCommandParameterInfo != null && selectedIndex > 0)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, 0);
                selectedParameterItem = expandedEnhancedCommandParameterInfo.ValueItems[selectedIndex];
                expandedEnhancedCommandParameterInfo.ValueItems.RemoveAt(selectedIndex);
                expandedEnhancedCommandParameterInfo.ValueItems.Insert(0, selectedParameterItem);
            }
        }
        #endregion
        #region ctlValueUpDown_OnUp
        private void ctlValueUpDown_OnUp(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;

            int selectedIndex;

            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (expandedEnhancedCommandParameterInfo != null && selectedIndex > 0)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, selectedIndex - 1);
                expandedEnhancedCommandParameterInfo.ValueItems.Reverse(selectedIndex - 1, 2);
            }
        }
        #endregion
        #region ctlValueUpDown_OnDown
        private void ctlValueUpDown_OnDown(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;

            int selectedIndex;

            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (expandedEnhancedCommandParameterInfo != null && selectedIndex >= 0 && selectedIndex < this._enhancedParameterValueItems.Count - 1)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, selectedIndex + 1);
                expandedEnhancedCommandParameterInfo.ValueItems.Reverse(selectedIndex, 2);
            }
        }
        #endregion
        #region ctlValueUpDown_OnLast
        private void ctlValueUpDown_OnLast(object sender, EventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            ExpandedEnhancedRemoteCommandParameterItem selectedParameterItem;

            int selectedIndex;

            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            selectedIndex = dgrEnhancedCommandValue.SelectedIndex;

            if (expandedEnhancedCommandParameterInfo != null && selectedIndex >= 0 && selectedIndex < this._enhancedParameterValueItems.Count - 1)
            {
                this._enhancedParameterValueItems.Move(selectedIndex, this._enhancedParameterValueItems.Count - 1);
                selectedParameterItem = expandedEnhancedCommandParameterInfo.ValueItems[selectedIndex];
                expandedEnhancedCommandParameterInfo.ValueItems.RemoveAt(selectedIndex);
                expandedEnhancedCommandParameterInfo.ValueItems.Add(selectedParameterItem);
            }
        }
        #endregion

        // Button Event
        /*
        #region btnListTypeA_Click
        private void btnListTypeA_Click(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo selectedItem;
            ExpandedEnhancedRemoteCommandParameterItem valueItem;
            string confirmMessage;
            bool process;
            int count;

            selectedItem = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            valueItem = null;

            if (selectedItem != null)
            {
                if (selectedItem.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    process = true;

                    if (selectedItem.ValueItems.Count > 1)
                    {
                        confirmMessage = " Value item count is over than 1. \n\n Do you want to process?";

                        if (MessageBox.Show(confirmMessage, "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            process = false;
                        }
                    }

                    if (process == true)
                    {
                        selectedItem.ListType = EnhancedParameterListType.A;
                        btnListTypeA.IsChecked = true;
                        btnListTypeB.IsChecked = false;

                        if (selectedItem.ValueItems.Count > 1)
                        {
                            valueItem = selectedItem.ValueItems[0];
                            valueItem.Name = string.Empty;
                            valueItem.ListType = EnhancedParameterListType.A;

                            count = selectedItem.ValueItems.Count;

                            for (int i = count - 1; i > 0; i--)
                            {
                                this._enhancedParameterValueItems.RemoveAt(i);
                                selectedItem.ValueItems.RemoveAt(i);
                            }
                        }

                        dgcValueName.IsReadOnly = true;
                        dgcValueRule.IsReadOnly = true;
                    }
                    else
                    {
                        selectedItem.ListType = EnhancedParameterListType.B;

                        foreach (var tempValueItem in selectedItem.ValueItems)
                        {
                            tempValueItem.ListType = EnhancedParameterListType.B;
                        }

                        btnListTypeA.IsChecked = false;
                        btnListTypeB.IsChecked = true;

                        dgcValueName.IsReadOnly = false;
                        dgcValueRule.IsReadOnly = false;
                    }
                }
            }
        }
        #endregion
        #region btnListTypeB_Click
        private void btnListTypeB_Click(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo selectedItem;

            selectedItem = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;

            if (selectedItem != null)
            {
                if (selectedItem.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    selectedItem.ListType = EnhancedParameterListType.B;

                    foreach (var tempValueItem in selectedItem.ValueItems)
                    {
                        tempValueItem.ListType = EnhancedParameterListType.B;
                    }

                    btnListTypeA.IsChecked = false;
                    btnListTypeB.IsChecked = true;

                    dgcValueName.IsReadOnly = false;
                    dgcValueRule.IsReadOnly = false;
                }
            }
        }
        #endregion
        */
        #region btnTriggerEdit_Click
        private void btnTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Settings.TriggerWindow triggerWindow;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            
            expandedEnhancedRemoteCommandInfo = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;
            
            if (expandedEnhancedRemoteCommandInfo != null)
            {
                triggerWindow = new Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Enhanced Remote Command", expandedEnhancedRemoteCommandInfo.RemoteCommand, expandedEnhancedRemoteCommandInfo.TriggerCollection);
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
                this._messageProcessor.RemoteCommandCollection.EnhancedRemoteCommandItems.Clear();

                foreach (var commandItem in this._enhancedCommandItems)
                {
                    this._messageProcessor.RemoteCommandCollection.EnhancedRemoteCommandItems.Add(commandItem);
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
                        this._childChanged = false;
                    }
                }
                else
                {
                    this._messageProcessor.IsDirty = true;
                    this._childChanged = false;
                }
            }
        }
        #endregion
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (this._childChanged == true)
            {
                MessageBoxResult msgResult = MessageBox.Show("Some data is changed.\n\nDo you want ignore changed data?", "Warning", MessageBoxButton.YesNo);

                if (msgResult == MessageBoxResult.Yes)
                {
                    Close();
                }
                else
                {
                    if (IsValid(out string errorText) == false)
                    {
                        MessageBox.Show(errorText);
                    }
                    else
                    {
                        this._messageProcessor.RemoteCommandCollection.EnhancedRemoteCommandItems.Clear();

                        foreach (var commandItem in this._enhancedCommandItems)
                        {
                            this._messageProcessor.RemoteCommandCollection.EnhancedRemoteCommandItems.Add(commandItem);
                        }
                    }

                    DialogResult = this._childChanged;
                }
            }
            else
            {
                Close();
            }
        }
        #endregion

        #region btnChangeValueSet_Click
        private void btnChangeValueSet_Click(object sender, RoutedEventArgs e)
        {
            ValueSetNameEditWindow window;
            ExpandedEnhancedRemoteCommandInfo selectedCMD;
            List<ValueSetNameEditWindow.DisplayData> valueSetNames;
            int valueSetNameIndex;

            selectedCMD = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;

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
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;
            selectedValueSet = cboValueSetName.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;

            if (selectedValueSet != null)
            {
                this._enhancedParameterItems = new ObservableCollection<ExpandedEnhancedRemoteCommandParameterInfo>(selectedValueSet.ParameterItems);

                dgrEnhancedCommandParameter.ItemsSource = this._enhancedParameterItems;
            }
        }
        #endregion
        #region ValueSetNameWindow_ValueSetNameChanged
        private void ValueSetNameWindow_ValueSetNameChanged(List<ValueSetNameEditWindow.DisplayData> changedData)
        {
            ExpandedEnhancedRemoteCommandValueSetCollection newCollection;
            ExpandedEnhancedRemoteCommandInfo selectedCMD;
            ExpandedEnhancedRemoteCommandValueSetInfo valueSetInfo;
            int valueSetNameIndex;

            newCollection = new ExpandedEnhancedRemoteCommandValueSetCollection();
            selectedCMD = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;

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
            //List<string> usedValueNames;
            AutoSendTrigger invalidTrigger;

            SECSItemFormat dataIDFormat;
            SECSItemFormat objSpecFormat;

            dynamic converted;

            result = true;
            errorText = string.Empty;

            usedCommandNames = new List<string>();
            invalidTrigger = null;

            dataIDFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.DATAID, SECSItemFormat.U4);
            objSpecFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.OBJSPEC, SECSItemFormat.A);

            foreach (var commandItem in this._enhancedCommandItems)
            {
                if (commandItem.AutoSend == true)
                {
                    if (commandItem.TriggerCollection.Items.Count == 0)
                    {
                        result = false;
                        errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n AutoSend selected, but Trigger is not selected";
                    }
                }

                if (result == true && commandItem.TriggerCollection.ValidateTriggers(out errorText, out invalidTrigger) == false)
                {
                    result = false;
                    errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n Trigger invalid, \n\n {invalidTrigger.ToString()}";
                }

                if (result == true)
                {
                    if ((dataIDFormat == SECSItemFormat.A || dataIDFormat == SECSItemFormat.J) == false)
                    {
                        if (string.IsNullOrEmpty(commandItem.DataID) == false && this._messageProcessor.ConvertValue(dataIDFormat, commandItem.DataID) == null)
                        {
                            result = false;
                            errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n DATAID convert fail";
                        }
                    }
                }

                if (result == true && this._messageProcessor.ConvertValue(objSpecFormat, commandItem.ObjSpec) == null)
                {
                    result = false;
                    errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n OBJSPEC convert fail";
                }

                if (result == true && string.IsNullOrEmpty(commandItem.RemoteCommand) == true)
                {
                    result = false;
                    errorText = $"No name command exists";
                }

                if (result == true)
                {
                    if (usedCommandNames.Contains(commandItem.RemoteCommand) == true)
                    {
                        result = false;
                        errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n name is dupelicated";
                    }
                    else
                    {
                        usedCommandNames.Add(commandItem.RemoteCommand);
                    }
                }

                if (result == true)
                {
                    foreach (ExpandedEnhancedRemoteCommandValueSetInfo valueSet in commandItem.ValueSetCollection.Items.Values)
                    {
                        //usedParameterNames = new List<string>();

                        foreach (var parameterItem in valueSet.ParameterItems)
                        {
                            if (parameterItem.Format == SECSItemFormat.X || parameterItem.Format == SECSItemFormat.None)
                            {
                                result = false;
                                errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n format is not selected";
                            }

                            if (result == true && string.IsNullOrEmpty(parameterItem.Name) == true)
                            {
                                result = false;
                                errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n no name parameter exist";
                            }

                            if (result == true && string.IsNullOrEmpty(parameterItem.Value) == false)
                            {
                                if (parameterItem.Format != SECSItemFormat.L && parameterItem.Format != SECSItemFormat.A && parameterItem.Format != SECSItemFormat.J && parameterItem.Count > 0)
                                {
                                    converted = this._messageProcessor.ConvertValue(parameterItem.Format, parameterItem.Count, parameterItem.Value);

                                    if (converted == null)
                                    {
                                        result = false;
                                        errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n Value convert failed.";
                                    }
                                }
                            }

                            if (result == true && string.IsNullOrEmpty(parameterItem.GenerateRule) == false)
                            {
                                if (this._messageProcessor.IsValidGenerateRule(parameterItem.Format, parameterItem.GenerateRule) == false)
                                {
                                    result = false;
                                    errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n Rule verify fail";
                                }
                            }

                            /*
                            if (result == true)
                            {
                                if (usedParameterNames.Contains(parameterItem.Name) == true)
                                {
                                    result = false;
                                    errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter: {parameterItem.Name} \n\n name is dupelicated";
                                }
                                else
                                {
                                    usedParameterNames.Add(parameterItem.Name);
                                }
                            }
                            */

                            if (result == true)
                            {
                                //usedValueNames = new List<string>();

                                foreach (var valueItem in parameterItem.ValueItems)
                                {
                                    if (valueItem.Format == SECSItemFormat.X || valueItem.Format == SECSItemFormat.None)
                                    {
                                        result = false;
                                        errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n Value Name: {valueItem.Name} \n\n format is not selected";
                                    }

                                    if (result == true && string.IsNullOrEmpty(valueItem.Value) == false && valueItem.Format != SECSItemFormat.L && valueItem.Format != SECSItemFormat.A && valueItem.Format == SECSItemFormat.J)
                                    {
                                        valueItem.Count = valueItem.Value.Count(t => t == ' ') + 1;

                                        converted = this._messageProcessor.ConvertValue(valueItem.Format, valueItem.Count, valueItem.Value);

                                        if (converted == null)
                                        {
                                            if (valueItem.Value != string.Empty)
                                            {
                                                result = false;
                                                errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n Value Name: {valueItem.Name} \n\n Value convert failed.";
                                            }
                                        }
                                    }

                                    if (result == true && string.IsNullOrEmpty(valueItem.GenerateRule) == false)
                                    {
                                        if (this._messageProcessor.IsValidGenerateRule(valueItem.Format, valueItem.GenerateRule) == false)
                                        {
                                            result = false;
                                            errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n Value Name: {valueItem.Name} Rule verify fail";
                                        }
                                    }

                                    /*
                                    if (result == true && string.IsNullOrEmpty(valueItem.Name) == true && valueItem.Format == SECSItemFormat.L)
                                    {
                                        result = false;
                                        errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n no name value exist";
                                    }
                                    */

                                    /*
                                    if (result == true)
                                    {
                                        if (usedValueNames.Contains(valueItem.Name) == true)
                                        {
                                            result = false;
                                            errorText = $" Remote Command: {commandItem.RemoteCommand} \n\n ValueSet Name: {valueSet.Name} \n\n Parameter Name: {parameterItem.Name} \n\n \n\n Value: {valueItem.Name} \n\n name is dupelicated";
                                        }
                                        else
                                        {
                                            usedValueNames.Add(valueItem.Name);
                                        }
                                    }
                                    */
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region FillDefault
        private void FillDefault(ExpandedEnhancedRemoteCommandValueSetInfo valueSetInfo)
        {
            if (valueSetInfo != null)
            {
                foreach (ExpandedEnhancedRemoteCommandParameterInfo parameterInfo in valueSetInfo.ParameterItems)
                {
                    switch (parameterInfo.Format)
                    {
                        case SECSItemFormat.None:
                        case SECSItemFormat.A:
                        case SECSItemFormat.J:
                        case SECSItemFormat.X:
                            break;
                        case SECSItemFormat.Boolean:
                            if (string.IsNullOrEmpty(parameterInfo.Value) == true)
                            {
                                parameterInfo.Value = false.ToString();
                            }
                            break;
                        case SECSItemFormat.L:
                            foreach (ExpandedEnhancedRemoteCommandParameterItem parameterItem in parameterInfo.ValueItems)
                            {
                                FillDefault(parameterItem);
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
        #region FillDefault
        private void FillDefault(ExpandedEnhancedRemoteCommandParameterItem item)
        {
            if (item != null)
            {
                switch (item.Format)
                {
                    case SECSItemFormat.None:
                    case SECSItemFormat.A:
                    case SECSItemFormat.J:
                    case SECSItemFormat.X:
                        break;
                    case SECSItemFormat.Boolean:
                        if (string.IsNullOrEmpty(item.Value) == true)
                        {
                            item.Value = false.ToString();
                        }
                        break;
                    case SECSItemFormat.L:
                        foreach (ExpandedEnhancedRemoteCommandParameterItem childItem in item.ChildParameterItem)
                        {
                            FillDefault(childItem);
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(item.Value) == true)
                        {
                            item.Value = 0.ToString();
                        }
                        break;
                }
            }
        }
        #endregion

        private void btnChildEdit_Click(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;
            CommandParameterListWindow window;
            List<ExpandedEnhancedRemoteCommandParameterItem> parameterStack;

            bool? childChanged;

            expandedEnhancedRemoteCommandInfo = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;
            expandedEnhancedCommandParameterInfo = dgrEnhancedCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            expandedEnhancedCommandParameterItem = dgrEnhancedCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;

            if (expandedEnhancedRemoteCommandInfo != null && expandedEnhancedCommandParameterInfo != null && expandedEnhancedCommandParameterItem != null
                && expandedEnhancedCommandParameterItem.Format == SECSItemFormat.L)
            {
                parameterStack = new List<ExpandedEnhancedRemoteCommandParameterItem>
                {
                    expandedEnhancedCommandParameterItem
                };

                window = new CommandParameterListWindow();
                window.Initialize(this._messageProcessor, expandedEnhancedRemoteCommandInfo.RemoteCommand, expandedEnhancedCommandParameterInfo, parameterStack);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = this;
                childChanged = window.ShowDialog();

                if (childChanged.HasValue == true && childChanged.Value == true)
                {
                    this._childChanged = true;
                }
            }
        }
    }
}