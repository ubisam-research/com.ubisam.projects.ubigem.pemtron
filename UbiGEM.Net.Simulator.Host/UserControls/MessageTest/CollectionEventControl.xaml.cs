using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.UserControls.MessageTest
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CollectionEventControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);
        public WriteLog OnWriteLog { get; set; }
        #endregion

        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private Window _parentWindow;
        #endregion

        #region Constructor
        public CollectionEventControl()
        {
            InitializeComponent();
        }
        #endregion

        // GroupBox Mouse Event
        #region GroupBox_MouseUp
        private void GroupBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            expander.IsExpanded = !expander.IsExpanded;
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

                if (parent is Window == true)
                {
                    this._parentWindow = parent as Window;
                }
            }

            InitializeComboBoxCollectionEvent();
        }
        #endregion

        // ComboBox Event
        #region cboCollectionEvent_DropDownOpened
        private void cboCollectionEvent_DropDownOpened(object sender, EventArgs e)
        {
            InitializeComboBoxCollectionEvent();
        }
        #endregion

        // Button Event
        #region btnS1F23_Click
        private void btnS1F23_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (this._messageProcessor.DataDictionaryCollection[Info.DataDictinaryList.CEID.ToString()] == null)
                {
                    MessageBox.Show(string.Format("Send S1F21 : Data Dictionary is null({0})", Info.DataDictinaryList.CEID.ToString()));
                }
                else
                {
                    driverResult = this._messageProcessor.SendS1F23();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S1F23 : Result={0}", driverResult));
                }
            }
            else
            {
                MessageBox.Show("Send S1F23 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS1F23Edit_Click
        private void btnS1F23Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingCollectionEventWindow settingCollectionEventWindow;

            settingCollectionEventWindow = new Popup.SettingCollectionEventWindow();
            settingCollectionEventWindow.Initialize(this._messageProcessor, this._messageProcessor.CurrentSetting.S1F23CollectionEventList);

            if (this._parentWindow != null)
            {
                settingCollectionEventWindow.Owner = this._parentWindow;
                settingCollectionEventWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingCollectionEventWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingCollectionEventWindow.ShowDialog();
        }
        #endregion
        #region btnS6F15_Click
        private void btnS6F15_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;
            string collectionEventID;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                collectionEventID = cboCollectionEvent.Text.Trim();
                if (string.IsNullOrEmpty(collectionEventID) == false)
                {
                    driverResult = this._messageProcessor.SendS6F15(collectionEventID);

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S6F15 : Result={0}", driverResult));
                }
                else
                {
                    MessageBox.Show("Send S6F15 : Collection Event ID is empty");
                }
            }
            else
            {
                MessageBox.Show("Send S6F15 Failed - Disconnected");
            }
        }
        #endregion

        // Private Method
        #region InitializeComboBoxCollectionEvent
        private void InitializeComboBoxCollectionEvent()
        {
            if (this._messageProcessor != null)
            {
                cboCollectionEvent.ItemsSource = this._messageProcessor.CollectionEventCollection.Items.OrderBy(t => t.Key).Select(t => string.Format("{0} : {1}", t.Value.CEID, t.Value.Name));
            }
        }
        #endregion
        #region RaiseWriteLog
        private void RaiseWriteLog(MessageProcessor.DriverLogType logType, string logText)
        {
            if (this.OnWriteLog != null)
            {
                this.OnWriteLog.BeginInvoke(logType, logText, null, null);
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