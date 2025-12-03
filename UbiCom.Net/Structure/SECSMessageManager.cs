using System;
using System.Linq;
using System.Xml.Linq;

namespace UbiCom.Net.Structure
{
    /// <summary>
    /// SECS Message manager입니다.
    /// </summary>
    public class SECSMessageManager : IDisposable
    {
        #region [HeaderSchema - Class]
        private static class HeaderSchema
        {
            public const string MESSAGE_NAME = "MessageName";
            public const string STREAM = "Stream";
            public const string FUNCTION = "Function";
            public const string DIRECTION = "Direction";
            public const string WAIT_BIT = "WaitBit";
            public const string AUTO_REPLY = "AutoReply";
            public const string NO_LOGGING = "NoLogging";
            public const string DESCRIPTION = "Description";
        }
        #endregion

        #region [BodySchema - Class]
        private static class BodySchema
        {
            public const string ITEM_NAME = "ItemName";
            public const string COUNT = "Count";
            public const string IS_FIXED = "IsFixed";
            public const string DEFAULT_VALUE = "DefaultValue";
        }
        #endregion

        private SECSMessageCollection _messageInfo;
        private SECSMessageValidate _validateMessage;
        private int _deviceId;
        private DeviceType _deviceType;
        private double _maxMessageSize;
        private bool _disposed;

        /// <summary>
        /// SECS Mesage를 가져오거나 설정합니다.
        /// </summary>
        public SECSMessageCollection Messages
        {
            get { return this._messageInfo; }
            set { this._messageInfo = value; }
        }

        /// <summary>
        /// SECS Mesage를 가져옵니다.
        /// </summary>
        /// <param name="stream">Stream no입니다.</param>
        /// <param name="function">Function no입니다.</param>
        /// <returns>SECS Mesage입니다.</returns>
        public SECSMessage this[int stream, int function]
        {
            get
            {
                SECSMessage result;
                SECSMessageCollection secsMessageCollection;

                try
                {
                    secsMessageCollection = this._messageInfo[stream, function];

                    if (secsMessageCollection != null)
                        result = this._messageInfo[stream, function].MessageInfo.FirstOrDefault().Value;
                    else
                        result = null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);

                    result = null;
                }

                return result;
            }
        }

