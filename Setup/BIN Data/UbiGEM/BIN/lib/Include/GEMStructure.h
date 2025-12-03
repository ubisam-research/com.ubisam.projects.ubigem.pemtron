#pragma once

#ifndef UBISAM_GEM_API
#define UBISAM_GEM_API __declspec(dllimport)
#else
#undef UBISAM_GEM_API
#define UBISAM_GEM_API __declspec(dllexport)
#endif

#include <Windows.h>
#include <stdint.h>
#include <tchar.h>


namespace UbiGEMWrapper {
    namespace Structure {
        typedef enum __GEMResult
        {
            GEMResult_Ok = 0,
            GEMResult_Unknown = 1,
            GEMResult_NotExistDriverName = 2,
            GEMResult_NotExistFile = 3,
            GEMResult_FileLoadFailed = 4,
            GEMResult_FileSaveFailed = 5,
            GEMResult_InvalidConfiguration = 6,
            GEMResult_AlreadyConnected = 7,
            GEMResult_SocketException = 8,
            GEMResult_LicenseVerificationFailed = 9,
            GEMResult_Disconnected = 11,
            GEMResult_ControlStateIsOffline = 12,
            GEMResult_SameState = 13,
            GEMResult_Undefined = 14,
            GEMResult_Disabled = 15,
            GEMResult_HSMSDriverError = 16,
            GEMResult_HSMSDriverDisconnected = 17,
            GEMResult_NotCommunicating = 18,
            GEMResult_MessageMakeFailed = 19,
            GEMResult_Exception = 20,
            GEMResult_Mismatch = 21,
            GEMResult_HostDenied = 22,
            GEMResult_InvalidFormat = 23
        } GEMResult;

        typedef enum __GEMVariableType
        {
            GEMVariable_ECV = 0,
            GEMVariable_SV = 1,
            GEMVariable_DVVAL = 2
        } GEMVariableType;

        typedef enum __GEMMessageDirection
        {
            GEMMessageDirection_ToEquipment,
            GEMMessageDirection_ToHost,
            GEMMessageDirection_Both
        } GEMMessageDirection;

        typedef enum __GEMSECSFormat
        {
            GEMSECSFormat_None = 0,
            GEMSECSFormat_L = 0x01,
            GEMSECSFormat_A = 0x41,
            GEMSECSFormat_B = 0x21,
            GEMSECSFormat_Boolean = 0x25,
            GEMSECSFormat_I1 = 0x65,
            GEMSECSFormat_I2 = 0x69,
            GEMSECSFormat_I4 = 0x71,
            GEMSECSFormat_I8 = 0x61,
            GEMSECSFormat_U1 = 0xa5,
            GEMSECSFormat_U2 = 0xa9,
            GEMSECSFormat_U4 = 0xb1,
            GEMSECSFormat_U8 = 0xa1,
            GEMSECSFormat_F4 = 0x91,
            GEMSECSFormat_F8 = 0x81,
            GEMSECSFormat_J = 0x45,
            GEMSECSFormat_X = 0xff
        } GEMSECSFormat;

        typedef enum __SECSTimeoutType
        {
            SECSTimeoutType_T1 = 0,
            SECSTimeoutType_T2 = 1,
            SECSTimeoutType_T3 = 2,
            SECSTimeoutType_T4 = 3,
            SECSTimeoutType_T5 = 4,
            SECSTimeoutType_T6 = 5,
            SECSTimeoutType_T7 = 6,
            SECSTimeoutType_T8 = 7,
            SECSTimeoutType_Linktest = 8,
        } SECSTimeoutType;

        typedef enum __GEMMessageValidationError
        {
            GEMMessageValidationError_Ok = 0,
            GEMMessageValidationError_UnrecognizedDeviceID = 1,
            GEMMessageValidationError_UnrecognizedSteam = 2,
            GEMMessageValidationError_UnrecognizedFunction = 3,
            GEMMessageValidationError_IllegalDataFormat = 4,
            GEMMessageValidationError_T3Timeout = 5,
            GEMMessageValidationError_DataToLong = 6
        } GEMMessageValidationError;

        typedef enum __GEMSpoolState
        {
            GEMSpoolState_Inactive = 0,
            GEMSpoolState_ActievLoadNotFull = 1,
            GEMSpoolState_ActievLoadFull = 2,
            GEMSpoolState_ActiveUnloadPurge = 3,
            GEMSpoolState_ActiveUnloadTransmit = 4,
            GEMSpoolState_ActiveUnloadNoOutput = 5
        } GEMSpoolState;

