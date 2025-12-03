using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UbiGEM.Net.Utility.Logger;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// GEM Driver의 확장 옵션 정보입니다.
    /// </summary>
    public class GEMExtensionOption
    {
        /// <summary>
        /// 시간 동기화 사용여부를 가져오거나 설정합니다.(true=GEM Driver에서 시간동기화 후 Event 발생함, false=Event 발생만 함)
        /// </summary>
        public bool UseAutoTimeSync { get; set; }

        /// <summary>
        /// Formatted PP 보고 시 PPName/PPValue pair 사용 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool UseFormattedPPValue { get; set; }

        /// <summary>
        /// Control State가 EQP Offline인 상태에서 S1F17 허용 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool UseS1F17InEQPOffline { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public GEMExtensionOption()
        {
            this.UseAutoTimeSync = false;
            this.UseS1F17InEQPOffline = false;
        }
    }

    /// <summary>
    /// GEM Driver의 옵션 정보입니다.
    /// </summary>
    public class GEMConfiguration
    {
        private const string DEFAULT_LOG_PATH = @"c:\Log";

        /// <summary>
        /// Log Path를 가져오거나 설정합니다.(Default Value = C:/Log)
        /// </summary>
        public string LogPath { get; set; }

        /// <summary>
        /// GEM Log 저장 방법을 가져오거나 설정합니다.(Default Value = Hour)
        /// </summary>
        public LogMode LogEnabledGEM { get; set; }

        /// <summary>
        /// Log 보관 기간을 가져오거나 설정합니다.(단위 = 일, Default Value = 30)
        /// </summary>
        public int LogExpirationDay { get; set; }

        /// <summary>
        /// GEM 확장 옵션을 가져오거나 설정합니다.
        /// </summary>
        public GEMExtensionOption ExtensionOption { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public GEMConfiguration()
        {
            this.LogPath = DEFAULT_LOG_PATH;
            this.LogEnabledGEM = LogMode.Hour;
            this.LogExpirationDay = 30;
            this.ExtensionOption = new GEMExtensionOption();
        }

        /// <summary>
        /// GEMConfiguration의 단순 복사본을 만듭니다.
        /// </summary>
        /// <returns>GEMConfiguration의 단순 복사본입니다.</returns>
        public GEMConfiguration Copy()
        {
            GEMConfiguration result = new GEMConfiguration()
            {
                LogPath = this.LogPath,
                LogEnabledGEM = this.LogEnabledGEM,
                LogExpirationDay = this.LogExpirationDay
            };

            return result;
        }
    }
}