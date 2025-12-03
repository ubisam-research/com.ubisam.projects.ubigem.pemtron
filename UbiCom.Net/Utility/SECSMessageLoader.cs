using System;
using System.Xml.Linq;

using UbiCom.Net.Structure;

namespace UbiCom.Net.Utility
{
    /// <summary>
    /// SECS Message loader입니다.
    /// </summary>
    internal class SECSMessageLoader
    {
        private const string TAG_MESSAGE = "SECSMessage";
        private const string TAG_HEADER = "Header";
        private const string TAG_BODY = "DataItem";

        #region [Class - HeaderSchema]
        private static class HeaderSchema
        {
            public const string MESSAGE_NAME = "MessageName";
            public const string STREAM = "Stream";
            public const string FUNCTION = "Function";
            public const string DIRECTION = "Direction";
            public const string WAIT_BIT = "WaitBit";
            public const string AUTO_REPLY = "AutoReply";
            public const string NO_LOGGING = "NoLogging";
            public const string DESCRIPTION = "Description";
        }
        #endregion
        #region [Class - BodySchema]
        private static class BodySchema
        {
            public const string ITEM_NAME = "ItemName";
            public const string COUNT = "Count";
            public const string IS_FIXED = "IsFixed";
            public const string DEFAULT_VALUE = "DefaultValue";
            public const string DATA_DICTIONARY = "DataDictionary";
        }
        #endregion

        private SECSMessageCollection _messageInfo;

