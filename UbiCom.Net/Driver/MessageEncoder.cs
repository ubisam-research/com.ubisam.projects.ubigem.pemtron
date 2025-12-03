using System;
using System.Collections.Generic;

namespace UbiCom.Net.Driver
{
    internal class MessageEncoder : IDisposable
    {
        private const int LENGTH_LENGTH = 4;
        private const int INDEX_LENGTH = 0;
        private const int INDEX_HEADER = 4;
        private const int INDEX_BODY = 14;
        private const string CLASS_NAME = "MessageEncoder";

        #region [HeaderSchema - Class]
        internal static class HeaderSchema
        {
            public const int LENGTH_TOTAL = 10;
            public const int LENGTH_DEVICE_ID = 2;
            public const int LENGTH_SYSTEM_BYTES = 4;

            public const int INDEX_DEVICE_ID = 0;
            public const int INDEX_STREAM = 2;
            public const int INDEX_FUNCTION = 3;
            public const int INDEX_P_TYPE = 4;
            public const int INDEX_S_TYPE = 5;
            public const int INDEX_SYSTEMBYTES = 6;
        }
        #endregion

        private readonly Utility.Logger.Logger _logger;
        private Structure.SECSMessage _message;

        private bool _disposed;

        public MessageEncoder(HSMSDriver driver, Structure.SECSMessage message)
        {
            this._logger = driver._logger;
            this._message = message;

            this._disposed = false;

            if (message.ControlMessageType == Structure.ControlMessageType.DataMessage)
            {
                if (message.WaitBit == true)
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MessageEncoder",
                        string.Format("S{0}F{1}W ({2:X8})", message.Stream, message.Function, message.SystemBytes));
                }
                else
                {
                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MessageEncoder",
                        string.Format("S{0}F{1} ({2:X8})", message.Stream, message.Function, message.SystemBytes));
                }
            }
            else
            {
                this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MessageEncoder",
                    string.Format("Control Message Type={0} ({1:X8})", message.ControlMessageType, message.SystemBytes));
            }
        }

        ~MessageEncoder()
        {
            Dispose(false);
        }

        public Structure.MessageError GetEncodingData(out byte[] data)
        {
            Structure.MessageError result;
            byte[] length;
            long totalLength;

            data = null;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "GetEncodingData", string.Empty);

            try
            {
                result = MakeHeader(out byte[] header);

                if (result == Structure.MessageError.Ok)
                {
                    result = MakeBody(out byte[] body);

                    if (result == Structure.MessageError.Ok)
                    {
                        totalLength = LENGTH_LENGTH + header.Length + body.Length;

                        data = new byte[totalLength];

                        length = Utility.Converter.ConvertLong2Bytes(totalLength - LENGTH_LENGTH, LENGTH_LENGTH);

                        Array.Copy(length, 0, data, INDEX_LENGTH, LENGTH_LENGTH);
                        Array.Copy(header, 0, data, INDEX_HEADER, header.Length);

                        if (body.Length > 0)
                        {
                            Array.Copy(body, 0, data, INDEX_BODY, body.Length);
                        }

                        length = null;
                    }

                    body = null;
                }

                header = null;
            }
            catch (Exception ex)
            {
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "GetEncodingData", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "GetEncodingData", string.Format("Result={0}", result));

            return result;
        }

        public Structure.MessageError GetEncodingData(int reasonCode, out byte[] data)
        {
            Structure.MessageError result;
            byte[] length;
            long totalLength;

            data = null;

            try
            {
                result = MakeHeader(reasonCode, out byte[] header);

                if (result == Structure.MessageError.Ok)
                {
                    result = MakeBody(out byte[] body);

                    if (result == Structure.MessageError.Ok)
                    {
                        totalLength = LENGTH_LENGTH + header.Length + body.Length;

                        data = new byte[totalLength];

                        length = Utility.Converter.ConvertLong2Bytes(totalLength - LENGTH_LENGTH, LENGTH_LENGTH);

                        Array.Copy(length, 0, data, INDEX_LENGTH, LENGTH_LENGTH);
                        Array.Copy(header, 0, data, INDEX_HEADER, header.Length);

                        if (body.Length > 0)
                        {
                            Array.Copy(body, 0, data, INDEX_BODY, body.Length);
                        }

                        length = null;
                    }

                    body = null;
                }

                header = null;
            }
            catch (Exception ex)
            {
                data = null;

                result = Structure.MessageError.Unknown;

                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (this._message != null)
                        this._message = null;

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Dispose", "Encoder Dispose");
                }

                this._disposed = true;
            }
        }

        private Structure.MessageError MakeHeader(out byte[] header)
        {
            Structure.MessageError result;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeHeader", string.Empty);

            header = new byte[HeaderSchema.LENGTH_TOTAL];

            try
            {
                byte[] systemBytes;
                if (this._message.ControlMessageType == Structure.ControlMessageType.DataMessage)
                {
                    byte[] deviceId = Utility.Converter.ConvertLong2Bytes(this._message.DeviceId, HeaderSchema.LENGTH_DEVICE_ID);
                    Array.Copy(deviceId, 0, header, HeaderSchema.INDEX_DEVICE_ID, HeaderSchema.LENGTH_DEVICE_ID);

                    if (this._message.WaitBit == true)
                    {
                        header[HeaderSchema.INDEX_STREAM] = (byte)(this._message.Stream - 0x80);
                    }
                    else
                    {
                        header[HeaderSchema.INDEX_STREAM] = (byte)this._message.Stream;
                    }

                    header[HeaderSchema.INDEX_FUNCTION] = (byte)this._message.Function;
                    header[HeaderSchema.INDEX_P_TYPE] = 0;
                    header[HeaderSchema.INDEX_S_TYPE] = (byte)(this._message.ControlMessageType.GetHashCode());

                    systemBytes = Utility.Converter.ConvertLong2Bytes(this._message.SystemBytes, HeaderSchema.LENGTH_SYSTEM_BYTES);
                    Array.Copy(systemBytes, 0, header, HeaderSchema.INDEX_SYSTEMBYTES, HeaderSchema.LENGTH_SYSTEM_BYTES);
                }
                else
                {
                    header[HeaderSchema.INDEX_DEVICE_ID] = 0xff;
                    header[HeaderSchema.INDEX_DEVICE_ID + 1] = 0xff;
                    header[HeaderSchema.INDEX_STREAM] = 0;
                    header[HeaderSchema.INDEX_FUNCTION] = 0;
                    header[HeaderSchema.INDEX_P_TYPE] = 0;
                    header[HeaderSchema.INDEX_S_TYPE] = (byte)(this._message.ControlMessageType.GetHashCode());

                    systemBytes = Utility.Converter.ConvertLong2Bytes(this._message.SystemBytes, HeaderSchema.LENGTH_SYSTEM_BYTES);
                    Array.Copy(systemBytes, 0, header, HeaderSchema.INDEX_SYSTEMBYTES, HeaderSchema.LENGTH_SYSTEM_BYTES);
                }

                result = Structure.MessageError.Ok;
            }
            catch (Exception ex)
            {
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "MakeHeader", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeHeader", string.Format("Result={0}", result));

            return result;
        }

        private Structure.MessageError MakeHeader(int reasonCode, out byte[] header)
        {
            Structure.MessageError result;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeHeader", string.Empty);

            header = new byte[HeaderSchema.LENGTH_TOTAL];

            try
            {
                byte[] systemBytes;
                if (this._message.ControlMessageType == Structure.ControlMessageType.DataMessage)
                {
                    byte[] deviceId = Utility.Converter.ConvertLong2Bytes(this._message.DeviceId, HeaderSchema.LENGTH_DEVICE_ID);
                    Array.Copy(deviceId, 0, header, HeaderSchema.INDEX_DEVICE_ID, HeaderSchema.LENGTH_DEVICE_ID);

                    if (this._message.WaitBit == true)
                    {
                        header[HeaderSchema.INDEX_STREAM] = (byte)(this._message.Stream - 0x80);
                    }
                    else
                    {
                        header[HeaderSchema.INDEX_STREAM] = (byte)this._message.Stream;
                    }

                    header[HeaderSchema.INDEX_FUNCTION] = (byte)this._message.Function;
                    header[HeaderSchema.INDEX_P_TYPE] = 0;
                    header[HeaderSchema.INDEX_S_TYPE] = (byte)(this._message.ControlMessageType.GetHashCode());

                    systemBytes = Utility.Converter.ConvertLong2Bytes(this._message.SystemBytes, HeaderSchema.LENGTH_SYSTEM_BYTES);
                    Array.Copy(systemBytes, 0, header, HeaderSchema.INDEX_SYSTEMBYTES, HeaderSchema.LENGTH_SYSTEM_BYTES);
                }
                else
                {
                    header[HeaderSchema.INDEX_DEVICE_ID] = 0xff;
                    header[HeaderSchema.INDEX_DEVICE_ID + 1] = 0xff;
                    header[HeaderSchema.INDEX_STREAM] = 0;
                    header[HeaderSchema.INDEX_FUNCTION] = (byte)reasonCode;
                    header[HeaderSchema.INDEX_P_TYPE] = 0;
                    header[HeaderSchema.INDEX_S_TYPE] = (byte)(this._message.ControlMessageType.GetHashCode());

                    systemBytes = Utility.Converter.ConvertLong2Bytes(this._message.SystemBytes, HeaderSchema.LENGTH_SYSTEM_BYTES);
                    Array.Copy(systemBytes, 0, header, HeaderSchema.INDEX_SYSTEMBYTES, HeaderSchema.LENGTH_SYSTEM_BYTES);
                }

                result = Structure.MessageError.Ok;
            }
            catch (Exception ex)
            {
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "MakeHeader", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeHeader", string.Format("Result={0}", result));

            return result;
        }

        private Structure.MessageError MakeBody(out byte[] body)
        {
            Structure.MessageError result;
            long totalLength;
            int index;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeBody", string.Empty);

            try
            {
                if (this._message.Body != null && this._message.Body.Count > 0)
                //if (this._message.Body != null && this._message.Body.Item.Count > 0)
                {
                    totalLength = this._message.Body.TotalBytes;
                    index = 0;
                    body = new byte[totalLength];

                    result = MakeBody(this._message.Body.AsList, ref body, ref index);
                    //result = MakeBody(this._message.Body.Item.Items, ref body, ref index);
                }
                else
                {
                    body = new byte[0];

                    result = Structure.MessageError.Ok;
                }
            }
            catch (Exception ex)
            {
                body = null;
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "MakeBody", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeBody", string.Format("Result={0}", result));

            return result;
        }

        private Structure.MessageError MakeBody(List<Structure.SECSItem> items, ref byte[] body, ref int index)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] length;
            int format;

            try
            {
                if (items != null && items.Count > 0)
                {
                    foreach (Structure.SECSItem temp in items)
                    {
                        if (result != Structure.MessageError.Ok)
                        {
                            break;
                        }

                        if (temp.IsFixed == true)
                        {
                            length = GetLength(temp.Format, temp.Length);
                            format = GetFormatLength(temp.Format, temp.Length);
                        }
                        else
                        {
                            if (temp.Format == Structure.SECSItemFormat.L)
                            {
                                length = GetLength(temp.Format, temp.Length);
                                format = GetFormatLength(temp.Format, temp.Length);
                            }
                            else
                            {
                                if (temp.Value != null)
                                {
                                    length = GetLength(temp.Format, temp.Value.Length);
                                    format = GetFormatLength(temp.Format, temp.Value.Length);
                                }
                                else
                                {
                                    length = GetLength(temp.Format, 0);
                                    format = GetFormatLength(temp.Format, 0);
                                }
                            }
                        }

                        body[index] = (byte)format;
                        index++;
                        Array.Copy(length, 0, body, index, length.Length);
                        index += length.Length;

                        switch (temp.Format)
                        {
                            case Structure.SECSItemFormat.L:
                                continue;
                            case Structure.SECSItemFormat.B:
                                result = SetBinary(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.Boolean:
                                result = SetBoolean(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.U1:
                                result = SetU1(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.I1:
                                result = SetI1(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.A:
                                result = SetAscii(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.J:
                                continue;
                            case Structure.SECSItemFormat.I2:
                                result = SetI2(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.U2:
                                result = SetU2(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.I4:
                                result = SetI4(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.F4:
                                result = SetF4(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.U4:
                                result = SetU4(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.I8:
                                result = SetI8(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.F8:
                                result = SetF8(ref body, ref index, temp);

                                continue;
                            case Structure.SECSItemFormat.U8:
                                result = SetU8(ref body, ref index, temp);

                                continue;
                        }
                    }

                    length = null;
                }
            }
            catch (Exception ex)
            {
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "MakeBody", ex);
            }

            return result;
        }

        private static byte[] GetLength(Structure.SECSItemFormat format, int length)
        {
            int formatLength;
            byte[] source;

            switch (format)
            {
                case Structure.SECSItemFormat.I2:
                case Structure.SECSItemFormat.U2:
                    formatLength = 2;
                    break;
                case Structure.SECSItemFormat.I4:
                case Structure.SECSItemFormat.U4:
                case Structure.SECSItemFormat.F4:
                    formatLength = 4;
                    break;
                case Structure.SECSItemFormat.I8:
                case Structure.SECSItemFormat.U8:
                case Structure.SECSItemFormat.F8:
                    formatLength = 8;
                    break;
                default:
                    formatLength = 1;
                    break;
            }

            source = Utility.Converter.ConvertLong2Bytes((long)(length * formatLength), 4);

            byte[] destination;

            if (length <= 0xff)
            {
                destination = new byte[1];
            }
            else if (length <= 0xffff)
            {
                destination = new byte[2];
            }
            else
            {
                destination = new byte[3];
            }

            Array.Copy(source, source.Length - destination.Length, destination, 0, destination.Length);

            return destination;
        }

        private static int GetFormatLength(Structure.SECSItemFormat format, int length)
        {
            int result;
            int lengthSize;
            int totalLength;

            switch (format)
            {
                case Structure.SECSItemFormat.I2:
                case Structure.SECSItemFormat.U2:
                    totalLength = length * 2;
                    break;
                case Structure.SECSItemFormat.I4:
                case Structure.SECSItemFormat.F4:
                case Structure.SECSItemFormat.U4:
                    totalLength = length * 4;
                    break;

                case Structure.SECSItemFormat.I8:
                case Structure.SECSItemFormat.F8:
                case Structure.SECSItemFormat.U8:
                    totalLength = length * 8;
                    break;
                default:
                    totalLength = length;
                    break;
            }

            if (totalLength <= 0xff)
            {
                lengthSize = 1;
            }
            else if (totalLength <= 0xffff)
            {
                lengthSize = 2;
            }
            else
            {
                lengthSize = 3;
            }

            if (format == Structure.SECSItemFormat.X)
            {
                result = (Structure.SECSItemFormat.A.GetHashCode() + (lengthSize - 1));
            }
            else
            {
                result = (format.GetHashCode() + (lengthSize - 1));
            }

            return result;
        }

        private static Structure.MessageError SetAscii(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            string stringValue;
            byte[] byteValue;

            if (value.Length > 0)
            {
                byteValue = Utility.Converter.ConvertString2Bytes(value.Value);

                if (byteValue.Length == value.Length)
                {
                    Array.Copy(byteValue, 0, body, index, value.Length);
                    index += value.Length;
                }
                else if (byteValue.Length > value.Length)
                {
                    Array.Copy(byteValue, 0, body, index, value.Length);
                    index += value.Length;
                }
                else if (byteValue.Length < value.Length)
                {
                    stringValue = value.Value;

                    stringValue = stringValue.PadRight(value.Length);

                    byteValue = Utility.Converter.ConvertString2Bytes(stringValue);

                    Array.Copy(byteValue, 0, body, index, value.Length);
                    index += value.Length;
                }
            }

            return result;
        }

        private static Structure.MessageError SetBinary(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                body[index] = value.Value;

                index += value.Length;
            }
            else if (value.Length > 1)
            {
                byteValue = value.Value;

                Array.Copy(byteValue, 0, body, index, value.Length);

                index += value.Length;
            }

            return result;
        }

        private static Structure.MessageError SetBoolean(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                body[index] = Convert.ToByte((bool)value.Value);

                index += value.Length;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertBooleans2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, value.Length);

                index += value.Length;
            }

            return result;
        }

        private static Structure.MessageError SetI1(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;

            if (value.Length == 1)
            {
                body[index] = (byte)((sbyte)value.Value);

                index += value.Length;
            }
            else if (value.Length > 1)
            {
                byte[] byteValue;

                byteValue = Utility.Converter.ConvertSbytes2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetI2(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertShort2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, value.Length * 2);

                index += value.Length * 2;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertShorts2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetI4(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertInt2Bytes(value.Value, 4);

                Array.Copy(byteValue, 0, body, index, value.Length * 4);

                index += value.Length * 4;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertInts2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetI8(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertLong2Bytes(value.Value, 8);

                Array.Copy(byteValue, 0, body, index, value.Length * 8);

                index += value.Length * 8;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertLongs2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetU1(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                body[index] = (byte)value.Value;

                index += value.Length;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertSbytes2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += value.Length;
            }

            return result;
        }

        private static Structure.MessageError SetU2(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertUshort2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, value.Length * 2);

                index += value.Length * 2;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertUshorts2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetU4(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertUint2Bytes(value.Value, 4);

                Array.Copy(byteValue, 0, body, index, value.Length * 4);

                index += value.Length * 4;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertUints2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetU8(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertUlong2Bytes(value.Value, 8);

                Array.Copy(byteValue, 0, body, index, value.Length * 8);

                index += value.Length * 8;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertUlongs2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetF4(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertFloat2Bytes(value.Value, 4);

                Array.Copy(byteValue, 0, body, index, value.Length * 4);

                index += value.Length * 4;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertFloats2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }

        private static Structure.MessageError SetF8(ref byte[] body, ref int index, Structure.SECSItem value)
        {
            Structure.MessageError result = Structure.MessageError.Ok;
            byte[] byteValue;

            if (value.Length == 1)
            {
                byteValue = Utility.Converter.ConvertDouble2Bytes(value.Value, 8);

                Array.Copy(byteValue, 0, body, index, value.Length * 8);

                index += value.Length * 8;
            }
            else if (value.Length > 1)
            {
                byteValue = Utility.Converter.ConvertDoubles2Bytes(value.Value);

                Array.Copy(byteValue, 0, body, index, byteValue.Length);

                index += byteValue.Length;
            }

            return result;
        }
    }
}