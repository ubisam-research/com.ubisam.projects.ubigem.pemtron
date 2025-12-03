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
    public partial class RemoteCommandControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);
        public WriteLog OnWriteLog { get; set; }
        #endregion

        #region MemberVariable
        private ObservableCollection<ExpandedRemoteCommandInfo> _displayInfo;
        private MessageProcessor _messageProcessor;

        private Window _parentWindow;
        #endregion

        #region Constructor
        public RemoteCommandControl()
        {
            InitializeComponent();

            this._displayInfo = new ObservableCollection<ExpandedRemoteCommandInfo>();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            InitializeRemoteCommand();

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
            ExpandedRemoteCommandInfo selectedInfo;
            ExpandedRemoteCommandValueSetInfo selectedValueSet;

            if (e.Source != null)
            {
                combobox = e.Source as ComboBox;
                selectedInfo = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;

                if (combobox != null && combobox.SelectedItem != null && selectedInfo != null)
                {
                    selectedValueSet = combobox.SelectedItem as ExpandedRemoteCommandValueSetInfo;

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
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            string errorText;

            expandedRemoteCommandInfo = dgrRemoteCommand.SelectedItem as ExpandedRemoteCommandInfo;
            errorText = string.Empty;

            if (expandedRemoteCommandInfo != null)
            {
                triggerWindow = new Popup.Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Remote Command", expandedRemoteCommandInfo.RemoteCommand, expandedRemoteCommandInfo.TriggerCollection);
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
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            Popup.ValueEdit.RemoteCommandValueEditWindow editWindow;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedRemoteCommandInfo)
                {
                    expandedRemoteCommandInfo = btn.DataContext as ExpandedRemoteCommandInfo;

                    editWindow = new Popup.ValueEdit.RemoteCommandValueEditWindow();
                    editWindow.Initialize(this._messageProcessor, expandedRemoteCommandInfo);

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
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            MessageError error;

            if (sender is Button == true)
            {
                btn = sender as Button;

                if (btn.DataContext is ExpandedRemoteCommandInfo)
                {
                    expandedRemoteCommandInfo = btn.DataContext as ExpandedRemoteCommandInfo;

                    if (this._messageProcessor.HSMSDriverConnected == true)
                    {
                        if (expandedRemoteCommandInfo.SelectedValueSet != null)
                        {
                            error = this._messageProcessor.SendS2F41(expandedRemoteCommandInfo, expandedRemoteCommandInfo.SelectedValueSet);

                            RaiseWriteLog(MessageProcessor.DriverLogType.INFO, string.Format("Send S2F41: Remote Command {0}: Result: {1}", expandedRemoteCommandInfo.RemoteCommand, error));
                        }
                        else
                        {
                            RaiseWriteLog(MessageProcessor.DriverLogType.INFO, string.Format("Send S2F41 Failed: Value Set is not selected"));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Send S2F41 Failed. Disconnected");
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
        #region InitializeRemoteCommand
        public void InitializeRemoteCommand()
        {
            ExpandedRemoteCommandInfo expandedRemoteCommandInfo;
            Dictionary<string, string> selectedValueSets;

            selectedValueSets = new Dictionary<string, string>();

            if (this._displayInfo == null)
            {
                this._displayInfo = new ObservableCollection<ExpandedRemoteCommandInfo>();
            }

            foreach (ExpandedRemoteCommandInfo cmdInfo in this._displayInfo)
            {
                if (cmdInfo.SelectedValueSet == null)
                {
                    selectedValueSets[cmdInfo.RemoteCommand] = "Default";
                }
                else
                {
                    selectedValueSets[cmdInfo.RemoteCommand] = cmdInfo.SelectedValueSet.Name;
                }
            }

            this._displayInfo.Clear();

            if (this._messageProcessor != null)
            {
                foreach (var item in this._messageProcessor.RemoteCommandCollection.RemoteCommandItems)
                {
                    expandedRemoteCommandInfo = item as ExpandedRemoteCommandInfo;

                    if (expandedRemoteCommandInfo != null)
                    {
                        if (selectedValueSets.ContainsKey(expandedRemoteCommandInfo.RemoteCommand))
                        {
                            expandedRemoteCommandInfo.SelectedValueSet = expandedRemoteCommandInfo.ValueSetCollection[selectedValueSets[expandedRemoteCommandInfo.RemoteCommand]];
                        }

                        if (expandedRemoteCommandInfo.SelectedValueSet == null)
                        {
                            expandedRemoteCommandInfo.SelectedValueSet = expandedRemoteCommandInfo.ValueSetCollection["Default"];
                        }

                        this._displayInfo.Add(expandedRemoteCommandInfo);
                    }
                }
            }

            dgrRemoteCommand.ItemsSource = this._displayInfo;
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
