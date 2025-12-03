using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Report 정보입니다.
    /// </summary>
    public class ReportInfo
    {
        /// <summary>
        /// Report ID를 가져오거나 설정합니다.
        /// </summary>
        public string ReportID { get; set; }

        /// <summary>
        /// Report의 Description을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Report에 Link 된 Variable 목록을 가져오거나 설정합니다.
        /// </summary>
        public VariableCollection Variables { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ReportInfo()
        {
            this.Description = string.Empty;

            this.Variables = new VariableCollection();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Report ID={0}, Description={1}, Variable Count={2}",
                this.ReportID, this.Description, this.Variables.Items.Count);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public ReportInfo CopyTo()
        {
            ReportInfo result;

            result = new ReportInfo()
            {
                ReportID = this.ReportID,
                Description = this.Description,
                Variables = this.Variables.CopyTo()
            };

            return result;
        }
    }

    /// <summary>
    /// Report Collection 정보입니다.
    /// </summary>
    public class ReportCollection
    {
        /// <summary>
        /// Report 정보를 가져오거나 설정합니다.
        /// </summary>
        public Dictionary<string, ReportInfo> Items { get; set; }

        /// <summary>
        /// Report 정보를 가져옵니다.
        /// </summary>
        /// <param name="reportId">가져올 Report ID입니다.</param>
        /// <returns></returns>
        public ReportInfo this[string reportId]
        {
            get
            {
                return this.Items.ContainsKey(reportId) == true ? this.Items[reportId] : null;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ReportCollection()
        {
            this.Items = new Dictionary<string, ReportInfo>();
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
        /// Report 정보를 추가합니다.
        /// </summary>
        /// <param name="reportInfo">추가할 Report 정보입니다.</param>
        public void Add(ReportInfo reportInfo)
        {
            this.Items[reportInfo.ReportID] = reportInfo;
        }

        /// <summary>
        /// Report 정보를 삭제합니다.
        /// </summary>
        /// <param name="reportInfo">삭제할 Report 정보입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Remove(ReportInfo reportInfo)
        {
            return this.Items.Remove(reportInfo.ReportID);
        }

        /// <summary>
        /// Report 정보를 삭제합니다.
        /// </summary>
        /// <param name="reportId">삭제할 Report ID입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Remove(string reportId)
        {
            return this.Items.Remove(reportId);
        }

        /// <summary>
        /// 지정된 Report 정보가 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="reportId">검사할 Report ID입니다.</param>
        /// <returns>Report 정보가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(string reportId)
        {
            return this.Items.ContainsKey(reportId);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public ReportCollection CopyTo()
        {
            ReportCollection result;

            result = new ReportCollection();

            foreach (KeyValuePair<string, ReportInfo> temp in this.Items)
            {
                result.Items[temp.Key] = temp.Value.CopyTo();
            }

            return result;
        }
    }
}