using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiSam.Net.KeyLock.Structure;
using UbiSam.Net.KeyLock.HardwareManager;

namespace UbiSam.Net.KeyLock.Checker.Info
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
        private List<string> _letters;
        private UsbDisk _targetDisk;
        private LicenseInfo _licenseInfo;
        private bool _isProtected;
        #endregion
        #region Property
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
        public UsbDisk TargetDisk
        {
            set
            {
                this._targetDisk = value;
                if (value != null)
                {
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
        #endregion
        public bool IsProtected
        {
            get
            {
                return this._isProtected;
            }
            set
            {
                if (this._isProtected != value)
                {
                    this._isProtected = value;
                    NotifyPropertyChanged("IsProtected");
                }
            }
        }
        #region Constructor
        public KeyLockInfo()
        {
            this._targetDisk = null;
            this._letters = new List<string>();
            this._licenseInfo = null;
            this._isProtected = false;
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
