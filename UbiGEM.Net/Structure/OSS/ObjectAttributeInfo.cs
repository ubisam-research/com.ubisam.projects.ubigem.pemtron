using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Object attribute 정보입니다.
    /// </summary>
    public class ObjectAttributeInfo
    {
        /// <summary>
        /// Object ID를 가져오거나 설정합니다.
        /// </summary>
        public string ObjectID { get; set; }

        /// <summary>
        /// Object attribute 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<AttributeInfo> Attributes { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ObjectAttributeInfo()
        {
            this.Attributes = new List<AttributeInfo>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Object ID={0}, Object attribute count={1}",
                this.ObjectID, this.Attributes.Count);
        }
    }

    /// <summary>
    /// Object type 정보입니다.
    /// </summary>
    public class ObjectTypeInfo
    {
        /// <summary>
        /// Object type을 가져오거나 설정합니다.
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Object attribute ID 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<string> AttributeIDs { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ObjectTypeInfo()
        {
            this.AttributeIDs = new List<string>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Object Type={0}, Object attribute count={1}",
                this.ObjectType, this.AttributeIDs.Count);
        }
    }

    /// <summary>
    /// Object error item 정보입니다.
    /// </summary>
    public class ObjectErrorItem
    {
        /// <summary>
        /// Error code를 가져오거나 설정합니다.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Error text를 가져오거나 설정합니다.
        /// </summary>
        public string ErrorText { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ObjectErrorItem()
        {
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Error Code={0}, Error Text={1}",
                this.ErrorCode, this.ErrorText);
        }
    }

    /// <summary>
    /// Object qualifier 정보입니다.
    /// </summary>
    public class ObjectQualifierInfo
    {
        /// <summary>
        /// Attribute ID를 가져오거나 설정합니다.
        /// </summary>
        public string AttributeID { get; set; }

        /// <summary>
        /// Attribute data를 가져오거나 설정합니다.
        /// </summary>
        public AttributeDataItem AttributeData { get; set; }

        /// <summary>
        /// Attribute relationship을 가져오거나 설정합니다.
        /// </summary>
        public int AttributeRelationship { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public ObjectQualifierInfo()
        {
            this.AttributeData = new AttributeDataItem();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("ID={0}, Data=[{1}], Relationship={2}",
                this.AttributeID, this.AttributeData, this.AttributeRelationship);
        }
    }

    /// <summary>
    /// Attribute data item 정보입니다.
    /// </summary>
    public class AttributeDataItem
    {
        /// <summary>
        /// Attribute data를 가져오거나 설정합니다.
        /// </summary>
        public string AttributeData { get; set; }

        /// <summary>
        /// Attribute data의 SECS format을 가져오거나 설정합니다.
        /// </summary>
        public SECSItemFormat AttributeDataFormat { get; set; }

        /// <summary>
        /// Attribute data의 child를 가져오거나 설정합니다. (AttributeDataFormat = L인 경우)
        /// </summary>
        public List<AttributeDataItem> ChildItems { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public AttributeDataItem()
        {
            this.AttributeDataFormat = SECSItemFormat.A;

            this.ChildItems = new List<AttributeDataItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            if (this.AttributeDataFormat == SECSItemFormat.L)
            {
                return string.Format("Data Format={0}, Data={1}, Child Count={2}",
                    this.AttributeDataFormat, this.AttributeData, this.ChildItems.Count);
            }
            else
            {
                return string.Format("Data Format={0}, Data={1}",
                    this.AttributeDataFormat, this.AttributeData);
            }
        }
    }

    /// <summary>
    /// Attribute 정보입니다.
    /// </summary>
    public class AttributeInfo
    {
        /// <summary>
        /// Attribute ID를 가져오거나 설정합니다.
        /// </summary>
        public string AttributeID { get; set; }

        /// <summary>
        /// Attribute data를 가져오거나 설정합니다.
        /// </summary>
        public AttributeDataItem AttributeData { get; set; }

        /// <summary>
        /// Attribute data를 가져옵니다. (단, Data format이 List가 아닌 경우)
        /// </summary>
        public string Value
        {
            get
            {
                if (this.AttributeData.AttributeDataFormat != SECSItemFormat.L)
                {
                    return this.AttributeData.AttributeData;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public AttributeInfo()
        {
            this.AttributeData = new AttributeDataItem();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("ID={0}, {1}",
                this.AttributeID, this.AttributeData);
        }
    }

    /// <summary>
    /// Request object attribute result 정보입니다.(Get/Set)
    /// </summary>
    public class RequestObjectAttributeResult
    {
        /// <summary>
        /// Object Ack를 가져오거나 설정합니다.
        /// </summary>
        public int ObjectAck { get; set; }
        /// <summary>
        /// Object attribute 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<ObjectAttributeInfo> ObjectAttributes { get; set; }

        /// <summary>
        /// Object error 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<ObjectErrorItem> ObjectErrors { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public RequestObjectAttributeResult()
        {
            this.ObjectAttributes = new List<ObjectAttributeInfo>();
            this.ObjectErrors = new List<ObjectErrorItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Object Ack={0}, Attribute Count={1}, Error Count={2}", this.ObjectAck, this.ObjectAttributes.Count, this.ObjectErrors.Count);
        }
    }

    /// <summary>
    /// Request object attribute name result 정보입니다.(Get)
    /// </summary>
    public class RequestObjectAttributeNameResult
    {
        /// <summary>
        /// Object Ack를 가져오거나 설정합니다.
        /// </summary>
        public int ObjectAck { get; set; }

        /// <summary>
        /// User defined ack code(ObjectAck = UserDefine인 경우)
        /// </summary>
        public int UserDefineAck { get; set; }

        /// <summary>
        /// Object ID 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<ObjectTypeInfo> ObjectTypes { get; set; }

        /// <summary>
        /// Object error 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<ObjectErrorItem> ObjectErrors { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public RequestObjectAttributeNameResult()
        {
            this.ObjectTypes = new List<ObjectTypeInfo>();
            this.ObjectErrors = new List<ObjectErrorItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Object Ack={0}, Type Count={1}, Error Count={2}", this.ObjectAck, this.ObjectTypes.Count, this.ObjectErrors.Count);
        }
    }

    /// <summary>
    /// Request object type result 정보입니다.(Get)
    /// </summary>
    public class RequestObjectTypeResult
    {
        /// <summary>
        /// Object Ack를 가져오거나 설정합니다.
        /// </summary>
        public int ObjectAck { get; set; }

        /// <summary>
        /// User defined ack code(ObjectAck = UserDefine인 경우)
        /// </summary>
        public int UserDefineAck { get; set; }

        /// <summary>
        /// Object type 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<string> ObjectTypes { get; set; }


        /// <summary>
        /// Object error 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<ObjectErrorItem> ObjectErrors { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public RequestObjectTypeResult()
        {
            this.ObjectTypes = new List<string>();
            this.ObjectErrors = new List<ObjectErrorItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Object Ack={0}, Type Count={1}, Error Count={2}", this.ObjectAck, this.ObjectTypes.Count, this.ObjectErrors.Count);
        }
    }

    /// <summary>
    /// Request object result 정보입니다.
    /// </summary>
    public class RequestObjectResult
    {
        /// <summary>
        /// Object Ack를 가져오거나 설정합니다.
        /// </summary>
        public int ObjectAck { get; set; }

        /// <summary>
        /// User defined ack code(ObjectAck = UserDefine인 경우)
        /// </summary>
        public int UserDefineAck { get; set; }

        /// <summary>
        /// Object spec을 가져오거나 설정합니다.
        /// </summary>
        public string ObjectSpec { get; set; }

        /// <summary>
        /// Object token을 가져오거나 설정합니다.
        /// </summary>
        public uint ObjectToken { get; set; }

        /// <summary>
        /// Object type 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<AttributeInfo> ObjectAttributes { get; set; }

        /// <summary>
        /// Object error 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<ObjectErrorItem> ObjectErrors { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public RequestObjectResult()
        {
            this.ObjectAttributes = new List<AttributeInfo>();
            this.ObjectErrors = new List<ObjectErrorItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Object Ack={0}, Attribute Count={1}, Error Count={2}", this.ObjectAck, this.ObjectAttributes.Count, this.ObjectErrors.Count);
        }
    }
}