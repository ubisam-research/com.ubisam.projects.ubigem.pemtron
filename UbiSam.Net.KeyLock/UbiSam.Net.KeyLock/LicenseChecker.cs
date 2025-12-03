using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UbiSam.Net.KeyLock.Structure;
using UbiSam.Net.KeyLock.HardwareManager;
using UbiSam.Net.KeyLock.Utilities;
using System.Diagnostics;

namespace UbiSam.Net.KeyLock
{
    public class LicenseChecker : IDisposable
    {
        #region [Events and Delegates]
        public delegate void LicenseCheckEventHandler(object sender, string uniqueKey, Product product, LicenseResult result);
        private readonly ListEventHolder<LicenseCheckEventHandler> _licenseCheckEventHolder = new ListEventHolder<LicenseCheckEventHandler>();
        public event LicenseCheckEventHandler LicenseCheckEvent
        {
            add
            {
                this._licenseCheckEventHolder.Add(value);
            }
            remove
            {
                this._licenseCheckEventHolder.Remove(value);
            }
        }
        public delegate void LicenseCheckByCodeEventHandler(object sender, string uniqueKey, string productCode, LicenseResult result);
        private readonly ListEventHolder<LicenseCheckByCodeEventHandler> _licenseCheckEventByCodeHolder = new ListEventHolder<LicenseCheckByCodeEventHandler>();
        public event LicenseCheckByCodeEventHandler LicenseCheckByCodeEvent
        {
            add
            {
                this._licenseCheckEventByCodeHolder.Add(value);
            }
            remove
            {
                this._licenseCheckEventByCodeHolder.Remove(value);
            }
        }

        public delegate void CheckActiveEventHandler(object sender, out string uniqueKey, out Product product, out bool isActive);
        private readonly List<CheckActiveEventHandler> _checkActiveEventHandlerCollection = new List<CheckActiveEventHandler>();
        public event CheckActiveEventHandler CheckActiveEvent
        {
            add
            {
                if (this._checkActiveEventHandlerCollection != null)
                {
                    lock (this._checkActiveEventHandlerCollection)
                    {
                        if (this._checkActiveEventHandlerCollection.Contains(value) == false)
                        {
                            this._checkActiveEventHandlerCollection.Add(value);
                        }
                    }
                }
            }
            remove
            {
                if (this._checkActiveEventHandlerCollection != null)
                {
                    lock (this._checkActiveEventHandlerCollection)
                    {
                        if (this._checkActiveEventHandlerCollection.Contains(value) == true)
                        {
                            this._checkActiveEventHandlerCollection.Remove(value);
                        }
                    }
                }
            }
        }

        public delegate void CheckActiveByCodeEventHandler(object sender, out string uniqueKey, out string productCode, out bool isActive);
        private readonly List<CheckActiveByCodeEventHandler> _checkActiveByCodeEventHandlerCollection = new List<CheckActiveByCodeEventHandler>();
        public event CheckActiveByCodeEventHandler CheckActiveByCodeEvent
        {
            add
            {
                if (this._checkActiveByCodeEventHandlerCollection != null)
                {
                    lock (this._checkActiveByCodeEventHandlerCollection)
                    {
                        if (this._checkActiveByCodeEventHandlerCollection.Contains(value) == false)
                        {
                            this._checkActiveByCodeEventHandlerCollection.Add(value);
                        }
                    }
                }
            }
            remove
            {
                if (this._checkActiveByCodeEventHandlerCollection != null)
                {
                    lock (this._checkActiveByCodeEventHandlerCollection)
                    {
                        if (this._checkActiveByCodeEventHandlerCollection.Contains(value) == true)
                        {
                            this._checkActiveByCodeEventHandlerCollection.Remove(value);
                        }
                    }
                }
            }
        }
        public delegate void RequestLogging(string message);
        public event RequestLogging OnRequestLogging;

        private void RaiseLicenseCheckEvent(string uniqueKey, string productCode, LicenseResult result)
        {
            Product product = ProductConverter.ConvertToProduct(productCode);

            this._licenseCheckEventHolder.RaiseEvent(this, uniqueKey, product, result);
            this._licenseCheckEventByCodeHolder.RaiseEvent(this, uniqueKey, productCode, result);
        }
        #endregion
        #region [Shared Item Variables]
        readonly int _myActiveProductInfoCollectionStartIndex;
        #endregion
        #region [Member variables]
        private readonly UsbController _usbController = new UsbController();
        private System.Timers.Timer _licenseFailWaitTimer = new System.Timers.Timer();
        private readonly Dictionary<string, List<ProductInfo>> _licensedProductCollection;

        private MemoryMappedFile _memoryMappedFile;

        private readonly List<LicenseFailInfo> _licenseFailCollection;

        private bool _disposed;
        private readonly List<string> _removingUniqueKeyCollection;

        private static ushort LastIID = 0;
        private readonly ushort MyIID;

        private readonly Mega _mega;
        private bool _usbStateChangedRegistered;

        private string _licensedLetter;
        private bool _isTimerStoppedByRequest;

