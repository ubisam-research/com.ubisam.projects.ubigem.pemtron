using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info
{
    public class ReferencePointItem
    {
        #region Proeprty
        public long X { get; set; }
        public long Y { get; set; }
        #endregion
        #region Contructor
        public ReferencePointItem()
        {
            this.X = (long)0;
            this.Y = (long)0;
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
        public ReferencePointItem Clone()
        {
            ReferencePointItem result;

            result = new ReferencePointItem()
            {
                X = this.X,
                Y = this.Y
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
    public class MapSetupData
    {
        #region Property
        public string MaterialID { get; set; }
        public byte IDType { get; set; }
        public string FlatNotchLocation { get; set; }
        public string FilmFrameRotation { get; set; }
        public string OriginLocation { get; set; }
        public byte ReferencePointSelect { get; set; }
        public List<ReferencePointItem> ReferencePoint { get; private set; }
        public string DieUnitsOfMeasure { get; set; }
        public string XAxisDieSize { get; set; }
        public string YAxisDieSize { get; set; }
        public ulong RowCount { get; set; }
        public ulong ColumnCount { get; set; }
        public string NullBinCodeValue { get; set; }
        public string BinCodeEquivalent { get; set; }
        public string ProcessDieCount { get; set; }
        public byte ProcessAxis { get; set; }
        public ulong MessageLength { get; set; }

        #endregion
        #region Constructor
        public MapSetupData()
        {
            this.MaterialID = string.Empty;
            this.IDType = 0;
            this.FlatNotchLocation = string.Empty;
            this.FilmFrameRotation = string.Empty;
            this.OriginLocation = string.Empty;
            this.ReferencePointSelect = 0;
            this.ReferencePoint = new List<ReferencePointItem>();
            this.DieUnitsOfMeasure = string.Empty;
            this.XAxisDieSize = string.Empty;
            this.YAxisDieSize = string.Empty;
            this.RowCount = 0;
            this.ColumnCount = 0;
            this.NullBinCodeValue = string.Empty;
            this.BinCodeEquivalent = string.Empty;
            this.ProcessDieCount = string.Empty;
            this.ProcessAxis = 0;
            this.MessageLength = 0;
        }
        #endregion
        #region Clone
        public MapSetupData Clone()
        {
            MapSetupData result;

            result = new MapSetupData()
            {
                MaterialID = this.MaterialID,
                IDType = this.IDType,
                FlatNotchLocation = this.FlatNotchLocation,
                FilmFrameRotation = this.FilmFrameRotation,
                OriginLocation = this.OriginLocation,
                ReferencePointSelect = this.ReferencePointSelect,
                DieUnitsOfMeasure = this.DieUnitsOfMeasure,
                XAxisDieSize = this.XAxisDieSize,
                YAxisDieSize = this.YAxisDieSize,
                RowCount = this.RowCount,
                ColumnCount = this.ColumnCount,
                NullBinCodeValue = this.NullBinCodeValue,
                BinCodeEquivalent = this.BinCodeEquivalent,
                ProcessDieCount = this.ProcessDieCount,
                ProcessAxis = this.ProcessAxis,
                MessageLength = this.MessageLength,
            };

            foreach (var item in this.ReferencePoint)
            {
                result.ReferencePoint.Add(item.Clone());
            }

            return result;
        }
        #endregion
    }
}
