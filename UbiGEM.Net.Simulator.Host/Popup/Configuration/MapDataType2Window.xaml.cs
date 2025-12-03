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
    public partial class MapDataType2Window : Window
    {
        #region MemberVariable
        private ObservableCollection<MapDataType2> _displayMapDataCollection;

        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public MapDataType2Window()
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

            this._displayMapDataCollection = new ObservableCollection<MapDataType2>();

            foreach (MapDataType2 mapDataType2 in this._messageProcessor.MapDataType2Collection)
            {
                this._displayMapDataCollection.Add(mapDataType2.Clone());
            }

            dgrMapSetupData.ItemsSource = this._displayMapDataCollection;
        }
        #endregion

        // DataGrid Event
        #region MapSetupData_OnAdd
        private void MapSetupData_OnAdd(object sender, EventArgs e)
        {
            MapDataType2 mapDataType2;

            mapDataType2 = new MapDataType2();

            this._displayMapDataCollection.Add(mapDataType2);
        }
        #endregion
        #region MapSetupData_OnRemove
        private void MapSetupData_OnRemove(object sender, EventArgs e)
        {
            MapDataType2 mapDataType2;

            mapDataType2 = dgrMapSetupData.SelectedItem as MapDataType2;

            if (mapDataType2 != null)
            {
                this._displayMapDataCollection.Remove(mapDataType2);
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
                this._messageProcessor.MapDataType2Collection.Clear();

                foreach (var mapSetupData in this._displayMapDataCollection)
                {
                    this._messageProcessor.MapDataType2Collection.Add(mapSetupData);
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

            MapDataType2 invalidData;

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
                        errorText = string.Format(" MapDataType2: {0} \n\n Material ID is dupelicated", mapSetupData.MaterialID);
                        invalidData = mapSetupData;
                    }
                    else
                    {
                        usedMaterialIDs.Add(mapSetupData.MaterialID);
                    }
                }
            }

            return result;
        }
        #endregion
    }
}