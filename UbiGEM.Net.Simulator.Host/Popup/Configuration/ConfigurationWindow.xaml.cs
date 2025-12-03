using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using UbiCom.Net.Utility.Logger;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.ValidRule;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// ConfigurationWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;

        private Configurtion _config;
        #endregion

        #region Constructor
        public ConfigurationWindow()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControls();

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

            this._config = this._messageProcessor.Configuration.Copy();

            DisplayConfiguration(this._config);
            
            chkAutoSendDefineReport.IsChecked = this._messageProcessor.CurrentSetting.AutoSendDefineReport;
            chkSendS1F13.IsChecked = this._messageProcessor.CurrentSetting.AutoSendS1F13;
            txtUGCFile.Text = this._messageProcessor.UGCFilepath;
            chkSaveReceivedRecipe.IsChecked = this._messageProcessor.CurrentSetting.IsSaveRecipeReceived;
        }
        #endregion

        // Buttton Event
        #region btnLogDirectorySelect_Click
        private void btnLogDirectorySelect_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;

            folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtLogDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }
        #endregion
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string confirmMessage;
            MessageBoxResult confirmResult;
            Structure.GemDriverError error;
            string result;

            result = IsValid();

            if (string.IsNullOrEmpty(result) == true)
            {
                if (this._messageProcessor.UGCFilepath != txtUGCFile.Text.Trim())
                {
                    confirmMessage = " Do you want to process? \n\n UGC file is changed. \r\n All data will reset";
                    confirmResult = MessageBox.Show(confirmMessage, "Warning", MessageBoxButton.YesNo);
                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        SetConfiguration();

                        this._messageProcessor.IsDirty = true;
                        DialogResult = true;
                        Close();
                    }
                }
                else
                {
                    SetConfiguration();

                    if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == false)
                    {
                        error = this._messageProcessor.SaveConfigFile(out string errorText);

                        if (error != Structure.GemDriverError.Ok)
                        {
                            MessageBox.Show(errorText);
                        }
                        else
                        {
                            this._messageProcessor.IsDirty = false;
                        }
                    }
                    else
                    {
                        this._messageProcessor.IsDirty = true;
                    }

                    DialogResult = true;
                    Close();
                }
            }
            else
            {
                MessageBox.Show(result, "Validation Fail", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion
        #region btnCancel_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        #endregion
        #region btnBrowserUGCFile_Click
        private void btnBrowserUGCFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog;
            Structure.GemDriverError error;
            string tempDriverName;
            string tempUGCDriverName;
            int tempUGCDeviceID;
            bool isOverwrite;
            string confirmMessage;

            openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                Filter = "UbiGEM Configuration Files (*.ugc)|*.ugc|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            openFileDialog.FileName = this._messageProcessor.UGCFilepath;
            
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                error = this._messageProcessor.TestUGCFile(openFileDialog.FileName, out string errorText);

                if (error == Structure.GemDriverError.Ok)
                {
                    this.txtUGCFile.Text = openFileDialog.FileName;

                    tempDriverName = txtDriverName.Text.Trim();

                    if (int.TryParse(txtDeviceId.Text.Trim(), out int tempDeviceID) == false)
                    {
                        tempDeviceID = 0;
                    }
                    tempUGCDriverName = this._messageProcessor.UGCConfiguration.DriverName;
                    tempUGCDeviceID = this._messageProcessor.UGCConfiguration.DeviceID;

                    isOverwrite = false;

                    if (string.IsNullOrEmpty(tempDriverName) == true && tempDeviceID == 0)
                    {
                        isOverwrite = true;
                    }
                    else
                    {
                        if (tempDriverName != tempUGCDriverName || tempDeviceID != tempUGCDeviceID)
                        {
                            confirmMessage = " Do you want to overwrite configuration using ugc?";

                            if (MessageBox.Show(confirmMessage, "Information", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                isOverwrite = true;
                            }
                        }
                    }

                    // overwrite 수행
                    if (isOverwrite == true)
                    {
                        txtDriverName.Text = tempUGCDriverName;
                        txtDeviceId.Text = tempUGCDeviceID.ToString();
                        cboSyncMode.SelectedItem = this._messageProcessor.UGCConfiguration.IsAsyncMode == false;

                        if (this._messageProcessor.UGCConfiguration.HSMSModeConfig.HSMSMode == HSMSMode.Active)
                        {
                            cboHsmsMode.SelectedItem = HSMSMode.Passive;
                        }
                        else
                        {
                            cboHsmsMode.SelectedItem = HSMSMode.Active;
                        }
                        
                        txtIp.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.RemoteIPAddress;
                        txtPort.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.RemotePortNo.ToString();
                        cboSecs1Log.SelectedItem = this._messageProcessor.UGCConfiguration.LogEnabledSECS1;
                        cboSecs2Log.SelectedItem = this._messageProcessor.UGCConfiguration.LogEnabledSECS2;
                        cboDriverLog.SelectedItem = this._messageProcessor.UGCConfiguration.LogEnabledSystem;
                        txtLogExpireDay.Text = this._messageProcessor.UGCConfiguration.LogExpirationDay.ToString();
                        txtLogDirectory.Text = this._messageProcessor.UGCConfiguration.LogPath;
                        txtT3Timeout.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.T3.ToString();
                        txtT5Timeout.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.T5.ToString();
                        txtT6Timeout.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.T6.ToString();
                        txtT7Timeout.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.T7.ToString();
                        txtT8Timeout.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.T8.ToString();
                        txtLinkTestInterval.Text = this._messageProcessor.UGCConfiguration.HSMSModeConfig.LinkTest.ToString();
                    }
                }
                else
                {
                    MessageBox.Show(errorText);
                }
            }
        }
        #endregion

        // Private Method
        #region InitializeControls
        private void InitializeControls()
        {
            cboSyncMode.Items.Add(true);
            cboSyncMode.Items.Add(false);

            foreach (HSMSMode tempMode in typeof(HSMSMode).GetEnumValues())
            {
                cboHsmsMode.Items.Add(tempMode);
            }

            foreach (LogMode tempMode in typeof(LogMode).GetEnumValues())
            {
                cboSecs1Log.Items.Add(tempMode);
                cboSecs2Log.Items.Add(tempMode);
                cboDriverLog.Items.Add(tempMode);
            }
        }
        #endregion
        #region DisplayConfiguration
        private void DisplayConfiguration(Configurtion configurtion)
        {
            Binding binding;

            NumberOnlyValidationRule numberOnlyValidationRule;
            NumberOnlyWithMinMaxValidationRule numberOnlyWithMinMaxValidationRule;
            IpAddressValidationRule ipAddressValidationRule;

            numberOnlyValidationRule = new NumberOnlyValidationRule();
            numberOnlyWithMinMaxValidationRule = new NumberOnlyWithMinMaxValidationRule(0, 65535);
            ipAddressValidationRule = new IpAddressValidationRule();

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("DriverName"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            BindingOperations.SetBinding(txtDriverName, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("DeviceID"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtDeviceId, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("IsAsyncMode"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            BindingOperations.SetBinding(cboSyncMode, ComboBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.HSMSMode"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            BindingOperations.SetBinding(cboHsmsMode, ComboBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.LocalIPAddress"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(ipAddressValidationRule);
            BindingOperations.SetBinding(txtIp, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.LocalPortNo"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyWithMinMaxValidationRule);
            BindingOperations.SetBinding(txtPort, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("LogEnabledSECS1"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            BindingOperations.SetBinding(cboSecs1Log, ComboBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("LogEnabledSECS2"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            BindingOperations.SetBinding(cboSecs2Log, ComboBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("LogEnabledSystem"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            BindingOperations.SetBinding(cboDriverLog, ComboBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("LogExpirationDay"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtLogExpireDay, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("LogPath"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            BindingOperations.SetBinding(txtLogDirectory, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.T3"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtT3Timeout, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.T5"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtT5Timeout, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.T6"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtT6Timeout, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.T7"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtT7Timeout, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.T8"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtT8Timeout, TextBox.TextProperty, binding);

            binding = new Binding
            {
                Source = configurtion,
                Path = new PropertyPath("HSMSModeConfig.LinkTest"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            binding.ValidationRules.Add(numberOnlyValidationRule);
            BindingOperations.SetBinding(txtLinkTestInterval, TextBox.TextProperty, binding);
        }
        #endregion
        #region IsValid
        private string IsValid()
        {
            Structure.GemDriverError openResult;

            string result = string.Empty;
            string logDir;
            string startupPath;
            string logPath;
            DirectoryInfo dirInfo;

            if (string.IsNullOrEmpty(txtUGCFile.Text.Trim()) == false)
            {
                if (System.IO.File.Exists(txtUGCFile.Text.Trim()) == false)
                {
                    result = "ugc file is not exist";
                    txtUGCFile.Focus();
                }
                else
                {
                    openResult = this._messageProcessor.TestUGCFile(txtUGCFile.Text.Trim(), out string errorText);

                    if (openResult != Structure.GemDriverError.Ok)
                    {
                        if (string.IsNullOrEmpty(errorText) == false)
                        {
                            result = string.Format("ugc file is not valid\n\n{0}", errorText);
                        }
                        else
                        {
                            result = string.Format("ugc file is not valid\n\n{0}", openResult.ToString());
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(txtDriverName.Text.Trim()) == true)
                {
                    result = "Driver Name is blank";

                    txtDriverName.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(txtDeviceId.Text.Trim()) == true)
                {
                    result = "Driver ID is blank";

                    txtDeviceId.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtDeviceId.Text.Trim(), out _) == false)
                {
                    result = "invalid Driver ID";

                    txtDeviceId.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(cboHsmsMode.Text) == true)
                {
                    result = "select HSMS Mode";

                    cboHsmsMode.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(txtIp.Text.Trim()) == true)
                {
                    result = "IP Address is blank";

                    txtIp.Focus();
                }
                else
                {
                    if (IPAddress.TryParse(txtIp.Text.Trim(), out _) == false)
                    {
                        result = "invalid IP Address";

                        txtIp.Focus();
                    }
                }
            }
            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(txtIp.Text.Trim()) == false)
                {
                    if (IPAddress.TryParse(txtIp.Text.Trim(), out _) == false)
                    {
                        result = "invalid IP Address";

                        txtIp.Focus();
                    }
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(txtPort.Text.Trim()) == true)
                {
                    result = "Port No is blank";

                    txtPort.Focus();
                }
                else
                {
                    if (int.TryParse(txtPort.Text.Trim(), out _) == false)
                    {
                        result = "invalid Port No";

                        txtPort.Focus();
                    }
                }
            }
            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(txtPort.Text) == false)
                {
                    if (int.TryParse(txtPort.Text.Trim(), out _) == false)
                    {
                        result = "invalid Local Port No";

                        txtPort.Focus();
                    }
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(cboSecs1Log.Text) == true)
                {
                    result = "select SECS-I Log";

                    cboSecs1Log.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(cboSecs2Log.Text) == true)
                {
                    result = "select SECS-II Log";

                    cboSecs2Log.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(cboDriverLog.Text) == true)
                {
                    result = "select Driver Log";

                    cboDriverLog.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtLogExpireDay.Text.Trim(), out _) == false)
                {
                    result = "invalid Log Expire Day";

                    txtLogExpireDay.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (string.IsNullOrEmpty(txtLogDirectory.Text.Trim()) == true)
                {
                    result = "select Log Directory";

                    txtLogDirectory.Focus();
                }
                else
                {
                    logDir = txtLogDirectory.Text.Trim();

                    if (logDir.IndexOf(':') > 0)
                    {
                        logPath = logDir;
                    }
                    else
                    {
                        startupPath = AppDomain.CurrentDomain.BaseDirectory;
                        logPath = string.Format("{0}{1}", startupPath, logDir);
                    }

                    try
                    {
                        dirInfo = new DirectoryInfo(logPath);
                    }
                    catch (NotSupportedException)
                    {
                        result = string.Format("invalid Log Directory");
                        txtLogDirectory.Focus();
                    }
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtT3Timeout.Text.Trim(), out _) == false)
                {
                    result = "invalid T3 Timeout";

                    txtT3Timeout.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtT5Timeout.Text.Trim(), out _) == false)
                {
                    result = "invalid T5 Timeout";

                    txtT5Timeout.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtT6Timeout.Text.Trim(), out _) == false)
                {
                    result = "invalid T6 Timeout";

                    txtT6Timeout.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtT7Timeout.Text.Trim(), out _) == false)
                {
                    result = "invalid T7 Timeout";

                    txtT7Timeout.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtT8Timeout.Text.Trim(), out _) == false)
                {
                    result = "invalid T8 Timeout";

                    txtT8Timeout.Focus();
                }
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (uint.TryParse(txtLinkTestInterval.Text.Trim(), out _) == false)
                {
                    result = "invalid Link Test Interval Timeout";

                    txtLinkTestInterval.Focus();
                }
            }

            return result;
        }
        #endregion
        #region SetConfiguration
        private void SetConfiguration()
        {
            this._messageProcessor.CurrentSetting.IsSaveRecipeReceived = chkSaveReceivedRecipe.IsChecked.Value;

            this._messageProcessor.Configuration.DriverName = txtDriverName.Text.Trim();
            this._messageProcessor.Configuration.DeviceType = DeviceType.Host;

            this._messageProcessor.CurrentSetting.AutoSendDefineReport = chkAutoSendDefineReport.IsChecked.Value;
            this._messageProcessor.CurrentSetting.AutoSendS1F13 = chkSendS1F13.IsChecked.Value;

            this._messageProcessor.Configuration.DeviceID = int.Parse(txtDeviceId.Text.Trim());
            this._messageProcessor.Configuration.IsAsyncMode = cboSyncMode.Text.ToUpper() == "FALSE";

            this._messageProcessor.UGCFilepath = this.txtUGCFile.Text.Trim();

            this._messageProcessor.Configuration.HSMSModeConfig.HSMSMode = (HSMSMode)Enum.Parse(typeof(HSMSMode), cboHsmsMode.Text);
            this._messageProcessor.Configuration.HSMSModeConfig.LocalIPAddress = txtIp.Text.Trim();
            this._messageProcessor.Configuration.HSMSModeConfig.RemoteIPAddress = txtIp.Text.Trim();

            if (int.TryParse(txtPort.Text.Trim(), out int intValue) == true)
            {
                this._messageProcessor.Configuration.HSMSModeConfig.LocalPortNo = intValue;
                this._messageProcessor.Configuration.HSMSModeConfig.RemotePortNo = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.HSMSModeConfig.LocalPortNo = 0;
                this._messageProcessor.Configuration.HSMSModeConfig.RemotePortNo = 0;
            }

            this._messageProcessor.Configuration.LogEnabledSECS1 = (LogMode)Enum.Parse(typeof(LogMode), cboSecs1Log.Text);
            this._messageProcessor.Configuration.LogEnabledSECS2 = (LogMode)Enum.Parse(typeof(LogMode), cboSecs2Log.Text);
            this._messageProcessor.Configuration.LogEnabledSystem = (LogMode)Enum.Parse(typeof(LogMode), cboDriverLog.Text);

            if (int.TryParse(txtLogExpireDay.Text.Trim(), out intValue) == true)
            {
                this._messageProcessor.Configuration.LogExpirationDay = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.LogExpirationDay = 30;
            }

            this._messageProcessor.Configuration.LogPath = txtLogDirectory.Text.Trim();

            if (int.TryParse(txtT3Timeout.Text, out intValue) == true)
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T3 = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T3 = 45;
            }

            if (int.TryParse(txtT5Timeout.Text, out intValue) == true)
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T5 = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T5 = 10;
            }

            if (int.TryParse(txtT6Timeout.Text, out intValue) == true)
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T6 = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T6 = 5;
            }

            if (int.TryParse(txtT7Timeout.Text, out intValue) == true)
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T7 = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T7 = 10;
            }

            if (int.TryParse(txtT8Timeout.Text, out intValue) == true)
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T8 = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.HSMSModeConfig.T8 = 5;
            }

            if (int.TryParse(txtLinkTestInterval.Text, out intValue) == true)
            {
                this._messageProcessor.Configuration.HSMSModeConfig.LinkTest = intValue;
            }
            else
            {
                this._messageProcessor.Configuration.HSMSModeConfig.LinkTest = 120;
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