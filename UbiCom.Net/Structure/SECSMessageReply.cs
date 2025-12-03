namespace UbiCom.Net.Structure
{
    /// <summary>
    /// Secondary message 정보입니다.
    /// </summary>
    public class SECSMessageReply : SECSMessageWrapper
    {
        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessageReply(Driver.HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
        }

        /// <summary>
        /// Secondary Message를 송신합니다.
        /// </summary>
        /// <param name="primaryMessage">연관된 Primary message입니다.</param>
        /// <returns>송신 결과입니다</returns>
        public MessageError Reply(SECSMessage primaryMessage)
        {
            MessageError result;

            if (MakeSECSMessage() == true)
            {
                result = this._driver.ReplySECSMessage(primaryMessage, this);
            }
            else
            {
                result = MessageError.DataIsNull;
            }

            return result;
        }
        /// <summary>
        /// Secondary Message를 송신합니다.
        /// </summary>
        /// <param name="systemBytes">연관된 Primary System Bytes입니다.</param>
        /// <returns>송신 결과입니다</returns>
        public MessageError Reply(uint systemBytes)
        {
            MessageError result;

            if (MakeSECSMessage() == true)
            {
                result = this._driver.ReplySECSMessage(systemBytes, this);
            }
            else
            {
                result = MessageError.DataIsNull;
            }

            return result;
        }
    }
}