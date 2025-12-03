Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Threading
Imports Microsoft.Win32
Imports UbiCom.Net.Structure
Imports UbiGEM.Net.Structure
Imports UbiGEM.Net.Utility.Logger
Imports Ubisam.GEM.Sample.VBnet.ConstatnsEnum

Class MainWindow
    Private Delegate Sub WriteLogDelegate(ByVal logLevel As UbiGEM.Net.Utility.Logger.LogLevel, ByVal strMsg As String)

#Region "Constant"
    Private ReadOnly DATETIME_TEXT_FORMAT As String = "{0} [{1}] {2}" & Environment.NewLine
    Private Const PROGRAM_TITLE_FORMAT As String = "{0} - {1}"
    Private Const PROGRAM_STATUS_FORMAT As String = "{0} - {1}:{2}"
    Private Const UGC_FILE_FILTER As String = "UbiGEM Configuration File (*.ugc)|*.ugc|All files (*.*)|*.*"
    Private Const PROGRAM_DEFAULT_TITLE As String = "UbiSam.GEM.Sample.CSharp"
    Private Const LOG_LINE_MAX_COUNT As Integer = 100
#End Region

    Private Sub mainWindow_Initialized(sender As Object, e As EventArgs) Handles MyBase.Initialized, MyBase.Initialized
        Initialize()
        UpdateTitle()
    End Sub

    Private Sub Initialize()
        _gemDriver = New UbiGEM.Net.Driver.GemDriver()
        _setAlarmList = New List(Of Long)()
    End Sub

    Private Sub _gemDriver_OnCommunicationStateChanged(ByVal communicationState As CommunicationState) Handles _gemDriver.OnCommunicationStateChanged
        WriteLog(LogLevel.Information, String.Format("OnCommunicationStateChanged - {0}", communicationState.ToString()))
    End Sub

    Private Sub _gemDriver_OnControlStateChanged(ByVal controlState As ControlState) Handles _gemDriver.OnControlStateChanged
        WriteLog(LogLevel.Information, String.Format("OnControlStateChanged - {0}", controlState.ToString()))
    End Sub

    Private Sub _gemDriver_OnEquipmentProcessState(ByVal equipmentProcessState As Byte) Handles _gemDriver.OnEquipmentProcessState
        WriteLog(LogLevel.Information, String.Format("OnEquipmentProcessState - {0}", equipmentProcessState.ToString()))
    End Sub

    Private Sub _gemDriver_OnGEMConnected(ByVal ipAddress As String, ByVal portNo As Integer) Handles _gemDriver.OnGEMConnected
        UpdateTitle("Connected", ipAddress, portNo)
    End Sub

    Private Sub _gemDriver_OnGEMSelected(ByVal ipAddress As String, ByVal portNo As Integer) Handles _gemDriver.OnGEMSelected
        UpdateTitle("Selected", ipAddress, portNo)
    End Sub

    Private Sub _gemDriver_OnGEMDeselected(ByVal ipAddress As String, ByVal portNo As Integer) Handles _gemDriver.OnGEMDeselected
        UpdateTitle("Deselected", ipAddress, portNo)
    End Sub

    Private Sub _gemDriver_OnGEMDisconnected(ByVal ipAddress As String, ByVal portNo As Integer) Handles _gemDriver.OnGEMDisconnected
        UpdateTitle("Disconnected", ipAddress, portNo)
    End Sub

    Private Sub _gemDriver_OnControlStateOnlineChangeFailed() Handles _gemDriver.OnControlStateOnlineChangeFailed
        WriteLog(LogLevel.[Error], "OnControlStateOnlineChangeFailed")
    End Sub

    Private Sub _gemDriver_OnReceivedRequestOffline(ByVal systemBytes As UInteger) Handles _gemDriver.OnReceivedRequestOffline
        WriteLog(LogLevel.Information, "Received Request Offline")
        _gemDriver.ReplyRequestOfflineAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedRequestOnline(ByVal systemBytes As UInteger) Handles _gemDriver.OnReceivedRequestOnline
        WriteLog(LogLevel.Information, "Received Request Online")
        _gemDriver.ReplyRequestOnlineAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedDefineReport() Handles _gemDriver.OnReceivedDefineReport
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Define Report")
    End Sub

    Private Sub _gemDriver_OnReceivedLinkEventReport() Handles _gemDriver.OnReceivedLinkEventReport
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received LinkEvent Report")
    End Sub

    Private Sub _gemDriver_OnReceivedEnableDisableEventReport() Handles _gemDriver.OnReceivedEnableDisableEventReport
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Enable Disable Event Send")
    End Sub

    Private Sub _gemDriver_OnReceivedRemoteCommand(ByVal remoteCommandInfo As RemoteCommandInfo) Handles _gemDriver.OnReceivedRemoteCommand
        Dim result As RemoteCommandResult = New RemoteCommandResult()
        Dim paramResult As RemoteCommandParameterResult
        Dim logText As String = String.Empty
        result.HostCommandAck = _ack

        For Each paramInfo As CommandParameterInfo In remoteCommandInfo.CommandParameter.Items
            paramResult = New RemoteCommandParameterResult(paramInfo.Name, CInt(CPACK.IllegalFormatSpecifiedForCPVAL))
            result.Items.Add(paramResult)
        Next

        logText = String.Format("[RemoteCommand={0}]" & Environment.NewLine, remoteCommandInfo.RemoteCommand)

        For Each paramInfo As CommandParameterInfo In remoteCommandInfo.CommandParameter.Items
            logText += String.Format(": [CPNAME={0},Format={1},CPVAL={2}]" & Environment.NewLine, paramInfo.Name, paramInfo.Format, paramInfo.Value)
        Next

        If logText.Length > 0 Then
            logText = logText.Substring(0, logText.Length - Environment.NewLine.Length)
        End If

        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, String.Format("OnReceivedRemoteCommand : {0}", logText))
        _gemDriver.ReplyRemoteCommandAck(remoteCommandInfo, result)
    End Sub

    Private Sub _gemDriver_OnReceivedEnhancedRemoteCommand(ByVal remoteCommandInfo As EnhancedRemoteCommandInfo) Handles _gemDriver.OnReceivedEnhancedRemoteCommand
        Dim result As RemoteCommandResult = New RemoteCommandResult()
        Dim paramResult As RemoteCommandParameterResult
        Dim logText As String = String.Empty
        result.HostCommandAck = _ack
        logText = String.Format("[RemoteCommand={0}]" & Environment.NewLine, remoteCommandInfo.RemoteCommand)

        For Each paramInfo As EnhancedCommandParameterInfo In remoteCommandInfo.EnhancedCommandParameter.Items

            If paramInfo.Format = SECSItemFormat.L Then
                logText += String.Format(": [CPNAME={0},Format={1},Count={2}]" & Environment.NewLine, paramInfo.Name, paramInfo.Format, paramInfo.Items.Count)
                paramResult = New RemoteCommandParameterResult(paramInfo.Name)

                For Each item As EnhancedCommandParameterItem In paramInfo.Items
                    logText += String.Format(":  [CPNAME={0},Format={1},CEPVAL={2}]" & Environment.NewLine, item.Name, item.Format, item.Value)

                    If String.IsNullOrEmpty(item.Name) = True Then
                        paramResult.ParameterListAck.Add(New RemoteCommandParameterResult(CInt(CPACK.IllegalFormatSpecifiedForCPVAL)))
                    Else
                        paramResult.ParameterListAck.Add(New RemoteCommandParameterResult(item.Name, CInt(CPACK.IllegalFormatSpecifiedForCPVAL)))
                    End If

                    CheckValidationParameterItem(item, paramResult)
                Next
            Else
                logText += String.Format(": [CPNAME={0},Format={1},CPVAL={2}]" & Environment.NewLine, paramInfo.Name, paramInfo.Format, paramInfo.Value)
                paramResult = New RemoteCommandParameterResult(paramInfo.Name, CInt(CPACK.IllegalFormatSpecifiedForCPVAL))
            End If

            result.Items.Add(paramResult)
            result.HostCommandAck = CInt(HCACK.ParameterIsInvalid)
        Next

        If logText.Length > 0 Then
            logText = logText.Substring(0, logText.Length - Environment.NewLine.Length)
        End If

        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, String.Format("OnReceivedEnhancedRemoteCommand : {0}", logText))
        _gemDriver.ReplyEnhancedRemoteCommandAck(remoteCommandInfo, result)
    End Sub

    Private Sub _gemDriver_OnReceivedNewECVSend(ByVal systemBytes As UInteger, ByVal newEcInfo As VariableCollection) Handles _gemDriver.OnReceivedNewECVSend
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received New ECV Send")
        _gemDriver.ReplyNewEquipmentConstantSend(systemBytes, newEcInfo, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedEnableDisableAlarmSend() Handles _gemDriver.OnReceivedEnableDisableAlarmSend
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Enable Disable Alarm Send")
    End Sub

    Private Sub _gemDriver_OnReceivedTerminalMessage(ByVal systemBytes As UInteger, ByVal tid As Integer, ByVal terminalMessage As String) Handles _gemDriver.OnReceivedTerminalMessage
        MessageBox.Show(terminalMessage, String.Format("{0} tid : {1}", "Received Terminal Message", tid), MessageBoxButton.OK, MessageBoxImage.Information)
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Terminal Message")
        _gemDriver.ReplyTerminalMessageAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedTerminalMultiMessage(ByVal systemBytes As UInteger, ByVal tid As Integer, ByVal terminalMessages As List(Of String)) Handles _gemDriver.OnReceivedTerminalMultiMessage
        Dim message As String = String.Empty

        For Each terminalMessage As String In terminalMessages
            message += terminalMessage & ","
        Next

        If message.Length > 0 Then
            message = message.Substring(0, message.Length - 1)
        End If

        MessageBox.Show(message, String.Format("{0} tid : {1}", "Received Terminal Multi Message", tid), MessageBoxButton.OK, MessageBoxImage.Information)
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Terminal Multi Message")
        _gemDriver.ReplyTerminalMultiMessageAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedPPRequest(ByVal systemBytes As UInteger, ByVal ppid As String) Handles _gemDriver.OnReceivedPPRequest
        Dim result As Boolean
        Dim ppbody As List(Of Byte) = Nothing
        result = MakePPBody(ppbody)
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received PP Request")
        _gemDriver.ReplyPPRequestAck(systemBytes, ppid, ppbody, result)
    End Sub

    Private Sub _gemDriver_OnReceivedPPSend(ByVal systemBytes As UInteger, ByVal ppid As String, ByVal ppbody As List(Of Byte)) Handles _gemDriver.OnReceivedPPSend
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received PP Send")
        _gemDriver.ReplyPPSendAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedPPLoadInquire(ByVal systemBytes As UInteger, ByVal ppid As String, ByVal length As Integer) Handles _gemDriver.OnReceivedPPLoadInquire
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received PP Load Inquire")
        _gemDriver.ReplyPPLoadInquireAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedDeletePPSend(ByVal systemBytes As UInteger, ByVal ppids As List(Of String)) Handles _gemDriver.OnReceivedDeletePPSend
        If ppids.Count = 0 Then
        Else

            For Each ppid As String In ppids
            Next
        End If

        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Delette PP Send")
        _gemDriver.ReplyPPDeleteAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedFmtPPRequest(ByVal systemBytes As UInteger, ByVal ppid As String) Handles _gemDriver.OnReceivedFmtPPRequest
        Dim result As Boolean = True
        Dim fmtPPCollection As FmtPPCollection = Nothing
        result = ProcessProgramParsing(ppid, False, fmtPPCollection)
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received FMT PP Request")
        _gemDriver.ReplyFmtPPRequestAck(systemBytes, ppid, fmtPPCollection, result)
    End Sub

    Private Sub _gemDriver_OnReceivedFmtPPSend(ByVal systemBytes As UInteger, ByVal fmtPPCollection As FmtPPCollection) Handles _gemDriver.OnReceivedFmtPPSend
        Dim lotText As String = String.Empty
        lotText += String.Format("[PPID={0}]" & Environment.NewLine, fmtPPCollection.PPID)

        For Each ppcodeInfo As FmtPPCCodeInfo In fmtPPCollection.Items
            lotText += String.Format(": [CCODE={0}]" & Environment.NewLine, ppcodeInfo.CommandCode)

            For Each ppitem As FmtPPItem In ppcodeInfo.Items
                lotText += String.Format(":    [PPNAME={0},PPVALUE={1},FORMAT={2}]" & Environment.NewLine, ppitem.PPName, ppitem.PPValue, ppitem.Format)
            Next
        Next

        If lotText.Length > 0 Then
            lotText = lotText.Substring(0, lotText.Length - Environment.NewLine.Length)
        End If

        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, String.Format("OnReceivedFmtPPSend : {0}", lotText))
        _gemDriver.ReplyFmtPPSendAck(systemBytes, _ack)
    End Sub

    Private Sub _gemDriver_OnReceivedCurrentEPPDRequest(ByVal systemBytes As UInteger) Handles _gemDriver.OnReceivedCurrentEPPDRequest
        Dim ppids As List(Of String) = New List(Of String)()
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Current EPPD Request")
        _gemDriver.ReplyCurrentEPPDRequestAck(systemBytes, ppids, True)
    End Sub

    Private Sub _gemDriver_OnReceivedDateTimeRequest(ByVal systemBytes As UInteger) Handles _gemDriver.OnReceivedDateTimeRequest
        Dim timeData As DateTime = DateTime.Now
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Date Time Request")
        _gemDriver.ReplyDateTimeRequest(systemBytes, timeData)
    End Sub

    Private Sub _gemDriver_OnReceivedDateTimeSetRequest(ByVal systemBytes As UInteger, ByVal timeData As DateTime) Handles _gemDriver.OnReceivedDateTimeSetRequest
        MessageBox.Show(timeData.ToString("yyyy-MM-dd HH:mm:ss.fff"), "Received Data Time Set Request", MessageBoxButton.OK, MessageBoxImage.Information)
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Date Time Set Request")
        _gemDriver.ReplyDateTimeSetRequest(systemBytes, _ack, timeData)
    End Sub

    Private Sub _gemDriver_OnReceivedLoopback(ByVal receiveData As List(Of Byte)) Handles _gemDriver.OnReceivedLoopback
        Dim strReceiveData As String = String.Empty

        For Each data As Byte In receiveData
            strReceiveData += data
        Next

        MessageBox.Show(strReceiveData, "Received Loopback", MessageBoxButton.OK, MessageBoxImage.Information)
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Loopback")
    End Sub

    Private Function _gemDriver_OnReceivedEstablishCommunicationsRequest(ByVal mdln As String, ByVal sofRev As String) As Integer Handles _gemDriver.OnReceivedEstablishCommunicationsRequest
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Establish Communication Request")
        Return _ack
    End Function

    Private Sub _gemDriver_OnUserPrimaryMessageReceived(ByVal message As SECSMessage) Handles _gemDriver.OnUserPrimaryMessageReceived
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "User PrimaryMessage Received")
    End Sub

    Private Sub _gemDriver_OnUserSecondaryMessageReceived(ByVal primaryMessage As SECSMessage, ByVal secondaryMessage As SECSMessage) Handles _gemDriver.OnUserSecondaryMessageReceived
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "User SecondaryMessage Received")
    End Sub

    Private Sub _gemDriver_OnReceivedUnknownMessage(ByVal message As SECSMessage) Handles _gemDriver.OnReceivedUnknownMessage
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Unknown Message")
    End Sub

    Private Sub _gemDriver_OnReceivedInvalidRemoteCommand(ByVal remoteCommandInfo As RemoteCommandInfo) Handles _gemDriver.OnReceivedInvalidRemoteCommand
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Invalid Remote Command")
    End Sub

    Private Sub _gemDriver_OnReceivedInvalidEnhancedRemoteCommand(ByVal remoteCommandInfo As EnhancedRemoteCommandInfo) Handles _gemDriver.OnReceivedInvalidEnhancedRemoteCommand
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Received Invalid Enhanced Remote Command")
    End Sub

    Private Sub _gemDriver_OnResponseTerminalRequest(ByVal ack As Integer) Handles _gemDriver.OnResponseTerminalRequest
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response Terminal Request")
    End Sub

    Private Sub _gemDriver_OnResponsePPRequest(ByVal ppid As String, ByVal ppbody As List(Of Byte)) Handles _gemDriver.OnResponsePPRequest
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response PP Request")
    End Sub

    Private Sub _gemDriver_OnResponsePPSend(ByVal ack As Integer, ByVal ppid As String) Handles _gemDriver.OnResponsePPSend
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response PP Send")
    End Sub

    Private Sub _gemDriver_OnResponsePPLoadInquire(ByVal ppgnt As Integer, ByVal ppid As String) Handles _gemDriver.OnResponsePPLoadInquire
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response PP Load Inquire")
    End Sub

    Private Sub _gemDriver_OnResponseFmtPPRequest(ByVal fmtPPCollection As FmtPPCollection) Handles _gemDriver.OnResponseFmtPPRequest
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response FMT PP Request")
    End Sub

    Private Sub _gemDriver_OnResponseFmtPPSend(ByVal ack As Integer, ByVal fmtPPCollection As FmtPPCollection) Handles _gemDriver.OnResponseFmtPPSend
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response FMT PP Send")
    End Sub

    Private Sub _gemDriver_OnResponseFmtPPVerification(ByVal fmtPPVerificationCollection As FmtPPVerificationCollection) Handles _gemDriver.OnResponseFmtPPVerification
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response FMT Verification Ack")
    End Sub

    Private Function _gemDriver_OnResponseDateTimeRequest(ByVal timeData As DateTime) As Boolean Handles _gemDriver.OnResponseDateTimeRequest
        Dim result As Boolean = True
        MessageBox.Show(timeData.ToString("yyyy/MM/dd HH:mm:ss"), "Response DateTime", MessageBoxButton.OK, MessageBoxImage.Information)
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response Date Time Request")
        Return result
    End Function

    Private Sub _gemDriver_OnResponseLoopback(ByVal receiveData As List(Of Byte), ByVal sendData As List(Of Byte)) Handles _gemDriver.OnResponseLoopback
        Dim result As Boolean = False

        If receiveData.Count = sendData.Count Then
            Dim count As Integer = receiveData.Count

            For i As Integer = 0 To count - 1

                If receiveData(i) <> sendData(i) Then
                    result = False
                    Exit For
                End If
            Next
        End If

        If result = True Then
            MessageBox.Show(String.Format("Receive Data : {0}, Send Data : {1}, Result - {2}", receiveData, sendData, result))
        End If

        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Response Loopback")
    End Sub

    Private Sub _gemDriver_OnResponseEventReportAcknowledge(ByVal ceid As String, ByVal ack As Integer) Handles _gemDriver.OnResponseEventReportAcknowledge
        If ceid = "10001" AndAlso ack = CInt(ACKC6.Accepted) Then
        End If
    End Sub

    Private Sub _gemDriver_OnVariableUpdateRequest(ByVal updateType As VariableUpdateType, ByVal variables As List(Of VariableInfo)) Handles _gemDriver.OnVariableUpdateRequest
        If updateType = VariableUpdateType.S1F3SelectedEquipmentStatusRequest Then
            Dim alarmSet As VariableInfo = _gemDriver.Variables(DefinedV.Alarmset)
            Dim alarmID As VariableInfo
            alarmSet.ChildVariables.Clear()

            For Each alid As Long In _setAlarmList
                alarmID = _gemDriver.Variables(DefinedV.ALID).CopyTo()
                alarmID.Value = alid
                alarmID = New VariableInfo() With {
                        .VID = "14",
                        .Name = "ALID",
                        .Format = SECSItemFormat.A,
                        .Length = 1,
                        .Value = alid
                    }
                alarmSet.ChildVariables.Add(alarmID)
            Next
        ElseIf updateType = VariableUpdateType.S6F19IndividualReportRequest Then
        Else
        End If

        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Variable Update Request")
    End Sub

    Private Sub _gemDriver_OnUserGEMMessageUpdateRequest(ByVal message As SECSMessage) Handles _gemDriver.OnUserGEMMessageUpdateRequest
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "GEM Message Update Request")
    End Sub

    Private Sub _gemDriver_OnTraceDataUpdateRequest(ByVal variables As List(Of VariableInfo)) Handles _gemDriver.OnTraceDataUpdateRequest
        WriteLog(UbiGEM.Net.Utility.Logger.LogLevel.Information, "Trace Data Update Request")
    End Sub

    Private Sub _gemDriver_OnWriteLog(ByVal logLevel As UbiGEM.Net.Utility.Logger.LogLevel, ByVal logText As String) Handles _gemDriver.OnWriteLog
        logText = logText.Substring(30)
        logText = logText.Substring(0, logText.Length - 2)
        WriteLog(logLevel, logText)
    End Sub

    Private Sub _gemDriver_OnSECS1Log(ByVal logLevel As UbiGEM.Net.Utility.Logger.LogLevel, ByVal logText As String) Handles _gemDriver.OnSECS1Log
    End Sub

    Private Sub _gemDriver_OnSECS2Log(ByVal logLevel As UbiGEM.Net.Utility.Logger.LogLevel, ByVal logText As String) Handles _gemDriver.OnSECS2Log
        logText = logText.Substring(30)
        logText = logText.Substring(0, logText.Length - 2)
        WriteLog(logLevel, logText)
    End Sub

    Private Sub btnOffline_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _driverResult = _gemDriver.RequestOffline()
    End Sub

    Private Sub btnLocalOnline_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _driverResult = _gemDriver.RequestOnlineLocal()
    End Sub

    Private Sub btnRemoteOnline_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _driverResult = _gemDriver.RequestOnlineRemote()
    End Sub

    Private Sub cbbECID_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim comboBox As ComboBox = TryCast(sender, ComboBox)
        Dim variable As VariableInfo = TryCast(comboBox.SelectedItem, VariableInfo)

        If comboBox IsNot Nothing AndAlso variable IsNot Nothing Then
            Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                        txtECIDValue.Text = _gemDriver.Variables(variable.VID).Value
                                                                    End Sub))
        End If
    End Sub

    Private Sub btnSetECIDValue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If cbbECID.SelectedItem IsNot Nothing Then
            Dim varInfo As VariableInfo = TryCast(cbbECID.SelectedItem, VariableInfo)

            If varInfo IsNot Nothing Then
                _driverResult = _gemDriver.SetEquipmentConstant(varInfo.VID, txtECIDValue.Text)
                WriteLog(LogLevel.Information, String.Format("Set ECID : {0}, Value : {1}, Result : {2}", varInfo.VID, varInfo.Value, _driverResult.ToString()))
            End If
        End If
    End Sub

    Private Sub btnSetECIDListValue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ecids As List(Of String) = New List(Of String)()
        Dim values As List(Of Object) = New List(Of Object)()
        _driverResult = _gemDriver.SetEquipmentConstant(ecids, values)

        If _driverResult <> GemDriverError.Ok Then
            WriteLog(LogLevel.[Error], String.Format("Set ECID Value List Error : {0}", _driverResult.ToString()))
        End If
    End Sub

    Private Sub cbbVID_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim comboBox As ComboBox = TryCast(sender, ComboBox)
        Dim variable As VariableInfo = TryCast(comboBox.SelectedItem, VariableInfo)

        If comboBox IsNot Nothing AndAlso variable IsNot Nothing Then
            Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                        txtVIDValue.Text = _gemDriver.Variables(variable.VID).Value
                                                                    End Sub))
        End If
    End Sub

    Private Sub btnSetVIDValue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If cbbVID.SelectedItem IsNot Nothing Then
            Dim varInfo As VariableInfo = TryCast(cbbVID.SelectedItem, VariableInfo)

            If varInfo IsNot Nothing Then
                _driverResult = _gemDriver.SetVariable(varInfo.VID, txtVIDValue.Text)
                WriteLog(LogLevel.Information, String.Format("Set VID : {0}, Value : {1}, Result : {2}", varInfo.VID, varInfo.Value, _driverResult.ToString()))
            End If
        End If
    End Sub

    Private Sub btnSetVariableList_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim vids As List(Of String) = New List(Of String)()
        Dim values As List(Of Object) = New List(Of Object)()
        _driverResult = _gemDriver.SetVariable(vids, values)

        If _driverResult <> GemDriverError.Ok Then
            WriteLog(LogLevel.[Error], String.Format("Set Variable Value List Error : {0}", _driverResult.ToString()))
        End If
    End Sub

    Private Sub btnCESendDefined_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If cbbCE.SelectedItem IsNot Nothing Then
            Dim selectedItem As KeyValuePair(Of String, CollectionEventInfo) = CType(cbbCE.SelectedItem, KeyValuePair(Of String, CollectionEventInfo))
            Dim ceInfo As CollectionEventInfo = TryCast(selectedItem.Value, CollectionEventInfo)

            If ceInfo IsNot Nothing AndAlso ceInfo.Name <> "EquipmentConstantChanged" AndAlso ceInfo.Name <> "EquipmentConstantChangedbyhost" Then
                _gemDriver.Variables(DefinedV.ControlState).Value = 5
                _driverResult = _gemDriver.ReportCollectionEvent(ceInfo.CEID)
            End If
        End If
    End Sub

    Private Sub btnCESend_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If cbbCE.SelectedItem IsNot Nothing Then
            Dim selectedItem As KeyValuePair(Of String, CollectionEventInfo) = CType(cbbCE.SelectedItem, KeyValuePair(Of String, CollectionEventInfo))
            Dim ceInfo As CollectionEventInfo = New CollectionEventInfo() With {
                    .CEID = selectedItem.Value.CEID,
                    .IsUse = True,
                    .Enabled = True
                }

            If ceInfo IsNot Nothing AndAlso ceInfo.Name <> "EquipmentConstantChanged" AndAlso ceInfo.Name <> "EquipmentConstantChangedbyhost" Then
                Dim rptInfo As ReportInfo
                rptInfo = New ReportInfo() With {
                        .ReportID = "1"
                    }
                rptInfo.Variables.Add(New VariableInfo() With {
                        .Name = "DeviceID",
                        .Format = SECSItemFormat.A,
                        .Value = "0"
                    })
                rptInfo.Variables.Add(New VariableInfo() With {
                        .Name = "ControlState",
                        .Format = SECSItemFormat.U1,
                        .Value = 5
                    })
                ceInfo.Reports.Add(rptInfo)
                _driverResult = _gemDriver.ReportCollectionEvent(ceInfo)
            End If
        End If
    End Sub

    Private Sub btnProcessingStateChange_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim processStete As Byte

        If Byte.TryParse(txtEQPProcessingState.Text, processStete) Then
            _driverResult = _gemDriver.ReportEquipmentProcessingState(processStete)
        End If
    End Sub

    Private Sub btnSetAlarm_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim alarmID As Long

        If Long.TryParse(txtAlarm.Text, alarmID) = True Then
            _driverResult = _gemDriver.ReportAlarmSet(alarmID)

            If _driverResult = GemDriverError.Ok Then
                UpdateSetAlarmList(alarmID, True)
            End If
        End If
    End Sub

    Private Sub btnClearAlarm_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim alarmID As Long

        If Long.TryParse(txtAlarm.Text, alarmID) = True Then
            _driverResult = _gemDriver.ReportAlarmClear(alarmID)

            If _driverResult = GemDriverError.Ok Then
                UpdateSetAlarmList(alarmID, False)
            End If
        End If
    End Sub

    Private Sub UpdateSetAlarmList(ByVal alarmID As Long, ByVal isSet As Boolean)
        If isSet = True Then
            _setAlarmList.Add(alarmID)
        Else
            _setAlarmList.Remove(alarmID)
        End If
    End Sub

    Private Sub btnReportTerminalMessage_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim tid As Integer

        If Integer.TryParse(txtTerminalTID.Text, tid) = True Then
            _gemDriver.ReportTerminalMessage(tid, txtTerminalMessage.Text)
        End If
    End Sub

    Private Sub btnRequestPPRequest_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ppid As String
        ppid = If(String.IsNullOrEmpty(txtPPID.Text) = True, "MGL19SS06MD", txtPPID.Text)
        _gemDriver.RequestPPRequest(ppid)
    End Sub

    Private Sub btnRequestPPSend_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ppid As String
        Dim ppbody As List(Of Byte) = Nothing
        ppid = If(String.IsNullOrEmpty(txtPPID.Text) = True, "MGL19SS06MD", txtPPID.Text)
        MakePPBody(ppbody)
        _driverResult = _gemDriver.RequestPPSend(ppid, ppbody)
    End Sub

    Private Sub btnRequestPPLoadInquire_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ppid As String
        Dim ppbody As List(Of Byte) = Nothing
        ppid = If(String.IsNullOrEmpty(txtPPID.Text) = True, "MGL19SS06MD", txtPPID.Text)
        MakePPBody(ppbody)
        _gemDriver.RequestPPLoadInquire(ppid, ppbody.Count)
    End Sub

    Private Sub btnRequestPPChanged_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ppid As String = String.Empty
        Dim ppstate As Integer = CInt(ProcessProgramChangeState.Credited)
        Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                    ppid = If(String.IsNullOrEmpty(txtPPID.Text) = True, "MGL19SS06MD", txtPPID.Text)
                                                                End Sub))
        _gemDriver.RequestPPChanged(ppstate, ppid)
    End Sub

    Private Function MakePPBody(<Out> ByRef ppbody As List(Of Byte)) As Boolean
        Dim result As Boolean = True
        Dim rand As Random = New Random(Guid.NewGuid().GetHashCode())
        Dim arrPPBody As Byte()
        Dim count As Integer
        ppbody = New List(Of Byte)()
        count = rand.[Next](0, 1000)
        arrPPBody = New Byte(count - 1) {}
        rand.NextBytes(arrPPBody)

        For Each data As Byte In arrPPBody
            ppbody.Add(data)
        Next

        Return result
    End Function

    Private Sub btnRequestFmtPPRequest_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ppid As String = If(String.IsNullOrEmpty(txtFMTPPID.Text) = True, "MGL19SS06MD", txtFMTPPID.Text)
        _gemDriver.RequestFmtPPRequest(ppid)
    End Sub

    Private Sub btnRequestFmtPPChanged_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ppid As String = String.Empty
        Dim fmtPPstate As Integer = CInt(ProcessProgramChangeState.Credited)
        Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                    ppid = If(String.IsNullOrEmpty(txtFMTPPID.Text) = True, "MGL19SS06MD", txtFMTPPID.Text)
                                                                End Sub))
        _gemDriver.RequestFmtPPChanged(fmtPPstate, ppid)
    End Sub

    Private Sub btnRequestFmtPPSendWithoutValue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fmtPPCollection As FmtPPCollection = Nothing
        Dim ppid As String = If(String.IsNullOrEmpty(txtFMTPPID.Text) = True, "MGL19SS06MD", txtFMTPPID.Text)
        ProcessProgramParsing(ppid, True, fmtPPCollection)
        _gemDriver.RequestFmtPPSendWithoutValue(fmtPPCollection)
    End Sub

    Private Sub btnRequestFmtPPSend_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fmtPPCollection As FmtPPCollection = Nothing
        Dim ppid As String = If(String.IsNullOrEmpty(txtFMTPPID.Text) = True, "MGL19SS06MD", txtFMTPPID.Text)
        ProcessProgramParsing(ppid, False, fmtPPCollection)
        _gemDriver.RequestFmtPPSend(fmtPPCollection)
    End Sub

    Private Sub btnRequestFmtPPVerificationSend_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ppid As String = If(String.IsNullOrEmpty(txtFMTPPID.Text) = True, "MGL19SS06MD", txtFMTPPID.Text)
        Dim fmtPPCollection As FmtPPVerificationCollection = New FmtPPVerificationCollection(ppid)
        Dim rand As Random = New Random(Guid.NewGuid().GetHashCode())

        For i As Integer = 0 To 10 - 1
            Dim info As FmtPPVerificationInfo = New FmtPPVerificationInfo()
            info.ACK = _ack
            info.SeqNum = rand.[Next](0, 1000)
            info.ErrW7 = String.Format("ERR{0}", rand.[Next](0, 1000))
            fmtPPCollection.Items.Add(info)
        Next

        _gemDriver.RequestFmtPPVerificationSend(fmtPPCollection)
    End Sub

    Private Function ProcessProgramParsing(ByVal ppid As String, ByVal withoutValue As Boolean, <Out> ByRef fmtPPCollection As FmtPPCollection) As Boolean
        Dim result As Boolean = True
        Dim root As XElement
        Dim element As XElement
        Dim subElement As XElement
        fmtPPCollection = New FmtPPCollection(ppid)

        If ppid = "MGL19SS06MD" Then
            Dim info As FmtPPCCodeInfo

            Try
                root = XElement.Load(System.IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.MyDocuments, "UbiSam\UbiGEM\Samples\Ubisam.GEM.Sample.VBnet\Resources\MGL19SS06MD.xml"))
                element = root.Element("CCodeInfoInfos")

                If element IsNot Nothing Then

                    For Each tempCCodeInfo As XElement In element.Elements("CCodeInfo")
                        info = New FmtPPCCodeInfo()
                        info.CommandCode = If(tempCCodeInfo.Attribute("CommandCode") IsNot Nothing, tempCCodeInfo.Attribute("CommandCode").Value, String.Empty)
                        subElement = tempCCodeInfo.Element("PPItems")

                        If subElement IsNot Nothing Then

                            For Each tempPPARM As XElement In subElement.Elements("PPItem")

                                If withoutValue = True Then
                                    Dim value As String
                                    Dim format As UbiCom.Net.[Structure].SECSItemFormat
                                    value = If(tempPPARM.Attribute("PPValue") IsNot Nothing, tempPPARM.Attribute("PPValue").Value, String.Empty)
                                    format = If(tempPPARM.Attribute("Format").Value IsNot Nothing, (CType([Enum].Parse(GetType(UbiCom.Net.[Structure].SECSItemFormat), tempPPARM.Attribute("Format").Value), UbiCom.Net.[Structure].SECSItemFormat)), UbiCom.Net.[Structure].SECSItemFormat.A)
                                    info.Add(value, format)
                                Else
                                    Dim name As String
                                    Dim value As String
                                    Dim format As UbiCom.Net.[Structure].SECSItemFormat
                                    name = If(tempPPARM.Attribute("PPName") IsNot Nothing, tempPPARM.Attribute("PPName").Value, String.Empty)
                                    value = If(tempPPARM.Attribute("PPValue") IsNot Nothing, tempPPARM.Attribute("PPValue").Value, String.Empty)
                                    format = If(tempPPARM.Attribute("Format").Value IsNot Nothing, (CType([Enum].Parse(GetType(UbiCom.Net.[Structure].SECSItemFormat), tempPPARM.Attribute("Format").Value), UbiCom.Net.[Structure].SECSItemFormat)), UbiCom.Net.[Structure].SECSItemFormat.A)
                                    info.Add(name, value, format)
                                End If
                            Next
                        End If

                        fmtPPCollection.Items.Add(info)
                    Next
                End If

            Catch ex As Exception
                result = False
            Finally
                root = Nothing
                element = Nothing
            End Try
        End If

        Return result
    End Function

    Private Sub btnRequestDateTime_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _driverResult = _gemDriver.RequestDateTime()
    End Sub

    Private Sub btnRequestLoopback_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim byteList As List(Of Byte) = New List(Of Byte)()

        If txtLoopbackValue.Text IsNot String.Empty Then
            Dim temp As String() = txtLoopbackValue.Text.Split(" "c)

            For Each byteValue As String In temp
                byteList.Add(Byte.Parse(byteValue))
            Next

            _driverResult = _gemDriver.RequestLoopback(byteList)
        End If
    End Sub

    Private Sub btnUserMessageSend_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If cbbUserMessage.SelectedItem IsNot Nothing Then
            Dim selectedItem As KeyValuePair(Of String, SECSMessage) = CType(cbbUserMessage.SelectedItem, KeyValuePair(Of String, SECSMessage))
            Dim message As SECSMessage = selectedItem.Value

            If message IsNot Nothing Then
                message.Body.Clear()
                '아래 형식의 User Defined Message를 송신하고자 할 경우
                ' <L, 2
                '     <B, 1 '2'>
                '     <A, 2 'OK'>
                ' >

                message.Body.Add(SECSItemFormat.L, 2, Nothing)
                message.Body.Add(SECSItemFormat.B, 1, 2)
                message.Body.Add(SECSItemFormat.A, 2, "OK")

                _gemDriver.SendSECSMessage(message)
            End If
        End If
    End Sub


    Private Sub btnAckApply_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _ack = GetAckValue()
    End Sub

    Private Function GetAckValue() As Integer
        Dim convertValue As Integer = 0
        Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                    Integer.TryParse(txtAck.Text, convertValue)
                                                                End Sub))
        Return convertValue
    End Function

