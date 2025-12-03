#pragma once

#ifndef UBICOM_CPP_API
#define UBICOM_CPP_API __declspec(dllimport)
#else
#define UBICOM_CPP_API __declspec(dllexport)
#endif

#include <string>
#include <vector>

#include <cstdint>

#ifndef UBICOM_CPP_STRUCTURE_H
#define UBICOM_CPP_STRUCTURE_H

#ifndef UNICODE
typedef std::string                                       tstring;
#else
typedef std::wstring                                      tstring;
#endif

#ifndef UNICODE
#define GetIPAddress               GetIPAddressA
#define SetIPAddress               SetIPAddressA
#define GetDriverName              GetDriverNameA
#define SetDriverName              SetDriverNameA
#define GetLogPath                 GetLogPathA
#define SetLogPath                 SetLogPathA
#define GetUMDFileName             GetUMDFileNameA
#define SetUMDFileName             SetUMDFileNameA
#define GetName                    GetNameA
#define SetName                    SetNameA
#define GetValueString             GetValueStringA
#define GetMessageDescription      GetMessageDescriptionA
#else
#define GetIPAddress               GetIPAddressW
#define SetIPAddress               SetIPAddressW
#define GetDriverName              GetDriverNameW
#define SetDriverName              SetDriverNameW
#define GetLogPath                 GetLogPathW
#define SetLogPath                 SetLogPathW
#define GetUMDFileName             GetUMDFileNameW
#define SetUMDFileName             SetUMDFileNameW
#define GetName                    GetNameW
#define SetName                    SetNameW
#define GetValueString             GetValueStringW
#define GetMessageDescription      GetMessageDescriptionW
#endif

namespace UbiCom
{
    namespace CPP
    {
        typedef enum __HSMSMode
        {
            HSMSMode_Passive,
            HSMSMode_Active,
        } HSMSMode;

        typedef enum __SECSDeviceType
        {
            SECSDeviceType_Host,
            SECSDeviceType_Equipment,
        } SECSDeviceType;

        typedef enum __SECSLogMode
        {
            SECSLogMode_None,
            SECSLogMode_Day,
            SECSLogMode_Hour,
        } SECSLogMode;

        typedef enum __SECSMode
        {
            SECSMode_HSMS,
        } SECSMode;

        typedef enum __SECSConnectionState
        {
            SECSConnectionState_Unknown,
            SECSConnectionState_Connected,
            SECSConnectionState_Selected,
            SECSConnectionState_Disconnected,
            SECSConnectionState_Deselected,
        } SECSConnectionState;

        typedef enum __SECSMessageDirection
        {
            SECSMessageDirection_ToEquipment,
            SECSMessageDirection_ToHost,
            SECSMessageDirection_Both
        } SECSMessageDirection;

        typedef enum __SECSItemFormat
        {
            SECSItemFormat_L,
            SECSItemFormat_A,
            SECSItemFormat_J,
            SECSItemFormat_B,
            SECSItemFormat_Boolean,
            SECSItemFormat_F4,
            SECSItemFormat_F8,
            SECSItemFormat_I1,
            SECSItemFormat_I2,
            SECSItemFormat_I4,
            SECSItemFormat_I8,
            SECSItemFormat_U1,
            SECSItemFormat_U2,
            SECSItemFormat_U4,
            SECSItemFormat_U8,
            SECSItemFormat_None,
        } SECSItemFormat;

        typedef enum __SECSTimeoutType
        {
            SECSTimeoutType_Linktest,
            SECSTimeoutType_T1,
            SECSTimeoutType_T2,
            SECSTimeoutType_T3,
            SECSTimeoutType_T4,
            SECSTimeoutType_T5,
            SECSTimeoutType_T6,
            SECSTimeoutType_T7,
            SECSTimeoutType_T8,
            SECSTimeoutType_UnknownTimeout
        } SECSTimeoutType;

        typedef enum __SECSLogLevel
        {
            SECSLogLevel_Error,
            SECSLogLevel_HSMS,
            SECSLogLevel_Information,
            SECSLogLevel_Receive,
            SECSLogLevel_Send,
            SECSLogLevel_Warning,
            SECSLogLevel_UnknownLevel,
        } SECSLogLevel;

        typedef enum __SECSMessageError
        {
            SECSMessageError_DataIsNull,
            SECSMessageError_DuplicateSystemBytes,
            SECSMessageError_InvalidLength,
            SECSMessageError_MessageQueueOverflow,
            SECSMessageError_NoConnected,
            SECSMessageError_NoInitialize,
            SECSMessageError_NotSelected,
            SECSMessageError_Ok,
            SECSMessageError_SocketIsNull,
            SECSMessageError_MessageUnknown,
            SECSMessageError_NotExistPrimaryMessage,
            SECSMessageError_MessageTargetIsNull
        } SECSMessageError;

