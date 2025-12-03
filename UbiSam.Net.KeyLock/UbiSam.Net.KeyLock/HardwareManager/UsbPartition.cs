using System;
using System.Text;

namespace UbiSam.Net.KeyLock.HardwareManager
{
    public class UsbPartition
    {

        public UsbDisk OwnerDisk { get; internal set; }

        internal UsbPartition(string letter)
        {
            this.Letter = letter;
            this.Volume = string.Empty;
            this.FreeSpace = 0;
            this.Size = 0;
        }

        public ulong FreeSpace
        {
            get;
            internal set;
        }

        public string Letter
        {
            get;
            private set;
        }

        public ulong Size
        {
            get;
            internal set;
        }

        public string Volume
        {
            get;
            internal set;
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Volume);
            builder.Append(" ");
            builder.Append("(");
            builder.Append(Letter);
            builder.Append(")");
            builder.Append(ConstData.FormatByteCount(FreeSpace));
            builder.Append(" free of ");
            builder.Append(ConstData.FormatByteCount(Size));

            return builder.ToString();
        }

    }
}
