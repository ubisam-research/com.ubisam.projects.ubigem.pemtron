using System.Windows;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using UbiSam.Net.KeyLock.Checker.Info;
using UbiSam.Net.KeyLock.Structure;
using System.Text;

namespace UbiSam.Net.KeyLock.Checker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region [Member Variables]
        private readonly ObservableCollection<KeyLockInfo> _availableTargetCollection;
        private readonly LicenseChecker _checker;
        private readonly System.Timers.Timer _updateLicenseProductTimer;
        #endregion
        #region [Properties]
        #endregion
        #region [Constructors]
        public MainWindow()
        {
            InitializeComponent();

            this._checker = new LicenseChecker();
            this._availableTargetCollection = new ObservableCollection<KeyLockInfo>();

            this._updateLicenseProductTimer = new System.Timers.Timer
            {
                Interval = 1000,
                AutoReset = false,
            };
            this._updateLicenseProductTimer.Elapsed += updateLicenseProductTimer_Elapsed;
            this._updateLicenseProductTimer.Start();

            this._checker.CheckActiveEvent += checker_CheckActiveEvent;

            /*
            this._checker.LicenseCheckEvent += checker_LicenseCheckEvent;
            this._checker.CheckActiveEvent += checker_CheckActiveEvent1;
            this._checker.CheckActiveEvent += checker_CheckActiveEvent2;
            */
        }
        #endregion
        #region [Timer Event Handlers]
        private void updateLicenseProductTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._updateLicenseProductTimer.Stop();
            UpdateLicensedProducts();
            this._updateLicenseProductTimer.Start();
        }

        #endregion
        #region [Window Event Hnadlers]
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this._updateLicenseProductTimer != null)
            {
                this._updateLicenseProductTimer.Enabled = false;
            }
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
            this._checker.ErrorMessageShow = ErrorMessageShowType.None;
            this._checker.USBKeyLockWarningDuration = 5 * 1000;
            this._checker.USBKeyLockWaitDuration = 10 * 1000;
            this._checker.LicenseFailWaitTimerEnabled = true;

            dgrAvailableTarget.ItemsSource = this._availableTargetCollection;
        }

        private void UpdateLicensedProducts()
        {
            StringBuilder sbLicensedProducts;

            sbLicensedProducts = new StringBuilder();

            foreach (string productCode in this._checker.GetLicensedProductCodeList())
            {
                if (string.IsNullOrEmpty(productCode) == false)
                {
                    if (sbLicensedProducts.Length != 0)
                    {
                        sbLicensedProducts.Append(",");
                    }

                    sbLicensedProducts.Append($"{productCode}");
                }
            }

            _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {

                lblLicensedProducts.Content = sbLicensedProducts.ToString();
            }));
        }
        #endregion
        #region [LicenseChecker Event Handlers]
        private void checker_CheckActiveEvent(object _, out string uniqueKey, out Product product, out bool isActive)
        {
            uniqueKey = "AAA2";
            product = Product.UbiCOM;
            isActive = true;
        }
        private void checker_CheckActiveEvent1(object _, out string uniqueKey, out Product product, out bool isActive)
        {
            uniqueKey = "BBB2";
            product = Product.UbiGEM;
            isActive = true;
        }
        private void checker_CheckActiveEvent2(object _, out string uniqueKey, out Product product, out bool isActive)
        {
            uniqueKey = "CCC2";
            product = Product.UbiGEM;
            isActive = true;
        }
        #endregion
    }
}