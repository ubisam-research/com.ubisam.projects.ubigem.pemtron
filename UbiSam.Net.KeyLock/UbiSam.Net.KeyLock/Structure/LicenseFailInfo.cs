using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiSam.Net.KeyLock.Structure
{
    internal class LicenseFailInfo
    {
        #region [Properties]
        internal string UniqueKey;
        internal string ProductCode;
        internal bool IsLicenseFail { get; set; }
        internal DateTime FirstFailDT { get; set; }
        internal bool IsLicenseWarning { get; private set; }
        internal bool IsLicenseWarningEventCalled { get; private set; }
        internal bool IsLicenseExpiredEventCalled { get; private set; }
        #endregion
        #region [Methods]
        public void Reset(bool isLicenseFail)
        {
            bool isLicenseFailBefore;

            isLicenseFailBefore = IsLicenseFail;

            IsLicenseFail = isLicenseFail;

            if (isLicenseFailBefore == true && isLicenseFail == false)
            {
                FirstFailDT = DateTime.Now;
                IsLicenseWarningEventCalled = false;
                IsLicenseExpiredEventCalled = false;
            }

            IsLicenseWarning = false;

            if (isLicenseFailBefore == false && isLicenseFail == true)
            {
                FirstFailDT = DateTime.Now;
                IsLicenseWarning = true;
            }

        }
        public void SetLicenseWarningEventCalled()
        {
            IsLicenseWarningEventCalled = true;
        }
        public void SetLicenseExpiredEventCalled()
        {
            IsLicenseExpiredEventCalled = true;
        }
        public override string ToString()
        {
            return $"[UniqueKey={UniqueKey},IsLicenseFail={IsLicenseFail},FirstFailDT={FirstFailDT:yyyyMMdd_HHmmss.fff},IsLicenseWarning={IsLicenseWarning},IsLicenseWarningEventCalled={IsLicenseWarningEventCalled},IsLicenseExpiredEventCalled={IsLicenseExpiredEventCalled}]";
        }
        #endregion
    }
}
