namespace UbiCom.Net.Structure
{
    /// <summary>
    /// Primary message 정보입니다.
    /// </summary>
    public class SECSMessageRequest : SECSMessageWrapper
    {
        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessageRequest(Driver.HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
        }

        /// <summary>
        /// Message를 송신합니다.
        /// </summary>
        /// <returns>송신 결과입니다</returns>
        public MessageError Request()
        {
            MessageError result;

            if (MakeSECSMessage() == true)
            {
                result = this._driver.SendSECSMessage(this);
            }
            else
            {
                result = MessageError.DataIsNull;
            }

            return result;
        }
    }
}