        typedef enum __GEMVariableUpdateType
        {
            GEMVariableUpdateType_S1F3 = 0,
            GEMVariableUpdateType_S6F11 = 1,
            GEMVariableUpdateType_S6F19 = 2
        } GEMVariableUpdateType;

        typedef enum __GEMCommunicationState
        {
            GEMCommunicationState_Disabled = 1,
            GEMCommunicationState_Enabled = 2,
            GEMCommunicationState_NotCommunication = 3,
            GEMCommunicationState_WaitCRFromHost = 4,
            GEMCommunicationState_WaitDelay = 5,
            GEMCommunicationState_WaitCRA = 6,
            GEMCommunicationState_Communicating = 7
        } GEMCommunicationState;

        typedef enum __GEMControlState
        {
            GEMControlState_EquipmentOffline = 1,
            GEMControlState_AttemptOnline = 2,
            GEMControlState_HostOffline = 3,
            GEMControlState_OnlineLocal = 4,
            GEMControlState_OnlineRemote = 5
        } GEMControlState;

		template<typename Type>
		class List
		{
		public:
			List();
			~List();
			UBISAM_GEM_API inline void Add(Type t);
			UBISAM_GEM_API inline Type At(int index);
			UBISAM_GEM_API inline int GetCount();
            UBISAM_GEM_API inline void Clear();
		private:
			int _limit;
			int _count;
			Type* _items;
		};
		
		class SECSItemCollection;
        class EnhancedCommandParameterInfo;

        class SECSValue
        {
        public:
            SECSValue(GEMSECSFormat eFormat);
            SECSValue(GEMSECSFormat eFormat, LPCTSTR value);
            SECSValue(GEMSECSFormat eFormat, bool value);
            SECSValue(GEMSECSFormat eFormat, uint8_t value);
            SECSValue(GEMSECSFormat eFormat, uint16_t value);
            SECSValue(GEMSECSFormat eFormat, uint32_t value);
            SECSValue(GEMSECSFormat eFormat, uint64_t value);
            SECSValue(GEMSECSFormat eFormat, int8_t value);
            SECSValue(GEMSECSFormat eFormat, int16_t value);
            SECSValue(GEMSECSFormat eFormat, int32_t value);
            SECSValue(GEMSECSFormat eFormat, int64_t value);
            SECSValue(GEMSECSFormat eFormat, float value);
            SECSValue(GEMSECSFormat eFormat, double value);
            SECSValue(GEMSECSFormat eFormat, List<bool>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<uint8_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<uint16_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<uint32_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<uint64_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<int8_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<int16_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<int32_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<int64_t>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<float>* arrayValue);
            SECSValue(GEMSECSFormat eFormat, List<double>* arrayValue);
            ~SECSValue();

            UBISAM_GEM_API void SetValue(LPCTSTR value);
            UBISAM_GEM_API void SetValue(bool value);
            UBISAM_GEM_API void SetValue(uint8_t value);
            UBISAM_GEM_API void SetValue(uint16_t value);
            UBISAM_GEM_API void SetValue(uint32_t value);
            UBISAM_GEM_API void SetValue(uint64_t value);
            UBISAM_GEM_API void SetValue(int8_t value);
            UBISAM_GEM_API void SetValue(int16_t value);
            UBISAM_GEM_API void SetValue(int32_t value);
            UBISAM_GEM_API void SetValue(int64_t value);
            UBISAM_GEM_API void SetValue(float value);
            UBISAM_GEM_API void SetValue(double value);

            UBISAM_GEM_API void SetValue(List<bool>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint8_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint16_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint32_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint64_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int8_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int16_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int32_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int64_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<float>* arrayValue);
            UBISAM_GEM_API void SetValue(List<double>* arrayValue);

            UBISAM_GEM_API void ClearArrayValue();

            UBISAM_GEM_API LPCTSTR GetValueString();
            UBISAM_GEM_API bool GetValueBool();
            UBISAM_GEM_API uint8_t GetValueB();
            UBISAM_GEM_API uint8_t GetValueU1();
            UBISAM_GEM_API uint16_t GetValueU2();
            UBISAM_GEM_API uint32_t GetValueU4();
            UBISAM_GEM_API uint64_t GetValueU8();
            UBISAM_GEM_API int8_t GetValueI1();
            UBISAM_GEM_API int16_t GetValueI2();
            UBISAM_GEM_API int32_t GetValueI4();
            UBISAM_GEM_API int64_t GetValueI8();
            UBISAM_GEM_API float GetValueF4();
            UBISAM_GEM_API double GetValueF8();

