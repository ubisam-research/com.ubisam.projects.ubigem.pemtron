using System;
using System.Xml.Linq;

using UbiCom.Net.Structure;

namespace UbiSam.GEM.Configration.Utility
{
    public class UmdLoader : IDisposable
    {
        private const string TAG_MESSAGE = "SECSMessage";
        private const string TAG_WRAPPER = "WrapperInfo";
        private const string TAG_HEADER = "Header";
        private const string TAG_BODY = "DataItem";

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

        private static class BodySchema
        {
            public const string ITEM_NAME = "ItemName";
            public const string COUNT = "Count";
            public const string IS_FIXED = "IsFixed";
            public const string DEFAULT_VALUE = "DefaultValue";
        }

        private static class WrapperSchema
        {
            public const string FILE_NAME = "FileName";
            public const string NAMESPACE = "Namespace";
            public const string CLASS_NAME = "ClassName";
            public const string DEVICE_TYPE = "DeviceType";
        }

        #region [Variables]
        private SECSMessageCollection _message;
        private SECSMode _secsMode;

        private string _wrapperFileName;
        private string _wrapperNamespace;
        private string _wrapperClassName;
        private DeviceType _wrapperDeviceType;
        #endregion

        #region [Properties]
        public SECSMessageCollection SECSMessage
        {
            get { return this._message; }
        }

        public SECSMode SECSMode
        {
            get { return this._secsMode; }
        }

        public string WrapperFileName
        {
            get { return this._wrapperFileName; }
        }

        public string WrapperNamespace
        {
            get { return this._wrapperNamespace; }
        }

        public string WrapperClassName
        {
            get { return this._wrapperClassName; }
        }

        public DeviceType WrapperDeviceType
        {
            get { return this._wrapperDeviceType; }
        }
        #endregion

        #region Constructor
        public UmdLoader()
        {
            this._message = null;
            this._wrapperFileName = string.Empty;
            this._wrapperNamespace = string.Empty;
            this._wrapperClassName = string.Empty;
            this._wrapperDeviceType = DeviceType.Host;
        }
        #endregion

        // Public Method
        #region Load
        public string Load(string umdFileName)
        {
            string result = string.Empty;
            XElement element;

            try
            {
                element = XElement.Load(umdFileName);

                result = ParsingUmdFile(element);
            }
            catch (Exception ex)
            {
                this._message = null;

                result = string.Format("UMD File Load Failed : {0}", ex.Message);
            }

            return result;
        }

        public string Load(XElement element)
        {
            string result = string.Empty;

            result = ParsingUmdFile(element);

            return result;
        }
        #endregion
        #region Dispose
        public void Dispose()
        {
            if (this._message != null)
            {
                this._message = null;
            }
        }
        #endregion

