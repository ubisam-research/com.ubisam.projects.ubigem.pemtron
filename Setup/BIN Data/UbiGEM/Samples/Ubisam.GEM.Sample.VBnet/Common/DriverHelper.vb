#Region "DefinedCE"
Public Class DefinedCE
    ''' <summary>
    ''' Offline
    ''' </summary>
    Public Const Offline As String = "1"
    ''' <summary>
    ''' Alarm Set
    ''' </summary>
    Public Const AlarmSet As String = "10"
    ''' <summary>
    ''' Alarm Clear
    ''' </summary>
    Public Const AlarmClear As String = "11"
    ''' <summary>
    ''' Limit Monitoring
    ''' </summary>
    Public Const LimitMonitoring As String = "12"
    ''' <summary>
    ''' Offline On Host
    ''' </summary>
    Public Const OfflineOnHost As String = "2"
    ''' <summary>
    ''' Online Local
    ''' </summary>
    Public Const OnlineLocal As String = "3"
    ''' <summary>
    ''' Online Remote
    ''' </summary>
    Public Const OnlineRemote As String = "4"
    ''' <summary>
    ''' Control State Changed
    ''' </summary>
    Public Const ControlStateChanged As String = "5"
    ''' <summary>
    ''' Equipment Constant Changed
    ''' </summary>
    Public Const EquipmentConstantChanged As String = "6"
    ''' <summary>
    ''' Equipment Constant Changed(HOST-S2F15)
    ''' </summary>
    Public Const EquipmentConstantChangedByHost As String = "7"
    ''' <summary>
    ''' Process Program Changed
    ''' </summary>
    Public Const ProcessProgramChanged As String = "8"
    ''' <summary>
    ''' Process State Changed
    ''' </summary>
    Public Const ProcessStateChanged As String = "9"
End Class
#End Region

#Region "DefinedEC"
Public Class DefinedEC
    Public Const InitCommunicationState As String = "101"
    ''' <summary>
    ''' The length of time, in seconds, of the interval between attempts to send S1F13 when establishing communications.
    ''' </summary>
    Public Const EstablishCommunicationsTimeout As String = "102"
    Public Const AreYouThereTimeout As String = "103"
    ''' <summary>
    ''' The setting of this ECV controls whether the equipment shall use use the variable item CLOCK and the data items STIME, TIMESTAMP, and TIME in  12-byte, 16-byte, or Extended format.
    ''' </summary>
    Public Const TimeFormat As String = "104"
    Public Const InitControlState As String = "105"
    Public Const OffLineSubState As String = "106"
    Public Const OnLineFailState As String = "107"
    Public Const OnLineSubState As String = "108"
    Public Const DeviceID As String = "109"
    Public Const IPAddress As String = "110"
    Public Const PortNumber As String = "111"
    Public Const ActiveMode As String = "112"
    Public Const T3Timeout As String = "113"
    Public Const T5Timeout As String = "114"
    Public Const T6Timeout As String = "115"
    Public Const T7Timeout As String = "116"
    Public Const T8Timeout As String = "117"
    Public Const LinkTestInterval As String = "118"
    Public Const HeartbeatInterval As String = "119"
End Class
#End Region

