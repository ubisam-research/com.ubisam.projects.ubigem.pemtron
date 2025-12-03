using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;
using UbiGEM.Net.Structure.WaferMap;
using UbiGEM.Net.Utility.Logger;

namespace UbiGEM.Net.Driver
{
    partial class GemDriver
    {
        /// <summary>
        /// Map set-up data send를 송신합니다.(S12F1)
        /// </summary>
        /// <param name="mapSetupData"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapSetupDataSend(MapSetupData mapSetupData)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            SECSItemFormat referencePointFormat;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapSetupDataSend");

                if (result == GemDriverError.Ok)
                {
                    referencePointFormat = GetSECSFormat(PreDefinedDataDictinary.REFP, SECSItemFormat.I2);

                    message = this._driver.Messages.GetMessageHeader(12, 1);

                    message.Body.Add(SECSItemFormat.L, 15, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, mapSetupData.MaterialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, mapSetupData.IDType, SECSItemFormat.B));

                    if (mapSetupData.FlatNotchLocation == null)
                    {
                        message.Body.Add(GetSECSFormat(PreDefinedDataDictinary.FNLOC, SECSItemFormat.U2), 0, null);
                    }
                    else
                    {
                        message.Body.Add(GetSECSItem("FNLOC", PreDefinedDataDictinary.FNLOC, mapSetupData.FlatNotchLocation, SECSItemFormat.U2));
                    }

                    if (mapSetupData.FilmFrameRotation == null)
                    {
                        message.Body.Add(GetSECSFormat(PreDefinedDataDictinary.FFROT, SECSItemFormat.U2), 0, null);
                    }
                    else
                    {
                        message.Body.Add(GetSECSItem("FFROT", PreDefinedDataDictinary.FFROT, mapSetupData.FilmFrameRotation, SECSItemFormat.U2));
                    }

                    if (mapSetupData.OriginLocation == null)
                    {
                        message.Body.Add(GetSECSFormat(PreDefinedDataDictinary.ORLOC, SECSItemFormat.B), 0, null);
                    }
                    else
                    {
                        message.Body.Add(GetSECSItem("ORLOC", PreDefinedDataDictinary.ORLOC, mapSetupData.OriginLocation, SECSItemFormat.B));
                    }

                    message.Body.Add(GetSECSItem("RPSEL  ", PreDefinedDataDictinary.RPSEL, mapSetupData.ReferencePointSelect, SECSItemFormat.U1));

