using System.Windows;
using UbiSam.Net.KeyLock.HardwareManager;
using System.Windows.Controls;
using System.Linq;
using System;
using UbiSam.Net.KeyLock.Utilities;
using UbiSam.Net.KeyLock.Structure;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using UbiSam.Net.KeyLock.Maker.Info;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Text;

namespace UbiSam.Net.KeyLock.Maker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region MemberVariable
        private readonly ObservableCollection<ProductMakeInfo> _productMakeInfoCollection;
        private readonly ObservableCollection<KeyLockInfo> _availableTargetCollection;
        private readonly UsbController _usbController;
        private bool _nonProgress;
        private bool _stopProgress;
        private int _progressing;
        private int _progressingMaximum;
        private int _progressingMinimum;
        private const int _stepCountMakingUSBKeylock = 10;

        private readonly LicenseChecker _checker;

        private readonly Mega _mega;
        private bool _megaMode;
        private readonly System.Timers.Timer _megaCheckerTimer;
        #endregion
        #region Property
        public bool MegaMode
        {
            get
            {
                return this._megaMode;
            }
            private set
            {
                if (this._megaMode != value)
                {
                    this._megaMode = value;
                    NotifyPropertyChanged("MegaMode");
                }
            }
        }
        public bool NonProgress
        {
            get
            {
                return this._nonProgress;
            }
            set
            {
                if (this._nonProgress != value)
                {
                    this._nonProgress = value;
                    NotifyPropertyChanged("NonProgress");
                }
            }
        }
        public int Progressing
        {
            get
            {
                return this._progressing;
            }
            set
            {
                if (this._progressing != value)
                {
                    this._progressing = value;
                    NotifyPropertyChanged("Progressing");
                }
            }
        }
        public int ProgressingMaximum
        {
            get
            {
                return this._progressingMaximum;
            }
            set
            {
                if (this._progressingMaximum != value)
                {
                    this._progressingMaximum = value;
                    NotifyPropertyChanged("ProgressingMaximum");
                }
            }
        }
        public int ProgressingMinimum
        {
            get
            {
                return this._progressingMinimum;
            }
            set
            {
                if (this._progressingMinimum != value)
                {
                    this._progressingMinimum = value;
                    NotifyPropertyChanged("ProgressingMinimum");
                }
            }
        }
        #endregion
        #region Constructor
        public MainWindow()
        {
            string myDocumentPath;
            string logPath;

            InitializeComponent();

            this._megaMode = false;
            this._mega = Mega.Construct();

            this._checker = new LicenseChecker();

            this.Progressing = 0;
            this.NonProgress = true;
            this._stopProgress = false;

            this._usbController = new UsbController();
            this._availableTargetCollection = new ObservableCollection<KeyLockInfo>();
            this._productMakeInfoCollection = new ObservableCollection<ProductMakeInfo>();

            this._usbController.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);

            foreach (Product product in (Product[])Enum.GetValues(typeof(Product)))
            {
                if (product != Product.None && product != Product.KeyIn)
                {
                    this._productMakeInfoCollection.Add(new ProductMakeInfo(product));
                }
            }

            dgrProductMaking.ItemsSource = this._productMakeInfoCollection;

            myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            logPath = string.Format(@"{0}\UbiSAM\KeyLockMaker", myDocumentPath);

            if (this._mega != null)
            {
                this._megaCheckerTimer = new System.Timers.Timer();
                this._megaCheckerTimer.Elapsed += MegaCheckerTimer_Elapsed;
                this._megaCheckerTimer.AutoReset = false;
                this._megaCheckerTimer.Interval = 1000;
                this._megaCheckerTimer.Start();
            }
        }
        #endregion
        // Window Event
        #region mainWindow_Loaded
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
        #endregion
        #region mainWindow_Closing
        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this._nonProgress == false)
            {
                MessageBox.Show("not completed before work");
                e.Cancel = true;
            }
        }
        #endregion

        // MegaLock Checker
        private void MegaCheckerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._megaCheckerTimer.Stop();

            if (this._nonProgress == true)
            {
                if (this._mega.Initialized == false)
                {
                    this._mega.Initialize();
                }
                if (this._mega.Connected == false)
                {
                    this._mega.Connect();
                }
                else
                {
                    if (this._nonProgress == true)
                    {
                        this._mega.Check();
                    }
                }

                this.MegaMode = this._mega.Connected;

                Dispatcher.Invoke(() =>
                {
                    if (this.MegaMode == true)
                    {
                        if (this._availableTargetCollection.FirstOrDefault(t => t.LockType == LockType.MegaLock && t.LicenseInfo != null && t.LicenseInfo.Identifier == this._mega.SerialNumberAsString) == null)
                        {
                            this._availableTargetCollection.Add(new KeyLockInfo()
                            {
                                LockType = LockType.MegaLock,
                                LicenseInfo = new LicenseInfo(this._mega.SerialNumberAsString),
                                TargetDisk = new MegaLockDisk(this._mega.IDAsString, this._mega.SerialNumberAsString),
                            });
                        }
                    }
                    else
                    {
                        KeyLockInfo searchedInfo = this._availableTargetCollection.FirstOrDefault(t => t.LockType == LockType.MegaLock);

                        if (searchedInfo != null)
                        {
                            this._availableTargetCollection.Remove(searchedInfo);
                        }
                    }
                });
            }

            this._megaCheckerTimer.Start();
        }
        // Event About USB
        #region DoStateChanged
        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            KeyLockInfo keyLockInfo;
            LicenseInfo licenseInfo;

            if (e.State == UsbStateChange.Added)
            {
                if (e.Partition != null && e.Partition.OwnerDisk != null)
                {
                    keyLockInfo = this._availableTargetCollection.FirstOrDefault(t => t.TargetDisk.DeviceID == e.Partition.OwnerDisk.DeviceID);

                    if (keyLockInfo == null)
                    {
                        licenseInfo = this._checker.GetLicenseFromUSBKey(e.Partition.OwnerDisk);

                        keyLockInfo = new KeyLockInfo()
                        {
                            TargetDisk = e.Partition.OwnerDisk,
                            LicenseInfo = licenseInfo
                        };

                        this._availableTargetCollection.Add(keyLockInfo);
                    }
                }
            }
            else if (e.State == UsbStateChange.Removed)
            {
                keyLockInfo = this._availableTargetCollection.FirstOrDefault(t => t.IsContainLetter(e.Partition.Letter));

                if (keyLockInfo != null)
                {
                    this._availableTargetCollection.Remove(keyLockInfo);
                }
            }
        }
        #endregion

        // Button Event
        #region btnStart_Click
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            bool progress;
            string errorText;

            StringBuilder sbProductMakingInfo;

            progress = false;
            errorText = string.Empty;

            sbProductMakingInfo = new StringBuilder();

            if (this._nonProgress == false)
            {
                MessageBox.Show("not completed before work");
            }
            else
            {
                progress = true;

                if (progress == true)
                {
                    if (this._productMakeInfoCollection.Count(t => t.IsSelected == true) == 0)
                    {
                        progress = false;
                        errorText = string.Format("product is not selected");
                    }

                    if (progress == true)
                    {
                        if (this._productMakeInfoCollection.Count(t => t.IsSelected == true && t.Count == 0) > 0)
                        {
                            progress = false;
                            errorText = string.Format("product count 0 is exists");
                        }
                    }

                    if (progress == true)
                    {
                        foreach (ProductMakeInfo productMakeInfo in this._productMakeInfoCollection.Where(t => t.IsSelected == true))
                        {
                            if (progress == true)
                            {
                                if (ProductMakeInfo.PredefinedProducts.Contains(productMakeInfo.Product) == false)
                                {
                                    if (string.IsNullOrWhiteSpace(productMakeInfo.ProductCode) == true || productMakeInfo.ProductCode.Length != 2)
                                    {
                                        progress = false;
                                        errorText = string.Format("invalid product code is exists");
                                    }
                                }
                            }
                        }
                    }
                }

                if (progress == true)
                {
                    sbProductMakingInfo.Clear();

                    foreach (ProductMakeInfo productMakeInfo in this._productMakeInfoCollection.Where(t => t.IsSelected == true))
                    {
                        sbProductMakingInfo.Append("\r\n");
                        sbProductMakingInfo.Append("\r\n");

                        if (ProductMakeInfo.PredefinedProducts.Contains(productMakeInfo.Product) == true)
                        {
                            sbProductMakingInfo.Append($"Product={productMakeInfo.Product}   Count={productMakeInfo.Count}");
                        }
                        else
                        {
                            sbProductMakingInfo.Append($"ProductCode={productMakeInfo.ProductCode}   Count={productMakeInfo.Count}");
                        }
                    }

                    sbProductMakingInfo.Append("\r\n");
                    sbProductMakingInfo.Append("\r\n");

                    if (MessageBox.Show($"Selected Product as follows are right? {sbProductMakingInfo}", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        this.NonProgress = false;
                        this._stopProgress = false;
                        System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(MakeUSBKeylockWorker))
                        {
                            Name = "MakeUSBKeylockWorker"
                        };
                        t.Start();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(errorText) == false)
                    {
                        MessageBox.Show(errorText);
                    }
                }
            }
        }
        #endregion
        #region btnStop_Click
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (this._nonProgress == false)
            {
                this._stopProgress = true;
            }
        }
        #endregion
        #region btnExit_Click
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (this._nonProgress == false)
            {
                MessageBox.Show("not completed before work");
            }
            else
            {
                Close();
            }
        }
        #endregion

        #region DataGridEvent
        private void btnProtect_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            KeyLockInfo keyLockInfo;
            string physicalDriveSection;
            int physicalDriveNum;
            List<string> partitionFileNames;
            string letter;
            string targetDirPath;

            string errorText;

            keyLockInfo = null;
            physicalDriveNum = -1;

            errorText = string.Empty;
            btn = e.Source as Button;

            this.NonProgress = false;

            if (btn != null)
            {
                keyLockInfo = btn.DataContext as KeyLockInfo;

                if (keyLockInfo == null)
                {
                    this.NonProgress = true;
                }
                else
                {
                    physicalDriveSection = keyLockInfo.TargetDisk.DeviceID;

                    if (string.IsNullOrEmpty(physicalDriveSection) == true)
                    {
                        errorText = string.Format("Can not make usb keylock on {0}", keyLockInfo.TargetDisk.DeviceID);
                        this.NonProgress = true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(physicalDriveSection) == false && physicalDriveSection.Length > "PHYSICALDRIVE".Length && physicalDriveSection.IndexOf("PHYSICALDRIVE") > -1)
                        {
                            physicalDriveSection = physicalDriveSection.Substring(physicalDriveSection.IndexOf("PHYSICALDRIVE") + "PHYSICALDRIVE".Length);

                            if (int.TryParse(physicalDriveSection, out physicalDriveNum) == false)
                            {
                                errorText = string.Format("physicalDriverNumber is not integer on {0}", keyLockInfo.TargetDisk.DeviceID);
                            }
                        }
                        else
                        {
                            errorText = string.Format("Can not make usb keylock on {0}", keyLockInfo.TargetDisk.DeviceID);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(errorText) == false || physicalDriveNum <= 0)
            {
                this.NonProgress = true;
            }
            else
            {
                partitionFileNames = MakePartitioningFile(physicalDriveNum, KeyLockInfo.VolumeName);
                letter = this._usbController.GetDriveLetterFromPhysicalNum(physicalDriveNum, KeyLockInfo.VolumeName);

                if (partitionFileNames != null && partitionFileNames.Count == 3)
                {
                    ProtectOnUSB(partitionFileNames[2]);

                    // Protect 검사
                    try
                    {
                        AddLog(string.Format("Start Protect on {0}", letter));

                        targetDirPath = string.Format(@"{0}\license_check", letter);

                        Directory.CreateDirectory(targetDirPath);

                        keyLockInfo.IsWriteProtectFailed = true;
                        AddLog(string.Format("Write Protect fail on {0}", letter), true);
                    }
                    catch
                    {
                        keyLockInfo.IsWriteProtectFailed = false;
                        AddLog(string.Format("Write Protect success on {0}", letter));
                    }
                }
                else
                {
                    errorText = string.Format("Write Protect fail on {0}", keyLockInfo.TargetDisk.DeviceID);
                    this.NonProgress = true;
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                MessageBox.Show(errorText);
                AddLog(errorText, true);
            }

            this.NonProgress = true;
        }
        private void chkTargetAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked;
            CheckBox checkBox;

            if (e.Source != null)
            {
                checkBox = e.Source as CheckBox;
                isChecked = checkBox.IsChecked.Value;

                foreach (KeyLockInfo info in this._availableTargetCollection)
                {
                    if (info.IsCandidate == true)
                    {
                        info.IsTarget = isChecked;
                    }
                }
            }
        }
        private void chkProtectAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked;
            CheckBox checkBox;

            if (e.Source != null)
            {
                checkBox = e.Source as CheckBox;
                isChecked = checkBox.IsChecked.Value;

                foreach (KeyLockInfo info in this._availableTargetCollection)
                {
                    if (info.IsCandidate == true)
                    {
                        info.IsProtect = isChecked;
                    }
                }
            }
        }
        #endregion

        // Private Method
        #region Initialize
        private void Initialize()
        {
            KeyLockInfo keyLockInfo;
            LicenseInfo licenseInfo;

            this._usbController.ReadAvailableDisks();

            foreach (UsbDisk disk in _usbController.ExistsDisks)
            {
                licenseInfo = this._checker.GetLicenseFromUSBKey(disk);

                keyLockInfo = new KeyLockInfo()
                {
                    TargetDisk = disk,
                    LicenseInfo = licenseInfo
                };

                this._availableTargetCollection.Add(keyLockInfo);
            }

            dgrAvailableTarget.ItemsSource = this._availableTargetCollection;
        }
        #endregion
        #region NotifyPropertyChanged
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        // Make Usb Keylock Thread
        #region MakeUSBKeylockWorker
        private void MakeUSBKeylockWorker()
        {
            StringBuilder sbMessage;
            string errorText;
            int totalStep;
            List<KeyLockInfo> targetCollection;
            KeyLockInfo tempKeyLockInfo;
            LicenseInfo licenseInfo;

            bool megaMode;

            int totalCount;
            int targetCount;
            int failCount;
            int writeProtectSkipCount;
            int writeProtectFailCount;
            int successCount;

            errorText = string.Empty;

            targetCollection = new List<KeyLockInfo>();

            totalStep = _stepCountMakingUSBKeylock * this._availableTargetCollection.Count(t => t.IsCandidate == true && t.IsTarget == true);

            megaMode = this.MegaMode;

            if (megaMode == false)
            {
                foreach (KeyLockInfo info in this._availableTargetCollection)
                {
                    if (info.IsCandidate == true && info.IsTarget == true)
                    {
                        targetCollection.Add(info);
                    }
                }
            }
            else
            {
                targetCollection.AddRange(this._availableTargetCollection.Where(t => t.LockType == LockType.MegaLock));

                if (targetCollection.Count != 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"MegaLock Type USB is not exists");
                    });
                }

                if(this._nonProgress == false)
                {
                    AddLog($"Start Make USBKeyLock on {targetCollection[0].TargetDisk.VID} {targetCollection[0].TargetDisk.SerialNumber}");

                    bool makeResult = MakeMegaLock(targetCollection[0]);
                    string res = makeResult == true ? "success" : "fail";

                    AddLog($"End Make USBKeyLock {res} on {targetCollection[0].TargetDisk.VID} {targetCollection[0].TargetDisk.SerialNumber}");

                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"End Make USBKeyLock {res} on {targetCollection[0].TargetDisk.VID} {targetCollection[0].TargetDisk.SerialNumber}");
                    });
                }
            }

            if (megaMode == false)
            {
                if (this._nonProgress == false)
                {
                    this.ProgressingMaximum = totalStep;
                    this.ProgressingMinimum = 0;
                    this.Progressing = 0;

                    foreach (KeyLockInfo info in targetCollection)
                    {
                        if (this._stopProgress == true)
                        {
                            AddLog(string.Format("Stop Make USBKeyLock"), true);
                            break;
                        }

                        tempKeyLockInfo = this._availableTargetCollection.FirstOrDefault(t => t.TargetDisk != null && t.TargetDisk.DeviceID == info.TargetDisk.DeviceID);

                        if (tempKeyLockInfo == null)
                        {
                            this.Progressing = this._progressing + _stepCountMakingUSBKeylock;
                        }
                        else
                        {
                            AddLog(string.Format("Start Make USBKeyLock on {0}", tempKeyLockInfo.TargetDisk.DeviceID));
                            MakeUSBKey(info, out errorText);
                        }
                    }
                }

                this.Progressing = totalStep;

                Dispatcher.Invoke(() =>
                {
                    totalCount = this._availableTargetCollection.Count;
                    targetCount = this._availableTargetCollection.Count(t => t.IsTarget == true);
                    failCount = this._availableTargetCollection.Count(t => t.IsTarget == true && t.IsLicenseFileCreateFailed == true);
                    writeProtectSkipCount = this._availableTargetCollection.Count(t => t.IsTarget == true && t.IsLicenseFileCreateFailed == false && t.IsProtect == false);
                    writeProtectFailCount = this._availableTargetCollection.Count(t => t.IsTarget == true && t.IsLicenseFileCreateFailed == false && t.IsProtect == true && t.IsWriteProtectFailed == true);
                    successCount = this._availableTargetCollection.Count(t => t.IsTarget == true && t.IsLicenseFileCreateFailed == false && t.IsProtect == true && t.IsWriteProtectFailed == false);

                    sbMessage = new StringBuilder();

                    sbMessage.AppendFormat("Total USB Count: {0}", totalCount);
                    sbMessage.AppendLine(); sbMessage.AppendLine();

                    sbMessage.AppendFormat("Target USB Count: {0}", targetCount);
                    sbMessage.AppendLine(); sbMessage.AppendLine();

                    sbMessage.AppendFormat("License Create Fail Count: {0}", failCount);
                    sbMessage.AppendLine(); sbMessage.AppendLine();

                    sbMessage.AppendFormat("Write Protect Skip Count: {0}", writeProtectSkipCount);
                    sbMessage.AppendLine(); sbMessage.AppendLine();

                    sbMessage.AppendFormat("Write Protect Fail Count: {0}", writeProtectFailCount);
                    sbMessage.AppendLine(); sbMessage.AppendLine();

                    sbMessage.AppendFormat("Success Count: {0}", successCount);

                    MessageBox.Show(sbMessage.ToString());

                    this._availableTargetCollection.Clear();

                    foreach (UsbDisk disk in _usbController.ExistsDisks)
                    {
                        licenseInfo = this._checker.GetLicenseFromUSBKey(disk);

                        tempKeyLockInfo = new KeyLockInfo()
                        {
                            TargetDisk = disk,
                            LicenseInfo = licenseInfo
                        };

                        this._availableTargetCollection.Add(tempKeyLockInfo);
                    }
                });
            }

            this.NonProgress = true;
        }
        #endregion
        #region MakeMegaLock
        private bool MakeMegaLock(KeyLockInfo keyLockInfo)
        {
            string data;
            string readed;

            bool result;

            List<ProductInfo> list;

            result = false;

            if (keyLockInfo != null && keyLockInfo.TargetDisk != null && keyLockInfo.TargetDisk.VID == this._mega.IDAsString && keyLockInfo.TargetDisk.SerialNumber == this._mega.SerialNumberAsString)
            {
                list = new List<ProductInfo>();

                foreach (ProductMakeInfo productMakeInfo in this._productMakeInfoCollection)
                {
                    if (productMakeInfo.IsSelected == true && productMakeInfo.Count > 0)
                    {
                        if (ProductMakeInfo.PredefinedProducts.Contains(productMakeInfo.Product) == true)
                        {
                            list.Add(new ProductInfo()
                            {
                                ProductCode = ProductConverter.Convert(productMakeInfo.Product),
                                Count = productMakeInfo.Count,
                            });
                        }
                        else
                        {
                            list.Add(new ProductInfo()
                            {
                                ProductCode = productMakeInfo.ProductCode,
                                Count = productMakeInfo.Count,
                            });
                        }
                    }
                }

                data = ProductConverter.Convert(list);

                result = this._mega.WriteProduct(data);

                if (result == true)
                {
                    readed = this._mega.ProductCode;
                    result = data != null && readed != null && data == readed;
                }

            }

            return result;
        }
        #endregion
        #region MakeUSBKey
        private void MakeUSBKey(KeyLockInfo keyLockInfo, out string errorText)
        {
            string volumeName;
            string secretKey;
            string plainText;
            string physicalDriveSection;

            StringBuilder sbProductMakingInfo;

            int physicalDriveNum;

            string letter;
            string[] keyLock;
            List<string> partitionFileNames;

            string licenseFilePath;
            string licenseCreatedFilePath;
            StreamWriter writer;

            string targetDirPath;
            int remainingStep;
            int progressStart;

            bool result;

            result = true;

            sbProductMakingInfo = new StringBuilder();

            errorText = string.Empty;
            partitionFileNames = null;
            physicalDriveSection = string.Empty;
            plainText = string.Empty;
            volumeName = string.Empty;
            secretKey = string.Empty;
            physicalDriveNum = -1;
            keyLock = null;
            letter = string.Empty;
            writer = null;
            licenseFilePath = string.Empty;
            licenseCreatedFilePath = string.Empty;

            progressStart = this._progressing;
            remainingStep = _stepCountMakingUSBKeylock;

            if (keyLockInfo.IsCandidate == false)
            {
                result = false;
                errorText = string.Format("Can not make usb keylock on {0}", keyLockInfo.TargetDisk.DeviceID);
                AddLog(errorText, true);
                this.Progressing = progressStart + _stepCountMakingUSBKeylock;
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                volumeName = KeyLockInfo.VolumeName;

                secretKey = string.Format("{0}_{1}_{2}", keyLockInfo.TargetDisk.VID, keyLockInfo.TargetDisk.PID, keyLockInfo.TargetDisk.SerialNumber);

                sbProductMakingInfo.Clear();

                foreach (ProductMakeInfo productMakeInfo in this._productMakeInfoCollection.Where(t => t.IsSelected == true))
                {
                    if (sbProductMakingInfo.Length != 0)
                    {
                        sbProductMakingInfo.Append(",");
                    }

                    sbProductMakingInfo.Append($"{productMakeInfo.Product}|{productMakeInfo.Count}");
                }

                plainText = sbProductMakingInfo.ToString();

                physicalDriveSection = keyLockInfo.TargetDisk.DeviceID;

                if (string.IsNullOrEmpty(physicalDriveSection) == true)
                {
                    result = false;
                    errorText = string.Format("Can not make usb keylock on {0}", keyLockInfo.TargetDisk.DeviceID);
                    AddLog(errorText, true);
                    this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                }
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                if (physicalDriveSection.IndexOf("PHYSICALDRIVE") > -1)
                {
                    physicalDriveSection = physicalDriveSection.Substring(physicalDriveSection.IndexOf("PHYSICALDRIVE") + "PHYSICALDRIVE".Length);

                    if (int.TryParse(physicalDriveSection, out physicalDriveNum) == false)
                    {
                        result = false;
                        errorText = string.Format("physicalDriverNumber is not integer on {0}", keyLockInfo.TargetDisk.DeviceID);
                        AddLog(errorText, true);
                        this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                    }
                }
                else
                {
                    result = false;
                    errorText = string.Format("physicalDriverNumber is not integer on {0}", keyLockInfo.TargetDisk.DeviceID);
                    AddLog(errorText, true);
                    this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                }
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                keyLock = Cryptography.MakeKeyLockV2(plainText, volumeName, secretKey);

                if (keyLock.Length != 3)
                {
                    result = false;
                    errorText = string.Format("Cryptography failed on {0}", keyLockInfo.TargetDisk.DeviceID);
                    AddLog(errorText, true);

                    this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                }
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                partitionFileNames = MakePartitioningFile(physicalDriveNum, volumeName);

                if (partitionFileNames == null  || partitionFileNames.Count != 3)
                {
                    result = false;
                    errorText = string.Format("make paritioning info failed on {0}", keyLockInfo.TargetDisk.DeviceID);
                    AddLog(errorText, true);

                    this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                }
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                // 단계 1. USB Devie 쓰기 방지 해제
                result = ClearReadonlyOnUSB(partitionFileNames[0]);

                if (result == false)
                {
                    keyLockInfo.IsWriteProtectFailed = true;
                    result = false;
                    errorText = string.Format("clear readonly failed on {0}", keyLockInfo.TargetDisk.DeviceID);
                    AddLog(errorText, true);

                    this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                }
                else
                {
                    // USB 쓰기 방지 해제 검사
                    try
                    {
                        targetDirPath = string.Format(@"{0}\license_check", letter);

                        Directory.CreateDirectory(targetDirPath);

                        AddLog(string.Format("clear readonly success on {0}", keyLockInfo.TargetDisk.DeviceID));
                    }
                    catch
                    {
                        result = false;
                        keyLockInfo.IsLicenseFileCreateFailed = false;
                        errorText = string.Format("clear readonly failed on {0}", keyLockInfo.TargetDisk.DeviceID);
                        AddLog(errorText, true);
                    }
                }
            }

            if(result == true)
            {
                // 단계 2. 파티션 재설정
                result = RepartitionOnUSB(partitionFileNames[1]);

                if (result == false)
                {
                    keyLockInfo.IsLicenseFileCreateFailed = true;
                    result = false;
                    errorText = string.Format("Repartitioning failed on {0}", keyLockInfo.TargetDisk.DeviceID);
                    AddLog(errorText, true);

                    this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                }
                else
                {
                    letter = this._usbController.GetDriveLetterFromPhysicalNum(physicalDriveNum, volumeName);

                    if (string.IsNullOrEmpty(letter) == true)
                    {
                        keyLockInfo.IsLicenseFileCreateFailed = true;
                        result = false;
                        errorText = string.Format("Repartitioning failed on {0}", keyLockInfo.TargetDisk.DeviceID);
                        AddLog(errorText, true);

                        this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                    }
                    else
                    {
                        this._usbController.Refresh(physicalDriveNum, keyLockInfo.TargetDisk);

                        if (keyLockInfo.TargetDisk.Count == 0)
                        {
                            keyLockInfo.IsLicenseFileCreateFailed = true;
                            result = false;
                            errorText = string.Format("Repartitioning failed on {0}", keyLockInfo.TargetDisk.DeviceID);
                            AddLog(errorText, true);

                            this.Progressing = progressStart + _stepCountMakingUSBKeylock;
                        }
                        else
                        {
                            AddLog(string.Format("repartition success on {0}", keyLockInfo.TargetDisk.DeviceID));
                        }
                    }
                }
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                // 단계 3. 라이선스 정보 쓰기
                try
                {
                    licenseFilePath = string.Format(@"{0}\license", letter);
                    writer = new StreamWriter(licenseFilePath);

                    writer.WriteLine(keyLock[0]);
                    writer.WriteLine(keyLock[1]);
                    writer.Write(keyLock[2]);

                    writer.Flush();
                    writer.Close();

                    AddLog(string.Format("Write license file success on {0}", keyLockInfo.TargetDisk.DeviceID));

                    licenseCreatedFilePath = string.Format(@"{0}\licenseInfo", letter);

                    writer = new StreamWriter(licenseCreatedFilePath);

                    writer.Write("Created: ");
                    writer.Write(DateTime.Now.ToString("yyyyMMdd HHmmss"));

                    writer.Flush();
                    writer.Close();

                    AddLog(string.Format("Write licenseInfo file success on {0}", keyLockInfo.TargetDisk.DeviceID));
                }
                catch(Exception e)
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }

                    result = false;
                    errorText = string.Format("License file writing failed on {0}, Message={1}", keyLockInfo.TargetDisk.DeviceID, e.Message);
                    AddLog(errorText, true);

                    this.Progressing = progressStart + _stepCountMakingUSBKeylock;

                    keyLockInfo.IsLicenseFileCreateFailed = true;
                }
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                // 단계 4. 추가 파일 복사
                /*
                if (string.IsNullOrEmpty(this._systemSetting[SettingKey.LastAdditionalDirectory]) == false)
                {
                    try
                    {
                        sourceDirInfo = new DirectoryInfo(this._systemSetting[SettingKey.LastAdditionalDirectory]);
                        targetDirPath = string.Format(@"{0}\", letter);
                        CopyAllFiles(this._systemSetting[SettingKey.LastAdditionalDirectory], targetDirPath);
                        AddLog(string.Format("copy additional files success on {0}", keyLockInfo.TargetDisk.DeviceID));
                    }
                    catch
                    {
                        AddLog(string.Format("copy additional files fail on {0}", keyLockInfo.TargetDisk.DeviceID), true);
                    }
                }
                else
                {
                    AddLog(string.Format("copy additional files skip on {0}", keyLockInfo.TargetDisk.DeviceID), true);
                }
                */
            }

            remainingStep--;
            this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

            if (result == true)
            {
                // 단계 5. 파일 Attr 수정
                result = ModifyFileAttr(licenseFilePath);

                if (result == false)
                {
                    keyLockInfo.IsWriteProtectFailed = true;
                    AddLog(string.Format("modify attr fail on {0}", licenseFilePath), true);
                }
                else
                {
                    AddLog(string.Format("modify attr success on {0}", licenseFilePath));
                    result = ModifyFileAttr(licenseCreatedFilePath);
                }

                if (result == false)
                {
                    keyLockInfo.IsWriteProtectFailed = true;
                    AddLog(string.Format("modify attr fail on {0}", licenseCreatedFilePath), true);
                }
                else
                {
                    AddLog(string.Format("modify attr success on {0}", licenseCreatedFilePath));

                    remainingStep--;
                    this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);

                    if (keyLockInfo.IsProtect == false)
                    {
                        keyLockInfo.IsWriteProtectFailed = true;
                        AddLog(string.Format("Write Protect skip on {0}", keyLockInfo.TargetDisk.SerialNumber));
                    }
                    else
                    {
                        // 단계 6. USB Disk 쓰기 방지 설정
                        ProtectOnUSB(partitionFileNames[2]);

                        // Protect 검사
                        try
                        {
                            targetDirPath = string.Format(@"{0}\license_check", letter);

                            Directory.CreateDirectory(targetDirPath);

                            keyLockInfo.IsWriteProtectFailed = true;
                            AddLog(string.Format("Write Protect fail on {0}", keyLockInfo.TargetDisk.SerialNumber), true);
                        }
                        catch
                        {
                            keyLockInfo.IsWriteProtectFailed = false;
                            AddLog(string.Format("Write Protect success on {0}", keyLockInfo.TargetDisk.SerialNumber));
                        }
                    }

                    remainingStep--;
                    this.Progressing = progressStart + (_stepCountMakingUSBKeylock - remainingStep);
                }

            }
        }
        #endregion
        #region ClearReadonlyOnUSB
        private bool ClearReadonlyOnUSB(string filename)
        {
            bool result;
            string log;
            string output;
            string err;

            result = true;

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;                          // do not start a new shell
                p.StartInfo.RedirectStandardOutput = true;                    // Redirects the on screen results
                p.StartInfo.RedirectStandardInput = true;                     // Redirects the on screen results
                p.StartInfo.RedirectStandardError = true;                     // Redirects the on screen results
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = @"C:\Windows\System32\diskpart.exe";   // executable to run
                p.StartInfo.Arguments = string.Format("/s {0}", filename);
                p.Start();                                                    // Starts the process

                log = string.Format("Clear readonly on usb using File: {0}", filename);
                AddLog(log);

                output = string.Format("stdout: {0}", p.StandardOutput.ReadToEnd());
                WriteDebug(output);

                err = string.Format("stderr: {0}", p.StandardError.ReadToEnd());
                WriteDebug(err);

                p.WaitForExit();                                              // Waits for the exe to finish

                if (p.ExitCode != 0)
                {
                    result = false;   
                }
            }

            return result;
        }
        #endregion
        #region RepartitionOnUSB
        private bool RepartitionOnUSB(string filename)
        {
            bool result;
            string log;
            string output;
            string err;

            result = true;

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;                          // do not start a new shell
                p.StartInfo.RedirectStandardOutput = true;                    // Redirects the on screen results
                p.StartInfo.RedirectStandardInput = true;                     // Redirects the on screen results
                p.StartInfo.RedirectStandardError = true;                     // Redirects the on screen results
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = @"C:\Windows\System32\diskpart.exe";   // executable to run
                p.StartInfo.Arguments = string.Format("/s {0}", filename);
                p.Start();                                                    // Starts the process

                log = string.Format("Repartition on usb using File: {0}", filename);
                AddLog(log);

                output = string.Format("stdout: {0}", p.StandardOutput.ReadToEnd());
                WriteDebug(output);

                err = string.Format("stderr: {0}", p.StandardError.ReadToEnd());
                WriteDebug(err);

                p.WaitForExit();                                              // Waits for the exe to finish

                if (p.ExitCode != 0)
                {
                    result = false;
                }
            }
            return result;
        }
        #endregion
        #region CopyAllFiles
        private void CopyAllFiles(string sourceDir, string targetDir)
        {
            foreach (string filenpath in Directory.GetFiles(sourceDir))
            {
                File.Copy(filenpath, Path.Combine(targetDir, Path.GetFileName(filenpath)));
            }

            foreach (string dirpath in Directory.GetDirectories(sourceDir))
            {
                CopyAllFiles(dirpath, Path.Combine(targetDir, Path.GetFileName(dirpath)));
            }
                
        }
        #endregion
        #region ModifyFileAttr
        private bool ModifyFileAttr(string filename)
        {
            bool result;
            string log;
            string output;
            string err;

            result = true;

            if (File.Exists(filename) == true)
            {
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;                          // do not start a new shell
                    p.StartInfo.RedirectStandardOutput = true;                    // Redirects the on screen results
                    p.StartInfo.RedirectStandardInput = true;                     // Redirects the on screen results
                    p.StartInfo.RedirectStandardError = true;                     // Redirects the on screen results
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = @"C:\Windows\System32\attrib.exe";   // executable to run
                    p.StartInfo.Arguments = string.Format("+s +h {0} /S /D", filename);
                    p.Start();                                                    // Starts the process

                    log = string.Format("Modify Attribute on usb using File: {0}", filename);
                    AddLog(log);

                    output = string.Format("stdout: {0}", p.StandardOutput.ReadToEnd());
                    WriteDebug(output);

                    err = string.Format("stderr: {0}", p.StandardError.ReadToEnd());
                    WriteDebug(err);

                    p.WaitForExit();                                              // Waits for the exe to finish

                    if (p.ExitCode != 0)
                    {
                        result = false;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
        #endregion
        #region ProtectOnUSB
        private bool ProtectOnUSB(string filename)
        {
            bool result;
            string log;
            string output;
            string err;

            result = true;

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;                          // do not start a new shell
                p.StartInfo.RedirectStandardOutput = true;                    // Redirects the on screen results
                p.StartInfo.RedirectStandardInput = true;                     // Redirects the on screen results
                p.StartInfo.RedirectStandardError = true;                     // Redirects the on screen results
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = @"C:\Windows\System32\diskpart.exe";   // executable to run
                p.StartInfo.Arguments = string.Format("/s {0}", filename);
                p.Start();                                                    // Starts the process

                log = string.Format("Write Protect on usb using File: {0}", filename);
                AddLog(log);

                output = string.Format("stdout: {0}", p.StandardOutput.ReadToEnd());
                WriteDebug(output);

                err = string.Format("stderr: {0}", p.StandardError.ReadToEnd());
                WriteDebug(err);

                p.WaitForExit();                                              // Waits for the exe to finish

                if (p.ExitCode != 0)
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion
        #region MakePartitioningFile
        private List<string> MakePartitioningFile(int physicalDriveNum, string volumeName)
        {
            List<string> result;

            result = new List<string>();

            StreamWriter writer = null;

            string dirPath = string.Format(@"{0}\{1}", Path.GetTempPath(), "UBISAM");
            string filePath1 = string.Format(@"{0}\UUKL_diskpart_{1}_1.txt", dirPath, physicalDriveNum);
            string filePath2 = string.Format(@"{0}\UUKL_diskpart_{1}_2.txt", dirPath, physicalDriveNum);
            string filePath3 = string.Format(@"{0}\UUKL_diskpart_{1}_3.txt", dirPath, physicalDriveNum);

            try
            {
                if (File.Exists(dirPath))
                {
                    File.Delete(dirPath);
                }
                if (Directory.Exists(dirPath) == false)
                {
                    Directory.CreateDirectory(dirPath);
                }

                writer = new StreamWriter(filePath1);
                writer.WriteLine(string.Format("select disk {0}", physicalDriveNum));
                writer.WriteLine(string.Format("attributes disk clear readonly"));
                writer.WriteLine(string.Format("exit"));
                writer.Flush();
                writer.Close();

                result.Add(filePath1);

                writer = new StreamWriter(filePath2);
                writer.WriteLine(string.Format("select disk {0}", physicalDriveNum));
                writer.WriteLine(string.Format("clean"));
                writer.WriteLine(string.Format("rescan"));
                writer.WriteLine(string.Format("select disk {0}", physicalDriveNum));
                writer.WriteLine(string.Format("create partition primary"));
                writer.WriteLine(string.Format("select partition 1"));
                writer.WriteLine(string.Format("format fs=ntfs quick label='{0}'", volumeName));
                writer.WriteLine(string.Format("active"));
                writer.WriteLine(string.Format("exit"));
                writer.Flush();
                writer.Close();

                result.Add(filePath2);

                writer = new StreamWriter(filePath3);
                writer.WriteLine(string.Format("select disk {0}", physicalDriveNum));
                writer.WriteLine(string.Format("attributes disk set readonly"));
                writer.WriteLine(string.Format("exit"));
                writer.Flush();
                writer.Close();

                result.Add(filePath3);
            }
            catch
            {
                if (writer != null) writer.Close();

                if (result.Count != 0)
                {
                    result.Clear();
                }
            }

            return result;
        }
        #endregion
        #region [Log]
        private void mnuLogClear_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                this.txtLog.Document.Blocks.Clear();
            }));
        }
        private void AddLog(string message, bool warning = false)
        {
            System.Windows.Media.Brush foreground;
            string logText;

            logText = string.Format("[{0}] {1}\n", DateTime.Now.ToString("yyyyMMdd HHmmss.fff"), message);

            if (warning == true)
            {
                foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                foreground = System.Windows.Media.Brushes.White;
            }

            Dispatcher.Invoke(new Action(delegate
            {
                try
                {
                    var newData = new Run(logText, txtLog.CaretPosition.GetInsertionPosition(LogicalDirection.Forward))
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

                        txtLog.ScrollToEnd();
                    }
                }
                catch
                {
                }
            }));
        }
        #endregion
        private void WriteDebug(string message)
        {
#if DEBUG
            string log;

            log = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyyMMdd HHmmss.fff"), message);
            Debug.WriteLine(log);
#endif
        }

        private void dgrProductMaking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgrProductMaking.SelectedItem is ProductMakeInfo selectedMakeInfo)
            {
                if (ProductMakeInfo.PredefinedProducts.Contains(selectedMakeInfo.Product) == true)
                {
                    btnRemoveProduct.IsEnabled = false;
                }
                else
                {
                    btnRemoveProduct.IsEnabled = true;
                }
            }
        }

        private void AddProduct(object sender, RoutedEventArgs e)
        {
            this._productMakeInfoCollection.Add(new ProductMakeInfo(Product.KeyIn));
        }

        private void RemoveProduct(object sender, RoutedEventArgs e)
        {
            if (dgrProductMaking.SelectedItem is ProductMakeInfo selectedMakeInfo)
            {
                if (ProductMakeInfo.PredefinedProducts.Contains(selectedMakeInfo.Product) == false)
                {
                    this._productMakeInfoCollection.Remove(selectedMakeInfo);
                }
            }
        }
    }
}