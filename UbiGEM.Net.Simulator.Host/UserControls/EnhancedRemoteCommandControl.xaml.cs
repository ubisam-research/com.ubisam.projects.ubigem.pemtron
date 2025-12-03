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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EnhancedRemoteCommandControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);
        public WriteLog OnWriteLog { get; set; }
        #endregion
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private Window _parentWindow;
        private ObservableCollection<ExpandedEnhancedRemoteCommandInfo> _displayInfo;
        #endregion
        #region Constructor
        public EnhancedRemoteCommandControl()
        {
            InitializeComponent();

            this._displayInfo = new ObservableCollection<ExpandedEnhancedRemoteCommandInfo>();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            InitializeEnhancedRemoteCommand();

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

        // DataGrid Event
        #region dgcValueSet_SelectionChanged
        private void dgcValueSet_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox combobox;
            ExpandedEnhancedRemoteCommandInfo selectedInfo;
            ExpandedEnhancedRemoteCommandValueSetInfo selectedValueSet;

            if (e.Source != null)
            {
                combobox = e.Source as ComboBox;
                selectedInfo = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;

                if (combobox != null && combobox.SelectedItem != null && selectedInfo != null)
                {
                    selectedValueSet = combobox.SelectedItem as ExpandedEnhancedRemoteCommandValueSetInfo;

                    if (selectedValueSet != null)
                    {
                        selectedInfo.SelectedValueSet = selectedValueSet;
                    }
                }
            }
        }
        #endregion

        // Button Event
        #region btnTriggerEdit_Click
        private void btnTriggerEdit_Click(object sender, RoutedEventArgs args)
        {
            Popup.Settings.TriggerWindow triggerWindow;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            expandedEnhancedRemoteCommandInfo = dgrEnhancedRemoteCommand.SelectedItem as ExpandedEnhancedRemoteCommandInfo;
            if (expandedEnhancedRemoteCommandInfo != null)
            {
                triggerWindow = new Popup.Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Enhanced Remote Command", expandedEnhancedRemoteCommandInfo.RemoteCommand, expandedEnhancedRemoteCommandInfo.TriggerCollection);
                triggerWindow.Owner = this._parentWindow;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
            }
        }
        #endregion
        #region btnRemoteCommandEdit_Click
        private void btnRemoteCommandEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            Popup.ValueEdit.EnhancedRemoteCommandValueEditWindow editWindow;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedEnhancedRemoteCommandInfo)
                {
                    expandedEnhancedRemoteCommandInfo = btn.DataContext as ExpandedEnhancedRemoteCommandInfo;

                    editWindow = new Popup.ValueEdit.EnhancedRemoteCommandValueEditWindow();
                    editWindow.Initialize(this._messageProcessor, expandedEnhancedRemoteCommandInfo);

                    if (this._parentWindow != null)
                    {
                        editWindow.Owner = this._parentWindow;
                        editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    }
                    else
                    {
                        editWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }

                    if (editWindow.ShowDialog() == true)
                    {
                        this._messageProcessor.IsDirty = true;
                    }
                }
            }
        }
        #endregion
        #region btnRemoteCommandSend_Click
        private void btnRemoteCommandSend_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;

            MessageError error;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedEnhancedRemoteCommandInfo)
                {
                    expandedEnhancedRemoteCommandInfo = btn.DataContext as ExpandedEnhancedRemoteCommandInfo;

                    if (this._messageProcessor.HSMSDriverConnected == true)
                    {
                        if (expandedEnhancedRemoteCommandInfo.SelectedValueSet != null)
                        {
                            error = this._messageProcessor.SendS2F49(expandedEnhancedRemoteCommandInfo, expandedEnhancedRemoteCommandInfo.SelectedValueSet);

                            RaiseWriteLog(MessageProcessor.DriverLogType.INFO, string.Format("Send S2F49: Enhanced Remote Command {0}: Result: {1}", expandedEnhancedRemoteCommandInfo.RemoteCommand, error));
                        }
                        else
                        {
                            RaiseWriteLog(MessageProcessor.DriverLogType.INFO, string.Format("Send S2F49 Failed: Value Set is not selected"));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Send S2F49 Failed. Disconnected");
                    }
                }
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
        #region InitializeEnhancedRemoteCommand
        public void InitializeEnhancedRemoteCommand()
        {
            ExpandedEnhancedRemoteCommandInfo expandedEnhancedRemoteCommandInfo;
            Dictionary<string, string> selectedValueSetNames;

            selectedValueSetNames = new Dictionary<string, string>();

            if (this._displayInfo == null)
            {
                this._displayInfo = new ObservableCollection<ExpandedEnhancedRemoteCommandInfo>();
            }

            foreach (ExpandedEnhancedRemoteCommandInfo cmdInfo in this._displayInfo)
            {
                if (cmdInfo.SelectedValueSet == null)
                {
                    selectedValueSetNames[cmdInfo.RemoteCommand] = "Default";
                }
                else
                {
                    selectedValueSetNames[cmdInfo.RemoteCommand] = cmdInfo.SelectedValueSet.Name;
                }
                
            }

            this._displayInfo.Clear();

            if (this._messageProcessor != null)
            {
                foreach (var item in this._messageProcessor.RemoteCommandCollection.EnhancedRemoteCommandItems)
                {
                    expandedEnhancedRemoteCommandInfo = item as ExpandedEnhancedRemoteCommandInfo;

                    if (expandedEnhancedRemoteCommandInfo != null)
                    {
                        if (selectedValueSetNames.ContainsKey(expandedEnhancedRemoteCommandInfo.RemoteCommand))
                        {
                            expandedEnhancedRemoteCommandInfo.SelectedValueSet = expandedEnhancedRemoteCommandInfo.ValueSetCollection[selectedValueSetNames[expandedEnhancedRemoteCommandInfo.RemoteCommand]];
                        }

                        if (expandedEnhancedRemoteCommandInfo.SelectedValueSet == null)
                        {
                            expandedEnhancedRemoteCommandInfo.SelectedValueSet = expandedEnhancedRemoteCommandInfo.ValueSetCollection["Default"];
                        }

                        this._displayInfo.Add(expandedEnhancedRemoteCommandInfo);
                    }
                }
            }

            dgrEnhancedRemoteCommand.ItemsSource = this._displayInfo;
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
    }
}