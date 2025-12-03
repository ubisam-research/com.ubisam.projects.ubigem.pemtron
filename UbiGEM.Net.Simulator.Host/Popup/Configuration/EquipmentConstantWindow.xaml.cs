using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

using System.Windows.Input;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// EquipmentConstantWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EquipmentConstantWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;

        private List<ExpandedVariableInfo> _currentVariableList;
        private ObservableCollection<ExpandedVariableInfo> _displayInfo;
        #endregion

        #region Constructor
        public EquipmentConstantWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;
            Style rowStyle;

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

            rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                     new MouseButtonEventHandler(dgrEquipmentConstant_RowDoubleClick)));
            dgrEquipmentConstant.RowStyle = rowStyle;

            this._displayInfo = new ObservableCollection<ExpandedVariableInfo>();
            this._currentVariableList = new List<ExpandedVariableInfo>();

            foreach (var item in this._messageProcessor.VariableCollection.ECV.Items.OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;
                expandedVariableInfo = expandedVariableInfo.Clone();
                if(expandedVariableInfo.VID == expandedVariableInfo.Name)
                {
                    expandedVariableInfo.VID = string.Empty;
                }
                this._displayInfo.Add(expandedVariableInfo);
                this._currentVariableList.Add(expandedVariableInfo);
            }

            dgrEquipmentConstant.ItemsSource = this._displayInfo;
        }
        #endregion

        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            expandedVariableInfo = new ExpandedVariableInfo()
            {
                IsInheritance = false,
                VID = string.Empty,
                IsUse = true,
                VIDType = Structure.VariableType.ECV,
                Format = UbiCom.Net.Structure.SECSItemFormat.A
            };

            this._displayInfo.Add(expandedVariableInfo);
            dgrEquipmentConstant.SelectedItem = expandedVariableInfo;
            dgrEquipmentConstant.ScrollIntoView(expandedVariableInfo);
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            expandedVariableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;

            if (expandedVariableInfo != null)
            {
                if (expandedVariableInfo.IsInheritance == false)
                {
                    this._displayInfo.Remove(expandedVariableInfo);
                }
            }
        }
        #endregion
        #region dgrEquipmentConstant_SelectionChanged
        private void dgrEquipmentConstant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;
            btnListEdit.Visibility = Visibility.Collapsed;

            expandedVariableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;

            if (expandedVariableInfo != null)
            {
                if (expandedVariableInfo.IsInheritance == true)
                {
                    dgcVID.IsReadOnly = true;
                    dgcName.IsReadOnly = true;
                    dgcLength.IsReadOnly = true;
                    dgcDescription.IsReadOnly = true;
                    ctlFunction.ChangeButtonEnabled(true, false);
                }
                else
                {
                    dgcVID.IsReadOnly = false;
                    dgcName.IsReadOnly = false;
                    dgcLength.IsReadOnly = false;
                    dgcDescription.IsReadOnly = false;
                    ctlFunction.ChangeButtonEnabled(true, true);

                }

                if (expandedVariableInfo.Format == SECSItemFormat.L)
                {
                    dgcLength.IsReadOnly = true;
                    btnListEdit.Visibility = Visibility.Visible;
                }
            }
        }
        #endregion
        #region dgcFormat_SelectionChanged
        private void dgcFormat_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox;
            ExpandedVariableInfo expandedVariableInfo;
            SECSItemFormat format;

            if (dgrEquipmentConstant.SelectedItem != null && e.Source != null)
            {
                expandedVariableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;
                comboBox = e.Source as ComboBox;

                if (expandedVariableInfo != null && comboBox != null && comboBox.SelectedItem != null && Enum.TryParse(comboBox.SelectedItem.ToString(), out format) == true)
                {
                    expandedVariableInfo.Format = format;
                }
            }
        }
        #endregion
        #region dgrEquipmentConstant_RowDoubleClick
        private void dgrEquipmentConstant_RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExpandedVariableInfo variableInfo;

            variableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null && variableInfo.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                btnListEdit.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }
        #endregion
        #region btnListEdit_Click
        private void btnListEdit_Click(object sender, RoutedEventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            VariableListWindow variableListWindow;
            List<ExpandedVariableInfo> parents;

            variableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null && variableInfo.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                parents = new List<ExpandedVariableInfo>();
                parents.Add(variableInfo);

                variableListWindow = new VariableListWindow();
                variableListWindow.Initialize(variableInfo, this._currentVariableList, parents, true);

                variableListWindow.Owner = this;
                variableListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                variableListWindow.ShowDialog();

                variableInfo.Length = variableInfo.ChildVariables.Count;
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            Structure.GemDriverError error;
            Structure.VariableInfo variableInfo;
            ExpandedVariableInfo expandedVariableInfo;

            if (IsValid(out errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                dgrEquipmentConstant.SelectedItem = null;

                foreach (var item in this._displayInfo)
                {
                    if (string.IsNullOrEmpty(item.VID) == false)
                    {
                        variableInfo = this._messageProcessor.VariableCollection.ECV[item.VID];
                    }
                    else
                    {
                        variableInfo = this._messageProcessor.VariableCollection.Items.FirstOrDefault(t => t.Name == item.Name);
                    }

                    if (variableInfo != null)
                    {
                        expandedVariableInfo = variableInfo as ExpandedVariableInfo;
                        expandedVariableInfo.Name = item.Name;
                        expandedVariableInfo.Format = item.Format;

                        if (item.Format == SECSItemFormat.L)
                        {
                            expandedVariableInfo.Length = item.ChildVariables.Count;
                        }
                        else
                        {
                            expandedVariableInfo.Length = item.Length;
                        }

                        if (item.Format != UbiCom.Net.Structure.SECSItemFormat.L)
                        {
                            expandedVariableInfo.Value = item.Value;
                            this._messageProcessor.UpdateSystemConfig(expandedVariableInfo.Name, item.Value);
                        }
                        else
                        {
                            variableInfo.ChildVariables.Clear();
                            foreach (var childItem in item.ChildVariables)
                            {
                                variableInfo.ChildVariables.Add(childItem);
                            }
                        }
                    }
                    else
                    {
                        expandedVariableInfo = item.Clone();

                        if (string.IsNullOrEmpty(expandedVariableInfo.VID) == true)
                        {
                            expandedVariableInfo.VID = expandedVariableInfo.Name;
                        }

                        this._messageProcessor.VariableCollection.Add(expandedVariableInfo);
                    }
                }

                dgrEquipmentConstant.Items.Refresh();

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

        // Private Method
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            List<string> usedIDs;
            List<string> usedNames;
            int count;
            dynamic converted;
            StringBuilder sbConverted;

            result = true;
            errorText = string.Empty;
            usedIDs = new List<string>();
            usedNames = new List<string>();
            sbConverted = new StringBuilder();

            foreach (var item in this._displayInfo)
            {
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (string.IsNullOrEmpty(item.Name) == true)
                    {
                        errorText = string.Format("Invalid Name, ECID {0}", item.VID == null ? string.Empty : item.VID);
                        result = false;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (item.VID.Length > 0 && this._messageProcessor.VariableCollection.Variables[item.VID] != null)
                    {
                        errorText = string.Format("Dupelicated Variable ID: {0}", item.VID);
                        result = false;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (item.VID.Length > 0 && this._messageProcessor.VariableCollection.Variables.GetVariableInfo(item.Name) != null)
                    {
                        errorText = string.Format("Dupelicated Variable Name: {0}", item.Name);
                        result = false;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (item.Format != SECSItemFormat.L && item.Format != SECSItemFormat.A && item.Format != SECSItemFormat.J && item.Value.Length > 0)
                    {
                        item.Value = item.Value.Trim();
                        count = item.Length;

                        converted = null;

                        if (count == -1)
                        {
                            string[] token = item.Value.Split(' ');

                            int localCount = token.Length;

                            converted = this._messageProcessor.ConvertValue(item.Format, localCount, item.Value);
                        }
                        else if (count > 0)
                        {
                            converted = this._messageProcessor.ConvertValue(item.Format, count, item.Value);
                        }

                        if (converted == null)
                        {
                            errorText = string.Format("Value convert fail. ECName: {0}", item.Name);
                            result = false;
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (usedIDs.Contains(item.VID) == true)
                    {
                        errorText = string.Format("Dupelicated ECID: {0}", item.VID);
                        result = false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.VID) == false)
                        {
                            usedIDs.Add(item.VID);
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (usedNames.Contains(item.Name) == true)
                    {
                        errorText = string.Format("Dupelicated ECName: {0}", item.Name);
                        result = false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.Name) == false)
                        {
                            usedNames.Add(item.Name);
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}