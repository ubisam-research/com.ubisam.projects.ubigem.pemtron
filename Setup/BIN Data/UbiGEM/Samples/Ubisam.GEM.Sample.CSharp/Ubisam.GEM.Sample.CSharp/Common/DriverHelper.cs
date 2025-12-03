using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiSam.GEM.Sample.CSharp
{
    #region DefinedCE
    public class DefinedCE
    {
        /// <summary>
        /// Offline
        /// </summary>
        public const string Offline = "1";
        /// <summary>
        /// Alarm Set
        /// </summary>
        public const string AlarmSet = "10";
        /// <summary>
        /// Alarm Clear
        /// </summary>
        public const string AlarmClear = "11";
        /// <summary>
        /// Limit Monitoring
        /// </summary>
        public const string LimitMonitoring = "12";
        /// <summary>
        /// Offline On Host
        /// </summary>
        public const string OfflineOnHost = "2";
        /// <summary>
        /// Online Local
        /// </summary>
        public const string OnlineLocal = "3";
        /// <summary>
        /// Online Remote
        /// </summary>
        public const string OnlineRemote = "4";
        /// <summary>
        /// Control State Changed
        /// </summary>
        public const string ControlStateChanged = "5";
        /// <summary>
        /// Equipment Constant Changed
        /// </summary>
        public const string EquipmentConstantChanged = "6";
        /// <summary>
        /// Equipment Constant Changed(HOST-S2F15)
        /// </summary>
        public const string EquipmentConstantChangedByHost = "7";
        /// <summary>
        /// Process Program Changed
        /// </summary>
        public const string ProcessProgramChanged = "8";
        /// <summary>
        /// Process State Changed
        /// </summary>
        public const string ProcessStateChanged = "9";
    }
    #endregion
    #region DefinedEC
    public class DefinedEC
    {
        public const string InitCommunicationState = "101";
        /// <summary>
        /// The length of time, in seconds, of the interval between attempts to send S1F13 when establishing communications.
        /// </summary>
        public const string EstablishCommunicationsTimeout = "102";
        public const string AreYouThereTimeout = "103";
        /// <summary>
        /// The setting of this ECV controls whether the equipment shall use use the variable item CLOCK and the data items STIME, TIMESTAMP, and TIME in  12-byte, 16-byte, or Extended format.
        /// </summary>
        public const string TimeFormat = "104";
        public const string InitControlState = "105";
        public const string OffLineSubState = "106";
        public const string OnLineFailState = "107";
        public const string OnLineSubState = "108";
        public const string DeviceID = "109";
        public const string IPAddress = "110";
        public const string PortNumber = "111";
        public const string ActiveMode = "112";
        public const string T3Timeout = "113";
        public const string T5Timeout = "114";
        public const string T6Timeout = "115";
        public const string T7Timeout = "116";
        public const string T8Timeout = "117";
        public const string LinkTestInterval = "118";
        public const string HeartbeatInterval = "119";
    }
    #endregion
    #region DefinedV
    public class DefinedV
    {
        public const string Clock = "1";
        public const string PPChangeName = "10";
        public const string PPChangeStatus = "11";
        public const string Alarmset = "12";
        public const string ALID = "14";
        public const string PPStateChangedInfo = "13";
        public const string ControlState = "2";
        public const string ProcessState = "3";
        public const string PreviousProcessState = "4";
        public const string ChangedECID = "5";
        public const string ChangedECV = "6";
        /// <summary>
        /// L2(ECID,ECV) List
        /// </summary>
        public const string ChangedECList = "7";
        public const string MDLN = "8";
        public const string SOFTREV = "9";
    }
    #endregion
    #region DefinedDataDictinary
    public class DefinedDataDictinary
    {
        /// <summary>
        /// Any binary string.
        /// </summary>
        public const string ABS = "ABS";
        /// <summary>
        /// Load Port Access Mode. Possible values are.
        /// </summary>
        public const string ACCESSMODE = "ACCESSMODE";
        /// <summary>
        /// After Command Codes.
        /// </summary>
        public const string ACDS = "ACDS";
        /// <summary>
        /// Indicates success of a request.
        /// </summary>
        public const string ACKA = "ACKA";
        /// <summary>
        /// Acknowledge Code, 1 byte.
        /// </summary>
        public const string ACKC10 = "ACKC10";
        /// <summary>
        /// Return code for secondary messages 1 byte.
        /// </summary>
        public const string ACKC13 = "ACKC13";
        /// <summary>
        /// Return code for secondary messages, 1 byte.
        /// </summary>
        public const string ACKC15 = "ACKC15";
        /// <summary>
        /// Acknowledge code, 1 byte.
        /// </summary>
        public const string ACKC3 = "ACKC3";
        /// <summary>
        /// Acknowledge code, 1 byte.
        /// </summary>
        public const string ACKC5 = "ACKC5";
        /// <summary>
        /// Acknowledge code, 1 byte.
        /// </summary>
        public const string ACKC6 = "ACKC6";
        /// <summary>
        /// Acknowledge code, 1 byte.
        /// </summary>
        public const string ACKC7 = "ACKC7";
        /// <summary>
        /// Acknowledge Code, 1 byte.
        /// </summary>
        public const string ACKC7A = "ACKC7A";
        /// <summary>
        /// .
        /// </summary>
        public const string AGENT = "AGENT";
        /// <summary>
        /// Alarm code byte.
        /// </summary>
        public const string ALCD = "ALCD";
        /// <summary>
        /// Alarm enable/disable code, 1 byte.
        /// </summary>
        public const string ALED = "ALED";
        /// <summary>
        /// Alarm identification.
        /// </summary>
        public const string ALID = "ALID";
        /// <summary>
        /// Alarm text limited to 120 characters.
        /// </summary>
        public const string ALTX = "ALTX";
        /// <summary>
        /// Contains a specific attribute value for a specific object.
        /// </summary>
        public const string ATTRDATA = "ATTRDATA";
        /// <summary>
        /// Identifier for an attribute for a specific type of object.
        /// </summary>
        public const string ATTRID = "ATTRID";
        /// <summary>
        /// The relationship that a specified qualifying value has to the value of an attribute of an object instance (the value of interest).
        /// </summary>
        public const string ATTRRELN = "ATTRRELN";
        /// <summary>
        /// A flag which enables or disables the Auto Clear function.
        /// </summary>
        public const string AUTOCLEAR = "AUTOCLEAR";
        /// <summary>
        /// A function that equipment closes the session automatically when operator access doesn’t occur exceeding the predefined maximum time.
        /// </summary>
        public const string AUTOCLOSE = "AUTOCLOSE";
        /// <summary>
        /// Before Command Codes.
        /// </summary>
        public const string BCDS = "BCDS";
        /// <summary>
        /// Bin code equivalents.
        /// </summary>
        public const string BCEQU = "BCEQU";
        /// <summary>
        /// The Bin List.
        /// </summary>
        public const string BINLT = "BINLT";
        /// <summary>
        /// Block Definition.
        /// </summary>
        public const string BLKDEF = "BLKDEF";
        /// <summary>
        /// Boot program Data.
        /// </summary>
        public const string BPD = "BPD";
        /// <summary>
        /// Byte Maximum.
        /// </summary>
        public const string BYTMAX = "BYTMAX";
        /// <summary>
        /// Carrier Action Acknowledge Code, 1 byte.
        /// </summary>
        public const string CAACK = "CAACK";
        /// <summary>
        /// Specifies the action requested for a carrier.
        /// </summary>
        public const string CARRIERACTION = "CARRIERACTION";
        /// <summary>
        /// The identifier of a carrier.
        /// </summary>
        public const string CARRIERID = "CARRIERID";
        /// <summary>
        /// The object specifier for a carrier. Conforms to OBJSPEC.
        /// </summary>
        public const string CARRIERSPEC = "CARRIERSPEC";
        /// <summary>
        /// The value of a carrier attribute.
        /// </summary>
        public const string CATTRDATA = "CATTRDATA";
        /// <summary>
        /// The name of a carrier attribute.
        /// </summary>
        public const string CATTRID = "CATTRID";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string CCACK = "CCACK";
        /// <summary>
        /// Command Code.
        /// </summary>
        public const string CCODE = "CCODE";
        /// <summary>
        /// Collection event or trace enable/disable code, 1 byte.
        /// </summary>
        public const string CEED = "CEED";
        /// <summary>
        /// Collected event ID.
        /// </summary>
        public const string CEID = "CEID";
        /// <summary>
        /// Collection event name.
        /// </summary>
        public const string CENAME = "CENAME";
        /// <summary>
        /// Command Enhanced Parameter Acknowledge.
        /// </summary>
        public const string CEPACK = "CEPACK";
        /// <summary>
        /// Command Enhanced Parameter Value.
        /// </summary>
        public const string CEPVAL = "CEPVAL";
        /// <summary>
        /// User defined.
        /// </summary>
        public const string CHKINFO = "CHKINFO";
        /// <summary>
        /// Checkpoint as defined by the sending system.
        /// </summary>
        public const string CKPNT = "CKPNT";
        /// <summary>
        /// Information if the session is closed properly.
        /// </summary>
        public const string CLSSTS = "CLSSTS";
        /// <summary>
        /// Command acknowledge code.
        /// </summary>
        public const string CMDA = "CMDA";
        /// <summary>
        /// Command Maximum.
        /// </summary>
        public const string CMDMAX = "CMDMAX";
        /// <summary>
        /// Command Name = 16 characters.
        /// </summary>
        public const string CNAME = "CNAME";
        /// <summary>
        /// Column count in die increments.
        /// </summary>
        public const string COLCT = "COLCT";
        /// <summary>
        /// Text description of contents of TBLELT. 1?20 characters.
        /// </summary>
        public const string COLHDR = "COLHDR";
        /// <summary>
        /// Establish Communications Acknowledge Code, 1 byte.
        /// </summary>
        public const string COMMACK = "COMMACK";
        /// <summary>
        /// Choice of available operators that compare the supplied value to the current attribute value. Evaluated as ‘Current value XX supplied value’ where XX is one of the enumerated values  (e.g., ‘GT’).
        /// </summary>
        public const string COMPARISONOPERATOR = "COMPARISONOPERATOR";
        /// <summary>
        /// Provides condition information for a subsystem component. Used in the data item in the CONDITIONLIST.
        /// </summary>
        public const string CONDITION = "CONDITION";
        /// <summary>
        /// A list of CONDITION data sent in a fixed order. CONDITIONLIST has the following form.
        /// </summary>
        public const string CONDITIONLIST = "CONDITIONLIST";
        /// <summary>
        /// Command Parameter Acknowledge Code, 1 byte.
        /// </summary>
        public const string CPACK = "CPACK";
        /// <summary>
        /// Command Parameter Name.
        /// </summary>
        public const string CPNAME = "CPNAME";
        /// <summary>
        /// Command Parameter Value.
        /// </summary>
        public const string CPVAL = "CPVAL";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string CRAACK = "CRAACK";
        /// <summary>
        /// Information concerning the result of the event.
        /// </summary>
        public const string CRAEACK = "CRAEACK";
        /// <summary>
        /// Equipment Acknowledgement code,  1 byte.
        /// </summary>
        public const string CSAACK = "CSAACK";
        /// <summary>
        /// Control Job command codes are assigned as follows.
        /// </summary>
        public const string CTLJOBCMD = "CTLJOBCMD";
        /// <summary>
        /// Identifier for Control Job. Conforms to OBJID.
        /// </summary>
        public const string CTLJOBID = "CTLJOBID";
        /// <summary>
        /// A vector or string of unformatted data.
        /// </summary>
        public const string DATA = "DATA";
        /// <summary>
        /// Acknowledge code for data.
        /// </summary>
        public const string DATAACK = "DATAACK";
        /// <summary>
        /// Data ID.
        /// </summary>
        public const string DATAID = "DATAID";
        /// <summary>
        /// Total bytes to be sent.
        /// </summary>
        public const string DATALENGTH = "DATALENGTH";
        /// <summary>
        /// Used to identify the data requested.
        /// </summary>
        public const string DATASEG = "DATASEG";
        /// <summary>
        /// Object type for Data Source Objects.
        /// </summary>
        public const string DATASRC = "DATASRC";
        /// <summary>
        /// Data location.
        /// </summary>
        public const string DATLC = "DATLC";
        /// <summary>
        /// Status response for the Delete PDE request.
        /// </summary>
        public const string DELRSPSTAT = "DELRSPSTAT";
        /// <summary>
        /// Status response for the GET PDE Directory request.
        /// </summary>
        public const string DIRRSPSTAT = "DIRRSPSTAT";
        /// <summary>
        /// Define Report Acknowledge Code, 1 byte.
        /// </summary>
        public const string DRACK = "DRACK";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string DRRACK = "DRRACK";
        /// <summary>
        /// Data set ID.
        /// </summary>
        public const string DSID = "DSID";
        /// <summary>
        /// The name of the Data Set.
        /// </summary>
        public const string DSNAME = "DSNAME";
        /// <summary>
        /// Data sample period. DSPER has two allowable formats.
        /// </summary>
        public const string DSPER = "DSPER";
        /// <summary>
        /// Die Units of Measure.
        /// </summary>
        public const string DUTMS = "DUTMS";
        /// <summary>
        /// Data value name.
        /// </summary>
        public const string DVNAME = "DVNAME";
        /// <summary>
        /// Data value.
        /// </summary>
        public const string DVVAL = "DVVAL";
        /// <summary>
        /// A descriptive name for the data variable.
        /// </summary>
        public const string DVVALNAME = "DVVALNAME";
        /// <summary>
        /// Equipment acknowledge code, 1 byte.
        /// </summary>
        public const string EAC = "EAC";
        /// <summary>
        /// Equipment constant default value.
        /// </summary>
        public const string ECDEF = "ECDEF";
        /// <summary>
        /// Equipment Constant ID.
        /// </summary>
        public const string ECID = "ECID";
        /// <summary>
        /// Equipment constant maximum value.
        /// </summary>
        public const string ECMAX = "ECMAX";
        /// <summary>
        /// Equipment constant minimum value.
        /// </summary>
        public const string ECMIN = "ECMIN";
        /// <summary>
        /// Equipment constant name.
        /// </summary>
        public const string ECNAME = "ECNAME";
        /// <summary>
        /// Equipment Constant Value.
        /// </summary>
        public const string ECV = "ECV";
        /// <summary>
        /// Expected data Identification.
        /// </summary>
        public const string EDID = "EDID";
        /// <summary>
        /// Equivalent material ID  (16 bytes maximum).
        /// </summary>
        public const string EMID = "EMID";
        /// <summary>
        /// Executive program data.
        /// </summary>
        public const string EPD = "EPD";
        /// <summary>
        /// Identifier that indicates equipment which the recipe is tuned for.
        /// </summary>
        public const string EQID = "EQID";
        /// <summary>
        /// A unique ASCII equipment identifier assigned by the factory to the equipment. Limited to a maximum of 80 characters.
        /// </summary>
        public const string EQNAME = "EQNAME";
        /// <summary>
        /// Enable/Disable Event Report. Acknowledge Code, 1 byte.
        /// </summary>
        public const string ERACK = "ERACK";
        /// <summary>
        /// Response component for single recipe check.
        /// </summary>
        public const string ERCACK = "ERCACK";
        /// <summary>
        /// Code identifying an error.
        /// </summary>
        public const string ERRCODE = "ERRCODE";
        /// <summary>
        /// Text string describing the error noted in the corresponding ERRCODE. Limited to 120 characters maximum.
        /// </summary>
        public const string ERRTEXT = "ERRTEXT";
        /// <summary>
        /// Text string describing error found in process program.
        /// </summary>
        public const string ERRW7 = "ERRW7";
        /// <summary>
        /// Response component for single recipe transfer.
        /// </summary>
        public const string ERXACK = "ERXACK";
        /// <summary>
        /// Object type for Event Source Objects.
        /// </summary>
        public const string EVNTSRC = "EVNTSRC";
        /// <summary>
        /// Unique identifier for the exception. Maximum length of 20 characters.
        /// </summary>
        public const string EXID = "EXID";
        /// <summary>
        /// Text which describes the nature of the exception.
        /// </summary>
        public const string EXMESSAGE = "EXMESSAGE";
        /// <summary>
        /// Text which specifies a recovery action for an exception. Maximum length of 40 bytes.
        /// </summary>
        public const string EXRECVRA = "EXRECVRA";
        /// <summary>
        /// Text which identifies the type of an exception. It is usually a single word of text.
        /// </summary>
        public const string EXTYPE = "EXTYPE";
        /// <summary>
        /// Function Identification.
        /// </summary>
        public const string FCNID = "FCNID";
        /// <summary>
        /// Film Frame Rotation.
        /// </summary>
        public const string FFROT = "FFROT";
        /// <summary>
        /// Data from the Data Set.
        /// </summary>
        public const string FILDAT = "FILDAT";
        /// <summary>
        /// Flat/Notch Location.
        /// </summary>
        public const string FNLOC = "FNLOC";
        /// <summary>
        /// Formatted Process Program Length.
        /// </summary>
        public const string FRMLEN = "FRMLEN";
        /// <summary>
        /// Status response for the Get PDE and Get PDEheader requests.
        /// </summary>
        public const string GETRSPSTAT = "GETRSPSTAT";
        /// <summary>
        /// Grant code, 1 byte.
        /// </summary>
        public const string GRANT = "GRANT";
        /// <summary>
        /// Permission to send, 1 byte.
        /// </summary>
        public const string GRANT6 = "GRANT6";
        /// <summary>
        /// Grant code, 1 byte.
        /// </summary>
        public const string GRNT1 = "GRNT1";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string GRXLACK = "GRXLACK";
        /// <summary>
        /// Logical unit or channel.
        /// </summary>
        public const string HANDLE = "HANDLE";
        /// <summary>
        /// Host Command Parameter Acknowledge Code, 1 byte.
        /// </summary>
        public const string HCACK = "HCACK";
        /// <summary>
        /// Conveys whether the corresponding handoff activity succeeded (= True) or failed (= False).
        /// </summary>
        public const string HOACK = "HOACK";
        /// <summary>
        /// Tells whether the cancel ready message was accepted or rejected.
        /// </summary>
        public const string HOCANCELACK = "HOCANCELACK";
        /// <summary>
        /// Identifier for the handoff command to be executed.
        /// </summary>
        public const string HOCMDNAME = "HOCMDNAME";
        /// <summary>
        /// Tells whether the halt command was accepted or rejected.
        /// </summary>
        public const string HOHALTACK = "HOHALTACK";
        /// <summary>
        /// Immediately After Command Codes.
        /// </summary>
        public const string IACDS = "IACDS";
        /// <summary>
        /// Immediately Before Command Codes.
        /// </summary>
        public const string IBCDS = "IBCDS";
        /// <summary>
        /// Id type.
        /// </summary>
        public const string IDTYP = "IDTYP";
        /// <summary>
        /// A specialized version of PTN indicating the InputPort.
        /// </summary>
        public const string INPTN = "INPTN";
        /// <summary>
        /// Specifies the action for a ReticleTransferJob.
        /// </summary>
        public const string JOBACTION = "JOBACTION";
        /// <summary>
        /// Length of the service program or process program in bytes.
        /// </summary>
        public const string LENGTH = "LENGTH";
        /// <summary>
        /// Acknowledgment code for variable limit attribute set, 1 byte.
        /// </summary>
        public const string LIMITACK = "LIMITACK";
        /// <summary>
        /// The identifier of a specific limit in the set of limits (as defined by UPPERDB and LOWERDB) for a variable to which the corresponding limit attributes refer, 1 byte.
        /// </summary>
        public const string LIMITID = "LIMITID";
        /// <summary>
        /// The maximum allowed value for the limit values of a specific variable.
        /// </summary>
        public const string LIMITMAX = "LIMITMAX";
        /// <summary>
        /// The minimum allowed value for the limit values of a specific variable.
        /// </summary>
        public const string LIMITMIN = "LIMITMIN";
        /// <summary>
        /// Used to link a completion message with a request that an operation be performed. LINKID is set to the value of RMOPID in the initial request except for the last completion message to be sent, where it is set to zero.
        /// </summary>
        public const string LINKID = "LINKID";
        /// <summary>
        /// Lower limit for numeric value.
        /// </summary>
        public const string LLIM = "LLIM";
        /// <summary>
        /// Machine material location code, 1 byte.
        /// </summary>
        public const string LOC = "LOC";
        /// <summary>
        /// The logical identifier of a material location.
        /// </summary>
        public const string LOCID = "LOCID";
        /// <summary>
        /// A variable limit attribute which defines the lower boundary of the deadband of a limit. The value applies to a single limit (LIMITID) for a specified VID. Thus, UPPERDB and LOWERDB as a pair define a limit.
        /// </summary>
        public const string LOWERDB = "LOWERDB";
        /// <summary>
        /// Link Report Acknowledge Code, 1 byte.
        /// </summary>
        public const string LRACK = "LRACK";
        /// <summary>
        /// Variable limit definition acknowledge code, 1 byte. Defines the error with the limit attributes for the referenceVID.
        /// </summary>
        public const string LVACK = "LVACK";
        /// <summary>
        /// Map Error.
        /// </summary>
        public const string MAPER = "MAPER";
        /// <summary>
        /// Map data format type.
        /// </summary>
        public const string MAPFT = "MAPFT";
        /// <summary>
        /// Provides MaxNumber information for each subspace. Used in the data item MAXNUMBERLIST.
        /// </summary>
        public const string MAXNUMBER = "MAXNUMBER";
        /// <summary>
        /// Maximum number of PEM Recipes allowed to be preserved in PRC after PJ creation. MaxNumber has a list structure so that it can be applied to each subspace. The usage of the list structure is equipment defined.
        /// </summary>
        public const string MAXNUMBERLIST = "MAXNUMBERLIST";
        /// <summary>
        /// Maximum time during which a PEM Recipe allowed to be in PRC after use.
        /// </summary>
        public const string MAXTIME = "MAXTIME";
        /// <summary>
        /// Identifier used to link a handoff command message with its eventual completion message. Corresponding messages carry the same value for this data item.
        /// </summary>
        public const string MCINDEX = "MCINDEX";
        /// <summary>
        /// Map data acknowledge.
        /// </summary>
        public const string MDACK = "MDACK";
        /// <summary>
        /// Equipment Model Type, 20 bytes max.
        /// </summary>
        public const string MDLN = "MDLN";
        /// <summary>
        /// Message expected in the form SxxFyy where x is stream and y is function.
        /// </summary>
        public const string MEXP = "MEXP";
        /// <summary>
        /// Material format code 1 byte by Format 10.
        /// </summary>
        public const string MF = "MF";
        /// <summary>
        /// SECS message block header associated with message block in error.
        /// </summary>
        public const string MHEAD = "MHEAD";
        /// <summary>
        /// Material ID.
        /// </summary>
        public const string MID = "MID";
        /// <summary>
        /// Material ID Acknowledge Code, 1 byte.
        /// </summary>
        public const string MIDAC = "MIDAC";
        /// <summary>
        /// Material ID Acknowledge Code, 1 byte.
        /// </summary>
        public const string MIDRA = "MIDRA";
        /// <summary>
        /// Message length.
        /// </summary>
        public const string MLCL = "MLCL";
        /// <summary>
        /// Matrix mode select, 1 byte.
        /// </summary>
        public const string MMODE = "MMODE";
        /// <summary>
        /// Not After Command Codes.
        /// </summary>
        public const string NACDS = "NACDS";
        /// <summary>
        /// Not Before Command Codes.
        /// </summary>
        public const string NBCDS = "NBCDS";
        /// <summary>
        /// Null bin code value.
        /// </summary>
        public const string NULBC = "NULBC";
        /// <summary>
        /// Acknowledge code.
        /// </summary>
        public const string OBJACK = "OBJACK";
        /// <summary>
        /// Specifies an action to be performed by an object.
        /// </summary>
        public const string OBJCMD = "OBJCMD";
        /// <summary>
        /// Identifier for an object.
        /// </summary>
        public const string OBJID = "OBJID";
        /// <summary>
        /// A text string that has an internal format and that is used to point to a specific object instance. The string is formed out of a sequence of formatted substrings, each specifying an object’s type and identifier. The substring format has the following four fields: object type,  colon character ‘:’,  object identifier,  greater-than symbol ‘>’ where the colon character ‘:’ is used to terminate an object type and the greater than symbol ‘>’ is used to terminate an identifier field. The object type field may be omitted where it may be otherwise determined. The final ‘>’ is optional.
        /// </summary>
        public const string OBJSPEC = "OBJSPEC";
        /// <summary>
        /// Token used for authorization.
        /// </summary>
        public const string OBJTOKEN = "OBJTOKEN";
        /// <summary>
        /// Identifier for a group or class of objects. All objects of the same type must have the same set of attributes available.
        /// </summary>
        public const string OBJTYPE = "OBJTYPE";
        /// <summary>
        /// Acknowledge code for OFF-LINE request.
        /// </summary>
        public const string OFLACK = "OFLACK";
        /// <summary>
        /// Acknowledge code for ONLINE request.
        /// </summary>
        public const string ONLACK = "ONLACK";
        /// <summary>
        /// Operation ID. A unique integer generated by the requestor of an operation, used where multiple completion confirmations may occur.
        /// </summary>
        public const string OPID = "OPID";
        /// <summary>
        /// Host-registered identifier of the operator who uses the Remote Access sessio.
        /// </summary>
        public const string OPRID = "OPRID";
        /// <summary>
        /// Host-registered password of the operator who uses the Remote Access session.
        /// </summary>
        public const string OPRPWORD = "OPRPWORD";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string ORAACK = "ORAACK";
        /// <summary>
        /// Information concerning the result of the event.
        /// </summary>
        public const string ORAEACK = "ORAEACK";
        /// <summary>
        /// Origin Location.
        /// </summary>
        public const string ORLOC = "ORLOC";
        /// <summary>
        /// A specialized version of PTN indicating the OutPutPort.
        /// </summary>
        public const string OUTPTN = "OUTPTN";
        /// <summary>
        /// The name of a parameter in a request.
        /// </summary>
        public const string PARAMNAME = "PARAMNAME";
        /// <summary>
        /// The value of the parameter named in PARAMNAME. Values that are lists are restricted to lists of single items of the same format type.
        /// </summary>
        public const string PARAMVAL = "PARAMVAL";
        /// <summary>
        /// Selection from available PDE attributes whose values could be reported.
        /// </summary>
        public const string PDEATTRIBUTE = "PDEATTRIBUTE";
        /// <summary>
        /// Selection from available PDE attributes that can be used to filter the PDE directory report.
        /// </summary>
        public const string PDEATTRIBUTENAME = "PDEATTRIBUTENAME";
        /// <summary>
        /// Contains the value of the corresponding PDEATTRIBUTE in the appropriate format.
        /// </summary>
        public const string PDEATTRIBUTEVALUE = "PDEATTRIBUTEVALUE";
        /// <summary>
        /// Contains the unique identifier of a PDE (uid) or of a PDE group (gid).
        /// </summary>
        public const string PDEREF = "PDEREF";
        /// <summary>
        /// Parameter Default Value.
        /// </summary>
        public const string PDFLT = "PDFLT";
        /// <summary>
        /// OK/NG response from the host to Pre-Exe Check event from equipment.
        /// </summary>
        public const string PECACK = "PECACK";
        /// <summary>
        /// Information concerning the result of the event.
        /// </summary>
        public const string PECEACK = "PECEACK";
        /// <summary>
        /// Response component for single recipe check.
        /// </summary>
        public const string PECRACK = "PECRACK";
        /// <summary>
        /// PEMFlag holds SecurityID to be used for PJ creation.
        /// </summary>
        public const string PEMFLAG = "PEMFLAG";
        /// <summary>
        /// Predefined form code, 1 byte.
        /// </summary>
        public const string PFCD = "PFCD";
        /// <summary>
        /// The action to be performed on a port group.
        /// </summary>
        public const string PGRPACTION = "PGRPACTION";
        /// <summary>
        /// Parameter Count Maximum.
        /// </summary>
        public const string PMAX = "PMAX";
        /// <summary>
        /// Parameter Name ≤16 characters.
        /// </summary>
        public const string PNAME = "PNAME";
        /// <summary>
        /// The action to be performed on a port.
        /// </summary>
        public const string PORTACTION = "PORTACTION";
        /// <summary>
        /// The identifier of a group of ports.
        /// </summary>
        public const string PORTGRPNAME = "PORTGRPNAME";
        /// <summary>
        /// Process Parameter.
        /// </summary>
        public const string PPARM = "PPARM";
        /// <summary>
        /// Process program body.
        /// </summary>
        public const string PPBODY = "PPBODY";
        /// <summary>
        /// Process program grant status, 1 byte.
        /// </summary>
        public const string PPGNT = "PPGNT";
        /// <summary>
        /// Process program ID.
        /// </summary>
        public const string PPID = "PPID";
        /// <summary>
        /// Process axis.
        /// </summary>
        public const string PRAXI = "PRAXI";
        /// <summary>
        /// Commands sent to a Process Job.
        /// </summary>
        public const string PRCMDNAME = "PRCMDNAME";
        /// <summary>
        /// Enable/Disable of PreExecution Check option. This defines use of optional Pre-Execution Check.
        /// </summary>
        public const string PRCPREEXECHK = "PRCPREEXECHK";
        /// <summary>
        /// Enable/Disable of entire PRC functionalities.
        /// </summary>
        public const string PRCSWITCH = "PRCSWITCH";
        /// <summary>
        /// Process Die Count.
        /// </summary>
        public const string PRDCT = "PRDCT";
        /// <summary>
        /// Information concerning the result of the event.
        /// </summary>
        public const string PREACK = "PREACK";
        /// <summary>
        /// Processing related event identification.
        /// </summary>
        public const string PREVENTID = "PREVENTID";
        /// <summary>
        /// Text string which uniquely identifies a Process Job.
        /// </summary>
        public const string PRJOBID = "PRJOBID";
        /// <summary>
        /// Notification of Processing status shall have one of the following values.
        /// </summary>
        public const string PRJOBMILESTONE = "PRJOBMILESTONE";
        /// <summary>
        /// The number of Process Jobs that can be created.
        /// </summary>
        public const string PRJOBSPACE = "PRJOBSPACE";
        /// <summary>
        /// Defines the order by which material in the Process Jobs material list will be processed. Possible values are assigned as follows.
        /// </summary>
        public const string PRMTRLORDER = "PRMTRLORDER";
        /// <summary>
        /// The list of event identifiers, which may be sent as an attribute value to a Process Job. When a Process Job encounters one of these events it will pause, until it receives the PRJobCommand RESUME.
        /// </summary>
        public const string PRPAUSEEVENT = "PRPAUSEEVENT";
        /// <summary>
        /// Indicates that the process resource start processing immediately when ready.
        /// </summary>
        public const string PRPROCESSSTART = "PRPROCESSSTART";
        /// <summary>
        /// Indicates the recipe specification type, whether tuning is applied and which method is used.
        /// </summary>
        public const string PRRECIPEMETHOD = "PRRECIPEMETHOD";
        /// <summary>
        /// Enumerated value, 1 byte.
        /// </summary>
        public const string PRSTATE = "PRSTATE";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string PRXACK = "PRXACK";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string PSRACK = "PSRACK";
        /// <summary>
        /// Material Port number, 1 byte.
        /// </summary>
        public const string PTN = "PTN";
        /// <summary>
        /// Information concerning the result of the event.
        /// </summary>
        public const string QREACK = "QREACK";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string QRXACK = "QRXACK";
        /// <summary>
        /// Information concerning the result of the event.
        /// </summary>
        public const string QRXLEACK = "QRXLEACK";
        /// <summary>
        /// Quantity in format, 1 byte.
        /// </summary>
        public const string QUA = "QUA";
        /// <summary>
        /// Reset acknowledge, 1 byte.
        /// </summary>
        public const string RAC = "RAC";
        /// <summary>
        /// Enable/Disable of entire RAC functionalities.
        /// </summary>
        public const string RACSWITCH = "RACSWITCH";
        /// <summary>
        /// Remote command code or string.
        /// </summary>
        public const string RCMD = "RCMD";
        /// <summary>
        /// The contents (value) of a recipe attribute.
        /// </summary>
        public const string RCPATTRDATA = "RCPATTRDATA";
        /// <summary>
        /// The name (identifier) of a non-identifier recipe attribute.
        /// </summary>
        public const string RCPATTRID = "RCPATTRID";
        /// <summary>
        /// Recipe body.
        /// </summary>
        public const string RCPBODY = "RCPBODY";
        /// <summary>
        /// Recipe body allowed list structure.
        /// </summary>
        public const string RCPBODYA = "RCPBODYA";
        /// <summary>
        /// Recipe class.
        /// </summary>
        public const string RCPCLASS = "RCPCLASS";
        /// <summary>
        /// Indicates an action to be performed on a recipe.
        /// </summary>
        public const string RCPCMD = "RCPCMD";
        /// <summary>
        /// .
        /// </summary>
        public const string RCPDEL = "RCPDEL";
        /// <summary>
        /// The length in bytes of a recipe section.
        /// </summary>
        public const string RCPDESCLTH = "RCPDESCLTH";
        /// <summary>
        /// Identifies a type of descriptor of a recipe: ‘ASDesc’, ‘BodyDesc’, ‘GenDesc..
        /// </summary>
        public const string RCPDESCNM = "RCPDESCNM";
        /// <summary>
        /// The timestamp of a recipe section, in the format ‘YYYYMMDDhhmmsscc..
        /// </summary>
        public const string RCPDESCTIME = "RCPDESCTIME";
        /// <summary>
        /// Recipe identifier. Formatted text conforming to the requirements of OBJSPEC.
        /// </summary>
        public const string RCPID = "RCPID";
        /// <summary>
        /// Recipe name.
        /// </summary>
        public const string RCPNAME = "RCPNAME";
        /// <summary>
        /// The new recipe identifier assigned as the result of a copy or rename operation.
        /// </summary>
        public const string RCPNEWID = "RCPNEWID";
        /// <summary>
        /// Indicates whether any preexisting recipe is to be overwritten (= TRUE) or not (= FALSE) on download.
        /// </summary>
        public const string RCPOWCODE = "RCPOWCODE";
        /// <summary>
        /// The name of a recipe variable parameter. Maximum length of 256 characters.
        /// </summary>
        public const string RCPPARNM = "RCPPARNM";
        /// <summary>
        /// The restrictions applied to a recipe variable parameter setting. Maximum length of 80 characters.
        /// </summary>
        public const string RCPPARRULE = "RCPPARRULE";
        /// <summary>
        /// The initial setting assigned to a recipe variable parameter. Text form restricted to maximum of 80 characters.
        /// </summary>
        public const string RCPPARVAL = "RCPPARVAL";
        /// <summary>
        /// Indicates whether a recipe is to be renamed (= TRUE) or copied (= FALSE).
        /// </summary>
        public const string RCPRENAME = "RCPRENAME";
        /// <summary>
        /// Indicates the sections of a recipe requested for transfer or being transferred.
        /// </summary>
        public const string RCPSECCODE = "RCPSECCODE";
        /// <summary>
        /// Recipe section name: ‘Generic’, ‘Body’, or ‘ASDS..
        /// </summary>
        public const string RCPSECNM = "RCPSECNM";
        /// <summary>
        /// Recipe specifier. The object specifier of a recipe.
        /// </summary>
        public const string RCPSPEC = "RCPSPEC";
        /// <summary>
        /// The status of a managed recipe.
        /// </summary>
        public const string RCPSTAT = "RCPSTAT";
        /// <summary>
        /// Indicates if an existing recipe is to be updated (= True) or a new recipe is to be created (= False).
        /// </summary>
        public const string RCPUPDT = "RCPUPDT";
        /// <summary>
        /// Recipe version.
        /// </summary>
        public const string RCPVERS = "RCPVERS";
        /// <summary>
        /// Maximum length to read.
        /// </summary>
        public const string READLN = "READLN";
        /// <summary>
        /// RCPSPEC' or 'PPID' RECID may not always be a unique identifier.
        /// </summary>
        public const string RECID = "RECID";
        /// <summary>
        /// Maximum length of a Discrete record.
        /// </summary>
        public const string RECLEN = "RECLEN";
        /// <summary>
        /// Reference Point.
        /// </summary>
        public const string REFP = "REFP";
        /// <summary>
        /// Reporting group size.
        /// </summary>
        public const string REPGSZ = "REPGSZ";
        /// <summary>
        /// Resolution code for numeric data.
        /// </summary>
        public const string RESC = "RESC";
        /// <summary>
        /// Contains the unique identifier of a PDE (uid).
        /// </summary>
        public const string RESOLUTION = "RESOLUTION";
        /// <summary>
        /// Status response for the Resolve PDE request. If more than one of these conditions applies, the first value on the list that applies should be returned.
        /// </summary>
        public const string RESPDESTAT = "RESPDESTAT";
        /// <summary>
        /// Object specifier for the recipe executor.
        /// </summary>
        public const string RESPEC = "RESPEC";
        /// <summary>
        /// Resolution value for numeric data.
        /// </summary>
        public const string RESV = "RESV";
        /// <summary>
        /// The object identifier for a reticle. Conforms to OBJSPEC.
        /// </summary>
        public const string RETICLEID = "RETICLEID";
        /// <summary>
        /// Instructions to indicate which pod slots will have reticles placed. Possible values for Reticle-PlacementInstruction are.
        /// </summary>
        public const string RETPLACEINSTR = "RETPLACEINSTR";
        /// <summary>
        /// Instructions to indicate which pod slots will have reticles removed.
        /// </summary>
        public const string RETREMOVEINSTR = "RETREMOVEINSTR";
        /// <summary>
        /// Reset code, 1 byte.
        /// </summary>
        public const string RIC = "RIC";
        /// <summary>
        /// Conveys whether a requested action was successfully completed, denied, completed with errors, or will be completed with notification to the requestor.
        /// </summary>
        public const string RMACK = "RMACK";
        /// <summary>
        /// Indicates the change that occurred for an object.
        /// </summary>
        public const string RMCHGSTAT = "RMCHGSTAT";
        /// <summary>
        /// Indicates the type of change for a recipe.
        /// </summary>
        public const string RMCHGTYPE = "RMCHGTYPE";
        /// <summary>
        /// The maximum total length, in bytes, of a multi-block message, used by the receiver to determine if the anticipated message exceeds the receiver’s capacity.
        /// </summary>
        public const string RMDATASIZE = "RMDATASIZE";
        /// <summary>
        /// Grant code, used to grant or deny a request. 1 byte.
        /// </summary>
        public const string RMGRNT = "RMGRNT";
        /// <summary>
        /// New name (identifier) assigned to a recipe namespace.
        /// </summary>
        public const string RMNEWNS = "RMNEWNS";
        /// <summary>
        /// Action to be performed on a recipe namespace.
        /// </summary>
        public const string RMNSCMD = "RMNSCMD";
        /// <summary>
        /// The object specifier of a recipe namespace.
        /// </summary>
        public const string RMNSSPEC = "RMNSSPEC";
        /// <summary>
        /// The object specifier of a distributed recipe namespace recorder.
        /// </summary>
        public const string RMRECSPEC = "RMRECSPEC";
        /// <summary>
        /// Set to TRUE if initiator of change request was an attached segment. Set to FALSE otherwise.
        /// </summary>
        public const string RMREQUESTOR = "RMREQUESTOR";
        /// <summary>
        /// The object specifier of a distributed recipe namespace segment.
        /// </summary>
        public const string RMSEGSPEC = "RMSEGSPEC";
        /// <summary>
        /// The amount of storage available for at least one recipe in a recipe namespace, in bytes.
        /// </summary>
        public const string RMSPACE = "RMSPACE";
        /// <summary>
        /// Row count in die increments.
        /// </summary>
        public const string ROWCT = "ROWCT";
        /// <summary>
        /// Reticle Pod management service acknowledge code. 1 byte.
        /// </summary>
        public const string RPMACK = "RPMACK";
        /// <summary>
        /// The LocationID towards which a reticle must be moved. Conforms to OBJID.
        /// </summary>
        public const string RPMDESTLOC = "RPMDESTLOC";
        /// <summary>
        /// The LocationID of the location from which to pick-up a reticle for moving it to another location. Conforms to OBJID.
        /// </summary>
        public const string RPMSOURLOC = "RPMSOURLOC";
        /// <summary>
        /// Reference Point Select.
        /// </summary>
        public const string RPSEL = "RPSEL";
        /// <summary>
        /// Report ID.
        /// </summary>
        public const string RPTID = "RPTID";
        /// <summary>
        /// A Trace Object attribute for a flag which, if set TRUE, causes only variables which have changed during the sample period to be included in a report.
        /// </summary>
        public const string RPTOC = "RPTOC";
        /// <summary>
        /// Required Command.
        /// </summary>
        public const string RQCMD = "RQCMD";
        /// <summary>
        /// Required Parameter.
        /// </summary>
        public const string RQPAR = "RQPAR";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string RRACK = "RRACK";
        /// <summary>
        /// Ready to Send Acknowledge code, 1 byte.
        /// </summary>
        public const string RSACK = "RSACK";
        /// <summary>
        /// Request Spool Data Acknowledge.
        /// </summary>
        public const string RSDA = "RSDA";
        /// <summary>
        /// Request Spool Data Code.
        /// </summary>
        public const string RSDC = "RSDC";
        /// <summary>
        /// Starting location for row or column. This item consists of 3 values (x,y,direction). If direction value is negative, it equals decreasing direction. If the value is positive, it equals increasing direction. Direction must be a nonzero value.
        /// </summary>
        public const string RSINF = "RSINF";
        /// <summary>
        /// Reset Spooling Acknowledge.
        /// </summary>
        public const string RSPACK = "RSPACK";
        /// <summary>
        /// Status response for the Ready To Send request.
        /// </summary>
        public const string RTSRSPSTAT = "RTSRSPSTAT";
        /// <summary>
        /// Type of record.
        /// </summary>
        public const string RTYPE = "RTYPE";
        /// <summary>
        /// Response component for a list of recipe transfer.
        /// </summary>
        public const string RXACK = "RXACK";
        /// <summary>
        /// Map set-up data acknowledge.
        /// </summary>
        public const string SDACK = "SDACK";
        /// <summary>
        /// Send bin information flag.
        /// </summary>
        public const string SDBIN = "SDBIN";
        /// <summary>
        /// Identifier of Security Class of the recipe.
        /// </summary>
        public const string SECID = "SECID";
        /// <summary>
        /// Reports overall success or failure of the sendPDE() request.
        /// </summary>
        public const string SENDRESULT = "SENDRESULT";
        /// <summary>
        /// Status response for the Send PDE request.
        /// </summary>
        public const string SENDRSPSTAT = "SENDRSPSTAT";
        /// <summary>
        /// Command Number.
        /// </summary>
        public const string SEQNUM = "SEQNUM";
        /// <summary>
        /// Status form code, 1 byte.
        /// </summary>
        public const string SFCD = "SFCD";
        /// <summary>
        /// Stored header related to the transaction timer.
        /// </summary>
        public const string SHEAD = "SHEAD";
        /// <summary>
        /// Used to reference material by slot (a position that holds material/substrates) in a carrier. This item may be implemented as an array in some messages.
        /// </summary>
        public const string SLOTID = "SLOTID";
        /// <summary>
        /// Sample numbe.
        /// </summary>
        public const string SMPLN = "SMPLN";
        /// <summary>
        /// Software revision code 20 bytes maximum.
        /// </summary>
        public const string SOFTREV = "SOFTREV";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string SPAACK = "SPAACK";
        /// <summary>
        /// Service program data.
        /// </summary>
        public const string SPD = "SPD";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string SPFACK = "SPFACK";
        /// <summary>
        /// Service program ID, 6 characters.
        /// </summary>
        public const string SPID = "SPID";
        /// <summary>
        /// Service parameter name defined in specific standard. If service parameter is defined as an object attribute, this is completely the same as ATTRID except format restrictions above.
        /// </summary>
        public const string SPNAME = "SPNAME";
        /// <summary>
        /// Service program results.
        /// </summary>
        public const string SPR = "SPR";
        /// <summary>
        /// Service parameter value, corresponding to SPNAME. If service parameter is defined as an object attribute, this is completely the same as ATTRDATA except format restrictions for the attribute.
        /// </summary>
        public const string SPVAL = "SPVAL";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string SRAACK = "SRAACK";
        /// <summary>
        /// Enable/Disable entire SRS functionalities. Default is Disabled.
        /// </summary>
        public const string SRSSWITCH = "SRSSWITCH";
        /// <summary>
        /// Indicates the success or failure of a requested action. Two characters.
        /// </summary>
        public const string SSACK = "SSACK";
        /// <summary>
        /// Indicates an action to be performed by the subsystem.
        /// </summary>
        public const string SSCMD = "SSCMD";
        /// <summary>
        /// Information concerning the result of the service.
        /// </summary>
        public const string SSSACK = "SSSACK";
        /// <summary>
        /// Provides status information for a subsystem component. Used in the data item STATUSLIST.
        /// </summary>
        public const string STATUS = "STATUS";
        /// <summary>
        /// A list of STATUS data sent in a fixed order. STATUSLIST has the following form.
        /// </summary>
        public const string STATUSLIST = "STATUSLIST";
        /// <summary>
        /// Text string describing the corresponding status response. Maximum length of 80 characters.
        /// </summary>
        public const string STATUSTXT = "STATUSTXT";
        /// <summary>
        /// String template. ASCII text string acceptable to equipment as a parameter value. A data string matches a template string if the data string is at least as long as the template and each character of the data string matches the corresponding character of the template. A null list indicates all user data is acceptable to the machine.
        /// </summary>
        public const string STEMP = "STEMP";
        /// <summary>
        /// Sample time, 12, 16 bytes, or Extended format as specified by the TimeFormat equipment constant value setting.
        /// </summary>
        public const string STIME = "STIME";
        /// <summary>
        /// Spool Stream Acknowledge.
        /// </summary>
        public const string STRACK = "STRACK";
        /// <summary>
        /// Stream Identification.
        /// </summary>
        public const string STRID = "STRID";
        /// <summary>
        /// Starting position in die coordinate position. Must be in (X,Y) order.
        /// </summary>
        public const string STRP = "STRP";
        /// <summary>
        /// Status variable value.
        /// </summary>
        public const string SV = "SV";
        /// <summary>
        /// Service acceptance acknowledge code, 1 byte.
        /// </summary>
        public const string SVCACK = "SVCACK";
        /// <summary>
        /// Service name provided on specified object asking by the host.
        /// </summary>
        public const string SVCNAME = "SVCNAME";
        /// <summary>
        /// Status variable ID.
        /// </summary>
        public const string SVID = "SVID";
        /// <summary>
        /// Status Variable Name.
        /// </summary>
        public const string SVNAME = "SVNAME";
        /// <summary>
        /// Identifies where a request for action or data is to be applied. If text, conforms to OBJSPEC.
        /// </summary>
        public const string TARGETID = "TARGETID";
        /// <summary>
        /// Contains the unique identifier (uid) of the PDE that is the starting point for the verification process.
        /// </summary>
        public const string TARGETPDE = "TARGETPDE";
        /// <summary>
        /// Object specifier of target object.
        /// </summary>
        public const string TARGETSPEC = "TARGETSPEC";
        /// <summary>
        /// Indicates success or failure.
        /// </summary>
        public const string TBLACK = "TBLACK";
        /// <summary>
        /// Provides information about the table or parts of the table being transferred or requested. Enumerated.
        /// </summary>
        public const string TBLCMD = "TBLCMD";
        /// <summary>
        /// Table element. The first table element in a row is used to identify the row.
        /// </summary>
        public const string TBLELT = "TBLELT";
        /// <summary>
        /// Table identifier. Text conforming to the requirements of OBJSPEC.
        /// </summary>
        public const string TBLID = "TBLID";
        /// <summary>
        /// A reserved text string to denote the format and application of the table. Text conforming to the requirements of OBJSPEC.
        /// </summary>
        public const string TBLTYP = "TBLTYP";
        /// <summary>
        /// TCID is the identifier of the TransferContainer.
        /// </summary>
        public const string TCID = "TCID";
        /// <summary>
        /// A single line of characters.
        /// </summary>
        public const string TEXT = "TEXT";
        /// <summary>
        /// Equipment acknowledgement code, 1 byte.
        /// </summary>
        public const string TIAACK = "TIAACK";
        /// <summary>
        /// Time Acknowledge Code, 1 byte.
        /// </summary>
        public const string TIACK = "TIACK";
        /// <summary>
        /// Terminal number, 1 byte.
        /// </summary>
        public const string TID = "TID";
        /// <summary>
        /// Time of day, 12, 16 bytes, or Extended format as specified by the TimeFormat equipment constant value setting.
        /// </summary>
        public const string TIME = "TIME";
        /// <summary>
        /// Timestamp in 12, 16 bytes, or Extended format indicating the time of an event, which encodes time as specified by the TimeFormat equipment constant value setting.
        /// </summary>
        public const string TIMESTAMP = "TIMESTAMP";
        /// <summary>
        /// Total samples to be made.
        /// </summary>
        public const string TOTSMP = "TOTSMP";
        /// <summary>
        /// Tells whether the related transfer activity was successful (= True) or unsuccessful (= False).
        /// </summary>
        public const string TRACK = "TRACK";
        /// <summary>
        /// Size, in bytes, of the TransferContainer proposed for transfer.
        /// </summary>
        public const string TRANSFERSIZE = "TRANSFERSIZE";
        /// <summary>
        /// Equipment assigned identifier for an atomic transfer.
        /// </summary>
        public const string TRATOMICID = "TRATOMICID";
        /// <summary>
        /// A Trace Object attribute for a control flag which, if set TRUE, causes the Trace Object to delete itself when it has completed a report.
        /// </summary>
        public const string TRAUTOD = "TRAUTOD";
        /// <summary>
        /// For each atomic transfer, this data item tells the equipment if it should automatically start the handoff when ready (= TRUE) or await the host’s ‘StartHandoff’ command (= FALSE) following setup. This data item only affects the primary transfer partner.
        /// </summary>
        public const string TRAUTOSTART = "TRAUTOSTART";
        /// <summary>
        /// Identifier of the transfer job-related command to be executed. Possible values.
        /// </summary>
        public const string TRCMDNAME = "TRCMDNAME";
        /// <summary>
        /// Direction of handoff.
        /// </summary>
        public const string TRDIR = "TRDIR";
        /// <summary>
        /// Trace request ID.
        /// </summary>
        public const string TRID = "TRID";
        /// <summary>
        /// Equipment assigned identifier for the transfer job.
        /// </summary>
        public const string TRJOBID = "TRJOBID";
        /// <summary>
        /// Milestone for a transfer job (e.g., started or complete).
        /// </summary>
        public const string TRJOBMS = "TRJOBMS";
        /// <summary>
        /// Host assigned identifier for the transfer job. Limited to a maximum of 80 characters.
        /// </summary>
        public const string TRJOBNAME = "TRJOBNAME";
        /// <summary>
        /// Common identifier for the atomic transfer used by the transfer partners to confirm that they are working on the same host-defined task.
        /// </summary>
        public const string TRLINK = "TRLINK";
        /// <summary>
        /// Identifier of the material location involved with the transfer. For one transfer partner, this will represent the designated source location for the material to be sent. For the other transfer partner, it will represent the designated destination location for the material to be received.
        /// </summary>
        public const string TRLOCATION = "TRLOCATION";
        /// <summary>
        /// Identifier for the material (transfer object) to be transferred.
        /// </summary>
        public const string TROBJNAME = "TROBJNAME";
        /// <summary>
        /// Type of object to be transferred.
        /// </summary>
        public const string TROBJTYPE = "TROBJTYPE";
        /// <summary>
        /// Identifier of the equipment port to be used for the handoff.
        /// </summary>
        public const string TRPORT = "TRPORT";
        /// <summary>
        /// Name of the equipment which will serve as the other transfer partner for this atomic transfer. This corresponds to EQNAME.
        /// </summary>
        public const string TRPTNR = "TRPTNR";
        /// <summary>
        /// Identifier of the transfer partner’s port to be used for the transfer.
        /// </summary>
        public const string TRPTPORT = "TRPTPORT";
        /// <summary>
        /// Name of the transfer recipe for this handoff. Limited to a maximum of 80 characters.
        /// </summary>
        public const string TRRCP = "TRRCP";
        /// <summary>
        /// Tells whether the equipment is to be the primary or secondary transfer partner.
        /// </summary>
        public const string TRROLE = "TRROLE";
        /// <summary>
        /// A Trace Object attribute which holds the value for sampling interval time.
        /// </summary>
        public const string TRSPER = "TRSPER";
        /// <summary>
        /// Tells whether the equipment is to be an active or passive participant in the transfer.
        /// </summary>
        public const string TRTYPE = "TRTYPE";
        /// <summary>
        /// Transfer status of input port, 1 byte.
        /// </summary>
        public const string TSIP = "TSIP";
        /// <summary>
        /// Transfer status of output port, 1 byte.
        /// </summary>
        public const string TSOP = "TSOP";
        /// <summary>
        /// Time to completion.
        /// </summary>
        public const string TTC = "TTC";
        /// <summary>
        /// Identifier of the Type of the recipe.
        /// </summary>
        public const string TYPEID = "TYPEID";
        /// <summary>
        /// Contains a unique identifier for a PDE.
        /// </summary>
        public const string UID = "UID";
        /// <summary>
        /// Upper limit for numeric value.
        /// </summary>
        public const string ULIM = "ULIM";
        /// <summary>
        /// Unformatted Process Program Length.
        /// </summary>
        public const string UNFLEN = "UNFLEN";
        /// <summary>
        /// Units Identifier.
        /// </summary>
        public const string UNITS = "UNITS";
        /// <summary>
        /// A variable limit attribute which defines the upper boundary of the deadband of a limit. The value applies to a single limit (LIMITID) for a specified VID. Thus, UPPERDB and LOWERDB as a pair define a limit.
        /// </summary>
        public const string UPPERDB = "UPPERDB";
        /// <summary>
        /// Variable data.
        /// </summary>
        public const string V = "V";
        /// <summary>
        /// Optional unique identifier of recipes.
        /// </summary>
        public const string VERID = "VERID";
        /// <summary>
        /// Selects whether to check only the target PDE or all associated PDEs within a multi-part recipe.
        /// </summary>
        public const string VERIFYDEPTH = "VERIFYDEPTH";
        /// <summary>
        /// Verification result.
        /// </summary>
        public const string VERIFYRSPSTAT = "VERIFYRSPSTAT";
        /// <summary>
        /// Boolean.
        /// </summary>
        public const string VERIFYSUCCESS = "VERIFYSUCCESS";
        /// <summary>
        /// Choice of the type of verification to perform.
        /// </summary>
        public const string VERIFYTYPE = "VERIFYTYPE";
        /// <summary>
        /// Variable ID.
        /// </summary>
        public const string VID = "VID";
        /// <summary>
        /// Variable Limit Attribute Acknowledge Code, 1 byte.
        /// </summary>
        public const string VLAACK = "VLAACK";
        /// <summary>
        /// X-axis die size (index).
        /// </summary>
        public const string XDIES = "XDIES";
        /// <summary>
        /// X and Y Coordinate Position. Must be in (X,Y) order.
        /// </summary>
        public const string XYPOS = "XYPOS";
        /// <summary>
        /// Y-axis die size (index).
        /// </summary>
        public const string YDIES = "YDIES";
    }
    #endregion
}