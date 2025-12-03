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
    /// ReportWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportWindow : Window
    {
        #region MemberVariable
        private ObservableCollection<ExpandedReportInfo> _displayInfo;
        private ObservableCollection<ExpandedVariableInfo> _displaySelectedVariableInfo;
        private ObservableCollection<ExpandedVariableInfo> _displayVariableInfo;

        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public ReportWindow()
        {
            InitializeComponent();

            this._displayInfo = new ObservableCollection<ExpandedReportInfo>();

            this._displayVariableInfo = new ObservableCollection<ExpandedVariableInfo>();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo expandedVariableInfo;

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

            foreach (var item in this._messageProcessor.ReportCollection.Items.Values.OrderBy(t => t.ReportID))
            {
                reportInfo = item as ExpandedReportInfo;
                reportInfo = reportInfo.Clone();
                this._displayInfo.Add(reportInfo);
            }

            dgrReport.ItemsSource = this._displayInfo;

            foreach (var item in this._messageProcessor.VariableCollection.SV.Items.OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;
                expandedVariableInfo = expandedVariableInfo.Clone();

                if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                {
                    this._displayVariableInfo.Add(expandedVariableInfo);
                }
            }

            foreach (var item in this._messageProcessor.VariableCollection.DVVal.Items.OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;
                expandedVariableInfo = expandedVariableInfo.Clone();

                if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                {
                    this._displayVariableInfo.Add(expandedVariableInfo);
                }
            }

            foreach (var item in this._messageProcessor.VariableCollection.ECV.Items.OrderBy(t => t.VID))
            {
                expandedVariableInfo = item as ExpandedVariableInfo;
                expandedVariableInfo = expandedVariableInfo.Clone();

                if (expandedVariableInfo.VID != expandedVariableInfo.Name)
                {
                    this._displayVariableInfo.Add(expandedVariableInfo);
                }
            }

            dgrVariable.ItemsSource = this._displayVariableInfo;
        }
        #endregion

        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;

            reportInfo = new ExpandedReportInfo()
            {
                ReportID = string.Empty,
                IsInheritance = false
            };

            this._displayInfo.Add(reportInfo);
            dgrReport.SelectedItem = reportInfo;
            dgrReport.ScrollIntoView(reportInfo);

            this._displaySelectedVariableInfo = new ObservableCollection<ExpandedVariableInfo>();

            foreach (var item in reportInfo.Variables.Items)
            {
                variableInfo = item as ExpandedVariableInfo;
                this._displaySelectedVariableInfo.Add(variableInfo);
            }
            dgrSelected.ItemsSource = this._displaySelectedVariableInfo;
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;

            if (reportInfo != null)
            {
                if (reportInfo.IsInheritance == true)
                {
                    MessageBox.Show(string.Format("Report ID: {0} can not remove", reportInfo.ReportID));
                }
                else
                {
                    this._displayInfo.Remove(reportInfo);
                    dgrSelected.ItemsSource = null;
                }
            }
        }
        #endregion
        #region dgrReport_SelectionChanged
        private void dgrReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;

            if (reportInfo != null)
            {
                if (reportInfo.IsInheritance == true)
                {
                    dgcReportID.IsReadOnly = true;
                    dgcDescription.IsReadOnly = true;

                    ctlFunctionReport.ChangeButtonEnabled(true, false);
                }
                else
                {
                    dgcReportID.IsReadOnly = false;
                    dgcDescription.IsReadOnly = false;

                    ctlFunctionReport.ChangeButtonEnabled(true, true);
                }

                this._displaySelectedVariableInfo = new ObservableCollection<ExpandedVariableInfo>();

                foreach (var item in reportInfo.Variables.Items)
                {
                    variableInfo = item as ExpandedVariableInfo;
                    this._displaySelectedVariableInfo.Add(variableInfo);
                }
                dgrSelected.ItemsSource = this._displaySelectedVariableInfo;
            }
        }
        #endregion

        // UserControl: Select, Unselect Event
        #region UCSelectUnSelectButton_OnSelect
        private void UCSelectUnSelectButton_OnSelect(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;
            variableInfo = dgrVariable.SelectedItem as ExpandedVariableInfo;

            if (reportInfo != null && variableInfo != null)
            {
                if (reportInfo.Variables.Items.FirstOrDefault(t => t.VID == variableInfo.VID) == null)
                {
                    reportInfo.Variables.Add(variableInfo);
                    this._displaySelectedVariableInfo.Add(variableInfo);
                    dgrSelected.SelectedItem = variableInfo;
                    dgrSelected.ScrollIntoView(variableInfo);
                }
            }
        }
        #endregion
        #region UCSelectUnSelectButton_OnUnselect
        private void UCSelectUnSelectButton_OnUnselect(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (reportInfo != null && variableInfo != null)
            {
                reportInfo.Variables.Remove(variableInfo);
                this._displaySelectedVariableInfo.Remove(variableInfo);
            }
        }
        #endregion

        // UserControl: Up, Down Event
        #region ctlUpDown_OnFirst
        private void ctlUpDown_OnFirst(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (reportInfo != null && variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displaySelectedVariableInfo.Move(selectedIndex, 0);

                    reportInfo.Variables.Items.Clear();

                    foreach (ExpandedVariableInfo item in this._displaySelectedVariableInfo)
                    {
                        reportInfo.Variables.Add(item);
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnUp
        private void ctlUpDown_OnUp(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (reportInfo != null && variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displaySelectedVariableInfo.Move(selectedIndex, selectedIndex - 1);
                    reportInfo.Variables.Items.Clear();

                    foreach (ExpandedVariableInfo item in this._displaySelectedVariableInfo)
                    {
                        reportInfo.Variables.Add(item);
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnDown
        private void ctlUpDown_OnDown(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (reportInfo != null && variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displaySelectedVariableInfo.Count - 1)
                {
                    this._displaySelectedVariableInfo.Move(selectedIndex, selectedIndex + 1);
                    reportInfo.Variables.Items.Clear();

                    foreach (ExpandedVariableInfo item in this._displaySelectedVariableInfo)
                    {
                        reportInfo.Variables.Add(item);
                    }
                }
            }
        }
        #endregion
        #region ctlUpDown_OnLast
        private void ctlUpDown_OnLast(object sender, EventArgs e)
        {
            ExpandedReportInfo reportInfo;
            ExpandedVariableInfo variableInfo;
            int selectedIndex;

            reportInfo = dgrReport.SelectedItem as ExpandedReportInfo;
            variableInfo = dgrSelected.SelectedItem as ExpandedVariableInfo;

            if (reportInfo != null && variableInfo != null)
            {
                selectedIndex = dgrSelected.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displaySelectedVariableInfo.Count - 1)
                {
                    this._displaySelectedVariableInfo.Move(selectedIndex, this._displaySelectedVariableInfo.Count - 1);
                    reportInfo.Variables.Items.Clear();

                    foreach (ExpandedVariableInfo item in this._displaySelectedVariableInfo)
                    {
                        reportInfo.Variables.Add(item);
                    }
                }
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            Structure.GemDriverError error;
            bool dupelicated;
            bool reportIdInvalid;
            string reportId;
            List<string> usedReportIDs;

            reportId = string.Empty;
            usedReportIDs = new List<string>();
            reportIdInvalid = false;
            dupelicated = false;

            foreach (ExpandedReportInfo tempReportInfo in this._displayInfo)
            {
                if (string.IsNullOrEmpty(tempReportInfo.ReportID) == true)
                {
                    reportIdInvalid = true;
                    reportId = tempReportInfo.ReportID;
                    break;
                }

                if (usedReportIDs.Contains(tempReportInfo.ReportID) == true)
                {
                    dupelicated = true;
                    reportId = tempReportInfo.ReportID;
                    break;
                }
                else
                {
                    usedReportIDs.Add(tempReportInfo.ReportID);
                }
            }

            if (reportIdInvalid == true)
            {
                MessageBox.Show(string.Format("Invalid Report ID: {0}", reportId));
            }
            else if (dupelicated == true)
            {
                MessageBox.Show(string.Format("Dupelicated Report ID: {0}", reportId));
            }
            else
            {
                this._messageProcessor.ReportCollection.Items.Clear();

                foreach (ExpandedReportInfo tempReportInfo in this._displayInfo)
                {
                    this._messageProcessor.ReportCollection.Add(tempReportInfo);
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