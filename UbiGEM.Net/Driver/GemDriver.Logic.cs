using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;
using UbiGEM.Net.Utility.Logger;

//[assembly: System.Runtime.CompilerServices.SuppressIldasm()]

namespace UbiGEM.Net.Driver
{
    partial class GemDriver
    {
        /// <summary>
        /// SV/DVVAL의 값을 설정합니다.
        /// </summary>
        /// <param name="vid">설정할 VID입니다.</param>
        /// <param name="newValue">설정할 값입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError SetVariable(string vid, object newValue)
        {
            GemDriverError result;
            VariableInfo variableInfo;
            LimitMonitoringInfo limitMonitoringInfo;
            string logText;
            object convertValue;

            try
            {
                variableInfo = this._variableCollection.Variables[vid];

                if (variableInfo != null)
                {
                    result = GemDriverError.Ok;

                    if (newValue == null)
                    {
                        convertValue = ConvertSecsValue(variableInfo, string.Empty, out result);

                        if (result == GemDriverError.Ok)
                        {
                            variableInfo.Value.SetValue(convertValue);
                        }
                    }
                    else if (newValue is string)
                    {
                        convertValue = ConvertSecsValue(variableInfo, newValue.ToString(), out result);

                        if (result == GemDriverError.Ok)
                        {
                            variableInfo.Value.SetValue(convertValue);
                        }
                    }
                    else
                    {
                        variableInfo.Value.SetValue(newValue);
                    }

                    if (result == GemDriverError.Ok)
                    {
                        if (this._limitMonitoringCollection.Exist(vid) == true)
                        {
                            limitMonitoringInfo = this._limitMonitoringCollection[vid];

                            limitMonitoringInfo.SetValue(newValue);

                            if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
                            {
                                //\\// 보고 여부 Check 해야 함
                                this._messageMaker.MakeEventReportByLimitMonitoring(this._collectionEventCollection, this._variableCollection, limitMonitoringInfo, out SECSMessage message, out string errorText);

                                if (message != null)
                                {
                                }
                            }
                        }
                    }
                    else if (result == GemDriverError.InvalidFormat)
                    {
                        logText = string.Format("SetVariable-Invalid Format:VID={0}, Format={1}, Value={2}", vid, variableInfo.Format, newValue);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                    else
                    {
                        logText = string.Format("SetVariable-Unknown error:Result={0}, VID:{1}", result, vid);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    logText = string.Format("SetVariable-Undefined VID:{0}", vid);

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// SV/DVVAL의 값을 설정합니다.
        /// </summary>
        /// <param name="vid">설정할 VID List입니다.</param>
        /// <param name="newValue">설정할 값 List입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError SetVariable(List<string> vid, List<object> newValue)
        {
            GemDriverError result;
            VariableInfo variableInfo;
            LimitMonitoringInfo limitMonitoringInfo;
            string logText;
            string undefinedVid;
            object convertValue;
            GemDriverError convertResult;

            try
            {
                if (vid != null && newValue != null)
                {
                    if (vid.Count == newValue.Count)
                    {
                        result = GemDriverError.Ok;
                        undefinedVid = string.Empty;

                        for (int i = 0; i < vid.Count; i++)
                        {
                            variableInfo = this._variableCollection.Variables[vid[i]];

                            if (variableInfo != null)
                            {
                                if (variableInfo.Name == PreDefinedV.ProcessState.ToString())
                                {
                                    if (newValue[i] == null)
                                    {
                                        convertValue = ConvertSecsValue(variableInfo, string.Empty, out convertResult);

                                        if (convertResult == GemDriverError.Ok)
                                        {
                                            result = ReportEquipmentProcessingState((byte)(convertValue));
                                        }
                                        else
                                        {
                                            result = GemDriverError.InvalidFormat;
                                        }
                                    }
                                    else if (newValue[i] is string)
                                    {
                                        convertValue = ConvertSecsValue(variableInfo, newValue[i].ToString(), out convertResult);

                                        if (convertResult == GemDriverError.Ok)
                                        {
                                            result = ReportEquipmentProcessingState((byte)(convertValue));
                                        }
                                        else
                                        {
                                            result = GemDriverError.InvalidFormat;
                                        }
                                    }
                                    else
                                    {
                                        result = ReportEquipmentProcessingState((byte)(newValue[i]));
                                    }

                                    if (result == GemDriverError.SameState)
                                    {
                                        result = GemDriverError.Ok;
                                    }
                                }
                                else
                                {
                                    if (newValue[i] == null)
                                    {
                                        convertValue = ConvertSecsValue(variableInfo, string.Empty, out result);

                                        if (result == GemDriverError.Ok)
                                        {
                                            variableInfo.Value.SetValue(convertValue);
                                        }
                                    }
                                    else if (newValue[i] is string)
                                    {
                                        convertValue = ConvertSecsValue(variableInfo, newValue[i].ToString(), out result);

                                        if (result == GemDriverError.Ok)
                                        {
                                            variableInfo.Value.SetValue(convertValue);
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value.SetValue(newValue[i]);
                                    }

                                    if (result == GemDriverError.Ok)
                                    {
                                        if (this._limitMonitoringCollection.Exist(vid[i]) == true)
                                        {
                                            limitMonitoringInfo = this._limitMonitoringCollection[vid[i]];

                                            limitMonitoringInfo.SetValue(newValue[i]);

                                            if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
                                            {
                                                //\\// 보고 여부 Check 해야 함
                                                this._messageMaker.MakeEventReportByLimitMonitoring(this._collectionEventCollection, this._variableCollection, limitMonitoringInfo, out SECSMessage message, out string errorText);

                                                if (message != null)
                                                {
                                                }
                                            }
                                        }
                                    }
                                }

                                if (result == GemDriverError.InvalidFormat)
                                {
                                    logText = string.Format("SetVariable-Invalid Format:VID={0}, Format={1}, Value={2}", vid, variableInfo.Format, newValue);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                                else if (result != GemDriverError.Ok)
                                {
                                    logText = string.Format("SetVariable-Unknown error:Result={0}, VID:{1}", result, vid);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                undefinedVid = string.Format("VID={0}", vid[i]);
                                result = GemDriverError.Undefined;

                                break;
                            }
                        }

                        if (result == GemDriverError.Undefined)
                        {
                            logText = string.Format("SetVariable-Undefined VID:{0}", undefinedVid);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        result = GemDriverError.Mismatch;

                        logText = string.Format("SetVariable-Count mismatch:VID={0}, Value={1}", vid.Count, newValue.Count);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    if (vid == null)
                    {
                        logText = "SetVariable-Variable list is null";
                    }
                    else
                    {
                        logText = "SetVariable-Value list is null";
                    }

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// SV/DVVAL의 값을 설정합니다.
        /// </summary>
        /// <param name="variableInfo">설정할 Variable Info입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError SetVariable(VariableInfo variableInfo)
        {
            GemDriverError result;
            VariableInfo orgVariableInfo;
            string logText;

            try
            {
                if (variableInfo != null)
                {
                    orgVariableInfo = this._variableCollection.Variables[variableInfo.VID];

                    if (orgVariableInfo != null)
                    {
                        orgVariableInfo.Length = variableInfo.Length;
                        orgVariableInfo.Value = variableInfo.Value;
                        orgVariableInfo.ChildVariables = variableInfo.ChildVariables;

                        result = GemDriverError.Ok;
                    }
                    else
                    {
                        result = GemDriverError.Undefined;

                        logText = string.Format("SetVariable-Undefined VID:{0}", variableInfo.VID);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    logText = "SetVariable-Variable is null";

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// SV/DVVAL의 값을 설정합니다.
        /// </summary>
        /// <param name="variableInfos">설정할 Variable Info입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError SetVariable(VariableCollection variableInfos)
        {
            GemDriverError result;
            VariableInfo orgVariableInfo;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                if (variableInfos != null)
                {
                    foreach (VariableInfo tempVariableInfo in variableInfos.Items)
                    {
                        orgVariableInfo = this._variableCollection.Variables[tempVariableInfo.VID];

                        if (orgVariableInfo != null)
                        {
                            orgVariableInfo.Length = tempVariableInfo.Length;
                            orgVariableInfo.Value = tempVariableInfo.Value;
                            orgVariableInfo.ChildVariables = tempVariableInfo.ChildVariables;
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            logText = string.Format("Undefined VID:{0}", tempVariableInfo.VID);

                            this._logger.WriteGEM(LogLevel.Error, logText);

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// SV/DVVAL의 값을 설정합니다.
        /// </summary>
        /// <param name="vid">설정할 VID입니다.</param>
        /// <param name="customVariableInfo"></param>
        /// <returns></returns>
        public GemDriverError SetVariable(string vid, CustomVariableInfo customVariableInfo)
        {
            GemDriverError result;
            VariableInfo variableInfo;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                variableInfo = this._variableCollection.Variables[vid];

                if (variableInfo != null)
                {
                    lock (variableInfo)
                    {
                        if (customVariableInfo.SetChildVariables(variableInfo, out string errorText) == false)
                        {
                            result = GemDriverError.MessageMakeFailed;

                            logText = string.Format("SetVariable-Set Custom Data:{0}, Reason={1}", vid, errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    logText = string.Format("SetVariable-Undefined VID:{0}", vid);

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// ECV를 설정하고 ECV changed 이벤트를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ecid">설정할 ECID입니다.</param>
        /// <param name="newValue">설정할 값입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError SetEquipmentConstant(string ecid, object newValue)
        {
            GemDriverError result;
            VariableInfo variableInfo;
            string errorText;
            string logText;
            object convertValue;

            try
            {
                variableInfo = this._variableCollection.ECV[ecid];

                if (variableInfo != null)
                {
                    result = GemDriverError.Ok;

                    if (newValue is string)
                    {
                        convertValue = ConvertSecsValue(variableInfo, newValue.ToString(), out result);

                        if (result == GemDriverError.Ok)
                        {
                            variableInfo.Value.SetValue(convertValue);
                        }
                    }
                    else
                    {
                        variableInfo.Value.SetValue(newValue);
                    }

                    if (result == GemDriverError.Ok)
                    {
                        result = CheckECVChangedByDriverValue(variableInfo);

                        if (result == GemDriverError.Ok)
                        {
                            try
                            {
                                result = this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.EquipmentConstants, false, out errorText);
                            }
                            catch
                            {
                                errorText = string.Empty;
                            }

                            if (result == GemDriverError.Ok)
                            {
                                if (CheckTransmittable("SetEquipmentConstant", true) == GemDriverError.Ok)
                                {
                                    result = SendEquipmentConstantChanged(variableInfo);
                                }
                            }
                            else
                            {
                                logText = string.Format("ECV save failed(SetEquipmentConstant):Error={0}", errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    logText = string.Format("Undefined ECID:{0}", ecid);

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// ECV를 (n개)설정하고 ECV changed 이벤트를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ecid">설정할 ECID List입니다.</param>
        /// <param name="newValue">설정할 값 List입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError SetEquipmentConstant(List<string> ecid, List<object> newValue)
        {
            GemDriverError result;
            List<VariableInfo> variableInfos;
            VariableInfo variableInfo;
            string errorText;
            string logText;
            string undefinedEcid;
            object convertValue;

            try
            {
                if (ecid.Count == newValue.Count)
                {
                    result = GemDriverError.Ok;
                    variableInfos = new List<VariableInfo>();
                    undefinedEcid = string.Empty;

                    for (int i = 0; i < ecid.Count; i++)
                    {
                        variableInfo = this._variableCollection.ECV[ecid[i]];

                        if (variableInfo != null)
                        {
                            if (newValue[i] is string)
                            {
                                convertValue = ConvertSecsValue(variableInfo, newValue[i].ToString(), out result);

                                if (result == GemDriverError.Ok)
                                {
                                    variableInfo.Value.SetValue(convertValue);
                                }
                            }
                            else
                            {
                                variableInfo.Value.SetValue(newValue[i]);
                            }

                            variableInfos.Add(variableInfo);
                        }
                        else
                        {
                            undefinedEcid = string.Format("ECID={0}", ecid[i]);
                            result = GemDriverError.Undefined;

                            break;
                        }
                    }

                    if (result == GemDriverError.Ok)
                    {
                        result = CheckECVChangedByDriverValue(variableInfos);

                        if (result == GemDriverError.Ok)
                        {
                            try
                            {
                                result = this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.EquipmentConstants, false, out errorText);
                            }
                            catch
                            {
                                errorText = string.Empty;
                            }

                            if (result == GemDriverError.Ok)
                            {
                                if (CheckTransmittable("SetEquipmentConstant", true) == GemDriverError.Ok)
                                {
                                    result = SendEquipmentConstantChanged(variableInfos);
                                }
                            }
                            else
                            {
                                logText = string.Format("ECV save failed(SetEquipmentConstant):Error={0}", errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        logText = string.Format("Undefined ECID:{0}", undefinedEcid);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                }
                else
                {
                    result = GemDriverError.Mismatch;

                    logText = string.Format("Count mismatch:ECID={0}, ECV={1}", ecid.Count, newValue.Count);

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }

            return result;
        }

        private void SetLimitMonitoring(SECSMessage message)
        {
            const int VARIABLE_DOES_NOT_EXIST = 1;
            const int VARIABLE_HAS_NO_LIMITS_CAPABILITY = 2;
            const int VARIABLE_REPEATED_IN_MESSAGE = 3;
            const int LIMIT_VALUE_ERROR = 4;
            const int UPPERDB_LESS_THEN_LIMITMAX = 2;
            const int LOWERDB_GREATER_LIMITMIN = 3;
            const int UPPERDB_GREATER_LOWERDB = 4;
            const int ILLEGAL_FORMAT_SPECIFIED = 5;
            const int CANNOT_TRANSLATED_TO_NUMERIC = 6;

            SECSMessage replyMessage;

            string vid;
            long limitId;
            List<LimitMonitoringInfo> limitMonitoringList;
            LimitMonitoringInfo limitMonitoringInfo;
            LimitMonitoringItem limitMonitoringItem;
            VariableInfo variableInfo;
            Dictionary<string, int> lvAck; // VID, LVACK
            Dictionary<string, Dictionary<long, int>> limitAck; // VID, LimitID, LIMITACK
            int itemAck;
            SECSItemFormat vidFormat;
            SECSItemFormat limitIdFormat;
            SECSItemFormat lvAckFormat;
            SECSItemFormat limitAckFormat;
            string logText;
            MessageError driverResult;

            try
            {
                limitMonitoringList = new List<LimitMonitoringInfo>();
                lvAck = new Dictionary<string, int>();
                limitAck = new Dictionary<string, Dictionary<long, int>>();

                if (message.Body.Item[0].SubItem[1].SubItem.Count > 0)
                {
                    foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[1].SubItem.Items)
                    {
                        if (tempSECSItem.SubItem[1].SubItem.Count > 0)
                        {
                            limitMonitoringInfo = new LimitMonitoringInfo();

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                vid = tempSECSItem.SubItem[0].Value;
                            }
                            else
                            {
                                vid = ((long)tempSECSItem.SubItem[0].Value).ToString();
                            }

                            variableInfo = this._variableCollection[vid];

                            if (variableInfo != null && variableInfo.IsUse == true)
                            {
                                if (lvAck.ContainsKey(vid) == false)
                                {
                                    if (variableInfo.Min == null || variableInfo.Max == null)
                                    {
                                        lvAck[vid] = VARIABLE_HAS_NO_LIMITS_CAPABILITY;
                                    }
                                    else
                                    {
                                        foreach (SECSItem tempLimit in tempSECSItem.SubItem[1].SubItem.Items)
                                        {
                                            limitId = tempLimit.SubItem[0].Value;
                                            itemAck = -1;

                                            if (tempLimit.SubItem[1].SubItem.Count == 2)
                                            {
                                                if (tempLimit.SubItem[1].SubItem[0].Format == SECSItemFormat.Boolean ||
                                                    tempLimit.SubItem[1].SubItem[1].Format != SECSItemFormat.Boolean)
                                                {
                                                    if (tempLimit.SubItem[1].SubItem[0].Format != tempLimit.SubItem[1].SubItem[1].Format ||
                                                        variableInfo.Format != SECSItemFormat.Boolean)
                                                    {
                                                        itemAck = ILLEGAL_FORMAT_SPECIFIED;
                                                    }
                                                    else
                                                    { //\\// 처리는 ????
                                                    }
                                                }
                                                else if (double.TryParse(tempLimit.SubItem[1].SubItem[0].Value, out double upper) == false ||
                                                    double.TryParse(tempLimit.SubItem[1].SubItem[1].Value, out double lower) == false)
                                                {
                                                    itemAck = CANNOT_TRANSLATED_TO_NUMERIC;
                                                }
                                                else if (variableInfo.Min.GetValueOrDefault() > lower)
                                                {
                                                    itemAck = LOWERDB_GREATER_LIMITMIN;
                                                }
                                                else if (variableInfo.Max.GetValueOrDefault() < upper)
                                                {
                                                    itemAck = UPPERDB_LESS_THEN_LIMITMAX;
                                                }
                                                else if (lower > upper)
                                                {
                                                    itemAck = UPPERDB_GREATER_LOWERDB;
                                                }
                                                else
                                                {
                                                    limitMonitoringItem = new LimitMonitoringItem()
                                                    {
                                                        LimitID = limitId,
                                                        UpperBoundary = upper,
                                                        LowerBoundary = lower
                                                    };

                                                    limitMonitoringInfo.Add(limitMonitoringItem);
                                                }
                                            }
                                            else
                                            {
                                            }

                                            if (itemAck > -1)
                                            {
                                                if (limitAck.ContainsKey(vid) == false)
                                                {
                                                    limitAck[vid] = new Dictionary<long, int>();
                                                }

                                                limitAck[vid][limitId] = itemAck;
                                                lvAck[vid] = LIMIT_VALUE_ERROR;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    lvAck[vid] = VARIABLE_REPEATED_IN_MESSAGE;
                                }
                            }
                            else
                            {
                                lvAck[vid] = VARIABLE_DOES_NOT_EXIST;
                            }

                            if (limitMonitoringInfo.Items.Count > 0)
                            {
                                limitMonitoringList.Add(limitMonitoringInfo);
                            }
                        }
                    }

                    if (lvAck.Count <= 0)
                    {
                        this._limitMonitoringCollection.Items.Clear();

                        limitMonitoringList.ForEach(t =>
                        {
                            this._limitMonitoringCollection.Items.Add(t);
                        });
                    }
                }
                else
                {
                    this._limitMonitoringCollection.Items.Clear();
                }

                logText = string.Format("Define Variable Limit Attributes Received(S2F{0}:{1}):Item Count={0}", message.Function, this._limitMonitoringCollection.Items.Count);

                this._logger.WriteGEM(LogLevel.Information, logText);

                replyMessage = this._driver.Messages.GetMessageHeader(2, 46);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (lvAck.Count <= 0)
                {
                    replyMessage.Body.Add("ACKC", GetSECSFormat(PreDefinedDataDictinary.VLAACK, SECSItemFormat.B), 1, ACK);
                    replyMessage.Body.Add(SECSItemFormat.L, 0, null);
                }
                else
                {
                    replyMessage.Body.Add("ACKC", GetSECSFormat(PreDefinedDataDictinary.VLAACK, SECSItemFormat.B), 1, NAK);

                    replyMessage.Body.Add("VARCOUNT", SECSItemFormat.L, lvAck.Count, null);

                    vidFormat = GetSECSFormat(PreDefinedDataDictinary.VID, SECSItemFormat.U2);
                    limitIdFormat = GetSECSFormat(PreDefinedDataDictinary.LIMITID, SECSItemFormat.U2);
                    lvAckFormat = GetSECSFormat(PreDefinedDataDictinary.LVACK, SECSItemFormat.B);
                    limitAckFormat = GetSECSFormat(PreDefinedDataDictinary.LIMITACK, SECSItemFormat.B);

                    foreach (KeyValuePair<string, int> tempLvAck in lvAck)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 3, null);
                        replyMessage.Body.Add("VID", vidFormat, 1, tempLvAck.Key);
                        replyMessage.Body.Add("ERRCODE", lvAckFormat, 1, tempLvAck.Value);

                        if (limitAck.ContainsKey(tempLvAck.Key) == true)
                        {
                            foreach (KeyValuePair<long, int> tempLimitAck in limitAck[tempLvAck.Key])
                            {
                                replyMessage.Body.Add("Limits", SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("LIMITID", limitIdFormat, 1, tempLimitAck.Key);
                                replyMessage.Body.Add("LIMITERROR", limitAckFormat, 1, tempLimitAck.Value);
                            }
                        }
                        else
                        {
                            replyMessage.Body.Add("Limits", SECSItemFormat.L, 0, null);
                        }
                    }
                }

                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S2F{0}:{1})", replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S2F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                this._logger.WriteGEM(ex);
            }
            finally
            {
                limitMonitoringInfo = null;
                limitMonitoringItem = null;
                replyMessage = null;
            }
        }

        private void CheckVariableUpdateRequest(CollectionEventInfo collectionEventInfo, bool useRaiseEvent, params PreDefinedV[] except)
        {
            List<VariableInfo> variableList;

            if (useRaiseEvent == true)
            {
                try
                {
                    variableList = (from ReportInfo tempReportInfo in collectionEventInfo.Reports.Items.Values
                                    from VariableInfo tempVariableInfo in tempReportInfo.Variables.Items
                                    where except.Any(t => t.ToString() == tempVariableInfo.Name) == false &&
                                          tempVariableInfo.VIDType != VariableType.ECV
                                    select tempVariableInfo).Distinct().ToList();

                    if (variableList != null && variableList.Count > 0)
                    {
                        this.OnVariableUpdateRequest?.Invoke(VariableUpdateType.S6F11EventReportSend, variableList, collectionEventInfo.CEID);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                    this._logger.WriteGEM(ex);
                }
                finally
                {
                    variableList = null;
                }
            }
        }

        private GemDriverError CheckTransmittable(string methodName, bool isOnlineCheck = true)
        {
            GemDriverError result;

            if (this._driver.Connected == true)
            {
                if (this._communicationState == CommunicationState.Communicating)
                {
                    if (isOnlineCheck == true)
                    {
                        if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
                        {
                            result = GemDriverError.Ok;
                        }
                        else
                        {
                            result = GemDriverError.ControlStateIsOffline;

                            this._logger.WriteGEM(LogLevel.Warning, $"{methodName}-Offline Mode");
                        }
                    }
                    else
                    {
                        result = GemDriverError.Ok;
                    }
                }
                else
                {
                    result = GemDriverError.NotCommunicating;

                    this._logger.WriteGEM(LogLevel.Warning, $"Failed to send:{methodName}-Driver Not Communicating");
                }
            }
            else
            {
                result = GemDriverError.Disconnected;

                this._logger.WriteGEM(LogLevel.Warning, $"Failed to send:{methodName}-Driver Disconnected");
            }

            return result;
        }

        private GemDriverError CheckECVChangedByDriverValue(VariableInfo ecv)
        {
            GemDriverError result;
            bool isUpdate;

            result = GemDriverError.Ok;

            if (ecv != null && ecv.PreDefined == true && Enum.TryParse<PreDefinedECV>(ecv.Name, out PreDefinedECV preDefined) == true)
            {
                isUpdate = false;

                try
                {
                    switch (preDefined)
                    {
                        case PreDefinedECV.DeviceID:
                            this._driver.Config.DeviceID = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.ActiveMode:
                            if (ecv.Value == true)
                            {
                                this._driver.Config.HSMSModeConfig.HSMSMode = HSMSMode.Active;
                            }
                            else
                            {
                                this._driver.Config.HSMSModeConfig.HSMSMode = HSMSMode.Passive;
                            }
                            isUpdate = true;
                            break;
                        case PreDefinedECV.IPAddress:
                            this._driver.Config.HSMSModeConfig.RemoteIPAddress = ecv.Value;
                            this._driver.Config.HSMSModeConfig.LocalIPAddress = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.PortNumber:
                            this._driver.Config.HSMSModeConfig.RemotePortNo = ecv.Value;
                            this._driver.Config.HSMSModeConfig.LocalPortNo = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.LinkTestInterval:
                            this._driver.Config.HSMSModeConfig.LinkTest = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.T3Timeout:
                            this._driver.Config.HSMSModeConfig.T3 = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.T5Timeout:
                            this._driver.Config.HSMSModeConfig.T5 = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.T6Timeout:
                            this._driver.Config.HSMSModeConfig.T6 = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.T7Timeout:
                            this._driver.Config.HSMSModeConfig.T7 = ecv.Value;
                            isUpdate = true;
                            break;
                        case PreDefinedECV.T8Timeout:
                            this._driver.Config.HSMSModeConfig.T8 = ecv.Value;
                            isUpdate = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    result = GemDriverError.Exception;
                    this._logger.WriteGEM(ex);
                }
                finally
                {
                    if (isUpdate == true)
                    {
                        GemDriverError saveResult;
                        string logText;

                        saveResult = this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.SECSDriver, false, out string errorText);

                        if (saveResult == GemDriverError.Ok)
                        {
                            logText = string.Format("HSMS Driver config changed:Name={0}, Value={1}", ecv.Name, ecv.Value.GetValue());

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        else
                        {
                            logText = string.Format("ECV save failed(CheckECVChangedByDriverValue):Error={0}", errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                }
            }

            return result;
        }

        private GemDriverError CheckECVChangedByDriverValue(List<VariableInfo> variableInfos)
        {
            GemDriverError result;
            bool isUpdate;
            string logText;

            result = GemDriverError.Ok;

            if (variableInfos != null)
            {
                logText = "HSMS Driver config changed:";
                isUpdate = false;

                try
                {
                    foreach (VariableInfo tempVariableInfo in variableInfos)
                    {
                        if (tempVariableInfo.PreDefined == true && Enum.TryParse<PreDefinedECV>(tempVariableInfo.Name, out PreDefinedECV preDefined) == true)
                        {
                            switch (preDefined)
                            {
                                case PreDefinedECV.DeviceID:
                                    this._driver.Config.DeviceID = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.ActiveMode:
                                    if (tempVariableInfo.Value == true)
                                    {
                                        this._driver.Config.HSMSModeConfig.HSMSMode = HSMSMode.Active;
                                    }
                                    else
                                    {
                                        this._driver.Config.HSMSModeConfig.HSMSMode = HSMSMode.Passive;
                                    }
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.IPAddress:
                                    this._driver.Config.HSMSModeConfig.RemoteIPAddress = tempVariableInfo.Value;
                                    this._driver.Config.HSMSModeConfig.LocalIPAddress = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.PortNumber:
                                    this._driver.Config.HSMSModeConfig.LocalPortNo = tempVariableInfo.Value;
                                    this._driver.Config.HSMSModeConfig.LocalPortNo = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.LinkTestInterval:
                                    this._driver.Config.HSMSModeConfig.LinkTest = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.T3Timeout:
                                    this._driver.Config.HSMSModeConfig.T3 = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.T5Timeout:
                                    this._driver.Config.HSMSModeConfig.T5 = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.T6Timeout:
                                    this._driver.Config.HSMSModeConfig.T6 = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.T7Timeout:
                                    this._driver.Config.HSMSModeConfig.T7 = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                                case PreDefinedECV.T8Timeout:
                                    this._driver.Config.HSMSModeConfig.T8 = tempVariableInfo.Value;
                                    isUpdate = true;
                                    logText += string.Format("[Name={0}, Value={1}] ", tempVariableInfo.Name, tempVariableInfo.Value.GetValue());
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = GemDriverError.Exception;
                    this._logger.WriteGEM(ex);
                }
                finally
                {
                    if (isUpdate == true)
                    {
                        GemDriverError saveResult;

                        saveResult = this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.SECSDriver, false, out string errorText);

                        if (saveResult == GemDriverError.Ok)
                        {
                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        else
                        {
                            logText = string.Format("ECV save failed(CheckECVChangedByDriverValue):Error={0}", errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                }
            }

            return result;
        }

        private static object ConvertSecsValue(VariableInfo variableInfo, string newValue, out GemDriverError convertResult)
        {
            object result;

            convertResult = GemDriverError.Ok;
            result = newValue;

            switch (variableInfo.Format)
            {
                case SECSItemFormat.B:
                    {
                        if (byte.TryParse(newValue, out byte value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.Boolean:
                    {
                        bool value;

                        if (newValue == "1" || newValue.ToUpper() == "TRUE")
                            value = true;
                        else
                            value = false;

                        result = value;
                    }
                    break;
                case SECSItemFormat.I1:
                    {
                        if (sbyte.TryParse(newValue, out sbyte value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.I2:
                    {
                        if (short.TryParse(newValue, out short value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.I4:
                    {
                        if (int.TryParse(newValue, out int value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.I8:
                    {
                        if (long.TryParse(newValue, out long value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.U1:
                    {
                        if (byte.TryParse(newValue, out byte value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.U2:
                    {
                        if (ushort.TryParse(newValue, out ushort value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.U4:
                    {
                        if (uint.TryParse(newValue, out uint value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.U8:
                    {
                        if (ulong.TryParse(newValue, out ulong value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.F4:
                    {
                        if (float.TryParse(newValue, out float value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
                case SECSItemFormat.F8:
                    {
                        if (double.TryParse(newValue, out double value) == true)
                        {
                            result = value;
                        }
                        else
                        {
                            convertResult = GemDriverError.InvalidFormat;
                        }
                    }
                    break;
            }

            return result;
        }

        private SECSItem ConvertStringToSECSItem(string name, SECSItemFormat format, string value)
        {
            SECSItem result;
            string[] splitData;

            try
            {
                splitData = value.Split(' ');

                if (splitData != null)
                {
                    if (splitData.Length <= 0)
                    {
                        result = new SECSItem(name, format, 0, null);
                    }
                    else if (splitData.Length == 1)
                    {
                        switch (format)
                        {
                            case SECSItemFormat.B:
                            case SECSItemFormat.U1:
                                result = new SECSItem(name, format, 1, Convert.ToByte(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.U2:
                                result = new SECSItem(name, format, 1, Convert.ToUInt16(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.U4:
                                result = new SECSItem(name, format, 1, Convert.ToUInt32(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.U8:
                                result = new SECSItem(name, format, 1, Convert.ToUInt64(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.I1:
                                result = new SECSItem(name, format, 1, Convert.ToSByte(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.I2:
                                result = new SECSItem(name, format, 1, Convert.ToInt16(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.I4:
                                result = new SECSItem(name, format, 1, Convert.ToInt32(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.I8:
                                result = new SECSItem(name, format, 1, Convert.ToInt64(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.F4:
                                result = new SECSItem(name, format, 1, Convert.ToSingle(value, CultureInfo.CurrentCulture));
                                break;
                            case SECSItemFormat.F8:
                                result = new SECSItem(name, format, 1, Convert.ToDouble(value, CultureInfo.CurrentCulture));
                                break;
                            default:
                                result = new SECSItem(name, format, 1, value);
                                break;
                        }
                    }
                    else
                    {
                        switch (format)
                        {
                            case SECSItemFormat.B:
                            case SECSItemFormat.U1:
                                {
                                    List<byte> listValue = new List<byte>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToByte(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.U2:
                                {
                                    List<ushort> listValue = new List<ushort>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToUInt16(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.U4:
                                {
                                    List<uint> listValue = new List<uint>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToUInt32(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.U8:
                                {
                                    List<ulong> listValue = new List<ulong>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToUInt64(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.I1:
                                {
                                    List<sbyte> listValue = new List<sbyte>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToSByte(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.I2:
                                {
                                    List<short> listValue = new List<short>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToInt16(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.I4:
                                {
                                    List<int> listValue = new List<int>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToInt32(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.I8:
                                {
                                    List<long> listValue = new List<long>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToInt64(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.F4:
                                {
                                    List<float> listValue = new List<float>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToSingle(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            case SECSItemFormat.F8:
                                {
                                    List<double> listValue = new List<double>();

                                    for (int i = 0; i < splitData.Length; i++)
                                    {
                                        if (string.IsNullOrEmpty(splitData[i]) == false)
                                        {
                                            listValue.Add(Convert.ToDouble(splitData[i], CultureInfo.CurrentCulture));
                                        }
                                    }

                                    result = new SECSItem(name, format, listValue.Count, listValue.ToArray());
                                }
                                break;
                            default:
                                result = new SECSItem(name, format, 1, value);
                                break;
                        }
                    }
                }
                else
                {
                    if (format == SECSItemFormat.A)
                    {
                        result = new SECSItem(name, format, Encoding.Default.GetByteCount(value), value);
                    }
                    else
                    {
                        result = new SECSItem(name, format, 1, value);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                this._logger.WriteGEM(ex);
            }

            return result;
        }

        private SECSItem ConvertVariableInfo2SECSItem(VariableInfo variableInfo)
        {
            SECSItem result;

            try
            {
                if (variableInfo.Format == SECSItemFormat.A || variableInfo.Format == SECSItemFormat.J)
                {
                    VariableInfo orgVariableInfo = this._orgVariableCollection[variableInfo.VID];

                    if (orgVariableInfo.Length > -1)
                    {
                        if (variableInfo.Value != null)
                        {
                            string value = variableInfo.Value.ToString();

                            if (orgVariableInfo.Length < Encoding.Default.GetByteCount(value))
                            {
                                string substring = Substring(value, 0, orgVariableInfo.Length);

                                this._logger.WriteGEM(LogLevel.Warning, $"Check the length of 'VARIABLE':Variable Name={variableInfo.Name}, Length={orgVariableInfo.Length}, Data Length={Encoding.Default.GetByteCount(value)}, Value={value}");

                                result = new SECSItem(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, substring), substring);
                            }
                            else
                            {
                                result = new SECSItem(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, variableInfo.Value), variableInfo.Value);
                            }
                        }
                        else
                        {
                            result = new SECSItem(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, variableInfo.Value), variableInfo.Value);
                        }
                    }
                    else
                    {
                        result = new SECSItem(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, variableInfo.Value), variableInfo.Value);
                    }
                }
                else
                {
                    result = new SECSItem(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, variableInfo.Value), variableInfo.Value);
                }
            }
            catch (Exception ex)
            {
                result = null;
                this._logger.WriteGEM(ex);
            }

            return result;
        }

        private GemDriverError FailAreYouThere()
        {
            GemDriverError result;
            string logText;
            ControlState controlState;
            VariableInfo onLineFailStateVariableInfo;
            VariableInfo controlStateVariableInfo;
            VariableInfo previousControlStateVariableInfo;

            result = GemDriverError.Ok;

            onLineFailStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedECV.OnLineFailState.ToString());
            controlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ControlState.ToString());
            previousControlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousControlState.ToString());

            if (onLineFailStateVariableInfo != null)
            {
                controlState = (ControlState)(int)onLineFailStateVariableInfo.Value;
            }
            else
            {
                controlState = ControlState.EquipmentOffline;
            }

            if (controlStateVariableInfo != null)
            {
                controlStateVariableInfo.Value = (int)controlState;
            }

            if (previousControlStateVariableInfo != null)
            {
                previousControlStateVariableInfo.Value = (int)this._controlState;
            }

            this._controlState = controlState;

            this.OnControlStateOnlineChangeFailed?.Invoke();

            this.OnControlStateChanged?.Invoke(this._controlState);

            logText = $"Control State Changed(FailAreYouThere):State={this._controlState}";

            this._logger.WriteGEM(LogLevel.Information, logText);

            return result;
        }

        private GemDriverError MakeEnhancedRemoteCommandInfo(SECSMessage message, EnhancedRemoteCommandInfo remoteCommandInfo, out EnhancedRemoteCommandInfo newRemoteCommandInfo, out Dictionary<string, int> cpAck)
        {
            GemDriverError result;

            EnhancedCommandParameterInfo commandParameterInfo;
            EnhancedCommandParameterInfo newCommandParameterInfo;
            EnhancedCommandParameterItem newEnhancedCommandParameterItem;

            result = GemDriverError.Ok;

            newRemoteCommandInfo = new EnhancedRemoteCommandInfo();
            cpAck = new Dictionary<string, int>();

            try
            {
                cpAck = new Dictionary<string, int>();

                newRemoteCommandInfo.RemoteCommand = remoteCommandInfo.RemoteCommand;
                newRemoteCommandInfo.SystemBytes = message.SystemBytes;
                newRemoteCommandInfo.Description = remoteCommandInfo.Description;

                newRemoteCommandInfo.DataID = message.Body.Item[0].SubItem[0].Value;
                newRemoteCommandInfo.ObjSpec = message.Body.Item[0].SubItem[1].Value;

                if (remoteCommandInfo.EnhancedCommandParameter != null && remoteCommandInfo.EnhancedCommandParameter.Items.Count > 0)
                {
                    List<string> configNames = remoteCommandInfo.EnhancedCommandParameter.Items.Select(t => t.Name).ToList();
                    List<string> rcvNames = new List<string>();

                    if (remoteCommandInfo.EnhancedCommandParameter.Items.Count == message.Body.Item[0].SubItem[3].SubItem.Items.Length)
                    {
                        foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[3].SubItem.Items)
                        {
                            rcvNames.Add(tempSECSItem.SubItem[0].Value);

                            commandParameterInfo = remoteCommandInfo.EnhancedCommandParameter[tempSECSItem.SubItem[0].Value];

                            if (commandParameterInfo != null)
                            {
                                newCommandParameterInfo = new EnhancedCommandParameterInfo()
                                {
                                    Name = commandParameterInfo.Name,
                                    Format = tempSECSItem.SubItem[1].Format,
                                    ParameterType = commandParameterInfo.ParameterType
                                };

                                if (newCommandParameterInfo.Format == SECSItemFormat.L)
                                {
                                    foreach (SECSItem tempSubSECSItem in tempSECSItem.SubItem[1].SubItem.Items)
                                    {
                                        if (tempSubSECSItem.Format == SECSItemFormat.L)
                                        {
                                            newEnhancedCommandParameterItem = new EnhancedCommandParameterItem()
                                            {
                                                Name = tempSubSECSItem.SubItem[0].Value,
                                                Format = tempSubSECSItem.SubItem[1].Format
                                            };

                                            if (newEnhancedCommandParameterItem.Format == SECSItemFormat.L)
                                            {
                                                MakeEnhancedRemoteCommandInfo(tempSubSECSItem.SubItem[1], ref newEnhancedCommandParameterItem);
                                            }
                                            else
                                            {
                                                newEnhancedCommandParameterItem.Value = tempSubSECSItem.SubItem[1].Value;
                                            }

                                            newCommandParameterInfo.Items.Add(newEnhancedCommandParameterItem);
                                        }
                                        else
                                        {
                                            newCommandParameterInfo.Items.Add(new EnhancedCommandParameterItem()
                                            {
                                                Format = tempSubSECSItem.Format,
                                                Value = tempSubSECSItem.Value
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (commandParameterInfo.Format == tempSECSItem.SubItem[1].Format)
                                    {
                                        newCommandParameterInfo.Value = tempSECSItem.SubItem[1].Value;
                                    }
                                    else
                                    {
                                        cpAck[tempSECSItem.SubItem[0].Value] = (int)CPACK.IllegalFormatSpecifiedForCPVAL;
                                    }
                                }

                                newRemoteCommandInfo.EnhancedCommandParameter.Add(newCommandParameterInfo);
                            }
                            else
                            {
                                cpAck[tempSECSItem.SubItem[0].Value] = (int)CPACK.ParameterNameDoesNotExist;
                            }
                        }
                    }
                    else
                    {
                        foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[3].SubItem.Items)
                        {
                            rcvNames.Add(tempSECSItem.SubItem[0].Value);

                            commandParameterInfo = remoteCommandInfo.EnhancedCommandParameter[tempSECSItem.SubItem[0].Value];

                            if (commandParameterInfo == null)
                            {
                                cpAck[tempSECSItem.SubItem[0].Value] = (int)CPACK.ParameterNameDoesNotExist;
                            }
                        }
                    }

                    foreach (string tempCPName in configNames)
                    {
                        if (rcvNames.Exists(t => t == tempCPName) == false)
                        {
                            cpAck[tempCPName] = (int)CPACK.ParameterNameDoesNotExist;
                        }
                    }
                }
                else
                {
                    foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[3].SubItem.Items)
                    {
                        newCommandParameterInfo = new EnhancedCommandParameterInfo()
                        {
                            Name = tempSECSItem.SubItem[0].Value,
                            Format = tempSECSItem.SubItem[1].Format
                        };

                        if (newCommandParameterInfo.Format == SECSItemFormat.L)
                        {
                            foreach (SECSItem tempSubSECSItem in tempSECSItem.SubItem[1].SubItem.Items)
                            {
                                if (tempSubSECSItem.Format == SECSItemFormat.L)
                                {
                                    newEnhancedCommandParameterItem = new EnhancedCommandParameterItem()
                                    {
                                        Name = tempSubSECSItem.SubItem[0].Value,
                                        Format = tempSubSECSItem.SubItem[1].Format
                                    };

                                    if (newEnhancedCommandParameterItem.Format == SECSItemFormat.L)
                                    {
                                        MakeEnhancedRemoteCommandInfo(tempSubSECSItem.SubItem[1], ref newEnhancedCommandParameterItem);
                                    }
                                    else
                                    {
                                        newEnhancedCommandParameterItem.Value = tempSubSECSItem.SubItem[1].Value;
                                    }

                                    newCommandParameterInfo.Items.Add(newEnhancedCommandParameterItem);
                                }
                                else
                                {
                                    newCommandParameterInfo.Items.Add(new EnhancedCommandParameterItem()
                                    {
                                        Format = tempSubSECSItem.Format,
                                        Value = tempSubSECSItem.Value
                                    });
                                }
                            }
                        }
                        else
                        {
                            newCommandParameterInfo.Value = tempSECSItem.SubItem[1].Value;
                        }

                        newRemoteCommandInfo.EnhancedCommandParameter.Add(newCommandParameterInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                newRemoteCommandInfo = null;

                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        private GemDriverError MakeEnhancedRemoteCommandInfo(SECSItem secsItem, ref EnhancedCommandParameterItem newEnhancedCommandParameterItem)
        {
            GemDriverError result;
            EnhancedCommandParameterItem newChildParameterItem;

            result = GemDriverError.Ok;

            try
            {
                foreach (SECSItem tempSubSECSItem in secsItem.SubItem.Items)
                {
                    if (tempSubSECSItem.SubItem == null || tempSubSECSItem.SubItem.Count <= 0)
                    {
                        newChildParameterItem = new EnhancedCommandParameterItem()
                        {
                            Value = tempSubSECSItem.Value,
                            Format = tempSubSECSItem.Format
                        };
                    }
                    else
                    {
                        newChildParameterItem = new EnhancedCommandParameterItem()
                        {
                            Name = tempSubSECSItem.SubItem[0].Value,
                            Format = tempSubSECSItem.SubItem[1].Format
                        };

                        if (newChildParameterItem.Format == SECSItemFormat.L)
                        {
                            MakeEnhancedRemoteCommandInfo(tempSubSECSItem.SubItem[1], ref newChildParameterItem);
                        }
                        else
                        {
                            newChildParameterItem.Value = tempSubSECSItem.SubItem[1].Value;
                        }

                    }

                    newEnhancedCommandParameterItem.ChildParameterItem.Items.Add(newChildParameterItem);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        //private GemDriverError MakeEnhancedRemoteCommandInfoViolationOfStandards(SECSMessage message, EnhancedRemoteCommandInfo remoteCommandInfo, out EnhancedRemoteCommandInfo newRemoteCommandInfo)
        //{
        //    GemDriverError result;

        //    EnhancedCommandParameterInfo commandParameterInfo;
        //    EnhancedCommandParameterInfo newCommandParameterInfo;
        //    EnhancedCommandParameterItem newEnhancedCommandParameterItem;

        //    result = GemDriverError.Ok;

        //    newRemoteCommandInfo = new EnhancedRemoteCommandInfo();

        //    try
        //    {
        //        newRemoteCommandInfo.RemoteCommand = remoteCommandInfo.RemoteCommand;
        //        newRemoteCommandInfo.SystemBytes = message.SystemBytes;
        //        newRemoteCommandInfo.Description = remoteCommandInfo.Description;

        //        newRemoteCommandInfo.DataID = message.Body.Item[0].SubItem[0].Value;
        //        newRemoteCommandInfo.ObjSpec = message.Body.Item[0].SubItem[1].Value;

        //        foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[3].SubItem.Items)
        //        {
        //            if (tempSECSItem.Format == SECSItemFormat.L && tempSECSItem.SubItem.Count == 2)
        //            {
        //                newCommandParameterInfo = new EnhancedCommandParameterInfo()
        //                {
        //                    Name = tempSECSItem.SubItem[0].Value,
        //                    Format = tempSECSItem.SubItem[1].Format
        //                };

        //                if (newCommandParameterInfo.Format == SECSItemFormat.L)
        //                {
        //                    foreach (SECSItem tempSubSECSItem in tempSECSItem.SubItem[1].SubItem.Items)
        //                    {
        //                        if (tempSubSECSItem.Format == SECSItemFormat.L)
        //                        {
        //                            newEnhancedCommandParameterItem = new EnhancedCommandParameterItem()
        //                            {
        //                                Name = tempSubSECSItem.SubItem[0].Value,
        //                                Format = tempSubSECSItem.SubItem[1].Format
        //                            };

        //                            if (newEnhancedCommandParameterItem.Format == SECSItemFormat.L)
        //                            {
        //                                MakeEnhancedRemoteCommandInfoViolationOfStandards(tempSubSECSItem.SubItem[1], ref newEnhancedCommandParameterItem);
        //                            }
        //                            else
        //                            {
        //                                newEnhancedCommandParameterItem.Value = tempSubSECSItem.SubItem[1].Value;
        //                            }

        //                            newCommandParameterInfo.Items.Add(newEnhancedCommandParameterItem);
        //                        }
        //                        else
        //                        {
        //                            newCommandParameterInfo.Items.Add(new EnhancedCommandParameterItem()
        //                            {
        //                                Format = tempSubSECSItem.Format,
        //                                Value = tempSubSECSItem.Value
        //                            });
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    newCommandParameterInfo.Value = tempSECSItem.SubItem[1].Value;
        //                }

        //                newRemoteCommandInfo.EnhancedCommandParameter.Add(newCommandParameterInfo);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        newRemoteCommandInfo = null;

        //        result = GemDriverError.Exception;

        //        this._logger.WriteGEM(ex);
        //    }

        //    return result;
        //}

        //private GemDriverError MakeEnhancedRemoteCommandInfoViolationOfStandards(SECSItem secsItem, ref EnhancedCommandParameterItem newEnhancedCommandParameterItem)
        //{
        //    GemDriverError result;
        //    EnhancedCommandParameterItem newChildParameterItem;

        //    result = GemDriverError.Ok;

        //    try
        //    {
        //        foreach (SECSItem tempSubSECSItem in secsItem.SubItem.Items)
        //        {
        //            if (tempSubSECSItem.SubItem == null || tempSubSECSItem.SubItem.Count <= 0)
        //            {
        //                newChildParameterItem = new EnhancedCommandParameterItem()
        //                {
        //                    Value = tempSubSECSItem.Value,
        //                    Format = tempSubSECSItem.Format
        //                };
        //            }
        //            else
        //            {
        //                newChildParameterItem = new EnhancedCommandParameterItem()
        //                {
        //                    Name = tempSubSECSItem.SubItem[0].Value,
        //                    Format = tempSubSECSItem.SubItem[1].Format
        //                };

        //                if (newChildParameterItem.Format == SECSItemFormat.L)
        //                {
        //                    MakeEnhancedRemoteCommandInfo(tempSubSECSItem.SubItem[1], ref newChildParameterItem);
        //                }
        //                else
        //                {
        //                    newChildParameterItem.Value = tempSubSECSItem.SubItem[1].Value;
        //                }

        //            }

        //            newEnhancedCommandParameterItem.ChildParameterItem.Items.Add(newChildParameterItem);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = GemDriverError.Exception;

        //        this._logger.WriteGEM(ex);
        //    }

        //    return result;
        //}

        private GemDriverError AddEnhancedRemoteCommandParameterResult(SECSBody body, RemoteCommandParameterResult parameterResult)
        {
            GemDriverError result;

            result = GemDriverError.Ok;

            try
            {
                body.Add(SECSItemFormat.L, 2, null);
                body.Add("CPNAME", GetSECSFormat(PreDefinedDataDictinary.CPNAME, SECSItemFormat.A), Encoding.Default.GetByteCount(parameterResult.CPName), parameterResult.CPName);

                if (parameterResult.ParameterListAck.Count > 0)
                {
                    body.Add(SECSItemFormat.L, parameterResult.ParameterListAck.Count, null);

                    foreach (RemoteCommandParameterResult tempChildRemoteCommandParameterResult in parameterResult.ParameterListAck)
                    {
                        result = AddEnhancedRemoteCommandParameterResult(body, tempChildRemoteCommandParameterResult);

                        if (result != GemDriverError.Ok)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    body.Add("CEPACK", GetSECSFormat(PreDefinedDataDictinary.CEPACK, SECSItemFormat.U1), 1, parameterResult.ParameterAck);
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