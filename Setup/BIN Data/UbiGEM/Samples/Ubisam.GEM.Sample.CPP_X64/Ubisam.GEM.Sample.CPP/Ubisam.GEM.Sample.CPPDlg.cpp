
// Ubisam.GEM.Sample.CPPDlg.cpp: 구현 파일
//

#include "pch.h"
#include "framework.h"
#include "Ubisam.GEM.Sample.CPP.h"
#include "Ubisam.GEM.Sample.CPPDlg.h"
#include "afxdialogex.h"
#include "DriverHelper.h"
#include <sstream>

#ifndef DEBUG
#pragma comment(lib, "UbiGEM.CLI.lib")
#else
#pragma comment(lib, "UbiGEM.CLId.lib")
#endif

#include <algorithm>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CUbisamGEMSampleCPPDlg 대화 상자

CUbisamGEMSampleCPPDlg::CUbisamGEMSampleCPPDlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_UBISAMGEMSAMPLECPP_DIALOG, pParent)
	, m_iAck(0)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_pWrapper = nullptr;
	srand((unsigned int)(time(NULL)));
}

CUbisamGEMSampleCPPDlg::~CUbisamGEMSampleCPPDlg()
{
	if (m_pWrapper != nullptr)
	{
		delete m_pWrapper;
		m_pWrapper = nullptr;
	}

	_CrtSetBreakAlloc(1633);
}

void CUbisamGEMSampleCPPDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDT_LOG, m_edtLog);
	DDX_Control(pDX, IDC_CBO_EC, m_cboEcv);
	DDX_Control(pDX, IDC_CBO_V, m_cboV);
	DDX_Control(pDX, IDC_CBO_CE, m_cboCe);
	DDX_Control(pDX, IDC_CBO_USER_MESSAGE, m_cboUserMessage);
	DDX_Control(pDX, IDC_EDT_ECV, m_edtECV);
	DDX_Control(pDX, IDC_EDT_V, m_edtV);
	DDX_Control(pDX, IDC_EDT_PROCESS_STATE, m_edtProcessState);
	DDX_Control(pDX, IDC_EDT_ALID, m_edtAlarmId);
	DDX_Control(pDX, IDC_EDT_TID, m_edtTId);
	DDX_Control(pDX, IDC_EDT_TERMINAL_MESSAGE, m_edtTerminalMessage);
	DDX_Control(pDX, IDC_EDT_PPID, m_edtPpid);
	DDX_Control(pDX, IDC_EDT_FMT_PPID, m_edtFmtPpid);
	DDX_Control(pDX, IDC_EDT_ABS, m_edtAbs);
	DDX_Control(pDX, IDC_EDT_ACK, m_edtAck);
}

BEGIN_MESSAGE_MAP(CUbisamGEMSampleCPPDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_COMMAND(ID_MNU_OPEN_UGC, &CUbisamGEMSampleCPPDlg::OnMnuOpenUgc)
	ON_COMMAND(ID_MNU_EXIT, &CUbisamGEMSampleCPPDlg::OnMnuExit)
	ON_COMMAND(ID_MNU_INITILAIZE, &CUbisamGEMSampleCPPDlg::OnMnuInitilaize)
	ON_COMMAND(ID_MNU_START, &CUbisamGEMSampleCPPDlg::OnMnuStart)
	ON_COMMAND(ID_MNU_STOP, &CUbisamGEMSampleCPPDlg::OnMnuStop)
	ON_BN_CLICKED(IDC_BTN_OFFLINE, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnOffline)
	ON_BN_CLICKED(IDC_BTN_ONLINE_LOCAL, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnOnlineLocal)
	ON_BN_CLICKED(IDC_BTN_ONLINE_REMOTE, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnOnlineRemote)
	ON_BN_CLICKED(IDC_BTN_EC_SET, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnEcSet)
	ON_BN_CLICKED(IDC_BTN_EC_LIST_SET, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnEcListSet)
	ON_BN_CLICKED(IDC_BTN_V_SET, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnVSet)
	ON_BN_CLICKED(IDC_BTN_V_LIST_SET, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnVListSet)
	ON_BN_CLICKED(IDC_BTN_REPORT_CE1, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnReportCe1)
	ON_BN_CLICKED(IDC_BTN_REPORT_CE2, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnReportCe2)
	ON_BN_CLICKED(IDC_BTN_PROCESS_STATE_CHANGE, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnProcessStateChange)
	ON_BN_CLICKED(IDC_BTN_ALARM_SET, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnAlarmSet)
	ON_BN_CLICKED(IDC_BTN_ALARM_CLEAR, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnAlarmClear)
	ON_BN_CLICKED(IDC_BTN_TERMINAL_MESSAGE_SEND, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnTerminalMessageSend)
	ON_BN_CLICKED(IDC_BTN_PP_REQUEST, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpRequest)
	ON_BN_CLICKED(IDC_BTN_PP_SEND, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpSend)
	ON_BN_CLICKED(IDC_BTN_PP_LOAD_INQUIRE, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpLoadInquire)
	ON_BN_CLICKED(IDC_BTN_PP_CHANGED, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpChanged)
	ON_BN_CLICKED(IDC_BTN_FMT_PP_REQUEST, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpRequest)
	ON_BN_CLICKED(IDC_BTN_FMT_PP_CHANGED, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpChanged)
	ON_BN_CLICKED(IDC_BTN_FMT_PP_SEND_WO_VALUE, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpSendWoValue)
	ON_BN_CLICKED(IDC_BTN_FMT_PP_SEND, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpSend)
	ON_BN_CLICKED(IDC_BTN_FMT_PP_VERIFICATION_SEND, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpVerificationSend)
	ON_BN_CLICKED(IDC_BTN_TIME_REQUEST, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnTimeRequest)
	ON_BN_CLICKED(IDC_BTN_LOOPBACK, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnLoopback)
	ON_BN_CLICKED(IDC_BTN_USER_MESSAGE_SEND, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnUserMessageSend)
	ON_BN_CLICKED(IDC_BTN_ACK_APPLY, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnAckApply)
	ON_BN_CLICKED(IDC_BTN_CLEAR_LOG, &CUbisamGEMSampleCPPDlg::OnBnClickedBtnClearLog)
	ON_CBN_SELCHANGE(IDC_CBO_EC, &CUbisamGEMSampleCPPDlg::OnCbnSelchangeCboEc)
END_MESSAGE_MAP()

//ON_BN_CLICKED(IDOK, &CUbisamGEMSampleCPPDlg::OnBnClickedOk)

// CUbisamGEMSampleCPPDlg 메시지 처리기

BOOL CUbisamGEMSampleCPPDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 이 대화 상자의 아이콘을 설정합니다.  응용 프로그램의 주 창이 대화 상자가 아닐 경우에는
	//  프레임워크가 이 작업을 자동으로 수행합니다.
	SetIcon(m_hIcon, TRUE);			// 큰 아이콘을 설정합니다.
	SetIcon(m_hIcon, FALSE);		// 작은 아이콘을 설정합니다.

	// TODO: 여기에 추가 초기화 작업을 추가합니다.

	m_pWrapper = new UbiGEMWrapper::GEMWrapper();

	m_pWrapper->SubscribeInitilaize(this);
	m_pWrapper->SubscribeGEMConnected();
	m_pWrapper->SubscribeGEMDisconnected();
	m_pWrapper->SubscribeGEMSelected();
	m_pWrapper->SubscribeGEMDeselected();
	m_pWrapper->SubscribeSECSTimeout();
	m_pWrapper->SubscribeSECST3Timeout();
	m_pWrapper->SubscribeInvalidMessageReceived();
	m_pWrapper->SubscribeReceivedUnknownMessage();
	m_pWrapper->SubscribeReceivedEstablishCommunicationsRequest();
	m_pWrapper->SubscribeReceivedRemoteCommand();
	m_pWrapper->SubscribeReceivedInvalidRemoteCommand();
	m_pWrapper->SubscribeReceivedEnhancedRemoteCommand();
	m_pWrapper->SubscribeReceivedInvalidEnhancedRemoteCommand();
	m_pWrapper->SubscribeReceivedNewECVSend();
	m_pWrapper->SubscribeReceivedLoopback();
	m_pWrapper->SubscribeReceivedTerminalMessage();
	m_pWrapper->SubscribeReceivedTerminalMultiMessage();
	m_pWrapper->SubscribeReceivedRequestOffline();
	m_pWrapper->SubscribeReceivedRequestOnline();
	m_pWrapper->SubscribeReceivedDefineReport();
	m_pWrapper->SubscribeReceivedLinkEventReport();
	m_pWrapper->SubscribeReceivedEnableDisableEventReport();
	m_pWrapper->SubscribeReceivedEnableDisableAlarmSend();
	m_pWrapper->SubscribeResponseDateTimeRequest();
	m_pWrapper->SubscribeResponseLoopback();
	m_pWrapper->SubscribeResponseEventReportAcknowledge();
	m_pWrapper->SubscribeReceivedPPLoadInquire();
	m_pWrapper->SubscribeReceivedPPSend();
	m_pWrapper->SubscribeReceivedFmtPPSend();
	m_pWrapper->SubscribeResponsePPLoadInquire();
	m_pWrapper->SubscribeResponsePPSend();
	m_pWrapper->SubscribeResponsePPRequest();
	m_pWrapper->SubscribeResponseFmtPPSend();
	m_pWrapper->SubscribeResponseFmtPPRequest();
	m_pWrapper->SubscribeResponseFmtPPVerification();
	m_pWrapper->SubscribeResponseTerminalRequest();
	m_pWrapper->SubscribeReceivedDateTimeRequest();
	m_pWrapper->SubscribeReceivedDateTimeSetRequest();
	m_pWrapper->SubscribeReceivedPPRequest();
	m_pWrapper->SubscribeReceivedFmtPPRequest();
	m_pWrapper->SubscribeReceivedDeletePPSend();
	m_pWrapper->SubscribeReceivedCurrentEPPDRequest();
	m_pWrapper->SubscribeUserPrimaryMessageReceived();
	m_pWrapper->SubscribeUserSecondaryMessageReceived();
	m_pWrapper->SubscribeCommunicationStateChanged();
	m_pWrapper->SubscribeControlStateChanged();
	m_pWrapper->SubscribeControlStateOnlineChangeFailed();
	m_pWrapper->SubscribeEquipmentProcessState();
	m_pWrapper->SubscribeSpoolStateChanged();
	m_pWrapper->SubscribeVariableUpdateRequest();
	m_pWrapper->SubscribeUserGEMMessageUpdateRequest();
	m_pWrapper->SubscribeTraceDataUpdateRequest();

	m_edtTId.SetWindowText(_T("1"));
	m_edtTerminalMessage.SetWindowText(_T("TerminalMessage"));
	m_edtFmtPpid.SetWindowText(_T("MGL19SS06MD"));
	m_edtAbs.SetWindowText(_T("0123456789"));
	m_edtPpid.SetWindowText(_T("0"));
	m_edtAlarmId.SetWindowText(_T("0"));
	m_edtProcessState.SetWindowText(_T("0"));
	m_edtAck.SetWindowText(_T("0"));

	UpdateDialogTitle();

	return TRUE;  // 포커스를 컨트롤에 설정하지 않으면 TRUE를 반환합니다.
}

