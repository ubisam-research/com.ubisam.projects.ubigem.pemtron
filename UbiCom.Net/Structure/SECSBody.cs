using System.Collections.Generic;

namespace UbiCom.Net.Structure
{
    /// <summary>
    /// SECS Message의 Body입니다.
    /// </summary>
    public class SECSBody
    {
        // Data 구조
        // Item   = Tree로 구성 됨, Receive 시 구성 됨
        // AsList = Item 순서대로(동일 Level), Message 생성(Send) 시 사용

        private readonly List<SECSItem> _items;

        /// <summary>
        /// 수신한 Item을 가져오거나 설정합니다.(Tree 형식으로 구성)
        /// </summary>
        public SECSItemCollection Item { get; set; }

        /// <summary>
        /// 송신할 Item 목록을 가져오거나 설정합니다.(동일 Level로 구성)
        /// </summary>
        public List<SECSItem> AsList
        {
            get { return this._items; }
        }

        /// <summary>
        /// Item의 수량을 가져옵니다.
        /// </summary>
        public int Count
        {
            get { return this._items.Count; }
        }

        /// <summary>
        /// Body의 Total Byte 크기를 가져옵니다.
        /// </summary>
        public long TotalBytes
        {
            get
            {
                long result = 0;

                if (this._items != null && this._items.Count > 0)
                {
                    foreach (SECSItem temp in this._items)
                    {
                        result += GetTotalLength(temp);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSBody()
        {
            this._items = new List<SECSItem>();
            this.Item = new SECSItemCollection();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Item Count={0}", this.Item.Count);
        }

        /// <summary>
        /// SECS Body의 단순 복사본을 만듭니다.
        /// </summary>
        /// <returns>SECS Body의 단순 복사본입니다.</returns>
        public SECSBody Clone()
        {
            SECSBody result = new SECSBody();

            foreach (SECSItem temp in this._items)
            {
                result.Add(temp.Clone());
            }

            result.Item = this.Item.Clone();

            return result;
        }

        /// <summary>
        /// SECS Header의 모든 Data를 초기화합니다.
        /// </summary>
        public void Clear()
        {
            this._items.Clear();
            this.Item.Clear();
        }

        /// <summary>
        /// SECS Item을 추가합니다.
        /// </summary>
        /// <param name="item">추가할 SECS Item입니다.</param>
        public void Add(Structure.SECSItem item)
        {
            this._items.Add(item.Clone());
        }

        /// <summary>
        /// SECS Item을 추가합니다.
        /// </summary>
        /// <param name="format">추가할 SECS Itemd의 format입니다.</param>
        /// <param name="length">추가할 SECS Itemd의 length입니다.</param>
        /// <param name="value">추가할 SECS Itemd의 value입니다.</param>
        public void Add(SECSItemFormat format, int length, Structure.SECSValue value)
        {
            Structure.SECSItem item;

            item = new SECSItem()
            {
                Format = format,
                Length = length,
                Value = value
            };

            this._items.Add(item);
        }

        /// <summary>
        /// SECS Item을 추가합니다.
        /// </summary>
        /// <param name="name">추가할 SECS Itemd의 name입니다.</param>
        /// <param name="format">추가할 SECS Itemd의 format입니다.</param>
        /// <param name="length">추가할 SECS Itemd의 length입니다.</param>
        /// <param name="value">추가할 SECS Itemd의 value입니다.</param>
        public void Add(string name, SECSItemFormat format, int length, Structure.SECSValue value)
        {
            Structure.SECSItem item;

            item = new SECSItem()
            {
                Name = name,
                Format = format,
                Length = length,
                Value = value
            };

            this._items.Add(item);
        }

        /// <summary>
        /// SECS Item을 추가합니다.
        /// </summary>
        /// <param name="name">추가할 SECS Itemd의 name입니다.</param>
        /// <param name="format">추가할 SECS Itemd의 format입니다.</param>
        /// <param name="length">추가할 SECS Itemd의 length입니다.</param>
        /// <param name="isFixed">추가할 SECS Itemd의 fix 여부입니다.</param>
        /// <param name="value">추가할 SECS Itemd의 value입니다.</param>
        public void Add(string name, SECSItemFormat format, int length, bool isFixed, Structure.SECSValue value)
        {
            Structure.SECSItem item;

            item = new SECSItem()
            {
                Name = name,
                Format = format,
                Length = length,
                IsFixed = isFixed,
                Value = value
            };

            this._items.Add(item);
        }

        private static long GetTotalLength(SECSItem item)
        {
            long result = 0;
            int length;

            if (item != null)
            {
                if (item.IsFixed == true)
                {
                    length = item.Length;
                }
                else
                {
                    if (item.Value == null)
                    {
                        if (item.Format == SECSItemFormat.L)
                        {
                            length = item.Length;
                        }
                        else
                        {
                            length = 0;
                        }
                    }
                    else
                    {
                        if (item.Format == SECSItemFormat.A || item.Format == SECSItemFormat.J)
                        {
                            if (item.Length != item.Value.Length)
                            {
                                item.Length = item.Value.Length;
                            }
                        }

                        length = item.Value.Length;
                    }
                }

                switch (item.Format)
                {
                    case SECSItemFormat.L:
                        result += 1 + GetItemLength(length);

                        break;
                    case SECSItemFormat.U1:
                    case SECSItemFormat.I1:
                    case SECSItemFormat.A:
                    case SECSItemFormat.B:
                    case SECSItemFormat.Boolean:
                    case SECSItemFormat.J:
                        result += 1 + GetItemLength(length) + length;

                        break;
                    case SECSItemFormat.I2:
                    case SECSItemFormat.U2:
                        result += 1 + GetItemLength(length * 2) + (length * 2);

                        break;
                    case SECSItemFormat.I4:
                    case SECSItemFormat.F4:
                    case SECSItemFormat.U4:
                        result += 1 + GetItemLength(length * 4) + (length * 4);

                        break;
                    case SECSItemFormat.I8:
                    case SECSItemFormat.F8:
                    case SECSItemFormat.U8:
                        result += 1 + GetItemLength(length * 8) + (length * 8);

                        break;
                }
            }

            return result;
        }

        private static int GetItemLength(int length)
        {
            if (length <= 0xff)
            {
                return 1;
            }

            if (length <= 0xffff)
            {
                return 2;
            }

            return 3;
        }
    }
}