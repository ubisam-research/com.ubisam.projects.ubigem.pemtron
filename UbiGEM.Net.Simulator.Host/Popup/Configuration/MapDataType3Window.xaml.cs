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
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapDataType3Window : Window
    {
        #region MemberVariable
        private ObservableCollection<MapDataType3> _displayMapDataCollection;
        private ObservableCollection<XYPosInfo> _displayReferenceStartingCollection;

        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public MapDataType3Window()
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

            this._displayMapDataCollection = new ObservableCollection<MapDataType3>();
            this._displayReferenceStartingCollection = new ObservableCollection<XYPosInfo>();

            foreach (MapDataType3 mapDataType3 in this._messageProcessor.MapDataType3Collection)
            {
                this._displayMapDataCollection.Add(mapDataType3.Clone());
            }

            dgrMapSetupData.ItemsSource = this._displayMapDataCollection;
            dgrReferencePoint.ItemsSource = this._displayReferenceStartingCollection;

            if (this._displayMapDataCollection.Count > 0)
            {
                dgrMapSetupData.SelectedItem = this._displayMapDataCollection[0];
            }
        }
        #endregion

        // DataGrid Event
        #region MapSetupData_OnAdd
        private void MapSetupData_OnAdd(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;

            mapDataType3 = new MapDataType3();

            this._displayMapDataCollection.Add(mapDataType3);
            dgrMapSetupData.SelectedItem = mapDataType3;
        }
        #endregion
        #region MapSetupData_OnRemove
        private void MapSetupData_OnRemove(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;

            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;

            if (mapDataType3 != null)
            {
                this._displayMapDataCollection.Remove(mapDataType3);
                dgrMapSetupData.SelectedItem = null;
            }
        }
        #endregion
        #region dgrMapSetupData_SelectionChanged
        private void dgrMapSetupData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MapDataType3 mapDataType3;

            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;

            if (mapDataType3 == null)
            {
                ctlFunctionReferencePoint.ChangeButtonEnabled(false, false);
            }
            else
            {
                ctlFunctionReferencePoint.ChangeButtonEnabled(true, true);

                this._displayReferenceStartingCollection.Clear();

                foreach (var item in mapDataType3.XYPOSList)
                {
                    this._displayReferenceStartingCollection.Add(item);
                }
            }
        }
        #endregion

        #region ReferencePoint_OnAdd
        private void ReferencePoint_OnAdd(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;
            XYPosInfo xyPosInfo;

            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;

            if (mapDataType3 != null)
            {
                xyPosInfo = new XYPosInfo();

                mapDataType3.XYPOSList.Add(xyPosInfo);
                this._displayReferenceStartingCollection.Add(xyPosInfo);
            }
        }
        #endregion
        #region ReferencePoint_OnRemove
        private void ReferencePoint_OnRemove(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;
            XYPosInfo xyPosInfo;

            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;
            xyPosInfo = dgrReferencePoint.SelectedItem as XYPosInfo;

            if (mapDataType3 != null && xyPosInfo != null)
            {
                mapDataType3.XYPOSList.Remove(xyPosInfo);
                this._displayReferenceStartingCollection.Remove(xyPosInfo);
            }
        }
        #endregion

        // UserControl: Up, Down Event
        #region ctlUpDown_OnFirst
        private void ctlUpDown_OnFirst(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;
            XYPosInfo xyPosInfo;

            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;

            int selectedIndex;

            if (mapDataType3 != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displayReferenceStartingCollection.Move(selectedIndex, 0);

                    xyPosInfo = mapDataType3.XYPOSList[selectedIndex];
                    mapDataType3.XYPOSList.Remove(xyPosInfo);
                    mapDataType3.XYPOSList.Insert(0, xyPosInfo);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnUp
        private void ctlUpDown_OnUp(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;

            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;

            int selectedIndex;

            if (mapDataType3 != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displayReferenceStartingCollection.Move(selectedIndex, selectedIndex - 1);
                    mapDataType3.XYPOSList.Reverse(selectedIndex - 1, 2);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnDown
        private void ctlUpDown_OnDown(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;
            
            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;

            int selectedIndex;

            if (mapDataType3 != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displayReferenceStartingCollection.Count - 1)
                {
                    this._displayReferenceStartingCollection.Move(selectedIndex, selectedIndex + 1);
                    mapDataType3.XYPOSList.Reverse(selectedIndex, 2);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnLast
        private void ctlUpDown_OnLast(object sender, EventArgs e)
        {
            MapDataType3 mapDataType3;
            XYPosInfo xyPosInfo;

            mapDataType3 = dgrMapSetupData.SelectedItem as MapDataType3;

            int selectedIndex;

            if (mapDataType3 != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displayReferenceStartingCollection.Count - 1)
                {
                    this._displayReferenceStartingCollection.Move(selectedIndex, this._displayReferenceStartingCollection.Count - 1);

                    xyPosInfo = mapDataType3.XYPOSList[selectedIndex];
                    mapDataType3.XYPOSList.RemoveAt(selectedIndex);
                    mapDataType3.XYPOSList.Add(xyPosInfo);
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

            errorText = string.Empty;

            if (IsValid(out errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                this._messageProcessor.MapDataType3Collection.Clear();

                foreach (var mapSetupData in this._displayMapDataCollection)
                {
                    this._messageProcessor.MapDataType3Collection.Add(mapSetupData);
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

        // Private Method
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            List<string> usedMaterialIDs ;
            List<string> usedRefrencePoints;

            MapDataType3 invalidData;

            errorText = string.Empty;

            result = true;
            usedMaterialIDs = new List<string>();

            invalidData = null;

            foreach (var mapSetupData in this._displayMapDataCollection)
            {
                if (result == true && string.IsNullOrEmpty(mapSetupData.MaterialID) == true)
                {
                    result = false;
                    errorText = string.Format("Material ID is empty");
                    invalidData = mapSetupData;
                }

                if (result == true)
                {
                    if (usedMaterialIDs.Contains(mapSetupData.MaterialID) == true)
                    {
                        result = false;
                        errorText = string.Format(" MapDataType3: {0} \n\n Material ID is dupelicated", mapSetupData.MaterialID);
                        invalidData = mapSetupData;
                    }
                    else
                    {
                        usedMaterialIDs.Add(mapSetupData.MaterialID);
                    }
                }

                if (result == true)
                {
                    usedRefrencePoints = new List<string>();

                    foreach (var rpItem in mapSetupData.XYPOSList)
                    {
                        if (result == true)
                        {
                            if (usedRefrencePoints.Contains(rpItem.DataString()) == true)
                            {
                                result = false;
                                errorText = string.Format(" MID: {0} \n\n XY Point: ({1},{2}) is dupelicated", mapSetupData.MaterialID, rpItem.X, rpItem.Y);
                                invalidData = mapSetupData;
                            }
                            else
                            {
                                usedRefrencePoints.Add(rpItem.DataString());
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
    }
}