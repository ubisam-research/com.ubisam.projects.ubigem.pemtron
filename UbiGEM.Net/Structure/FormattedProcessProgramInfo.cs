using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Formatted Process Program Item입니다.
    /// </summary>
    public class FmtPPItem
    {
        /// <summary>
        /// PP Name을 가져오거나 설정합니다.
        /// </summary>
        public string PPName { get; set; }

        /// <summary>
        /// PP Value를 가져오거나 설정합니다.
        /// </summary>
        public string PPValue { get; set; }

        /// <summary>
        /// PP Value의 Format을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSItemFormat Format { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public FmtPPItem()
        {
            this.PPName = string.Empty;
            this.PPValue = string.Empty;
            this.Format = UbiCom.Net.Structure.SECSItemFormat.A;
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="ppName"></param>
        /// <param name="ppValue"></param>
        /// <param name="format"></param>
        public FmtPPItem(string ppName, string ppValue, UbiCom.Net.Structure.SECSItemFormat format)
        {
            this.PPName = ppName;
            this.PPValue = ppValue;
            this.Format = format;
        }

        /// <summary>
        /// 현재 개체를 나타내는 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 개체를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("PP Name={0}, PP Value={1}, Format={2}", this.PPName, this.PPValue, this.Format);
        }
    }

    /// <summary>
    /// Formatted Process Program의 Command Code 정보입니다.
    /// </summary>
    public class FmtPPCCodeInfo
    {
        /// <summary>
        /// Command Code를 가져오거나 설정합니다.
        /// </summary>
        public string CommandCode { get; set; }

        /// <summary>
        /// Formatted Process Program Item을 가져오거나 설정합니다.
        /// </summary>
        public List<FmtPPItem> Items { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public FmtPPCCodeInfo()
        {
            this.CommandCode = string.Empty;
            this.Items = new List<FmtPPItem>();
        }

        /// <summary>
        /// 현재 개체를 나타내는 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 개체를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Command Code={0}, Item Count={1}", this.CommandCode, this.Items.Count);
        }

        /// <summary>
        /// PPARAM을 추가합니다.
        /// </summary>
        /// <param name="pparam">추가할 PPARAM입니다.</param>
        /// <param name="format">추가할 PPValue의 format입니다.</param>
        public void Add(string pparam, UbiCom.Net.Structure.SECSItemFormat format)
        {
            this.Items.Add(new FmtPPItem()
            {
                PPName = pparam,
                PPValue = pparam,
                Format = format
            });
        }

        /// <summary>
        /// PPARAM을 추가합니다.
        /// </summary>
        /// <param name="ppName">추가할 PPName입니다.</param>
        /// <param name="ppValue">추가할 PPValue입니다.</param>
        /// <param name="format">추가할 PPValue의 format입니다.</param>
        public void Add(string ppName, string ppValue, UbiCom.Net.Structure.SECSItemFormat format)
        {
            this.Items.Add(new FmtPPItem()
            {
                PPName = ppName,
                PPValue = ppValue,
                Format = format
            });
        }
    }

    /// <summary>
    /// Formatted Process Program 정보입니다.
    /// </summary>
    public class FmtPPCollection
    {
        /// <summary>
        /// Process Program ID를 가져오거나 설정합니다.
        /// </summary>
        public string PPID { get; set; }

        /// <summary>
        /// MDLN을 가져오거나 설정합니다.
        /// </summary>
        public string MDLN { get; set; }

        /// <summary>
        /// SOFTREV를 가져오거나 설정합니다.
        /// </summary>
        public string SOFTREV { get; set; }

        /// <summary>
        /// Formatted Process Program Info를 가져오거나 설정합니다.
        /// </summary>
        public List<FmtPPCCodeInfo> Items { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="ppid">PPID입니다.</param>
        public FmtPPCollection(string ppid)
        {
            this.PPID = ppid;
            this.MDLN = string.Empty;
            this.SOFTREV = string.Empty;
            this.Items = new List<FmtPPCCodeInfo>();
        }

        /// <summary>
        /// 현재 개체를 나타내는 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 개체를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("PPID={0}, Item Count={1}", this.PPID, this.Items.Count);
        }

        /// <summary>
        /// Formatted Process Program의 Command Code를 추가합니다.
        /// </summary>
        /// <param name="fmtPPCCodeInfo">추가 할 Command Code 정보입니다.</param>
        public void AddCode(FmtPPCCodeInfo fmtPPCCodeInfo)
        {
            this.Items.Add(fmtPPCCodeInfo);
        }
    }

    /// <summary>
    /// Formatted Process Program Verification Item입니다.
    /// </summary>
    public class FmtPPVerificationInfo
    {
        /// <summary>
        /// ACK를 가져오거나 설정합니다.
        /// </summary>
        public int ACK { get; set; }

        /// <summary>
        /// Command Number를 가져오거나 설정합니다.
        /// </summary>
        public int SeqNum { get; set; }

        /// <summary>
        /// 오류 내용을 가져오거나 설정합니다.
        /// </summary>
        public string ErrW7 { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public FmtPPVerificationInfo()
        {
        }

        /// <summary>
        /// 현재 개체를 나타내는 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 개체를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("ACK={0}, SEQ No={1}, Error={2}", this.ACK, this.SeqNum, this.ErrW7);
        }
    }

    /// <summary>
    /// Formatted Process Program Verification 정보입니다.
    /// </summary>
    public class FmtPPVerificationCollection
    {
        /// <summary>
        /// Process Program ID를 가져오거나 설정합니다.
        /// </summary>
        public string PPID { get; set; }

        /// <summary>
        /// Formatted Process Program Verification Item을 가져오거나 설정합니다.
        /// </summary>
        public List<FmtPPVerificationInfo> Items { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="ppid">PPID입니다.</param>
        public FmtPPVerificationCollection(string ppid)
        {
            this.PPID = ppid;
            this.Items = new List<FmtPPVerificationInfo>();
        }

        /// <summary>
        /// 현재 개체를 나타내는 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 개체를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("PPID={0}, Item Count={1}", this.PPID, this.Items.Count);
        }

        /// <summary>
        /// Verification을 추가합니다.
        /// </summary>
        /// <param name="ack">추가할 ACK입니다.</param>
        /// <param name="seqNum">추가할 Command Number입니다.</param>
        /// <param name="errW7">추가할 오류 내용입니다.</param>
        public void Add(int ack, int seqNum, string errW7)
        {
            this.Items.Add(new FmtPPVerificationInfo()
            {
                ACK = ack,
                SeqNum = seqNum,
                ErrW7 = errW7
            });
        }
    }
}