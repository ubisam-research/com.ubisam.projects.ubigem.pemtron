using System.Management;

namespace UbiSam.Net.KeyLock.HardwareManager
{

    public static class WmiExtensions
    {

        public static ManagementObject First(this ManagementObjectSearcher searcher)
        {
            ManagementObject result = null;

            try
            {
                if (searcher != null)
                {
                    foreach (ManagementObject item in searcher.Get())
                    {
                        result = item;
                        break;
                    }
                }
            }
            catch { }

            return result;
        }
    }
}
