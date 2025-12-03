using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;

namespace UbiGEM.Net.Driver
{
    internal class MessageMaker
    {
        public delegate void UserGEMMessageUpdateRequestEventHandler(SECSMessage message);
        public delegate void MessageMakerLoggingEventHandler(string logData);

        public event UserGEMMessageUpdateRequestEventHandler OnUserGEMMessageUpdateRequest;
        public event MessageMakerLoggingEventHandler OnMessageMakerLogging;

        private DeviceType _deviceType;

        private DataDictionaryCollection _dataDictionaryCollection;

        private SECSItemFormat _dataidFormat;
        private SECSItemFormat _ceidFormat;
        private SECSItemFormat _rptidFormat;
        private SECSItemFormat _vidFormat;
        private SECSMessageCollection _secsMessageCollection;
        private SECSMessageCollection _userGEMMessageCollection;
        private SECSMessageCollection _userMessageCollection;

        private VariableCollection _orgVariableCollection;
        public long _dataId;

        public MessageMaker()
        {
            this._secsMessageCollection = new SECSMessageCollection();
            this._dataDictionaryCollection = new DataDictionaryCollection();
            this._userGEMMessageCollection = new SECSMessageCollection();
            this._userMessageCollection = new SECSMessageCollection();
            this._deviceType = DeviceType.Equipment;
        }

        public void Initialize(
            DeviceType deviceType,
            SECSMessageCollection secsMessageCollection,
            CollectionEventCollection collectionEventCollection,
            ReportCollection reportCollection,
            VariableCollection variableCollection,
            DataDictionaryCollection dataDictionaryCollection,
            SECSMessageCollection userGEMMessageCollection,
            SECSMessageCollection userMessageCollection,
            VariableCollection orgVariableCollection)
        {
            DataDictionaryInfo dataDictionaryInfo;

            this._secsMessageCollection = secsMessageCollection;
            this._dataDictionaryCollection = dataDictionaryCollection;
            this._userGEMMessageCollection = userGEMMessageCollection;
            this._userMessageCollection = userMessageCollection;
            this._orgVariableCollection = orgVariableCollection;
            this._deviceType = deviceType;

            dataDictionaryInfo = dataDictionaryCollection[PreDefinedDataDictinary.DATAID.ToString()];
            this._dataidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

            dataDictionaryInfo = dataDictionaryCollection[PreDefinedDataDictinary.CEID.ToString()];
            this._ceidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

            dataDictionaryInfo = dataDictionaryCollection[PreDefinedDataDictinary.RPTID.ToString()];
            this._rptidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

            dataDictionaryInfo = dataDictionaryCollection[PreDefinedDataDictinary.VID.ToString()];
            this._vidFormat = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : SECSItemFormat.U2;

            this._dataId = 0;
        }