// 대화 상자에 최소화 단추를 추가할 경우 아이콘을 그리려면
//  아래 코드가 필요합니다.  문서/뷰 모델을 사용하는 MFC 애플리케이션의 경우에는
//  프레임워크에서 이 작업을 자동으로 수행합니다.

void CUbisamGEMSampleCPPDlg::OnPaint()
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
HCURSOR CUbisamGEMSampleCPPDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CUbisamGEMSampleCPPDlg::OnGEMConnected(LPCTSTR strIpAddress, int nPortNo)
{
	CString strLog;

	strLog.Format(_T("OnGEMConnected : [IP=%s,Port=%d]"), strIpAddress, nPortNo);

	AddLog(strLog);

	UpdateDialogTitle();
}

void CUbisamGEMSampleCPPDlg::OnGEMDisconnected(LPCTSTR strIpAddress, int nPortNo)
{
	CString strLog;

	strLog.Format(_T("OnGEMDisconnected : [IP=%s,Port=%d]"), strIpAddress, nPortNo);

	AddLog(strLog);

	UpdateDialogTitle();
}

void CUbisamGEMSampleCPPDlg::OnGEMSelected(LPCTSTR strIpAddress, int nPortNo)
{
	CString strLog;

	strLog.Format(_T("OnGEMSelected : [IP=%s,Port=%d]"), strIpAddress, nPortNo);

	AddLog(strLog);
}

void CUbisamGEMSampleCPPDlg::OnGEMDeselected(LPCTSTR strIpAddress, int nPortNo)
{
	CString strLog;

	strLog.Format(_T("OnGEMDeselected : [IP=%s,Port=%d]"), strIpAddress, nPortNo);

	AddLog(strLog);
}

void CUbisamGEMSampleCPPDlg::OnSECSTimeout(UbiGEMWrapper::Structure::SECSTimeoutType timeoutType)
{
	CString strLog;

	strLog.Format(_T("OnSECSTimeout : [TimeoutType=%d]"), timeoutType);

	AddLog(strLog);
}

void CUbisamGEMSampleCPPDlg::OnSECST3Timeout(UbiGEMWrapper::Structure::SECSMessage* message)
{
	CString strLog;

	strLog.Format(_T("OnSECST3Timeout : [Message=%s(S%dF%d), SystemBytes=%d]"), message->Name, message->Stream, message->Function, message->SystemBytes);

	AddLog(strLog);
}

void CUbisamGEMSampleCPPDlg::OnInvalidMessageReceived(UbiGEMWrapper::Structure::GEMMessageValidationError error, UbiGEMWrapper::Structure::SECSMessage* pMessage)
{
	CString strLog;

	strLog.Format(_T("OnInvalidMessageReceived : [Stream=%d,Function=%d,Name=%s]"), pMessage->Stream, pMessage->Function, pMessage->Name);

	AddLog(strLog);
}

void CUbisamGEMSampleCPPDlg::OnReceivedUnknownMessage(UbiGEMWrapper::Structure::SECSMessage* pMessage)
{
	CString strLog;

	strLog.Format(_T("OnReceivedUnknownMessage : [Stream=%d,Function=%d]"), pMessage->Stream, pMessage->Function);

	AddLog(strLog);
}

// Host에서 S1F13(Establish Communication)이 수신될 때 발생하는 이벤트입니다.
int CUbisamGEMSampleCPPDlg::OnReceivedEstablishCommunicationsRequest(LPCTSTR strMdln, LPCTSTR strSofRev)
{
	CString strLog;

	strLog.Format(_T("OnReceivedEstablishCommunicationsRequest : [MDLN=%s,SOFTREV=%s]"), strMdln, strSofRev);

	AddLog(strLog);

	return m_iAck;
}

// Host에서 S2F41(Remote Command)가 수신될 때 발생하는 이벤트입니다.
// RemoteCommandInfo 의 아이템을 순회하는 코드입니다.
// RemoteCommandParameterResult 에 IRemoteCommandParameterResult 를 추가하여 parameter 별 ack를 구성할 수 있습니다.
void CUbisamGEMSampleCPPDlg::OnReceivedRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo)
{
	UbiGEMWrapper::Structure::RemoteCommandResult* pResult = nullptr;
	UbiGEMWrapper::Structure::CommandParameterInfo* pParamInfo = nullptr;
	CString strLog;
	CString strValue;

	pResult = m_pWrapper->CreateRemoteCommandResult(m_iAck);

	strLog.AppendFormat(_T("OnReceivedRemoteCommand : [RemoteCommand=%s]"), remoteCommandInfo->RemoteCommand);

	if (pResult->Items == nullptr && m_iAck != 0)
	{
		pResult->InitializeItems();
	}

	for (int i = 0, n = remoteCommandInfo->CommandParameter->Items->GetCount(); i < n; i++)
	{
		pParamInfo = remoteCommandInfo->CommandParameter->Items->At(i);

		if (pParamInfo != nullptr)
		{
			strValue = SECSValueAsString(pParamInfo->Format, pParamInfo->Value);
			strLog.AppendFormat(_T("\r\n:   [CPNAME=%s,Format=%s,Value=%s]"), pParamInfo->Name, SECSFormatAsString(pParamInfo->Format), strValue);

			// CP 검증 후 result에 CP별 ack 를 설정합니다.
			if (m_iAck != 0)
			{
				pResult->AddParameterResult(pParamInfo->Name, m_iAck);
			}
		}
	}

	// CP 검증 후 result 에 HCACK를 설정합니다
	AddLog(strLog);

	UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyRemoteCommandAck(remoteCommandInfo, pResult);

	AddLog(_T("ReplyRemoteCommandAck"), gemResult);

	m_pWrapper->DeleteRemoteCommandResult(pResult);
	pResult = nullptr;

}

// UbiGEM Configuration 파일에 정의되지 않은 Remote Command 가 수신될 경우 발생합니다.
void CUbisamGEMSampleCPPDlg::OnReceivedInvalidRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo)
{
	CString strLog;

	strLog.Format(_T("OnReceivedInvalidRemoteCommand : [RemoteCommand=%s]"), remoteCommandInfo->RemoteCommand);

	AddLog(strLog);
}

// Host에서 S2F49(Enhanced Remote Command)가 수신될 때 발생하는 이벤트입니다.
// EnhancedRemoteCommandInfo 의 아이템을 순회하는 코드입니다.
// RemoteCommandResult 에 RemoteCommandParameterResult 를 추가하여 parameter 별 ack를 구성할 수 있습니다.
void CUbisamGEMSampleCPPDlg::OnReceivedEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommand)
{
	UbiGEMWrapper::Structure::RemoteCommandResult* pResult = nullptr;
	UbiGEMWrapper::Structure::RemoteCommandParameterResult* pParamResult = nullptr;
	UbiGEMWrapper::Structure::RemoteCommandParameterResult* pParamSubResult = nullptr;
	UbiGEMWrapper::Structure::EnhancedCommandParameterInfo* pParamInfo = nullptr;
	UbiGEMWrapper::Structure::EnhancedCommandParameterItem* pParamItem = nullptr;
	UbiGEMWrapper::Structure::EnhancedCommandParameterItem* pParamSubItem = nullptr;
	CString strLog;
	CString strValue;

	pResult = m_pWrapper->CreateRemoteCommandResult(m_iAck);

	strLog.AppendFormat(_T("OnReceivedEnhancedRemoteCommand : [RemoteCommand=%s]"), remoteCommand->RemoteCommand);

	// 로그 출력
	if(remoteCommand->EnhancedCommandParameter != nullptr && remoteCommand->EnhancedCommandParameter->Items != nullptr)
	{
		for (int i = 0, n = remoteCommand->EnhancedCommandParameter->Items->GetCount(); i < n; i++)
		{
			pParamResult = nullptr;
			pParamInfo = remoteCommand->EnhancedCommandParameter->Items->At(i);

			if (pParamInfo->Format == UbiGEMWrapper::Structure::GEMSECSFormat_L)
			{
				strLog.AppendFormat(_T("\r\n:   [CPNAME=%s,Format=L,Count=%d]"), pParamInfo->Name, pParamInfo->Items->GetCount());

				if (m_iAck != 0)
				{
					pParamResult = m_pWrapper->CreateRemoteCommandParameterResult(pParamInfo->Name, m_iAck);
					pResult->Items->Add(pParamResult);
				}

				if (pParamInfo->Items != nullptr)
				{
					// 2차원 데이터
					for (int j = 0, m = pParamInfo->Items->GetCount(); j < m; j++)
					{
						pParamSubResult = nullptr;
						pParamItem = pParamInfo->Items->At(j);

						if (pParamResult != nullptr && m_iAck != 0)
						{
							pParamSubResult = m_pWrapper->CreateRemoteCommandParameterResult(pParamItem->Name, m_iAck);
							pParamResult->ParameterListAck->Add(pParamSubResult);
						}

						// 3차원 데이터
						if (pParamItem->Format == UbiGEMWrapper::Structure::GEMSECSFormat_L)
						{
							if (pParamItem->ChildParameterItems != nullptr)
							{
								strLog.AppendFormat(_T("\r\n:      [CPNAME=%s,Format=L,Count=%d]"), pParamItem->Name, pParamItem->ChildParameterItems->GetCount());

								for (int k = 0, l = pParamItem->ChildParameterItems->GetCount(); k < l; k++)
								{
									pParamSubItem = pParamItem->ChildParameterItems->At(k);

									if (pParamSubResult != nullptr && m_iAck != 0)
									{
										pParamSubResult->AddChildParameterResult(pParamSubItem->Name, m_iAck);
									}

									strValue = SECSValueAsString(pParamSubItem->Format, pParamSubItem->Value);
									strLog.AppendFormat(_T("\r\n:         [CPNAME=%s,Format=%s,CEPVAL=%s]"), pParamSubItem->Name, SECSFormatAsString(pParamSubItem->Format), strValue);
								}
							}
						}
						else
						{
							if (pParamResult != nullptr && m_iAck != 0)
							{
								pParamResult->AddChildParameterResult(pParamItem->Name, m_iAck);
							}

							strValue = SECSValueAsString(pParamItem->Format, pParamItem->Value);
							strLog.AppendFormat(_T("\r\n:      [CPNAME=%s,Format=%s,CEPVAL=%s]"), pParamItem->Name, SECSFormatAsString(pParamItem->Format), strValue);
						}
					}
				}
			}
			else
			{
				if (pResult != nullptr && m_iAck != 0)
				{
					pResult->AddParameterResult(pParamInfo->Name, m_iAck);
				}

				strValue = SECSValueAsString(pParamInfo->Format, pParamInfo->Value);
				strLog.AppendFormat(_T("\r\n:   [CPNAME=%s,Format=%s,CEPVAL=%s]"), pParamInfo->Name, SECSFormatAsString(pParamInfo->Format), strValue);

			}
		}
	}

	AddLog(strLog);

	UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyEnhancedRemoteCommandAck(remoteCommand, pResult);
	AddLog(_T("ReplyEnhancedRemoteCommandAck"), gemResult);

	m_pWrapper->DeleteRemoteCommandResult(pResult);
	pResult = nullptr;

}

