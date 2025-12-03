using System.Collections.Generic;
using System.Text;
using UbiSam.Net.KeyLock.Utilities;

namespace UbiSam.Net.KeyLock.Structure
{
    public class LicenseInfo
    {
        #region [Properties]
        public LicenseResult Result
        {
            get;
            protected internal set;
        }
        public string Letter
        {
            get;
            protected internal set;
        }
        public string KeyLockInfo
        {
            get;
            protected internal set;
        }
        public string Volume
        {
            get;
            protected internal set;
        }
        public string AdditionalInfo
        {
            get;
            protected internal set;
        }
        public string Identifier
        {
            get;
            protected internal set;
        }
        public string LicensedProducts
        {
            get
            {
                StringBuilder sbResult;

                sbResult = new StringBuilder();

                foreach (var product in LicensedProductCountCollection.Keys)
                {
                    if (sbResult.Length != 0)
                    {
                        sbResult.Append(",");
                    }

                    sbResult.Append($"{product}");
                }

                return sbResult.ToString();
            }
        }
        public string USBKeyInfo
        {
            get
            {
                string encrypted = $"USB|{Cryptography.Encrypt2(Identifier)}";
                return Cryptography.Encrypt1(encrypted);
            }
        }
        public string SystemKeyInfo
        {
            get
            {
                string encrypted = $"SYSTEM|{Cryptography.Encrypt2(Identifier)}";
                return Cryptography.Encrypt1(encrypted);
            }
        }
        public Dictionary<string, int> LicensedProductCountCollection { get; }
        #endregion
        public LicenseInfo()
        {
            this.LicensedProductCountCollection = new Dictionary<string, int>();
        }
        public LicenseInfo(string megaLockSerial)
        {
            this.Identifier = megaLockSerial;
            this.LicensedProductCountCollection = new Dictionary<string, int>();
        }
    }

}
