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

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingTerminalWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingObjectSelectorForS14F7 : Window
    {
        #region Wrapper
        class Wrapper
        {
            #region Property
            public bool IsSelected { get; set; }
            public string Value { get; set; }
            #endregion
            #region Contructor
            public Wrapper()
            {
                this.IsSelected = false;
                this.Value = string.Empty;
            }
            #endregion
        }
        #endregion
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private ObservableCollection<Wrapper> _displayTypeList;
        #endregion
        #region Constructor
        public SettingObjectSelectorForS14F7()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string selectedObjectSpecifier;

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

            if (this._messageProcessor != null)
            {
                RefreshObjectSpecifierComboBox();

                selectedObjectSpecifier = this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F7;

                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;
                }

                RefreshObjectTypeDataGrid();
            }
        }
        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.IsDirty = true;

            DialogResult = true;

            Close();
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
            grbTitle.Header = "Object Selector";

            this._displayTypeList = new ObservableCollection<Wrapper>();
            dgrObjectType.ItemsSource = this._displayTypeList;
        }
        #endregion
        #region Refresh
        private void RefreshObjectSpecifierComboBox()
        {
            List<string> objectSpecifierList;

            objectSpecifierList = new List<string>();

            foreach (var gemObject in this._messageProcessor.GEMObjectCollection.Items)
            {
                if (string.IsNullOrEmpty(gemObject.OBJSPEC) == false && objectSpecifierList.Contains(gemObject.OBJSPEC) == false)
                {
                    objectSpecifierList.Add(gemObject.OBJSPEC);
                }
            }

            cboObjectSpecifier.ItemsSource = objectSpecifierList;
        }
        private void RefreshObjectTypeDataGrid()
        {
            string selectedObjectSpecifier;
            List<string> selectedTypes;
            Wrapper wrapper;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;

            this._displayTypeList.Clear();

            if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
            {
                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7.ContainsKey(selectedObjectSpecifier) == false)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] = new List<string>();
                }

                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] == null)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] = new List<string>();
                }

                selectedTypes = this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier];

                foreach (var gemObject in this._messageProcessor.GEMObjectCollection.Items.Where(t => t.OBJSPEC == selectedObjectSpecifier))
                {
                    if (string.IsNullOrEmpty(gemObject.OBJTYPE) == false && this._displayTypeList.FirstOrDefault(t => t.Value == gemObject.OBJTYPE) == null)
                    {
                        wrapper = new Wrapper()
                        {
                            IsSelected = false,
                            Value = gemObject.OBJTYPE,
                        };

                        if (selectedTypes != null && selectedTypes.Contains(gemObject.OBJTYPE) == true)
                        {
                            wrapper.IsSelected = true;
                        }

                        this._displayTypeList.Add(wrapper);
                    }
                }
            }
        }
        #endregion
        #region [ComboBox Event]
        private void cboObjectSpecifier_DropDownOpened(object sender, EventArgs e)
        {
            string selectedObjectSpecifier;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;
            RefreshObjectSpecifierComboBox();

            if (selectedObjectSpecifier != null)
            {
                cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;
            }
        }
        private void cboObjectSpecifier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedObjectSpecifier;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;

            RefreshObjectTypeDataGrid();

            if (selectedObjectSpecifier != null)
            {
                this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F7 = selectedObjectSpecifier;
            }
        }
        #endregion
        #region DataGridEvent
        private void cboObjectTypeChecked(object sender, EventArgs e)
        {
            string selectedObjectSpecifier;
            Wrapper selectedItem;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;
            selectedItem = dgrObjectType.SelectedItem as Wrapper;

            if (selectedObjectSpecifier != null && selectedItem != null)
            {
                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7.ContainsKey(selectedObjectSpecifier) == false)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] = new List<string>();
                }

                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] == null)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] = new List<string>();
                }

                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier].Contains(selectedItem.Value) == false)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier].Add(selectedItem.Value);
                }
            }
        }
        private void cboObjectTypeUnchecked(object sender, EventArgs e)
        {
            string selectedObjectSpecifier;
            Wrapper selectedItem;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;
            selectedItem = dgrObjectType.SelectedItem as Wrapper;

            if (selectedObjectSpecifier != null && selectedItem != null)
            {
                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7.ContainsKey(selectedObjectSpecifier) == false)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] = new List<string>();
                }

                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] == null)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier] = new List<string>();
                }

                if (this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier].Contains(selectedItem.Value) == true)
                {
                    this._messageProcessor.CurrentSetting.SelectedObjectTypeListForS14F7[selectedObjectSpecifier].Remove(selectedItem.Value);
                }
            }
        }
        #endregion
    }
}