        public MessageMakeError MakeS1F1(out SECSMessage message, out string errorText)
        {
            MessageMakeError result;

            errorText = string.Empty;
            message = this._userGEMMessageCollection.GetMessageHeader(1, 1, this._deviceType);

            try
            {
                if (message != null)
                {
                    result = MessageMakeError.Ok;

                    MakeUserGEMMessage(ref message);
                }
                else
                {
                    message = this._secsMessageCollection.GetMessageHeader(1, 1, this._deviceType);

                    if (message != null)
                    {
                        if (this._deviceType == DeviceType.Equipment)
                        {
                            message.Direction = SECSMessageDirection.ToHost;

                            result = MessageMakeError.Ok;
                        }
                        else
                        {
                            message.Direction = SECSMessageDirection.ToEquipment;
                            result = MessageMakeError.Ok;
                        }
                    }
                    else
                    {
                        result = MessageMakeError.NoDefined;
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS1F4(VariableCollection variableCollection, List<VariableInfo> variables, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;

            result = MessageMakeError.Ok;

            errorText = string.Empty;
            message = this._secsMessageCollection.GetMessageHeader(1, 4);

            try
            {
                if (message != null)
                {
                    if (result == MessageMakeError.Ok)
                    {
                        message.Body.Add("SVCOUNT", SECSItemFormat.L, variables.Count, null);

                        foreach (VariableInfo tempVariableInfo in variables)
                        {
                            if (tempVariableInfo != null)
                            {
                                CheckVariableValue(variableCollection, tempVariableInfo);

                                if (tempVariableInfo.Format == SECSItemFormat.L)
                                {
                                    if (tempVariableInfo.ChildVariables != null)
                                    {
                                        message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.ChildVariables.Count, null);
                                    }
                                    else
                                    {
                                        message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);
                                    }

                                    MakeChildSecsItem(variableCollection, tempVariableInfo, ref message);
                                }
                                else if (tempVariableInfo.Format == SECSItemFormat.A)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                }
                                else
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                }
                            }
                            else
                            {
                                message.Body.Add(SECSItemFormat.L, 0, null);
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add(SECSItemFormat.L, 0, null);
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS1F13(VariableCollection variableCollection, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            VariableInfo variableInfo;

            errorText = string.Empty;
            message = this._userGEMMessageCollection.GetMessageHeader(1, 13, this._deviceType);

            try
            {
                if (message != null)
                {
                    result = MessageMakeError.Ok;

                    MakeUserGEMMessage(ref message);
                }
                else
                {
                    message = this._secsMessageCollection.GetMessageHeader(1, 13, this._deviceType);

                    if (message != null)
                    {
                        if (this._deviceType == DeviceType.Equipment)
                        {
                            message.Direction = SECSMessageDirection.ToHost;

                            message.Body.Add(SECSItemFormat.L, 2, null);

                            variableInfo = variableCollection.GetVariableInfo(PreDefinedV.MDLN.ToString());
                            CheckVariableValue(variableCollection, variableInfo);
                            message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));

                            variableInfo = variableCollection.GetVariableInfo(PreDefinedV.SOFTREV.ToString());
                            CheckVariableValue(variableCollection, variableInfo);
                            message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));

                            result = MessageMakeError.Ok;
                        }
                        else
                        {
                            message.Direction = SECSMessageDirection.ToEquipment;
                            result = MessageMakeError.Ok;
                        }
                    }
                    else
                    {
                        result = MessageMakeError.NoDefined;
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS1F22(VariableCollection variableCollection, SECSMessage primaryMessage, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            VariableInfo variableInfo;
            string vid;

            result = MessageMakeError.Ok;

            errorText = string.Empty;
            message = this._secsMessageCollection.GetMessageHeader(1, 22);

            try
            {
                if (message != null)
                {
                    if (primaryMessage.Body.Item[0].SubItem.Count <= 0)
                    {
                        message.Body.Add("DVVALCOUNT", SECSItemFormat.L, variableCollection.DVVal.Items.Count, null);

                        foreach (VariableInfo tempVariableInfo in variableCollection.DVVal.Items)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._vidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("VID", this._vidFormat, Encoding.Default.GetByteCount(tempVariableInfo.VID), tempVariableInfo.VID);
                            }
                            else
                            {
                                message.Body.Add("VID", this._vidFormat, 1, long.Parse(tempVariableInfo.VID));
                            }

                            message.Body.Add("DVVALNAME", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Name), tempVariableInfo.Name);

                            if (tempVariableInfo.Units != null)
                            {
                                message.Body.Add("UNITS", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Units.ToString()), tempVariableInfo.Units.ToString());
                            }
                            else
                            {
                                message.Body.Add("UNITS", SECSItemFormat.A, 0, string.Empty);
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("DVVALCOUNT", SECSItemFormat.L, primaryMessage.Body.Item[0].SubItem.Count, null);

                        foreach (SECSItem tempVid in primaryMessage.Body.Item[0].SubItem.Items)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._vidFormat == SECSItemFormat.A)
                            {
                                vid = tempVid.Value;
                            }
                            else
                            {
                                vid = ((long)tempVid.Value).ToString();
                            }

                            variableInfo = variableCollection[vid];

                            if (this._vidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("VID", this._vidFormat, Encoding.Default.GetByteCount(vid), vid);
                            }
                            else
                            {
                                message.Body.Add("VID", this._vidFormat, 1, long.Parse(vid));
                            }

                            if (variableInfo != null && variableInfo.VIDType == VariableType.DVVAL)
                            {
                                message.Body.Add("DVVALNAME", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Name), variableInfo.Name);

                                if (variableInfo.Units != null)
                                {
                                    message.Body.Add("UNITS", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Units.ToString()), variableInfo.Units.ToString());
                                }
                                else
                                {
                                    message.Body.Add("UNITS", SECSItemFormat.A, 0, string.Empty);
                                }
                            }
                            else
                            {
                                message.Body.Add("DVVALNAME", SECSItemFormat.A, 0, string.Empty);
                                message.Body.Add("UNITS", SECSItemFormat.A, 0, string.Empty);
                            }
                        }
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS1F24(CollectionEventCollection collectionEventCollection, SECSMessage primaryMessage, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            List<CollectionEventInfo> collectionEventInfos;
            CollectionEventInfo collectionEventInfo;
            List<VariableInfo> variableInfos;
            string ceid;

            result = MessageMakeError.Ok;

            errorText = string.Empty;
            message = this._secsMessageCollection.GetMessageHeader(1, 24);

            try
            {
                if (message != null)
                {
                    if (primaryMessage.Body.Item[0].SubItem.Count <= 0)
                    {
                        collectionEventInfos = (from CollectionEventInfo tempCollectionEventInfo in collectionEventCollection.Items.Values
                                                where tempCollectionEventInfo.IsUse == true
                                                select tempCollectionEventInfo).ToList();

                        message.Body.Add("CECOUNT", SECSItemFormat.L, collectionEventInfos.Count, null);

                        foreach (CollectionEventInfo tempCollectionEventInfo in collectionEventInfos)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("CEID", this._ceidFormat, Encoding.Default.GetByteCount(tempCollectionEventInfo.CEID), tempCollectionEventInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add("CEID", this._ceidFormat, 1, long.Parse(tempCollectionEventInfo.CEID));
                            }

                            message.Body.Add("CENAME", SECSItemFormat.A, Encoding.Default.GetByteCount(tempCollectionEventInfo.Name), tempCollectionEventInfo.Name);

                            variableInfos = (from ReportInfo tempReportInfo in tempCollectionEventInfo.Reports.Items.Values
                                             from VariableInfo tempVariableInfo in tempReportInfo.Variables.Items
                                             where tempVariableInfo.VIDType == VariableType.DVVAL
                                             select tempVariableInfo).ToList();

                            if (variableInfos != null)
                            {
                                message.Body.Add("DVVALCOUNT", SECSItemFormat.L, variableInfos.Count, null);

                                foreach (VariableInfo tempVariableInfo in variableInfos)
                                {
                                    if (this._vidFormat == SECSItemFormat.A)
                                    {
                                        message.Body.Add("VID", this._vidFormat, Encoding.Default.GetByteCount(tempVariableInfo.VID), tempVariableInfo.VID);
                                    }
                                    else
                                    {
                                        message.Body.Add("VID", this._vidFormat, 1, long.Parse(tempVariableInfo.VID));
                                    }
                                }
                            }
                            else
                            {
                                message.Body.Add("DVVALCOUNT", SECSItemFormat.L, 0, null);
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("CECOUNT", SECSItemFormat.L, primaryMessage.Body.Item[0].SubItem.Count, null);

                        foreach (SECSItem tempCeid in primaryMessage.Body.Item[0].SubItem.Items)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                ceid = tempCeid.Value;
                            }
                            else
                            {
                                ceid = ((long)tempCeid.Value).ToString();
                            }

                            collectionEventInfo = collectionEventCollection[ceid];

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("CEID", this._ceidFormat, Encoding.Default.GetByteCount(ceid), ceid);
                            }
                            else
                            {
                                message.Body.Add("CEID", this._ceidFormat, 1, long.Parse(ceid));
                            }

                            if (collectionEventInfo != null)
                            {
                                message.Body.Add("CENAME", SECSItemFormat.A, Encoding.Default.GetByteCount(collectionEventInfo.Name), collectionEventInfo.Name);

                                variableInfos = (from ReportInfo tempReportInfo in collectionEventInfo.Reports.Items.Values
                                                 from VariableInfo tempVariableInfo in tempReportInfo.Variables.Items
                                                 where tempVariableInfo.VIDType == VariableType.DVVAL
                                                 select tempVariableInfo).ToList();

                                if (variableInfos != null)
                                {
                                    message.Body.Add("DVVALCOUNT", SECSItemFormat.L, variableInfos.Count, null);

                                    foreach (VariableInfo tempVariableInfo in variableInfos)
                                    {
                                        if (this._vidFormat == SECSItemFormat.A)
                                        {
                                            message.Body.Add("VID", this._vidFormat, Encoding.Default.GetByteCount(tempVariableInfo.VID), tempVariableInfo.VID);
                                        }
                                        else
                                        {
                                            message.Body.Add("VID", this._vidFormat, 1, long.Parse(tempVariableInfo.VID));
                                        }
                                    }
                                }
                                else
                                {
                                    message.Body.Add("DVVALCOUNT", SECSItemFormat.L, 0, null);
                                }
                            }
                            else
                            {
                                message.Body.Add("CENAME", SECSItemFormat.A, 0, string.Empty);
                                message.Body.Add("DVVALCOUNT", SECSItemFormat.L, 0, null);
                            }
                        }
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS2F14(VariableCollection variableCollection, SECSMessage primaryMessage, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            VariableInfo variableInfo;
            string ecid;

            result = MessageMakeError.Ok;

            errorText = string.Empty;
            message = this._secsMessageCollection.GetMessageHeader(2, 14);

            try
            {
                if (message != null)
                {
                    if (primaryMessage.Body.Item[0].SubItem.Count <= 0)
                    {
                        var varECList = variableCollection.ECV.Items.Where(t => t.IsUse == true);

                        if (varECList != null)
                        {
                            message.Body.Add("ECVCOUNT", SECSItemFormat.L, varECList.Count(), null);

                            foreach (VariableInfo tempVariableInfo in varECList)
                            {
                                CheckVariableValue(variableCollection, tempVariableInfo);

                                message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                            }
                        }
                        else
                        {
                            message.Body.Add("ECVCOUNT", SECSItemFormat.L, 0, null);
                        }
                    }
                    else
                    {
                        message.Body.Add("ECVCOUNT", SECSItemFormat.L, primaryMessage.Body.Item[0].SubItem.Count, null);

                        foreach (SECSItem tempEcid in primaryMessage.Body.Item[0].SubItem.Items)
                        {
                            if (this._vidFormat == SECSItemFormat.A)
                            {
                                ecid = tempEcid.Value;
                            }
                            else
                            {
                                ecid = ((long)tempEcid.Value).ToString();
                            }

                            variableInfo = variableCollection[ecid];

                            if (variableInfo != null)
                            {
                                if (variableInfo.IsUse == true)
                                {
                                    CheckVariableValue(variableCollection, variableInfo);

                                    message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                                }
                                else
                                {
                                    message.Body.Add(SECSItemFormat.L, 0, null);
                                }
                            }
                            else
                            {
                                message.Body.Add(SECSItemFormat.L, 0, null);
                            }
                        }
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS2F25(byte[] abs, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            SECSItemFormat secsItemFormat;

            errorText = string.Empty;
            message = this._userGEMMessageCollection.GetMessageHeader(2, 25, this._deviceType);

            try
            {
                if (message != null)
                {
                    result = MessageMakeError.Ok;

                    MakeUserGEMMessage(ref message);
                }
                else
                {
                    message = this._secsMessageCollection.GetMessageHeader(2, 25, this._deviceType);

                    if (message != null)
                    {
                        secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.ABS, SECSItemFormat.B);

                        message.Direction = SECSMessageDirection.ToHost;

                        if (abs.Length == 1)
                        {
                            message.Body.Add(PreDefinedDataDictinary.ABS.ToString(), secsItemFormat, abs.Length, abs[0]);
                        }
                        else
                        {
                            message.Body.Add(PreDefinedDataDictinary.ABS.ToString(), secsItemFormat, abs.Length, abs);
                        }

                        result = MessageMakeError.Ok;
                    }
                    else
                    {
                        result = MessageMakeError.NoDefined;
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS2F26(byte[] abs, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            SECSItemFormat secsItemFormat;

            errorText = string.Empty;
            message = this._userGEMMessageCollection.GetMessageHeader(2, 26, this._deviceType);

            try
            {
                if (message != null)
                {
                    result = MessageMakeError.Ok;

                    MakeUserGEMMessage(ref message);
                }
                else
                {
                    message = this._secsMessageCollection.GetMessageHeader(2, 26, this._deviceType);

                    if (message != null)
                    {
                        secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.ABS, SECSItemFormat.B);

                        message.Direction = SECSMessageDirection.ToHost;

                        if (abs.Length == 1)
                        {
                            message.Body.Add(PreDefinedDataDictinary.ABS.ToString(), secsItemFormat, abs.Length, abs[0]);
                        }
                        else
                        {
                            message.Body.Add(PreDefinedDataDictinary.ABS.ToString(), secsItemFormat, abs.Length, abs);
                        }

                        result = MessageMakeError.Ok;
                    }
                    else
                    {
                        result = MessageMakeError.NoDefined;
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS2F30(VariableCollection variableCollection, SECSMessage primaryMessage, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            VariableInfo variableInfo;
            string ecid;
            SECSItemFormat ecidFormat;

            result = MessageMakeError.Ok;

            errorText = string.Empty;
            message = this._secsMessageCollection.GetMessageHeader(2, 30);

            try
            {
                if (message != null)
                {
                    ecidFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                    if (primaryMessage.Body.Item[0].SubItem.Count <= 0)
                    {
                        message.Body.Add("ECIDCOUNT", SECSItemFormat.L, variableCollection.ECV.Items.Count, null);

                        foreach (VariableInfo tempVariableInfo in variableCollection.ECV.Items)
                        {
                            message.Body.Add(SECSItemFormat.L, 6, null);

                            if (ecidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ECID", ecidFormat, Encoding.Default.GetByteCount(tempVariableInfo.VID), tempVariableInfo.VID);
                            }
                            else
                            {
                                message.Body.Add("ECID", ecidFormat, 1, tempVariableInfo.VID);
                            }

                            message.Body.Add("ECNAME", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Name), tempVariableInfo.Name);
                            message.Body.Add("ECMIN", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Min.GetValueOrDefault().ToString()), tempVariableInfo.Min.GetValueOrDefault().ToString());
                            message.Body.Add("ECMAX", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Max.GetValueOrDefault().ToString()), tempVariableInfo.Max.GetValueOrDefault().ToString());
                            message.Body.Add("ECDEF", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Default.ToString()), tempVariableInfo.Default.ToString());
                            message.Body.Add("ECUNIT", SECSItemFormat.A, Encoding.Default.GetByteCount(tempVariableInfo.Units.ToString()), tempVariableInfo.Units.ToString());
                        }
                    }
                    else
                    {
                        message.Body.Add(SECSItemFormat.L, primaryMessage.Body.Item[0].SubItem.Count, null);

                        foreach (SECSItem tempEcid in primaryMessage.Body.Item[0].SubItem.Items)
                        {
                            if (ecidFormat == SECSItemFormat.A)
                            {
                                ecid = tempEcid.Value;
                            }
                            else
                            {
                                ecid = ((long)tempEcid.Value).ToString();
                            }

                            variableInfo = variableCollection[ecid];

                            if (variableInfo != null)
                            {
                                message.Body.Add(SECSItemFormat.L, 6, null);

                                if (ecidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("ECID", ecidFormat, Encoding.Default.GetByteCount(variableInfo.VID), variableInfo.VID);
                                }
                                else
                                {
                                    message.Body.Add("ECID", ecidFormat, 1, variableInfo.VID);
                                }

                                message.Body.Add("ECNAME", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Name), variableInfo.Name);
                                message.Body.Add("ECMIN", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Min.GetValueOrDefault().ToString()), variableInfo.Min.GetValueOrDefault().ToString());
                                message.Body.Add("ECMAX", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Max.GetValueOrDefault().ToString()), variableInfo.Max.GetValueOrDefault().ToString());
                                message.Body.Add("ECDEF", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Default.ToString()), variableInfo.Default.ToString());
                                message.Body.Add("ECUNIT", SECSItemFormat.A, Encoding.Default.GetByteCount(variableInfo.Units.ToString()), variableInfo.Units.ToString());
                            }
                            else
                            {
                                message.Body.Add(SECSItemFormat.L, 6, null);

                                if (ecidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("ECID", ecidFormat, ecid.Length, ecid);
                                }
                                else
                                {
                                    message.Body.Add("ECID", ecidFormat, 1, tempEcid.Value);
                                }

                                message.Body.Add("ECNAME", SECSItemFormat.A, 0, string.Empty);
                                message.Body.Add("ECMIN", SECSItemFormat.A, 0, string.Empty);
                                message.Body.Add("ECMAX", SECSItemFormat.A, 0, string.Empty);
                                message.Body.Add("ECDEF", SECSItemFormat.A, 0, string.Empty);
                                message.Body.Add("ECUNIT", SECSItemFormat.A, 0, string.Empty);
                            }
                        }
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS6F16(CollectionEventCollection collectionEventCollection, VariableCollection variableCollection, string ceid, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError reportMakeResult;
            CollectionEventInfo collectionEventInfo;

            result = MessageMakeError.Ok;
            collectionEventInfo = collectionEventCollection[ceid];

            errorText = string.Empty;
            message = this._secsMessageCollection.GetMessageHeader(6, 16);

            try
            {
                if (message != null)
                {
                    message.Body.Add(SECSItemFormat.L, 3, null);

                    if (this._dataidFormat == SECSItemFormat.A)
                    {
                        message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                    }
                    else
                    {
                        message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                    }

                    if (collectionEventInfo != null)
                    {
                        if (this._ceidFormat == SECSItemFormat.A)
                        {
                            message.Body.Add(collectionEventInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(ceid), ceid);
                        }
                        else
                        {
                            message.Body.Add(collectionEventInfo.Name, this._ceidFormat, 1, ceid);
                        }

                        message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, collectionEventInfo.Reports.Items.Count, null);

                        foreach (ReportInfo tempReportInfo in collectionEventInfo.Reports.Items.Values)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            if (this._rptidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                            }
                            else
                            {
                                message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                            }

                            reportMakeResult = MakeReportInfo(variableCollection, message, tempReportInfo, out errorText);

                            if (reportMakeResult != MessageMakeError.Ok)
                            {
                                message = null;
                                result = MessageMakeError.Exception;

                                break;
                            }
                        }
                    }
                    else
                    {
                        if (this._ceidFormat == SECSItemFormat.A)
                        {
                            message.Body.Add(this._ceidFormat, Encoding.Default.GetByteCount(ceid), ceid);
                        }
                        else
                        {
                            message.Body.Add(this._ceidFormat, 1, ceid);
                        }

                        message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, 0, null);
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeS6F20(ReportInfo reportInfo, VariableCollection variableCollection, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError reportMakeResult;

            errorText = string.Empty;
            message = this._secsMessageCollection.GetMessageHeader(6, 20);

            try
            {
                if (message != null)
                {
                    if (reportInfo != null)
                    {
                        reportMakeResult = MakeReportInfo(variableCollection, message, reportInfo, out errorText);

                        if (reportMakeResult == MessageMakeError.Ok)
                        {
                            result = MessageMakeError.Ok;
                        }
                        else
                        {
                            message = null;
                            result = reportMakeResult;
                        }
                    }
                    else
                    {
                        message.Body.Add(SECSItemFormat.L, 0, null);
                        result = MessageMakeError.Ok;
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeEventReport(CollectionEventInfo collectionEventInfo, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;

            try
            {
                errorText = string.Empty;

                if (collectionEventInfo.Enabled == true)
                {
                    reportCollection = collectionEventInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(collectionEventInfo.CEID), collectionEventInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, 1, collectionEventInfo.CEID);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        errorText = "Reports is null";
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.Disabled;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;

                if (collectionEventInfo != null)
                {
                    errorText = string.Format("(collectionEventInfo={0}:{1}){2}", collectionEventInfo.CEID, collectionEventInfo.Name, ex.Message);
                }
                else
                {
                    errorText = string.Format("(collectionEventInfo=null){0}", ex.Message);
                }
            }

            return result;
        }

        public MessageMakeError MakeEventReport(CollectionEventInfo collectionEventInfo, VariableCollection variableCollection, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;

            try
            {
                errorText = string.Empty;

                if (collectionEventInfo.Enabled == true)
                {
                    reportCollection = collectionEventInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(collectionEventInfo.CEID), collectionEventInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, 1, collectionEventInfo.CEID);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(variableCollection, message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.Disabled;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;

                if (collectionEventInfo != null)
                {
                    errorText = string.Format("(collectionEventInfo={0}:{1}){2}", collectionEventInfo.CEID, collectionEventInfo.Name, ex.Message);
                }
                else
                {
                    errorText = string.Format("(collectionEventInfo=null){0}", ex.Message);
                }
            }

            return result;
        }

        public MessageMakeError MakeEventReport(CollectionEventInfo collectionEventInfo, VariableCollection allVariables, List<VariableInfo> variables, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;

            try
            {
                errorText = string.Empty;

                if (collectionEventInfo.Enabled == true)
                {
                    reportCollection = collectionEventInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(collectionEventInfo.CEID), collectionEventInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, 1, collectionEventInfo.CEID);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(allVariables, variables, message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.Disabled;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;

                if (collectionEventInfo != null)
                {
                    errorText = string.Format("(collectionEventInfo={0}:{1}){2}", collectionEventInfo.CEID, collectionEventInfo.Name, ex.Message);
                }
                else
                {
                    errorText = string.Format("(collectionEventInfo=null){0}", ex.Message);
                }
            }

            return result;
        }

        public MessageMakeError MakeEventReport(CollectionEventInfo collectionEventInfo, VariableCollection allVariables, List<string> vids, List<string> values, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;

            try
            {
                errorText = string.Empty;

                if (collectionEventInfo.Enabled == true)
                {
                    reportCollection = collectionEventInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(collectionEventInfo.CEID), collectionEventInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, 1, collectionEventInfo.CEID);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(allVariables, vids, values, message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.Disabled;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;

                if (collectionEventInfo != null)
                {
                    errorText = string.Format("(collectionEventInfo={0}:{1}){2}", collectionEventInfo.CEID, collectionEventInfo.Name, ex.Message);
                }
                else
                {
                    errorText = string.Format("(collectionEventInfo=null){0}", ex.Message);
                }
            }

            return result;
        }

        public MessageMakeError MakeEventReport(CollectionEventInfo collectionEventInfo, SECSItemCollection secsItems, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;

            try
            {
                errorText = string.Empty;

                if (collectionEventInfo.Enabled == true)
                {
                    reportCollection = collectionEventInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(collectionEventInfo.CEID), collectionEventInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add(collectionEventInfo.Name, this._ceidFormat, 1, collectionEventInfo.CEID);
                            }

                            foreach (SECSItem tempSECSItem in secsItems.Items)
                            {
                                message.Body.Add(tempSECSItem);
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.Disabled;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;

                if (collectionEventInfo != null)
                {
                    errorText = string.Format("(collectionEventInfo={0}:{1}){2}", collectionEventInfo.CEID, collectionEventInfo.Name, ex.Message);
                }
                else
                {
                    errorText = string.Format("(collectionEventInfo=null){0}", ex.Message);
                }
            }

            return result;
        }

        public MessageMakeError MakeEventReportWithoutEnableCheck(
            CollectionEventCollection collectionEventCollection,
            VariableCollection variableCollection,
            string ceid,
            out SECSMessage message,
            out string errorText)
        {
            MessageMakeError result;
            CollectionEventInfo ceidInfo;
            ReportCollection reportCollection;

            errorText = string.Empty;

            try
            {
                ceidInfo = collectionEventCollection[ceid];

                if (ceidInfo != null)
                {
                    message = this._secsMessageCollection.GetMessageHeader(6, 11);

                    if (message != null)
                    {
                        reportCollection = ceidInfo.Reports;

                        if (reportCollection != null)
                        {
                            result = MessageMakeError.Ok;

                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(ceid), ceid);
                            }
                            else
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, 1, ceid);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(variableCollection, message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.Ok;

                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(ceid), ceid);
                            }
                            else
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, 1, ceid);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, 0, null);
                        }
                    }
                    else
                    {
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;

                errorText = string.Format("(CEID={0}){1}", ceid, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeEventReportByECChanged(CollectionEventInfo ceidInfo, VariableCollection variableCollection, VariableInfo variable, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;
            VariableInfo ecChangeListInfo;
            VariableInfo ecChangeVariableInfo;
            VariableInfo ecChangevalueInfo;
            SECSItemFormat ecidFormat;

            errorText = string.Empty;

            try
            {
                if (ceidInfo.Enabled == true)
                {
                    reportCollection = ceidInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        ecChangeVariableInfo = variableCollection.GetVariableInfo(PreDefinedV.ChangedECList.ToString());

                        if (ecChangeVariableInfo != null)
                        {
                            ecChangeVariableInfo.ChildVariables.Clear();
                            ecidFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                            ecChangeListInfo = new VariableInfo()
                            {
                                Format = SECSItemFormat.L,
                                Length = 2
                            };

                            ecChangevalueInfo = new VariableInfo()
                            {
                                Format = ecidFormat,
                                Name = variable.Name
                            };

                            ecChangevalueInfo.Value.SetValue(ecidFormat, ConvertSecsValue(variable, ecidFormat, variable.VID));

                            ecChangeListInfo.ChildVariables.Add(ecChangevalueInfo);

                            ecChangeListInfo.ChildVariables.Add(new VariableInfo()
                            {
                                Format = variable.Format,
                                Value = variable.Value
                            });

                            ecChangeVariableInfo.ChildVariables.Add(ecChangeListInfo);
                        }

                        ecChangeVariableInfo = variableCollection.GetVariableInfo(PreDefinedV.ChangedECID.ToString());

                        if (ecChangeVariableInfo != null)
                        {
                            ecidFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                            if (ecChangeVariableInfo.Format == SECSItemFormat.L)
                            {
                                if (ecidFormat == SECSItemFormat.A)
                                {
                                    List<string> ecChangeList = new List<string>
                                    {
                                        variable.VID
                                    };

                                    ecChangeVariableInfo.Value = ecChangeList.ToArray();

                                    ecChangeList = null;
                                }
                                else
                                {
                                    List<long> ecChangeList = new List<long>
                                    {
                                        long.Parse(variable.VID)
                                    };

                                    ecChangeVariableInfo.Value = ecChangeList.ToArray();

                                    ecChangeList = null;
                                }
                            }
                            else
                            {
                                ecChangeVariableInfo.Value = variable.VID;
                            }
                        }

                        ecChangeVariableInfo = variableCollection.GetVariableInfo(PreDefinedV.ChangedECV.ToString());

                        if (ecChangeVariableInfo != null)
                        {
                            if (ecChangeVariableInfo.Format == SECSItemFormat.L)
                            {
                                ecChangeVariableInfo.ChildVariables.Clear();

                                ecChangeVariableInfo.Length = 1;

                                ecChangeVariableInfo.ChildVariables.Add(variable);
                            }
                            else
                            {
                                ecChangeVariableInfo.Value = variable.Value;
                            }
                        }

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(ceidInfo.CEID), ceidInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, 1, ceidInfo.CEID);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(variableCollection, message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.Disabled;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;

                if (ceidInfo != null)
                {
                    errorText = string.Format("(collectionEventInfo={0}:{1}){2}", ceidInfo.CEID, ceidInfo.Name, ex.Message);
                }
                else
                {
                    errorText = string.Format("(collectionEventInfo=null){0}", ex.Message);
                }
            }

            return result;
        }

        public MessageMakeError MakeEventReportByECChanged(CollectionEventInfo ceidInfo, VariableCollection variableCollection, List<VariableInfo> variable, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;
            VariableInfo ecChangeListInfo;
            VariableInfo ecChangeVariableInfo;
            VariableInfo ecChangevalueInfo;
            SECSItemFormat ecidFormat;

            try
            {
                errorText = string.Empty;

                if (ceidInfo.Enabled == true)
                {
                    reportCollection = ceidInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        ecChangeVariableInfo = variableCollection.GetVariableInfo(PreDefinedV.ChangedECList.ToString());

                        if (ecChangeVariableInfo != null)
                        {
                            ecChangeVariableInfo.ChildVariables.Clear();
                            ecidFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                            foreach (VariableInfo tempVariableInfo in variable)
                            {
                                ecChangeListInfo = new VariableInfo()
                                {
                                    Format = SECSItemFormat.L,
                                    Length = 2
                                };

                                ecChangevalueInfo = new VariableInfo()
                                {
                                    Format = ecidFormat,
                                    Name = tempVariableInfo.Name
                                };

                                ecChangevalueInfo.Value.SetValue(ecidFormat, ConvertSecsValue(tempVariableInfo, ecidFormat, tempVariableInfo.VID));

                                ecChangeListInfo.ChildVariables.Add(ecChangevalueInfo);

                                ecChangeListInfo.ChildVariables.Add(new VariableInfo()
                                {
                                    Format = tempVariableInfo.Format,
                                    Value = tempVariableInfo.Value
                                });

                                ecChangeVariableInfo.ChildVariables.Add(ecChangeListInfo);
                            }
                        }

                        ecChangeVariableInfo = variableCollection.GetVariableInfo(PreDefinedV.ChangedECID.ToString());

                        if (ecChangeVariableInfo != null)
                        {
                            ecidFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                            if (ecChangeVariableInfo.Format == SECSItemFormat.L)
                            {
                                if (ecidFormat == SECSItemFormat.A)
                                {
                                    List<string> ecChangeList = new List<string>();

                                    ecChangeList.AddRange(variable.Select(t => t.VID));
                                    ecChangeVariableInfo.Value = ecChangeList.ToArray();

                                    ecChangeList = null;
                                }
                                else
                                {
                                    List<long> ecChangeList = new List<long>();

                                    ecChangeList.AddRange(variable.Select(t => long.Parse(t.VID)));
                                    ecChangeVariableInfo.Value = ecChangeList.ToArray();

                                    ecChangeList = null;
                                }
                            }
                            else
                            {
                                if (variable.Count == 1)
                                {
                                    ecChangeVariableInfo.Value = variable.FirstOrDefault().VID;
                                }
                                else
                                {
                                    result = MessageMakeError.NoDefined;
                                }
                            }
                        }

                        ecChangeVariableInfo = variableCollection.GetVariableInfo(PreDefinedV.ChangedECV.ToString());

                        if (ecChangeVariableInfo != null)
                        {
                            if (ecChangeVariableInfo.Format == SECSItemFormat.L)
                            {
                                ecChangeVariableInfo.ChildVariables.Clear();

                                ecChangeVariableInfo.Length = variable.Count;

                                ecChangeVariableInfo.ChildVariables.AddRange(variable);
                            }
                            else
                            {
                                if (variable.Count == 1)
                                {
                                    ecChangeVariableInfo.Value = variable.FirstOrDefault().Value;
                                }
                                else
                                {
                                    result = MessageMakeError.NoDefined;
                                }
                            }
                        }

                        if (result == MessageMakeError.Ok)
                        {
                            message = this._secsMessageCollection.GetMessageHeader(6, 11);

                            if (message != null)
                            {
                                message.Body.Add(SECSItemFormat.L, 3, null);

                                if (this._dataidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                                }
                                else
                                {
                                    message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                                }

                                if (this._ceidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add(ceidInfo.Name, this._ceidFormat, Encoding.Default.GetByteCount(ceidInfo.CEID), ceidInfo.CEID);
                                }
                                else
                                {
                                    message.Body.Add(ceidInfo.Name, this._ceidFormat, 1, ceidInfo.CEID);
                                }

                                message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                                foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                                {
                                    message.Body.Add(SECSItemFormat.L, 2, null);

                                    if (this._rptidFormat == SECSItemFormat.A)
                                    {
                                        message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                    }
                                    else
                                    {
                                        message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                    }

                                    result = MakeReportInfo(variableCollection, message, tempReportInfo, out errorText);

                                    if (result != MessageMakeError.Ok)
                                    {
                                        message = null;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                result = MessageMakeError.NoDefined;
                            }
                        }
                        else
                        {
                            message = null;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.Disabled;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }
            finally
            {
                reportCollection = null;
            }

            return result;
        }

        public MessageMakeError MakeEventReportByAlarm(CollectionEventInfo ceidInfo, VariableCollection variableCollection, AlarmInfo alarmInfo, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            ReportCollection reportCollection;

            errorText = string.Empty;

            try
            {
                if (ceidInfo != null && ceidInfo.Enabled == true)
                {
                    reportCollection = ceidInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, (ceidInfo.CEID + alarmInfo.ID).Length, ceidInfo.CEID + alarmInfo.ID);
                            }
                            else
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, 1, ceidInfo.CEID + alarmInfo.ID);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(variableCollection, message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeEventReportByLimitMonitoring(
            CollectionEventCollection collectionEventCollection,
            VariableCollection variableCollection,
            LimitMonitoringInfo limitMonitoringInfo,
            out SECSMessage message,
            out string errorText)
        {
            MessageMakeError result;
            CollectionEventInfo ceidInfo;
            ReportCollection reportCollection;

            errorText = string.Empty;

            try
            {
                //ceidInfo = collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.LimitMonitoringBase.ToString());
                ceidInfo = collectionEventCollection.GetCollectionEventInfo(PreDefinedCE.LimitMonitoring.ToString());

                if (ceidInfo != null && ceidInfo.Enabled == true)
                {
                    reportCollection = ceidInfo.Reports;

                    if (reportCollection != null)
                    {
                        result = MessageMakeError.Ok;

                        message = this._secsMessageCollection.GetMessageHeader(6, 11);

                        if (message != null)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            if (this._dataidFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("DATAID", this._dataidFormat, Encoding.Default.GetByteCount(this._dataId.ToString()), this._dataId.ToString());
                            }
                            else
                            {
                                message.Body.Add("DATAID", this._dataidFormat, 1, this._dataId);
                            }

                            if (this._ceidFormat == SECSItemFormat.A)
                            {
                                //message.Body.Add(ceidInfo.Name, this._ceidFormat, (ceidInfo.CEID + limitMonitoringInfo.Variable.VID).Length, ceidInfo.CEID);
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, ceidInfo.CEID.Length, ceidInfo.CEID);
                            }
                            else
                            {
                                message.Body.Add(ceidInfo.Name, this._ceidFormat, 1, ceidInfo.CEID);
                            }

                            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, reportCollection.Items.Count, null);

                            foreach (ReportInfo tempReportInfo in reportCollection.Items.Values)
                            {
                                message.Body.Add(SECSItemFormat.L, 2, null);

                                if (this._rptidFormat == SECSItemFormat.A)
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, Encoding.Default.GetByteCount(tempReportInfo.ReportID), tempReportInfo.ReportID);
                                }
                                else
                                {
                                    message.Body.Add("RPTID", this._rptidFormat, 1, tempReportInfo.ReportID);
                                }

                                result = MakeReportInfo(variableCollection, message, tempReportInfo, out errorText);

                                if (result != MessageMakeError.Ok)
                                {
                                    message = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = MessageMakeError.NoDefined;
                        }
                    }
                    else
                    {
                        message = null;
                        result = MessageMakeError.NoDefined;
                    }
                }
                else
                {
                    message = null;
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }

            return result;
        }

        public MessageMakeError MakeReportInfo(SECSMessage message, ReportInfo reportInfo, out string errorText)
        {
            MessageMakeError result;

            result = MessageMakeError.Ok;
            errorText = string.Empty;

            try
            {
                message.Body.Add("VCOUNT", SECSItemFormat.L, reportInfo.Variables.Items.Count, null);

                foreach (VariableInfo tempVariableInfo in reportInfo.Variables.Items)
                {
                    if (tempVariableInfo.Format == SECSItemFormat.L)
                    {
                        if (tempVariableInfo.ChildVariables != null)
                        {
                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.ChildVariables.Count, null);
                        }
                        else
                        {
                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);
                        }

                        result = MakeVariableInfoByList(message, tempVariableInfo, out errorText);

                        if (result != MessageMakeError.Ok)
                        {
                            break;
                        }
                    }
                    else
                    {
                        CheckVariableValue(tempVariableInfo);

                        if (tempVariableInfo.Format == SECSItemFormat.A)
                        {
                            message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                        }
                        else
                        {
                            result = ValidateFormat(tempVariableInfo, out errorText);

                            if (result == MessageMakeError.Ok)
                            {
                                message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(RPTID={0}){1}", reportInfo.ReportID, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeReportInfo(VariableCollection variableCollection, SECSMessage message, ReportInfo reportInfo, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError childResult;
            VariableInfo variableInfo;
            SECSItemFormat secsItemFormat;

            result = MessageMakeError.Ok;
            errorText = string.Empty;

            try
            {
                message.Body.Add("VCOUNT", SECSItemFormat.L, reportInfo.Variables.Items.Count, null);

                foreach (VariableInfo tempVariableInfo in reportInfo.Variables.Items)
                {
                    if (tempVariableInfo.Format == SECSItemFormat.L)
                    {
                        if (tempVariableInfo.Name == PreDefinedV.ChangedECID.ToString())
                        {
                            CheckVariableValue(variableCollection, tempVariableInfo);

                            secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                            if (secsItemFormat == SECSItemFormat.A)
                            {
                                string[] ecids;

                                ecids = tempVariableInfo.Value;

                                message.Body.Add(tempVariableInfo.Name, SECSItemFormat.L, ecids.Length, null);

                                foreach (string tempEcid in ecids)
                                {
                                    variableInfo = variableCollection[tempEcid];

                                    if (variableInfo != null)
                                    {
                                        message.Body.Add(variableInfo.Name, secsItemFormat, Encoding.Default.GetByteCount(tempEcid), tempEcid);
                                    }
                                    else
                                    {
                                        result = MessageMakeError.NoDefined;
                                        break;
                                    }
                                }

                                ecids = null;
                            }
                            else
                            {
                                long[] ecids;

                                ecids = tempVariableInfo.Value;

                                message.Body.Add(tempVariableInfo.Name, SECSItemFormat.L, ecids.Length, null);

                                foreach (long tempEcid in ecids)
                                {
                                    variableInfo = variableCollection[tempEcid.ToString()];

                                    if (variableInfo != null)
                                    {
                                        childResult = ValidateFormat(variableInfo, out errorText);

                                        if (childResult != MessageMakeError.Ok)
                                        {
                                            result = childResult;
                                        }

                                        if (result == MessageMakeError.Ok)
                                        {
                                            message.Body.Add(variableInfo.Name, secsItemFormat, 1, tempEcid);
                                        }
                                    }
                                    else
                                    {
                                        result = MessageMakeError.NoDefined;
                                        break;
                                    }
                                }

                                ecids = null;
                            }

                            variableInfo = null;
                        }
                        else
                        {
                            if (tempVariableInfo.ChildVariables != null && tempVariableInfo.ChildVariables.Count > 0)
                            {
                                tempVariableInfo.Length = tempVariableInfo.ChildVariables.Count;
                            }
                            else
                            {
                                tempVariableInfo.Length = 0;
                            }

                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);

                            childResult = MakeVariableInfoByList(variableCollection, message, tempVariableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }
                        }
                    }
                    else
                    {
                        variableInfo = variableCollection[tempVariableInfo.VID];

                        CheckVariableValue(variableCollection, variableInfo);

                        if (variableInfo.Format == SECSItemFormat.A)
                        {
                            message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                        }
                        else
                        {
                            childResult = ValidateFormat(variableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }

                            if (result == MessageMakeError.Ok)
                            {
                                message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(RPTID={0}){1}", reportInfo.ReportID, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeReportInfo(VariableCollection allVariables, List<VariableInfo> variables, SECSMessage message, ReportInfo reportInfo, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError childResult;
            VariableInfo variableInfo;
            SECSItemFormat secsItemFormat;

            result = MessageMakeError.Ok;
            errorText = string.Empty;

            try
            {
                message.Body.Add("VCOUNT", SECSItemFormat.L, reportInfo.Variables.Items.Count, null);

                foreach (VariableInfo tempVariableInfo in reportInfo.Variables.Items)
                {
                    if (tempVariableInfo.Format == SECSItemFormat.L)
                    {
                        if (tempVariableInfo.Name == PreDefinedV.ChangedECID.ToString())
                        {
                            CheckVariableValue(allVariables, tempVariableInfo);

                            secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                            if (secsItemFormat == SECSItemFormat.A)
                            {
                                string[] ecids;

                                ecids = tempVariableInfo.Value;

                                message.Body.Add(tempVariableInfo.Name, SECSItemFormat.L, ecids.Length, null);

                                foreach (string tempEcid in ecids)
                                {
                                    variableInfo = allVariables[tempEcid];

                                    if (variableInfo != null)
                                    {
                                        message.Body.Add(variableInfo.Name, secsItemFormat, Encoding.Default.GetByteCount(tempEcid), tempEcid);
                                    }
                                    else
                                    {
                                        result = MessageMakeError.NoDefined;
                                        break;
                                    }
                                }

                                ecids = null;
                            }
                            else
                            {
                                long[] ecids;

                                ecids = tempVariableInfo.Value;

                                message.Body.Add(tempVariableInfo.Name, SECSItemFormat.L, ecids.Length, null);

                                foreach (long tempEcid in ecids)
                                {
                                    variableInfo = variables.FirstOrDefault(t => t.VID == tempEcid.ToString());

                                    if (variableInfo == null)
                                    {
                                        variableInfo = allVariables[tempEcid.ToString()];
                                    }

                                    if (variableInfo != null)
                                    {
                                        childResult = ValidateFormat(variableInfo, out errorText);

                                        if (childResult != MessageMakeError.Ok)
                                        {
                                            result = childResult;
                                        }

                                        if (result == MessageMakeError.Ok)
                                        {
                                            message.Body.Add(variableInfo.Name, secsItemFormat, 1, tempEcid);
                                        }
                                    }
                                    else
                                    {
                                        result = MessageMakeError.NoDefined;
                                        break;
                                    }
                                }

                                ecids = null;
                            }

                            variableInfo = null;
                        }
                        else
                        {
                            if (tempVariableInfo.ChildVariables != null && tempVariableInfo.ChildVariables.Count > 0)
                            {
                                tempVariableInfo.Length = tempVariableInfo.ChildVariables.Count;
                            }
                            else
                            {
                                tempVariableInfo.Length = 0;
                            }

                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);

                            childResult = MakeVariableInfoByList(allVariables, variables, message, tempVariableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }
                        }
                    }
                    else
                    {
                        variableInfo = variables.FirstOrDefault(t => t.Name == tempVariableInfo.Name);

                        if (variableInfo != null)
                        {
                            CheckVariableValue(variables, variableInfo);
                        }
                        else
                        {
                            variableInfo = allVariables[tempVariableInfo.VID];

                            CheckVariableValue(allVariables, variableInfo);
                        }

                        if (variableInfo.Format == SECSItemFormat.A)
                        {
                            message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                        }
                        else
                        {
                            childResult = ValidateFormat(variableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }

                            if (result == MessageMakeError.Ok)
                            {
                                message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(RPTID={0}){1}", reportInfo.ReportID, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeReportInfo(VariableCollection allVariables, List<string> vids, List<string> values, SECSMessage message, ReportInfo reportInfo, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError childResult;
            VariableInfo variableInfo;
            SECSItemFormat secsItemFormat;

            result = MessageMakeError.Ok;
            errorText = string.Empty;

            try
            {
                message.Body.Add("VCOUNT", SECSItemFormat.L, reportInfo.Variables.Items.Count, null);

                foreach (VariableInfo tempVariableInfo in reportInfo.Variables.Items)
                {
                    if (tempVariableInfo.Format == SECSItemFormat.L)
                    {
                        if (tempVariableInfo.Name == PreDefinedV.ChangedECID.ToString())
                        {
                            CheckVariableValue(allVariables, tempVariableInfo);

                            secsItemFormat = GetSECSFormat(PreDefinedDataDictinary.ECID, SECSItemFormat.U4);

                            if (secsItemFormat == SECSItemFormat.A)
                            {
                                string[] ecids;

                                ecids = tempVariableInfo.Value;

                                message.Body.Add(tempVariableInfo.Name, SECSItemFormat.L, ecids.Length, null);

                                foreach (string tempEcid in ecids)
                                {
                                    variableInfo = allVariables[tempEcid];

                                    if (variableInfo != null)
                                    {
                                        message.Body.Add(variableInfo.Name, secsItemFormat, Encoding.Default.GetByteCount(tempEcid), tempEcid);
                                    }
                                    else
                                    {
                                        result = MessageMakeError.NoDefined;
                                        break;
                                    }
                                }

                                ecids = null;
                            }
                            else
                            {
                                long[] ecids;

                                ecids = tempVariableInfo.Value;

                                message.Body.Add(tempVariableInfo.Name, SECSItemFormat.L, ecids.Length, null);

                                foreach (long tempEcid in ecids)
                                {
                                    int index = vids.FindIndex(t => t == tempEcid.ToString());

                                    if (index >= 0)
                                    {
                                        variableInfo = allVariables[tempEcid.ToString()].CopyTo();

                                        variableInfo.Value.SetValue(variableInfo.Format, ConvertSecsValue(variableInfo, variableInfo.Format, values[index]));
                                    }
                                    else
                                    {
                                        variableInfo = allVariables[tempEcid.ToString()];
                                    }

                                    if (variableInfo != null)
                                    {
                                        childResult = ValidateFormat(variableInfo, out errorText);

                                        if (childResult != MessageMakeError.Ok)
                                        {
                                            result = childResult;
                                        }

                                        if (result == MessageMakeError.Ok)
                                        {
                                            message.Body.Add(variableInfo.Name, secsItemFormat, 1, tempEcid);
                                        }
                                    }
                                    else
                                    {
                                        result = MessageMakeError.NoDefined;
                                        break;
                                    }
                                }

                                ecids = null;
                            }

                            variableInfo = null;
                        }
                        else
                        {
                            if (tempVariableInfo.ChildVariables != null && tempVariableInfo.ChildVariables.Count > 0)
                            {
                                tempVariableInfo.Length = tempVariableInfo.ChildVariables.Count;
                            }
                            else
                            {
                                tempVariableInfo.Length = 0;
                            }

                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);

                            childResult = MakeVariableInfoByList(allVariables, vids, values, message, tempVariableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }
                        }
                    }
                    else
                    {
                        int index = vids.FindIndex(t => t == tempVariableInfo.VID);

                        if (index >= 0)
                        {
                            variableInfo = allVariables[tempVariableInfo.VID].CopyTo();

                            variableInfo.Value.SetValue(variableInfo.Format, ConvertSecsValue(variableInfo, variableInfo.Format, values[index]));
                        }
                        else
                        {
                            variableInfo = allVariables[tempVariableInfo.VID];
                        }

                        CheckVariableValue(allVariables, variableInfo);

                        if (variableInfo.Format == SECSItemFormat.A)
                        {
                            message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                        }
                        else
                        {
                            childResult = ValidateFormat(variableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }

                            if (result == MessageMakeError.Ok)
                            {
                                message.Body.Add(ConvertVariableInfo2SECSItem(variableInfo));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(RPTID={0}){1}", reportInfo.ReportID, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeVariableInfoByList(SECSMessage message, VariableInfo variableInfo, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError childResult;

            errorText = string.Empty;
            result = MessageMakeError.Ok;

            try
            {
                if (variableInfo != null && variableInfo.ChildVariables != null)
                {
                    foreach (VariableInfo tempVariableInfo in variableInfo.ChildVariables)
                    {
                        if (tempVariableInfo.Format == SECSItemFormat.L)
                        {
                            if (tempVariableInfo.ChildVariables != null)
                            {
                                message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.ChildVariables.Count, null);
                            }
                            else
                            {
                                message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);
                            }

                            childResult = MakeVariableInfoByList(message, tempVariableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }
                        }
                        else
                        {
                            CheckVariableValue(tempVariableInfo);

                            if (tempVariableInfo.Format == SECSItemFormat.A)
                            {
                                message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                            }
                            else
                            {
                                childResult = ValidateFormat(tempVariableInfo, out errorText);

                                if (childResult != MessageMakeError.Ok)
                                {
                                    result = childResult;
                                }

                                if (result == MessageMakeError.Ok)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(VID={0},VName={1}){2}", variableInfo.VID, variableInfo.Name, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeVariableInfoByList(VariableCollection variableCollection, SECSMessage message, VariableInfo variableInfo, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError childResult;
            VariableInfo getVariableInfo;

            errorText = string.Empty;
            result = MessageMakeError.Ok;

            try
            {
                if (variableInfo != null && variableInfo.ChildVariables != null)
                {
                    foreach (VariableInfo tempVariableInfo in variableInfo.ChildVariables)
                    {
                        if (tempVariableInfo.Format == SECSItemFormat.L)
                        {
                            if (tempVariableInfo.ChildVariables != null)
                            {
                                tempVariableInfo.Length = tempVariableInfo.ChildVariables.Count;
                            }
                            else
                            {
                                tempVariableInfo.Length = 0;
                            }

                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);

                            childResult = MakeVariableInfoByList(variableCollection, message, tempVariableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }
                        }
                        else
                        {
                            if (tempVariableInfo != null)
                            {
                                if (tempVariableInfo.Format == SECSItemFormat.A)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                }
                                else
                                {
                                    childResult = ValidateFormat(tempVariableInfo, out errorText);

                                    if (childResult != MessageMakeError.Ok)
                                    {
                                        result = childResult;
                                    }

                                    if (result == MessageMakeError.Ok)
                                    {
                                        message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                    }
                                }
                            }
                            else
                            {
                                getVariableInfo = variableCollection[tempVariableInfo.VID];

                                CheckVariableValue(variableCollection, getVariableInfo);

                                if (getVariableInfo.Format == SECSItemFormat.A)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(getVariableInfo));
                                }
                                else
                                {
                                    childResult = ValidateFormat(getVariableInfo, out errorText);

                                    if (childResult != MessageMakeError.Ok)
                                    {
                                        result = childResult;
                                    }

                                    if (result == MessageMakeError.Ok)
                                    {
                                        message.Body.Add(ConvertVariableInfo2SECSItem(getVariableInfo));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(VID={0},VName={1}){2}", variableInfo.VID, variableInfo.Name, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeVariableInfoByList(VariableCollection allVariables, List<VariableInfo> variables, SECSMessage message, VariableInfo variableInfo, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError childResult;
            VariableInfo getVariableInfo;

            errorText = string.Empty;
            result = MessageMakeError.Ok;

            try
            {
                if (variableInfo != null && variableInfo.ChildVariables != null)
                {
                    foreach (VariableInfo tempVariableInfo in variableInfo.ChildVariables)
                    {
                        if (tempVariableInfo.Format == SECSItemFormat.L)
                        {
                            if (tempVariableInfo.ChildVariables != null)
                            {
                                tempVariableInfo.Length = tempVariableInfo.ChildVariables.Count;
                            }
                            else
                            {
                                tempVariableInfo.Length = 0;
                            }

                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);

                            childResult = MakeVariableInfoByList(allVariables, variables, message, tempVariableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }
                        }
                        else
                        {
                            if (tempVariableInfo != null)
                            {
                                if (tempVariableInfo.Format == SECSItemFormat.A)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                }
                                else
                                {
                                    childResult = ValidateFormat(tempVariableInfo, out errorText);

                                    if (childResult != MessageMakeError.Ok)
                                    {
                                        result = childResult;
                                    }

                                    if (result == MessageMakeError.Ok)
                                    {
                                        message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                    }
                                }
                            }
                            else
                            {
                                getVariableInfo = variables.FirstOrDefault(t => t.VID == tempVariableInfo.VID);

                                if (getVariableInfo == null)
                                {
                                    getVariableInfo = allVariables[tempVariableInfo.VID];

                                    CheckVariableValue(allVariables, getVariableInfo);
                                }
                                else
                                {
                                    CheckVariableValue(variables, getVariableInfo);
                                }

                                if (getVariableInfo.Format == SECSItemFormat.A)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(getVariableInfo));
                                }
                                else
                                {
                                    childResult = ValidateFormat(getVariableInfo, out errorText);

                                    if (childResult != MessageMakeError.Ok)
                                    {
                                        result = childResult;
                                    }

                                    if (result == MessageMakeError.Ok)
                                    {
                                        message.Body.Add(ConvertVariableInfo2SECSItem(getVariableInfo));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(VID={0},VName={1}){2}", variableInfo.VID, variableInfo.Name, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeVariableInfoByList(VariableCollection allVariables, List<string> vids, List<string> values, SECSMessage message, VariableInfo variableInfo, out string errorText)
        {
            MessageMakeError result;
            MessageMakeError childResult;
            VariableInfo getVariableInfo;

            errorText = string.Empty;
            result = MessageMakeError.Ok;

            try
            {
                if (variableInfo != null && variableInfo.ChildVariables != null)
                {
                    foreach (VariableInfo tempVariableInfo in variableInfo.ChildVariables)
                    {
                        if (tempVariableInfo.Format == SECSItemFormat.L)
                        {
                            if (tempVariableInfo.ChildVariables != null)
                            {
                                tempVariableInfo.Length = tempVariableInfo.ChildVariables.Count;
                            }
                            else
                            {
                                tempVariableInfo.Length = 0;
                            }

                            message.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);

                            childResult = MakeVariableInfoByList(allVariables, vids, values, message, tempVariableInfo, out errorText);

                            if (childResult != MessageMakeError.Ok)
                            {
                                result = childResult;
                            }
                        }
                        else
                        {
                            if (tempVariableInfo != null)
                            {
                                if (tempVariableInfo.Format == SECSItemFormat.A)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                }
                                else
                                {
                                    childResult = ValidateFormat(tempVariableInfo, out errorText);

                                    if (childResult != MessageMakeError.Ok)
                                    {
                                        result = childResult;
                                    }

                                    if (result == MessageMakeError.Ok)
                                    {
                                        message.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                                    }
                                }
                            }
                            else
                            {
                                int index = vids.FindIndex(t => t == tempVariableInfo.VID);

                                if (index >= 0)
                                {
                                    getVariableInfo = allVariables[tempVariableInfo.VID].CopyTo();

                                    getVariableInfo.Value.SetValue(getVariableInfo.Format, ConvertSecsValue(getVariableInfo, getVariableInfo.Format, values[index]));
                                }
                                else
                                {
                                    getVariableInfo = allVariables[tempVariableInfo.VID];
                                }

                                CheckVariableValue(allVariables, getVariableInfo);

                                if (getVariableInfo.Format == SECSItemFormat.A)
                                {
                                    message.Body.Add(ConvertVariableInfo2SECSItem(getVariableInfo));
                                }
                                else
                                {
                                    childResult = ValidateFormat(getVariableInfo, out errorText);

                                    if (childResult != MessageMakeError.Ok)
                                    {
                                        result = childResult;
                                    }

                                    if (result == MessageMakeError.Ok)
                                    {
                                        message.Body.Add(ConvertVariableInfo2SECSItem(getVariableInfo));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = string.Format("(VID={0},VName={1}){2}", variableInfo.VID, variableInfo.Name, ex.Message);
            }

            return result;
        }

        public MessageMakeError MakeTraceReport(VariableCollection variableCollection, TraceInfo traceInfo, out SECSMessage message, out string errorText)
        {
            MessageMakeError result;
            VariableInfo variableInfo;
            string stime;

            result = MessageMakeError.Ok;
            errorText = string.Empty;

            try
            {
                message = this._secsMessageCollection.GetMessageHeader(6, 1);

                if (message != null)
                {
                    message.Body.Add(SECSItemFormat.L, 4, null);

                    SECSItemFormat itemFormat = GetSECSFormat(PreDefinedDataDictinary.TRID, SECSItemFormat.U2);

                    if (itemFormat == SECSItemFormat.A)
                    {
                        message.Body.Add("TRID", itemFormat, traceInfo.TraceID.Length, traceInfo.TraceID);
                    }
                    else
                    {
                        message.Body.Add("TRID", itemFormat, 1, traceInfo.TraceID);
                    }

                    message.Body.Add("SMPLN", GetSECSFormat(PreDefinedDataDictinary.SMPLN, SECSItemFormat.U2), 1, traceInfo.CurrentSample);

                    variableInfo = variableCollection.GetVariableInfo(PreDefinedECV.TimeFormat.ToString());

                    if (variableInfo != null && variableInfo.IsUse == true)
                    {
                        if (variableInfo.Value.GetValue().ToString() == "1")
                        {
                            stime = DateTime.Now.ToString("yyyyMMddHHmmssff");
                        }
                        else
                        {
                            stime = DateTime.Now.ToString("yyMMddHHmmss");
                        }
                    }
                    else
                    {
                        stime = DateTime.Now.ToString("yyyyMMddHHmmssff");
                    }

                    message.Body.Add("STIME", GetSECSFormat(PreDefinedDataDictinary.STIME, SECSItemFormat.A), stime.Length, stime);

                    message.Body.Add("SVCOUNT", SECSItemFormat.L, traceInfo.Values.Count, null);

                    for (int i = 0; i < traceInfo.Values.Count; i++)
                    {
                        if (traceInfo.Values[i].Format == SECSItemFormat.L)
                        {
                            if (traceInfo.Values[i].ChildVariables != null)
                            {
                                message.Body.Add(traceInfo.Values[i].Name, traceInfo.Values[i].Format, traceInfo.Values[i].ChildVariables.Count, null);
                            }
                            else
                            {
                                message.Body.Add(traceInfo.Values[i].Name, traceInfo.Values[i].Format, traceInfo.Values[i].Length, null);
                            }

                            result = MakeVariableInfoByList(message, traceInfo.Values[i], out errorText);

                            if (result != MessageMakeError.Ok)
                            {
                                break;
                            }
                        }
                        else if (traceInfo.Values[i].Format == SECSItemFormat.A)
                        {
                            message.Body.Add(traceInfo.Variables[i % traceInfo.Variables.Count].Name,
                                             traceInfo.Values[i].Format,
                                             Encoding.Default.GetByteCount(traceInfo.Values[i].Value.ToString()),
                                             traceInfo.Values[i].Value);
                        }
                        else
                        {
                            message.Body.Add(traceInfo.Variables[i % traceInfo.Variables.Count].Name,
                                             traceInfo.Values[i].Format,
                                             traceInfo.Values[i].Value.Length,
                                             traceInfo.Values[i].Value);
                        }
                    }
                }
                else
                {
                    result = MessageMakeError.NoDefined;
                }
            }
            catch (Exception ex)
            {
                message = null;
                result = MessageMakeError.Exception;
                errorText = ex.Message;
            }
            finally
            {
            }

            return result;
        }

        public MessageMakeError ValidateFormat(VariableInfo variableInfo)
        {
            MessageMakeError result;
            MessageMakeError childResult;
            bool isInvalidFormat;

            result = MessageMakeError.Ok;

            try
            {
                if (variableInfo.Format == SECSItemFormat.L)
                {
                    foreach (VariableInfo tempVariableInfo in variableInfo.ChildVariables)
                    {
                        childResult = ValidateFormat(tempVariableInfo);

                        if (childResult != MessageMakeError.Ok)
                        {
                            result = childResult;
                        }
                    }
                }
                else if (variableInfo.Format == SECSItemFormat.A ||
                    variableInfo.Format == SECSItemFormat.J)
                {
                }
                else
                {
                    isInvalidFormat = false;

                    if (variableInfo.Length == 1)
                    {
                        if (variableInfo.Value.GetValue() is string)
                        {
                            switch (variableInfo.Format)
                            {
                                case SECSItemFormat.B:
                                case SECSItemFormat.I1:
                                    {
                                        if (sbyte.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        if (short.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        if (int.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        if (long.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.U1:
                                    {
                                        if (byte.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.U2:
                                    {
                                        if (ushort.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.U4:
                                    {
                                        if (uint.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.U8:
                                    {
                                        if (ulong.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.F4:
                                    {
                                        if (float.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.F8:
                                    {
                                        if (double.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                                case SECSItemFormat.Boolean:
                                    {
                                        if (bool.TryParse(variableInfo.Value.GetValue().ToString(), out _) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                switch (variableInfo.Format)
                                {
                                    case SECSItemFormat.B:
                                    case SECSItemFormat.I1:
                                        {
                                            sbyte value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.I2:
                                        {
                                            short value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.I4:
                                        {
                                            int value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.I8:
                                        {
                                            long value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U1:
                                        {
                                            byte value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U2:
                                        {
                                            ushort value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U4:
                                        {
                                            uint value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U8:
                                        {
                                            ulong value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.F4:
                                        {
                                            float value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.F8:
                                        {
                                            double value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.Boolean:
                                        {
                                            bool value = variableInfo.Value;
                                        }
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                isInvalidFormat = true;
                                System.Diagnostics.Debug.Print(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            switch (variableInfo.Format)
                            {
                                case SECSItemFormat.B:
                                case SECSItemFormat.I1:
                                    {
                                        sbyte[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.I2:
                                    {
                                        short[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.I4:
                                    {
                                        int[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.I8:
                                    {
                                        long[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.U1:
                                    {
                                        byte[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.U2:
                                    {
                                        ushort[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.U4:
                                    {
                                        uint[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.U8:
                                    {
                                        ulong[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.F4:
                                    {
                                        float[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.F8:
                                    {
                                        double[] value = variableInfo.Value;
                                    }
                                    break;
                                case SECSItemFormat.Boolean:
                                    {
                                        bool[] value = variableInfo.Value;
                                    }
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            isInvalidFormat = true;
                            System.Diagnostics.Debug.Print(ex.Message);
                        }
                    }

                    if (isInvalidFormat == true)
                    {
                        result = MessageMakeError.InvalidFormat;

                        this.OnMessageMakerLogging?.Invoke(string.Format("Invalid Format:VID={0}, V Name={1}, Format={2}, Value={3}", variableInfo.VID, variableInfo.Name, variableInfo.Format, variableInfo.Value.GetValue()));
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        public MessageMakeError ValidateFormat(VariableInfo variableInfo, out string errorText)
        {
            MessageMakeError result;
            bool isInvalidFormat;

            result = MessageMakeError.Ok;
            errorText = string.Empty;

            try
            {
                if (variableInfo.Format == SECSItemFormat.L ||
                    variableInfo.Format == SECSItemFormat.A ||
                    variableInfo.Format == SECSItemFormat.J)
                {
                }
                else
                {
                    isInvalidFormat = false;

                    if (variableInfo.Value.GetValue() is string)
                    {
                        switch (variableInfo.Format)
                        {
                            case SECSItemFormat.B:
                            case SECSItemFormat.I1:
                                {
                                    if (sbyte.TryParse(variableInfo.Value.GetValue().ToString(), out sbyte value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.I2:
                                {
                                    if (short.TryParse(variableInfo.Value.GetValue().ToString(), out short value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.I4:
                                {
                                    if (int.TryParse(variableInfo.Value.GetValue().ToString(), out int value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.I8:
                                {
                                    if (long.TryParse(variableInfo.Value.GetValue().ToString(), out long value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.U1:
                                {
                                    if (byte.TryParse(variableInfo.Value.GetValue().ToString(), out byte value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.U2:
                                {
                                    if (ushort.TryParse(variableInfo.Value.GetValue().ToString(), out ushort value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.U4:
                                {
                                    if (uint.TryParse(variableInfo.Value.GetValue().ToString(), out uint value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.U8:
                                {
                                    if (ulong.TryParse(variableInfo.Value.GetValue().ToString(), out ulong value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.F4:
                                {
                                    if (float.TryParse(variableInfo.Value.GetValue().ToString(), out float value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.F8:
                                {
                                    if (double.TryParse(variableInfo.Value.GetValue().ToString(), out double value) == false)
                                    {
                                        if (ValidateFormatByEmptyString(variableInfo) == false)
                                        {
                                            isInvalidFormat = true;
                                        }
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                            case SECSItemFormat.Boolean:
                                {
                                    if (bool.TryParse(variableInfo.Value.GetValue().ToString(), out bool value) == false)
                                    {
                                        isInvalidFormat = true;
                                    }
                                    else
                                    {
                                        variableInfo.Value = value;
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (variableInfo.Length == 1 || (variableInfo.Value != null && variableInfo.Value.Length == 1))
                        {
                            try
                            {
                                switch (variableInfo.Format)
                                {
                                    case SECSItemFormat.B:
                                    case SECSItemFormat.I1:
                                        {
                                            sbyte value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.I2:
                                        {
                                            short value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.I4:
                                        {
                                            int value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.I8:
                                        {
                                            long value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U1:
                                        {
                                            byte value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U2:
                                        {
                                            ushort value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U4:
                                        {
                                            uint value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.U8:
                                        {
                                            ulong value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.F4:
                                        {
                                            float value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.F8:
                                        {
                                            double value = variableInfo.Value;
                                        }
                                        break;
                                    case SECSItemFormat.Boolean:
                                        {
                                            bool value = variableInfo.Value;
                                        }
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                isInvalidFormat = true;
                                System.Diagnostics.Debug.Print(ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                switch (variableInfo.Format)
                                {
                                    case SECSItemFormat.B:
                                    case SECSItemFormat.I1:
                                        {
                                            sbyte[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.I2:
                                        {
                                            short[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.I4:
                                        {
                                            int[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.I8:
                                        {
                                            long[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.U1:
                                        {
                                            byte[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.U2:
                                        {
                                            ushort[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.U4:
                                        {
                                            uint[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.U8:
                                        {
                                            ulong[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.F4:
                                        {
                                            float[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.F8:
                                        {
                                            double[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                if (ValidateFormatByEmptyString(variableInfo) == false)
                                                {
                                                    isInvalidFormat = true;
                                                }
                                            }
                                        }
                                        break;
                                    case SECSItemFormat.Boolean:
                                        {
                                            bool[] value = variableInfo.Value;

                                            if (value == null)
                                            {
                                                isInvalidFormat = true;
                                            }
                                        }
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                isInvalidFormat = true;
                                System.Diagnostics.Debug.Print(ex.Message);
                            }
                        }
                    }

                    if (isInvalidFormat == true)
                    {
                        result = MessageMakeError.InvalidFormat;

                        errorText = string.Format("Invalid Format:VID={0}, V Name={1}, Format={2}, Value={3}", variableInfo.VID, variableInfo.Name, variableInfo.Format, variableInfo.Value.GetValue());

                        this.OnMessageMakerLogging?.Invoke(errorText);
                    }
                }
            }
            catch (Exception ex)
            {
                result = MessageMakeError.Exception;
                errorText = ex.Message;
                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        private SECSItemFormat GetSECSFormat(PreDefinedDataDictinary dataDictinary, SECSItemFormat defaultFormat)
        {
            SECSItemFormat result;
            DataDictionaryInfo dataDictionaryInfo;

            dataDictionaryInfo = this._dataDictionaryCollection[dataDictinary.ToString()];
            result = (dataDictionaryInfo != null) ? dataDictionaryInfo.Format.First() : defaultFormat;

            return result;
        }

        private static int GetLength(SECSItemFormat format, object value)
        {
            int result;

            if (format == SECSItemFormat.A || format == SECSItemFormat.J)
            {
                result = Encoding.Default.GetByteCount(value.ToString().Trim());
            }
            else
            {
                if (value == null)
                {
                    result = 0;
                }
                else if (value is string)
                {
                    string[] splitData;

                    splitData = value.ToString().Split(' ');

                    if (splitData != null)
                    {
                        result = splitData.Length;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    if (value is IList list)
                    {
                        result = list.Count;
                    }
                    else if (value.GetType().IsArray == true)
                    {
                        if (value is IEnumerable array)
                        {
                            int totalCount = 0;

                            foreach (var temp in array)
                            {
                                totalCount++;
                            }

                            result = totalCount;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                    else
                    {
                        result = 1;
                    }
                }
            }

            return result;
        }

        private object ConvertSecsValue(VariableInfo variableInfo, SECSItemFormat secsItemFormat, string newValue)
        {
            object result;

            switch (secsItemFormat)
            {
                case SECSItemFormat.B:
                    {
                        if (byte.TryParse(newValue, out byte value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
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
                        if (sbyte.TryParse(newValue, out sbyte value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.I2:
                    {
                        if (short.TryParse(newValue, out short value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.I4:
                    {
                        if (int.TryParse(newValue, out int value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.I8:
                    {
                        if (long.TryParse(newValue, out long value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.U1:
                    {
                        if (byte.TryParse(newValue, out byte value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.U2:
                    {
                        if (ushort.TryParse(newValue, out ushort value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.U4:
                    {
                        if (uint.TryParse(newValue, out uint value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.U8:
                    {
                        if (ulong.TryParse(newValue, out ulong value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.F4:
                    {
                        if (float.TryParse(newValue, out float value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                case SECSItemFormat.F8:
                    {
                        if (double.TryParse(newValue, out double value) == false)
                        {
                            this.OnMessageMakerLogging?.Invoke($"Data conversion failed:VID={variableInfo.VID}, V Name={variableInfo.Name}, Format={secsItemFormat}, Value={newValue}");
                        }

                        result = value;
                    }
                    break;
                default:
                    result = newValue;
                    break;
            }

            return result;
        }

        private static void CheckVariableValue(VariableInfo variableInfo)
        {
            if (variableInfo.Value != null && variableInfo.Value.IsEmpty == true)
            {
                switch (variableInfo.Format)
                {
                    case SECSItemFormat.I1:
                    case SECSItemFormat.I2:
                    case SECSItemFormat.I4:
                    case SECSItemFormat.I8:
                    case SECSItemFormat.U1:
                    case SECSItemFormat.U2:
                    case SECSItemFormat.U4:
                    case SECSItemFormat.U8:
                    case SECSItemFormat.F4:
                    case SECSItemFormat.F8:
                    case SECSItemFormat.B:
                        variableInfo.Value.SetValue(0);

                        break;
                    case SECSItemFormat.Boolean:
                        variableInfo.Value.SetValue(false);
                        break;
                    case SECSItemFormat.J:
                    case SECSItemFormat.A:
                        variableInfo.Value.SetValue(string.Empty);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void CheckVariableValue(VariableCollection variableCollection, VariableInfo variableInfo)
        {
            VariableInfo ecInfo;
            bool assingedValue;

            assingedValue = false;

            if (variableInfo.PreDefined == true)
            {
                if (variableInfo.Name == PreDefinedV.Clock.ToString())
                {
                    assingedValue = true;

                    ecInfo = variableCollection.GetVariableInfo(PreDefinedECV.TimeFormat.ToString());

                    if (ecInfo != null && ecInfo.IsUse == true)
                    {
                        if (ecInfo.Value.GetValue().ToString() == "1")
                        {
                            variableInfo.Value.SetValue(DateTime.Now.ToString("yyyyMMddHHmmssff"));
                        }
                        else
                        {
                            variableInfo.Value.SetValue(DateTime.Now.ToString("yyMMddHHmmss"));
                        }
                    }
                    else
                    {
                        variableInfo.Value.SetValue(DateTime.Now.ToString("yyyyMMddHHmmssff"));
                    }
                }
            }

            if (assingedValue == false && variableInfo.Value != null && variableInfo.Value.IsEmpty == true)
            {
                switch (variableInfo.Format)
                {
                    case SECSItemFormat.I1:
                    case SECSItemFormat.I2:
                    case SECSItemFormat.I4:
                    case SECSItemFormat.I8:
                    case SECSItemFormat.U1:
                    case SECSItemFormat.U2:
                    case SECSItemFormat.U4:
                    case SECSItemFormat.U8:
                    case SECSItemFormat.F4:
                    case SECSItemFormat.F8:
                    case SECSItemFormat.B:
                        variableInfo.Value.SetValue(0);

                        break;
                    case SECSItemFormat.Boolean:
                        variableInfo.Value.SetValue(false);
                        break;
                    case SECSItemFormat.J:
                    case SECSItemFormat.A:
                        variableInfo.Value.SetValue(string.Empty);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void CheckVariableValue(List<VariableInfo> variables, VariableInfo variableInfo)
        {
            VariableInfo ecInfo;
            bool assingedValue;

            assingedValue = false;

            if (variableInfo.PreDefined == true)
            {
                if (variableInfo.Name == PreDefinedV.Clock.ToString())
                {
                    assingedValue = true;

                    ecInfo = variables.FirstOrDefault(t => t.Name == PreDefinedECV.TimeFormat.ToString());

                    if (ecInfo != null && ecInfo.IsUse == true)
                    {
                        if (ecInfo.Value.GetValue().ToString() == "1")
                        {
                            variableInfo.Value.SetValue(DateTime.Now.ToString("yyyyMMddHHmmssff"));
                        }
                        else
                        {
                            variableInfo.Value.SetValue(DateTime.Now.ToString("yyMMddHHmmss"));
                        }
                    }
                    else
                    {
                        variableInfo.Value.SetValue(DateTime.Now.ToString("yyyyMMddHHmmssff"));
                    }
                }
            }

            if (assingedValue == false && variableInfo.Value != null && variableInfo.Value.IsEmpty == true)
            {
                switch (variableInfo.Format)
                {
                    case SECSItemFormat.I1:
                    case SECSItemFormat.I2:
                    case SECSItemFormat.I4:
                    case SECSItemFormat.I8:
                    case SECSItemFormat.U1:
                    case SECSItemFormat.U2:
                    case SECSItemFormat.U4:
                    case SECSItemFormat.U8:
                    case SECSItemFormat.F4:
                    case SECSItemFormat.F8:
                    case SECSItemFormat.B:
                        variableInfo.Value.SetValue(0);

                        break;
                    case SECSItemFormat.Boolean:
                        variableInfo.Value.SetValue(false);
                        break;
                    case SECSItemFormat.J:
                    case SECSItemFormat.A:
                        variableInfo.Value.SetValue(string.Empty);
                        break;
                    default:
                        break;
                }
            }
        }

        private static bool ValidateFormatByEmptyString(VariableInfo variableInfo)
        {
            bool result;

            try
            {
                if (variableInfo.Value != null &&
                    variableInfo.Value.GetValue() is string &&
                    string.IsNullOrEmpty(variableInfo.Value.GetValue().ToString()) == true &&
                    variableInfo.Value.Length == 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                System.Diagnostics.Debug.Print(ex.Message);
            }

            return result;
        }

        private void MakeUserGEMMessage(ref SECSMessage userMessage)
        {
            this.OnUserGEMMessageUpdateRequest?.Invoke(userMessage);
        }

        private void MakeChildSecsItem(VariableCollection variableCollection, VariableInfo variableInfo, ref SECSMessage secsMessage)
        {
            if (variableInfo.Format == SECSItemFormat.L)
            {
                foreach (VariableInfo tempVariableInfo in variableInfo.ChildVariables)
                {
                    CheckVariableValue(variableCollection, tempVariableInfo);

                    if (tempVariableInfo.Format == SECSItemFormat.L)
                    {
                        if (tempVariableInfo.ChildVariables != null)
                        {
                            secsMessage.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.ChildVariables.Count, null);
                        }
                        else
                        {
                            secsMessage.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, tempVariableInfo.Length, null);
                        }

                        MakeChildSecsItem(variableCollection, tempVariableInfo, ref secsMessage);
                    }
                    else if (tempVariableInfo.Format == SECSItemFormat.A)
                    {
                        secsMessage.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                    }
                    else
                    {
                        if (tempVariableInfo.Value != null)
                        {
                            secsMessage.Body.Add(ConvertVariableInfo2SECSItem(tempVariableInfo));
                        }
                        else
                        {
                            secsMessage.Body.Add(tempVariableInfo.Name, tempVariableInfo.Format, 0, tempVariableInfo.Value);
                        }
                    }
                }
            }
        }

        private static string Substring(string source, int startIndex, int length)
        {
            try
            {
                if (string.IsNullOrEmpty(source) == false)
                {
                    Encoding encoding = Encoding.GetEncoding("euc-kr");
                    byte[] buffer = encoding.GetBytes(source);
                    int bufferLength = buffer.Length;

                    if (startIndex < 0)
                    {
                        startIndex = 0;
                    }
                    else if (startIndex > bufferLength)
                    {
                        startIndex = bufferLength;
                    }

                    if (length < 0)
                    {
                        length = 0;
                    }
                    else if (length > bufferLength - startIndex)
                    {
                        length = bufferLength - startIndex;
                    }

                    if (startIndex < bufferLength)
                    {
                        int nCopyStart = 0;
                        int nCopyLen = 0;

                        if (startIndex >= 1)
                        {
                            while (true)
                            {
                                if (buffer[nCopyStart] >= 0x80)
                                {
                                    nCopyStart++;
                                }

                                nCopyStart++;

                                if (nCopyStart >= startIndex)
                                {
                                    if (nCopyStart > startIndex)
                                    {
                                        length--;
                                    }

                                    break;
                                }
                            }
                        }

                        int index = 0;

                        while (index < length)
                        {
                            if (buffer[nCopyStart + index] >= 0x80)
                            {
                                index++;
                            }

                            index++;
                        }

                        nCopyLen = (index <= length) ? index : index - 2;

                        if (nCopyLen >= 1)
                        {
                            return encoding.GetString(buffer, nCopyStart, nCopyLen);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return string.Empty;
        }

        private SECSItem ConvertVariableInfo2SECSItem(VariableInfo variableInfo)
        {
            SECSItem result;
            string errorText;

            try
            {
                if (variableInfo.Format == SECSItemFormat.A || variableInfo.Format == SECSItemFormat.J)
                {
                    VariableInfo orgVariableInfo = this._orgVariableCollection[variableInfo.VID];

                    if (orgVariableInfo != null && orgVariableInfo.Length > -1)
                    {
                        if (variableInfo.Value != null)
                        {
                            string value = variableInfo.Value.ToString();

                            if (orgVariableInfo.Length < Encoding.Default.GetByteCount(value))
                            {
                                string substring = Substring(value, 0, orgVariableInfo.Length);

                                result = new SECSItem(variableInfo.Name, variableInfo.Format, GetLength(variableInfo.Format, substring), substring);

                                errorText = $"Check the length of 'VARIABLE':Variable Name={variableInfo.Name}, Length={orgVariableInfo.Length}, Data Length={Encoding.Default.GetByteCount(value)}, Value={value}";

                                this.OnMessageMakerLogging?.Invoke(errorText);
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

                errorText = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);

                this.OnMessageMakerLogging?.Invoke(errorText);
            }

            return result;
        }
    }
}