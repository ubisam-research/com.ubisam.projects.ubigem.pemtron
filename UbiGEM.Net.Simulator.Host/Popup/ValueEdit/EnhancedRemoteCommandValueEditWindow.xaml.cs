using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup.ValueEdit
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EnhancedRemoteCommandValueEditWindow : Window
    {
        #region MemberVariable
        private ExpandedEnhancedRemoteCommandInfo _expandedEnhancedRemoteCommandInfo;
        private List<ExpandedEnhancedRemoteCommandParameterInfo> _displayParameterItems;
        private MessageProcessor _messageProcessor;

        private bool _dataEdited;
        #endregion
        #region Constructor
        public EnhancedRemoteCommandValueEditWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            
            if (this._expandedEnhancedRemoteCommandInfo == null)
            {
                MessageBox.Show("ercmdInfo is not initialized");
                Close();
            }
            else
            {
                if (this._expandedEnhancedRemoteCommandInfo.RemoteCommand != null)
                {
                    lblName.Content = this._expandedEnhancedRemoteCommandInfo.RemoteCommand.Replace("_", "__");
                }
                else
                {
                    lblName.Content = string.Empty;
                }

                lblValueSet.Content = this._expandedEnhancedRemoteCommandInfo.SelectedValueSet.Name;

                if (this._expandedEnhancedRemoteCommandInfo.Description != null)
                {
                    lblDescription.Content = this._expandedEnhancedRemoteCommandInfo.Description.Replace("_", "__");
                }
                else
                {
                    lblDescription.Content = string.Empty;
                }

                txtDataID.Text = this._expandedEnhancedRemoteCommandInfo.DataID;
                txtObjSpec.Text = this._expandedEnhancedRemoteCommandInfo.ObjSpec;

                this._displayParameterItems = new List<ExpandedEnhancedRemoteCommandParameterInfo>();

                foreach (var parameter in this._expandedEnhancedRemoteCommandInfo.SelectedValueSet.ParameterItems)
                {
                    this._displayParameterItems.Add(parameter.Clone());
                }

                dgrCommandParameter.ItemsSource = this._displayParameterItems;
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            Structure.GemDriverError error;

            SECSItemFormat dataIDFormat;
            SECSItemFormat objSpecFormat;
            dataIDFormat = this._messageProcessor.GetSECSFormat(Info.DataDictinaryList.DATAID, SECSItemFormat.U4);
            objSpecFormat = this._messageProcessor.GetSECSFormat(Info.DataDictinaryList.OBJSPEC, SECSItemFormat.A);

            object converted;

            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;

            errorText = string.Empty;

            if (string.IsNullOrEmpty(this.txtDataID.Text) == false)
            {
                converted = this._messageProcessor.ConvertValue(dataIDFormat, this.txtDataID.Text);

                if (converted == null)
                {
                    errorText = "DATAID convert fail. \n\n format: " + dataIDFormat.ToString();
                }
            }

            if (string.IsNullOrEmpty(errorText) == true)
            {
                converted = this._messageProcessor.ConvertValue(objSpecFormat, this.txtObjSpec.Text);

                if (converted == null)
                {
                    errorText = "OBJSPEC convert fail. \n\n format: " + objSpecFormat.ToString();
                }
            }

            if (string.IsNullOrEmpty(errorText) == true)
            {
                foreach (var parameterItem in this._displayParameterItems)
                {
                    if (string.IsNullOrEmpty(parameterItem.GenerateRule) == false)
                    {
                        if (this._messageProcessor.IsValidGenerateRule(parameterItem.Format, parameterItem.GenerateRule) == false)
                        {
                            errorText = string.Format("Rule of [Parameter Name: {0}] verify fail", parameterItem.Name);
                            break;
                        }
                    }

                    if (parameterItem.Format != SECSItemFormat.L)
                    {
                        converted = this._messageProcessor.ConvertValue(parameterItem.Format, parameterItem.Count, parameterItem.Value);

                        if (converted == null)
                        {
                            if (parameterItem.Value != string.Empty)
                            {
                                errorText = string.Format("Error: {0} can not convert", parameterItem.Name);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var valueItem in parameterItem.ValueItems)
                        {
                            if (string.IsNullOrEmpty(valueItem.GenerateRule) == false)
                            {
                                if (this._messageProcessor.IsValidGenerateRule(valueItem.Format, valueItem.GenerateRule) == false)
                                {
                                    errorText = string.Format("Rule of [Value Name: [{0}] of [Parameter Name: {1}] verify fail", valueItem.Name, parameterItem.Name);
                                    break;
                                }
                            }

                            if (valueItem.Format != SECSItemFormat.L)
                            {
                                if (string.IsNullOrEmpty(valueItem.Value) == false)
                                {
                                    if (valueItem.Format != SECSItemFormat.A && valueItem.Format != SECSItemFormat.J)
                                    {
                                        converted = this._messageProcessor.ConvertValue(valueItem.Format, valueItem.Count, valueItem.Value);

                                        if (converted == null)
                                        {
                                            if (valueItem.Value != string.Empty)
                                            {
                                                errorText = string.Format("Error: {0} can not convert", valueItem.Name);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (valueItem.Format != SECSItemFormat.A && valueItem.Format != SECSItemFormat.J)
                                    {
                                        valueItem.Count = valueItem.Value.Count(t => t == ' ') + 1;

                                        converted = this._messageProcessor.ConvertValue(valueItem.Format, valueItem.Count, valueItem.Value);

                                        if (converted == null)
                                        {
                                            if (valueItem.Value != string.Empty)
                                            {
                                                errorText = string.Format("Error: {0} can not convert", valueItem.Name);
                                                break;
                                            }
                                        }
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
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                this._expandedEnhancedRemoteCommandInfo.DataID = txtDataID.Text;
                this._expandedEnhancedRemoteCommandInfo.ObjSpec = txtObjSpec.Text;

                foreach (var item in this._displayParameterItems)
                {
                    expandedEnhancedCommandParameterInfo = this._expandedEnhancedRemoteCommandInfo.SelectedValueSet.ParameterItems.FirstOrDefault(t => t.Name == item.Name);

                    if (expandedEnhancedCommandParameterInfo != null)
                    {
                        if (expandedEnhancedCommandParameterInfo.Format != SECSItemFormat.L)
                        {
                            expandedEnhancedCommandParameterInfo.Count = item.Count;
                            expandedEnhancedCommandParameterInfo.GenerateRule = item.GenerateRule;
                            expandedEnhancedCommandParameterInfo.Value = item.Value;
                        }
                        else
                        {
                            expandedEnhancedCommandParameterInfo.UseChildLength = item.UseChildLength;
                            expandedEnhancedCommandParameterInfo.ValueItems.Clear();
                            expandedEnhancedCommandParameterInfo.ValueItems.AddRange(item.ValueItems);
                        }
                    }
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
                        this._dataEdited = false;
                    }
                }
                else
                {
                    this._messageProcessor.IsDirty = true;
                    this._dataEdited = true;
                }
            }
        }
        #endregion
        #region btnCancel_Click
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            bool dataEdited = false;

            if (this._dataEdited == false)
            {
                foreach (var item in this._displayParameterItems)
                {
                    if (dataEdited == true)
                    {
                        break;
                    }

                    var target = this._expandedEnhancedRemoteCommandInfo.SelectedValueSet.ParameterItems.FirstOrDefault(t => t.Name == item.Name);

                    if (target != null)
                    {
                        if (target.Count != item.Count)
                        {
                            dataEdited = true;
                        }
                        else if (target.Value != item.Value)
                        {
                            dataEdited = true;
                        }
                        else if (target.GenerateRule != item.GenerateRule)
                        {
                            dataEdited = true;
                        }
                    }
                }

                if (dataEdited == true)
                {
                    MessageBoxResult msgResult = MessageBox.Show("Some data is changed.\n\nDo you want ignore changed data?", "Warning", MessageBoxButton.YesNo);

                    if (msgResult == MessageBoxResult.Yes)
                    {
                        this._dataEdited = false;
                    }
                    else
                    {
                        this._dataEdited = true;
                        foreach (var item in this._displayParameterItems)
                        {
                            var target = this._expandedEnhancedRemoteCommandInfo.SelectedValueSet.ParameterItems.FirstOrDefault(t => t.Name == item.Name);

                            if (target != null)
                            {
                                target.Count = item.Count;
                                target.GenerateRule = item.GenerateRule;
                                target.Value = item.Value;
                            }
                        }
                    }
                }
            }

            DialogResult = this._dataEdited;

            Close();
        }
        #endregion

        // DataGrid Event
        #region dgrCommandParameter_SelectionChanged
        private void dgrCommandParameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;

            dgrCommandValue.IsEnabled = false;

            if (dgrCommandParameter.SelectedItem != null && dgrCommandParameter.SelectedItem is ExpandedEnhancedRemoteCommandParameterInfo == true)
            {
                expandedEnhancedCommandParameterInfo = dgrCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;

                if (expandedEnhancedCommandParameterInfo.Format == SECSItemFormat.L)
                {
                    dgrCommandValue.ItemsSource = expandedEnhancedCommandParameterInfo.ValueItems;
                    dgrCommandValue.IsEnabled = true;
                }
                else
                {
                    dgrCommandValue.ItemsSource = null;
                    dgrCommandValue.IsEnabled = false;
                }
            }
        }
        #endregion
        #region btnChildEdit_Click
        private void btnChildEdit_Click(object sender, RoutedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo expandedEnhancedCommandParameterInfo;
            ExpandedEnhancedRemoteCommandParameterItem expandedEnhancedCommandParameterItem;
            CommandParameterValueEditListWindow window;
            List<ExpandedEnhancedRemoteCommandParameterItem> parameterStack;

            expandedEnhancedCommandParameterInfo = dgrCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
            expandedEnhancedCommandParameterItem = dgrCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;

            if (this._expandedEnhancedRemoteCommandInfo != null && expandedEnhancedCommandParameterInfo != null && expandedEnhancedCommandParameterItem != null
                && expandedEnhancedCommandParameterItem.Format == SECSItemFormat.L)
            {
                parameterStack = new List<ExpandedEnhancedRemoteCommandParameterItem>
                {
                    expandedEnhancedCommandParameterItem
                };

                window = new CommandParameterValueEditListWindow();
                window.Initialize(this._messageProcessor, this._expandedEnhancedRemoteCommandInfo.RemoteCommand, expandedEnhancedCommandParameterInfo, parameterStack);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    this._dataEdited = true;
                }
            }
        }
        #endregion
        #region dgcParameterChildLength_SelectionChanged
        private void dgcParameterChildLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedEnhancedRemoteCommandParameterInfo selectedInfo;
            ComboBox comboBox;

            selectedInfo = dgrCommandParameter.SelectedItem as ExpandedEnhancedRemoteCommandParameterInfo;
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

            selectedItem = dgrCommandValue.SelectedItem as ExpandedEnhancedRemoteCommandParameterItem;
            comboBox = e.Source as ComboBox;

            if (selectedItem != null && comboBox != null && comboBox.SelectedValue != null)
            {
                selectedItem.UseChildLength = bool.Parse(comboBox.SelectedValue.ToString());
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo)
        {
            this._messageProcessor = messageProcessor;
            this._expandedEnhancedRemoteCommandInfo = expandedEnhancedRemoteCommandInfo;
        }
        #endregion
    }
}