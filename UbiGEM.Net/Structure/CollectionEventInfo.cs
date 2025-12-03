using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Collection event 정보입니다.
    /// </summary>
    public class CollectionEventInfo
    {
        /// <summary>
        /// GEM driver 기본 등록 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool PreDefined { get; set; }

        /// <summary>
        /// Collection event ID를 가져오거나 설정합니다.
        /// </summary>
        public string CEID { get; set; }

        /// <summary>
        /// Collection event name을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection event description을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Collection event 사용 여부를 가져오거나 설정합니다.(GEM Driver)
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// Collection event 사용 여부를 가져오거나 설정합니다.(HOST)
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Base collection event 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool IsBase { get; set; }

        /// <summary>
        /// Link된 report를 가져오거나 설정합니다.
        /// </summary>
        public ReportCollection Reports { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public CollectionEventInfo()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;

            this.Reports = new ReportCollection();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("CEID={0}, Name={1}, Pre-Define={2}, Use={3}, Enable={4}, Description={5}, Report Count={6}",
                this.CEID, this.Name, this.PreDefined, this.IsUse, this.Enabled, this.Description, this.Reports.Items.Count);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public CollectionEventInfo CopyTo()
        {
            CollectionEventInfo result;

            result = new CollectionEventInfo()
            {
                CEID = this.CEID,
                Name = this.Name,
                Enabled = this.Enabled,
                IsBase = this.IsBase,
                IsUse = this.IsUse,
                PreDefined = this.PreDefined,
                Description = this.Description,
                Reports = this.Reports.CopyTo()
            };

            return result;
        }
    }

    /// <summary>
    /// Collection event collection 정보입니다.
    /// </summary>
    public class CollectionEventCollection
    {
        /// <summary>
        /// Collection event를 가져오거나 설정합니다,
        /// </summary>
        public Dictionary<string, CollectionEventInfo> Items { get; set; }

        /// <summary>
        /// Collection event를 가져옵니다.
        /// </summary>
        /// <param name="ceid">가져올 CEID입니다.</param>
        /// <returns>Collection event입니다.</returns>
        public CollectionEventInfo this[string ceid]
        {
            get
            {
                if (this.Items.ContainsKey(ceid) == true &&
                    this.Items[ceid].IsUse == true)
                {
                    return this.Items[ceid];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public CollectionEventCollection()
        {
            this.Items = new Dictionary<string, CollectionEventInfo>();
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
        /// Collection event를 추가합니다.
        /// </summary>
        /// <param name="ceidInfo">추가할 collection event입니다.</param>
        public void Add(CollectionEventInfo ceidInfo)
        {
            this.Items[ceidInfo.CEID] = ceidInfo;
        }

        /// <summary>
        /// Collection event를 삭제합니다.
        /// </summary>
        /// <param name="ceidInfo">삭제할 collection event입니다.</param>
        /// <returns></returns>
        public bool Remove(CollectionEventInfo ceidInfo)
        {
            return this.Items.Remove(ceidInfo.CEID);
        }

        /// <summary>
        /// CEID와 일치하는 요소가 포함되어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="ceid">검색할 CEID입니다.</param>
        /// <returns>지정한 요소가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(string ceid)
        {
            return this.Items.Any(t => (t.Value != null && t.Value.IsUse == true && t.Key == ceid));
        }

        /// <summary>
        /// Collection event를 가져옵니다.
        /// </summary>
        /// <param name="name">가져올 name입니다.</param>
        /// <returns>Collection event입니다.</returns>
        public CollectionEventInfo GetCollectionEventInfo(string name)
        {
            var varCollectionEvent = from temp in this.Items
                                     where temp.Value.IsUse == true &&
                                           temp.Value.Name == name
                                     select temp;

            if (varCollectionEvent != null && varCollectionEvent.Count() > 0)
            {
                return varCollectionEvent.First().Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public CollectionEventCollection CopyTo()
        {
            CollectionEventCollection result;

            result = new CollectionEventCollection();

            foreach (KeyValuePair<string, CollectionEventInfo> temp in this.Items)
            {
                result.Items[temp.Key] = temp.Value.CopyTo();
            }

            return result;
        }
    }
}