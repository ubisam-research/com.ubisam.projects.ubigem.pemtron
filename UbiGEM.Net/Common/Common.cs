using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UbiGEM.Net
{
    class Common
    {
        [DllImport("kernel32.dll")]
        internal extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        internal struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }
    }
}