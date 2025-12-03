using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;
using UbiGEM.Net.Structure.WaferMap;
using UbiGEM.Net.Utility.Logger;

namespace UbiGEM.Net.Driver
{
    partial class GemDriver
    {
        private void Driver_OnSECSConnected(object sender, string ipAddress, int portNo)
        {
            string logText;

            try
            {
                logText = string.Format("SECS Driver Connected:IP Address{0}, Port No={1}", ipAddress, portNo);

                this._logger.WriteGEM(LogLevel.Information, logText);
            }
            catch { }

            new ConnectionStateChangedEventHandler(FireOnConnectionStateChanged).BeginInvoke(ipAddress, portNo, null, null);
        }

        private void Driver_OnSECSDisconnected(object sender, string ipAddress, int portNo)
        {
            string logText;

            try
            {
                logText = string.Format("SECS Driver Disconnected:IP Address{0}, Port No={1}", ipAddress, portNo);

                this._logger.WriteGEM(LogLevel.Information, logText);
            }
            catch { }

            new ConnectionStateChangedEventHandler(FireOnDisconnectionStateChanged).BeginInvoke(ipAddress, portNo, null, null);

            if (this._communicationState != CommunicationState.Disabled &&
                this._communicationState != CommunicationState.NotCommunication)
            {
                this._communicationState = CommunicationState.NotCommunication;

                new CommunicationStateChangedEventHandler(FireOnCommunicationStateChanged).BeginInvoke(this._communicationState, null, null);
            }
        }

        private void Driver_OnSECSDeselected(object sender, string ipAddress, int portNo)
        {
            string logText;

            try
            {
                logText = string.Format("SECS Driver Deselected:IP Address{0}, Port No={1}", ipAddress, portNo);

                this._logger.WriteGEM(LogLevel.Information, logText);
            }
            catch { }

            new ConnectionStateChangedEventHandler(FireOnGEMDeselected).BeginInvoke(ipAddress, portNo, null, null);

            if (this._communicationState != CommunicationState.Disabled &&
                this._communicationState != CommunicationState.NotCommunication)
            {
                this._communicationState = CommunicationState.NotCommunication;

                new CommunicationStateChangedEventHandler(FireOnCommunicationStateChanged).BeginInvoke(this._communicationState, null, null);
            }
        }

        private void Driver_OnSECSSelected(object sender, string ipAddress, int portNo)
        {
            string logText;

            try
            {
                logText = string.Format("SECS Driver Selected:IP Address{0}, Port No={1}", ipAddress, portNo);

                if (this._communicationState != CommunicationState.Disabled)
                {
                    this._communicationState = CommunicationState.WaitCRFromHost;

                    new CommunicationStateChangedEventHandler(FireOnCommunicationStateChanged).BeginInvoke(this._communicationState, null, null);
                }

                this._logger.WriteGEM(LogLevel.Information, logText);
            }
            catch { }

            new ConnectionStateChangedEventHandler(FireOnGEMSelected).BeginInvoke(ipAddress, portNo, null, null);

            VariableInfo variableInfo = this._variableCollection.GetVariableInfo(PreDefinedECV.InitCommunicationState.ToString());
            CommunicationState ecInitCommunicationState = CommunicationState.Disabled;

            if (variableInfo != null)
            {
                if (variableInfo.HasCustomMapping == true)
                {
                    ecInitCommunicationState = variableInfo.GetCustomMapping(variableInfo.Value.ToString(), CommunicationState.Disabled);
                }
                else
                {
                    ecInitCommunicationState = (CommunicationState)(int)variableInfo.Value;
                }
            }

            switch (ecInitCommunicationState)
            {
                case CommunicationState.Communicating:
                    if (this._communicationState != CommunicationState.Communicating)
                    {
                        EstablishCommunication();
                    }
                    break;
                case CommunicationState.Enabled:
                    this._communicationState = CommunicationState.Enabled;
                    break;
            }
        }

        private void Driver_OnReceivedPrimaryMessage(object sender, UbiCom.Net.Structure.SECSMessage message)
        {
            bool exist;
            string logText;

            if ((this._communicationState == CommunicationState.Communicating) ||
                (this._communicationState != CommunicationState.Communicating &&
                 ((message.Stream == 1 && message.Function == 13) ||
                 (message.Stream == 9))))
            {
                exist = this._userGEMMessageCollection.Exist(message.Stream, message.Function, DeviceType.Host);

                if (exist == true)
                {
                    this.OnUserPrimaryMessageReceived?.Invoke(message);
                }
                else
                {
                    exist = this._userMessageCollection.Exist(message.Stream, message.Function, DeviceType.Host);

                    if (exist == true)
                    {
                        this.OnUserPrimaryMessageReceived?.Invoke(message);
                    }
                    else
                    {
                        AnalyzePrimaryMessage(message);
                    }
                }
            }
            else
            {
                logText = string.Format("Wrong communication state:State={0}, S{1}F{2}:{3})", this._communicationState, message.Stream, message.Function, message.Name);

                this._logger.WriteGEM(LogLevel.Information, logText);
            }
        }

        private void Driver_OnReceivedSecondaryMessage(object sender, UbiCom.Net.Structure.SECSMessage primaryMessage, UbiCom.Net.Structure.SECSMessage secondaryMessage)
        {
            bool exist;
            string logText;

            if ((this._communicationState == CommunicationState.Communicating) ||
                (this._communicationState != CommunicationState.Communicating &&
                 (secondaryMessage.Stream == 1 && secondaryMessage.Function == 14)))
            {
                exist = this._userGEMMessageCollection.Exist(primaryMessage.Stream, primaryMessage.Function, DeviceType.Host);

                if (exist == true)
                {
                    this.OnUserSecondaryMessageReceived?.Invoke(primaryMessage, secondaryMessage);
                }
                else
                {
                    exist = this._userMessageCollection.Exist(primaryMessage.Stream, primaryMessage.Function, DeviceType.Host);

                    if (exist == true)
                    {
                        this.OnUserSecondaryMessageReceived?.Invoke(primaryMessage, secondaryMessage);
                    }
                    else
                    {
                        AnalyzeSecondaryMessage(primaryMessage, secondaryMessage);
                    }
                }
            }
            else
            {
                logText = string.Format("Wrong communication state:State={0}, S{1}F{2}:{3})", this._communicationState, secondaryMessage.Stream, secondaryMessage.Function, secondaryMessage.Name);

                this._logger.WriteGEM(LogLevel.Information, logText);
            }
        }

        private void Driver_OnReceivedInvalidPrimaryMessage(object sender, UbiCom.Net.Structure.MessageValidationError reason, UbiCom.Net.Structure.SECSMessage message)
        {
            string logText;

            logText = string.Format("Received Invalid Primary Message:Reason{0}, Message=(S{1}F{2}){3}", reason, message.Stream, message.Function, message.Name);

            this._logger.WriteGEM(LogLevel.Information, logText);

            this.OnInvalidMessageReceived?.Invoke(reason, message);
        }

        private void Driver_OnReceivedInvalidSecondaryMessage(object sender, UbiCom.Net.Structure.MessageValidationError reason, UbiCom.Net.Structure.SECSMessage primaryMessage, UbiCom.Net.Structure.SECSMessage secondaryMessage)
        {
            string logText;

            logText = string.Format("Received Invalid Secondary Message:Reason{0}, Message=(S{1}F{2}){3}", reason, secondaryMessage.Stream, secondaryMessage.Function, secondaryMessage.Name);

            this._logger.WriteGEM(LogLevel.Information, logText);

            this.OnInvalidMessageReceived?.Invoke(reason, secondaryMessage);
        }

        private void Driver_OnReceivedUnknownMessage(object sender, UbiCom.Net.Structure.SECSMessage message)
        {
            string logText;

            logText = string.Format("Received Unknown Message:Message=(S{0}F{1}){2}", message.Stream, message.Function, message.Name);

            this._logger.WriteGEM(LogLevel.Information, logText);

            this.OnReceivedUnknownMessage?.Invoke(message);
        }

        private void Driver_OnT3Timeout(object sender, UbiCom.Net.Structure.SECSMessage message)
        {
            string logText;

            logText = string.Format("T3 Timeout:Message=(S{0}F{1}){2}", message.Stream, message.Function, message.Name);

            this._logger.WriteGEM(LogLevel.Information, logText);
        }

        private void Driver_OnTimeout(object sender, UbiCom.Net.Structure.TimeoutType timeoutType)
        {
            string logText;

            logText = string.Format("Timeout:{0}", timeoutType);

            this._logger.WriteGEM(LogLevel.Information, logText);
        }

        private void Driver_OnSECS1WriteLog(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            new Utility.Logger.LogWriteEventHandler(FireOnSECS1Log).BeginInvoke((LogLevel)logLevel, logText, null, null);
        }

        private void Driver_OnSECS2WriteLog(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            new Utility.Logger.LogWriteEventHandler(FireOnSECS2Log).BeginInvoke((LogLevel)logLevel, logText, null, null);
        }

        private void Logger_OnWriteLog(Utility.Logger.LogLevel logLevel, string logText)
        {
            new Utility.Logger.LogWriteEventHandler(FireOnWriteLog).BeginInvoke(logLevel, logText, null, null);
        }

        private void TimerManager_OnReportTraceData(TraceInfo traceInfo)
        {
            SECSMessage message;
            MessageMakeError messageMakeError;
            MessageError driverResult;
            List<VariableInfo> variables;
            string logText;

            try
            {
                traceInfo.CurrentSample++;

                if (CheckTransmittable("timerManager_OnReportTraceData") == GemDriverError.Ok)
                {
                    variables = traceInfo.Variables;

                    this.OnTraceDataUpdateRequest?.Invoke(variables);

                    traceInfo.AddValue(variables);
                    traceInfo.KeepCount++;

                    if (traceInfo.KeepCount >= traceInfo.ReportGroupSize || traceInfo.CurrentSample >= traceInfo.TotalSample)
                    {
                        if (traceInfo.Values.Count > 0)
                        {
                            messageMakeError = this._messageMaker.MakeTraceReport(this._variableCollection, traceInfo, out message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S6F1):Result={0}", driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, "Transmission successful(S6F1)");
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S6F1):Result={0}", driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                logText = string.Format("Message make failure(S6F1):Result={0}, Error Text={1}", messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }

                            traceInfo.KeepCount = 0;
                            traceInfo.ClearValue();
                        }
                    }

                    variables = null;
                    message = null;
                }

                if (traceInfo.TotalSample <= traceInfo.CurrentSample)
                {
                    logText = string.Format("Trace info removed:TID={0}, TOTSMP={1}, CURSMP={2}", traceInfo.TraceID, traceInfo.TotalSample, traceInfo.CurrentSample);

                    this._traceCollection.Remove(traceInfo);
                    this._timerManager.StopTrace(traceInfo);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }
        }

        private void MessageMaker_OnUserGEMMessageUpdateRequest(SECSMessage message)
        {
            this.OnUserGEMMessageUpdateRequest?.Invoke(message);
        }
        
        private void MessageMaker_OnMessageMakerLogging(string logData)
        {
            this._logger.WriteGEM(LogLevel.Warning, logData);
        }

