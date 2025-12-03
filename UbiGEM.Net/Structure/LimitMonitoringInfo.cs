using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Limit monitoring item입니다.
    /// </summary>
    public class LimitMonitoringItem
    {
        /// <summary>
        /// Limit ID를 가져오거나 설정합니다.
        /// </summary>
        public long LimitID { get; set; }

        /// <summary>
        /// 하위 경걔를 가져오거나 설정합니다.
        /// </summary>
        public double LowerBoundary { get; set; }

        /// <summary>
        /// 상위 경걔를 가져오거나 설정합니다.
        /// </summary>
        public double UpperBoundary { get; set; }

        /// <summary>
        /// 현재 보고 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool IsReported { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public LimitMonitoringItem()
        {
            this.LimitID = 0;
            this.LowerBoundary = 0.0;
            this.UpperBoundary = 0.0;
            this.IsReported = false;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("ID={0}, Lower={1}, Upper={2}", this.LimitID, this.LowerBoundary, this.UpperBoundary);
        }
    }

    /// <summary>
    /// Limit monitoring info입니다.
    /// </summary>
    public class LimitMonitoringInfo
    {
        /// <summary>
        /// Monitoring할 variable을 가져오거나 설정합니다.
        /// </summary>
        public VariableInfo Variable { get; set; }

        /// <summary>
        /// Variable의 최소값을 가져오거나 설정합니다.
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// Variable의 최대값을 가져오거나 설정합니다.
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// Limit monitoring item을 가져오거나 설정합니다.
        /// </summary>
        public List<LimitMonitoringItem> Items { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public LimitMonitoringInfo()
        {
            this.Variable = new VariableInfo();
            this.Items = new List<LimitMonitoringItem>();

            this.Min = string.Empty;
            this.Max = string.Empty;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Variable=[{0}], Item Count={1}", this.Variable, this.Items.Count);
        }

        /// <summary>
        /// Limit monitoring item을 추가합니다.
        /// </summary>
        /// <param name="limitMonitoringItem">추가할 Limit monitoring item입니다.</param>
        public void Add(LimitMonitoringItem limitMonitoringItem)
        {
            this.Items.Add(limitMonitoringItem);
        }

        /// <summary>
        /// Limit monitoring item을 삭제합니다.
        /// </summary>
        /// <param name="limitMonitoringItem">삭제할 Limit monitoring item입니다.</param>
        public void Remove(LimitMonitoringItem limitMonitoringItem)
        {
            var varInfo = (from LimitMonitoringItem tempTraceInfo in this.Items
                           where tempTraceInfo.LimitID == limitMonitoringItem.LimitID
                           select tempTraceInfo).FirstOrDefault();

            if (varInfo != null)
            {
                this.Items.Remove(varInfo);
            }
        }

        /// <summary>
        /// 지정된 조건자에 정의된 조건과 일치하는 요소가 포함되어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="limitID">확인할 Limit ID입니다.</param>
        /// <returns>지정된 조건자에 정의된 조건과 일치하는 요소가 하나 이상 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(long limitID)
        {
            return this.Items.Exists(t => t.LimitID == limitID);
        }

        /// <summary>
        /// Variable의 값을 설정합니다.
        /// </summary>
        /// <param name="value">설정할 값입니다.</param>
        public void SetValue(dynamic value)
        {
            //private dynamic _value;
        }
    }

    /// <summary>
    /// Limit monitoring collection입니다.
    /// </summary>
    public class LimitMonitoringCollection
    {
        /// <summary>
        /// Limit monitoring info를 가져오거나 설정합니다.
        /// </summary>
        public List<LimitMonitoringInfo> Items { get; set; }

        /// <summary>
        /// Limit monitoring info를 가져옵니다.
        /// </summary>
        /// <param name="vid">가져올 VID입니다.</param>
        /// <returns>Limit monitoring info입니다.(없을 경우 null)</returns>
        public LimitMonitoringInfo this[string vid]
        {
            get { return this.Items.FirstOrDefault(t => t.Variable.VID == vid); }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public LimitMonitoringCollection()
        {
            this.Items = new List<LimitMonitoringInfo>();
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
        /// Limit monitoring info을 추가합니다.
        /// </summary>
        /// <param name="limitMonitoringInfo">추가할 Limit monitoring info입니다.</param>
        public void Add(LimitMonitoringInfo limitMonitoringInfo)
        {
            this.Items.Add(limitMonitoringInfo);
        }

        /// <summary>
        /// Limit monitoring info를 삭제합니다.
        /// </summary>
        /// <param name="limitMonitoringInfo">삭제할 Limit monitoring info입니다.</param>
        public void Remove(LimitMonitoringInfo limitMonitoringInfo)
        {
            var varInfo = (from LimitMonitoringInfo tempTraceInfo in this.Items
                           where tempTraceInfo.Variable.VID == limitMonitoringInfo.Variable.VID
                           select tempTraceInfo).FirstOrDefault();

            if (varInfo != null)
            {
                this.Items.Remove(varInfo);
            }
        }

        /// <summary>
        /// 지정된 조건자에 정의된 조건과 일치하는 요소가 포함되어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="vid">확인할 VID입니다.</param>
        /// <returns>지정된 조건자에 정의된 조건과 일치하는 요소가 하나 이상 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(string vid)
        {
            return this.Items.Exists(t => t.Variable.VID == vid);
        }
    }
}