        private bool _isFirstTimerElapsed;
        private bool _lastMegaConnected;
        #endregion
        #region [Proeprties]
        public ErrorMessageShowType ErrorMessageShow { set; get; } = ErrorMessageShowType.None;
        public int USBKeyLockWaitDuration { set; get; } = 3 * 60 * 60 * 1000; // 기본값 3시간
        public int USBKeyLockWarningDuration { set; get; } = 1 * 15 * 60 * 1000; //  기본값 15분
        public bool ExitOnLicenseFail { set; get; }
        public bool LicenseFailWaitTimerEnabled
        {
            set
            {
                bool licenseFailWaitTimerEnabledBefore;

                licenseFailWaitTimerEnabledBefore = this._licenseFailWaitTimer.Enabled;

                if (value == false)
                {
                    this._licenseFailWaitTimer.Enabled = false;
                    this._isTimerStoppedByRequest = true;
                }
                else
                {
                    if (licenseFailWaitTimerEnabledBefore == false)
                    {
                        this._isTimerStoppedByRequest = false;

                        this._licenseFailWaitTimer.Enabled = true;

                        if (this._usbStateChangedRegistered == false)
                        {
                            this._usbController.StateChanged += new UsbStateChangedEventHandler(this.DoStateChanged);
                            this._usbStateChangedRegistered = true;
                        }

                        this._licenseFailCollection.Clear();
                    }
                }
            }
            get
            {
                return this._licenseFailWaitTimer.Enabled;
            }
        }
        #endregion
        #region [Constructors and Destructors]
        public LicenseChecker()
        {
            Mutex systemMutex;
            ActiveProductInfo[] activeProductInfoCollection;
            byte[] activeProductInfoRawCollection;
            int unusedPidCount;

            int tempIID;

            this._licensedLetter = string.Empty;
            this._isFirstTimerElapsed = true;

            this._isTimerStoppedByRequest = false;
            this._usbStateChangedRegistered = false;
            this._mega = Mega.Construct();

            if (this._mega != null)
            {
                this._mega.Initialize();
            }

            this._disposed = false;

            this._lastMegaConnected = false;
            this._removingUniqueKeyCollection = new List<string>();

            this._licenseFailCollection = new List<LicenseFailInfo>();

            this._licensedProductCollection = new Dictionary<string, List<ProductInfo>>();

            activeProductInfoRawCollection = new byte[ActiveProductInfoCodec.SHARED_ITEM_TOTAL_LENGTH];

            systemMutex = new Mutex(false, "6A973131-2D9E-4CD3-892B-0A4D334F55D6");
            systemMutex.WaitOne();

            tempIID = LastIID + 1;

            if (tempIID >= ushort.MaxValue)
            {
                tempIID = 1;
            }

            MyIID = (ushort)tempIID;
            LastIID = MyIID;

            // create or open _memoryMappedFile
            this._memoryMappedFile = MemoryMappedFile.CreateOrOpen("6B3FD099-99F3-4545-902A-ED4893C6D86B", ActiveProductInfoCodec.SHARED_ITEM_TOTAL_LENGTH);

            activeProductInfoCollection = null;

            using (var accessor = this._memoryMappedFile.CreateViewAccessor())
            {
                accessor.ReadArray(0, activeProductInfoRawCollection, 0, ActiveProductInfoCodec.SHARED_ITEM_TOTAL_LENGTH);
                activeProductInfoCollection = ActiveProductInfoCodec.Decode(activeProductInfoRawCollection);
            }

            this._myActiveProductInfoCollectionStartIndex = -1;

            if (activeProductInfoCollection != null)
            {
                if (activeProductInfoCollection.Length == ActiveProductInfoCodec.SHARED_ITEM_TOTAL_COUNT)
                {
                    unusedPidCount = 0;

                    for (int i = 0; i < ActiveProductInfoCodec.SHARED_ITEM_TOTAL_COUNT; i++)
                    {
                        if (i % ActiveProductInfoCodec.SHARED_ITEM_COUNT_PER_INSTANCE == 0)
                        {
                            if (unusedPidCount == 5)
                            {
                                this._myActiveProductInfoCollectionStartIndex = i - 5;
                                break;
                            }

                            unusedPidCount = 0;
                        }

                        if (activeProductInfoCollection[i].PID == 0)
                        {
                            unusedPidCount++;
                        }
                    }
                }
            }

            if (this._myActiveProductInfoCollectionStartIndex != -1)
            {
                for (int i = this._myActiveProductInfoCollectionStartIndex; i < this._myActiveProductInfoCollectionStartIndex + ActiveProductInfoCodec.SHARED_ITEM_COUNT_PER_INSTANCE; i++)
                {
                    activeProductInfoCollection[i].PID = Process.GetCurrentProcess().Id;
                    activeProductInfoCollection[i].IID = MyIID;
                }

                activeProductInfoRawCollection = ActiveProductInfoCodec.Encode(activeProductInfoCollection, this._myActiveProductInfoCollectionStartIndex, ActiveProductInfoCodec.SHARED_ITEM_COUNT_PER_INSTANCE);

                if (activeProductInfoRawCollection != null)
                {
                    using (MemoryMappedViewAccessor accessor = this._memoryMappedFile.CreateViewAccessor())
                    {
                        accessor.WriteArray(this._myActiveProductInfoCollectionStartIndex * ActiveProductInfoCodec.SHARED_ITEM_LENGTH, activeProductInfoRawCollection, 0, activeProductInfoRawCollection.Length);
                    }
                }
            }

            systemMutex.ReleaseMutex();

            this._licenseFailWaitTimer.Interval = 30 * 1000;
#if DEBUG
            this._licenseFailWaitTimer.Interval = 5 * 1000;
#endif
            this._licenseFailWaitTimer.AutoReset = true;
            this._licenseFailWaitTimer.Elapsed += LicenseFailWaitTimer_Elapsed;
            this._licenseFailWaitTimer.Enabled = false;
        }
        ~LicenseChecker() => Dispose(false);
        #endregion
        #region [Event Handlers]
        private void LicenseFailWaitTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
#if DEBUG
            this.USBKeyLockWaitDuration = 2 * 60 * 1000;
            this.USBKeyLockWarningDuration = 1 * 60 * 1000;
#endif
            this._licenseFailWaitTimer.Stop();

            UpdateLicenseWithMega();
            UpdateLicenseWithUSB();
            UpdateLicenseWithSystem();

            CheckLicense(out bool licenseSuccess);

            if (this._isFirstTimerElapsed == true)
            {
                this.OnRequestLogging?.Invoke($"License Check Result={licenseSuccess}");
                this._isFirstTimerElapsed = false;
            }

            DateTime now = DateTime.Now;

