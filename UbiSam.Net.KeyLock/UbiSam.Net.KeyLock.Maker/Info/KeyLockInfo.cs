using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiSam.Net.KeyLock.HardwareManager;
using System.IO;
using UbiSam.Net.KeyLock.Structure;

namespace UbiSam.Net.KeyLock.Maker.Info
{
    public class KeyLockInfo : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Const String
        public const string VolumeName = "UUKL";
        #endregion
        #region MemberVariable
        private bool _isTarget;
        private bool _isProtect;
        private readonly List<string> _letters;
        private UsbDisk _targetDisk;

        private LicenseInfo _licenseInfo;
        private bool _licenseFileCreateFailed;
        private bool _writeProtectFailed;
        #endregion
        #region Property
        public LockType LockType
        {
            get; set;
        }
        public LicenseInfo LicenseInfo
        {
            get
            {
                return this._licenseInfo;
            }
            set
            {
                if (this._licenseInfo != value)
                {
                    this._licenseInfo = value;
                    NotifyPropertyChanged("_licenseInfo");
                }
            }
        }
        public bool UseExpire { set; get; }
        public string ExpireDate { set; get; }

        public UsbDisk TargetDisk
        {
            set
            {
                this._targetDisk = value;

                if (value != null)
                {
                    this._letters.Clear();
                    
                    foreach (UsbPartition partition in this._targetDisk)
                    {
                        this._letters.Add(partition.Letter);
                    }
                }
            }
            get
            {
                return this._targetDisk;
            }
        }
        public string Reason
        {
            get
            {
                string result;

                result = string.Empty;

                if (this.TargetDisk != null)
                {
                    if (this.TargetDisk.VID == string.Empty || this.TargetDisk.PID == string.Empty)
                    {
                        result = "target device can not make usb keylock";
                    }
                    else if (this.TargetDisk.SerialNumber == "________________")
                    {
                        result = "target device can not make usb keylock";
                    }
                }
                else
                {
                    result = "target disk is not set";
                }
                return result;
            }
        }
        public bool IsCandidate
        {
            get
            {
                bool result;

                if (this.LockType == LockType.MegaLock)
                {
                    result = true;
                }
                else
                {
                    if (this.TargetDisk != null)
                    {
                        if (this.TargetDisk.VID == string.Empty || this.TargetDisk.PID == string.Empty)
                        {
                            this.IsTarget = false;
                            this.IsProtect = false;
                            result = false;
                        }
                        else if (this.TargetDisk.SerialNumber == "________________")
                        {
                            this.IsTarget = false;
                            this.IsProtect = false;
                            result = false;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }

                return result;
            }
        }
        public bool IsTarget
        {
            get
            {
                return this._isTarget;
            }
            set
            {
                if (this._isTarget != value)
                {
                    this._isTarget = value;
                    NotifyPropertyChanged("IsTarget");
                }
            }
        }
        public bool IsProtect
        {
            get
            {
                return this._isProtect;
            }
            set
            {
                if (this._isProtect != value)
                {
                    this._isProtect = value;
                    NotifyPropertyChanged("IsProtect");
                }
            }
        }
        public bool IsLicenseFileCreateFailed
        {
            get { return this._licenseFileCreateFailed; }
            set
            {
                if (this._licenseFileCreateFailed != value)
                {
                    this._licenseFileCreateFailed = value;
                    NotifyPropertyChanged("IsLicenseFileCreateFailed");
                    NotifyPropertyChanged("NeedWriteProtect");
                }
            }
        }
        public bool IsWriteProtectFailed
        {
            get { return this._writeProtectFailed; }
            set
            {
                if (this._writeProtectFailed != value)
                {
                    this._writeProtectFailed = value;
                    NotifyPropertyChanged("IsWriteProtectFailed");
                    NotifyPropertyChanged("NeedWriteProtect");
                }
            }
        }
        public bool NeedWriteProtect
        {
            get
            {
                return this._licenseFileCreateFailed == false && this._writeProtectFailed == true;
            }
        }
        #endregion
        #region Constructor
        public KeyLockInfo()
        {
            this.LockType = LockType.Normal;
            this._targetDisk = null;
            this._licenseInfo = null;
            this._isTarget = true;
            this._isProtect = true;
            this.UseExpire = false;
            this.ExpireDate = "0000-00-00";
            this._letters = new List<string>();
            this._licenseFileCreateFailed = false;
            this._writeProtectFailed = false;
        }
        #endregion
        // Public Method
        #region IsContainLetter
        public bool IsContainLetter(string letter)
        {
            bool result;

            result = false;

            if (this._letters != null)
            {
                result = this._letters.Contains(letter);
            }

            return result;
        }
        #endregion
        #region NotifyPropertyChanged
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
