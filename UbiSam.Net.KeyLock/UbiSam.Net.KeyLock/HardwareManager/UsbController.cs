using System;
using System.Runtime.InteropServices;
using System.Management;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;

namespace UbiSam.Net.KeyLock.HardwareManager
{

    public class UsbController
    {
        #region DriverWindow

        private class DriverWindow : NativeWindow, IDisposable
        {
            // Contains information about a logical volume.
            [StructLayout(LayoutKind.Sequential)]
            public struct DEV_BROADCAST_VOLUME
            {
                public int dbcv_size;           // size of the struct
                public int dbcv_devicetype;     // DBT_DEVTYP_VOLUME
                public int dbcv_reserved;       // reserved; do not use
                public int dbcv_unitmask;       // Bit 0=A, bit 1=B, and so on (bitmask)
                public short dbcv_flags;        // DBTF_MEDIA=0x01, DBTF_NET=0x02 (bitmask)
            }

            // WMI 관련 이벤트 정의
            private const int WM_DEVICECHANGE = 0x0219;             // device state change
            private const int DBT_DEVICEARRIVAL = 0x8000;           // detected a new device
            private const int DBT_DEVICEQUERYREMOVE = 0x8001;       // preparing to remove
            private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;    // removed 
            private const int DBT_DEVTYP_VOLUME = 0x00000002;       // logical volume


            public DriverWindow()
            {
                // create a generic window with no class name
                base.CreateHandle(new CreateParams());
            }

            public void Dispose()
            {
                base.DestroyHandle();
                GC.SuppressFinalize(this);
            }

            public event UsbStateChangedEventHandler StateChanged;

            protected override void WndProc(ref Message message)
            {
                base.WndProc(ref message);

                if ((message.Msg == WM_DEVICECHANGE) && (message.LParam != IntPtr.Zero))
                {
                    try
                    {
                        DEV_BROADCAST_VOLUME volume = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(
                            message.LParam, typeof(DEV_BROADCAST_VOLUME));

                        if (volume.dbcv_devicetype == DBT_DEVTYP_VOLUME)
                        {
                            switch (message.WParam.ToInt32())
                            {
                                case DBT_DEVICEARRIVAL:
                                    SignalDeviceChange(UsbStateChange.Added, volume);
                                    break;

                                case DBT_DEVICEQUERYREMOVE:
                                    // can intercept
                                    break;

                                case DBT_DEVICEREMOVECOMPLETE:
                                    SignalDeviceChange(UsbStateChange.Removed, volume);
                                    break;
                            }
                        }
                    }
                    catch { }
                }
            }

            private void SignalDeviceChange(UsbStateChange state, DEV_BROADCAST_VOLUME volume)
            {
                try
                {
                    string letter = ToUnitName(volume.dbcv_unitmask);

                    if (StateChanged != null)
                    {
                        UsbPartition partition = new UsbPartition(letter);

                        StateChanged(new UsbStateChangedEventArgs(state, partition));
                    }
                }
                catch { }
            }

            private string ToUnitName(int mask)
            {
                int offset = 0;

                while ((offset < 26) && ((mask & 0x00000001) == 0))
                {
                    mask >>= 1;
                    offset++;
                }

                if (offset < 26)
                {
                    return string.Format("{0}:", Convert.ToChar(Convert.ToInt32('A') + offset));
                }

                return "?:";
            }
        }
        #endregion WndProc Driver

        #region Event
        private delegate void GetDiskInformationDelegate(UsbPartition part);
        private delegate void RemoveDiskInformationDelegate(UsbPartition part);

        private DriverWindow _window;
        private bool _isDisposed;

        private event UsbStateChangedEventHandler OnUsbStateChangedEventHandler;
        #endregion


        //========================================================================================
        // Constructor
        //========================================================================================

        public UsbController()
        {
            this._window = null;
            this.OnUsbStateChangedEventHandler = null;
            this._isDisposed = false;
        }


        #region Lifecycle

        ~UsbController()
        {
            Dispose();
        }


