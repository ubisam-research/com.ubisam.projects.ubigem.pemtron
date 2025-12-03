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
    public partial class GEMObjectWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;

        private ObservableCollection<GEMObject> _gemObjectItems;
        private ObservableCollection<GEMObjectID> _gemObjectIDItems;
        private ObservableCollection<GEMObjectAttribute> _gemObjectAttributeItems;
        #endregion
        #region Constructor
        public GEMObjectWindow()
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

            ctlFunctionObjectID.ChangeButtonEnabled(false, false);
            ctlObjectIDUpDown.ChangeButtonEnabledAll(false);
            ctlFunctionAttributes.ChangeButtonEnabled(false, false);
            ctlAttrUpDown.ChangeButtonEnabledAll(false);

            this._gemObjectItems = new ObservableCollection<GEMObject>();

            dgrSpecifier.ItemsSource = this._gemObjectItems;

            foreach (GEMObject gemObject in this._messageProcessor.GEMObjectCollection.Items)
            {
                this._gemObjectItems.Add(gemObject.Clone());
            }

            if (this._gemObjectItems.Count > 0)
            {
                dgrSpecifier.SelectedItem = this._gemObjectItems[0];

                ctlFunctionObjectID.ChangeButtonEnabled(true, true);
                ctlObjectIDUpDown.ChangeButtonEnabledAll(true);

                this._gemObjectIDItems = new ObservableCollection<GEMObjectID>();
                dgrObjectID.ItemsSource = this._gemObjectIDItems;

                foreach (GEMObjectID gemObjectID in this._gemObjectItems[0].ObjectIDCollection.Items)
                {
                    this._gemObjectIDItems.Add(gemObjectID);
                }

                if (this._gemObjectIDItems.Count > 0)
                {
                    dgrObjectID.SelectedItem = this._gemObjectIDItems[0];

                    ctlFunctionAttributes.ChangeButtonEnabled(true, true);
                    ctlAttrUpDown.ChangeButtonEnabledAll(true);

                    this._gemObjectAttributeItems = new ObservableCollection<GEMObjectAttribute>();
                    dgrAttributes.ItemsSource = this._gemObjectAttributeItems;

                    foreach (GEMObjectAttribute attr in this._gemObjectIDItems[0].ObjectAttributeCollection.Items)
                    {
                        this._gemObjectAttributeItems.Add(attr);
                    }
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

                this._messageProcessor.GEMObjectCollection.Clear();

                foreach (GEMObject gemObject in this._gemObjectItems)
                {
                    this._messageProcessor.GEMObjectCollection.Add(gemObject);
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
            GEMObject invalidObject;
            GEMObjectID invalidObjectID;
            GEMObjectAttribute invalidAttr;

            result = true;
            errorText = string.Empty;
            invalidObject = null;
            invalidObjectID = null;
            invalidAttr = null;

            foreach (GEMObject gemObject in this._gemObjectItems)
            {
                if(result == true && gemObject.Validate(out errorText) == false)
                {
                    result = false;
                    invalidObject = gemObject;
                }

                if (result == true)
                {
                    foreach (GEMObjectID gemObjectID in gemObject.ObjectIDCollection.Items)
                    {
                        if (result == true && gemObject.Validate(out errorText) == false)
                        {
                            result = false;
                            invalidObject = gemObject;
                            invalidObjectID = gemObjectID;
                        }

                        if (result == true)
                        {
                            foreach (GEMObjectAttribute attr in gemObjectID.ObjectAttributeCollection.Items)
                            {
                                if (result == true && attr.Validate(out errorText) == false)
                                {
                                    result = false;
                                    invalidObject = gemObject;
                                    invalidObjectID = gemObjectID;
                                    invalidAttr = attr;
                                }
                            }
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
                if (invalidObjectID != null)
                {
                    dgrSpecifier.SelectedItem = invalidObject;
                    dgrObjectID.SelectedItem = invalidObjectID;
                }
                if (invalidAttr != null)
                {
                    dgrSpecifier.SelectedItem = invalidObject;
                    dgrObjectID.SelectedItem = invalidObjectID;
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
            GEMObject gemObject;

            gemObject = new GEMObject();

            this._gemObjectItems.Add(gemObject);
            dgrAttributes.ItemsSource = null;
            dgrSpecifier.SelectedItem = gemObject;
        }
        private void Specifier_OnRemove(object sender, EventArgs e)
        {
            GEMObject gemObject;

            gemObject = dgrSpecifier.SelectedItem as GEMObject;

            if (gemObject != null)
            {
                this._gemObjectItems.Remove(gemObject);

                dgrSpecifier.SelectedItem = null;
                dgrObjectID.ItemsSource = null;
                dgrAttributes.ItemsSource = null;
            }
        }
        #endregion
        #region ObjectID Add/Remove
        private void ObjectID_OnAdd(object sender, EventArgs e)
        {
            GEMObject gemObject;
            GEMObjectID gemObjectID;

            gemObject = dgrSpecifier.SelectedItem as GEMObject;

            if (gemObject != null)
            {
                gemObjectID = new GEMObjectID();

                gemObject.ObjectIDCollection.Add(gemObjectID);
                this._gemObjectIDItems.Add(gemObjectID);
                dgrObjectID.SelectedItem = gemObjectID;
            }
        }
        private void ObjectID_OnRemove(object sender, EventArgs e)
        {
            GEMObject gemObject;
            GEMObjectID gemObjectID;

            gemObject = dgrSpecifier.SelectedItem as GEMObject;
            gemObjectID = dgrObjectID.SelectedItem as GEMObjectID;

            if (gemObject != null && gemObjectID != null)
            {
                gemObject.ObjectIDCollection.Remove(gemObjectID);
                this._gemObjectIDItems.Remove(gemObjectID);
                dgrObjectID.SelectedItem = null;
                dgrAttributes.ItemsSource = null;
            }
        }
        #endregion
        #region Attributes Add/Remove
        private void Attributes_OnAdd(object sender, EventArgs e)
        {
            GEMObjectID gemObjectID;
            GEMObjectAttribute attr;

            gemObjectID = dgrObjectID.SelectedItem as GEMObjectID;

            if (gemObjectID != null)
            {
                attr = new GEMObjectAttribute();

                gemObjectID.ObjectAttributeCollection.Add(attr);
                this._gemObjectAttributeItems.Add(attr);

                dgrAttributes.SelectedItem = attr;
            }
        }
        private void Attributes_OnRemove(object sender, EventArgs e)
        {
            GEMObjectID gemObjectID;
            GEMObjectAttribute attr;

            gemObjectID = dgrObjectID.SelectedItem as GEMObjectID;
            attr = dgrAttributes.SelectedItem as GEMObjectAttribute;

            if (gemObjectID != null && attr != null)
            {
                gemObjectID.ObjectAttributeCollection.Remove(attr);
                this._gemObjectAttributeItems.Remove(attr);
                dgrAttributes.SelectedItem = null;
            }
        }
        #endregion
        #endregion
        #region UpDown Event
        #region ObjectID Up/Down
        private void ctlObjectIDUpDown_OnFirst(object sender, EventArgs e)
        {
            GEMObject selectedObject;
            GEMObjectID selectedObjectID;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as GEMObject;
            selectedIndex = dgrObjectID.SelectedIndex;

            if (selectedObject != null && selectedIndex > 0)
            {
                this._gemObjectIDItems.Move(selectedIndex, 0);

                selectedObjectID = selectedObject.ObjectIDCollection.Items[selectedIndex];
                selectedObject.ObjectIDCollection.Items.RemoveAt(selectedIndex);
                selectedObject.ObjectIDCollection.Items.Insert(0, selectedObjectID);
            }
        }
        private void ctlObjectIDUpDown_OnUp(object sender, EventArgs e)
        {
            GEMObject selectedObject;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as GEMObject;
            selectedIndex = dgrObjectID.SelectedIndex;

            if (selectedObject != null && selectedIndex > 0)
            {
                this._gemObjectIDItems.Move(selectedIndex, selectedIndex - 1);
                selectedObject.ObjectIDCollection.Items.Reverse(selectedIndex - 1, 2);
            }
        }
        private void ctlObjectIDUpDown_OnDown(object sender, EventArgs e)
        {
            GEMObject selectedObject;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as GEMObject;
            selectedIndex = dgrObjectID.SelectedIndex;

            if (selectedObject != null && selectedIndex >= 0 && selectedIndex < this._gemObjectIDItems.Count - 1)
            {
                this._gemObjectIDItems.Move(selectedIndex, selectedIndex + 1);
                selectedObject.ObjectIDCollection.Items.Reverse(selectedIndex, 2);
            }
        }
        private void ctlObjectIDUpDown_OnLast(object sender, EventArgs e)
        {
            GEMObject selectedObject;
            GEMObjectID selectedObjectID;
            int selectedIndex;

            selectedObject = dgrSpecifier.SelectedItem as GEMObject;
            selectedIndex = dgrObjectID.SelectedIndex;

            if (selectedObject != null && selectedIndex >= 0 && selectedIndex < this._gemObjectIDItems.Count - 1)
            {
                this._gemObjectIDItems.Move(selectedIndex, this._gemObjectIDItems.Count - 1);

                selectedObjectID = selectedObject.ObjectIDCollection.Items[selectedIndex];
                selectedObject.ObjectIDCollection.Items.RemoveAt(selectedIndex);
                selectedObject.ObjectIDCollection.Items.Add(selectedObjectID);
            }
        }
        #endregion
        #region Attribute Up/Down
        private void ctlAttrUpDown_OnFirst(object sender, EventArgs e)
        {
            GEMObjectID selectedObjectID;
            GEMObjectAttribute selectedAttr;
            int selectedIndex;

            selectedObjectID = dgrObjectID.SelectedItem as GEMObjectID;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObjectID != null && selectedIndex > 0)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, 0);

                selectedAttr = selectedObjectID.ObjectAttributeCollection.Items[selectedIndex];
                selectedObjectID.ObjectAttributeCollection.Items.RemoveAt(selectedIndex);
                selectedObjectID.ObjectAttributeCollection.Items.Insert(0, selectedAttr);
            }
        }
        private void ctlAttrUpDown_OnUp(object sender, EventArgs e)
        {
            GEMObjectID selectedObjectID;
            int selectedIndex;

            selectedObjectID = dgrObjectID.SelectedItem as GEMObjectID;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObjectID != null && selectedIndex > 0)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, selectedIndex - 1);
                selectedObjectID.ObjectAttributeCollection.Items.Reverse(selectedIndex - 1, 2);
            }
        }
        private void ctlAttrUpDown_OnDown(object sender, EventArgs e)
        {
            GEMObjectID selectedObjectID;
            int selectedIndex;

            selectedObjectID = dgrObjectID.SelectedItem as GEMObjectID;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObjectID != null && selectedIndex >= 0 && selectedIndex < this._gemObjectAttributeItems.Count - 1)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, selectedIndex + 1);
                selectedObjectID.ObjectAttributeCollection.Items.Reverse(selectedIndex, 2);
            }
        }
        private void ctlAttrUpDown_OnLast(object sender, EventArgs e)
        {
            GEMObjectID selectedObjectID;
            GEMObjectAttribute selectedAttr;
            int selectedIndex;

            selectedObjectID = dgrObjectID.SelectedItem as GEMObjectID;
            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedObjectID != null && selectedIndex >= 0 && selectedIndex < this._gemObjectAttributeItems.Count - 1)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, this._gemObjectIDItems.Count - 1);

                selectedAttr = selectedObjectID.ObjectAttributeCollection.Items[selectedIndex];
                selectedObjectID.ObjectAttributeCollection.Items.RemoveAt(selectedIndex);
                selectedObjectID.ObjectAttributeCollection.Items.Add(selectedAttr);
            }
        }
        #endregion
        #endregion
        #region DataGrid Event
        private void dgrSpecifier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GEMObject selectedItem;

            selectedItem = dgrSpecifier.SelectedItem as GEMObject;

            if (selectedItem != null)
            {
                this._gemObjectIDItems = new ObservableCollection<GEMObjectID>(selectedItem.ObjectIDCollection.Items);
                dgrObjectID.ItemsSource = this._gemObjectIDItems;

                dgrAttributes.ItemsSource = null;

                if (this._gemObjectIDItems.Count > 0)
                {
                    dgrObjectID.SelectedItem = this._gemObjectIDItems[0];
                }
            }
        }
        private void dgrObjectID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GEMObjectID selectedItem;

            selectedItem = dgrObjectID.SelectedItem as GEMObjectID;

            if (selectedItem != null)
            {
                this._gemObjectAttributeItems = new ObservableCollection<GEMObjectAttribute>(selectedItem.ObjectAttributeCollection.Items);
                dgrAttributes.ItemsSource = this._gemObjectAttributeItems;
                ctlFunctionAttributes.ChangeButtonEnabled(true, true);
                ctlAttrUpDown.ChangeButtonEnabledAll(true);
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
            GEMObjectListWindow window;
            GEMObject selectedObject;
            GEMObjectID selectedObjectID;

            GEMObjectAttribute attr;
            List<GEMObjectAttribute> attributeStack;

            selectedObject = dgrSpecifier.SelectedItem as GEMObject;
            selectedObjectID = dgrObjectID.SelectedItem as GEMObjectID;
            attr = dgrAttributes.SelectedItem as GEMObjectAttribute;

            if (selectedObject != null && selectedObjectID != null && attr != null && attr.Format == SECSItemFormat.L)
            {
                attributeStack = new List<GEMObjectAttribute>();
                attributeStack.Add(attr);

                window = new GEMObjectListWindow();
                window.Owner = this;
                window.Initialize(this._messageProcessor, selectedObject, selectedObjectID, attributeStack);
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