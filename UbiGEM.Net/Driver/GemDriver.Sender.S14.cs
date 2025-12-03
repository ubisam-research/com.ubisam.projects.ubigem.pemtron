using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;
using UbiGEM.Net.Utility.Logger;

namespace UbiGEM.Net.Driver
{
    partial class GemDriver
    {
        /// <summary>
        /// Get attribute request를 송신합니다.(S14F1)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="objectType"></param>
        /// <param name="objectIDs"></param>
        /// <param name="objectQualifiers"></param>
        /// <param name="attributes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestGetAttribute(
            string objectSpec,
            string objectType,
            List<string> objectIDs,
            List<ObjectQualifierInfo> objectQualifiers,
            List<string> attributes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestGetAttribute");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 1);

                    message.Body.Add(SECSItemFormat.L, 5, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("OBJTYPE", PreDefinedDataDictinary.OBJTYPE, objectType, SECSItemFormat.A));

                    if (objectIDs != null)
                    {
                        message.Body.Add("OBJECTCOUNT", SECSItemFormat.L, objectIDs.Count, null);

                        foreach (string tempObjectID in objectIDs)
                        {
                            message.Body.Add(GetSECSItem("OBJID", PreDefinedDataDictinary.OBJID, tempObjectID, SECSItemFormat.A));
                        }
                    }
                    else
                    {
                        message.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                    }

                    if (objectQualifiers != null)
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, objectQualifiers.Count, null);

