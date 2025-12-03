using System;

namespace UbiCom.Net.Driver
{
    internal class MessageDecoder : IDisposable
    {
        private const string CLASS_NAME = "MessageDecoder";

        #region [HeaderSchema - Class]
        private static class HeaderSchema
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

        private RawData _rawData;

        private Structure.Configurtion _config;
        private readonly Utility.Logger.Logger _logger;
        private Structure.SECSMessage _message;

        private bool _disposed;

        public MessageDecoder(HSMSDriver driver, RawData rawData)
        {
            this._config = driver._config;
            this._logger = driver._logger;
            this._message = new Structure.SECSMessage();

            this._rawData = rawData;
            this._disposed = false;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ctor", string.Format("Length={0}", rawData.Length));
        }

        ~MessageDecoder()
        {
            Dispose(false);
        }

        public Structure.MessageError GetDecodingData(Structure.SECSMessage message)
        {
            Structure.MessageError result;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "GetDecodingData", string.Empty);

            try
            {
                result = MakeHeader(message);

                if (result == Structure.MessageError.Ok)
                {
                    if (message.ControlMessageType == Structure.ControlMessageType.DataMessage)
                    {
                        result = MakeBody(message.Body);

                        if (result == Structure.MessageError.Ok)
                        {
                            message.Length = this._rawData.Length;
                        }
                    }
                    else
                    {
                        message.Length = this._rawData.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "GetDecodingData", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "GetDecodingData", string.Format("Result={0}", result));

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
                    if (this._rawData != null)
                        this._rawData = null;

                    if (this._config != null)
                        this._config = null;

                    if (this._message != null)
                        this._message = null;

                    this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "Dispose", "Decoder Dispose");
                }

