using System.Collections.Generic;
using System.Linq;

namespace UbiCom.Net.Structure
{
    /// <summary>
    /// SECS Meesage입니다.
    /// </summary>
    public class SECSMessage
    {
        //#region Constants
        //private const int LENGTH_MESSAGE_HEADER = 14;
        //private const int LENGTH_TOTAL = 10;
        //private const int LENGTH_DEVICE_ID = 2;
        //private const int LENGTH_SYSTEM_BYTES = 4;

        //private const int INDEX_DEVICE_ID = 0;
        //private const int INDEX_STREAM = 2;
        //private const int INDEX_FUNCTION = 3;
        //private const int INDEX_P_TYPE = 4;
        //private const int INDEX_S_TYPE = 5;
        //private const int INDEX_SYSTEMBYTES = 6;
        //#endregion

        /// <summary>
        /// SECS Message의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Control Message Type을 가져오거나 설정합니다.
        /// </summary>
        public ControlMessageType ControlMessageType { get; set; }

        /// <summary>
        /// Direction을 가져오거나 설정합니다.
        /// </summary>
        public SECSMessageDirection Direction { get; set; }

        /// <summary>
        /// Device ID를 가져오거나 설정합니다.
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Description을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Stream No을 가져오거나 설정합니다.
        /// </summary>
        public int Stream { get; set; }

        /// <summary>
        /// Function No를 가져오거나 설정합니다.
        /// </summary>
        public int Function { get; set; }

        /// <summary>
        /// Wait Bit 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool WaitBit { get; set; }

        /// <summary>
        /// Auto Reply 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool AutoReply { get; set; }

        /// <summary>
        /// Logging 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool NoLogging { get; set; }

        /// <summary>
        /// System Bytes를 가져오거나 설정합니다.
        /// </summary>
        public uint SystemBytes { get; set; }

        /// <summary>
        /// Status / Reason Code를 가져오거나 설정합니다.(Select Status / Deselect Status / Reject Reason)
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// SECS Message의 Body를 가져오거나 설정합니다.
        /// </summary>
        public SECSBody Body { get; set; }

        /// <summary>
        /// SECS Message의 전체 길이를 가져오거나 설정합니다.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 사용자 정의 Data를 가져오거나 설정합니다.
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessage()
        {
            this.Body = new SECSBody();
            this.UserData = null;

            this.Name = string.Empty;
            this.ControlMessageType = Structure.ControlMessageType.DataMessage;
            this.Direction = SECSMessageDirection.Both;
            this.DeviceId = -1;
            this.Description = string.Empty;
            this.Stream = -1;
            this.Function = -1;
            this.WaitBit = false;
            this.AutoReply = false;
            this.NoLogging = false;
            this.SystemBytes = 0;
            this.StatusCode = 0;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            if (this.ControlMessageType == Structure.ControlMessageType.DataMessage)
            {
                if (this.SystemBytes > 0)
                {
                    if (this.Direction == SECSMessageDirection.ToEquipment)
                        return string.Format("S{0}F{1}(H->E)  : {2} [SystemBytes={3:X8}]", this.Stream, this.Function, this.Name, this.SystemBytes);
                    else if (this.Direction == SECSMessageDirection.ToHost)
                        return string.Format("S{0}F{1}(E->H)  : {2} [SystemBytes={3:X8}]", this.Stream, this.Function, this.Name, this.SystemBytes);
                    else
                        return string.Format("S{0}F{1}(H<->E) : {2} [SystemBytes={3:X8}]", this.Stream, this.Function, this.Name, this.SystemBytes);
                }
                else
                {
                    if (this.Direction == SECSMessageDirection.ToEquipment)
                        return string.Format("S{0}F{1}(H->E)  : {2}", this.Stream, this.Function, this.Name);
                    else if (this.Direction == SECSMessageDirection.ToHost)
                        return string.Format("S{0}F{1}(E->H)  : {2}", this.Stream, this.Function, this.Name);
                    else
                        return string.Format("S{0}F{1}(H<->E) : {2}", this.Stream, this.Function, this.Name);
                }
            }
            else
            {
                if (this.SystemBytes > 0)
                {
                    return string.Format("{0} [SystemBytes={1:X8}]", this.ControlMessageType, this.SystemBytes);
                }
                else
                {
                    return string.Format("{0}", this.ControlMessageType);
                }
            }
        }

