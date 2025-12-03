using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Service parameter value 정보입니다.
    /// </summary>
    public class ServiceParameterValueItem
    {
        /// <summary>
        /// Service parameter data를 가져오거나 설정합니다.
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// Attribute data의 SECS format을 가져오거나 설정합니다.
        /// </summary>
        public SECSItemFormat ParameterValueFormat { get; set; }

        /// <summary>
        /// Attribute data의 child를 가져오거나 설정합니다. (AttributeDataFormat = L인 경우)
        /// </summary>
        public List<ServiceParameterValueItem> ChildItems { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ServiceParameterValueItem()
        {
            this.ParameterValueFormat = SECSItemFormat.A;

            this.ChildItems = new List<ServiceParameterValueItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            if (this.ParameterValueFormat == SECSItemFormat.L)
            {
                return string.Format("Data Format={0}, Data={1}, Child Count={2}",
                    this.ParameterValueFormat, this.ParameterValue, this.ChildItems.Count);
            }
            else
            {
                return string.Format("Data Format={0}, Data={1}",
                    this.ParameterValueFormat, this.ParameterValue);
            }
        }
    }


    /// <summary>
    /// Service parameter 정보입니다.
    /// </summary>
    public class ServiceParameterInfo
    {
        /// <summary>
        /// Service parameter name을 가져오거나 설정합니다.
        /// </summary>
        public string SPName { get; set; }

        /// <summary>
        /// Service parameter data를 가져오거나 설정합니다.
        /// </summary>
        public ServiceParameterValueItem SPValue { get; set; }

        /// <summary>
        /// Attribute data를 가져옵니다. (단, Data format이 List가 아닌 경우)
        /// </summary>
        public string Value
        {
            get
            {
                if (this.SPValue.ParameterValueFormat != SECSItemFormat.L)
                {
                    return this.SPValue.ParameterValue;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ServiceParameterInfo()
        {
            this.SPValue = new ServiceParameterValueItem();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("SPName={0}, SPValue=[{1}]",
                this.SPName, this.SPValue);
        }
    }

    /// <summary>
    /// Generic service 정보입니다.
    /// </summary>
    public class GenericServiceInfo
    {
        /// <summary>
        /// Service name을 가져오거나 설정합니다.
        /// </summary>
        public string DataID { get; set; }

        /// <summary>
        /// Operation ID를 가져오거나 설정합니다.
        /// </summary>
        public int OperationID { get; set; }

        /// <summary>
        /// Object spec을 가져오거나 설정합니다.
        /// </summary>
        public string ObjectSpec { get; set; }

        /// <summary>
        /// Service name을 가져오거나 설정합니다.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Service parameter data를 가져오거나 설정합니다.
        /// </summary>
        public List<ServiceParameterInfo> ServiceParameter { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public GenericServiceInfo()
        {
            this.ServiceParameter = new List<ServiceParameterInfo>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Dta ID={0}, Object ID={1}, Object spec={2}, SP name={3}, Parameter count={4}",
                this.DataID, this.OperationID, this.ObjectSpec, this.ServiceName, this.ServiceParameter.Count);
        }
    }
}