// UbiGEM Configuration 파일에 정의되지 않은 Enhanced Remote Command 가 수신될 경우 발생합니다.
void CUbisamGEMSampleCPPDlg::OnReceivedInvalidEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo)
{
	CString strLog;

	strLog.Format(_T("OnReceivedInvalidEnhancedRemoteCommand [RemoteCommand=%s]"), remoteCommandInfo->RemoteCommand);
	AddLog(strLog);
}

// Host에서 S2F15(New ECV Send)가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedNewECVSend(unsigned int systemBytes, UbiGEMWrapper::Structure::VariableCollection* newEcInfo)
{
	CString strLog;
	UbiGEMWrapper::Structure::VariableInfo* pItem;

	strLog.Format(_T("OnReceivedNewECVSend : [Count=%d]"), newEcInfo->Items->GetCount());

	for (int i = 0, n = newEcInfo->Items->GetCount(); i < n; i++)
	{
		pItem = newEcInfo->Items->At(i);
		strLog.AppendFormat(_T("\r\n:   [Name=%s,VID=%s]"), pItem->Name, pItem->VID);
	}

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyNewEquipmentConstantSend(systemBytes, newEcInfo, m_iAck);
		AddLog(_T("ReplyNewEquipmentConstantSend"), gemResult);
	}
}

// Host에서 S2F25(Loopback)이 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedLoopback(UbiGEMWrapper::Structure::List<unsigned char>* receiveData)
{
	CString strLog;
	CString strReceiveData;

	strLog.Format(_T("OnReceivedLoopback : [Count=%d]"), receiveData->GetCount());

	for (int i = 0; i < receiveData->GetCount(); i++)
	{
		strLog.AppendFormat(_T(" %d"), receiveData->At(i));
	}

	AddLog(strLog);
}

// Host에서 S10F3(Terminal Message Single)이 수신될 때 발생하는 이벤트입니다
void CUbisamGEMSampleCPPDlg::OnReceivedTerminalMessage(unsigned int systemBytes, int tid, LPCTSTR strTerminalMessage)
{
	CString strLog;

	strLog.AppendFormat(_T("OnReceivedTerminalMessage: [TID=%d,Tmsg=%s,systemBytes=%d]"), tid, strTerminalMessage,systemBytes);

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyTerminalMessageAck(systemBytes, this->m_iAck);
		AddLog(_T("ReplyTerminalMessageAck"), gemResult);
	}

}

// Host에서 S10F5(Terminal Message Multi)가 수신될 때 발생하는 이벤트입니다
void CUbisamGEMSampleCPPDlg::OnReceivedTerminalMultiMessage(unsigned int systemBytes, int tid, UbiGEMWrapper::Structure::List<LPCTSTR>* strTerminalMessages)
{
	CString strLog;

	size_t length = strTerminalMessages->GetCount();

	strLog.AppendFormat(_T("OnReceivedTerminalMultiMessage: [TID=%d]"), tid);

	for (int i = 0; i < length; i++)
	{
		strLog.AppendFormat(_T("\r\n:   [Tmsg=%s]"), strTerminalMessages->At(i));
	}

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyTerminalMultiMessageAck(systemBytes, this->m_iAck);
		AddLog(_T("ReplyTerminalMultiMessageAck"), gemResult);
	}

} 

// Host에서 S1F15(Offline Request)가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedRequestOffline(unsigned int systemBytes)
{
	CString strLog;

	strLog.Format(_T("OnReceivedRequestOffline"));

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyRequestOfflineAck(systemBytes, m_iAck);

		AddLog(_T("ReplyRequestOfflickAck"), gemResult);
	}
}

// Host에서 S1F17(Online Request)가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedRequestOnline(unsigned int systemBytes)
{
	CString strLog;

	strLog.Format(_T("OnReceivedRequestOnline"));

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyRequestOnlineAck(systemBytes, m_iAck);

		AddLog(_T("ReplyRequestOnlineAck"), gemResult);
	}
}

// S2F33(Define Report)가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedDefineReport()
{
	CString strLog;

	strLog.Format(_T("OnReceivedDefineReport"));

	AddLog(strLog);
}

// S2F35(Link Event Report)가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedLinkEventReport()
{
	CString strLog;

	strLog.Format(_T("OnReceivedLinkEventReport"));

	AddLog(strLog);
}

// S2F37(Event Report Enable/Disable)이 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedEnableDisableEventReport()
{
	CString strLog;

	strLog.Format(_T("OnReceivedEnableDisableEventReport"));

	AddLog(strLog);
}

// S5F3(Alarm Enable/Disable Send)가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedEnableDisableAlarmSend()
{
	CString strLog;

	strLog.Format(_T("OnReceivedEnableDisableAlarmSend"));

	AddLog(strLog);
}

// S7F17(Date Time Reqeust)를 발송 후 Host에서 S7F18이 수신될 때 발생하는 이벤트입니다.
bool CUbisamGEMSampleCPPDlg::OnResponseDateTimeRequest(UbiGEMWrapper::Structure::DateTime* timeData)
{
	CString strLog;

	strLog.AppendFormat(_T("OnResponseDateTimeRequest : [%04d-%02d-%02d  %02d:%02d:%02d:%02d]"), timeData->Year,timeData->Month,timeData->Day,timeData->Hour,timeData->Minute,timeData->Second, timeData->MilliSecond / 10);

	AddLog(strLog);

	return true;
}

// S2F25(Loopback)을 발송 후 Host에서 S2F26이 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponseLoopback(UbiGEMWrapper::Structure::List<unsigned char>* receiveData, UbiGEMWrapper::Structure::List<unsigned char>* sendData)
{
	CString strLog;

	strLog.Format(_T("OnResponseLoopback : [receiveData=%d,sendData=%d]"), receiveData->GetCount(), sendData->GetCount());

	AddLog(strLog);
}

// S6F11(Event Report)의 Secondary Message(S6F12)가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponseEventReportAcknowledge(LPCTSTR ceid, int ack)
{
	CString strLog;

	strLog.Format(_T("OnResponseEventReportAcknowledge : [CEID=%s,ACKC6=%d]"), ceid, ack);

	AddLog(strLog);
}

// Host에서 S7F1(PP Load Inquire)가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedPPLoadInquire(unsigned int systemBytes, LPCTSTR ppid, int length)
{
	CString strLog;

	strLog.AppendFormat(_T("OnReceivedPPLoadInquire : [SystemByte=%d,PPID=%s,LENGTH=%d]"), systemBytes , ppid, length);

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyPPLoadInquireAck(systemBytes, m_iAck);

		AddLog(_T("ReplyPPLoadInquireAck"), gemResult);
	}
}

// Host에서 S7F3(PP Send)가 수신될 때 발생하는 이벤트입니다
void CUbisamGEMSampleCPPDlg::OnReceivedPPSend(unsigned int systemBytes, LPCTSTR ppid, UbiGEMWrapper::Structure::List<unsigned char>* ppbody)
{
	CString strLog;

	strLog.AppendFormat(_T("OnReceivedPPSend : [PPID=%s]"), ppid);
	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyPPSendAck(systemBytes, m_iAck);
		AddLog(_T("ReplyPPSendAck"), gemResult);
	}
	
}

// Host에서 S7F23(Formatted PP Send)가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedFmtPPSend(unsigned int systemBytes, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection)
{
	CString strLog;
	strLog.AppendFormat(_T("OnReceivedFmtPPSend: [PPID=%s]"), fmtPPCollection->PPID);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyFmtPPSendAck(systemBytes, m_iAck);
		AddLog(_T("ReplyFmtPPSendAck"), gemResult);
	}
}

// S7F1(PP Load Inquire)를 발송 후 Host에서 S7F2가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponsePPLoadInquire(int ppgnt, LPCTSTR ppid)
{
	CString strLog;

	strLog.AppendFormat(_T("OnResponsePPLoadInquire: [PPGNT=%d,PPID=%s]"), ppgnt, ppid);

	AddLog(strLog);
}

// S7F3(PP Send)를 발송 후 Host에서 S7F4가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponsePPSend(int ack, LPCTSTR ppid)
{
	CString strLog;

	strLog.AppendFormat(_T("OnResponsePPLoadInquire: [ACKC7=%d,PPID=%s]"), ack, ppid);

	AddLog(strLog);
}

// S7F5(PP Request)를 발송 후 Host에서 S7F6이 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponsePPRequest(LPCTSTR ppid, UbiGEMWrapper::Structure::List<unsigned char>* ppbody)
{
	CString strLog;

	byte* pArrBuf;
	size_t lCount;

	pArrBuf = nullptr;

	strLog.AppendFormat(_T("OnResponsePPRequest: [PPID=%s]"), ppid);

	if (ppbody != nullptr)
	{
		lCount = ppbody->GetCount();

		if (lCount > 0)
		{
			pArrBuf = new byte[lCount];

			for (int i = 0; i < lCount; i++)
			{
				pArrBuf[i] = ppbody->At(i);
			}

			// pArrBuf를 File로 저장 또는 다른 처리

			delete[] pArrBuf;
			pArrBuf = nullptr;
		}
	}

	AddLog(strLog);
}

// S7F25(Formatted PP Send)를 발송 후 Host에서 S7F26이 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponseFmtPPSend(int ack, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection)
{
	CString strLog;

	strLog.Format(_T("OnResponseFmtPPSend : [ACKC6=%d,PPID=%s]"), ack, fmtPPCollection->PPID);

	AddLog(strLog);
}

// S2F23(Formatted PP Request) 발송 후 Host에서 S2F24가 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponseFmtPPRequest(UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection)
{
	CString strLog;

	strLog.Format(_T("OnResponseFmtPPRequest : [PPID=%s]"), fmtPPCollection->PPID);

	AddLog(strLog);
}

