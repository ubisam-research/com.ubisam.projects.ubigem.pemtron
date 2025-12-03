#pragma once

#ifndef __SAMPLE_HEADER
#define __SAMPLE_HEADER

#define SAMPLE_CEID_Offline _T("1") // Offline
#define SAMPLE_CEID_AlarmSet _T("10") // Alarm Set
#define SAMPLE_CEID_AlarmClear _T("11") // Alarm Clear
#define SAMPLE_CEID_LimitMonitoring _T("12") // Limit Monitoring
#define SAMPLE_CEID_OfflineOnHost _T("2") // Offline On Host
#define SAMPLE_CEID_OnlineLocal _T("3") // Online Local
#define SAMPLE_CEID_OnlineRemote _T("4") // Online Remote
#define SAMPLE_CEID_ControlStateChanged _T("5") // Control State Changed
#define SAMPLE_CEID_EquipmentConstantChanged _T("6") // Equipment Constant Changed
#define SAMPLE_CEID_EquipmentConstantChangedByHost _T("7") // Equipment Constant Changed(HOST-S2F15)
#define SAMPLE_CEID_ProcessProgramChanged _T("8") // Process Program Changed
#define SAMPLE_CEID_ProcessStateChanged _T("9") // Process State Changed

#define SAMPLE_ECID_InitCommunicationState _T("101")
#define SAMPLE_ECID_EstablishCommunicationsTimeout _T("102") // The length of time, in seconds, of the interval between attempts to send S1F13 when establishing communications.
#define SAMPLE_ECID_AreYouThereTimeout _T("103")
#define SAMPLE_ECID_TimeFormat _T("104") // The setting of this ECV controls whether the equipment shall use use the variable item CLOCK and the data items STIME, TIMESTAMP, and TIME in  12-byte, 16-byte, or Extended format.
#define SAMPLE_ECID_InitControlState _T("105")
#define SAMPLE_ECID_OffLineSubState _T("106")
#define SAMPLE_ECID_OnLineFailState _T("107")
#define SAMPLE_ECID_OnLineSubState _T("108")
#define SAMPLE_ECID_DeviceID _T("109")
#define SAMPLE_ECID_IPAddress _T("110")
#define SAMPLE_ECID_PortNumber _T("111")
#define SAMPLE_ECID_ActiveMode _T("112")
#define SAMPLE_ECID_T3Timeout _T("113")
#define SAMPLE_ECID_T5Timeout _T("114")
#define SAMPLE_ECID_T6Timeout _T("115")
#define SAMPLE_ECID_T7Timeout _T("116")
#define SAMPLE_ECID_T8Timeout _T("117")
#define SAMPLE_ECID_LinkTestInterval _T("118")
#define SAMPLE_ECID_HeartbeatInterval _T("119")

#define SAMPLE_VID_Clock _T("1")
#define SAMPLE_VID_PPChangeName _T("10")
#define SAMPLE_VID_PPChangeStatus _T("11")
#define SAMPLE_VID_Alarmset _T("12")
#define SAMPLE_VID_PPStateChangedInfo _T("13")
#define SAMPLE_VID_ALID _T("14")
#define SAMPLE_VID_ControlState _T("2")
#define SAMPLE_VID_ProcessState _T("3")
#define SAMPLE_VID_PreviousProcessState _T("4")
#define SAMPLE_VID_ChangedECID _T("5")
#define SAMPLE_VID_ChangedECV _T("6")
#define SAMPLE_VID_ChangedECList _T("7") // L2(ECID,ECV) List
#define SAMPLE_VID_MDLN _T("8")
#define SAMPLE_VID_SOFTREV _T("9")

