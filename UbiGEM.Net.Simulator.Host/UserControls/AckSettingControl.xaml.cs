using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AckSettingControl : UserControl
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private ObservableCollection<Info.AckInfo> _displayInfo;
        #endregion
        #region Constructor
        public AckSettingControl()
        {
            InitializeComponent();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            parent = this.Parent;

            if (parent != null)
            {
                while (LogicalTreeHelper.GetParent(parent) != null && parent is Window == false)
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }
            }

            InitializeCurrentAck();
        }
        #endregion

        // DataGrid CheckBox Event
        #region chkAll_Click
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox;
            bool isUse;

            if (_messageProcessor != null && e.Source != null)
            {
                checkBox = e.Source as CheckBox;

                if (checkBox != null)
                {
                    isUse = checkBox.IsChecked.Value;

                    foreach (Info.AckInfo ackItem in this._messageProcessor.CurrentSetting.AckCollection.Items)
                    {
                        ackItem.Use = isUse;
                    }
                }
            }
        }
        #endregion
        #region dgcValue_LostFocus
        private void dgcValue_LostFocus(object sender, RoutedEventArgs e)
        {
            Info.AckInfo ackInfo;
            TextBox textBox;

            ackInfo = dgrACK.SelectedItem as Info.AckInfo;

            if (ackInfo != null && e.Source != null)
            {
                textBox = e.Source as TextBox;

                if (textBox != null)
                {
                    if (byte.TryParse(textBox.Text, out _) == true)
                    {
                        this._messageProcessor.IsDirty = true;
                    }
                    else
                    {
                        textBox.Text = ackInfo.Value.ToString();
                    }
                }
            }
        }
        #endregion

        // Public Method
        #region InitializeCurrentAck
        public void InitializeCurrentAck()
        {
            if (this._messageProcessor != null)
            {
                this._displayInfo = new ObservableCollection<Info.AckInfo>(this._messageProcessor.CurrentSetting.AckCollection.Items);
                dgrACK.ItemsSource = this._displayInfo;
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion
    }
}
