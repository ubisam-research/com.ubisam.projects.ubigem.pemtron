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
    public partial class SettingObjectSelectorForS14F5 : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public SettingObjectSelectorForS14F5()
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

                selectedObjectSpecifier = this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F5;

                if (string.IsNullOrEmpty(selectedObjectSpecifier) == false)
                {
                    cboObjectSpecifier.SelectedItem = selectedObjectSpecifier;
                }
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

            if (selectedObjectSpecifier != null)
            {
                this._messageProcessor.CurrentSetting.SelectedObjectSpecifierForS14F5 = selectedObjectSpecifier;
            }
        }
        #endregion
    }
}