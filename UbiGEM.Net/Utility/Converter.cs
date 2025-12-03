using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Utility
{
    [ComVisible(false)]
    internal static class Converter
    {
        internal static byte[] ConvertBooleans2Bytes(bool[] value)
        {
            byte[] result = new byte[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                result[i] = Convert.ToByte(value);
            }

            return result;
        }

        internal static byte[] ConvertInt2Bytes(int value, int length)
        {
            byte[] result = new byte[length];

            Array.Copy(BitConverter.GetBytes(value), result, length);

            return ReverseBytes(result);
        }

        internal static byte[] ConvertInts2Bytes(int[] value)
        {
            byte[] result = new byte[value.Length * 4];
            byte[] valueTemp;

            for (int i = 0; i < value.Length; i++)
            {
                valueTemp = ConvertInt2Bytes(value[i], 4);

                Array.Copy(valueTemp, 0, result, i * 4, 4);
            }

            return result;
        }

        internal static byte[] ConvertLong2Bytes(long value, int length)
        {
            byte[] result = new byte[length];

            Array.Copy(BitConverter.GetBytes(value), result, length);

            return ReverseBytes(result);
        }

        internal static byte[] ConvertLongs2Bytes(long[] value)
        {
            byte[] result = new byte[value.Length * 8];
            byte[] valueTemp;

            for (int i = 0; i < value.Length; i++)
            {
                valueTemp = ConvertLong2Bytes(value[i], 8);

                Array.Copy(valueTemp, 0, result, i * 8, 8);
            }

            return result;
        }

        internal static byte[] ConvertShort2Bytes(short value)
        {
            return new byte[] { ((byte)((int)value >> 8)), ((byte)(value & 0xff)) };
        }

        internal static byte[] ConvertShorts2Bytes(short[] value)
        {
            byte[] result = new byte[value.Length * 2];

            for (int i = 0; i < value.Length; i++)
            {
                result[i * 2] = (byte)((int)value[i] >> 8);
                result[i * 2 + 1] = (byte)(value[i] & 0xff);
            }

            return result;
        }

        internal static byte[] ConvertUshort2Bytes(ushort value)
        {
            return new byte[] { ((byte)((int)value >> 8)), ((byte)(value & 0xff)) };
        }

        internal static byte[] ConvertUshorts2Bytes(ushort[] value)
        {
            byte[] result = new byte[value.Length * 2];

            for (int i = 0; i < value.Length; i++)
            {
                result[i * 2] = (byte)((int)value[i] >> 8);
                result[i * 2 + 1] = (byte)(value[i] & 0xff);
            }

            return result;
        }

        internal static byte[] ConvertUint2Bytes(uint value, int length)
        {
            byte[] result = new byte[length];

            Array.Copy(BitConverter.GetBytes(value), 0, result, 0, 4);

            return ReverseBytes(result);
        }

        internal static byte[] ConvertUints2Bytes(uint[] value)
        {
            byte[] result = new byte[value.Length * 4];
            byte[] valueTemp;

            for (int i = 0; i < value.Length; i++)
            {
                valueTemp = ConvertUint2Bytes(value[i], 4);

                Array.Copy(valueTemp, 0, result, i * 4, 4);
            }

            return result;
        }

        internal static byte[] ConvertUlong2Bytes(ulong value, int length)
        {
            byte[] result = new byte[length];

            Array.Copy(BitConverter.GetBytes(value), result, length);

            return ReverseBytes(result);
        }

        internal static byte[] ConvertUlongs2Bytes(ulong[] value)
        {
            byte[] result = new byte[value.Length * 8];
            byte[] valueTemp;

            for (int i = 0; i < value.Length; i++)
            {
                valueTemp = ConvertUlong2Bytes(value[i], 8);

                Array.Copy(valueTemp, 0, result, i * 8, 8);
            }

            return result;
        }

        internal static byte[] ConvertFloat2Bytes(float value, int length)
        {
            byte[] result = new byte[length];

            Array.Copy(BitConverter.GetBytes(value), result, length);

            return ReverseBytes(result);
        }

        internal static byte[] ConvertFloats2Bytes(float[] value)
        {
            byte[] result = new byte[value.Length * 4];
            byte[] valueTemp;

            for (int i = 0; i < value.Length; i++)
            {
                valueTemp = ConvertFloat2Bytes(value[i], 4);

                Array.Copy(valueTemp, 0, result, i * 4, 4);
            }

            return result;
        }

        internal static byte[] ConvertDouble2Bytes(double value, int length)
        {
            byte[] result = new byte[length];

            Array.Copy(BitConverter.GetBytes(value), result, length);

            return ReverseBytes(result);
        }

        internal static byte[] ConvertDoubles2Bytes(double[] value)
        {
            byte[] result = new byte[value.Length * 4];
            byte[] valueTemp;

            for (int i = 0; i < value.Length; i++)
            {
                valueTemp = ConvertDouble2Bytes(value[i], 4);

                Array.Copy(valueTemp, 0, result, i * 4, 4);
            }

            return result;
        }

        internal static byte[] ReverseBytes(byte[] source)
        {
            byte[] result = new byte[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                result[i] = source[(source.Length - i) - 1];
            }

            return result;
        }

        internal static short ConvertBytes2Short(byte[] value)
        {
            value = Swap(value);

            return BitConverter.ToInt16(value, 0);
        }

        internal static ushort ConvertBytes2Ushort(byte[] value)
        {
            value = Swap(value);

            return BitConverter.ToUInt16(value, 0);
        }

        internal static int ConvertBytes2Int(byte[] value)
        {
            int result;

            switch (value.Length)
            {
                case 1:
                    result = value[0] & 0xff;

                    if (result < 0)
                        result += 256;

                    break;
                case 2:
                    value = Swap(value);
                    result = BitConverter.ToInt16(value, 0);
                    break;
                case 4:
                    value = Swap(value);
                    result = BitConverter.ToInt32(value, 0);
                    break;
                default:
                    result = 0;
                    break;
            }

            return result;
        }

        internal static int ConvertBytes2Int(byte[] value, int length)
        {
            int result;

            switch (length)
            {
                case 1:
                    result = (int)((uint)value[0]); // & 0xff;

                    break;
                case 2:
                    value = Swap(value, length);
                    result = BitConverter.ToUInt16(value, 0);
                    if (result < 0)
                        result += ushort.MaxValue;
                    break;
                case 4:
                    value = Swap(value, length);
                    result = (int)BitConverter.ToUInt32(value, 0);
                    if (result < 0)
                        result = (int)(result + uint.MaxValue);
                    break;
                default:
                    result = 0;
                    break;
            }

            return result;
        }
        internal static uint ConvertBytes2Uint(byte[] value)
        {
            value = Swap(value);

            return BitConverter.ToUInt32(value, 0);
        }

        internal static long ConvertBytes2Long(byte[] value)
        {
            value = Swap(value);

            return BitConverter.ToInt64(value, 0);
        }

        internal static ulong ConvertBytes2Ulong(byte[] value)
        {
            value = Swap(value);

            return BitConverter.ToUInt64(value, 0);
        }

        internal static float ConvertBytes2Float(byte[] value)
        {
            value = Swap(value);

            return BitConverter.ToSingle(value, 0);
        }

        internal static double ConvertBytes2Double(byte[] value)
        {
            value = Swap(value);

            return BitConverter.ToDouble(value, 0);
        }

        internal static string ConvertBytes2String(byte[] value)
        {
            string result;

            try
            {
                if (value == null)
                {
                    result = string.Empty;
                }
                else
                {
                    result = Encoding.GetEncoding(Encoding.Default.CodePage).GetString(value);

                    if (result.IndexOf('\0') > 0)
                    {
                        result = result.Replace('\0', ' ');
                    }
                }
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        internal static byte[] ConvertString2Bytes(string value)
        {
            byte[] result;

            try
            {
                result = Encoding.GetEncoding(Encoding.Default.CodePage).GetBytes(value);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        internal static byte[] ConvertSbytes2Bytes(sbyte[] value)
        {
            byte[] result;

            try
            {
                result = (from sbyte temp in value
                          select (byte)temp).ToArray();
            }
            catch
            {
                result = null;
            }

            return result;
        }

        internal static string ConvertBytes2Jis(byte[] value)
        {
            string result;

            try
            {
                result = Encoding.Unicode.GetString(value);

                if (result.IndexOf('\0') > 0)
                {
                    result = result.Replace('\0', ' ');
                }
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        internal static bool[] ConvertBytes2BooleanArray(byte[] value)
        {
            bool[] result;
            int count;

            if (value.Length > 0)
            {
                count = value.Length;

                result = new bool[count];

                for (int i = 0; i < count; i++)
                {
                    result[i] = value[i] > 0;
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static sbyte[] ConvertBytes2SbyteArray(byte[] value)
        {
            sbyte[] result;
            int count;

            if (value.Length > 0)
            {
                count = value.Length;

                result = new sbyte[count];

                for (int i = 0; i < count; i++)
                {
                    result[i] = (sbyte)value[i];
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static short[] ConvertBytes2ShortArray(byte[] value)
        {
            short[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {
                count = value.Length / 2;

                result = new short[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[2];

                    Array.Copy(value, i * 2, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToInt16(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static int[] ConvertBytes2IntArray(byte[] value)
        {
            int[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {
                count = value.Length / 4;

                result = new int[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[4];

                    Array.Copy(value, i * 4, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToInt32(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static long[] ConvertBytes2LongArray(byte[] value)
        {
            Int64[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {
                count = value.Length / 8;

                result = new Int64[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[8];

                    Array.Copy(value, i * 8, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToInt64(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static ushort[] ConvertBytes2UshortArray(byte[] value)
        {
            ushort[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {
                count = value.Length / 2;

                result = new ushort[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[2];

                    Array.Copy(value, i * 2, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToUInt16(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static uint[] ConvertBytes2UintArray(byte[] value)
        {
            uint[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {
                count = value.Length / 4;

                result = new uint[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[4];

                    Array.Copy(value, i * 4, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToUInt32(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static ulong[] ConvertBytes2UlongArray(byte[] value)
        {
            UInt64[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {
                count = value.Length / 8;

                result = new UInt64[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[8];

                    Array.Copy(value, i * 8, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToUInt64(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static float[] ConvertBytes2FloatArray(byte[] value)
        {
            float[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {
                count = value.Length / 4;

                result = new float[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[4];

                    Array.Copy(value, i * 4, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToSingle(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static double[] ConvertBytes2DoubleArray(byte[] value)
        {
            double[] result;
            int count;
            byte[] valueTemp;

            if (value.Length > 0)
            {

                count = value.Length / 8;

                result = new double[count];

                for (int i = 0; i < count; i++)
                {
                    valueTemp = new byte[8];

                    Array.Copy(value, i * 8, valueTemp, 0, valueTemp.Length);

                    valueTemp = Swap(valueTemp);

                    result[i] = BitConverter.ToDouble(valueTemp, 0);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static byte[] Swap(byte[] value)
        {
            byte[] buffer = new byte[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                buffer[i] = value[(value.Length - i) - 1];
            }

            return buffer;
        }

        internal static byte[] Swap(byte[] value, int length)
        {
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                buffer[i] = value[(length - i) - 1];
            }

            return buffer;
        }
    }
}