        // Method
        #region ParsingUmdFile
        private string ParsingUmdFile(XElement root)
        {
#if DEBUG_TRACE
            DateTime startTime = DateTime.Now;
#endif
            string result = string.Empty;
            SECSMessage message;
            XElement wrapperRoot;
            XElement wrapperElement;

            try
            {
                this._secsMode = SECSMode.HSMS;

                this._message = new SECSMessageCollection();

                var var = root.Elements(TAG_MESSAGE);

                foreach (XElement temp in var)
                {
                    message = MakeMessage(temp);

                    if (message == null)
                    {
                        result = "UMD 파일 열기에 실패했습니다.";

                        this._message = null;

                        break;
                    }
                    else
                    {
                        this._message.Add(message);
                    }
                }

                wrapperRoot = root.Element(TAG_WRAPPER);

                if (wrapperRoot != null)
                {
                    wrapperElement = wrapperRoot.Element(WrapperSchema.FILE_NAME);
                    this._wrapperFileName = (wrapperElement != null) ? wrapperElement.Value : string.Empty;

                    wrapperElement = wrapperRoot.Element(WrapperSchema.NAMESPACE);
                    this._wrapperNamespace = (wrapperElement != null) ? wrapperElement.Value : string.Empty;

                    wrapperElement = wrapperRoot.Element(WrapperSchema.CLASS_NAME);
                    this._wrapperClassName = (wrapperElement != null) ? wrapperElement.Value : string.Empty;

                    wrapperElement = wrapperRoot.Element(WrapperSchema.DEVICE_TYPE);

                    if (wrapperElement != null)
                    {
                        if (Enum.TryParse<DeviceType>(wrapperElement.Value, out this._wrapperDeviceType) == false)
                        {
                            this._wrapperDeviceType = DeviceType.Host;
                        }
                    }
                    else
                    {
                        this._wrapperDeviceType = DeviceType.Host;
                    }
                }
                else
                {
                    this._wrapperFileName = string.Empty;
                    this._wrapperNamespace = string.Empty;
                    this._wrapperClassName = string.Empty;
                    this._wrapperDeviceType = DeviceType.Host;
                }
            }
            catch (Exception ex)
            {
                this._message = null;

                result = string.Format("UMD File Load Failed : {0}", ex.Message);
            }
#if DEBUG_TRACE
            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
#endif
            return result;
        }
        #endregion
        #region MakeMessage
        private SECSMessage MakeMessage(XElement element)
        {
#if DEBUG_TRACE
            DateTime startTime = DateTime.Now;
#endif
            SECSMessage result;
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
            }
            catch (Exception ex)
            {
                result = null;

                System.Diagnostics.Debug.Print(ex.Message);
            }
#if DEBUG_TRACE
            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
#endif
            return result;
        }
        #endregion
        #region MakeHeader
        private SECSMessage MakeHeader(XElement element)
        {
#if DEBUG_TRACE
            DateTime startTime = DateTime.Now;
#endif
            SECSMessage result;

            try
            {
                result = new SECSMessage()
                {
                    Name = element.Element(HeaderSchema.MESSAGE_NAME).Value,
                    Stream = int.Parse(element.Element(HeaderSchema.STREAM).Value),
                    Function = int.Parse(element.Element(HeaderSchema.FUNCTION).Value),
                    Direction = (SECSMessageDirection)Enum.Parse(typeof(SECSMessageDirection), element.Element(HeaderSchema.DIRECTION).Value),
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
#if DEBUG_TRACE
            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
#endif
            return result;
        }
        #endregion
        #region MakeBody
        private SECSBody MakeBody(XElement element)
        {
#if DEBUG_TRACE
            DateTime startTime = DateTime.Now;
#endif
            SECSBody result;

            try
            {
                result = new SECSBody();

                foreach (XElement temp in element.Elements())
                {
                    MakeBody(result.Item, temp);
                }
            }
            catch (Exception ex)
            {
                result = null;

                System.Diagnostics.Debug.Print(ex.Message);
            }
#if DEBUG_TRACE
            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
#endif
            return result;
        }

        private string MakeBody(SECSItemCollection items, XElement element)
        {
#if DEBUG_TRACE
            DateTime startTime = DateTime.Now;
#endif
            string result;
            SECSItem item;
            XAttribute subValue;
            SECSItemFormat format;
            string[] defaultValue;

            try
            {
                format = (SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), element.Name.LocalName);

                item = new SECSItem()
                {
                    Format = (UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(SECSItemFormat), element.Name.LocalName),
                    Length = int.Parse(element.Attribute(BodySchema.COUNT).Value),
                    IsFixed = bool.Parse(element.Attribute(BodySchema.IS_FIXED).Value)
                };

                subValue = element.Attribute(BodySchema.ITEM_NAME);

                if (subValue != null)
                {
                    item.Name = subValue.Value;
                }

                if (format == SECSItemFormat.L)
                {
                    foreach (XElement temp in element.Elements())
                    {
                        MakeBody(item.SubItem, temp);
                    }

                    items.Add(item);
                }
                else if (format == SECSItemFormat.A || format == SECSItemFormat.J)
                {
                    subValue = element.Attribute(BodySchema.DEFAULT_VALUE);

                    if (subValue != null)
                    {
                        item.Value = subValue.Value;
                    }

                    items.Add(item);
                }
                else if (item.Length == 1)
                {
                    subValue = element.Attribute(BodySchema.DEFAULT_VALUE);

                    #region [Length == 1 Default Value Setting]
                    if (subValue != null)
                    {
                        switch (format)
                        {
                            case SECSItemFormat.B:
                                item.Value = byte.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.Boolean:
                                item.Value = bool.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.I1:
                                item.Value = sbyte.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.I2:
                                item.Value = short.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.I4:
                                item.Value = int.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.I8:
                                item.Value = long.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.U1:
                                item.Value = byte.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.U2:
                                item.Value = ushort.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.U4:
                                item.Value = uint.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.U8:
                                item.Value = ulong.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.F4:
                                item.Value = float.Parse(subValue.Value);
                                break;
                            case SECSItemFormat.F8:
                                item.Value = double.Parse(subValue.Value);
                                break;
                        }
                    }
                    #endregion

                    items.Add(item);
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
                            case SECSItemFormat.B:
                                {
                                    byte[] defaultelement = new byte[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = byte.Parse(defaultValue[i]);
                                    }

                                    item.Value = defaultelement;
                                }

                                break;
                            case SECSItemFormat.Boolean:
                                {
                                    bool[] defaultelement = new bool[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = bool.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.I1:
                                {
                                    sbyte[] defaultelement = new sbyte[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = sbyte.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.I2:
                                {
                                    short[] defaultelement = new short[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = short.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.I4:
                                {
                                    int[] defaultelement = new int[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = int.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.I8:
                                {
                                    long[] defaultelement = new long[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = long.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.U1:
                                {
                                    byte[] defaultelement = new byte[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = byte.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.U2:
                                {
                                    ushort[] defaultelement = new ushort[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = ushort.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.U4:
                                {
                                    uint[] defaultelement = new uint[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = uint.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.U8:
                                {
                                    ulong[] defaultelement = new ulong[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = ulong.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.F4:
                                {
                                    float[] defaultelement = new float[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = float.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                            case SECSItemFormat.F8:
                                {
                                    double[] defaultelement = new double[item.Length];

                                    for (int i = 0; i < item.Length; i++)
                                    {
                                        defaultelement[i] = double.Parse(defaultValue[i]);
                                    }
                                }

                                break;
                        }
                    }
                    #endregion

                    items.Add(item);
                }
                else if (item.Length == 0)
                {
                    items.Add(item);
                }

                result = string.Empty;
            }
            catch (Exception ex)
            {
                result = ex.Message;

                System.Diagnostics.Debug.Print(ex.Message);
            }
#if DEBUG_TRACE
            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
#endif
            return result;
        }
        #endregion
    }
}