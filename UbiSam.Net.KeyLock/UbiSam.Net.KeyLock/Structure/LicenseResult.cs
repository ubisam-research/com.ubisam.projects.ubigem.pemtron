using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiSam.Net.KeyLock.Structure
{
    #region LicenseResult
    public enum LicenseResult
    {
        LicenseOk,
        ProductMismatch,
        Expired,
        Warning,
        Unknown,
        KeyMismatch,
        DeviceMismatch,
    }
    #endregion
}