        /// <summary>
        /// SECS Message의 단순 복사본을 만듭니다.(단, User Data는 복사하지 않음)
        /// </summary>
        /// <returns>SECS Message의 단순 복사본입니다.</returns>
        public SECSMessage Clone()
        {
            return new SECSMessage()
            {
                Name = this.Name,
                ControlMessageType = this.ControlMessageType,
                Direction = this.Direction,
                DeviceId = this.DeviceId,
                Description = this.Description,
                Stream = this.Stream,
                Function = this.Function,
                WaitBit = this.WaitBit,
                AutoReply = this.AutoReply,
                NoLogging = this.NoLogging,
                SystemBytes = this.SystemBytes,
                StatusCode = this.StatusCode,
                Body = this.Body.Clone(),
                Length = this.Length
            };
        }
    }

    /// <summary>
    /// SECS Meesage Collection입니다.
    /// </summary>
    public class SECSMessageCollection
    {
        private readonly Dictionary<string, SECSMessage> _messageInfo;

        /// <summary>
        /// SECS Message를 가져옵니다.
        /// </summary>
        public KeyValuePair<string, SECSMessage>[] MessageInfo
        {
            get
            {
                return this._messageInfo.ToArray();
            }
        }

        /// <summary>
        /// 지정한 이름과 연결된 SECS Message를 가져오거나 설정합니다.
        /// </summary>
        /// <param name="name">가져올 SECS Messag의 이름입니다.</param>
        /// <returns>지정한 이름과 연결된 SECS Messag입니다.</returns>
        public SECSMessage this[string name]
        {
            get { return this._messageInfo.ContainsKey(name) == true ? this._messageInfo[name] : null; }
            set
            {
                if (this._messageInfo.ContainsKey(name) == true)
                {
                    this._messageInfo[name] = value;
                }
            }
        }

