using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Simulator.Host.Popup;

namespace UbiGEM.Net.Simulator.Host.UserControls.MessageTest
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GEMObjectControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);

        public WriteLog OnWriteLog { get; set; }
        #endregion

        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private Window _parentWindow;
        #endregion

        // GroupBox Mouse Event
        #region GroupBox_MouseUp
        private void GroupBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            expander.IsExpanded = !expander.IsExpanded;
        }
        #endregion

        #region Constructor
        public GEMObjectControl()
        {
            InitializeComponent();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            InitializeGEMObject();

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

        // Button Event
        #region Button Event
        private void btnS14F1Send_Click(object sender, RoutedEventArgs e)
        {
            if (this._messageProcessor != null)
            {
                this._messageProcessor.SendS14F1();
            }
        }
        private void btnS14F1Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F1 dialog;

            dialog = new SettingObjectSelectorForS14F1();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }
        private void btnS14F3Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F3 dialog;

            dialog = new SettingObjectSelectorForS14F3();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }
        private void btnS14F3Send_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.SendS14F3();
        }
        private void btnS14F5Send_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F5 dialog;

            dialog = new SettingObjectSelectorForS14F5();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();

            this._messageProcessor.SendS14F5();
        }
        private void btnS14F7Send_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.SendS14F7();
        }
        private void btnS14F7Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F7 dialog;

            dialog = new SettingObjectSelectorForS14F7();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }
        private void btnS14F9Send_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.SendS14F9();
        }
        private void btnS14F9Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F9 dialog;

            dialog = new SettingObjectSelectorForS14F9();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }
        private void btnS14F11Send_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.SendS14F11();
        }
        private void btnS14F11Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F11 dialog;

            dialog = new SettingObjectSelectorForS14F11();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }
        private void btnS14F13Send_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.SendS14F13();
        }
        private void btnS14F13Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F13 dialog;

            dialog = new SettingObjectSelectorForS14F13();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }
        private void btnS14F15Send_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.SendS14F15();
        }
        private void btnS14F15Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F15 dialog;

            dialog = new SettingObjectSelectorForS14F15();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }

        private void btnS14F17Send_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.SendS14F17();
        }
        private void btnS14F17Edit_Click(object sender, RoutedEventArgs e)
        {
            SettingObjectSelectorForS14F17 dialog;

            dialog = new SettingObjectSelectorForS14F17();
            dialog.Owner = this._parentWindow;
            dialog.Initialize(this._messageProcessor);
            dialog.ShowDialog();
        }
        private void btnS14F19Send_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion
        #region InitializeGEMObject
        public void InitializeGEMObject()
        {
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
