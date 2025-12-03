using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Data dictionary 정보입니다.
    /// </summary>
    public class DataDictionaryInfo
    {
        /// <summary>
        /// GEM driver 기본 등록 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool PreDefined { get; set; }

        /// <summary>
        /// Name을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Format을 가져오거나 설정합니다.
        /// </summary>
        public List<UbiCom.Net.Structure.SECSItemFormat> Format { get; set; }

        /// <summary>
        /// Allowable Format을 가져오거나 설정합니다.
        /// </summary>
        public List<UbiCom.Net.Structure.SECSItemFormat> AllowableFormats { get; set; }

        /// <summary>
        /// Length를 가져오거나 설정합니다.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Description을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 현재값을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSValue Value { get; set; }

        /// <summary>
        /// Format을 string으로 변환하여 가져옵니다.
        /// </summary>
        public string FormatString
        {
            get
            {
                string result;

                result = string.Empty;

                this.Format.ForEach(t =>
                {
                    result += t.ToString() + ",";
                });

                if (result.Length > 0)
                    result = result.Substring(0, result.Length - 1);

                return result;
            }
        }

        /// <summary>
        /// Allowable format을 string으로 변환하여 가져옵니다.
        /// </summary>
        public string AllowableFormatString
        {
            get
            {
                string result;

                result = string.Empty;

                this.AllowableFormats.ForEach(t =>
                {
                    result += t.ToString() + ",";
                });

                if (result.Length > 0)
                    result = result.Substring(0, result.Length - 1);

                return result;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public DataDictionaryInfo()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;

            this.Format = new List<UbiCom.Net.Structure.SECSItemFormat>();
            this.AllowableFormats = new List<UbiCom.Net.Structure.SECSItemFormat>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Name={0}, Pre-Define={1}, Description={2}",
                this.Name, this.PreDefined, this.Description);
        }
    }

    /// <summary>
    /// Data dictionary collection 정보입니다.
    /// </summary>
    public class DataDictionaryCollection
    {
        /// <summary>
        /// Data dictionary를 가져오거나 설정합니다.
        /// </summary>
        public Dictionary<string, DataDictionaryInfo> Items { get; set; }

        /// <summary>
        /// Data dictionary를 가져옵니다.
        /// </summary>
        /// <param name="name">가져올 name입니다.</param>
        /// <returns>Data dictionary입니다.(없을 경우 null)</returns>
        public DataDictionaryInfo this[string name]
        {
            get
            {
                return this.Items.ContainsKey(name) == true ? this.Items[name] : null;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public DataDictionaryCollection()
        {
            this.Items = new Dictionary<string, DataDictionaryInfo>();
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
        /// Data dictionary를 추가합니다.
        /// </summary>
        /// <param name="dataDictionaryInfo">추가할 Data dictionary입니다.</param>
        public void Add(DataDictionaryInfo dataDictionaryInfo)
        {
            this.Items[dataDictionaryInfo.Name] = dataDictionaryInfo;
        }

        /// <summary>
        /// Data dictionary을 삭제합니다.
        /// </summary>
        /// <param name="dataDictionaryInfo">삭제할 Data dictionary입니다.</param>
        /// <returns> 요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Remove(DataDictionaryInfo dataDictionaryInfo)
        {
            return this.Items.Remove(dataDictionaryInfo.Name);
        }

        /// <summary>
        /// Data dictionary가 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="name">찾을 data dictionary name입니다.</param>
        /// <returns>지정한 요소가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(string name)
        {
            return this.Items.ContainsKey(name);
        }
    }
}