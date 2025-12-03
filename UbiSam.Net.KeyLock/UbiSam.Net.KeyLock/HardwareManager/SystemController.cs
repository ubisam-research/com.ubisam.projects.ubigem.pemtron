using System;
using System.Runtime.InteropServices;
using System.Management;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace UbiSam.Net.KeyLock.HardwareManager
{
    public class SystemController
    {
        public static string CPU_ID
        {
            get
            {
                StringBuilder sbResult;

                ManagementClass mc;
                ManagementObjectCollection moc;

                sbResult = new StringBuilder();

                mc = new ManagementClass("win32_processor");
                moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (mo.Properties["processorID"] != null && mo.Properties["processorID"].Value != null)
                    {
                        if (sbResult.Length != 0)
                        {
                            sbResult.Append(",");
                        }

                        sbResult.Append(mo.Properties["processorID"].Value.ToString());
                    }
                }

                return sbResult.ToString();
            }
        }
        public static string MB_ID
        {
            get
            {
                string result;

                ManagementClass mc;
                ManagementObjectCollection moc;

                result = string.Empty;

                mc = new ManagementClass("Win32_BaseBoard");
                moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
                    {
                        result = mo.Properties["SerialNumber"].Value.ToString();
                        break;
                    }
                }

                return result;
            }
        }
        public static string BIOS_ID
        {
            get
            {
                string result;

                ManagementClass mc;
                ManagementObjectCollection moc;

                result = string.Empty;

                mc = new ManagementClass("Win32_BIOS");
                moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
                    {
                        result = mo.Properties["SerialNumber"].Value.ToString();
                        break;
                    }
                }

                return result;
            }
        }
        public static string Vendor
        {
            get
            {
                string result;

                ManagementClass mc;
                ManagementObjectCollection moc;

                result = string.Empty;

                mc = new ManagementClass("Win32_ComputerSystemProduct");
                moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (mo.Properties["Vendor"] != null && mo.Properties["Vendor"].Value != null)
                    {
                        result = mo.Properties["Vendor"].Value.ToString();
                        break;
                    }
                }

                return result;
            }
        }
        public static string IdentifyingNumber
        {
            get
            {
                string result;

                ManagementClass mc;
                ManagementObjectCollection moc;

                result = string.Empty;

                mc = new ManagementClass("Win32_ComputerSystemProduct");
                moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (mo.Properties["IdentifyingNumber"] != null && mo.Properties["IdentifyingNumber"].Value != null)
                    {
                        result = mo.Properties["IdentifyingNumber"].Value.ToString();
                        break;
                    }
                }

                return result;
            }
        }
        public static string UUID
        {
            get
            {
                string result;

                ManagementClass mc;
                ManagementObjectCollection moc;

                result = string.Empty;

                mc = new ManagementClass("Win32_ComputerSystemProduct");
                moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (mo.Properties["UUID"] != null && mo.Properties["UUID"].Value != null)
                    {
                        result = mo.Properties["UUID"].Value.ToString();
                        break;
                    }
                }

                return result;
            }
        }
    }

}
