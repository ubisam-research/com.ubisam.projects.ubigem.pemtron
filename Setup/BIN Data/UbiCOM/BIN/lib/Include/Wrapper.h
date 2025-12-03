#pragma once

#ifndef UBICOM_CPP_API
#define UBICOM_CPP_API __declspec(dllimport)
#else
#define UBICOM_CPP_API __declspec(dllexport)
#endif

#include <string>
#include <map>
#include <vector>

#include <cstdint>

#include "Structure.h"

#ifndef UBICOM_CPP_WRAPPER_H
#define UBICOM_CPP_WRAPPER_H

#ifndef UNICODE
typedef std::string                                       tstring;
#else
typedef std::wstring                                      tstring;
#endif


#ifndef UNICODE
#define tcout                                             cout
#define SubscribeSECSConnectedEventHandler                SubscribeSECSConnectedEventHandlerA
#define SubscribeSECSDisconnectedEventHandler			  SubscribeSECSDisconnectedEventHandlerA
#define SubscribeSECSSelectedEventHandler				  SubscribeSECSSelectedEventHandlerA
#define SubscribeSECSDeselectedEventHandler				  SubscribeSECSDeselectedEventHandlerA
#define SubscribeSECS1WriteLogEventHandler				  SubscribeSECS1WriteLogEventHandlerA
#define SubscribeSECS2WriteLogEventHandler				  SubscribeSECS2WriteLogEventHandlerA
#define OnSECSConnectedEventHandler                       OnSECSConnectedEventHandlerA
#define OnSECSDisconnectedEventHandler					  OnSECSDisconnectedEventHandlerA
#define OnSECSSelectedEventHandler						  OnSECSSelectedEventHandlerA
#define OnSECSDeselectedEventHandler					  OnSECSDeselectedEventHandlerA
#define OnSECS1WriteLogEventHandler						  OnSECS1WriteLogEventHandlerA
#define OnSECS2WriteLogEventHandler						  OnSECS2WriteLogEventHandlerA
#else
#define tcout                                             wcout
#define SubscribeSECSConnectedEventHandler                SubscribeSECSConnectedEventHandlerW
#define SubscribeSECSDisconnectedEventHandler			  SubscribeSECSDisconnectedEventHandlerW
#define SubscribeSECSSelectedEventHandler				  SubscribeSECSSelectedEventHandlerW
#define SubscribeSECSDeselectedEventHandler				  SubscribeSECSDeselectedEventHandlerW
#define SubscribeSECS1WriteLogEventHandler				  SubscribeSECS1WriteLogEventHandlerW
#define SubscribeSECS2WriteLogEventHandler				  SubscribeSECS2WriteLogEventHandlerW
#define OnSECSConnectedEventHandler                       OnSECSConnectedEventHandlerW
#define OnSECSDisconnectedEventHandler					  OnSECSDisconnectedEventHandlerW
#define OnSECSSelectedEventHandler						  OnSECSSelectedEventHandlerW
#define OnSECSDeselectedEventHandler					  OnSECSDeselectedEventHandlerW
#define OnSECS1WriteLogEventHandler						  OnSECS1WriteLogEventHandlerW
#define OnSECS2WriteLogEventHandler						  OnSECS2WriteLogEventHandlerW
#endif

namespace UbiCom
{
	namespace CPP
	{
		class HSMSDriverAccessor;

		class IHSMSDriverEvents
		{
		public:
			virtual void OnReceivedInvalidPrimaryMessageEventHandler(SECSMessageValidationError reason, SECSMessage* message) = 0;
			virtual void OnReceivedInvalidSecondaryMessageEventHandler(SECSMessageValidationError reason, SECSMessage* primaryMessage, SECSMessage* secondaryMessage) = 0;
			virtual void OnSentControlMessageEventHandler(SECSMessage* message) = 0;
			virtual void OnSentSECSMessageEventHandler(SECSMessage* message) = 0;
			virtual void OnReceivedControlMessageEventHandler(SECSMessage* message) = 0;
			virtual void OnReceivedPrimaryMessageEventHandler(SECSMessage* message) = 0;
			virtual void OnReceivedSecondaryMessageEventHandler(SECSMessage* primaryMessage, SECSMessage* secondaryMessage) = 0;
			virtual void OnReceivedUnknownMessageEventHandler(SECSMessage* message) = 0;
			virtual void OnTimeoutEventHandler(SECSTimeoutType timeoutType) = 0;
			virtual void OnT3TimeoutEventHandler(SECSMessage* message) = 0;

			virtual void OnSECSConnectedEventHandler(tstring& ipAddress, int portNo) = 0;
			virtual void OnSECSDisconnectedEventHandler(tstring& ipAddress, int portNo) = 0;
			virtual void OnSECSSelectedEventHandler(tstring& ipAddress, int portNo) = 0;
			virtual void OnSECSDeselectedEventHandler(tstring& ipAddress, int portNo) = 0;
			virtual void OnSECS1WriteLogEventHandler(SECSLogLevel logLevel, tstring& logText) = 0;
			virtual void OnSECS2WriteLogEventHandler(SECSLogLevel logLevel, tstring& logText) = 0;
		};

