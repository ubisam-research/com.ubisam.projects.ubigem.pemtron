using System;

namespace UbiSam.Net.KeyLock.HardwareManager
{
    #region LicenseCheckEventArgs
    public class ConstData
    {
        #region Property
        public static readonly ulong KB = 1024;
        public static readonly ulong MB = KB * 1024;
        public static readonly ulong GB = MB * 1024;
        public static readonly ulong TB = GB * 1024;
        #endregion

        #region FormatByteCount
        public static string FormatByteCount(ulong bytes)
        {
            string format = null;

            if (bytes < ConstData.KB)
            {
                format = string.Format("{0} Bytes", bytes);
            }
            else if (bytes < ConstData.MB)
            {
                bytes = bytes / ConstData.KB;
                format = string.Format("{0} KB", bytes.ToString("N"));
            }
            else if (bytes < ConstData.GB)
            {
                bytes = bytes / ConstData.MB;
                format = string.Format("{0} MB", bytes.ToString("N1"));
            }
            else if (bytes < ConstData.TB)
            {
                bytes = bytes / ConstData.GB;
                format = string.Format("{0} GB", bytes.ToString("N1"));
            }
            else
            {
                bytes = bytes / ConstData.TB;
                format = string.Format("{0} TB", bytes.ToString("N1"));
            }

            return format;
        }
        #endregion
    }
    #endregion
}