        /// <summary>
        /// SECS Message 정보를 가져옵니다.
        /// </summary>
        public SECSMessageCollection Message
        {
            get { return this._messageInfo; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessageLoader()
        {
            this._messageInfo = new SECSMessageCollection();
        }

        /// <summary>
        /// SECS message를 load합니다.
        /// </summary>
        /// <param name="umdFileName">UbiCom message definition file입니다.</param>
        /// <returns>처리 결과입니다.(OK시 string.Empty)</returns>
        public string Load(string umdFileName)
        {
            string result;
            XElement root;

            if (System.IO.File.Exists(umdFileName) == false)
            {
                this._messageInfo = null;

                result = Resources.ErrorString.DoseNotExistUMDFile;
            }
            else
            {
                root = XElement.Load(umdFileName);

                result = Load(root);
            }

            return result;
        }

        /// <summary>
        /// SECS message를 load합니다.
        /// </summary>
        /// <param name="rootElement">Message 구조의 root element입니다.</param>
        /// <returns>처리 결과입니다.(OK시 string.Empty)</returns>
        public string Load(XElement rootElement)
        {
            string result = string.Empty;
            Structure.SECSMessage message;

            try
            {
                message = null;

                var var = rootElement.Elements(TAG_MESSAGE);

                foreach (XElement temp in var)
                {
                    message = MakeMessage(temp);

                    if (message == null)
                    {
                        result = Resources.ErrorString.FailedUMDFileLoad;

                        this._messageInfo = null;

                        break;
                    }
                    else
                    {
                        this._messageInfo.Add(message);
                    }
                }

                message = null;
            }
            catch (Exception ex)
            {
                result = string.Format("Message Load Failed : {0}", ex.Message);
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                if (this._messageInfo.MessageInfo.Length <= 0)
                {
                    result = Resources.ErrorString.DoseNotExistDefinedMessageSet;
                }
            }
            else
            {
                this._messageInfo = null;
            }

            return result;
        }

        private Structure.SECSMessage MakeMessage(XElement element)
        {
            Structure.SECSMessage result;
            XElement headerElement;
            XElement bodyElement;

            try
            {
                headerElement = element.Element(TAG_HEADER);
                bodyElement = element.Element(TAG_BODY);

                if (headerElement != null && bodyElement != null)
                {
                    result = MakeHeader(headerElement);

                    if (result != null)
                    {
                        result.Body = MakeBody(bodyElement);
                    }
                }
                else
                {
                    result = null;
                }

                headerElement = null;
                bodyElement = null;
            }
            catch (Exception ex)
            {
                result = null;

                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        private static Structure.SECSMessage MakeHeader(XElement element)
        {
            Structure.SECSMessage result;

            try
            {
                result = new Structure.SECSMessage()
                {
                    Name = element.Element(HeaderSchema.MESSAGE_NAME).Value,
                    Stream = int.Parse(element.Element(HeaderSchema.STREAM).Value),
                    Function = int.Parse(element.Element(HeaderSchema.FUNCTION).Value),
                    Direction = (Structure.SECSMessageDirection)Enum.Parse(typeof(Structure.SECSMessageDirection), element.Element(HeaderSchema.DIRECTION).Value),
                    WaitBit = bool.Parse(element.Element(HeaderSchema.WAIT_BIT).Value),
                    AutoReply = bool.Parse(element.Element(HeaderSchema.AUTO_REPLY).Value),
                    NoLogging = bool.Parse(element.Element(HeaderSchema.NO_LOGGING).Value),
                    Description = element.Element(HeaderSchema.DESCRIPTION).Value
                };
            }
            catch (Exception ex)
            {
                result = null;

                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        private Structure.SECSBody MakeBody(XElement element)
        {
            Structure.SECSBody result;

            try
            {
                result = new Structure.SECSBody();

                foreach (XElement temp in element.Elements())
                {
                    MakeBody(result, temp);
                }
            }
            catch (Exception ex)
            {
                result = null;

                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        private string MakeBody(Structure.SECSBody body, XElement element)
        {
            string result;
            Structure.SECSItem item;
            XAttribute subValue;
            Structure.SECSItemFormat format;
            string[] defaultValue;

            try
            {
                format = (Structure.SECSItemFormat)Enum.Parse(typeof(Structure.SECSItemFormat), element.Name.LocalName);

                item = new Structure.SECSItem()
                {
                    Format = (Structure.SECSItemFormat)Enum.Parse(typeof(Structure.SECSItemFormat), element.Name.LocalName),
                    Length = int.Parse(element.Attribute(BodySchema.COUNT).Value),
                    IsFixed = bool.Parse(element.Attribute(BodySchema.IS_FIXED).Value)
                };

                subValue = element.Attribute(BodySchema.ITEM_NAME);

                if (subValue != null)
                    item.Name = subValue.Value;

                if (format == Structure.SECSItemFormat.L)
                {
                    body.Add(item);

                    foreach (XElement temp in element.Elements())
                    {
                        MakeBody(body, temp);
                    }
                }
                else if (format == Structure.SECSItemFormat.A || format == Structure.SECSItemFormat.J)
                {
                    subValue = element.Attribute(BodySchema.DEFAULT_VALUE);

                    if (subValue != null)
                    {
                        item.Value = subValue.Value;
                    }

                    body.Add(item);
                }
                else if (item.Length == 1)
                {
                    subValue = element.Attribute(BodySchema.DEFAULT_VALUE);

                    #region [Length == 1 Default Value Setting]
                    if (subValue != null)
                    {
                        switch (format)
                        {
                            case Structure.SECSItemFormat.B:
                                item.Value = byte.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.Boolean:
                                item.Value = bool.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.I1:
                                item.Value = sbyte.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.I2:
                                item.Value = short.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.I4:
                                item.Value = int.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.I8:
                                item.Value = Int64.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.U1:
                                item.Value = byte.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.U2:
                                item.Value = ushort.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.U4:
                                item.Value = uint.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.U8:
                                item.Value = UInt64.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.F4:
                                item.Value = float.Parse(subValue.Value);
                                break;
                            case Structure.SECSItemFormat.F8:
                                item.Value = double.Parse(subValue.Value);
                                break;
                        }
                    }
                    #endregion

                    body.Add(item);
                }
                else if (item.Length > 1)
                {
                    subValue = element.Attribute(BodySchema.DEFAULT_VALUE);

                    #region [Length > 1 Default Value Setting]
                    if (subValue != null)
                    {
                        defaultValue = subValue.Value.Split(' ');

                        switch (format)
                        {
                            case Structure.SECSItemFormat.B:
                                {
                                    byte[] defaultElement = new byte[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = byte.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.Boolean:
                                {
                                    bool[] defaultElement = new bool[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = bool.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.I1:
                                {
                                    sbyte[] defaultElement = new sbyte[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = sbyte.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.I2:
                                {
                                    short[] defaultElement = new short[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = short.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.I4:
                                {
                                    int[] defaultElement = new int[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = int.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.I8:
                                {
                                    Int64[] defaultElement = new Int64[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = Int64.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.U1:
                                {
                                    byte[] defaultElement = new byte[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = byte.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.U2:
                                {
                                    ushort[] defaultElement = new ushort[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = ushort.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.U4:
                                {
                                    uint[] defaultElement = new uint[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = uint.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.U8:
                                {
                                    UInt64[] defaultElement = new UInt64[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = UInt64.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.F4:
                                {
                                    float[] defaultElement = new float[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = float.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                            case Structure.SECSItemFormat.F8:
                                {
                                    double[] defaultElement = new double[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultElement[i] = double.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultElement;
                                }

                                break;
                        }

                        defaultValue = null;
                    }
                    #endregion

                    body.Add(item);
                }
                else
                {
                    body.Add(item);
                }

                item = null;
                subValue = null;
                defaultValue = null;

                result = string.Empty;
            }
            catch (Exception ex)
            {
                result = Resources.ErrorString.FailedUMDFileLoad;

                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }
    }
}