            UBISAM_GEM_API bool* GetValueBoolArray();
            UBISAM_GEM_API uint8_t* GetValueBArray();
            UBISAM_GEM_API uint8_t* GetValueU1Array();
            UBISAM_GEM_API uint16_t* GetValueU2Array();
            UBISAM_GEM_API uint32_t* GetValueU4Array();
            UBISAM_GEM_API uint64_t* GetValueU8Array();
            UBISAM_GEM_API int8_t* GetValueI1Array();
            UBISAM_GEM_API int16_t* GetValueI2Array();
            UBISAM_GEM_API int32_t* GetValueI4Array();
            UBISAM_GEM_API int64_t* GetValueI8Array();
            UBISAM_GEM_API float* GetValueF4Array();
            UBISAM_GEM_API double* GetValueF8Array();
        public:
            GEMSECSFormat Format;
            int Length;

        private:
            void InitArrayValue();

            LPCTSTR _stringValue;
            bool _boolValue;
            uint64_t _uValue;
            int64_t _iValue;
            double _fValue;

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
        };

        class SECSItem
        {
        public:
            SECSItem(LPCTSTR strName, GEMSECSFormat eFormat);
            SECSItem(LPCTSTR strName, SECSValue* secsValue = nullptr);
            ~SECSItem();
            UBISAM_GEM_API void InitializeSubItem();
        public:
            LPCTSTR Name;
            GEMSECSFormat Format;
            int Length;
            SECSItemCollection* SubItem;
            SECSValue* Value;
        };

        class SECSItemCollection
        {
        public:
            SECSItemCollection();
            ~SECSItemCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void Clear();
            UBISAM_GEM_API void AddSECSItem(SECSItem* pSECSItem);
            UBISAM_GEM_API void AddListItem(LPCTSTR strName, int length);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, LPCTSTR value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, bool value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, uint8_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, uint16_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, uint32_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, uint64_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, int8_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, int16_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, int32_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, int64_t value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, float value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, double value);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<bool>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<uint8_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<uint16_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<uint32_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<uint64_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<int8_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<int16_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<int32_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<int64_t>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<float>* arrayValue);
            UBISAM_GEM_API void AddSECSItem(LPCTSTR strName, GEMSECSFormat eFormat, List<double>* arrayValue);

            UBISAM_GEM_API SECSItem* GetSECSItem(LPCTSTR strName);
            UBISAM_GEM_API SECSItem* GetSECSItem(int index);

