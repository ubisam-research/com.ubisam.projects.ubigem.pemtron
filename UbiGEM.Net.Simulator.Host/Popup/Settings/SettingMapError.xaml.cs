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
    public partial class SettingMapError : Window
    {
        #region Wrapper
        class Wrapper
        {
            #region Property
            public byte MAPER { get; set; }
            public string Desc { get; set; }
            #endregion
            #region Contructor
            public Wrapper()
            {
                this.MAPER = 0;
                this.Desc = string.Empty;
            }
            #endregion
        }
        #endregion
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public SettingMapError()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Wrapper> mapErrorItems;

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
                mapErrorItems = new List<Wrapper>();
                mapErrorItems.Add(new Wrapper() { MAPER = 0, Desc = "ID not found" });
                mapErrorItems.Add(new Wrapper() { MAPER = 1, Desc = "Invalid Data" });
                mapErrorItems.Add(new Wrapper() { MAPER = 2, Desc = "Format Error" });

                cboMapEr.ItemsSource = mapErrorItems;

                cboMapEr.SelectedItem = mapErrorItems.FirstOrDefault(t => t.MAPER == this._messageProcessor.CurrentSetting.SelectedMapError);
                txtDATLC.Text = this._messageProcessor.CurrentSetting.DATLC.ToString();
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
            grbTitle.Header = "Map Error";
        }
        #endregion
        #region [UI Event]
        private void cboMapEr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Wrapper selectedItem;

            selectedItem = cboMapEr.SelectedItem as Wrapper;

            if (selectedItem != null)
            {
                this._messageProcessor.CurrentSetting.SelectedMapError = selectedItem.MAPER;

                this._messageProcessor.IsDirty = true;
            }
        }
        private void txtDATLC_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte converted;

            if (byte.TryParse(txtDATLC.Text, out converted) == true)
            {
                this._messageProcessor.CurrentSetting.DATLC = converted;
                this._messageProcessor.IsDirty = true;
            }
            else
            {
                txtDATLC.Text = this._messageProcessor.CurrentSetting.DATLC.ToString();
            }   
        }
        #endregion
    }
}