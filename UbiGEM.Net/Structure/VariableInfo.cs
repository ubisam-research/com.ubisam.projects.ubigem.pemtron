using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Variable 정보입니다.
    /// </summary>
    public class VariableInfo
    {
        private UbiCom.Net.Structure.SECSItemFormat _format;
        private Dictionary<string, string> _customMapping;
        private bool _hasCustomMapping;

        /// <summary>
        /// GEM Driver의 사전 등록 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool PreDefined { get; set; }

        /// <summary>
        /// Variable 종류를 가져오거나 설정합니다.
        /// </summary>
        public VariableType VIDType { get; set; }

        /// <summary>
        /// Variable ID를 가져오거나 설정합니다.
        /// </summary>
        public string VID { get; set; }

        /// <summary>
        /// Variable Name을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Variable의 사용 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// Variable의 Format을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSItemFormat Format
        {
            get { return this._format; }
            set
            {
                this._format = value;

                if (this._format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    if (this.ChildVariables == null)
                    {
                        this.ChildVariables = new VariableCollection();
                        //this.ChildVariables = new List<VariableInfo>();
                    }
                }
            }
        }

        /// <summary>
        /// Variable의 Length를 가져오거나 설정합니다.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Variable의 최소값을 가져오거나 설정합니다.
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// Variable의 최대값을 가져오거나 설정합니다.
        /// </summary>
        public double? Max { get; set; }

        /// <summary>
        /// Variable의 현재값을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSValue Value { get; set; }

        /// <summary>
        /// Variable의 초기값을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSValue Default { get; set; }

        /// <summary>
        /// Variable의 단위를 가져오거나 설정합니다.
        /// </summary>
        public object Units { get; set; }

        /// <summary>
        /// Variable의 Description을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Variable의 하위 Variable을 가져오거나 설정합니다.(Format=L일 경우)
        /// </summary>
        public VariableCollection ChildVariables { get; set; }

        internal bool HasCustomMapping
        {
            get { return this._hasCustomMapping; }
        }

        internal Dictionary<string, string> CustomMapping
        {
            get { return this._customMapping; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public VariableInfo()
        {
            this.VID = string.Empty;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.VIDType = VariableType.DVVAL;

            this.Value = new UbiCom.Net.Structure.SECSValue();
            this.Default = new UbiCom.Net.Structure.SECSValue();
            this.ChildVariables = null;

            this._customMapping = null;
            this._hasCustomMapping = false;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("VID={0}({1}), Name={2}, Pre-Define={3}, Use={4}, Value={5}, Description={6}",
                this.VID, this.VIDType, this.Name, this.PreDefined, this.IsUse, this.Value, this.Description);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public VariableInfo CopyTo()
        {
            VariableInfo result;

            result = new VariableInfo()
            {
                PreDefined = this.PreDefined,
                VIDType = this.VIDType,
                VID = this.VID,
                Name = this.Name,
                IsUse = this.IsUse,
                Format = this._format,
                Length = this.Length,
                Min = this.Min,
                Max = this.Max,
                Units = this.Units,
                Description = this.Description
            };

            if (this.Value != null)
            {
                result.Value.SetValue(this.Value.GetValue());
            }
            else
            {
                result.Value = null;
            }

            if (this.Default != null)
            {
                result.Default = this.Default;
            }
            else
            {
                result.Default = null;
            }

            if (this.ChildVariables != null)
            {
                result.ChildVariables = this.ChildVariables.CopyTo();
            }

            return result;
        }

        internal void SetCustomMapping(string key, string value)
        {
            if (this._customMapping == null)
            {
                this._customMapping = new Dictionary<string, string>();

                this._hasCustomMapping = true;
            }

            this._customMapping[key] = value;
        }

        internal string GetCustomMapping(string key)
        {
            if (this._customMapping.ContainsKey(key) == true)
            {
                return this._customMapping[key];
            }
            else
            {
                return string.Empty;
            }
        }

        internal T GetCustomMapping<T>(string value, T defaultValue)
        {
            if (typeof(T).IsEnum == true)
            {
                string key = string.Empty;

                foreach (KeyValuePair<string, string> temp in this._customMapping)
                {
                    if (temp.Value == value)
                    {
                        key = temp.Key;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(key) == false)
                {
                    foreach (T item in Enum.GetValues(typeof(T)))
                    {
                        if (item.ToString().ToLower().Equals(key.Trim().ToLower()))
                        {
                            return item;
                        }
                    }
                }

                return defaultValue;
            }

            return defaultValue;
        }
    }

    /// <summary>
    /// Variable Collection 정보입니다.
    /// </summary>
    public class VariableCollection : IEnumerable, IEnumerator
    {
        private int _position;

        /// <summary>
        /// Variable Item을 가져오거나 설정합니다.
        /// </summary>
        public List<VariableInfo> Items { get; set; }

        /// <summary>
        /// 컬렉션에서 열거자의 현재 위치에 있는 요소를 가져옵니다.
        /// </summary>
        public object Current
        {
            get
            {
                return Items[this._position];
            }
        }

        /// <summary>
        /// Equipment Constant 정보를 가져옵니다.
        /// </summary>
        public VariableCollection ECV
        {
            get
            {
                VariableCollection result;

                result = new VariableCollection();

                foreach (VariableInfo temp in this.Items.Where(t => t.VIDType == VariableType.ECV))
                {
                    result.Add(temp);
                }

                return result;
            }
        }

        /// <summary>
        /// Data Variable 정보를 가져옵니다.
        /// </summary>
        public VariableCollection DVVal
        {
            get
            {
                VariableCollection result;

                result = new VariableCollection();

                foreach (VariableInfo temp in this.Items.Where(t => t.VIDType == VariableType.DVVAL))
                {
                    result.Add(temp);
                }

                return result;
            }
        }

        /// <summary>
        /// Status Variable 정보를 가져옵니다.
        /// </summary>
        public VariableCollection SV
        {
            get
            {
                VariableCollection result;

                result = new VariableCollection();

                foreach (VariableInfo temp in this.Items.Where(t => t.VIDType == VariableType.SV))
                {
                    result.Add(temp);
                }

                return result;
            }
        }

        /// <summary>
        /// Data &amp; Status Variable 정보를 가져옵니다.
        /// </summary>
        public VariableCollection Variables
        {
            get
            {
                VariableCollection result;

                result = new VariableCollection();

                foreach (VariableInfo temp in this.Items.Where(t => t.VIDType != VariableType.ECV))
                {
                    result.Add(temp);
                }

                return result;
            }
        }

        /// <summary>
        /// Variable의 전체 개수를 가져옵니다.
        /// </summary>
        public int Count
        {
            get
            {
                if (this.Items != null)
                {
                    return this.Items.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Variable 정보를 가져옵니다.
        /// </summary>
        /// <param name="vid">가져올 Variable ID입니다.</param>
        /// <returns>Variable 정보입니다.(단, 없을 경우 null)</returns>
        public VariableInfo this[string vid]
        {
            get
            {
                return this.Items.FirstOrDefault(t => t.VID == vid);
            }
        }

        /// <summary>
        /// Variable 정보를 가져옵니다.
        /// </summary>
        /// <param name="index">가져올 variable index입니다.</param>
        /// <returns>Variable 정보입니다.(단, 없을 경우 null)</returns>
        public VariableInfo this[int index]
        {
            get
            {
                return this.Items[index];
            }
            set
            {
                this.Items[index] = value;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public VariableCollection()
        {
            this.Items = new List<VariableInfo>();

            this._position = -1;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Item Count={0}", this.Items.Count);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public VariableCollection CopyTo()
        {
            VariableCollection result;

            result = new VariableCollection();

            foreach (VariableInfo temp in this.Items)
            {
                result.Items.Add(temp.CopyTo());
            }

            return result;
        }

        /// <summary>
        /// Variable 정보를 추가합니다.
        /// </summary>
        /// <param name="vidInfo">추가할 Variable 정보입니다.</param>
        public void Add(VariableInfo vidInfo)
        {
            this.Items.Add(vidInfo);
        }

        /// <summary>
        /// Variable 정보를 삭제합니다.
        /// </summary>
        /// <param name="vidInfo">삭제할 Variable 정보입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(VariableInfo vidInfo)
        {
            bool result;
            int index;

            result = false;

            if (vidInfo != null && string.IsNullOrEmpty(vidInfo.VID) == false)
            {
                index = this.Items.FindIndex(t => t.VID == vidInfo.VID);

                if (index >= 0)
                {
                    this.Items.RemoveAt(index);

                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Variable 정보를 삭제합니다.
        /// </summary>
        /// <param name="vid">삭제할 Variable ID입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(string vid)
        {
            bool result;
            int index;

            result = false;

            if (string.IsNullOrEmpty(vid) == false)
            {
                index = this.Items.FindIndex(t => t.VID == vid);

                if (index >= 0)
                {
                    this.Items.RemoveAt(index);

                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 지정된 Variable이 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="vid">검사할 Variable ID입니다.</param>
        /// <returns>Variable이 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(string vid)
        {
            return this.Items.Exists(t => t.VID == vid);
        }

        /// <summary>
        /// 지정된 Variable이 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="name">검사할 Variable Name입니다.</param>
        /// <returns>Variable이 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist2(string name)
        {
            return this.Items.Exists(t => t.Name == name);
        }

        /// <summary>
        /// Variable info를 가져옵니다.
        /// </summary>
        /// <param name="name">가져올 name입니다.</param>
        /// <returns>Variable info입니다.</returns>
        public VariableInfo GetVariableInfo(string name)
        {
            return this.Items.FirstOrDefault(t => t.Name == name);
        }

        /// <summary>
        /// Variable info를 가져옵니다.
        /// </summary>
        /// <param name="variableType">가져올 variable type입니다.</param>
        /// <param name="name">가져올 name입니다.</param>
        /// <returns>Variable info입니다.</returns>
        public VariableInfo GetVariableInfo(VariableType variableType, string name)
        {
            return this.Items.FirstOrDefault(t => t.VIDType == variableType && t.Name == name);
        }

        /// <summary>
        /// Variable 정보를 삭제합니다.
        /// </summary>
        public void Clear()
        {
            if (this.Items != null)
            {
                this.Items.Clear();
            }

            Reset();
        }

        /// <summary>
        /// 지정된 컬렉션의 요소를 System.Collections.Generic.List`1의 끝에 추가합니다.
        /// </summary>
        /// <param name="variables"></param>
        public void AddRange(List<VariableInfo> variables)
        {
            this.Items.AddRange(variables);
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns> 컬렉션을 반복하는 데 사용할 수 있는 System.Collections.IEnumerator 개체입니다.</returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                yield return this.Items[i];
            }
        }

        /// <summary>
        /// 열거자를 컬렉션의 다음 요소로 이동합니다.
        /// </summary>
        /// <returns>열거자가 다음 요소로 이동한 경우 true이(가) 반환되고, 컬렉션의 끝을 지난 경우 false이(가) 반환됩니다.</returns>
        public bool MoveNext()
        {
            if (this._position == this.Items.Count - 1)
            {
                Reset();
                return false;
            }

            this._position++;

            return (this._position < this.Items.Count);
        }

        /// <summary>
        /// 컬렉션의 첫 번째 요소 앞의 초기 위치에 열거자를 설정합니다.
        /// </summary>
        public void Reset()
        {
            this._position = -1;
        }
    }
}