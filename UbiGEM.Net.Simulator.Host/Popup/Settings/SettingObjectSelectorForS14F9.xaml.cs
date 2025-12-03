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
    public partial class SettingObjectSelectorForS14F9 : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private ObservableCollection<GEMObjectAttribute> _displayAttrIDList;
        #endregion
        #region Constructor
        public SettingObjectSelectorForS14F9()
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

                selectedObjectSpecifier = this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F9;

                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;
                }

                RefreshObjectTypeComboBox();

                selectedObjectType = this._messageProcessor.CurrentSetting.SelectedObjectTypeForS14F9;

                if (string.IsNullOrEmpty(selectedObjectType) == false)
                {
                    cboObjectType.SelectedItem = selectedObjectType;
                }

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
        private void RefreshObjectAttrIDDataGrid()
        {
            string selectedObjectSpecifier;
            string selectedObjectType;
            GEMObject gemObject;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;
            selectedObjectType = cboObjectType.SelectedItem as string;

            this._displayAttrIDList.Clear();

            if (string.IsNullOrEmpty(selectedObjectSpecifier) == false && string.IsNullOrEmpty(selectedObjectType) == false)
            {
                gemObject = this._messageProcessor.GEMObjectCollection.Items.FirstOrDefault(t => t.OBJSPEC == selectedObjectSpecifier && t.OBJTYPE == selectedObjectType);

                if (gemObject == null)
                {
                    gemObject = new GEMObject()
                    {
                        OBJSPEC = selectedObjectSpecifier,
                        OBJTYPE = selectedObjectType,
                    };

                    this._messageProcessor.GEMObjectCollection.Add(gemObject);
                }

                if (gemObject != null)
                {
                    foreach (var gemObjectAttribute in gemObject.AttributeCollection.Items)
                    {
                        if (this._displayAttrIDList.FirstOrDefault(t => t.ATTRID == gemObjectAttribute.ATTRID) == null)
                        {
                            this._displayAttrIDList.Add(gemObjectAttribute);
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
                this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F9 = selectedObjectSpecifier;
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
                this._messageProcessor.CurrentSetting.SelectedObjectTypeForS14F9 = selectedObjectType;
            }

            RefreshObjectAttrIDDataGrid();
        }
        #endregion
        #region DataGridEvent
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