        public void Dispose()
        {
            if (!this._isDisposed)
            {
                if (this._window != null)
                {
                    this._window.StateChanged -= new UsbStateChangedEventHandler(DoStateChanged);
                    this._window.Dispose();
                    this._window = null;
                }

                this._isDisposed = true;

                GC.SuppressFinalize(this);
            }
        }
        #endregion Lifecycle


        //========================================================================================
        // Events/Properties
        //========================================================================================

        public event UsbStateChangedEventHandler StateChanged
        {
            add
            {
                if (this._window == null)
                {
                    // create the driver window once a consumer registers for notifications
                    this._window = new DriverWindow();
                    this._window.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);
                }

                this.OnUsbStateChangedEventHandler = (UsbStateChangedEventHandler)Delegate.Combine(this.OnUsbStateChangedEventHandler, value);
            }

            remove
            {
                this.OnUsbStateChangedEventHandler = (UsbStateChangedEventHandler)Delegate.Remove(this.OnUsbStateChangedEventHandler, value);

                if (this.OnUsbStateChangedEventHandler == null)
                {
                    // destroy the driver window once the consumer stops listening
                    this._window.StateChanged -= new UsbStateChangedEventHandler(DoStateChanged);
                    this._window.Dispose();
                    this._window = null;
                }
            }
        }

        public void ReadAvailableDisks()
        {
            ManagementObjectSearcher drives;
            string deviceID;
            UsbDisk usbDisk;

            this._existsDisks.Clear();

            drives = new ManagementObjectSearcher("select * from Win32_DiskDrive where InterfaceType='USB'");

            try
            {
                foreach (ManagementObject drive in drives.Get())
                {
                    deviceID = string.Empty;

                    usbDisk = null;

                    if (drive != null)
                    {
                        // Serial number: pnpDeviceID 중 일부
                        deviceID = drive["DeviceID"].ToString();

                        usbDisk = new UsbDisk
                        {
                            DeviceID = deviceID
                        };

                        UpdateUsbDisk(usbDisk);

                        this._existsDisks.Add(usbDisk);

                        
                        if (usbDisk != null)
                        {
                            // associate physical disks with partitions
                            ManagementObjectCollection partitions = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition", usbDisk.DeviceID)).Get();

                            // USB Disk별 partition 정보 조회
                            foreach (ManagementObject partition in partitions)
                            {
                                // associate partitions with logical disks (drive letter volumes)
                                ManagementObject logical = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition", partition["DeviceID"])).First();

                                if (logical == null) continue;
                                // finally find the logical disk entry to determine the volume name
                                ManagementObject volume = new ManagementObjectSearcher(string.Format("select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{0}'", logical["Name"])).First();
                                UsbPartition usbPartition = new UsbPartition(logical["Name"].ToString());
                                if (volume["VolumeName"] == null) usbPartition.Volume = "Broken";
                                else usbPartition.Volume = volume["VolumeName"].ToString();
                                usbPartition.FreeSpace = Convert.ToUInt64(volume["FreeSpace"]);
                                usbPartition.Size = Convert.ToUInt64(volume["Size"]);
                                usbDisk.AddPartition(usbPartition);

                            }
                        }
                    }
                }
            }
            catch { }
        }
        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            try
            {
                // we can only interrogate drives that are added...
                // cannot see something that is no longer there!
                if ((e.State == UsbStateChange.Added) && (e.Partition.Letter[0] != '?'))
                {
                    // the following Begin/End invokes looks strange but are required
                    // to resolve a "DisconnectedContext was detected" exception which
                    // occurs when the current thread terminates before the WMI queries
                    // can complete.  I'm not exactly sure why that would happen...

                    GetDiskInformationDelegate gdi = new GetDiskInformationDelegate(GetDiskInformation);
                    IAsyncResult result = gdi.BeginInvoke(e.Partition, null, null);
                    gdi.EndInvoke(result);
                }

                if ((e.State == UsbStateChange.Removed) && (e.Partition.Letter[0] != '?'))
                {
                    // the following Begin/End invokes looks strange but are required
                    // to resolve a "DisconnectedContext was detected" exception which
                    // occurs when the current thread terminates before the WMI queries
                    // can complete.  I'm not exactly sure why that would happen...

                    RemoveDiskInformationDelegate gdi = new RemoveDiskInformationDelegate(RemoveDiskInformation);
                    IAsyncResult result = gdi.BeginInvoke(e.Partition, null, null);
                    gdi.EndInvoke(result);
                }
            }
            catch { }

