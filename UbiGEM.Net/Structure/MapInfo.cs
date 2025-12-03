using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Structure.WaferMap
{
    /// <summary>
    /// Reference point item입니다.
    /// </summary>
    public class ReferencePointItem
    {
        /// <summary>
        /// Reference Point(X).
        /// </summary>
        public long X { get; set; }

        /// <summary>
        /// Reference Point(Y).
        /// </summary>
        public long Y { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ReferencePointItem()
        {
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="x">Reference point x입니다.</param>
        /// <param name="y">Reference point y입니다.</param>
        public ReferencePointItem(long x, long y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("X={0}, Y={1}", this.X, this.Y);
        }
    }

    /// <summary>
    /// Map data item입니다.
    /// </summary>
    public class MapDataItem
    {
        /// <summary>
        /// The Bin List.
        /// </summary>
        public string BinList { get; set; }

        /// <summary>
        /// Start location(X).
        /// </summary>
        public long X { get; set; }

        /// <summary>
        /// Start location(Y).
        /// </summary>
        public long Y { get; set; }

        /// <summary>
        /// Start location(Direction).
        /// </summary>
        public long Direction { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public MapDataItem()
        {
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="binList">The Bin List.</param>
        /// <param name="x">Start location x입니다.</param>
        /// <param name="y">Start location y입니다.</param>
        /// <param name="direction">Start location direction입니다.</param>
        public MapDataItem(string binList, long x, long y, long direction)
        {
            this.BinList = binList;
            this.X = x;
            this.Y = y;
            this.Direction = direction;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Bin List={0}, X={1}, Y={2}, Direction={3}", this.BinList, this.X, this.Y, this.Direction);
        }
    }

    /// <summary>
    /// Start position item입니다.
    /// </summary>
    public class StartPositionItem
    {
        /// <summary>
        /// Start location(X).
        /// </summary>
        public long X { get; set; }

        /// <summary>
        /// Start location(Y).
        /// </summary>
        public long Y { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public StartPositionItem()
        {
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="x">Start location x입니다.</param>
        /// <param name="y">Start location y입니다.</param>
        public StartPositionItem(long x, long y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("X={0}, Y={1}", this.X, this.Y);
        }
    }

    /// <summary>
    /// Map set-up data입니다.
    /// </summary>
    public class MapSetupData
    {
        private readonly List<ReferencePointItem> _referencePoint;

        /// <summary>
        /// Material ID를 가져오거나 설정합니다.
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// ID type을 가져오거나 설정합니다.
        /// </summary>
        public int IDType { get; set; }

        /// <summary>
        /// Flat/Notch location.
        /// </summary>
        public ulong? FlatNotchLocation { get; set; }

        /// <summary>
        /// Film frame rotation.
        /// </summary>
        public ulong? FilmFrameRotation { get; set; }

        /// <summary>
        /// Origin location.
        /// </summary>
        public ulong? OriginLocation { get; set; }

        /// <summary>
        /// Reference point select.
        /// </summary>
        public ulong ReferencePointSelect { get; set; }

        /// <summary>
        /// Die units of measure.
        /// </summary>
        public string DieUnitsOfMeasure { get; set; }

        /// <summary>
        /// X-axis die size (index).
        /// </summary>
        public double XAxisDieSize { get; set; }

        /// <summary>
        /// Y-axis die size (index).
        /// </summary>
        public double YAxisDieSize { get; set; }

        /// <summary>
        /// Row count in die increments.
        /// </summary>
        public ulong RowCount { get; set; }

        /// <summary>
        /// Column count in die increments.
        /// </summary>
        public ulong ColumnCount { get; set; }

        /// <summary>
        /// Bin code equivalents.
        /// </summary>
        public string BinCodeEquivalents { get; set; }

        /// <summary>
        /// Null bin code value.
        /// </summary>
        public string NullBinCodeValue { get; set; }

        /// <summary>
        /// Process Die Count.
        /// </summary>
        public ulong? ProcessDieCount { get; set; }

        /// <summary>
        /// Process axis.
        /// </summary>
        public long ProcessAxis { get; set; }

        /// <summary>
        /// Message length.
        /// </summary>
        public long MessageLength { get; set; }

        /// <summary>
        /// Reference Point.
        /// </summary>
        public List<ReferencePointItem> ReferencePoint
        {
            get { return this._referencePoint; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public MapSetupData()
        {
            this._referencePoint = new List<ReferencePointItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Material ID={0}, ID Type={1}", this.MaterialId, this.IDType);
        }

        /// <summary>
        /// Reference point를 추가합니다.
        /// </summary>
        /// <param name="x">Reference point x입니다.</param>
        /// <param name="y">Reference point y입니다.</param>
        public void AddReferencePoint(long x, long y)
        {
            this.ReferencePoint.Add(new ReferencePointItem(x, y));
        }
    }

    /// <summary>
    ///  Map data type 1 입니다.
    /// </summary>
    public class MapDataType1
    {
        private readonly List<MapDataItem> _mapDataItem;

        /// <summary>
        /// Material ID를 가져오거나 설정합니다.
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// ID type을 가져오거나 설정합니다.
        /// </summary>
        public int IDType { get; set; }

        /// <summary>
        /// Map data.
        /// </summary>
        public List<MapDataItem> MapData
        {
            get { return this._mapDataItem; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public MapDataType1()
        {
            this._mapDataItem = new List<MapDataItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Material ID={0}, ID Type={1}", this.MaterialId, this.IDType);
        }

        /// <summary>
        /// Map data를 추가합니다.
        /// </summary>
        /// <param name="binList">The Bin List.</param>
        /// <param name="x">Start location x입니다.</param>
        /// <param name="y">Start location y입니다.</param>
        /// <param name="direction">Start location direction입니다.</param>
        public void AddMapData(string binList, long x, long y, long direction)
        {
            this.MapData.Add(new MapDataItem(binList, x, y, direction));
        }
    }

    /// <summary>
    ///  Map data type 2 입니다.
    /// </summary>
    public class MapDataType2
    {
        /// <summary>
        /// Material ID를 가져오거나 설정합니다.
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// ID type을 가져오거나 설정합니다.
        /// </summary>
        public int IDType { get; set; }

        /// <summary>
        /// Start position(X).
        /// </summary>
        public long StartPositionX { get; set; }

        /// <summary>
        /// Start position(Y).
        /// </summary>
        public long StartPositionY { get; set; }

        /// <summary>
        /// The Bin List.
        /// </summary>
        public string BinList { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public MapDataType2()
        {
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Material ID={0}, ID Type={1}", this.MaterialId, this.IDType);
        }
    }

    /// <summary>
    ///  Map data type 3 입니다.
    /// </summary>
    public class MapDataType3
    {
        private readonly List<MapDataItem> _mapDataItem;

        /// <summary>
        /// Material ID를 가져오거나 설정합니다.
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// ID type을 가져오거나 설정합니다.
        /// </summary>
        public int IDType { get; set; }

        /// <summary>
        /// Map data.
        /// </summary>
        public List<MapDataItem> MapData
        {
            get { return this._mapDataItem; }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public MapDataType3()
        {
            this._mapDataItem = new List<MapDataItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Material ID={0}, ID Type={1}", this.MaterialId, this.IDType);
        }

        /// <summary>
        /// Map data를 추가합니다.
        /// </summary>
        /// <param name="binList">The Bin List.</param>
        /// <param name="x">Start location x입니다.</param>
        /// <param name="y">Start location y입니다.</param>
        public void AddMapData(string binList, long x, long y)
        {
            this.MapData.Add(new MapDataItem(binList, x, y, 0));
        }
    }
}