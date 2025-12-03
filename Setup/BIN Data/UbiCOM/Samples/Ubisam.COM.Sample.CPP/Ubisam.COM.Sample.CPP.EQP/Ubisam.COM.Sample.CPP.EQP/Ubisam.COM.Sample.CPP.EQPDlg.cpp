
// Ubisam.COM.Sample.CPP.EQPDlg.cpp : 구현 파일
//

#include "stdafx.h"
#include "Ubisam.COM.Sample.CPP.EQP.h"
#include "Ubisam.COM.Sample.CPP.EQPDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CUbisamCOMSampleCPPEQPDlg 대화 상자




CUbisamCOMSampleCPPEQPDlg::CUbisamCOMSampleCPPEQPDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CUbisamCOMSampleCPPEQPDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	this->m_uLastReceivedS1F1SystemByte = 0;
}

void CUbisamCOMSampleCPPEQPDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LOG, m_txtLog);
	DDX_Control(pDX, IDC_DEVICE_ID, m_txtDeviceID);
	DDX_Control(pDX, IDC_DEVICE_TYPE_EQP, m_radEQP);
	DDX_Control(pDX, IDC_DEVICE_TYPE_HOST, m_radHost);
	DDX_Control(pDX, IDC_DRIVER_NAME, m_txtDriverNameConfig);
	DDX_Control(pDX, IDC_LOG_PATH, m_txtLogPath);
	DDX_Control(pDX, IDC_BROWSE_LOG_PATH, m_btnBrowseLogPath);
	DDX_Control(pDX, IDC_UMD_FILE, m_txtUMDFile);
	DDX_Control(pDX, IDC_BROWSE_UMD_FILE, m_btnBrowseUMDFile);
	DDX_Control(pDX, IDC_HSMS_PASSIVE, m_radPassive);
	DDX_Control(pDX, IDC_HSMS_ACTIVE, m_radActive);
	DDX_Control(pDX, IDC_IP_ADDRESS, m_txtIPAddress);
	DDX_Control(pDX, IDC_PORT_NO, m_txtPortNo);
	DDX_Control(pDX, IDC_INIT_CONFIG, m_btnInitConfig);
	DDX_Control(pDX, IDC_UCF_FILE, m_txtUCFFile);
	DDX_Control(pDX, IDC_BROWSE_UCF_FILE, m_btnBrowseUCFFile);
	DDX_Control(pDX, IDC_DRIVER_NAME_UCF, m_txtDriverNameUCF);
	DDX_Control(pDX, IDC_INIT_UCF_FILE, m_btnInitUCF);
}

BEGIN_MESSAGE_MAP(CUbisamCOMSampleCPPEQPDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_EXIT, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedExit)
	ON_BN_CLICKED(IDC_CLEAR_LOG, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedClearLog)
	ON_BN_CLICKED(IDC_INIT_CONFIG, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedInitConfig)
	ON_BN_CLICKED(IDC_INIT_UCF_FILE, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedInitUcfFile)
	ON_BN_CLICKED(IDC_CONNECTION_OPEN, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedConnectionOpen)
	ON_BN_CLICKED(IDC_CONNECTION_CLOSE, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedConnectionClose)
	ON_BN_CLICKED(IDC_SEND_S1F1, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedSendS1f1)
	ON_BN_CLICKED(IDC_REPLY_S1F2, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedReplyS1f2)
	ON_BN_CLICKED(IDC_BROWSE_LOG_PATH, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedBrowseLogPath)
	ON_BN_CLICKED(IDC_BROWSE_UMD_FILE, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedBrowseUmdFile)
	ON_BN_CLICKED(IDC_BROWSE_UCF_FILE, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedBrowseUcfFile)
	ON_BN_CLICKED(IDC_SEND_S6F11, &CUbisamCOMSampleCPPEQPDlg::OnBnClickedSendS6f11)
END_MESSAGE_MAP()


// CUbisamCOMSampleCPPEQPDlg 메시지 처리기