#Region "Other"
    Private Sub btnLogClear_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        txtLogs.Document.Blocks.Clear()
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub txtNumber_PreviewKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
        If Not (((Key.D0 <= e.Key) AndAlso (e.Key <= Key.D9)) OrElse ((Key.NumPad0 <= e.Key) AndAlso (e.Key <= Key.NumPad9)) OrElse e.Key = Key.Back) Then
            e.Handled = True
        End If
    End Sub

    Private Sub mainWindow_Closed(ByVal sender As Object, ByVal e As EventArgs)
        _gemDriver.[Stop]()
        _gemDriver.Dispose()
    End Sub

    Private Sub WriteLog(ByVal logLevel As LogLevel, ByVal logText As String)
        Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                    Dim lineCount As Integer

                                                                    If txtLogs IsNot Nothing Then
                                                                        lineCount = GetLineCount()

                                                                        If LOG_LINE_MAX_COUNT <= lineCount Then
                                                                            txtLogs.Document.Blocks.Remove(txtLogs.Document.Blocks.FirstBlock)
                                                                        End If

                                                                        Dim tr As TextRange = New TextRange(txtLogs.Document.ContentEnd, txtLogs.Document.ContentEnd)
                                                                        tr.Text = String.Format(DATETIME_TEXT_FORMAT, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), logLevel.ToString(), logText)

                                                                        Try
                                                                            txtLogs.ScrollToEnd()
                                                                        Catch
                                                                        End Try
                                                                    End If
                                                                End Sub))
    End Sub

    Private Function GetLineCount() As Integer
        Dim lineCount As Integer

        If String.IsNullOrWhiteSpace(GetAsText()) Then
            Return 0
        End If

        lineCount = Regex.Matches(GetAsRTF(), Regex.Escape("\par")).Count - 1
        Return lineCount
    End Function

    Private Function GetAsText() As String
        Return New TextRange(txtLogs.Document.ContentStart, txtLogs.Document.ContentEnd).Text
    End Function

    Private Function GetAsRTF() As String
        Using memoryStream As MemoryStream = New MemoryStream()
            Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                        Dim textRange As TextRange = New TextRange(txtLogs.Document.ContentStart, txtLogs.Document.ContentEnd)
                                                                        textRange.Save(memoryStream, DataFormats.Rtf)
                                                                        memoryStream.Seek(0, SeekOrigin.Begin)
                                                                    End Sub))

            Using streamReader As StreamReader = New StreamReader(memoryStream)
                Return streamReader.ReadToEnd()
            End Using
        End Using
    End Function

    Private Sub UpdateTitle()
        Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()

                                                                    If String.IsNullOrEmpty(_ugcFileName) = True Then
                                                                        Title = PROGRAM_DEFAULT_TITLE
                                                                    Else
                                                                        Title = String.Format(PROGRAM_TITLE_FORMAT, PROGRAM_DEFAULT_TITLE, _ugcFileName)
                                                                    End If
                                                                End Sub))
    End Sub

    Private Sub UpdateTitle(ByVal connectionState As String, ByVal ipAddress As String, ByVal portNo As Integer)
        Dispatcher.Invoke(DispatcherPriority.Normal, New Action(Sub()
                                                                    Title = String.Format(PROGRAM_TITLE_FORMAT, PROGRAM_DEFAULT_TITLE, _ugcFileName) & " - " & String.Format(PROGRAM_STATUS_FORMAT, connectionState, ipAddress, portNo)
                                                                End Sub))
    End Sub

    Private Function CheckValidationParameterItem(ByVal enhancedCommandParameterItem As EnhancedCommandParameterItem, ByVal paramResult As RemoteCommandParameterResult) As String
        Dim logText As String = String.Empty

        If enhancedCommandParameterItem.Format = SECSItemFormat.L Then

            For Each item As EnhancedCommandParameterItem In enhancedCommandParameterItem.ChildParameterItem.Items
                logText += String.Format(":  [CPNAME={0},Format={1},CEPVAL={2}]" & Environment.NewLine, item.Name, item.Format, item.Value)

                If String.IsNullOrEmpty(item.Name) = True Then
                    paramResult.ParameterListAck.Add(New RemoteCommandParameterResult(CInt(CPACK.IllegalFormatSpecifiedForCPVAL)))
                Else
                    paramResult.ParameterListAck.Add(New RemoteCommandParameterResult(item.Name, CInt(CPACK.IllegalFormatSpecifiedForCPVAL)))
                End If

                logText += CheckValidationParameterItem(item, paramResult)
            Next
        End If

        Return logText
    End Function
