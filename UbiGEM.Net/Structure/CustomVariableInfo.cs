using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// 사용자 정의 Variable 정보입니다.
    /// </summary>
    public class CustomVariableInfo
    {
        internal class VariableMakerItem
        {
            private string _name;
            private UbiCom.Net.Structure.SECSItemFormat _format;
            private UbiCom.Net.Structure.SECSValue _value;
            
            public string Name
            {
                get { return this._name; }
            }

            public UbiCom.Net.Structure.SECSItemFormat Format
            {
                get { return this._format; }
            }

            public UbiCom.Net.Structure.SECSValue Value
            {
                get { return this._value; }
            }

            public VariableMakerItem(UbiCom.Net.Structure.SECSItemFormat format, UbiCom.Net.Structure.SECSValue value)
            {
                this._name = string.Empty;
                this._format = format;

                if (value == null)
                {
                    this._value = new UbiCom.Net.Structure.SECSValue();

                    this._value.SetValue(format, null);
                }
                else
                {
                    this._value = value;
                }
            }

            public VariableMakerItem(string name, UbiCom.Net.Structure.SECSItemFormat format, UbiCom.Net.Structure.SECSValue value)
            {
                this._name = name;
                this._format = format;

                if (value == null)
                {
                    this._value = new UbiCom.Net.Structure.SECSValue();

                    this._value.SetValue(format, null);
                }
                else
                {
                    this._value = value;
                }
            }

            public override string ToString()
            {
                return $"Name={this._name}, Format={this._format}, Value={this._value}";
            }
        }

        private List<VariableMakerItem> _items;
        
        private int _index;

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public CustomVariableInfo()
        {
            this._items = new List<VariableMakerItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Count={0}", this._items.Count);
        }

        /// <summary>
        /// 전체 하위 item을 삭제합니다.
        /// </summary>
        public void Clear()
        {
            this._items.Clear();
        }

        /// <summary>
        /// 하위 item을 추가합니다.
        /// </summary>
        /// <param name="format">Item의 SECS format 입니다.</param>
        /// <param name="value">Item의 value입니다.</param>
        public void Add(UbiCom.Net.Structure.SECSItemFormat format, UbiCom.Net.Structure.SECSValue value)
        {
            this._items.Add(new VariableMakerItem(format, value));
        }

        /// <summary>
        /// 하위 item을 추가합니다.
        /// </summary>
        /// <param name="name">Item name입니다.</param>
        /// <param name="format">Item의 SECS format 입니다.</param>
        /// <param name="value">Item의 value입니다.</param>
        public void Add(string name, UbiCom.Net.Structure.SECSItemFormat format, UbiCom.Net.Structure.SECSValue value)
        {
            this._items.Add(new VariableMakerItem(name, format, value));
        }

        internal bool SetChildVariables(VariableInfo variableInfo, out string errorText)
        {
            bool result = true;

            errorText = string.Empty;

            if (variableInfo.ChildVariables != null)
            {
                variableInfo.ChildVariables.Clear();
            }

            this._index = 0;

            while (this._items.Count > this._index)
            {
                result = SetChildVariables(variableInfo.ChildVariables, out errorText);

                if (result == false)
                {
                    return result;
                }

                this._index++;
            }

            return result;
        }

        private bool SetChildVariables(VariableCollection childVariables, out string errorText)
        {
            bool result;
            
            errorText = string.Empty;

            try
            {
                VariableInfo variableInfo = new VariableInfo()
                {
                    Name = this._items[this._index].Name,
                    Format = this._items[this._index].Format,
                    Value = this._items[this._index].Value
                };

                if (this._items[this._index].Format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    if (this._items[this._index].Value.IsEmpty == true)
                    {
                        errorText = $"({this._index}) value 오류(List count):value is null";

                        return false;
                    }

                    if (int.TryParse(this._items[this._index].Value, out int listCount) == false)
                    {
                        errorText = $"({this._index}) value 오류(List count):value='{this._items[this._index].Value}'";

                        return false;
                    }

                    if (listCount > 0)
                    {
                        int currentIndex = 0;

                        variableInfo.ChildVariables = new VariableCollection();

                        while (listCount > currentIndex)
                        {
                            this._index++;

                            result = SetChildVariables(variableInfo.ChildVariables, out errorText);

                            if (result == false)
                            {
                                return result;
                            }

                            currentIndex++;
                        }
                    }
                }

                childVariables.Add(variableInfo);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;

                return false;
            }

            return true;
        }
    }
}