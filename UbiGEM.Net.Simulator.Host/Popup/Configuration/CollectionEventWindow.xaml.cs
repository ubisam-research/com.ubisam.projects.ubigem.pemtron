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

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// CollectionEventWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CollectionEventWindow : Window
    {
        #region MemberVariable
        private ObservableCollection<ExpandedCollectionEventInfo> _displayInfo;
        private ObservableCollection<ExpandedReportInfo> _displayReportInfo;
        private ObservableCollection<ExpandedReportInfo> _selectedReportInfo;

        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public CollectionEventWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedReportInfo expandedReportInfo;
            ExpandedCollectionEventInfo expandedCollectionEventInfo;

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

            dgrCollectionEvent.AddHandler(CommandManager.PreviewExecutedEvent, (ExecutedRoutedEventHandler)((eventSender, args) =>
            {
                ExpandedCollectionEventInfo selectedCollectionEventInfo;
                DataGridCell cell;
                DataGridColumn column;
                string columnHeader;
                bool inheritance;

                selectedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;

                if (args.Command == DataGrid.BeginEditCommand)
                {
                    if (selectedCollectionEventInfo != null)
                    {
                        cell = args.OriginalSource as DataGridCell;
                        if (cell != null)
                        {
                            column = cell.Column;
                            inheritance = this._messageProcessor.ReportCollection.Items.ContainsKey(selectedCollectionEventInfo.CEID);

                            if (inheritance == true)
                            {
                                if (column != null)
                                {
                                    columnHeader = column.Header.ToString();
                                    if (columnHeader != "Enabled")
                                    {
                                        args.Handled = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }));

            this._displayInfo = new ObservableCollection<ExpandedCollectionEventInfo>();

            foreach (Structure.CollectionEventInfo info in this._messageProcessor.CollectionEventCollection.Items.Values.OrderBy(t => t.CEID))
            {
                expandedCollectionEventInfo = info as ExpandedCollectionEventInfo;
                this._displayInfo.Add(expandedCollectionEventInfo.Clone());
            }

            this._displayReportInfo = new ObservableCollection<ExpandedReportInfo>();

            foreach (var item in this._messageProcessor.ReportCollection.Items.Values.OrderBy(t => t.ReportID))
            {
                expandedReportInfo = item as ExpandedReportInfo;
                this._displayReportInfo.Add(expandedReportInfo);
            }

            dgrReport.ItemsSource = this._displayReportInfo;

            dgrCollectionEvent.ItemsSource = null;
            dgrCollectionEvent.ItemsSource = this._displayInfo;
        }
        #endregion

        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;

            expandedCollectionEventInfo = new ExpandedCollectionEventInfo()
            {
                CEID = string.Empty,
                IsInheritance = false,
                IsUse = true,
                Enabled = true
            };

            this._displayInfo.Add(expandedCollectionEventInfo);
            dgrCollectionEvent.SelectedItem = expandedCollectionEventInfo;
            dgrCollectionEvent.ScrollIntoView(expandedCollectionEventInfo);
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;

            expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;

            if (expandedCollectionEventInfo != null)
            {
                if (expandedCollectionEventInfo.IsInheritance == true)
                {
                    MessageBox.Show(string.Format("CEID: {0} can not remove", expandedCollectionEventInfo.CEID));
                }
                else
                {
                    this._displayInfo.Remove(expandedCollectionEventInfo);
                    dgrSelected.ItemsSource = null;
                }
            }
        }
        #endregion
        #region dgrCollectionEvent_SelectionChanged
        private void dgrCollectionEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo reportInfo;

            if (dgrCollectionEvent.SelectedItem != null)
            {
                expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;

                if (expandedCollectionEventInfo.IsInheritance == true)
                {
                    dgcCEID.IsReadOnly = true;
                    dgcName.IsReadOnly = true;
                    dgcDescription.IsReadOnly = true;

                    ctlFunction.ChangeButtonEnabled(true, false);
                }
                else
                {
                    dgcCEID.IsReadOnly = false;
                    dgcName.IsReadOnly = false;
                    dgcDescription.IsReadOnly = false;

                    ctlFunction.ChangeButtonEnabled(true, true);
                }

                this._selectedReportInfo = new ObservableCollection<ExpandedReportInfo>();

                foreach (var report in expandedCollectionEventInfo.Reports.Items.Values)
                {
                    reportInfo = report as ExpandedReportInfo;
                    this._selectedReportInfo.Add(reportInfo);
                }

                dgrSelected.ItemsSource = this._selectedReportInfo;
            }
        }
        #endregion

        // UserControl: Select, Unselect Event
        #region UCSelectUnSelectButton_OnSelect
        private void UCSelectUnSelectButton_OnSelect(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo reportInfo;

            expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;
            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;

            if (expandedCollectionEventInfo != null && reportInfo != null)
            {
                if (expandedCollectionEventInfo.Reports.Items.ContainsKey(reportInfo.ReportID) == false)
                {
                    expandedCollectionEventInfo.Reports.Add(reportInfo);
                    this._selectedReportInfo.Add(reportInfo);

                    dgrSelected.SelectedItem = reportInfo;
                    dgrSelected.ScrollIntoView(reportInfo);
                }
            }
        }
        #endregion
        #region UCSelectUnSelectButton_OnUnselect
        private void UCSelectUnSelectButton_OnUnselect(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo reportInfo;

            expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;
            reportInfo = dgrSelected.SelectedItem as ExpandedReportInfo;

            if (expandedCollectionEventInfo != null && reportInfo != null)
            {
                expandedCollectionEventInfo.Reports.Remove(reportInfo);
                this._selectedReportInfo.Remove(reportInfo);
            }
        }
        #endregion

        // UserControl: Up, Down Event
        #region ctlUpDown_OnFirst
        private void ctlUpDown_OnFirst(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo reportInfo;
            int selectedIndex;

            expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;
            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;

            if (expandedCollectionEventInfo != null && reportInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._selectedReportInfo.Move(selectedIndex, 0);

                    expandedCollectionEventInfo.Reports.Items.Clear();

                    foreach (ExpandedReportInfo item in this._selectedReportInfo)
                    {
                        expandedCollectionEventInfo.Reports.Items[item.ReportID] = item;
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnUp
        private void ctlUpDown_OnUp(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo reportInfo;
            int selectedIndex;

            expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;
            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;

            if (expandedCollectionEventInfo != null && reportInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._selectedReportInfo.Move(selectedIndex, selectedIndex - 1);

                    expandedCollectionEventInfo.Reports.Items.Clear();

                    foreach (ExpandedReportInfo item in this._selectedReportInfo)
                    {
                        expandedCollectionEventInfo.Reports.Items[item.ReportID] = item;
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnDown
        private void ctlUpDown_OnDown(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo reportInfo;
            int selectedIndex;

            expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;
            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;

            if (expandedCollectionEventInfo != null && reportInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._selectedReportInfo.Count - 1)
                {
                    this._selectedReportInfo.Move(selectedIndex, selectedIndex + 1);

                    expandedCollectionEventInfo.Reports.Items.Clear();

                    foreach (ExpandedReportInfo item in this._selectedReportInfo)
                    {
                        expandedCollectionEventInfo.Reports.Items[item.ReportID] = item;
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnLast
        private void ctlUpDown_OnLast(object sender, EventArgs e)
        {
            ExpandedCollectionEventInfo expandedCollectionEventInfo;
            ExpandedReportInfo reportInfo;
            int selectedIndex;

            expandedCollectionEventInfo = dgrCollectionEvent.SelectedItem as ExpandedCollectionEventInfo;
            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;

            if (expandedCollectionEventInfo != null && reportInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._selectedReportInfo.Count - 1)
                {
                    this._selectedReportInfo.Move(selectedIndex, this._selectedReportInfo.Count - 1);

                    expandedCollectionEventInfo.Reports.Items.Clear();

                    foreach (ExpandedReportInfo item in this._selectedReportInfo)
                    {
                        expandedCollectionEventInfo.Reports.Items[item.ReportID] = item;
                    }
                }
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Structure.GemDriverError error;
            string errorText;
            List<string> usedCEIDs;
            List<string> usedNames;

            errorText = string.Empty;
            usedCEIDs = new List<string>();
            usedNames = new List<string>();

            foreach (ExpandedCollectionEventInfo item in this._displayInfo)
            {
                if (usedCEIDs.Contains(item.CEID) == true)
                {
                    errorText = string.Format(" CEID: {0} is dupelicated.", item.CEID);
                    break;
                }
                else
                {
                    usedCEIDs.Add(item.CEID);
                }

                if (usedNames.Contains(item.Name) == true)
                {
                    errorText = string.Format(" CE Name: {0} is dupelicated.", item.Name);
                    break;
                }
                else
                {
                    usedNames.Add(item.Name);
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                this._messageProcessor.CollectionEventCollection.Items.Clear();

                foreach (ExpandedCollectionEventInfo item in this._displayInfo)
                {
                    this._messageProcessor.CollectionEventCollection.Items[item.CEID] = item;
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

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion
    }
}