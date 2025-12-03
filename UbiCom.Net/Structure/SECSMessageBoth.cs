namespace UbiCom.Net.Structure
{
    /// <summary>
    /// 양방향 SECS message입니다.
    /// </summary>
    public class SECSMessageBoth : SECSMessageWrapper
    {
        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessageBoth(Driver.HSMSDriver driver, string messageName)
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