		class HSMSDriver
		{
		public:
			UBICOM_CPP_API HSMSDriver();
			UBICOM_CPP_API ~HSMSDriver();

			UBICOM_CPP_API SECSConfiguration* GetConfiguration();
			UBICOM_CPP_API void SetConfiguration(SECSConfiguration* configuration);
			
			UBICOM_CPP_API SECSDriverError Initialize(SECSConfiguration* configuration);
			UBICOM_CPP_API SECSDriverError Initialize(tstring ucfFilePath, tstring driverName);

			UBICOM_CPP_API SECSDriverError Open();
			UBICOM_CPP_API void Close();

			UBICOM_CPP_API SECSMessage* GetSECSMessageHeader(int stream, int function);
			UBICOM_CPP_API SECSMessage* GetSECSMessage(int stream, int function);

			UBICOM_CPP_API void AddUserDefinedMessage(SECSMessage* message);
			UBICOM_CPP_API SECSMessageError SendSECSMessage(SECSMessage* message);
			UBICOM_CPP_API SECSMessageError ReplySECSMessage(SECSMessage* primaryMessage, SECSMessage* secondaryMessage);
			UBICOM_CPP_API SECSMessageError ReplySECSMessage(uint32_t primarySystemBytes, SECSMessage* secondaryMessage);

			UBICOM_CPP_API void UnsubscribeEvents();

			UBICOM_CPP_API void SubscribeReceivedInvalidPrimaryMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeReceivedInvalidSecondaryMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeSentControlMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeSentSECSMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeReceivedControlMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeReceivedPrimaryMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeReceivedSecondaryMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeReceivedUnknownMessageEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeTimeoutEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeT3TimeoutEventHandler(IHSMSDriverEvents* driverEventReceiver);

			UBICOM_CPP_API void SubscribeSECSConnectedEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeSECSDisconnectedEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeSECSSelectedEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeSECSDeselectedEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeSECS1WriteLogEventHandler(IHSMSDriverEvents* driverEventReceiver);
			UBICOM_CPP_API void SubscribeSECS2WriteLogEventHandler(IHSMSDriverEvents* driverEventReceiver);

		private:
			HSMSDriverAccessor* m_pAccessor;
			SECSConfiguration* m_pConfiguration;

			IHSMSDriverEvents* m_pfnReceivedInvalidPrimaryMessageEventHandler;
			IHSMSDriverEvents* m_pfnReceivedInvalidSecondaryMessageEventHandler;
			IHSMSDriverEvents* m_pfnSentControlMessageEventHandler;
			IHSMSDriverEvents* m_pfnSentSECSMessageEventHandler;
			IHSMSDriverEvents* m_pfnReceivedControlMessageEventHandler;
			IHSMSDriverEvents* m_pfnReceivedPrimaryMessageEventHandler;
			IHSMSDriverEvents* m_pfnReceivedSecondaryMessageEventHandler;
			IHSMSDriverEvents* m_pfnReceivedUnknownMessageEventHandler;
			IHSMSDriverEvents* m_pfnTimeoutEventHandler;
			IHSMSDriverEvents* m_pfnT3TimeoutEventHandler;

			IHSMSDriverEvents* m_pfnSECSConnectedEventHandler;
			IHSMSDriverEvents* m_pfnSECSDisconnectedEventHandler;
			IHSMSDriverEvents* m_pfnSECSSelectedEventHandler;
			IHSMSDriverEvents* m_pfnSECSDeselectedEventHandler;
			IHSMSDriverEvents* m_pfnSECS1WriteLogEventHandler;
			IHSMSDriverEvents* m_pfnSECS2WriteLogEventHandler;

			void OnReceivedInvalidPrimaryMessageEventHandler(SECSMessageValidationError reason, SECSMessage* message);
			void OnReceivedInvalidSecondaryMessageEventHandler(SECSMessageValidationError reason, SECSMessage* primaryMessage, SECSMessage* secondaryMessage);
			void OnSentControlMessageEventHandler(SECSMessage* message);
			void OnSentSECSMessageEventHandler(SECSMessage* message);
			void OnReceivedControlMessageEventHandler(SECSMessage* message);
			void OnReceivedPrimaryMessageEventHandler(SECSMessage* message);
			void OnReceivedSecondaryMessageEventHandler(SECSMessage* primaryMessage, SECSMessage* secondaryMessage);
			void OnReceivedUnknownMessageEventHandler(SECSMessage* message);
			void OnTimeoutEventHandler(SECSTimeoutType timeoutType);
			void OnT3TimeoutEventHandler(SECSMessage* message);

			void OnSECSConnectedEventHandler(tstring& ipAddress, int portNo);
			void OnSECSDisconnectedEventHandler(tstring& ipAddress, int portNo);
			void OnSECSSelectedEventHandler(tstring& ipAddress, int portNo);
			void OnSECSDeselectedEventHandler(tstring& ipAddress, int portNo);
			void OnSECS1WriteLogEventHandler(SECSLogLevel logLevel, tstring& logText);
			void OnSECS2WriteLogEventHandler(SECSLogLevel logLevel, tstring& logText);
		};

	}
}
#endif
