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
    public partial class SettingObjectSelectorForS14F15 : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private ObservableCollection<GEMObjectAttribute> _displayAttrIDList;
        #endregion
        #region Constructor
        public SettingObjectSelectorForS14F15()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string selectedObjectSpecifier;
            AttachedObjectActionInfo actionInfo;

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

                selectedObjectSpecifier = this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F15;

                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;
                    txtOBJCMD.IsReadOnly = false;
                    txtOBJTOKEN.IsReadOnly = false;

                    if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15.ContainsKey(selectedObjectSpecifier) == false)
                    {
                        this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                    }

                    if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] == null)
                    {
                        this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                    }

                    actionInfo = this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier];

                    txtOBJCMD.Text = actionInfo.OBJCMD.ToString();
                    txtOBJTOKEN.Text = actionInfo.OBJTOKEN.ToString();
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

            foreach (var gemObject in this._messageProcessor.SupervisedGEMObjectCollection.Items)
            {
                if (string.IsNullOrEmpty(gemObject.OBJSPEC) == false && objectSpecifierList.Contains(gemObject.OBJSPEC) == false)
                {
                    objectSpecifierList.Add(gemObject.OBJSPEC);
                }
            }

            cboObjectSpecifier.ItemsSource = objectSpecifierList;
        }
        private void RefreshObjectAttrIDDataGrid()
        {
            string selectedObjectSpecifier;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;

            this._displayAttrIDList.Clear();

            if (selectedObjectSpecifier != null)
            {
                foreach (var gemObject in this._messageProcessor.SupervisedGEMObjectCollection.Items.Where(t => t.OBJSPEC == selectedObjectSpecifier))
                {
                    foreach (var gemObjectAttribute in gemObject.GEMObjectAttributeCollection.Items)
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
            AttachedObjectActionInfo actionInfo;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
            {
                this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F15 = selectedObjectSpecifier;

                cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;
                txtOBJCMD.IsReadOnly = false;
                txtOBJTOKEN.IsReadOnly = false;

                if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15.ContainsKey(selectedObjectSpecifier) == false)
                {
                    this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                }

                if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] == null)
                {
                    this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                }

                actionInfo = this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier];

                txtOBJCMD.Text = actionInfo.OBJCMD.ToString();
                txtOBJTOKEN.Text = actionInfo.OBJTOKEN.ToString();
            }
            else
            {
                txtOBJCMD.IsReadOnly = true;
                txtOBJTOKEN.IsReadOnly = true;
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
        #region TextBox Event
        private void txtOBJCMD_TextChanged(object sender, TextChangedEventArgs e)
        {
            string selectedObjectSpecifier;
            AttachedObjectActionInfo actionInfo;
            byte objCMD;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
            {
                this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F15 = selectedObjectSpecifier;

                cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;

                if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15.ContainsKey(selectedObjectSpecifier) == false)
                {
                    this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                }

                if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] == null)
                {
                    this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                }

                actionInfo = this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier];

                if (byte.TryParse(txtOBJCMD.Text, out objCMD) == true)
                {
                    actionInfo.OBJCMD = objCMD;
                }
                else
                {
                    txtOBJCMD.Text = actionInfo.OBJCMD.ToString();
                }
            }
        }

        private void txtOBJTOKEN_TextChanged(object sender, TextChangedEventArgs e)
        {
            string selectedObjectSpecifier;
            AttachedObjectActionInfo actionInfo;
            uint objToken;

            selectedObjectSpecifier = cboObjectSpecifier.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
            {
                this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F15 = selectedObjectSpecifier;

                cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;

                if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15.ContainsKey(selectedObjectSpecifier) == false)
                {
                    this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                }

                if (this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] == null)
                {
                    this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier] = new AttachedObjectActionInfo();
                }

                actionInfo = this._messageProcessor.CurrentSetting.SelectedAttachedObjectActionListForS14F15[selectedObjectSpecifier];

                if (uint.TryParse(txtOBJTOKEN.Text, out objToken) == true)
                {
                    actionInfo.OBJTOKEN = objToken;
                }
                else
                {
                    txtOBJCMD.Text = actionInfo.OBJTOKEN.ToString();
                }
            }
        }
        #endregion
    }
}