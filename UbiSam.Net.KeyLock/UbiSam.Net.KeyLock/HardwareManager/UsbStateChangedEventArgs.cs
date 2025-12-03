using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiSam.Net.KeyLock.HardwareManager
{
    public delegate void UsbStateChangedEventHandler(UsbStateChangedEventArgs e);
    public class UsbStateChangedEventArgs : EventArgs
    {

        public UsbStateChangedEventArgs(UsbStateChange state, UsbPartition partition)
        {
            this.State = state;
            this.Partition = partition;
        }
        public UsbPartition Partition
        {
            get;
            private set;
        }
        public UsbStateChange State
        {
            get;
            private set;
        }
    }
}
