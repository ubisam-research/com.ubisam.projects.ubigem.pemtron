using System.Collections.Generic;
using System.Linq;

namespace UbiSam.Net.KeyLock.HardwareManager
{
    public class UsbDiskCollection : List<UsbDisk>
    {

        public UsbDiskCollection(UsbDiskCollection collection) : base(collection)
        {
        }

        public UsbDiskCollection() : base()
        {
        }

        public bool Contains(string deviceID)
        {
            return this.AsQueryable<UsbDisk>().Any(d => d.DeviceID == deviceID) == true;
        }

        public bool Remove(string deviceID)
        {
            UsbDisk disk = (
                            this.AsQueryable<UsbDisk>()
                            .Where(d => d.DeviceID == deviceID
                        ).Select(d => d)).FirstOrDefault<UsbDisk>();

            if (disk != null)
            {
                return this.Remove(disk);
            }

            return false;
        }

        public UsbDisk this[string deviceID]
        {
            get
            {
                if (Contains(deviceID) == false) return null;

                UsbDisk disk =
                (this.AsQueryable<UsbDisk>()
                .Where(d => d.DeviceID == deviceID)
                .Select(d => d)).FirstOrDefault<UsbDisk>();

                return disk;
            }
        }

        public void RemovePartition(string letter)
        {
            var partition = this.AsQueryable<UsbDisk>().SelectMany(disk => disk).Where(part => part.Letter == letter).FirstOrDefault();
            if (partition == null) return;

            UsbDisk owner = partition.OwnerDisk;
            owner.RemovePartition(partition);

            this.Remove(owner.DeviceID);
        }
    }
}
