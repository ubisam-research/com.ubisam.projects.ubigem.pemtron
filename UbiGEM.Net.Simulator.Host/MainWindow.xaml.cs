using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Structure;

namespace UbiGEM.Net.Simulator.Host
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region enum
        private enum SelectedFrame
        {
            Main,
            MessageTest,
            UserMessage
        }
        #endregion

        #region MemberVariables
        private readonly MessageProcessor _messageProcessor;
        private readonly SystemSetting _systemSetting;
        private SelectedFrame _selectedLeftFrame;
        private Popup.AckAndReplyWindow _ackAndReplyWindow;
        private bool _isLogAutoScroll;
        private bool _useSECS2Log;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            this._systemSetting = new SystemSetting();
            this._messageProcessor = new MessageProcessor();

            this._messageProcessor.Configuration.DeviceType = DeviceType.Host;
            this._messageProcessor.Configuration.UMDFileName = "[{StandardMessageSet}]";

            this._messageProcessor.ConfigFilepath = string.Empty;
            this._messageProcessor.UGCFilepath = string.Empty;

            this._messageProcessor.OnDriverConnected += SchedulerDriverConnected;
            this._messageProcessor.OnDriverDisconnected += SchedulerDriverDisconnected;
            this._messageProcessor.OnDriverSelected += SchedulerDriverSelected;
            this._messageProcessor.OnDriverDeselected += SchedulerDriverDeselected;

            this._messageProcessor.OnDriverLogAdded1 += SchedulerLogAdded1;
            this._messageProcessor.OnDriverLogAdded2 += SchedulerLogAdded2;
            this._messageProcessor.OnSECS2LogAdded += SchedulerSECS2LogAdded;

            this._messageProcessor.OnCommunicationStateChanged += SchedulerCommunicationStateChanged;
            this._messageProcessor.OnControlStateChanged += SchedulerControlStateChanged;

            this._selectedLeftFrame = SelectedFrame.Main;

            this.ctlMessageTest.OnWriteLog += ctl_WriteLog;

            ctlRCMD.OnWriteLog += ctl_WriteLog;
            ctlERCMD.OnWriteLog += ctl_WriteLog;
            ctlTraceData.OnWriteLog += ctl_WriteLog;
            ctlLimitMonitoring.OnWriteLog += ctl_WriteLog;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            this._isLogAutoScroll = true;
            this._useSECS2Log = true;
        }
        #endregion

        #region CurrentDomain_UnhandledException
        private const string DIR = @"UbiSam\UbiGEM\ShutDown_log";
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string logText;
            string filepath;
            string dirPath;
            Exception ex;
            string myDocumentPath;

            try
            {
                ex = e.ExceptionObject as Exception;
                logText = $"CurrentDomain_UnhandledException:Exception={ex}\r\n{ex.StackTrace}";
                myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dirPath = $@"{myDocumentPath}\{DIR}";

                if (System.IO.Directory.Exists(dirPath) == false)
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }

                filepath = $@"{dirPath}\{DateTime.Now:yyyyMMddHHmmss}.log";
                System.IO.File.WriteAllText(filepath, logText);
            }
            catch
            {
            }
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string lastEditedFilepath;
            GemDriverError result;
            string message;

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

            this._messageProcessor.Initialize();

            // Set MessageProcessor to user controls
            this.ctlMessageTest.Initialize(this._messageProcessor);
            this.ctlUserMessage.Initialize(this._messageProcessor);

            this.ctlRCMD.Initialize(this._messageProcessor);
            this.ctlERCMD.Initialize(this._messageProcessor);
            this.ctlTraceData.Initialize(this._messageProcessor);
            this.ctlLimitMonitoring.Initialize(this._messageProcessor);

            // Load Last Edited File
            #region Load Last Edited File
            lastEditedFilepath = this._systemSetting.LastEditedFilepath;

            if (string.IsNullOrEmpty(lastEditedFilepath) == false && System.IO.File.Exists(lastEditedFilepath) == true)
            {
                result = this._messageProcessor.LoadConfigFile(lastEditedFilepath, out string errorText);

                if (result == GemDriverError.Ok)
                {
                    this.Title = "GEM HOST Simulator - " + lastEditedFilepath;
                    this._messageProcessor.ConfigFilepath = lastEditedFilepath;

                    ctlRCMD.InitializeRemoteCommand();
                    ctlERCMD.InitializeEnhancedRemoteCommand();
                    ctlTraceData.InitializeTraceData();
                    ctlLimitMonitoring.InitializeLimitMonitoring();
                    ctlUserMessage.InitializeUserMessages();
                    _ackAndReplyWindow?.InitializeData();
                }
                else
                {
                    this._systemSetting.LastEditedFilepath = string.Empty;
                    this._systemSetting.SaveSystemSetting();

                    message = $" last edited file: {lastEditedFilepath}\n\n Load Fail\n\n reason: {errorText}";
                    MessageBox.Show(message);
                    
                    ctlRCMD.InitializeRemoteCommand();
                    ctlERCMD.InitializeEnhancedRemoteCommand();
                    ctlTraceData.InitializeTraceData();
                    ctlLimitMonitoring.InitializeLimitMonitoring();
                    ctlUserMessage.InitializeUserMessages();
                    _ackAndReplyWindow?.InitializeData();
                }
            }
            else
            {
                this._systemSetting.LastEditedFilepath = string.Empty;
                this._systemSetting.SaveSystemSetting();

                this._messageProcessor.NewProject();

                ctlRCMD.InitializeRemoteCommand();
                ctlERCMD.InitializeEnhancedRemoteCommand();
                ctlTraceData.InitializeTraceData();
                ctlLimitMonitoring.InitializeLimitMonitoring();
                ctlUserMessage.InitializeUserMessages();
                _ackAndReplyWindow?.InitializeData();
            }
            #endregion

            txtLog.Document.PageWidth = 1000;
        }
        #endregion
        #region Window_Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            this._messageProcessor.StopScheduler();

            Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        #endregion
        #region Window_Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult confirmResult;
            string confirmMessage;

            if (this._messageProcessor.IsDirty == true)
            {
                confirmMessage = " Do you want to exit?\n\n Modified data will be lost.";
                confirmResult = MessageBox.Show(confirmMessage, "Warning", MessageBoxButton.YesNo);

                if (confirmResult != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                confirmMessage = " Do you want to exit?";
                confirmResult = MessageBox.Show(confirmMessage, "Warning", MessageBoxButton.YesNo);

                if (confirmResult != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

        // Driver Event
        #region SchedulerDriverConnected
        private void SchedulerDriverConnected(object sender, string ipAddress, int portNo)
        {
            WriteLog(MessageProcessor.DriverLogType.INFO, $"Connected - IP={ipAddress}, Port={portNo}");
        }
        #endregion
        #region SchedulerDriverDisconnected
        private void SchedulerDriverDisconnected(object sender, string ipAddress, int portNo)
        {
            WriteLog(MessageProcessor.DriverLogType.INFO, $"Disconnected - IP={ipAddress}, Port={portNo}");
        }
        #endregion
        #region SchedulerDriverSelected
        private void SchedulerDriverSelected(object sender, string ipAddress, int portNo)
        {
            WriteLog(MessageProcessor.DriverLogType.INFO, $"Selected - IP={ipAddress}, Port={portNo}");
        }
        #endregion
        #region SchedulerDriverDeselected
        private void SchedulerDriverDeselected(object sender, string ipAddress, int portNo)
        {
            WriteLog(MessageProcessor.DriverLogType.INFO, $"Deselected - IP={ipAddress}, Port={portNo}");
        }
        #endregion
        #region SchedulerLogAdded1
        private void SchedulerLogAdded1(object sender, MessageProcessor.DriverLogType logType, string logText)
        {
            if (logText.EndsWith("\r\n") == false || logText.EndsWith("\n") == false)
            {
                WriteLog(logType, $"{logText}\r\n");
            }
            else
            {
                WriteLog(logType, logText);
            }
        }
        #endregion
        #region SchedulerLogAdded2
        private void SchedulerLogAdded2(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            if (logText.EndsWith("\r\n") == false || logText.EndsWith("\n") == false)
            {
                WriteLog(logLevel, $"{logText}\r\n");
            }
            else
            {
                WriteLog(logLevel, logText);
            }
        }
        #endregion
        #region SchedulerSECS2LogAdded
        private void SchedulerSECS2LogAdded(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            if (logText.EndsWith("\r\n") == false || logText.EndsWith("\n") == false)
            {
                WriteLog(logLevel, $"{logText}\r\n");
            }
            else
            {
                WriteLog(logLevel, logText);
            }
        }
        #endregion
        #region SchedulerControlStateChanged
        private void SchedulerControlStateChanged(ControlState controlState)
        {
            WriteLog(MessageProcessor.DriverLogType.INFO, $"Control State Changed: {controlState}");

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {
                if (controlState.GetHashCode() == 0)
                {
                    this.lblControlState.Content = ControlState.EquipmentOffline.ToString();
                }
                else
                {
                    this.lblControlState.Content = controlState.ToString();
                }

                switch (controlState)
                {
                    case ControlState.EquipmentOffline:
                    case ControlState.HostOffline:
                        this.recControlState.Fill = Brushes.DarkRed;
                        lblControlState.Foreground = Brushes.White;
                        break;
                    case ControlState.OnlineLocal:
                        this.recControlState.Fill = Brushes.Yellow;
                        lblControlState.Foreground = Brushes.Black;
                        break;
                    case ControlState.OnlineRemote:
                        this.recControlState.Fill = Brushes.DarkGreen;
                        lblControlState.Foreground = Brushes.White;
                        break;
                    default:
                        this.recControlState.Fill = Brushes.DarkRed;
                        lblControlState.Foreground = Brushes.White;
                        break;
                }
            }));
        }
        #endregion
        #region SchedulerCommunicationStateChanged
        private void SchedulerCommunicationStateChanged(CommunicationState communicationState)
        {
            WriteLog(MessageProcessor.DriverLogType.INFO, $"Communication State Changed: {communicationState}");

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {
                this.lblCommunicationState.Content = communicationState.ToString();

                switch (communicationState)
                {
                    case CommunicationState.Communicating:
                        this.recCommunicationState.Fill = Brushes.DarkGreen;
                        break;
                    case CommunicationState.WaitCRFromHost:
                    case CommunicationState.WaitCRA:
                        this.recCommunicationState.Fill = Brushes.Yellow;
                        break;
                    default:
                        this.recCommunicationState.Fill = Brushes.DarkRed;
                        break;
                }
            }));
        }
        #endregion

        // Popup Window Event
        #region settingTerminalWindow_OnTerminalMessageChanged
        private void settingTerminalWindow_OnTerminalMessageChanged(Popup.SettingTerminalWindow.TerminalMessageSelectType selectType, TerminalMessageInfo terminalMessageInfo)
        {
            this._messageProcessor.CurrentSetting.TerminalMessage = terminalMessageInfo;

            this._messageProcessor.IsDirty = true;
        }
        #endregion

        // UserControl Event
        #region ctl_WriteLog
        private void ctl_WriteLog(MessageProcessor.DriverLogType logType, string logText)
        {
            if (logText.EndsWith("\r\n") == false || logText.EndsWith("\n") == false)
            {
                WriteLog(logType, $"{logText}\r\n");
            }
            else
            {
                WriteLog(logType, logText);
            }
        }
        #endregion

        // Menu Event
        #region mnuNew_Click
        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            bool processContinue;
            MessageBoxResult confirmResult;
            string confirmMessage;

            processContinue = false;

            if (this._messageProcessor.IsDirty == true)
            {
                confirmMessage = " Do you want to New Configuration?\n\n Modified data will be lost";
                confirmResult = MessageBox.Show(confirmMessage, "Warning", MessageBoxButton.YesNo);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    processContinue = true;
                }
            }
            else
            {
                processContinue = true;
            }

            if (processContinue == true)
            {
                this._messageProcessor.NewProject();
                this._messageProcessor.ConfigFilepath = string.Empty;

                this._systemSetting.LastEditedFilepath = string.Empty;
                this._systemSetting.SaveSystemSetting();

                this.Title = "GEM Host Simulator";

                ctlRCMD.InitializeRemoteCommand();
                ctlERCMD.InitializeEnhancedRemoteCommand();
                ctlTraceData.InitializeTraceData();
                ctlLimitMonitoring.InitializeLimitMonitoring();
                ctlUserMessage.InitializeUserMessages();
                _ackAndReplyWindow?.InitializeData();
            }
        }
        #endregion
        #region mnuOpen_Click
        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openDialog;
            GemDriverError openResult;

            bool processContinue;
            MessageBoxResult confirmResult;
            string confirmMessage;

            processContinue = false;

            if (this._messageProcessor.IsDirty == true)
            {
                confirmMessage = " Do you want to open file?\n\n Modified data will be lost";
                confirmResult = MessageBox.Show(confirmMessage, "Warning", MessageBoxButton.YesNo);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    processContinue = true;
                }
            }
            else
            {
                processContinue = true;
            }

            if (processContinue == true)
            {
                openDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "UbiGEM Host Simulator Configuration Files (*.ughsc)|*.ughsc|All files (*.*)|*.*"
                };

                if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == false)
                {
                    openDialog.FileName = this._messageProcessor.ConfigFilepath;
                }

                openDialog.RestoreDirectory = true;

                if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this._systemSetting.LastEditedFilepath = string.Empty;
                    this._systemSetting.SaveSystemSetting();

                    this._messageProcessor.NewProject();
                    openResult = this._messageProcessor.LoadConfigFile(openDialog.FileName, out string errorText);

                    if (openResult == GemDriverError.Ok)
                    {
                        this.Title = "GEM Host Simulator - " + openDialog.FileName;
                        this._messageProcessor.ConfigFilepath = openDialog.FileName;

                        this._systemSetting.LastEditedFilepath = openDialog.FileName;
                        this._systemSetting.SaveSystemSetting();
                    }
                    else
                    {
                        this.Title = "GEM Host Simulator";
                        MessageBox.Show(errorText);
                    }

                    ctlRCMD.InitializeRemoteCommand();
                    ctlERCMD.InitializeEnhancedRemoteCommand();
                    ctlTraceData.InitializeTraceData();
                    ctlLimitMonitoring.InitializeLimitMonitoring();
                    ctlUserMessage.InitializeUserMessages();
                    _ackAndReplyWindow?.InitializeData();
                }
            }
        }
        #endregion
        #region mnuSave_Click
        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            GemDriverError error;

            if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == true)
            {
                mnuSaveAs.RaiseEvent(e);
            }
            else
            {
                error = this._messageProcessor.SaveConfigFile(out string errorText);

                if (error == GemDriverError.Ok)
                {
                    MessageBox.Show("UbiGEM host configuration file save success");
                    this._messageProcessor.IsDirty = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(errorText) == true)
                    {
                        MessageBox.Show($"Save fail. Reason: {error}");
                    }
                    else
                    {
                        MessageBox.Show($"Save fail. {errorText}");
                    }
                }
            }
        }
        #endregion
        #region mnuSaveAs_Click
        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveDialog;
            GemDriverError saveResult;

            saveDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "UbiGEM Host Simulator Configuration Files (*.ughsc)|*.ughsc|All files (*.*)|*.*"
            };
            if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == false)
            {
                saveDialog.FileName = this._messageProcessor.ConfigFilepath;
            }

            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveResult = this._messageProcessor.SaveConfigFile(saveDialog.FileName, out _);

                if (saveResult == GemDriverError.Ok)
                {
                    this._messageProcessor.IsDirty = false;

                    this.Title = "GEM Host Simulator - " + saveDialog.FileName;
                    this._messageProcessor.ConfigFilepath = saveDialog.FileName;

                    this._systemSetting.LastEditedFilepath = saveDialog.FileName;
                    this._systemSetting.SaveSystemSetting();

                    MessageBox.Show("UbiGEM host configuration file save success");
                }
                else
                {
                    MessageBox.Show("Save fail. Reason: {0}", saveResult.ToString());
                }
            }
        }
        #endregion
        #region mnuExit_Click
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.StopScheduler();

            Close();
        }
        #endregion
        #region mnuOpenConnection_Click
        private void mnuOpenConnection_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(this._messageProcessor.UGCFilepath) == true)
            {
                MessageBox.Show("UGC file is not selected.");
            }
            else
            {
                if (this._messageProcessor.StartScheduler(out string errorText) == true)
                {
                    WriteLog(MessageProcessor.DriverLogType.INFO, "Driver Open Success");
                }
                else
                {
                    WriteLog(MessageProcessor.DriverLogType.INFO, $"Driver Open Fail: {errorText}");
                }
            }
        }
        #endregion
        #region mnuCloseConnection_Click
        private void mnuCloseConnection_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.StopScheduler();

            WriteLog(MessageProcessor.DriverLogType.INFO, "Driver Close");
        }
        #endregion
        #region mnuConfiguration_Click
        private void mnuConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Popup.ConfigurationWindow popupWindow;
            GemDriverError openResult;

            bool processContinue;
            Configurtion configurationBefore;
            bool autoSendDefineReportBefore;
            bool autoSendS1F13Before;
            bool isSaveReceivedRecipeBefore;
            string ugcFilepathBefore;
            string ugcFilepathAfter;

            processContinue = true;

            if (processContinue == true)
            {
                popupWindow = new Popup.ConfigurationWindow();
                popupWindow.Initialize(this._messageProcessor);

                popupWindow.Owner = this;
                popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                ugcFilepathBefore = this._messageProcessor.UGCFilepath;

                popupWindow.ShowDialog();

                if (popupWindow.DialogResult != null && popupWindow.DialogResult == true)
                {
                    ugcFilepathAfter = this._messageProcessor.UGCFilepath;

                    // apply changed ugc file
                    #region Apply Changed UGC File
                    if (ugcFilepathBefore != ugcFilepathAfter)
                    {
                        configurationBefore = this._messageProcessor.Configuration.Copy();
                        autoSendDefineReportBefore = this._messageProcessor.CurrentSetting.AutoSendDefineReport;
                        isSaveReceivedRecipeBefore = this._messageProcessor.CurrentSetting.IsSaveRecipeReceived;
                        autoSendS1F13Before = this._messageProcessor.CurrentSetting.AutoSendS1F13;

                        this._messageProcessor.NewProject();
                        this._messageProcessor.UGCFilepath = ugcFilepathAfter;
                        this._messageProcessor.Configuration = configurationBefore;
                        this._messageProcessor.CurrentSetting.AutoSendDefineReport = autoSendDefineReportBefore;
                        this._messageProcessor.CurrentSetting.IsSaveRecipeReceived = isSaveReceivedRecipeBefore;
                        this._messageProcessor.CurrentSetting.AutoSendS1F13 = autoSendS1F13Before;
                        openResult = this._messageProcessor.LoadUGCFile(ugcFilepathAfter, out _);

                        if (openResult == GemDriverError.Ok)
                        {
                            this.Title = "GEM Host Simulator";

                            this._messageProcessor.InitializeHSMSDriver();
                        }

                        ctlRCMD.InitializeRemoteCommand();
                        ctlERCMD.InitializeEnhancedRemoteCommand();
                        ctlTraceData.InitializeTraceData();
                        ctlLimitMonitoring.InitializeLimitMonitoring();
                        ctlUserMessage.InitializeUserMessages();

                        this._messageProcessor.UpdateAckAndReply();
                        _ackAndReplyWindow?.InitializeData();

                        this._messageProcessor.IsDirty = true;
                    }
                    #endregion

                    // Save ubigem host simulator configuration
                    #region Save UbiGEM Host Simulator Configuration
                    if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == true)
                    {
                        mnuSaveAs.RaiseEvent(e);
                    }
                    #endregion
                }
            }
        }
        #endregion
        #region mnuDefault_Click
        private void mnuDefault_Click(object sender, RoutedEventArgs e)
        {
            if (this._selectedLeftFrame != SelectedFrame.Main)
            {
                this._selectedLeftFrame = SelectedFrame.Main;
                mnuDefault.IsChecked = true;
                mnuStandardMessage.IsChecked = false;
                mnuUserMessage.IsChecked = false;

                grdMain.Visibility = Visibility.Visible;
                grdMessageTest.Visibility = Visibility.Collapsed;
                grdUserMessage.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
        #region mnuStandardMessage_Click
        private void mnuStandardMessage_Click(object sender, RoutedEventArgs e)
        {
            if (this._selectedLeftFrame != SelectedFrame.MessageTest)
            {
                this._selectedLeftFrame = SelectedFrame.MessageTest;
                mnuDefault.IsChecked = false;
                mnuStandardMessage.IsChecked = true;
                mnuUserMessage.IsChecked = false;

                grdMain.Visibility = Visibility.Collapsed;
                grdMessageTest.Visibility = Visibility.Visible;
                grdUserMessage.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
        #region mnuUserMessage_Click
        private void mnuUserMessage_Click(object sender, RoutedEventArgs e)
        {
            if (this._selectedLeftFrame != SelectedFrame.UserMessage)
            {
                this._selectedLeftFrame = SelectedFrame.UserMessage;
                mnuDefault.IsChecked = false;
                mnuStandardMessage.IsChecked = false;
                mnuUserMessage.IsChecked = true;

                grdMain.Visibility = Visibility.Collapsed;
                grdMessageTest.Visibility = Visibility.Collapsed;
                grdUserMessage.Visibility = Visibility.Visible;
            }
        }
        #endregion
        #region mnuEquipmentConstatns_Click
        private void mnuEquipmentConstatns_Click(object sender, RoutedEventArgs e)
        {
            Popup.EquipmentConstantWindow popupWindow;

            popupWindow = new Popup.EquipmentConstantWindow();
            popupWindow.Initialize(this._messageProcessor);

            popupWindow.Owner = this;
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();
        }
        #endregion
        #region mnuVariables_Click
        private void mnuVariables_Click(object sender, RoutedEventArgs e)
        {
            Popup.VariableWindow popupWindow;

            popupWindow = new Popup.VariableWindow();
            popupWindow.Initialize(this._messageProcessor);

            popupWindow.Owner = this;
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();
        }
        #endregion
        #region mnuReports_Click
        private void mnuReports_Click(object sender, RoutedEventArgs e)
        {
            Popup.ReportWindow popupWindow;

            popupWindow = new Popup.ReportWindow();
            popupWindow.Initialize(this._messageProcessor);

            popupWindow.Owner = this;
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();
        }
        #endregion
        #region mnuCollectionEvents_Click
        private void mnuCollectionEvents_Click(object sender, RoutedEventArgs e)
        {
            Popup.CollectionEventWindow popupWindow;

            popupWindow = new Popup.CollectionEventWindow();
            popupWindow.Initialize(this._messageProcessor);

            popupWindow.Owner = this;
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();
        }
        #endregion
        #region mnuFormattedProcessProgram_Click
        private void mnuFormattedProcessProgram_Click(object sender, RoutedEventArgs e)
        {
            Popup.FormattedProcessProgramWindow window;

            window = new Popup.FormattedProcessProgramWindow();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();

            ctlMessageTest.InitializeFmtPP();
        }
        #endregion
        #region mnuObjectManager_Click
        private void mnuObjectManager_Click(object sender, RoutedEventArgs e)
        {
            Popup.GEMObjectWindow window;

            window = new Popup.GEMObjectWindow();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();

            ctlMessageTest.InitializeGEMObject();
        }
        #endregion
        #region mnuObjectCreatedManager_Click
        private void mnuObjectCreatedManager_Click(object sender, RoutedEventArgs e)
        {
            Popup.GEMObjectCreatedWindow window;

            window = new Popup.GEMObjectCreatedWindow();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();

            ctlMessageTest.InitializeGEMObject();
        }
        #endregion
        #region mnuSupervisedObjectManager_Click
        private void mnuSupervisedObjectManager_Click(object sender, RoutedEventArgs e)
        {
            Popup.SupervisedGEMObjectWindow window;

            window = new Popup.SupervisedGEMObjectWindow();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();

            ctlMessageTest.InitializeGEMObject();
        }
        #endregion
        #region mnuWaferMapManager_Click
        private void mnuWaferMapManager_Click(object sender, RoutedEventArgs e)
        {
            Popup.WaferMapWindow window;

            window = new Popup.WaferMapWindow();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
        #endregion
        #region MapDataType1
        private void mnuMapDataType1_Click(object sender, RoutedEventArgs e)
        {
            Popup.MapDataType1Window window;

            window = new Popup.MapDataType1Window();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
        #endregion
        #region MapDataType2
        private void mnuMapDataType2_Click(object sender, RoutedEventArgs e)
        {
            Popup.MapDataType2Window window;

            window = new Popup.MapDataType2Window();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
        #endregion
        #region MapDataType3
        private void mnuMapDataType3_Click(object sender, RoutedEventArgs e)
        {
            Popup.MapDataType3Window window;

            window = new Popup.MapDataType3Window();
            window.Initialize(this._messageProcessor);

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
        #endregion
        #region mnuAlarms_Click
        private void mnuAlarms_Click(object sender, RoutedEventArgs e)
        {
            Popup.AlarmWindow popupWindow;

            popupWindow = new Popup.AlarmWindow();
            popupWindow.Initialize(this._messageProcessor);

            popupWindow.Owner = this;
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();
        }
        #endregion
        #region mnuRemoteCommands_Click
        private void mnuRemoteCommands_Click(object sender, RoutedEventArgs e)
        {
            Popup.RemoteCommandWindow popupWindow;

            popupWindow = new Popup.RemoteCommandWindow();
            popupWindow.Initialize(this._messageProcessor);

            popupWindow.Owner = this;
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();

            ctlRCMD.InitializeRemoteCommand();
        }
        #endregion
        #region mnuEnhancedRemoteCommands_Click
        private void mnuEnhancedRemoteCommands_Click(object sender, RoutedEventArgs e)
        {
            Popup.EnhancedRemoteCommandWindow popupWindow;

            popupWindow = new Popup.EnhancedRemoteCommandWindow();
            popupWindow.Initialize(this._messageProcessor);
            popupWindow.Owner = this;
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (popupWindow.ShowDialog() == true)
            {
                this._messageProcessor.IsDirty = true;
            }

            ctlERCMD.InitializeEnhancedRemoteCommand();
        }
        #endregion
        #region mnuTraceData_Click
        private void mnuTraceData_Click(object sender, RoutedEventArgs e)
        {
            Popup.TraceDataWindow traceDataWindow;

            traceDataWindow = new Popup.TraceDataWindow();
            traceDataWindow.Initialize(this._messageProcessor);
            
            traceDataWindow.Owner = this;
            traceDataWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            traceDataWindow.ShowDialog();
            this.ctlTraceData.InitializeTraceData();
        }
        #endregion
        #region mnuLimitMonitoring_Click
        private void mnuLimitMonitoring_Click(object sender, RoutedEventArgs e)
        {
            Popup.LimitMonitoringWindow settingLimitMonitoringWindow;

            settingLimitMonitoringWindow = new Popup.LimitMonitoringWindow();
            settingLimitMonitoringWindow.Initialize(this._messageProcessor);

            settingLimitMonitoringWindow.Owner = this;
            settingLimitMonitoringWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            settingLimitMonitoringWindow.ShowDialog();

            ctlLimitMonitoring.InitializeLimitMonitoring();
        }
        #endregion
        #region mnuAckAndReply_Click
        private void mnuAckAndReply_Click(object sender, RoutedEventArgs e)
        {
            if (this._ackAndReplyWindow == null)
            {
                this._ackAndReplyWindow = new Popup.AckAndReplyWindow();
                this._ackAndReplyWindow.Initialize(this._messageProcessor);
                this._ackAndReplyWindow.Owner = this;
                this._ackAndReplyWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this._ackAndReplyWindow.Topmost = true;
            }
            if (this._ackAndReplyWindow.Visibility != Visibility.Visible)
            {
                this._ackAndReplyWindow.Visibility = Visibility.Visible;
            }
        }
        #endregion

        // CheckBox Event
        #region chkUseSECS2Log_Unchecked
        private void chkUseSECS2Log_Unchecked(object sender, RoutedEventArgs e)
        {
            this._useSECS2Log = false;
        }
        #endregion
        #region chkUseSECS2Log_Checked
        private void chkUseSECS2Log_Checked(object sender, RoutedEventArgs e)
        {
            this._useSECS2Log = true;
        }
        #endregion
        // Context Menu Event
        #region mnuLogClear_Click
        private void mnuLogClear_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                this.txtLog.Document.Blocks.Clear();
            }));
        }
        private void mnuAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            this._isLogAutoScroll = mnuAutoScroll.IsChecked;
        }
        #endregion

        // Button Event
        #region btnS1F15_Click
        private void btnS1F15_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS1F15();

                this.WriteLog(MessageProcessor.DriverLogType.SEND, $"Send S1F15 : Result={driverResult}");
            }
            else
            {
                MessageBox.Show("Send S1F15 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS1F17_Click
        private void btnS1F17_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS1F17();

                this.WriteLog(MessageProcessor.DriverLogType.SEND, $"Send S1F17 : Result={driverResult}");
            }
            else
            {
                MessageBox.Show("Send S1F17 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F17_Click
        private void btnS2F17_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS2F17();

                this.WriteLog(MessageProcessor.DriverLogType.SEND, $"Send S2F17 : Result={driverResult}");
            }
            else
            {
                MessageBox.Show("Send S2F17 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS2F31_Click
        private void btnS2F31_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            if (this._messageProcessor.HSMSDriverConnected == true)
            {
                driverResult = this._messageProcessor.SendS2F31();

                this.WriteLog(MessageProcessor.DriverLogType.SEND, $"Send S2F31 : Result={driverResult}");
            }
            else
            {
                MessageBox.Show("Send S2F31 Failed - Disconnected");
            }
        }
        #endregion
        #region btnS10F3_Click
        private void btnS10F3_Click(object sender, RoutedEventArgs e)
        {
            MessageError driverResult;

            Popup.SettingTerminalWindow settingTerminalWindow;

            settingTerminalWindow = new Popup.SettingTerminalWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            settingTerminalWindow.Initialize(this._messageProcessor, Popup.SettingTerminalWindow.TerminalMessageSelectType.S10F3, this._messageProcessor.CurrentSetting.TerminalMessage);
            settingTerminalWindow.OnTerminalMessageChanged += settingTerminalWindow_OnTerminalMessageChanged;
            settingTerminalWindow.ShowDialog();
            settingTerminalWindow.OnTerminalMessageChanged -= settingTerminalWindow_OnTerminalMessageChanged;

            settingTerminalWindow = null;

            if (string.IsNullOrEmpty(this._messageProcessor.CurrentSetting.TerminalMessage.S10F3TID) == true)
            {
                MessageBox.Show("Invalid S103 Terminal Message TID");
            }
            else
            {
                if (this._messageProcessor.HSMSDriverConnected == true)
                {
                    driverResult = this._messageProcessor.SendS10F3();

                    this.WriteLog(MessageProcessor.DriverLogType.SEND, $"Send S10F3 : Result={driverResult}");
                }
                else
                {
                    MessageBox.Show("Send S10F3 Failed - Disconnected");
                }
            }
        }
        #endregion

        // Private Method
        #region WriteLog
        private void WriteLog(UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            Brush foreground;
            string text;

            switch (logLevel)
            {
                case UbiCom.Net.Utility.Logger.LogLevel.Warning:
                case UbiCom.Net.Utility.Logger.LogLevel.Error:
                    foreground = Brushes.Red;
                    break;
                case UbiCom.Net.Utility.Logger.LogLevel.Send:
                    foreground = Brushes.LightGreen;
                    break;
                case UbiCom.Net.Utility.Logger.LogLevel.Receive:
                    foreground = Brushes.LightBlue;
                    break;
                default:
                    foreground = Brushes.White;
                    break;
            }

            if (this._useSECS2Log == false)
            {
                if (logText.IndexOf("\r\n") > -1)
                {
                    text = logText.Substring(0, logText.IndexOf("\r\n"));
                }
                else if (logText.IndexOf("\n") > -1)
                {
                    text = logText.Substring(0, logText.IndexOf("\n"));
                }
                else
                {
                    text = logText;
                }
            }
            else
            {
                text = logText;
            }

            if (text.EndsWith("\r\n") == false || text.EndsWith("\n") == false)
            {
                text = $"{text}\r\n";
            }

            Dispatcher.BeginInvoke(new Action(delegate
            {
                try
                {
                    
                    var newData = new Run(text, txtLog.CaretPosition.GetInsertionPosition(LogicalDirection.Forward))
                    {
                        Foreground = foreground
                    };

                    if (txtLog.CaretPosition.Paragraph != null)
                    {
                        while (txtLog.CaretPosition.Paragraph.Inlines.Count > 200)
                        {
                            txtLog.CaretPosition.Paragraph.Inlines.Remove(txtLog.CaretPosition.Paragraph.Inlines.FirstInline);
                        }

                        txtLog.CaretPosition.Paragraph.Inlines.Add(newData);

                        if (this._isLogAutoScroll == true)
                        {
                            txtLog.ScrollToEnd();
                        }
                    }
                }
                catch
                {
                }
            }));
        }

        private void WriteLog(MessageProcessor.DriverLogType logType, string logText)
        {
            string writeText;
            Brush foreground;

            if (logText.EndsWith("\r\n") == false || logText.EndsWith("\n") == false)
            {
                writeText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {logType} {logText}\r\n";
            }
            else
            {
                writeText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {logType} {logText}";
            }

            switch (logType)
            {
                case MessageProcessor.DriverLogType.WARN:
                    foreground = Brushes.Red;
                    break;
                case MessageProcessor.DriverLogType.SEND:
                    foreground = Brushes.LightGreen;
                    break;
                case MessageProcessor.DriverLogType.RECV:
                    foreground = Brushes.LightBlue;
                    break;
                default:
                    foreground = Brushes.White;
                    break;
            }

            Dispatcher.BeginInvoke(new Action(delegate
            {
                try
                {
                    var newData = new Run(writeText, txtLog.CaretPosition.GetInsertionPosition(LogicalDirection.Forward))
                    {
                        Foreground = foreground
                    };

                    if (txtLog.CaretPosition.Paragraph != null)
                    {
                        while (txtLog.CaretPosition.Paragraph.Inlines.Count > 200)
                        {
                            txtLog.CaretPosition.Paragraph.Inlines.Remove(txtLog.CaretPosition.Paragraph.Inlines.FirstInline);
                        }

                        txtLog.CaretPosition.Paragraph.Inlines.Add(newData);

                        if (this._isLogAutoScroll == true)
                        {
                            txtLog.ScrollToEnd();
                        }
                    }
                }
                catch
                {
                }
            }));
        }
        #endregion

    }
}