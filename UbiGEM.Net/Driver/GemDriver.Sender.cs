using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;
using UbiGEM.Net.Utility.Logger;

namespace UbiGEM.Net.Driver
{
    /// <summary>
    /// GEM Driver입니다.
    /// </summary>
    partial class GemDriver
    {
        /// <summary>
        /// HOST와 Establish Communication을 수행합니다.
        /// </summary>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError EstablishCommunication()
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CommunicationState communicationState;
            string logText;

            try
            {
                if (this._driver.Connected == true)
                {
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

                    if (ecInitCommunicationState == CommunicationState.Communicating ||
                        this._communicationState == CommunicationState.Communicating)
                    {
                        messageMakeError = this._messageMaker.MakeS1F13(this._variableCollection, out SECSMessage message, out string errorText);

                        if (messageMakeError == MessageMakeError.Ok)
                        {
                            driverResult = this._driver.SendSECSMessage(message);

                            if (driverResult == MessageError.Ok)
                            {
                                communicationState = CommunicationState.WaitCRA;
                                result = GemDriverError.Ok;
                                logText = "Transmission successful(S1F13), Communication State=" + communicationState.ToString();

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                communicationState = CommunicationState.WaitDelay;
                                result = GemDriverError.Ok;

                                if ((int.TryParse(this._variableCollection.GetVariableInfo(PreDefinedECV.EstablishCommunicationsTimeout.ToString()).Value.ToString(), out int dueTime) == true) &&
                                    (dueTime > 0))
                                {
                                    this._timerCommDelay.Change(dueTime * 1000, System.Threading.Timeout.Infinite);
                                }
                                else
                                {
                                    dueTime = 0;
                                }

                                result = GemDriverError.HSMSDriverError;
                                logText = string.Format("Transmission failure(S1F13), Result={0}, Communication State={1}, Establish Communication Timeout Interval={2}", messageMakeError, communicationState, dueTime * 1000);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        else
                        {
                            communicationState = CommunicationState.WaitDelay;
                            result = GemDriverError.Ok;

                            if ((int.TryParse(this._variableCollection.GetVariableInfo(PreDefinedECV.EstablishCommunicationsTimeout.ToString()).Value.ToString(), out int dueTime) == true) &&
                                (dueTime > 0))
                            {
                                this._timerCommDelay.Change(dueTime * 1000, System.Threading.Timeout.Infinite);
                            }
                            else
                            {
                                dueTime = 0;
                            }

                            result = GemDriverError.MessageMakeFailed;
                            logText = string.Format("Message make failure(S1F13), Result={0}, Communication State={1}, Establish Communication Timeout Interval={2}, Error Text={3}", messageMakeError, communicationState, dueTime * 1000, errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        communicationState = CommunicationState.WaitCRFromHost;
                        result = GemDriverError.Ok;

                        this._logger.WriteGEM(LogLevel.Information, "Communication State=" + communicationState.ToString());
                    }
                }
                else
                {
                    communicationState = CommunicationState.NotCommunication;
                    result = GemDriverError.HSMSDriverDisconnected;

                    this._logger.WriteGEM(LogLevel.Warning, "Failed to send:EstablishCommunication-Driver Disconnected");
                }

                if (this._communicationState != communicationState)
                {
                    this._communicationState = communicationState;

                    this.OnCommunicationStateChanged?.Invoke(this._communicationState);
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
        /// Are you there(S1F1)를 송신합니다.
        /// </summary>
        /// <returns></returns>
        public GemDriverError RequestAreYouThere()
        {
            GemDriverError result;
            MessageError driverResult;
            string logText;
            MessageMakeError messageMakeError;

            if (this._driver.Connected == true)
            {
                try
                {
                    messageMakeError = this._messageMaker.MakeS1F1(out SECSMessage message, out string errorText);

                    if (messageMakeError == MessageMakeError.Ok)
                    {
                        driverResult = this._driver.SendSECSMessage(message);

                        if (driverResult == MessageError.Ok)
                        {
                            result = GemDriverError.Ok;
                            logText = "Transmission successful(S1F1)";

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        else
                        {
                            result = GemDriverError.HSMSDriverError;
                            logText = "Transmission failure(S1F1)";

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        result = GemDriverError.MessageMakeFailed;
                        logText = string.Format("Message make failure(S1F1), Result={0}, Error Text={1}", messageMakeError, errorText);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                }
                catch (Exception ex)
                {
                    result = GemDriverError.Exception;

                    this._logger.WriteGEM(ex);
                }
            }
            else
            {
                result = GemDriverError.Disconnected;

                this._logger.WriteGEM(LogLevel.Warning, "Failed to send:RequestAreYouThere-Driver Disconnected");
            }

            return result;
        }

        /// <summary>
        /// Control State를 Equipment Offline으로 변경하고 Control state changed event를 HOST로 보고합니다.(S6F11) 또한, HOST 보고 등 오류가 발생해도 Equipment Offline으로 변경 됩니다.
        /// </summary>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestOffline()
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            ControlState controlState;
            CollectionEventInfo collectionEventInfo;
            VariableInfo controlStateVariableInfo;
            VariableInfo previousControlStateVariableInfo;
            string logText;

            controlState = this._controlState;

            try
            {
                collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ControlStateChanged.ToString());

                if (collectionEventInfo == null)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.Offline.ToString());
                }

                if (collectionEventInfo != null)
                {
                    controlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ControlState.ToString());
                    previousControlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousControlState.ToString());

                    result = CheckTransmittable("RequestOffline", false);

                    if (result == GemDriverError.Ok)
                    {
                        if (this._controlState != ControlState.EquipmentOffline)
                        {
                            if (controlStateVariableInfo != null)
                            {
                                controlStateVariableInfo.Value.SetValue((int)ControlState.EquipmentOffline);
                            }

                            if (previousControlStateVariableInfo != null)
                            {
                                previousControlStateVariableInfo.Value.SetValue((int)this._controlState);
                            }

                            CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ControlState, PreDefinedV.PreviousControlState);

                            messageMakeError = this._messageMaker.MakeEventReportWithoutEnableCheck(this._collectionEventCollection, this._variableCollection, collectionEventInfo.CEID, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    controlState = ControlState.EquipmentOffline;
                                    result = GemDriverError.Ok;
                                    logText = string.Format("Transmission successful(S6F11:({0}){1}), Control State={2}", collectionEventInfo.CEID, collectionEventInfo.Name, controlState);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;
                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.Ok;

                            this._logger.WriteGEM(LogLevel.Warning, "RequestOffline-Same State");
                        }
                    }

                    if (result != GemDriverError.Ok)
                    {
                        if (controlStateVariableInfo != null)
                        {
                            controlState = (ControlState)((int)(controlStateVariableInfo.Value));
                        }
                        else
                        {
                            controlState = ControlState.EquipmentOffline;
                        }
                    }

                    if (this._controlState != controlState)
                    {
                        if (controlStateVariableInfo != null)
                        {
                            controlStateVariableInfo.Value = (int)controlState;
                        }

                        if (previousControlStateVariableInfo != null)
                        {
                            previousControlStateVariableInfo.Value = (int)this._controlState;
                        }

                        this._controlState = controlState;

                        this.OnControlStateChanged?.Invoke(this._controlState);
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    this._logger.WriteGEM(LogLevel.Error, "RequestOffline-No Use Collection Event");
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
        /// Control State를 Online-Local로 변경합니다.
        /// </summary>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestOnlineLocal()
        {
            GemDriverError result;
            VariableInfo controlStateVariableInfo;
            VariableInfo previousControlStateVariableInfo;
            string logText;
            CollectionEventInfo collectionEventInfo;

            try
            {
                collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ControlStateChanged.ToString());

                if (collectionEventInfo == null)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.OnlineLocal.ToString());
                }

                if (collectionEventInfo != null)
                {
                    controlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ControlState.ToString());
                    previousControlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousControlState.ToString());

                    result = CheckTransmittable("RequestOnlineLocal", false);

                    if (result == GemDriverError.Ok)
                    {
                        switch (this._controlState)
                        {
                            case ControlState.EquipmentOffline:
                            case ControlState.HostOffline:
                                this._controlState = ControlState.AttemptOnline;

                                result = SendAreYouThere(ControlState.OnlineLocal);

                                if (result == GemDriverError.Ok)
                                {
                                    if (controlStateVariableInfo != null)
                                    {
                                        controlStateVariableInfo.Value = (int)ControlState.AttemptOnline;
                                    }

                                    if (previousControlStateVariableInfo != null)
                                    {
                                        previousControlStateVariableInfo.Value = (int)this._controlState;
                                    }

                                    this.OnControlStateChanged?.Invoke(this._controlState);
                                }
                                else
                                {
                                    logText = string.Format("AreYouThere failure, Result={0}", result);

                                    this._logger.WriteGEM(LogLevel.Warning, logText);
                                }

                                break;
                            case ControlState.OnlineLocal:
                                result = GemDriverError.SameState;

                                this._logger.WriteGEM(LogLevel.Warning, "RequestOnlineLocal-Same State");
                                break;
                            case ControlState.OnlineRemote:
                                result = SendControlState(ControlState.OnlineLocal);
                                break;
                            case ControlState.AttemptOnline:
                            default:
                                result = GemDriverError.Ok;
                                break;
                        }
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    this._logger.WriteGEM(LogLevel.Error, "RequestOnlineLocal-No Use Collection Event");
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
        /// Control State를 Online-Remote로 변경합니다.
        /// </summary>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestOnlineRemote()
        {
            GemDriverError result;
            VariableInfo controlStateVariableInfo;
            VariableInfo previousControlStateVariableInfo;
            string logText;
            CollectionEventInfo collectionEventInfo;

            try
            {
                collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ControlStateChanged.ToString());

                if (collectionEventInfo == null)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.OnlineRemote.ToString());
                }

                if (collectionEventInfo != null)
                {
                    controlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ControlState.ToString());
                    previousControlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousControlState.ToString());

                    result = CheckTransmittable("RequestOnlineRemote", false);

                    if (result == GemDriverError.Ok)
                    {
                        switch (this._controlState)
                        {
                            case ControlState.EquipmentOffline:
                            case ControlState.HostOffline:
                                this._controlState = ControlState.AttemptOnline;

                                result = SendAreYouThere(ControlState.OnlineRemote);

                                if (result == GemDriverError.Ok)
                                {
                                    if (controlStateVariableInfo != null)
                                    {
                                        controlStateVariableInfo.Value = (int)ControlState.AttemptOnline;
                                    }

                                    if (previousControlStateVariableInfo != null)
                                    {
                                        previousControlStateVariableInfo.Value = (int)this._controlState;
                                    }

                                    this.OnControlStateChanged?.Invoke(this._controlState);
                                }
                                else
                                {
                                    logText = string.Format("AreYouThere failure, Result={0}", result);

                                    this._logger.WriteGEM(LogLevel.Warning, logText);
                                }

                                break;
                            case ControlState.OnlineLocal:
                                result = SendControlState(ControlState.OnlineRemote);
                                break;
                            case ControlState.OnlineRemote:
                                result = GemDriverError.SameState;

                                this._logger.WriteGEM(LogLevel.Warning, "RequestOnlineRemote-Same State");
                                break;
                            case ControlState.AttemptOnline:
                            default:
                                result = GemDriverError.Ok;
                                break;
                        }
                    }
                }
                else
                {
                    result = GemDriverError.Undefined;

                    this._logger.WriteGEM(LogLevel.Error, "RequestOnlineRemote-No Use Collection Event");
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
        /// 
        /// </summary>
        /// <returns></returns>
        public GemDriverError RequestHostOffline()
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            ControlState controlState;
            CollectionEventInfo collectionEventInfo;
            VariableInfo controlStateVariableInfo;
            VariableInfo previousControlStateVariableInfo;
            string logText;

            controlState = ControlState.AttemptOnline;

            try
            {
                controlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ControlState.ToString());
                previousControlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousControlState.ToString());

                result = CheckTransmittable("RequestHostOffline", false);

                if (result == GemDriverError.Ok)
                {
                    if (this._controlState != ControlState.AttemptOnline)
                    {
                        collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.OfflineOnHost.ToString());

                        if (collectionEventInfo == null)
                        {
                            collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ControlStateChanged.ToString());

                            if (collectionEventInfo == null)
                            {
                                collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.Offline.ToString());
                            }
                        }

                        if (collectionEventInfo != null)
                        {
                            if (controlStateVariableInfo != null)
                            {
                                controlStateVariableInfo.Value = (int)ControlState.HostOffline;
                            }

                            if (previousControlStateVariableInfo != null)
                            {
                                previousControlStateVariableInfo.Value = (int)this._controlState;
                            }

                            CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ControlState, PreDefinedV.PreviousControlState);

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    controlState = ControlState.HostOffline;
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1}), Control State={2}", collectionEventInfo.CEID, collectionEventInfo.Name, controlState);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.Unknown;
                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "RequestHostOffline-No Use Collection Event");
                        }
                    }
                    else
                    {
                        result = GemDriverError.Ok;

                        this._logger.WriteGEM(LogLevel.Warning, "RequestHostOffline-Same State");
                    }
                }

                if (result != GemDriverError.Ok)
                {
                    if (controlStateVariableInfo != null)
                    {
                        controlState = (ControlState)((int)(controlStateVariableInfo.Value));
                    }
                    else
                    {
                        controlState = ControlState.EquipmentOffline;
                    }
                }

                if (this._controlState != controlState)
                {
                    if (controlStateVariableInfo != null)
                    {
                        controlStateVariableInfo.Value = (int)controlState;
                    }

                    if (previousControlStateVariableInfo != null)
                    {
                        previousControlStateVariableInfo.Value = (int)this._controlState;
                    }

                    this._controlState = controlState;

                    this.OnControlStateChanged?.Invoke(this._controlState);
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
        /// Equipment Processing State changed 이벤트를 HOST로 보고합니다.
        /// </summary>
        /// <param name="equipmentProcessState">변경 할 Equipment Processing State입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportEquipmentProcessingState(byte equipmentProcessState)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            VariableInfo variableInfo;
            string logText;

            try
            {
                result = CheckTransmittable("ReportEquipmentProcessingState");

                if (result == GemDriverError.Ok)
                {
                    // 4pemtron
                    if (this._equipmentProcessState != equipmentProcessState)
                    {
                        collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ProcessStateChanged.ToString());

                        if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                        {
                            CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock);
                            // 4pemtron
                            variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousProcessState.ToString());
                            if (variableInfo != null)
                            {
                                variableInfo.Value = this._equipmentProcessState; 
                            }

                            variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ProcessState.ToString());

                            if (variableInfo != null)
                            {
                                variableInfo.Value = equipmentProcessState;
                            }

                            this._equipmentProcessState = equipmentProcessState;

                            this.OnEquipmentProcessState?.Invoke(this._equipmentProcessState);

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    //variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousProcessState.ToString());
                                    //if (variableInfo != null)
                                    //{
                                    //    variableInfo.Value = this._equipmentProcessState;
                                    //}

                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1}), Process State={2}", collectionEventInfo.CEID, collectionEventInfo.Name, equipmentProcessState);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;
                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        else
                        {
                            if (collectionEventInfo != null)
                            {
                                result = GemDriverError.Undefined;

                                this._logger.WriteGEM(LogLevel.Error, "ReportEquipmentProcessingState-Undefined Collection Event");
                            }
                            else
                            {
                                result = GemDriverError.Disabled;

                                this._logger.WriteGEM(LogLevel.Error, string.Format("ReportEquipmentProcessingState-Disabled Collection Event:({0}){1}", collectionEventInfo.CEID, collectionEventInfo.Name));
                            }
                        }
                    }
                    // 4pemtron
                    else
                    {
                        result = GemDriverError.SameState;

                        this._logger.WriteGEM(LogLevel.Warning, "ReportEquipmentProcessingState-Same State");
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
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ceid">보고 할 Collection Event ID입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(string ceid)
        {
            return ReportCollectionEvent(ceid, true);
        }

        /// <summary>
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="collectionEventInfo">보고 할 Collection Event 정보입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(CollectionEventInfo collectionEventInfo)
        {
            return ReportCollectionEvent(collectionEventInfo, false);
        }

        /// <summary>
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ceid">보고 할 Collection Event ID입니다.</param>
        /// <param name="secsItems">Event를 구성할 Items입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(string ceid, SECSItemCollection secsItems)
        {
            GemDriverError result;
            CollectionEventInfo collectionEventInfo;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            bool sendEvent;
            string logText;

            try
            {
                result = CheckTransmittable("ReportCollectionEvent");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection[ceid];

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        result = GemDriverError.Ok;

                        if (collectionEventInfo.PreDefined == true && Enum.TryParse<PreDefinedCE>(collectionEventInfo.Name, out PreDefinedCE preDefinedCollectionEvent) == true)
                        {
                            switch (preDefinedCollectionEvent)
                            {
                                case PreDefinedCE.Offline:
                                    result = RequestOffline();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineLocal:
                                    result = RequestOnlineLocal();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineRemote:
                                    result = RequestOnlineRemote();
                                    sendEvent = true;
                                    break;
                                default:
                                    sendEvent = false;
                                    break;
                            }
                        }
                        else
                        {
                            sendEvent = false;
                        }

                        if (sendEvent == false)
                        {
                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, secsItems, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;

                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Undefined Collection Event");
                        }
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
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ceid">보고 할 Collection Event ID입니다.</param>
        /// <param name="useRaiseEvent">DVVAL update event 발생 여부입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(string ceid, bool useRaiseEvent)
        {
            GemDriverError result;
            CollectionEventInfo collectionEventInfo;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            bool sendEvent;
            string logText;

            try
            {
                result = CheckTransmittable("ReportCollectionEvent");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection[ceid];

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        result = GemDriverError.Ok;

                        if (collectionEventInfo.PreDefined == true && Enum.TryParse<PreDefinedCE>(collectionEventInfo.Name, out PreDefinedCE preDefinedCollectionEvent) == true)
                        {
                            switch (preDefinedCollectionEvent)
                            {
                                case PreDefinedCE.Offline:
                                    result = RequestOffline();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineLocal:
                                    result = RequestOnlineLocal();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineRemote:
                                    result = RequestOnlineRemote();
                                    sendEvent = true;
                                    break;
                                default:
                                    sendEvent = false;
                                    break;
                            }
                        }
                        else
                        {
                            sendEvent = false;
                        }

                        if (sendEvent == false)
                        {
                            if (useRaiseEvent == true)
                            {
                                CheckVariableUpdateRequest(collectionEventInfo, useRaiseEvent, PreDefinedV.Clock);
                            }

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;

                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Undefined Collection Event");
                        }
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
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="collectionEventInfo">보고 할 Collection Event 정보입니다.</param>
        /// <param name="useRaiseEvent">DVVAL update event 발생 여부입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(CollectionEventInfo collectionEventInfo, bool useRaiseEvent)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            bool sendEvent;
            string logText;

            try
            {
                result = CheckTransmittable("ReportCollectionEvent");

                if (result == GemDriverError.Ok)
                {
                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        result = GemDriverError.Ok;

                        if (collectionEventInfo.PreDefined == true && Enum.TryParse<PreDefinedCE>(collectionEventInfo.Name, out PreDefinedCE preDefinedCollectionEvent) == true)
                        {
                            switch (preDefinedCollectionEvent)
                            {
                                case PreDefinedCE.Offline:
                                    result = RequestOffline();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineLocal:
                                    result = RequestOnlineLocal();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineRemote:
                                    result = RequestOnlineRemote();
                                    sendEvent = true;
                                    break;
                                default:
                                    sendEvent = false;
                                    break;
                            }
                        }
                        else
                        {
                            sendEvent = false;
                        }

                        if (sendEvent == false)
                        {
                            if (useRaiseEvent == true)
                            {
                                CheckVariableUpdateRequest(collectionEventInfo, useRaiseEvent, PreDefinedV.Clock);
                            }

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;

                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Undefined Collection Event");
                        }
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
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ceid">보고 할 Collection Event ID입니다.</param>
        /// <param name="variables">Message를 생성할 현재 variable 리스트입니다. (포함되지 않은 variable이 link 되어 있을 경우 전체 variable에서 찾음)</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(string ceid, List<VariableInfo> variables)
        {
            return ReportCollectionEvent(ceid, variables, false);
        }

        /// <summary>
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ceid">보고 할 Collection Event ID입니다.</param>
        /// <param name="variables">Message를 생성할 현재 variable 리스트입니다. (포함되지 않은 variable이 link 되어 있을 경우 전체 variable에서 찾음)</param>
        /// <param name="useRaiseEvent">DVVAL update event 발생 여부입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(string ceid, List<VariableInfo> variables, bool useRaiseEvent)
        {
            GemDriverError result;
            CollectionEventInfo collectionEventInfo;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            bool sendEvent;
            string logText;

            try
            {
                result = CheckTransmittable("ReportCollectionEvent");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection[ceid];

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        result = GemDriverError.Ok;

                        if (collectionEventInfo.PreDefined == true && Enum.TryParse<PreDefinedCE>(collectionEventInfo.Name, out PreDefinedCE preDefinedCollectionEvent) == true)
                        {
                            switch (preDefinedCollectionEvent)
                            {
                                case PreDefinedCE.Offline:
                                    result = RequestOffline();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineLocal:
                                    result = RequestOnlineLocal();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineRemote:
                                    result = RequestOnlineRemote();
                                    sendEvent = true;
                                    break;
                                default:
                                    sendEvent = false;
                                    break;
                            }
                        }
                        else
                        {
                            sendEvent = false;
                        }

                        if (sendEvent == false)
                        {
                            if (useRaiseEvent == true)
                            {
                                CheckVariableUpdateRequest(collectionEventInfo, useRaiseEvent, PreDefinedV.Clock);
                            }

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, variables, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;

                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Undefined Collection Event");
                        }
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
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ceid">보고 할 Collection Event ID입니다.</param>
        /// <param name="vids">Message를 생성할 현재 VID 리스트입니다. (포함되지 않은 variable이 link 되어 있을 경우 전체 variable에서 찾음)</param>
        /// <param name="values">Message를 생성할 현재 value 리스트입니다. (포함되지 않은 variable이 link 되어 있을 경우 전체 variable에서 찾음)</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(string ceid, List<string> vids, List<string> values)
        {
            return ReportCollectionEvent(ceid, vids, values, false);
        }

        /// <summary>
        /// Collection Event를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="ceid">보고 할 Collection Event ID입니다.</param>
        /// <param name="vids">Message를 생성할 현재 VID 리스트입니다. (포함되지 않은 variable이 link 되어 있을 경우 전체 variable에서 찾음)</param>
        /// <param name="values">Message를 생성할 현재 value 리스트입니다. (포함되지 않은 variable이 link 되어 있을 경우 전체 variable에서 찾음)</param>
        /// <param name="useRaiseEvent"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportCollectionEvent(string ceid, List<string> vids, List<string> values, bool useRaiseEvent)
        {
            GemDriverError result;
            CollectionEventInfo collectionEventInfo;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            bool sendEvent;
            string logText;

            try
            {
                if (vids == null)
                {
                    logText = string.Format("ReportCollectionEvent-VIDS is null:ceid={0}", ceid);

                    this._logger.WriteGEM(LogLevel.Error, logText);

                    return GemDriverError.MessageMakeFailed;
                }

                if (values == null)
                {
                    logText = string.Format("ReportCollectionEvent-Values is null:ceid={0}", ceid);

                    this._logger.WriteGEM(LogLevel.Error, logText);

                    return GemDriverError.MessageMakeFailed;
                }

                if (vids.Count != values.Count)
                {
                    logText = string.Format("ReportCollectionEvent-Count mismatch:ceid={0}, VID={1}, Value={2}", ceid, vids.Count, values.Count);

                    this._logger.WriteGEM(LogLevel.Error, logText);

                    return GemDriverError.Mismatch;
                }

                result = CheckTransmittable("ReportCollectionEvent");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection[ceid];

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        result = GemDriverError.Ok;

                        if (collectionEventInfo.PreDefined == true && Enum.TryParse<PreDefinedCE>(collectionEventInfo.Name, out PreDefinedCE preDefinedCollectionEvent) == true)
                        {
                            switch (preDefinedCollectionEvent)
                            {
                                case PreDefinedCE.Offline:
                                    result = RequestOffline();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineLocal:
                                    result = RequestOnlineLocal();
                                    sendEvent = true;
                                    break;
                                case PreDefinedCE.OnlineRemote:
                                    result = RequestOnlineRemote();
                                    sendEvent = true;
                                    break;
                                default:
                                    sendEvent = false;
                                    break;
                            }
                        }
                        else
                        {
                            sendEvent = false;
                        }

                        if (sendEvent == false)
                        {
                            if (useRaiseEvent == true)
                            {
                                CheckVariableUpdateRequest(collectionEventInfo, useRaiseEvent, PreDefinedV.Clock);
                            }

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, vids, values, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;

                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportCollectionEvent-Undefined Collection Event");
                        }
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
        /// Alarm 발생 보고입니다.
        /// </summary>
        /// <param name="alarmId">발생 보고할 Alarm ID입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportAlarmSet(long alarmId)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            AlarmInfo alarmInfo;
            CollectionEventInfo collectionEventInfo;
            string logText;
            VariableInfo variableInfo;

            try
            {
                result = CheckTransmittable("ReportAlarmSet");

                if (result == GemDriverError.Ok)
                {
                    alarmInfo = this._alarmCollection[alarmId];

                    if (alarmInfo == null)
                    {
                        alarmInfo = new AlarmInfo()
                        {
                            Code = 0,
                            ID = alarmId,
                            Description = "Undefined Alarm",
                            Enabled = true
                        };

                        result = GemDriverError.Undefined;

                        this._logger.WriteGEM(LogLevel.Warning, "ReportAlarmSet-Undefined Alarm");
                    }

                    if (alarmInfo != null && alarmInfo.Enabled == true)
                    {
                        message = this._driver.Messages.GetMessageHeader(5, 1);

                        message.Body.Add(SECSItemFormat.L, 3, null);
                        message.Body.Add("ALCD", GetSECSFormat(PreDefinedDataDictinary.ALCD, SECSItemFormat.U1), 1, (alarmInfo.Code + 128));
                        message.Body.Add("ALID", GetSECSFormat(PreDefinedDataDictinary.ALID, SECSItemFormat.U2), 1, alarmInfo.ID);
                        message.Body.Add("ALTX", GetSECSFormat(PreDefinedDataDictinary.ALTX, SECSItemFormat.A), Encoding.Default.GetByteCount(alarmInfo.Description), alarmInfo.Description);

                        driverResult = this._driver.SendSECSMessage(message);

                        if (driverResult == MessageError.Ok)
                        {
                            result = GemDriverError.Ok;

                            logText = string.Format("Transmission successful(S5F1:Alarm=({0}){1}", alarmInfo.ID, alarmInfo.Description);

                            this._logger.WriteGEM(LogLevel.Information, logText);

                            collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.AlarmSet.ToString());

                            if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                            {
                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALID.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.ID);
                                }

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALCD.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.Code);
                                }

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALTX.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.Description);
                                }

                                CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ALID, PreDefinedV.ALCD, PreDefinedV.ALTX);

                                messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out message, out string errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.SendSECSMessage(message);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        result = GemDriverError.Ok;

                                        logText = string.Format("Transmission successful(S6F11:({0}){1}), Alarm=({2}){3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, alarmInfo.ID, alarmInfo.Description);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        result = GemDriverError.HSMSDriverError;
                                        logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    result = GemDriverError.MessageMakeFailed;
                                    logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, messageMakeError, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        else
                        {
                            result = GemDriverError.HSMSDriverError;
                            logText = string.Format("Transmission failure(S5F1):Result={0}", driverResult);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (alarmInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, string.Format("ReportAlarmSet-Disabled Alarm:({0}){1}", alarmInfo.ID, alarmInfo.Description));
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportAlarmSet-Undefined Alarm");
                        }
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
        /// Alarm 발생 보고입니다.
        /// </summary>
        /// <param name="alarmId">발생 보고할 Alarm ID입니다.</param>
        /// <param name="alarmCd">발생 보고할 Alarm Code입니다.</param>
        /// <param name="alarmText">발생 보고할 Alarm Text입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportAlarmSet(long alarmId, int alarmCd, string alarmText)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            AlarmInfo alarmInfo;
            CollectionEventInfo collectionEventInfo;
            string logText;
            VariableInfo variableInfo;

            try
            {
                result = CheckTransmittable("ReportAlarmSet");

                if (result == GemDriverError.Ok)
                {
                    alarmInfo = this._alarmCollection[alarmId];

                    if (alarmInfo == null)
                    {
                        alarmInfo = new AlarmInfo()
                        {
                            Code = alarmCd,
                            ID = alarmId,
                            Description = alarmText,
                            Enabled = true
                        };
                    }
                    else
                    {
                        alarmInfo.Code = alarmCd;
                        alarmInfo.Description = alarmText;
                    }

                    if (alarmInfo != null && alarmInfo.Enabled == true)
                    {
                        message = this._driver.Messages.GetMessageHeader(5, 1);

                        message.Body.Add(SECSItemFormat.L, 3, null);
                        message.Body.Add("ALCD", GetSECSFormat(PreDefinedDataDictinary.ALCD, SECSItemFormat.U1), 1, (alarmInfo.Code + 128));
                        message.Body.Add("ALID", GetSECSFormat(PreDefinedDataDictinary.ALID, SECSItemFormat.U2), 1, alarmInfo.ID);
                        message.Body.Add("ALTX", GetSECSFormat(PreDefinedDataDictinary.ALTX, SECSItemFormat.A), Encoding.Default.GetByteCount(alarmInfo.Description), alarmInfo.Description);

                        driverResult = this._driver.SendSECSMessage(message);

                        if (driverResult == MessageError.Ok)
                        {
                            result = GemDriverError.Ok;

                            logText = string.Format("Transmission successful(S5F1:Alarm=({0}){1}", alarmInfo.ID, alarmInfo.Description);

                            this._logger.WriteGEM(LogLevel.Information, logText);

                            collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.AlarmSet.ToString());

                            if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                            {
                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALID.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.ID);
                                }

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALCD.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.Code);
                                }

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALTX.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.Description);
                                }

                                CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ALID, PreDefinedV.ALCD, PreDefinedV.ALTX);

                                messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out message, out string errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.SendSECSMessage(message);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        result = GemDriverError.Ok;

                                        logText = string.Format("Transmission successful(S6F11:({0}){1}), Alarm=({2}){3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, alarmInfo.ID, alarmInfo.Description);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        result = GemDriverError.HSMSDriverError;
                                        logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    result = GemDriverError.MessageMakeFailed;
                                    logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, messageMakeError, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        else
                        {
                            result = GemDriverError.HSMSDriverError;
                            logText = string.Format("Transmission failure(S5F1):Result={0}", driverResult);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (alarmInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, string.Format("ReportAlarmSet-Disabled Alarm:({0}){1}", alarmInfo.ID, alarmInfo.Description));
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportAlarmSet-Undefined Alarm");
                        }
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
        /// Alarm 해제 보고입니다.
        /// </summary>
        /// <param name="alarmId">해제 보고할 Alarm ID입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportAlarmClear(long alarmId)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            AlarmInfo alarmInfo;
            CollectionEventInfo collectionEventInfo;
            string logText;
            VariableInfo variableInfo;

            try
            {
                result = CheckTransmittable("ReportAlarmClear");

                if (result == GemDriverError.Ok)
                {
                    alarmInfo = this._alarmCollection[alarmId];

                    if (alarmInfo == null)
                    {
                        alarmInfo = new AlarmInfo()
                        {
                            Code = 0,
                            ID = alarmId,
                            Description = "Undefined Alarm",
                            Enabled = true
                        };

                        result = GemDriverError.Undefined;

                        this._logger.WriteGEM(LogLevel.Warning, "ReportAlarmSet-Undefined Alarm");
                    }

                    if (alarmInfo != null && alarmInfo.Enabled == true)
                    {
                        message = this._driver.Messages.GetMessageHeader(5, 1);

                        message.Body.Add(SECSItemFormat.L, 3, null);
                        message.Body.Add("ALCD", GetSECSFormat(PreDefinedDataDictinary.ALCD, SECSItemFormat.U1), 1, alarmInfo.Code);
                        message.Body.Add("ALID", GetSECSFormat(PreDefinedDataDictinary.ALID, SECSItemFormat.U2), 1, alarmInfo.ID);
                        message.Body.Add("ALTX", GetSECSFormat(PreDefinedDataDictinary.ALTX, SECSItemFormat.A), Encoding.Default.GetByteCount(alarmInfo.Description), alarmInfo.Description);

                        driverResult = this._driver.SendSECSMessage(message);

                        if (driverResult == MessageError.Ok)
                        {
                            result = GemDriverError.Ok;

                            logText = string.Format("Transmission successful(S5F1:Alarm=({0}){1}", alarmInfo.ID, alarmInfo.Description);

                            this._logger.WriteGEM(LogLevel.Information, logText);

                            collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.AlarmClear.ToString());

                            if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                            {
                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALID.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.ID);
                                }

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALCD.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.Code);
                                }

                                CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ALID, PreDefinedV.ALCD);

                                messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out message, out string errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.SendSECSMessage(message);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        result = GemDriverError.Ok;

                                        logText = string.Format("Transmission successful(S6F11:({0}){1}), Alarm=({2}){3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, alarmInfo.ID, alarmInfo.Description);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        result = GemDriverError.HSMSDriverError;
                                        logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    result = GemDriverError.MessageMakeFailed;
                                    logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, messageMakeError, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        else
                        {
                            result = GemDriverError.HSMSDriverError;
                            logText = string.Format("Transmission failure(S5F1):Result={0}", driverResult);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (alarmInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, string.Format("ReportAlarmClear-Disabled Alarm:({0}){1}", alarmInfo.ID, alarmInfo.Description));
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportAlarmClear-Undefined Alarm");
                        }
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
        /// Alarm 해제 보고입니다.
        /// </summary>
        /// <param name="alarmId">해제 보고할 Alarm ID입니다.</param>
        /// <param name="alarmCd">해제 보고할 Alarm Code입니다.</param>
        /// <param name="alarmText">해제 보고할 Alarm Text입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportAlarmClear(long alarmId, int alarmCd, string alarmText)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            AlarmInfo alarmInfo;
            CollectionEventInfo collectionEventInfo;
            string logText;
            VariableInfo variableInfo;

            try
            {
                result = CheckTransmittable("ReportAlarmClear");

                if (result == GemDriverError.Ok)
                {
                    alarmInfo = this._alarmCollection[alarmId];

                    if (alarmInfo == null)
                    {
                        alarmInfo = new AlarmInfo()
                        {
                            Code = alarmCd,
                            ID = alarmId,
                            Description = alarmText,
                            Enabled = true
                        };
                    }
                    else
                    {
                        alarmInfo.Code = alarmCd;
                        alarmInfo.Description = alarmText;
                    }

                    if (alarmInfo != null && alarmInfo.Enabled == true)
                    {
                        message = this._driver.Messages.GetMessageHeader(5, 1);

                        message.Body.Add(SECSItemFormat.L, 3, null);
                        message.Body.Add("ALCD", GetSECSFormat(PreDefinedDataDictinary.ALCD, SECSItemFormat.U1), 1, alarmInfo.Code);
                        message.Body.Add("ALID", GetSECSFormat(PreDefinedDataDictinary.ALID, SECSItemFormat.U2), 1, alarmInfo.ID);
                        message.Body.Add("ALTX", GetSECSFormat(PreDefinedDataDictinary.ALTX, SECSItemFormat.A), Encoding.Default.GetByteCount(alarmInfo.Description), alarmInfo.Description);

                        driverResult = this._driver.SendSECSMessage(message);

                        if (driverResult == MessageError.Ok)
                        {
                            result = GemDriverError.Ok;

                            logText = string.Format("Transmission successful(S5F1:Alarm=({0}){1}", alarmInfo.ID, alarmInfo.Description);

                            this._logger.WriteGEM(LogLevel.Information, logText);

                            collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.AlarmClear.ToString());

                            if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                            {
                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALID.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.ID);
                                }

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ALCD.ToString());

                                if (variableInfo != null)
                                {
                                    variableInfo.Value.SetValue(alarmInfo.Code);
                                }

                                CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ALID, PreDefinedV.ALCD);

                                messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out message, out string errorText);

                                if (messageMakeError == MessageMakeError.Ok)
                                {
                                    driverResult = this._driver.SendSECSMessage(message);

                                    if (driverResult == MessageError.Ok)
                                    {
                                        result = GemDriverError.Ok;

                                        logText = string.Format("Transmission successful(S6F11:({0}){1}), Alarm=({2}){3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, alarmInfo.ID, alarmInfo.Description);

                                        this._logger.WriteGEM(LogLevel.Information, logText);
                                    }
                                    else
                                    {
                                        result = GemDriverError.HSMSDriverError;
                                        logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, driverResult);

                                        this._logger.WriteGEM(LogLevel.Error, logText);
                                    }
                                }
                                else
                                {
                                    result = GemDriverError.MessageMakeFailed;
                                    logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID + alarmId, collectionEventInfo.Name, messageMakeError, errorText);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                        }
                        else
                        {
                            result = GemDriverError.HSMSDriverError;
                            logText = string.Format("Transmission failure(S5F1):Result={0}", driverResult);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (alarmInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, string.Format("ReportAlarmClear-Disabled Alarm:({0}){1}", alarmInfo.ID, alarmInfo.Description));
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportAlarmClear-Undefined Alarm");
                        }
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
        /// Terminal Message를 HOST로 송신합니다.(S10F1)
        /// </summary>
        /// <param name="tid">보고할 Terminal ID입니다.</param>
        /// <param name="terminalMessage">보고할 Terminal Message입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportTerminalMessage(int tid, string terminalMessage)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = CheckTransmittable("ReportTerminalMessage");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(10, 1);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add("TID", GetSECSFormat(PreDefinedDataDictinary.TID, SECSItemFormat.B), 1, tid);

                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.TEXT, SECSItemFormat.A);

                    if (secsItemFormat == SECSItemFormat.A)
                    {
                        message.Body.Add("TEXT", secsItemFormat, Encoding.Default.GetByteCount(terminalMessage), terminalMessage);
                    }
                    else
                    {
                        message.Body.Add("TEXT", secsItemFormat, 1, terminalMessage);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = string.Format("Transmission successful(S10F1:TID={0}, Text={1}", tid, terminalMessage);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S10F1):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// HOST에서 EC Change 명령을 수신 시 정상일 경우(S2F15) ECV changed 이벤트를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="variableInfo">변경된 ECV입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportEquipmentConstantChangedByHost(VariableInfo variableInfo)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            string logText;

            try
            {
                result = CheckTransmittable("ReportEquipmentConstantChangedByHost");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.EquipmentConstantChangedByHost.ToString());

                    if (collectionEventInfo == null)
                    {
                        collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.EquipmentConstantChanged.ToString());
                    }

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        messageMakeError = this._messageMaker.MakeEventReportByECChanged(collectionEventInfo, this._variableCollection, variableInfo, out SECSMessage message, out string errorText);

                        if (messageMakeError == MessageMakeError.Ok)
                        {
                            driverResult = this._driver.SendSECSMessage(message);

                            if (driverResult == MessageError.Ok)
                            {
                                result = GemDriverError.Ok;

                                logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                result = GemDriverError.HSMSDriverError;

                                logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.MessageMakeFailed;
                            logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "ReportEquipmentConstantChangedByHost-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportEquipmentConstantChangedByHost-Undefined Collection Event");
                        }
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
        /// HOST에서 EC Change 명령을 수신 시 정상일 경우(S2F15) ECV changed 이벤트를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="variableInfos">변경된 ECV list입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportEquipmentConstantChangedByHost(List<VariableInfo> variableInfos)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            string logText;

            try
            {
                result = CheckTransmittable("ReportEquipmentConstantChangedByHost");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.EquipmentConstantChangedByHost.ToString());

                    if (collectionEventInfo == null)
                    {
                        collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.EquipmentConstantChanged.ToString());
                    }

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ChangedECID, PreDefinedV.ChangedECV, PreDefinedV.ChangedECList);

                        messageMakeError = this._messageMaker.MakeEventReportByECChanged(collectionEventInfo, this._variableCollection, variableInfos, out SECSMessage message, out string errorText);

                        if (messageMakeError == MessageMakeError.Ok)
                        {
                            driverResult = this._driver.SendSECSMessage(message);

                            if (driverResult == MessageError.Ok)
                            {
                                result = GemDriverError.Ok;

                                logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                result = GemDriverError.HSMSDriverError;

                                logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.MessageMakeFailed;
                            logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "ReportEquipmentConstantChangedByHost-Undefined Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "ReportEquipmentConstantChangedByHost-Disabled Collection Event");
                        }
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
        /// Conversation Timeout을 HOST로 보고합니다.(S9F13)
        /// </summary>
        /// <param name="mexp">SxxFyy 형식의 Message 구분입니다.</param>
        /// <param name="edid">식별 데이터입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReportConversationTimeout(string mexp, string edid)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = CheckTransmittable("ReportConversationTimeout");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(9, 13);

                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.EDID, SECSItemFormat.A);

                    message.Body.Add(SECSItemFormat.L, 2, null);
                    message.Body.Add("MEXP", SECSItemFormat.A, Encoding.Default.GetByteCount(mexp), mexp);

                    if (secsItemFormat == SECSItemFormat.A)
                    {
                        message.Body.Add("EDID", secsItemFormat, Encoding.Default.GetByteCount(edid), edid);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(edid) == false)
                        {
                            if (secsItemFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("EDID", secsItemFormat, Encoding.Default.GetByteCount(edid), edid);
                            }
                            else
                            {
                                message.Body.Add("EDID", secsItemFormat, 1, edid);
                            }
                        }
                        else
                        {
                            message.Body.Add("EDID", secsItemFormat, 0, string.Empty);
                        }
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = string.Format("Transmission successful(S9F13:MEXP={0}, EDID={1}", mexp, edid);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S9F13):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Loopback data를 HOST로 송신합니다.(S2F25)
        /// </summary>
        /// <param name="abs">송신할 Data입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestLoopback(List<byte> abs)
        {
            GemDriverError result;
            MessageError driverResult;
            string logText;
            StringBuilder sb;
            MessageMakeError messageMakeError;

            try
            {
                result = CheckTransmittable("RequestLoopback");

                if (result == GemDriverError.Ok)
                {
                    sb = new StringBuilder(200);

                    messageMakeError = this._messageMaker.MakeS2F25(abs.ToArray(), out SECSMessage message, out string errorText);

                    if (messageMakeError == MessageMakeError.Ok)
                    {
                        driverResult = this._driver.SendSECSMessage(message);

                        if (driverResult == MessageError.Ok)
                        {
                            result = GemDriverError.Ok;

                            sb.Append("Transmission successful(S2F25):");

                            foreach (long tempAbs in abs)
                            {
                                sb.AppendFormat("{0} ", tempAbs);
                            }

                            logText = sb.ToString();

                            this._logger.WriteGEM(LogLevel.Information, logText);
                        }
                        else
                        {
                            result = GemDriverError.HSMSDriverError;
                            logText = string.Format("Transmission failure(S2F25):Result={0}", driverResult);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        logText = string.Format("Message make failure(S2F25), Result={0}, Error Text={1}", messageMakeError, errorText);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// HOST로 시간 Data를 요청합니다.(S2F17)
        /// </summary>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestDateTime()
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestDateTime");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(2, 17);

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S2F17)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S2F17):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program(Unformatted) changed 이벤트를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="changeStatus">변경 상태입니다.</param>
        /// <param name="ppid">연관된 PPID입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestPPChanged(long changeStatus, string ppid)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            string logText;
            VariableInfo variableInfo;
            object convertValue;

            try
            {
                result = CheckTransmittable("RequestPPChanged");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ProcessProgramChanged.ToString());

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PPChangeName.ToString());

                        if (variableInfo != null)
                        {
                            convertValue = ConvertSecsValue(variableInfo, ppid, out result);

                            if (result == GemDriverError.Ok)
                            {
                                variableInfo.Value.SetValue(convertValue);
                            }
                        }

                        if (result == GemDriverError.Ok)
                        {
                            variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PPChangeStatus.ToString());

                            if (variableInfo != null)
                            {
                                variableInfo.Value.SetValue(changeStatus);
                            }

                            CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.PPChangeName, PreDefinedV.PPChangeStatus);

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;

                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "RequestPPChanged-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "RequestPPChanged-Undefined Collection Event");
                        }
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
        /// Process Program(Formatted) changed 이벤트를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="changeStatus">변경 상태입니다.</param>
        /// <param name="ppid">연관된 PPID입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestFmtPPChanged(long changeStatus, string ppid)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            string logText;
            VariableInfo variableInfo;
            object convertValue;

            try
            {
                result = CheckTransmittable("RequestFmtPPChanged");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ProcessProgramChanged.ToString());

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PPChangeName.ToString());

                        if (variableInfo != null)
                        {
                            convertValue = ConvertSecsValue(variableInfo, ppid, out result);

                            if (result == GemDriverError.Ok)
                            {
                                variableInfo.Value.SetValue(convertValue);
                            }
                        }

                        if (result == GemDriverError.Ok)
                        {
                            variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PPChangeStatus.ToString());

                            if (variableInfo != null)
                            {
                                variableInfo.Value.SetValue(changeStatus);
                            }

                            CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.PPChangeName, PreDefinedV.PPChangeStatus);

                            messageMakeError = this._messageMaker.MakeEventReport(collectionEventInfo, this._variableCollection, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;

                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}. Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "RequestFmtPPChanged-Undefined Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "RequestFmtPPChanged-Disabled Collection Event");
                        }
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
        /// Process Program Load를 요청합니다.(S7F1)
        /// </summary>
        /// <param name="ppid">요청할 PPID입니다.</param>
        /// <param name="length">PP Body의 Length입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestPPLoadInquire(string ppid, int length)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = CheckTransmittable("RequestPPLoadInquire");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(7, 1);
                    message.UserData = ppid;

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);

                    message.Body.Add("PPID", secsItemFormat, GetLength(secsItemFormat, ppid), ppid);
                    message.Body.Add("LENGTH", GetSECSFormat(PreDefinedDataDictinary.LENGTH, SECSItemFormat.U2), 1, length);

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        this._logger.WriteGEM(LogLevel.Information, "Transmission successful(S7F1)");
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S7F1):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program(Unformatted)을 보고합니다.(S7F3)
        /// </summary>
        /// <param name="ppid">보고할 PPID입니다.</param>
        /// <param name="ppbody">PP Body입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestPPSend(string ppid, List<byte> ppbody)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = CheckTransmittable("RequestPPSend");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(7, 3);
                    message.UserData = ppid;

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);

                    message.Body.Add("PPID", secsItemFormat, GetLength(secsItemFormat, ppid), ppid);
                    message.Body.Add("PPBODY", GetSECSFormat(PreDefinedDataDictinary.PPBODY, SECSItemFormat.B), ppbody.Count, ppbody.ToArray());

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = string.Format("Transmission successful(S7F3):PPID={0}, Length={1}", ppid, ppbody.Count);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S7F3):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program(Unformatted)을 보고합니다.(S7F3)
        /// </summary>
        /// <param name="ppid">보고할 PPID입니다.</param>
        /// <param name="fileName">PP file name입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestPPSend(string ppid, string fileName)
        {
            GemDriverError result;
            byte[] ppbody;

            try
            {
                ppbody = System.IO.File.ReadAllBytes(fileName);

                result = RequestPPSend(ppid, ppbody.ToList());
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Process Program(Unformatted)을 요청합니다.(S7F5)
        /// </summary>
        /// <param name="ppid">요청할 PPID입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestPPRequest(string ppid)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = CheckTransmittable("RequestPPRequest");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(7, 5);

                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);

                    message.UserData = ppid;

                    message.Body.Add("PPID", secsItemFormat, GetLength(secsItemFormat, ppid), ppid);

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        this._logger.WriteGEM(LogLevel.Information, "Transmission successful(S7F5)");
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S7F5):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program(Formatted)을 Name/Value Pair로 보고합니다.(S7F23)
        /// </summary>
        /// <param name="formattedProcessProgramCollection">보고할 PP 정보입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestFmtPPSend(FmtPPCollection formattedProcessProgramCollection)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            VariableInfo variableInfo;
            SECSItemFormat ppidFormat;
            SECSItemFormat ccodeFormat;
            SECSItemFormat ppNameFormat;
            SECSValue secsValue;

            try
            {
                result = CheckTransmittable("RequestFmtPPSend");

                if (result == GemDriverError.Ok)
                {
                    ccodeFormat = GetSECSFormat(PreDefinedDataDictinary.CCODE, SECSItemFormat.A);
                    ppNameFormat = GetSECSFormat(PreDefinedDataDictinary.PPNAME, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(7, 23);
                    message.UserData = formattedProcessProgramCollection;

                    message.Body.Add(SECSItemFormat.L, 4, null);

                    ppidFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);
                    message.Body.Add("PPID", ppidFormat, GetLength(ppidFormat, formattedProcessProgramCollection.PPID), formattedProcessProgramCollection.PPID);

                    variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.MDLN.ToString());

                    if (string.IsNullOrEmpty(formattedProcessProgramCollection.MDLN) == false)
                    {
                        if (variableInfo.Format == SECSItemFormat.A || variableInfo.Format == SECSItemFormat.J)
                        {
                            message.Body.Add(variableInfo.Name, variableInfo.Format, Encoding.Default.GetByteCount(formattedProcessProgramCollection.MDLN), formattedProcessProgramCollection.MDLN);
                        }
                        else
                        {
                            message.Body.Add(variableInfo.Name, variableInfo.Format, 1, formattedProcessProgramCollection.MDLN);
                        }
                    }
                    else
                    {
                        message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                    }

                    variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.SOFTREV.ToString());

                    if (string.IsNullOrEmpty(formattedProcessProgramCollection.SOFTREV) == false)
                    {
                        if (variableInfo.Format == SECSItemFormat.A || variableInfo.Format == SECSItemFormat.J)
                        {
                            message.Body.Add(variableInfo.Name, variableInfo.Format, Encoding.Default.GetByteCount(formattedProcessProgramCollection.SOFTREV), formattedProcessProgramCollection.SOFTREV);
                        }
                        else
                        {
                            message.Body.Add(variableInfo.Name, variableInfo.Format, 1, formattedProcessProgramCollection.SOFTREV);
                        }
                    }
                    else
                    {
                        message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                    }

                    message.Body.Add("COMMANDCOUNT", SECSItemFormat.L, formattedProcessProgramCollection.Items.Count, null);

                    foreach (FmtPPCCodeInfo tempFormattedProcessProgramInfo in formattedProcessProgramCollection.Items)
                    {
                        message.Body.Add(SECSItemFormat.L, 2, null);

                        if (ccodeFormat == SECSItemFormat.A || ccodeFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("CCODE", ccodeFormat, Encoding.Default.GetByteCount(tempFormattedProcessProgramInfo.CommandCode), tempFormattedProcessProgramInfo.CommandCode);
                        }
                        else
                        {
                            message.Body.Add("CCODE", ccodeFormat, 1, tempFormattedProcessProgramInfo.CommandCode);
                        }

                        message.Body.Add("PPARMCOUNT", SECSItemFormat.L, tempFormattedProcessProgramInfo.Items.Count, null);

                        if (this._configFileManager.GEMConfiguration.ExtensionOption.UseFormattedPPValue == true)
                        {
                            foreach (FmtPPItem tempFormattedProcessProgramItem in tempFormattedProcessProgramInfo.Items)
                            {
                                message.Body.Add("PPARM", SECSItemFormat.L, 2, null);

                                message.Body.Add("PPNAME", ppNameFormat, GetLength(ppNameFormat, tempFormattedProcessProgramItem.PPName), tempFormattedProcessProgramItem.PPName);

                                secsValue = MakeSECSValue(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue);

                                message.Body.Add("PPVALUE", tempFormattedProcessProgramItem.Format, GetLength(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue), secsValue);
                            }
                        }
                        else
                        {
                            foreach (FmtPPItem tempFormattedProcessProgramItem in tempFormattedProcessProgramInfo.Items)
                            {
                                secsValue = MakeSECSValue(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue);

                                message.Body.Add("PPARM", tempFormattedProcessProgramItem.Format, GetLength(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue), secsValue);
                            }
                        }
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        this._logger.WriteGEM(LogLevel.Information, "Transmission successful(S7F23)");
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S7F23):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program(Formatted)을 Name Only로 보고합니다.(S7F23)
        /// </summary>
        /// <param name="formattedProcessProgramCollection">보고할 PP 정보입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestFmtPPSendWithoutValue(FmtPPCollection formattedProcessProgramCollection)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            VariableInfo variableInfo;
            SECSItemFormat ccodeFormat;
            SECSItemFormat ppNameFormat;
            SECSValue secsValue;

            try
            {
                result = CheckTransmittable("RequestFmtPPSendWithoutValue");

                if (result == GemDriverError.Ok)
                {
                    ccodeFormat = GetSECSFormat(PreDefinedDataDictinary.CCODE, SECSItemFormat.A);
                    ppNameFormat = GetSECSFormat(PreDefinedDataDictinary.PPNAME, SECSItemFormat.A);

                    message = this._driver.Messages.GetMessageHeader(7, 23);

                    message.Body.Add(SECSItemFormat.L, 4, null);
                    message.Body.Add("PPID", GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A), formattedProcessProgramCollection.PPID.Length, formattedProcessProgramCollection.PPID);

                    variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.MDLN.ToString());
                    message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));

                    variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.SOFTREV.ToString());
                    message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));

                    message.Body.Add("COMMANDCOUNT", SECSItemFormat.L, formattedProcessProgramCollection.Items.Count, null);

                    foreach (FmtPPCCodeInfo tempFormattedProcessProgramInfo in formattedProcessProgramCollection.Items)
                    {
                        message.Body.Add(SECSItemFormat.L, 2, null);

                        if (ccodeFormat == SECSItemFormat.A || ccodeFormat == SECSItemFormat.J)
                        {
                            message.Body.Add("CCODE", ccodeFormat, Encoding.Default.GetByteCount(tempFormattedProcessProgramInfo.CommandCode), tempFormattedProcessProgramInfo.CommandCode);
                        }
                        else
                        {
                            message.Body.Add("CCODE", ccodeFormat, 1, tempFormattedProcessProgramInfo.CommandCode);
                        }

                        message.Body.Add("PPARMCOUNT", SECSItemFormat.L, tempFormattedProcessProgramInfo.Items.Count, null);

                        foreach (FmtPPItem tempFormattedProcessProgramItem in tempFormattedProcessProgramInfo.Items)
                        {
                            secsValue = MakeSECSValue(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue);

                            message.Body.Add("PPARM", tempFormattedProcessProgramItem.Format, GetLength(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue), secsValue);
                        }
                    }

                    message.UserData = formattedProcessProgramCollection;

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        this._logger.WriteGEM(LogLevel.Information, "Transmission successful(S7F23)");
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S7F23):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program(Formatted)을 요청합니다.(S7F25)
        /// </summary>
        /// <param name="ppid">요청할 PPID입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestFmtPPRequest(string ppid)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = CheckTransmittable("RequestFmtPPRequest");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(7, 25);

                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);

                    message.Body.Add("PPID", secsItemFormat, GetLength(secsItemFormat, ppid), ppid);

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = string.Format("Transmission successful(S7F25):PPID={0}", ppid);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S7F25):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program Verification을 HOST로 송신합니다.(S7F27)
        /// </summary>
        /// <param name="fmtPPVerificationCollection">보고할 Formatted Process Program Verification 정보입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestFmtPPVerificationSend(FmtPPVerificationCollection fmtPPVerificationCollection)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;
            SECSItemFormat ackFormat;
            SECSItemFormat seqNumFormat;

            try
            {
                result = CheckTransmittable("RequestFmtPPVerificationSend");

                if (result == GemDriverError.Ok)
                {
                    ackFormat = GetSECSFormat(PreDefinedDataDictinary.ACKC7A, SECSItemFormat.B);
                    seqNumFormat = GetSECSFormat(PreDefinedDataDictinary.SEQNUM, SECSItemFormat.U2);

                    message = this._driver.Messages.GetMessageHeader(7, 27);
                    message.UserData = fmtPPVerificationCollection;
                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    message.Body.Add("PPID", secsItemFormat, GetLength(secsItemFormat, fmtPPVerificationCollection.PPID), fmtPPVerificationCollection.PPID);

                    message.Body.Add("ERRORCOUNT", SECSItemFormat.L, fmtPPVerificationCollection.Items.Count, null);

                    foreach (FmtPPVerificationInfo tempFmtPPVerificationInfo in fmtPPVerificationCollection.Items)
                    {
                        message.Body.Add(SECSItemFormat.L, 3, null);
                        message.Body.Add("ACKC7A", ackFormat, 1, tempFmtPPVerificationInfo.ACK);
                        message.Body.Add("SEQNUM", seqNumFormat, 1, tempFmtPPVerificationInfo.SeqNum);
                        message.Body.Add("ERRW7", SECSItemFormat.A, Encoding.Default.GetByteCount(tempFmtPPVerificationInfo.ErrW7), tempFmtPPVerificationInfo.ErrW7);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = string.Format("Transmission successful(S7F27):PPID={0}, Error Count={1}", fmtPPVerificationCollection.PPID, fmtPPVerificationCollection.Items.Count);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission failure(S7F27):Result={0}", driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Remote Command Acknowledge를 HOST로 송신합니다.(S2F42)
        /// </summary>
        /// <param name="remoteCommandInfo">수신한 remote command입니다.</param>
        /// <param name="remoteCommandResult">Remote command acknowledge입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyRemoteCommandAck(RemoteCommandInfo remoteCommandInfo, RemoteCommandResult remoteCommandResult)
        {
            GemDriverError result;
            MessageError driverResult;
            SECSMessage replyMessage;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(2, 42);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                replyMessage.Body.Add("HCACK", GetSECSFormat(PreDefinedDataDictinary.HCACK, SECSItemFormat.B), 1, remoteCommandResult.HostCommandAck);

                if (remoteCommandResult.HostCommandAck == (int)HCACK.ParameterIsInvalid)
                {
                    replyMessage.Body.Add("CPCOUNT", SECSItemFormat.L, remoteCommandResult.Items.Count, null);

                    foreach (RemoteCommandParameterResult tempRemoteCommandParameterResult in remoteCommandResult.Items)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                        replyMessage.Body.Add("CPNAME", GetSECSFormat(PreDefinedDataDictinary.CPNAME, SECSItemFormat.A), Encoding.Default.GetByteCount(tempRemoteCommandParameterResult.CPName), tempRemoteCommandParameterResult.CPName);

                        replyMessage.Body.Add("CPACK", GetSECSFormat(PreDefinedDataDictinary.CPACK, SECSItemFormat.B), 1, tempRemoteCommandParameterResult.ParameterAck);
                    }
                }
                else
                {
                    replyMessage.Body.Add(SECSItemFormat.L, 0, null);
                }

                driverResult = this._driver.ReplySECSMessage(remoteCommandInfo.SystemBytes, replyMessage);

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
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Enhanced Remote Command Acknowledge를 HOST로 송신합니다.(S2F50)
        /// </summary>
        /// <param name="remoteCommandInfo">수신한 remote command입니다.</param>
        /// <param name="remoteCommandResult">Remote command acknowledge입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyEnhancedRemoteCommandAck(EnhancedRemoteCommandInfo remoteCommandInfo, RemoteCommandResult remoteCommandResult)
        {
            GemDriverError result;
            MessageError driverResult;
            SECSMessage replyMessage;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(2, 50);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                replyMessage.Body.Add("HCACK", GetSECSFormat(PreDefinedDataDictinary.HCACK, SECSItemFormat.B), 1, remoteCommandResult.HostCommandAck);

                replyMessage.Body.Add("CPCOUNT", SECSItemFormat.L, remoteCommandResult.Items.Count, null);

                foreach (RemoteCommandParameterResult tempRemoteCommandParameterResult in remoteCommandResult.Items)
                {
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("CPNAME", GetSECSFormat(PreDefinedDataDictinary.CPNAME, SECSItemFormat.A), Encoding.Default.GetByteCount(tempRemoteCommandParameterResult.CPName), tempRemoteCommandParameterResult.CPName);

                    if (tempRemoteCommandParameterResult.ParameterListAck.Count > 0)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, tempRemoteCommandParameterResult.ParameterListAck.Count, null);

                        foreach (RemoteCommandParameterResult tempChildRemoteCommandParameterResult in tempRemoteCommandParameterResult.ParameterListAck)
                        {
                            result = AddEnhancedRemoteCommandParameterResult(replyMessage.Body, tempChildRemoteCommandParameterResult);

                            if (result != GemDriverError.Ok)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        replyMessage.Body.Add("CEPACK", GetSECSFormat(PreDefinedDataDictinary.CEPACK, SECSItemFormat.U1), 1, tempRemoteCommandParameterResult.ParameterAck);
                    }
                }

                driverResult = this._driver.ReplySECSMessage(remoteCommandInfo.SystemBytes, replyMessage);

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
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Request Offline Acknowledge를 HOST로 송신합니다.(S1F16)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack">Offline request acknowledge입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyRequestOfflineAck(uint systemBytes, int ack)
        {
            GemDriverError result;
            MessageError driverResult;
            SECSMessage replyMessage;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(1, 16);

                replyMessage.Body.Add("OFLACK", GetSECSFormat(PreDefinedDataDictinary.OFLACK, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S1F{0}:{1}):OFLACK={2}", replyMessage.Function, replyMessage.Name, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);

                    if (ack == (int)OFLACK.OfflineAcknowledge)
                    {
                        RequestHostOffline();
                    }
                }
                else
                {
                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Request Online Acknowledge를 HOST로 송신합니다.(S1F18)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack">Offline request acknowledge입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyRequestOnlineAck(uint systemBytes, int ack)
        {
            GemDriverError result;
            MessageError driverResult;
            SECSMessage replyMessage;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(1, 18);

                replyMessage.Body.Add("ONLACK", GetSECSFormat(PreDefinedDataDictinary.ONLACK, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S1F{0}:{1}):ONLACK={2}", replyMessage.Function, replyMessage.Name, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S1F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
                }

                if (ack == (int)ONLACK.OnlineAccepted)
                {
                    VariableInfo variableInfo = this._variableCollection.GetVariableInfo(PreDefinedECV.OnLineSubState.ToString());

                    if (variableInfo != null)
                    {
                        if ((ControlState)(int)variableInfo.Value == ControlState.OnlineLocal)
                        {
                            GemDriverError gemDriverResult = SendControlState(ControlState.OnlineLocal);

                            if (gemDriverResult == GemDriverError.Ok)
                            {
                                logText = string.Format("Control State Changed:State={0}", this._controlState);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                        }
                        else
                        {
                            GemDriverError gemDriverResult = SendControlState(ControlState.OnlineRemote);

                            if (gemDriverResult == GemDriverError.Ok)
                            {
                                logText = string.Format("Control State Changed:State={0}", this._controlState);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                        }
                    }
                    else
                    {
                        logText = string.Format("EC dose not exist:{0}", PreDefinedECV.OnLineSubState);

                        this._logger.WriteGEM(LogLevel.Warning, logText);
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
        /// New Equipment Constant Acknowledge를 HOST로 송신합니다.(S2F16)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="newVariableCollection">수신한 EC 정보입니다.</param>
        /// <param name="ack">Equipment acknowledge입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyNewEquipmentConstantSend(uint systemBytes, VariableCollection newVariableCollection, int ack)
        {
            GemDriverError result;
            MessageError driverResult;
            SECSMessage replyMessage;
            string logText;
            VariableInfo variableInfo;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(2, 16);

                replyMessage.Body.Add("EAC", GetSECSFormat(PreDefinedDataDictinary.EAC, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

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

                if (ack == (int)EAC.Acknowledge && newVariableCollection != null && newVariableCollection.ECV.Items.Count > 0)
                {
                    foreach (VariableInfo temp in newVariableCollection.ECV.Items)
                    {
                        variableInfo = this._variableCollection[temp.VID];

                        variableInfo.Value.SetValue(temp.Value.GetValue());
                    }

                    this._configFileManager.SaveConfigFile(Tool.ConfigFileManager.ConfigType.EquipmentConstants, false, out string errorText);

                    if (string.IsNullOrEmpty(errorText) == false)
                    {
                        logText = string.Format("New ECV save failed(S2F{0}:{1}):Error={2}", replyMessage.Function, replyMessage.Name, errorText);

                        this._logger.WriteGEM(LogLevel.Error, logText);
                    }
                    else
                    {
                        ReportEquipmentConstantChangedByHost(newVariableCollection.Items);
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
        /// Date time을 HOST로 송신합니다.(S2F18)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="timeData">송신할 date time입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyDateTimeRequest(uint systemBytes, DateTime timeData)
        {
            GemDriverError result;
            MessageError driverResult;
            SECSMessage replyMessage;
            string logText;
            VariableInfo variableInfo;
            string time;

            try
            {
                result = GemDriverError.Ok;

                variableInfo = this._variableCollection.ECV.GetVariableInfo(PreDefinedECV.TimeFormat.ToString());

                if (variableInfo != null && variableInfo.IsUse == true)
                {
                    if (variableInfo.Value.GetValue().ToString() == "1")
                    {
                        time = timeData.ToString("yyyyMMddHHmmssff");
                    }
                    else if (variableInfo.Value.GetValue().ToString() == "2")
                    {
                        time = timeData.ToString("yyyyMMddHHmmss");
                    }
                    else
                    {
                        time = timeData.ToString("yyMMddHHmmss");
                    }
                }
                else
                {
                    time = timeData.ToString("yyyyMMddHHmmssff");
                }

                replyMessage = this._driver.Messages.GetMessageHeader(2, 18);

                replyMessage.Body.Add("TIMEDATE", SECSItemFormat.A, time.Length, time);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

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
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Date time을 HOST로 송신합니다.(S2F32)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack"></param>
        /// <param name="targetTime"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyDateTimeSetRequest(uint systemBytes, int ack, DateTime targetTime)
        {
            GemDriverError result;
            MessageError driverResult;
            SECSMessage replyMessage;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(2, 32);

                replyMessage.Body.Add("TIACK", GetSECSFormat(PreDefinedDataDictinary.TIACK, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

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

                if (ack == (int)TIACK.OK && this._configFileManager.GEMConfiguration.ExtensionOption.UseAutoTimeSync == true)
                {
                    try
                    {
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
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        /// <summary>
        /// Process Program Load Inquire Acknowledge를 HOST로 송신합니다.(S7F2)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="processProgramGrantStatus"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyPPLoadInquireAck(uint systemBytes, int processProgramGrantStatus)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(7, 2);

                replyMessage.Body.Add("PPGNT", GetSECSFormat(PreDefinedDataDictinary.PPGNT, SECSItemFormat.B), 1, processProgramGrantStatus);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S7F{0}:{1}):PPGNT={2}", replyMessage.Function, replyMessage.Name, processProgramGrantStatus);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S7F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program Send Acknowledge(Unformatted)를 HOST로 송신합니다.(S7F4)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyPPSendAck(uint systemBytes, int ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(7, 4);

                replyMessage.Body.Add("ACKC7", GetSECSFormat(PreDefinedDataDictinary.ACKC7, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S7F{0}:{1}):ACK={2}", replyMessage.Function, replyMessage.Name, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S7F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program Request Acknowledge(Unformatted)를 HOST로 송신합니다.(S7F6)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppid"></param>
        /// <param name="ppbody"></param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyPPRequestAck(uint systemBytes, string ppid, List<byte> ppbody, bool ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(7, 6);

                if (ack == true && ppbody != null)
                {
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);

                    if (secsItemFormat == SECSItemFormat.A)
                    {
                        replyMessage.Body.Add("PPID", secsItemFormat, Encoding.Default.GetByteCount(ppid), ppid);
                    }
                    else
                    {
                        replyMessage.Body.Add("PPID", secsItemFormat, GetLength(secsItemFormat, ppid), ppid);
                    }

                    replyMessage.Body.Add("PPBODY", GetSECSFormat(PreDefinedDataDictinary.PPBODY, SECSItemFormat.B), ppbody.Count, ppbody.ToArray());
                }
                else
                {
                    replyMessage.Body.Add(SECSItemFormat.L, 0, null);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S7F{0}:{1}):PPID={2}, Length={3}", replyMessage.Function, replyMessage.Name, ppid, ppbody.Count);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S7F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program Delete Acknowledge를 HOST로 송신합니다.(S7F18)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyPPDeleteAck(uint systemBytes, int ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(7, 18);

                replyMessage.Body.Add("ACKC7", GetSECSFormat(PreDefinedDataDictinary.ACKC7, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S7F{0}:{1}):ACK={2}", replyMessage.Function, replyMessage.Name, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S7F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// PP List Request Acknowledge를 HOST로 송신합니다.(S7F20)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppids"></param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyCurrentEPPDRequestAck(uint systemBytes, List<string> ppids, bool ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;
            SECSItemFormat secsItemFormat;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(7, 20);

                if (ack == true && ppids != null)
                {
                    replyMessage.Body.Add("PPIDCOUNT", SECSItemFormat.L, ppids.Count, null);
                    secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);

                    ppids.ForEach(t =>
                    {
                        if (secsItemFormat == SECSItemFormat.A)
                        {
                            replyMessage.Body.Add("PPID", secsItemFormat, Encoding.Default.GetByteCount(t), t);
                        }
                        else
                        {
                            replyMessage.Body.Add("PPID", secsItemFormat, t.Length, t);
                        }
                    });
                }
                else
                {
                    ppids = new List<string>();
                    replyMessage.Body.Add("PPIDCOUNT", SECSItemFormat.L, 0, null);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S7F{0}:{1}):PP Count={2}", replyMessage.Function, replyMessage.Name, ppids.Count);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S7F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

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
                replyMessage = null;
            }

            return result;
        }

        /// <summary>
        /// Process Program Send Acknowledge(Formatted)를 HOST로 송신합니다.(S7F24)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyFmtPPSendAck(uint systemBytes, int ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(7, 24);

                replyMessage.Body.Add("ACKC7", GetSECSFormat(PreDefinedDataDictinary.ACKC7, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S7F{0}:{1}):ACK={2}", replyMessage.Function, replyMessage.Name, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S7F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Process Program Request Acknowledge(Formatted)를 HOST로 송신합니다.(S7F26)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ppid"></param>
        /// <param name="formattedProcessProgramCollection"></param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyFmtPPRequestAck(uint systemBytes, string ppid, FmtPPCollection formattedProcessProgramCollection, bool ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;
            VariableInfo variableInfo;
            SECSItemFormat ppidFormat;
            SECSItemFormat ccodeFormat;
            SECSItemFormat ppNameFormat;
            SECSItemFormat ppValueFormat;
            SECSItemFormat pParmFormat;
            SECSValue secsValue;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(7, 26);

                replyMessage.Body.Add(SECSItemFormat.L, 4, null);

                ppidFormat = GetSECSFormat(PreDefinedDataDictinary.PPID, SECSItemFormat.A);
                replyMessage.Body.Add("PPID", ppidFormat, GetLength(ppidFormat, ppid), ppid);

                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.MDLN.ToString());

                if (formattedProcessProgramCollection != null && string.IsNullOrEmpty(formattedProcessProgramCollection.MDLN) == false)
                {
                    replyMessage.Body.Add(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, formattedProcessProgramCollection.MDLN), formattedProcessProgramCollection.MDLN);
                }
                else
                {
                    replyMessage.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                }

                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.SOFTREV.ToString());

                if (formattedProcessProgramCollection != null && string.IsNullOrEmpty(formattedProcessProgramCollection.SOFTREV) == false)
                {
                    replyMessage.Body.Add(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, formattedProcessProgramCollection.SOFTREV), formattedProcessProgramCollection.SOFTREV);
                }
                else
                {
                    replyMessage.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                }

                if (ack == true && formattedProcessProgramCollection != null)
                {
                    replyMessage.Body.Add("COMMANDCOUNT", SECSItemFormat.L, formattedProcessProgramCollection.Items.Count, null);

                    if (this._configFileManager.GEMConfiguration.ExtensionOption.UseFormattedPPValue == true)
                    {
                        ccodeFormat = GetSECSFormat(PreDefinedDataDictinary.CCODE, SECSItemFormat.A);
                        ppNameFormat = GetSECSFormat(PreDefinedDataDictinary.PPNAME, SECSItemFormat.A);
                        ppValueFormat = GetSECSFormat(PreDefinedDataDictinary.PPVALUE, SECSItemFormat.A);

                        foreach (FmtPPCCodeInfo tempFormattedProcessProgramInfo in formattedProcessProgramCollection.Items)
                        {
                            replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                            if (ccodeFormat == SECSItemFormat.A || ccodeFormat == SECSItemFormat.J)
                            {
                                replyMessage.Body.Add("CCODE", ccodeFormat, Encoding.Default.GetByteCount(tempFormattedProcessProgramInfo.CommandCode), tempFormattedProcessProgramInfo.CommandCode);
                            }
                            else
                            {
                                replyMessage.Body.Add("CCODE", ccodeFormat, 1, tempFormattedProcessProgramInfo.CommandCode);
                            }

                            replyMessage.Body.Add("PPARMCOUNT", SECSItemFormat.L, tempFormattedProcessProgramInfo.Items.Count, null);

                            foreach (FmtPPItem tempFormattedProcessProgramItem in tempFormattedProcessProgramInfo.Items)
                            {
                                replyMessage.Body.Add("PPARM", SECSItemFormat.L, 2, null);

                                replyMessage.Body.Add("PPNAME", ppNameFormat, GetLength(ppNameFormat, tempFormattedProcessProgramItem.PPName), tempFormattedProcessProgramItem.PPName);

                                secsValue = MakeSECSValue(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue);

                                replyMessage.Body.Add("PPVALUE", tempFormattedProcessProgramItem.Format, GetLength(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue), secsValue);
                            }
                        }
                    }
                    else
                    {
                        ccodeFormat = GetSECSFormat(PreDefinedDataDictinary.CCODE, SECSItemFormat.A);
                        pParmFormat = GetSECSFormat(PreDefinedDataDictinary.PPARM, SECSItemFormat.A);

                        foreach (FmtPPCCodeInfo tempFormattedProcessProgramInfo in formattedProcessProgramCollection.Items)
                        {
                            replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                            if (ccodeFormat == SECSItemFormat.A || ccodeFormat == SECSItemFormat.J)
                            {
                                replyMessage.Body.Add("CCODE", ccodeFormat, Encoding.Default.GetByteCount(tempFormattedProcessProgramInfo.CommandCode), tempFormattedProcessProgramInfo.CommandCode);
                            }
                            else
                            {
                                replyMessage.Body.Add("CCODE", ccodeFormat, 1, tempFormattedProcessProgramInfo.CommandCode);
                            }

                            replyMessage.Body.Add("PPARMCOUNT", SECSItemFormat.L, tempFormattedProcessProgramInfo.Items.Count, null);

                            foreach (FmtPPItem tempFormattedProcessProgramItem in tempFormattedProcessProgramInfo.Items)
                            {
                                secsValue = MakeSECSValue(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue);

                                replyMessage.Body.Add(tempFormattedProcessProgramItem.PPName, tempFormattedProcessProgramItem.Format, GetLength(tempFormattedProcessProgramItem.Format, tempFormattedProcessProgramItem.PPValue), secsValue);
                            }
                        }
                    }
                }
                else
                {
                    replyMessage.Body.Add(SECSItemFormat.L, 0, null);

                    if (formattedProcessProgramCollection == null)
                    {
                        logText = string.Format("S7F{0}:{1}, Error Text={2}", replyMessage.Function, replyMessage.Name, "'FmtPPCollection' is null");

                        this._logger.WriteGEM(LogLevel.Warning, logText);
                    }
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S7F{0}:{1}):PPID={2}, Result={3}", replyMessage.Function, replyMessage.Name, ppid, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S7F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Terminal Message Acknowledge를 HOST로 송신합니다.(S10F4)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyTerminalMessageAck(uint systemBytes, int ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(10, 4);

                replyMessage.Body.Add("ACKC10", GetSECSFormat(PreDefinedDataDictinary.ACKC10, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S10F{0}:{1}):ACK={2}", replyMessage.Function, replyMessage.Name, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S10F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// Terminal Message Acknowledge를 HOST로 송신합니다.(S10F6)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="ack"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyTerminalMultiMessageAck(uint systemBytes, int ack)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(10, 6);

                replyMessage.Body.Add("ACKC10", GetSECSFormat(PreDefinedDataDictinary.ACKC10, SECSItemFormat.B), 1, ack);

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S10F{0}:{1}):ACK={2}", replyMessage.Function, replyMessage.Name, ack);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S10F{0}:{1}):Result={2}", replyMessage.Function, replyMessage.Name, driverResult);

                    this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// 사용자 정의 Primary SECS Message를 송신합니다.
        /// </summary>
        /// <param name="message">송신할 Primary Message입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError SendSECSMessage(SECSMessage message)
        {
            GemDriverError result;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("SendSECSMessage", false);

                if (result == GemDriverError.Ok)
                {
                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = string.Format("Transmission successful(S{0}F{1}:({2})", message.Stream, message.Function, message.Name);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission successful(S{0}F{1}:({2}):Result={3}", message.Stream, message.Function, message.Name, driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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
        /// 사용자 정의 Secondary SECS Message를 송신합니다.
        /// </summary>
        /// <param name="primaryMessage">수신한 Primary Message입니다.</param>
        /// <param name="secondaryMessage">송신할 Secondary Message입니다.</param>
        /// <returns>처리 결과입니다.</returns>
        public GemDriverError ReplySECSMessage(SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            GemDriverError result;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("ReplySECSMessage", false);

                if (result == GemDriverError.Ok)
                {
                    driverResult = this._driver.ReplySECSMessage(primaryMessage, secondaryMessage);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = string.Format("Transmission successful(S{0}F{1}:({2})", secondaryMessage.Stream, secondaryMessage.Function, secondaryMessage.Name);

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;
                        logText = string.Format("Transmission successful(S{0}F{1}:({2}):Result={3}", secondaryMessage.Stream, secondaryMessage.Function, secondaryMessage.Name, driverResult);

                        this._logger.WriteGEM(LogLevel.Error, logText);
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

        private GemDriverError SendAreYouThere(ControlState controlState)
        {
            GemDriverError result;
            MessageError driverResult;
            string logText;
            MessageMakeError messageMakeError;
            int dueTime;
            VariableInfo variableInfo;

            if (this._driver.Connected == true)
            {
                if (this._controlState == ControlState.AttemptOnline)
                {
                    try
                    {
                        messageMakeError = this._messageMaker.MakeS1F1(out SECSMessage message, out string errorText);

                        if (messageMakeError == MessageMakeError.Ok)
                        {
                            message.UserData = controlState;

                            driverResult = this._driver.SendSECSMessage(message);

                            if (driverResult == MessageError.Ok)
                            {
                                result = GemDriverError.Ok;
                                logText = "Transmission successful(S1F1)";

                                this._logger.WriteGEM(LogLevel.Information, logText);

                                variableInfo = this._variableCollection.GetVariableInfo(PreDefinedECV.AreYouThereTimeout.ToString());

                                if (variableInfo != null)
                                {
                                    if ((int.TryParse(variableInfo.Value.ToString(), out dueTime) == true) &&
                                        (dueTime > 0))
                                    {
                                    }
                                    else
                                    {
                                        dueTime = 10;
                                    }
                                }
                                else
                                {
                                    dueTime = 10;
                                }

                                this._timerAreYouThere.Change(dueTime * 1000, System.Threading.Timeout.Infinite);
                            }
                            else
                            {
                                result = GemDriverError.HSMSDriverError;
                                logText = "Transmission failure(S1F1)";

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.MessageMakeFailed;
                            logText = string.Format("Message make failure(S1F1), Result={0}, Error Text={1}", messageMakeError, errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    catch (Exception ex)
                    {
                        result = GemDriverError.Exception;

                        this._logger.WriteGEM(ex);
                    }
                }
                else
                {
                    result = GemDriverError.Ok;

                    this._logger.WriteGEM(LogLevel.Warning, "Failed to send:SendAreYouThere-Current State=AttemptOnline");
                }
            }
            else
            {
                result = GemDriverError.Disconnected;

                this._logger.WriteGEM(LogLevel.Warning, "Failed to send:SendAreYouThere-Driver Disconnected");
            }

            return result;
        }

        private GemDriverError SendControlState(ControlState controlState)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            VariableInfo controlStateVariableInfo;
            VariableInfo previousControlStateVariableInfo;
            string logText;

            try
            {
                controlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.ControlState.ToString());
                previousControlStateVariableInfo = this._variableCollection.GetVariableInfo(PreDefinedV.PreviousControlState.ToString());

                result = CheckTransmittable("SendControlState", false);

                if (result == GemDriverError.Ok)
                {
                    if (this._controlState != controlState)
                    {
                        collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.ControlStateChanged.ToString());

                        if (collectionEventInfo == null)
                        {
                            switch (controlState)
                            {
                                case ControlState.OnlineLocal:
                                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.OnlineLocal.ToString());
                                    break;
                                case ControlState.OnlineRemote:
                                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.OnlineRemote.ToString());
                                    break;
                                default:
                                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.Offline.ToString());
                                    break;
                            }
                        }

                        if (collectionEventInfo != null)
                        {
                            if (controlStateVariableInfo != null)
                            {
                                controlStateVariableInfo.Value = (int)controlState;
                            }

                            if (previousControlStateVariableInfo != null)
                            {
                                previousControlStateVariableInfo.Value = (int)this._controlState;
                            }

                            CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ControlState, PreDefinedV.PreviousControlState);

                            messageMakeError = this._messageMaker.MakeEventReportWithoutEnableCheck(this._collectionEventCollection, this._variableCollection, collectionEventInfo.CEID, out SECSMessage message, out string errorText);

                            if (messageMakeError == MessageMakeError.Ok)
                            {
                                driverResult = this._driver.SendSECSMessage(message);

                                if (driverResult == MessageError.Ok)
                                {
                                    result = GemDriverError.Ok;

                                    logText = string.Format("Transmission successful(S6F11:({0}){1}), Control State={2}", collectionEventInfo.CEID, collectionEventInfo.Name, controlState);

                                    this._logger.WriteGEM(LogLevel.Information, logText);
                                }
                                else
                                {
                                    result = GemDriverError.HSMSDriverError;
                                    logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                    this._logger.WriteGEM(LogLevel.Error, logText);
                                }
                            }
                            else
                            {
                                result = GemDriverError.MessageMakeFailed;
                                logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                                this._logger.WriteGEM(LogLevel.Error, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "SendControlState-Undefined message");
                        }
                    }
                    else
                    {
                        result = GemDriverError.Ok;

                        this._logger.WriteGEM(LogLevel.Warning, "SendControlState-Same State");
                    }
                }

                if (result != GemDriverError.Ok)
                {
                    if (controlStateVariableInfo != null)
                    {
                        controlState = (ControlState)((int)(controlStateVariableInfo.Value));
                    }
                    else
                    {
                        VariableInfo variableInfo = this._variableCollection.GetVariableInfo(VariableType.ECV, PreDefinedECV.OnLineFailState.ToString());

                        if (variableInfo != null)
                        {
                            try
                            {
                                ControlState onLineFailState;

                                onLineFailState = (ControlState)((int)variableInfo.Value);

                                switch (onLineFailState)
                                {
                                    case ControlState.EquipmentOffline:
                                    case ControlState.OnlineLocal:
                                    case ControlState.OnlineRemote:
                                        break;
                                    default:
                                        onLineFailState = ControlState.EquipmentOffline;
                                        break;
                                }

                                controlState = onLineFailState;
                            }
                            catch (Exception ex)
                            {
                                controlState = ControlState.EquipmentOffline;

                                this._logger.WriteGEM(ex);
                            }
                        }
                        else
                        {
                            controlState = ControlState.EquipmentOffline;
                        }
                    }
                }

                if (this._controlState != controlState)
                {
                    if (controlStateVariableInfo != null)
                    {
                        controlStateVariableInfo.Value = (int)controlState;
                    }

                    if (previousControlStateVariableInfo != null)
                    {
                        previousControlStateVariableInfo.Value = (int)this._controlState;
                    }

                    this._controlState = controlState;

                    this.OnControlStateChanged?.Invoke(this._controlState);
                }
            }
            catch (Exception ex)
            {
                result = GemDriverError.Exception;

                this._logger.WriteGEM(ex);
            }

            return result;
        }

        private GemDriverError SendEquipmentConstantChanged(VariableInfo variableInfo)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            string logText;

            try
            {
                result = CheckTransmittable("SendEquipmentConstantChanged");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.EquipmentConstantChanged.ToString());

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ChangedECID, PreDefinedV.ChangedECV, PreDefinedV.ChangedECList);

                        messageMakeError = this._messageMaker.MakeEventReportByECChanged(collectionEventInfo, this._variableCollection, variableInfo, out SECSMessage message, out string errorText);

                        if (messageMakeError == MessageMakeError.Ok)
                        {
                            driverResult = this._driver.SendSECSMessage(message);

                            if (driverResult == MessageError.Ok)
                            {
                                result = GemDriverError.Ok;

                                logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                result = GemDriverError.HSMSDriverError;

                                logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.MessageMakeFailed;
                            logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "SendEquipmentConstantChanged-Disabled Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "SendEquipmentConstantChanged-Undefined Collection Event");
                        }
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
        /// ECV changed 이벤트를 HOST로 보고합니다.(S6F11)
        /// </summary>
        /// <param name="variableInfos">변경된 ECV list입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        private GemDriverError SendEquipmentConstantChanged(List<VariableInfo> variableInfos)
        {
            GemDriverError result;
            MessageError driverResult;
            MessageMakeError messageMakeError;
            CollectionEventInfo collectionEventInfo;
            string logText;

            try
            {
                result = CheckTransmittable("SendEquipmentConstantChanged");

                if (result == GemDriverError.Ok)
                {
                    collectionEventInfo = this._collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.EquipmentConstantChanged.ToString());

                    if (collectionEventInfo != null && collectionEventInfo.Enabled == true)
                    {
                        CheckVariableUpdateRequest(collectionEventInfo, true, PreDefinedV.Clock, PreDefinedV.ChangedECID, PreDefinedV.ChangedECV, PreDefinedV.ChangedECList);

                        messageMakeError = this._messageMaker.MakeEventReportByECChanged(collectionEventInfo, this._variableCollection, variableInfos, out SECSMessage message, out string errorText);

                        if (messageMakeError == MessageMakeError.Ok)
                        {
                            driverResult = this._driver.SendSECSMessage(message);

                            if (driverResult == MessageError.Ok)
                            {
                                result = GemDriverError.Ok;

                                logText = string.Format("Transmission successful(S6F11:({0}){1})", collectionEventInfo.CEID, collectionEventInfo.Name);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                            else
                            {
                                result = GemDriverError.HSMSDriverError;

                                logText = string.Format("Transmission failure(S6F11:({0}){1}), Result={2}", collectionEventInfo.CEID, collectionEventInfo.Name, driverResult);

                                this._logger.WriteGEM(LogLevel.Information, logText);
                            }
                        }
                        else
                        {
                            result = GemDriverError.MessageMakeFailed;
                            logText = string.Format("Message make failure(S6F11:({0}){1}), Result={2}, Error Text={3}", collectionEventInfo.CEID, collectionEventInfo.Name, messageMakeError, errorText);

                            this._logger.WriteGEM(LogLevel.Error, logText);
                        }
                    }
                    else
                    {
                        if (collectionEventInfo != null)
                        {
                            result = GemDriverError.Undefined;

                            this._logger.WriteGEM(LogLevel.Error, "SendEquipmentConstantChanged-Undefined Collection Event");
                        }
                        else
                        {
                            result = GemDriverError.Disabled;

                            this._logger.WriteGEM(LogLevel.Error, "SendEquipmentConstantChanged-Disabled Collection Event");
                        }
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