        /// <summary>
        /// SECS Mesage를 가져옵니다.
        /// </summary>
        /// <param name="stream">Stream no입니다.</param>
        /// <param name="function">Function no입니다.</param>
        /// <param name="direction">Direction입니다.</param>
        /// <returns>SECS Mesage입니다.</returns>
        public SECSMessage this[int stream, int function, SECSMessageDirection direction]
        {
            get
            {
                SECSMessage result;

                try
                {
                    result = (from SECSMessage tempSECSMessage in this._messageInfo[stream, function].MessageInfo.Select(t => t.Value)
                              where tempSECSMessage.Stream == stream &&
                                    tempSECSMessage.Function == function &&
                                    (tempSECSMessage.Direction == SECSMessageDirection.Both || tempSECSMessage.Direction == direction)
                              select tempSECSMessage).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);

                    result = null;
                }

                return result;
            }
        }

        /// <summary>
        /// SECS Mesage를 가져옵니다.
        /// </summary>
        /// <param name="stream">Stream no입니다.</param>
        /// <param name="function">Function no입니다.</param>
        /// <param name="deviceType">Device Type입니다.</param>
        /// <returns>SECS Mesage입니다.</returns>
        public SECSMessage this[int stream, int function, DeviceType deviceType]
        {
            get
            {
                SECSMessage result;

                try
                {
                    if (deviceType == DeviceType.Host)
                    {
                        result = (from SECSMessage tempSECSMessage in this._messageInfo[stream, function].MessageInfo.Select(t => t.Value)
                                  where tempSECSMessage.Stream == stream &&
                                        tempSECSMessage.Function == function &&
                                        (tempSECSMessage.Direction == SECSMessageDirection.Both || tempSECSMessage.Direction == SECSMessageDirection.ToEquipment)
                                  select tempSECSMessage).FirstOrDefault();
                    }
                    else
                    {
                        result = (from SECSMessage tempSECSMessage in this._messageInfo[stream, function].MessageInfo.Select(t => t.Value)
                                  where tempSECSMessage.Stream == stream &&
                                        tempSECSMessage.Function == function &&
                                        (tempSECSMessage.Direction == SECSMessageDirection.Both || tempSECSMessage.Direction == SECSMessageDirection.ToHost)
                                  select tempSECSMessage).FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);

                    result = null;
                }

                return result;
            }
        }

        /// <summary>
        /// SECS Mesage를 가져옵니다.
        /// </summary>
        /// <param name="messageName">Message name입니다.</param>
        /// <returns>SECS Mesage입니다.</returns>
        public SECSMessage this[string messageName]
        {
            get { return this._messageInfo[messageName]; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSMessageManager()
        {
            this._messageInfo = new SECSMessageCollection();
            this._validateMessage = new SECSMessageValidate();

            this._disposed = false;
        }

        /// <summary>
        /// 기본 소멸자입니다.
        /// </summary>
        ~SECSMessageManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// SECS Message manager를 초기화합니다.
        /// </summary>
        /// <param name="config">환경 설정 정보입니다.</param>
        public void Initialize(Structure.Configurtion config)
        {
            this._deviceType = config.DeviceType;

            this._deviceId = config.DeviceID;
            this._maxMessageSize = config.MaxMessageSize;
        }

        /// <summary>
        /// Message 구조를 로드합니다.
        /// </summary>
        /// <param name="umdFileName">Message 구조 파일입니다.</param>
        /// <returns></returns>
        public string Load(string umdFileName)
        {
            string result;
            Utility.SECSMessageLoader loader;

            if (System.IO.File.Exists(umdFileName) == false)
            {
                this._messageInfo = null;

                result = Resources.ErrorString.DoseNotExistUMDFile;
            }
            else
            {
                loader = new Utility.SECSMessageLoader();

                result = loader.Load(umdFileName);

                if (string.IsNullOrEmpty(result) == true)
                {
                    this._messageInfo = loader.Message;

                    result = this._validateMessage.MakeOriginalMessage(this._deviceType, this._deviceId, this._maxMessageSize, this._messageInfo);
                }
                else
                {
                    this._messageInfo = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Message 구조를 로드합니다.
        /// </summary>
        /// <param name="rootElement">Message 구조의 root element입니다.</param>
        /// <returns></returns>
        public string Load(XElement rootElement)
        {
            string result;
            Utility.SECSMessageLoader loader;

            loader = new Utility.SECSMessageLoader();

            result = loader.Load(rootElement);

            if (string.IsNullOrEmpty(result) == true)
            {
                this._messageInfo = loader.Message;

                result = this._validateMessage.MakeOriginalMessage(this._deviceType, this._deviceId, this._maxMessageSize, this._messageInfo);
            }
            else
            {
                this._messageInfo = null;
            }

            return result;
        }

        /// <summary>
        /// 할당된 리소스를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 할당된 리소스를 해제합니다.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (this._messageInfo != null)
                        this._messageInfo = null;

                    if (this._validateMessage != null)
                        this._validateMessage = null;
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// 사용자 정의 SECS Message를 추가합니다.
        /// </summary>
        /// <param name="message">추가 할 message입니다.</param>
        public void AddUserDefinedMessage(SECSMessage message)
        {
            this._messageInfo.Add(message);
            this._validateMessage.MakeOriginalMessage(message);
        }

        /// <summary>
        /// 수신 Message를 validate합니다.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public MessageValidationError ValidateReceivedMessage(SECSMessage message)
        {
            MessageValidationError result;

            result = this._validateMessage.ValidateReceivedMessage(message);

            return result;
        }

        /// <summary>
        /// 사용자 정의 Message의 구조를 Update합니다.(UMD 사용하지 않을 경우)
        /// </summary>
        /// <param name="message">Update할 Message입니다.</param>
        public void UpdateUserMessageStructure(SECSMessage message)
        {
            SECSMessage source;

            source = this._messageInfo[message.Name];

            if (source != null)
            {
                this._messageInfo[message.Name] = message;
                this._validateMessage.UpdateOriginalMessage(message);
            }
        }

        public void SetDeviceID(int nDeviceID)
        {
            _deviceId = nDeviceID;
            _validateMessage._deviceId = nDeviceID;
        }
    }
}