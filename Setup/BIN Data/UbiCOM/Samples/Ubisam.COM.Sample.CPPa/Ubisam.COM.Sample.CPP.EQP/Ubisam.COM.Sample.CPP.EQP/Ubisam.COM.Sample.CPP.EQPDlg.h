
// Ubisam.COM.Sample.CPP.EQPDlg.h : 헤더 파일
//

#pragma once

#include <UbiCom\CPP\Structure.h>
#include <UbiCom\CPP\Wrapper.h>


// CUbisamCOMSampleCPPEQPDlg 대화 상자
class CUbisamCOMSampleCPPEQPDlg : public CDialogEx, UbiCom::CPP::IHSMSDriverEvents
{
// 생성입니다.
public:
	CUbisamCOMSampleCPPEQPDlg(CWnd* pParent = NULL);	// 표준 생성자입니다.

// 대화 상자 데이터입니다.
	enum { IDD = IDD_UBISAMCOMSAMPLECPPEQP_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 지원입니다.


private:
	UbiCom::CPP::HSMSDriver* m_pDriver;
	CString m_strUMDFilePath;
	CString m_strUCFFilePath;
	CString m_strLogPath;

	void OnReceivedInvalidPrimaryMessageEventHandler(UbiCom::CPP::SECSMessageValidationError reason, UbiCom::CPP::SECSMessage* message);
	void OnReceivedInvalidSecondaryMessageEventHandler(UbiCom::CPP::SECSMessageValidationError reason, UbiCom::CPP::SECSMessage* primaryMessage, UbiCom::CPP::SECSMessage* secondaryMessage);
	void OnSentControlMessageEventHandler(UbiCom::CPP::SECSMessage* message);
	void OnSentSECSMessageEventHandler(UbiCom::CPP::SECSMessage* message);
	void OnReceivedControlMessageEventHandler(UbiCom::CPP::SECSMessage* message);
	void OnReceivedPrimaryMessageEventHandler(UbiCom::CPP::SECSMessage* message);
	void OnReceivedSecondaryMessageEventHandler(UbiCom::CPP::SECSMessage* primaryMessage, UbiCom::CPP::SECSMessage* secondaryMessage);
	void OnReceivedUnknownMessageEventHandler(UbiCom::CPP::SECSMessage* message);
	void OnTimeoutEventHandler(UbiCom::CPP::SECSTimeoutType timeoutType);
	void OnT3TimeoutEventHandler(UbiCom::CPP::SECSMessage* message);

	void OnSECSConnectedEventHandler(tstring& ipAddress, int portNo);
	void OnSECSDisconnectedEventHandler(tstring& ipAddress, int portNo);
	void OnSECSSelectedEventHandler(tstring& ipAddress, int portNo);
	void OnSECSDeselectedEventHandler(tstring& ipAddress, int portNo);
	void OnSECS1WriteLogEventHandler(UbiCom::CPP::SECSLogLevel logLevel, tstring& logText);
	void OnSECS2WriteLogEventHandler(UbiCom::CPP::SECSLogLevel logLevel, tstring& logText);

	CString ConvertToCString(UbiCom::CPP::SECSLogLevel logLevel);
	CString ConvertToCString(tstring& str);
	CString ConvertToCString(UbiCom::CPP::SECSTimeoutType timeoutType);
	CString ConvertToCString(UbiCom::CPP::SECSMessageValidationError error);
	CString ConvertToCString(UbiCom::CPP::SECSDriverError error);
	CString ConvertToCString(UbiCom::CPP::SECSMessageError error);

	void AddLog(CString& logText, bool withoutTime = true);

	CString GetContainingFolder(CString& filePath);
	void ChangeUCFFilePath(CString& filePath);
	void ChangeUMDFilePath(CString& filePath);

	CString ConvertSECSItemValueToCString(UbiCom::CPP::SECSItem* item);

// 구현입니다.
protected:
	HICON m_hIcon;

	// 생성된 메시지 맵 함수
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()

private:
	CEdit m_txtDeviceID;
	CButton m_radEQP;
	CButton m_radHost;
	CEdit m_txtDriverNameConfig;
	CEdit m_txtLogPath;
	CButton m_btnBrowseLogPath;
	CEdit m_txtUMDFile;
	CButton m_btnBrowseUMDFile;
	CButton m_radPassive;
	CButton m_radActive;
	CEdit m_txtIPAddress;
	CEdit m_txtPortNo;
	CButton m_btnInitConfig;
	CEdit m_txtUCFFile;
	CButton m_btnBrowseUCFFile;
	CEdit m_txtDriverNameUCF;
	CButton m_btnInitUCF;
	CEdit m_txtLog;

	uint32_t m_uLastReceivedS1F1SystemByte;
public:
	afx_msg void OnBnClickedBrowseLogPath();
	afx_msg void OnBnClickedBrowseUmdFile();
	afx_msg void OnBnClickedInitConfig();
	afx_msg void OnBnClickedBrowseUcfFile();
	afx_msg void OnBnClickedInitUcfFile();
	afx_msg void OnBnClickedClearLog();
	afx_msg void OnBnClickedExit();
	afx_msg void OnBnClickedConnectionOpen();
	afx_msg void OnBnClickedConnectionClose();
	afx_msg void OnBnClickedSendS1f1();
	afx_msg void OnBnClickedReplyS1f2();
	afx_msg void OnBnClickedSendS6f11();

private:
	void AnalyzeS2F41(UbiCom::CPP::SECSMessage* message);

};