// S2F27(Formatted PP Verification Send)를 발송 후 Host에서 S2F28이 수신될 때 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnResponseFmtPPVerification(UbiGEMWrapper::Structure::FmtPPVerificationCollection* fmtPPVerificationCollection)
{
	CString strLog;

	strLog.Format(_T("OnResponseFmtPPVerification : [PPID=%s]"), fmtPPVerificationCollection->PPID);

	AddLog(strLog);
}

void CUbisamGEMSampleCPPDlg::OnResponseTerminalRequest(int ack)
{
	CString strLog;

	strLog.Format(_T("OnResponseTerminalRequest : [ACKC10=%d]"), ack);

	AddLog(strLog);
}

// S2F17(Date Time Reqeust)가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedDateTimeRequest(unsigned int systemBytes, UbiGEMWrapper::Structure::DateTime* timeData)
{
	CTime time;
	CString strLog;

	if (m_pWrapper != nullptr)
	{
		time = CTime::GetCurrentTime();
		timeData->Year = time.GetYear();
		timeData->Month = time.GetMonth();
		timeData->Day = time.GetDay();
		timeData->Hour = time.GetHour();
		timeData->Minute = time.GetMinute();
		timeData->Second = time.GetSecond();

		strLog.AppendFormat(_T("OnReceivedDateTimeRequest : [TimeData=%04d-%02d-%02d  %02d:%02d:%02d]"), timeData->Year, timeData->Month, timeData->Day, timeData->Hour, timeData->Minute, timeData->Second);

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyDateTimeRequest(systemBytes, timeData);

		AddLog(_T("ReplyDateTimeRequest"), gemResult);
	}
}

// S2F31(Date Time Set Request)가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedDateTimeSetRequest(unsigned int systemBytes, UbiGEMWrapper::Structure::DateTime* timeData)
{
	CString strLog;

	if (timeData != nullptr)
	{
		strLog.AppendFormat(_T("OnReceivedDataTimeSetRequest : [TimeData=%04d-%02d-%02d  %02d:%02d:%02d:%02d]"), timeData->Year, timeData->Month, timeData->Day, timeData->Hour, timeData->Minute, timeData->Second, timeData->MilliSecond / 10);
	}
	else
	{
		strLog.AppendFormat(_T("OnReceivedDataTimeSetRequest : [TimeData is null]"));
	}

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyDateTimeSetRequest(systemBytes, this->m_iAck, timeData);
		AddLog(_T("ReplyDateTimeSetRequest"), gemResult);
	}
}

// S7F5(PP Reqeust)가 수신될 경우 발생하는 이벤트입니다
void CUbisamGEMSampleCPPDlg::OnReceivedPPRequest(unsigned int systemBytes, LPCTSTR ppid)
{
	CString strLog;
	UbiGEMWrapper::Structure::List<uint8_t>* ppBody;

	strLog.AppendFormat(_T("OnReceivedPPRequest : [PPID=%s]"), ppid);
	AddLog(strLog);

	bool result = true;

	if (m_pWrapper != nullptr)
	{
		// Recipe(Process Program)을 못찾을 경우 result 를 false 로 설정
		// result = false;
		ppBody = m_pWrapper->CreateU1List();
		MakePPBody(ppBody);

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyPPRequestAck(systemBytes, ppid, ppBody, result);

		m_pWrapper->DeleteU1List(ppBody);
		ppBody = nullptr;

		AddLog(_T("ReplyPPreqeustAck"), gemResult);
	}
}

// S7F25(Formatted PP Reqeust)가 수신될 경우 발생하는 이벤트입니다
void CUbisamGEMSampleCPPDlg::OnReceivedFmtPPRequest(unsigned int systemBytes, LPCTSTR ppid)
{
	CString strLog;
	UbiGEMWrapper::Structure::FmtPPCollection* pFmtPPCollection;
	bool result = true;

	strLog.AppendFormat(_T("OnReceivedFmtPPRequest : [PPID=%s]"), ppid);
	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		// Recipe(Process Program)을 못찾을 경우 result 를 false 로 설정
		// result = false;
		pFmtPPCollection = m_pWrapper->CreateFmtPPCollection(ppid);

		ProcessProgramParsing(false, pFmtPPCollection);

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyFmtPPRequestAck(systemBytes, ppid, pFmtPPCollection, result);

		m_pWrapper->DeleteFmtPPCollection(pFmtPPCollection);
		pFmtPPCollection = nullptr;

		AddLog(_T("ReplyFmtPPRequestAck"), gemResult);

	}
}

// S7F17(Delete PP Send)가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnReceivedDeletePPSend(unsigned int systemBytes, UbiGEMWrapper::Structure::List<LPCTSTR>* ppids)
{
	CString strLog;

	if (ppids != nullptr)
	{
		strLog.AppendFormat(_T("OnReceivedDeletePPSend : [Count=%d]"), ppids->GetCount());

		for (int i = 0; i < ppids->GetCount(); i++)
		{
			// ppid에 해당하는 Recipe(Process Program) 파일 삭제 진행
			strLog.AppendFormat(_T("%s "), ppids->At(i));
		}
	}
	else
	{
		strLog.AppendFormat(_T("OnReceivedDeletePPSend : [ppids is null]"), ppids->GetCount());
	}

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyPPDeleteAck(systemBytes, m_iAck);
		AddLog(_T("ReplyPPDeleteAck"), gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnReceivedCurrentEPPDRequest(unsigned int systemBytes)
{
	CString strLog;
	UbiGEMWrapper::Structure::List<LPCTSTR>* ppids;

	strLog.Format(_T("ReceivedCurrentEPPDRequest"));
	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		ppids = m_pWrapper->CreateStringList();

		// 현재 저장되어 있는 Recipe(Process Program) 파일의 목록을 사용해야 합니다.

		ppids->Add(_T("PPID01"));
		ppids->Add(_T("PPID02"));
		ppids->Add(_T("PPID03"));

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReplyCurrentEPPDRequestAck(systemBytes, ppids, true);

		m_pWrapper->DeleteStringList(ppids, false);
		ppids = nullptr;

		AddLog(_T("ReplyCurrentEPPDRequestAck"), gemResult);
	}

}

// 사용자 정의 Message로 등록한 Stream, Function 중 Primary 메시지가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnUserPrimaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* pPrimaryMessage)
{
	CString strLog;
	UbiGEMWrapper::Structure::SECSMessage* pSecondaryMessage;

	strLog.AppendFormat(_T("OnUserPrimaryMessageReceived : [Stream=%d,Function=%d,Name=%s]"), pPrimaryMessage->Stream, pPrimaryMessage->Function, pPrimaryMessage->Name);

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		if (pPrimaryMessage->Stream == 99 && pPrimaryMessage->Function == 1)
		{
			pSecondaryMessage = m_pWrapper->CreateUserMessage(_T("S99F2"), pPrimaryMessage->Stream, pPrimaryMessage->Function + 1, false, true);

			if (pSecondaryMessage != nullptr)
			{
				pSecondaryMessage->Body->AddSECSItem(_T("ACKC99"), UbiGEMWrapper::Structure::GEMSECSFormat_B, m_iAck);
				m_pWrapper->ReplySECSMessage(pPrimaryMessage, pSecondaryMessage);

				m_pWrapper->DeleteSECSMessage(pSecondaryMessage);
				pSecondaryMessage = nullptr;
			}
		}
	}
}

// 사용자 정의 Message로 등록한 Stream, Function 중 Secondary 메시지가 수신될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnUserSecondaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* primaryMessage, UbiGEMWrapper::Structure::SECSMessage* secondaryMessage)
{
	CString strLog;

	strLog.AppendFormat(_T("OnUserSecondaryMessageReceived"));

	if (primaryMessage == nullptr)
	{
		strLog.AppendFormat(_T("\r\n:   PrimaryMessage is null"));
	}
	else
	{
		strLog.AppendFormat(_T("\r\n:   PrimaryMessage : [Stream=%d,Function=%d,Name=%s]"), primaryMessage->Stream, primaryMessage->Function, primaryMessage->Name);
	}

	if (secondaryMessage == nullptr)
	{
		strLog.AppendFormat(_T("\r\n:   SecondaryMessage is null"));
	}
	else
	{
		strLog.AppendFormat(_T("\r\n:   SecondaryMessage : [Stream=%d,Function=%d,Name=%s]"), secondaryMessage->Stream, secondaryMessage->Function, secondaryMessage->Name);
	}

	AddLog(strLog);
}

// Communication State가 변경될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnCommunicationStateChanged(UbiGEMWrapper::Structure::GEMCommunicationState communicationState)
{
	CString strLog;

	strLog.AppendFormat(_T("OnCommunicationStateChanged : [CommunicationState=%d]"), communicationState);

	AddLog(strLog);
}

// Control State가 변경될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnControlStateChanged(UbiGEMWrapper::Structure::GEMControlState controlState)
{
	CString strLog;

	strLog.Format(_T("OnControlStateChanged : [ControlState=%d]"), controlState);

	AddLog(strLog);
	
	UpdateDialogTitle();
}

void CUbisamGEMSampleCPPDlg::OnControlStateOnlineChangeFailed()
{
	CString strLog;

	strLog.Format(_T("OnControlStateOnlineChangeFailed"));

	AddLog(strLog);

	UpdateDialogTitle();
}

// EquipmentProcess State가 변경될 경우 발생하는 이벤트입니다.
void CUbisamGEMSampleCPPDlg::OnEquipmentProcessState(unsigned char equipmentProcessState)
{
	CString strLog;

	strLog.Format(_T("OnEquipmentProcessState : [EquipmentProcessstate=%d]"), equipmentProcessState);

	AddLog(strLog);
}

