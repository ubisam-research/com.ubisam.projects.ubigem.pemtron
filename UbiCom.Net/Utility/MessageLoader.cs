using System;
using System.Xml.Linq;

using UbiCom.Net.Driver;

namespace UbiCom.Net.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageLoader : IDisposable
    {
        private Structure.SECSMessageManager _secsMessageMgr;
        private HSMSDriver _driver;
        private bool _disposed;

        /// <summary>
        /// SECS Message Structure를 가져옵니다.
        /// </summary>
        public Structure.SECSMessageManager SECSMessageMgr
        {
            get { return this._secsMessageMgr; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public MessageLoader(HSMSDriver driver)
        {
            this._driver = driver;
            this._secsMessageMgr = new Structure.SECSMessageManager();

            this._disposed = false;
        }

        /// <summary>
        /// 기본 소멸자입니다.
        /// </summary>
        ~MessageLoader()
        {
            Dispose(false);
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 애플리케이션 정의 작업을 수행합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 애플리케이션 정의 작업을 수행합니다.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (this._driver != null)
                    {
                        this._driver = null;
                    }

                    if (this._secsMessageMgr != null)
                    {
                        this._secsMessageMgr = null;
                    }
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Message Structure를 load합니다.
        /// </summary>
        /// <param name="configuration">환경 설정 정보입니다.</param>
        /// <returns></returns>
        public string Load(Structure.Configurtion configuration)
        {
            string result;
            XElement root;

            if (System.IO.File.Exists(configuration.UMDFileName) == false)
            {
                result = Resources.ErrorString.DoseNotExistUMDFile;
            }
            else
            {
                root = XElement.Load(configuration.UMDFileName);

                this._secsMessageMgr = new Structure.SECSMessageManager();

                this._secsMessageMgr.Initialize(configuration);
                result = this._secsMessageMgr.Load(root);
            }

            return result;
        }

        /// <summary>
        /// Message Structure를 load합니다.
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public string Load(XElement rootElement)
        {
            string result;

            if (rootElement != null)
            {
                this._secsMessageMgr = new Structure.SECSMessageManager();

                result = this._secsMessageMgr.Load(rootElement);
            }
            else
            {
                result = "Element is null";
            }

            return result;
        }

        /// <summary>
        /// 수신 Message를 validate합니다.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Structure.MessageValidationError ValidateReceivedMessage(Structure.SECSMessage message)
        {
            return this._secsMessageMgr.ValidateReceivedMessage(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public Structure.SECSMessage GetSECSMessage(int stream, int function)
        {
            return this._secsMessageMgr[stream, function].Clone();
        }
    }
}