using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info
{
    public class XYPosInfo
    {
        #region Proeprty
        public long X { get; set; }
        public long Y { get; set; }
        public string BinList { get; set; }
        #endregion
        #region Contructor
        public XYPosInfo()
        {
            this.X = (long)0;
            this.Y = (long)0;
            this.BinList = string.Empty;
        }
        #endregion
        #region DataString
        public string DataString()
        {
            string result;

            result = string.Format("{0} {1}", this.X, this.Y);

            return result;
        }
        #endregion
        #region Clone
        public XYPosInfo Clone()
        {
            XYPosInfo result;

            result = new XYPosInfo()
            {
                X = this.X,
                Y = this.Y,
                BinList = this.BinList,
            };

            return result;
        }
        #endregion
        #region ToString
        public override string ToString()
        {
            string result;

            result = string.Format("[X={0},Y={1}]", this.X, this.Y);

            return result;
        }
        #endregion
    }
    public class MapDataType3
    {
        #region Property
        public string MaterialID { get; set; }
        public byte IDType { get; set; }
        public List<XYPosInfo> XYPOSList { get; set; }

        #endregion
        #region Constructor
        public MapDataType3()
        {
            this.MaterialID = string.Empty;
            this.IDType = 0;
            this.XYPOSList = new List<XYPosInfo>();
        }
        #endregion
        #region Clone
        public MapDataType3 Clone()
        {
            MapDataType3 result;

            result = new MapDataType3()
            {
                MaterialID = this.MaterialID,
                IDType = this.IDType,
            };

            foreach (var item in this.XYPOSList)
            {
                result.XYPOSList.Add(item.Clone());
            }

            return result;
        }
        #endregion
    }
}
