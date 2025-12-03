using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WaferMapWindow : Window
    {
        #region MemberVariable
        private ObservableCollection<MapSetupData> _displayMapSetupCollection;
        private ObservableCollection<ReferencePointItem> _displayReferencePointCollection;

        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public WaferMapWindow()
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

            this._displayMapSetupCollection = new ObservableCollection<MapSetupData>();
            this._displayReferencePointCollection = new ObservableCollection<ReferencePointItem>();

            foreach (MapSetupData mapSetupData in this._messageProcessor.MapSetupDataCollection)
            {
                this._displayMapSetupCollection.Add(mapSetupData.Clone());
            }

            dgrMapSetupData.ItemsSource = this._displayMapSetupCollection;
            dgrReferencePoint.ItemsSource = this._displayReferencePointCollection;

            if (this._displayMapSetupCollection.Count > 0)
            {
                dgrMapSetupData.SelectedItem = this._displayMapSetupCollection[0];
            }
        }
        #endregion

        // DataGrid Event
        #region MapSetupData_OnAdd
        private void MapSetupData_OnAdd(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;

            mapSetupData = new MapSetupData();

            this._displayMapSetupCollection.Add(mapSetupData);
            dgrMapSetupData.SelectedItem = mapSetupData;
        }
        #endregion
        #region MapSetupData_OnRemove
        private void MapSetupData_OnRemove(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;

            if (mapSetupData != null)
            {
                this._displayMapSetupCollection.Remove(mapSetupData);
                dgrMapSetupData.SelectedItem = null;
            }
        }
        #endregion
        #region dgrMapSetupData_SelectionChanged
        private void dgrMapSetupData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MapSetupData mapSetupData;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;

            if (mapSetupData == null)
            {
                ctlFunctionReferencePoint.ChangeButtonEnabled(false, false);
            }
            else
            {
                ctlFunctionReferencePoint.ChangeButtonEnabled(true, true);

                this._displayReferencePointCollection.Clear();

                foreach (var item in mapSetupData.ReferencePoint)
                {
                    this._displayReferencePointCollection.Add(item);
                }
            }
        }
        #endregion

        #region ReferencePoint_OnAdd
        private void ReferencePoint_OnAdd(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;
            ReferencePointItem referencePointItem;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;

            if (mapSetupData != null)
            {
                referencePointItem = new ReferencePointItem();

                mapSetupData.ReferencePoint.Add(referencePointItem);
                this._displayReferencePointCollection.Add(referencePointItem);
            }
        }
        #endregion
        #region ReferencePoint_OnRemove
        private void ReferencePoint_OnRemove(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;
            ReferencePointItem referencePointItem;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;
            referencePointItem = dgrReferencePoint.SelectedItem as ReferencePointItem;

            if (mapSetupData != null && referencePointItem != null)
            {
                mapSetupData.ReferencePoint.Remove(referencePointItem);
                this._displayReferencePointCollection.Remove(referencePointItem);
            }
        }
        #endregion

        // UserControl: Up, Down Event
        #region ctlUpDown_OnFirst
        private void ctlUpDown_OnFirst(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;
            ReferencePointItem referencePointItem;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;

            int selectedIndex;

            if (mapSetupData != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displayReferencePointCollection.Move(selectedIndex, 0);

                    referencePointItem = mapSetupData.ReferencePoint[selectedIndex];
                    mapSetupData.ReferencePoint.Remove(referencePointItem);
                    mapSetupData.ReferencePoint.Insert(0, referencePointItem);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnUp
        private void ctlUpDown_OnUp(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;

            int selectedIndex;

            if (mapSetupData != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex > 0)
                {
                    this._displayReferencePointCollection.Move(selectedIndex, selectedIndex - 1);
                    mapSetupData.ReferencePoint.Reverse(selectedIndex - 1, 2);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnDown
        private void ctlUpDown_OnDown(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;

            int selectedIndex;

            if (mapSetupData != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displayReferencePointCollection.Count - 1)
                {
                    this._displayReferencePointCollection.Move(selectedIndex, selectedIndex + 1);
                    mapSetupData.ReferencePoint.Reverse(selectedIndex, 2);
                }
            }
        }
        #endregion
        #region ctlUpDown_OnLast
        private void ctlUpDown_OnLast(object sender, EventArgs e)
        {
            MapSetupData mapSetupData;
            ReferencePointItem referencePointItem;

            mapSetupData = dgrMapSetupData.SelectedItem as MapSetupData;

            int selectedIndex;

            if (mapSetupData != null)
            {
                selectedIndex = dgrReferencePoint.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < this._displayReferencePointCollection.Count - 1)
                {
                    this._displayReferencePointCollection.Move(selectedIndex, this._displayReferencePointCollection.Count - 1);

                    referencePointItem = mapSetupData.ReferencePoint[selectedIndex];
                    mapSetupData.ReferencePoint.RemoveAt(selectedIndex);
                    mapSetupData.ReferencePoint.Add(referencePointItem);
                }
            }
        }
        #endregion

        // Button Event
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
                this._messageProcessor.MapSetupDataCollection.Clear();

                foreach (var mapSetupData in this._displayMapSetupCollection)
                {
                    this._messageProcessor.MapSetupDataCollection.Add(mapSetupData);
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

            MapSetupData invalidData;

            SECSItemFormat fnLocFormat;
            SECSItemFormat ffRotFormat;
            SECSItemFormat orLocFormat;
            SECSItemFormat prdCtFormat;
            SECSItemFormat xdiesFormat;
            SECSItemFormat ydiesFormat;

            string compact;
            dynamic converted;

            errorText = string.Empty;

            result = true;
            usedMaterialIDs = new List<string>();
            fnLocFormat = SECSItemFormat.U2;
            ffRotFormat = SECSItemFormat.U2;
            orLocFormat = SECSItemFormat.B;
            prdCtFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.PRDCT, SECSItemFormat.U2);
            xdiesFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.XDIES, SECSItemFormat.U2);
            ydiesFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.YDIES, SECSItemFormat.U2);

            foreach (var mapSetupData in this._displayMapSetupCollection)
            {
                if (result == true && string.IsNullOrEmpty(mapSetupData.MaterialID) == true)
                {
                    result = false;
                    errorText = string.Format("Material ID is empty");
                    invalidData = mapSetupData;
                }

                if (result == true && string.IsNullOrEmpty(mapSetupData.FlatNotchLocation) == false)
                {
                    compact = mapSetupData.FlatNotchLocation.Trim();

                    converted = this._messageProcessor.ConvertValue(fnLocFormat, compact);

                    if (converted == null)
                    {
                        result = false;
                        errorText = string.Format(" Can not convert \n\n MatrialID: {0} \n\n FNLOC Format: {1}", mapSetupData.MaterialID, fnLocFormat.ToString());
                        invalidData = mapSetupData;
                    }
                }

                if (result == true && string.IsNullOrEmpty(mapSetupData.FilmFrameRotation) == false)
                {
                    compact = mapSetupData.FilmFrameRotation.Trim();

                    converted = this._messageProcessor.ConvertValue(ffRotFormat, compact);

                    if (converted == null)
                    {
                        result = false;
                        errorText = string.Format(" Can not convert \n\n MatrialID: {0} \n\n FFROT Format: {1}", mapSetupData.MaterialID, ffRotFormat.ToString());
                        invalidData = mapSetupData;
                    }
                }

                if (result == true && string.IsNullOrEmpty(mapSetupData.OriginLocation) == false)
                {
                    compact = mapSetupData.OriginLocation.Trim();

                    converted = this._messageProcessor.ConvertValue(orLocFormat, compact);

                    if (converted == null)
                    {
                        result = false;
                        errorText = string.Format(" Can not convert \n\n MatrialID: {0} \n\n ORLOC Format: {1}", mapSetupData.MaterialID, orLocFormat.ToString());
                        invalidData = mapSetupData;
                    }
                }

                if (result == true && string.IsNullOrEmpty(mapSetupData.ProcessDieCount) == false)
                {
                    compact = mapSetupData.ProcessDieCount.Trim();

                    converted = this._messageProcessor.ConvertValue(prdCtFormat, compact);

                    if (converted == null)
                    {
                        result = false;
                        errorText = string.Format(" Can not convert \n\n MatrialID: {0} \n\n PRDCT Format: {1}", mapSetupData.MaterialID, prdCtFormat.ToString());
                        invalidData = mapSetupData;
                    }
                }

                if (result == true && string.IsNullOrEmpty(mapSetupData.XAxisDieSize) == false)
                {
                    compact = mapSetupData.XAxisDieSize.Trim();

                    converted = this._messageProcessor.ConvertValue(xdiesFormat, compact);

                    if (converted == null)
                    {
                        result = false;
                        errorText = string.Format(" Can not convert \n\n MatrialID: {0} \n\n XDIES Format: {1}", mapSetupData.MaterialID, xdiesFormat.ToString());
                        invalidData = mapSetupData;
                    }
                }

                if (result == true && string.IsNullOrEmpty(mapSetupData.YAxisDieSize) == false)
                {
                    compact = mapSetupData.YAxisDieSize.Trim();

                    converted = this._messageProcessor.ConvertValue(ydiesFormat, compact);

                    if (converted == null)
                    {
                        result = false;
                        errorText = string.Format(" Can not convert \n\n MatrialID: {0} \n\n YDIES Format: {1}", mapSetupData.MaterialID, ydiesFormat.ToString());
                        invalidData = mapSetupData;
                    }
                }

                if (result == true)
                {
                    if (usedMaterialIDs.Contains(mapSetupData.MaterialID) == true)
                    {
                        result = false;
                        errorText = string.Format(" MapSetupData: {0} \n\n Material ID is dupelicated", mapSetupData.MaterialID);
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

                    foreach (var rpItem in mapSetupData.ReferencePoint)
                    {
                        if (result == true)
                        {
                            if (usedRefrencePoints.Contains(rpItem.DataString()) == true)
                            {
                                result = false;
                                errorText = string.Format(" MID: {0} \n\n Reference Point: ({1},{2}) is dupelicated", mapSetupData.MaterialID, rpItem.X, rpItem.Y);
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