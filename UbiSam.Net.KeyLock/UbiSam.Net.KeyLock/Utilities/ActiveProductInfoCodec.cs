using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiSam.Net.KeyLock.Structure;

namespace UbiSam.Net.KeyLock.Utilities
{
    internal class ActiveProductInfoCodec
    {
        internal const int SHARED_ITEM_LENGTH = 80;
        internal const int SHARED_ITEM_TOTAL_COUNT = 1000;
        internal const int SHARED_ITEM_TOTAL_LENGTH = SHARED_ITEM_LENGTH * SHARED_ITEM_TOTAL_COUNT;
        internal const int SHARED_ITEM_COUNT_PER_INSTANCE = 5;
        internal static ActiveProductInfo[] Decode(byte[] data)
        {
            ActiveProductInfo[] result;

            result = null;

            if (data != null && data.Length == SHARED_ITEM_TOTAL_LENGTH)
            {
                result = new ActiveProductInfo[SHARED_ITEM_TOTAL_COUNT];

                for (int i = 0; i < SHARED_ITEM_TOTAL_COUNT; i++)
                {
                    result[i] = Decode(data, i * SHARED_ITEM_LENGTH);
                }

            }

            return result;
        }

        private static ActiveProductInfo Decode(byte[] data, int offset)
        {
            ActiveProductInfo result;
            string converted;
            int pid;
            ushort iid;
            string uniqueKey;
            int productHashCode;
            Product product;
            string tempDateTime;
            bool dateTimeConverted;
            DateTime? lastDT;

            pid = 0;
            uniqueKey = string.Empty;
            product = Product.None;
            lastDT = null;

            if (data != null && data.Length == SHARED_ITEM_TOTAL_LENGTH && offset + SHARED_ITEM_LENGTH <= SHARED_ITEM_TOTAL_LENGTH)
            {
                converted = Encoding.Default.GetString(data, offset, SHARED_ITEM_LENGTH);

                if (converted != null && converted.Length == SHARED_ITEM_LENGTH)
                {
                    if (converted.StartsWith("###") == true && converted.EndsWith("$$") == true)
                    {
                        try
                        {
                            /*
                            3 + 4 + 1 + 4 + 1 + 36 + 1 + 8 + 1 + 19 + 2 = 80
                            "###" + HEX_PID + "@" + HEX_IID + "@" + UK + "@" + HEX_PRODUCT + "@" + yyyyMMdd HHmmss.fff + "$$"
                            0       3         7     8         12    13   49    50            58    59                    78
                            */
                            pid = DecodeHexToInt32(converted.Substring(3, 4));
                            iid = (ushort)DecodeHexToInt32(converted.Substring(8, 4));
                            uniqueKey = converted.Substring(13, 36);
                            productHashCode = DecodeHexToInt32(converted.Substring(50, 8));
                            product = (Product)productHashCode;

                            dateTimeConverted = true;

                            tempDateTime = converted.Substring(59, 19);

                            dateTimeConverted &= int.TryParse(tempDateTime.Substring(0, 4), out int year);
                            dateTimeConverted &= int.TryParse(tempDateTime.Substring(4, 2), out int month);
                            dateTimeConverted &= int.TryParse(tempDateTime.Substring(6, 2), out int day);
                            dateTimeConverted &= int.TryParse(tempDateTime.Substring(9, 2), out int hour);
                            dateTimeConverted &= int.TryParse(tempDateTime.Substring(11, 2), out int minute);
                            dateTimeConverted &= int.TryParse(tempDateTime.Substring(13, 2), out int second);
                            dateTimeConverted &= int.TryParse(tempDateTime.Substring(16), out int millisecond);

                            if (dateTimeConverted == true)
                            {
                                lastDT = new DateTime(year, month, day, hour, minute, second, millisecond);
                            }
                            else
                            {
                                lastDT = DateTime.Now;
                            }
                        }
                        catch { }
                    }
                }
            }

            result = new ActiveProductInfo(uniqueKey, product, lastDT)
            {
                PID = pid
            };

            return result;
        }

        private static int DecodeHexToInt32(string data)
        {
            int result;

            result = 0;

            if (data != null)
            {
                int.TryParse(data, System.Globalization.NumberStyles.HexNumber, null, out result);
            }

            return result;
        }

        /*
        3 + 4 + 1 + 4 + 1 + 36 + 1 + 8 + 1 + 19 + 2 = 80
        "###" + HEX_PID + "@" + HEX_IID + "@" + UK + "@" + HEX_PRODUCT + "@" + yyyyMMdd HHmmss.fff + "$$"
        0       3         7     8         12    13   49    50            58    59                    78
        */
        internal static byte[] Encode(ActiveProductInfo[] data, int offset, int count)
        {
            byte[] result;
            string temp;
            int start;
            Product product;

            result = null;

            if (data != null && data.Length == SHARED_ITEM_TOTAL_COUNT && offset >= 0 && count > 0)
            {
                result = new byte[SHARED_ITEM_LENGTH * count];

                for (int i = offset, n = offset + count; i < n; i++)
                {
                    start = (i - offset) * SHARED_ITEM_LENGTH;
                    product = ProductConverter.ConvertToProduct(data[i].ProductCode);

                    temp = $"###{data[i].PID:X4}@{data[i].IID:X4}@{data[i].UniqueKey,36}@{product.GetHashCode():X8}@{data[i].LastDT:yyyyMMdd HHmmss.fff}$$";

                    Array.Copy(Encoding.Default.GetBytes(temp), 0, result, start, SHARED_ITEM_LENGTH);
                }
            }

            return result;
        }

        internal static byte[] Encode(ActiveProductInfo data)
        {
            byte[] result;
            string temp;
            int start;
            Product product;

            result = null;

            if (data != null)
            {
                result = new byte[SHARED_ITEM_LENGTH];

                start = 0;

                product = ProductConverter.ConvertToProduct(data.ProductCode);

                temp = $"###{data.PID:X4}@{data.IID:X4}@{data.UniqueKey,36}@{product.GetHashCode():X8}@{data.LastDT:yyyyMMdd HHmmss.fff}$$";
                Array.Copy(Encoding.Default.GetBytes(temp), 0, result, start, SHARED_ITEM_LENGTH);
            }

            return result;
        }
    }
}
