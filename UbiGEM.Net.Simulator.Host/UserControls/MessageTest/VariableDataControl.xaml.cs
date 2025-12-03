using System.Collections.Generic;
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
    public partial class VariableDataControl : UserControl
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
        public VariableDataControl()
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

                if (parent is Window == true)
                {
                    this._parentWindow = parent as Window;
                }
            }
        }
        #endregion

        // GroupBox Mouse Event
        #region GroupBox_MouseUp
        private void GroupBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            expander.IsExpanded = !expander.IsExpanded;
        }
        #endregion

        // Button Event
        #region btnS1F3_Click
        private void btnS1F3_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (this._messageProcessor.DataDictionaryCollection[Info.DataDictinaryList.SVID.ToString()] == null)
                {
                    MessageBox.Show(string.Format("Send S1F3 : Data Dictionary is null({0})", Info.DataDictinaryList.SVID.ToString()));
                }
                else
                {
                    driverResult = this._messageProcessor.SendS1F3();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S1F3 : Result={0}", driverResult));
                }
            }
            else
            {
                MessageBox.Show("Send S1F3 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS1F3Edit_Click
        private void btnS1F3Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingVariableWindow settingVariableWindow;

            settingVariableWindow = new Popup.SettingVariableWindow();
            settingVariableWindow.Initialize(this._messageProcessor, Popup.SettingVariableWindow.VariableSelectType.S1F3, this._messageProcessor.CurrentSetting.S1F3SelectedEquipmentStatus);

            if (this._parentWindow != null)
            {
                settingVariableWindow.Owner = this._parentWindow;
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingVariableWindow.OnVariableSelected += settingVariableWindow_OnVariableSelected;

            settingVariableWindow.ShowDialog();

            settingVariableWindow.OnVariableSelected -= settingVariableWindow_OnVariableSelected;

            settingVariableWindow = null;
        }
        #endregion
        #region btnS1F11_Click
        private void btnS1F11_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (this._messageProcessor.DataDictionaryCollection[Info.DataDictinaryList.SVID.ToString()] == null)
                {
                    MessageBox.Show(string.Format("Send S1F11 : Data Dictionary is null({0})", Info.DataDictinaryList.SVID.ToString()));
                }
                else
                {
                    driverResult = this._messageProcessor.SendS1F11();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S1F11 : Result={0}", driverResult));
                }
            }
            else
            {
                MessageBox.Show("Send S1F11 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS1F11Edit_Click
        private void btnS1F11Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingVariableWindow settingVariableWindow;

            settingVariableWindow = new Popup.SettingVariableWindow();
            settingVariableWindow.Initialize(this._messageProcessor, Popup.SettingVariableWindow.VariableSelectType.S1F11, this._messageProcessor.CurrentSetting.S1F11StatusVariableNamelist);

            if (this._parentWindow != null)
            {
                settingVariableWindow.Owner = this._parentWindow;
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingVariableWindow.OnVariableSelected += settingVariableWindow_OnVariableSelected;

            settingVariableWindow.ShowDialog();

            settingVariableWindow.OnVariableSelected -= settingVariableWindow_OnVariableSelected;

            settingVariableWindow = null;
        }
        #endregion
        #region btnS1F21_Click
        private void btnS1F21_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (this._messageProcessor.DataDictionaryCollection[Info.DataDictinaryList.SVID.ToString()] == null)
                {
                    MessageBox.Show(string.Format("Send S1F21 : Data Dictionary is null({0})", Info.DataDictinaryList.SVID.ToString()));
                }
                else
                {
                    driverResult = this._messageProcessor.SendS1F21();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S1F21 : Result={0}", driverResult));
                }
            }
            else
            {
                MessageBox.Show("Send S1F21 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS1F21Edit_Click
        private void btnS1F21Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingVariableWindow settingVariableWindow;

            settingVariableWindow = new Popup.SettingVariableWindow();
            settingVariableWindow.Initialize(this._messageProcessor, Popup.SettingVariableWindow.VariableSelectType.S1F21, this._messageProcessor.CurrentSetting.S1F21DataVariableNamelist);

            if (this._parentWindow != null)
            {
                settingVariableWindow.Owner = this._parentWindow;
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingVariableWindow.OnVariableSelected += settingVariableWindow_OnVariableSelected;

            settingVariableWindow.ShowDialog();

            settingVariableWindow.OnVariableSelected -= settingVariableWindow_OnVariableSelected;

            settingVariableWindow = null;
        }
        #endregion
        #region btnS2F13_Click
        private void btnS2F13_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (this._messageProcessor.DataDictionaryCollection[Info.DataDictinaryList.ECID.ToString()] == null)
                {
                    MessageBox.Show(string.Format("Send S2F13 : Data Dictionary is null({0})", Info.DataDictinaryList.SVID.ToString()));
                }
                else
                {
                    driverResult = this._messageProcessor.SendS2F13();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S2F13 : Result={0}", driverResult));
                }
            }
            else
            {
                MessageBox.Show("Send S2F13 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F13Edit_Click
        private void btnS2F13Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingVariableWindow settingVariableWindow;

            settingVariableWindow = new Popup.SettingVariableWindow();
            settingVariableWindow.Initialize(this._messageProcessor, Popup.SettingVariableWindow.VariableSelectType.S2F13, this._messageProcessor.CurrentSetting.S2F13EquipmentConstant);
            
            if (this._parentWindow != null)
            {
                settingVariableWindow.Owner = this._parentWindow;
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingVariableWindow.OnVariableSelected += settingVariableWindow_OnVariableSelected;

            settingVariableWindow.ShowDialog();

            settingVariableWindow.OnVariableSelected -= settingVariableWindow_OnVariableSelected;

            settingVariableWindow = null;
        }
        #endregion
        #region btnS2F15_Click
        private void btnS2F15_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (this._messageProcessor.DataDictionaryCollection[Info.DataDictinaryList.SVID.ToString()] == null)
                {
                    MessageBox.Show(string.Format("Send S2F15 : Data Dictionary is null({0})", Info.DataDictinaryList.SVID.ToString()));
                }
                else
                {
                    driverResult = this._messageProcessor.SendS2F15();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S2F15 : Result={0}", driverResult));
                }
            }
            else
            {
                MessageBox.Show("Send S2F15 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F15Edit_Click
        private void btnS2F15Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingVariableWindow settingVariableWindow;

            settingVariableWindow = new Popup.SettingVariableWindow();
            settingVariableWindow.Initialize(this._messageProcessor, Popup.SettingVariableWindow.VariableSelectType.S2F15, this._messageProcessor.CurrentSetting.S2F15NewEquipmentConstant);

            if (this._parentWindow != null)
            {
                settingVariableWindow.Owner = this._parentWindow;
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingVariableWindow.OnVariableSelectedWithValue += settingVariableWindow_OnVariableSelectedWithValue;

            settingVariableWindow.ShowDialog();

            settingVariableWindow.OnVariableSelectedWithValue -= settingVariableWindow_OnVariableSelectedWithValue;

            settingVariableWindow = null;
        }
        #endregion
        #region btnS2F29_Click
        private void btnS2F29_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                if (this._messageProcessor.DataDictionaryCollection[Info.DataDictinaryList.ECID.ToString()] == null)
                {
                    MessageBox.Show(string.Format("Send S2F29 : Data Dictionary is null({0})", Info.DataDictinaryList.SVID.ToString()));
                }
                else
                {
                    driverResult = this._messageProcessor.SendS2F29();

                    RaiseWriteLog(MessageProcessor.DriverLogType.SEND, string.Format("Send S2F29 : Result={0}", driverResult));
                }
            }
            else
            {
                MessageBox.Show("Send S2F29 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F29Edit_Click
        private void btnS2F29Edit_Click(object sender, RoutedEventArgs e)
        {
            Popup.SettingVariableWindow settingVariableWindow;

            settingVariableWindow = new Popup.SettingVariableWindow();
            settingVariableWindow.Initialize(this._messageProcessor, Popup.SettingVariableWindow.VariableSelectType.S2F29, this._messageProcessor.CurrentSetting.S2F29EquipmentConstantNamelist);

            if (this._parentWindow != null)
            {
                settingVariableWindow.Owner = this._parentWindow;
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                settingVariableWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            settingVariableWindow.OnVariableSelected += settingVariableWindow_OnVariableSelected;

            settingVariableWindow.ShowDialog();

            settingVariableWindow.OnVariableSelected -= settingVariableWindow_OnVariableSelected;

            settingVariableWindow = null;
        }
        #endregion

        // Popup Window Event
        #region settingVariableWindow_OnVariableSelected
        private void settingVariableWindow_OnVariableSelected(Popup.SettingVariableWindow.VariableSelectType selectType, List<string> selectedItems)
        {
            switch (selectType)
            {
                case Popup.SettingVariableWindow.VariableSelectType.S1F3:
                    this._messageProcessor.CurrentSetting.S1F3SelectedEquipmentStatus = selectedItems;
                    break;
                case Popup.SettingVariableWindow.VariableSelectType.S1F11:
                    this._messageProcessor.CurrentSetting.S1F11StatusVariableNamelist = selectedItems;
                    break;
                case Popup.SettingVariableWindow.VariableSelectType.S2F13:
                    this._messageProcessor.CurrentSetting.S2F13EquipmentConstant = selectedItems;
                    break;
                case Popup.SettingVariableWindow.VariableSelectType.S1F21:
                    this._messageProcessor.CurrentSetting.S1F21DataVariableNamelist = selectedItems;
                    break;
                case Popup.SettingVariableWindow.VariableSelectType.S2F29:
                    this._messageProcessor.CurrentSetting.S2F29EquipmentConstantNamelist = selectedItems;
                    break;
                case Popup.SettingVariableWindow.VariableSelectType.S2F47:
                    this._messageProcessor.CurrentSetting.S2F47VariableLimitAttributeRequest = selectedItems;
                    break;
            }

            this._messageProcessor.IsDirty = true;
        }
        #endregion
        #region settingVariableWindow_OnVariableSelectedWithValue
        private void settingVariableWindow_OnVariableSelectedWithValue(Popup.SettingVariableWindow.VariableSelectType selectType, Dictionary<string, dynamic> selectedItems)
        {
            this._messageProcessor.CurrentSetting.S2F15NewEquipmentConstant = selectedItems.Keys.ToList();

            this._messageProcessor.IsDirty = true;
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