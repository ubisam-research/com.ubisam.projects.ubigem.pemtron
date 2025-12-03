
// Ubisam.GEM.Sample.CPPDlg.h: 헤더 파일
//

#pragma once

#include <stdint.h>
#include <vector>
#include <algorithm>
#include <GEMStructure.h>
#include <GEMWrapper.h>

// CUbisamGEMSampleCPPDlg 대화 상자
class CUbisamGEMSampleCPPDlg : public CDialogEx, UbiGEMWrapper::IGEMDriverEvents
{
// 생성입니다.
public:
	CUbisamGEMSampleCPPDlg(CWnd* pParent = nullptr);	// 표준 생성자입니다.
	~CUbisamGEMSampleCPPDlg();

// 대화 상자 데이터입니다.
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_UBISAMGEMSAMPLECPP_DIALOG };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 지원입니다.

// 구현입니다.
protected:
	HICON m_hIcon;

	// 생성된 메시지 맵 함수
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()

private:
	UbiGEMWrapper::GEMWrapper* m_pWrapper;
	CString m_strUgcFileName;
    std::vector<int64_t> m_setAlarmList;
    int m_iAck;
    

private:
	void OnGEMConnected(LPCTSTR strIpAddress, int nPortNo);
	void OnGEMDisconnected(LPCTSTR strIpAddress, int nPortNo);
    void OnGEMSelected(LPCTSTR strIpAddress, int nPortNo);
    void OnGEMDeselected(LPCTSTR strIpAddress, int nPortNo);

    void OnSECSTimeout(UbiGEMWrapper::Structure::SECSTimeoutType timeoutType);
    void OnSECST3Timeout(UbiGEMWrapper::Structure::SECSMessage* message);

    void OnInvalidMessageReceived(UbiGEMWrapper::Structure::GEMMessageValidationError error, UbiGEMWrapper::Structure::SECSMessage* message);
    void OnReceivedUnknownMessage(UbiGEMWrapper::Structure::SECSMessage* message);
    int OnReceivedEstablishCommunicationsRequest(LPCTSTR strMdln, LPCTSTR strSofRev);
    void OnReceivedRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo);
    void OnReceivedInvalidRemoteCommand(UbiGEMWrapper::Structure::RemoteCommandInfo* remoteCommandInfo);
    void OnReceivedEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo);
    void OnReceivedInvalidEnhancedRemoteCommand(UbiGEMWrapper::Structure::EnhancedRemoteCommandInfo* remoteCommandInfo);
    void OnReceivedNewECVSend(unsigned int systemBytes, UbiGEMWrapper::Structure::VariableCollection* newEcInfo);
    void OnReceivedLoopback(UbiGEMWrapper::Structure::List<unsigned char>* receiveData);
    void OnReceivedTerminalMessage(unsigned int systemBytes, int tid, LPCTSTR strTerminalMessage);
    void OnReceivedTerminalMultiMessage(unsigned int systemBytes, int tid, UbiGEMWrapper::Structure::List<LPCTSTR>* strTerminalMessages);
    void OnReceivedRequestOffline(unsigned int systemBytes);
    void OnReceivedRequestOnline(unsigned int systemBytes);
    void OnReceivedDefineReport();
    void OnReceivedLinkEventReport();
    void OnReceivedEnableDisableEventReport();
    void OnReceivedEnableDisableAlarmSend();
    bool OnResponseDateTimeRequest(UbiGEMWrapper::Structure::DateTime* timeData);
    void OnResponseLoopback(UbiGEMWrapper::Structure::List<unsigned char>* receiveData, UbiGEMWrapper::Structure::List<unsigned char>* sendData);
    void OnResponseEventReportAcknowledge(LPCTSTR ceid, int ack);
    void OnReceivedPPLoadInquire(unsigned int systemBytes, LPCTSTR ppid, int length);
    void OnReceivedPPSend(unsigned int systemBytes, LPCTSTR ppid, UbiGEMWrapper::Structure::List<unsigned char>* ppbody);
    void OnReceivedFmtPPSend(unsigned int systemBytes, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
    void OnResponsePPLoadInquire(int ppgnt, LPCTSTR ppid);
    void OnResponsePPSend(int ack, LPCTSTR ppid);
    void OnResponsePPRequest(LPCTSTR ppid, UbiGEMWrapper::Structure::List<unsigned char>* ppbody);
    void OnResponseFmtPPSend(int ack, UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
    void OnResponseFmtPPRequest(UbiGEMWrapper::Structure::FmtPPCollection* fmtPPCollection);
    void OnResponseFmtPPVerification(UbiGEMWrapper::Structure::FmtPPVerificationCollection* fmtPPVerificationCollection);
    void OnResponseTerminalRequest(int ack);
    void OnReceivedDateTimeRequest(unsigned int systemBytes, UbiGEMWrapper::Structure::DateTime* timeData);
    void OnReceivedDateTimeSetRequest(unsigned int systemBytes, UbiGEMWrapper::Structure::DateTime* timeData);
    void OnReceivedPPRequest(unsigned int systemBytes, LPCTSTR ppid);
    void OnReceivedFmtPPRequest(unsigned int systemBytes, LPCTSTR ppid);
    void OnReceivedDeletePPSend(unsigned int systemBytes, UbiGEMWrapper::Structure::List<LPCTSTR>* ppids);
    void OnReceivedCurrentEPPDRequest(unsigned int systemBytes);
    void OnUserPrimaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* message);
    void OnUserSecondaryMessageReceived(UbiGEMWrapper::Structure::SECSMessage* primaryMessage, UbiGEMWrapper::Structure::SECSMessage* secondaryMessage);
    void OnCommunicationStateChanged(UbiGEMWrapper::Structure::GEMCommunicationState communicationState);
    void OnControlStateChanged(UbiGEMWrapper::Structure::GEMControlState controlState);
    void OnControlStateOnlineChangeFailed();
    void OnEquipmentProcessState(unsigned char equipmentProcessState);
    //void OnSpoolStateChanged(UbiGEMWrapper::Structure::GEMSpoolState spoolState);
    void OnVariableUpdateRequest(UbiGEMWrapper::Structure::GEMVariableUpdateType updateType, UbiGEMWrapper::Structure::List<LPCTSTR>* vids);
    void OnUserGEMMessageUpdateRequest(UbiGEMWrapper::Structure::SECSMessage* message);
    void OnTraceDataUpdateRequest(UbiGEMWrapper::Structure::List<LPCTSTR>* vids);

private:
    void InitializeControls();
	CString ConvertDriverResult(UbiGEMWrapper::Structure::GEMResult eResult);
	void UpdateDialogTitle();
	void AddLog(CString strLog);
    void AddLog(CString strLog, UbiGEMWrapper::Structure::GEMResult eResult);
    CString GetGUIString(CComboBox* cboTarget);
    CString GetGUIString(CEdit* edtTarget);
    CString CheckValidationParameterItem(UbiGEMWrapper::Structure::EnhancedCommandParameterItem* enhancedCommandParameterItem, UbiGEMWrapper::Structure::RemoteCommandParameterResult* paramResult);
    void MakePPBody(UbiGEMWrapper::Structure::List<unsigned char>* ppBody);
    void ProcessProgramParsing(bool withoutValue, UbiGEMWrapper::Structure::FmtPPCollection* fmtPpCollection);

    CString SECSFormatAsString(UbiGEMWrapper::Structure::GEMSECSFormat format);
    CString SECSValueAsString(UbiGEMWrapper::Structure::GEMSECSFormat format, UbiGEMWrapper::Structure::SECSValue *value);

public:
	afx_msg void OnMnuOpenUgc();
	afx_msg void OnMnuExit();
	afx_msg void OnMnuInitilaize();
	afx_msg void OnMnuStart();
	afx_msg void OnMnuStop();

public:
    CComboBox m_cboEcv;
    CComboBox m_cboV;
    CComboBox m_cboCe;
    CEdit m_edtLog;
    CComboBox m_cboUserMessage;
    CEdit m_edtECV; 
    CEdit m_edtV;
    CEdit m_edtProcessState;
    CEdit m_edtAlarmId;
    CEdit m_edtTId;
    CEdit m_edtTerminalMessage;
    CEdit m_edtPpid;
    CEdit m_edtFmtPpid;
    CEdit m_edtAbs;
    CEdit m_edtAck;
    afx_msg void OnBnClickedBtnOffline();
    afx_msg void OnBnClickedBtnOnlineLocal();
    afx_msg void OnBnClickedBtnOnlineRemote();
    afx_msg void OnBnClickedBtnEcSet();
    afx_msg void OnBnClickedBtnEcListSet();
    afx_msg void OnBnClickedBtnVSet();
    afx_msg void OnBnClickedBtnVListSet();
    afx_msg void OnBnClickedBtnReportCe1();
    afx_msg void OnBnClickedBtnReportCe2();
    afx_msg void OnBnClickedBtnProcessStateChange();
    afx_msg void OnBnClickedBtnAlarmSet();
    afx_msg void OnBnClickedBtnAlarmClear();
    afx_msg void OnBnClickedBtnTerminalMessageSend();
    afx_msg void OnBnClickedBtnPpRequest();
    afx_msg void OnBnClickedBtnPpSend();
    afx_msg void OnBnClickedBtnPpLoadInquire();
    afx_msg void OnBnClickedBtnPpChanged();
    afx_msg void OnBnClickedBtnFmtPpRequest();
    afx_msg void OnBnClickedBtnFmtPpChanged();
    afx_msg void OnBnClickedBtnFmtPpSendWoValue();
    afx_msg void OnBnClickedBtnFmtPpSend();
    afx_msg void OnBnClickedBtnFmtPpVerificationSend();
    afx_msg void OnBnClickedBtnTimeRequest();
    afx_msg void OnBnClickedBtnLoopback();
    afx_msg void OnBnClickedBtnUserMessageSend();
    afx_msg void OnBnClickedBtnAckApply();
    afx_msg void OnBnClickedBtnClearLog();
    //afx_msg void OnBnClickedOk();
    afx_msg void OnCbnSelchangeCboEc();
};
