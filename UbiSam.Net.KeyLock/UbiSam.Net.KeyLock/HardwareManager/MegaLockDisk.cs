namespace UbiSam.Net.KeyLock.HardwareManager
{
    public class MegaLockDisk : UsbDisk
    {
        public MegaLockDisk(string id, string serialNumber)
        {
            base.VID = id;
            base.PID = id;
            base.SerialNumber = serialNumber;
        }
    }
}