            foreach (LicenseFailInfo failInfo in this._licenseFailCollection)
            {
                if (failInfo.IsLicenseFail == true)
                {
                    if (failInfo.IsLicenseWarning == true)
                    {
                        if ((now - failInfo.FirstFailDT).TotalMilliseconds > this.USBKeyLockWarningDuration)
                        {
                            if (failInfo.IsLicenseWarningEventCalled == false)
                            {
                                failInfo.SetLicenseWarningEventCalled();
                                RaiseLicenseCheckEvent(failInfo.UniqueKey, failInfo.ProductCode, LicenseResult.Warning);
                            }
                        }
                    }
                    else
                    {
                        if ((now - failInfo.FirstFailDT).TotalMilliseconds > this.USBKeyLockWaitDuration)
                        {
                            if (failInfo.IsLicenseExpiredEventCalled == false)
                            {
                                failInfo.SetLicenseExpiredEventCalled();
                                RaiseLicenseCheckEvent(failInfo.UniqueKey, failInfo.ProductCode, LicenseResult.Expired);
                            }
                        }
                    }
                }
            }

            if (this._isTimerStoppedByRequest == false)
            {
                this._licenseFailWaitTimer.Start();
            }
        }
        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            string partitionDrive;
            string ownerDiskInfoString;

            partitionDrive = "Partition null";

            if (e.Partition != null)
            {
                partitionDrive = e.Partition.Letter;

                if (string.IsNullOrEmpty(partitionDrive) == true)
                {
                    partitionDrive = "Letter Unknown";
                }
            }

            ownerDiskInfoString = "OwnerDiskInfo null";

            if (e.Partition.OwnerDisk != null)
            {
                ownerDiskInfoString = e.Partition.OwnerDisk.DeviceID;

                if (string.IsNullOrEmpty(ownerDiskInfoString) == true)
                {
                    ownerDiskInfoString = "OwnerDiskDeviceID Unknown";
                }
            }

