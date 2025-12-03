using System.Windows;
using System;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UbiSam.Net.KeyLock.Structure;
using System.Text;

namespace UbiSam.Net.KeyLock.Checker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LicenseCheckWindow : Window
    {
        #region [Member Variables]
        private readonly LicenseChecker _checker;
        private readonly Guid _guid;
        #endregion
        #region [Properties]
        #endregion
        #region [Constructors]
        public LicenseCheckWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            this._guid = Guid.NewGuid();
            Title = $"UBISAM Keylock Checker V2.2.0 PID={Process.GetCurrentProcess().Id}";
            this._checker = new LicenseChecker();

            _ = new Timer(this.UpdateLicensedProducts).Change(100, 1000);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DateTime now = DateTime.Now;
            FileInfo file;

            string shutDownLogOutputPath;

            shutDownLogOutputPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\UbiSam\KeyLock\Shutdown_KeyLock_{Process.GetCurrentProcess().Id}_{now:yyyyMMdd_HHmmss.fff}.log";

            file = new FileInfo(shutDownLogOutputPath);

            if (file.Directory.Exists == false)
            {
                file.Directory.Create();
            }

            File.AppendAllText(shutDownLogOutputPath, e.ToString());
        }
        #endregion
        #region [Window Event Hnadlers]
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
        #endregion

        #region [Button Event Handlers]
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        #region [Private Methods]
        private void Initialize()
        {
            // define interval license checking
            this._checker.ErrorMessageShow = ErrorMessageShowType.None;
            this._checker.USBKeyLockWarningDuration = 10 * 1000;
            this._checker.USBKeyLockWaitDuration = 20 * 1000;
            this._checker.LicenseFailWaitTimerEnabled = true;

            this._checker.LicenseCheckEvent += checker_LicenseCheckEvent;
            this._checker.CheckActiveEvent += checker_CheckActiveEvent;
            this._checker.CheckActiveByCodeEvent += checker_CheckActiveByCodeEvent;
        }

        private void UpdateLicensedProducts(object o)
        {
            StringBuilder sbLicensedProducts;

            sbLicensedProducts = new StringBuilder();

            List<string> serialNumbers = this._checker.GetSerialNumbers();

            foreach (string productCode in this._checker.GetLicensedProductCodeList())
            {
                if (sbLicensedProducts.Length != 0)
                {
                    sbLicensedProducts.Append(",");
                }

                sbLicensedProducts.Append($"{productCode}");
            }

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(()=>
            {
                dgrSerialNumbers.ItemsSource = serialNumbers;
                lblLicensedProducts.Content = sbLicensedProducts.ToString();

                if (sbLicensedProducts.Length == 0)
                {
                    lblVerifyResult.Content = "License Verify Fail.";
                }
                else
                {
                    lblVerifyResult.Content = "License OK.";
                }

            }));
        }
        #endregion
        #region [LicenseChecker Event Handlers]
        private void checker_LicenseCheckEvent(object sender, string uniqueKey, Product product, LicenseResult result)
        {
        }
        private void checker_CheckActiveEvent(object sender, out string uniqueKey, out Product product, out bool isActive)
        {
            uniqueKey = this._guid.ToString();
            product = Product.UbiCOM;
            isActive = true;
        }
        private void checker_CheckActiveByCodeEvent(object sender, out string uniqueKey, out string productCode, out bool isActive)
        {
            uniqueKey = "ABCD";
            productCode = "AB";
            isActive = true;
        }
        #endregion
    }
}