            this.OnUsbStateChangedEventHandler?.Invoke(e);
        }

        private readonly UsbDiskCollection _existsDisks = new UsbDiskCollection();
        public UsbDiskCollection ExistsDisks { get { return new UsbDiskCollection(this._existsDisks); } }

        // Partition 기준으로 Usb Disk 정보를 역으로 추적 후 모든 파티션 정보를 다시 읽음
        private void GetDiskInformation(UsbPartition part)
        {
            string query;
            ManagementObject partitionObject;
            ManagementObject driveObject;

            if (part != null && part.OwnerDisk == null)
            {
                query = $"associators of {{Win32_LogicalDisk.DeviceID='{part.Letter}'}} where AssocClass = Win32_LogicalDiskToPartition";

                partitionObject = null;

                try
                {
                    partitionObject = new ManagementObjectSearcher(query).First();
                }
                catch { }

                if (partitionObject != null)
                {
                    query = $"associators of {{Win32_DiskPartition.DeviceID='{partitionObject["DeviceID"]}'}}  where resultClass = Win32_DiskDrive";

                    driveObject = null;

                    try
                    {
                        driveObject = new ManagementObjectSearcher(query).First();
                    }
                    catch { }
                    
                    if (driveObject != null)
                    {
                        UsbDisk disk = new UsbDisk
                        {
                            DeviceID = driveObject["DeviceID"].ToString()
                        };

                        UpdateUsbDisk(disk);

                        part.OwnerDisk = disk;

                        if (_existsDisks.Contains(disk.DeviceID) == false)
                        {
                            _existsDisks.Add(disk);
                        }

                        disk.ClearPartition();

                        query = $"associators of {{Win32_DiskDrive.DeviceID='{driveObject["DeviceID"]}'}} where AssocClass = Win32_DiskDriveToDiskPartition";

                        ManagementObjectCollection partitions = null;

                        try
                        {
                            partitions = new ManagementObjectSearcher(query).Get();
                        }
                        catch { }

                        if (partitions != null)
                        {
                            foreach (ManagementObject partition in partitions)
                            {
                                query = $"associators of {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} where AssocClass = Win32_LogicalDiskToPartition";
                                ManagementObject logical = null;
                                
                                try
                                {
                                    logical = new ManagementObjectSearcher(query).First();
                                }
                                catch { }

                                if (logical != null)
                                {
                                    query = $"select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{logical["Name"]}'";

                                    try
                                    {
                                        ManagementObject volume = new ManagementObjectSearcher(query).First();

                                        if (volume != null)
                                        {
                                            if (volume["VolumeName"] == null) part.Volume = "Broken";
                                            else part.Volume = volume["VolumeName"].ToString();
                                            part.FreeSpace = Convert.ToUInt64(volume["FreeSpace"]);
                                            part.Size = Convert.ToUInt64(volume["Size"]);
                                            disk.AddPartition(part);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveDiskInformation(UsbPartition part)
        {
            this._existsDisks.RemovePartition(part.Letter);
        }

        public string GetDriveLetterFromPhysicalNum(int physicalNum, string volumeName)
        {
            string ret = string.Empty;

            try
            {
                foreach (ManagementObject drive in new ManagementObjectSearcher("select * from Win32_DiskDrive where InterfaceType='USB'").Get())
                {
                    if (drive == null) continue;

                    string deviceID = drive["DeviceID"].ToString().Replace("\\", "").Replace(".", "");
                    if (deviceID.StartsWith("PHYSICALDRIVE") == false) continue;
                    string physicalDriveSection = deviceID.Substring("PHYSICALDRIVE".Length);
                    if (Convert.ToInt32(physicalDriveSection) != physicalNum) continue;

                    // associate physical disks with partitions
                    ManagementObjectCollection partitions = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition", drive["DeviceID"])).Get();

                    foreach (ManagementObject partition in partitions)
                    {
                        // associate partitions with logical disks (drive letter volumes)
                        ManagementObject logical = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition", partition["DeviceID"])).First();
                        if (logical == null) continue;
                        // finally find the logical disk entry to determine the volume name

                        ManagementObject volume = new ManagementObjectSearcher(string.Format("select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{0}'", logical["Name"])).First();
                        if (volume["VolumeName"] == null) continue;
                        if (volume["VolumeName"].ToString() != volumeName) continue;
                        ret = logical["Name"].ToString();
                        break;
                    }
                }
            }
            catch 
            { 
                ret = string.Empty; 
            }

            return ret;
        }
        public void Refresh(int physicalNum, UsbDisk targetDisk)
        {
            UsbPartition usbPartition;

            targetDisk.ClearPartition();

            try
            {
                foreach (ManagementObject drive in new ManagementObjectSearcher("select * from Win32_DiskDrive where InterfaceType='USB'").Get())
                {
                    if (drive == null) continue;

                    string deviceID = drive["DeviceID"].ToString().Replace("\\", "").Replace(".", "");
                    if (deviceID.StartsWith("PHYSICALDRIVE") == false) continue;
                    string physicalDriveSection = deviceID.Substring("PHYSICALDRIVE".Length);
                    if (Convert.ToInt32(physicalDriveSection) != physicalNum) continue;

                    // associate physical disks with partitions
                    ManagementObjectCollection partitions = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition", drive["DeviceID"])).Get();

                    foreach (ManagementObject partition in partitions)
                    {
                        // associate partitions with logical disks (drive letter volumes)
                        ManagementObject logical = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition", partition["DeviceID"])).First();
                        if (logical == null) continue;
                        // finally find the logical disk entry to determine the volume name

                        ManagementObject volume = new ManagementObjectSearcher(string.Format("select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{0}'", logical["Name"])).First();

                        usbPartition = new UsbPartition(logical["Name"].ToString());
                        if (volume["VolumeName"] == null) usbPartition.Volume = "Broken";
                        else usbPartition.Volume = volume["VolumeName"].ToString();
                        usbPartition.FreeSpace = Convert.ToUInt64(volume["FreeSpace"]);
                        usbPartition.Size = Convert.ToUInt64(volume["Size"]);
                        targetDisk.AddPartition(usbPartition);
                    }
                }
            }
            catch { }
        }
        /*
        #region GetMachineCode
        public bool GetMachineCode(out string machineCode)
        {
            bool result;

            string cpuSN;
            string mbSN;

            info = string.Empty;

            cpuSN = string.Empty;
            mbSN = string.Empty;

            ManagementClass mc;
            ManagementObjectCollection moc;
            
            mc = new ManagementClass("win32_processor");
            moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuSN = mo.Properties["processorID"].Value.ToString();
                break;
            }

            mc = new ManagementClass("Win32_BaseBoard");
            moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                mbSN = mo.Properties["SerialNumber"].Value.ToString();
                break;
            }

            result = false;

            if (string.IsNullOrEmpty(cpuSN) == false && string.IsNullOrEmpty(mbSN) == false)
            {
                result = true;
                info = $"{cpuSN}_{mbSN}";
            }

            return result;

        }
        #endregion
        */
        private void UpdateUsbDisk(UsbDisk usbDisk)
        {
            string query;
            ManagementObjectSearcher drives;
            string deviceSerialNumber;
            string deviceID;
            string pnpDeviceID;
            string[] tokens;
            string VID;
            string PID;
            ManagementObjectSearcher oSearcher;

            if (usbDisk != null && string.IsNullOrEmpty(usbDisk.DeviceID) == false)
            {
                query = $"select * from Win32_DiskDrive where InterfaceType='USB' and DeviceID='{usbDisk.DeviceID.Replace("\\", "\\\\")}'";

                drives = null;

                try
                {
                    drives = new ManagementObjectSearcher(query);
                }
                catch { }

                if (drives != null)
                {
                    foreach (ManagementObject drive in drives.Get())
                    {
                        deviceSerialNumber = string.Empty;
                        deviceID = string.Empty;
                        pnpDeviceID = string.Empty;

                        VID = string.Empty;
                        PID = string.Empty;

                        if (drive != null)
                        {
                            // Serial number: pnpDeviceID 중 일부
                            pnpDeviceID = drive["PNPDeviceID"].ToString();

                            tokens = pnpDeviceID.Split(new char[] { '\\' });

                            if (pnpDeviceID.StartsWith("USBSTOR") == true && tokens != null && tokens.Length == 3)
                            {
                                deviceSerialNumber = tokens.Last();

                                if (deviceSerialNumber.Contains("&") == true)
                                {
                                    deviceSerialNumber = deviceSerialNumber.Substring(0, deviceSerialNumber.IndexOf("&"));
                                }

                                if (string.IsNullOrEmpty(deviceSerialNumber) == false)
                                {
                                    oSearcher = null;

                                    try
                                    {
                                        oSearcher = new ManagementObjectSearcher(@"root\CIMV2", "Select * from Win32_USBHub");
                                    }
                                    catch { }

                                    if (oSearcher != null)
                                    {
                                        foreach (ManagementObject oResult in oSearcher.Get())
                                        {
                                            if (oResult != null)
                                            {
                                                var oValue = oResult["DeviceID"];

                                                if (oValue != null)
                                                {
                                                    string szDeviceID = oValue.ToString();

                                                    if (szDeviceID.Contains(deviceSerialNumber) == true && szDeviceID.Length > 17 && szDeviceID.Contains("VID_") == true && szDeviceID.Contains("PID_") == true)
                                                    {
                                                        usbDisk.Model = drive["Model"].ToString();
                                                        usbDisk.Size = Convert.ToUInt64(drive["Size"]);
                                                        usbDisk.VID = szDeviceID.Substring(szDeviceID.IndexOf("VID_") + 4, 4);
                                                        usbDisk.PID = szDeviceID.Substring(szDeviceID.IndexOf("PID_") + 4, 4);
                                                        usbDisk.SerialNumber = szDeviceID.Substring(szDeviceID.LastIndexOf("\\") + 1);

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                            ManagementObjectCollection partitions = null;
                            // associate physical disks with partitions
                            try
                            {
                                partitions = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition", drive["DeviceID"])).Get();
                            }
                            catch { }

                            if (partitions != null)
                            {
                                // USB Disk별 partition 정보 조회
                                foreach (ManagementObject partition in partitions)
                                {
                                    ManagementObject logical = null;
                                    // associate partitions with logical disks (drive letter volumes)
                                    try
                                    {
                                        logical = new ManagementObjectSearcher(string.Format("associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition", partition["DeviceID"])).First();
                                    }
                                    catch { }

                                    if (logical != null)
                                    {
                                        // finally find the logical disk entry to determine the volume name
                                        ManagementObject volume;

                                        try
                                        {
                                            volume = new ManagementObjectSearcher(string.Format("select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{0}'", logical["Name"])).First();

                                            UsbPartition usbPartition = new UsbPartition(logical["Name"].ToString());
                                            if (volume["VolumeName"] == null) usbPartition.Volume = "Broken";
                                            else usbPartition.Volume = volume["VolumeName"].ToString();
                                            usbPartition.FreeSpace = Convert.ToUInt64(volume["FreeSpace"]);
                                            usbPartition.Size = Convert.ToUInt64(volume["Size"]);
                                            usbDisk.AddPartition(usbPartition);
                                        }
                                        catch { }

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