                        foreach (ObjectQualifierInfo tempObjectQualifierInfo in objectQualifiers)
                        {
                            message.Body.Add(SECSItemFormat.L, 3, null);

                            message.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempObjectQualifierInfo.AttributeID, SECSItemFormat.A));

                            if (tempObjectQualifierInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                message.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempObjectQualifierInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempObjectQualifierInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref message);
                                }
                            }
                            else if (tempObjectQualifierInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ATTRDATA", tempObjectQualifierInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempObjectQualifierInfo.AttributeData.AttributeData), tempObjectQualifierInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                //message.Body.Add("ATTRDATA", tempObjectQualifierInfo.AttributeData.AttributeDataFormat, 1, tempObjectQualifierInfo.AttributeData.AttributeData);
                                message.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempObjectQualifierInfo.AttributeData.AttributeDataFormat, tempObjectQualifierInfo.AttributeData.AttributeData));
                            }

                            message.Body.Add(GetSECSItem("ATTRRELN", PreDefinedDataDictinary.ATTRRELN, tempObjectQualifierInfo.AttributeRelationship, SECSItemFormat.U1));
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    }

                    if (attributes != null)
                    {
                        message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, attributes.Count, null);

                        foreach (string tempAttributeID in attributes)
                        {
                            message.Body.Add(GetSECSItem("ATTRIBUTEID", PreDefinedDataDictinary.ATTRID, tempAttributeID, SECSItemFormat.A));
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRIBUTECOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F1)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F1):Result={0}", driverResult);

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
        /// Set attribute request를 송신합니다.(S14F3)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="objectType"></param>
        /// <param name="objectIDs"></param>
        /// <param name="attributes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestSetAttribute(
            string objectSpec,
            string objectType,
            List<string> objectIDs,
            List<AttributeInfo> attributes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestSetAttribute");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 3);

                    message.Body.Add(SECSItemFormat.L, 4, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("OBJTYPE", PreDefinedDataDictinary.OBJTYPE, objectType, SECSItemFormat.A));

                    if (objectIDs != null)
                    {
                        message.Body.Add("OBJECTCOUNT", SECSItemFormat.L, objectIDs.Count, null);

                        foreach (string tempObjectID in objectIDs)
                        {
                            message.Body.Add(GetSECSItem("OBJID", PreDefinedDataDictinary.OBJID, tempObjectID, SECSItemFormat.A));
                        }
                    }
                    else
                    {
                        message.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                    }

                    if (attributes != null)
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in attributes)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            message.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                message.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref message);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                //message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                message.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F3)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F3):Result={0}", driverResult);

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
        /// Get type request를 송신합니다.(S14F5)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestGetType(string objectSpec)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestGetType");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 5);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F5)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F5):Result={0}", driverResult);

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
        /// Get attribute request를 송신합니다.(S14F7)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="objectTypes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestGetAttributeName(string objectSpec, List<string> objectTypes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestGetAttributeName");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 7);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));

                    if (objectTypes != null)
                    {
                        message.Body.Add("OBJECTCOUNT", SECSItemFormat.L, objectTypes.Count, null);

                        foreach (string tempObjectType in objectTypes)
                        {
                            message.Body.Add(GetSECSItem("OBJTYPE", PreDefinedDataDictinary.OBJTYPE, tempObjectType, SECSItemFormat.A));
                        }
                    }
                    else
                    {
                        message.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F7)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F7):Result={0}", driverResult);

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
        /// Create object request를 송신합니다.(S14F9)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="objectType"></param>
        /// <param name="attributes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestCreateObject(
            string objectSpec,
            string objectType,
            List<AttributeInfo> attributes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestCreateObject");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 9);

                    message.Body.Add(SECSItemFormat.L, 3, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("OBJTYPE", PreDefinedDataDictinary.OBJTYPE, objectType, SECSItemFormat.A));

                    if (attributes != null)
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in attributes)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            message.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                message.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref message);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                //message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                message.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F9)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F9):Result={0}", driverResult);

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
        /// Delete object request를 송신합니다.(S14F11)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="attributes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestDeleteObject(string objectSpec, List<AttributeInfo> attributes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestDeleteObject");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 11);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));

                    if (attributes != null)
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in attributes)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            message.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                message.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref message);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                //message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                message.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F11)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F11):Result={0}", driverResult);

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
        /// Object attach request를 송신합니다.(S14F13)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="attributes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestObjectAttach(string objectSpec, List<AttributeInfo> attributes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestObjectAttach");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 13);

                    message.Body.Add(SECSItemFormat.L, 2, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));

                    if (attributes != null)
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in attributes)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            message.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                message.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref message);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                //message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                message.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F13)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F13):Result={0}", driverResult);

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
        /// Create object request를 송신합니다.(S14F15)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="objectCommand"></param>
        /// <param name="objectToken"></param>
        /// <param name="attributes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestAttachedObjectAction(
            string objectSpec,
            ulong objectCommand,
            ulong objectToken,
            List<AttributeInfo> attributes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestAttachedObjectAction");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 15);

                    message.Body.Add(SECSItemFormat.L, 4, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("OBJCMD", PreDefinedDataDictinary.OBJCMD, objectCommand, SECSItemFormat.U1));
                    message.Body.Add(GetSECSItem("OBJTOKEN", PreDefinedDataDictinary.OBJTOKEN, objectToken, SECSItemFormat.U4));

                    if (attributes != null)
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in attributes)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            message.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                message.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref message);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                //message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                message.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F15)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F15):Result={0}", driverResult);

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
        ///  Supervised object action request를 송신합니다.(S14F17)
        /// </summary>
        /// <param name="objectSpec"></param>
        /// <param name="objectCommand"></param>
        /// <param name="targetSpec"></param>
        /// <param name="attributes"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError RequestSupervisedObjectAction(
            string objectSpec,
            int objectCommand,
            string targetSpec,
            List<AttributeInfo> attributes)
        {
            GemDriverError result;
            SECSMessage message;
            MessageError driverResult;
            string logText;

            try
            {
                result = CheckTransmittable("RequestSupervisedObjectAction");

                if (result == GemDriverError.Ok)
                {
                    message = this._driver.Messages.GetMessageHeader(14, 17);

                    message.Body.Add(SECSItemFormat.L, 4, null);

                    message.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, objectSpec, SECSItemFormat.A));
                    message.Body.Add(GetSECSItem("OBJCMD", PreDefinedDataDictinary.OBJCMD, objectCommand, SECSItemFormat.U1));
                    message.Body.Add(GetSECSItem("TARGETSPEC", PreDefinedDataDictinary.TARGETSPEC, targetSpec, SECSItemFormat.A));

                    if (attributes != null)
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in attributes)
                        {
                            message.Body.Add(SECSItemFormat.L, 2, null);

                            message.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                message.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref message);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                //message.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                message.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }
                    else
                    {
                        message.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    }

                    driverResult = this._driver.SendSECSMessage(message);

                    if (driverResult == MessageError.Ok)
                    {
                        result = GemDriverError.Ok;

                        logText = "Transmission successful(S14F17)";

                        this._logger.WriteGEM(LogLevel.Information, logText);
                    }
                    else
                    {
                        result = GemDriverError.HSMSDriverError;

                        logText = string.Format("Transmission failure(S14F17):Result={0}", driverResult);

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
        /// GetAttr Data를 HOST로 송신합니다.(S14F2)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectAttributeResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyGetAttributeRequest(uint systemBytes, RequestObjectAttributeResult requestObjectAttributeResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 2);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (requestObjectAttributeResult != null)
                {
                    replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, requestObjectAttributeResult.ObjectAttributes.Count, null);

                    foreach (ObjectAttributeInfo tempObjectAttributeInfo in requestObjectAttributeResult.ObjectAttributes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("OBJID", PreDefinedDataDictinary.OBJID, tempObjectAttributeInfo.ObjectID, SECSItemFormat.A));

                        replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, tempObjectAttributeInfo.Attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in tempObjectAttributeInfo.Attributes)
                        {
                            replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                            replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                replyMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref replyMessage);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                if (string.IsNullOrEmpty(tempAttributeInfo.AttributeData.AttributeData) == false)
                                {
                                    replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 0, string.Empty);
                                }
                            }
                            else
                            {
                                //replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                replyMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectAttributeResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectAttributeResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectAttributeResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectAttributeResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// SetAttr Data를 HOST로 송신합니다.(S14F4)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectAttributeResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplySetAttributeRequest(uint systemBytes, RequestObjectAttributeResult requestObjectAttributeResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 4);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (requestObjectAttributeResult != null)
                {
                    replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, requestObjectAttributeResult.ObjectAttributes.Count, null);

                    foreach (ObjectAttributeInfo tempObjectAttributeInfo in requestObjectAttributeResult.ObjectAttributes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("OBJID", PreDefinedDataDictinary.OBJID, tempObjectAttributeInfo.ObjectID, SECSItemFormat.A));

                        replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, tempObjectAttributeInfo.Attributes.Count, null);

                        foreach (AttributeInfo tempAttributeInfo in tempObjectAttributeInfo.Attributes)
                        {
                            replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                            replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                            if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                            {
                                replyMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                                foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                                {
                                    MakeChildObjectAttributeData(tempAttributeItem, ref replyMessage);
                                }
                            }
                            else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                            {
                                //replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                                if (string.IsNullOrEmpty(tempAttributeInfo.AttributeData.AttributeData) == false)
                                {
                                    replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                                }
                                else
                                {
                                    replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 0, string.Empty);
                                }
                            }
                            else
                            {
                                //replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                                replyMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectAttributeResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectAttributeResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectAttributeResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add("OBJECTCOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectAttributeResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// GetType Data를 HOST로 송신합니다.(S14F6)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectTypeResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyGetTypeRequest(uint systemBytes, RequestObjectTypeResult requestObjectTypeResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 6);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (requestObjectTypeResult != null)
                {
                    replyMessage.Body.Add("OBJTYPECOUNT", SECSItemFormat.L, requestObjectTypeResult.ObjectTypes.Count, null);

                    foreach (string tempObjectType in requestObjectTypeResult.ObjectTypes)
                    {
                        replyMessage.Body.Add(GetSECSItem("OBJTYPE", PreDefinedDataDictinary.OBJTYPE, tempObjectType, SECSItemFormat.A));
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectTypeResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectTypeResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectTypeResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add("OBJTYPECOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectTypeResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// GetAttrName Data를 HOST로 송신합니다.(S14F8)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectAttributeNameResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyGetAttrNameRequest(uint systemBytes, RequestObjectAttributeNameResult requestObjectAttributeNameResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 8);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (requestObjectAttributeNameResult != null)
                {
                    replyMessage.Body.Add("OBJTYPECOUNT", SECSItemFormat.L, requestObjectAttributeNameResult.ObjectTypes.Count, null);

                    foreach (ObjectTypeInfo tempObjectTypeInfo in requestObjectAttributeNameResult.ObjectTypes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("OBJTYPE", PreDefinedDataDictinary.OBJTYPE, tempObjectTypeInfo.ObjectType, SECSItemFormat.A));

                        replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, tempObjectTypeInfo.AttributeIDs.Count, null);

                        foreach (string tempAttributeID in tempObjectTypeInfo.AttributeIDs)
                        {
                            replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeID, SECSItemFormat.A));
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectAttributeNameResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectAttributeNameResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectAttributeNameResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add("OBJTYPECOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectAttributeNameResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// Create Object Acknowledge를 HOST로 송신합니다.(S14F10)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyCreateObjectRequest(uint systemBytes, RequestObjectResult requestObjectResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 10);

                replyMessage.Body.Add(SECSItemFormat.L, 3, null);

                if (requestObjectResult != null)
                {
                    replyMessage.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, requestObjectResult.ObjectSpec, SECSItemFormat.A));
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, requestObjectResult.ObjectAttributes.Count, null);

                    foreach (AttributeInfo tempAttributeInfo in requestObjectResult.ObjectAttributes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                        if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                        {
                            replyMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                            foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                            {
                                MakeChildObjectAttributeData(tempAttributeItem, ref replyMessage);
                            }
                        }
                        else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                        {
                            if (string.IsNullOrEmpty(tempAttributeInfo.AttributeData.AttributeData) == false)
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 0, string.Empty);
                            }
                        }
                        else
                        {
                            replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 1, tempAttributeInfo.AttributeData.AttributeData);
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add(GetSECSItem("OBJSPEC", PreDefinedDataDictinary.OBJSPEC, string.Empty, SECSItemFormat.A));
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// Delete Object Acknowledge를 HOST로 송신합니다.(S14F12)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyDeleteObjectRequest(uint systemBytes, RequestObjectResult requestObjectResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 12);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (requestObjectResult != null)
                {
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, requestObjectResult.ObjectAttributes.Count, null);

                    foreach (AttributeInfo tempAttributeInfo in requestObjectResult.ObjectAttributes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                        if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                        {
                            replyMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                            foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                            {
                                MakeChildObjectAttributeData(tempAttributeItem, ref replyMessage);
                            }
                        }
                        else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                        {
                            if (string.IsNullOrEmpty(tempAttributeInfo.AttributeData.AttributeData) == false)
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 0, string.Empty);
                            }
                        }
                        else
                        {
                            replyMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// Object Attach Acknowledge를 HOST로 송신합니다.(S14F14)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyObjectAttachRequest(uint systemBytes, RequestObjectResult requestObjectResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 14);

                replyMessage.Body.Add(SECSItemFormat.L, 3, null);

                if (requestObjectResult != null)
                {
                    replyMessage.Body.Add(GetSECSItem("OBJTOKEN", PreDefinedDataDictinary.OBJTOKEN, requestObjectResult.ObjectToken, SECSItemFormat.U4));
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, requestObjectResult.ObjectAttributes.Count, null);

                    foreach (AttributeInfo tempAttributeInfo in requestObjectResult.ObjectAttributes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                        if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                        {
                            replyMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                            foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                            {
                                MakeChildObjectAttributeData(tempAttributeItem, ref replyMessage);
                            }
                        }
                        else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                        {
                            if (string.IsNullOrEmpty(tempAttributeInfo.AttributeData.AttributeData) == false)
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 0, string.Empty);
                            }
                        }
                        else
                        {
                            replyMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add(GetSECSItem("OBJTOKEN", PreDefinedDataDictinary.OBJTOKEN, string.Empty, SECSItemFormat.U4));
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// Attached Object Action Acknowledge를 HOST로 송신합니다.(S14F16)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplyAttachedObjectActionRequest(uint systemBytes, RequestObjectResult requestObjectResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 16);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (requestObjectResult != null)
                {
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, requestObjectResult.ObjectAttributes.Count, null);

                    foreach (AttributeInfo tempAttributeInfo in requestObjectResult.ObjectAttributes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ERRTEXT, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                        if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                        {
                            replyMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                            foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                            {
                                MakeChildObjectAttributeData(tempAttributeItem, ref replyMessage);
                            }
                        }
                        else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                        {
                            if (string.IsNullOrEmpty(tempAttributeInfo.AttributeData.AttributeData) == false)
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                replyMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                            }
                        }
                        else
                        {
                            replyMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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
        /// Supervised Object Action Acknowledge를 HOST로 송신합니다.(S14F18)
        /// </summary>
        /// <param name="systemBytes">Primary message의 system bytes입니다.</param>
        /// <param name="requestObjectResult"></param>
        /// <returns>수행 결과입니다.</returns>
        public GemDriverError ReplySupervisedObjectActionRequest(uint systemBytes, RequestObjectResult requestObjectResult)
        {
            GemDriverError result;
            SECSMessage replyMessage;
            MessageError driverResult;
            string logText;

            try
            {
                result = GemDriverError.Ok;

                replyMessage = this._driver.Messages.GetMessageHeader(14, 18);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                if (requestObjectResult != null)
                {
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, requestObjectResult.ObjectAttributes.Count, null);

                    foreach (AttributeInfo tempAttributeInfo in requestObjectResult.ObjectAttributes)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ATTRID", PreDefinedDataDictinary.ATTRID, tempAttributeInfo.AttributeID, SECSItemFormat.A));

                        if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.L)
                        {
                            replyMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, tempAttributeInfo.AttributeData.ChildItems.Count, null);

                            foreach (AttributeDataItem tempAttributeItem in tempAttributeInfo.AttributeData.ChildItems)
                            {
                                MakeChildObjectAttributeData(tempAttributeItem, ref replyMessage);
                            }
                        }
                        else if (tempAttributeInfo.AttributeData.AttributeDataFormat == SECSItemFormat.A)
                        {
                            if (string.IsNullOrEmpty(tempAttributeInfo.AttributeData.AttributeData) == false)
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeInfo.AttributeData.AttributeData), tempAttributeInfo.AttributeData.AttributeData);
                            }
                            else
                            {
                                replyMessage.Body.Add("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, 0, string.Empty);
                            }
                        }
                        else
                        {
                            replyMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeInfo.AttributeData.AttributeDataFormat, tempAttributeInfo.AttributeData.AttributeData));
                        }
                    }

                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, requestObjectResult.ObjectAck);

                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, requestObjectResult.ObjectErrors.Count, null);

                    foreach (ObjectErrorItem tempObjectErrorItem in requestObjectResult.ObjectErrors)
                    {
                        replyMessage.Body.Add(SECSItemFormat.L, 2, null);

                        replyMessage.Body.Add(GetSECSItem("ERRCODE", PreDefinedDataDictinary.ERRCODE, tempObjectErrorItem.ErrorCode, SECSItemFormat.U1));
                        replyMessage.Body.Add(GetSECSItem("ERRTEXT", PreDefinedDataDictinary.ERRTEXT, tempObjectErrorItem.ErrorText, SECSItemFormat.A));
                    }
                }
                else
                {
                    replyMessage.Body.Add("ATTRCOUNT", SECSItemFormat.L, 0, null);
                    replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("OBJACK", GetSECSFormat(PreDefinedDataDictinary.OBJACK, SECSItemFormat.U1), 1, 1);
                    replyMessage.Body.Add("ERRORCOUNT", SECSItemFormat.L, 0, null);

                    logText = string.Format("S{0}F{1}:{2}), Error Text={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, "'RequestObjectResult' is null");

                    this._logger.WriteGEM(LogLevel.Warning, logText);
                }

                driverResult = this._driver.ReplySECSMessage(systemBytes, replyMessage);

                if (driverResult == MessageError.Ok)
                {
                    logText = string.Format("Transmission successful(S{0}F{1}:{2})", replyMessage.Stream, replyMessage.Function, replyMessage.Name);

                    this._logger.WriteGEM(LogLevel.Information, logText);
                }
                else
                {
                    logText = string.Format("Transmission failure(S{0}F{1}:{2}):Result={3}", replyMessage.Stream, replyMessage.Function, replyMessage.Name, driverResult);

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

        private void MakeChildObjectAttributeData(AttributeDataItem attributeItem, ref SECSMessage secsMessage)
        {
            if (attributeItem.AttributeDataFormat == SECSItemFormat.L)
            {
                secsMessage.Body.Add("ATTRDATACOUNT", SECSItemFormat.L, attributeItem.ChildItems.Count, null);

                foreach (AttributeDataItem tempAttributeItem in attributeItem.ChildItems)
                {
                    if (tempAttributeItem.AttributeDataFormat == SECSItemFormat.L)
                    {
                        MakeChildObjectAttributeData(tempAttributeItem, ref secsMessage);
                    }
                    else if (tempAttributeItem.AttributeDataFormat == SECSItemFormat.A)
                    {
                        secsMessage.Body.Add("ATTRDATA", tempAttributeItem.AttributeDataFormat, Encoding.Default.GetByteCount(tempAttributeItem.AttributeData), tempAttributeItem.AttributeData);
                    }
                    else
                    {
                        //secsMessage.Body.Add("ATTRDATA", tempAttributeItem.AttributeDataFormat, 1, tempAttributeItem.AttributeData);
                        secsMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", tempAttributeItem.AttributeDataFormat, tempAttributeItem.AttributeData));
                    }
                }
            }
            else if (attributeItem.AttributeDataFormat == SECSItemFormat.A)
            {
                secsMessage.Body.Add("ATTRDATA", attributeItem.AttributeDataFormat, Encoding.Default.GetByteCount(attributeItem.AttributeData), attributeItem.AttributeData);
            }
            else
            {
                //secsMessage.Body.Add("ATTRDATA", attributeItem.AttributeDataFormat, 1, attributeItem.AttributeData);
                secsMessage.Body.Add(ConvertStringToSECSItem("ATTRDATA", attributeItem.AttributeDataFormat, attributeItem.AttributeData));
            }
        }
    }
}