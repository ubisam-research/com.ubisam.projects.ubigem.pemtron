using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info
{
    public class ReferenceStartingInfo
    {
        #region Proeprty
        public long X { get; set; }
        public long Y { get; set; }
        public long Direction { get; set; }
        public string BinList { get; set; }
        #endregion
        #region Contructor
        public ReferenceStartingInfo()
        {
            this.X = (long)0;
            this.Y = (long)0;
            this.Direction = 1;
            this.BinList = string.Empty;
        }
        #endregion
        #region DataString
        public string DataString()
        {
            string result;

            result = string.Format("{0} {1} {2}", this.X, this.Y, this.Direction);

            return result;
        }
        public string XYString()
        {
            string result;

            result = string.Format("{0} {1}", this.X, this.Y);

            return result;
        }
        #endregion
        #region Clone
        public ReferenceStartingInfo Clone()
        {
            ReferenceStartingInfo result;

            result = new ReferenceStartingInfo()
            {
                X = this.X,
                Y = this.Y,
                Direction = this.Direction,
                BinList = this.BinList
            };

            return result;
        }
        #endregion
        #region ToString
        public override string ToString()
        {
            string result;

            result = string.Format("[X={0},Y={1},Direction={2}]", this.X, this.Y, this.Direction);

            return result;
        }
        #endregion
    }
    public class MapDataType1
    {
        #region Property
        public string MaterialID { get; set; }
        public byte IDType { get; set; }
        public List<ReferenceStartingInfo> ReferenceStartingList { get; private set; }
        
        #endregion
        #region Constructor
        public MapDataType1()
        {
            this.MaterialID = string.Empty;
            this.IDType = 0;
            this.ReferenceStartingList = new List<ReferenceStartingInfo>();
        }
        #endregion
        #region Clone
        public MapDataType1 Clone()
        {
            MapDataType1 result;

            result = new MapDataType1()
            {
                MaterialID = this.MaterialID,
                IDType = this.IDType,
            };

            foreach (var item in this.ReferenceStartingList)
            {
                result.ReferenceStartingList.Add(item.Clone());
            }

            return result;
        }
        #endregion
    }
}