// ReportCollectionEvent 메소드를 호출하고, Variable 정보의 Update가 필요할 때 발생하는 이벤트입니다.
// variables 순회하고, Value를 assign 하는 코드입니다.
// DLL 내부에서 데이터의 타입에 따라 변환합니다.
void CUbisamGEMSampleCPPDlg::OnVariableUpdateRequest(UbiGEMWrapper::Structure::GEMVariableUpdateType updateType, UbiGEMWrapper::Structure::List<LPCTSTR>* vids)
{
	CString strLog;

	strLog.Format(_T("OnVariableUpdateRequest : [Count=%d]"), vids->GetCount());
	AddLog(strLog);

	/*
	// List Type Variable의 데이터 설정 방법
	// VID=2000 이고 구조가 아래와 같고, n = 5, m = 4 인 경우
	// Ln DataList
	//    L3 DataInfo
	//       A DataID
	//       U1 SubDataCount
	//       Lm SubDataList
	//          L2 SubDataInfo
	//              A SubDataID
	//              U1 SubDataNo

	UbiGEMWrapper::Structure::VariableInfo* dataList = m_pWrapper->CreateVariable(_T("2000"), UbiGEMWrapper::Structure::GEMSECSFormat_L, _T("DataList"));

	for(int i = 0; i < 5; i++)
	{
		int subCount = 4;

		UbiGEMWrapper::Structure::VariableInfo* dataInfo = m_pWrapper->CreateVariable(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_L, _T("DataInfo"));
		
		dataList->AddChildVariableInfo(dataInfo);

		UbiGEMWrapper::Structure::VariableInfo* dataID = m_pWrapper->CreateVariable(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_A, _T("DataID"));
		dataID->SetValue(_T("DataID"));
		dataInfo->AddChildVariableInfo(dataID);

		UbiGEMWrapper::Structure::VariableInfo* subDataCount = m_pWrapper->CreateVariable(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_U1, _T("SubDataCount"));
		subDataCount->SetValue((uint8_t)subCount);
		dataInfo->AddChildVariableInfo(subDataCount);

		UbiGEMWrapper::Structure::VariableInfo* subDataList = m_pWrapper->CreateVariable(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_L, _T("SubDataList"));
		dataInfo->AddChildVariableInfo(subDataList);

		for(int j = 0; j < subCount; j++)
		{
			UbiGEMWrapper::Structure::VariableInfo* subDataInfo = m_pWrapper->CreateVariable(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_L, _T("SubDataInfo"));
			subDataList->AddChildVariableInfo(subDataInfo);

			UbiGEMWrapper::Structure::VariableInfo* subDataID = m_pWrapper->CreateVariable(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_A, _T("SubDataID"));
			subDataID->SetValue(_T("SubDataID"));
			subDataInfo->AddChildVariableInfo(subDataID);

			UbiGEMWrapper::Structure::VariableInfo* subDataNo = m_pWrapper->CreateVariable(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_U1, _T("SubDataNo"));
			subDataNo->SetValue(_T("SubDataNo"));
			subDataInfo->AddChildVariableInfo(subDataNo);
		}
	}

	m_pWrapper->SetVariable(dataList);
	*/

	if (m_pWrapper != nullptr)
	{
		for (int i = 0, n = vids->GetCount(); i < n; i++)
		{
			CString vid = vids->At(i);

			if (vid == SAMPLE_VID_Alarmset)
			{
				// Format 'L'의 ChildVariable 'n'개 값 설정 방법
				UbiGEMWrapper::Structure::VariableInfo* alarmSet = m_pWrapper->GetVariable(SAMPLE_VID_Alarmset);
				UbiGEMWrapper::Structure::VariableInfo* alarmID;

				if (alarmSet != nullptr)
				{
					for (size_t i = 0; i < m_setAlarmList.size(); i++)
					{
						// 하위 Variable 생성 방법
						alarmID = m_pWrapper->GetVariable(SAMPLE_VID_ALID);
						alarmID->SetValue(m_setAlarmList.at(i));

						alarmSet->AddChildVariableInfo(alarmID);
					}

					m_pWrapper->SetVariable(alarmSet);

					// 상위 V 삭제 시 하위 V 자동 삭제됨
					m_pWrapper->DeleteVariableInfo(alarmSet);
					alarmSet = nullptr;
				}
			}
		}
	}
}

// 사용자 정의 GEM Message의 업데이트가 필요할 경우 발생합니다.
void CUbisamGEMSampleCPPDlg::OnUserGEMMessageUpdateRequest(UbiGEMWrapper::Structure::SECSMessage* pMessage)
{
	CString strLog;

	strLog.AppendFormat(_T("OnUserGEMMessageUpdateRequest: [Stream=%d,Function=%d]"), pMessage->Stream, pMessage->Function);

	AddLog(strLog);
}

// Trace Data를 발송하기 위해 Variable의 Update가 필요한 경우 발생합니다.
void CUbisamGEMSampleCPPDlg::OnTraceDataUpdateRequest(UbiGEMWrapper::Structure::List<LPCTSTR>* vids)
{
	CString strLog;
	UbiGEMWrapper::Structure::VariableInfo* variableInfo;

	if (vids == nullptr)
	{
		strLog.AppendFormat(_T("OnTraceDataUpdateRequest: [vids is null]"));
	}
	else
	{
		strLog.AppendFormat(_T("OnTraceDataUpdateRequest: [Count=%d]"), vids->GetCount());
	}

	AddLog(strLog);

	if (m_pWrapper != nullptr)
	{
		for (int i = 0; i < vids->GetCount(); i++)
		{
			variableInfo = m_pWrapper->GetVariable(vids->At(i));

			// Trace에 등록한 VID와 비교하고, 현재 값을 Set 합니다
			if (_tcscmp(vids->At(i), SAMPLE_VID_ControlState) == 0)
			{
				variableInfo->SetValue(5);
			}

			m_pWrapper->SetVariable(variableInfo);

			m_pWrapper->DeleteVariableInfo(variableInfo);
			variableInfo = nullptr;
		}
	}
}

void CUbisamGEMSampleCPPDlg::InitializeControls()
{
	if (m_pWrapper != nullptr)
	{
		CString strItem;

		m_cboEcv.ResetContent();
		m_cboV.ResetContent();

		// GetAll********* API는 Sample 내의 UI에 Item을 추가하기 위해 사용하는 메소드입니다.
		UbiGEMWrapper::Structure::VariableCollection* pVariables = m_pWrapper->GetAllVariable();

		if (pVariables != nullptr)
		{
			UbiGEMWrapper::Structure::VariableInfo* pVariable;

			for (int i = 0, n = pVariables->Items->GetCount(); i < n; i++)
			{
				pVariable = pVariables->Items->At(i);

				if (pVariable != nullptr)
				{
					strItem.Format(_T("%s : %s"), pVariable->VID, pVariable->Name);

					if (pVariable->VIDType == UbiGEMWrapper::Structure::GEMVariable_ECV)
					{
						m_cboEcv.AddString(strItem);
					}
					else
					{
						m_cboV.AddString(strItem);
					}
				}
			}

			// GetAllVariable() 로 가져온 데이터는 Driver 의 소멸자에서 자동으로 해제 됩니다.
			pVariables = nullptr;
		}

		m_cboCe.ResetContent();

		// GetAll********* API는 Sample 내의 UI에 Item을 추가하기 위해 사용하는 메소드입니다.

		UbiGEMWrapper::Structure::CollectionEventCollection* pCollectionEvents = m_pWrapper->GetAllCollectionEventInfo();

		if (pCollectionEvents != nullptr)
		{
			UbiGEMWrapper::Structure::CollectionEventInfo* pCollectionEvent;

			for (int i = 0, n = pCollectionEvents->Items->GetCount(); i < n; i++)
			{
				pCollectionEvent = pCollectionEvents->Items->At(i);

				if (pCollectionEvent != nullptr)
				{
					strItem.Format(_T("%s : %s"), pCollectionEvent->CEID, pCollectionEvent->Name);

					m_cboCe.AddString(strItem);
				}
			}

			// GetAllCollectionEventInfo() 로 가져온 데이터는 Driver 의 소멸자에서 자동으로 해제 됩니다.
			pCollectionEvents = nullptr;
		}

		m_cboUserMessage.AddString(_T("UserTestMessage"));
	}
}

CString CUbisamGEMSampleCPPDlg::ConvertDriverResult(UbiGEMWrapper::Structure::GEMResult gemResult)
{
	CString strResult;

	switch (gemResult)
	{
	case UbiGEMWrapper::Structure::GEMResult_Ok:
		strResult = _T("OK");
		break;
	case UbiGEMWrapper::Structure::GEMResult_Unknown:
		strResult = _T("Unknown");
		break;
	case UbiGEMWrapper::Structure::GEMResult_NotExistDriverName:
		strResult = _T("Not Exist Driver Name");
		break;
	case UbiGEMWrapper::Structure::GEMResult_NotExistFile:
		strResult = _T("Not Exist File");
		break;
	case UbiGEMWrapper::Structure::GEMResult_FileLoadFailed:
		strResult = _T("File Load Failed");
		break;
	case UbiGEMWrapper::Structure::GEMResult_FileSaveFailed:
		strResult = _T("File Save Failed");
		break;
	case UbiGEMWrapper::Structure::GEMResult_InvalidConfiguration:
		strResult = _T("Invalid Configuration");
		break;
	case UbiGEMWrapper::Structure::GEMResult_AlreadyConnected:
		strResult = _T("Already Connected");
		break;
	case UbiGEMWrapper::Structure::GEMResult_SocketException:
		strResult = _T("Socket Exception");
		break;
	case UbiGEMWrapper::Structure::GEMResult_LicenseVerificationFailed:
		strResult = _T("License Verification Failed");
		break;
	case UbiGEMWrapper::Structure::GEMResult_Disconnected:
		strResult = _T("Disconnected");
		break;
	case UbiGEMWrapper::Structure::GEMResult_ControlStateIsOffline:
		strResult = _T("Control State Is Offline");
		break;
	case UbiGEMWrapper::Structure::GEMResult_SameState:
		strResult = _T("Same State");
		break;
	case UbiGEMWrapper::Structure::GEMResult_Undefined:
		strResult = _T("Undefined");
		break;
	case UbiGEMWrapper::Structure::GEMResult_Disabled:
		strResult = _T("Disabled");
		break;
	case UbiGEMWrapper::Structure::GEMResult_HSMSDriverError:
		strResult = _T("HSMS Driver Error");
		break;
	case UbiGEMWrapper::Structure::GEMResult_HSMSDriverDisconnected:
		strResult = _T("HSMS Driver Disconnected");
		break;
	case UbiGEMWrapper::Structure::GEMResult_NotCommunicating:
		strResult = _T("Not Communicating");
		break;
	case UbiGEMWrapper::Structure::GEMResult_MessageMakeFailed:
		strResult = _T("Message Make Failed");
		break;
	case UbiGEMWrapper::Structure::GEMResult_Exception:
		strResult = _T("Exception");
		break;
	case UbiGEMWrapper::Structure::GEMResult_Mismatch:
		strResult = _T("Mismatch");
		break;
	case UbiGEMWrapper::Structure::GEMResult_HostDenied:
		strResult = _T("Host Denied");
		break;
	case UbiGEMWrapper::Structure::GEMResult_InvalidFormat:
		strResult = _T("Invalid Format");
		break;
	default:
		strResult = _T("Undefined");
		break;
	}

	return strResult;
}

