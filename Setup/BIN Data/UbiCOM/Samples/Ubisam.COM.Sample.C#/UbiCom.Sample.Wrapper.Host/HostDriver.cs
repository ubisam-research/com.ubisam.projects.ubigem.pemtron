using System;
using System.Collections;
using System.Collections.Generic;

using UbiCom.Net.Structure;
using UbiCom.Net.Driver;

namespace UbiCom.Sample.Wrapper.Host
{
    #region HostDriver
    /// <summary>
    ///
    /// </summary>
    public class HostDriver : HSMSDriver
    {
        #region [Delegate / Event Define]
        public delegate void S1F1ReceivedEventHandler(CS1F1 S1F1);
        public delegate void S6F11ReceivedEventHandler(CS6F11 S6F11);
        public delegate void S2F79ReceivedEventHandler(CS2F79 S2F79);
        public delegate void S1F2_ToHostReceivedEventHandler(SECSMessage primaryMessage, CS1F2_ToHost S1F2_ToHost);
        public delegate void S2F42ReceivedEventHandler(SECSMessage primaryMessage, CS2F42 S2F42);

        public event S1F1ReceivedEventHandler OnS1F1Received;
        public event S6F11ReceivedEventHandler OnS6F11Received;
        public event S2F79ReceivedEventHandler OnS2F79Received;
        public event S1F2_ToHostReceivedEventHandler OnS1F2_ToHostReceived;
        public event S2F42ReceivedEventHandler OnS2F42Received;
        #endregion

        #region Constructor
        public HostDriver()
        {
            this.OnReceivedPrimaryMessage += new ReceivedPrimaryMessageEventHandler(HostDriver_OnReceivedPrimaryMessage);
            this.OnReceivedSecondaryMessage += new ReceivedSecondaryMessageEventHandler(HostDriver_OnReceivedSecondaryMessage);
        }
        #endregion

        // Driver Event
        #region HostDriver_OnReceivedPrimaryMessage
        void HostDriver_OnReceivedPrimaryMessage(object sender, SECSMessage message)
        {
            switch (message.Name)
            {
                case "S1F1":
                    CS1F1 S1F1 = new CS1F1(this, "S1F1");

                    S1F1.SystemBytes = message.SystemBytes;
                    S1F1.Body = message.Body;

                    ConvertWrapper(S1F1, message);

                    if (this.OnS1F1Received != null)
                        this.OnS1F1Received(S1F1);

                    break;
                case "S6F11":
                    CS6F11 S6F11 = new CS6F11(this, "S6F11");

                    S6F11.SystemBytes = message.SystemBytes;
                    S6F11.Body = message.Body;

                    ConvertWrapper(S6F11, message);

                    if (this.OnS6F11Received != null)
                        this.OnS6F11Received(S6F11);

                    break;
                case "S2F79":
                    CS2F79 S2F79 = new CS2F79(this, "S2F79");

                    S2F79.SystemBytes = message.SystemBytes;
                    S2F79.Body = message.Body;

                    ConvertWrapper(S2F79, message);

                    if (this.OnS2F79Received != null)
                        this.OnS2F79Received(S2F79);

                    break;
            }
        }
        #endregion
        #region HostDriver_OnReceivedSecondaryMessage
        void HostDriver_OnReceivedSecondaryMessage(object sender, SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
            switch (secondaryMessage.Name)
            {
                case "S1F2_ToHost":
                    CS1F2_ToHost S1F2_ToHost = new CS1F2_ToHost(this, "S1F2_ToHost");

                    S1F2_ToHost.SystemBytes = secondaryMessage.SystemBytes;
                    S1F2_ToHost.Body = secondaryMessage.Body;

                    ConvertWrapper(S1F2_ToHost, secondaryMessage);

                    if (this.OnS1F2_ToHostReceived != null)
                        this.OnS1F2_ToHostReceived(primaryMessage, S1F2_ToHost);

                    break;
                case "S2F42":
                    CS2F42 S2F42 = new CS2F42(this, "S2F42");

                    S2F42.SystemBytes = secondaryMessage.SystemBytes;
                    S2F42.Body = secondaryMessage.Body;

                    ConvertWrapper(S2F42, secondaryMessage);

                    if (this.OnS2F42Received != null)
                        this.OnS2F42Received(primaryMessage, S2F42);

                    break;
            }
        }
        #endregion

        #region [Get Class Instance]
        public CS1F1 S1F1()
        {
            return new CS1F1(this, "S1F1");
        }

        public CS1F2_ToHost S1F2_ToHost()
        {
            return new CS1F2_ToHost(this, "S1F2_ToHost");
        }

        public CS1F2_ToEqp S1F2_ToEqp()
        {
            return new CS1F2_ToEqp(this, "S1F2_ToEqp");
        }

        public CS2F41 S2F41()
        {
            return new CS2F41(this, "S2F41");
        }

        public CS2F42 S2F42()
        {
            return new CS2F42(this, "S2F42");
        }

        public CS6F11 S6F11()
        {
            return new CS6F11(this, "S6F11");
        }

        public CS6F12 S6F12()
        {
            return new CS6F12(this, "S6F12");
        }

        public CS2F79 S2F79()
        {
            return new CS2F79(this, "S2F79");
        }

        public CS2F80_L0 S2F80_L0()
        {
            return new CS2F80_L0(this, "S2F80_L0");
        }

        public CS2F80_L21 S2F80_L21()
        {
            return new CS2F80_L21(this, "S2F80_L21");
        }
        #endregion

        #region [Converter]
        private void ConvertWrapper(SECSMessageWrapper message, SECSMessage source)
        {
            message.SystemBytes = source.SystemBytes;
            message.UserData = source.UserData;

            for (int i = 0; i < source.Body.Item.Count; i++)
            {
                if (message.Items[i].Format == SECSItemFormat.X)
                {
                    message.Items[i].Value = source.Body.Item.Items[i];
                }
                else
                {
                    if (source.Body.Item.Items[i].Format == SECSItemFormat.L)
                    {
                        ConvertWrapper(message.Items, source.Body.Item.Items[i]);
                    }
                    else
                    {
                        message.Items[i].Value = GetValue(source.Body.Item.Items[i]);
                    }
                }
            }
        }