BOOL CUbisamCOMSampleCPPEQPDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 이 대화 상자의 아이콘을 설정합니다. 응용 프로그램의 주 창이 대화 상자가 아닐 경우에는
	//  프레임워크가 이 작업을 자동으로 수행합니다.
	SetIcon(m_hIcon, TRUE);			// 큰 아이콘을 설정합니다.
	SetIcon(m_hIcon, FALSE);		// 작은 아이콘을 설정합니다.

	// TODO: 여기에 추가 초기화 작업을 추가합니다.

	this->m_pDriver = new UbiCom::CPP::HSMSDriver;

	this->m_pDriver->SubscribeReceivedInvalidPrimaryMessageEventHandler(this);
	this->m_pDriver->SubscribeReceivedInvalidSecondaryMessageEventHandler(this);
	this->m_pDriver->SubscribeSentControlMessageEventHandler(this);
	this->m_pDriver->SubscribeSentSECSMessageEventHandler(this);
	this->m_pDriver->SubscribeReceivedControlMessageEventHandler(this);
	this->m_pDriver->SubscribeReceivedPrimaryMessageEventHandler(this);
	this->m_pDriver->SubscribeReceivedSecondaryMessageEventHandler(this);
	this->m_pDriver->SubscribeReceivedUnknownMessageEventHandler(this);
	this->m_pDriver->SubscribeTimeoutEventHandler(this);
	this->m_pDriver->SubscribeT3TimeoutEventHandler(this);

	this->m_pDriver->SubscribeSECSConnectedEventHandler(this);
	this->m_pDriver->SubscribeSECSDisconnectedEventHandler(this);
	this->m_pDriver->SubscribeSECSSelectedEventHandler(this);
	this->m_pDriver->SubscribeSECSDeselectedEventHandler(this);
	this->m_pDriver->SubscribeSECS1WriteLogEventHandler(this);
	this->m_pDriver->SubscribeSECS2WriteLogEventHandler(this);


	this->m_txtDeviceID.SetWindowText(_T("1"));
	this->m_radEQP.SetCheck(1);
	this->m_txtDriverNameConfig.SetWindowText(_T("CPP_TEST_EQP"));
	this->m_strLogPath = "D:\\Log";
	this->m_txtLogPath.SetWindowText(m_strLogPath);
	m_strUMDFilePath = "..\\..\\Sample.umd";
	ChangeUMDFilePath(m_strUMDFilePath);
	this->m_radPassive.SetCheck(1);
	this->m_txtIPAddress.SetWindowText(_T("127.0.0.1"));
	this->m_txtPortNo.SetWindowText(_T("7000"));

	return TRUE;  // 포커스를 컨트롤에 설정하지 않으면 TRUE를 반환합니다.
}

// 대화 상자에 최소화 단추를 추가할 경우 아이콘을 그리려면
//  아래 코드가 필요합니다. 문서/뷰 모델을 사용하는 MFC 응용 프로그램의 경우에는
//  프레임워크에서 이 작업을 자동으로 수행합니다.

void CUbisamCOMSampleCPPEQPDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 그리기를 위한 디바이스 컨텍스트입니다.

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 클라이언트 사각형에서 아이콘을 가운데에 맞춥니다.
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 아이콘을 그립니다.
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// 사용자가 최소화된 창을 끄는 동안에 커서가 표시되도록 시스템에서
//  이 함수를 호출합니다.
HCURSOR CUbisamCOMSampleCPPEQPDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CUbisamCOMSampleCPPEQPDlg::OnReceivedInvalidPrimaryMessageEventHandler(UbiCom::CPP::SECSMessageValidationError reason, UbiCom::CPP::SECSMessage* message)
{
	CString text;

	text.Append(_T("OnReceivedInvalidPrimaryMessageEventHandler."));
	text.AppendFormat(_T(" Reason=%s"), ConvertToCString(reason));

	if (message != nullptr)
	{
		text.AppendFormat(_T(" PrimaryMessage=[Stream=%d, Function=%d], "), message->Stream, message->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnReceivedInvalidSecondaryMessageEventHandler(UbiCom::CPP::SECSMessageValidationError reason, UbiCom::CPP::SECSMessage* primaryMessage, UbiCom::CPP::SECSMessage* secondaryMessage)
{
	CString text;

	text.Append(_T("OnReceivedInvalidPrimaryMessageEventHandler."));
	text.AppendFormat(_T(" Reason=%s"), ConvertToCString(reason));

	if (primaryMessage != nullptr)
	{
		text.AppendFormat(_T(", PrimaryMessage=[Stream=%d, Function=%d], "), primaryMessage->Stream, primaryMessage->Function);
	}

	if (secondaryMessage != nullptr)
	{
		text.AppendFormat(_T(", SrimaryMessage=[Stream=%d, Function=%d], "), secondaryMessage->Stream, secondaryMessage->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnSentControlMessageEventHandler(UbiCom::CPP::SECSMessage* message)
{
	CString text;

	text.Append(_T("OnSentControlMessageEventHandler."));
	
	if (message != nullptr)
	{
		text.AppendFormat(_T(" Message=[Stream=%d, Function=%d], "), message->Stream, message->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnSentSECSMessageEventHandler(UbiCom::CPP::SECSMessage* message)
{
	CString text;

	text.Append(_T("OnSentSECSMessageEventHandler."));
	
	if (message != nullptr)
	{
		text.AppendFormat(_T(" Message=[Stream=%d, Function=%d], "), message->Stream, message->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnReceivedControlMessageEventHandler(UbiCom::CPP::SECSMessage* message)
{
	CString text;

	text.Append(_T("OnReceivedControlMessageEventHandler."));
	
	if (message != nullptr)
	{
		text.AppendFormat(_T(" Message=[Stream=%d, Function=%d], "), message->Stream, message->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnReceivedPrimaryMessageEventHandler(UbiCom::CPP::SECSMessage* primaryMessage)
{
	CString text;

	text.Append(_T("OnReceivedPrimaryMessageEventHandler."));

	if (primaryMessage != nullptr)
	{
		text.AppendFormat(_T(" PrimaryMessage=[Stream=%d, Function=%d], "), primaryMessage->Stream, primaryMessage->Function);
	}

	AddLog(text);

	if (primaryMessage->AutoReply == false && primaryMessage->WaitBit == true)
	{
		if (primaryMessage->Stream == 1 && primaryMessage->Function == 1)
		{
			this->m_uLastReceivedS1F1SystemByte = primaryMessage->SystemBytes;
		}
		else if (primaryMessage->Stream == 2 && primaryMessage->Function == 41)
		{
			AnalyzeS2F41(primaryMessage);
		}
	}
}

void CUbisamCOMSampleCPPEQPDlg::OnReceivedSecondaryMessageEventHandler(UbiCom::CPP::SECSMessage* primaryMessage, UbiCom::CPP::SECSMessage* secondaryMessage)
{
	CString text;
	text.Append(_T("OnReceivedSecondaryMessageEventHandler."));

	if(primaryMessage != nullptr)
	{
		text.AppendFormat(_T(" PrimaryMessage=[Stream=%d, Function=%d], "), primaryMessage->Stream, primaryMessage->Function);
	}

	if(secondaryMessage != nullptr)
	{
		text.AppendFormat(_T(" SecondaryMessage=[Stream=%d, Function=%d], "), secondaryMessage->Stream, secondaryMessage->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnReceivedUnknownMessageEventHandler(UbiCom::CPP::SECSMessage* message)
{
	CString text;
	text.Append(_T("OnReceivedUnknownMessageEventHandler."));

	if(message != nullptr)
	{
		text.AppendFormat(_T(" Message=[Stream=%d, Function=%d], "), message->Stream, message->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnTimeoutEventHandler(UbiCom::CPP::SECSTimeoutType timeoutType)
{
	CString text;
	text.Append(_T("OnTimeoutEventHandler."));

	text.AppendFormat(_T(" %s"), ConvertToCString(timeoutType));

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnT3TimeoutEventHandler(UbiCom::CPP::SECSMessage* message)
{
	CString text;
	text.Append(_T("OnT3TimeoutEventHandler."));

	if(message != nullptr)
	{
		text.AppendFormat(_T(" Message=[Stream=%d, Function=%d], "), message->Stream, message->Function);
	}

	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnSECSConnectedEventHandler(tstring& ipAddress, int portNo)
{
	CString text;
	text.Format(_T("SECS Connected. IP=%s, PortNo=%d"), ConvertToCString(ipAddress), portNo);
	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnSECSDisconnectedEventHandler(tstring& ipAddress, int portNo)
{
	CString text;
	text.Format(_T("SECS Disconnected. IP=%s, PortNo=%d"), ConvertToCString(ipAddress), portNo);
	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnSECSSelectedEventHandler(tstring& ipAddress, int portNo)
{
	CString text;
	text.Format(_T("SECS Selected. IP=%s, PortNo=%d"), ConvertToCString(ipAddress), portNo);
	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnSECSDeselectedEventHandler(tstring& ipAddress, int portNo)
{
	CString text;
	text.Format(_T("SECS Deselected. IP=%s, PortNo=%d"), ConvertToCString(ipAddress), portNo);
	AddLog(text);
}

void CUbisamCOMSampleCPPEQPDlg::OnSECS1WriteLogEventHandler(UbiCom::CPP::SECSLogLevel logLevel, tstring& logText)
{
	AddLog(ConvertToCString(logText), false);
}

void CUbisamCOMSampleCPPEQPDlg::OnSECS2WriteLogEventHandler(UbiCom::CPP::SECSLogLevel logLevel, tstring& logText)
{
	AddLog(ConvertToCString(logText), false);
}

CString CUbisamCOMSampleCPPEQPDlg::ConvertToCString(UbiCom::CPP::SECSLogLevel logLevel)
{
	CString result;

	switch (logLevel)
	{
	case UbiCom::CPP::SECSLogLevel_Error:
		result = _T("LogLevel=Error");
		break;
	case UbiCom::CPP::SECSLogLevel_HSMS:
		result = _T("LogLevel=HSMS");
		break;
	case UbiCom::CPP::SECSLogLevel_Information:
		result = _T("LogLevel=Information");
		break;
	case UbiCom::CPP::SECSLogLevel_Receive:
		result = _T("LogLevel=Receive");
		break;
	case UbiCom::CPP::SECSLogLevel_Send:
		result = _T("LogLevel=Send");
		break;
	case UbiCom::CPP::SECSLogLevel_Warning:
		result = _T("LogLevel=Warning");
		break;
	case UbiCom::CPP::SECSLogLevel_UnknownLevel:
	default:
		result = _T("LogLevel=Unknown");
		break;
	}

	return result;
}

CString CUbisamCOMSampleCPPEQPDlg::ConvertToCString(tstring& str)
{
	CString result;

	result = str.c_str();

	return result;
}

CString CUbisamCOMSampleCPPEQPDlg::ConvertToCString(UbiCom::CPP::SECSTimeoutType timeoutType)
{
	CString result;

	switch(timeoutType)
	{
		case UbiCom::CPP::SECSTimeoutType_Linktest:
			result.Format(_T("SECSTimeoutType=Linktest"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T1:
			result.Format(_T("SECSTimeoutType=T1"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T2:
			result.Format(_T("SECSTimeoutType=T2"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T3:
			result.Format(_T("SECSTimeoutType=T3"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T4:
			result.Format(_T("SECSTimeoutType=T4"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T5:
			result.Format(_T("SECSTimeoutType=T5"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T6:
			result.Format(_T("SECSTimeoutType=T6"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T7:
			result.Format(_T("SECSTimeoutType=T7"));
			break;
		case UbiCom::CPP::SECSTimeoutType_T8:
			result.Format(_T("SECSTimeoutType=T8"));
			break;
		case UbiCom::CPP::SECSTimeoutType_UnknownTimeout:
		default:
			result.Format(_T("SECSTimeoutType=Unknown"));
			break;
	}

	return result;
}

CString CUbisamCOMSampleCPPEQPDlg::ConvertToCString(UbiCom::CPP::SECSMessageValidationError error)
{
	CString result;

	switch (error)
	{
	case UbiCom::CPP::SECSMessageValidationError_DataToLong:
		result = "DataToLong";
		break;
	case UbiCom::CPP::SECSMessageValidationError_IllegalDataFormat:
		result = "IllegalDataFormat";
		break;
	case UbiCom::CPP::SECSMessageValidationError_T3Timeout:
		result = "T3Timeout";
		break;
	case UbiCom::CPP::SECSMessageValidationError_UnrecognizedDeviceID:
		result = "UnrecognizedDeviceID";
		break;
	case UbiCom::CPP::SECSMessageValidationError_UnrecognizedFunction:
		result = "UnrecognizedFunction";
		break;
	case UbiCom::CPP::SECSMessageValidationError_UnrecognizedSteam:
		result = "UnrecognizedSteam";
		break;
	case UbiCom::CPP::SECSMessageValidationError_Ok:
	default:
		result = "Ok";
		break;
	}

	return result;
}

CString CUbisamCOMSampleCPPEQPDlg::ConvertToCString(UbiCom::CPP::SECSDriverError error)
{
	CString result;

	switch (error)
	{
	case UbiCom::CPP::SECSDriverError_AlreadyConnected:
		result = _T("AlreadyConnected");
		break;
	case UbiCom::CPP::SECSDriverError_FileLoadFailed:
		result = _T("FileLoadFailed");
		break;
	case UbiCom::CPP::SECSDriverError_FileSaveFailed:
		result = _T("FileSaveFailed");
		break;
	case UbiCom::CPP::SECSDriverError_InvalidConfiguration:
		result = _T("InvalidConfiguration");
		break;
	case UbiCom::CPP::SECSDriverError_NotExistFile:
		result = _T("NotExistFile");
		break;
	case UbiCom::CPP::SECSDriverError_NotExistDriverName:
		result = _T("NotExistDriverName");
		break;
	case UbiCom::CPP::SECSDriverError_Ok:
		result = _T("Ok");
		break;
	case UbiCom::CPP::SECSDriverError_SocketException:
		result = _T("SocketException");
		break;
	case UbiCom::CPP::SECSDriverError_TrialVersion:
		result = _T("TrialVersion");
		break;
	case UbiCom::CPP::SECSDriverError_DriverIsNull:
		result = _T("DriverIsNull");
		break;
	case UbiCom::CPP::SECSDriverError_DriverUnknown:
	default:
		result = _T("Unknown");
		break;
	}

	return result;
}

CString CUbisamCOMSampleCPPEQPDlg::ConvertToCString(UbiCom::CPP::SECSMessageError error)
{
	CString result;

	switch (error)
	{
	case UbiCom::CPP::SECSMessageError_DataIsNull:
		result = _T("DataIsNull");
		break;
	case UbiCom::CPP::SECSMessageError_DuplicateSystemBytes:
		result = _T("DuplicateSystemBytes");
		break;
	case UbiCom::CPP::SECSMessageError_InvalidLength:
		result = _T("InvalidLength");
		break;
	case UbiCom::CPP::SECSMessageError_MessageQueueOverflow:
		result = _T("MessageQueueOverflow");
		break;
	case UbiCom::CPP::SECSMessageError_NoConnected:
		result = _T("NoConnected");
		break;
	case UbiCom::CPP::SECSMessageError_NoInitialize:
		result = _T("NoInitialize");
		break;
	case UbiCom::CPP::SECSMessageError_NotSelected:
		result = _T("NotSelected");
		break;
	case UbiCom::CPP::SECSMessageError_Ok:
		result = _T("Ok");
		break;
	case UbiCom::CPP::SECSMessageError_SocketIsNull:
		result = _T("SocketIsNull");
		break;
	case UbiCom::CPP::SECSMessageError_NotExistPrimaryMessage:
		result = _T("NotExistPrimaryMessage");
		break;
	case UbiCom::CPP::SECSMessageError_MessageTargetIsNull:
		result = _T("MessageTargetIsNull");
		break;
	case UbiCom::CPP::SECSMessageError_MessageUnknown:
	default:
		result = _T("Unknown");
		break;
	}

	return result;
}

void CUbisamCOMSampleCPPEQPDlg::AddLog(CString& message, bool withoutTime)
{
	CString text;
	SYSTEMTIME st;
	CString strAll;

	if(withoutTime == true)
	{
		GetLocalTime(&st);
		text.Format(_T("[%04d-%02d-%02d %02d:%02d:%02d.%03d] %s"), st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds, message);
	}
	else
	{
		text.Format(message);
	}

	m_txtLog.GetWindowText(strAll);

	strAll += "\r\n";
	strAll += text;

	m_txtLog.SetWindowText(strAll);

	m_txtLog.LineScroll(m_txtLog.GetLineCount());
}

CString CUbisamCOMSampleCPPEQPDlg::GetContainingFolder(CString &filePath)
{
	CString result;

	result = filePath;

	PathRemoveFileSpec(result.GetBuffer(0));
	result.ReleaseBuffer(-1);

	return result;
}

void CUbisamCOMSampleCPPEQPDlg::ChangeUMDFilePath(CString& filePath)
{
	TCHAR * szFileName;
	TCHAR * szFileExt;
	CString strFileNameWithExt;
	CString strTitle;
    
	szFileName = nullptr;
	szFileExt = nullptr;

	if(filePath != "")
	{
		m_strUMDFilePath = filePath;
		szFileName = new TCHAR[512];
		szFileExt = new TCHAR[128];

		_tsplitpath_s(filePath, nullptr, 0, nullptr, 0, szFileName, 512, szFileExt, 128);
		strFileNameWithExt.Format(_T("%s%s"), szFileName, szFileExt);
		m_txtUMDFile.SetWindowText(strFileNameWithExt);

		strTitle.Format(_T("Ubisam.COM.Sample.CPP.EQP - %s"), filePath);
		SetWindowText(strTitle);
	}
	else
	{
		SetWindowText(_T("Ubisam.COM.Sample.CPP.EQP"));
	}

	if(szFileName != nullptr)
	{
		delete [] szFileName;
	}
	if(szFileExt != nullptr)
	{
		delete [] szFileExt;
	}
}

void CUbisamCOMSampleCPPEQPDlg::ChangeUCFFilePath(CString& filePath)
{
	TCHAR * szFileName;
	TCHAR * szFileExt;
	CString strFileNameWithExt;
    
	szFileName = nullptr;
	szFileExt = nullptr;

	if(filePath != "")
	{
		m_strUCFFilePath = filePath;
		szFileName = new TCHAR[512];
		szFileExt = new TCHAR[128];

		_tsplitpath_s(filePath, nullptr, 0, nullptr, 0, szFileName, 512, szFileExt, 128);
		strFileNameWithExt.Format(_T("%s%s"), szFileName, szFileExt);
		m_txtUCFFile.SetWindowText(strFileNameWithExt);
	}

	if(szFileName != nullptr)
	{
		delete [] szFileName;
	}
	if(szFileExt != nullptr)
	{
		delete [] szFileExt;
	}
}

void CUbisamCOMSampleCPPEQPDlg::OnBnClickedInitConfig()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	UbiCom::CPP::SECSConfiguration configuration;
	UbiCom::CPP::SECSDriverError success;

	CString temp;
	UbiCom::CPP::SECSDeviceType deviceType;
	UbiCom::CPP::SECSLogMode logMode;
	UbiCom::CPP::HSMSMode hsmsMode;

	if (this->m_pDriver != nullptr)
	{
		this->m_pDriver->Close();

		m_txtDeviceID.GetWindowText(temp);

		configuration.DeviceID = _ttoi(temp);

		deviceType = UbiCom::CPP::SECSDeviceType_Equipment;

		if (m_radEQP.GetCheck() == 0)
		{
			deviceType = UbiCom::CPP::SECSDeviceType_Host;
		}

		configuration.DeviceType = deviceType;

		m_txtDriverNameConfig.GetWindowText(temp);
		configuration.SetDriverName(temp.GetString());

		logMode = UbiCom::CPP::SECSLogMode_Hour;

		configuration.LogEnabledSECS1 = logMode;
		configuration.LogEnabledSECS2 = logMode;
		configuration.LogEnabledSystem = logMode;

		configuration.SetLogPath(m_strLogPath.GetString());

		configuration.SetUMDFileName(m_strUMDFilePath.GetString());

		hsmsMode = UbiCom::CPP::HSMSMode_Passive;

		if (m_radPassive.GetCheck() == 0)
		{
			hsmsMode = UbiCom::CPP::HSMSMode_Active;
		}

		configuration.HSMSMode = hsmsMode;

		m_txtIPAddress.GetWindowText(temp);
		configuration.SetIPAddress(temp.GetString());

		m_txtPortNo.GetWindowText(temp);
		configuration.PortNo = _ttoi(temp);

		success = m_pDriver->Initialize(&configuration);

		if (success != UbiCom::CPP::SECSDriverError_Ok)
		{
			temp.Format(_T("Initialize Fail: Reason=%s"), ConvertToCString(success));
			AddLog(temp);
		}
	}

}

void CUbisamCOMSampleCPPEQPDlg::OnBnClickedInitUcfFile()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	UbiCom::CPP::SECSDriverError success;

	CString driverName;
	CString temp;

	if (this->m_pDriver != nullptr)
	{
		this->m_pDriver->Close();

		m_txtDriverNameUCF.GetWindowText(driverName);

		success = m_pDriver->Initialize(m_strUCFFilePath.GetString(), driverName.GetString());

		if (success != UbiCom::CPP::SECSDriverError_Ok)
		{
			temp.Format(_T("Initialize Fail: Reason=%s"), ConvertToCString(success));
			AddLog(temp);
		}
	}
}

void CUbisamCOMSampleCPPEQPDlg::OnBnClickedClearLog()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	m_txtLog.SetWindowTextW(_T(""));
}

void CUbisamCOMSampleCPPEQPDlg::OnBnClickedExit()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	if (m_pDriver != nullptr)
	{
		m_pDriver->Close();
		m_pDriver = nullptr;
	}

	DestroyWindow();
}



void CUbisamCOMSampleCPPEQPDlg::OnBnClickedConnectionOpen()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	UbiCom::CPP::SECSDriverError success;
	CString temp;

	if (m_pDriver != nullptr)
	{
		success = m_pDriver->Open();

		if (success != UbiCom::CPP::SECSDriverError_Ok)
		{
			temp.Format(_T("Initialize Fail: Reason=%s"), ConvertToCString(success));
			AddLog(temp);
		}
	}
}


void CUbisamCOMSampleCPPEQPDlg::OnBnClickedConnectionClose()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	if (m_pDriver != nullptr)
	{
		m_pDriver->Close();
	}
}


void CUbisamCOMSampleCPPEQPDlg::OnBnClickedSendS1f1()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	UbiCom::CPP::SECSMessage* message;
	UbiCom::CPP::SECSMessageError success;
	CString temp;

	if (m_pDriver != nullptr)
	{
		message = m_pDriver->GetSECSMessage(1, 1);
		/*
		// 보내는 S1F1 구조
		아이템 없음
		*/

		success = m_pDriver->SendSECSMessage(message);

		delete message;

		if (success != UbiCom::CPP::SECSMessageError_Ok)
		{
			temp.Format(_T("SendS1F1 Fail: Reason=%s"), ConvertToCString(success));
		}
		else
		{
			temp.Format(_T("SendS1F1 Success"));
		}

		AddLog(temp);
	}
}


void CUbisamCOMSampleCPPEQPDlg::OnBnClickedReplyS1f2()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	UbiCom::CPP::SECSMessageError success;
	UbiCom::CPP::SECSMessage* secondaryMessage;

	CString temp;

	if (this->m_pDriver != nullptr)
	{
		secondaryMessage = m_pDriver->GetSECSMessage(1, 2);

		if (secondaryMessage != nullptr)
		{
			/*
			// 보내는 S1F2 구조
<L,2
	<A, 6 'UbiCom' [MDLN]>
	<A, 6 'v1.1.1' [SOFTREV]>
>
			*/
			secondaryMessage->AddListChild(_T(""), 2);
			secondaryMessage->AddChild(_T("MDLN"), UbiCom::CPP::SECSItemFormat_A, _T("UbiCom"));
			secondaryMessage->AddChild(_T("SOFTREV"), UbiCom::CPP::SECSItemFormat_A, _T("v1.1.1"));

			success = this->m_pDriver->ReplySECSMessage(this->m_uLastReceivedS1F1SystemByte, secondaryMessage);

			delete secondaryMessage;

			if (success != UbiCom::CPP::SECSMessageError_Ok)
			{
				temp.Format(_T("ReplyS1F2 Fail: Reason=%s"), ConvertToCString(success));
			}
			else
			{
				temp.Format(_T("ReplyS1F2 Success"));
			}

			AddLog(temp);
		}
	}
}


void CUbisamCOMSampleCPPEQPDlg::OnBnClickedBrowseLogPath()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	BROWSEINFO bi;
	ZeroMemory(&bi,sizeof(BROWSEINFO));
	LPMALLOC pMalloc;
	LPITEMIDLIST pidl = SHBrowseForFolder(&bi);
 
	if (pidl==NULL)
		return;
 
	if(pidl != NULL)
	{
		TCHAR* path = new TCHAR[MAX_PATH];	
 
		SHGetPathFromIDList(pidl,path);
		//		MessageBox(NULL,path,TEXT("Choose"),MB_OK);
		 if (SUCCEEDED (SHGetMalloc (& pMalloc))) // pidl pointed objects should be run out of the release, before neglected
		{
			pMalloc->Free(pidl);
			pMalloc->Release();
		}

		m_strLogPath = path;
		m_txtLogPath.SetWindowText(m_strLogPath);
		UpdateData(FALSE);	
 
		delete [] path;
	}
}


void CUbisamCOMSampleCPPEQPDlg::OnBnClickedBrowseUmdFile()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	CString umdFilePath;   

	CFileDialog dlg(TRUE);
	dlg.m_ofn.lpstrInitialDir = GetContainingFolder(m_strUMDFilePath);
	dlg.m_ofn.lpstrFilter = _T("UbiCom Message Define File (*.umd)\0*.umd\0\0");

	if( dlg.DoModal() == IDOK)
	{
		umdFilePath = dlg.GetPathName();
		ChangeUMDFilePath(umdFilePath);
	}
}


void CUbisamCOMSampleCPPEQPDlg::OnBnClickedBrowseUcfFile()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	CString ucfFilePath;   

	CFileDialog dlg(TRUE);
	dlg.m_ofn.lpstrInitialDir = GetContainingFolder(m_strUCFFilePath);
	dlg.m_ofn.lpstrFilter = _T("UbiCom Config File (*.ucf)\0*.ucf\0\0");

	if( dlg.DoModal() == IDOK)
	{
		ucfFilePath = dlg.GetPathName();
		ChangeUCFFilePath(ucfFilePath);
	}
}


void CUbisamCOMSampleCPPEQPDlg::OnBnClickedSendS6f11()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	UbiCom::CPP::SECSMessage* message;
	UbiCom::CPP::SECSMessageError success;
	CString temp;

	if (m_pDriver != nullptr)
	{
		message = m_pDriver->GetSECSMessage(6, 11);

		// 보내는 S6F11 구조
		/*
<L,3
	<I4,1 '0' [DATAID]>
	<I4,1 '10010' [CEID]>
	<L,1 [RPTIDCOUNT]
		<L,2
			<I4,1 '10010' [RPTID]>
			<L,2 [VCOUNT]
				<A,5 'LOT11' [V]>
				<L,3 [V]
					<A,2 'A1'>
					<A,2 'NG'>
					<L, 2
						<A,2 'NG'>
						<U1,1 '3'>
					>
				>
			>
		>
	>
>
		*/

		message->AddListChild(_T(""), 3);
			message->AddChild(_T("DATAID"), UbiCom::CPP::SECSItemFormat_I4, 0);
			message->AddChild(_T("CEID"), UbiCom::CPP::SECSItemFormat_I4, 10010);
			message->AddListChild(_T("RPTIDCOUNT"), 1);
				message->AddListChild(_T(""), 2);
					message->AddChild(_T("RPTID"), UbiCom::CPP::SECSItemFormat_I4, 10010);
					message->AddListChild(_T("VCOUNT"), 2);
						message->AddChild(_T("V"), UbiCom::CPP::SECSItemFormat_A, _T("LOT11"));
						message->AddListChild(_T("V"), 3);
							message->AddChild(_T(""), UbiCom::CPP::SECSItemFormat_A, _T("A1"));
							message->AddChild(_T(""), UbiCom::CPP::SECSItemFormat_A, _T("NG"));
							message->AddListChild(_T(""), 2);
							message->AddChild(_T(""), UbiCom::CPP::SECSItemFormat_A, _T("NG"));
							message->AddChild(_T(""), UbiCom::CPP::SECSItemFormat_U1, (uint8_t)3);


		success = m_pDriver->SendSECSMessage(message);

		delete message;

		if (success != UbiCom::CPP::SECSMessageError_Ok)
		{
			temp.Format(_T("SendS6F11 Fail: Reason=%s"), ConvertToCString(success));
		}
		else
		{
			temp.Format(_T("SendS6F11 Success"));
		}

		AddLog(temp);
	}
}

void CUbisamCOMSampleCPPEQPDlg::AnalyzeS2F41(UbiCom::CPP::SECSMessage* message)
{
	UbiCom::CPP::SECSMessage* secondaryMessage;
	UbiCom::CPP::SECSMessageError success;
	CString temp;

	secondaryMessage = nullptr;

	if (message != nullptr)
	{
		CString log;

		UbiCom::CPP::SECSItemCollection* rootCollection;
		UbiCom::CPP::SECSItem* remoteCommandItem;
		UbiCom::CPP::SECSItemCollection* cpCountCollection;
		
		UbiCom::CPP::SECSItem* cpCountItem;
		UbiCom::CPP::SECSItemCollection* cpCollection;
		UbiCom::CPP::SECSItem* cpNameItem;
		UbiCom::CPP::SECSItem* cpValItem;

		CString rcmd;
		CString cpName;
		CString cpValue;

		/*
		// 받은 S2F41 메시지 구조
<L,2
	<A,6 'PERMIT' [RCMD]>
	<L,3 [CPCOUNT]
		<L,2
			<A,11 'CONFIRMFLAG' [CPNAME]>
			<A,1 'Y' [CPVAL]>
		>
		<L,2
			<A,7 'MODELID' [CPNAME]>
			<A,5 'MID11' [CPVAL]>
		>
		<L,2
			<A,4 'OPID' [CPNAME]>
			<A,5 '11231' [CPVAL]>
		>
	>
>
		Retriving 예제로 Log화면에 Text 출력
		*/

		if (message->Items != nullptr && message->Items->GetCount() == 1)
		{
			rootCollection = message->Items->GetItem(0)->SubItem;

			if (rootCollection != nullptr && rootCollection->GetCount() == 2)
			{
				remoteCommandItem = rootCollection->GetItem(0);
				rcmd = ConvertSECSItemValueToCString(remoteCommandItem);

				cpCountCollection = rootCollection->GetItem(1)->SubItem;

				log.AppendFormat(_T("RCMD=%s"), rcmd);

				if (cpCountCollection != nullptr)
				{
					for (int i = 0; i < cpCountCollection->GetCount(); i++)
					{
						cpCountItem = cpCountCollection->GetItem(i);

						if (cpCountItem != nullptr)
						{
							cpCollection = cpCountItem->SubItem;

							if (cpCollection != nullptr && cpCollection->GetCount() == 2)
							{
								cpNameItem = cpCollection->GetItem(0);
								cpValItem = cpCollection->GetItem(1);

								if (cpNameItem != nullptr && cpValItem != nullptr)
								{
									cpName = ConvertSECSItemValueToCString(cpNameItem);
									cpValue = ConvertSECSItemValueToCString(cpValItem);

									log.AppendFormat(_T("\r\n    CPNAME=%s CPVAL=%s"), cpName, cpValue);
								}

							}
						}
					}
				}

				AddLog(log);

			}
		}

		if (this->m_pDriver != nullptr)
		{
			secondaryMessage = this->m_pDriver->GetSECSMessage(2, 42);

			if (secondaryMessage != nullptr)
			{
				/*
				// 보내는 S4F42 구조
	<L,2
		<B,1 '0' [HCACK]>
		<L,0>
	>
				*/
				secondaryMessage->AddListChild(_T(""), 2);
				secondaryMessage->AddChild(_T("HCACK"), UbiCom::CPP::SECSItemFormat_B, (uint8_t)0);
				secondaryMessage->AddListChild(_T(""), 0);

				success = m_pDriver->ReplySECSMessage(message, secondaryMessage);

				if (success != UbiCom::CPP::SECSMessageError_Ok)
				{
					temp.Format(_T("ReplyS2F42 Fail: Reason=%s"), ConvertToCString(success));
				}
				else
				{
					temp.Format(_T("ReplyS2F42 Success"));
				}

				AddLog(temp);
			}
		}
	}
}

CString CUbisamCOMSampleCPPEQPDlg::ConvertSECSItemValueToCString(UbiCom::CPP::SECSItem* item)
{
	CString result(_T(""));

	if (item != nullptr)
	{
		switch (item->Format)
		{
		case UbiCom::CPP::SECSItemFormat_A:
		case UbiCom::CPP::SECSItemFormat_J:
			result = item->GetValueString().c_str();
			break;
		case UbiCom::CPP::SECSItemFormat_Boolean:
			if (item->Length == 1)
			{
				if (item->GetValueBool() == true)
				{
					result = _T("True");
				}
				else
				{
					result = _T("False");
				}
			}
			else
			{
				bool* src = item->GetValueBoolArray();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						if (src[i] == true)
						{
							result.Append(_T("True "));
						}
						else
						{
							result.Append(_T("False "));
						}
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_B:
			if (item->Length == 1)
			{
				result.Format(_T("%d"), item->GetValueB());
			}
			else
			{
				uint8_t* src = item->GetValueBArray();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%d "), src[i]);
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_U1:
			if (item->Length == 1)
			{
				result.Format(_T("%d"), item->GetValueU1());
			}
			else
			{
				uint8_t* src = item->GetValueU1Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%d "), src[i]);
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_U2:
			if (item->Length == 1)
			{
				result.Format(_T("%d"), item->GetValueU2());
			}
			else
			{
				uint16_t* src = item->GetValueU2Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%d "), src[i]);
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_U4:
			if (item->Length == 1)
			{
				result.Format(_T("%lu"), item->GetValueU4());
			}
			else
			{
				uint32_t* src = item->GetValueU4Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%lu "), src[i]);
					}
				}
			}

			break;
		case UbiCom::CPP::SECSItemFormat_U8:
			if (item->Length == 1)
			{
				result.Format(_T("%llu"), item->GetValueU8());
			}
			else
			{
				uint64_t* src = item->GetValueU8Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%llu "), src[i]);
					}
				}
			}

			break;
		case UbiCom::CPP::SECSItemFormat_I1:
			if (item->Length == 1)
			{
				result.Format(_T("%d"), item->GetValueI1());
			}
			else
			{
				int8_t* src = item->GetValueI1Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%d "), src[i]);
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_I2:
			if (item->Length == 1)
			{
				result.Format(_T("%d"), item->GetValueI2());
			}
			else
			{
				int16_t* src = item->GetValueI2Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%d "), src[i]);
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_I4:
			if (item->Length == 1)
			{
				result.Format(_T("%l"), item->GetValueI4());
			}
			else
			{
				int32_t* src = item->GetValueI4Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%l "), src[i]);
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_I8:
			if (item->Length == 1)
			{
				result.Format(_T("%ll"), item->GetValueI8());
			}
			else
			{
				int64_t* src = item->GetValueI8Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%ll "), src[i]);
					}
				}
			}

			break;
		case UbiCom::CPP::SECSItemFormat_F4:
			if (item->Length == 1)
			{
				result.Format(_T("%f"), item->GetValueF4());
			}
			else
			{
				float* src = item->GetValueF4Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%f "), src[i]);
					}
				}
			}
			break;
		case UbiCom::CPP::SECSItemFormat_F8:
			if (item->Length == 1)
			{
				result.Format(_T("%f"), item->GetValueF8());
			}
			else
			{
				double* src = item->GetValueF8Array();

				if (item->Length > 0 && src != nullptr)
				{
					for (int i = 0; i < item->Length; i++)
					{
						result.AppendFormat(_T("%f "), src[i]);
					}
				}
			}
			break;
		}
		return result;
	}
}