void CUbisamGEMSampleCPPDlg::UpdateDialogTitle()
{
	CString strTitle;

	strTitle = _T("UbiGEM Sample : ");

	if (m_pWrapper != nullptr)
	{
		if (m_pWrapper->GetConnected() == true)
		{
			switch (m_pWrapper->GetControlState())
			{
			case UbiGEMWrapper::Structure::GEMControlState_OnlineRemote:
				strTitle.Append(_T("Connected / OnlineRemote"));
				break;
			case UbiGEMWrapper::Structure::GEMControlState_OnlineLocal:
				strTitle.Append(_T("Connected / OnlineLocal"));
				break;
			case UbiGEMWrapper::Structure::GEMControlState_HostOffline:
				strTitle.Append(_T("Connected / HostOffline"));
				break;
			case UbiGEMWrapper::Structure::GEMControlState_AttemptOnline:
				strTitle.Append(_T("Connected / AttemptOnline"));
				break;
			default:
				strTitle.Append(_T("Connected / EquipmentOffline"));
				break;
			}
		}
		else
		{
			strTitle.Append(_T("Disconnected"));
		}
	}

	strTitle.AppendFormat(_T(" - %s"), m_strUgcFileName);

	SetWindowText(strTitle);
}

void CUbisamGEMSampleCPPDlg::AddLog(CString strLog)
{
	CString strData;
	CTime cTime = CTime::GetCurrentTime();

	strData.AppendFormat(_T("[%04d-%02d-%02d "),
		cTime.GetYear(),
		cTime.GetMonth(),
		cTime.GetDay());

	strData.AppendFormat(_T("%02d:%02d:%02d]"),
		cTime.GetHour(),
		cTime.GetMinute(),
		cTime.GetSecond());

	strData.AppendFormat(_T(" %s\r\n"), strLog);

	if (m_edtLog.m_hWnd != nullptr)
	{
		m_edtLog.SetSel(-2, -1);
		m_edtLog.ReplaceSel(strData);
	}
}

void CUbisamGEMSampleCPPDlg::AddLog(CString strLog, UbiGEMWrapper::Structure::GEMResult gemResult)
{
	CString strData;
	CTime cTime = CTime::GetCurrentTime();

	strData.AppendFormat(_T("[%04d-%02d-%02d "),
		cTime.GetYear(),
		cTime.GetMonth(),
		cTime.GetDay());

	strData.AppendFormat(_T("%02d:%02d:%02d]"),
		cTime.GetHour(),
		cTime.GetMinute(),
		cTime.GetSecond());

	strData.AppendFormat(_T(" [Result=%s] : %s\r\n"), ConvertDriverResult(gemResult), strLog);

	if (m_edtLog.m_hWnd != nullptr)
	{
		m_edtLog.SetSel(-2, -1);
		m_edtLog.ReplaceSel(strData);
	}
}

CString CUbisamGEMSampleCPPDlg::GetGUIString(CComboBox* pTarget)
{
	CString strInputValue;
	CString strSelectedItem;

	pTarget->GetWindowText(strSelectedItem);

	if (strSelectedItem.GetAllocLength() > 0)
	{
		CString strToken;
		int nTokenPos = 0;

		strInputValue = strSelectedItem.Tokenize(_T(":"), nTokenPos).Trim();
	}
	else
	{
		strInputValue = _T("");
	}

	return strInputValue;
}

CString CUbisamGEMSampleCPPDlg::GetGUIString(CEdit* pTarget)
{
	CString strSelectedItem;
	pTarget->GetWindowText(strSelectedItem);
	return strSelectedItem;
}

CString CUbisamGEMSampleCPPDlg::CheckValidationParameterItem(UbiGEMWrapper::Structure::EnhancedCommandParameterItem* enhancedCommandParameterItem, UbiGEMWrapper::Structure::RemoteCommandParameterResult* paramResult)
{
	CString result;
	UbiGEMWrapper::Structure::EnhancedCommandParameterItem* item;

	if (enhancedCommandParameterItem->Format == UbiGEMWrapper::Structure::GEMSECSFormat_L && enhancedCommandParameterItem->ChildParameterItems != nullptr)
	{
		for (int i = 0; i < enhancedCommandParameterItem->ChildParameterItems->GetCount(); i++)
		{
			item = enhancedCommandParameterItem->ChildParameterItems->At(i);

			result.AppendFormat(_T("\r\n:   [CPNAME=%s,Format=%s,CEPVAL=%s]"), item->Name, SECSFormatAsString(item->Format), item->Value);

			if(paramResult != nullptr)
			{
				paramResult->AddChildParameterResult(item->Name, m_iAck);
			}
			result.Append(CheckValidationParameterItem(item, paramResult));
		}
	}

	return result;
}

void CUbisamGEMSampleCPPDlg::MakePPBody(UbiGEMWrapper::Structure::List<unsigned char>* ppBody)
{
	int start_number = 0;
	int end_number = 100;
	size_t count = rand() % 999;

	if (ppBody != nullptr)
	{
		for (int i = 0; i < count; i++)
		{
			ppBody->Add((rand() % (end_number - start_number + 1)) + start_number);
		}
	}
}

void CUbisamGEMSampleCPPDlg::ProcessProgramParsing(bool withoutValue, UbiGEMWrapper::Structure::FmtPPCollection* fmtPpCollection)
{
	if (fmtPpCollection != nullptr)
	{
		UbiGEMWrapper::Structure::FmtPPCCodeInfo* pCameraIsUse = m_pWrapper->CreateFmtPPCCodeInfo(_T("CAMERA_IS_USE"));
		fmtPpCollection->AddFmtPPCCodeInfo(pCameraIsUse);

		pCameraIsUse->AddFmtPPItem(_T("CAMERA1"), _T("True"), UbiGEMWrapper::Structure::GEMSECSFormat_Boolean);
		pCameraIsUse->AddFmtPPItem(_T("CAMERA2"), _T("True"), UbiGEMWrapper::Structure::GEMSECSFormat_Boolean);
		pCameraIsUse->AddFmtPPItem(_T("CAMERA3"), _T("True"), UbiGEMWrapper::Structure::GEMSECSFormat_Boolean);
		pCameraIsUse->AddFmtPPItem(_T("CAMERA4"), _T("True"), UbiGEMWrapper::Structure::GEMSECSFormat_Boolean);
		pCameraIsUse->AddFmtPPItem(_T("CAMERA5"), _T("True"), UbiGEMWrapper::Structure::GEMSECSFormat_Boolean);

		UbiGEMWrapper::Structure::FmtPPCCodeInfo* pCameraLigthValue = m_pWrapper->CreateFmtPPCCodeInfo(_T("CAMERA_LIGHT_VALUE"));
		fmtPpCollection->AddFmtPPCCodeInfo(pCameraLigthValue);

		pCameraLigthValue->AddFmtPPItem(_T("CAMERA1"), _T("10"), UbiGEMWrapper::Structure::GEMSECSFormat_U1);
		pCameraLigthValue->AddFmtPPItem(_T("CAMERA2"), _T("20"), UbiGEMWrapper::Structure::GEMSECSFormat_U2);
		pCameraLigthValue->AddFmtPPItem(_T("CAMERA3"), _T("30"), UbiGEMWrapper::Structure::GEMSECSFormat_U4);
		pCameraLigthValue->AddFmtPPItem(_T("CAMERA4"), _T("40"), UbiGEMWrapper::Structure::GEMSECSFormat_U8);
		pCameraLigthValue->AddFmtPPItem(_T("CAMERA5"), _T("0"), UbiGEMWrapper::Structure::GEMSECSFormat_I1);

		UbiGEMWrapper::Structure::FmtPPCCodeInfo* pBeamProfiler = m_pWrapper->CreateFmtPPCCodeInfo(_T("BEAM_PROFILER"));
		fmtPpCollection->AddFmtPPCCodeInfo(pBeamProfiler);

		pBeamProfiler->AddFmtPPItem(_T("CNTPERMM"), _T("100.0000000000"), UbiGEMWrapper::Structure::GEMSECSFormat_F4);
		pBeamProfiler->AddFmtPPItem(_T("SPDRATIO"), _T("0.0000000000"), UbiGEMWrapper::Structure::GEMSECSFormat_F8);
		pBeamProfiler->AddFmtPPItem(_T("ACCDCC"), _T("0"), UbiGEMWrapper::Structure::GEMSECSFormat_I2);
		pBeamProfiler->AddFmtPPItem(_T("LENGTH"), _T("500"), UbiGEMWrapper::Structure::GEMSECSFormat_I4);

		UbiGEMWrapper::Structure::FmtPPCCodeInfo* pData = m_pWrapper->CreateFmtPPCCodeInfo(_T("DATA"));
		fmtPpCollection->AddFmtPPCCodeInfo(pData);

		pData->AddFmtPPItem(_T("GLOBAL_SIZE"), _T("100.2500, 50.3500"), UbiGEMWrapper::Structure::GEMSECSFormat_A);
		pData->AddFmtPPItem(_T("MARK_SIZE"), _T("1.0000, 1.0000"), UbiGEMWrapper::Structure::GEMSECSFormat_A);
		pData->AddFmtPPItem(_T("MARK_DISTANCE"), _T("77"), UbiGEMWrapper::Structure::GEMSECSFormat_I8);
	}
}

void CUbisamGEMSampleCPPDlg::OnMnuOpenUgc()
{
	CString filter = _T("Sample(*.ugc)|*.ugc||");
	CFileDialog dlg(TRUE, _T(""), _T(""), OFN_HIDEREADONLY, filter);
	//dlg.m_ofn.lpstrInitialDir = "C:\Users\ubisam007\Documents\UbiSam\UbiGEM\Samples";

	if (dlg.DoModal() == IDOK)	
	{
		CString strTitle;
		m_strUgcFileName = dlg.GetPathName();
	}

	UpdateDialogTitle();
}

void CUbisamGEMSampleCPPDlg::OnMnuExit()
{
	AfxGetMainWnd()->PostMessage(WM_COMMAND, ID_APP_EXIT, 0);
}

void CUbisamGEMSampleCPPDlg::OnMnuInitilaize()
{
	CString strLog;
	CString strError;
	
	UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->Initialize(m_strUgcFileName);
	
	if (gemResult == UbiGEMWrapper::Structure::GEMResult_Ok)
	{
		InitializeControls();
		strLog.Format(_T("Driver Initilaize"));
	}
	else
	{
		strError = m_pWrapper->GetInitializeError();
		strLog.Format(_T("Driver Initialize Fail. [Reason=%s]"), strError);
	}

	AddLog(strLog, gemResult);
}