#define SAMPLE_DD_ABS _T("ABS") // Any binary string.
#define SAMPLE_DD_ACCESSMODE _T("ACCESSMODE") // Load Port Access Mode. Possible values are.
#define SAMPLE_DD_ACDS _T("ACDS") // After Command Codes.
#define SAMPLE_DD_ACKA _T("ACKA") // Indicates success of a request.
#define SAMPLE_DD_ACKC10 _T("ACKC10") // Acknowledge Code, 1 byte.
#define SAMPLE_DD_ACKC13 _T("ACKC13") // Return code for secondary messages 1 byte.
#define SAMPLE_DD_ACKC15 _T("ACKC15") // Return code for secondary messages, 1 byte.
#define SAMPLE_DD_ACKC3 _T("ACKC3") // Acknowledge code, 1 byte.
#define SAMPLE_DD_ACKC5 _T("ACKC5") // Acknowledge code, 1 byte.
#define SAMPLE_DD_ACKC6 _T("ACKC6") // Acknowledge code, 1 byte.
#define SAMPLE_DD_ACKC7 _T("ACKC7") // Acknowledge code, 1 byte.
#define SAMPLE_DD_ACKC7A _T("ACKC7A") // Acknowledge Code, 1 byte.
#define SAMPLE_DD_AGENT _T("AGENT") // .
#define SAMPLE_DD_ALCD _T("ALCD") // Alarm code byte.
#define SAMPLE_DD_ALED _T("ALED") // Alarm enable/disable code, 1 byte.
#define SAMPLE_DD_ALID _T("ALID") // Alarm identification.
#define SAMPLE_DD_ALTX _T("ALTX") // Alarm text limited to 120 characters.
#define SAMPLE_DD_ATTRDATA _T("ATTRDATA") // Contains a specific attribute value for a specific object.
#define SAMPLE_DD_ATTRID _T("ATTRID") // Identifier for an attribute for a specific type of object.
#define SAMPLE_DD_ATTRRELN _T("ATTRRELN") // The relationship that a specified qualifying value has to the value of an attribute of an object instance (the value of interest).
#define SAMPLE_DD_AUTOCLEAR _T("AUTOCLEAR") // A flag which enables or disables the Auto Clear function.
#define SAMPLE_DD_AUTOCLOSE _T("AUTOCLOSE") // A function that equipment closes the session automatically when operator access doesn¡¯t occur exceeding the predefined maximum time.
#define SAMPLE_DD_BCDS _T("BCDS") // Before Command Codes.
#define SAMPLE_DD_BCEQU _T("BCEQU") // Bin code equivalents.
#define SAMPLE_DD_BINLT _T("BINLT") // The Bin List.
#define SAMPLE_DD_BLKDEF _T("BLKDEF") // Block Definition.
#define SAMPLE_DD_BPD _T("BPD") // Boot program Data.
#define SAMPLE_DD_BYTMAX _T("BYTMAX") // Byte Maximum.
#define SAMPLE_DD_CAACK _T("CAACK") // Carrier Action Acknowledge Code, 1 byte.
#define SAMPLE_DD_CARRIERACTION _T("CARRIERACTION") // Specifies the action requested for a carrier.
#define SAMPLE_DD_CARRIERID _T("CARRIERID") // The identifier of a carrier.
#define SAMPLE_DD_CARRIERSPEC _T("CARRIERSPEC") // The object specifier for a carrier. Conforms to OBJSPEC.
#define SAMPLE_DD_CATTRDATA _T("CATTRDATA") // The value of a carrier attribute.
#define SAMPLE_DD_CATTRID _T("CATTRID") // The name of a carrier attribute.
#define SAMPLE_DD_CCACK _T("CCACK") // Information concerning the result of the service.
#define SAMPLE_DD_CCODE _T("CCODE") // Command Code.
#define SAMPLE_DD_CEED _T("CEED") // Collection event or trace enable/disable code, 1 byte.
#define SAMPLE_DD_CEID _T("CEID") // Collected event ID.
#define SAMPLE_DD_CENAME _T("CENAME") // Collection event name.
#define SAMPLE_DD_CEPACK _T("CEPACK") // Command Enhanced Parameter Acknowledge.
#define SAMPLE_DD_CEPVAL _T("CEPVAL") // Command Enhanced Parameter Value.
#define SAMPLE_DD_CHKINFO _T("CHKINFO") // User defined.
#define SAMPLE_DD_CKPNT _T("CKPNT") // Checkpoint as defined by the sending system.
#define SAMPLE_DD_CLSSTS _T("CLSSTS") // Information if the session is closed properly.
#define SAMPLE_DD_CMDA _T("CMDA") // Command acknowledge code.
#define SAMPLE_DD_CMDMAX _T("CMDMAX") // Command Maximum.
#define SAMPLE_DD_CNAME _T("CNAME") // Command Name = 16 characters.
#define SAMPLE_DD_COLCT _T("COLCT") // Column count in die increments.
#define SAMPLE_DD_COLHDR _T("COLHDR") // Text description of contents of TBLELT. 1?20 characters.
#define SAMPLE_DD_COMMACK _T("COMMACK") // Establish Communications Acknowledge Code, 1 byte.
#define SAMPLE_DD_COMPARISONOPERATOR _T("COMPARISONOPERATOR") // Choice of available operators that compare the supplied value to the current attribute value. Evaluated as ¡®Current value XX supplied value¡¯ where XX is one of the enumerated values  (e.g., ¡®GT¡¯).
#define SAMPLE_DD_CONDITION _T("CONDITION") // Provides condition information for a subsystem component. Used in the data item in the CONDITIONLIST.
#define SAMPLE_DD_CONDITIONLIST _T("CONDITIONLIST") // A list of CONDITION data sent in a fixed order. CONDITIONLIST has the following form.
#define SAMPLE_DD_CPACK _T("CPACK") // Command Parameter Acknowledge Code, 1 byte.
#define SAMPLE_DD_CPNAME _T("CPNAME") // Command Parameter Name.
#define SAMPLE_DD_CPVAL _T("CPVAL") // Command Parameter Value.
#define SAMPLE_DD_CRAACK _T("CRAACK") // Information concerning the result of the service.
#define SAMPLE_DD_CRAEACK _T("CRAEACK") // Information concerning the result of the event.
#define SAMPLE_DD_CSAACK _T("CSAACK") // Equipment Acknowledgement code,  1 byte.
#define SAMPLE_DD_CTLJOBCMD _T("CTLJOBCMD") // Control Job command codes are assigned as follows.
#define SAMPLE_DD_CTLJOBID _T("CTLJOBID") // Identifier for Control Job. Conforms to OBJID.
#define SAMPLE_DD_DATA _T("DATA") // A vector or string of unformatted data.
#define SAMPLE_DD_DATAACK _T("DATAACK") // Acknowledge code for data.
#define SAMPLE_DD_DATAID _T("DATAID") // Data ID.
#define SAMPLE_DD_DATALENGTH _T("DATALENGTH") // Total bytes to be sent.
#define SAMPLE_DD_DATASEG _T("DATASEG") // Used to identify the data requested.
#define SAMPLE_DD_DATASRC _T("DATASRC") // Object type for Data Source Objects.
#define SAMPLE_DD_DATLC _T("DATLC") // Data location.
#define SAMPLE_DD_DELRSPSTAT _T("DELRSPSTAT") // Status response for the Delete PDE request.
#define SAMPLE_DD_DIRRSPSTAT _T("DIRRSPSTAT") // Status response for the GET PDE Directory request.
#define SAMPLE_DD_DRACK _T("DRACK") // Define Report Acknowledge Code, 1 byte.
#define SAMPLE_DD_DRRACK _T("DRRACK") // Information concerning the result of the service.
#define SAMPLE_DD_DSID _T("DSID") // Data set ID.
#define SAMPLE_DD_DSNAME _T("DSNAME") // The name of the Data Set.
#define SAMPLE_DD_DSPER _T("DSPER") // Data sample period. DSPER has two allowable formats.
#define SAMPLE_DD_DUTMS _T("DUTMS") // Die Units of Measure.
#define SAMPLE_DD_DVNAME _T("DVNAME") // Data value name.
#define SAMPLE_DD_DVVAL _T("DVVAL") // Data value.
#define SAMPLE_DD_DVVALNAME _T("DVVALNAME") // A descriptive name for the data variable.
#define SAMPLE_DD_EAC _T("EAC") // Equipment acknowledge code, 1 byte.
#define SAMPLE_DD_ECDEF _T("ECDEF") // Equipment constant default value.
#define SAMPLE_DD_ECID _T("ECID") // Equipment Constant ID.
#define SAMPLE_DD_ECMAX _T("ECMAX") // Equipment constant maximum value.
#define SAMPLE_DD_ECMIN _T("ECMIN") // Equipment constant minimum value.
#define SAMPLE_DD_ECNAME _T("ECNAME") // Equipment constant name.
#define SAMPLE_DD_ECV _T("ECV") // Equipment Constant Value.
#define SAMPLE_DD_EDID _T("EDID") // Expected data Identification.
#define SAMPLE_DD_EMID _T("EMID") // Equivalent material ID  (16 bytes maximum).
#define SAMPLE_DD_EPD _T("EPD") // Executive program data.
#define SAMPLE_DD_EQID _T("EQID") // Identifier that indicates equipment which the recipe is tuned for.
#define SAMPLE_DD_EQNAME _T("EQNAME") // A unique ASCII equipment identifier assigned by the factory to the equipment. Limited to a maximum of 80 characters.
#define SAMPLE_DD_ERACK _T("ERACK") // Enable/Disable Event Report. Acknowledge Code, 1 byte.
#define SAMPLE_DD_ERCACK _T("ERCACK") // Response component for single recipe check.
#define SAMPLE_DD_ERRCODE _T("ERRCODE") // Code identifying an error.
#define SAMPLE_DD_ERRTEXT _T("ERRTEXT") // Text string describing the error noted in the corresponding ERRCODE. Limited to 120 characters maximum.
#define SAMPLE_DD_ERRW7 _T("ERRW7") // Text string describing error found in process program.
#define SAMPLE_DD_ERXACK _T("ERXACK") // Response component for single recipe transfer.
#define SAMPLE_DD_EVNTSRC _T("EVNTSRC") // Object type for Event Source Objects.
#define SAMPLE_DD_EXID _T("EXID") // Unique identifier for the exception. Maximum length of 20 characters.
#define SAMPLE_DD_EXMESSAGE _T("EXMESSAGE") // Text which describes the nature of the exception.
#define SAMPLE_DD_EXRECVRA _T("EXRECVRA") // Text which specifies a recovery action for an exception. Maximum length of 40 bytes.
#define SAMPLE_DD_EXTYPE _T("EXTYPE") // Text which identifies the type of an exception. It is usually a single word of text.
#define SAMPLE_DD_FCNID _T("FCNID") // Function Identification.
#define SAMPLE_DD_FFROT _T("FFROT") // Film Frame Rotation.
#define SAMPLE_DD_FILDAT _T("FILDAT") // Data from the Data Set.
#define SAMPLE_DD_FNLOC _T("FNLOC") // Flat/Notch Location.
#define SAMPLE_DD_FRMLEN _T("FRMLEN") // Formatted Process Program Length.
#define SAMPLE_DD_GETRSPSTAT _T("GETRSPSTAT") // Status response for the Get PDE and Get PDEheader requests.
#define SAMPLE_DD_GRANT _T("GRANT") // Grant code, 1 byte.
#define SAMPLE_DD_GRANT6 _T("GRANT6") // Permission to send, 1 byte.
#define SAMPLE_DD_GRNT1 _T("GRNT1") // Grant code, 1 byte.
#define SAMPLE_DD_GRXLACK _T("GRXLACK") // Information concerning the result of the service.
#define SAMPLE_DD_HANDLE _T("HANDLE") // Logical unit or channel.
#define SAMPLE_DD_HCACK _T("HCACK") // Host Command Parameter Acknowledge Code, 1 byte.
#define SAMPLE_DD_HOACK _T("HOACK") // Conveys whether the corresponding handoff activity succeeded (= True) or failed (= False).
#define SAMPLE_DD_HOCANCELACK _T("HOCANCELACK") // Tells whether the cancel ready message was accepted or rejected.
#define SAMPLE_DD_HOCMDNAME _T("HOCMDNAME") // Identifier for the handoff command to be executed.
#define SAMPLE_DD_HOHALTACK _T("HOHALTACK") // Tells whether the halt command was accepted or rejected.
#define SAMPLE_DD_IACDS _T("IACDS") // Immediately After Command Codes.
#define SAMPLE_DD_IBCDS _T("IBCDS") // Immediately Before Command Codes.
#define SAMPLE_DD_IDTYP _T("IDTYP") // Id type.
#define SAMPLE_DD_INPTN _T("INPTN") // A specialized version of PTN indicating the InputPort.
#define SAMPLE_DD_JOBACTION _T("JOBACTION") // Specifies the action for a ReticleTransferJob.
#define SAMPLE_DD_LENGTH _T("LENGTH") // Length of the service program or process program in bytes.
#define SAMPLE_DD_LIMITACK _T("LIMITACK") // Acknowledgment code for variable limit attribute set, 1 byte.
#define SAMPLE_DD_LIMITID _T("LIMITID") // The identifier of a specific limit in the set of limits (as defined by UPPERDB and LOWERDB) for a variable to which the corresponding limit attributes refer, 1 byte.
#define SAMPLE_DD_LIMITMAX _T("LIMITMAX") // The maximum allowed value for the limit values of a specific variable.
#define SAMPLE_DD_LIMITMIN _T("LIMITMIN") // The minimum allowed value for the limit values of a specific variable.
#define SAMPLE_DD_LINKID _T("LINKID") // Used to link a completion message with a request that an operation be performed. LINKID is set to the value of RMOPID in the initial request except for the last completion message to be sent, where it is set to zero.
#define SAMPLE_DD_LLIM _T("LLIM") // Lower limit for numeric value.
#define SAMPLE_DD_LOC _T("LOC") // Machine material location code, 1 byte.
#define SAMPLE_DD_LOCID _T("LOCID") // The logical identifier of a material location.
#define SAMPLE_DD_LOWERDB _T("LOWERDB") // A variable limit attribute which defines the lower boundary of the deadband of a limit. The value applies to a single limit (LIMITID) for a specified VID. Thus, UPPERDB and LOWERDB as a pair define a limit.
#define SAMPLE_DD_LRACK _T("LRACK") // Link Report Acknowledge Code, 1 byte.
#define SAMPLE_DD_LVACK _T("LVACK") // Variable limit definition acknowledge code, 1 byte. Defines the error with the limit attributes for the referenceVID.
#define SAMPLE_DD_MAPER _T("MAPER") // Map Error.
#define SAMPLE_DD_MAPFT _T("MAPFT") // Map data format type.
#define SAMPLE_DD_MAXNUMBER _T("MAXNUMBER") // Provides MaxNumber information for each subspace. Used in the data item MAXNUMBERLIST.
#define SAMPLE_DD_MAXNUMBERLIST _T("MAXNUMBERLIST") // Maximum number of PEM Recipes allowed to be preserved in PRC after PJ creation. MaxNumber has a list structure so that it can be applied to each subspace. The usage of the list structure is equipment defined.
#define SAMPLE_DD_MAXTIME _T("MAXTIME") // Maximum time during which a PEM Recipe allowed to be in PRC after use.
#define SAMPLE_DD_MCINDEX _T("MCINDEX") // Identifier used to link a handoff command message with its eventual completion message. Corresponding messages carry the same value for this data item.
#define SAMPLE_DD_MDACK _T("MDACK") // Map data acknowledge.
#define SAMPLE_DD_MDLN _T("MDLN") // Equipment Model Type, 20 bytes max.
#define SAMPLE_DD_MEXP _T("MEXP") // Message expected in the form SxxFyy where x is stream and y is function.
#define SAMPLE_DD_MF _T("MF") // Material format code 1 byte by Format 10.
#define SAMPLE_DD_MHEAD _T("MHEAD") // SECS message block header associated with message block in error.
#define SAMPLE_DD_MID _T("MID") // Material ID.
#define SAMPLE_DD_MIDAC _T("MIDAC") // Material ID Acknowledge Code, 1 byte.
#define SAMPLE_DD_MIDRA _T("MIDRA") // Material ID Acknowledge Code, 1 byte.
#define SAMPLE_DD_MLCL _T("MLCL") // Message length.
#define SAMPLE_DD_MMODE _T("MMODE") // Matrix mode select, 1 byte.
#define SAMPLE_DD_NACDS _T("NACDS") // Not After Command Codes.
#define SAMPLE_DD_NBCDS _T("NBCDS") // Not Before Command Codes.
#define SAMPLE_DD_NULBC _T("NULBC") // Null bin code value.
#define SAMPLE_DD_OBJACK _T("OBJACK") // Acknowledge code.
#define SAMPLE_DD_OBJCMD _T("OBJCMD") // Specifies an action to be performed by an object.
#define SAMPLE_DD_OBJID _T("OBJID") // Identifier for an object.
#define SAMPLE_DD_OBJSPEC _T("OBJSPEC") // A text string that has an internal format and that is used to point to a specific object instance. The string is formed out of a sequence of formatted substrings, each specifying an object¡¯s type and identifier. The substring format has the following four fields: object type,  colon character ¡®:¡¯,  object identifier,  greater-than symbol ¡®>¡¯ where the colon character ¡®:¡¯ is used to terminate an object type and the greater than symbol ¡®>¡¯ is used to terminate an identifier field. The object type field may be omitted where it may be otherwise determined. The final ¡®>¡¯ is optional.
#define SAMPLE_DD_OBJTOKEN _T("OBJTOKEN") // Token used for authorization.
#define SAMPLE_DD_OBJTYPE _T("OBJTYPE") // Identifier for a group or class of objects. All objects of the same type must have the same set of attributes available.
#define SAMPLE_DD_OFLACK _T("OFLACK") // Acknowledge code for OFF-LINE request.
#define SAMPLE_DD_ONLACK _T("ONLACK") // Acknowledge code for ONLINE request.
#define SAMPLE_DD_OPID _T("OPID") // Operation ID. A unique integer generated by the requestor of an operation, used where multiple completion confirmations may occur.
#define SAMPLE_DD_OPRID _T("OPRID") // Host-registered identifier of the operator who uses the Remote Access sessio.
#define SAMPLE_DD_OPRPWORD _T("OPRPWORD") // Host-registered password of the operator who uses the Remote Access session.
#define SAMPLE_DD_ORAACK _T("ORAACK") // Information concerning the result of the service.
#define SAMPLE_DD_ORAEACK _T("ORAEACK") // Information concerning the result of the event.
#define SAMPLE_DD_ORLOC _T("ORLOC") // Origin Location.
#define SAMPLE_DD_OUTPTN _T("OUTPTN") // A specialized version of PTN indicating the OutPutPort.
#define SAMPLE_DD_PARAMNAME _T("PARAMNAME") // The name of a parameter in a request.
#define SAMPLE_DD_PARAMVAL _T("PARAMVAL") // The value of the parameter named in PARAMNAME. Values that are lists are restricted to lists of single items of the same format type.
#define SAMPLE_DD_PDEATTRIBUTE _T("PDEATTRIBUTE") // Selection from available PDE attributes whose values could be reported.
#define SAMPLE_DD_PDEATTRIBUTENAME _T("PDEATTRIBUTENAME") // Selection from available PDE attributes that can be used to filter the PDE directory report.
#define SAMPLE_DD_PDEATTRIBUTEVALUE _T("PDEATTRIBUTEVALUE") // Contains the value of the corresponding PDEATTRIBUTE in the appropriate format.
#define SAMPLE_DD_PDEREF _T("PDEREF") // Contains the unique identifier of a PDE (uid) or of a PDE group (gid).
#define SAMPLE_DD_PDFLT _T("PDFLT") // Parameter Default Value.
#define SAMPLE_DD_PECACK _T("PECACK") // OK/NG response from the host to Pre-Exe Check event from equipment.
#define SAMPLE_DD_PECEACK _T("PECEACK") // Information concerning the result of the event.
#define SAMPLE_DD_PECRACK _T("PECRACK") // Response component for single recipe check.
#define SAMPLE_DD_PEMFLAG _T("PEMFLAG") // PEMFlag holds SecurityID to be used for PJ creation.
#define SAMPLE_DD_PFCD _T("PFCD") // Predefined form code, 1 byte.
#define SAMPLE_DD_PGRPACTION _T("PGRPACTION") // The action to be performed on a port group.
#define SAMPLE_DD_PMAX _T("PMAX") // Parameter Count Maximum.
#define SAMPLE_DD_PNAME _T("PNAME") // Parameter Name ¡Â16 characters.
#define SAMPLE_DD_PORTACTION _T("PORTACTION") // The action to be performed on a port.
#define SAMPLE_DD_PORTGRPNAME _T("PORTGRPNAME") // The identifier of a group of ports.
#define SAMPLE_DD_PPARM _T("PPARM") // Process Parameter.
#define SAMPLE_DD_PPBODY _T("PPBODY") // Process program body.
#define SAMPLE_DD_PPGNT _T("PPGNT") // Process program grant status, 1 byte.
#define SAMPLE_DD_PPID _T("PPID") // Process program ID.
#define SAMPLE_DD_PRAXI _T("PRAXI") // Process axis.
#define SAMPLE_DD_PRCMDNAME _T("PRCMDNAME") // Commands sent to a Process Job.
#define SAMPLE_DD_PRCPREEXECHK _T("PRCPREEXECHK") // Enable/Disable of PreExecution Check option. This defines use of optional Pre-Execution Check.
#define SAMPLE_DD_PRCSWITCH _T("PRCSWITCH") // Enable/Disable of entire PRC functionalities.
#define SAMPLE_DD_PRDCT _T("PRDCT") // Process Die Count.
#define SAMPLE_DD_PREACK _T("PREACK") // Information concerning the result of the event.
#define SAMPLE_DD_PREVENTID _T("PREVENTID") // Processing related event identification.
#define SAMPLE_DD_PRJOBID _T("PRJOBID") // Text string which uniquely identifies a Process Job.
#define SAMPLE_DD_PRJOBMILESTONE _T("PRJOBMILESTONE") // Notification of Processing status shall have one of the following values.
#define SAMPLE_DD_PRJOBSPACE _T("PRJOBSPACE") // The number of Process Jobs that can be created.
#define SAMPLE_DD_PRMTRLORDER _T("PRMTRLORDER") // Defines the order by which material in the Process Jobs material list will be processed. Possible values are assigned as follows.
#define SAMPLE_DD_PRPAUSEEVENT _T("PRPAUSEEVENT") // The list of event identifiers, which may be sent as an attribute value to a Process Job. When a Process Job encounters one of these events it will pause, until it receives the PRJobCommand RESUME.
#define SAMPLE_DD_PRPROCESSSTART _T("PRPROCESSSTART") // Indicates that the process resource start processing immediately when ready.
#define SAMPLE_DD_PRRECIPEMETHOD _T("PRRECIPEMETHOD") // Indicates the recipe specification type, whether tuning is applied and which method is used.
#define SAMPLE_DD_PRSTATE _T("PRSTATE") // Enumerated value, 1 byte.
#define SAMPLE_DD_PRXACK _T("PRXACK") // Information concerning the result of the service.
#define SAMPLE_DD_PSRACK _T("PSRACK") // Information concerning the result of the service.
#define SAMPLE_DD_PTN _T("PTN") // Material Port number, 1 byte.
#define SAMPLE_DD_QREACK _T("QREACK") // Information concerning the result of the event.
#define SAMPLE_DD_QRXACK _T("QRXACK") // Information concerning the result of the service.
#define SAMPLE_DD_QRXLEACK _T("QRXLEACK") // Information concerning the result of the event.
#define SAMPLE_DD_QUA _T("QUA") // Quantity in format, 1 byte.
#define SAMPLE_DD_RAC _T("RAC") // Reset acknowledge, 1 byte.
#define SAMPLE_DD_RACSWITCH _T("RACSWITCH") // Enable/Disable of entire RAC functionalities.
#define SAMPLE_DD_RCMD _T("RCMD") // Remote command code or string.
#define SAMPLE_DD_RCPATTRDATA _T("RCPATTRDATA") // The contents (value) of a recipe attribute.
#define SAMPLE_DD_RCPATTRID _T("RCPATTRID") // The name (identifier) of a non-identifier recipe attribute.
#define SAMPLE_DD_RCPBODY _T("RCPBODY") // Recipe body.
#define SAMPLE_DD_RCPBODYA _T("RCPBODYA") // Recipe body allowed list structure.
#define SAMPLE_DD_RCPCLASS _T("RCPCLASS") // Recipe class.
#define SAMPLE_DD_RCPCMD _T("RCPCMD") // Indicates an action to be performed on a recipe.
#define SAMPLE_DD_RCPDEL _T("RCPDEL") // .
#define SAMPLE_DD_RCPDESCLTH _T("RCPDESCLTH") // The length in bytes of a recipe section.
#define SAMPLE_DD_RCPDESCNM _T("RCPDESCNM") // Identifies a type of descriptor of a recipe: ¡®ASDesc¡¯, ¡®BodyDesc¡¯, ¡®GenDesc..
#define SAMPLE_DD_RCPDESCTIME _T("RCPDESCTIME") // The timestamp of a recipe section, in the format ¡®YYYYMMDDhhmmsscc..
#define SAMPLE_DD_RCPID _T("RCPID") // Recipe identifier. Formatted text conforming to the requirements of OBJSPEC.
#define SAMPLE_DD_RCPNAME _T("RCPNAME") // Recipe name.
#define SAMPLE_DD_RCPNEWID _T("RCPNEWID") // The new recipe identifier assigned as the result of a copy or rename operation.
#define SAMPLE_DD_RCPOWCODE _T("RCPOWCODE") // Indicates whether any preexisting recipe is to be overwritten (= TRUE) or not (= FALSE) on download.
#define SAMPLE_DD_RCPPARNM _T("RCPPARNM") // The name of a recipe variable parameter. Maximum length of 256 characters.
#define SAMPLE_DD_RCPPARRULE _T("RCPPARRULE") // The restrictions applied to a recipe variable parameter setting. Maximum length of 80 characters.
#define SAMPLE_DD_RCPPARVAL _T("RCPPARVAL") // The initial setting assigned to a recipe variable parameter. Text form restricted to maximum of 80 characters.
#define SAMPLE_DD_RCPRENAME _T("RCPRENAME") // Indicates whether a recipe is to be renamed (= TRUE) or copied (= FALSE).
#define SAMPLE_DD_RCPSECCODE _T("RCPSECCODE") // Indicates the sections of a recipe requested for transfer or being transferred.
#define SAMPLE_DD_RCPSECNM _T("RCPSECNM") // Recipe section name: ¡®Generic¡¯, ¡®Body¡¯, or ¡®ASDS..
#define SAMPLE_DD_RCPSPEC _T("RCPSPEC") // Recipe specifier. The object specifier of a recipe.
#define SAMPLE_DD_RCPSTAT _T("RCPSTAT") // The status of a managed recipe.
#define SAMPLE_DD_RCPUPDT _T("RCPUPDT") // Indicates if an existing recipe is to be updated (= True) or a new recipe is to be created (= False).
#define SAMPLE_DD_RCPVERS _T("RCPVERS") // Recipe version.
#define SAMPLE_DD_READLN _T("READLN") // Maximum length to read.
#define SAMPLE_DD_RECID _T("RECID") // RCPSPEC' or 'PPID' RECID may not always be a unique identifier.
#define SAMPLE_DD_RECLEN _T("RECLEN") // Maximum length of a Discrete record.
#define SAMPLE_DD_REFP _T("REFP") // Reference Point.
#define SAMPLE_DD_REPGSZ _T("REPGSZ") // Reporting group size.
#define SAMPLE_DD_RESC _T("RESC") // Resolution code for numeric data.
#define SAMPLE_DD_RESOLUTION _T("RESOLUTION") // Contains the unique identifier of a PDE (uid).
#define SAMPLE_DD_RESPDESTAT _T("RESPDESTAT") // Status response for the Resolve PDE request. If more than one of these conditions applies, the first value on the list that applies should be returned.
#define SAMPLE_DD_RESPEC _T("RESPEC") // Object specifier for the recipe executor.
#define SAMPLE_DD_RESV _T("RESV") // Resolution value for numeric data.
#define SAMPLE_DD_RETICLEID _T("RETICLEID") // The object identifier for a reticle. Conforms to OBJSPEC.
#define SAMPLE_DD_RETPLACEINSTR _T("RETPLACEINSTR") // Instructions to indicate which pod slots will have reticles placed. Possible values for Reticle-PlacementInstruction are.
#define SAMPLE_DD_RETREMOVEINSTR _T("RETREMOVEINSTR") // Instructions to indicate which pod slots will have reticles removed.
#define SAMPLE_DD_RIC _T("RIC") // Reset code, 1 byte.
#define SAMPLE_DD_RMACK _T("RMACK") // Conveys whether a requested action was successfully completed, denied, completed with errors, or will be completed with notification to the requestor.
#define SAMPLE_DD_RMCHGSTAT _T("RMCHGSTAT") // Indicates the change that occurred for an object.
#define SAMPLE_DD_RMCHGTYPE _T("RMCHGTYPE") // Indicates the type of change for a recipe.
#define SAMPLE_DD_RMDATASIZE _T("RMDATASIZE") // The maximum total length, in bytes, of a multi-block message, used by the receiver to determine if the anticipated message exceeds the receiver¡¯s capacity.
#define SAMPLE_DD_RMGRNT _T("RMGRNT") // Grant code, used to grant or deny a request. 1 byte.
#define SAMPLE_DD_RMNEWNS _T("RMNEWNS") // New name (identifier) assigned to a recipe namespace.
#define SAMPLE_DD_RMNSCMD _T("RMNSCMD") // Action to be performed on a recipe namespace.
#define SAMPLE_DD_RMNSSPEC _T("RMNSSPEC") // The object specifier of a recipe namespace.
#define SAMPLE_DD_RMRECSPEC _T("RMRECSPEC") // The object specifier of a distributed recipe namespace recorder.
#define SAMPLE_DD_RMREQUESTOR _T("RMREQUESTOR") // Set to TRUE if initiator of change request was an attached segment. Set to FALSE otherwise.
#define SAMPLE_DD_RMSEGSPEC _T("RMSEGSPEC") // The object specifier of a distributed recipe namespace segment.
#define SAMPLE_DD_RMSPACE _T("RMSPACE") // The amount of storage available for at least one recipe in a recipe namespace, in bytes.
#define SAMPLE_DD_ROWCT _T("ROWCT") // Row count in die increments.
#define SAMPLE_DD_RPMACK _T("RPMACK") // Reticle Pod management service acknowledge code. 1 byte.
#define SAMPLE_DD_RPMDESTLOC _T("RPMDESTLOC") // The LocationID towards which a reticle must be moved. Conforms to OBJID.
#define SAMPLE_DD_RPMSOURLOC _T("RPMSOURLOC") // The LocationID of the location from which to pick-up a reticle for moving it to another location. Conforms to OBJID.
#define SAMPLE_DD_RPSEL _T("RPSEL") // Reference Point Select.
#define SAMPLE_DD_RPTID _T("RPTID") // Report ID.
#define SAMPLE_DD_RPTOC _T("RPTOC") // A Trace Object attribute for a flag which, if set TRUE, causes only variables which have changed during the sample period to be included in a report.
#define SAMPLE_DD_RQCMD _T("RQCMD") // Required Command.
#define SAMPLE_DD_RQPAR _T("RQPAR") // Required Parameter.
#define SAMPLE_DD_RRACK _T("RRACK") // Information concerning the result of the service.
#define SAMPLE_DD_RSACK _T("RSACK") // Ready to Send Acknowledge code, 1 byte.
#define SAMPLE_DD_RSDA _T("RSDA") // Request Spool Data Acknowledge.
#define SAMPLE_DD_RSDC _T("RSDC") // Request Spool Data Code.
#define SAMPLE_DD_RSINF _T("RSINF") // Starting location for row or column. This item consists of 3 values (x,y,direction). If direction value is negative, it equals decreasing direction. If the value is positive, it equals increasing direction. Direction must be a nonzero value.
#define SAMPLE_DD_RSPACK _T("RSPACK") // Reset Spooling Acknowledge.
#define SAMPLE_DD_RTSRSPSTAT _T("RTSRSPSTAT") // Status response for the Ready To Send request.
#define SAMPLE_DD_RTYPE _T("RTYPE") // Type of record.
#define SAMPLE_DD_RXACK _T("RXACK") // Response component for a list of recipe transfer.
#define SAMPLE_DD_SDACK _T("SDACK") // Map set-up data acknowledge.
#define SAMPLE_DD_SDBIN _T("SDBIN") // Send bin information flag.
#define SAMPLE_DD_SECID _T("SECID") // Identifier of Security Class of the recipe.
#define SAMPLE_DD_SENDRESULT _T("SENDRESULT") // Reports overall success or failure of the sendPDE() request.
#define SAMPLE_DD_SENDRSPSTAT _T("SENDRSPSTAT") // Status response for the Send PDE request.
#define SAMPLE_DD_SEQNUM _T("SEQNUM") // Command Number.
#define SAMPLE_DD_SFCD _T("SFCD") // Status form code, 1 byte.
#define SAMPLE_DD_SHEAD _T("SHEAD") // Stored header related to the transaction timer.
#define SAMPLE_DD_SLOTID _T("SLOTID") // Used to reference material by slot (a position that holds material/substrates) in a carrier. This item may be implemented as an array in some messages.
#define SAMPLE_DD_SMPLN _T("SMPLN") // Sample numbe.
#define SAMPLE_DD_SOFTREV _T("SOFTREV") // Software revision code 20 bytes maximum.
#define SAMPLE_DD_SPAACK _T("SPAACK") // Information concerning the result of the service.
#define SAMPLE_DD_SPD _T("SPD") // Service program data.
#define SAMPLE_DD_SPFACK _T("SPFACK") // Information concerning the result of the service.
#define SAMPLE_DD_SPID _T("SPID") // Service program ID, 6 characters.
#define SAMPLE_DD_SPNAME _T("SPNAME") // Service parameter name defined in specific standard. If service parameter is defined as an object attribute, this is completely the same as ATTRID except format restrictions above.
#define SAMPLE_DD_SPR _T("SPR") // Service program results.
#define SAMPLE_DD_SPVAL _T("SPVAL") // Service parameter value, corresponding to SPNAME. If service parameter is defined as an object attribute, this is completely the same as ATTRDATA except format restrictions for the attribute.
#define SAMPLE_DD_SRAACK _T("SRAACK") // Information concerning the result of the service.
#define SAMPLE_DD_SRSSWITCH _T("SRSSWITCH") // Enable/Disable entire SRS functionalities. Default is Disabled.
#define SAMPLE_DD_SSACK _T("SSACK") // Indicates the success or failure of a requested action. Two characters.
#define SAMPLE_DD_SSCMD _T("SSCMD") // Indicates an action to be performed by the subsystem.
#define SAMPLE_DD_SSSACK _T("SSSACK") // Information concerning the result of the service.
#define SAMPLE_DD_STATUS _T("STATUS") // Provides status information for a subsystem component. Used in the data item STATUSLIST.
#define SAMPLE_DD_STATUSLIST _T("STATUSLIST") // A list of STATUS data sent in a fixed order. STATUSLIST has the following form.
#define SAMPLE_DD_STATUSTXT _T("STATUSTXT") // Text string describing the corresponding status response. Maximum length of 80 characters.
#define SAMPLE_DD_STEMP _T("STEMP") // String template. ASCII text string acceptable to equipment as a parameter value. A data string matches a template string if the data string is at least as long as the template and each character of the data string matches the corresponding character of the template. A null list indicates all user data is acceptable to the machine.
#define SAMPLE_DD_STIME _T("STIME") // Sample time, 12, 16 bytes, or Extended format as specified by the TimeFormat equipment constant value setting.
#define SAMPLE_DD_STRACK _T("STRACK") // Spool Stream Acknowledge.
#define SAMPLE_DD_STRID _T("STRID") // Stream Identification.
#define SAMPLE_DD_STRP _T("STRP") // Starting position in die coordinate position. Must be in (X,Y) order.
#define SAMPLE_DD_SV _T("SV") // Status variable value.
#define SAMPLE_DD_SVCACK _T("SVCACK") // Service acceptance acknowledge code, 1 byte.
#define SAMPLE_DD_SVCNAME _T("SVCNAME") // Service name provided on specified object asking by the host.
#define SAMPLE_DD_SVID _T("SVID") // Status variable ID.
#define SAMPLE_DD_SVNAME _T("SVNAME") // Status Variable Name.
#define SAMPLE_DD_TARGETID _T("TARGETID") // Identifies where a request for action or data is to be applied. If text, conforms to OBJSPEC.
#define SAMPLE_DD_TARGETPDE _T("TARGETPDE") // Contains the unique identifier (uid) of the PDE that is the starting point for the verification process.
#define SAMPLE_DD_TARGETSPEC _T("TARGETSPEC") // Object specifier of target object.
#define SAMPLE_DD_TBLACK _T("TBLACK") // Indicates success or failure.
#define SAMPLE_DD_TBLCMD _T("TBLCMD") // Provides information about the table or parts of the table being transferred or requested. Enumerated.
#define SAMPLE_DD_TBLELT _T("TBLELT") // Table element. The first table element in a row is used to identify the row.
#define SAMPLE_DD_TBLID _T("TBLID") // Table identifier. Text conforming to the requirements of OBJSPEC.
#define SAMPLE_DD_TBLTYP _T("TBLTYP") // A reserved text string to denote the format and application of the table. Text conforming to the requirements of OBJSPEC.
#define SAMPLE_DD_TCID _T("TCID") // TCID is the identifier of the TransferContainer.
#define SAMPLE_DD_TEXT _T("TEXT") // A single line of characters.
#define SAMPLE_DD_TIAACK _T("TIAACK") // Equipment acknowledgement code, 1 byte.
#define SAMPLE_DD_TIACK _T("TIACK") // Time Acknowledge Code, 1 byte.
#define SAMPLE_DD_TID _T("TID") // Terminal number, 1 byte.
#define SAMPLE_DD_TIME _T("TIME") // Time of day, 12, 16 bytes, or Extended format as specified by the TimeFormat equipment constant value setting.
#define SAMPLE_DD_TIMESTAMP _T("TIMESTAMP") // Timestamp in 12, 16 bytes, or Extended format indicating the time of an event, which encodes time as specified by the TimeFormat equipment constant value setting.
#define SAMPLE_DD_TOTSMP _T("TOTSMP") // Total samples to be made.
#define SAMPLE_DD_TRACK _T("TRACK") // Tells whether the related transfer activity was successful (= True) or unsuccessful (= False).
#define SAMPLE_DD_TRANSFERSIZE _T("TRANSFERSIZE") // Size, in bytes, of the TransferContainer proposed for transfer.
#define SAMPLE_DD_TRATOMICID _T("TRATOMICID") // Equipment assigned identifier for an atomic transfer.
#define SAMPLE_DD_TRAUTOD _T("TRAUTOD") // A Trace Object attribute for a control flag which, if set TRUE, causes the Trace Object to delete itself when it has completed a report.
#define SAMPLE_DD_TRAUTOSTART _T("TRAUTOSTART") // For each atomic transfer, this data item tells the equipment if it should automatically start the handoff when ready (= TRUE) or await the host¡¯s ¡®StartHandoff¡¯ command (= FALSE) following setup. This data item only affects the primary transfer partner.
#define SAMPLE_DD_TRCMDNAME _T("TRCMDNAME") // Identifier of the transfer job-related command to be executed. Possible values.
#define SAMPLE_DD_TRDIR _T("TRDIR") // Direction of handoff.
#define SAMPLE_DD_TRID _T("TRID") // Trace request ID.
#define SAMPLE_DD_TRJOBID _T("TRJOBID") // Equipment assigned identifier for the transfer job.
#define SAMPLE_DD_TRJOBMS _T("TRJOBMS") // Milestone for a transfer job (e.g., started or complete).
#define SAMPLE_DD_TRJOBNAME _T("TRJOBNAME") // Host assigned identifier for the transfer job. Limited to a maximum of 80 characters.
#define SAMPLE_DD_TRLINK _T("TRLINK") // Common identifier for the atomic transfer used by the transfer partners to confirm that they are working on the same host-defined task.
#define SAMPLE_DD_TRLOCATION _T("TRLOCATION") // Identifier of the material location involved with the transfer. For one transfer partner, this will represent the designated source location for the material to be sent. For the other transfer partner, it will represent the designated destination location for the material to be received.
#define SAMPLE_DD_TROBJNAME _T("TROBJNAME") // Identifier for the material (transfer object) to be transferred.
#define SAMPLE_DD_TROBJTYPE _T("TROBJTYPE") // Type of object to be transferred.
#define SAMPLE_DD_TRPORT _T("TRPORT") // Identifier of the equipment port to be used for the handoff.
#define SAMPLE_DD_TRPTNR _T("TRPTNR") // Name of the equipment which will serve as the other transfer partner for this atomic transfer. This corresponds to EQNAME.
#define SAMPLE_DD_TRPTPORT _T("TRPTPORT") // Identifier of the transfer partner¡¯s port to be used for the transfer.
#define SAMPLE_DD_TRRCP _T("TRRCP") // Name of the transfer recipe for this handoff. Limited to a maximum of 80 characters.
#define SAMPLE_DD_TRROLE _T("TRROLE") // Tells whether the equipment is to be the primary or secondary transfer partner.
#define SAMPLE_DD_TRSPER _T("TRSPER") // A Trace Object attribute which holds the value for sampling interval time.
#define SAMPLE_DD_TRTYPE _T("TRTYPE") // Tells whether the equipment is to be an active or passive participant in the transfer.
#define SAMPLE_DD_TSIP _T("TSIP") // Transfer status of input port, 1 byte.
#define SAMPLE_DD_TSOP _T("TSOP") // Transfer status of output port, 1 byte.
#define SAMPLE_DD_TTC _T("TTC") // Time to completion.
#define SAMPLE_DD_TYPEID _T("TYPEID") // Identifier of the Type of the recipe.
#define SAMPLE_DD_UID _T("UID") // Contains a unique identifier for a PDE.
#define SAMPLE_DD_ULIM _T("ULIM") // Upper limit for numeric value.
#define SAMPLE_DD_UNFLEN _T("UNFLEN") // Unformatted Process Program Length.
#define SAMPLE_DD_UNITS _T("UNITS") // Units Identifier.
#define SAMPLE_DD_UPPERDB _T("UPPERDB") // A variable limit attribute which defines the upper boundary of the deadband of a limit. The value applies to a single limit (LIMITID) for a specified VID. Thus, UPPERDB and LOWERDB as a pair define a limit.
#define SAMPLE_DD_V _T("V") // Variable data.
#define SAMPLE_DD_VERID _T("VERID") // Optional unique identifier of recipes.
#define SAMPLE_DD_VERIFYDEPTH _T("VERIFYDEPTH") // Selects whether to check only the target PDE or all associated PDEs within a multi-part recipe.
#define SAMPLE_DD_VERIFYRSPSTAT _T("VERIFYRSPSTAT") // Verification result.
#define SAMPLE_DD_VERIFYSUCCESS _T("VERIFYSUCCESS") // Boolean.
#define SAMPLE_DD_VERIFYTYPE _T("VERIFYTYPE") // Choice of the type of verification to perform.
#define SAMPLE_DD_VID _T("VID") // Variable ID.
#define SAMPLE_DD_VLAACK _T("VLAACK") // Variable Limit Attribute Acknowledge Code, 1 byte.
#define SAMPLE_DD_XDIES _T("XDIES") // X-axis die size (index).
#define SAMPLE_DD_XYPOS _T("XYPOS") // X and Y Coordinate Position. Must be in (X,Y) order.
#define SAMPLE_DD_YDIES _T("YDIES") // Y-axis die size (index).

#endif