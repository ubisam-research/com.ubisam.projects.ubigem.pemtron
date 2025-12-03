namespace UbiCom.Net.Structure
{
    /// <summary>
    /// 수신 message정보입니다.
    /// </summary>
    public class SECSMessageReceive : SECSMessageWrapper
    {
        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessageReceive(Driver.HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
        }
    }
}