        private void AnalyzePrimaryMessage(SECSMessage message)
        {
            bool abortTransaction;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            logText = string.Format("Received(S{0}F{1}):Primary message", message.Stream, message.Function);

            this._logger.WriteGEM(LogLevel.Receive, logText);

            if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
            {
                abortTransaction = false;
            }
            else
            {
                if ((message.Stream == 1 && message.Function == 1) ||
                    (message.Stream == 1 && message.Function == 13) ||
                    (message.Stream == 1 && message.Function == 17))
                {
                    abortTransaction = false;
                }
                else
                {
                    abortTransaction = true;
                }
            }

            if (abortTransaction == false)
            {
                if (this._userMessageCollection.Exist(message.Stream, message.Function, DeviceType.Host) == false)
                {
                    switch (message.Stream)
                    {
                        case 1:
                            _ = AnalyzePrimaryMessageStream1(message);
                            break;
                        case 2:
                            _ = AnalyzePrimaryMessageStream2(message);
                            break;
                        //case 3:
                        //    _ = AnalyzePrimaryMessageStream3(message);
                        //    break;
                        //case 4:
                        //    _ = AnalyzePrimaryMessageStream4(message);
                        //    break;
                        case 5:
                            _ = AnalyzePrimaryMessageStream5(message);
                            break;
                        case 6:
                            _ = AnalyzePrimaryMessageStream6(message);
                            break;
                        case 7:
                            _ = AnalyzePrimaryMessageStream7(message);
                            break;
                        //case 8:
                        //    _ = AnalyzePrimaryMessageStream8(message);
                        //    break;
                        case 9:
                            _ = AnalyzePrimaryMessageStream9(message);
                            break;
                        case 10:
                            _ = AnalyzePrimaryMessageStream10(message);
                            break;
                        //case 11:
                        //    _ = AnalyzePrimaryMessageStream11(message);
                        //    break;
                        case 12:
                            _ = AnalyzePrimaryMessageStream12(message);
                            break;
                        //case 13:
                        //    analyzeResult = AnalyzePrimaryMessageStream13(message);
                        //    break;
                        case 14:
                            _ = AnalyzePrimaryMessageStream14(message);
                            break;
                        //case 15:
                        //    _ = AnalyzePrimaryMessageStream15(message);
                        //    break;
                        //case 16:
                        //    _ = AnalyzePrimaryMessageStream16(message);
                        //    break;
                        //case 17:
                        //    _ = AnalyzePrimaryMessageStream17(message);
                        //    break;
                        default:
                            break;
                    }
                }
                else
                {
                    logText = string.Format("User Defined Message(S{0}F{1}:{2})", message.Stream, message.Function, message.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
            }
            else
            {
                if (message.WaitBit == true)
                {
                    replyMessage = this._driver.Messages.GetMessageHeader(message.Stream, 0);

                    if (replyMessage != null)
                    {
                        driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                        if (driverResult == MessageError.Ok)
                        {
                            logText = string.Format("Transmission successful(S{0}F0)", message.Stream);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        else
                        {
                            logText = string.Format("Transmission failure(S{0}F0):Result={1}", message.Stream, driverResult);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        logText = string.Format("Abort transaction failure(S{0}F{1}):Undefined reply message", message.Stream, message.Function);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                }
            }
        }

        private void AnalyzeSecondaryMessage(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            string logText;

            logText = string.Format("Received(S{0}F{1}):Secondary message", secondaryMessage.Stream, secondaryMessage.Function);

            this._logger.WriteGEM(LogLevel.Receive, logText);

            if (this._userMessageCollection.Exist(secondaryMessage.Stream, secondaryMessage.Function, DeviceType.Host) == false)
            {
                switch (secondaryMessage.Stream)
                {
                    case 1:
                        _ = AnalyzeSecondaryMessageStream1(primaryMessage, secondaryMessage);
                        break;
                    case 2:
                        _ = AnalyzeSecondaryMessageStream2(primaryMessage, secondaryMessage);
                        break;
                    //case 3:
                    //    _ = AnalyzeSecondaryMessageStream3(primaryMessage, secondaryMessage);
                    //    break;
                    //case 4:
                    //    _ = AnalyzeSecondaryMessageStream4(primaryMessage, secondaryMessage);
                    //    break;
                    case 5:
                        _ = AnalyzeSecondaryMessageStream5(primaryMessage, secondaryMessage);
                        break;
                    case 6:
                        _ = AnalyzeSecondaryMessageStream6(primaryMessage, secondaryMessage);
                        break;
                    case 7:
                        _ = AnalyzeSecondaryMessageStream7(primaryMessage, secondaryMessage);
                        break;
                    //case 8:
                    //    _ = AnalyzeSecondaryMessageStream8(primaryMessage, secondaryMessage);
                    //    break;
                    case 9:
                        _ = AnalyzeSecondaryMessageStream9(primaryMessage, secondaryMessage);
                        break;
                    case 10:
                        _ = AnalyzeSecondaryMessageStream10(primaryMessage, secondaryMessage);
                        break;
                    //case 11:
                    //    _ = AnalyzeSecondaryMessageStream11(primaryMessage, secondaryMessage);
                    //    break;
                    case 12:
                        _ = AnalyzeSecondaryMessageStream12(primaryMessage, secondaryMessage);
                        break;
                    //case 13:
                    //    analyzeResult = AnalyzeSecondaryMessageStream13(primaryMessage, secondaryMessage);
                    //    break;
                    case 14:
                        _ = AnalyzeSecondaryMessageStream14(primaryMessage, secondaryMessage);
                        break;
                    //case 15:
                    //    _ = AnalyzeSecondaryMessageStream15(primaryMessage, secondaryMessage);
                    //    break;
                    //case 16:
                    //    _ = AnalyzeSecondaryMessageStream16(primaryMessage, secondaryMessage);
                    //    break;
                    //case 17:
                    //    _ = AnalyzeSecondaryMessageStream17(primaryMessage, secondaryMessage);
                    //    break;
                    default:
                        break;
                }
            }
            else
            {
                logText = string.Format("User Defined Message(S{0}F{1}:{2})", secondaryMessage.Stream, secondaryMessage.Function, secondaryMessage.Name);

                this._logger.WriteGEM(LogLevel.Information, logText);

                this.OnUserSecondaryMessageReceived?.Invoke(primaryMessage, secondaryMessage);
            }
        }

        public bool SelfRecive(byte[] receivedData)
        {
            return this._driver.SelfReciveBlock(receivedData);
        }

        private bool bSelfRecvS1F13 = false;
        public bool SelfReciveS1F13()
        {
            bSelfRecvS1F13 = true;
            return this._driver.SelfReciveS1F13();
        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream1(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            MessageMakeError messageMakeError;
            MessageError driverResult;
            string logText;
            string errorText;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S1F1]
                case 1:
                    {
                        VariableInfo variableInfo;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            replyMessage = this._driver.Messages.GetMessageHeader(1, 2);
                            replyMessage.Function = 2;

                            replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                            variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.MDLN.ToString());
                            replyMessage.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));

                            variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.SOFTREV.ToString());
                            replyMessage.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S1F3]
                case 3:
                    {
                        List<VariableInfo> variables;
                        VariableInfo variableInfo;
                        string vid;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            variables = new List<VariableInfo>();

                            if (message.Body.Item[0].SubItem.Count > 0)
                            {
                                foreach (SECSItem tempVid in message.Body.Item[0].SubItem.Items)
                                {
                                    if (this._vidFormat == SECSItemFormat.A)
                                    {
                                        vid = tempVid.Value;
                                    }
                                    else
                                    {
                                        vid = ((long)tempVid.Value).ToString();
                                    }

                                    variableInfo = this._variableCollection[vid];

                                    if (variableInfo == null)
                                    {
                                        variables.Add(new VariableInfo()
                                        {
                                            Format = SECSItemFormat.L,
                                            Length = 0,
                                            Description = DESCRIPTION_UNKNOWN_VID
                                        });
                                    }
                                    else
                                    {
                                        variables.Add(variableInfo);
                                    }
                                }
                            }
                            else
                            {
                                variables = this._variableCollection.SV.Items;
                            }

                            if (result == AnalyzeMessageError.Ok)
                            {
                                this.OnVariableUpdateRequest?.Invoke(VariableUpdateType.S1F3SelectedEquipmentStatusRequest, variables, string.Empty);

                                messageMakeError = this._messageMaker.MakeS1F4(this._variableCollection, variables, out replyMessage, out errorText);
                            }
                            else
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(1, 4);
                                replyMessage.Body.Add("SVCOUNT", SECSItemFormat.L, 0, null);

                                errorText = string.Empty;
                                messageMakeError = MessageMakeError.Ok;
                            }

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S1F{0}:{1} [{2}])", replyMessage.Function, replyMessage.Name, message.SystemBytes);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                logText = string.Format("Message make failure(S1F{0}:{1}):Result={2}, Error Text={3}", message.Function, message.Name, result, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S1F11]
                case 11:
                    {
                        VariableCollection variableCollection;
                        VariableInfo variableInfo;
                        string vid;
                        SECSItemFormat secsItemFormat;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            variableCollection = new VariableCollection();

                            if (message.Body.Item[0].SubItem.Count > 0)
                            {
                                foreach (SECSItem tempVid in message.Body.Item[0].SubItem.Items)
                                {
                                    if (this._vidFormat == SECSItemFormat.A)
                                    {
                                        vid = tempVid.Value;
                                    }
                                    else
                                    {
                                        vid = ((long)tempVid.Value).ToString();
                                    }

                                    variableInfo = this._variableCollection[vid];

                                    if (variableInfo == null)
                                    {
                                        variableCollection.Add(new VariableInfo()
                                        {
                                            VID = vid,
                                            Format = SECSItemFormat.L,
                                            Length = 0,
                                            Description = DESCRIPTION_UNKNOWN_VID
                                        });
                                    }
                                    else
                                    {
                                        variableCollection.Add(variableInfo);
                                    }
                                }
                            }
                            else
                            {
                                variableCollection = this._variableCollection.SV;
                            }

                            replyMessage = this._driver.Messages.GetMessageHeader(1, 12);

                            if (result == AnalyzeMessageError.Ok)
                            {
                                secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.VID, SECSItemFormat.U2);

                                replyMessage.Body.Add("SVIDCOUNT", SECSItemFormat.L, variableCollection.Items.Count, null);

                                foreach (VariableInfo tempVariableInfo in variableCollection.Items)
                                {
                                    if (tempVariableInfo.Format == SECSItemFormat.L &&
                                        tempVariableInfo.Length == 0 &&
                                        tempVariableInfo.Description == DESCRIPTION_UNKNOWN_VID)
                                    {
                                        replyMessage.Body.Add(SECSItemFormat.L, 3, null);

                                        if (secsItemFormat == SECSItemFormat.A || secsItemFormat == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("SVID", secsItemFormat, Encoding.Default.GetByteCount(tempVariableInfo.VID), tempVariableInfo.VID);
                                        }
                                        else
                                        {
                                            replyMessage.Body.Add("SVID", secsItemFormat, 1, tempVariableInfo.VID);
                                        }

                                        replyMessage.Body.Add("SVNAME", SECSItemFormat.A, 0, string.Empty);
                                        replyMessage.Body.Add("UNITS", SECSItemFormat.A, 0, string.Empty);
                                    }
                                    else
                                    {
                                        replyMessage.Body.Add(SECSItemFormat.L, 3, null);

                                        if (secsItemFormat == SECSItemFormat.A || secsItemFormat == SECSItemFormat.J)
                                        {
                                            replyMessage.Body.Add("SVID", secsItemFormat, Encoding.Default.GetByteCount(tempVariableInfo.VID), tempVariableInfo.VID);
                                        }
                                        else
                                        {
                                            replyMessage.Body.Add("SVID", secsItemFormat, 1, tempVariableInfo.VID);
                                        }

                                        replyMessage.Body.Add("SVNAME", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Name), tempVariableInfo.Name);
                                        replyMessage.Body.Add("UNITS", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Units.ToString()), tempVariableInfo.Units.ToString());
                                    }
                                }
                            }
                            else
                            {
                                replyMessage.Body.Add("SVIDCOUNT", SECSItemFormat.L, 0, null);
                            }

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S1F13]
                case 13:
                    {
                        VariableInfo variableInfo;
                        int commAck;

                        try
                        { 
                            this._timerCommDelay.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                            logText = string.Format("Communication State Changed:S1F13, State={0}->{1}", this._communicationState, CommunicationState.Communicating);

                            this._communicationState = CommunicationState.Communicating;

                            this._logger.WriteGEM(LogLevel.Information, logText); 

                            if (this.OnReceivedEstablishCommunicationsRequest != null)
                            {
                                if (message.Body != null && message.Body.Item.Count == 1 && message.Body.Item[0].Format == SECSItemFormat.L && message.Body.Item[0].SubItem.Count == 2)
                                {
                                    commAck = this.OnReceivedEstablishCommunicationsRequest(message.Body.Item[0].SubItem[0].Value, message.Body.Item[0].SubItem[1].Value);
                                }
                                else
                                {
                                    commAck = this.OnReceivedEstablishCommunicationsRequest(string.Empty, string.Empty);
                                }
                            }
                            else
                            {
                                commAck = (int)COMMACK.Accepted;
                            }

                            if( true == bSelfRecvS1F13 )
                            {
                                bSelfRecvS1F13 = false;
                            }
                            else
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(1, 14);

                                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                                replyMessage.Body.Add("COMMACK", GetSECSFormat(PreDefinedDataDictinary.COMMACK, SECSItemFormat.B), 1, commAck);

                                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.MDLN.ToString());
                                if (variableInfo != null)
                                {
                                    replyMessage.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                                }
                                else
                                {
                                    replyMessage.Body.Add(new SECSItem(PreDefinedV.MDLN.ToString(), SECSItemFormat.A, 0, string.Empty));
                                }

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.SOFTREV.ToString());
                                if (variableInfo != null)
                                {
                                    replyMessage.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                                }
                                else
                                {
                                    replyMessage.Body.Add(new SECSItem(PreDefinedV.SOFTREV.ToString(), SECSItemFormat.A, 0, string.Empty));
                                }

                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }

                            this.OnCommunicationStateChanged?.Invoke(this._communicationState);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S1F15]
                case 15:
                    {
                        try
                        {
                            if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
                            {
                                this.OnReceivedRequestOffline?.Invoke(message.SystemBytes);
                            }
                            else
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(1, 0);

                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S1F17]
                case 17:
                    {
                        int processResult;

                        try
                        {
                            replyMessage = this._driver.Messages.GetMessageHeader(1, 18);

                            if (this._controlState == ControlState.HostOffline)
                            {
                                this.OnReceivedRequestOnline?.Invoke(message.SystemBytes);
                            }
                            else if (this._controlState == ControlState.EquipmentOffline)
                            {
                                if (this._configFileManager.GEMConfiguration.ExtensionOption.UseS1F17InEQPOffline == true)
                                {
                                    this.OnReceivedRequestOnline?.Invoke(message.SystemBytes);
                                }
                                else
                                {
                                    processResult = (int)ONLACK.OnlineNotAllowed;
                                    replyMessage.Body.Add("ONLACK", GetSECSFormat(PreDefinedDataDictinary.ONLACK, SECSItemFormat.B), 1, processResult);

                                    driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        logText = string.Format("Transmission successful(S1F{0}:{1}):ONLACK={2}", replyMessage.Function, replyMessage.Name, processResult);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                            }
                            else if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
                            {
                                processResult = (int)ONLACK.EquipmentAlreadyOnline;
                                replyMessage.Body.Add("ONLACK", GetSECSFormat(PreDefinedDataDictinary.ONLACK, SECSItemFormat.B), 1, processResult);

                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S1F{0}:{1}):ONLACK={2}", replyMessage.Function, replyMessage.Name, processResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                processResult = (int)ONLACK.OnlineNotAllowed;
                                replyMessage.Body.Add("ONLACK", GetSECSFormat(PreDefinedDataDictinary.ONLACK, SECSItemFormat.B), 1, processResult);

                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S1F{0}:{1}):ONLACK={2}", replyMessage.Function, replyMessage.Name, processResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S1F21]
                case 21:
                    {
                        try
                        {
                            if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
                            {
                                result = AnalyzeMessageError.Ok;

                                messageMakeError = this._messageMaker.MakeS1F22(this._variableCollection, message, out replyMessage, out errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    logText = string.Format("Message make failure(S1F22):Result={0}, Error Text={1}", messageMakeError, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(1, 0);

                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S1F23]
                case 23:
                    {
                        try
                        {
                            if (this._controlState == ControlState.OnlineLocal || this._controlState == ControlState.OnlineRemote)
                            {
                                result = AnalyzeMessageError.Ok;

                                messageMakeError = this._messageMaker.MakeS1F24(this._collectionEventCollection, message, out replyMessage, out errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    logText = string.Format("Message make failure(S1F24):Result={0}, Error Text={1}", result, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(1, 0);

                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    logText = string.Format("Transmission successful(S1F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream2(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            //SECSMessage relationMessage;
            MessageMakeError messageMakeError;
            MessageError driverResult;
            string logText;
            string errorText;

            result = AnalyzeMessageError.Ok;
            //_traceCollection
            switch (message.Function)
            {
                #region [S2F13]
                case 13:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            messageMakeError = this._messageMaker.MakeS2F14(this._variableCollection, message, out replyMessage, out errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
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
                            else
                            {
                                logText = string.Format("Message make failure(S2F14):Result={0}, Error Text={1}", result, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }
                    break;
                #endregion
                #region [S2F15]
                case 15:
                    {
                        VariableCollection newVariableCollection;
                        VariableInfo variableInfo;
                        VariableInfo newVariableInfo;
                        string ecid;
                        int processResult;
                        bool isAsync;

                        try
                        {
                            isAsync = false;
                            result = AnalyzeMessageError.Ok;
                            processResult = (int)EAC.Acknowledge;
                            newVariableCollection = new VariableCollection();

                            foreach (SECSItem tempEcid in message.Body.Item[0].SubItem.Items)
                            {
                                if (this._vidFormat == SECSItemFormat.A)
                                {
                                    ecid = tempEcid.SubItem[0].Value;
                                }
                                else
                                {
                                    ecid = ((long)tempEcid.SubItem[0].Value).ToString();
                                }

                                variableInfo = this._variableCollection.ECV[ecid];

                                if (variableInfo != null)
                                {
                                    newVariableInfo = variableInfo.CopyTo();

                                    newVariableInfo.Value.SetValue(tempEcid.SubItem[1].Value.GetValue());

                                    newVariableCollection.Add(newVariableInfo);
                                }
                                else
                                {
                                    processResult = (int)EAC.Denied_DoesNotExist;

                                    break;
                                }
                            }

                            if (processResult == (int)EAC.Acknowledge)
                            {
                                if (newVariableCollection.ECV.Items.Count > 0)
                                {
                                    GemDriverError saveResult;

                                    saveResult = CheckECVChangedByDriverValue(newVariableCollection.ECV.Items);

                                    if (saveResult == GemDriverError.Ok)
                                    {
                                        if (this.OnReceivedNewECVSend != null)
                                        {
                                            this.OnReceivedNewECVSend(message.SystemBytes, newVariableCollection);

                                            isAsync = true;
                                        }
                                    }
                                    else
                                    {
                                        processResult = (int)EAC.Denied_Etc;
                                    }
                                }
                            }

                            if (isAsync == false)
                            {
                                ReplyNewEquipmentConstantSend(message.SystemBytes, newVariableCollection, processResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }
                    break;
                #endregion
                #region [S2F17]
                case 17:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            this.OnReceivedDateTimeRequest?.Invoke(message.SystemBytes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }
                    break;
                #endregion
                #region [S2F23]
                case 23:
                    {
                        TraceInfo traceInfo;
                        SECSItem vidSecsInfo;
                        VariableInfo variableInfo;
                        string tid;
                        long totalSample;
                        string vid;
                        int ack;

                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            ack = (int)TIAACK.EverythingCorrect;

                            tid = message.Body.Item[0].SubItem[0].Value;

                            totalSample = message.Body.Item[0].SubItem[2].Value;

                            if (totalSample <= 0)
                            {
                                traceInfo = this._traceCollection[tid];

                                if (traceInfo != null)
                                {
                                    this._timerManager.StopTrace(traceInfo);
                                    this._traceCollection.Remove(traceInfo);

                                    traceInfo.TotalSample = 0;
                                }
                            }
                            else
                            {
                                traceInfo = this._traceCollection[tid];

                                if (traceInfo != null)
                                {
                                    this._timerManager.StopTrace(traceInfo);
                                    this._traceCollection.Remove(traceInfo);
                                }

                                traceInfo = new TraceInfo()
                                {
                                    TraceID = message.Body.Item[0].SubItem[0].Value,
                                    Dsper = message.Body.Item[0].SubItem[1].Value,
                                    TotalSample = message.Body.Item[0].SubItem[2].Value,
                                    ReportGroupSize = message.Body.Item[0].SubItem[3].Value
                                };

                                if (traceInfo.TotalMillisecond <= 0)
                                {
                                    result = AnalyzeMessageError.Unknown;
                                    ack = (int)TIAACK.InvalidPeriod;
                                }
                                else if (traceInfo.ReportGroupSize <= 0)
                                {
                                    result = AnalyzeMessageError.Unknown;
                                    ack = (int)TIAACK.InvalidREPGSZ;
                                }
                                else
                                {
                                    vidSecsInfo = message.Body.Item[0].SubItem[4];

                                    foreach (SECSItem tempVid in vidSecsInfo.SubItem.Items)
                                    {
                                        if (this._vidFormat == SECSItemFormat.A)
                                        {
                                            vid = tempVid.Value;
                                        }
                                        else
                                        {
                                            vid = ((long)tempVid.Value).ToString();
                                        }

                                        variableInfo = this._variableCollection[vid];

                                        if (variableInfo != null)
                                        {
                                            traceInfo.Variables.Add(variableInfo);
                                        }
                                        else
                                        {
                                            this._logger.WriteGEM(LogLevel.Warning, string.Format("Unknown SVID specified:{0}", vid));

                                            result = AnalyzeMessageError.Undefined;
                                            ack = (int)TIAACK.UnknownSVIDSpecified;
                                            break;
                                        }
                                    }
                                }
                            }

                            replyMessage = this._driver.Messages.GetMessageHeader(2, 24);

                            replyMessage.Body.Add("TIAACK", GetSECSFormat(PreDefinedDataDictinary.TIAACK, SECSItemFormat.B), 1, ack);

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S2F{0}:{1}):TIAACK={2}", replyMessage.Function, replyMessage.Name, ack);

                                this._logger.WriteGEM(LogLevel.Information, logText);

                                if (result == AnalyzeMessageError.Ok && traceInfo != null && ack == (int)TIAACK.EverythingCorrect)
                                {
                                    if (totalSample > 0)
                                    {
                                        this._traceCollection.Add(traceInfo);
                                        this._timerManager.StartTrace(traceInfo);

                                        logText = string.Format("Append trace information:[{0}]", traceInfo);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                }
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S2F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }
                    break;
                #endregion
                #region [S2F25]
                case 25:
                    {
                        byte[] abs;
                        StringBuilder sb;

                        try
                        {
                            sb = new StringBuilder(200);
                            result = AnalyzeMessageError.Ok;

                            if (message.Body.Item[0].Value.Length == 1)
                            {
                                abs = new byte[1];

                                abs[0] = message.Body.Item[0].Value;
                            }
                            else if (message.Body.Item[0].Value.Length == 0)
                            {
                                abs = new byte[0];
                            }
                            else
                            {
                                abs = message.Body.Item[0].Value;
                            }

                            this.OnReceivedLoopback?.Invoke(abs.ToList());

                            messageMakeError = this._messageMaker.MakeS2F26(abs, out replyMessage, out errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                if (driverResult == MessageError.Ok)
                                {
                                    sb.AppendFormat("Transmission successful(S2F{0}:{1}):", replyMessage.Function, replyMessage.Name);

                                    foreach (byte tempAbs in abs)
                                    {
                                        sb.AppendFormat("{0} ", tempAbs);
                                    }

                                    logText = sb.ToString();

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    logText = string.Format("Transmission failure(S2F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                logText = string.Format("Message make failure(S2F{0}:{1}), Result={2}, Error Text={3}", replyMessage.Function, replyMessage.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            abs = null;
                            sb = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F29]
                case 29:
                    {
                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            messageMakeError = this._messageMaker.MakeS2F30(this._variableCollection, message, out replyMessage, out errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
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
                            else
                            {
                                logText = string.Format("Message make failure(S2F30):Result={0}, Error Text={1}", result, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F31]
                case 31:
                    {
                        DateTime? timeData;
                        string timeString;

                        try
                        {
                            timeString = message.Body.Item[0].Value;

                            if (timeString.Length == 16)
                            {
                                timeData = new DateTime(int.Parse(timeString.Substring(0, 4)),
                                                        int.Parse(timeString.Substring(4, 2)),
                                                        int.Parse(timeString.Substring(6, 2)),
                                                        int.Parse(timeString.Substring(8, 2)),
                                                        int.Parse(timeString.Substring(10, 2)),
                                                        int.Parse(timeString.Substring(12, 2)),
                                                        int.Parse(timeString.Substring(14, 2)) * 10);
                            }
                            else if (timeString.Length == 12)
                            {
                                timeData = new DateTime(int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + timeString.Substring(0, 2)),
                                                        int.Parse(timeString.Substring(2, 2)),
                                                        int.Parse(timeString.Substring(4, 2)),
                                                        int.Parse(timeString.Substring(6, 2)),
                                                        int.Parse(timeString.Substring(8, 2)),
                                                        int.Parse(timeString.Substring(10, 2)));
                            }
                            else
                            {
                                timeData = null;
                            }

                            if (timeData != null)
                            {
                                this.OnReceivedDateTimeSetRequest?.Invoke(message.SystemBytes, timeData.GetValueOrDefault());
                            }
                            else
                            {
                                this._logger.WriteGEM(LogLevel.Warning, $"Data conversion error(S2F31):Time String Length={timeString.Length}");

                                ReplyDateTimeSetRequest(message.SystemBytes, (int)TIACK.Error, DateTime.Now);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            timeData = null;
                            replyMessage = null;
                        }
                    }

                    break;
                #endregion
                #region [S2F33]
                case 33:
                    {
                        SECSItem reportSecsInfo;
                        SECSItem vidSecsInfo;
                        ReportCollection reportCollection;
                        ReportInfo reportInfo;
                        VariableInfo variableInfo;
                        string reportId;
                        string vid;
                        int ack;

                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            ack = (int)DRACK.Accept;

                            reportCollection = new ReportCollection();

                            try
                            {
                                reportSecsInfo = message.Body.Item[0].SubItem[1];

                                foreach (SECSItem tempReport in reportSecsInfo.SubItem.Items)
                                {
                                    if (this._rptidFormat == SECSItemFormat.A)
                                    {
                                        reportId = tempReport.SubItem[0].Value;
                                    }
                                    else
                                    {
                                        reportId = ((long)tempReport.SubItem[0].Value).ToString();
                                    }

                                    if (reportCollection.Exist(reportId) == false)
                                    {
                                        reportInfo = new ReportInfo()
                                        {
                                            ReportID = reportId
                                        };

                                        vidSecsInfo = tempReport.SubItem[1];

                                        foreach (SECSItem tempVid in vidSecsInfo.SubItem.Items)
                                        {
                                            if (this._vidFormat == SECSItemFormat.A)
                                            {
                                                vid = tempVid.Value;
                                            }
                                            else
                                            {
                                                vid = ((long)tempVid.Value).ToString();
                                            }

                                            variableInfo = this._variableCollection[vid];

                                            if (variableInfo != null)
                                            {
                                                reportInfo.Variables.Add(variableInfo);
                                            }
                                            else
                                            {
                                                this._logger.WriteGEM(LogLevel.Warning, string.Format("VID does not exist:{0}", vid));

                                                ack = (int)DRACK.Denied_AtLeastVIDDoesNotExist;
                                            }
                                        }

                                        if (ack == (int)DRACK.Accept)
                                        {
                                            reportCollection.Add(reportInfo);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        ack = (int)DRACK.Denied_AtLeastOneRPTIDAlreadyDefined;

                                        this._logger.WriteGEM(LogLevel.Warning, string.Format("RPTID does not exist:{0}", reportId));

                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ack = (int)DRACK.Denied_InvalidFormat;
                                result = AnalyzeMessageError.Exception;

                                this._logger.WriteGEM(ex);
                            }

                            replyMessage = this._driver.Messages.GetMessageHeader(2, 34);

                            replyMessage.Body.Add("DRACK", GetSECSFormat(PreDefinedDataDictinary.DRACK, SECSItemFormat.B), 1, ack);

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S2F{0}:{1}):DRACK={2}", replyMessage.Function, replyMessage.Name, ack);

                                this._logger.WriteGEM(LogLevel.Information, logText);

                                logText = string.Format("Clear report information:[{0}]", this._reportCollection);

                                this._logger.WriteGEM(LogLevel.Information, logText);

                                if (reportCollection.Items.Count > 0)
                                {
                                    foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                                    {
                                        reportInfo = this._reportCollection[tempReportInfo.ReportID];

                                        if (reportInfo != null)
                                        {
                                            reportInfo.Variables = tempReportInfo.Variables;
                                        }
                                        else
                                        {
                                            this._reportCollection.Add(tempReportInfo);
                                        }
                                    }
                                }
                                else
                                {
                                    this._reportCollection.Items.Clear();
                                }

                                this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.Reports, false, out errorText);

                                this.OnReceivedDefineReport?.Invoke();

                                if (string.IsNullOrEmpty(errorText) == false)
                                {
                                    logText = string.Format("Define report save failed(S2F{0}:{1}):Error={2}", message.Function, message.Name, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S2F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            reportSecsInfo = null;
                            vidSecsInfo = null;
                            reportSecsInfo = null;
                            reportInfo = null;
                            variableInfo = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F35]
                case 35:
                    {
                        SECSItem eventSecsInfo;
                        SECSItem reportSecsInfo;
                        CollectionEventCollection collectionEventCollection;
                        CollectionEventInfo collectionEventInfo;
                        CollectionEventInfo newCollectionEventInfo;
                        ReportInfo reportInfo;
                        string ceId;
                        string reportId;
                        int ack;

                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            ack = (int)LRACK.Accept;

                            collectionEventCollection = new CollectionEventCollection();

                            try
                            {
                                eventSecsInfo = message.Body.Item[0].SubItem[1];

                                foreach (SECSItem tempEvent in eventSecsInfo.SubItem.Items)
                                {
                                    if (this._ceidFormat == SECSItemFormat.A)
                                    {
                                        ceId = tempEvent.SubItem[0].Value;
                                    }
                                    else
                                    {
                                        ceId = ((long)tempEvent.SubItem[0].Value).ToString();
                                    }

                                    collectionEventInfo = this._collectionEventCollection[ceId];

                                    if (collectionEventInfo != null)
                                    {
                                        newCollectionEventInfo = new CollectionEventInfo()
                                        {
                                            CEID = collectionEventInfo.CEID,
                                            Name = collectionEventInfo.Name,
                                            PreDefined = collectionEventInfo.PreDefined,
                                            IsUse = collectionEventInfo.IsUse,
                                            Enabled = collectionEventInfo.Enabled,
                                            Description = collectionEventInfo.Description
                                        };

                                        reportSecsInfo = tempEvent.SubItem[1];

                                        foreach (SECSItem tempReport in reportSecsInfo.SubItem.Items)
                                        {
                                            if (this._rptidFormat == SECSItemFormat.A)
                                            {
                                                reportId = tempReport.Value;
                                            }
                                            else
                                            {
                                                reportId = ((long)tempReport.Value).ToString();
                                            }

                                            reportInfo = this._reportCollection[reportId];

                                            if (reportInfo != null)
                                            {
                                                newCollectionEventInfo.Reports.Add(reportInfo);
                                            }
                                            else
                                            {
                                                result = AnalyzeMessageError.Unknown;
                                                ack = (int)LRACK.Denied_AtLeastRPTIDDoesNotExist;
                                                this._logger.WriteGEM(LogLevel.Warning, string.Format("RPTID does not exist:{0}", reportId));
                                            }
                                        }

                                        if (result == AnalyzeMessageError.Ok)
                                        {
                                            collectionEventCollection.Add(newCollectionEventInfo);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        result = AnalyzeMessageError.Unknown;
                                        ack = (int)LRACK.Denied_AtLeastCEIDDoesNotExist;

                                        this._logger.WriteGEM(LogLevel.Warning, string.Format("CEID does not exist:{0}", ceId));

                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ack = (int)LRACK.Denied_InvalidFormat;
                                result = AnalyzeMessageError.Exception;

                                this._logger.WriteGEM(ex);
                            }

                            replyMessage = this._driver.Messages.GetMessageHeader(2, 36);

                            replyMessage.Body.Add("LRACK", GetSECSFormat(PreDefinedDataDictinary.LRACK, SECSItemFormat.B), 1, ack);

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S2F{0}:{1}):LRACK={2}", replyMessage.Function, replyMessage.Name, ack);

                                this._logger.WriteGEM(LogLevel.Information, logText);

                                if (ack == (int)LRACK.Accept)
                                {
                                    bool isChanged = false;

                                    foreach (KeyValuePair<string, CollectionEventInfo> tempCollectionEventInfo in this._collectionEventCollection.Items)
                                    {
                                        collectionEventInfo = collectionEventCollection[tempCollectionEventInfo.Key];

                                        if (collectionEventInfo != null)
                                        {
                                            tempCollectionEventInfo.Value.Reports = collectionEventInfo.Reports;

                                            isChanged = true;
                                        }
                                    }

                                    if (isChanged == true)
                                    {
                                        this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.CollectionEvents, false, out errorText);

                                        if (string.IsNullOrEmpty(errorText) == false)
                                        {
                                            logText = string.Format("Link event report save failed(S2F{0}:{1}):Error={2}", message.Function, message.Name, errorText);

                                            this._logger.WriteGEM(LogLevel.Error, logText);
                                        }
                                    }

                                    this.OnReceivedLinkEventReport?.Invoke();
                                }
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S2F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            eventSecsInfo = null;
                            reportSecsInfo = null;
                            collectionEventCollection = null;
                            collectionEventInfo = null;
                            newCollectionEventInfo = null;
                            reportInfo = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F37]
                case 37:
                    {
                        bool enable;
                        string ceid;
                        List<string> ceids;
                        int ack;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            enable = message.Body.Item[0].SubItem[0].Value;

                            ack = (int)ERACK.Accept;
                            ceids = new List<string>();

                            foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[1].SubItem.Items)
                            {
                                if (this._ceidFormat == SECSItemFormat.A)
                                {
                                    ceid = tempSECSItem.Value;
                                }
                                else
                                {
                                    ceid = ((long)tempSECSItem.Value).ToString();
                                }

                                ceids.Add(ceid);

                                if (this._collectionEventCollection.Exist(ceid) == false)
                                {
                                    this._logger.WriteGEM(LogLevel.Warning, string.Format("CEID does not exist:{0}", ceid));

                                    ack = (int)ERACK.Denied_AtLeastCEIDDoesNotExist;
                                }
                            }

                            replyMessage = this._driver.Messages.GetMessageHeader(2, 38);

                            replyMessage.Body.Add("ERACK", GetSECSFormat(PreDefinedDataDictinary.ERACK, SECSItemFormat.B), 1, ack);

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S2F{0}:{1}):ERACK={2}", replyMessage.Function, replyMessage.Name, ack);

                                this._logger.WriteGEM(LogLevel.Information, logText);

                                if (ack == (int)ERACK.Accept)
                                {
                                    if (ceids.Count > 0)
                                    {
                                        foreach (string tempCeid in ceids)
                                        {
                                            this._collectionEventCollection[tempCeid].Enabled = enable;
                                        }
                                    }
                                    else
                                    {
                                        foreach (CollectionEventInfo tempCollectionEventInfo in this._collectionEventCollection.Items.Values)
                                        {
                                            tempCollectionEventInfo.Enabled = enable;
                                        }
                                    }

                                    try
                                    {
                                        this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.CollectionEvents, enable, out errorText);
                                    }
                                    catch
                                    {
                                        errorText = string.Empty;
                                    }

                                    this.OnReceivedEnableDisableEventReport?.Invoke();

                                    if (string.IsNullOrEmpty(errorText) == false)
                                    {
                                        logText = string.Format("Enable/Disable event report save failed(S2F{0}:{1}):Error={2}", message.Function, message.Name, errorText);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S2F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            ceids = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F41]
                case 41:
                    {
                        RemoteCommandInfo remoteCommandInfo;
                        CommandParameterInfo commandParameterInfo;
                        Dictionary<string, int> cpAck;
                        RemoteCommandResult remoteCommandResult;
                        bool isInvalid;

                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            isInvalid = false;

                            cpAck = new Dictionary<string, int>();

                            remoteCommandInfo = this._remoteCommandCollection[message.Body.Item[0].SubItem[0].Value];

                            if (remoteCommandInfo != null)
                            {
                                remoteCommandInfo = remoteCommandInfo.CopyTo();

                                remoteCommandInfo.SystemBytes = message.SystemBytes;

                                List<string> configNames = remoteCommandInfo.CommandParameter.Items.Select(t => t.Name).ToList();
                                List<string> rcvNames = new List<string>();

                                foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[1].SubItem.Items)
                                {
                                    rcvNames.Add(tempSECSItem.SubItem[0].Value);

                                    if( "pass" == remoteCommandInfo.Description )
                                    {
                                        CommandParameterInfo TempCommandParameterInfo = new CommandParameterInfo();
                                        TempCommandParameterInfo.Name = tempSECSItem.SubItem[0].Value;
                                        TempCommandParameterInfo.Value = tempSECSItem.SubItem[1].Value;
                                        remoteCommandInfo.CommandParameter.Add(TempCommandParameterInfo);
                                        continue;
                                    }

                                    commandParameterInfo = remoteCommandInfo.CommandParameter[tempSECSItem.SubItem[0].Value];

                                    if (commandParameterInfo != null)
                                    {
                                        if (commandParameterInfo.Format == tempSECSItem.SubItem[1].Format)
                                        {
                                            commandParameterInfo.Value = tempSECSItem.SubItem[1].Value;
                                        }
                                        else
                                        {
                                            cpAck[tempSECSItem.SubItem[0].Value] = (int)CPACK.IllegalFormatSpecifiedForCPVAL;
                                        }
                                    }
                                    else
                                    {
                                        cpAck[tempSECSItem.SubItem[0].Value] = (int)CPACK.ParameterNameDoesNotExist;
                                    }
                                }

                                foreach (string tempCPName in configNames)
                                {
                                    if (rcvNames.Exists(t => t == tempCPName) == false)
                                    {
                                        cpAck[tempCPName] = (int)CPACK.ParameterNameDoesNotExist;
                                    }
                                }

                                if (cpAck.Count <= 0 || "pass" == remoteCommandInfo.Description)
                                {
                                    if (this.OnReceivedRemoteCommand != null)
                                    {
                                        this.OnReceivedRemoteCommand(remoteCommandInfo);

                                        remoteCommandResult = null;
                                    }
                                    else
                                    {
                                        remoteCommandResult = new RemoteCommandResult()
                                        {
                                            HostCommandAck = (int)HCACK.Acknowledge
                                        };
                                    }
                                }
                                else
                                {
                                    RaiseInvalidRemoteCommand(message);
                                    isInvalid = true;

                                    remoteCommandResult = new RemoteCommandResult()
                                    {
                                        HostCommandAck = (int)HCACK.ParameterIsInvalid
                                    };
                                }
                            }
                            else
                            {
                                RaiseInvalidRemoteCommand(message);
                                isInvalid = true;

                                remoteCommandInfo = new RemoteCommandInfo()
                                {
                                    RemoteCommand = message.Body.Item[0].SubItem[0].Value,
                                    SystemBytes = message.SystemBytes
                                };

                                remoteCommandResult = new RemoteCommandResult()
                                {
                                    HostCommandAck = (int)HCACK.CommandDoesNotExist
                                };

                                result = AnalyzeMessageError.Undefined;
                            }

                            if (isInvalid == true)
                            {
                                ReplyRemoteCommandAck(remoteCommandInfo, remoteCommandResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            remoteCommandInfo = null;
                            commandParameterInfo = null;
                            cpAck = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F43]
                case 43:
                    {
                        const int SPOOLING_NOT_ALLOWED = 1;
                        const int UNKNOWN_STREAM = 2;
                        const int UNKNOWN_FUNCTION = 3;

                        int stream;
                        int function;
                        SECSMessageCollection secsMessageCollection;
                        //         Stream,         Ack, Function
                        Dictionary<int, Dictionary<int, List<int>>> spoolingAck;
                        int count;
                        SECSItemFormat stridItemFormat;
                        SECSItemFormat strackItemFormat;
                        SECSItemFormat fcnidItemFormat;

                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            spoolingAck = new Dictionary<int, Dictionary<int, List<int>>>();

                            if (message.Body.Item[0].Length <= 0)
                            {
                                this._spoolingCollection.Remove();
                            }
                            else
                            {
                                this._spoolingCollection.Remove();

                                foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem.Items)
                                {
                                    stream = tempSECSItem.SubItem[0].Value;

                                    if (tempSECSItem.SubItem[1].Length <= 0)
                                    {
                                        this._spoolingCollection.Add(stream);
                                    }
                                    else
                                    {
                                        foreach (SECSItem tempItem in tempSECSItem.SubItem[1].SubItem.Items)
                                        {
                                            function = tempItem.Value;

                                            if (stream == 1)
                                            {
                                                if (spoolingAck.ContainsKey(stream) == false)
                                                {
                                                    spoolingAck[stream] = new Dictionary<int, List<int>>();
                                                }

                                                if (spoolingAck[stream].ContainsKey(SPOOLING_NOT_ALLOWED) == false)
                                                {
                                                    spoolingAck[stream][SPOOLING_NOT_ALLOWED] = new List<int>();
                                                }

                                                spoolingAck[stream][SPOOLING_NOT_ALLOWED].Add(function);
                                            }
                                            else
                                            {
                                                count = this._driver.Messages.MessageInfo.Count(t => t.Value.Stream == stream);

                                                if (count <= 0)
                                                {
                                                    if (spoolingAck.ContainsKey(stream) == false)
                                                    {
                                                        spoolingAck[stream] = new Dictionary<int, List<int>>();
                                                    }

                                                    if (spoolingAck[stream].ContainsKey(UNKNOWN_STREAM) == false)
                                                    {
                                                        spoolingAck[stream][UNKNOWN_STREAM] = new List<int>();
                                                    }

                                                    spoolingAck[stream][UNKNOWN_STREAM].Add(function);
                                                }
                                                else
                                                {
                                                    secsMessageCollection = this._driver.Messages[stream, function];

                                                    if (secsMessageCollection != null)
                                                    {
                                                        this._spoolingCollection.Add(stream, function);
                                                    }
                                                    else
                                                    {
                                                        if (spoolingAck.ContainsKey(stream) == false)
                                                        {
                                                            spoolingAck[stream] = new Dictionary<int, List<int>>();
                                                        }

                                                        if (spoolingAck[stream].ContainsKey(UNKNOWN_FUNCTION) == false)
                                                        {
                                                            spoolingAck[stream][UNKNOWN_FUNCTION] = new List<int>();
                                                        }

                                                        spoolingAck[stream][UNKNOWN_FUNCTION].Add(function);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (spoolingAck.Count <= 0)
                            {
                                replyMessage = this._driver.Messages.GetMessageHeader(2, 44);

                                replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("RSACK", GetSECSFormat(PreDefinedDataDictinary.RSPACK, SECSItemFormat.B), 1, ACK);
                                replyMessage.Body.Add("STRIDCOUNT", SECSItemFormat.L, 0, null);
                            }
                            else
                            {
                                this._spoolingCollection.Remove();

                                replyMessage = this._driver.Messages.GetMessageHeader(2, 44);

                                var varAck = from KeyValuePair<int, Dictionary<int, List<int>>> tempStream in spoolingAck
                                             from KeyValuePair<int, List<int>> tempAck in tempStream.Value
                                             select new
                                             {
                                                 Stream = tempStream.Key,
                                                 Ack = tempAck.Key,
                                                 Functions = tempAck.Value
                                             };

                                replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                                replyMessage.Body.Add("RSACK", GetSECSFormat(PreDefinedDataDictinary.RSPACK, SECSItemFormat.B), 1, NAK);
                                replyMessage.Body.Add("STRIDCOUNT", SECSItemFormat.L, varAck.Count(), null);

                                stridItemFormat = GetSECSFormat(PreDefinedDataDictinary.STRID, SECSItemFormat.U2);
                                strackItemFormat = GetSECSFormat(PreDefinedDataDictinary.STRACK, SECSItemFormat.B);
                                fcnidItemFormat = GetSECSFormat(PreDefinedDataDictinary.FCNID, SECSItemFormat.U2);

                                foreach (var tempAck in varAck)
                                {
                                    replyMessage.Body.Add(SECSItemFormat.L, 3, null);
                                    replyMessage.Body.Add("STRID", stridItemFormat, 1, tempAck.Stream);
                                    replyMessage.Body.Add("STRACK", strackItemFormat, 1, tempAck.Ack);
                                    replyMessage.Body.Add("FCNIDCOUNT", SECSItemFormat.L, tempAck.Functions.Count, null);

                                    foreach (int tempFunction in tempAck.Functions)
                                    {
                                        replyMessage.Body.Add("FCNID", fcnidItemFormat, 1, tempFunction);
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
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            secsMessageCollection = null;
                            spoolingAck = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F45]
                case 45:
                    {
                        SetLimitMonitoring(message);
                    }
                    break;
                #endregion
                #region [S2F47]
                case 47:
                    {
                        List<string> vids;
                        VariableInfo variableInfo;
                        SECSItemFormat vidFormat;
                        SECSItemFormat limitIdFormat;
                        SECSItemFormat upperFormat;
                        SECSItemFormat lowerFormat;
                        LimitMonitoringInfo limitMonitoringInfo;

                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            vids = new List<string>();

                            foreach (SECSItem tempVid in message.Body.Item[0].SubItem.Items)
                            {
                                vids.Add(tempVid.Value);
                            }

                            if (vids.Count <= 0)
                            {
                                vids = this._limitMonitoringCollection.Items.Select(t => t.Variable.VID).ToList();
                            }

                            vidFormat = GetSECSFormat(PreDefinedDataDictinary.VID, SECSItemFormat.U2);
                            limitIdFormat = GetSECSFormat(PreDefinedDataDictinary.LIMITID, SECSItemFormat.U2);
                            upperFormat = GetSECSFormat(PreDefinedDataDictinary.UPPERDB, SECSItemFormat.A);
                            lowerFormat = GetSECSFormat(PreDefinedDataDictinary.LOWERDB, SECSItemFormat.A);

                            replyMessage = this._driver.Messages.GetMessageHeader(2, 48);

                            replyMessage.Body.Add("VARCOUNT", SECSItemFormat.L, vids.Count, null);

                            foreach (string tempVid in vids)
                            {
                                variableInfo = this._variableCollection[tempVid];

                                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                                if (variableInfo != null)
                                {
                                    if (vidFormat == SECSItemFormat.A)
                                    {
                                        replyMessage.Body.Add(variableInfo.Name, vidFormat, Encoding.Default.GetByteCount(tempVid), tempVid);
                                    }
                                    else
                                    {
                                        replyMessage.Body.Add(variableInfo.Name, vidFormat, 1, tempVid);
                                    }
                                }
                                else
                                    replyMessage.Body.Add(tempVid.ToString(), vidFormat, 1, tempVid);

                                if (variableInfo == null || variableInfo.Min == null || variableInfo.Max == null)
                                {
                                    replyMessage.Body.Add(SECSItemFormat.L, 0, null);
                                }
                                else
                                {
                                    replyMessage.Body.Add(SECSItemFormat.L, 4, null);

                                    replyMessage.Body.Add("UNITS", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Units.ToString()), variableInfo.Units.ToString());

                                    if (variableInfo.Min != null)
                                    {
                                        replyMessage.Body.Add("LIMITMIN", variableInfo.Format, 1, variableInfo.Min);
                                    }
                                    else
                                    {
                                        replyMessage.Body.Add("LIMITMIN", variableInfo.Format, 0, string.Empty);
                                    }

                                    if (variableInfo.Max != null)
                                    {
                                        replyMessage.Body.Add("LIMITMAX", variableInfo.Format, 1, variableInfo.Min);
                                    }
                                    else
                                    {
                                        replyMessage.Body.Add("LIMITMAX", variableInfo.Format, 0, string.Empty);
                                    }

                                    limitMonitoringInfo = this._limitMonitoringCollection[tempVid];

                                    if (limitMonitoringInfo != null)
                                    {
                                        replyMessage.Body.Add("LIMITCOUNT", SECSItemFormat.L, limitMonitoringInfo.Items.Count, null);

                                        foreach (LimitMonitoringItem tempLimitMonitoringItem in limitMonitoringInfo.Items)
                                        {
                                            replyMessage.Body.Add(SECSItemFormat.L, 3, null);
                                            replyMessage.Body.Add("LIMITID", limitIdFormat, 1, tempLimitMonitoringItem.LimitID);
                                            replyMessage.Body.Add("UPPERDB", upperFormat, GetLength(upperFormat, tempLimitMonitoringItem.UpperBoundary), tempLimitMonitoringItem.UpperBoundary);
                                            replyMessage.Body.Add("LOWERDB", lowerFormat, GetLength(lowerFormat, tempLimitMonitoringItem.LowerBoundary), tempLimitMonitoringItem.LowerBoundary);
                                        }
                                    }
                                    else
                                    {
                                        replyMessage.Body.Add("LIMITCOUNT", SECSItemFormat.L, 0, null);
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
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            vids = null;
                            variableInfo = null;
                            limitMonitoringInfo = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                #region [S2F49]
                case 49:
                    {
                        EnhancedRemoteCommandInfo remoteCommandInfo;
                        EnhancedRemoteCommandInfo newRemoteCommandInfo;
                        Dictionary<string, int> cpAck;
                        RemoteCommandResult remoteCommandResult;
                        bool isInvalid;

                        try
                        {
                            result = AnalyzeMessageError.Ok;
                            isInvalid = false;

                            cpAck = new Dictionary<string, int>();

                            remoteCommandInfo = this._remoteCommandCollection.GetEnhancedRemoteCommand(message.Body.Item[0].SubItem[2].Value);

                            if (remoteCommandInfo != null)
                            {
                                //MakeEnhancedRemoteCommandInfoViolationOfStandards(message, remoteCommandInfo, out newRemoteCommandInfo);

                                MakeEnhancedRemoteCommandInfo(message, remoteCommandInfo, out newRemoteCommandInfo, out cpAck);

                                if (cpAck.Count <= 0)
                                {
                                    if (this.OnReceivedEnhancedRemoteCommand != null)
                                    {
                                        this.OnReceivedEnhancedRemoteCommand(newRemoteCommandInfo);

                                        remoteCommandResult = null;
                                    }
                                    else
                                    {
                                        remoteCommandResult = new RemoteCommandResult()
                                        {
                                            HostCommandAck = (int)HCACK.Acknowledge
                                        };
                                    }
                                }
                                else
                                {
                                    RaiseInvalidEnhancedRemoteCommand(message);
                                    isInvalid = true;

                                    remoteCommandResult = new RemoteCommandResult()
                                    {
                                        HostCommandAck = (int)HCACK.ParameterIsInvalid
                                    };

                                    foreach (KeyValuePair<string, int> tempCPAck in cpAck)
                                    {
                                        remoteCommandResult.AddParameterResult(tempCPAck.Key, tempCPAck.Value);
                                    }
                                }
                            }
                            else
                            {
                                RaiseInvalidEnhancedRemoteCommand(message);
                                isInvalid = true;

                                newRemoteCommandInfo = new EnhancedRemoteCommandInfo()
                                {
                                    RemoteCommand = message.Body.Item[0].SubItem[2].Value,
                                    SystemBytes = message.SystemBytes
                                };

                                remoteCommandResult = new RemoteCommandResult()
                                {
                                    HostCommandAck = (int)HCACK.CommandDoesNotExist
                                };

                                result = AnalyzeMessageError.Undefined;
                            }

                            if (isInvalid == true)
                            {
                                ReplyEnhancedRemoteCommandAck(newRemoteCommandInfo, remoteCommandResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            remoteCommandInfo = null;
                            newRemoteCommandInfo = null;
                            cpAck = null;
                            replyMessage = null;
                        }
                    }
                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream3(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream4(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream5(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S5F3]
                case 3:
                    {
                        byte aled;
                        bool enabled;
                        long alarmId;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            aled = message.Body.Item[0].SubItem[0].Value;

                            if (aled >= 128)
                            {
                                enabled = true;
                            }
                            else
                            {
                                enabled = false;
                            }

                            if (message.Body.Item[0].SubItem[1].Length == 0)
                            {
                                this._alarmCollection.SetEnabled(enabled);
                            }
                            else
                            {
                                alarmId = message.Body.Item[0].SubItem[1].Value;

                                if (this._alarmCollection.SetEnabled(enabled, alarmId) == false)
                                {
                                    result = AnalyzeMessageError.Unknown;

                                    logText = string.Format("Alarmdose not exist(S5F{0}:{1}):ALID={2}", message.Function, message.Name, alarmId);

                                    this._logger.WriteGEM(LogLevel.Warning, logText);
                                }
                            }

                            replyMessage = this._driver.Messages.GetMessageHeader(5, 4);

                            if (result == AnalyzeMessageError.Ok)
                            {
                                replyMessage.Body.Add("ACKC5", GetSECSFormat(PreDefinedDataDictinary.ACKC5, SECSItemFormat.B), 1, ACK);
                            }
                            else
                            {
                                replyMessage.Body.Add("ACKC5", GetSECSFormat(PreDefinedDataDictinary.ACKC5, SECSItemFormat.B), 1, NAK);
                            }

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S5F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S5F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }

                            this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.Alarms, false, out string errorText);

                            this.OnReceivedEnableDisableAlarmSend?.Invoke();

                            if (string.IsNullOrEmpty(errorText) == false)
                            {
                                logText = string.Format("Enable/Disable alarm report save failed(S5F{0}:{1}):Error={2}", message.Function, message.Name, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            replyMessage = null;
                        }
                    }

                    break;
                #endregion
                #region [S5F5]
                case 5:
                    {
                        List<long> alarmIds;
                        SECSItemFormat secsItemFormatAlcd;
                        SECSItemFormat secsItemFormatAlid;
                        SECSItemFormat secsItemFormatAltx;
                        AlarmInfo alarmInfo;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            if (message.Body.Item[0].Length == 0)
                            {
                                alarmIds = this._alarmCollection.Items.Select(t => t.ID).ToList();
                            }
                            else
                            {
                                alarmIds = new List<long>();

                                if (message.Body.Item[0].Format == SECSItemFormat.L)
                                {
                                    foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem.Items)
                                    {
                                        alarmIds.Add(tempSECSItem.Value);
                                    }
                                }
                                else
                                {
                                    if (message.Body.Item[0].Length == 1)
                                    {
                                        alarmIds.Add(message.Body.Item[0].Value);
                                    }
                                    else
                                    {
                                        if (message.Body.Item[0].Value.GetValue() is IList)
                                        {
                                            IList list = message.Body.Item[0].Value.GetValue() as IList;

                                            for (int i = 0; i < list.Count; i++)
                                            {
                                                alarmIds.Add(Convert.ToInt64(list[i]));
                                            }
                                        }
                                    }
                                }
                            }

                            secsItemFormatAlcd = GetSECSFormat(PreDefinedDataDictinary.ALCD, SECSItemFormat.U2);
                            secsItemFormatAlid = GetSECSFormat(PreDefinedDataDictinary.ALID, SECSItemFormat.U2);
                            secsItemFormatAltx = GetSECSFormat(PreDefinedDataDictinary.ALTX, SECSItemFormat.A);

                            replyMessage = this._driver.Messages.GetMessageHeader(5, 6);

                            replyMessage.Body.Add("ALIDCOUNT", SECSItemFormat.L, alarmIds.Count, null);

                            foreach (long tempAlarmId in alarmIds)
                            {
                                alarmInfo = this._alarmCollection[tempAlarmId];

                                replyMessage.Body.Add(SECSItemFormat.L, 3, null);

                                if (alarmInfo != null)
                                {
                                    replyMessage.Body.Add("ALCD", secsItemFormatAlcd, 1, alarmInfo.Code);
                                    replyMessage.Body.Add("ALID", secsItemFormatAlid, 1, alarmInfo.ID);
                                    replyMessage.Body.Add("ALTX", secsItemFormatAltx, Encoding.Default.GetByteCount(alarmInfo.Description), alarmInfo.Description);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ALCD", secsItemFormatAlcd, 0, 0);
                                    replyMessage.Body.Add("ALID", secsItemFormatAlid, 1, alarmInfo.ID);
                                    replyMessage.Body.Add("ALTX", secsItemFormatAltx, 0, string.Empty);
                                }
                            }

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S5F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S5F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            replyMessage = null;
                        }
                    }

                    break;
                #endregion
                #region [S5F7]
                case 7:
                    {
                        List<long> alarmIds;
                        SECSItemFormat secsItemFormatAlcd;
                        SECSItemFormat secsItemFormatAlid;
                        SECSItemFormat secsItemFormatAltx;
                        AlarmInfo alarmInfo;

                        try
                        {
                            result = AnalyzeMessageError.Ok;

                            alarmIds = this._alarmCollection.Items.Where(t => t.Enabled == true).Select(t => t.ID).ToList();

                            secsItemFormatAlcd = GetSECSFormat(PreDefinedDataDictinary.ALCD, SECSItemFormat.U2);
                            secsItemFormatAlid = GetSECSFormat(PreDefinedDataDictinary.ALID, SECSItemFormat.U2);
                            secsItemFormatAltx = GetSECSFormat(PreDefinedDataDictinary.ALTX, SECSItemFormat.A);

                            replyMessage = this._driver.Messages.GetMessageHeader(5, 8);

                            replyMessage.Body.Add("ALIDCOUNT", SECSItemFormat.L, alarmIds.Count, null);

                            foreach (long tempAlarmId in alarmIds)
                            {
                                alarmInfo = this._alarmCollection[tempAlarmId];

                                if (alarmInfo != null)
                                {
                                    replyMessage.Body.Add(SECSItemFormat.L, 3, null);
                                    replyMessage.Body.Add("ALCD", secsItemFormatAlcd, 1, alarmInfo.Code);
                                    replyMessage.Body.Add("ALID", secsItemFormatAlid, 1, alarmInfo.ID);
                                    replyMessage.Body.Add("ALTX", secsItemFormatAltx, Encoding.Default.GetByteCount(alarmInfo.Description), alarmInfo.Description);
                                }
                                else
                                {
                                    replyMessage.Body.Add(SECSItemFormat.L, 0, null);
                                }
                            }

                            driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                            if (driverResult == MessageError.Ok)
                            {
                                logText = string.Format("Transmission successful(S5F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                logText = string.Format("Transmission failure(S5F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                            replyMessage = null;
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            replyMessage = null;

            return result;
        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream6(SECSMessage message)
        {
            AnalyzeMessageError result;
            SECSMessage replyMessage;
            MessageMakeError messageMakeError;
            MessageError driverResult;
            string logText;
            string errorText;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S6F15]
                case 15:
                    {
                        SECSItem secsItem;
                        string ceid;

                        try
                        {
                            secsItem = message.Body.Item[0];

                            if (secsItem != null)
                            {
                                if (this._ceidFormat == SECSItemFormat.A)
                                {
                                    ceid = secsItem.Value;
                                }
                                else
                                {
                                    ceid = ((long)secsItem.Value).ToString();
                                }

                                CollectionEventInfo collectionEventInfo;

                                collectionEventInfo = this._collectionEventCollection[ceid];

                                CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock);

                                messageMakeError = this._messageMaker.MakeS6F16(this._collectionEventCollection, this._variableCollection, ceid, out replyMessage, out errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        logText = string.Format("Transmission successful(S6F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        logText = string.Format("Transmission failure(S6F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    logText = string.Format("Message make failure(S6F16), Result={0}, Error Text={1}", messageMakeError, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                result = AnalyzeMessageError.Unknown;
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S6F19]
                case 19:
                    {
                        SECSItem secsItem;
                        string rptid;
                        ReportInfo reportInfo;

                        try
                        {
                            secsItem = message.Body.Item[0];

                            if (secsItem != null)
                            {
                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    rptid = secsItem.Value;
                                }
                                else
                                {
                                    rptid = ((long)secsItem.Value).ToString();
                                }

                                reportInfo = this._reportCollection[rptid];

                                if (reportInfo != null && reportInfo.Variables.Items.Count > 0)
                                {
                                    this.OnVariableUpdateRequest?.Invoke(VariableUpdateType.S6F19IndividualReportRequest, reportInfo.Variables.Items, string.Empty);
                                }

                                messageMakeError = this._messageMaker.MakeS6F20(reportInfo, this._variableCollection, out replyMessage, out errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        logText = string.Format("Transmission successful(S6F{0}:{1})", replyMessage.Function, replyMessage.Name);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        logText = string.Format("Transmission failure(S6F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    logText = string.Format("Message make failure(S6F20), Result={0}, Error Text={1}", messageMakeError, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                result = AnalyzeMessageError.Unknown;
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream7(SECSMessage message)
        {
            AnalyzeMessageError result;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S7F1]
                case 1:
                    {
                        string ppid;
                        long length;

                        try
                        {
                            ppid = message.Body.Item[0].SubItem[0].Value;
                            length = message.Body.Item[0].SubItem[1].Value;

                            this.OnReceivedPPLoadInquire?.Invoke(message.SystemBytes, ppid, (int)length);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F3]
                case 3:
                    {
                        string ppid;
                        byte[] ppbody;

                        try
                        {
                            ppid = message.Body.Item[0].SubItem[0].Value;
                            ppbody = message.Body.Item[0].SubItem[1].Value;

                            this.OnReceivedPPSend?.Invoke(message.SystemBytes, ppid, ppbody.ToList());
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F5]
                case 5:
                    {
                        string ppid;

                        try
                        {
                            ppid = message.Body.Item[0].Value;

                            this.OnReceivedPPRequest?.Invoke(message.SystemBytes, ppid);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F17]
                case 17:
                    {
                        List<string> ppids;

                        try
                        {
                            ppids = new List<string>();

                            foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem.Items)
                            {
                                ppids.Add(tempSECSItem.Value);
                            }

                            this.OnReceivedDeletePPSend?.Invoke(message.SystemBytes, ppids);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F19]
                case 19:
                    {
                        try
                        {
                            this.OnReceivedCurrentEPPDRequest?.Invoke(message.SystemBytes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F23]
                case 23:
                    {
                        FmtPPCollection formattedProcessProgramCollection;
                        FmtPPCCodeInfo formattedProcessProgramInfo;
                        SECSItem secsItem;

                        try
                        {
                            formattedProcessProgramCollection = new FmtPPCollection(message.Body.Item[0].SubItem[0].Value)
                            {
                                MDLN = message.Body.Item[0].SubItem[1].Value,
                                SOFTREV = message.Body.Item[0].SubItem[2].Value
                            };

                            for (int i = 0; i < message.Body.Item[0].SubItem[3].Length; i++)
                            {
                                formattedProcessProgramInfo = new FmtPPCCodeInfo()
                                {
                                    CommandCode = message.Body.Item[0].SubItem[3].SubItem[i].SubItem[0].Value
                                };

                                secsItem = message.Body.Item[0].SubItem[3].SubItem[i].SubItem[1];

                                for (int j = 0; j < secsItem.Length; j++)
                                {
                                    if (secsItem.SubItem[j].Format == SECSItemFormat.L)
                                    {
                                        if (secsItem.SubItem[j].Length == 2)
                                        {
                                            formattedProcessProgramInfo.Add(secsItem.SubItem[j].SubItem[0].Value, GetValue(secsItem.SubItem[j].SubItem[1]), secsItem.SubItem[j].SubItem[1].Format);
                                        }
                                    }
                                    else
                                    {
                                        formattedProcessProgramInfo.Add(GetValue(secsItem.SubItem[j]), secsItem.SubItem[j].Format);
                                    }
                                }

                                formattedProcessProgramCollection.Items.Add(formattedProcessProgramInfo);
                            }

                            this.OnReceivedFmtPPSend?.Invoke(message.SystemBytes, formattedProcessProgramCollection);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F25]
                case 25:
                    {
                        string ppid;

                        try
                        {
                            ppid = message.Body.Item[0].Value;

                            this.OnReceivedFmtPPRequest?.Invoke(message.SystemBytes, ppid);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream8(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream9(SECSMessage message)
        {
            AnalyzeMessageError result;

            switch (message.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream10(SECSMessage message)
        {
            AnalyzeMessageError result;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S10F3]
                case 3:
                    {
                        int tid;
                        string terminalMessage;

                        try
                        {
                            tid = message.Body.Item[0].SubItem[0].Value;
                            terminalMessage = message.Body.Item[0].SubItem[1].Value;

                            this.OnReceivedTerminalMessage?.Invoke(message.SystemBytes, tid, terminalMessage);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                #region [S10F5]
                case 5:
                    {
                        int tid;
                        List<string> terminalMessages;

                        try
                        {
                            terminalMessages = new List<string>();

                            tid = message.Body.Item[0].SubItem[0].Value;

                            foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[1].SubItem.Items)
                            {
                                terminalMessages.Add(tempSECSItem.Value);
                            }

                            this.OnReceivedTerminalMultiMessage?.Invoke(message.SystemBytes, tid, terminalMessages);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream11(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream12(SECSMessage message)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S12F19]
                case 19:
                    {
                        int mapError;
                        int dataLocation;

                        try
                        {
                            mapError = message.Body.Item[0].SubItem[0].Value;
                            dataLocation = message.Body.Item[0].SubItem[1].Value;

                            this.OnReceivedMapErrorReportSend?.Invoke(mapError, dataLocation);

                            logText = string.Format("S12F19(Map Error Report Send):Map error={0}, Data location={1}", mapError, dataLocation);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream13(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzePrimaryMessageStream14(SECSMessage message)
        {
            AnalyzeMessageError result;

            result = AnalyzeMessageError.Ok;

            switch (message.Function)
            {
                #region [S14F1]
                case 1:
                    {
                        ObjectQualifierInfo objectQualifierInfo;
                        AttributeDataItem attributeDataItem;
                        string objectSpec;
                        string objectType;
                        List<string> objectIDs;
                        List<ObjectQualifierInfo> objectQualifiers;
                        List<string> attributes;

                        try
                        {
                            objectSpec = message.Body.Item[0].SubItem[0].Value;
                            objectType = message.Body.Item[0].SubItem[1].Value;

                            objectIDs = new List<string>();
                            objectQualifiers = new List<ObjectQualifierInfo>();
                            attributes = new List<string>();

                            foreach (SECSItem tempObjectId in message.Body.Item[0].SubItem[2].SubItem.Items)
                            {
                                objectIDs.Add(tempObjectId.Value);
                            }

                            foreach (SECSItem tempQualifier in message.Body.Item[0].SubItem[3].SubItem.Items)
                            {
                                objectQualifierInfo = new ObjectQualifierInfo()
                                {
                                    AttributeID = tempQualifier.SubItem[0].Value,
                                    AttributeRelationship = tempQualifier.SubItem[2].Value
                                };

                                attributeDataItem = new AttributeDataItem();

                                SetObjectAttributeData(tempQualifier.SubItem[1], ref attributeDataItem);

                                objectQualifierInfo.AttributeData = attributeDataItem;

                                objectQualifiers.Add(objectQualifierInfo);
                            }

                            foreach (SECSItem tempAttribute in message.Body.Item[0].SubItem[4].SubItem.Items)
                            {
                                attributes.Add(tempAttribute.Value);
                            }

                            this.OnReceivedGetAttributeRequest?.Invoke(objectSpec, objectType, objectIDs, objectQualifiers, attributes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F3]
                case 3:
                    {
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        string objectSpec;
                        string objectType;
                        List<string> objectIDs;
                        List<AttributeInfo> attributes;

                        try
                        {
                            objectIDs = new List<string>();
                            attributes = new List<AttributeInfo>();

                            objectSpec = message.Body.Item[0].SubItem[0].Value;
                            objectType = message.Body.Item[0].SubItem[1].Value;

                            foreach (SECSItem tempObjectId in message.Body.Item[0].SubItem[2].SubItem.Items)
                            {
                                objectIDs.Add(tempObjectId.Value);
                            }

                            foreach (SECSItem tempAttribute in message.Body.Item[0].SubItem[3].SubItem.Items)
                            {
                                attributeInfo = new AttributeInfo()
                                {
                                    AttributeID = tempAttribute.SubItem[0].Value
                                };

                                attributeDataItem = new AttributeDataItem();

                                SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                attributeInfo.AttributeData = attributeDataItem;

                                attributes.Add(attributeInfo);
                            }

                            this.OnReceivedSetAttributeRequest?.Invoke(objectSpec, objectType, objectIDs, attributes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F5]
                case 5:
                    {
                        string objectSpec;

                        try
                        {
                            objectSpec = message.Body.Item[0].Value;

                            this.OnReceivedGetTypeRequest?.Invoke(objectSpec);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F7]
                case 7:
                    {
                        string objectSpec;
                        List<string> objTypes; 

                        try
                        {
                            objTypes = new List<string>();

                            objectSpec = message.Body.Item[0].SubItem[0].Value;

                            foreach (SECSItem tempObjectType in message.Body.Item[0].SubItem[1].SubItem.Items)
                            {
                                objTypes.Add(tempObjectType.Value);
                            }

                            this.OnReceivedGetAttributeNameRequest?.Invoke(objectSpec, objTypes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F9]
                case 9:
                    { 
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        string objectSpec;
                        string objectType;
                        List<AttributeInfo> attributes;

                        try
                        {
                            attributes = new List<AttributeInfo>();

                            objectSpec = message.Body.Item[0].SubItem[0].Value;
                            objectType = message.Body.Item[0].SubItem[1].Value;

                            foreach (SECSItem tempAttribute in message.Body.Item[0].SubItem[2].SubItem.Items)
                            {
                                attributeInfo = new AttributeInfo()
                                {
                                    AttributeID = tempAttribute.SubItem[0].Value
                                };

                                attributeDataItem = new AttributeDataItem();

                                SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                attributeInfo.AttributeData = attributeDataItem;

                                attributes.Add(attributeInfo);
                            }

                            this.OnReceivedCreateObjectRequest?.Invoke(message.SystemBytes, objectSpec, objectType, attributes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F11]
                case 11:
                    { 
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        string objectSpec;
                        List<AttributeInfo> attributes;

                        try
                        {
                            attributes = new List<AttributeInfo>();

                            objectSpec = message.Body.Item[0].SubItem[0].Value;

                            foreach (SECSItem tempAttribute in message.Body.Item[0].SubItem[1].SubItem.Items)
                            {
                                attributeInfo = new AttributeInfo()
                                {
                                    AttributeID = tempAttribute.SubItem[0].Value
                                };

                                attributeDataItem = new AttributeDataItem();

                                SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                attributeInfo.AttributeData = attributeDataItem;

                                attributes.Add(attributeInfo);
                            }

                            this.OnReceivedDeleteObjectRequest?.Invoke(objectSpec, attributes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F13]
                case 13:
                    { 
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        string objectSpec;
                        List<AttributeInfo> attributes;

                        try
                        {
                            attributes = new List<AttributeInfo>();

                            objectSpec = message.Body.Item[0].SubItem[0].Value;

                            foreach (SECSItem tempAttribute in message.Body.Item[0].SubItem[1].SubItem.Items)
                            {
                                attributeInfo = new AttributeInfo()
                                {
                                    AttributeID = tempAttribute.SubItem[0].Value
                                };

                                attributeDataItem = new AttributeDataItem();

                                SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                attributeInfo.AttributeData = attributeDataItem;

                                attributes.Add(attributeInfo);
                            }

                            this.OnReceivedAttachObjectRequest?.Invoke(objectSpec, attributes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F15]
                case 15:
                    { 
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        string objectSpec;
                        ulong objectCommand;
                        ulong objectToken;
                        List<AttributeInfo> attributes;

                        try
                        {
                            attributes = new List<AttributeInfo>();

                            objectSpec = message.Body.Item[0].SubItem[0].Value;

                            switch (message.Body.Item[0].SubItem[1].Format)
                            {
                                case SECSItemFormat.U1:
                                    {
                                        objectCommand = (byte)message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                case SECSItemFormat.U2:
                                    {
                                        objectCommand = (ushort)message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                case SECSItemFormat.U4:
                                    {
                                        objectCommand = (uint)message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                case SECSItemFormat.U8:
                                    {
                                        objectCommand = message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                case SECSItemFormat.B:
                                case SECSItemFormat.I1:
                                    {
                                        objectCommand = (ulong)(sbyte)message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        objectCommand = (ulong)(short)message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        objectCommand = (ulong)(int)message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        objectCommand = (ulong)(long)message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                                default:
                                    {
                                        objectCommand = message.Body.Item[0].SubItem[1].Value;
                                    }
                                    break;
                            }

                            switch (message.Body.Item[0].SubItem[2].Format)
                            {
                                case SECSItemFormat.U1:
                                    {
                                        objectToken = (byte)message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                case SECSItemFormat.U2:
                                    {
                                        objectToken = (ushort)message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                case SECSItemFormat.U4:
                                    {
                                        objectToken = (uint)message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                case SECSItemFormat.U8:
                                    {
                                        objectToken = message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                case SECSItemFormat.B:
                                case SECSItemFormat.I1:
                                    {
                                        objectToken = (ulong)(sbyte)message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        objectToken = (ulong)(short)message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        objectToken = (ulong)(int)message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        objectToken = (ulong)(long)message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                                default:
                                    {
                                        objectToken = message.Body.Item[0].SubItem[2].Value;
                                    }
                                    break;
                            }

                            foreach (SECSItem tempAttribute in message.Body.Item[0].SubItem[3].SubItem.Items)
                            {
                                attributeInfo = new AttributeInfo()
                                {
                                    AttributeID = tempAttribute.SubItem[0].Value
                                };

                                attributeDataItem = new AttributeDataItem();

                                SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                attributeInfo.AttributeData = attributeDataItem;

                                attributes.Add(attributeInfo);
                            }

                            this.OnReceivedAttachedObjectActionRequest?.Invoke(objectSpec, objectCommand, objectToken, attributes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F17]
                case 17:
                    { 
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        string objectSpec;
                        int objectCommand;
                        string targetSpec;
                        List<AttributeInfo> attributes;

                        try
                        {
                            attributes = new List<AttributeInfo>();

                            objectSpec = message.Body.Item[0].SubItem[0].Value;
                            objectCommand = message.Body.Item[0].SubItem[1].Value;
                            targetSpec = message.Body.Item[0].SubItem[2].Value;

                            foreach (SECSItem tempAttribute in message.Body.Item[0].SubItem[3].SubItem.Items)
                            {
                                attributeInfo = new AttributeInfo()
                                {
                                    AttributeID = tempAttribute.SubItem[0].Value
                                };

                                attributeDataItem = new AttributeDataItem();

                                SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                attributeInfo.AttributeData = attributeDataItem;

                                attributes.Add(attributeInfo);
                            }

                            this.OnReceivedSupervisedObjectActionRequest?.Invoke(objectSpec, objectCommand, targetSpec, attributes);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F19]
                case 19:
                    {

                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream15(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream16(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

//        private AnalyzeMessageError AnalyzePrimaryMessageStream17(SECSMessage message)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            //SECSMessage replyMessage;
//            //MessageMakeError messageMakeError;

//            result = AnalyzeMessageError.Ok;

//            switch (message.Function)
//            {
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }

//            //replyMessage = null;
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream1(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            GemDriverError driverResult;
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S1F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S1F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);

                        try
                        {
                            if (primaryMessage.Function == 1)
                            {
                                if (primaryMessage.UserData != null)
                                {
                                    if (this._controlState == ControlState.AttemptOnline)
                                    {
                                        FailAreYouThere();

                                        this._timerAreYouThere.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                                    }

                                    if (this._controlState == ControlState.AttemptOnline)
                                    {
                                        if (Enum.TryParse<ControlState>(primaryMessage.UserData.ToString(), out ControlState controlState) == true)
                                        {
                                            driverResult = SendControlState(controlState);

                                            if (driverResult == GemDriverError.Ok)
                                            {
                                                logText = string.Format("Control State Changed:State={0}", this._controlState);

                                                this._logger.WriteGEM(LogLevel.Information, logText);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                #region [S1F2]
                case 2:
                    {
                        try
                        {
                            if (this._controlState == ControlState.AttemptOnline)
                            {
                                if (primaryMessage.UserData != null)
                                {
                                    this._timerAreYouThere.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                                    if (Enum.TryParse<ControlState>(primaryMessage.UserData.ToString(), out ControlState controlState) == true)
                                    {
                                        driverResult = SendControlState(controlState);

                                        if (driverResult == GemDriverError.Ok)
                                        {
                                            logText = string.Format("Control State Changed:State={0}", this._controlState);

                                            this._logger.WriteGEM(LogLevel.Information, logText);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                #region [S1F14]
                case 14:
                    {
                        try
                        {
                            int commAck;

                            commAck = secondaryMessage.Body.Item[0].SubItem[0].Value;

                            if (commAck == (int)COMMACK.Accepted)
                            {
                                this._timerCommDelay.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                                if (this._communicationState == CommunicationState.WaitCRA)
                                {
                                    logText = string.Format("Communication State Changed:COMMACK={0}, State={1}->{2}", commAck, this._communicationState, CommunicationState.Communicating);

                                    this._communicationState = CommunicationState.Communicating;

                                    this.OnCommunicationStateChanged?.Invoke(this._communicationState);
                                }
                                else
                                {
                                    logText = string.Format("Communication State:Stat:COMMACK={0}, State={1}", commAck, this._communicationState);
                                }
                            }
                            else
                            {
                                logText = string.Format("Communication State Changed:COMMACK={0}", commAck);
                            }

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream2(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S2F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S2F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }

                    break;
                #endregion
                #region [S2F18]
                case 18:
                    {
                        DateTime? timeData;
                        string timeString;
                        bool processResult;

                        try
                        {
                            timeString = secondaryMessage.Body.Item[0].Value;

                            if (timeString.Length == 16)
                            {
                                timeData = new DateTime(int.Parse(timeString.Substring(0, 4)),
                                                        int.Parse(timeString.Substring(4, 2)),
                                                        int.Parse(timeString.Substring(6, 2)),
                                                        int.Parse(timeString.Substring(8, 2)),
                                                        int.Parse(timeString.Substring(10, 2)),
                                                        int.Parse(timeString.Substring(12, 2)),
                                                        int.Parse(timeString.Substring(14, 2)) * 10);
                            }
                            else if (timeString.Length == 12)
                            {
                                timeData = new DateTime(int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + timeString.Substring(0, 2)),
                                                        int.Parse(timeString.Substring(2, 2)),
                                                        int.Parse(timeString.Substring(4, 2)),
                                                        int.Parse(timeString.Substring(6, 2)),
                                                        int.Parse(timeString.Substring(8, 2)),
                                                        int.Parse(timeString.Substring(10, 2)));
                            }
                            else
                            {
                                timeData = null;
                            }

                            if (timeData != null)
                            {
                                if (this.OnResponseDateTimeRequest != null)
                                {
                                    processResult = this.OnResponseDateTimeRequest(timeData.GetValueOrDefault());
                                }
                                else
                                {
                                    processResult = true;
                                }

                                if (processResult == true && this._configFileManager.GEMConfiguration.ExtensionOption.UseAutoTimeSync == true)
                                {
                                    try
                                    {
                                        DateTime targetTime = timeData.Value;
                                        TimeSpan ts = DateTime.UtcNow - targetTime;

                                        targetTime = targetTime.Add(ts);

                                        Common.SYSTEMTIME systemTime = new Common.SYSTEMTIME()
                                        {
                                            wYear = (ushort)targetTime.Year,
                                            wMonth = (ushort)targetTime.Month,
                                            wDay = (ushort)targetTime.Day,
                                            wHour = (ushort)targetTime.Hour,
                                            wMinute = (ushort)targetTime.Minute,
                                            wSecond = (ushort)targetTime.Second
                                        };

                                        _ = Common.SetSystemTime(ref systemTime);

                                        this._logger.WriteGEM(LogLevel.Information, "Auto Time Sync OK");
                                    }
                                    catch (Exception exTimeSync)
                                    {
                                        logText = string.Format("Auto Time Sync Failed:Error={0}", exTimeSync.Message);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                }

                                logText = string.Format("Time Data Received:Time Data={0}", timeData.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss.ff"));

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                logText = string.Format("Time Data Received:Length={0}", timeString.Length);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S2F26]
                case 26:
                    {
                        byte[] sendAbs;
                        byte[] receiveAbs;
                        StringBuilder sb;

                        try
                        {
                            sb = new StringBuilder(200);

                            if (primaryMessage.Body.AsList[0].Value.Length == 1)
                            {
                                receiveAbs = new byte[1];

                                receiveAbs[0] = primaryMessage.Body.AsList[0].Value;
                            }
                            else if (primaryMessage.Body.AsList[0].Value.Length == 0)
                            {
                                receiveAbs = new byte[0];
                            }
                            else
                            {
                                receiveAbs = primaryMessage.Body.AsList[0].Value;
                            }

                            if (secondaryMessage.Body.Item[0].Value.Length == 1)
                            {
                                sendAbs = new byte[1];

                                sendAbs[0] = secondaryMessage.Body.Item[0].Value;
                            }
                            else if (secondaryMessage.Body.Item[0].Value.Length == 0)
                            {
                                sendAbs = new byte[0];
                            }
                            else
                            {
                                sendAbs = secondaryMessage.Body.Item[0].Value;
                            }

                            this.OnResponseLoopback?.Invoke(receiveAbs.ToList(), sendAbs.ToList());

                            sb.Append("Loopback Diagnostic Data Received(S2F26):Send=[ ");

                            foreach (long tempAbs in sendAbs)
                            {
                                sb.AppendFormat("{0} ", tempAbs);
                            }

                            sb.Append("] Receive=[ ");

                            foreach (long tempAbs in receiveAbs)
                            {
                                sb.AppendFormat("{0} ", tempAbs);
                            }

                            sb.Append("]");

                            logText = sb.ToString();

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream3(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S3F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S3F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream4(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S4F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S4F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream5(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S5F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S5F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream6(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S6F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S6F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }

                    break;
                #endregion
                #region [S6F12]
                case 12:
                    {
                        int ack;
                        string ceid;

                        try
                        {
                            ack = secondaryMessage.Body.Item[0].Value;
                            ceid = primaryMessage.Body.AsList[2].Value;

                            this.OnResponseEventReportAcknowledge?.Invoke(ceid, ack);

                            logText = string.Format("S6F12(Event Report Acknowledge):CEID={0}, ACK={1}", ceid, ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream7(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S7F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S7F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }

                    break;
                #endregion
                #region [S7F2]
                case 2:
                    {
                        int processProgramGrantStatus;
                        string ppid;

                        try
                        {
                            processProgramGrantStatus = secondaryMessage.Body.Item[0].Value;
                            ppid = primaryMessage.UserData as string;

                            this.OnResponsePPLoadInquire?.Invoke(processProgramGrantStatus, ppid);

                            logText = string.Format("S7F2(Process Program Load Grant):PPID={0}, PPGNT={1}", ppid, processProgramGrantStatus);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F4]
                case 4:
                    {
                        int ack;
                        string ppid;

                        try
                        {
                            ack = (int)secondaryMessage.Body.Item[0].Value;
                            ppid = primaryMessage.UserData as string;

                            this.OnResponsePPSend?.Invoke(ack, ppid);

                            logText = string.Format("S7F4(Process Program Load Grant):PPID={0}, ACK={1}", ppid, ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F6]
                case 6:
                    {
                        string ppid;
                        byte[] ppbody;

                        try
                        {
                            if (secondaryMessage.Body.Item.Count > 0 && secondaryMessage.Body.Item[0].SubItem != null && secondaryMessage.Body.Item[0].SubItem.Count > 0)
                            {
                                ppid = secondaryMessage.Body.Item[0].SubItem[0].Value;
                                ppbody = secondaryMessage.Body.Item[0].SubItem[1].Value;

                                this.OnResponsePPRequest?.Invoke(ppid, ppbody.ToList());

                                logText = string.Format("S7F6(Process Program Data):PPID={0}, Length={1}", ppid, ppbody.Length);
                            }
                            else
                            {
                                this.OnResponsePPRequest?.Invoke(primaryMessage.UserData.ToString(), null);

                                logText = string.Format("S7F6(Process Program Data):PPID={0}, Denied", primaryMessage.UserData);
                            }

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F24]
                case 24:
                    {
                        int ack;
                        FmtPPCollection formattedProcessProgramCollection;

                        try
                        {
                            ack = (int)secondaryMessage.Body.Item[0].Value;
                            formattedProcessProgramCollection = primaryMessage.UserData as FmtPPCollection;

                            this.OnResponseFmtPPSend?.Invoke(ack, formattedProcessProgramCollection);

                            logText = string.Format("S7F24(Formatted Process Program Acknowledge):PPID={0}, ACK={1}", formattedProcessProgramCollection.PPID, ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F26]
                case 26:
                    {
                        FmtPPCollection formattedProcessProgramCollection;
                        FmtPPCCodeInfo formattedProcessProgramInfo;
                        SECSItem secsItem;

                        try
                        {
                            formattedProcessProgramCollection = new FmtPPCollection(secondaryMessage.Body.Item[0].SubItem[0].Value)
                            {
                                MDLN = secondaryMessage.Body.Item[0].SubItem[1].Value,
                                SOFTREV = secondaryMessage.Body.Item[0].SubItem[2].Value
                            };

                            for (int i = 0; i < secondaryMessage.Body.Item[0].SubItem[3].Length; i++)
                            {
                                formattedProcessProgramInfo = new FmtPPCCodeInfo()
                                {
                                    CommandCode = secondaryMessage.Body.Item[0].SubItem[3].SubItem[i].SubItem[0].Value
                                };

                                secsItem = secondaryMessage.Body.Item[0].SubItem[3].SubItem[i].SubItem[1];

                                for (int j = 0; j < secsItem.Length; j++)
                                {
                                    if (secsItem.SubItem[j].Format == SECSItemFormat.L)
                                    {
                                        if (secsItem.SubItem[j].Length == 2)
                                        {
                                            formattedProcessProgramInfo.Add(secsItem.SubItem[j].SubItem[0].Value, GetValue(secsItem.SubItem[j].SubItem[1]), secsItem.SubItem[j].SubItem[1].Format);
                                        }
                                    }
                                    else
                                    {
                                        formattedProcessProgramInfo.Add(GetValue(secsItem.SubItem[j]), secsItem.SubItem[j].Format);
                                    }
                                }

                                formattedProcessProgramCollection.Items.Add(formattedProcessProgramInfo);
                            }

                            this.OnResponseFmtPPRequest?.Invoke(formattedProcessProgramCollection);

                            logText = string.Format("S7F26(Formatted Process Program Data):PPID={0}", formattedProcessProgramCollection.PPID);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S7F28]
                case 28:
                    {
                        FmtPPVerificationCollection fmtPPVerificationCollection;

                        try
                        {
                            fmtPPVerificationCollection = primaryMessage.UserData as FmtPPVerificationCollection;

                            this.OnResponseFmtPPVerification?.Invoke(fmtPPVerificationCollection);

                            logText = string.Format("S7F28(Formatted Process Program Verification):PPID={0}", fmtPPVerificationCollection.PPID);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream8(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S8F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S8F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream9(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;

            switch (secondaryMessage.Function)
            {
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream10(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S10F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S10F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }

                    break;
                #endregion
                #region [S10F2]
                case 2:
                    {
                        int ack;

                        try
                        {
                            ack = (int)secondaryMessage.Body.Item[0].Value;

                            this.OnResponseTerminalRequest?.Invoke(ack);

                            logText = string.Format("S10F2(Terminal Request Acknowledge):ACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                        finally
                        {
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream11(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S11F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S11F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream12(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S12F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S12F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }

                    break;
                #endregion
                #region [S12F2]
                case 2:
                    {
                        int setupDataAck;

                        try
                        {
                            setupDataAck = secondaryMessage.Body.Item[0].Value;

                            this.OnResponseMapSetupDataAck?.Invoke(setupDataAck);

                            logText = string.Format("S12F2(Map Set-up Data Acknowledge):SDACK={0}", setupDataAck);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F4]
                case 4:
                    {
                        MapSetupData mapSetupData;

                        try
                        {
                            if (secondaryMessage.Body.Item[0].Length > 0)
                            {
                                mapSetupData = new MapSetupData
                                {
                                    MaterialId = secondaryMessage.Body.Item[0].SubItem[0].Value,
                                    IDType = secondaryMessage.Body.Item[0].SubItem[1].Value
                                };

                                if (secondaryMessage.Body.Item[0].SubItem[2].Value != null &&
                                    secondaryMessage.Body.Item[0].SubItem[2].Value.GetValue() != null)
                                {
                                    mapSetupData.FlatNotchLocation = (ulong)secondaryMessage.Body.Item[0].SubItem[2].Value;
                                }
                                else
                                {
                                    mapSetupData.FlatNotchLocation = null;
                                }

                                if (secondaryMessage.Body.Item[0].SubItem[3].Value != null &&
                                    secondaryMessage.Body.Item[0].SubItem[3].Value.GetValue() != null)
                                {
                                    mapSetupData.OriginLocation = (ulong)secondaryMessage.Body.Item[0].SubItem[3].Value;
                                }
                                else
                                {
                                    mapSetupData.OriginLocation = null;
                                }

                                mapSetupData.ReferencePointSelect = secondaryMessage.Body.Item[0].SubItem[4].Value;

                                for (int i = 0; i < secondaryMessage.Body.Item[0].SubItem[5].Length; i++)
                                {
                                    if (secondaryMessage.Body.Item[0].SubItem[5].SubItem[i].Value.GetValue() is IList list)
                                    {
                                        mapSetupData.ReferencePoint.Add(new ReferencePointItem()
                                        {
                                            X = Convert.ToInt64(list[0]),
                                            Y = Convert.ToInt64(list[1])
                                        });
                                    }
                                }

                                mapSetupData.DieUnitsOfMeasure = secondaryMessage.Body.Item[0].SubItem[6].Value;
                                mapSetupData.XAxisDieSize = secondaryMessage.Body.Item[0].SubItem[7].Value;
                                mapSetupData.YAxisDieSize = secondaryMessage.Body.Item[0].SubItem[8].Value;
                                mapSetupData.RowCount = secondaryMessage.Body.Item[0].SubItem[9].Value;
                                mapSetupData.ColumnCount = secondaryMessage.Body.Item[0].SubItem[10].Value;
                                mapSetupData.ProcessDieCount = secondaryMessage.Body.Item[0].SubItem[11].Value;
                                mapSetupData.BinCodeEquivalents = secondaryMessage.Body.Item[0].SubItem[12].Value;
                                mapSetupData.NullBinCodeValue = secondaryMessage.Body.Item[0].SubItem[13].Value;
                                mapSetupData.MessageLength = secondaryMessage.Body.Item[0].SubItem[14].Value;
                            }
                            else
                            {
                                mapSetupData = null;
                            }

                            this.OnResponseMapSetupData?.Invoke(mapSetupData);

                            if (mapSetupData != null)
                            {
                                logText = string.Format("S12F4(Map Set-up Data):MID={0}", mapSetupData.MaterialId);
                            }
                            else
                            {
                                logText = "S12F4(Map Set-up Data):MID is empty";
                            }

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F6]
                case 6:
                    {
                        int ack;

                        try
                        {
                            ack = secondaryMessage.Body.Item[0].Value;

                            this.OnResponseMapTransmitGrant?.Invoke(ack);

                            logText = string.Format("S12F6(Map Transmit Grant):GRNT1={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F8]
                case 8:
                    {
                        int ack;

                        try
                        {
                            ack = secondaryMessage.Body.Item[0].Value;

                            this.OnResponseMapDataAckType1?.Invoke(ack);

                            logText = string.Format("S12F8(Map Data Acknowledge Type 1):MDACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F10]
                case 10:
                    {
                        int ack;

                        try
                        {
                            ack = secondaryMessage.Body.Item[0].Value;

                            this.OnResponseMapDataAckType2?.Invoke(ack);

                            logText = string.Format("S12F10(Map Data Acknowledge Type 2):MDACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F12]
                case 12:
                    {
                        int ack;

                        try
                        {
                            ack = secondaryMessage.Body.Item[0].Value;

                            this.OnResponseMapDataAckType3?.Invoke(ack);

                            logText = string.Format("S12F12(Map Data Acknowledge Type 3):MDACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F14]
                case 14:
                    {
                        MapDataType1 mapData;

                        try
                        {
                            if (secondaryMessage.Body.Item[0].Length > 0)
                            {
                                mapData = new MapDataType1
                                {
                                    MaterialId = secondaryMessage.Body.Item[0].SubItem[0].Value,
                                    IDType = secondaryMessage.Body.Item[0].SubItem[1].Value
                                };

                                for (int i = 0; i < secondaryMessage.Body.Item[0].SubItem[2].Length; i++)
                                {
                                    if (secondaryMessage.Body.Item[0].SubItem[2].SubItem[i].SubItem[0].Value.GetValue() is IList list)
                                    {
                                        mapData.MapData.Add(new MapDataItem()
                                        {
                                            X = Convert.ToInt64(list[0]),
                                            Y = Convert.ToInt64(list[1]),
                                            Direction = Convert.ToInt64(list[2]),
                                            BinList = secondaryMessage.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].Value
                                        });
                                    }
                                }
                            }
                            else
                            {
                                mapData = null;
                            }

                            this.OnResponseMapDataType1?.Invoke(mapData);

                            if (mapData != null)
                            {
                                logText = string.Format("S12F14(Map Data Type 1):MID={0}", mapData.MaterialId);
                            }
                            else
                            {
                                logText = "S12F14(Map Data Type 1):MID is empty";
                            }

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F16]
                case 16:
                    {
                        MapDataType2 mapData;

                        try
                        {
                            if (secondaryMessage.Body.Item[0].Length > 0)
                            {
                                mapData = new MapDataType2
                                {
                                    MaterialId = secondaryMessage.Body.Item[0].SubItem[0].Value,
                                    IDType = secondaryMessage.Body.Item[0].SubItem[1].Value
                                };

                                if (secondaryMessage.Body.Item[0].SubItem[2].Value.GetValue() is IList list)
                                {
                                    mapData.StartPositionX = Convert.ToInt64(list[0]);
                                    mapData.StartPositionY = Convert.ToInt64(list[1]);
                                }

                                mapData.BinList = secondaryMessage.Body.Item[0].SubItem[3].Value;
                            }
                            else
                            {
                                mapData = null;
                            }

                            this.OnResponseMapDataType2?.Invoke(mapData);

                            if (mapData != null)
                            {
                                logText = string.Format("S12F16(Map Data Type 2):MID={0}", mapData.MaterialId);
                            }
                            else
                            {
                                logText = "S12F16(Map Data Type 2):MID is empty";
                            }

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S12F18]
                case 18:
                    {
                        MapDataType3 mapData;

                        try
                        {
                            if (secondaryMessage.Body.Item[0].Length > 0)
                            {
                                mapData = new MapDataType3
                                {
                                    MaterialId = secondaryMessage.Body.Item[0].SubItem[0].Value,
                                    IDType = secondaryMessage.Body.Item[0].SubItem[1].Value
                                };

                                for (int i = 0; i < secondaryMessage.Body.Item[0].SubItem[2].Length; i++)
                                {
                                    if (secondaryMessage.Body.Item[0].SubItem[2].SubItem[i].SubItem[0].Value.GetValue() is IList list)
                                    {

                                        mapData.MapData.Add(new MapDataItem()
                                        {
                                            X = Convert.ToInt64(list[0]),
                                            Y = Convert.ToInt64(list[1]),
                                            BinList = secondaryMessage.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].Value
                                        });
                                    }
                                }
                            }
                            else
                            {
                                mapData = null;
                            }

                            this.OnResponseMapDataType3?.Invoke(mapData);

                            if (mapData != null)
                            {
                                logText = string.Format("S12F18(Map Data Type 3):MID={0}", mapData.MaterialId);
                            }
                            else
                            {
                                logText = "S12F18(Map Data Type 3):MID is empty";
                            }

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream13(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S13F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S13F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private AnalyzeMessageError AnalyzeSecondaryMessageStream14(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            AnalyzeMessageError result;
            string logText;

            result = AnalyzeMessageError.Ok;

            switch (secondaryMessage.Function)
            {
                #region [S14F0]
                case 0:
                    {
                        logText = string.Format("Abort Transaction:State=S14F0, SystemBytes={0}", secondaryMessage.SystemBytes);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }

                    break;
                #endregion
                #region [S14F2]
                case 2:
                    {
                        int ack;
                        List<ObjectAttributeInfo> objectAttributeInfos;
                        List<ObjectErrorItem> objectErrorItems;
                        ObjectAttributeInfo objectAttributeInfo;
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            objectAttributeInfos = new List<ObjectAttributeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            if (secondaryMessage.Body.Item[0].SubItem[0] != null && secondaryMessage.Body.Item[0].SubItem[0].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[0].SubItem.Items)
                                {
                                    if (tempObject.SubItem[0].Value != null)
                                    {
                                        objectAttributeInfo = new ObjectAttributeInfo()
                                        {
                                            ObjectID = tempObject.SubItem[0].Value
                                        };

                                        foreach (SECSItem tempAttribute in tempObject.SubItem[1].SubItem.Items)
                                        {
                                            attributeInfo = new AttributeInfo()
                                            {
                                                AttributeID = tempAttribute.SubItem[0].Value
                                            };

                                            attributeDataItem = new AttributeDataItem();

                                            SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                            attributeInfo.AttributeData = attributeDataItem;

                                            objectAttributeInfo.Attributes.Add(attributeInfo);
                                        }

                                        objectAttributeInfos.Add(objectAttributeInfo);
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[1].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[1].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseGetAttributeData?.Invoke(ack, objectAttributeInfos, objectErrorItems);

                            logText = string.Format("S14F2(Get Attribute Data):OBJACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F4]
                case 4:
                    {
                        int ack;
                        List<ObjectAttributeInfo> objectAttributeInfos;
                        List<ObjectErrorItem> objectErrorItems;
                        ObjectAttributeInfo objectAttributeInfo;
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            objectAttributeInfos = new List<ObjectAttributeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            if (secondaryMessage.Body.Item[0].SubItem[0] != null && secondaryMessage.Body.Item[0].SubItem[0].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[0].SubItem.Items)
                                {
                                    if (tempObject.SubItem[0].Value != null)
                                    {
                                        objectAttributeInfo = new ObjectAttributeInfo()
                                        {
                                            ObjectID = tempObject.SubItem[0].Value
                                        };

                                        foreach (SECSItem tempAttribute in tempObject.SubItem[1].SubItem.Items)
                                        {
                                            attributeInfo = new AttributeInfo()
                                            {
                                                AttributeID = tempAttribute.SubItem[0].Value
                                            };

                                            attributeDataItem = new AttributeDataItem();

                                            SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                            attributeInfo.AttributeData = attributeDataItem;

                                            objectAttributeInfo.Attributes.Add(attributeInfo);
                                        }

                                        objectAttributeInfos.Add(objectAttributeInfo);
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[1].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[1].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseSetAttributeData?.Invoke(ack, objectAttributeInfos, objectErrorItems);

                            logText = string.Format("S14F4(Set Attribute Data):OBJACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F6]
                case 6:
                    {
                        int ack;
                        List<string> objectTypes;
                        List<ObjectErrorItem> objectErrorItems;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            objectTypes = new List<string>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            if (secondaryMessage.Body.Item[0].SubItem[0] != null && secondaryMessage.Body.Item[0].SubItem[0].Length > 0)
                            {
                                foreach (SECSItem tempType in secondaryMessage.Body.Item[0].SubItem[0].SubItem.Items)
                                {
                                    if (tempType.Value != null)
                                    {
                                        objectTypes.Add(tempType.Value);
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[1].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[1].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseGetTypeData?.Invoke(ack, objectTypes, objectErrorItems);

                            logText = string.Format("S14F6(Get Type Data):OBJACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F8]
                case 8:
                    {
                        int ack;
                        List<ObjectTypeInfo> objectTypeInfos;
                        List<ObjectErrorItem> objectErrorItems;
                        ObjectTypeInfo objectTypeInfo;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            objectTypeInfos = new List<ObjectTypeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            if (secondaryMessage.Body.Item[0].SubItem[0] != null && secondaryMessage.Body.Item[0].SubItem[0].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[0].SubItem.Items)
                                {
                                    if (tempObject.SubItem[0].Value != null)
                                    {
                                        objectTypeInfo = new ObjectTypeInfo()
                                        {
                                            ObjectType = tempObject.SubItem[0].Value
                                        };

                                        foreach (SECSItem tempAttribute in tempObject.SubItem[1].SubItem.Items)
                                        {
                                            objectTypeInfo.AttributeIDs.Add(tempAttribute.Value);
                                        }

                                        objectTypeInfos.Add(objectTypeInfo);
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[1].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[1].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseGetAttributeNameData?.Invoke(ack, objectTypeInfos, objectErrorItems);

                            logText = string.Format("S14F8(Get Attribute Name Data):OBJACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F10]
                case 10:
                    {
                        string objectSpec;
                        int ack;
                        List<AttributeInfo> attributeInfos;
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        List<ObjectErrorItem> objectErrorItems;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            attributeInfos = new List<AttributeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            objectSpec = secondaryMessage.Body.Item[0].SubItem[0].Value;

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[1].SubItem.Items)
                                {
                                    foreach (SECSItem tempAttribute in tempObject.SubItem[0].SubItem.Items)
                                    {
                                        attributeInfo = new AttributeInfo()
                                        {
                                            AttributeID = tempAttribute.SubItem[0].Value
                                        };

                                        attributeDataItem = new AttributeDataItem();

                                        SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                        attributeInfo.AttributeData = attributeDataItem;

                                        attributeInfos.Add(attributeInfo);
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[2] != null && secondaryMessage.Body.Item[0].SubItem[2].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[2].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[2].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[2].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseCreateObject?.Invoke(objectSpec, ack, attributeInfos, objectErrorItems);

                            logText = string.Format("S14F10(Create Object Acknowledge):OBJSPEC={0}, OBJACK={1}", objectSpec, ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F12]
                case 12:
                    {
                        int ack;
                        List<AttributeInfo> attributeInfos;
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        List<ObjectErrorItem> objectErrorItems;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            attributeInfos = new List<AttributeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            if (secondaryMessage.Body.Item[0].SubItem[0] != null && secondaryMessage.Body.Item[0].SubItem[0].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[0].SubItem.Items)
                                {
                                    if (tempObject.SubItem[0].Value != null)
                                    {
                                        foreach (SECSItem tempAttribute in tempObject.SubItem[0].SubItem.Items)
                                        {
                                            attributeInfo = new AttributeInfo()
                                            {
                                                AttributeID = tempAttribute.SubItem[0].Value
                                            };

                                            attributeDataItem = new AttributeDataItem();

                                            SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                            attributeInfo.AttributeData = attributeDataItem;

                                            attributeInfos.Add(attributeInfo);
                                        }
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[1].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[1].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseDeleteObject?.Invoke(ack, attributeInfos, objectErrorItems);

                            logText = string.Format("S14F12(Delete Object Acknowledge):OBJACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F14]
                case 14:
                    {
                        ulong objectToken;
                        int ack;
                        List<AttributeInfo> attributeInfos;
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        List<ObjectErrorItem> objectErrorItems;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            attributeInfos = new List<AttributeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            switch (secondaryMessage.Body.Item[0].SubItem[0].Format)
                            {
                                case SECSItemFormat.U1:
                                    {
                                        objectToken = (byte)secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                case SECSItemFormat.U2:
                                    {
                                        objectToken = (ushort)secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                case SECSItemFormat.U4:
                                    {
                                        objectToken = (uint)secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                case SECSItemFormat.U8:
                                    {
                                        objectToken = secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                case SECSItemFormat.B:
                                case SECSItemFormat.I1:
                                    {
                                        objectToken = (ulong)(sbyte)secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        objectToken = (ulong)(short)secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        objectToken = (ulong)(int)secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        objectToken = (ulong)(long)secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                                default:
                                    {
                                        objectToken = secondaryMessage.Body.Item[0].SubItem[0].Value;
                                    }
                                    break;
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[1].SubItem.Items)
                                {
                                    if (tempObject.SubItem[0].Value != null)
                                    {
                                        foreach (SECSItem tempAttribute in tempObject.SubItem[0].SubItem.Items)
                                        {
                                            attributeInfo = new AttributeInfo()
                                            {
                                                AttributeID = tempAttribute.SubItem[0].Value
                                            };

                                            attributeDataItem = new AttributeDataItem();

                                            SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                            attributeInfo.AttributeData = attributeDataItem;

                                            attributeInfos.Add(attributeInfo);
                                        }
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[2] != null && secondaryMessage.Body.Item[0].SubItem[2].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[2].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[2].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[2].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[2].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseObjectAttach?.Invoke(objectToken, ack, attributeInfos, objectErrorItems);

                            logText = string.Format("S14F14(Object Attach Acknowledge):OBJTOKEN={0}, OBJACK={1}", objectToken, ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F16]
                case 16:
                    {
                        int ack;
                        List<AttributeInfo> attributeInfos;
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        List<ObjectErrorItem> objectErrorItems;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            attributeInfos = new List<AttributeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            if (secondaryMessage.Body.Item[0].SubItem[0] != null && secondaryMessage.Body.Item[0].SubItem[0].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[0].SubItem.Items)
                                {
                                    if (tempObject.SubItem[0].Value != null)
                                    {
                                        foreach (SECSItem tempAttribute in tempObject.SubItem[0].SubItem.Items)
                                        {
                                            attributeInfo = new AttributeInfo()
                                            {
                                                AttributeID = tempAttribute.SubItem[0].Value
                                            };

                                            attributeDataItem = new AttributeDataItem();

                                            SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                            attributeInfo.AttributeData = attributeDataItem;

                                            attributeInfos.Add(attributeInfo);
                                        }
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[1].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[1].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseAttachedObjectAction?.Invoke(ack, attributeInfos, objectErrorItems);

                            logText = string.Format("S14F16(Attached Object Action Acknowledge):OBJACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                #region [S14F18]
                case 18:
                    {
                        int ack;
                        List<AttributeInfo> attributeInfos;
                        AttributeInfo attributeInfo;
                        AttributeDataItem attributeDataItem;
                        List<ObjectErrorItem> objectErrorItems;

                        try
                        {
                            ack = (int)OBJACK.Error;

                            attributeInfos = new List<AttributeInfo>();
                            objectErrorItems = new List<ObjectErrorItem>();

                            if (secondaryMessage.Body.Item[0].SubItem[0] != null && secondaryMessage.Body.Item[0].SubItem[0].Length > 0)
                            {
                                foreach (SECSItem tempObject in secondaryMessage.Body.Item[0].SubItem[0].SubItem.Items)
                                {
                                    if (tempObject.SubItem[0].Value != null)
                                    {
                                        foreach (SECSItem tempAttribute in tempObject.SubItem[0].SubItem.Items)
                                        {
                                            attributeInfo = new AttributeInfo()
                                            {
                                                AttributeID = tempAttribute.SubItem[0].Value
                                            };

                                            attributeDataItem = new AttributeDataItem();

                                            SetObjectAttributeData(tempAttribute.SubItem[1], ref attributeDataItem);

                                            attributeInfo.AttributeData = attributeDataItem;

                                            attributeInfos.Add(attributeInfo);
                                        }
                                    }
                                }
                            }

                            if (secondaryMessage.Body.Item[0].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].Length > 0)
                            {
                                ack = secondaryMessage.Body.Item[0].SubItem[1].SubItem[0].Value;

                                if (secondaryMessage.Body.Item[0].SubItem[1].SubItem[1] != null && secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].Length > 0)
                                {
                                    foreach (SECSItem tempError in secondaryMessage.Body.Item[0].SubItem[1].SubItem[1].SubItem.Items)
                                    {
                                        objectErrorItems.Add(new ObjectErrorItem()
                                        {
                                            ErrorCode = tempError.SubItem[0].Value,
                                            ErrorText = tempError.SubItem[1].Value
                                        });
                                    }
                                }
                            }

                            this.OnResponseSupervisedObjectAction?.Invoke(ack, attributeInfos, objectErrorItems);

                            logText = string.Format("S14F18(Supervised Object Action Acknowledge):OBJACK={0}", ack);

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        catch (Exception ex)
                        {
                            result = AnalyzeMessageError.Exception;

                            this._logger.WriteGEM(ex);
                        }
                    }

                    break;
                #endregion
                default:
                    result = AnalyzeMessageError.Undefined;
                    break;
            }

            return result;
        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream15(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S15F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S15F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream16(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S16F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S16F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

//        private AnalyzeMessageError AnalyzeSecondaryMessageStream17(SECSMessage primaryMessage, SECSMessage secondaryMessage)
//        {
//#if DEBUG_TRACE
//            DateTime startTime = DateTime.Now;
//#endif
//            AnalyzeMessageError result;
//            string logText;

//            result = AnalyzeMessageError.Ok;

//            switch (secondaryMessage.Function)
//            {
//                #region [S17F0]
//                case 0:
//                    {
//                        logText = string.Format("Abort Transaction:State=S17F0, SystemBytes={0}", secondaryMessage.SystemBytes);

//                        this._logger.WriteGEM(LogLevel.Information, logText);
//                    }

//                    break;
//                #endregion
//                default:
//                    result = AnalyzeMessageError.Undefined;
//                    break;
//            }
//#if DEBUG_TRACE
//            DebugFunctions.PrintMethodInfo(this, startTime, new System.Diagnostics.StackFrame());
//#endif
//            return result;
//        }

        private void RaiseInvalidRemoteCommand(SECSMessage message)
        {
            RemoteCommandInfo remoteCommandInfo;
            CommandParameterInfo commandParameterInfo;

            try
            {
                if (this.OnReceivedInvalidRemoteCommand != null)
                {
                    remoteCommandInfo = new RemoteCommandInfo
                    {
                        RemoteCommand = message.Body.Item[0].SubItem[0].Value
                    };

                    foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[1].SubItem.Items)
                    {
                        commandParameterInfo = new CommandParameterInfo()
                        {
                            Name = tempSECSItem.SubItem[0].Value,
                            Format = tempSECSItem.SubItem[1].Format,
                            Value = tempSECSItem.SubItem[1].Value
                        };

                        remoteCommandInfo.CommandParameter.Add(commandParameterInfo);
                    }

                    this.OnReceivedInvalidRemoteCommand(remoteCommandInfo);
                }
            }
            catch (Exception ex)
            {
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }
        }

        // 
        //S2F49 수신 후 첫번째 ECP를 첫번째 depth에 한해서 UbiGEM driver가 검증을 하는데...
        //오류 발생 시(RCMD, CEPName, format 오류 등...) S2F50(HCACK=1 or 3) 자동 응답을 하고
        //application으로 OnReceivedInvalidEnhancedRemoteCommand 이벤트를 발생 시킵니다.
        //// RaiseInvalidEnhancedRemoteCommand
        //이 부분에서 bug가 있어 이벤트 발생이 안되네요.
        //GemDriver.cs / RaiseInvalidEnhancedRemoteCommand method에서
        //if (this.OnReceivedInvalidRemoteCommand != null)
        //이 부분이
        //if (this.OnReceivedInvalidEnhancedRemoteCommand != null)
        //이렇게 변경되어야 합니다.
        //
        private void RaiseInvalidEnhancedRemoteCommand(SECSMessage message)
        {
            EnhancedRemoteCommandInfo remoteCommandInfo;
            EnhancedCommandParameterInfo commandParameterInfo;

            try
            {
                if (this.OnReceivedInvalidEnhancedRemoteCommand != null)
                {
                    remoteCommandInfo = new EnhancedRemoteCommandInfo
                    {
                        RemoteCommand = message.Body.Item[0].SubItem[2].Value
                    };

                    foreach (SECSItem tempSECSItem in message.Body.Item[0].SubItem[3].SubItem.Items)
                    {
                        commandParameterInfo = new EnhancedCommandParameterInfo()
                        {
                            Name = tempSECSItem.SubItem[0].Value,
                            Format = tempSECSItem.SubItem[1].Format
                        };

                        if (tempSECSItem.SubItem[1].SubItem.Count > 0 && tempSECSItem.SubItem[1].SubItem[0].Format == SECSItemFormat.L)
                        {
                            foreach (SECSItem tempSubSECSItem in tempSECSItem.SubItem[1].SubItem.Items)
                            {
                                commandParameterInfo.Items.Add(new EnhancedCommandParameterItem()
                                {
                                    Name = tempSubSECSItem.SubItem[0].Value,
                                    Value = tempSubSECSItem.SubItem[1].Value
                                });
                            }
                        }
                        else
                        {
                            commandParameterInfo.Value = tempSECSItem.SubItem[1].Value;
                        }

                        remoteCommandInfo.EnhancedCommandParameter.Add(commandParameterInfo);
                    }

                    this.OnReceivedInvalidEnhancedRemoteCommand(remoteCommandInfo);
                }
            }
            catch (Exception ex)
            {
                this._logger.WriteGEM(ex);
            }
            finally
            {
            }
        }

        private void SetObjectAttributeData(SECSItem secsItem, ref AttributeDataItem attributeDataItem)
        {
            AttributeDataItem childAttributeDataItem;

            attributeDataItem.AttributeDataFormat = secsItem.Format;

            if (secsItem.Format == SECSItemFormat.L)
            {
                foreach (SECSItem tempSECSItem in secsItem.SubItem.Items)
                {
                    childAttributeDataItem = new AttributeDataItem();

                    SetObjectAttributeData(tempSECSItem, ref childAttributeDataItem);

                    attributeDataItem.ChildItems.Add(childAttributeDataItem);
                }
            }
            else
            {
                if (secsItem.Value != null)
                {
                    attributeDataItem.AttributeData = secsItem.Value.ToString();
                }
            }
        }
    }
}