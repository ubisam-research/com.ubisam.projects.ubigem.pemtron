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
    /// VariableListWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VariableListWindow : Window
    {
        #region MemberVariable
        private ExpandedVariableInfo _selectedVariable;
        private ObservableCollection<ExpandedVariableInfo> _displayAllVariables;
        private List<ExpandedVariableInfo> _allVariableList;
        private List<ExpandedVariableInfo> _parents;
        private ObservableCollection<ExpandedVariableInfo> _childVariableList;
        private bool _isECV;

        #endregion
        #region Constructor
        public VariableListWindow()
        {
            InitializeComponent();

            this._parents = null;
            this._selectedVariable = null;
            this._displayAllVariables = null;
            this._isECV = false;
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
                                     new MouseButtonEventHandler(dgrSelected_RowDoubleClick)));
            dgrSelected.RowStyle = rowStyle;

            this._childVariableList = new ObservableCollection<ExpandedVariableInfo>();
            this._displayAllVariables = new ObservableCollection<ExpandedVariableInfo>();

            foreach (var variableItem in this._selectedVariable.ChildVariables)
            {
                if (variableItem.VID == variableItem.Name)
                {
                    variableItem.VID = string.Empty;
                }
                this._childVariableList.Add(variableItem);
            }

            if (this._allVariableList != null)
            {
                if (this._isECV == false)
                {
                    foreach (var variableItem in this._allVariableList.Where(t => t.VIDType == Structure.VariableType.SV).OrderBy(t => t.VID))
                    {
                        expandedVariableInfo = variableItem as ExpandedVariableInfo;

                        if (this._parents.FirstOrDefault(t => t.Name == expandedVariableInfo.Name) == null)
                        {
                            this._displayAllVariables.Add(expandedVariableInfo);
                        }
                    }

                    foreach (var variableItem in this._allVariableList.Where(t => t.VIDType == Structure.VariableType.DVVAL).OrderBy(t => t.VID))
                    {
                        expandedVariableInfo = variableItem as ExpandedVariableInfo;

                        if (this._parents.FirstOrDefault(t => t.Name == expandedVariableInfo.Name) == null)
                        {
                            this._displayAllVariables.Add(expandedVariableInfo);
                        }
                    }
                }


                foreach (var variableItem in this._allVariableList.Where(t => t.VIDType == Structure.VariableType.ECV).OrderBy(t => t.VID))
                {
                    expandedVariableInfo = variableItem as ExpandedVariableInfo;

                    if (this._parents.FirstOrDefault(t => t.Name == expandedVariableInfo.Name) == null)
                    {
                        if (variableItem.VIDType == Structure.VariableType.ECV)
                        {
                            this._displayAllVariables.Add(expandedVariableInfo);
                        }
                    }
                }
            }

            dgrSelected.ItemsSource = this._childVariableList;
            dgrVariable.ItemsSource = this._displayAllVariables;
        }
        #endregion

        // Button Event
        #region dgrSelected Event Handlers
        private void dgrSelected_RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExpandedVariableInfo variableInfo;

            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null && variableInfo.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                btnListEdit.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }
        private void dgrSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;
            btnListEdit.Visibility = Visibility.Collapsed;

            expandedVariableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (expandedVariableInfo != null)
            {
                if (expandedVariableInfo.Format == SECSItemFormat.L)
                {
                    btnListEdit.Visibility = Visibility.Visible;
                }
            }
        }
        #endregion
        #region btnListEdit_Click
        private void btnListEdit_Click(object sender, RoutedEventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            VariableListWindow variableListWindow;
            List<ExpandedVariableInfo> parents;

            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null && variableInfo.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                parents = new List<ExpandedVariableInfo>();
                parents.AddRange(this._parents);
                parents.Add(variableInfo);

                variableListWindow = new VariableListWindow();
                variableListWindow.Initialize(variableInfo, this._allVariableList, parents, false);

                variableListWindow.Owner = this;
                variableListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                variableListWindow.ShowDialog();

                variableInfo.Length = variableInfo.ChildVariables.Count;
            }
        }
        #endregion
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        // UserControl: Select, Unselect Event
        #region UCSelectUnSelectButton_OnSelect
        private void UCSelectUnSelectButton_OnSelect(object sender, EventArgs e)
        {
            ExpandedVariableInfo variableInfo;

            variableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null)
            {
                if (this._selectedVariable.ChildVariables.FirstOrDefault(t => t.Name == variableInfo.Name) == null)
                {
                    this._selectedVariable.ChildVariables.Add(variableInfo);
                    this._childVariableList.Add(variableInfo);
                    dgrSelected.SelectedItem = variableInfo;
                    dgrSelected.ScrollIntoView(variableInfo);
                }
            }
        }
        #endregion
        #region UCSelectUnSelectButton_OnUnselect
        private void UCSelectUnSelectButton_OnUnselect(object sender, EventArgs e)
        {
            ExpandedVariableInfo variableInfo;

            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null)
            {
                this._selectedVariable.ChildVariables.Remove(variableInfo);
                this._childVariableList.Remove(variableInfo);
            }
        }
        #endregion

        // UserControl: Up, Down Event
        #region ctlUpDown_OnFirst
        private void ctlUpDown_OnFirst(object sender, EventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._childVariableList.Move(selectedIndex, 0);
                    this._selectedVariable.ChildVariables.Clear();

                    foreach (ExpandedVariableInfo item in this._childVariableList)
                    {
                        this._selectedVariable.ChildVariables.Add(item);
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnUp
        private void ctlUpDown_OnUp(object sender, EventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._childVariableList.Move(selectedIndex, selectedIndex - 1);
                    this._selectedVariable.ChildVariables.Clear();

                    foreach (ExpandedVariableInfo item in this._childVariableList)
                    {
                        this._selectedVariable.ChildVariables.Add(item);
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnDown
        private void ctlUpDown_OnDown(object sender, EventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._childVariableList.Count - 1)
                {
                    this._childVariableList.Move(selectedIndex, selectedIndex + 1);
                    this._selectedVariable.ChildVariables.Clear();

                    foreach (ExpandedVariableInfo item in this._childVariableList)
                    {
                        this._selectedVariable.ChildVariables.Add(item);
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnLast
        private void ctlUpDown_OnLast(object sender, EventArgs e)
        {
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._childVariableList.Count - 1)
                {
                    this._childVariableList.Move(selectedIndex, this._childVariableList.Count - 1);
                    this._selectedVariable.ChildVariables.Clear();

                    foreach (ExpandedVariableInfo item in this._childVariableList)
                    {
                        this._selectedVariable.ChildVariables.Add(item);
                    }
                }
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(ExpandedVariableInfo selectedVariable, List<ExpandedVariableInfo> variables, List<ExpandedVariableInfo> parents, bool isECV)
        {
            this._allVariableList = variables;

            this._parents = parents;
            this._isECV = isECV;

            if (selectedVariable != null)
            {
                this._selectedVariable = selectedVariable;
            }
        }
        #endregion

    }
}