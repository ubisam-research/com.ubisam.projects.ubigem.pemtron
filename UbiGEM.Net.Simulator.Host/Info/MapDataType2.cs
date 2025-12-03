using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info
{
    public class MapDataType2
    {
        #region Property
        public string MaterialID { get; set; }
        public byte IDType { get; set; }
        public long StartPointX { get; set; }
        public long StartPointY { get; set; }
        public string BinList { get; set; }
        
        #endregion
        #region Constructor
        public MapDataType2()
        {
            this.MaterialID = string.Empty;
            this.IDType = 0;
            this.StartPointX = 0;
            this.StartPointY = 0;
            this.BinList = string.Empty;
        }
        #endregion
        #region Clone
        public MapDataType2 Clone()
        {
            MapDataType2 result;

            result = new MapDataType2()
            {
                MaterialID = this.MaterialID,
                IDType = this.IDType,
                StartPointX = this.StartPointX,
                StartPointY = this.StartPointY,
                BinList = this.BinList,
            };

            return result;
        }
        #endregion
        #region StrpString
        public string StrpString()
        {
            string result;

            result = string.Format("{0} {1}", this.StartPointX, this.StartPointY);

            return result;
        }
        #endregion
    }
}