        public:
            List<SECSItem*>* Items;
        };

        class SECSMessage
        {
        public:
            SECSMessage(LPCTSTR strName, int nStream, int nFunction, bool bUseWaitBit, GEMMessageDirection direction = GEMMessageDirection_Both, LPCTSTR strUserData = nullptr, LPCTSTR strDescription = nullptr);
            ~SECSMessage();
            UBISAM_GEM_API void InitializeBody();
        public:
            LPCTSTR Name;
            int Stream;
            int Function;
            bool UseWaitBit;
            GEMMessageDirection Direction;
            uint32_t SystemBytes;
            LPCTSTR UserData;
            LPCTSTR Description;

            SECSItemCollection* Body;
        };

        class SECSMessageCollection
        {
        public:
            SECSMessageCollection();
            ~SECSMessageCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddSECSMessage(SECSMessage* pSECSMessage);
            UBISAM_GEM_API SECSMessage* GetSECSMessage(LPCTSTR strName);
            UBISAM_GEM_API SECSMessage* GetSECSMessage(int stream, int function);
            UBISAM_GEM_API SECSMessage* GetSECSMessage(int index);

        public:
            List<SECSMessage*>* Items;
        };

        class CommandParameterInfo
        {
        public:
            CommandParameterInfo(LPCTSTR strName, GEMSECSFormat format, SECSValue* value = nullptr);
            ~CommandParameterInfo();
        public:
            LPCTSTR Name;
            GEMSECSFormat Format;
            SECSValue* Value;
        };

        class CommandParameterCollection
        {
        public:
            CommandParameterCollection();
            ~CommandParameterCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API CommandParameterInfo* GetCommandParameterInfo(LPCTSTR strName);
            UBISAM_GEM_API CommandParameterInfo* GetCommandParameterInfo(int index);
        public:
            List<CommandParameterInfo*>* Items;
        };

        class RemoteCommandInfo
        {
        public:
            RemoteCommandInfo(LPCTSTR strCommand, LPCTSTR strDescription = nullptr);
            ~RemoteCommandInfo();
            UBISAM_GEM_API void InitializeCommandParameter();
        public:
            LPCTSTR RemoteCommand;
            LPCTSTR Description;
            uint32_t SystemBytes;
            CommandParameterCollection* CommandParameter;
        };

        class EnhancedCommandParameterItem
        {
        public:
            EnhancedCommandParameterItem(LPCTSTR strName, GEMSECSFormat format, SECSValue* value = nullptr);
            ~EnhancedCommandParameterItem();
            UBISAM_GEM_API void InitializeChildParameterItem();
        public:
            LPCTSTR Name;
            GEMSECSFormat Format;
            SECSValue* Value;
            List<EnhancedCommandParameterItem*>* ChildParameterItems;
        };

        class EnhancedCommandParameterInfo
        {
        public:
            EnhancedCommandParameterInfo(LPCTSTR strName, GEMSECSFormat format, SECSValue* value = nullptr);
            ~EnhancedCommandParameterInfo();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API EnhancedCommandParameterItem* GetEnhancedCommandParameterItem(LPCTSTR strName);
            UBISAM_GEM_API EnhancedCommandParameterItem* GetEnhancedCommandParameterItem(int index);
        public:
            LPCTSTR Name;
            GEMSECSFormat Format;
            SECSValue* Value;
            List<EnhancedCommandParameterItem*>* Items;
        };

        class EnhancedCommandParameterCollection
        {
        public:
            EnhancedCommandParameterCollection();
            ~EnhancedCommandParameterCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API EnhancedCommandParameterInfo* GetEnhancedCommandParameterInfo(LPCTSTR strName);
            UBISAM_GEM_API EnhancedCommandParameterInfo* GetEnhancedCommandParameterInfo(int index);
        public:
            List<EnhancedCommandParameterInfo*>* Items;
        };

        class EnhancedRemoteCommandInfo
        {
        public:
            EnhancedRemoteCommandInfo(uint32_t systemBytes, LPCTSTR strCommand, LPCTSTR strDataID = nullptr, LPCTSTR strObjSpec = nullptr, LPCTSTR strDescription = nullptr);
            ~EnhancedRemoteCommandInfo();
            UBISAM_GEM_API void InitializeEnhancedCommandParameter();
        public:
            uint32_t SystemBytes;
            LPCTSTR RemoteCommand;
            LPCTSTR DataID;
            LPCTSTR ObjSpec;
            LPCTSTR Description;
            EnhancedCommandParameterCollection* EnhancedCommandParameter;
        };

        class RemoteCommandParameterResult
        {
        public:
            RemoteCommandParameterResult(LPCTSTR strCPName, int nParameterAck);
            ~RemoteCommandParameterResult();
            UBISAM_GEM_API void InitializeParameterListAck();
            UBISAM_GEM_API void AddChildParameterResult(RemoteCommandParameterResult* pRemoteCommandParameterResult);
            UBISAM_GEM_API void AddChildParameterResult(LPCTSTR strCPName, int nParameterAck);
        public:
            LPCTSTR CPName;
            int ParameterAck;
            List<RemoteCommandParameterResult*>* ParameterListAck;
        };

        class RemoteCommandResult
        {
        public:
            RemoteCommandResult(int nHostCommandAck = 0);
            ~RemoteCommandResult();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddParameterResult(RemoteCommandParameterResult* pRemoteCommandParameterResult);
            UBISAM_GEM_API void AddParameterResult(LPCTSTR cpName, int parameterAck);
        public:
            int HostCommandAck;
            List<RemoteCommandParameterResult*>* Items;
        };

        class VariableInfo
        {
        public:
            VariableInfo(LPCTSTR strVID, GEMSECSFormat format, LPCTSTR strName = nullptr, SECSValue* value = nullptr, GEMVariableType vidType = GEMVariable_SV, LPCTSTR strDescription = nullptr);
            ~VariableInfo();
            UBISAM_GEM_API void InitializeChildVariables();
            UBISAM_GEM_API void AddChildVariableInfo(VariableInfo* pVariableInfo);
            UBISAM_GEM_API VariableInfo* GetChildVariableInfo(LPCTSTR strName);
            UBISAM_GEM_API VariableInfo* GetChildVariableInfo(int index);

            UBISAM_GEM_API void SetValue(LPCTSTR value);
            UBISAM_GEM_API void SetValue(bool value);
            UBISAM_GEM_API void SetValue(uint8_t value);
            UBISAM_GEM_API void SetValue(uint16_t value);
            UBISAM_GEM_API void SetValue(uint32_t value);
            UBISAM_GEM_API void SetValue(uint64_t value);
            UBISAM_GEM_API void SetValue(int8_t value);
            UBISAM_GEM_API void SetValue(int16_t value);
            UBISAM_GEM_API void SetValue(int32_t value);
            UBISAM_GEM_API void SetValue(int64_t value);
            UBISAM_GEM_API void SetValue(float value);
            UBISAM_GEM_API void SetValue(double value);

            UBISAM_GEM_API void SetValue(List<bool>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint8_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint16_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint32_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<uint64_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int8_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int16_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int32_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<int64_t>* arrayValue);
            UBISAM_GEM_API void SetValue(List<float>* arrayValue);
            UBISAM_GEM_API void SetValue(List<double>* arrayValue);
        public:
            LPCTSTR VID;
            LPCTSTR Name;
            GEMSECSFormat Format;
            GEMVariableType VIDType;
            SECSValue* Value;
            LPCTSTR Description;
            int Length;
            double Min;
            double Max;
            List<VariableInfo*>* ChildVariables;
        };

        class VariableCollection
        {
        public:
            VariableCollection();
            ~VariableCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddVariableInfo(VariableInfo* pVariableInfo);
            UBISAM_GEM_API VariableInfo* GetVariableInfo(LPCTSTR strName);
            UBISAM_GEM_API VariableInfo* GetVariableInfo(int index);
        public:
			List<VariableInfo*>* Items;
        };

        class ReportInfo
        {
        public:
            ReportInfo(LPCTSTR strReportID, LPCTSTR strDescription = nullptr);
            ~ReportInfo();
            UBISAM_GEM_API void InitializeVariables();
        public:
            LPCTSTR ReportID;
            LPCTSTR Description;
            VariableCollection* Variables;
        };

        class ReportCollection
        {
        public:
            ReportCollection();
            ~ReportCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddReportInfo(ReportInfo* pReportInfo);
            UBISAM_GEM_API ReportInfo* GetReportInfo(LPCTSTR strReportId);
            UBISAM_GEM_API ReportInfo* GetReportInfo(int index);
        public:
            List<ReportInfo*>* Items;
        };

        class CollectionEventInfo
        {
        public:
            CollectionEventInfo(LPCTSTR strCEID, LPCTSTR strName = nullptr, LPCTSTR strDescription = nullptr, bool enabled = true);
            ~CollectionEventInfo();
            UBISAM_GEM_API void InitializeReports();
        public:
			LPCTSTR Name;
            LPCTSTR CEID;
            LPCTSTR Description;
            bool Enabled;
			ReportCollection* Reports;
        };

        class CollectionEventCollection
        {
        public:
            CollectionEventCollection();
            ~CollectionEventCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddCollectionEventInfo(CollectionEventInfo* pCollectionEventInfo);
            UBISAM_GEM_API CollectionEventInfo* GetCollectionEventInfo(LPCTSTR strCEID);
            UBISAM_GEM_API CollectionEventInfo* GetCollectionEventInfo(int index);
        public:
			List<CollectionEventInfo*>* Items;
        };

        class FmtPPItem
        {
        public:
            FmtPPItem(LPCTSTR strPPName, LPCTSTR strPPValue, GEMSECSFormat eFormat);
            ~FmtPPItem();
        public:
            LPCTSTR PPName;
            LPCTSTR PPValue;
            GEMSECSFormat Format;
        };

        class FmtPPCCodeInfo
        {
        public:
            FmtPPCCodeInfo(LPCTSTR strCommandCode);
            ~FmtPPCCodeInfo();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddFmtPPItem(FmtPPItem* pFmtPPItem);
            UBISAM_GEM_API void AddFmtPPItem(LPCTSTR strPPValue, GEMSECSFormat eFormat);
            UBISAM_GEM_API void AddFmtPPItem(LPCTSTR strPPName, LPCTSTR strPPValue, GEMSECSFormat eFormat);
            UBISAM_GEM_API FmtPPItem* GetFmtPPItem(LPCTSTR strPPName);
            UBISAM_GEM_API FmtPPItem* GetFmtPPItem(int index);
        public:
            LPCTSTR CommandCode;
            List<FmtPPItem*>* Items;
        };

        class FmtPPCollection
        {
        public:
            FmtPPCollection(LPCTSTR strPPID, LPCTSTR strMDLN = nullptr, LPCTSTR strSOFTREV = nullptr);
            ~FmtPPCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddFmtPPCCodeInfo(FmtPPCCodeInfo* pFmtPPCCodeInfo);
            UBISAM_GEM_API FmtPPCCodeInfo* GetFmtPPCCodeInfo(LPCTSTR strCommandCode);
            UBISAM_GEM_API FmtPPCCodeInfo* GetFmtPPCCodeInfo(int index);
        public:
            LPCTSTR PPID;
            LPCTSTR MDLN;
            LPCTSTR SOFTREV;
            List<FmtPPCCodeInfo*>* Items;
        };

        class FmtPPVerificationInfo
        {
        public:
            FmtPPVerificationInfo(int nACK, int nSeqNum, LPCTSTR strErrW7);
            ~FmtPPVerificationInfo();
        public:
            int ACK;
            int SeqNum;
            LPCTSTR ErrW7;
        };

        class FmtPPVerificationCollection
        {
        public:
            FmtPPVerificationCollection(LPCTSTR strPPID);
            ~FmtPPVerificationCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API void AddFmtPPVerificationInfo(FmtPPVerificationInfo* pFmtPPVerificationInfo);
            UBISAM_GEM_API void AddFmtPPVerificationInfo(int nACK, int nSeqNum, LPCTSTR strErrW7);
            UBISAM_GEM_API FmtPPVerificationInfo* GetFmtPPVerificationInfo(int index);
        public:
            LPCTSTR PPID;
            List<FmtPPVerificationInfo*>* Items;
        };

        class DateTime
        {
        public:
            DateTime(int nYear = 0, int nMonth = 0, int nDay = 0, int nHour = 0, int nMinute = 0, int nSecond = 0, int nMilliSecond = 0);
            ~DateTime();
        public:
            int Year;
            int Month;
            int Day;
            int Hour;
            int Minute;
            int Second;
            int MilliSecond;
        };

        class AlarmInfo
        {
        public:
            AlarmInfo(int64_t alarmID, int code, bool enabled, LPCTSTR strDescription = nullptr);
            ~AlarmInfo();
        public:
            int64_t ID;
            int Code;
            bool Enabled;
            LPCTSTR Description;
        };

        class AlarmCollection
        {
        public:
            AlarmCollection();
            ~AlarmCollection();
            UBISAM_GEM_API void InitializeItems();
            UBISAM_GEM_API AlarmInfo* GetAlarmInfo(int index);
            UBISAM_GEM_API AlarmInfo* GetAlarmInfoByID(int64_t nAlarmId);
        public:
            List<AlarmInfo*>* Items;
        };

        class CustomVariableInfo
        {
        public:
            CustomVariableInfo();
            ~CustomVariableInfo();
            UBISAM_GEM_API void Clear();
            int GetCount();
            SECSItem* At(int index);
            
            UBISAM_GEM_API void AddList(LPCTSTR strName, int length);
            UBISAM_GEM_API void AddList(int length);

            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, LPCTSTR strValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, uint8_t uValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, uint16_t uValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, uint32_t uValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, uint64_t uValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, int8_t iValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, int16_t iValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, int32_t iValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, int64_t iValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, float fValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, double dValue);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, bool boolValue);

            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<uint8_t>* uValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<uint16_t>* uValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<uint32_t>* uValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<uint64_t>* uValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<int8_t>* iValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<int16_t>* iValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<int32_t>* iValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<int64_t>* iValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<float>* fValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<double>* dValues);
            UBISAM_GEM_API void Add(LPCTSTR strName, GEMSECSFormat format, List<bool>* boolValues);

            UBISAM_GEM_API void Add(GEMSECSFormat format, LPCTSTR strValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, uint8_t uValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, uint16_t uValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, uint32_t uValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, uint64_t uValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, int8_t iValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, int16_t iValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, int32_t iValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, int64_t iValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, float fValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, double dValue);
            UBISAM_GEM_API void Add(GEMSECSFormat format, bool boolValue);

            UBISAM_GEM_API void Add(GEMSECSFormat format, List<uint8_t>* uValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<uint16_t>* uValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<uint32_t>* uValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<uint64_t>* uValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<int8_t>* iValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<int16_t>* iValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<int32_t>* iValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<int64_t>* iValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<float>* fValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<double>* dValues);
            UBISAM_GEM_API void Add(GEMSECSFormat format, List<bool>* boolValues);
        private:
            SECSItemCollection* _collection;
        };
	}
}