        /// <summary>
        /// 지정한 Stream No / Function No와 연결된 SECS Message을 가져옵니다.
        /// </summary>
        /// <param name="stream">가져올 SECS Messag의 Stream No입니다.</param>
        /// <param name="function">가져올 SECS Messag의 Function No입니다.</param>
        /// <returns>지정한 Stream No / Function No와 연결된 SECS Messag입니다.</returns>
        public SECSMessageCollection this[int stream, int function]
        {
            get
            {
                SECSMessageCollection result = new SECSMessageCollection();

                var var = from SECSMessage temp in this._messageInfo.Values
                          where temp.Stream == stream &&
                                temp.Function == function
                          select temp;

                foreach (SECSMessage temp in var)
                {
                    result.Add(temp);
                }

                return result;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessageCollection()
        {
            this._messageInfo = new Dictionary<string, SECSMessage>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Total Count={0}", this._messageInfo.Count);
        }

        /// <summary>
        /// SECS Message를 추가합니다.
        /// </summary>
        /// <param name="secsMessage">추가할 SECS Message입니다.</param>
        public void Add(SECSMessage secsMessage)
        {
            if (this._messageInfo.ContainsKey(secsMessage.Name) == true)
            {
                this._messageInfo.Remove(secsMessage.Name);
            }

            this._messageInfo.Add(secsMessage.Name, secsMessage);
        }

        /// <summary>
        /// SECS Message의 모든 Data를 초기화합니다.
        /// </summary>
        public void Clear()
        {
            this._messageInfo.Clear();
        }

        /// <summary>
        /// SECS Message Collection의 단순 복사본을 만듭니다.
        /// </summary>
        /// <returns>SECS Message Collection의 단순 복사본입니다.</returns>
        public SECSMessageCollection Clone()
        {
            SECSMessageCollection result = new SECSMessageCollection();

            foreach (KeyValuePair<string, SECSMessage> temp in this._messageInfo)
            {
                result.Add(temp.Value.Clone());
            }

            return result;
        }

        /// <summary>
        /// Message의 존재 여부입니다.
        /// </summary>
        /// <param name="stream">검색할 Stream No입니다.</param>
        /// <param name="function">검색할 Function No입니다.</param>
        /// <param name="deviceType">검색할 Device Type입니다.</param>
        /// <returns></returns>
        public bool Exist(int stream, int function, DeviceType deviceType)
        {
            bool result;

            if (deviceType == DeviceType.Host)
            {
                var varMessage = from SECSMessage tempMessage in this._messageInfo.Values
                                 where tempMessage.Stream == stream &&
                                       tempMessage.Function == function &&
                                       tempMessage.Direction != SECSMessageDirection.ToHost
                                 select tempMessage;

                if (varMessage != null && varMessage.Any() == true)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                var varMessage = from SECSMessage tempMessage in this._messageInfo.Values
                                 where tempMessage.Stream == stream &&
                                       tempMessage.Function == function &&
                                       tempMessage.Direction != SECSMessageDirection.ToEquipment
                                 select tempMessage;

                if (varMessage != null && varMessage.Any() == true)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Message를 가져옵니다.
        /// </summary>
        /// <param name="stream">검색할 Stream No입니다.</param>
        /// <param name="function">검색할 Function No입니다.</param>
        /// <param name="deviceType">검색할 Device Type입니다.</param>
        /// <returns></returns>
        public SECSMessage GetMessage(int stream, int function, DeviceType deviceType)
        {
            SECSMessage result;

            if (deviceType == DeviceType.Host)
            {
                result = (from SECSMessage tempMessage in this._messageInfo.Values
                          where tempMessage.Stream == stream &&
                                tempMessage.Function == function &&
                                tempMessage.Direction != SECSMessageDirection.ToHost
                          select tempMessage).FirstOrDefault();
            }
            else
            {
                result = (from SECSMessage tempMessage in this._messageInfo.Values
                          where tempMessage.Stream == stream &&
                                tempMessage.Function == function &&
                                tempMessage.Direction != SECSMessageDirection.ToEquipment
                          select tempMessage).FirstOrDefault();
            }

            return result;
        }

        /// <summary>
        /// Message Header를 설정합니다.
        /// </summary>
        /// <param name="stream">검색할 Stream No입니다.</param>
        /// <param name="function">검색할 Function No입니다.</param>
        /// <returns></returns>
        public SECSMessage GetMessageHeader(int stream, int function)
        {
            SECSMessage message;

            var varMessage = (from SECSMessage tempMessage in this._messageInfo.Values
                              where tempMessage.Stream == stream &&
                                    tempMessage.Function == function
                              select tempMessage).FirstOrDefault();

            if (varMessage != null)
            {
                message = new SECSMessage()
                {
                    AutoReply = varMessage.AutoReply,
                    ControlMessageType = varMessage.ControlMessageType,
                    Description = varMessage.Description,
                    DeviceId = varMessage.DeviceId,
                    Direction = varMessage.Direction,
                    Function = varMessage.Function,
                    Name = varMessage.Name,
                    NoLogging = varMessage.NoLogging,
                    Stream = varMessage.Stream,
                    WaitBit = varMessage.WaitBit
                };
            }
            else
            {
                message = null;
            }

            return message;
        }

        /// <summary>
        /// Message Header를 설정합니다.
        /// </summary>
        /// <param name="stream">검색할 Stream No입니다.</param>
        /// <param name="function">검색할 Function No입니다.</param>
        /// <param name="deviceType">검색할 Device Type입니다.</param>
        /// <returns></returns>
        public SECSMessage GetMessageHeader(int stream, int function, DeviceType deviceType)
        {
            SECSMessage message;
            SECSMessage findMessage;

            if (deviceType == DeviceType.Host)
            {
                findMessage = (from SECSMessage tempMessage in this._messageInfo.Values
                               where tempMessage.Stream == stream &&
                                     tempMessage.Function == function &&
                                     tempMessage.Direction != SECSMessageDirection.ToHost
                               select tempMessage).FirstOrDefault();
            }
            else
            {
                findMessage = (from SECSMessage tempMessage in this._messageInfo.Values
                               where tempMessage.Stream == stream &&
                                     tempMessage.Function == function &&
                                     tempMessage.Direction != SECSMessageDirection.ToEquipment
                               select tempMessage).FirstOrDefault();
            }

            if (findMessage != null)
            {
                message = new SECSMessage()
                {
                    AutoReply = findMessage.AutoReply,
                    ControlMessageType = findMessage.ControlMessageType,
                    Description = findMessage.Description,
                    DeviceId = findMessage.DeviceId,
                    Direction = findMessage.Direction,
                    Function = findMessage.Function,
                    Name = findMessage.Name,
                    NoLogging = findMessage.NoLogging,
                    Stream = findMessage.Stream,
                    WaitBit = findMessage.WaitBit
                };
            }
            else
            {
                message = null;
            }

            return message;
        }

        /// <summary>
        /// Message Header를 설정합니다.
        /// </summary>
        /// <param name="messageName">검색할 SECS Messag의 이름입니다.</param>
        /// <returns></returns>
        public SECSMessage GetMessageHeader(string messageName)
        {
            SECSMessage message;

            var varMessage = (from SECSMessage tempMessage in this._messageInfo.Values
                              where tempMessage.Name == messageName
                              select tempMessage).FirstOrDefault();

            if (varMessage != null)
            {
                message = new SECSMessage()
                {
                    AutoReply = varMessage.AutoReply,
                    ControlMessageType = varMessage.ControlMessageType,
                    Description = varMessage.Description,
                    DeviceId = varMessage.DeviceId,
                    Direction = varMessage.Direction,
                    Function = varMessage.Function,
                    Name = varMessage.Name,
                    NoLogging = varMessage.NoLogging,
                    Stream = varMessage.Stream,
                    WaitBit = varMessage.WaitBit
                };
            }
            else
            {
                message = null;
            }

            return message;
        }

        /// <summary>
        /// Message Header를 설정합니다.
        /// </summary>
        /// <param name="messageName">검색할 SECS Messag의 이름입니다.</param>
        /// <param name="deviceType">검색할 Device Type입니다.</param>
        /// <returns></returns>
        public SECSMessage GetMessageHeader(string messageName, DeviceType deviceType)
        {
            SECSMessage message;
            SECSMessage findMessage;

            if (deviceType == DeviceType.Host)
            {
                findMessage = (from SECSMessage tempMessage in this._messageInfo.Values
                               where tempMessage.Name == messageName &&
                                     tempMessage.Direction != SECSMessageDirection.ToHost
                               select tempMessage).FirstOrDefault();
            }
            else
            {
                findMessage = (from SECSMessage tempMessage in this._messageInfo.Values
                               where tempMessage.Name == messageName &&
                                     tempMessage.Direction != SECSMessageDirection.ToEquipment
                               select tempMessage).FirstOrDefault();
            }

            if (findMessage != null)
            {
                message = new SECSMessage()
                {
                    AutoReply = findMessage.AutoReply,
                    ControlMessageType = findMessage.ControlMessageType,
                    Description = findMessage.Description,
                    DeviceId = findMessage.DeviceId,
                    Direction = findMessage.Direction,
                    Function = findMessage.Function,
                    Name = findMessage.Name,
                    NoLogging = findMessage.NoLogging,
                    Stream = findMessage.Stream,
                    WaitBit = findMessage.WaitBit
                };
            }
            else
            {
                message = null;
            }

            return message;
        }
    }
}