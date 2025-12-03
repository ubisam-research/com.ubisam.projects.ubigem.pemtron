using System.Collections.Generic;
using System.Linq;

namespace UbiCom.Net.Structure
{
    /// <summary>
    /// SECS Body의 Item입니다.
    /// </summary>
    public class SECSItem
    {
        /// <summary>
        /// Item의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item의 format을 가져오거나 설정합니다.
        /// </summary>
        public SECSItemFormat Format { get; set; }

        /// <summary>
        /// Item의 length 고정 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Item의 length를 가져오거나 설정합니다.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Item의 하위 item을 가져오거나 설정합니다.
        /// </summary>
        public SECSItemCollection SubItem { get; set; }

        /// <summary>
        /// Item의 상위 item을 가져오거나 설정합니다.
        /// </summary>
        public SECSItem Parent { get; set; }

        /// <summary>
        /// Item의 value를 가져오거나 설정합니다.
        /// </summary>
        public SECSValue Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object DriverData { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSItem()
        {
            this.Parent = null;
            this.Value = new SECSValue();
            this.IsFixed = true;
            this.SubItem = new SECSItemCollection();
            this.DriverData = null;
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="name">SECS item의 name입니다.</param>
        /// <param name="format">SECS Itemd의 format입니다.</param>
        /// <param name="length">SECS Itemd의 length입니다.</param>
        /// <param name="value">SECS Itemd의 value입니다.</param>
        public SECSItem(string name, SECSItemFormat format, int length, SECSValue value)
        {
            this.Name = name;
            this.Format = format;
            this.IsFixed = false;
            this.Length = length;
            this.Parent = null;
            this.Value = value;
            this.SubItem = new SECSItemCollection();
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
                if (this.Value == null || this.Value.IsEmpty == true)
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
        /// SECS Item의 단순 복사본을 만듭니다.
        /// </summary>
        /// <returns>SECS Item의 단순 복사본입니다.</returns>
        public SECSItem Clone()
        {
            SECSItem result = new SECSItem()
            {
                Name = this.Name,
                Format = this.Format,
                IsFixed = this.IsFixed,
                Length = this.Length
            };

            if (this.Value != null)
            {
                result.Value = new SECSValue();

                result.Value.SetValue(this.Format, this.Value.GetValue());
            }

            if (result.SubItem != null)
            {
                foreach (SECSItem temp in this.SubItem.Items)
                {
                    result.SubItem.Add(temp.Clone() as SECSItem);
                }
            }

            if (this.Parent != null)
                result.Parent = this.Parent.Clone();

            return result;
        }

        /// <summary>
        /// SECS Item의 모든 Data를 초기화합니다.
        /// </summary>
        public void Clear()
        {
            this.Name = string.Empty;
            this.Format = SECSItemFormat.L;
            this.IsFixed = false;
            this.Length = -1;
            this.Parent = null;
            this.Value = null;
            this.SubItem.Clear();
        }
    }

    /// <summary>
    /// SECS Body의 Item Collection입니다.
    /// </summary>
    public class SECSItemCollection
    {
        private readonly List<SECSItem> _itemInfo;

        /// <summary>
        /// System.Collections.Generic.List&lt;SECSItem&gt;를 반환합니다.
        /// </summary>
        public SECSItem[] Items
        {
            get { return this._itemInfo.ToArray(); }
        }

        /// <summary>
        /// Item Count를 가져옵니다.
        /// </summary>
        public int Count
        {
            get { return this._itemInfo.Count; }
        }

        /// <summary>
        /// 지정한 이름과 연결된 SECS Item을 가져옵니다.
        /// </summary>
        /// <param name="name">가져올 SECS Item의 이름입니다.</param>
        /// <returns>지정한 이름과 연결된 SECS Item입니다.</returns>
        public SECSItem this[string name]
        {
            get
            {
                return this._itemInfo.FirstOrDefault(t => t.Name == name);
            }
        }

        /// <summary>
        /// 해당 Index의 SECS Item을 가져옵니다.
        /// </summary>
        /// <param name="index">가져올 Index입니다.</param>
        /// <returns></returns>
        public SECSItem this[int index]
        {
            get
            {
                return (this._itemInfo.Count > index) ? this._itemInfo[index] : null;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSItemCollection()
        {
            this._itemInfo = new List<SECSItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Total Count={0}", this._itemInfo.Count);
        }

        /// <summary>
        /// Body Item을 추가합니다.
        /// </summary>
        /// <param name="secsItem">추가할 Body Item입니다.</param>
        public void Add(SECSItem secsItem)
        {
            this._itemInfo.Add(secsItem);
        }

        /// <summary>
        /// Body Item의 모든 Data를 초기화합니다.
        /// </summary>
        public void Clear()
        {
            this._itemInfo.Clear();
        }

        /// <summary>
        /// SECS Item Collection의 단순 복사본을 만듭니다.
        /// </summary>
        /// <returns>SECS Item Collection의 단순 복사본입니다.</returns>
        public SECSItemCollection Clone()
        {
            SECSItemCollection result;

            if (this._itemInfo == null)
            {
                result = null;
            }
            else
            {
                result = new SECSItemCollection();

                foreach (SECSItem temp in this._itemInfo)
                {
                    result.Add(temp.Clone());
                }
            }

            return result;
        }

        /// <summary>
        /// SECS Item의 이름과 일치하는 요소가 포함되어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="name">검색할 SECS Item의 이름입니다.</param>
        /// <returns>지정한 요소가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exists(string name)
        {
            return this._itemInfo.Exists(t => t.Name == name);
        }
    }
}