void CUbisamGEMSampleCPPDlg::OnMnuStart()
{
	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->Start();

		AddLog(_T("Driver Start"), gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnMnuStop()
{
	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->Stop();

		AddLog(_T("Driver Stop"), gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnOffline()
{
	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestOffline();

		AddLog(_T("Request Offline"), gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnOnlineLocal()
{
	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestOnlineLocal();

		
		AddLog(_T("RequestOnlineLocal"), gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnOnlineRemote()
{
	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestOnlineRemote();

		AddLog(_T("RequestOnlineRemote"), gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnEcSet()
{
	if (m_pWrapper != nullptr)
	{
		CString strEcid = GetGUIString(&m_cboEcv);
		CString strEcv = GetGUIString(&m_edtECV);
		CString strLog;

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->SetEquipmentConstant(strEcid, strEcv);

		strLog.Format(_T("Set EquipmentConstant : [ECID=%s,ECV=%s]"), strEcid, strEcv);
		
		AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnEcListSet() 
{
	if (m_pWrapper != nullptr)
	{
		CString strEcid;
		CString strEcv;
		CString strLog;

		if (m_pWrapper != nullptr)
		{
			strEcid = _T("102");
			strEcv = _T("0");
			UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->SetEquipmentConstant(strEcid, strEcv);
			strLog.Format(_T("Set EquipmentConstant : [ECID=%s,ECV=%s])"), strEcid, strEcv);
			AddLog(strLog, gemResult);

			strEcid = _T("103");
			strEcv = _T("10");
			gemResult = m_pWrapper->SetEquipmentConstant(strEcid, strEcv);
			strLog.Format(_T("Set EquipmentConstant : [ECID=%s,ECV=%s])"), strEcid, strEcv);
			AddLog(strLog, gemResult);
		}
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnVSet()
{
	CString strLog;
	CString strVid = GetGUIString(&m_cboV);
	CString strValue = GetGUIString(&m_edtV);
	UbiGEMWrapper::Structure::GEMResult gemResult;

	if (m_pWrapper != nullptr)
	{
		gemResult = m_pWrapper->SetVariable(strVid, strValue);

		strLog.Format(_T("Set Variable : [VID=%s,SV=%s])"), strVid, strValue);

		AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnVListSet()
{
	if (m_pWrapper != nullptr)
	{
		CString strVid = GetGUIString(&m_cboV);
		CString strValue = GetGUIString(&m_edtV);

		CString strLog;

		strVid = _T("14");
		strValue = _T("MDL11");
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->SetVariable(strVid, strValue);
		strLog.Format(_T("Set Variable : [VID=%s,SV=%s])"), strVid, strValue);
		AddLog(strLog, gemResult);

		strVid = _T("15");
		strValue = _T("V1.0");
		gemResult = m_pWrapper->SetVariable(strVid, strValue);
		strLog.Format(_T("Set Variable : [VID=%s,SV=%s])"), strVid, strValue);
		AddLog(strLog, gemResult);

		strVid = _T("33");
		strValue = _T("1");
		gemResult = m_pWrapper->SetVariable(strVid, strValue);
		strLog.Format(_T("Set Equipment Constant(VID=%s, SV=%s)"), strVid, strValue);
		AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnReportCe1()
{
	// ReportCollectionEvent(string) API의 사용은 미리 정의된 Collection Event를 보고할 경우 입니다.
	// OnVariableUpdateRequest Event 발생 합니다.
	// OnVariableUpdateRequest Event 내에서 Variable의 값을 설정 하는것도 가능합니다.

	CString strCEID = GetGUIString(&m_cboCe);

	// EquipmentConstantChanged 관련 Collection Event 는 직접적으로 호출하면 안됩니다.
	if (strCEID != SAMPLE_CEID_EquipmentConstantChanged && strCEID != SAMPLE_CEID_EquipmentConstantChangedByHost && strCEID != _T(""))
	{
		if (m_pWrapper != nullptr)
		{
			m_pWrapper->SetVariable(SAMPLE_VID_ControlState, 3);

			// OnVariableUpdateReqeust 이벤트 발생을 원하지 않을 경우
			// UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReportCollectionEvent(strCEID, false);

			UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReportCollectionEvent(strCEID);

			AddLog(_T("ReportCollectionEvent1"), gemResult);
		}
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnReportCe2()
{
	// ReportCollectionEvent(CollectionEventInfo) API는 Report 하려는 Collection Event의 구조가 복잡할 경우 사용하기 좋습니다.
	// // Collection Event를 Code로 구성하여 Report 합니다.
	// ※ 호스트에서 DefineReport를 사용하는 업체는 ReportCollectionEvent(CollectionEventInfo) API를 사용하실 경우, Code 수정이 불가피 합니다.

	CString strCEID = GetGUIString(&m_cboCe);

	// EquipmentConstantChanged 관련 Collection Event 는 직접적으로 호출하면 안됩니다.
	if (strCEID != SAMPLE_CEID_EquipmentConstantChanged && strCEID != SAMPLE_CEID_EquipmentConstantChangedByHost && strCEID != _T(""))
	{
		UbiGEMWrapper::Structure::CollectionEventInfo* collectionEventInfo;
		UbiGEMWrapper::Structure::ReportInfo* reportInfo;
		UbiGEMWrapper::Structure::VariableInfo* variableInfo;

		if (m_pWrapper != nullptr)
		{
			collectionEventInfo = m_pWrapper->CreateCollectionEventInfo(strCEID);
			
			reportInfo = m_pWrapper->CreateReportInfo(_T("1"));

			collectionEventInfo->Reports->AddReportInfo(reportInfo);

			variableInfo = m_pWrapper->CreateVariable(_T("100"), UbiGEMWrapper::Structure::GEMSECSFormat_A, _T("V1"));
			variableInfo->SetValue(_T("1.0.0"));

			reportInfo->Variables->AddVariableInfo(variableInfo);

			variableInfo = m_pWrapper->CreateVariable(_T("200"), UbiGEMWrapper::Structure::GEMSECSFormat_U1, _T("V2"));
			variableInfo->SetValue(25);

			reportInfo->Variables->AddVariableInfo(variableInfo);

			UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReportCollectionEvent(collectionEventInfo);

			AddLog(_T("ReportCollectionEvent2"), gemResult);

			// CE 삭제 시 하위 아이템인 REPORT와 V 자동 삭제됨
			m_pWrapper->DeleteCollectionEventInfo(collectionEventInfo);
			collectionEventInfo = nullptr;
		}
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnProcessStateChange()
{
	CString str;
	CString strLog;
	m_edtProcessState.GetWindowText(str);
	int istr;
	istr = _ttoi(str);

	UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReportEquipmentProcessingState(istr);

	strLog.Format(_T("ReportEquipmentProcessingState : [State=%d]"), istr);
	AddLog(strLog, gemResult);
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnAlarmSet()
{
	CString str;
	CString strLog;
	m_edtAlarmId.GetWindowText(str);
	int64_t alarmID = _ttoi64(str);

	std::vector<int64_t>::iterator it = std::find(m_setAlarmList.begin(), m_setAlarmList.end(), alarmID);

	if (it == m_setAlarmList.end())
	{
		m_setAlarmList.push_back(alarmID);

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReportAlarmSet(alarmID);

		strLog.Format(_T("AlarmSet : [ALID=%lld]"), alarmID);
		AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnAlarmClear()
{
	CString str;
	CString strLog;
	m_edtAlarmId.GetWindowText(str);
	int64_t alarmID = _ttoi64(str);

	std::vector<int64_t>::iterator it = std::find(m_setAlarmList.begin(), m_setAlarmList.end(), alarmID);
	
	if (it != m_setAlarmList.end())
	{
		m_setAlarmList.erase(it);
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReportAlarmClear(alarmID);

		strLog.Format(_T("AlarmClear : [ALID=%lld]"), alarmID);
		AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnTerminalMessageSend()
{
	CString tid;
	CString strLog;
	m_edtTId.GetWindowText(tid);
	CString tmsg = GetGUIString(&m_edtTerminalMessage);
	int itid = _ttoi(tid);

	UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->ReportTerminalMessage(itid, tmsg);

	strLog.Format(_T("ReportTerminalMessage : [TID=%d,TEXT=%s]"), itid, tmsg);
	AddLog(strLog, gemResult);
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpRequest()
{
	CString ppid = GetGUIString(&m_edtPpid);
	CString strppid,strLog;
	strppid.Format(_T("%s"),ppid);

	if (m_pWrapper != nullptr && strppid.GetAllocLength() > 0)
	{
			UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestPPRequest(ppid);

			strLog.Format(_T("RequestPPRequest : [PPID=%s]"), ppid);

			AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpSend()
{
	CString ppid = GetGUIString(&m_edtPpid);
	CString strppid, strLog;
	byte bRand;
	int nStart;
	int nEnd;
	int nCount;

	nStart = 0;
	nEnd = 1000;
	nCount = ((rand() % (nEnd - nStart + 1)) + nStart);
	UbiGEMWrapper::Structure::List<uint8_t> *ppbody;

	strppid.Format(_T("%s"), ppid);

	if (m_pWrapper != nullptr && strppid.GetAllocLength() > 0)
	{
		ppbody = m_pWrapper->CreateU1List();
		nEnd = 255;
		for (int i = 0; i < nCount; i++)
		{
			bRand = (byte)((rand() % (nEnd - nStart + 1)) + nStart);
			ppbody->Add(bRand);
		}
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestPPSend(ppid, ppbody);

		strLog.Format(_T("RequestPPSend : [PPID=%s]"), ppid);

		AddLog(strLog, gemResult);

		m_pWrapper->DeleteU1List(ppbody);
		ppbody = nullptr;
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpLoadInquire()
{
	CString ppid = GetGUIString(&m_edtPpid);
	CString strppid,strLog;
	int nLnegth;
	int nStart;
	int nEnd;

	nStart = 0;
	nEnd = 1000;

	strppid.Format(_T("%s"), ppid);

	if (m_pWrapper != nullptr && strppid.GetAllocLength() > 0)
	{
		nLnegth = ((rand() % (nEnd - nStart + 1)) + nStart);

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestPPLoadInquire(ppid, nLnegth);

		strLog.Format(_T("RequestPPLoadInquire : [PPID=%s]"), ppid);

		AddLog(strLog, gemResult);
	}
	
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnPpChanged()
{
	CString ppid = GetGUIString(&m_edtPpid);
	CString strLog;
	int64_t llStatus;
	llStatus = 0;

	UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestPPChanged(llStatus,ppid);

	strLog.Format(_T("RequestPPChanged : [PPID=%s]"), ppid);

	AddLog(strLog, gemResult);
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpRequest()
{
	CString ppid = GetGUIString(&m_edtFmtPpid);
	CString strppid;
	CString strLog;
	
	strppid.Format(_T("%s"), ppid);

	if (m_pWrapper != nullptr && strppid.GetAllocLength() > 0)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestFmtPPRequest(ppid);

		strLog.Format(_T("RequestFmtPPRequest : [PPID=%s]"), ppid);

		AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpChanged()
{
	CString ppid = GetGUIString(&m_edtFmtPpid);
	CString strLog;

	int64_t llStatus;

	llStatus = 0;

	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestFmtPPChanged(llStatus, ppid);
		strLog.Format(_T("RequestFmtPPChanged : [PPID=%s]"), ppid);

		AddLog(strLog, gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpSendWoValue()
{
	CString ppid = GetGUIString(&m_edtFmtPpid);
	CString strLog;
	UbiGEMWrapper::Structure::FmtPPCollection* pFmtPPCollection;

	if (m_pWrapper != nullptr && ppid != _T(""))
	{
		pFmtPPCollection = m_pWrapper->CreateFmtPPCollection(ppid, _T("5"), _T("5"));

		ProcessProgramParsing(false, pFmtPPCollection);

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestFmtPPSendWithoutValue(pFmtPPCollection);

		strLog.Format(_T("RequestFmtPPSendWithoutValue : [PPID=%s]"), ppid);

		AddLog(strLog, gemResult);

		m_pWrapper->DeleteFmtPPCollection(pFmtPPCollection);
		pFmtPPCollection = nullptr;
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpSend()
{
	CString ppid = GetGUIString(&m_edtFmtPpid);
	CString strppid, strLog;
	strppid.Format(_T("%s"), ppid);

	UbiGEMWrapper::Structure::FmtPPCollection* pFmtPPCollection;

	if (m_pWrapper != nullptr && strppid.GetAllocLength() > 0)
	{ 
		pFmtPPCollection = m_pWrapper->CreateFmtPPCollection(ppid, _T("5"), _T("5"));

		ProcessProgramParsing(false, pFmtPPCollection);

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestFmtPPSend(pFmtPPCollection);

		strLog.Format(_T("RequestFmtPPSend : [fmtPPID=%s]"), ppid);

		AddLog(strLog, gemResult);

		m_pWrapper->DeleteFmtPPCollection(pFmtPPCollection);
		pFmtPPCollection = nullptr;
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnFmtPpVerificationSend()
{
	CString ppid = GetGUIString(&m_edtFmtPpid);
	CString strppid, strLog;
	strppid.Format(_T("%s"), ppid);


	UbiGEMWrapper::Structure::FmtPPVerificationCollection* pFmtPPVerificationCollection;

	pFmtPPVerificationCollection = m_pWrapper->CreateFmtPPVerificationCollection(ppid);

	if (m_pWrapper != nullptr && strppid.GetAllocLength() > 0)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestFmtPPVerificationSend(pFmtPPVerificationCollection);

		strLog.Format(_T("RequestFmtPPVerificationSend : [fmtPPID=%s]"), ppid);

		AddLog(strLog, gemResult);
	}

	m_pWrapper->DeleteFmtPPVerificationCollection(pFmtPPVerificationCollection);
	pFmtPPVerificationCollection = nullptr;
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnTimeRequest()
{
	if (m_pWrapper != nullptr)
	{
		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestDateTime();

		AddLog(_T("RequestDateTime"), gemResult);
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnLoopback()
{
	CString Abs = GetGUIString(&m_edtAbs);
	UbiGEMWrapper::Structure::List<uint8_t>* pAbs;

	int length = Abs.GetAllocLength();

	if (m_pWrapper != nullptr && length > 0)
	{
		pAbs = m_pWrapper->CreateU1List();

		for (int i = 0; i < length; i++)
		{
			pAbs->Add((uint8_t)Abs.GetAt(i));
		}

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->RequestLoopback(pAbs);

		AddLog(_T("RequestLoopback"), gemResult);

		m_pWrapper->DeleteU1List(pAbs);
		pAbs = nullptr;
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnUserMessageSend()
{
	CString strBlank = _T("");
	CString strName;

	UbiGEMWrapper::Structure::SECSMessage* pMessage = m_pWrapper->GetUserMessage(99, 1, true);
	
	if (m_pWrapper != nullptr && pMessage != nullptr)
	{
		pMessage->Body->InitializeItems();

		// 아래 형식의 User Defined Message를 송신하고자 할 경우
		// <L, 2
		//     <B, 1 '2'>
		//     <A, 2 'OK'>
		// >
		pMessage->Body->AddListItem(_T(""), 2);
		pMessage->Body->AddSECSItem(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_B, 1);
		pMessage->Body->AddSECSItem(_T(""), UbiGEMWrapper::Structure::GEMSECSFormat_A, _T("OK"));

		UbiGEMWrapper::Structure::GEMResult gemResult = m_pWrapper->SendSECSMessage(pMessage);

		AddLog(_T("SendUserMessage"), gemResult);

		m_pWrapper->DeleteSECSMessage(pMessage);
		pMessage = nullptr;
	}
	else
	{
		AddLog(_T("SendUserMessage failed. [Reason=Not found]"));
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnAckApply()
{
	CString strAck;
	int iAck;

	m_edtAck.GetWindowText(strAck);
	iAck = _ttoi(strAck);

	if (m_pWrapper != nullptr)
	{
		m_iAck = iAck;
	}
}

void CUbisamGEMSampleCPPDlg::OnBnClickedBtnClearLog()
{
	m_edtLog.SetWindowText(_T(""));
}

void CUbisamGEMSampleCPPDlg::OnCbnSelchangeCboEc()
{
	if (m_pWrapper != nullptr)
	{
		CString str;
		CString strEcid = GetGUIString(&m_cboEcv);

		UbiGEMWrapper::Structure::VariableInfo *pVariableInfo = m_pWrapper->GetVariable(strEcid);

		if (pVariableInfo != nullptr)
		{
			switch (pVariableInfo->Value->Format)
			{
			case UbiGEMWrapper::Structure::GEMSECSFormat_Boolean:
				if (pVariableInfo->Value->GetValueBool() == true)
				{
					str.Format(_T("True"));
				}
				else
				{
					str.Format(_T("False"));
				}
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_A:
				str.Format(_T("%s"), pVariableInfo->Value->GetValueString());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I1:
				str.Format(_T("%d"), pVariableInfo->Value->GetValueI1());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I2:
				str.Format(_T("%d"), pVariableInfo->Value->GetValueI2());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I4:
				str.Format(_T("%d"), pVariableInfo->Value->GetValueI4());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I8:
				str.Format(_T("%lld"), pVariableInfo->Value->GetValueI8());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U1:
				str.Format(_T("%u"), pVariableInfo->Value->GetValueU1());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U2:
				str.Format(_T("%u"), pVariableInfo->Value->GetValueU2());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U4:
				str.Format(_T("%u"), pVariableInfo->Value->GetValueU4());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U8:
				str.Format(_T("%llu"), pVariableInfo->Value->GetValueU8());
				m_edtECV.SetWindowText(str);
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_X:
				break;
			default:
				break;
			}

			m_pWrapper->DeleteVariableInfo(pVariableInfo);
			pVariableInfo = nullptr;
		}
	}
}

CString CUbisamGEMSampleCPPDlg::SECSFormatAsString(UbiGEMWrapper::Structure::GEMSECSFormat format)
{
	CString result;

	result.Append(UbiGEMWrapper::SECSItemFormatAsString(format));

	return result;
}
CString CUbisamGEMSampleCPPDlg::SECSValueAsString(UbiGEMWrapper::Structure::GEMSECSFormat format, UbiGEMWrapper::Structure::SECSValue* value)
{
	CString result;

	if (value != nullptr)
	{
			switch (format)
			{
			case UbiGEMWrapper::Structure::GEMSECSFormat_A:
			case UbiGEMWrapper::Structure::GEMSECSFormat_J:
				if (value->Length > 0)
				{
					result.Format(_T("%s"), value->GetValueString());
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_B:
				if (value->Length == 1)
				{
					result.Format(_T("%d"), value->GetValueB());
				}
				else if (value->Length > 1)
				{
					uint8_t* arr = value->GetValueBArray();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%d "), arr[i]);
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_Boolean:
				if (value->Length == 1)
				{
					result.Format(_T("%s"), value->GetValueBool() == true ? _T("T") : _T("F"));
				}
				else if (value->Length > 1)
				{
					bool* arr = value->GetValueBoolArray();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%s "), arr[i] == true ? _T("T") : _T("F"));
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_F4:
				if (value->Length == 1)
				{
					result.Format(_T("%f"), value->GetValueF4());
				}
				else if (value->Length > 1)
				{
					float* arr = value->GetValueF4Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%s "), arr[i]);
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_F8:
				if (value->Length == 1)
				{
					result.Format(_T("%f"), value->GetValueF8());
				}
				else if (value->Length > 1)
				{
					double* arr = value->GetValueF8Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%f "), arr[i]);
					}
				}

				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I1:
				if (value->Length == 1)
				{
					result.Format(_T("%d"), value->GetValueI1());
				}
				else if (value->Length > 1)
				{
					int8_t* arr = value->GetValueI1Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%d "), arr[i]);
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I2:
				if (value->Length == 1)
				{
					result.Format(_T("%d"), value->GetValueI2());
				}
				else if (value->Length > 1)
				{
					int16_t* arr = value->GetValueI2Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%d "), arr[i]);
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I4:
				if (value->Length == 1)
				{
					result.Format(_T("%d"), value->GetValueI4());
				}
				else if (value->Length > 1)
				{
					int32_t* arr = value->GetValueI4Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%d "), arr[i]);
					}
				}

				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_I8:
				if (value->Length == 1)
				{
					result.Format(_T("%lld"), value->GetValueI8());
				}
				else if (value->Length > 1)
				{
					int64_t* arr = value->GetValueI8Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%lld "), arr[i]);
					}
				}

				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U1:
				if (value->Length == 1)
				{
					result.Format(_T("%u"), value->GetValueU1());
				}
				else if (value->Length > 1)
				{
					uint8_t* arr = value->GetValueU1Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%u "), arr[i]);
					}
				}

				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U2:
				if (value->Length == 1)
				{
					result.Format(_T("%u"), value->GetValueU2());
				}
				else if (value->Length > 1)
				{
					uint16_t* arr = value->GetValueU2Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%u "), arr[i]);
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U4:
				if (value->Length == 1)
				{
					result.Format(_T("%u"), value->GetValueU4());
				}
				else if (value->Length > 1)
				{
					uint32_t* arr = value->GetValueU4Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%u "), arr[i]);
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_U8:
				if (value->Length == 1)
				{
					result.Format(_T("%llu"), value->GetValueU8());
				}
				else if (value->Length > 1)
				{
					uint64_t* arr = value->GetValueU8Array();

					for (int i = 0; i < value->Length; i++)
					{
						result.AppendFormat(_T("%llu "), arr[i]);
					}
				}
				break;
			case UbiGEMWrapper::Structure::GEMSECSFormat_L:
			case UbiGEMWrapper::Structure::GEMSECSFormat_X:
			case UbiGEMWrapper::Structure::GEMSECSFormat_None:
			default:
				break;
		}
	}

	return result;
}