            this.OnRequestLogging?.Invoke($"USB {e.State}. PartitionDrive={partitionDrive}, OwnerDiskDeviceID={ownerDiskInfoString}");
        }
        #endregion
        #region [Public Methods]
        public List<Product> GetLicensedProductList()
        {
            List<Product> result;
            Product product;

            result = new List<Product>();
            

            lock (this._licensedProductCollection)
            {
                foreach (string type in this._licensedProductCollection.Keys)
                {
                    foreach (ProductInfo productInfo in this._licensedProductCollection[type])
                    {
                        product = ProductConverter.ConvertToProduct(productInfo.ProductCode);

                        if (product != Product.None && result.Contains(product) == false)
                        {
                            result.Add(product);
                        }
                    }
                }
            }

            return result;
        }
        public List<string> GetLicensedProductCodeList()
        {
            List<string> result;

            result = new List<string>();


            lock (this._licensedProductCollection)
            {
                foreach (string type in this._licensedProductCollection.Keys)
                {
                    foreach (ProductInfo productInfo in this._licensedProductCollection[type])
                    {
                        if (string.IsNullOrEmpty(productInfo.ProductCode) == false && result.Contains(productInfo.ProductCode) == false)
                        {
                            result.Add(productInfo.ProductCode);
                        }
                    }
                }
            }

            return result;
        }
        public LicenseInfo GetLicenseFromUSBKey(UsbDisk disk)
        {
            // define result first.
            LicenseInfo ret = null;

            // retrieving partition in usb disk on the machine
            if (disk == null)
            {
                ret = new LicenseInfo
                {
                    Result = LicenseResult.Unknown
                };
                return ret;
            }

            string productCode;

            var partitions = disk.Select(part => part);

            string licenseFilePath = string.Empty;
            string secretKey = string.Empty;
            string volumeName = string.Empty;
            string cipherText = string.Empty;
            string hashKey = string.Empty;
            string salt = string.Empty;
            string sha = string.Empty;

            // version 2.0 변수
            string versionKey;
            string productKey;
            string usbKey;

            string encryptedUSBKey;
            string decryptedVersion;
            string decryptedProduct;
            string[] splittedProducts;
            string tempProduct;
            string tempCount;

            string keyVersion;
            string keyType;

            string readedText = string.Empty;
            string[] readedLines;
            string[] tokens = null;

            productCode = string.Empty;

            if (partitions != null)
            {
                int partitionCount = partitions.Count();

                if (partitionCount < 1 || partitionCount > 1)
                {
                    return new LicenseInfo
                    {
                        Result = LicenseResult.Unknown
                    };
                }

                UsbPartition part = partitions.First();

                licenseFilePath = string.Format(@"{0}\license", part.Letter);

                // check license file exists
                if (File.Exists(licenseFilePath) == false)
                {
                    return new LicenseInfo
                    {
                        Result = LicenseResult.Unknown
                    };
                }

                disk = part.OwnerDisk;
                volumeName = part.Volume;

                // create secret key(identifier)
                if (disk.VID == string.Empty || disk.PID == string.Empty || disk.SerialNumber == "________________")
                {
                    return new LicenseInfo
                    {
                        Result = LicenseResult.DeviceMismatch,
                        KeyLockInfo = disk.ToString()
                    };
                }
                using (var reader = new StreamReader(licenseFilePath))
                {
                    readedText = reader.ReadToEnd();
                }

                readedLines = Regex.Split(readedText, "\r\n|\r|\n");

                if (readedLines.Length < 2)
                {
                    ret = new LicenseInfo
                    {
                        Result = LicenseResult.DeviceMismatch,
                        KeyLockInfo = disk.ToString()
                    };
                }
                else if (readedLines.Length == 2)
                {
                    #region Check Keylock V1.0
                    // V1.0 Keylock일 가능성 있음
                    secretKey = string.Format("{0}_{1}_{2}", disk.VID, disk.PID, disk.SerialNumber);

                    // split cipherText and hashKey
                    cipherText = readedLines[0];
                    hashKey = readedLines[1];

                    // make sha512 hash for confirm
                    sha = Cryptography.Encrypt9(secretKey, cipherText);

                    // descrypt cipher text
                    salt = Cryptography.Decrypt8(cipherText, volumeName);

                    // check hash key
                    if (sha == hashKey && salt.IndexOf("|") >= 0)
                    {
                        // license ok
                        tokens = salt.Split('|');

                        if (tokens.Length == 2)
                        {
                            ret = new LicenseInfo
                            {
                                Letter = part.Letter,
                                KeyLockInfo = disk.ToString(),
                                Volume = volumeName,
                                AdditionalInfo = tokens[0],
                                Identifier = secretKey
                            };

                            if (string.IsNullOrEmpty(ret.AdditionalInfo) == true)
                            {
                                productCode = "UC";

                                if (ret.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                {
                                    ret.LicensedProductCountCollection.Add(productCode, 2);
                                }

                                productCode = "UG";

                                if (ret.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                {
                                    ret.LicensedProductCountCollection.Add(productCode, 1);
                                }
                            }
                            else
                            {
                                if (ret.AdditionalInfo == Product.UbiCOM.ToString())
                                {
                                    productCode = "UC";

                                    if (ret.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                    {
                                        ret.LicensedProductCountCollection.Add(productCode, 2);
                                    }
                                }
                                else if (ret.AdditionalInfo == Product.UbiGEM.ToString())
                                {
                                    productCode = "UC";

                                    if (ret.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                    {
                                        ret.LicensedProductCountCollection.Add(productCode, 1);
                                    }
                                }
                                else if (ret.AdditionalInfo == $"{Product.UbiCOM}_{Product.UbiGEM}")
                                {
                                    productCode = "UC";

                                    if (ret.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                    {
                                        ret.LicensedProductCountCollection.Add(productCode, 2);
                                    }

                                    productCode = "UG";

                                    if (ret.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                    {
                                        ret.LicensedProductCountCollection.Add(productCode, 1);
                                    }
                                }
                            }

                            if (ret.LicensedProductCountCollection.Count > 0)
                            {
                                ret.Result = LicenseResult.LicenseOk;
                            }
                        }
                        else
                        {
                            // license key mismatch
                            ret = new LicenseInfo
                            {
                                Result = LicenseResult.KeyMismatch,
                                KeyLockInfo = disk.ToString(),
                                Volume = volumeName
                            };
                        }
                    }
                    else
                    {
                        // license key mismatch
                        ret = new LicenseInfo
                        {
                            Result = LicenseResult.KeyMismatch,
                            KeyLockInfo = disk.ToString(),
                            Volume = volumeName
                        };
                    }
                    #endregion
                }
                else if (readedLines.Length == 3)
                {
                    // V2.0 이상 keylock 일 가능성 있음
                    #region Check Keylock V2.0
                    secretKey = string.Format("{0}_{1}_{2}", disk.VID, disk.PID, disk.SerialNumber);

                    // split cipherText and hashKey
                    versionKey = readedLines[0];
                    productKey = readedLines[1];
                    usbKey = readedLines[2];

                    // decrypt productKey
                    decryptedVersion = Cryptography.Decrypt3(versionKey, volumeName);

                    keyVersion = string.Empty;
                    keyType = string.Empty;

                    if (decryptedVersion.Count(t => t == '|') == 1)
                    {
                        keyVersion = decryptedVersion.Substring(0, decryptedVersion.IndexOf('|'));
                        keyType = decryptedVersion.Substring(decryptedVersion.IndexOf('|') + 1);
                    }

                    // check hash key
                    if (keyVersion == "V2" && keyType == "USB")
                    {
                        #region KeyLock V2 USB Type Check
                        // make sha512 hash for confirm
                        encryptedUSBKey = Cryptography.Encrypt4(secretKey, versionKey);

                        if (usbKey != encryptedUSBKey)
                        {
                            // license key mismatch
                            ret = new LicenseInfo
                            {
                                Result = LicenseResult.KeyMismatch,
                                KeyLockInfo = disk.ToString(),
                                Volume = volumeName
                            };
                        }
                        else
                        {
                            decryptedProduct = Cryptography.Decrypt3(productKey, versionKey);
                            splittedProducts = decryptedProduct.Split(',');

                            ret = new LicenseInfo
                            {
                                Letter = part.Letter,
                                KeyLockInfo = disk.ToString(),
                                Volume = volumeName,
                                Identifier = secretKey
                            };

                            foreach (string productCountPair in splittedProducts)
                            {
                                if (productCountPair.Count(t => t == '|') == 1)
                                {
                                    tempProduct = productCountPair.Substring(0, productCountPair.IndexOf('|'));
                                    tempCount = productCountPair.Substring(productCountPair.IndexOf('|') + 1);

                                    if (Enum.TryParse(tempProduct, out Product product) == true && int.TryParse(tempCount, out int productCount) == true)
                                    {
                                        productCode = ProductConverter.Convert(product);

                                        if (string.IsNullOrEmpty(productCode) == false)
                                        {
                                            if (ret.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                            {
                                                ret.LicensedProductCountCollection.Add(productCode, productCount);
                                            }
                                            else
                                            {
                                                ret.LicensedProductCountCollection[productCode] += productCount;
                                            }
                                        }
                                    }
                                }
                            }

                            if (ret.LicensedProductCountCollection.Count > 0)
                            {
                                ret.Result = LicenseResult.LicenseOk;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        // license key mismatch
                        ret = new LicenseInfo
                        {
                            Result = LicenseResult.KeyMismatch,
                            KeyLockInfo = disk.ToString(),
                            Volume = volumeName
                        };
                    }
                    #endregion
                }
            }


            // if ret is null, license device(keylock) mismatch
            if (ret == null)
            {
                ret = new LicenseInfo
                {
                    Result = LicenseResult.DeviceMismatch,
                    KeyLockInfo = disk.ToString()
                };
            }

            return ret;
        }
        public void RemoveUniqueKey(string uniqueKey)
        {
            lock (this)
            {
                this._removingUniqueKeyCollection.Add(uniqueKey);
            }
        }
        public List<string> GetSerialNumbers()
        {
            List<string> result;
            LicenseInfo licenseInfo;

            result = new List<string>();

            if (this._mega.Initialized == true && this._mega.Connected == false)
            {
                this._mega.Connect();
            }

            if (this._mega.Connected == true)
            {
                result.Add(this._mega.SerialNumberAsString);
            }

            foreach (UsbDisk disk in this._usbController.ExistsDisks)
            {
                licenseInfo = GetLicenseFromUSBKey(disk);

                if (licenseInfo.Result == LicenseResult.LicenseOk)
                {
                    result.Add(licenseInfo.Identifier);
                }
            }

            return result;
        }
        #endregion
        #region [Private methods]
        private void UpdateLicenseWithUSB()
        {
            LicenseInfo licenseInfo;

            licenseInfo = GetLicensedInfoWithUSB();

            if (licenseInfo == null)
            {
                this._usbController.ReadAvailableDisks();

                foreach (UsbDisk disk in this._usbController.ExistsDisks)
                {
                    try
                    {
                        licenseInfo = GetLicenseFromUSBKey(disk);

                        if (licenseInfo != null && licenseInfo.Result == LicenseResult.LicenseOk)
                        {
                            break;
                        }
                    }
                    catch { }
                }
            }

            if (licenseInfo != null && licenseInfo.Result == LicenseResult.LicenseOk)
            {
                this._licensedLetter = licenseInfo.Letter;

                lock (this._licensedProductCollection)
                {
                    if (this._licensedProductCollection.ContainsKey("USB") == false)
                    {
                        this._licensedProductCollection.Add("USB", new List<ProductInfo>());
                    }

                    if (this._licensedProductCollection["USB"] == null)
                    {
                        this._licensedProductCollection["USB"] = new List<ProductInfo>();
                    }

                    foreach (string key in licenseInfo.LicensedProductCountCollection.Keys)
                    {
                        ProductInfo productInfo = this._licensedProductCollection["USB"].FirstOrDefault(t => t.ProductCode == key);

                        if (productInfo == null)
                        {
                            productInfo = new ProductInfo()
                            {
                                ProductCode = key,
                                Count = licenseInfo.LicensedProductCountCollection[key],
                            };

                            this._licensedProductCollection["USB"].Add(productInfo);
                        }
                        else
                        {
                            productInfo.Count += licenseInfo.LicensedProductCountCollection[key];
                        }
                    }
                }
            }
            else
            {
                this._licensedLetter = string.Empty;

                lock (this._licensedProductCollection)
                {
                    if (this._licensedProductCollection.ContainsKey("USB") == true)
                    {
                        this._licensedProductCollection.Remove("USB");
                    }
                }
            }
        }
        private LicenseInfo GetLicensedInfoWithUSB()
        {
            LicenseInfo licenseInfo;
            UsbDisk disk;

            licenseInfo = null;
            disk = null;

            if (string.IsNullOrEmpty(this._licensedLetter) == false)
            {
                disk = (from UsbDisk usbDisk in _usbController.ExistsDisks
                        from UsbPartition usbPartition in usbDisk
                        where usbPartition.Letter == this._licensedLetter
                        select usbDisk).FirstOrDefault();
            }

            if (disk != null)
            {
                licenseInfo = GetLicenseFromUSBKey(disk);

                if (licenseInfo != null && licenseInfo.Result != LicenseResult.LicenseOk)
                {
                    licenseInfo = null;
                }
            }

            return licenseInfo;
        }
        private void UpdateLicenseWithSystem()
        {
            LicenseInfo licenseInfo;

            string systemLicensePath;
            string readedText;
            string[] readedLines;

            string secretKey;
            string versionKey;
            string productKey;
            List<string> systemKeys;

            string decryptedVersion;
            string keyVersion;
            string keyType;
            string decryptedProduct;
            string[] splittedProducts;
            string tempProduct;
            string tempCount;

            string encryptedSystemKey;
            string productCode;

            systemLicensePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\UbiSam\UbiSam.Net.Keylock.license";

            licenseInfo = null;

            if (File.Exists(systemLicensePath) == true)
            {
                using (var reader = new StreamReader(systemLicensePath))
                {
                    readedText = reader.ReadToEnd();
                }

                readedLines = Regex.Split(readedText, "\r\n|\r|\n");

                if (readedLines.Length >= 3)
                {
                    // Keylock V2 이상일 가능성 있음
                    secretKey = $"{SystemController.CPU_ID}|{SystemController.MB_ID}|{SystemController.BIOS_ID}|{SystemController.Vendor}|{SystemController.IdentifyingNumber}|{SystemController.UUID}";

                    // split cipherText and hashKey
                    versionKey = readedLines[0];
                    productKey = readedLines[1];
                    systemKeys = new List<string>();

                    for (int i = 2; i < readedLines.Length; i++)
                    {
                        systemKeys.Add(readedLines[i]);
                    }

                    // decrypt productKey
                    decryptedVersion = Cryptography.Decrypt3(versionKey, "UUKL");

                    keyVersion = string.Empty;
                    keyType = string.Empty;

                    if (decryptedVersion.Count(t => t == '|') == 1)
                    {
                        keyVersion = decryptedVersion.Substring(0, decryptedVersion.IndexOf('|'));
                        keyType = decryptedVersion.Substring(decryptedVersion.IndexOf('|') + 1);
                    }

                    // check hash key
                    if (keyVersion == "V2" && keyType == "SYSTEM")
                    {
                        #region KeyLock V2 System Type Check
                        // make sha512 hash for confirm
                        encryptedSystemKey = Cryptography.Encrypt4(secretKey, versionKey);

                        if (systemKeys.Contains(encryptedSystemKey) == true)
                        {
                            decryptedProduct = Cryptography.Decrypt3(productKey, versionKey);
                            splittedProducts = decryptedProduct.Split(',');

                            licenseInfo = new LicenseInfo
                            {
                                Identifier = secretKey
                            };

                            foreach (string productCountPair in splittedProducts)
                            {
                                if (productCountPair.Count(t => t == '|') == 1)
                                {
                                    tempProduct = productCountPair.Substring(0, productCountPair.IndexOf('|'));
                                    tempCount = productCountPair.Substring(productCountPair.IndexOf('|') + 1);

                                    if (Enum.TryParse(tempProduct, out Product product) == true && int.TryParse(tempCount, out int productCount) == true)
                                    {
                                        productCode = ProductConverter.Convert(product);

                                        if (string.IsNullOrEmpty(productCode) == false)
                                        {
                                            if (licenseInfo.LicensedProductCountCollection.ContainsKey(productCode) == false)
                                            {
                                                licenseInfo.LicensedProductCountCollection[productCode] = productCount;
                                            }
                                            else
                                            {
                                                licenseInfo.LicensedProductCountCollection[productCode] += productCount;
                                            }
                                        }
                                    }
                                }
                            }

                            if (licenseInfo.LicensedProductCountCollection.Count > 0)
                            {
                                licenseInfo.Result = LicenseResult.LicenseOk;
                            }
                        }
                        #endregion
                    }
                }
            }

            if (licenseInfo != null && licenseInfo.Result == LicenseResult.LicenseOk)
            {
                lock (this._licensedProductCollection)
                {
                    if (this._licensedProductCollection.ContainsKey("System") == false)
                    {
                        this._licensedProductCollection.Add("System", new List<ProductInfo>());
                    }

                    if (this._licensedProductCollection["System"] == null)
                    {
                        this._licensedProductCollection["System"] = new List<ProductInfo>();
                    }

                    foreach (string key in licenseInfo.LicensedProductCountCollection.Keys)
                    {
                        ProductInfo productInfo = this._licensedProductCollection["System"].FirstOrDefault(t => t.ProductCode == key);

                        if (productInfo == null)
                        {
                            productInfo = new ProductInfo()
                            {
                                ProductCode = key,
                                Count = licenseInfo.LicensedProductCountCollection[key],
                            };

                            this._licensedProductCollection["System"].Add(productInfo);
                        }
                        else
                        {
                            productInfo.Count += licenseInfo.LicensedProductCountCollection[key];
                        }
                    }
                }
            }
        }
        private void CheckLicense(out bool licenseSuccess)
        {
            ProductInfo productInfo;
            string uniqueKey;
            Product product;
            string productCode;
            bool isActive;

            byte[] activeProductRawCollection;
            ActiveProductInfo[] globalActiveProductCollection;

            ActiveProductCollection localActiveProductCollection;
            ActiveProductInfo tempActiveProductInfo;
            Dictionary<string, List<string>> usedProductCollection;
            List<ProductInfo> licensedProductCollection;

            List<ActiveProductInfo> removingActiveProductInfo;

            ProductInfo licensedProductInfo;
            LicenseFailInfo failInfo;

            // removingUniqueKey
            List<string> removingUniqueKeyCollection;
            DateTime now;

            licenseSuccess = false;

            now = DateTime.Now;

            removingUniqueKeyCollection = new List<string>();

            if (this._removingUniqueKeyCollection.Count > 0)
            {
                lock (this)
                {
                    removingUniqueKeyCollection.AddRange(this._removingUniqueKeyCollection);
                    this._removingUniqueKeyCollection.Clear();
                }
            }
            // 허용된 Product Count 갱신
            licensedProductCollection = new List<ProductInfo>();

            lock (this._licensedProductCollection)
            {
                if (this._licensedProductCollection.Count > 0 && this._licensedProductCollection.Values.Sum(t => t.Count) > 0)
                {
                    licenseSuccess = true;
                }

                foreach (var licensedProductList in this._licensedProductCollection.Values)
                {
                    foreach (ProductInfo tempProductInfo in licensedProductList)
                    {
                        productInfo = licensedProductCollection.FirstOrDefault(t => t.ProductCode == tempProductInfo.ProductCode);

                        if (productInfo == null)
                        {
                            productInfo = new ProductInfo()
                            {
                                ProductCode = tempProductInfo.ProductCode,
                                Count = tempProductInfo.Count,
                            };

                            licensedProductCollection.Add(productInfo);
                        }
                        else
                        {
                            productInfo.Count += tempProductInfo.Count;
                        }
                    }
                }
            }

            localActiveProductCollection = new ActiveProductCollection();

            // localActiveProduct Update
            if (this._checkActiveEventHandlerCollection != null)
            {
                List<CheckActiveEventHandler> list;

                lock (this._checkActiveEventHandlerCollection)
                {
                    list = new List<CheckActiveEventHandler>(this._checkActiveEventHandlerCollection);

                }
                List<CheckActiveEventHandler> rm = new List<CheckActiveEventHandler>();

                foreach (CheckActiveEventHandler handler in list)
                {
                    uniqueKey = string.Empty;
                    product = Product.None;
                    isActive = false;

                    if (handler != null)
                    {
                        try
                        {
                            handler.Invoke(this, out uniqueKey, out product, out isActive);
                        }
                        catch 
                        {
                            rm.Add(handler);
                        }

                        if (string.IsNullOrEmpty(uniqueKey) == false && product != Product.None)
                        {
                            if (removingUniqueKeyCollection.Contains(uniqueKey) == false)
                            {
                                if (isActive == true)
                                {
                                    tempActiveProductInfo = localActiveProductCollection[uniqueKey];

                                    if (tempActiveProductInfo == null)
                                    {
                                        tempActiveProductInfo = new ActiveProductInfo(uniqueKey, product)
                                        {
                                            PID = Process.GetCurrentProcess().Id,
                                            IID = MyIID
                                        };

                                        localActiveProductCollection.Add(tempActiveProductInfo);
                                    }
                                }
                            }
                            else
                            {
                                if (rm.Contains(handler) == false)
                                {
                                    rm.Add(handler);
                                }
                            }
                        }
                    }
                }

                if (rm.Count > 0)
                {
                    lock (this._checkActiveEventHandlerCollection)
                    {
                        foreach (var handler in rm)
                        {
                            this._checkActiveEventHandlerCollection.Remove(handler);
                        }
                    }
                }
            }

            if (this._checkActiveByCodeEventHandlerCollection != null)
            {
                List<CheckActiveByCodeEventHandler> list;
                List<CheckActiveByCodeEventHandler> rm = new List<CheckActiveByCodeEventHandler>();

                lock (this._checkActiveByCodeEventHandlerCollection)
                {
                    list = new List<CheckActiveByCodeEventHandler>(this._checkActiveByCodeEventHandlerCollection);
                }

                foreach (CheckActiveByCodeEventHandler handler in list)
                {
                    uniqueKey = string.Empty;
                    productCode = string.Empty;
                    isActive = false;

                    if (handler != null)
                    {
                        try
                        {
                            handler.Invoke(this, out uniqueKey, out productCode, out isActive);

                        }
                        catch
                        {
                            rm.Add(handler);
                        }

                        if (string.IsNullOrEmpty(uniqueKey) == false && string.IsNullOrEmpty(productCode) == false)
                        {
                            if (removingUniqueKeyCollection.Contains(uniqueKey) == false)
                            {
                                if (isActive == true)
                                {
                                    tempActiveProductInfo = localActiveProductCollection[uniqueKey];

                                    if (tempActiveProductInfo == null)
                                    {
                                        tempActiveProductInfo = new ActiveProductInfo(uniqueKey, productCode)
                                        {
                                            PID = Process.GetCurrentProcess().Id,
                                            IID = MyIID
                                        };

                                        localActiveProductCollection.Add(tempActiveProductInfo);
                                    }
                                }
                            }
                            else
                            {
                                if (rm.Contains(handler) == false)
                                {
                                    rm.Add(handler);
                                }
                            }
                        }
                    }
                }

                if (rm.Count > 0)
                {
                    lock (this._checkActiveByCodeEventHandlerCollection)
                    {
                        foreach (CheckActiveByCodeEventHandler handler in rm)
                        {
                            this._checkActiveByCodeEventHandlerCollection.Remove(handler);
                        }
                    }
                }
            }

            globalActiveProductCollection = null;

            if (this._memoryMappedFile != null)
            {
                //**// Update Active Product Collection from shared memory
                activeProductRawCollection = new byte[ActiveProductInfoCodec.SHARED_ITEM_TOTAL_LENGTH];

                using (var accessor = this._memoryMappedFile.CreateViewAccessor())
                {
                    accessor.ReadArray(0, activeProductRawCollection, 0, ActiveProductInfoCodec.SHARED_ITEM_TOTAL_LENGTH);
                    globalActiveProductCollection = ActiveProductInfoCodec.Decode(activeProductRawCollection);
                }
            }

            // GlobalActiveProductCollection LocalActiveProductCollection 정리
            if (globalActiveProductCollection != null)
            {
                for (int i = 0, n = localActiveProductCollection.Count; i < n; i++)
                {
                    tempActiveProductInfo = globalActiveProductCollection.FirstOrDefault(t => t.UniqueKey == localActiveProductCollection[i].UniqueKey);

                    if (tempActiveProductInfo == null)
                    {
                        globalActiveProductCollection[this._myActiveProductInfoCollectionStartIndex + i].UniqueKey = localActiveProductCollection[i].UniqueKey;
                        globalActiveProductCollection[this._myActiveProductInfoCollectionStartIndex + i].ProductCode = localActiveProductCollection[i].ProductCode;
                        globalActiveProductCollection[this._myActiveProductInfoCollectionStartIndex + i].UpdateLastDT();
                    }
                    else
                    {
                        tempActiveProductInfo.UpdateLastDT();
                    }
                }
            }

            removingActiveProductInfo = new List<ActiveProductInfo>();
            removingActiveProductInfo.AddRange(globalActiveProductCollection.Where(t => (now - t.LastDT).TotalMinutes > 5));

            if (removingUniqueKeyCollection.Count > 0)
            {
                foreach (var item in removingUniqueKeyCollection)
                {
                    if (removingActiveProductInfo.FirstOrDefault(t => t.UniqueKey == item) == null)
                    {
                        tempActiveProductInfo = globalActiveProductCollection.FirstOrDefault(t => t.UniqueKey == item);

                        if (tempActiveProductInfo != null)
                        {
                            removingActiveProductInfo.Add(tempActiveProductInfo);
                        }
                    }
                }
            }

            if (this._memoryMappedFile != null)
            {
                // 갱신된 active product collection write
                using (var accessor = this._memoryMappedFile.CreateViewAccessor())
                {
                    for (int i = 0; i < ActiveProductInfoCodec.SHARED_ITEM_TOTAL_COUNT; i++)
                    {
                        int start = i * ActiveProductInfoCodec.SHARED_ITEM_LENGTH;
                        tempActiveProductInfo = globalActiveProductCollection[i];

                        if (i >= this._myActiveProductInfoCollectionStartIndex && i < this._myActiveProductInfoCollectionStartIndex + 5)
                        {
                            accessor.WriteArray(start, ActiveProductInfoCodec.Encode(tempActiveProductInfo), 0, ActiveProductInfoCodec.SHARED_ITEM_LENGTH);
                        }
                        else
                        {
                            if (removingActiveProductInfo.Find(t => t.UniqueKey == tempActiveProductInfo.UniqueKey) != null)
                            {
                                tempActiveProductInfo.PID = 0;
                                tempActiveProductInfo.IID = 0;
                                tempActiveProductInfo.UniqueKey = string.Empty;
                                tempActiveProductInfo.ProductCode = string.Empty;

                                accessor.WriteArray(start, ActiveProductInfoCodec.Encode(tempActiveProductInfo), 0, ActiveProductInfoCodec.SHARED_ITEM_LENGTH);
                            }
                        }
                    }
                }
            }

            // 사용중 Product Count 갱신
            usedProductCollection = new Dictionary<string, List<string>>();

            foreach (ActiveProductInfo info in globalActiveProductCollection)
            {
                if (usedProductCollection.ContainsKey(info.ProductCode) == false)
                {
                    usedProductCollection.Add(info.ProductCode, new List<string>());
                }

                if (usedProductCollection[info.ProductCode] == null)
                {
                    usedProductCollection[info.ProductCode] = new List<string>();
                }

                if (usedProductCollection[info.ProductCode].Contains(info.UniqueKey) == false)
                {
                    usedProductCollection[info.ProductCode].Add(info.UniqueKey);
                }
            }

            // 사용중 Product Count와 허용된 Product Count 비교
            foreach (KeyValuePair<string, List<string>> usedProduct in usedProductCollection.Where(t => t.Key != string.Empty))
            {
                licensedProductInfo = licensedProductCollection.FirstOrDefault(t => t.ProductCode == usedProduct.Key);

                if (licensedProductInfo == null)
                {
                    foreach (var usedLicenseUniqueKey in usedProduct.Value)
                    {
                        failInfo = this._licenseFailCollection.FirstOrDefault(t => t.UniqueKey == usedLicenseUniqueKey);

                        if (failInfo == null)
                        {
                            failInfo = new LicenseFailInfo()
                            {
                                UniqueKey = usedLicenseUniqueKey,
                                FirstFailDT = DateTime.Now,
                                ProductCode = usedProduct.Key,
                                IsLicenseFail = true,
                            };

                            this._licenseFailCollection.Add(failInfo);
                        }

                        if (failInfo.IsLicenseFail == false)
                        {
                            failInfo.Reset(true);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < usedProduct.Value.Count; i++)
                    {
                        var usedLicenseUniqueKey = usedProduct.Value[i];

                        failInfo = this._licenseFailCollection.FirstOrDefault(t => t.UniqueKey == usedLicenseUniqueKey);

                        if (failInfo == null)
                        {
                            failInfo = new LicenseFailInfo()
                            {
                                UniqueKey = usedLicenseUniqueKey,
                                FirstFailDT = DateTime.Now,
                                ProductCode = usedProduct.Key,
                                IsLicenseFail = true,
                            };
                            this._licenseFailCollection.Add(failInfo);
                        }

                        if (i < licensedProductInfo.Count)
                        {
                            if (failInfo.IsLicenseFail == true)
                            {
                                failInfo.Reset(false);
                            }
                        }
                        else
                        {
                            if (failInfo.IsLicenseFail == false)
                            {
                                failInfo.Reset(true);
                            }
                        }
                    }
                }

            }
        }
        private void UpdateLicenseWithMega()
        {
            string licensedString;
            List<ProductInfo> productCollection;

            bool connected;

            if (this._mega != null)
            {
                connected = this._mega.Connected;

                if (connected == false)
                {
                    connected = this._mega.Connect();
                }
                else
                {
                    this._mega.Check();
                    connected = this._mega.Connected;
                }

                if (connected == false)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        Thread.Sleep(new Random().Next(1000) + 1000);

                        connected = this._mega.Connect();

                        if (connected == true)
                        {
                            break;
                        }
                    }
                }

                licensedString = this._mega.ProductCode;
                productCollection = ProductConverter.ConvertMega(licensedString);

                if (this._lastMegaConnected != connected)
                {
                    this.OnRequestLogging?.Invoke($"Mega Connection Changed. State={connected}");
                    this._lastMegaConnected = connected;
                }

                lock (this._licensedProductCollection)
                {
                    if (this._licensedProductCollection.ContainsKey("MEGA") == false)
                    {
                        this._licensedProductCollection["MEGA"] = productCollection;
                        this.OnRequestLogging?.Invoke($"Mega Changed. LicensedProduct = {licensedString}");
                    }
                    else
                    {
                        if (EqualsProductInfoList(this._licensedProductCollection["MEGA"], productCollection) == false)
                        {
                            _ = this._licensedProductCollection.Remove("MEGA");

                            this._licensedProductCollection["MEGA"] = productCollection;
                            this.OnRequestLogging?.Invoke($"Mega Changed. LicensedProduct = {licensedString}");
                        }
                    }
                }
            }
        }
        private bool EqualsProductInfoList(List<ProductInfo> list1, List<ProductInfo> list2)
        {
            bool notEquals;

            notEquals = false;

            if (list1 == null && list2 == null)
            {
                notEquals = false;
            }
            else if (list1 != null && list2 != null)
            {
                notEquals |= list1.Count != list2.Count;

                if (notEquals == false)
                {
                    foreach (ProductInfo productInfo1 in list1)
                    {
                        if (notEquals == true)
                        {
                            break;
                        }
                        ProductInfo productInfo2 = list2.FirstOrDefault(t => t.ProductCode == productInfo1.ProductCode);

                        if (productInfo2 != null)
                        {
                            notEquals |= productInfo1.Count != productInfo2.Count;
                        }
                        else
                        {
                            notEquals |= true;
                        }
                    }
                }
            }
            else
            {
                notEquals |= true;
            }

            return notEquals == false;
        }
        #endregion
        #region [Dispose Methods]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                this._disposed = true;

                this._isTimerStoppedByRequest = true;

                if (disposing == true)
                {
                    if (this._licenseFailWaitTimer != null)
                    {
                        this._licenseFailWaitTimer.Stop();
                        this._licenseFailWaitTimer = null;
                    }
                }

                if (this._memoryMappedFile != null)
                {
                    this._memoryMappedFile.Dispose();
                    this._memoryMappedFile = null;
                }
            }
        }
        #endregion
    }
}