        typedef enum __SECSDriverError
        {
            SECSDriverError_AlreadyConnected,
            SECSDriverError_FileLoadFailed,
            SECSDriverError_FileSaveFailed,
            SECSDriverError_InvalidConfiguration,
            SECSDriverError_NotExistFile,
            SECSDriverError_NotExistDriverName,
            SECSDriverError_Ok,
            SECSDriverError_SocketException,
            SECSDriverError_TrialVersion,
            SECSDriverError_DriverUnknown,
            SECSDriverError_DriverIsNull,
        } SECSDriverError;

        typedef enum __SECSMessageValidationError
        {
            SECSMessageValidationError_Ok,
            SECSMessageValidationError_UnrecognizedDeviceID,
            SECSMessageValidationError_UnrecognizedSteam,
            SECSMessageValidationError_UnrecognizedFunction,
            SECSMessageValidationError_IllegalDataFormat,
            SECSMessageValidationError_T3Timeout,
            SECSMessageValidationError_DataToLong,
        } SECSMessageValidationError;

        typedef enum __SECSControlMessageType
        {
            SECSControlMessageType_DataMessage,
            SECSControlMessageType_SelectRequest,
            SECSControlMessageType_SelectResponse,
            SECSControlMessageType_DeselectRequest,
            SECSControlMessageType_DeselectResponse,
            SECSControlMessageType_LinktestRequest,
            SECSControlMessageType_LinktestResponse,
            SECSControlMessageType_RejectRequest,
            SECSControlMessageType_SeparateRequest,
        } SECSControlMessageType;

        class SECSItemCollection;

        class SECSConfiguration
        {
        private:
            tstring _driverName;
            tstring _logPath;
            tstring _umdFileName;
            tstring _ipAddress;

        public:
            int DeviceID;
            SECSDeviceType DeviceType;
            bool IsAsyncMode;
            SECSLogMode LogEnabledSECS1;
            SECSLogMode LogEnabledSECS2;
            SECSLogMode LogEnabledSystem;
            int LogExpirationDay;
            double MaxMessageSize;
            SECSMode SECSMode;
            HSMSMode HSMSMode;
            int PortNo;
            int T3;
            int T5;
            int T6;
            int T7;
            int T8;
            int LinkTest;

        public:
            UBICOM_CPP_API SECSConfiguration();
            UBICOM_CPP_API ~SECSConfiguration();

            UBICOM_CPP_API tstring GetDriverName();
            UBICOM_CPP_API void SetDriverName(tstring driverName);
            UBICOM_CPP_API tstring GetLogPath();
            UBICOM_CPP_API void SetLogPath(tstring logPath);
            UBICOM_CPP_API tstring GetUMDFileName();
            UBICOM_CPP_API void SetUMDFileName(tstring umdFileName);
            UBICOM_CPP_API tstring GetIPAddress();
            UBICOM_CPP_API void SetIPAddress(tstring ipAddress);
        };

        class SECSItem
        {
        private:
            tstring _name;

            tstring _stringValue;
            bool _boolValue;
            uint8_t _u1Value;
            uint16_t _u2Value;
            uint32_t _u4Value;
            uint64_t _u8Value;
            int8_t _i1Value;
            int16_t _i2Value;
            int32_t _i4Value;
            int64_t _i8Value;
            float _f4Value;
            double _f8Value;

            bool* _boolArrayValue;
            uint8_t* _u1ArrayValue;
            uint16_t* _u2ArrayValue;
            uint32_t* _u4ArrayValue;
            uint64_t* _u8ArrayValue;
            int8_t* _i1ArrayValue;
            int16_t* _i2ArrayValue;
            int32_t* _i4ArrayValue;
            int64_t* _i8ArrayValue;
            float* _f4ArrayValue;
            double* _f8ArrayValue;

            void InitArrayValue();

