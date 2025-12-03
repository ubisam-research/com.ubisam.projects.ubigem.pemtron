using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiCom.Net.Structure
{
    internal class SECSMessageValidate
    {
        #region [ValidateItem - Class]
        private class ValidateItem
        {
            public string Name { get; set; }
            public SECSItemFormat Format { get; set; }
            public bool IsFixed { get; set; }
            public int Length { get; set; }
            public List<ValidateItem> SubItem { get; set; }

            public ValidateItem()
            {
                this.SubItem = new List<ValidateItem>();
            }

            /// <summary>
            /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
            /// </summary>
            /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
            public override string ToString()
            {
                string result;

                if (this.Format == SECSItemFormat.L)
                {
                    result = string.Format("L, {0}", this.Length);
                }
                else
                {
                    if (this.IsFixed == true)
                    {
                        result = string.Format("{0}, {1}", this.Format, this.Length);
                    }
                    else
                    {
                        result = string.Format("{0}, {1}v", this.Format, this.Length);
                    }
                }

                return result;
            }

            public int GetChildLength(bool exceptListLength = true)
            {
                int result;

                result = 0;

                GetTotalLength(this, exceptListLength, ref result);

                return result;
            }

            private void GetTotalLength(ValidateItem item, bool exceptListLength, ref int totalLength)
            {
                if (item.Format == SECSItemFormat.L)
                {
                    foreach (ValidateItem temp in item.SubItem)
                    {
                        GetTotalLength(temp, exceptListLength, ref totalLength);
                    }

                    if (exceptListLength == false)
                    {
                        totalLength += item.Length;
                    }
                }
                else
                {
                    totalLength += item.Length;
                }
            }
        }
        #endregion

        #region [ValidateMessage - Class]
        private class ValidateMessage
        {
            public string Name { get; set; }
            public SECSMessageDirection Direction { get; set; }
            public int DeviceId { get; set; }
            public bool WaitBit { get; set; }
            public int Stream { get; set; }
            public int Function { get; set; }
            public bool AutoReply { get; set; }
            public List<ValidateItem> Item;

            public ValidateMessage(SECSMessage message)
            {
                this.Item = new List<ValidateItem>();

                MakeHeader(message);
                MakeBody(message);
            }

            /// <summary>
            /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
            /// </summary>
            /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
            public override string ToString()
            {
                if (this.Direction == SECSMessageDirection.ToEquipment)
                    return string.Format("S{0}F{1}(Host -> EQP)", this.Stream, this.Function);
                else if (this.Direction == SECSMessageDirection.ToHost)
                    return string.Format("S{0}F{1}(EQP -> Host)", this.Stream, this.Function);
                else
                    return string.Format("S{0}F{1}(Host <-> EQP)", this.Stream, this.Function);
            }

            private void MakeHeader(SECSMessage message)
            {
                this.Name = message.Name;
                this.DeviceId = message.DeviceId;
                this.Direction = message.Direction;
                this.WaitBit = message.WaitBit;
                this.Stream = message.Stream;
                this.Function = message.Function;
                this.AutoReply = message.AutoReply;
            }

            private void MakeBody(SECSMessage message)
            {
                int currentIndex = 0;
                int itemCount;
                SECSItem item;
                ValidateItem validateItem;

                itemCount = message.Body.Count;

                while (currentIndex < itemCount)
                {
                    item = message.Body.AsList[currentIndex];

                    if (item.Format == SECSItemFormat.L)
                    {
                        validateItem = new ValidateItem()
                        {
                            Name = item.Name,
                            Format = SECSItemFormat.L,
                            IsFixed = item.IsFixed,
                            Length = item.Length
                        };

                        currentIndex++;

                        if (item.IsFixed == false)
                        {
                            AddItem(message, validateItem, 1, ref currentIndex);
                        }
                        else
                        {
                            AddItem(message, validateItem, item.Length, ref currentIndex);
                        }

                        this.Item.Add(validateItem);
                    }
                    else
                    {
                        this.Item.Add(new ValidateItem()
                        {
                            Name = item.Name,
                            Format = item.Format,
                            IsFixed = item.IsFixed,
                            Length = item.Length
                        });

                        currentIndex++;
                    }
                }
            }

            private void AddItem(SECSMessage message, ValidateItem validateItem, int addCount, ref int currentIndex)
            {
                int currentCount;
                SECSItem item;
                ValidateItem validateItemTemp;

                currentCount = 0;

                while (currentCount < addCount)
                {
                    item = message.Body.AsList[currentIndex];

                    if (item.Format == SECSItemFormat.L)
                    {
                        validateItemTemp = new ValidateItem()
                        {
                            Name = item.Name,
                            Format = SECSItemFormat.L,
                            IsFixed = item.IsFixed,
                            Length = item.Length
                        };

                        currentIndex++;

                        if (item.IsFixed == false)
                        {
                            AddItem(message, validateItemTemp, 1, ref currentIndex);
                        }
                        else
                        {
                            AddItem(message, validateItemTemp, item.Length, ref currentIndex);
                        }

                        validateItem.SubItem.Add(validateItemTemp);
                    }
                    else
                    {
                        validateItem.SubItem.Add(new ValidateItem()
                        {
                            Name = item.Name,
                            Format = item.Format,
                            IsFixed = item.IsFixed,
                            Length = item.Length
                        });

                        currentIndex++;
                    }

                    currentCount++;
                }
            }
        }
        #endregion

        private DeviceType _deviceType;
        internal int _deviceId;
        private double _maxMessageSize;

        private readonly List<ValidateMessage> _validateMessage;

        public SECSMessageValidate()
        {
            this._validateMessage = new List<ValidateMessage>();
        }

        public void Clear()
        {
            this._validateMessage.Clear();
        }

        public string MakeOriginalMessage(SECSMessage messageInfo)
        {
            string result = string.Empty;

            try
            {
                this._validateMessage.Add(new ValidateMessage(messageInfo));
            }
            catch (Exception ex)
            {
                result = string.Format("UMD File Parsing Failed : {0}", ex.Message);
            }

            return result;
        }

        public string MakeOriginalMessage(DeviceType deviceType, int deviceId, double maxMessageSize, SECSMessageCollection messageInfo)
        {
            string result = string.Empty;

            try
            {
                this._deviceType = deviceType;
                this._deviceId = deviceId;
                this._maxMessageSize = maxMessageSize;

                this._validateMessage.Clear();

                foreach (KeyValuePair<string, SECSMessage> temp in messageInfo.MessageInfo)
                {
                    this._validateMessage.Add(new ValidateMessage(temp.Value));
                }
            }
            catch (Exception ex)
            {
                result = string.Format("UMD File Parsing Failed : {0}", ex.Message);
            }

            return result;
        }

        public void UpdateOriginalMessage(SECSMessage message)
        {
            int index;

            try
            {
                index = this._validateMessage.FindIndex(t => t.Name == message.Name);

                if (index >= 0)
                {
                    this._validateMessage[index] = new ValidateMessage(message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        public MessageValidationError ValidateReceivedMessage(SECSMessage message)
        {
            MessageValidationError result = MessageValidationError.Ok;

            if (this._deviceId != message.DeviceId)
            {
                result = MessageValidationError.UnrecognizedDeviceID;
            }
            else if (this._maxMessageSize < message.Length)
            {
                result = MessageValidationError.DataToLong;
            }
            else
            {
                var varOriginalValidate = from ValidateMessage temp in this._validateMessage
                                          where temp.Stream == message.Stream &&
                                                (temp.Direction == SECSMessageDirection.Both ||
                                                (this._deviceType == DeviceType.Equipment && temp.Direction == SECSMessageDirection.ToEquipment) ||
                                                (this._deviceType == DeviceType.Host && temp.Direction == SECSMessageDirection.ToHost))
                                          select temp;

                if (varOriginalValidate.Any() == false)
                {
                    result = MessageValidationError.UnrecognizedSteam;
                }
                else
                {
                    var varMessage = varOriginalValidate.Where(t => t.Function == message.Function);

                    if (varMessage.Any() == false)
                    {
                        result = MessageValidationError.UnrecognizedFunction;
                    }
                    else
                    {
                        result = CheckMessageFormat(message, varMessage);
                    }
                }
            }

            return result;
        }

        private MessageValidationError CheckMessageFormat(SECSMessage receiveMessage, IEnumerable<ValidateMessage> originalValidate)
        {
            MessageValidationError result = MessageValidationError.Ok;
            ValidateMessage receiveValidate;

            receiveValidate = new ValidateMessage(receiveMessage);

            foreach (ValidateMessage temp in originalValidate)
            {
                SECSBody body = receiveMessage.Body.Clone();

                result = CheckMessageFormat(temp, receiveValidate, body);

                if (result == MessageValidationError.Ok)
                {
                    receiveMessage.AutoReply = temp.AutoReply;
                    receiveMessage.Name = temp.Name;
                    receiveMessage.Body = body;

                    break;
                }
            }

            return result;
        }

        private MessageValidationError CheckMessageFormat(ValidateMessage originalValidate, ValidateMessage receiveValidate, SECSBody body)
        {
            MessageValidationError result = MessageValidationError.Ok;
            int index;

            if (originalValidate.Item.Count != receiveValidate.Item.Count)
            {
                result = MessageValidationError.IllegalDataFormat;
            }
            else if (originalValidate.Item.Count > 0)
            {
                index = 0;

                for (int i = 0; i < receiveValidate.Item.Count; i++)
                {
                    result = CheckMessageFormat(originalValidate.Item[i], receiveValidate.Item[i], body.AsList, body.Item, ref index);

                    if (result != MessageValidationError.Ok)
                        break;
                }
            }

            return result;
        }

        private MessageValidationError CheckMessageFormat(ValidateItem originalItem, ValidateItem receiveItem, List<SECSItem> itemList, SECSItemCollection itemsTree, ref int index)
        {
            MessageValidationError result = MessageValidationError.Ok;
            int childTotalLength;
            ValidateItem originalListItem;
            SECSItem item;

            if (originalItem.Format == SECSItemFormat.X)
            {
                childTotalLength = receiveItem.GetChildLength();

                if ((originalItem.IsFixed == true && originalItem.Length != childTotalLength) ||
                    (originalItem.IsFixed == false && originalItem.Length >= 0 && originalItem.Length < childTotalLength))
                {
                    result = MessageValidationError.IllegalDataFormat;
                }
                else
                {
                    if (receiveItem.Format == SECSItemFormat.L)
                    {
                        itemList[index].Name = originalItem.Name;
                        item = itemList[index];
                        item.IsFixed = originalItem.IsFixed;

                        index++;

                        for (int i = 0; i < receiveItem.SubItem.Count; i++)
                        {
                            result = CheckMessageFormat(originalItem, receiveItem.SubItem[i], itemList, item.SubItem, ref index);

                            if (result != MessageValidationError.Ok)
                                break;
                        }

                        if (result == MessageValidationError.Ok)
                        {
                            itemsTree.Add(item);
                        }
                    }
                    else
                    {
                        itemList[index].Name = originalItem.Name;
                        item = itemList[index];
                        index++;

                        itemsTree.Add(item);
                    }
                }
            }
            else if (originalItem.Format != receiveItem.Format ||
                (originalItem.IsFixed == true && originalItem.Length != receiveItem.Length) ||
                (originalItem.IsFixed == false && originalItem.Length >= 0 && originalItem.Length < receiveItem.Length))
            {
                result = MessageValidationError.IllegalDataFormat;
            }
            else if (originalItem.Format == SECSItemFormat.L)
            {
                if (originalItem.IsFixed == false)
                {
                    if (originalItem.SubItem.Count == 1 && originalItem.SubItem[0].Format == SECSItemFormat.L)
                    {
                        originalListItem = originalItem.SubItem[0];

                        itemList[index].Name = originalItem.Name;
                        item = itemList[index];
                        item.IsFixed = originalItem.IsFixed;

                        index++;

                        for (int i = 0; i < receiveItem.SubItem.Count; i++)
                        {
                            result = CheckMessageFormat(originalListItem, receiveItem.SubItem[i], itemList, item.SubItem, ref index);

                            if (result != MessageValidationError.Ok)
                                break;
                        }

                        if (result == MessageValidationError.Ok)
                        {
                            itemsTree.Add(item);
                        }
                    }
                    else
                    {
                        if (originalItem.Length >= 0 && originalItem.Length < receiveItem.SubItem.Count)
                        {
                            result = MessageValidationError.IllegalDataFormat;
                        }
                        else
                        {
                            itemList[index].Name = originalItem.Name;
                            item = itemList[index];
                            item.IsFixed = originalItem.IsFixed;

                            index++;

                            for (int i = 0; i < receiveItem.SubItem.Count; i++)
                            {
                                result = CheckMessageFormat(originalItem.SubItem[0], receiveItem.SubItem[i], itemList, item.SubItem, ref index);

                                if (result != MessageValidationError.Ok)
                                    break;
                            }

                            if (result == MessageValidationError.Ok)
                            {
                                itemsTree.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    if (originalItem.SubItem.Count != receiveItem.SubItem.Count)
                    {
                        result = MessageValidationError.IllegalDataFormat;
                    }
                    else
                    {
                        itemList[index].Name = originalItem.Name;
                        item = itemList[index];
                        item.IsFixed = originalItem.IsFixed;

                        index++;

                        for (int i = 0; i < receiveItem.SubItem.Count; i++)
                        {
                            result = CheckMessageFormat(originalItem.SubItem[i], receiveItem.SubItem[i], itemList, item.SubItem, ref index);

                            if (result != MessageValidationError.Ok)
                                break;
                        }

                        if (result == MessageValidationError.Ok)
                        {
                            itemsTree.Add(item);
                        }
                    }
                }
            }
            else
            {
                itemList[index].Name = originalItem.Name;
                item = itemList[index];
                index++;

                itemsTree.Add(item);
            }

            return result;
        }
    }
}