        private void ConvertWrapper(SECSItemCollectionWrapper items, SECSItem sourceItem)
        {
            SECSItemWrapper item;

            if (sourceItem.IsFixed == true)
            {
                if (items[sourceItem.Name].Format == SECSItemFormat.L)
                {
                    item = items[sourceItem.Name];
                    item.MakeList(sourceItem.Name, sourceItem.Length);
                    List<SECSItemWrapper> list = (item.Value as SECSItemCollectionWrapper).AsList;

                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            SECSItemWrapper itemWrapper = list[i] as SECSItemWrapper;

                            if (itemWrapper.Format == SECSItemFormat.L)
                            {
                                itemWrapper.MakeList(sourceItem.SubItem.Items[i].Name, sourceItem.SubItem.Items[i].Length);

                                if (itemWrapper.Length > 0)
                                {
                                    if (itemWrapper.IsFixed == false)
                                    {
                                        IList childList = itemWrapper.Value as IList;

                                        for (int j = 0; j < childList.Count; j++)
                                        {
                                            ConvertWrapper((SECSItemCollectionWrapper)childList[j], sourceItem.SubItem.Items[i].SubItem[j]);
                                        }
                                    }
                                    else
                                    {
                                        IList childList = (itemWrapper.Value as SECSItemCollectionWrapper).AsList;

                                        for (int j = 0; j < childList.Count; j++)
                                        {
                                            SECSItemWrapper childItemWrapper = childList[j] as SECSItemWrapper;

                                            if (childItemWrapper.Format == SECSItemFormat.L)
                                            {
                                                childItemWrapper.MakeList(sourceItem.SubItem.Items[i].SubItem[j].Name, sourceItem.SubItem.Items[i].SubItem[j].Length);
                                                IList childList2 = childItemWrapper.Value as IList;

                                                for (int k = 0; k < childList2.Count; k++)
                                                {
                                                    ConvertWrapper((SECSItemCollectionWrapper)childList2[k], sourceItem.SubItem.Items[i].SubItem[j].SubItem[k]);
                                                }
                                            }
                                            else if (childItemWrapper.Format == SECSItemFormat.X)
                                            {
                                                childItemWrapper.Value = sourceItem.SubItem.Items[i].SubItem[j];
                                            }
                                            else
                                            {
                                                childItemWrapper.Value = GetValue(sourceItem.SubItem.Items[i].SubItem[j]);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (itemWrapper.Format == SECSItemFormat.X)
                            {
                                itemWrapper.Value = sourceItem.SubItem.Items[i];
                            }
                            else
                            {
                                itemWrapper.Value = GetValue(sourceItem.SubItem.Items[i]);
                            }
                        }
                    }
                }
                else if (items[sourceItem.Name].Format == SECSItemFormat.X)
                {
                    items[sourceItem.Name].Value = sourceItem;
                }
                else
                {
                    if (sourceItem.SubItem.Count > 0)
                    {
                        for (int i = 0; i < sourceItem.Length; i++)
                        {
                            if (sourceItem.SubItem.Items[i].Format == SECSItemFormat.L)
                            {
                                ConvertWrapper(items, sourceItem);
                            }
                            else
                            {
                                items[i].Value = GetValue(sourceItem);
                            }
                        }
                    }
                    else
                    {
                        if (sourceItem.Format == SECSItemFormat.L)
                        {
                            ConvertWrapper(items, sourceItem);
                        }
                        else
                        {
                            items[sourceItem.Name].Value = GetValue(sourceItem);
                        }
                    }
                }
            }
            else
            {
                if (items[sourceItem.Name].Format == SECSItemFormat.L)
                {
                    item = items[sourceItem.Name];
                    item.MakeList(sourceItem.Name, sourceItem.Length);
                    IList list = item.Value as IList;

                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            ConvertWrapper((SECSItemCollectionWrapper)list[i], sourceItem.SubItem.Items[i]);
                        }
                    }
                }
                else if (items[sourceItem.Name].Format == SECSItemFormat.X)
                {
                    items[sourceItem.Name].Value = sourceItem;
                }
                else
                {
                    for (int i = 0; i < sourceItem.SubItem.Count; i++)
                    {
                        ConvertWrapper(items, sourceItem.SubItem.Items[i]);
                    }
                }
            }
        }

        private object GetValue(SECSItem item)
        {
            object result = null;

            if (item.Format == SECSItemFormat.A || item.Format == SECSItemFormat.J)
            {
                if (item.IsFixed == true)
                {
                    result = ((string)item.Value).PadRight(item.Length);
                }
                else
                {
                    result = (string)item.Value;
                }
            }
            else
            {
                if (item.Length > 1)
                {
                    switch (item.Format)
                    {
                        case SECSItemFormat.U1:
                        case SECSItemFormat.B:
                            result = (byte[])item.Value;
                            break;
                        case SECSItemFormat.I1:
                            result = (sbyte[])item.Value;
                            break;
                        case SECSItemFormat.Boolean:
                            result = (bool[])item.Value;
                            break;
                        case SECSItemFormat.I2:
                            result = (short[])item.Value;
                            break;
                        case SECSItemFormat.U2:
                            result = (ushort[])item.Value;
                            break;
                        case SECSItemFormat.I4:
                            result = (int[])item.Value;
                            break;
                        case SECSItemFormat.U4:
                            result = (uint[])item.Value;
                            break;
                        case SECSItemFormat.F4:
                            result = (float[])item.Value;
                            break;
                        case SECSItemFormat.I8:
                            result = (long[])item.Value;
                            break;
                        case SECSItemFormat.U8:
                            result = (ulong[])item.Value;
                            break;
                        case SECSItemFormat.F8:
                            result = (double[])item.Value;
                            break;
                    }
                }
                else
                {
                    switch (item.Format)
                    {
                        case SECSItemFormat.U1:
                        case SECSItemFormat.B:
                            result = (byte)item.Value;
                            break;
                        case SECSItemFormat.I1:
                            result = (sbyte)item.Value;
                            break;
                        case SECSItemFormat.Boolean:
                            result = (bool)item.Value;
                            break;
                        case SECSItemFormat.I2:
                            result = (short)item.Value;
                            break;
                        case SECSItemFormat.U2:
                            result = (ushort)item.Value;
                            break;
                        case SECSItemFormat.I4:
                            result = (int)item.Value;
                            break;
                        case SECSItemFormat.U4:
                            result = (uint)item.Value;
                            break;
                        case SECSItemFormat.F4:
                            result = (float)item.Value;
                            break;
                        case SECSItemFormat.I8:
                            result = (long)item.Value;
                            break;
                        case SECSItemFormat.U8:
                            result = (ulong)item.Value;
                            break;
                        case SECSItemFormat.F8:
                            result = (double)item.Value;
                            break;
                    }
                }
            }

            return result;
        }
        #endregion
    }
    #endregion

