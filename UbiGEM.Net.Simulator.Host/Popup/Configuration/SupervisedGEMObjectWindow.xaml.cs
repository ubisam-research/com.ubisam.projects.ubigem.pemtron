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
    public partial class SupervisedGEMObjectWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;

        private ObservableCollection<SupervisedGEMObject> _gemObjectItems;
        private ObservableCollection<GEMObjectAttribute> _gemObjectAttributeItems;
        #endregion
        #region Constructor
        public SupervisedGEMObjectWindow()
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

            ctlFunctionAttributes.ChangeButtonEnabled(false, false);
            ctlAttrUpDown.ChangeButtonEnabledAll(false);

            this._gemObjectItems = new ObservableCollection<SupervisedGEMObject>();

            dgrSpecifier.ItemsSource = this._gemObjectItems;

            foreach (SupervisedGEMObject gemObject in this._messageProcessor.SupervisedGEMObjectCollection.Items)
            {
                this._gemObjectItems.Add(gemObject.Clone());
            }

            if (this._gemObjectItems.Count > 0)
            {
                dgrSpecifier.SelectedItem = this._gemObjectItems[0];

                ctlFunctionAttributes.ChangeButtonEnabled(true, true);
                ctlAttrUpDown.ChangeButtonEnabledAll(true);

                this._gemObjectAttributeItems = new ObservableCollection<GEMObjectAttribute>();
                dgrAttributes.ItemsSource = this._gemObjectAttributeItems;

                foreach (GEMObjectAttribute attr in this._gemObjectItems[0].GEMObjectAttributeCollection.Items)
                {
                    this._gemObjectAttributeItems.Add(attr);
                }
            }
        }
        #endregion

        // DataGrid Event

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

                this._messageProcessor.SupervisedGEMObjectCollection.Clear();

                foreach (SupervisedGEMObject gemObject in this._gemObjectItems)
                {
                    this._messageProcessor.SupervisedGEMObjectCollection.Add(gemObject);
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
            SupervisedGEMObject invalidObject;
            GEMObjectAttribute invalidAttr;

            result = true;
            errorText = string.Empty;
            invalidObject = null;
            invalidAttr = null;

            foreach (SupervisedGEMObject gemObject in this._gemObjectItems)
            {
                if (result == true && gemObject.Validate(out errorText) == false)
                {
                    result = false;
                    invalidObject = gemObject;
                }

                if (result == true)
                {
                    foreach (GEMObjectAttribute attr in gemObject.GEMObjectAttributeCollection.Items)
                    {
                        if (result == true && attr.Validate(out errorText) == false)
                        {
                            result = false;
                            invalidObject = gemObject;
                            invalidAttr = attr;
                        }
                    }
                }
            }

            if (result == false)
            {
                if (invalidObject != null)
                {
                    dgrSpecifier.SelectedItem = invalidObject;
                }
                if (invalidAttr != null)
                {
                    dgrSpecifier.SelectedItem = invalidObject;
                    dgrAttributes.SelectedItem = invalidAttr;
                }
            }
            return result;
        }
        #endregion

        #region Add/Remote Event
        #region Specifier Add/Remove
        private void Specifier_OnAdd(object sender, EventArgs e)
        {
            SupervisedGEMObject gemObject;

            gemObject = new SupervisedGEMObject();

            this._gemObjectItems.Add(gemObject);
            dgrAttributes.ItemsSource = null;
            dgrSpecifier.SelectedItem = gemObject;
        }
        private void Specifier_OnRemove(object sender, EventArgs e)
        {
            SupervisedGEMObject gemObject;

            gemObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;

            if (gemObject != null)
            {
                this._gemObjectItems.Remove(gemObject);

                dgrSpecifier.SelectedItem = null;
                dgrAttributes.ItemsSource = null;
            }
        }
        #endregion
        #region Attributes Add/Remove
        private void Attributes_OnAdd(object sender, EventArgs e)
        {
            SupervisedGEMObject gemObject;
            GEMObjectAttribute attr;

            gemObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;

            if (gemObject != null)
            {
                attr = new GEMObjectAttribute();

                gemObject.GEMObjectAttributeCollection.Add(attr);
                this._gemObjectAttributeItems.Add(attr);

                dgrAttributes.SelectedItem = attr;
            }
        }
        private void Attributes_OnRemove(object sender, EventArgs e)
        {
            SupervisedGEMObject gemObject;
            GEMObjectAttribute attr;

            gemObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;
            attr = dgrAttributes.SelectedItem as GEMObjectAttribute;

            if (gemObject != null && attr != null)
            {
                gemObject.GEMObjectAttributeCollection.Remove(attr);
                this._gemObjectAttributeItems.Remove(attr);
                dgrAttributes.SelectedItem = null;
            }
        }
        #endregion
        #endregion
        #region UpDown Event
        #region Attribute Up/Down
        private void ctlAttrUpDown_OnFirst(object sender, EventArgs e)
        {
            SupervisedGEMObject selectedObject;
            GEMObjectAttribute selectedAttr;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObject != null && selectedIndex > 0)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, 0);

                selectedAttr = selectedObject.GEMObjectAttributeCollection.Items[selectedIndex];
                selectedObject.GEMObjectAttributeCollection.Items.RemoveAt(selectedIndex);
                selectedObject.GEMObjectAttributeCollection.Items.Insert(0, selectedAttr);
            }
        }
        private void ctlAttrUpDown_OnUp(object sender, EventArgs e)
        {
            SupervisedGEMObject selectedObject;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObject != null && selectedIndex > 0)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, selectedIndex - 1);
                selectedObject.GEMObjectAttributeCollection.Items.Reverse(selectedIndex - 1, 2);
            }
        }
        private void ctlAttrUpDown_OnDown(object sender, EventArgs e)
        {
            SupervisedGEMObject selectedObject;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObject != null && selectedIndex >= 0 && selectedIndex < this._gemObjectAttributeItems.Count - 1)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, selectedIndex + 1);
                selectedObject.GEMObjectAttributeCollection.Items.Reverse(selectedIndex, 2);
            }
        }
        private void ctlAttrUpDown_OnLast(object sender, EventArgs e)
        {
            SupervisedGEMObject selectedObject;
            GEMObjectAttribute selectedAttr;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObject != null && selectedIndex >= 0 && selectedIndex < this._gemObjectAttributeItems.Count - 1)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, this._gemObjectAttributeItems.Count - 1);

                selectedAttr = selectedObject.GEMObjectAttributeCollection.Items[selectedIndex];
                selectedObject.GEMObjectAttributeCollection.Items.RemoveAt(selectedIndex);
                selectedObject.GEMObjectAttributeCollection.Items.Add(selectedAttr);
            }
        }
        #endregion
        #endregion
        #region DataGrid Event
        private void dgrSpecifier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SupervisedGEMObject selectedItem;

            selectedItem = dgrSpecifier.SelectedItem as SupervisedGEMObject;

            if (selectedItem != null)
            {
                ctlFunctionAttributes.ChangeButtonEnabled(true, true);
                ctlAttrUpDown.ChangeButtonEnabledAll(true);

                this._gemObjectAttributeItems = new ObservableCollection<GEMObjectAttribute>(selectedItem.GEMObjectAttributeCollection.Items);
                dgrAttributes.ItemsSource = this._gemObjectAttributeItems;
            }
            else
            {
                ctlFunctionAttributes.ChangeButtonEnabled(false, false);
                ctlAttrUpDown.ChangeButtonEnabledAll(false);
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
        private void dgrAttributes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SupervisedGEMObjectListWindow window;
            SupervisedGEMObject selectedObject;

            GEMObjectAttribute attr;
            List<GEMObjectAttribute> attributeStack;

            selectedObject = dgrSpecifier.SelectedItem as SupervisedGEMObject;
            attr = dgrAttributes.SelectedItem as GEMObjectAttribute;

            if (selectedObject != null && attr != null && attr.Format == SECSItemFormat.L)
            {
                attributeStack = new List<GEMObjectAttribute>();
                attributeStack.Add(attr);

                window = new SupervisedGEMObjectListWindow();
                window.Owner = this;
                window.Initialize(this._messageProcessor, selectedObject, attributeStack);
                window.ShowDialog();
            }
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
        #endregion
    }
}