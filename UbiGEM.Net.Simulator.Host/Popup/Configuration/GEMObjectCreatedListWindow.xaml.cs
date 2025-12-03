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
    public partial class GEMObjectCreatedListWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;

        private GEMObject _gemObject;
        private List<GEMObjectAttribute> _attributeStack;
        private ObservableCollection<GEMObjectAttribute> _gemObjectAttributeItems;
        #endregion
        #region Constructor
        public GEMObjectCreatedListWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GEMObjectAttribute parentItem;

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

            lblOBJSPEC.Content = this._gemObject.OBJSPEC;
            lblOBJTYPE.Content = this._gemObject.OBJTYPE;

            dgrAttrStack.ItemsSource = this._attributeStack;

            this._gemObjectAttributeItems = new ObservableCollection<GEMObjectAttribute>();

            parentItem = this._attributeStack.LastOrDefault();

            if (parentItem != null)
            {
                foreach (GEMObjectAttribute child in parentItem.ChildAttributes.Items)
                {
                    this._gemObjectAttributeItems.Add(child.Clone());
                }
            }

            dgrAttributes.ItemsSource = this._gemObjectAttributeItems;
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            GEMObjectAttribute parentItem;

            parentItem = this._attributeStack.LastOrDefault();

            errorText = string.Empty;

            if (IsValid(out errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                parentItem.ChildAttributes.Clear();
                parentItem.ChildAttributes.Items.AddRange(this._gemObjectAttributeItems);
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
        public void Initialize(MessageProcessor messageProcessor, GEMObject gemObject, List<GEMObjectAttribute> attributeStack)
        {
            this._messageProcessor = messageProcessor;
            this._gemObject = gemObject;
            this._attributeStack = attributeStack;
        }
        #endregion

        // Private Method
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            GEMObjectAttribute parentItem;
            GEMObjectAttribute invalidAttr;

            result = true;
            errorText = string.Empty;
            invalidAttr = null;

            parentItem = this._attributeStack.LastOrDefault();

            if (parentItem != null && this._gemObjectAttributeItems.Count > 0)
            {
                foreach (GEMObjectAttribute attr in this._gemObjectAttributeItems)
                {
                    if (result == true && attr.Validate(out errorText) == false)
                    {
                        result = false;
                        invalidAttr = attr;
                    }
                }
            }

            if (result == false)
            {
                if (invalidAttr != null)
                {
                    dgrAttributes.SelectedItem = invalidAttr;
                }
            }
            return result;
        }
        #endregion

        #region Add/Remote Event
        #region Attributes Add/Remove
        private void Attributes_OnAdd(object sender, EventArgs e)
        {
            GEMObjectAttribute parentItem;
            GEMObjectAttribute newItem;

            parentItem = this._attributeStack.LastOrDefault();

            if (parentItem != null)
            {
                newItem = new GEMObjectAttribute();
                this._gemObjectAttributeItems.Add(newItem);
                dgrAttributes.SelectedItem = newItem;
            }
        }
        private void Attributes_OnRemove(object sender, EventArgs e)
        {
            GEMObjectAttribute selectedItem;

            selectedItem = dgrAttributes.SelectedItem as GEMObjectAttribute;

            if (selectedItem != null)
            {
                this._gemObjectAttributeItems.Remove(selectedItem);
            }
        }
        #endregion
        #endregion
        #region UpDown Event
        #region Attribute Up/Down
        private void ctlAttrUpDown_OnFirst(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedIndex > 0)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, 0);
            }
        }
        private void ctlAttrUpDown_OnUp(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedIndex > 0)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, selectedIndex - 1);
            }
        }
        private void ctlAttrUpDown_OnDown(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < this._gemObjectAttributeItems.Count - 1)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, selectedIndex + 1);
            }
        }
        private void ctlAttrUpDown_OnLast(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrAttributes.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < this._gemObjectAttributeItems.Count - 1)
            {
                this._gemObjectAttributeItems.Move(selectedIndex, this._gemObjectAttributeItems.Count - 1);
            }
        }
        #endregion
        #endregion
        #region DataGrid Event
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
            GEMObjectCreatedListWindow window;
            GEMObjectAttribute attr;

            attr = dgrAttributes.SelectedItem as GEMObjectAttribute;

            if (attr != null && attr.Format == SECSItemFormat.L)
            {
                this._attributeStack.Add(attr);

                window = new GEMObjectCreatedListWindow();
                window.Owner = this;
                window.Initialize(this._messageProcessor, this._gemObject, this._attributeStack);
                window.ShowDialog();

                this._attributeStack.Remove(attr);
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