    #region [Message Class]
    #region [CS1F1 - Are You There Request]
    [Serializable]
    public class CS1F1 : SECSMessageBoth
    {

        #region Constructor
        public CS1F1(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 1;
            this.Function = 1;
            this.WaitBit = true;
            this.Direction = SECSMessageDirection.Both;
            this.AutoReply = false;

        }
        #endregion
    }
    #endregion

    #region [CS1F2_ToHost - Are You There Request]
    [Serializable]
    public class CS1F2_ToHost : SECSMessageReceive
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            public string MDLN
            {
                get { return (string)this.AsDictionary["MDLN"].Value; }
                set { this.AsDictionary["MDLN"].Value = value; }
            }

            public string SOFTREV
            {
                get { return (string)this.AsDictionary["SOFTREV"].Value; }
                set { this.AsDictionary["SOFTREV"].Value = value; }
            }

            #region Constructor
            public CL()
            {
                Add(new SECSItemWrapper("MDLN", SECSItemFormat.A, 20, false, "UbiSAM"));
                Add(new SECSItemWrapper("SOFTREV", SECSItemFormat.A, 20, false, "SWREV"));
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        public string MDLN
        {
            get { return this._L.MDLN; }
            set { this._L.MDLN = value; }
        }
        public string SOFTREV
        {
            get { return this._L.SOFTREV; }
            set { this._L.SOFTREV = value; }
        }
        #endregion

        #region Constructor
        public CS1F2_ToHost(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 1;
            this.Function = 2;
            this.WaitBit = false;
            this.Direction = SECSMessageDirection.ToHost;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 2, true, this._L));
        }
        #endregion
    }
    #endregion

    #region [CS1F2_ToEqp - Are You There Request]
    [Serializable]
    public class CS1F2_ToEqp : SECSMessageReply
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            #region Constructor
            public CL()
            {
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        #endregion

        #region Constructor
        public CS1F2_ToEqp(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 1;
            this.Function = 2;
            this.WaitBit = false;
            this.Direction = SECSMessageDirection.ToEquipment;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 0, true, this._L));
        }
        #endregion
    }
    #endregion

    #region [CS2F41 - Host Command Send]
    [Serializable]
    public class CS2F41 : SECSMessageRequest
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            public string RCMD
            {
                get { return (string)this.AsDictionary["RCMD"].Value; }
                set { this.AsDictionary["RCMD"].Value = value; }
            }

            #region [CPCOUNT]
            public class CCPCOUNT : SECSItemCollectionWrapper
            {
                #region [CPINFO]
                public class CCPINFO : SECSItemCollectionWrapper
                {
                    public string CPNAME
                    {
                        get { return (string)this.AsDictionary["CPNAME"].Value; }
                        set { this.AsDictionary["CPNAME"].Value = value; }
                    }

                    public SECSItem CPVAL
                    {
                        get { return (SECSItem)this.AsDictionary["CPVAL"].Value; }
                        set { this.AsDictionary["CPVAL"].Value = value; }
                    }

                    #region Constructor
                    public CCPINFO()
                    {
                        Add(new SECSItemWrapper("CPNAME", SECSItemFormat.A, 1000, false, string.Empty));
                        Add(new SECSItemWrapper("CPVAL", SECSItemFormat.X, 1000, false, null));
                    }
                    #endregion
                }
                #endregion

                #region [Properties]
                private CCPINFO _CPINFO;
                public string CPNAME
                {
                    get { return this._CPINFO.CPNAME; }
                    set { this._CPINFO.CPNAME = value; }
                }
                public SECSItem CPVAL
                {
                    get { return this._CPINFO.CPVAL; }
                    set { this._CPINFO.CPVAL = value; }
                }
                #endregion

                #region Constructor
                public CCPCOUNT()
                {
                    this._CPINFO = new CCPINFO();
                    Add(new SECSItemWrapper("CPINFO", SECSItemFormat.L, 2, true, this._CPINFO));
                }
                #endregion
            }
            #endregion

            #region [Properties]
            private List<CCPCOUNT> _CPCOUNT;
            public List<CCPCOUNT> CPCOUNT
            {
                get { return this._CPCOUNT; }
            }

            public int CPCOUNTCount
            {
                get { return this._CPCOUNT.Count; }
                set
                {
                    this._CPCOUNT.Clear();
                    this.AsDictionary["CPCOUNT"].Length = value;

                    for (int i = 0; i < value; i++)
                    {
                        this._CPCOUNT.Add(new CCPCOUNT());
                    }
                }
            }
            #endregion

            #region Constructor
            public CL()
            {
                Add(new SECSItemWrapper("RCMD", SECSItemFormat.A, 1000, false, string.Empty));
                this._CPCOUNT = new List<CCPCOUNT>();
                Add(new SECSItemWrapper("CPCOUNT", this._CPCOUNT));
            }
            #endregion

            #region MakeList
            public override void MakeList(string name, int count)
            {
                switch (name)
                {
                    case "CPCOUNT":
                        this.CPCOUNTCount = count;
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        public string RCMD
        {
            get { return this._L.RCMD; }
            set { this._L.RCMD = value; }
        }
        public List<CL.CCPCOUNT> CPCOUNT
        {
            get { return this._L.CPCOUNT; }
        }

        public int CPCOUNTCount
        {
            get { return this._L.CPCOUNTCount; }
            set { this._L.CPCOUNTCount = value; }
        }
        #endregion

        #region Constructor
        public CS2F41(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 2;
            this.Function = 41;
            this.WaitBit = true;
            this.Direction = SECSMessageDirection.ToEquipment;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 2, true, this._L));
        }
        #endregion
    }
    #endregion

    #region [CS2F42 - Host Command Send]
    [Serializable]
    public class CS2F42 : SECSMessageReceive
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            public byte HCACK
            {
                get { return (byte)this.AsDictionary["HCACK"].Value; }
                set { this.AsDictionary["HCACK"].Value = value; }
            }

            #region [CPCOUNT]
            public class CCPCOUNT : SECSItemCollectionWrapper
            {
                #region [CPINFO]
                public class CCPINFO : SECSItemCollectionWrapper
                {
                    public string CPNAME
                    {
                        get { return (string)this.AsDictionary["CPNAME"].Value; }
                        set { this.AsDictionary["CPNAME"].Value = value; }
                    }

                    public byte CPACK
                    {
                        get { return (byte)this.AsDictionary["CPACK"].Value; }
                        set { this.AsDictionary["CPACK"].Value = value; }
                    }

                    #region Constructor
                    public CCPINFO()
                    {
                        Add(new SECSItemWrapper("CPNAME", SECSItemFormat.A, 1000, false, string.Empty));
                        Add(new SECSItemWrapper("CPACK", SECSItemFormat.B, 1, true, (byte)0));
                    }
                    #endregion
                }
                #endregion

                #region [Properties]
                private CCPINFO _CPINFO;
                public string CPNAME
                {
                    get { return this._CPINFO.CPNAME; }
                    set { this._CPINFO.CPNAME = value; }
                }
                public byte CPACK
                {
                    get { return this._CPINFO.CPACK; }
                    set { this._CPINFO.CPACK = value; }
                }
                #endregion

                #region Constructor
                public CCPCOUNT()
                {
                    this._CPINFO = new CCPINFO();
                    Add(new SECSItemWrapper("CPINFO", SECSItemFormat.L, 2, true, this._CPINFO));
                }
                #endregion
            }
            #endregion

            #region [Properties]
            private List<CCPCOUNT> _CPCOUNT;
            public List<CCPCOUNT> CPCOUNT
            {
                get { return this._CPCOUNT; }
            }

            public int CPCOUNTCount
            {
                get { return this._CPCOUNT.Count; }
                set
                {
                    this._CPCOUNT.Clear();
                    this.AsDictionary["CPCOUNT"].Length = value;

                    for (int i = 0; i < value; i++)
                    {
                        this._CPCOUNT.Add(new CCPCOUNT());
                    }
                }
            }
            #endregion

            #region Constructor
            public CL()
            {
                Add(new SECSItemWrapper("HCACK", SECSItemFormat.B, 1, true, (byte)0));
                this._CPCOUNT = new List<CCPCOUNT>();
                Add(new SECSItemWrapper("CPCOUNT", this._CPCOUNT));
            }
            #endregion

            #region MakeList
            public override void MakeList(string name, int count)
            {
                switch (name)
                {
                    case "CPCOUNT":
                        this.CPCOUNTCount = count;
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        public byte HCACK
        {
            get { return this._L.HCACK; }
            set { this._L.HCACK = value; }
        }
        public List<CL.CCPCOUNT> CPCOUNT
        {
            get { return this._L.CPCOUNT; }
        }

        public int CPCOUNTCount
        {
            get { return this._L.CPCOUNTCount; }
            set { this._L.CPCOUNTCount = value; }
        }
        #endregion

        #region Constructor
        public CS2F42(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 2;
            this.Function = 42;
            this.WaitBit = false;
            this.Direction = SECSMessageDirection.ToHost;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 2, true, this._L));
        }
        #endregion
    }
    #endregion

    #region [CS6F11 - Event Report Send]
    [Serializable]
    public class CS6F11 : SECSMessageReceive
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            public int DATAID
            {
                get { return (int)this.AsDictionary["DATAID"].Value; }
                set { this.AsDictionary["DATAID"].Value = value; }
            }

            public int CEID
            {
                get { return (int)this.AsDictionary["CEID"].Value; }
                set { this.AsDictionary["CEID"].Value = value; }
            }

            #region [RPTIDCOUNT]
            public class CRPTIDCOUNT : SECSItemCollectionWrapper
            {
                #region [RPTINFO]
                public class CRPTINFO : SECSItemCollectionWrapper
                {
                    public int RPTID
                    {
                        get { return (int)this.AsDictionary["RPTID"].Value; }
                        set { this.AsDictionary["RPTID"].Value = value; }
                    }

                    #region [VCOUNT]
                    public class CVCOUNT : SECSItemCollectionWrapper
                    {
                        public SECSItem V
                        {
                            get { return (SECSItem)this.AsDictionary["V"].Value; }
                            set { this.AsDictionary["V"].Value = value; }
                        }

                        #region Constructor
                        public CVCOUNT()
                        {
                            Add(new SECSItemWrapper("V", SECSItemFormat.X, 10000, false, null));
                        }
                        #endregion
                    }
                    #endregion

                    #region [Properties]
                    private List<CVCOUNT> _VCOUNT;
                    public List<CVCOUNT> VCOUNT
                    {
                        get { return this._VCOUNT; }
                    }

                    public int VCOUNTCount
                    {
                        get { return this._VCOUNT.Count; }
                        set
                        {
                            this._VCOUNT.Clear();
                            this.AsDictionary["VCOUNT"].Length = value;

                            for (int i = 0; i < value; i++)
                            {
                                this._VCOUNT.Add(new CVCOUNT());
                            }
                        }
                    }
                    #endregion

                    #region Constructor
                    public CRPTINFO()
                    {
                        Add(new SECSItemWrapper("RPTID", SECSItemFormat.I4, 1, true, (int)0));
                        this._VCOUNT = new List<CVCOUNT>();
                        Add(new SECSItemWrapper("VCOUNT", this._VCOUNT));
                    }
                    #endregion

                    #region MakeList
                    public override void MakeList(string name, int count)
                    {
                        switch (name)
                        {
                            case "VCOUNT":
                                this.VCOUNTCount = count;
                                break;
                        }
                    }
                    #endregion
                }
                #endregion

                #region [Properties]
                private CRPTINFO _RPTINFO;
                public int RPTID
                {
                    get { return this._RPTINFO.RPTID; }
                    set { this._RPTINFO.RPTID = value; }
                }
                public List<CRPTINFO.CVCOUNT> VCOUNT
                {
                    get { return this._RPTINFO.VCOUNT; }
                }

                public int VCOUNTCount
                {
                    get { return this._RPTINFO.VCOUNTCount; }
                    set { this._RPTINFO.VCOUNTCount = value; }
                }
                #endregion

                #region Constructor
                public CRPTIDCOUNT()
                {
                    this._RPTINFO = new CRPTINFO();
                    Add(new SECSItemWrapper("RPTINFO", SECSItemFormat.L, 2, true, this._RPTINFO));
                }
                #endregion
            }
            #endregion

            #region [Properties]
            private List<CRPTIDCOUNT> _RPTIDCOUNT;
            public List<CRPTIDCOUNT> RPTIDCOUNT
            {
                get { return this._RPTIDCOUNT; }
            }

            public int RPTIDCOUNTCount
            {
                get { return this._RPTIDCOUNT.Count; }
                set
                {
                    this._RPTIDCOUNT.Clear();
                    this.AsDictionary["RPTIDCOUNT"].Length = value;

                    for (int i = 0; i < value; i++)
                    {
                        this._RPTIDCOUNT.Add(new CRPTIDCOUNT());
                    }
                }
            }
            #endregion

            #region Constructor
            public CL()
            {
                Add(new SECSItemWrapper("DATAID", SECSItemFormat.I4, 1, true, (int)0));
                Add(new SECSItemWrapper("CEID", SECSItemFormat.I4, 1, true, (int)0));
                this._RPTIDCOUNT = new List<CRPTIDCOUNT>();
                Add(new SECSItemWrapper("RPTIDCOUNT", this._RPTIDCOUNT));
            }
            #endregion

            #region MakeList
            public override void MakeList(string name, int count)
            {
                switch (name)
                {
                    case "RPTIDCOUNT":
                        this.RPTIDCOUNTCount = count;
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        public int DATAID
        {
            get { return this._L.DATAID; }
            set { this._L.DATAID = value; }
        }
        public int CEID
        {
            get { return this._L.CEID; }
            set { this._L.CEID = value; }
        }
        public List<CL.CRPTIDCOUNT> RPTIDCOUNT
        {
            get { return this._L.RPTIDCOUNT; }
        }

        public int RPTIDCOUNTCount
        {
            get { return this._L.RPTIDCOUNTCount; }
            set { this._L.RPTIDCOUNTCount = value; }
        }
        #endregion

        #region Constructor
        public CS6F11(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 6;
            this.Function = 11;
            this.WaitBit = true;
            this.Direction = SECSMessageDirection.ToHost;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 3, true, this._L));
        }
        #endregion
    }
    #endregion

    #region [CS6F12 - Event Report Send]
    [Serializable]
    public class CS6F12 : SECSMessageReply
    {

        #region [Properties]
        public byte ACKC6
        {
            get { return (byte)this.Items["ACKC6"].Value; }
            set { this.Items["ACKC6"].Value = value; }
        }
        #endregion

        #region Constructor
        public CS6F12(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 6;
            this.Function = 12;
            this.WaitBit = false;
            this.Direction = SECSMessageDirection.ToEquipment;
            this.AutoReply = false;

            Add(new SECSItemWrapper("ACKC6", SECSItemFormat.B, 1, true, (byte)0));
        }
        #endregion
    }
    #endregion

    #region [CS2F79 - Glass Data Request]
    [Serializable]
    public class CS2F79 : SECSMessageReceive
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            public ushort UNITID
            {
                get { return (ushort)this.AsDictionary["UNITID"].Value; }
                set { this.AsDictionary["UNITID"].Value = value; }
            }

            public string GLSID
            {
                get { return (string)this.AsDictionary["GLSID"].Value; }
                set { this.AsDictionary["GLSID"].Value = value; }
            }

            public ushort GLSCODE
            {
                get { return (ushort)this.AsDictionary["GLSCODE"].Value; }
                set { this.AsDictionary["GLSCODE"].Value = value; }
            }

            public string REQOPTION
            {
                get { return (string)this.AsDictionary["REQOPTION"].Value; }
                set { this.AsDictionary["REQOPTION"].Value = value; }
            }

            #region Constructor
            public CL()
            {
                Add(new SECSItemWrapper("UNITID", SECSItemFormat.U2, 1, true, (ushort)0));
                Add(new SECSItemWrapper("GLSID", SECSItemFormat.A, 16, true, string.Empty));
                Add(new SECSItemWrapper("GLSCODE", SECSItemFormat.U2, 1, true, (ushort)0));
                Add(new SECSItemWrapper("REQOPTION", SECSItemFormat.A, 1, true, string.Empty));
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        public ushort UNITID
        {
            get { return this._L.UNITID; }
            set { this._L.UNITID = value; }
        }
        public string GLSID
        {
            get { return this._L.GLSID; }
            set { this._L.GLSID = value; }
        }
        public ushort GLSCODE
        {
            get { return this._L.GLSCODE; }
            set { this._L.GLSCODE = value; }
        }
        public string REQOPTION
        {
            get { return this._L.REQOPTION; }
            set { this._L.REQOPTION = value; }
        }
        #endregion

        #region Constructor
        public CS2F79(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 2;
            this.Function = 79;
            this.WaitBit = true;
            this.Direction = SECSMessageDirection.ToHost;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 4, true, this._L));
        }
        #endregion
    }
    #endregion

    #region [CS2F80_L0 - Glass Data]
    [Serializable]
    public class CS2F80_L0 : SECSMessageReply
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            public ushort UNITID
            {
                get { return (ushort)this.AsDictionary["UNITID"].Value; }
                set { this.AsDictionary["UNITID"].Value = value; }
            }

            public byte ACKC2
            {
                get { return (byte)this.AsDictionary["ACKC2"].Value; }
                set { this.AsDictionary["ACKC2"].Value = value; }
            }

            #region [GLSINFO]
            public class CGLSINFO : SECSItemCollectionWrapper
            {
                #region Constructor
                public CGLSINFO()
                {
                }
                #endregion
            }
            #endregion

            #region [Properties]
            private CGLSINFO _GLSINFO;
            #endregion

            #region Constructor
            public CL()
            {
                Add(new SECSItemWrapper("UNITID", SECSItemFormat.U2, 1, true, (ushort)0));
                Add(new SECSItemWrapper("ACKC2", SECSItemFormat.B, 1, true, (byte)0));
                this._GLSINFO = new CGLSINFO();
                Add(new SECSItemWrapper("GLSINFO", SECSItemFormat.L, 0, true, this._GLSINFO));
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        public ushort UNITID
        {
            get { return this._L.UNITID; }
            set { this._L.UNITID = value; }
        }
        public byte ACKC2
        {
            get { return this._L.ACKC2; }
            set { this._L.ACKC2 = value; }
        }
        #endregion

        #region Constructor
        public CS2F80_L0(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 2;
            this.Function = 80;
            this.WaitBit = false;
            this.Direction = SECSMessageDirection.ToEquipment;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 3, true, this._L));
        }
        #endregion
    }
    #endregion

    #region [CS2F80_L21 - Glass Data]
    [Serializable]
    public class CS2F80_L21 : SECSMessageReply
    {
        #region [L]
        public class CL : SECSItemCollectionWrapper
        {
            public ushort UNITID
            {
                get { return (ushort)this.AsDictionary["UNITID"].Value; }
                set { this.AsDictionary["UNITID"].Value = value; }
            }

            public byte ACKC2
            {
                get { return (byte)this.AsDictionary["ACKC2"].Value; }
                set { this.AsDictionary["ACKC2"].Value = value; }
            }

            #region [GLSINFO]
            public class CGLSINFO : SECSItemCollectionWrapper
            {
                public string LOTID
                {
                    get { return (string)this.AsDictionary["LOTID"].Value; }
                    set { this.AsDictionary["LOTID"].Value = value; }
                }

                public string CSTID
                {
                    get { return (string)this.AsDictionary["CSTID"].Value; }
                    set { this.AsDictionary["CSTID"].Value = value; }
                }

                public string SLOTID
                {
                    get { return (string)this.AsDictionary["SLOTID"].Value; }
                    set { this.AsDictionary["SLOTID"].Value = value; }
                }

                public string RECIPEID
                {
                    get { return (string)this.AsDictionary["RECIPEID"].Value; }
                    set { this.AsDictionary["RECIPEID"].Value = value; }
                }

                public string PRCID
                {
                    get { return (string)this.AsDictionary["PRCID"].Value; }
                    set { this.AsDictionary["PRCID"].Value = value; }
                }

                public string GLSODR
                {
                    get { return (string)this.AsDictionary["GLSODR"].Value; }
                    set { this.AsDictionary["GLSODR"].Value = value; }
                }

                public string GLSID
                {
                    get { return (string)this.AsDictionary["GLSID"].Value; }
                    set { this.AsDictionary["GLSID"].Value = value; }
                }

                public string GLSJUDGE
                {
                    get { return (string)this.AsDictionary["GLSJUDGE"].Value; }
                    set { this.AsDictionary["GLSJUDGE"].Value = value; }
                }

                public string PNLIF
                {
                    get { return (string)this.AsDictionary["PNLIF"].Value; }
                    set { this.AsDictionary["PNLIF"].Value = value; }
                }

                public string SUBMDLIF
                {
                    get { return (string)this.AsDictionary["SUBMDLIF"].Value; }
                    set { this.AsDictionary["SUBMDLIF"].Value = value; }
                }

                public byte PORTID
                {
                    get { return (byte)this.AsDictionary["PORTID"].Value; }
                    set { this.AsDictionary["PORTID"].Value = value; }
                }

                public ushort GLSCODE
                {
                    get { return (ushort)this.AsDictionary["GLSCODE"].Value; }
                    set { this.AsDictionary["GLSCODE"].Value = value; }
                }

                public byte REINPUT
                {
                    get { return (byte)this.AsDictionary["REINPUT"].Value; }
                    set { this.AsDictionary["REINPUT"].Value = value; }
                }

                public string GLSTHICK
                {
                    get { return (string)this.AsDictionary["GLSTHICK"].Value; }
                    set { this.AsDictionary["GLSTHICK"].Value = value; }
                }

                public string PARTNUM
                {
                    get { return (string)this.AsDictionary["PARTNUM"].Value; }
                    set { this.AsDictionary["PARTNUM"].Value = value; }
                }

                public string PRODTYPE
                {
                    get { return (string)this.AsDictionary["PRODTYPE"].Value; }
                    set { this.AsDictionary["PRODTYPE"].Value = value; }
                }

                public string ATTRIBUTE
                {
                    get { return (string)this.AsDictionary["ATTRIBUTE"].Value; }
                    set { this.AsDictionary["ATTRIBUTE"].Value = value; }
                }

                public string GLSTYPE
                {
                    get { return (string)this.AsDictionary["GLSTYPE"].Value; }
                    set { this.AsDictionary["GLSTYPE"].Value = value; }
                }

                public string KEYID
                {
                    get { return (string)this.AsDictionary["KEYID"].Value; }
                    set { this.AsDictionary["KEYID"].Value = value; }
                }

                #region [LSDCOUNT]
                public class CLSDCOUNT : SECSItemCollectionWrapper
                {
                    #region [LSDINFO]
                    public class CLSDINFO : SECSItemCollectionWrapper
                    {
                        public string LSDITEM
                        {
                            get { return (string)this.AsDictionary["LSDITEM"].Value; }
                            set { this.AsDictionary["LSDITEM"].Value = value; }
                        }

                        public string LSDVALUE
                        {
                            get { return (string)this.AsDictionary["LSDVALUE"].Value; }
                            set { this.AsDictionary["LSDVALUE"].Value = value; }
                        }

                        #region Constructor
                        public CLSDINFO()
                        {
                            Add(new SECSItemWrapper("LSDITEM", SECSItemFormat.A, 24, true, string.Empty));
                            Add(new SECSItemWrapper("LSDVALUE", SECSItemFormat.A, 40, true, string.Empty));
                        }
                        #endregion
                    }
                    #endregion

                    #region [Properties]
                    private CLSDINFO _LSDINFO;
                    public string LSDITEM
                    {
                        get { return this._LSDINFO.LSDITEM; }
                        set { this._LSDINFO.LSDITEM = value; }
                    }
                    public string LSDVALUE
                    {
                        get { return this._LSDINFO.LSDVALUE; }
                        set { this._LSDINFO.LSDVALUE = value; }
                    }
                    #endregion

                    #region Constructor
                    public CLSDCOUNT()
                    {
                        this._LSDINFO = new CLSDINFO();
                        Add(new SECSItemWrapper("LSDINFO", SECSItemFormat.L, 2, true, this._LSDINFO));
                    }
                    #endregion
                }
                #endregion

                #region [Properties]
                private List<CLSDCOUNT> _LSDCOUNT;
                public List<CLSDCOUNT> LSDCOUNT
                {
                    get { return this._LSDCOUNT; }
                }

                public int LSDCOUNTCount
                {
                    get { return this._LSDCOUNT.Count; }
                    set
                    {
                        this._LSDCOUNT.Clear();
                        this.AsDictionary["LSDCOUNT"].Length = value;

                        for (int i = 0; i < value; i++)
                        {
                            this._LSDCOUNT.Add(new CLSDCOUNT());
                        }
                    }
                }
                #endregion

                #region [GSDCOUNT]
                public class CGSDCOUNT : SECSItemCollectionWrapper
                {
                    #region [GSDINFO]
                    public class CGSDINFO : SECSItemCollectionWrapper
                    {
                        public string GSDITEM
                        {
                            get { return (string)this.AsDictionary["GSDITEM"].Value; }
                            set { this.AsDictionary["GSDITEM"].Value = value; }
                        }

                        public string GSDVALUE
                        {
                            get { return (string)this.AsDictionary["GSDVALUE"].Value; }
                            set { this.AsDictionary["GSDVALUE"].Value = value; }
                        }

                        #region Constructor
                        public CGSDINFO()
                        {
                            Add(new SECSItemWrapper("GSDITEM", SECSItemFormat.A, 24, true, string.Empty));
                            Add(new SECSItemWrapper("GSDVALUE", SECSItemFormat.A, 40, true, string.Empty));
                        }
                        #endregion
                    }
                    #endregion

                    #region [Properties]
                    private CGSDINFO _GSDINFO;
                    public string GSDITEM
                    {
                        get { return this._GSDINFO.GSDITEM; }
                        set { this._GSDINFO.GSDITEM = value; }
                    }
                    public string GSDVALUE
                    {
                        get { return this._GSDINFO.GSDVALUE; }
                        set { this._GSDINFO.GSDVALUE = value; }
                    }
                    #endregion

                    #region Constructor
                    public CGSDCOUNT()
                    {
                        this._GSDINFO = new CGSDINFO();
                        Add(new SECSItemWrapper("GSDINFO", SECSItemFormat.L, 2, true, this._GSDINFO));
                    }
                    #endregion
                }
                #endregion

                #region [Properties]
                private List<CGSDCOUNT> _GSDCOUNT;
                public List<CGSDCOUNT> GSDCOUNT
                {
                    get { return this._GSDCOUNT; }
                }

                public int GSDCOUNTCount
                {
                    get { return this._GSDCOUNT.Count; }
                    set
                    {
                        this._GSDCOUNT.Clear();
                        this.AsDictionary["GSDCOUNT"].Length = value;

                        for (int i = 0; i < value; i++)
                        {
                            this._GSDCOUNT.Add(new CGSDCOUNT());
                        }
                    }
                }
                #endregion

                #region Constructor
                public CGLSINFO()
                {
                    Add(new SECSItemWrapper("LOTID", SECSItemFormat.A, 16, true, string.Empty));
                    Add(new SECSItemWrapper("CSTID", SECSItemFormat.A, 14, true, string.Empty));
                    Add(new SECSItemWrapper("SLOTID", SECSItemFormat.A, 3, true, string.Empty));
                    Add(new SECSItemWrapper("RECIPEID", SECSItemFormat.A, 24, true, string.Empty));
                    Add(new SECSItemWrapper("PRCID", SECSItemFormat.A, 8, true, string.Empty));
                    Add(new SECSItemWrapper("GLSODR", SECSItemFormat.A, 1, true, string.Empty));
                    Add(new SECSItemWrapper("GLSID", SECSItemFormat.A, 16, true, string.Empty));
                    Add(new SECSItemWrapper("GLSJUDGE", SECSItemFormat.A, 2, true, string.Empty));
                    Add(new SECSItemWrapper("PNLIF", SECSItemFormat.A, 400, true, string.Empty));
                    Add(new SECSItemWrapper("SUBMDLIF", SECSItemFormat.A, 400, true, string.Empty));
                    Add(new SECSItemWrapper("PORTID", SECSItemFormat.B, 1, true, (byte)0));
                    Add(new SECSItemWrapper("GLSCODE", SECSItemFormat.U2, 1, true, (ushort)0));
                    Add(new SECSItemWrapper("REINPUT", SECSItemFormat.B, 1, true, (byte)0));
                    Add(new SECSItemWrapper("GLSTHICK", SECSItemFormat.A, 5, true, string.Empty));
                    Add(new SECSItemWrapper("PARTNUM", SECSItemFormat.A, 14, true, string.Empty));
                    Add(new SECSItemWrapper("PRODTYPE", SECSItemFormat.A, 12, true, string.Empty));
                    Add(new SECSItemWrapper("ATTRIBUTE", SECSItemFormat.A, 12, true, string.Empty));
                    Add(new SECSItemWrapper("GLSTYPE", SECSItemFormat.A, 1, true, string.Empty));
                    Add(new SECSItemWrapper("KEYID", SECSItemFormat.A, 32, true, string.Empty));
                    this._LSDCOUNT = new List<CLSDCOUNT>();
                    Add(new SECSItemWrapper("LSDCOUNT", this._LSDCOUNT));
                    this._GSDCOUNT = new List<CGSDCOUNT>();
                    Add(new SECSItemWrapper("GSDCOUNT", this._GSDCOUNT));
                }
                #endregion

                #region MakeList
                public override void MakeList(string name, int count)
                {
                    switch (name)
                    {
                        case "LSDCOUNT":
                            this.LSDCOUNTCount = count;
                            break;
                        case "GSDCOUNT":
                            this.GSDCOUNTCount = count;
                            break;
                    }
                }
                #endregion
            }
            #endregion

            #region [Properties]
            private CGLSINFO _GLSINFO;
            public string LOTID
            {
                get { return this._GLSINFO.LOTID; }
                set { this._GLSINFO.LOTID = value; }
            }
            public string CSTID
            {
                get { return this._GLSINFO.CSTID; }
                set { this._GLSINFO.CSTID = value; }
            }
            public string SLOTID
            {
                get { return this._GLSINFO.SLOTID; }
                set { this._GLSINFO.SLOTID = value; }
            }
            public string RECIPEID
            {
                get { return this._GLSINFO.RECIPEID; }
                set { this._GLSINFO.RECIPEID = value; }
            }
            public string PRCID
            {
                get { return this._GLSINFO.PRCID; }
                set { this._GLSINFO.PRCID = value; }
            }
            public string GLSODR
            {
                get { return this._GLSINFO.GLSODR; }
                set { this._GLSINFO.GLSODR = value; }
            }
            public string GLSID
            {
                get { return this._GLSINFO.GLSID; }
                set { this._GLSINFO.GLSID = value; }
            }
            public string GLSJUDGE
            {
                get { return this._GLSINFO.GLSJUDGE; }
                set { this._GLSINFO.GLSJUDGE = value; }
            }
            public string PNLIF
            {
                get { return this._GLSINFO.PNLIF; }
                set { this._GLSINFO.PNLIF = value; }
            }
            public string SUBMDLIF
            {
                get { return this._GLSINFO.SUBMDLIF; }
                set { this._GLSINFO.SUBMDLIF = value; }
            }
            public byte PORTID
            {
                get { return this._GLSINFO.PORTID; }
                set { this._GLSINFO.PORTID = value; }
            }
            public ushort GLSCODE
            {
                get { return this._GLSINFO.GLSCODE; }
                set { this._GLSINFO.GLSCODE = value; }
            }
            public byte REINPUT
            {
                get { return this._GLSINFO.REINPUT; }
                set { this._GLSINFO.REINPUT = value; }
            }
            public string GLSTHICK
            {
                get { return this._GLSINFO.GLSTHICK; }
                set { this._GLSINFO.GLSTHICK = value; }
            }
            public string PARTNUM
            {
                get { return this._GLSINFO.PARTNUM; }
                set { this._GLSINFO.PARTNUM = value; }
            }
            public string PRODTYPE
            {
                get { return this._GLSINFO.PRODTYPE; }
                set { this._GLSINFO.PRODTYPE = value; }
            }
            public string ATTRIBUTE
            {
                get { return this._GLSINFO.ATTRIBUTE; }
                set { this._GLSINFO.ATTRIBUTE = value; }
            }
            public string GLSTYPE
            {
                get { return this._GLSINFO.GLSTYPE; }
                set { this._GLSINFO.GLSTYPE = value; }
            }
            public string KEYID
            {
                get { return this._GLSINFO.KEYID; }
                set { this._GLSINFO.KEYID = value; }
            }
            public List<CGLSINFO.CLSDCOUNT> LSDCOUNT
            {
                get { return this._GLSINFO.LSDCOUNT; }
            }

            public int LSDCOUNTCount
            {
                get { return this._GLSINFO.LSDCOUNTCount; }
                set { this._GLSINFO.LSDCOUNTCount = value; }
            }
            public List<CGLSINFO.CGSDCOUNT> GSDCOUNT
            {
                get { return this._GLSINFO.GSDCOUNT; }
            }

            public int GSDCOUNTCount
            {
                get { return this._GLSINFO.GSDCOUNTCount; }
                set { this._GLSINFO.GSDCOUNTCount = value; }
            }
            #endregion

            #region Constructor
            public CL()
            {
                Add(new SECSItemWrapper("UNITID", SECSItemFormat.U2, 1, true, (ushort)0));
                Add(new SECSItemWrapper("ACKC2", SECSItemFormat.B, 1, true, (byte)0));
                this._GLSINFO = new CGLSINFO();
                Add(new SECSItemWrapper("GLSINFO", SECSItemFormat.L, 21, true, this._GLSINFO));
            }
            #endregion
        }
        #endregion

        #region [Properties]
        private CL _L;
        public ushort UNITID
        {
            get { return this._L.UNITID; }
            set { this._L.UNITID = value; }
        }
        public byte ACKC2
        {
            get { return this._L.ACKC2; }
            set { this._L.ACKC2 = value; }
        }
        public string LOTID
        {
            get { return this._L.LOTID; }
            set { this._L.LOTID = value; }
        }
        public string CSTID
        {
            get { return this._L.CSTID; }
            set { this._L.CSTID = value; }
        }
        public string SLOTID
        {
            get { return this._L.SLOTID; }
            set { this._L.SLOTID = value; }
        }
        public string RECIPEID
        {
            get { return this._L.RECIPEID; }
            set { this._L.RECIPEID = value; }
        }
        public string PRCID
        {
            get { return this._L.PRCID; }
            set { this._L.PRCID = value; }
        }
        public string GLSODR
        {
            get { return this._L.GLSODR; }
            set { this._L.GLSODR = value; }
        }
        public string GLSID
        {
            get { return this._L.GLSID; }
            set { this._L.GLSID = value; }
        }
        public string GLSJUDGE
        {
            get { return this._L.GLSJUDGE; }
            set { this._L.GLSJUDGE = value; }
        }
        public string PNLIF
        {
            get { return this._L.PNLIF; }
            set { this._L.PNLIF = value; }
        }
        public string SUBMDLIF
        {
            get { return this._L.SUBMDLIF; }
            set { this._L.SUBMDLIF = value; }
        }
        public byte PORTID
        {
            get { return this._L.PORTID; }
            set { this._L.PORTID = value; }
        }
        public ushort GLSCODE
        {
            get { return this._L.GLSCODE; }
            set { this._L.GLSCODE = value; }
        }
        public byte REINPUT
        {
            get { return this._L.REINPUT; }
            set { this._L.REINPUT = value; }
        }
        public string GLSTHICK
        {
            get { return this._L.GLSTHICK; }
            set { this._L.GLSTHICK = value; }
        }
        public string PARTNUM
        {
            get { return this._L.PARTNUM; }
            set { this._L.PARTNUM = value; }
        }
        public string PRODTYPE
        {
            get { return this._L.PRODTYPE; }
            set { this._L.PRODTYPE = value; }
        }
        public string ATTRIBUTE
        {
            get { return this._L.ATTRIBUTE; }
            set { this._L.ATTRIBUTE = value; }
        }
        public string GLSTYPE
        {
            get { return this._L.GLSTYPE; }
            set { this._L.GLSTYPE = value; }
        }
        public string KEYID
        {
            get { return this._L.KEYID; }
            set { this._L.KEYID = value; }
        }
        public List<CL.CGLSINFO.CLSDCOUNT> LSDCOUNT
        {
            get { return this._L.LSDCOUNT; }
        }

        public int LSDCOUNTCount
        {
            get { return this._L.LSDCOUNTCount; }
            set { this._L.LSDCOUNTCount = value; }
        }
        public List<CL.CGLSINFO.CGSDCOUNT> GSDCOUNT
        {
            get { return this._L.GSDCOUNT; }
        }

        public int GSDCOUNTCount
        {
            get { return this._L.GSDCOUNTCount; }
            set { this._L.GSDCOUNTCount = value; }
        }
        #endregion

        #region Constructor
        public CS2F80_L21(HSMSDriver driver, string messageName)
            : base(driver, messageName)
        {
            this.Stream = 2;
            this.Function = 80;
            this.WaitBit = false;
            this.Direction = SECSMessageDirection.ToEquipment;
            this.AutoReply = false;

            this._L = new CL();
            Add(new SECSItemWrapper("L", SECSItemFormat.L, 3, true, this._L));
        }
        #endregion
    }
    #endregion
    #endregion
}
