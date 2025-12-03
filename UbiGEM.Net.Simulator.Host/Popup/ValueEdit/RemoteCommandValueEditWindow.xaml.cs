using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup.ValueEdit
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RemoteCommandValueEditWindow : Window
    {
        #region MemberVariable
        private ExpandedRemoteCommandInfo _expandedRemoteCommandInfo;
        private List<ExpandedRemoteCommandParameterInfo> _displayParameterItems;

        private MessageProcessor _messageProcessor;

        private bool _dataEdited;
        #endregion
        #region Constructor
        public RemoteCommandValueEditWindow()
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

            if (this._expandedRemoteCommandInfo == null)
            {
                MessageBox.Show("rcmdInfo is not initialized");
                Close();
            }
            else
            {
                if (this._expandedRemoteCommandInfo.RemoteCommand != null)
                {
                    lblName.Content = this._expandedRemoteCommandInfo.RemoteCommand.Replace("_", "__");
                }
                else
                {
                    lblName.Content = string.Empty;
                }

                lblValueSet.Content = this._expandedRemoteCommandInfo.SelectedValueSet.Name;

                if (this._expandedRemoteCommandInfo.Description != null)
                {
                    lblDescription.Content = this._expandedRemoteCommandInfo.Description.Replace("_", "__");
                }
                else
                {
                    lblDescription.Content = string.Empty;
                }

                this._displayParameterItems = new List<ExpandedRemoteCommandParameterInfo>();

                foreach (var item in this._expandedRemoteCommandInfo.SelectedValueSet.ParameterItems)
                {
                    this._displayParameterItems.Add(item.Clone());
                }

                dgrCommandParameters.ItemsSource = this._displayParameterItems;
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            dynamic converted;
            Structure.GemDriverError error;

            errorText = string.Empty;

            if (string.IsNullOrEmpty(errorText) == true)
            {
                foreach (var parameterItem in this._displayParameterItems)
                {
                    if (string.IsNullOrEmpty(parameterItem.Value) == false)
                    {
                        if (parameterItem.Format != SECSItemFormat.A && parameterItem.Format != SECSItemFormat.J && parameterItem.Count > 0)
                        {
                            converted = this._messageProcessor.ConvertValue(parameterItem.Format, parameterItem.Count, parameterItem.Value);

                            if (converted == null)
                            {
                                errorText = string.Format(" Parameter Name: {0} \n\n Value convert failed.", parameterItem.Name);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(errorText) == true && string.IsNullOrEmpty(parameterItem.GenerateRule) == false)
                    {
                        if (this._messageProcessor.IsValidGenerateRule(parameterItem.Format, parameterItem.GenerateRule) == false)
                        {
                            errorText = string.Format("Rule of [Parameter Name: {0}] verify fail", parameterItem.Name);
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
                foreach (var item in this._displayParameterItems)
                {
                    var target = this._expandedRemoteCommandInfo.SelectedValueSet.ParameterItems.FirstOrDefault(t => t.Name == item.Name);

                    if (target != null)
                    {
                        target.Count = item.Count;
                        target.GenerateRule = item.GenerateRule;
                        target.Value = item.Value;
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
                    
                    var target = this._expandedRemoteCommandInfo.SelectedValueSet.ParameterItems.FirstOrDefault(t => t.Name == item.Name);

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
                            var target = this._expandedRemoteCommandInfo.SelectedValueSet.ParameterItems.FirstOrDefault(t => t.Name == item.Name);

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
        #region btnDefaultValueSet_Click
        private void btnDefaultValueSet_Click(object sender, RoutedEventArgs e)
        {
            if (this._displayParameterItems != null)
            {
                foreach (ExpandedRemoteCommandParameterInfo parameterInfo in this._displayParameterItems)
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

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, ExpandedRemoteCommandInfo expandedRemoteCommandInfo)
        {
            this._messageProcessor = messageProcessor;
            this._expandedRemoteCommandInfo = expandedRemoteCommandInfo;
        }
        #endregion
    }
}