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
    public partial class SettingObjectSelectorForS14F3 : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private ObservableCollection<GEMObjectID> _displayObjectIDList;
        private ObservableCollection<GEMObjectAttribute> _displayAttrIDList;
        #endregion
        #region Constructor
        public SettingObjectSelectorForS14F3()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string selectedObjectSpecifier;
            string selectedObjectType;

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

                selectedObjectSpecifier = this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F3;

                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;
                }

                RefreshObjectTypeComboBox();

                selectedObjectType = this._messageProcessor.CurrentSetting.SelectedObjectTypeForS14F3;

                if (string.IsNullOrEmpty(selectedObjectType) == false)
                {
                    cboObjectType.SelectedItem = selectedObjectType;
                }

                RefreshObjectIDDataGrid();

                RefreshObjectAttrIDDataGrid();
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

            this._displayObjectIDList = new ObservableCollection<GEMObjectID>();
            dgrObjectID.ItemsSource = this._displayObjectIDList;

            this._displayAttrIDList = new ObservableCollection<GEMObjectAttribute>();
            dgrAttributes.ItemsSource = this._displayAttrIDList;
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
        private void RefreshObjectTypeComboBox()
        {
            List<string> objectTypeList;
            string selectedObjectSpecifier;

            objectTypeList = new List<string>();
            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;

            foreach (var gemObject in this._messageProcessor.GEMObjectCollection.Items)
            {
                if (string.IsNullOrEmpty(gemObject.OBJTYPE) == false && selectedObjectSpecifier == gemObject.OBJSPEC && objectTypeList.Contains(gemObject.OBJTYPE) == false)
                {
                    objectTypeList.Add(gemObject.OBJTYPE);
                }
            }

            cboObjectType.ItemsSource = objectTypeList;
        }
        private void RefreshObjectIDDataGrid()
        {
            string selectedObjectSpecifier;
            string selectedObjectType;
            this._displayObjectIDList.Clear();

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;
            selectedObjectType = cboObjectType.SelectedItem as string;

            this._displayObjectIDList.Clear();

            if (string.IsNullOrEmpty(selectedObjectSpecifier) == false && string.IsNullOrEmpty(selectedObjectType) == false
                && this._messageProcessor.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType) != null)
            {
                foreach (var gemObjectID in this._messageProcessor.GEMObjectCollection.Items.Where(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType).SelectMany(t => t.ObjectIDCollection.Items))
                {
                    if (string.IsNullOrEmpty(gemObjectID.OBJID) == false && this._displayObjectIDList.FirstOrDefault(t => t.OBJID == gemObjectID.OBJID) == null)
                    {
                        this._displayObjectIDList.Add(gemObjectID);
                    }
                }
            }
        }
        private void RefreshObjectAttrIDDataGrid()
        {
            string selectedObjectSpecifier;
            string selectedObjectType;
            List<string> selectedObjectIDList;
            GEMObject gemObject;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;
            selectedObjectType = cboObjectType.SelectedItem as string;

            this._displayAttrIDList.Clear();

            if (selectedObjectSpecifier != null && selectedObjectType != null)
            {
                selectedObjectIDList = new List<string>();

                gemObject = this._messageProcessor.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType);

                if (gemObject != null)
                {
                    foreach (var selectedObjectID in gemObject.ObjectIDCollection.Items.Where(t => t.IsSelected == true).Select(t => t.OBJID))
                    {
                        selectedObjectIDList.Add(selectedObjectID);
                    }

                }

                if (gemObject != null)
                {
                    foreach (var gemObjectID in gemObject.ObjectIDCollection.Items)
                    {
                        foreach (var gemObjectAttribute in gemObjectID.ObjectAttributeCollection.Items)
                        {
                            if (this._displayAttrIDList.FirstOrDefault(t => t.ATTRID == gemObjectAttribute.ATTRID) == null)
                            {
                                if (selectedObjectIDList.Count == 0)
                                {
                                    this._displayAttrIDList.Add(gemObjectAttribute);
                                }
                                else
                                {
                                    if (selectedObjectIDList.Contains(gemObjectID.OBJID) == true)
                                    {
                                        this._displayAttrIDList.Add(gemObjectAttribute);
                                    }
                                }
                            }
                        }
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
            string selectedObjectType;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;
            if (selectedObjectSpecifier != null)
            {
                this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F3 = selectedObjectSpecifier;
            }

            selectedObjectType = cboObjectType.SelectedItem as string;
            RefreshObjectTypeComboBox();

            if (selectedObjectType != null)
            {
                cboObjectType.SelectedItem = selectedObjectType;
            }
        }
        private void cboObjectType_DropDownOpened(object sender, EventArgs e)
        {
            string selectedObjectType;

            selectedObjectType = cboObjectType.SelectedItem as string;
            RefreshObjectTypeComboBox();

            if (selectedObjectType != null)
            {
                cboObjectType.SelectedItem = selectedObjectType;
            }
        }
        private void cboObjectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedObjectType;

            selectedObjectType = cboObjectType.SelectedItem as string;

            if (selectedObjectType != null)
            {
                this._messageProcessor.CurrentSetting.SelectedObjectTypeForS14F3 = selectedObjectType;
            }

            RefreshObjectIDDataGrid();
            RefreshObjectAttrIDDataGrid();
        }
        #endregion
        #region DataGridEvent
        private void cboObjectIDChecked(object sender, EventArgs e)
        {
            GEMObjectID selectedItem;

            selectedItem = dgrObjectID.SelectedItem as GEMObjectID;

            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
            }

            RefreshObjectAttrIDDataGrid();
        }
        private void cboObjectIDUnchecked(object sender, EventArgs e)
        {
            GEMObjectID selectedItem;
            
            selectedItem = dgrObjectID.SelectedItem as GEMObjectID;

            if (selectedItem != null)
            {
                selectedItem.IsSelected = false;
            }

            RefreshObjectAttrIDDataGrid();
        }
        private void dgcAttrFormat_SelectionChanged(object sender, EventArgs e)
        {
            GEMObjectAttribute selectedItem;
            ComboBox comboBox;
            SECSItemFormat format;

            comboBox = sender as ComboBox;
            selectedItem = dgrAttributes.SelectedItem as GEMObjectAttribute;

            if (selectedItem != null && comboBox != null && comboBox.SelectedItem != null)
            {
                if (Enum.TryParse(comboBox.SelectedItem.ToString(), out format) == true)
                {
                    selectedItem.Format = format;
                }
            }
        }
        private void txtAttributeValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            GEMObjectAttribute attr;
            TextBox textBox;

            attr = dgrAttributes.SelectedItem as GEMObjectAttribute;

            textBox = e.Source as TextBox;

            if (attr != null && textBox != null)
            {
                if (string.IsNullOrEmpty(textBox.Text) == false)
                {
                    if (attr.Format == SECSItemFormat.A || attr.Format == SECSItemFormat.J)
                    {
                        attr.ATTRDATA = textBox.Text;
                    }
                    else
                    {
                        if (this._messageProcessor.ConvertValue(attr.Format, textBox.Text) == null)
                        {
                            textBox.Text = attr.ATTRDATA;
                        }
                        else
                        {
                            attr.ATTRDATA = textBox.Text;
                        }
                    }
                }
                else
                {
                    attr.ATTRDATA = textBox.Text;
                }
            }
        }
        #endregion
    }
}