                    if (mapSetupData.ReferencePoint != null && mapSetupData.ReferencePoint.Count > 0)
                    {
                        message.Body.Add("POINTCOUNT", SECSItemFormat.L, mapSetupData.ReferencePoint.Count, null);

                        foreach (ReferencePointItem tempReferencePointItem in mapSetupData.ReferencePoint)
                        {
                            switch (referencePointFormat)
                            {
                                case SECSItemFormat.I1:
                                    {
                                        byte[] referencePoint = new byte[2];

                                        referencePoint[0] = (byte)tempReferencePointItem.X;
                                        referencePoint[1] = (byte)tempReferencePointItem.Y;

                                        message.Body.Add("REFP", referencePointFormat, referencePoint.Length, referencePoint);
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        short[] referencePoint = new short[2];

                                        referencePoint[0] = (short)tempReferencePointItem.X;
                                        referencePoint[1] = (short)tempReferencePointItem.Y;

                                        message.Body.Add("REFP", referencePointFormat, referencePoint.Length, referencePoint);
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        int[] referencePoint = new int[2];

                                        referencePoint[0] = (int)tempReferencePointItem.X;
                                        referencePoint[1] = (int)tempReferencePointItem.Y;

                                        message.Body.Add("REFP", referencePointFormat, referencePoint.Length, referencePoint);
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        long[] referencePoint = new long[2];

                                        referencePoint[0] = tempReferencePointItem.X;
                                        referencePoint[1] = tempReferencePointItem.Y;

                                        message.Body.Add("REFP", referencePointFormat, referencePoint.Length, referencePoint);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("POINTCOUNT", SECSItemFormat.L, 0, null);
                    }

                    message.Body.Add(GetSECSItem("DUTMS", PreDefinedDataDictinary.DUTMS, mapSetupData.DieUnitsOfMeasure, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("XDIES", PreDefinedDataDictinary.XDIES, mapSetupData.XAxisDieSize, SECSItemFormat.U4));
                    message.Body.Add(GetSECSItem("YDIES", PreDefinedDataDictinary.YDIES, mapSetupData.YAxisDieSize, SECSItemFormat.U4));
                    message.Body.Add(GetSECSItem("ROWCT", PreDefinedDataDictinary.ROWCT, mapSetupData.RowCount, SECSItemFormat.U4));
                    message.Body.Add(GetSECSItem("COLCT", PreDefinedDataDictinary.COLCT, mapSetupData.ColumnCount, SECSItemFormat.U4));
                    message.Body.Add(GetSECSItem("NULBC ", PreDefinedDataDictinary.NULBC, mapSetupData.NullBinCodeValue, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("PRDCT ", PreDefinedDataDictinary.PRDCT, mapSetupData.ProcessDieCount, SECSItemFormat.U4));
                    message.Body.Add(GetSECSItem("PRAXI ", PreDefinedDataDictinary.PRAXI, mapSetupData.ProcessAxis, SECSItemFormat.B));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F1)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F1):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map set-up data request를 송신합니다.(S12F3)
        /// </summary>
        /// <param name="materialId">Material ID입니다.</param>
        /// <param name="idType">ID type입니다.</param>
        /// <param name="mapDataFormatType"></param>
        /// <param name="flatNotchLocation">Flat/Notch location.</param>
        /// <param name="filmFrameRotation">Film frame rotation.</param>
        /// <param name="originLocation">Origin location.</param>
        /// <param name="processAxis">Process axis.</param>
        /// <param name="binCodeEquivalents">Bin code equivalents.</param>
        /// <param name="nullBinCodeValue">Null bin code value.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapSetupDataRequest(
            string materialId,
            int idType,
            int mapDataFormatType,
            ulong? flatNotchLocation,
            ulong? filmFrameRotation,
            ulong? originLocation,
            long processAxis,
            string binCodeEquivalents,
            string nullBinCodeValue)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            SECSItemFormat referencePointFormat;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapSetupDataRequest");

                if (result == GemDriverError.Ok)
                {
                    referencePointFormat = GetSECSFormat(PreDefinedDataDictinary.REFP, SECSItemFormat.I2);

                    message = this._driver.Messages.GetMessageHeader(12, 3);

                    message.Body.Add(SECSItemFormat.L, 9, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, materialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, idType, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("MAPFT", PreDefinedDataDictinary.MAPFT, mapDataFormatType, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("FNLOC", PreDefinedDataDictinary.FNLOC, flatNotchLocation, SECSItemFormat.U2));
                    message.Body.Add(GetSECSItem("FFROT", PreDefinedDataDictinary.FFROT, filmFrameRotation, SECSItemFormat.U2));
                    message.Body.Add(GetSECSItem("ORLOC ", PreDefinedDataDictinary.ORLOC, originLocation, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("PRAXI ", PreDefinedDataDictinary.PRAXI, processAxis, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("BCEQU", PreDefinedDataDictinary.BCEQU, binCodeEquivalents, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("NULBC ", PreDefinedDataDictinary.NULBC, nullBinCodeValue, SECSItemFormat.A));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F3)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F3):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        ///  Map transmit inquire를 송신합니다.(S12F5)
        /// </summary>
        /// <param name="materialId">Material ID입니다.</param>
        /// <param name="idType">ID type입니다.</param>
        /// <param name="mapDataFormatType"></param>
        /// <param name="messagelength">Message length.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapTransmitInquire(
            string materialId,
            int idType,
            int mapDataFormatType,
            long messagelength)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            SECSItemFormat referencePointFormat;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapTransmitInquire");

                if (result == GemDriverError.Ok)
                {
                    referencePointFormat = GetSECSFormat(PreDefinedDataDictinary.REFP, SECSItemFormat.I2);

                    message = this._driver.Messages.GetMessageHeader(12, 5);

                    message.Body.Add(SECSItemFormat.L, 4, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, materialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, idType, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("MAPFT", PreDefinedDataDictinary.MAPFT, mapDataFormatType, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("MLCL ", PreDefinedDataDictinary.MLCL, messagelength, SECSItemFormat.U4));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F5)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F5):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map data send(Type 1)를 송신합니다.(S12F7)
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapDataSendType1(MapDataType1 mapData)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            SECSItemFormat startLocationFormat;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapDataSendType1");

                if (result == GemDriverError.Ok)
                {
                    startLocationFormat = GetSECSFormat(PreDefinedDataDictinary.RSINF, SECSItemFormat.I2);

                    message = this._driver.Messages.GetMessageHeader(12, 7);

                    message.Body.Add(SECSItemFormat.L, 3, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, mapData.MaterialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, mapData.IDType, SECSItemFormat.B));

                    if (mapData.MapData != null && mapData.MapData.Count > 0)
                    {
                        message.Body.Add("MAPDATACOUNT", SECSItemFormat.L, mapData.MapData.Count, null);

                        foreach (MapDataItem tempMapDataItem in mapData.MapData)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            switch (startLocationFormat)
                            {
                                case SECSItemFormat.I1:
                                    {
                                        byte[] startLocation = new byte[3];

                                        startLocation[0] = (byte)tempMapDataItem.X;
                                        startLocation[1] = (byte)tempMapDataItem.Y;
                                        startLocation[2] = (byte)tempMapDataItem.Direction;

                                        message.Body.Add("RSINF", startLocationFormat, startLocation.Length, startLocation);
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        short[] startLocation = new short[3];

                                        startLocation[0] = (short)tempMapDataItem.X;
                                        startLocation[1] = (short)tempMapDataItem.Y;
                                        startLocation[2] = (short)tempMapDataItem.Direction;

                                        message.Body.Add("RSINF", startLocationFormat, startLocation.Length, startLocation);
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        int[] startLocation = new int[3];

                                        startLocation[0] = (int)tempMapDataItem.X;
                                        startLocation[1] = (int)tempMapDataItem.Y;
                                        startLocation[2] = (int)tempMapDataItem.Direction;

                                        message.Body.Add("RSINF", startLocationFormat, startLocation.Length, startLocation);
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        long[] startLocation = new long[3];

                                        startLocation[0] = tempMapDataItem.X;
                                        startLocation[1] = tempMapDataItem.Y;
                                        startLocation[2] = tempMapDataItem.Direction;

                                        message.Body.Add("RSINF", startLocationFormat, startLocation.Length, startLocation);
                                    }
                                    break;
                            }

                            message.Body.Add(GetSECSItem("BINLT", PreDefinedDataDictinary.BINLT, tempMapDataItem.BinList, SECSItemFormat.A));
                        }
                    }
                    else
                    {
                        message.Body.Add("MAPDATACOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F7)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F7):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map data send(Type 2)를 송신합니다.(S12F9)
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapDataSendType2(MapDataType2 mapData)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            SECSItemFormat startPositionFormat;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapDataSendType2");

                if (result == GemDriverError.Ok)
                {
                    startPositionFormat = GetSECSFormat(PreDefinedDataDictinary.STRP, SECSItemFormat.I2);

                    message = this._driver.Messages.GetMessageHeader(12, 9);

                    message.Body.Add(SECSItemFormat.L, 4, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, mapData.MaterialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, mapData.IDType, SECSItemFormat.B));

                    switch (startPositionFormat)
                    {
                        case SECSItemFormat.I1:
                            {
                                byte[] startPosition = new byte[2];

                                startPosition[0] = (byte)mapData.StartPositionX;
                                startPosition[1] = (byte)mapData.StartPositionY;

                                message.Body.Add("STRP", startPositionFormat, startPosition.Length, startPosition);
                            }
                            break;
                        case SECSItemFormat.I2:
                            {
                                short[] startPosition = new short[2];

                                startPosition[0] = (short)mapData.StartPositionX;
                                startPosition[1] = (short)mapData.StartPositionY;

                                message.Body.Add("STRP", startPositionFormat, startPosition.Length, startPosition);
                            }
                            break;
                        case SECSItemFormat.I4:
                            {
                                int[] startPosition = new int[2];

                                startPosition[0] = (int)mapData.StartPositionX;
                                startPosition[1] = (int)mapData.StartPositionY;

                                message.Body.Add("STRP", startPositionFormat, startPosition.Length, startPosition);
                            }
                            break;
                        case SECSItemFormat.I8:
                            {
                                long[] startPosition = new long[3];

                                startPosition[0] = mapData.StartPositionX;
                                startPosition[1] = mapData.StartPositionY;

                                message.Body.Add("STRP", startPositionFormat, startPosition.Length, startPosition);
                            }
                            break;
                    }

                    message.Body.Add(GetSECSItem("BINLT", PreDefinedDataDictinary.BINLT, mapData.BinList, SECSItemFormat.A));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F9)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F9):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map data send(Type 3)를 송신합니다.(S12F11)
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapDataSendType3(MapDataType3 mapData)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            SECSItemFormat positionFormat;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapDataSendType3");

                if (result == GemDriverError.Ok)
                {
                    positionFormat = GetSECSFormat(PreDefinedDataDictinary.XYPOS, SECSItemFormat.I2);

                    message = this._driver.Messages.GetMessageHeader(12, 11);

                    message.Body.Add(SECSItemFormat.L, 3, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, mapData.MaterialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, mapData.IDType, SECSItemFormat.B));

                    if (mapData.MapData != null && mapData.MapData.Count > 0)
                    {
                        message.Body.Add("MAPDATACOUNT", SECSItemFormat.L, mapData.MapData.Count, null);

                        foreach (MapDataItem tempMapDataItem in mapData.MapData)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            switch (positionFormat)
                            {
                                case SECSItemFormat.I1:
                                    {
                                        byte[] position = new byte[2];

                                        position[0] = (byte)tempMapDataItem.X;
                                        position[1] = (byte)tempMapDataItem.Y;

                                        message.Body.Add("XYPOS", positionFormat, position.Length, position);
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        short[] position = new short[2];

                                        position[0] = (short)tempMapDataItem.X;
                                        position[1] = (short)tempMapDataItem.Y;

                                        message.Body.Add("XYPOS", positionFormat, position.Length, position);
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        int[] position = new int[2];

                                        position[0] = (int)tempMapDataItem.X;
                                        position[1] = (int)tempMapDataItem.Y;

                                        message.Body.Add("XYPOS", positionFormat, position.Length, position);
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        long[] position = new long[2];

                                        position[0] = tempMapDataItem.X;
                                        position[1] = tempMapDataItem.Y;

                                        message.Body.Add("XYPOS", positionFormat, position.Length, position);
                                    }
                                    break;
                            }

                            message.Body.Add(GetSECSItem("BINLT", PreDefinedDataDictinary.BINLT, tempMapDataItem.BinList, SECSItemFormat.A));
                        }
                    }
                    else
                    {
                        message.Body.Add("MAPDATACOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F11)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F11):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map data request(Type 1)를 송신합니다.(S12F13)
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="idType"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapDataType1(string materialId, int idType)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapDataType1");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(12, 13);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, materialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, idType, SECSItemFormat.B));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F13)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F13):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map data request(Type 2)를 송신합니다.(S12F15)
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="idType"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapDataType2(string materialId, int idType)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapDataType2");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(12, 15);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, materialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, idType, SECSItemFormat.B));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F15)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F15):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map data request(Type 3)를 송신합니다.(S12F17)
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="idType"></param>
        /// <param name="sendBinFlag">Send bin information flag</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestMapDataType3(string materialId, int idType, int sendBinFlag)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestMapDataType3");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(12, 17);

                    message.Body.Add(SECSItemFormat.L, 3, null);
                    message.Body.Add(GetSECSItem("MID", PreDefinedDataDictinary.MID, materialId, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("IDTYP", PreDefinedDataDictinary.IDTYP, idType, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("SDBIN", PreDefinedDataDictinary.SDBIN, sendBinFlag, SECSItemFormat.B));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F17)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F17):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Map Error Report Send를 송신합니다.(S12F19)
        /// </summary>
        /// <param name="mapError">Map Error.</param>
        /// <param name="dataLocation">Data location. </param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError SendMapErrorReport(int mapError, int dataLocation)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("SendMapErrorReport");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(12, 19);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add(GetSECSItem("MAPER", PreDefinedDataDictinary.MAPER, mapError, SECSItemFormat.B));
                    message.Body.Add(GetSECSItem("DATLC", PreDefinedDataDictinary.DATLC, dataLocation, SECSItemFormat.U1));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S12F19)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S12F19):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }
    }
}