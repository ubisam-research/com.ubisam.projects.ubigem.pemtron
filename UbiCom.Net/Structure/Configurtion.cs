using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace UbiCom.Net.Structure
{
    /// <summary>
    /// Driver 환경 설정 Class입니다.
    /// </summary>
    /// 
    [Serializable]
    public class Configurtion
    {
        private const int DEFAULT_T3 = 45;
        private const int DEFAULT_T5 = 10;
        private const int DEFAULT_T6 = 5;
        private const int DEFAULT_T7 = 10;
        private const int DEFAULT_T8 = 5;
        private const int DEFAULT_LINKTEST = 120;

        #region [SECS1 - Class]
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class SECS1
        {
        }
        #endregion

        #region [HSMS - Class]
        /// <summary>
        /// HSMS 환경 설정 Value입니다.
        /// </summary>
        /// 
        [Serializable]
        public class HSMS
        {
            private int _t3;
            private int _t5;
            private int _t6;
            private int _t7;
            private int _t8;
            private int _linktest;

            /// <summary>
            /// HSMS Mode를 가져오거나 설정합니다.(Default Value = Active)
            /// </summary>
            public HSMSMode HSMSMode { get; set; }

            /// <summary>
            /// Remote IP Address를 가져오거나 설정합니다.(Active Mode)
            /// </summary>
            public string RemoteIPAddress { get; set; }

            /// <summary>
            /// Remote Port No를 가져오거나 설정합니다.(Active Mode)
            /// </summary>
            public int RemotePortNo { get; set; }

            /// <summary>
            /// Local IP Address를 가져오거나 설정합니다.(Passive Mode)
            /// </summary>
            public string LocalIPAddress { get; set; }

            /// <summary>
            /// Local Port No를 가져오거나 설정합니다.(Passive Mode)
            /// </summary>
            public int LocalPortNo { get; set; }

            /// <summary>
            /// T3 Timeout을 가져오거나 설정합니다.(단위 = s, Default Value = 45)
            /// </summary>
            public int T3
            {
                get { return this._t3; }
                set { this._t3 = (value <= 0) ? DEFAULT_T3 : value; }
            }

            /// <summary>
            /// T5 Timeout을 가져오거나 설정합니다.(단위 = s, Default Value = 10)
            /// </summary>
            public int T5
            {
                get { return this._t5; }
                set { this._t5 = (value <= 0) ? DEFAULT_T5 : value; }
            }

            /// <summary>
            /// T6 Timeout을 가져오거나 설정합니다.(단위 = s, Default Value = 5)
            /// </summary>
            public int T6
            {
                get { return this._t6; }
                set { this._t6 = (value <= 0) ? DEFAULT_T6 : value; }
            }

            /// <summary>
            /// T7 Timeout을 가져오거나 설정합니다.(단위 = s, Default Value = 10)
            /// </summary>
            public int T7
            {
                get { return this._t7; }
                set { this._t7 = (value <= 0) ? DEFAULT_T7 : value; }
            }

            /// <summary>
            /// T8 Timeout을 가져오거나 설정합니다.(단위 = s, Default Value = 5)
            /// </summary>
            public int T8
            {
                get { return this._t8; }
                set { this._t8 = (value <= 0) ? DEFAULT_T8 : value; }
            }

            /// <summary>
            /// Link Test 주기를 가져오거나 설정합니다.(단위 = s, Default Value = 120)
            /// </summary>
            public int LinkTest
            {
                get { return this._linktest; }
                set { this._linktest = (value <= 0) ? DEFAULT_LINKTEST : value; }
            }

            /// <summary>
            /// 기본 생성자입니다.
            /// </summary>
            public HSMS()
            {
                this.HSMSMode = HSMSMode.Active;
                this.RemoteIPAddress = string.Empty;
                this.RemotePortNo = 0;
                this.LocalIPAddress = string.Empty;
                this.LocalPortNo = 0;

                this._t3 = DEFAULT_T3;
                this._t5 = DEFAULT_T5;
                this._t6 = DEFAULT_T6;
                this._t7 = DEFAULT_T7;
                this._t8 = DEFAULT_T8;
                this._linktest = DEFAULT_LINKTEST;
            }

            /// <summary>
            /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
            /// </summary>
            /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
            public override string ToString()
            {
                if (this.HSMSMode == HSMSMode.Active)
                {
                    return string.Format("Mode={0}, Remote IP={1}, Remote Port={2}, T3={3}, T5={4}, T6={5}, T7={6}, T8={7}, Link Test={8}",
                                         this.HSMSMode, this.RemoteIPAddress, this.RemotePortNo,
                                         this.T3, this.T5, this.T6, this.T7, this.T8, this.LinkTest);
                }
                else
                {
                    return string.Format("Mode={0}, Local IP={1}, Local Port={2}, T3={3}, T5={4}, T6={5}, T7={6}, T8={7}, Link Test={8}",
                                         this.HSMSMode, this.LocalIPAddress, this.LocalPortNo,
                                         this.T3, this.T5, this.T6, this.T7, this.T8, this.LinkTest);
                }
            }

            /// <summary>
            /// HSMS 환경 설정을 검증합니다.
            /// </summary>
            /// <param name="errorText">검증 실패 사유입니다.(검증 성공 시 string.Empty)</param>
            /// <returns>검증 결과입니다.</returns>
            public Structure.DriverError ValidateConfiguration(out string errorText)
            {
                Structure.DriverError result = Structure.DriverError.Ok;

                errorText = string.Empty;

                try
                {
                    if (this.T3 <= 0 || this.T5 <= 0 || this.T6 <= 0 || this.T7 <= 0 || this.T8 <= 0)
                    {
                        result = Structure.DriverError.InvalidConfiguration;

                        errorText = Resources.ErrorString.InvalidTimeout;
                    }
                    else
                    {
                        if (this.HSMSMode == HSMSMode.Active)
                        {
                            if (System.Net.IPAddress.TryParse(this.RemoteIPAddress, out System.Net.IPAddress address) == false)
                            {
                                result = Structure.DriverError.InvalidConfiguration;

                                errorText = Resources.ErrorString.InvalidIPAddress;
                            }
                            else if (this.RemotePortNo <= 0)
                            {
                                result = Structure.DriverError.InvalidConfiguration;

                                errorText = Resources.ErrorString.InvalidPortNo;
                            }
                        }
                        else
                        {
                            if (this.LocalPortNo <= 0)
                            {
                                result = Structure.DriverError.InvalidConfiguration;

                                errorText = Resources.ErrorString.InvalidPortNo;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = Structure.DriverError.Unknown;

                    errorText = string.Format("{0}{1}{2}", Resources.ErrorString.ExceptionOccurrence, Environment.NewLine, ex.Message);
                }

                return result;
            }

            /// <summary>
            /// HSMS의 단순 복사본을 만듭니다.
            /// </summary>
            /// <returns>HSMS의 단순 복사본입니다.</returns>
            public HSMS Copy()
            {
                return new HSMS()
                {
                    HSMSMode = this.HSMSMode,
                    LinkTest = this.LinkTest,
                    LocalIPAddress = this.LocalIPAddress,
                    LocalPortNo = this.LocalPortNo,
                    RemoteIPAddress = this.RemoteIPAddress,
                    RemotePortNo = this.RemotePortNo,
                    T3 = this.T3,
                    T5 = this.T5,
                    T6 = this.T6,
                    T7 = this.T7,
                    T8 = this.T8
                };
            }
        }
        #endregion

        private const string DEFAULT_LOG_PATH = @"c:\Log";

        /// <summary>
        /// Driver Name을 가져오거나 설정합니다.
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// Device Type을 가져오거나 설정합니다.(Default Value = Equipment)
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// Message 송/수신 처리 방법을 가져오거나 설정합니다.(Default Value = true)
        /// </summary>
        public bool IsAsyncMode { get; set; }

        /// <summary>
        /// Device ID를 가져오거나 설정합니다.(Default Value = 0)
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// SECS Mode를 가져오거나 설정합니다.(Default Value = HSMS)
        /// </summary>
        public SECSMode SECSMode { get; set; }

        /// <summary>
        /// Message의 최대 Size를 가져오거나 설정합니다.(Default Value = 2MB)
        /// </summary>
        public double MaxMessageSize { get; set; }

        /// <summary>
        /// UMD File Name(Path 포함)을 가져오거나 설정합니다.
        /// </summary>
        public string UMDFileName { get; set; }

        /// <summary>
        /// Log Path를 가져오거나 설정합니다.(Default Value = C:/Log)
        /// </summary>
        public string LogPath { get; set; }

        /// <summary>
        /// SECS-I Log 저장 방법을 가져오거나 설정합니다.(Default Value = Hour)
        /// </summary>
        public LogMode LogEnabledSECS1 { get; set; }

        /// <summary>
        /// SECS-II Log 저장 방법을 가져오거나 설정합니다.(Default Value = Hour)
        /// </summary>
        public LogMode LogEnabledSECS2 { get; set; }

        /// <summary>
        /// Driver Log 저장 방법을 가져오거나 설정합니다.(Default Value = None)
        /// </summary>
        public LogMode LogEnabledSystem { get; set; }

        /// <summary>
        /// Log 보관 기간을 가져오거나 설정합니다.(단위 = 일, Default Value = 30)
        /// </summary>
        public int LogExpirationDay { get; set; }

        /// <summary>
        /// SECS1 Mode의 설정을 가져오거나 설정합니다.
        /// </summary>
        public SECS1 SECS1ModeConfig { get; set; }

        /// <summary>
        /// HSMS Mode의 설정을 가져오거나 설정합니다.
        /// </summary>
        public HSMS HSMSModeConfig { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public Configurtion()
        {
            this.DriverName = string.Empty;
            this.DeviceType = DeviceType.Equipment;
            this.IsAsyncMode = true;
            this.DeviceID = 0;
            this.SECSMode = SECSMode.HSMS;
            this.MaxMessageSize = 2 * 1024 * 1024;
            this.UMDFileName = string.Empty;
            this.LogPath = DEFAULT_LOG_PATH;
            this.LogEnabledSECS1 = LogMode.Hour;
            this.LogEnabledSECS2 = LogMode.Hour;
            this.LogEnabledSystem = LogMode.None;
            this.LogExpirationDay = 30;

            this.SECS1ModeConfig = new SECS1();
            this.HSMSModeConfig = new HSMS();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Driver Name={0}, SECS Mode={1}, Device Type={2}, Async Mode={3}, Device ID={4}, UMD File={5}",
                                 this.DriverName, this.SECSMode, this.DeviceType, this.IsAsyncMode, this.DeviceID, this.UMDFileName);
        }

        /// <summary>
        /// 환경 설정을 검증합니다.
        /// </summary>
        /// <param name="errorText">검증 실패 사유입니다.(검증 성공 시 string.Empty)</param>
        /// <returns>검증 결과입니다.</returns>
        public Structure.DriverError ValidateConfiguration(out string errorText)
        {
            Structure.DriverError result = Structure.DriverError.Ok;

            errorText = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(this.DriverName) == true)
                {
                    result = Structure.DriverError.NotExistDriverName;

                    errorText = Resources.ErrorString.DoseNotExistDriverName;
                }
                else
                {
                    if (this.SECSMode == Structure.SECSMode.HSMS)
                    {
                        if (this.HSMSModeConfig == null)
                        {
                            result = Structure.DriverError.InvalidConfiguration;

                            errorText = Resources.ErrorString.ConfigurationIsNull;
                        }
                        else
                        {
                            result = this.HSMSModeConfig.ValidateConfiguration(out errorText);
                        }
                    }
                    else
                    {
                        //\\// SECS-I 추가
                    }
                }
            }
            catch (Exception ex)
            {
                result = Structure.DriverError.Unknown;

                errorText = string.Format("{0}{1}{2}", Resources.ErrorString.ExceptionOccurrence, Environment.NewLine, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Configurtion의 단순 복사본을 만듭니다.
        /// </summary>
        /// <returns>Configurtion의 단순 복사본입니다.</returns>
        public Configurtion Copy()
        {
            Configurtion result = new Configurtion()
            {
                DriverName = this.DriverName,
                DeviceID = this.DeviceID,
                DeviceType = this.DeviceType,
                IsAsyncMode = this.IsAsyncMode,
                LogEnabledSECS1 = this.LogEnabledSECS1,
                LogEnabledSECS2 = this.LogEnabledSECS2,
                LogEnabledSystem = this.LogEnabledSystem,
                LogExpirationDay = this.LogExpirationDay,
                LogPath = this.LogPath,
                MaxMessageSize = this.MaxMessageSize,
                SECSMode = this.SECSMode,
                UMDFileName = this.UMDFileName,
                HSMSModeConfig = this.HSMSModeConfig.Copy()
            };

            return result;
        }
    }

    /// <summary>
    /// Driver 환경 설정 Collection Class입니다.
    /// </summary>
    [Serializable]
    public class ConfigurtionCollection
    {
        private const string TAG_ROOT = "Configuration";

        #region [DriverSchema - Class]
        private static class DriverSchema
        {
            public const string TAG_ROOT = "Driver";

            public const string DRIVER_NAME = "Name";
            public const string DEVICE_TYPE = "Type";
            public const string IS_ASYNC_MODE = "IsAsyncMode";
            public const string DEVICE_ID = "DeviceID";
            public const string SECS_MODE = "Mode";
            public const string MAX_MESSAGE_SIZE = "MaxMessageSize";
            public const string UMD_FILE_NAME = "UMDFileName";
        }
        #endregion

        #region [SECS1ModeSchema - Class]
        private static class SECS1ModeSchema
        {
        }
        #endregion

        #region [HSMSModeSchema - Class]
        private static class HSMSModeSchema
        {
            public const string TAG_ROOT = "HSMS";

            public const string HSMS_MODE = "HSMSMode";
            public const string REMOTE_IP_ADDRESS = "RemoteIPAddress";
            public const string REMOTE_PORT_NO = "RemotePortNo";
            public const string LOCAL_IP_ADDRESS = "LocalIPAddress";
            public const string LOCAL_PORT_NO = "LocalPortNo";

            public static class TimeoutSchema
            {
                public const string TAG_ROOT = "Timeout";

                public const string T3 = "T3";
                public const string T5 = "T5";
                public const string T6 = "T6";
                public const string T7 = "T7";
                public const string T8 = "T8";
                public const string LINK_TEST = "LinkTest";
            }
        }
        #endregion

        #region [LogSchema - Class]
        private static class LogSchema
        {
            public const string LOG_PATH = "LogPath";
            public const string LOG_ENABLED_SECS1 = "LogEnabledSECS1";
            public const string LOG_ENABLED_SECS2 = "LogEnabledSECS2";
            public const string LOG_ENABLED_SYSTEM = "LogEnabledSystem";
            public const string LOG_EXPIRATION_DAY = "LogExpirationDay";
        }
        #endregion

        private Dictionary<string, Configurtion> _configurtion;

        /// <summary>
        /// 설정된 driver수를 가져옵니다.
        /// </summary>
        public int Count
        {
            get { return this._configurtion.Count; }
        }

        /// <summary>
        /// Driver 정보를 가져옵니다.(Key=Driver Name)
        /// </summary>
        public KeyValuePair<string, Configurtion>[] AsDictionary
        {
            get { return this._configurtion.ToArray(); }
        }

        /// <summary>
        /// Driver 정보를 가져옵니다.
        /// </summary>
        public Configurtion[] AsList
        {
            get { return this._configurtion.Values.ToArray(); }
        }

        /// <summary>
        /// Driver 정보를 가져옵니다.
        /// </summary>
        /// <param name="driverName">Driver name입니다.</param>
        /// <returns></returns>
        public Configurtion this[string driverName]
        {
            get { return this._configurtion.ContainsKey(driverName) == true ? this._configurtion[driverName] : null; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ConfigurtionCollection()
        {
            this._configurtion = new Dictionary<string, Configurtion>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            if (this._configurtion != null)
            {
                return "Driver Count=" + this._configurtion.Count.ToString();
            }
            else
            {
                return "Driver Information is null";
            }
        }

        /// <summary>
        /// Configuration File을 Load합니다.
        /// </summary>
        /// <param name="fileName">Load할 Configuration Full File Name입니다.</param>
        /// <param name="errorText">Configuration File Load 실패 사유입니다.(Load 성공 시 string.Empty)</param>
        /// <returns>Configuration File Load 결과입니다.</returns>
        public Structure.DriverError Load(string fileName, out string errorText)
        {
            Structure.DriverError result = Structure.DriverError.Ok;
            XElement element;
            Configurtion configurtion;

            errorText = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(fileName) == true || System.IO.File.Exists(fileName) == false)
                {
                    result = Structure.DriverError.NotExistFile;

                    errorText = Resources.ErrorString.DoseNotExistConfigurationFile;
                }
                else
                {
                    this._configurtion = new Dictionary<string, Configurtion>();

                    element = XElement.Load(fileName);

                    var var = element.Elements(DriverSchema.TAG_ROOT);

                    foreach (XElement temp in var)
                    {
                        configurtion = DeserializeDriverInfo(temp);

                        if (configurtion == null)
                        {
                            result = DriverError.FileLoadFailed;
                            errorText = Resources.ErrorString.FailedConfigurationFileLoad;

                            break;
                        }
                        else
                        {
                            this._configurtion[configurtion.DriverName] = configurtion;
                        }
                    }

                    element = null;
                    configurtion = null;
                }
            }
            catch (Exception ex)
            {
                this._configurtion = null;
                result = Structure.DriverError.Unknown;

                errorText = string.Format("{0}{1}{2}", Resources.ErrorString.ExceptionOccurrence, Environment.NewLine, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Configuration File을 Save합니다.
        /// </summary>
        /// <param name="fileName">Save할 Configuration Full File Name입니다.</param>
        /// <param name="errorText">Configuration File Save 실패 사유입니다.(Save 성공 시 string.Empty)</param>
        /// <returns>Configuration File Save 결과입니다.</returns>
        public Structure.DriverError Save(string fileName, out string errorText)
        {
            Structure.DriverError result = Structure.DriverError.Ok;
            XElement configuration;

            errorText = string.Empty;

            try
            {
                configuration = new XElement(TAG_ROOT);

                configuration = SerializeDriverInfo();

                if (configuration != null)
                {
                    configuration.Save(fileName);
                }
                else
                {
                    result = Structure.DriverError.FileSaveFailed;

                    errorText = Resources.ErrorString.FailedConfigurationFileSave;
                }

                configuration = null;
            }
            catch (Exception ex)
            {
                result = Structure.DriverError.Unknown;

                errorText = string.Format("{0}{1}{2}", Resources.ErrorString.ExceptionOccurrence, Environment.NewLine, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Driver Name이 등록되어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="driverName">설정에서 찾을 수 있는 Driver Name입니다.</param>
        /// <returns>지정한 Diver Name이 있는 요소가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Contains(string driverName)
        {
            return this._configurtion.ContainsKey(driverName);
        }

        /// <summary>
        /// Driver Configuration을 추가합니다.
        /// </summary>
        /// <param name="configurtion">추가 할 Driver Configuration입니다.</param>
        public void Add(Configurtion configurtion)
        {
            this._configurtion[configurtion.DriverName] = configurtion;
        }

        /// <summary>
        /// Driver Configuration을 삭제합니다.
        /// </summary>
        /// <param name="driverName">제거 할 Driver Name입니다.</param>
        /// <returns>성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다.이 메서드는 key가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(string driverName)
        {
            return this._configurtion.Remove(driverName);
        }

        private XElement SerializeDriverInfo()
        {
            XElement result;
            XElement driver;
            XElement hsms;
            XElement timeout;

            try
            {
                result = new XElement(TAG_ROOT);

                foreach (KeyValuePair<string, Configurtion> tempConfig in this._configurtion)
                {
                    driver = new XElement(DriverSchema.TAG_ROOT,
                                          new XElement(DriverSchema.DRIVER_NAME, tempConfig.Value.DriverName),
                                          new XElement(DriverSchema.DEVICE_TYPE, tempConfig.Value.DeviceType),
                                          new XElement(DriverSchema.IS_ASYNC_MODE, tempConfig.Value.IsAsyncMode),
                                          new XElement(DriverSchema.DEVICE_ID, tempConfig.Value.DeviceID),
                                          new XElement(DriverSchema.SECS_MODE, tempConfig.Value.SECSMode),
                                          new XElement(DriverSchema.MAX_MESSAGE_SIZE, tempConfig.Value.MaxMessageSize),
                                          new XElement(DriverSchema.UMD_FILE_NAME, tempConfig.Value.UMDFileName),
                                          new XElement(LogSchema.LOG_ENABLED_SECS1, tempConfig.Value.LogEnabledSECS1),
                                          new XElement(LogSchema.LOG_ENABLED_SECS2, tempConfig.Value.LogEnabledSECS2),
                                          new XElement(LogSchema.LOG_ENABLED_SYSTEM, tempConfig.Value.LogEnabledSystem),
                                          new XElement(LogSchema.LOG_EXPIRATION_DAY, tempConfig.Value.LogExpirationDay),
                                          new XElement(LogSchema.LOG_PATH, tempConfig.Value.LogPath));

                    hsms = new XElement(HSMSModeSchema.TAG_ROOT,
                                        new XElement(HSMSModeSchema.HSMS_MODE, tempConfig.Value.HSMSModeConfig.HSMSMode),
                                        new XElement(HSMSModeSchema.REMOTE_IP_ADDRESS, tempConfig.Value.HSMSModeConfig.RemoteIPAddress),
                                        new XElement(HSMSModeSchema.REMOTE_PORT_NO, tempConfig.Value.HSMSModeConfig.RemotePortNo),
                                        new XElement(HSMSModeSchema.LOCAL_IP_ADDRESS, tempConfig.Value.HSMSModeConfig.LocalIPAddress),
                                        new XElement(HSMSModeSchema.LOCAL_PORT_NO, tempConfig.Value.HSMSModeConfig.LocalPortNo));

                    timeout = new XElement(HSMSModeSchema.TimeoutSchema.TAG_ROOT,
                                           new XElement(HSMSModeSchema.TimeoutSchema.T3, tempConfig.Value.HSMSModeConfig.T3),
                                           new XElement(HSMSModeSchema.TimeoutSchema.T5, tempConfig.Value.HSMSModeConfig.T5),
                                           new XElement(HSMSModeSchema.TimeoutSchema.T6, tempConfig.Value.HSMSModeConfig.T6),
                                           new XElement(HSMSModeSchema.TimeoutSchema.T7, tempConfig.Value.HSMSModeConfig.T7),
                                           new XElement(HSMSModeSchema.TimeoutSchema.T8, tempConfig.Value.HSMSModeConfig.T8),
                                           new XElement(HSMSModeSchema.TimeoutSchema.LINK_TEST, tempConfig.Value.HSMSModeConfig.LinkTest));

                    hsms.Add(timeout);
                    driver.Add(hsms);

                    result.Add(driver);
                }

                driver = null;
                hsms = null;
                timeout = null;
            }
            catch
            {
                result = null;
            }

            return result;
        }

        private static Configurtion DeserializeDriverInfo(XElement element)
        {
            Configurtion result;
            XElement hsms;
            XElement timeout;

            try
            {
                result = new Configurtion();

                if (element.Element(DriverSchema.DRIVER_NAME) != null)
                {
                    result.DriverName = element.Element(DriverSchema.DRIVER_NAME).Value;

                    if (element.Element(DriverSchema.DEVICE_TYPE) != null)
                    {
                        result.DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), element.Element(DriverSchema.DEVICE_TYPE).Value);

                        if (element.Element(DriverSchema.IS_ASYNC_MODE) != null)
                            result.IsAsyncMode = bool.Parse(element.Element(DriverSchema.IS_ASYNC_MODE).Value);

                        if (element.Element(DriverSchema.DEVICE_ID) != null)
                            result.DeviceID = int.Parse(element.Element(DriverSchema.DEVICE_ID).Value);

                        if (element.Element(DriverSchema.SECS_MODE) != null)
                            result.SECSMode = (SECSMode)Enum.Parse(typeof(SECSMode), element.Element(DriverSchema.SECS_MODE).Value);

                        if (element.Element(DriverSchema.MAX_MESSAGE_SIZE) != null)
                            result.MaxMessageSize = double.Parse(element.Element(DriverSchema.MAX_MESSAGE_SIZE).Value);

                        if (element.Element(DriverSchema.UMD_FILE_NAME) != null)
                            result.UMDFileName = element.Element(DriverSchema.UMD_FILE_NAME).Value;

                        hsms = element.Element(HSMSModeSchema.TAG_ROOT);

                        if (hsms != null)
                        {
                            if (hsms.Element(HSMSModeSchema.HSMS_MODE) != null)
                                result.HSMSModeConfig.HSMSMode = (HSMSMode)Enum.Parse(typeof(HSMSMode), hsms.Element(HSMSModeSchema.HSMS_MODE).Value);

                            if (hsms.Element(HSMSModeSchema.REMOTE_IP_ADDRESS) != null)
                                result.HSMSModeConfig.RemoteIPAddress = hsms.Element(HSMSModeSchema.REMOTE_IP_ADDRESS).Value;

                            if (hsms.Element(HSMSModeSchema.REMOTE_PORT_NO) != null)
                                result.HSMSModeConfig.RemotePortNo = int.Parse(hsms.Element(HSMSModeSchema.REMOTE_PORT_NO).Value);

                            if (hsms.Element(HSMSModeSchema.LOCAL_IP_ADDRESS) != null)
                                result.HSMSModeConfig.LocalIPAddress = hsms.Element(HSMSModeSchema.LOCAL_IP_ADDRESS).Value;

                            if (hsms.Element(HSMSModeSchema.LOCAL_PORT_NO) != null)
                                result.HSMSModeConfig.LocalPortNo = int.Parse(hsms.Element(HSMSModeSchema.LOCAL_PORT_NO).Value);

                            timeout = hsms.Element(HSMSModeSchema.TimeoutSchema.TAG_ROOT);

                            if (timeout != null)
                            {
                                if (timeout.Element(HSMSModeSchema.TimeoutSchema.T3) != null)
                                    result.HSMSModeConfig.T3 = int.Parse(timeout.Element(HSMSModeSchema.TimeoutSchema.T3).Value);

                                if (timeout.Element(HSMSModeSchema.TimeoutSchema.T5) != null)
                                    result.HSMSModeConfig.T5 = int.Parse(timeout.Element(HSMSModeSchema.TimeoutSchema.T5).Value);

                                if (timeout.Element(HSMSModeSchema.TimeoutSchema.T6) != null)
                                    result.HSMSModeConfig.T6 = int.Parse(timeout.Element(HSMSModeSchema.TimeoutSchema.T6).Value);

                                if (timeout.Element(HSMSModeSchema.TimeoutSchema.T7) != null)
                                    result.HSMSModeConfig.T7 = int.Parse(timeout.Element(HSMSModeSchema.TimeoutSchema.T7).Value);

                                if (timeout.Element(HSMSModeSchema.TimeoutSchema.T8) != null)
                                    result.HSMSModeConfig.T8 = int.Parse(timeout.Element(HSMSModeSchema.TimeoutSchema.T8).Value);

                                if (timeout.Element(HSMSModeSchema.TimeoutSchema.LINK_TEST) != null)
                                    result.HSMSModeConfig.LinkTest = int.Parse(timeout.Element(HSMSModeSchema.TimeoutSchema.LINK_TEST).Value);
                            }
                            else
                            {
                                result = null;
                            }
                        }
                        else
                        {
                            result = null;
                        }

                        if (result != null)
                        {
                            if (element.Element(LogSchema.LOG_ENABLED_SECS1) != null)
                                result.LogEnabledSECS1 = (LogMode)Enum.Parse(typeof(LogMode), element.Element(LogSchema.LOG_ENABLED_SECS1).Value);

                            if (element.Element(LogSchema.LOG_ENABLED_SECS2) != null)
                                result.LogEnabledSECS2 = (LogMode)Enum.Parse(typeof(LogMode), element.Element(LogSchema.LOG_ENABLED_SECS2).Value);

                            if (element.Element(LogSchema.LOG_ENABLED_SYSTEM) != null)
                                result.LogEnabledSystem = (LogMode)Enum.Parse(typeof(LogMode), element.Element(LogSchema.LOG_ENABLED_SYSTEM).Value);

                            if (element.Element(LogSchema.LOG_EXPIRATION_DAY) != null)
                                result.LogExpirationDay = int.Parse(element.Element(LogSchema.LOG_EXPIRATION_DAY).Value);

                            if (element.Element(LogSchema.LOG_ENABLED_SECS1) != null)
                                result.LogPath = element.Element(LogSchema.LOG_PATH).Value;
                        }
                    }
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }
    }
}