#End Region

#Region "Menu"
    Private Sub btnOepnUGC_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim openFileDialog As OpenFileDialog = New OpenFileDialog() With {
                .Filter = UGC_FILE_FILTER
            }

        If openFileDialog.ShowDialog() = True Then
            _ugcFileName = openFileDialog.FileName
            UpdateTitle()
        End If
    End Sub

    Private Sub btnExit_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub btnInitialize_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim errorText As String = Nothing
        _driverResult = _gemDriver.Initialize(_ugcFileName, errorText)

        If _driverResult <> GemDriverError.Ok Then
            WriteLog(LogLevel.[Error], String.Format("Driver Inialize - Result : {0}, Text : {1}", _driverResult.ToString(), errorText))
        Else
            cbbECID.ItemsSource = _gemDriver.Variables.ECV.Items.Where(Function(t) t.Format <> UbiCom.Net.[Structure].SECSItemFormat.L).ToList()
            cbbVID.ItemsSource = _gemDriver.Variables.Variables.Items.Where(Function(t) t.Format <> UbiCom.Net.[Structure].SECSItemFormat.L).ToList()
            cbbCE.ItemsSource = _gemDriver.CollectionEvents.Items
            cbbUserMessage.ItemsSource = _gemDriver.UserMessage.MessageInfo
        End If
    End Sub

    Private Sub btnStart_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _driverResult = _gemDriver.Start()

        If _driverResult <> GemDriverError.Ok Then
            WriteLog(LogLevel.[Error], String.Format("Driver Start - Result : {0}", _driverResult.ToString()))
        End If
    End Sub

    Private Sub btnStop_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _gemDriver.[Stop]()
    End Sub
#End Region

#Region "Variables"
    Private WithEvents _gemDriver As UbiGEM.Net.Driver.GemDriver
    Private _driverResult As GemDriverError
    Private _setAlarmList As List(Of Long)
    Private _ack As Integer
    Private _ugcFileName As String
#End Region
End Class