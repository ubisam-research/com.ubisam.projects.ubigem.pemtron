using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace UbiSam.Net.KeyLock.HardwareManager
{
    public class UsbDisk : List<UsbPartition>
    {
        internal UsbDisk()
        {
            this.Model = string.Empty;
            this.Size = 0;
        }

        public string SerialNumber
        {
            get;
            internal set;
        }

        public string VID
        {
            get;
            internal set;
        }

        public string PID
        {
            get;
            internal set;
        }

        public string Model
        {
            get;
            internal set;
        }

        public ulong Size
        {
            get;
            internal set;
        }

        public string DeviceID
        {
            get;
            internal set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Model);
            builder.Append(" (");

            StringBuilder driveLetters = new StringBuilder();
            bool isFirst = true;
            foreach(UsbPartition partition in this)
            {
                if (isFirst == false) driveLetters.Append(",");
                isFirst = false;

                driveLetters.Append(partition.Letter);
            }

            builder.Append(driveLetters);
            builder.Append(") ");
            builder.Append(ConstData.FormatByteCount(Size));

            builder.Append(" ");
            builder.Append(DeviceID);

            builder.Append(" ");
            builder.Append(VID);

            builder.Append(" ");
            builder.Append(PID);

            builder.Append(" ");
            builder.Append(SerialNumber);

            return builder.ToString();
        }

        public bool Contains(string letter)
        {
            return this.AsQueryable<UsbPartition>().Any(part => part.Letter == letter) == true;
        }

        public void AddPartition(UsbPartition partition)
        {
            var existsPartition = this.AsQueryable<UsbPartition>().Where(part => part.Letter == partition.Letter).Select(part => part).FirstOrDefault();
            if (existsPartition != null) return;

            partition.OwnerDisk = this;
            this.Add(partition);
        }

        public void RemovePartition(UsbPartition partition)
        {
            var existsPartition = this.AsQueryable<UsbPartition>().Where(part => part.Letter == partition.Letter).Select(part => part).FirstOrDefault();
            if (existsPartition == null) return;

            this.Remove(partition);
        }

        internal void ClearPartition()
        {
            this.Clear();
        }
    }
}
