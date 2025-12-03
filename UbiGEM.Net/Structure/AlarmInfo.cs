using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Alarm info입니다.
    /// </summary>
    public class AlarmInfo
    {
        /// <summary>
        /// Alarm ID를 가져오거나 설정합니다.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// Alarm Code를 가져오거나 설정합니다.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Alarm 사용 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Alarm Text를 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public AlarmInfo()
        {
            this.Description = string.Empty;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("ALID={0}, ALCD={1}, Enabled={2}, ALTX={3}",
                this.ID, this.Code, this.Enabled, this.Description);
        }
    }

    /// <summary>
    /// Alarm info colelction입니다.
    /// </summary>
    public class AlarmCollection
    {
        /// <summary>
        /// Alarm info를 가져오거나 설정합니다.
        /// </summary>
        public List<AlarmInfo> Items { get; set; }

        /// <summary>
        /// Alarm info를 가져옵니다.
        /// </summary>
        /// <param name="alarmId">검색할 Alarm ID입니다.</param>
        /// <returns>Alarm info입니다.(없을 경우 null)</returns>
        public AlarmInfo this[long alarmId]
        {
            get { return this.Items.FirstOrDefault(t => t.ID == alarmId); }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public AlarmCollection()
        {
            this.Items = new List<AlarmInfo>();
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
        /// Alarm info를 추가합니다.
        /// </summary>
        /// <param name="alarmInfo">추가할 alarm info입니다.</param>
        public void Add(AlarmInfo alarmInfo)
        {
            this.Items.Add(alarmInfo);
        }

        /// <summary>
        /// Alarm info를 삭제합니다.
        /// </summary>
        /// <param name="alarmInfo">삭제할 alarm info입니다.</param>
        public void Remove(AlarmInfo alarmInfo)
        {
            var varAlarmInfo = (from AlarmInfo tempAlarmInfo in this.Items
                                where tempAlarmInfo.ID == alarmInfo.ID &&
                                      tempAlarmInfo.Code == alarmInfo.Code &&
                                      tempAlarmInfo.Enabled == alarmInfo.Enabled &&
                                      tempAlarmInfo.Description == alarmInfo.Description
                                select tempAlarmInfo).FirstOrDefault();

            if (varAlarmInfo != null)
            {
                this.Items.Remove(varAlarmInfo);
            }
        }

        /// <summary>
        /// Alarm 사용 여부를 설정합니다.
        /// </summary>
        /// <param name="enabled">사용 여부입니다.</param>
        public void SetEnabled(bool enabled)
        {
            this.Items.ForEach(t =>
            {
                t.Enabled = enabled;
            });
        }

        /// <summary>
        /// Alarm 사용 여부를 설정합니다.
        /// </summary>
        /// <param name="enabled">사용 여부입니다.</param>
        /// <param name="alid">설정할 Alarm ID입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public bool SetEnabled(bool enabled, long alid)
        {
            bool result;
            AlarmInfo alarmInfo;

            alarmInfo = this.Items.FirstOrDefault(t => t.ID == alid);

            if (alarmInfo != null)
            {
                alarmInfo.Enabled = enabled;

                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}