        public:
            SECSItemFormat Format;
            int Length;
            SECSItemCollection* SubItem;

            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format);

            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, TCHAR* value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, tstring value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, bool value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, uint8_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, uint16_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, uint32_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, uint64_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int8_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int16_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int32_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int64_t value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, float value);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, double value);

            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, bool* boolArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, uint8_t* u1ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, uint16_t* u2ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, uint32_t* u4ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, uint64_t* u8ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, int8_t* i1ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, int16_t* i2ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, int32_t* i4ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, int64_t* i8ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, float* f4ArrayValue);
            UBICOM_CPP_API SECSItem(tstring name, SECSItemFormat format, int arrayLength, double* f8ArrayValue);

            UBICOM_CPP_API ~SECSItem();

            UBICOM_CPP_API tstring GetName();
            UBICOM_CPP_API void SetName(tstring name);

            UBICOM_CPP_API void SetValue(tstring value);
            UBICOM_CPP_API void SetValue(bool value);
            UBICOM_CPP_API void SetValue(uint8_t value);
            UBICOM_CPP_API void SetValue(uint16_t value);
            UBICOM_CPP_API void SetValue(uint32_t value);
            UBICOM_CPP_API void SetValue(uint64_t value);
            UBICOM_CPP_API void SetValue(int8_t value);
            UBICOM_CPP_API void SetValue(int16_t value);
            UBICOM_CPP_API void SetValue(int32_t value);
            UBICOM_CPP_API void SetValue(int64_t value);
            UBICOM_CPP_API void SetValue(float value);
            UBICOM_CPP_API void SetValue(double value);

            UBICOM_CPP_API void SetValue(int arrayLength, bool* boolArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, uint8_t* u1ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, uint16_t* u2ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, uint32_t* u4ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, uint64_t* u8ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, int8_t* i1ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, int16_t* i2ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, int32_t* i4ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, int64_t* i8ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, float* f4ArrayValue);
            UBICOM_CPP_API void SetValue(int arrayLength, double* f8ArrayValue);

            UBICOM_CPP_API void ClearArrayValue();

            UBICOM_CPP_API tstring GetValueString();
            UBICOM_CPP_API bool GetValueBool();
            UBICOM_CPP_API uint8_t GetValueB();
            UBICOM_CPP_API uint8_t GetValueU1();
            UBICOM_CPP_API uint16_t GetValueU2();
            UBICOM_CPP_API uint32_t GetValueU4();
            UBICOM_CPP_API uint64_t GetValueU8();
            UBICOM_CPP_API int8_t GetValueI1();
            UBICOM_CPP_API int16_t GetValueI2();
            UBICOM_CPP_API int32_t GetValueI4();
            UBICOM_CPP_API int64_t GetValueI8();
            UBICOM_CPP_API float GetValueF4();
            UBICOM_CPP_API double GetValueF8();

            UBICOM_CPP_API bool* GetValueBoolArray();
            UBICOM_CPP_API uint8_t* GetValueBArray();
            UBICOM_CPP_API uint8_t* GetValueU1Array();
            UBICOM_CPP_API uint16_t* GetValueU2Array();
            UBICOM_CPP_API uint32_t* GetValueU4Array();
            UBICOM_CPP_API uint64_t* GetValueU8Array();
            UBICOM_CPP_API int8_t* GetValueI1Array();
            UBICOM_CPP_API int16_t* GetValueI2Array();
            UBICOM_CPP_API int32_t* GetValueI4Array();
            UBICOM_CPP_API int64_t* GetValueI8Array();
            UBICOM_CPP_API float* GetValueF4Array();
            UBICOM_CPP_API double* GetValueF8Array();

            UBICOM_CPP_API void AddChild(SECSItem* item);

            UBICOM_CPP_API void AddListChild(tstring name, int listLength);

            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, TCHAR* value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, tstring value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, bool value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint8_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint16_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint32_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint64_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int8_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int16_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int32_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int64_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, float value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, double value);

            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, bool* boolArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint8_t* u1ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint16_t* u2ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint32_t* u4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint64_t* u8ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int8_t* i1ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int16_t* i2ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int32_t* i4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int64_t* i8ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, float* f4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, double* f8ArrayValue);

        };

        class SECSItemCollection
        {
        public:
            std::vector<SECSItem*>* Items;

            UBICOM_CPP_API SECSItemCollection();
            UBICOM_CPP_API ~SECSItemCollection();

            UBICOM_CPP_API size_t GetCount();

            UBICOM_CPP_API void AddChild(SECSItem* item);

            UBICOM_CPP_API void AddListChild(tstring name, int listLength);

            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, TCHAR* value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, tstring value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, bool value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint8_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint16_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint32_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint64_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int8_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int16_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int32_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int64_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, float value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, double value);

            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, bool* boolArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint8_t* u1ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint16_t* u2ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint32_t* u4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint64_t* u8ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int8_t* i1ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int16_t* i2ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int32_t* i4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int64_t* i8ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, float* f4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, double* f8ArrayValue);

            UBICOM_CPP_API SECSItem* GetItem(size_t index);
            UBICOM_CPP_API SECSItem* GetItem(tstring name);
        };

        class SECSMessage
        {
        private:
            tstring _name;
            tstring _messageDescription;

        public:
            SECSControlMessageType ControlMessageType;
            int Stream;
            int Function;
            bool WaitBit;
            SECSMessageDirection Direction;
            bool AutoReply;
            uint32_t SystemBytes;
            int DeviceID;
            int Length;
            bool NoLogging;
            int StatusCode;

            std::vector<SECSItem*>* Body;
            SECSItemCollection* Items;
            SECSItem* UserData;

            UBICOM_CPP_API SECSMessage();
            UBICOM_CPP_API ~SECSMessage();

            UBICOM_CPP_API tstring GetName();
            UBICOM_CPP_API tstring GetMessageDescription();

            UBICOM_CPP_API void SetName(tstring name);
            UBICOM_CPP_API void SetMessageDescription(tstring description);

            UBICOM_CPP_API void AddChild(SECSItem* item);

            UBICOM_CPP_API void AddListChild(tstring name, int listLength);

            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, TCHAR* value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, tstring value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, bool value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint8_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint16_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint32_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, uint64_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int8_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int16_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int32_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int64_t value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, float value);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, double value);

            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, bool* boolArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint8_t* u1ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint16_t* u2ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint32_t* u4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, uint64_t* u8ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int8_t* i1ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int16_t* i2ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int32_t* i4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, int64_t* i8ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, float* f4ArrayValue);
            UBICOM_CPP_API void AddChild(tstring name, SECSItemFormat format, int arrayLength, double* f8ArrayValue);
        };
    }
}

#endif