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
using UbiGEM.Net.Simulator.Host.Schedules;
using UbiGEM.Net.Structure;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// EquipmentConstantWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EquipmentConstantWindow1 : Window
    {
        #region MemberVariable
        private Scheduler _scheduler;

        private ObservableCollection<ExpandedVariableInfo> _displayInfo;
        #endregion
        #region Constructor
        public EquipmentConstantWindow1()
        {
            InitializeComponent();

            this._scheduler = Scheduler.Instance;
        }
        #endregion
        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            this.MouseLeftButtonDown += delegate { DragMove(); };

            this._displayInfo = new ObservableCollection<ExpandedVariableInfo>();

            foreach (var item in this._scheduler.VariableCollection.EquipmentConstants.Values.OrderBy(t => t.VID))
            {
                expandedVariableInfo = new ExpandedVariableInfo(item);
                expandedVariableInfo.IsInheritance = true;

                this._displayInfo.Add(expandedVariableInfo);
            }

            foreach (var item in this._scheduler.LocalVariableCollection.EquipmentConstants.Values.OrderBy(t => t.VID))
            {
                expandedVariableInfo = new ExpandedVariableInfo(item);
                expandedVariableInfo.IsInheritance = false;

                this._displayInfo.Add(expandedVariableInfo);
            }

            dgrEquipmentConstant.ItemsSource = this._displayInfo;
        }
        #endregion
        #region Window_Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool dupelicated;
            bool dupelicatedSub;
            bool convertFail;
            bool convertFailSub;
            long errorVID;
            long errorSubVID;
            List<long> usedVIDs;
            Dictionary<long, List<long>> usedSubVIDs;

            dupelicated = false;
            dupelicatedSub = false;
            convertFail = false;
            convertFailSub = false;
            errorVID = 0;
            errorSubVID = 0;

            usedVIDs = new List<long>();
            usedSubVIDs = new Dictionary<long, List<long>>();

            foreach (var item in this._displayInfo)
            {
                if (item.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    foreach (var subItem in item.ChildVariables)
                    {
                        if (usedSubVIDs.ContainsKey(item.VID) == false)
                        {
                            usedSubVIDs[item.VID] = new List<long>();
                        }

                        if (usedSubVIDs[item.VID].Contains(subItem.VID) == true)
                        {
                            dupelicatedSub = true;
                            errorVID = item.VID;
                            errorSubVID = subItem.VID;
                        }
                        else
                        {
                            usedSubVIDs[item.VID].Add(subItem.VID);

                            if (StaticMethod.ConvertValue(subItem.Format, subItem.Value) == null)
                            {
                                convertFailSub = true;
                                errorVID = item.VID;
                                errorSubVID = subItem.VID;
                            }
                        }
                        if (convertFailSub == true)
                        {
                            break;
                        }
                        if (dupelicatedSub == true)
                        {
                            break;
                        }
                    }
                }

                if (usedVIDs.Contains(item.VID) == true)
                {
                    dupelicated = true;
                    errorVID = item.VID;
                }
                else
                {
                    usedVIDs.Add(item.VID);

                    if (StaticMethod.ConvertValue(item.Format, item.Value) == null)
                    {
                        convertFail = true;
                        errorVID = item.VID;
                    }
                }
                if (dupelicatedSub == true || dupelicated == true || convertFailSub == true || convertFail == true)
                {
                    break;
                }
            }

            if (convertFailSub == true)
            {
                MessageBox.Show(string.Format("VID:{0} is convert fails in List of VID{1}", errorSubVID, errorVID));
                e.Cancel = true;
            }
            else if (convertFail == true)
            {
                MessageBox.Show(string.Format("VID:{0} is convert fails", errorVID));
                e.Cancel = true;
            }
            else if (dupelicatedSub == true)
            {
                MessageBox.Show(string.Format("VID:{0} is duplicated in List of VID{1}", errorSubVID, errorVID));
                e.Cancel = true;
            }
            else if (dupelicated == true)
            {
                MessageBox.Show(string.Format("VID:{0} is duplicated", errorVID));
                e.Cancel = true;
            }
            else
            {
                this._scheduler.LocalVariableCollection.EquipmentConstants.Clear();

                foreach (var item in this._displayInfo)
                {
                    if (item.IsInheritance == false)
                    {
                        this._scheduler.LocalVariableCollection.Add(item);

                        if (item.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                        {
                            foreach (var subItem in item.ChildVariables)
                            {
                                this._scheduler.LocalVariableCollection[item.VID].ChildVariables.Add(subItem);
                            }
                        }
                    }
                    else
                    {
                        if (this._scheduler.VariableCollection.EquipmentConstants[item.VID] != null)
                        {
                            this._scheduler.VariableCollection.EquipmentConstants[item.VID].Value.SetValue(StaticMethod.ConvertValue(item.Format, item.Value));

                            if (item.Format == UbiCom.Net.Structure.SECSItemFormat.L)
                            {
                                foreach (var subItem in item.ChildVariables)
                                {
                                    if (this._scheduler.VariableCollection.EquipmentConstants[item.VID].ChildVariables[subItem.VID] != null)
                                    {
                                        this._scheduler.VariableCollection.EquipmentConstants[item.VID].ChildVariables[subItem.VID].Value.SetValue(StaticMethod.ConvertValue(subItem.Format, subItem.Value));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            expandedVariableInfo = new ExpandedVariableInfo()
            {
                VID = -1,
                IsInheritance = false,
                IsUse = true,
                VIDType = VariableType.EquipmentConstant
            };

            this._displayInfo.Add(expandedVariableInfo);
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            ExpandedVariableInfo expandedVariableInfo;

            expandedVariableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;

            if (expandedVariableInfo != null)
            {
                if (this._scheduler.VariableCollection.EquipmentConstants.ContainsKey(expandedVariableInfo.VID) == false)
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

            expandedVariableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;

            if (expandedVariableInfo != null)
            {
                if (this._scheduler.VariableCollection.EquipmentConstants.ContainsKey(expandedVariableInfo.VID) == false)
                {
                    ctlFunction.ChangeButtonEnabled(true, true);
                }
                else
                {
                    ctlFunction.ChangeButtonEnabled(true, false);
                }
            }
        }
        #endregion
        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        #region btnVariableItemEdit_Click
        private void btnVariableItemEdit_Click(object sender, RoutedEventArgs e)
        {
            /*
            ExpandedVariableInfo variableInfo;
            VariableListWindow variableListWindow;

            variableInfo = dgrEquipmentConstant.SelectedItem as ExpandedVariableInfo;

            if (variableInfo != null)
            {
                variableListWindow = new VariableListWindow();
                variableListWindow.Initialize(variableInfo);

                variableListWindow.Owner = this;
                variableListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                variableListWindow.ShowDialog();
            }
            */
        }
        #endregion
        // Public Method

        // Private Method
    }
}