using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Trace 정보입니다.
    /// </summary>
    public class TraceInfo
    {
        private string _dsper;
        private long _totalMillisecond;
        private string _timeFormat;

        /// <summary>
        /// Trace ID를 가져오거나 설정합니다.
        /// </summary>
        public string TraceID { get; set; }

        /// <summary>
        /// Data sample period을 가져오거나 설정합니다.
        /// </summary>
        public string Dsper
        {
            get { return this._dsper; }
            set
            {
                this._dsper = value;

                if (string.IsNullOrEmpty(this._dsper) == false)
                {
                    if (this.Dsper.Length == 6)
                    {
                        this._totalMillisecond = (long.Parse(this.Dsper.Substring(0, 2)) * 60 * 60 +
                                                  long.Parse(this.Dsper.Substring(2, 2)) * 60 +
                                                  long.Parse(this.Dsper.Substring(4, 2))) * 1000;
                    }
                    else if (this.Dsper.Length == 8)
                    {
                        this._totalMillisecond = ((long.Parse(this.Dsper.Substring(0, 2)) * 60 * 60 +
                                                   long.Parse(this.Dsper.Substring(2, 2)) * 60 +
                                                   long.Parse(this.Dsper.Substring(4, 2))) * 1000) +
                                                  (long.Parse(this.Dsper.Substring(6, 2)) * 10);
                    }
                    else
                    {
                        this._totalMillisecond = 0;
                    }
                }
                else
                {
                    this._totalMillisecond = 0;
                }
            }
        }

        /// <summary>
        /// Total samples to be made를 가져오거나 설정합니다.
        /// </summary>
        public long TotalSample { get; set; }

        /// <summary>
        /// Reporting group size를 가져오거나 설정합니다.
        /// </summary>
        public long ReportGroupSize { get; set; }

        /// <summary>
        /// 현재 Sample number를 가져오거나 설정합니다. 
        /// </summary>
        public long CurrentSample { get; set; }

        /// <summary>
        /// Group 보고할 경우 현재 보관중인 Sample의 개수를 가져오거나 설정합니다.
        /// </summary>
        public long KeepCount { get; set; }

        /// <summary>
        /// Trace 대상 Variable 정보를 가져오거나 설정합니다.
        /// </summary>
        public List<VariableInfo> Variables { get; set; }

        /// <summary>
        /// 수집된 Trace value를 가져오거나 설정합니다.
        /// </summary>
        public List<VariableInfo> Values { get; set; }

        /// <summary>
        /// 수집 주기를 가져오거나 설정합니다.
        /// </summary>
        public long TotalMillisecond
        {
            get { return this._totalMillisecond; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public TraceInfo()
        {
            this.CurrentSample = 0;
            this.KeepCount = 0;

            this.Variables = new List<VariableInfo>();
            this.Values = new List<VariableInfo>();

            this._timeFormat = "yyyyMMddHHmmssff";
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("TID={0}, Period={1}, Total Sample Number={2}, Group Size={2}, Variable Count={4}",
                this.TraceID, this.Dsper, this.TotalSample, this.ReportGroupSize, this.Variables.Count);
        }

        /// <summary>
        /// Trace report시 사용할 Time Format을 설정합니다.
        /// </summary>
        /// <param name="variableInfo"></param>
        public void SetTimeFormat(VariableInfo variableInfo)
        {
            if (variableInfo != null && variableInfo.Value.Length == 12)
            {
                this._timeFormat = "yyMMddHHmmss";
            }
            else
            {
                this._timeFormat = "yyyyMMddHHmmssff";
            }
        }

        /// <summary>
        /// Trace value를 추가합니다.
        /// </summary>
        public void AddValue(List<VariableInfo> currentVariables)
        {
            foreach (VariableInfo tempVariableInfo in currentVariables)
            {
                CheckVariableValue(tempVariableInfo);

                this.Values.Add(tempVariableInfo.CopyTo());
            }
        }

        /// <summary>
        /// Trace value를 추가합니다.
        /// </summary>
        /// <param name="variableInfo">추가할 Variable입니다.</param>
        public void AddValue(VariableInfo variableInfo)
        {
            CheckVariableValue(variableInfo);

            this.Values.Add(variableInfo.CopyTo());
        }

        /// <summary>
        /// 모든 Trace varible을 삭제합니다.
        /// </summary>
        public void ClearValue()
        {
            this.Values.Clear();
        }

        private void CheckVariableValue(VariableInfo variableInfo)
        {
            bool assingedValue;

            assingedValue = false;

            if (variableInfo.PreDefined == true)
            {
                if (variableInfo.Name == PreDefinedV.Clock.ToString())
                {
                    assingedValue = true;

                    variableInfo.Value.SetValue(DateTime.Now.ToString(this._timeFormat));
                }
            }

            if (assingedValue == false && variableInfo.Value.IsEmpty == true)
            {
                switch (variableInfo.Format)
                {
                    case UbiCom.Net.Structure.SECSItemFormat.I1:
                    case UbiCom.Net.Structure.SECSItemFormat.I2:
                    case UbiCom.Net.Structure.SECSItemFormat.I4:
                    case UbiCom.Net.Structure.SECSItemFormat.I8:
                    case UbiCom.Net.Structure.SECSItemFormat.U1:
                    case UbiCom.Net.Structure.SECSItemFormat.U2:
                    case UbiCom.Net.Structure.SECSItemFormat.U4:
                    case UbiCom.Net.Structure.SECSItemFormat.U8:
                    case UbiCom.Net.Structure.SECSItemFormat.F4:
                    case UbiCom.Net.Structure.SECSItemFormat.F8:
                    case UbiCom.Net.Structure.SECSItemFormat.B:
                        variableInfo.Value.SetValue(0);

                        break;
                    case UbiCom.Net.Structure.SECSItemFormat.Boolean:
                        variableInfo.Value.SetValue(false);
                        break;
                    case UbiCom.Net.Structure.SECSItemFormat.J:
                    case UbiCom.Net.Structure.SECSItemFormat.A:
                        variableInfo.Value.SetValue(string.Empty);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Trace Collection 정보입니다.
    /// </summary>
    public class TraceCollection
    {
        /// <summary>
        /// Trace 정보를 가져오거나 설정합니다.
        /// </summary>
        public List<TraceInfo> Items { get; set; }

        /// <summary>
        /// Trace 정보를 가져옵니다.
        /// </summary>
        /// <param name="traceId">가져올 Trace ID입니다.</param>
        /// <returns>Trace 정보입니다.(단, 없을 경우 null)</returns>
        public TraceInfo this[string traceId]
        {
            get { return this.Items.FirstOrDefault(t => t.TraceID == traceId); }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public TraceCollection()
        {
            this.Items = new List<TraceInfo>();
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
        /// Trace 정보를 추가합니다.
        /// </summary>
        /// <param name="traceInfo">추가할 Trace 정보입니다.</param>
        public void Add(TraceInfo traceInfo)
        {
            this.Items.Add(traceInfo);
        }

        /// <summary>
        /// Trace 정보를 삭제합니다.
        /// </summary>
        /// <param name="traceInfo">삭제할 Trace 정보입니다.</param>
        public void Remove(TraceInfo traceInfo)
        {
            var varInfo = (from TraceInfo tempTraceInfo in this.Items
                           where tempTraceInfo.TraceID == traceInfo.TraceID
                           select tempTraceInfo).FirstOrDefault();

            if (varInfo != null)
            {
                this.Items.Remove(varInfo);
            }
        }

        /// <summary>
        /// 지정된 Trace 정보가 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="traceId">검사할 Trace ID입니다.</param>
        /// <returns>Trace 정보가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(string traceId)
        {
            return this.Items.Exists(t => t.TraceID == traceId);
        }
    }
}