#Region "DefinedV"
Public Class DefinedV
    Public Const Clock As String = "1"
    Public Const PPChangeName As String = "10"
    Public Const PPChangeStatus As String = "11"
    Public Const Alarmset As String = "12"
    Public Const PPStateChangedInfo As String = "13"
    Public Const ALID As String = "14"
    Public Const ControlState As String = "2"
    Public Const ProcessState As String = "3"
    Public Const PreviousProcessState As String = "4"
    Public Const ChangedECID As String = "5"
    Public Const ChangedECV As String = "6"
    ''' <summary>
    ''' L2(ECID,ECV) List
    ''' </summary>
    Public Const ChangedECList As String = "7"
    Public Const MDLN As String = "8"
    Public Const SOFTREV As String = "9"
End Class
#End Region

#Region "DefinedDataDictinary"
Public Class DefinedDataDictinary
    ''' <summary>
    ''' Any binary string.
    ''' </summary>
    Public Const ABS As String = "ABS"
    ''' <summary>
    ''' Load Port Access Mode. Possible values are.
    ''' </summary>
    Public Const ACCESSMODE As String = "ACCESSMODE"
    ''' <summary>
    ''' After Command Codes.
    ''' </summary>
    Public Const ACDS As String = "ACDS"
    ''' <summary>
    ''' Indicates success of a request.
    ''' </summary>
    Public Const ACKA As String = "ACKA"
    ''' <summary>
    ''' Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const ACKC10 As String = "ACKC10"
    ''' <summary>
    ''' Return code for secondary messages 1 byte.
    ''' </summary>
    Public Const ACKC13 As String = "ACKC13"
    ''' <summary>
    ''' Return code for secondary messages, 1 byte.
    ''' </summary>
    Public Const ACKC15 As String = "ACKC15"
    ''' <summary>
    ''' Acknowledge code, 1 byte.
    ''' </summary>
    Public Const ACKC3 As String = "ACKC3"
    ''' <summary>
    ''' Acknowledge code, 1 byte.
    ''' </summary>
    Public Const ACKC5 As String = "ACKC5"
    ''' <summary>
    ''' Acknowledge code, 1 byte.
    ''' </summary>
    Public Const ACKC6 As String = "ACKC6"
    ''' <summary>
    ''' Acknowledge code, 1 byte.
    ''' </summary>
    Public Const ACKC7 As String = "ACKC7"
    ''' <summary>
    ''' Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const ACKC7A As String = "ACKC7A"
    ''' <summary>
    ''' .
    ''' </summary>
    Public Const AGENT As String = "AGENT"
    ''' <summary>
    ''' Alarm code byte.
    ''' </summary>
    Public Const ALCD As String = "ALCD"
    ''' <summary>
    ''' Alarm enable/disable code, 1 byte.
    ''' </summary>
    Public Const ALED As String = "ALED"
    ''' <summary>
    ''' Alarm identification.
    ''' </summary>
    Public Const ALID As String = "ALID"
    ''' <summary>
    ''' Alarm text limited to 120 characters.
    ''' </summary>
    Public Const ALTX As String = "ALTX"
    ''' <summary>
    ''' Contains a specific attribute value for a specific object.
    ''' </summary>
    Public Const ATTRDATA As String = "ATTRDATA"
    ''' <summary>
    ''' Identifier for an attribute for a specific type of object.
    ''' </summary>
    Public Const ATTRID As String = "ATTRID"
    ''' <summary>
    ''' The relationship that a specified qualifying value has to the value of an attribute of an object instance (the value of interest).
    ''' </summary>
    Public Const ATTRRELN As String = "ATTRRELN"
    ''' <summary>
    ''' A flag which enables or disables the Auto Clear function.
    ''' </summary>
    Public Const AUTOCLEAR As String = "AUTOCLEAR"
    ''' <summary>
    ''' A function that equipment closes the session automatically when operator access doesn’t occur exceeding the predefined maximum time.
    ''' </summary>
    Public Const AUTOCLOSE As String = "AUTOCLOSE"
    ''' <summary>
    ''' Before Command Codes.
    ''' </summary>
    Public Const BCDS As String = "BCDS"
    ''' <summary>
    ''' Bin code equivalents.
    ''' </summary>
    Public Const BCEQU As String = "BCEQU"
    ''' <summary>
    ''' The Bin List.
    ''' </summary>
    Public Const BINLT As String = "BINLT"
    ''' <summary>
    ''' Block Definition.
    ''' </summary>
    Public Const BLKDEF As String = "BLKDEF"
    ''' <summary>
    ''' Boot program Data.
    ''' </summary>
    Public Const BPD As String = "BPD"
    ''' <summary>
    ''' Byte Maximum.
    ''' </summary>
    Public Const BYTMAX As String = "BYTMAX"
    ''' <summary>
    ''' Carrier Action Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const CAACK As String = "CAACK"
    ''' <summary>
    ''' Specifies the action requested for a carrier.
    ''' </summary>
    Public Const CARRIERACTION As String = "CARRIERACTION"
    ''' <summary>
    ''' The identifier of a carrier.
    ''' </summary>
    Public Const CARRIERID As String = "CARRIERID"
    ''' <summary>
    ''' The object specifier for a carrier. Conforms to OBJSPEC.
    ''' </summary>
    Public Const CARRIERSPEC As String = "CARRIERSPEC"
    ''' <summary>
    ''' The value of a carrier attribute.
    ''' </summary>
    Public Const CATTRDATA As String = "CATTRDATA"
    ''' <summary>
    ''' The name of a carrier attribute.
    ''' </summary>
    Public Const CATTRID As String = "CATTRID"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const CCACK As String = "CCACK"
    ''' <summary>
    ''' Command Code.
    ''' </summary>
    Public Const CCODE As String = "CCODE"
    ''' <summary>
    ''' Collection event or trace enable/disable code, 1 byte.
    ''' </summary>
    Public Const CEED As String = "CEED"
    ''' <summary>
    ''' Collected event ID.
    ''' </summary>
    Public Const CEID As String = "CEID"
    ''' <summary>
    ''' Collection event name.
    ''' </summary>
    Public Const CENAME As String = "CENAME"
    ''' <summary>
    ''' Command Enhanced Parameter Acknowledge.
    ''' </summary>
    Public Const CEPACK As String = "CEPACK"
    ''' <summary>
    ''' Command Enhanced Parameter Value.
    ''' </summary>
    Public Const CEPVAL As String = "CEPVAL"
    ''' <summary>
    ''' User defined.
    ''' </summary>
    Public Const CHKINFO As String = "CHKINFO"
    ''' <summary>
    ''' Checkpoint as defined by the sending system.
    ''' </summary>
    Public Const CKPNT As String = "CKPNT"
    ''' <summary>
    ''' Information if the session is closed properly.
    ''' </summary>
    Public Const CLSSTS As String = "CLSSTS"
    ''' <summary>
    ''' Command acknowledge code.
    ''' </summary>
    Public Const CMDA As String = "CMDA"
    ''' <summary>
    ''' Command Maximum.
    ''' </summary>
    Public Const CMDMAX As String = "CMDMAX"
    ''' <summary>
    ''' Command Name = 16 characters.
    ''' </summary>
    Public Const CNAME As String = "CNAME"
    ''' <summary>
    ''' Column count in die increments.
    ''' </summary>
    Public Const COLCT As String = "COLCT"
    ''' <summary>
    ''' Text description of contents of TBLELT. 1?20 characters.
    ''' </summary>
    Public Const COLHDR As String = "COLHDR"
    ''' <summary>
    ''' Establish Communications Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const COMMACK As String = "COMMACK"
    ''' <summary>
    ''' Choice of available operators that compare the supplied value to the current attribute value. Evaluated as ‘Current value XX supplied value’ where XX is one of the enumerated values  (e.g., ‘GT’).
    ''' </summary>
    Public Const COMPARISONOPERATOR As String = "COMPARISONOPERATOR"
    ''' <summary>
    ''' Provides condition information for a subsystem component. Used in the data item in the CONDITIONLIST.
    ''' </summary>
    Public Const CONDITION As String = "CONDITION"
    ''' <summary>
    ''' A list of CONDITION data sent in a fixed order. CONDITIONLIST has the following form.
    ''' </summary>
    Public Const CONDITIONLIST As String = "CONDITIONLIST"
    ''' <summary>
    ''' Command Parameter Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const CPACK As String = "CPACK"
    ''' <summary>
    ''' Command Parameter Name.
    ''' </summary>
    Public Const CPNAME As String = "CPNAME"
    ''' <summary>
    ''' Command Parameter Value.
    ''' </summary>
    Public Const CPVAL As String = "CPVAL"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const CRAACK As String = "CRAACK"
    ''' <summary>
    ''' Information concerning the result of the event.
    ''' </summary>
    Public Const CRAEACK As String = "CRAEACK"
    ''' <summary>
    ''' Equipment Acknowledgement code,  1 byte.
    ''' </summary>
    Public Const CSAACK As String = "CSAACK"
    ''' <summary>
    ''' Control Job command codes are assigned as follows.
    ''' </summary>
    Public Const CTLJOBCMD As String = "CTLJOBCMD"
    ''' <summary>
    ''' Identifier for Control Job. Conforms to OBJID.
    ''' </summary>
    Public Const CTLJOBID As String = "CTLJOBID"
    ''' <summary>
    ''' A vector or string of unformatted data.
    ''' </summary>
    Public Const DATA As String = "DATA"
    ''' <summary>
    ''' Acknowledge code for data.
    ''' </summary>
    Public Const DATAACK As String = "DATAACK"
    ''' <summary>
    ''' Data ID.
    ''' </summary>
    Public Const DATAID As String = "DATAID"
    ''' <summary>
    ''' Total bytes to be sent.
    ''' </summary>
    Public Const DATALENGTH As String = "DATALENGTH"
    ''' <summary>
    ''' Used to identify the data requested.
    ''' </summary>
    Public Const DATASEG As String = "DATASEG"
    ''' <summary>
    ''' Object type for Data Source Objects.
    ''' </summary>
    Public Const DATASRC As String = "DATASRC"
    ''' <summary>
    ''' Data location.
    ''' </summary>
    Public Const DATLC As String = "DATLC"
    ''' <summary>
    ''' Status response for the Delete PDE request.
    ''' </summary>
    Public Const DELRSPSTAT As String = "DELRSPSTAT"
    ''' <summary>
    ''' Status response for the GET PDE Directory request.
    ''' </summary>
    Public Const DIRRSPSTAT As String = "DIRRSPSTAT"
    ''' <summary>
    ''' Define Report Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const DRACK As String = "DRACK"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const DRRACK As String = "DRRACK"
    ''' <summary>
    ''' Data set ID.
    ''' </summary>
    Public Const DSID As String = "DSID"
    ''' <summary>
    ''' The name of the Data Set.
    ''' </summary>
    Public Const DSNAME As String = "DSNAME"
    ''' <summary>
    ''' Data sample period. DSPER has two allowable formats.
    ''' </summary>
    Public Const DSPER As String = "DSPER"
    ''' <summary>
    ''' Die Units of Measure.
    ''' </summary>
    Public Const DUTMS As String = "DUTMS"
    ''' <summary>
    ''' Data value name.
    ''' </summary>
    Public Const DVNAME As String = "DVNAME"
    ''' <summary>
    ''' Data value.
    ''' </summary>
    Public Const DVVAL As String = "DVVAL"
    ''' <summary>
    ''' A descriptive name for the data variable.
    ''' </summary>
    Public Const DVVALNAME As String = "DVVALNAME"
    ''' <summary>
    ''' Equipment acknowledge code, 1 byte.
    ''' </summary>
    Public Const EAC As String = "EAC"
    ''' <summary>
    ''' Equipment constant default value.
    ''' </summary>
    Public Const ECDEF As String = "ECDEF"
    ''' <summary>
    ''' Equipment Constant ID.
    ''' </summary>
    Public Const ECID As String = "ECID"
    ''' <summary>
    ''' Equipment constant maximum value.
    ''' </summary>
    Public Const ECMAX As String = "ECMAX"
    ''' <summary>
    ''' Equipment constant minimum value.
    ''' </summary>
    Public Const ECMIN As String = "ECMIN"
    ''' <summary>
    ''' Equipment constant name.
    ''' </summary>
    Public Const ECNAME As String = "ECNAME"
    ''' <summary>
    ''' Equipment Constant Value.
    ''' </summary>
    Public Const ECV As String = "ECV"
    ''' <summary>
    ''' Expected data Identification.
    ''' </summary>
    Public Const EDID As String = "EDID"
    ''' <summary>
    ''' Equivalent material ID  (16 bytes maximum).
    ''' </summary>
    Public Const EMID As String = "EMID"
    ''' <summary>
    ''' Executive program data.
    ''' </summary>
    Public Const EPD As String = "EPD"
    ''' <summary>
    ''' Identifier that indicates equipment which the recipe is tuned for.
    ''' </summary>
    Public Const EQID As String = "EQID"
    ''' <summary>
    ''' A unique ASCII equipment identifier assigned by the factory to the equipment. Limited to a maximum of 80 characters.
    ''' </summary>
    Public Const EQNAME As String = "EQNAME"
    ''' <summary>
    ''' Enable/Disable Event Report. Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const ERACK As String = "ERACK"
    ''' <summary>
    ''' Response component for single recipe check.
    ''' </summary>
    Public Const ERCACK As String = "ERCACK"
    ''' <summary>
    ''' Code identifying an error.
    ''' </summary>
    Public Const ERRCODE As String = "ERRCODE"
    ''' <summary>
    ''' Text string describing the error noted in the corresponding ERRCODE. Limited to 120 characters maximum.
    ''' </summary>
    Public Const ERRTEXT As String = "ERRTEXT"
    ''' <summary>
    ''' Text string describing error found in process program.
    ''' </summary>
    Public Const ERRW7 As String = "ERRW7"
    ''' <summary>
    ''' Response component for single recipe transfer.
    ''' </summary>
    Public Const ERXACK As String = "ERXACK"
    ''' <summary>
    ''' Object type for Event Source Objects.
    ''' </summary>
    Public Const EVNTSRC As String = "EVNTSRC"
    ''' <summary>
    ''' Unique identifier for the exception. Maximum length of 20 characters.
    ''' </summary>
    Public Const EXID As String = "EXID"
    ''' <summary>
    ''' Text which describes the nature of the exception.
    ''' </summary>
    Public Const EXMESSAGE As String = "EXMESSAGE"
    ''' <summary>
    ''' Text which specifies a recovery action for an exception. Maximum length of 40 bytes.
    ''' </summary>
    Public Const EXRECVRA As String = "EXRECVRA"
    ''' <summary>
    ''' Text which identifies the type of an exception. It is usually a single word of text.
    ''' </summary>
    Public Const EXTYPE As String = "EXTYPE"
    ''' <summary>
    ''' Function Identification.
    ''' </summary>
    Public Const FCNID As String = "FCNID"
    ''' <summary>
    ''' Film Frame Rotation.
    ''' </summary>
    Public Const FFROT As String = "FFROT"
    ''' <summary>
    ''' Data from the Data Set.
    ''' </summary>
    Public Const FILDAT As String = "FILDAT"
    ''' <summary>
    ''' Flat/Notch Location.
    ''' </summary>
    Public Const FNLOC As String = "FNLOC"
    ''' <summary>
    ''' Formatted Process Program Length.
    ''' </summary>
    Public Const FRMLEN As String = "FRMLEN"
    ''' <summary>
    ''' Status response for the Get PDE and Get PDEheader requests.
    ''' </summary>
    Public Const GETRSPSTAT As String = "GETRSPSTAT"
    ''' <summary>
    ''' Grant code, 1 byte.
    ''' </summary>
    Public Const GRANT As String = "GRANT"
    ''' <summary>
    ''' Permission to send, 1 byte.
    ''' </summary>
    Public Const GRANT6 As String = "GRANT6"
    ''' <summary>
    ''' Grant code, 1 byte.
    ''' </summary>
    Public Const GRNT1 As String = "GRNT1"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const GRXLACK As String = "GRXLACK"
    ''' <summary>
    ''' Logical unit or channel.
    ''' </summary>
    Public Const HANDLE As String = "HANDLE"
    ''' <summary>
    ''' Host Command Parameter Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const HCACK As String = "HCACK"
    ''' <summary>
    ''' Conveys whether the corresponding handoff activity succeeded (= True) or failed (= False).
    ''' </summary>
    Public Const HOACK As String = "HOACK"
    ''' <summary>
    ''' Tells whether the cancel ready message was accepted or rejected.
    ''' </summary>
    Public Const HOCANCELACK As String = "HOCANCELACK"
    ''' <summary>
    ''' Identifier for the handoff command to be executed.
    ''' </summary>
    Public Const HOCMDNAME As String = "HOCMDNAME"
    ''' <summary>
    ''' Tells whether the halt command was accepted or rejected.
    ''' </summary>
    Public Const HOHALTACK As String = "HOHALTACK"
    ''' <summary>
    ''' Immediately After Command Codes.
    ''' </summary>
    Public Const IACDS As String = "IACDS"
    ''' <summary>
    ''' Immediately Before Command Codes.
    ''' </summary>
    Public Const IBCDS As String = "IBCDS"
    ''' <summary>
    ''' Id type.
    ''' </summary>
    Public Const IDTYP As String = "IDTYP"
    ''' <summary>
    ''' A specialized version of PTN indicating the InputPort.
    ''' </summary>
    Public Const INPTN As String = "INPTN"
    ''' <summary>
    ''' Specifies the action for a ReticleTransferJob.
    ''' </summary>
    Public Const JOBACTION As String = "JOBACTION"
    ''' <summary>
    ''' Length of the service program or process program in bytes.
    ''' </summary>
    Public Const LENGTH As String = "LENGTH"
    ''' <summary>
    ''' Acknowledgment code for variable limit attribute set, 1 byte.
    ''' </summary>
    Public Const LIMITACK As String = "LIMITACK"
    ''' <summary>
    ''' The identifier of a specific limit in the set of limits (as defined by UPPERDB and LOWERDB) for a variable to which the corresponding limit attributes refer, 1 byte.
    ''' </summary>
    Public Const LIMITID As String = "LIMITID"
    ''' <summary>
    ''' The maximum allowed value for the limit values of a specific variable.
    ''' </summary>
    Public Const LIMITMAX As String = "LIMITMAX"
    ''' <summary>
    ''' The minimum allowed value for the limit values of a specific variable.
    ''' </summary>
    Public Const LIMITMIN As String = "LIMITMIN"
    ''' <summary>
    ''' Used to link a completion message with a request that an operation be performed. LINKID is set to the value of RMOPID in the initial request except for the last completion message to be sent, where it is set to zero.
    ''' </summary>
    Public Const LINKID As String = "LINKID"
    ''' <summary>
    ''' Lower limit for numeric value.
    ''' </summary>
    Public Const LLIM As String = "LLIM"
    ''' <summary>
    ''' Machine material location code, 1 byte.
    ''' </summary>
    Public Const LOC As String = "LOC"
    ''' <summary>
    ''' The logical identifier of a material location.
    ''' </summary>
    Public Const LOCID As String = "LOCID"
    ''' <summary>
    ''' A variable limit attribute which defines the lower boundary of the deadband of a limit. The value applies to a single limit (LIMITID) for a specified VID. Thus, UPPERDB and LOWERDB as a pair define a limit.
    ''' </summary>
    Public Const LOWERDB As String = "LOWERDB"
    ''' <summary>
    ''' Link Report Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const LRACK As String = "LRACK"
    ''' <summary>
    ''' Variable limit definition acknowledge code, 1 byte. Defines the error with the limit attributes for the referenceVID.
    ''' </summary>
    Public Const LVACK As String = "LVACK"
    ''' <summary>
    ''' Map Error.
    ''' </summary>
    Public Const MAPER As String = "MAPER"
    ''' <summary>
    ''' Map data format type.
    ''' </summary>
    Public Const MAPFT As String = "MAPFT"
    ''' <summary>
    ''' Provides MaxNumber information for each subspace. Used in the data item MAXNUMBERLIST.
    ''' </summary>
    Public Const MAXNUMBER As String = "MAXNUMBER"
    ''' <summary>
    ''' Maximum number of PEM Recipes allowed to be preserved in PRC after PJ creation. MaxNumber has a list structure so that it can be applied to each subspace. The usage of the list structure is equipment defined.
    ''' </summary>
    Public Const MAXNUMBERLIST As String = "MAXNUMBERLIST"
    ''' <summary>
    ''' Maximum time during which a PEM Recipe allowed to be in PRC after use.
    ''' </summary>
    Public Const MAXTIME As String = "MAXTIME"
    ''' <summary>
    ''' Identifier used to link a handoff command message with its eventual completion message. Corresponding messages carry the same value for this data item.
    ''' </summary>
    Public Const MCINDEX As String = "MCINDEX"
    ''' <summary>
    ''' Map data acknowledge.
    ''' </summary>
    Public Const MDACK As String = "MDACK"
    ''' <summary>
    ''' Equipment Model Type, 20 bytes max.
    ''' </summary>
    Public Const MDLN As String = "MDLN"
    ''' <summary>
    ''' Message expected in the form SxxFyy where x is stream and y is function.
    ''' </summary>
    Public Const MEXP As String = "MEXP"
    ''' <summary>
    ''' Material format code 1 byte by Format 10.
    ''' </summary>
    Public Const MF As String = "MF"
    ''' <summary>
    ''' SECS message block header associated with message block in error.
    ''' </summary>
    Public Const MHEAD As String = "MHEAD"
    ''' <summary>
    ''' Material ID.
    ''' </summary>
    Public Const MID As String = "MID"
    ''' <summary>
    ''' Material ID Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const MIDAC As String = "MIDAC"
    ''' <summary>
    ''' Material ID Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const MIDRA As String = "MIDRA"
    ''' <summary>
    ''' Message length.
    ''' </summary>
    Public Const MLCL As String = "MLCL"
    ''' <summary>
    ''' Matrix mode select, 1 byte.
    ''' </summary>
    Public Const MMODE As String = "MMODE"
    ''' <summary>
    ''' Not After Command Codes.
    ''' </summary>
    Public Const NACDS As String = "NACDS"
    ''' <summary>
    ''' Not Before Command Codes.
    ''' </summary>
    Public Const NBCDS As String = "NBCDS"
    ''' <summary>
    ''' Null bin code value.
    ''' </summary>
    Public Const NULBC As String = "NULBC"
    ''' <summary>
    ''' Acknowledge code.
    ''' </summary>
    Public Const OBJACK As String = "OBJACK"
    ''' <summary>
    ''' Specifies an action to be performed by an object.
    ''' </summary>
    Public Const OBJCMD As String = "OBJCMD"
    ''' <summary>
    ''' Identifier for an object.
    ''' </summary>
    Public Const OBJID As String = "OBJID"
    ''' <summary>
    ''' A text string that has an internal format and that is used to point to a specific object instance. The string is formed out of a sequence of formatted substrings, each specifying an object’s type and identifier. The substring format has the following four fields: object type,  colon character ‘:’,  object identifier,  greater-than symbol ‘>’ where the colon character ‘:’ is used to terminate an object type and the greater than symbol ‘>’ is used to terminate an identifier field. The object type field may be omitted where it may be otherwise determined. The final ‘>’ is optional.
    ''' </summary>
    Public Const OBJSPEC As String = "OBJSPEC"
    ''' <summary>
    ''' Token used for authorization.
    ''' </summary>
    Public Const OBJTOKEN As String = "OBJTOKEN"
    ''' <summary>
    ''' Identifier for a group or class of objects. All objects of the same type must have the same set of attributes available.
    ''' </summary>
    Public Const OBJTYPE As String = "OBJTYPE"
    ''' <summary>
    ''' Acknowledge code for OFF-LINE request.
    ''' </summary>
    Public Const OFLACK As String = "OFLACK"
    ''' <summary>
    ''' Acknowledge code for ONLINE request.
    ''' </summary>
    Public Const ONLACK As String = "ONLACK"
    ''' <summary>
    ''' Operation ID. A unique integer generated by the requestor of an operation, used where multiple completion confirmations may occur.
    ''' </summary>
    Public Const OPID As String = "OPID"
    ''' <summary>
    ''' Host-registered identifier of the operator who uses the Remote Access sessio.
    ''' </summary>
    Public Const OPRID As String = "OPRID"
    ''' <summary>
    ''' Host-registered password of the operator who uses the Remote Access session.
    ''' </summary>
    Public Const OPRPWORD As String = "OPRPWORD"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const ORAACK As String = "ORAACK"
    ''' <summary>
    ''' Information concerning the result of the event.
    ''' </summary>
    Public Const ORAEACK As String = "ORAEACK"
    ''' <summary>
    ''' Origin Location.
    ''' </summary>
    Public Const ORLOC As String = "ORLOC"
    ''' <summary>
    ''' A specialized version of PTN indicating the OutPutPort.
    ''' </summary>
    Public Const OUTPTN As String = "OUTPTN"
    ''' <summary>
    ''' The name of a parameter in a request.
    ''' </summary>
    Public Const PARAMNAME As String = "PARAMNAME"
    ''' <summary>
    ''' The value of the parameter named in PARAMNAME. Values that are lists are restricted to lists of single items of the same format type.
    ''' </summary>
    Public Const PARAMVAL As String = "PARAMVAL"
    ''' <summary>
    ''' Selection from available PDE attributes whose values could be reported.
    ''' </summary>
    Public Const PDEATTRIBUTE As String = "PDEATTRIBUTE"
    ''' <summary>
    ''' Selection from available PDE attributes that can be used to filter the PDE directory report.
    ''' </summary>
    Public Const PDEATTRIBUTENAME As String = "PDEATTRIBUTENAME"
    ''' <summary>
    ''' Contains the value of the corresponding PDEATTRIBUTE in the appropriate format.
    ''' </summary>
    Public Const PDEATTRIBUTEVALUE As String = "PDEATTRIBUTEVALUE"
    ''' <summary>
    ''' Contains the unique identifier of a PDE (uid) or of a PDE group (gid).
    ''' </summary>
    Public Const PDEREF As String = "PDEREF"
    ''' <summary>
    ''' Parameter Default Value.
    ''' </summary>
    Public Const PDFLT As String = "PDFLT"
    ''' <summary>
    ''' OK/NG response from the host to Pre-Exe Check event from equipment.
    ''' </summary>
    Public Const PECACK As String = "PECACK"
    ''' <summary>
    ''' Information concerning the result of the event.
    ''' </summary>
    Public Const PECEACK As String = "PECEACK"
    ''' <summary>
    ''' Response component for single recipe check.
    ''' </summary>
    Public Const PECRACK As String = "PECRACK"
    ''' <summary>
    ''' PEMFlag holds SecurityID to be used for PJ creation.
    ''' </summary>
    Public Const PEMFLAG As String = "PEMFLAG"
    ''' <summary>
    ''' Predefined form code, 1 byte.
    ''' </summary>
    Public Const PFCD As String = "PFCD"
    ''' <summary>
    ''' The action to be performed on a port group.
    ''' </summary>
    Public Const PGRPACTION As String = "PGRPACTION"
    ''' <summary>
    ''' Parameter Count Maximum.
    ''' </summary>
    Public Const PMAX As String = "PMAX"
    ''' <summary>
    ''' Parameter Name ≤16 characters.
    ''' </summary>
    Public Const PNAME As String = "PNAME"
    ''' <summary>
    ''' The action to be performed on a port.
    ''' </summary>
    Public Const PORTACTION As String = "PORTACTION"
    ''' <summary>
    ''' The identifier of a group of ports.
    ''' </summary>
    Public Const PORTGRPNAME As String = "PORTGRPNAME"
    ''' <summary>
    ''' Process Parameter.
    ''' </summary>
    Public Const PPARM As String = "PPARM"
    ''' <summary>
    ''' Process program body.
    ''' </summary>
    Public Const PPBODY As String = "PPBODY"
    ''' <summary>
    ''' Process program grant status, 1 byte.
    ''' </summary>
    Public Const PPGNT As String = "PPGNT"
    ''' <summary>
    ''' Process program ID.
    ''' </summary>
    Public Const PPID As String = "PPID"
    ''' <summary>
    ''' Process axis.
    ''' </summary>
    Public Const PRAXI As String = "PRAXI"
    ''' <summary>
    ''' Commands sent to a Process Job.
    ''' </summary>
    Public Const PRCMDNAME As String = "PRCMDNAME"
    ''' <summary>
    ''' Enable/Disable of PreExecution Check option. This defines use of optional Pre-Execution Check.
    ''' </summary>
    Public Const PRCPREEXECHK As String = "PRCPREEXECHK"
    ''' <summary>
    ''' Enable/Disable of entire PRC functionalities.
    ''' </summary>
    Public Const PRCSWITCH As String = "PRCSWITCH"
    ''' <summary>
    ''' Process Die Count.
    ''' </summary>
    Public Const PRDCT As String = "PRDCT"
    ''' <summary>
    ''' Information concerning the result of the event.
    ''' </summary>
    Public Const PREACK As String = "PREACK"
    ''' <summary>
    ''' Processing related event identification.
    ''' </summary>
    Public Const PREVENTID As String = "PREVENTID"
    ''' <summary>
    ''' Text string which uniquely identifies a Process Job.
    ''' </summary>
    Public Const PRJOBID As String = "PRJOBID"
    ''' <summary>
    ''' Notification of Processing status shall have one of the following values.
    ''' </summary>
    Public Const PRJOBMILESTONE As String = "PRJOBMILESTONE"
    ''' <summary>
    ''' The number of Process Jobs that can be created.
    ''' </summary>
    Public Const PRJOBSPACE As String = "PRJOBSPACE"
    ''' <summary>
    ''' Defines the order by which material in the Process Jobs material list will be processed. Possible values are assigned as follows.
    ''' </summary>
    Public Const PRMTRLORDER As String = "PRMTRLORDER"
    ''' <summary>
    ''' The list of event identifiers, which may be sent as an attribute value to a Process Job. When a Process Job encounters one of these events it will pause, until it receives the PRJobCommand RESUME.
    ''' </summary>
    Public Const PRPAUSEEVENT As String = "PRPAUSEEVENT"
    ''' <summary>
    ''' Indicates that the process resource start processing immediately when ready.
    ''' </summary>
    Public Const PRPROCESSSTART As String = "PRPROCESSSTART"
    ''' <summary>
    ''' Indicates the recipe specification type, whether tuning is applied and which method is used.
    ''' </summary>
    Public Const PRRECIPEMETHOD As String = "PRRECIPEMETHOD"
    ''' <summary>
    ''' Enumerated value, 1 byte.
    ''' </summary>
    Public Const PRSTATE As String = "PRSTATE"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const PRXACK As String = "PRXACK"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const PSRACK As String = "PSRACK"
    ''' <summary>
    ''' Material Port number, 1 byte.
    ''' </summary>
    Public Const PTN As String = "PTN"
    ''' <summary>
    ''' Information concerning the result of the event.
    ''' </summary>
    Public Const QREACK As String = "QREACK"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const QRXACK As String = "QRXACK"
    ''' <summary>
    ''' Information concerning the result of the event.
    ''' </summary>
    Public Const QRXLEACK As String = "QRXLEACK"
    ''' <summary>
    ''' Quantity in format, 1 byte.
    ''' </summary>
    Public Const QUA As String = "QUA"
    ''' <summary>
    ''' Reset acknowledge, 1 byte.
    ''' </summary>
    Public Const RAC As String = "RAC"
    ''' <summary>
    ''' Enable/Disable of entire RAC functionalities.
    ''' </summary>
    Public Const RACSWITCH As String = "RACSWITCH"
    ''' <summary>
    ''' Remote command code or string.
    ''' </summary>
    Public Const RCMD As String = "RCMD"
    ''' <summary>
    ''' The contents (value) of a recipe attribute.
    ''' </summary>
    Public Const RCPATTRDATA As String = "RCPATTRDATA"
    ''' <summary>
    ''' The name (identifier) of a non-identifier recipe attribute.
    ''' </summary>
    Public Const RCPATTRID As String = "RCPATTRID"
    ''' <summary>
    ''' Recipe body.
    ''' </summary>
    Public Const RCPBODY As String = "RCPBODY"
    ''' <summary>
    ''' Recipe body allowed list structure.
    ''' </summary>
    Public Const RCPBODYA As String = "RCPBODYA"
    ''' <summary>
    ''' Recipe class.
    ''' </summary>
    Public Const RCPCLASS As String = "RCPCLASS"
    ''' <summary>
    ''' Indicates an action to be performed on a recipe.
    ''' </summary>
    Public Const RCPCMD As String = "RCPCMD"
    ''' <summary>
    ''' .
    ''' </summary>
    Public Const RCPDEL As String = "RCPDEL"
    ''' <summary>
    ''' The length in bytes of a recipe section.
    ''' </summary>
    Public Const RCPDESCLTH As String = "RCPDESCLTH"
    ''' <summary>
    ''' Identifies a type of descriptor of a recipe: ‘ASDesc’, ‘BodyDesc’, ‘GenDesc..
    ''' </summary>
    Public Const RCPDESCNM As String = "RCPDESCNM"
    ''' <summary>
    ''' The timestamp of a recipe section, in the format ‘YYYYMMDDhhmmsscc..
    ''' </summary>
    Public Const RCPDESCTIME As String = "RCPDESCTIME"
    ''' <summary>
    ''' Recipe identifier. Formatted text conforming to the requirements of OBJSPEC.
    ''' </summary>
    Public Const RCPID As String = "RCPID"
    ''' <summary>
    ''' Recipe name.
    ''' </summary>
    Public Const RCPNAME As String = "RCPNAME"
    ''' <summary>
    ''' The new recipe identifier assigned as the result of a copy or rename operation.
    ''' </summary>
    Public Const RCPNEWID As String = "RCPNEWID"
    ''' <summary>
    ''' Indicates whether any preexisting recipe is to be overwritten (= TRUE) or not (= FALSE) on download.
    ''' </summary>
    Public Const RCPOWCODE As String = "RCPOWCODE"
    ''' <summary>
    ''' The name of a recipe variable parameter. Maximum length of 256 characters.
    ''' </summary>
    Public Const RCPPARNM As String = "RCPPARNM"
    ''' <summary>
    ''' The restrictions applied to a recipe variable parameter setting. Maximum length of 80 characters.
    ''' </summary>
    Public Const RCPPARRULE As String = "RCPPARRULE"
    ''' <summary>
    ''' The initial setting assigned to a recipe variable parameter. Text form restricted to maximum of 80 characters.
    ''' </summary>
    Public Const RCPPARVAL As String = "RCPPARVAL"
    ''' <summary>
    ''' Indicates whether a recipe is to be renamed (= TRUE) or copied (= FALSE).
    ''' </summary>
    Public Const RCPRENAME As String = "RCPRENAME"
    ''' <summary>
    ''' Indicates the sections of a recipe requested for transfer or being transferred.
    ''' </summary>
    Public Const RCPSECCODE As String = "RCPSECCODE"
    ''' <summary>
    ''' Recipe section name: ‘Generic’, ‘Body’, or ‘ASDS..
    ''' </summary>
    Public Const RCPSECNM As String = "RCPSECNM"
    ''' <summary>
    ''' Recipe specifier. The object specifier of a recipe.
    ''' </summary>
    Public Const RCPSPEC As String = "RCPSPEC"
    ''' <summary>
    ''' The status of a managed recipe.
    ''' </summary>
    Public Const RCPSTAT As String = "RCPSTAT"
    ''' <summary>
    ''' Indicates if an existing recipe is to be updated (= True) or a new recipe is to be created (= False).
    ''' </summary>
    Public Const RCPUPDT As String = "RCPUPDT"
    ''' <summary>
    ''' Recipe version.
    ''' </summary>
    Public Const RCPVERS As String = "RCPVERS"
    ''' <summary>
    ''' Maximum length to read.
    ''' </summary>
    Public Const READLN As String = "READLN"
    ''' <summary>
    ''' RCPSPEC' or 'PPID' RECID may not always be a unique identifier.
    ''' </summary>
    Public Const RECID As String = "RECID"
    ''' <summary>
    ''' Maximum length of a Discrete record.
    ''' </summary>
    Public Const RECLEN As String = "RECLEN"
    ''' <summary>
    ''' Reference Point.
    ''' </summary>
    Public Const REFP As String = "REFP"
    ''' <summary>
    ''' Reporting group size.
    ''' </summary>
    Public Const REPGSZ As String = "REPGSZ"
    ''' <summary>
    ''' Resolution code for numeric data.
    ''' </summary>
    Public Const RESC As String = "RESC"
    ''' <summary>
    ''' Contains the unique identifier of a PDE (uid).
    ''' </summary>
    Public Const RESOLUTION As String = "RESOLUTION"
    ''' <summary>
    ''' Status response for the Resolve PDE request. If more than one of these conditions applies, the first value on the list that applies should be returned.
    ''' </summary>
    Public Const RESPDESTAT As String = "RESPDESTAT"
    ''' <summary>
    ''' Object specifier for the recipe executor.
    ''' </summary>
    Public Const RESPEC As String = "RESPEC"
    ''' <summary>
    ''' Resolution value for numeric data.
    ''' </summary>
    Public Const RESV As String = "RESV"
    ''' <summary>
    ''' The object identifier for a reticle. Conforms to OBJSPEC.
    ''' </summary>
    Public Const RETICLEID As String = "RETICLEID"
    ''' <summary>
    ''' Instructions to indicate which pod slots will have reticles placed. Possible values for Reticle-PlacementInstruction are.
    ''' </summary>
    Public Const RETPLACEINSTR As String = "RETPLACEINSTR"
    ''' <summary>
    ''' Instructions to indicate which pod slots will have reticles removed.
    ''' </summary>
    Public Const RETREMOVEINSTR As String = "RETREMOVEINSTR"
    ''' <summary>
    ''' Reset code, 1 byte.
    ''' </summary>
    Public Const RIC As String = "RIC"
    ''' <summary>
    ''' Conveys whether a requested action was successfully completed, denied, completed with errors, or will be completed with notification to the requestor.
    ''' </summary>
    Public Const RMACK As String = "RMACK"
    ''' <summary>
    ''' Indicates the change that occurred for an object.
    ''' </summary>
    Public Const RMCHGSTAT As String = "RMCHGSTAT"
    ''' <summary>
    ''' Indicates the type of change for a recipe.
    ''' </summary>
    Public Const RMCHGTYPE As String = "RMCHGTYPE"
    ''' <summary>
    ''' The maximum total length, in bytes, of a multi-block message, used by the receiver to determine if the anticipated message exceeds the receiver’s capacity.
    ''' </summary>
    Public Const RMDATASIZE As String = "RMDATASIZE"
    ''' <summary>
    ''' Grant code, used to grant or deny a request. 1 byte.
    ''' </summary>
    Public Const RMGRNT As String = "RMGRNT"
    ''' <summary>
    ''' New name (identifier) assigned to a recipe namespace.
    ''' </summary>
    Public Const RMNEWNS As String = "RMNEWNS"
    ''' <summary>
    ''' Action to be performed on a recipe namespace.
    ''' </summary>
    Public Const RMNSCMD As String = "RMNSCMD"
    ''' <summary>
    ''' The object specifier of a recipe namespace.
    ''' </summary>
    Public Const RMNSSPEC As String = "RMNSSPEC"
    ''' <summary>
    ''' The object specifier of a distributed recipe namespace recorder.
    ''' </summary>
    Public Const RMRECSPEC As String = "RMRECSPEC"
    ''' <summary>
    ''' Set to TRUE if initiator of change request was an attached segment. Set to FALSE otherwise.
    ''' </summary>
    Public Const RMREQUESTOR As String = "RMREQUESTOR"
    ''' <summary>
    ''' The object specifier of a distributed recipe namespace segment.
    ''' </summary>
    Public Const RMSEGSPEC As String = "RMSEGSPEC"
    ''' <summary>
    ''' The amount of storage available for at least one recipe in a recipe namespace, in bytes.
    ''' </summary>
    Public Const RMSPACE As String = "RMSPACE"
    ''' <summary>
    ''' Row count in die increments.
    ''' </summary>
    Public Const ROWCT As String = "ROWCT"
    ''' <summary>
    ''' Reticle Pod management service acknowledge code. 1 byte.
    ''' </summary>
    Public Const RPMACK As String = "RPMACK"
    ''' <summary>
    ''' The LocationID towards which a reticle must be moved. Conforms to OBJID.
    ''' </summary>
    Public Const RPMDESTLOC As String = "RPMDESTLOC"
    ''' <summary>
    ''' The LocationID of the location from which to pick-up a reticle for moving it to another location. Conforms to OBJID.
    ''' </summary>
    Public Const RPMSOURLOC As String = "RPMSOURLOC"
    ''' <summary>
    ''' Reference Point Select.
    ''' </summary>
    Public Const RPSEL As String = "RPSEL"
    ''' <summary>
    ''' Report ID.
    ''' </summary>
    Public Const RPTID As String = "RPTID"
    ''' <summary>
    ''' A Trace Object attribute for a flag which, if set TRUE, causes only variables which have changed during the sample period to be included in a report.
    ''' </summary>
    Public Const RPTOC As String = "RPTOC"
    ''' <summary>
    ''' Required Command.
    ''' </summary>
    Public Const RQCMD As String = "RQCMD"
    ''' <summary>
    ''' Required Parameter.
    ''' </summary>
    Public Const RQPAR As String = "RQPAR"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const RRACK As String = "RRACK"
    ''' <summary>
    ''' Ready to Send Acknowledge code, 1 byte.
    ''' </summary>
    Public Const RSACK As String = "RSACK"
    ''' <summary>
    ''' Request Spool Data Acknowledge.
    ''' </summary>
    Public Const RSDA As String = "RSDA"
    ''' <summary>
    ''' Request Spool Data Code.
    ''' </summary>
    Public Const RSDC As String = "RSDC"
    ''' <summary>
    ''' Starting location for row or column. This item consists of 3 values (x,y,direction). If direction value is negative, it equals decreasing direction. If the value is positive, it equals increasing direction. Direction must be a nonzero value.
    ''' </summary>
    Public Const RSINF As String = "RSINF"
    ''' <summary>
    ''' Reset Spooling Acknowledge.
    ''' </summary>
    Public Const RSPACK As String = "RSPACK"
    ''' <summary>
    ''' Status response for the Ready To Send request.
    ''' </summary>
    Public Const RTSRSPSTAT As String = "RTSRSPSTAT"
    ''' <summary>
    ''' Type of record.
    ''' </summary>
    Public Const RTYPE As String = "RTYPE"
    ''' <summary>
    ''' Response component for a list of recipe transfer.
    ''' </summary>
    Public Const RXACK As String = "RXACK"
    ''' <summary>
    ''' Map set-up data acknowledge.
    ''' </summary>
    Public Const SDACK As String = "SDACK"
    ''' <summary>
    ''' Send bin information flag.
    ''' </summary>
    Public Const SDBIN As String = "SDBIN"
    ''' <summary>
    ''' Identifier of Security Class of the recipe.
    ''' </summary>
    Public Const SECID As String = "SECID"
    ''' <summary>
    ''' Reports overall success or failure of the sendPDE() request.
    ''' </summary>
    Public Const SENDRESULT As String = "SENDRESULT"
    ''' <summary>
    ''' Status response for the Send PDE request.
    ''' </summary>
    Public Const SENDRSPSTAT As String = "SENDRSPSTAT"
    ''' <summary>
    ''' Command Number.
    ''' </summary>
    Public Const SEQNUM As String = "SEQNUM"
    ''' <summary>
    ''' Status form code, 1 byte.
    ''' </summary>
    Public Const SFCD As String = "SFCD"
    ''' <summary>
    ''' Stored header related to the transaction timer.
    ''' </summary>
    Public Const SHEAD As String = "SHEAD"
    ''' <summary>
    ''' Used to reference material by slot (a position that holds material/substrates) in a carrier. This item may be implemented as an array in some messages.
    ''' </summary>
    Public Const SLOTID As String = "SLOTID"
    ''' <summary>
    ''' Sample numbe.
    ''' </summary>
    Public Const SMPLN As String = "SMPLN"
    ''' <summary>
    ''' Software revision code 20 bytes maximum.
    ''' </summary>
    Public Const SOFTREV As String = "SOFTREV"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const SPAACK As String = "SPAACK"
    ''' <summary>
    ''' Service program data.
    ''' </summary>
    Public Const SPD As String = "SPD"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const SPFACK As String = "SPFACK"
    ''' <summary>
    ''' Service program ID, 6 characters.
    ''' </summary>
    Public Const SPID As String = "SPID"
    ''' <summary>
    ''' Service parameter name defined in specific standard. If service parameter is defined as an object attribute, this is completely the same as ATTRID except format restrictions above.
    ''' </summary>
    Public Const SPNAME As String = "SPNAME"
    ''' <summary>
    ''' Service program results.
    ''' </summary>
    Public Const SPR As String = "SPR"
    ''' <summary>
    ''' Service parameter value, corresponding to SPNAME. If service parameter is defined as an object attribute, this is completely the same as ATTRDATA except format restrictions for the attribute.
    ''' </summary>
    Public Const SPVAL As String = "SPVAL"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const SRAACK As String = "SRAACK"
    ''' <summary>
    ''' Enable/Disable entire SRS functionalities. Default is Disabled.
    ''' </summary>
    Public Const SRSSWITCH As String = "SRSSWITCH"
    ''' <summary>
    ''' Indicates the success or failure of a requested action. Two characters.
    ''' </summary>
    Public Const SSACK As String = "SSACK"
    ''' <summary>
    ''' Indicates an action to be performed by the subsystem.
    ''' </summary>
    Public Const SSCMD As String = "SSCMD"
    ''' <summary>
    ''' Information concerning the result of the service.
    ''' </summary>
    Public Const SSSACK As String = "SSSACK"
    ''' <summary>
    ''' Provides status information for a subsystem component. Used in the data item STATUSLIST.
    ''' </summary>
    Public Const STATUS As String = "STATUS"
    ''' <summary>
    ''' A list of STATUS data sent in a fixed order. STATUSLIST has the following form.
    ''' </summary>
    Public Const STATUSLIST As String = "STATUSLIST"
    ''' <summary>
    ''' Text string describing the corresponding status response. Maximum length of 80 characters.
    ''' </summary>
    Public Const STATUSTXT As String = "STATUSTXT"
    ''' <summary>
    ''' String template. ASCII text string acceptable to equipment as a parameter value. A data string matches a template string if the data string is at least as long as the template and each character of the data string matches the corresponding character of the template. A null list indicates all user data is acceptable to the machine.
    ''' </summary>
    Public Const STEMP As String = "STEMP"
    ''' <summary>
    ''' Sample time, 12, 16 bytes, or Extended format as specified by the TimeFormat equipment constant value setting.
    ''' </summary>
    Public Const STIME As String = "STIME"
    ''' <summary>
    ''' Spool Stream Acknowledge.
    ''' </summary>
    Public Const STRACK As String = "STRACK"
    ''' <summary>
    ''' Stream Identification.
    ''' </summary>
    Public Const STRID As String = "STRID"
    ''' <summary>
    ''' Starting position in die coordinate position. Must be in (X,Y) order.
    ''' </summary>
    Public Const STRP As String = "STRP"
    ''' <summary>
    ''' Status variable value.
    ''' </summary>
    Public Const SV As String = "SV"
    ''' <summary>
    ''' Service acceptance acknowledge code, 1 byte.
    ''' </summary>
    Public Const SVCACK As String = "SVCACK"
    ''' <summary>
    ''' Service name provided on specified object asking by the host.
    ''' </summary>
    Public Const SVCNAME As String = "SVCNAME"
    ''' <summary>
    ''' Status variable ID.
    ''' </summary>
    Public Const SVID As String = "SVID"
    ''' <summary>
    ''' Status Variable Name.
    ''' </summary>
    Public Const SVNAME As String = "SVNAME"
    ''' <summary>
    ''' Identifies where a request for action or data is to be applied. If text, conforms to OBJSPEC.
    ''' </summary>
    Public Const TARGETID As String = "TARGETID"
    ''' <summary>
    ''' Contains the unique identifier (uid) of the PDE that is the starting point for the verification process.
    ''' </summary>
    Public Const TARGETPDE As String = "TARGETPDE"
    ''' <summary>
    ''' Object specifier of target object.
    ''' </summary>
    Public Const TARGETSPEC As String = "TARGETSPEC"
    ''' <summary>
    ''' Indicates success or failure.
    ''' </summary>
    Public Const TBLACK As String = "TBLACK"
    ''' <summary>
    ''' Provides information about the table or parts of the table being transferred or requested. Enumerated.
    ''' </summary>
    Public Const TBLCMD As String = "TBLCMD"
    ''' <summary>
    ''' Table element. The first table element in a row is used to identify the row.
    ''' </summary>
    Public Const TBLELT As String = "TBLELT"
    ''' <summary>
    ''' Table identifier. Text conforming to the requirements of OBJSPEC.
    ''' </summary>
    Public Const TBLID As String = "TBLID"
    ''' <summary>
    ''' A reserved text string to denote the format and application of the table. Text conforming to the requirements of OBJSPEC.
    ''' </summary>
    Public Const TBLTYP As String = "TBLTYP"
    ''' <summary>
    ''' TCID is the identifier of the TransferContainer.
    ''' </summary>
    Public Const TCID As String = "TCID"
    ''' <summary>
    ''' A single line of characters.
    ''' </summary>
    Public Const TEXT As String = "TEXT"
    ''' <summary>
    ''' Equipment acknowledgement code, 1 byte.
    ''' </summary>
    Public Const TIAACK As String = "TIAACK"
    ''' <summary>
    ''' Time Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const TIACK As String = "TIACK"
    ''' <summary>
    ''' Terminal number, 1 byte.
    ''' </summary>
    Public Const TID As String = "TID"
    ''' <summary>
    ''' Time of day, 12, 16 bytes, or Extended format as specified by the TimeFormat equipment constant value setting.
    ''' </summary>
    Public Const TIME As String = "TIME"
    ''' <summary>
    ''' Timestamp in 12, 16 bytes, or Extended format indicating the time of an event, which encodes time as specified by the TimeFormat equipment constant value setting.
    ''' </summary>
    Public Const TIMESTAMP As String = "TIMESTAMP"
    ''' <summary>
    ''' Total samples to be made.
    ''' </summary>
    Public Const TOTSMP As String = "TOTSMP"
    ''' <summary>
    ''' Tells whether the related transfer activity was successful (= True) or unsuccessful (= False).
    ''' </summary>
    Public Const TRACK As String = "TRACK"
    ''' <summary>
    ''' Size, in bytes, of the TransferContainer proposed for transfer.
    ''' </summary>
    Public Const TRANSFERSIZE As String = "TRANSFERSIZE"
    ''' <summary>
    ''' Equipment assigned identifier for an atomic transfer.
    ''' </summary>
    Public Const TRATOMICID As String = "TRATOMICID"
    ''' <summary>
    ''' A Trace Object attribute for a control flag which, if set TRUE, causes the Trace Object to delete itself when it has completed a report.
    ''' </summary>
    Public Const TRAUTOD As String = "TRAUTOD"
    ''' <summary>
    ''' For each atomic transfer, this data item tells the equipment if it should automatically start the handoff when ready (= TRUE) or await the host’s ‘StartHandoff’ command (= FALSE) following setup. This data item only affects the primary transfer partner.
    ''' </summary>
    Public Const TRAUTOSTART As String = "TRAUTOSTART"
    ''' <summary>
    ''' Identifier of the transfer job-related command to be executed. Possible values.
    ''' </summary>
    Public Const TRCMDNAME As String = "TRCMDNAME"
    ''' <summary>
    ''' Direction of handoff.
    ''' </summary>
    Public Const TRDIR As String = "TRDIR"
    ''' <summary>
    ''' Trace request ID.
    ''' </summary>
    Public Const TRID As String = "TRID"
    ''' <summary>
    ''' Equipment assigned identifier for the transfer job.
    ''' </summary>
    Public Const TRJOBID As String = "TRJOBID"
    ''' <summary>
    ''' Milestone for a transfer job (e.g., started or complete).
    ''' </summary>
    Public Const TRJOBMS As String = "TRJOBMS"
    ''' <summary>
    ''' Host assigned identifier for the transfer job. Limited to a maximum of 80 characters.
    ''' </summary>
    Public Const TRJOBNAME As String = "TRJOBNAME"
    ''' <summary>
    ''' Common identifier for the atomic transfer used by the transfer partners to confirm that they are working on the same host-defined task.
    ''' </summary>
    Public Const TRLINK As String = "TRLINK"
    ''' <summary>
    ''' Identifier of the material location involved with the transfer. For one transfer partner, this will represent the designated source location for the material to be sent. For the other transfer partner, it will represent the designated destination location for the material to be received.
    ''' </summary>
    Public Const TRLOCATION As String = "TRLOCATION"
    ''' <summary>
    ''' Identifier for the material (transfer object) to be transferred.
    ''' </summary>
    Public Const TROBJNAME As String = "TROBJNAME"
    ''' <summary>
    ''' Type of object to be transferred.
    ''' </summary>
    Public Const TROBJTYPE As String = "TROBJTYPE"
    ''' <summary>
    ''' Identifier of the equipment port to be used for the handoff.
    ''' </summary>
    Public Const TRPORT As String = "TRPORT"
    ''' <summary>
    ''' Name of the equipment which will serve as the other transfer partner for this atomic transfer. This corresponds to EQNAME.
    ''' </summary>
    Public Const TRPTNR As String = "TRPTNR"
    ''' <summary>
    ''' Identifier of the transfer partner’s port to be used for the transfer.
    ''' </summary>
    Public Const TRPTPORT As String = "TRPTPORT"
    ''' <summary>
    ''' Name of the transfer recipe for this handoff. Limited to a maximum of 80 characters.
    ''' </summary>
    Public Const TRRCP As String = "TRRCP"
    ''' <summary>
    ''' Tells whether the equipment is to be the primary or secondary transfer partner.
    ''' </summary>
    Public Const TRROLE As String = "TRROLE"
    ''' <summary>
    ''' A Trace Object attribute which holds the value for sampling interval time.
    ''' </summary>
    Public Const TRSPER As String = "TRSPER"
    ''' <summary>
    ''' Tells whether the equipment is to be an active or passive participant in the transfer.
    ''' </summary>
    Public Const TRTYPE As String = "TRTYPE"
    ''' <summary>
    ''' Transfer status of input port, 1 byte.
    ''' </summary>
    Public Const TSIP As String = "TSIP"
    ''' <summary>
    ''' Transfer status of output port, 1 byte.
    ''' </summary>
    Public Const TSOP As String = "TSOP"
    ''' <summary>
    ''' Time to completion.
    ''' </summary>
    Public Const TTC As String = "TTC"
    ''' <summary>
    ''' Identifier of the Type of the recipe.
    ''' </summary>
    Public Const TYPEID As String = "TYPEID"
    ''' <summary>
    ''' Contains a unique identifier for a PDE.
    ''' </summary>
    Public Const UID As String = "UID"
    ''' <summary>
    ''' Upper limit for numeric value.
    ''' </summary>
    Public Const ULIM As String = "ULIM"
    ''' <summary>
    ''' Unformatted Process Program Length.
    ''' </summary>
    Public Const UNFLEN As String = "UNFLEN"
    ''' <summary>
    ''' Units Identifier.
    ''' </summary>
    Public Const UNITS As String = "UNITS"
    ''' <summary>
    ''' A variable limit attribute which defines the upper boundary of the deadband of a limit. The value applies to a single limit (LIMITID) for a specified VID. Thus, UPPERDB and LOWERDB as a pair define a limit.
    ''' </summary>
    Public Const UPPERDB As String = "UPPERDB"
    ''' <summary>
    ''' Variable data.
    ''' </summary>
    Public Const V As String = "V"
    ''' <summary>
    ''' Optional unique identifier of recipes.
    ''' </summary>
    Public Const VERID As String = "VERID"
    ''' <summary>
    ''' Selects whether to check only the target PDE or all associated PDEs within a multi-part recipe.
    ''' </summary>
    Public Const VERIFYDEPTH As String = "VERIFYDEPTH"
    ''' <summary>
    ''' Verification result.
    ''' </summary>
    Public Const VERIFYRSPSTAT As String = "VERIFYRSPSTAT"
    ''' <summary>
    ''' Boolean.
    ''' </summary>
    Public Const VERIFYSUCCESS As String = "VERIFYSUCCESS"
    ''' <summary>
    ''' Choice of the type of verification to perform.
    ''' </summary>
    Public Const VERIFYTYPE As String = "VERIFYTYPE"
    ''' <summary>
    ''' Variable ID.
    ''' </summary>
    Public Const VID As String = "VID"
    ''' <summary>
    ''' Variable Limit Attribute Acknowledge Code, 1 byte.
    ''' </summary>
    Public Const VLAACK As String = "VLAACK"
    ''' <summary>
    ''' X-axis die size (index).
    ''' </summary>
    Public Const XDIES As String = "XDIES"
    ''' <summary>
    ''' X and Y Coordinate Position. Must be in (X,Y) order.
    ''' </summary>
    Public Const XYPOS As String = "XYPOS"
    ''' <summary>
    ''' Y-axis die size (index).
    ''' </summary>
    Public Const YDIES As String = "YDIES"
End Class
#End Region