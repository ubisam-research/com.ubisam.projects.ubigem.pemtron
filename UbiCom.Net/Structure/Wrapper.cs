using System;
using System.Collections;
using System.Collections.Generic;

namespace UbiCom.Net.Structure
{
    /// <summary>
    /// Wrapper interface입니다.
    /// </summary>
    public interface IWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="name"></param>
        /// <param name="count"></param>
        void MakeList(IList list, string name, int count);
    }

    /// <summary>
    /// SECS Item Wrapper입니다.
    /// </summary>
    public class SECSItemWrapper
    {
        /// <summary>
        /// 이 SECSItemWrapper을 소유하는 SECSItemCollectionWrapper를 가져오거나 설정합니다.
        /// </summary>
        public SECSItemCollectionWrapper Owner { get; set; }

        /// <summary>
        /// Item의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item의 Format을 가져오거나 설정합니다.
        /// </summary>
        public SECSItemFormat Format { get; set; }

        /// <summary>
        /// Item의 Length 고정 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Item의 Length를 가져오거나 설정합니다.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 이 SECSItemWrapper을 소유하는 SECSItem를 가져오거나 설정합니다.
        /// </summary>
        public SECSItem Parent { get; set; }

        /// <summary>
        /// Item의 Value를 가져오거나 설정합니다.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="item">SECS Item입니다.</param>
        public SECSItemWrapper(SECSItemWrapper item)
        {
            this.Name = item.Name;
            this.Format = item.Format;
            this.IsFixed = item.IsFixed;
            this.Length = item.Length;
            this.Value = item.Value;
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="name">SECS Item의 name입니다.</param>
        /// <param name="format">SECS Item의 format입니다.</param>
        /// <param name="length">SECS Item의 length입니다.</param>
        /// <param name="isFixed">SECS Item의 fix 여부입니다.</param>
        /// <param name="value">SECS Item의 value입니다.</param>
        public SECSItemWrapper(string name, SECSItemFormat format, int length, bool isFixed, object value)
        {
            this.Name = name;
            this.Format = format;
            this.Length = length;
            this.IsFixed = isFixed;
            this.Value = value;
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="format">SECS Item의 format입니다.</param>
        /// <param name="length">SECS Item의 length입니다.</param>
        /// <param name="value">SECS Item의 value입니다.</param>
        public SECSItemWrapper(SECSItemFormat format, int length, object value)
        {
            this.Format = format;
            this.Length = length;
            this.Value = value;
        }

        /// <summary>
        /// 기본 생성자입니다.(format=L)
        /// </summary>
        /// <param name="name">SECS Item의 name입니다.</param>
        /// <param name="value">SECS Item의 value입니다.</param>
        public SECSItemWrapper(string name, object value)
        {
            this.Name = name;
            this.Format = SECSItemFormat.L;
            this.IsFixed = false;
            this.Value = value;
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
                if (this.IsFixed == true)
                {
                    if (string.IsNullOrEmpty(this.Name) == true)
                        result = string.Format("L, {0}", this.Length);
                    else
                        result = string.Format("L, {0} <{1}>", this.Length, this.Name);
                }
                else
                {
                    if (string.IsNullOrEmpty(this.Name) == true)
                        result = string.Format("L, {0}v", this.Length);
                    else
                        result = string.Format("L, {0}v <{1}>", this.Length, this.Name);
                }
            }
            else
            {
                if (this.Value == null)
                {
                    if (this.IsFixed == true)
                    {
                        result = string.Format("{0}, {1} <{2}>", this.Format, this.Length, this.Name);
                    }
                    else
                    {
                        result = string.Format("{0}, {1}v <{2}>", this.Format, this.Length, this.Name);
                    }
                }
                else
                {
                    if (this.IsFixed == true)
                    {
                        result = string.Format("{0}, {1} '{2}' <{3}>", this.Format, this.Length, this.Value.ToString().PadRight(this.Length, ' '), this.Name);
                    }
                    else
                    {
                        result = string.Format("{0}, {1}v '{2}' <{3}>", this.Format, this.Length, this.Value.ToString(), this.Name);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        public void MakeList(string name, int count)
        {
            this.Owner.MakeList(name, count);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SECSItemCollectionWrapper : IWrapper
    {
        private readonly Dictionary<string, SECSItemWrapper> _items;
        private readonly List<SECSItemWrapper> _itemList;

        /// <summary>
        /// 
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, SECSItemWrapper> AsDictionary
        {
            get { return this._items; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<SECSItemWrapper> AsList
        {
            get { return this._itemList; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SECSItemWrapper this[int index]
        {
            get
            {
                return this._itemList.Count > index ? this._itemList[index] : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SECSItemWrapper this[string name]
        {
            get
            {
                return this._items.ContainsKey(name) ? this._items[name] : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SECSItemCollectionWrapper()
        {
            this._items = new Dictionary<string, SECSItemWrapper>();
            this._itemList = new List<SECSItemWrapper>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Total Count={0}", this._items.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(SECSItemWrapper item)
        {
            //this.Owner = this;
            item.Owner = this;
            this._items[item.Name] = item;
            this._itemList.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        public virtual void MakeList(string name, int count)
        {
            if (this.Owner is SECSMessageWrapper)
            {
                (this.Owner as SECSMessageWrapper).MakeList(name, count);
            }
            else if (this.Owner is SECSItemCollectionWrapper)
            {
                (this.Owner as SECSItemCollectionWrapper).MakeList(name, count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="name"></param>
        /// <param name="count"></param>
        public virtual void MakeList(IList list, string name, int count)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SECSMessageWrapper : SECSMessage, IWrapper
    {
        private const string CLASS_NAME = "SECSMessageWrapper";

        /// <summary>
        /// 
        /// </summary>
        protected Driver.HSMSDriver _driver;

        /// <summary>
        /// 
        /// </summary>
        public SECSItemCollectionWrapper Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="messageName"></param>
        public SECSMessageWrapper(Driver.HSMSDriver driver, string messageName)
        {
            this._driver = driver;
            this.Items = new SECSItemCollectionWrapper();

            this.DeviceId = this._driver.Config.DeviceID;

            this.Name = messageName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected void Add(SECSItemWrapper item)
        {
            this.Items.Owner = this;
            item.Owner = this.Items;
            this.Items.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        protected void Add(string name, int length)
        {
            SECSItemWrapper item = new SECSItemWrapper(name, SECSItemFormat.L, length, true, null);

            this.Items.Owner = this;
            item.Owner = this.Items;
            this.Items.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void Add(string name, object value)
        {
            SECSItemWrapper item = new SECSItemWrapper(name, value);

            this.Items.Owner = this;
            item.Owner = this.Items;
            this.Items.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        public virtual void MakeList(string name, int count)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="name"></param>
        /// <param name="count"></param>
        public virtual void MakeList(IList list, string name, int count)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool MakeSECSMessage()
        {
            bool result;

            try
            {
                foreach (SECSItemWrapper temp in this.Items.AsList)
                {
                    AddSECSItem(temp);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;

                this._driver._logger.WriteException(DateTime.Now, CLASS_NAME, "MakeSECSMessage", ex);
            }

            return result;
        }

        private static SECSValue GetValue(SECSItemWrapper item)
        {
            SECSValue result = null;

            if (item.Format == SECSItemFormat.A || item.Format == SECSItemFormat.J)
            {
                if (item.IsFixed == true)
                {
                    result = ((string)item.Value).PadRight(item.Length);
                }
                else
                {
                    result = (string)item.Value;
                }
            }
            else
            {
                if (item.Length > 1)
                {
                    switch (item.Format)
                    {
                        case SECSItemFormat.I1:
                            result = (sbyte[])item.Value;
                            break;
                        case SECSItemFormat.U1:
                        case SECSItemFormat.B:
                            result = (byte[])item.Value;
                            break;
                        case SECSItemFormat.Boolean:
                            result = (bool[])item.Value;
                            break;
                        case SECSItemFormat.I2:
                            result = (short[])item.Value;
                            break;
                        case SECSItemFormat.U2:
                            result = (ushort[])item.Value;
                            break;
                        case SECSItemFormat.I4:
                            result = (int[])item.Value;
                            break;
                        case SECSItemFormat.F4:
                            result = (float[])item.Value;
                            break;
                        case SECSItemFormat.U4:
                            result = (uint[])item.Value;
                            break;
                        case SECSItemFormat.I8:
                            result = (long[])item.Value;
                            break;
                        case SECSItemFormat.F8:
                            result = (double[])item.Value;
                            break;
                        case SECSItemFormat.U8:
                            result = (ulong[])item.Value;
                            break;
                    }
                }
                else
                {
                    switch (item.Format)
                    {
                        case SECSItemFormat.I1:
                            result = (sbyte)item.Value;
                            break;
                        case SECSItemFormat.U1:
                        case SECSItemFormat.B:
                            result = (byte)item.Value;
                            break;
                        case SECSItemFormat.Boolean:
                            result = (bool)item.Value;
                            break;
                        case SECSItemFormat.I2:
                            result = (short)item.Value;
                            break;
                        case SECSItemFormat.U2:
                            result = (ushort)item.Value;
                            break;
                        case SECSItemFormat.I4:
                            result = (int)item.Value;
                            break;
                        case SECSItemFormat.F4:
                            result = (float)item.Value;
                            break;
                        case SECSItemFormat.U4:
                            result = (uint)item.Value;
                            break;
                        case SECSItemFormat.I8:
                            result = (long)item.Value;
                            break;
                        case SECSItemFormat.F8:
                            result = (double)item.Value;
                            break;
                        case SECSItemFormat.U8:
                            result = (ulong)item.Value;
                            break;
                    }
                }
            }

            return result;
        }

        private void AddSECSItem(SECSItemWrapper item)
        {
            SECSItem secsItem;
            SECSItem anyItem;

            if (item.Format == SECSItemFormat.L) // && item.IsFixed == false)
            {
                if (item.Value is IList list)
                {
                    if (item.IsFixed == false)
                    {
                        secsItem = new SECSItem()
                        {
                            Format = item.Format,
                            IsFixed = item.IsFixed,
                            Length = list.Count,
                            Name = item.Name,
                            Value = null
                        };
                    }
                    else
                    {
                        secsItem = new SECSItem()
                        {
                            Format = item.Format,
                            IsFixed = item.IsFixed,
                            Length = item.Length,
                            Name = item.Name,
                            Value = null
                        };
                    }

                    Body.Add(secsItem);

                    for (int i = 0; i < list.Count; i++)
                    {
                        AddSECSItem((IWrapper)list[i]);
                    }
                }
                else
                {
                    secsItem = new SECSItem()
                    {
                        Format = item.Format,
                        IsFixed = item.IsFixed,
                        Length = item.Length,
                        Name = item.Name,
                        Value = null
                    };

                    Body.Add(secsItem);

                    if (item.IsFixed == true && item.Length > 0)
                    {
                        SECSItemCollectionWrapper wrapperData;

                        wrapperData = item.Value as SECSItemCollectionWrapper;

                        if (wrapperData != null)
                        {
                            SECSValue value;

                            foreach (SECSItemWrapper tempWrapperItem in wrapperData.AsList)
                            {
                                if (tempWrapperItem.Format == SECSItemFormat.L)
                                {
                                    AddSECSItem(tempWrapperItem);
                                }
                                else if (tempWrapperItem.Format == SECSItemFormat.A || tempWrapperItem.Format == SECSItemFormat.J)
                                {
                                    value = GetValue(tempWrapperItem);

                                    secsItem = new SECSItem()
                                    {
                                        Format = tempWrapperItem.Format,
                                        IsFixed = tempWrapperItem.IsFixed,
                                        Length = value.ToString().Length,
                                        Name = tempWrapperItem.Name,
                                        Value = value
                                    };

                                    Body.Add(secsItem);
                                }
                                else
                                {
                                    value = GetValue(tempWrapperItem);

                                    secsItem = new SECSItem()
                                    {
                                        Format = tempWrapperItem.Format,
                                        IsFixed = tempWrapperItem.IsFixed,
                                        Length = tempWrapperItem.Length,
                                        Name = tempWrapperItem.Name,
                                        Value = value
                                    };

                                    Body.Add(secsItem);
                                }
                            }
                        }
                    }
                }
            }
            else if (item.Format == SECSItemFormat.X)
            {
                secsItem = item.Value as SECSItem;

                if (secsItem != null)
                {
                    Body.Add(secsItem);
                }
            }
            else
            {
                SECSValue value;

                value = GetValue(item);

                if (item.Format == SECSItemFormat.A || item.Format == SECSItemFormat.J)
                {
                    secsItem = new SECSItem()
                    {
                        Format = item.Format,
                        IsFixed = item.IsFixed,
                        Length = value.ToString().Length,
                        Name = item.Name,
                        Value = value
                    };

                    Body.Add(secsItem);
                }
                else if (item.Format == SECSItemFormat.X)
                {
                    anyItem = item.Value as SECSItem;

                    if (anyItem.Format == SECSItemFormat.L)
                    {
                        secsItem = new SECSItem()
                        {
                            Format = anyItem.Format,
                            IsFixed = anyItem.IsFixed,
                            Length = anyItem.SubItem.Count,
                            Name = anyItem.Name,
                            Value = null
                        };

                        Body.Add(secsItem);

                        foreach (SECSItem temp in anyItem.SubItem.Items)
                        {
                            if (temp.SubItem is IList list)
                            {
                                if (temp.IsFixed == false)
                                {
                                    secsItem = new SECSItem()
                                    {
                                        Format = temp.Format,
                                        IsFixed = temp.IsFixed,
                                        Length = list.Count,
                                        Name = temp.Name,
                                        Value = null
                                    };
                                }
                                else
                                {
                                    secsItem = new SECSItem()
                                    {
                                        Format = temp.Format,
                                        IsFixed = temp.IsFixed,
                                        Length = temp.Length,
                                        Name = temp.Name,
                                        Value = null
                                    };
                                }

                                Body.Add(secsItem);

                                for (int i = 0; i < list.Count; i++)
                                {
                                    AddSECSItem((IWrapper)list[i]);
                                }
                            }
                            else
                            {
                                secsItem = new SECSItem()
                                {
                                    Format = temp.Format,
                                    IsFixed = temp.IsFixed,
                                    Length = temp.Length,
                                    Name = temp.Name,
                                    Value = temp.Value
                                };

                                Body.Add(secsItem);
                            }
                        }
                    }
                    else
                    {
                        secsItem = new SECSItem()
                        {
                            Format = anyItem.Format,
                            IsFixed = anyItem.IsFixed,
                            Length = anyItem.Length,
                            Name = anyItem.Name,
                            Value = anyItem.Value
                        };

                        Body.Add(secsItem);
                    }
                }
                else
                {
                    secsItem = new SECSItem()
                    {
                        Format = item.Format,
                        IsFixed = item.IsFixed,
                        Length = item.Length,
                        Name = item.Name,
                        Value = value
                    };

                    Body.Add(secsItem);
                }
            }
        }

        private void AddSECSItem(IWrapper item)
        {
            SECSItem secsItem;

            foreach (SECSItemWrapper temp in (item as SECSItemCollectionWrapper).AsList)
            {
                if (temp.Format == SECSItemFormat.L) // && temp.IsFixed == false)
                {
                    if (temp is IList list)
                    {
                        if (temp.IsFixed == false)
                        {
                            secsItem = new SECSItem()
                            {
                                Format = temp.Format,
                                IsFixed = temp.IsFixed,
                                Length = list.Count,
                                Name = temp.Name,
                                Value = null
                            };
                        }
                        else
                        {
                            secsItem = new SECSItem()
                            {
                                Format = temp.Format,
                                IsFixed = temp.IsFixed,
                                Length = temp.Length,
                                Name = temp.Name,
                                Value = null
                            };
                        }

                        Body.Add(secsItem);

                        for (int i = 0; i < list.Count; i++)
                        {
                            AddSECSItem((SECSItemCollectionWrapper)list[i]);
                        }
                    }
                    else
                    {
                        secsItem = new SECSItem()
                        {
                            Format = temp.Format,
                            IsFixed = temp.IsFixed,
                            Length = temp.Length,
                            Name = temp.Name,
                            Value = null
                        };

                        Body.Add(secsItem);

                        if (temp.Value != null)
                        {
                            list = temp.Value as IList;

                            if (list != null)
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    AddSECSItem((IWrapper)list[i]);
                                }
                            }
                            else
                            {
                                if (temp.Value is SECSItemCollectionWrapper wrapperList)
                                {
                                    foreach (SECSItemWrapper tempSECSItemWrapper in wrapperList.AsList)
                                    {
                                        AddSECSItem(tempSECSItemWrapper);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (temp.Format == SECSItemFormat.X)
                {
                    secsItem = temp.Value as SECSItem;

                    Body.Add(secsItem);

                    if (secsItem.Format == SECSItemFormat.L)
                    {
                        foreach (SECSItem tempSECSItem in secsItem.SubItem.Items)
                        {
                            AddSECSItemByTypeX(tempSECSItem);
                        }
                    }
                }
                else
                {
                    SECSValue value = GetValue(temp);

                    if (temp.Format == SECSItemFormat.A || temp.Format == SECSItemFormat.J)
                    {
                        secsItem = new SECSItem()
                        {
                            Format = temp.Format,
                            IsFixed = temp.IsFixed,
                            Length = value.ToString().Length,
                            Name = temp.Name,
                            Value = value
                        };
                    }
                    else
                    {
                        secsItem = new SECSItem()
                        {
                            Format = temp.Format,
                            IsFixed = temp.IsFixed,
                            Length = temp.Length,
                            Name = temp.Name,
                            Value = value
                        };
                    }

                    Body.Add(secsItem);
                }
            }
        }

        private void AddSECSItemByTypeX(SECSItem secsItem)
        {
            Body.Add(secsItem);

            if (secsItem.Format == SECSItemFormat.L)
            {
                foreach (SECSItem tempSECSItem in secsItem.SubItem.Items)
                {
                    AddSECSItemByTypeX(tempSECSItem);
                }
            }
        }
    }
}