                this._disposed = true;
            }
        }

        private Structure.MessageError MakeHeader(Structure.SECSMessage message)
        {
            Structure.MessageError result;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeHeader", string.Empty);

            try
            {
                if (this._config.DeviceType == Structure.DeviceType.Equipment)
                {
                    message.Direction = Structure.SECSMessageDirection.ToEquipment;
                }
                else
                {
                    message.Direction = Structure.SECSMessageDirection.ToHost;
                }

                message.ControlMessageType = (Structure.ControlMessageType)this._rawData.Header[HeaderSchema.INDEX_S_TYPE];

                byte[] systemBytes = new byte[HeaderSchema.LENGTH_SYSTEM_BYTES];
                Array.Copy(this._rawData.Header, HeaderSchema.INDEX_SYSTEMBYTES, systemBytes, 0, HeaderSchema.LENGTH_SYSTEM_BYTES);

                systemBytes = Utility.Converter.Swap(systemBytes);

                message.SystemBytes = BitConverter.ToUInt32(systemBytes, 0);

                if (message.ControlMessageType == Structure.ControlMessageType.DataMessage)
                {
                    byte[] deviceId = new byte[HeaderSchema.LENGTH_DEVICE_ID];

                    Array.Copy(this._rawData.Header, HeaderSchema.INDEX_DEVICE_ID, deviceId, HeaderSchema.INDEX_DEVICE_ID, HeaderSchema.LENGTH_DEVICE_ID);

                    deviceId = Utility.Converter.Swap(deviceId);

                    message.DeviceId = BitConverter.ToInt16(deviceId, 0);
                    message.Stream = this._rawData.Header[HeaderSchema.INDEX_STREAM] & 0xff;

                    if (message.Stream > 0x80)
                    {
                        message.Stream -= 0x80;

                        message.WaitBit = true;
                    }

                    message.Function = this._rawData.Header[HeaderSchema.INDEX_FUNCTION] & 0xff;
                }
                else
                {
                    message.StatusCode = this._rawData.Header[HeaderSchema.INDEX_FUNCTION] & 0xff;
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

        private Structure.MessageError MakeBody(Structure.SECSBody body)
        {
            Structure.MessageError result;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeBody", string.Empty);

            try
            {
                result = ParsingData(body);

                if (result != Structure.MessageError.Ok)
                {
                    body = null;
                }
            }
            catch (Exception ex)
            {
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "MakeBody", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "MakeBody", string.Format("Result={0}", result));

            return result;
        }

        private Structure.MessageError ParsingData(Structure.SECSBody body)
        {
            Structure.MessageError result;
            Structure.SECSItem item;
            Structure.SECSItemFormat itemFormat;
            byte formatByte;
            byte[] valueBytes;
            int length;

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ParsingData", string.Empty);

            try
            {
                int index = 0;

                while (index < this._rawData.Length)
                {
                    formatByte = this._rawData.Body[index];
                    index++;

                    length = GetBinaryLength(formatByte, ref index);
                    itemFormat = GetItemFormat(formatByte);

                    item = new Structure.SECSItem()
                    {
                        Format = itemFormat
                    };

                    if (itemFormat == Structure.SECSItemFormat.L)
                    {
                        item.Value = string.Empty;
                        item.Length = length;

                        body.Add(item);
                    }
                    else
                    {
                        valueBytes = GetBinaryValue(itemFormat, length, out int realLength, ref index);

                        if (itemFormat == Structure.SECSItemFormat.A ||
                            itemFormat == Structure.SECSItemFormat.J)
                        {
                            item.Value = Utility.Converter.ConvertBytes2String(valueBytes);
                            item.Length = realLength;
                        }
                        else if (realLength == 1)
                        {
                            switch (itemFormat)
                            {
                                case Structure.SECSItemFormat.B:
                                    item.Value = valueBytes[0];
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.Boolean:
                                    item.Value = valueBytes[0] & 0xff;
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I1:
                                    item.Value = (sbyte)valueBytes[0];
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I2:
                                    item.Value = Utility.Converter.ConvertBytes2Short(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I4:
                                    item.Value = Utility.Converter.ConvertBytes2Int(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I8:
                                    item.Value = Utility.Converter.ConvertBytes2Long(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U1:
                                    item.Value = valueBytes[0];
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U2:
                                    item.Value = Utility.Converter.ConvertBytes2Ushort(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U4:
                                    item.Value = Utility.Converter.ConvertBytes2Uint(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U8:
                                    item.Value = Utility.Converter.ConvertBytes2Ulong(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.F4:
                                    item.Value = Utility.Converter.ConvertBytes2Float(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.F8:
                                    item.Value = Utility.Converter.ConvertBytes2Double(valueBytes);
                                    item.Length = realLength;
                                    break;
                            }
                        }
                        else
                        {
                            switch (itemFormat)
                            {
                                case Structure.SECSItemFormat.B:
                                    item.Value = valueBytes;
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.Boolean:
                                    item.Value = Utility.Converter.ConvertBytes2BooleanArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I1:
                                    item.Value = Utility.Converter.ConvertBytes2SbyteArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I2:
                                    item.Value = Utility.Converter.ConvertBytes2ShortArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I4:
                                    item.Value = Utility.Converter.ConvertBytes2IntArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.I8:
                                    item.Value = Utility.Converter.ConvertBytes2LongArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U1:
                                    item.Value = valueBytes;
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U2:
                                    item.Value = Utility.Converter.ConvertBytes2UshortArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U4:
                                    item.Value = Utility.Converter.ConvertBytes2UintArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.U8:
                                    item.Value = Utility.Converter.ConvertBytes2UlongArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.F4:
                                    item.Value = Utility.Converter.ConvertBytes2FloatArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                                case Structure.SECSItemFormat.F8:
                                    item.Value = Utility.Converter.ConvertBytes2DoubleArray(valueBytes);
                                    item.Length = realLength;
                                    break;
                            }
                        }

                        body.Add(item);
                    }
                }

                result = Structure.MessageError.Ok;
            }
            catch (Exception ex)
            {
                result = Structure.MessageError.Unknown;

                this._logger.WriteException(DateTime.Now, CLASS_NAME, "ParsingData", ex);
            }

            this._logger.WriteDriver(DateTime.Now, Utility.Logger.LogLevel.Information, CLASS_NAME, "ParsingData", string.Format("Result={0}", result));

            return result;
        }

        private int GetBinaryLength(byte format, ref int index)
        {
            int result;
            int length;
            byte[] destination;

            switch (format)
            {
                case 0x01: // L
                case 0x21: // B
                case 0x25: // Boolean
                case 0x41: // A
                case 0x45: // J
                case 0x61: // I8
                case 0x65: // I1
                case 0x69: // I2
                case 0x71: // I4
                case 0x81: // F8
                case 0x91: // F4
                case 0xa1: // U8
                case 0xa5: // U1
                case 0xa9: // U2
                case 0xb1: // U4
                    length = 1;
                    break;
                case 0x02: // L
                case 0x22: // B
                case 0x26: // Boolean
                case 0x42: // A
                case 0x46: // J
                case 0x62: // I8
                case 0x66: // I1
                case 0x6a: // I2
                case 0x72: // I4
                case 0x82: // F8
                case 0x92: // F4
                case 0xa2: // U8
                case 0xa6: // U1
                case 0xaa: // U2
                case 0xb2: // U4
                    length = 2;
                    break;
                case 0x03: // L
                case 0x23: // B
                case 0x27: // Boolean
                case 0x43: // A
                case 0x47: // J
                case 0x63: // I8
                case 0x67: // I1
                case 0x6b: // I2
                case 0x73: // I4
                case 0x83: // F8
                case 0x93: // F4
                case 0xa3: // U8
                case 0xa7: // U1
                case 0xab: // U2
                case 0xb3: // U4
                    length = 3;
                    break;
                default:
                    length = 0;
                    break;
            }

            destination = new byte[length];

            Array.Copy(this._rawData.Body, index, destination, 0, length);

            index += length;

            result = Utility.Converter.ConvertBytes2Int(destination, length);

            return result;
        }

        private static Structure.SECSItemFormat GetItemFormat(byte format)
        {
            Structure.SECSItemFormat result;

            switch (format)
            {
                case 0x01:
                case 0x02:
                case 0x03:
                    result = Structure.SECSItemFormat.L;
                    break;
                case 0x21:
                case 0x22:
                case 0x23:
                    result = Structure.SECSItemFormat.B;
                    break;
                case 0x25:
                case 0x26:
                case 0x27:
                    result = Structure.SECSItemFormat.Boolean;
                    break;
                case 0x41:
                case 0x42:
                case 0x43:
                    result = Structure.SECSItemFormat.A;
                    break;
                case 0x45:
                case 0x46:
                case 0x47:
                    result = Structure.SECSItemFormat.J;
                    break;
                case 0x61:
                case 0x62:
                case 0x63:
                    result = Structure.SECSItemFormat.I8;
                    break;
                case 0x65:
                case 0x66:
                case 0x67:
                    result = Structure.SECSItemFormat.I1;
                    break;
                case 0x69:
                case 0x6a:
                case 0x6b:
                    result = Structure.SECSItemFormat.I2;
                    break;
                case 0x71:
                case 0x72:
                case 0x73:
                    result = Structure.SECSItemFormat.I4;
                    break;
                case 0x81:
                case 0x82:
                case 0x83:
                    result = Structure.SECSItemFormat.F8;
                    break;
                case 0x91:
                case 0x92:
                case 0x93:
                    result = Structure.SECSItemFormat.F4;
                    break;
                case 0xa1:
                case 0xa2:
                case 0xa3:
                    result = Structure.SECSItemFormat.U8;
                    break;
                case 0xa5:
                case 0xa6:
                case 0xa7:
                    result = Structure.SECSItemFormat.U1;
                    break;
                case 0xa9:
                case 0xaa:
                case 0xab:
                    result = Structure.SECSItemFormat.U2;
                    break;
                case 0xb1:
                case 0xb2:
                case 0xb3:
                    result = Structure.SECSItemFormat.U4;
                    break;
                default:
                    result = Structure.SECSItemFormat.L;
                    break;
            }

            return result;
        }

        private byte[] GetBinaryValue(Structure.SECSItemFormat itemFormat, int length, out int realLength, ref int index)
        {
            byte[] result;

            if (length > 0)
            {
                realLength = GetValueLength(itemFormat, length);

                result = new byte[length];

                Array.Copy(this._rawData.Body, index, result, 0, length);

                index += length;
            }
            else
            {
                realLength = 0;
                result = null;
            }

            return result;
        }

        private static int GetValueLength(Structure.SECSItemFormat format, int length)
        {
            int result;

            switch (format)
            {
                case Structure.SECSItemFormat.I2:
                case Structure.SECSItemFormat.U2:
                    result = length / 2;
                    break;
                case Structure.SECSItemFormat.I4:
                case Structure.SECSItemFormat.F4:
                case Structure.SECSItemFormat.U4:
                    result = length / 4;
                    break;

                case Structure.SECSItemFormat.I8:
                case Structure.SECSItemFormat.F8:
                case Structure.SECSItemFormat.U8:
                    result = length / 8;
                    break;
                default:
                    result = length;
                    break;
            }

            return result;
        }
    }
}