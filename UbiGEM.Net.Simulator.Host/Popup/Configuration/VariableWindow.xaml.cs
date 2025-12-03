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
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// VariableWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VariableWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;

        private List<ExpandedVariableInfo> _currentVariableList;
        private ObservableCollection<ExpandedVariableInfo> _displayInfo;
        #endregion
        #region Constructor
        public VariableWindow()
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
                                     new MouseButtonEventHandler(dgrVariable_RowDoubleClick)));
            dgrVariable.RowStyle = rowStyle;

            this._displayInfo = new ObservableCollection<ExpandedVariableInfo>();
            this._currentVariableList = new List<ExpandedVariableInfo>();

            foreach (var item in this._messageProcessor.VariableCollection.SV.Items.OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;
                expandedVariableInfo = expandedVariableInfo.Clone();

                if (expandedVariableInfo.VID == expandedVariableInfo.Name)
                {
                    expandedVariableInfo.VID = string.Empty;
                }

                this._displayInfo.Add(expandedVariableInfo);
                this._currentVariableList.Add(expandedVariableInfo);
            }

            foreach (var item in this._messageProcessor.VariableCollection.DVVal.Items.OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;
                expandedVariableInfo = expandedVariableInfo.Clone();

                if (expandedVariableInfo.VID == expandedVariableInfo.Name)
                {
                    expandedVariableInfo.VID = string.Empty;
                }

                this._displayInfo.Add(expandedVariableInfo);
                this._currentVariableList.Add(expandedVariableInfo);
            }

            foreach (var item in this._messageProcessor.VariableCollection.ECV.Items.OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;
                expandedVariableInfo = expandedVariableInfo.Clone();

                if (expandedVariableInfo.VID == expandedVariableInfo.Name)
                {
                    expandedVariableInfo.VID = string.Empty;
                }

                this._currentVariableList.Add(expandedVariableInfo);
            }

            dgrVariable.ItemsSource = this._displayInfo;
        }
        #endregion

        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            expandedVariableInfo = new ExpandedVariableInfo()
            {
                VID = string.Empty,
                IsInheritance = false,
                IsUse = true,
                VIDType = Structure.VariableType.DVVAL
            };

            this._displayInfo.Add(expandedVariableInfo);
            dgrVariable.SelectedItem = expandedVariableInfo;
            dgrVariable.ScrollIntoView(expandedVariableInfo);
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            expandedVariableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

            if (expandedVariableInfo != null)
            {
                if (expandedVariableInfo.IsInheritance == false)
                {
                    this._displayInfo.Remove(expandedVariableInfo);
                }
            }
        }
        #endregion
        #region dgrVariable_SelectionChanged
        private void dgrVariable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            expandedVariableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

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
                else
                {
                    btnListEdit.Visibility = Visibility.Collapsed;
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

            if (dgrVariable.SelectedItem != null && e.Source != null)
            {
                expandedVariableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;
                comboBox = e.Source as ComboBox;

                if (expandedVariableInfo != null && comboBox != null && comboBox.SelectedItem != null && Enum.TryParse(comboBox.SelectedItem.ToString(), out format) == true)
                {
                    if (expandedVariableInfo.Format != format)
                    {
                        expandedVariableInfo.Format = format;
                        expandedVariableInfo.Value = string.Empty;
                    }
                }
            }
        }
        #endregion
        #region dgrVariable_RowDoubleClick
        private void dgrVariable_RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExpandedVariableInfo variableInfo;

            variableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null && variableInfo.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                btnListEdit.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }
        #endregion

        // Button Event
        #region btnListEdit_Click
        private void btnListEdit_Click(object sender, RoutedEventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            VariableListWindow variableListWindow;
            List<ExpandedVariableInfo> parents;

            variableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null && variableInfo.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                parents = new List<ExpandedVariableInfo>();
                parents.Add(variableInfo);

                variableListWindow = new VariableListWindow();
                variableListWindow.Initialize(variableInfo, this._currentVariableList, parents, false);

                variableListWindow.Owner = this;
                variableListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                variableListWindow.ShowDialog();

                variableInfo.Length = variableInfo.ChildVariables.Count;
            }
        }
        #endregion
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
                dgrVariable.SelectedItem = null;

                foreach (var item in this._displayInfo)
                {
                    if (string.IsNullOrEmpty(item.VID) == false)
                    {
                        variableInfo = this._messageProcessor.VariableCollection.Variables[item.VID];
                    }
                    else
                    {
                        variableInfo = this._messageProcessor.VariableCollection.Variables.Items.FirstOrDefault(t => t.Name == item.Name);
                    }

                    expandedVariableInfo = variableInfo as ExpandedVariableInfo;

                    if (expandedVariableInfo != null)
                    {
                        expandedVariableInfo.Format = item.Format;
                        expandedVariableInfo.Length = item.ChildVariables.Count;

                        if (item.Format != UbiCom.Net.Structure.SECSItemFormat.L)
                        {
                            expandedVariableInfo.Value = item.Value;
                        }
                        else
                        {
                            expandedVariableInfo.ChildVariables.Clear();
                            expandedVariableInfo.ChildVariables.AddRange(item.ChildVariables);
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

                dgrVariable.Items.Refresh();

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

        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            List<string> usedIDs;

            result = true;
            errorText = string.Empty;
            usedIDs = new List<string>();

            foreach (var item in this._displayInfo)
            {
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (string.IsNullOrEmpty(item.Name) == true)
                    {
                        errorText = string.Format("Invalid Name, VID {0}", item.VID == null ? string.Empty : item.VID);
                        result = false;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (item.VID.Length > 0 && this._messageProcessor.VariableCollection.ECV[item.VID] != null)
                    {
                        errorText = string.Format("Dupelicated Equipment Constants ID: {0}", item.VID);
                        result = false;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (usedIDs.Contains(item.VID) == true)
                    {
                        errorText = string.Format("Dupelicated VID: {0}", item.VID);
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
            }
            return result;
        }
        #endregion
    }
}