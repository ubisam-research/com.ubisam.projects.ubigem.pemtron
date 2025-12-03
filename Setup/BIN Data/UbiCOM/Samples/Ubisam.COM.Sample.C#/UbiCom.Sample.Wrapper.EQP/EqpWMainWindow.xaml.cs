using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UbiCom.Net.Driver;
using UbiCom.Net.Structure;

namespace UbiCom.Sample.Wrapper.EQP
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EqpWMainWindow : Window
    {
        private const int MAX_LOG_LINES = 5000;

        private EQPDriver _driver;
        private bool _addSecs2Log;
        private bool _autoScroll;

        public EqpWMainWindow()
        {
            InitializeComponent();

            this._driver = new EQPDriver();

            this._driver.OnSECSConnected += _driver_OnSECSConnected;
            this._driver.OnSECSDisconnected += _driver_OnSECSDisconnected;
            this._driver.OnTimeout += _driver_OnTimeout;
            this._driver.OnT3Timeout += _driver_OnT3Timeout;

            this._driver.OnSECS2WriteLog += _driver_OnSECS2WriteLog;

            this._driver.OnS1F1Received += _driver_OnS1F1Received;
            this._driver.OnS1F2_ToEqpReceived += _driver_OnS1F2_ToEqpReceived;
            this._driver.OnS2F41Received += _driver_OnS2F41Received;
            this._driver.OnS6F12Received += _driver_OnS6F12Received;

            this._driver.OnS2F80_L0Received += _driver_OnS2F80_L0Received;
            this._driver.OnS2F80_L21Received += _driver_OnS2F80_L21Received;

            this._addSecs2Log = true;
            this._autoScroll = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void _driver_OnSECSConnected(object sender, string ipAddress, int portNo)
        {
            WriteLog(string.Format("Connected : IP={0}, Port={1}", ipAddress, portNo));
        }

        private void _driver_OnSECSDisconnected(object sender, string ipAddress, int portNo)
        {
            WriteLog(string.Format("Disconnected : IP={0}, Port={1}", ipAddress, portNo));
        }

        private void _driver_OnS1F1Received(CS1F1 S1F1)
        {
            MessageError driverResult;

            WriteLog(string.Format("{0} Received", S1F1.Name));

            CS1F2_ToHost s1f2 = this._driver.S1F2_ToHost();

            s1f2.MDLN = "UbiSAM";
            s1f2.SOFTREV = "1.0.0";

            driverResult = s1f2.Reply(S1F1);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0} Reply : Result={1}", s1f2.Name, driverResult));
            }
        }

        private void _driver_OnS1F2_ToEqpReceived(SECSMessage primaryMessage, CS1F2_ToEqp S1F2_ToEqp)
        {
            WriteLog(string.Format("{0} Received", S1F2_ToEqp.Name));
        }

        private void _driver_OnS2F41Received(CS2F41 S2F41)
        {
            StringBuilder sb = new StringBuilder(1000);

            //<L,2                                      
            //    <A,6 'TEST_CMD' [RCMD]>               => RCMD
            //    <L,3 [CPCOUNT]                        => CPCOUNT
            //        <L,2                              
            //            <A,11 'CONFIRMFLAG' [CPNAME]> => CPCOUNT[0].CPNAME
            //            <A,1 'Y' [CPVAL]>             => CPCOUNT[0].CPVAL
            //        >
            //        <L,2                              
            //            <A,7 'MODELID' [CPNAME]>      => CPCOUNT[1].CPNAME
            //            <A,5 'MID11' [CPVAL]>         => CPCOUNT[1].CPVAL
            //        >
            //        <L,2                              
            //            <A,3 'QTY'>                   => CPCOUNT[2].CPNAME
            //            <U2,1 '123'>                  => CPCOUNT[2].CPVAL
            //        >
            //    >
            //>
            sb.AppendFormat("    RCMD={0}\r\n", S2F41.RCMD);
            
            for (int i = 0; i < S2F41.CPCOUNTCount; i++)
            {
                sb.AppendFormat("    CP Name={0}, CP Value={1}\r\n", S2F41.CPCOUNT[i].CPNAME, S2F41.CPCOUNT[i].CPVAL);
            }

            WriteLog(string.Format("{0} Received\r\n{1}", S2F41.Name, sb.ToString()));

            CS2F42 s2f42 = this._driver.S2F42();

            s2f42.HCACK = 0;

            MessageError driverResult = s2f42.Reply(S2F41);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0} Reply : Result={1}", s2f42.Name, driverResult));
            }
        }

        private void _driver_OnS6F12Received(SECSMessage primaryMessage, CS6F12 S6F12)
        {
            WriteLog(string.Format("{0} Received", S6F12.Name));
        }

        private void _driver_OnS2F80_L0Received(SECSMessage primaryMessage, CS2F80_L0 S2F80_L0)
        {
            StringBuilder sb = new StringBuilder(1000);

            //<L,3 [L]
            //    <U2,1 '101' [UNITID]> => UNITID
            //    <B,1 '0' [ACKC2]>     => ACKC2
            //    <L,0 [GLSINFO]>       => GLSINFO
            //>
            sb.AppendFormat("    UNITID={0}\r\n", S2F80_L0.UNITID);
            sb.AppendFormat("    ACKC2={0}\r\n", S2F80_L0.ACKC2);

            WriteLog(string.Format("{0} Received\r\n{1}", S2F80_L0.Name, sb.ToString()));
        }

        private void _driver_OnS2F80_L21Received(SECSMessage primaryMessage, CS2F80_L21 S2F80_L21)
        {
            StringBuilder sb = new StringBuilder(1000);
            //<L,3 [L]
            //    <U2,1 '101' [UNITID]>                             => UNITID
            //    <B,1 '0' [ACKC2]>                                 => ACKC2
            //    <L,21 [GLSINFO]                                   
            //        <A,16 'LOT_001         ' [LOTID]>             => LOTID
            //        <A,14 'CST_001       ' [CSTID]>               => CSTID
            //        <A,3 '001' [SLOTID]>                          => SLOTID
            //        <A,24 'PPID_001                ' [RECIPEID]>  => RECIPEID
            //        <A,8 '1000    ' [PRCID]>                      => PRCID
            //        <A,1 'G' [GLSODR]>                            => GLSODR
            //        <A,16 'GLSID_001       ' [GLSID]>             => GLSID
            //        <A,2 'G ' [GLSJUDGE]>                         => GLSJUDGE
            //        <A,400 'GGGGNNNGGGG                                                                                                                                                                                                                                                                                                                                                                                                     ' [PNLIF]>
            //        <A,400 '                                                                                                                                                                                                                                                                                                                                                                                                                ' [SUBMDLIF]>
            //        <B,1 '3' [PORTID]>                            => PORTID
            //        <U2,1 '10101' [GLSCODE]>                      => GLSCODE
            //        <B,1 '0' [REINPUT]>                           => REINPUT
            //        <A,5 '6    ' [GLSTHICK]>                      => GLSTHICK
            //        <A,14 '              ' [PARTNUM]>             => PARTNUM
            //        <A,12 '            ' [PRODTYPE]>              => PRODTYPE
            //        <A,12 '            ' [ATTRIBUTE]>             => ATTRIBUTE
            //        <A,1 'P' [GLSTYPE]>                           => GLSTYPE
            //        <A,32 'KEY_001                         ' [KEYID]> => KEYID
            //        <L,3 [LSDCOUNT]                                                      => LSDCOUNT
            //            <L,2 [LSDINFO]
            //                <A,24 'LSDITEM1                ' [LSDITEM]>                  => LSDCOUNT[0].LSDITEM
            //                <A,40 'LSDVALUE1                               ' [LSDVALUE]> => LSDCOUNT[0].LSDVALUE
            //            >
            //            <L,2 [LSDINFO]
            //                <A,24 'LSDITEM2                ' [LSDITEM]>                  => LSDCOUNT[1].LSDITEM
            //                <A,40 'LSDVALUE2                               ' [LSDVALUE]> => LSDCOUNT[1].LSDVALUE
            //            >
            //            <L,2 [LSDINFO]
            //                <A,24 'LSDITEM3                ' [LSDITEM]>                  => LSDCOUNT[2].LSDITEM
            //                <A,40 'LSDVALUE3                               ' [LSDVALUE]> => LSDCOUNT[2].LSDVALUE
            //            >
            //        >
            //        <L,2 [GSDCOUNT]
            //            <L,2 [GSDINFO]
            //                <A,24 'GSDITEM1                ' [GSDITEM]>                  => GSDCOUNT[0].GSDITEM
            //                <A,40 'GSDVALUE1                               ' [GSDVALUE]> => GSDCOUNT[0].GSDVALUE
            //            >
            //            <L,2 [GSDINFO]
            //                <A,24 'GSDITEM2                ' [GSDITEM]>                  => GSDCOUNT[1].GSDITEM
            //                <A,40 'GSDVALUE2                               ' [GSDVALUE]> => GSDCOUNT[1].GSDVALUE
            //            >
            //        >
            //    >
            //>
            sb.AppendFormat("    UNITID={0}\r\n", S2F80_L21.UNITID);
            sb.AppendFormat("    ACKC2={0}\r\n", S2F80_L21.ACKC2);
            sb.AppendFormat("        LOTID={0}\r\n", S2F80_L21.LOTID);
            sb.AppendFormat("        CSTID={0}\r\n", S2F80_L21.CSTID);
            sb.AppendFormat("        SLOTID={0}\r\n", S2F80_L21.SLOTID);
            sb.AppendFormat("        RECIPEID={0}\r\n", S2F80_L21.RECIPEID);
            sb.AppendFormat("        PRCID={0}\r\n", S2F80_L21.PRCID);
            sb.AppendFormat("        GLSODR={0}\r\n", S2F80_L21.GLSODR);
            sb.AppendFormat("        GLSID={0}\r\n", S2F80_L21.GLSID);
            sb.AppendFormat("        GLSJUDGE={0}\r\n", S2F80_L21.GLSJUDGE);
            sb.AppendFormat("        PNLIF={0}\r\n", S2F80_L21.PNLIF);
            sb.AppendFormat("        SUBMDLIF={0}\r\n", S2F80_L21.SUBMDLIF);
            sb.AppendFormat("        PORTID={0}\r\n", S2F80_L21.PORTID);
            sb.AppendFormat("        GLSCODE={0}\r\n", S2F80_L21.GLSCODE);
            sb.AppendFormat("        REINPUT={0}\r\n", S2F80_L21.REINPUT);
            sb.AppendFormat("        GLSTHICK={0}\r\n", S2F80_L21.GLSTHICK);
            sb.AppendFormat("        PARTNUM={0}\r\n", S2F80_L21.PARTNUM);
            sb.AppendFormat("        PRODTYPE={0}\r\n", S2F80_L21.PRODTYPE);
            sb.AppendFormat("        ATTRIBUTE={0}\r\n", S2F80_L21.ATTRIBUTE);
            sb.AppendFormat("        GLSTYPE={0}\r\n", S2F80_L21.GLSTYPE);
            sb.AppendFormat("        KEYID={0}\r\n", S2F80_L21.KEYID);

            sb.AppendFormat("        LSDCOUNT={0}\r\n", S2F80_L21.LSDCOUNTCount);
            for (int i = 0; i < S2F80_L21.LSDCOUNTCount; i++)
            {
                sb.AppendFormat("            LSD #{0}:Name={1}, Value={2}\r\n",i+1, S2F80_L21.LSDCOUNT[i].LSDITEM, S2F80_L21.LSDCOUNT[i].LSDVALUE);
            }

            sb.AppendFormat("        GSDCOUNT={0}\r\n", S2F80_L21.GSDCOUNTCount);
            for (int i = 0; i < S2F80_L21.GSDCOUNTCount; i++)
            {
                sb.AppendFormat("            GSD #{0}:Name={1}, Value={2}\r\n", i + 1, S2F80_L21.GSDCOUNT[i].GSDITEM, S2F80_L21.GSDCOUNT[i].GSDVALUE);
            }

            WriteLog(string.Format("{0} Received\r\n{1}", S2F80_L21.Name, sb.ToString()));
        }

        private void _driver_OnTimeout(object sender, UbiCom.Net.Structure.TimeoutType timeoutType)
        {
            WriteLog(string.Format("Timeout : Type={0}", timeoutType));
        }

        private void _driver_OnT3Timeout(object sender, UbiCom.Net.Structure.SECSMessage message)
        {
            WriteLog(string.Format("T3 Timeout : Message={0}", message));
        }

        private void _driver_OnSECS2WriteLog(object sender, UbiCom.Net.Utility.Logger.LogLevel logLevel, string logText)
        {
            if (this._addSecs2Log == true)
            {
                WriteSecs2Log(logText);
            }
        }

        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            string errorText;
            DriverError driverResult;

            driverResult = this._driver.Initialize(new Configurtion()
            {
                DeviceID = 1,
                DeviceType = DeviceType.Equipment,
                DriverName = "EqpTest",
                HSMSModeConfig = new Configurtion.HSMS()
                {
                    HSMSMode = HSMSMode.Passive,
                    LocalIPAddress = "127.0.0.1",
                    LocalPortNo = 7000
                },
                SECSMode = SECSMode.HSMS,
                UMDFileName = "Sample.umd",
                LogEnabledSystem = LogMode.Hour,
                LogPath = @"C:\Log"
            },
            out errorText);

            if (driverResult == DriverError.Ok)
            {
                WriteLog(string.Format("Initialize : Result={0}", driverResult));
            }
            else
            {
                WriteLog(string.Format("Initialize : Result={0}, Reason={1}", driverResult, errorText));
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            DriverError driverResult;

            driverResult = this._driver.Open();

            WriteLog(string.Format("Open : Result={0}", driverResult));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DriverError driverResult;

            driverResult = this._driver.Close();

            WriteLog(string.Format("Close : Result={0}", driverResult));
        }

        private void btnS1F1_Click(object sender, RoutedEventArgs e)
        {
            CS1F1 s1f1 = this._driver.S1F1();

            MessageError driverResult = s1f1.Request();

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0} Send : Result={1}", s1f1.Name, driverResult));
            }
        }

        private void btnS6F11_Click(object sender, RoutedEventArgs e)
        {
            //<L,3
            //    <I4,1 '6750318' [DATAID]>
            //    <I4,1 '2097200' [CEID]>
            //    <L,1 [RPTIDCOUNT]
            //        <L,2
            //            <I4,1 '1' [RPTID]>
            //            <L,2 [VCOUNT]
            //                <A,5 'LOT11' [V]>
            //                <L,3 [V]
            //                    <A,2 'A1'>
            //                    <A,2 'NG'>
            //                    <L,2
            //                        <A,2 'NG'>
            //                        <U1,1 '3'>
            //                    >
            //                >
            //            >
            //        >
            //    >
            //>
            CS6F11 s6f11 = this._driver.S6F11();

            s6f11.DATAID = 6750318;
            s6f11.CEID = 2097200;
            s6f11.RPTIDCOUNTCount = 1;
            s6f11.RPTIDCOUNT[0].RPTID = 1;
            s6f11.RPTIDCOUNT[0].VCOUNTCount = 2;
            s6f11.RPTIDCOUNT[0].VCOUNT[0].V = new SECSItem("V", SECSItemFormat.A, 5, "LOT11");

            SECSItem childItem = new SECSItem("", SECSItemFormat.L, 3, null);
            childItem.SubItem.Add(new SECSItem("", SECSItemFormat.A, 2, "A1"));
            childItem.SubItem.Add(new SECSItem("", SECSItemFormat.A, 2, "NG"));

            SECSItem childItem2 = new SECSItem("", SECSItemFormat.L, 2, null);
            childItem2.SubItem.Add(new SECSItem("", SECSItemFormat.A, 2, "NG"));
            childItem2.SubItem.Add(new SECSItem("", SECSItemFormat.U1, 1, 3));
            childItem.SubItem.Add(childItem2);

            s6f11.RPTIDCOUNT[0].VCOUNT[1].V = childItem;

            MessageError driverResult = s6f11.Request();

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0} Send : Result={1}", s6f11.Name, driverResult));
            }
        }

        private void btnS2F79_0_Click(object sender, RoutedEventArgs e)
        {
            CS2F79 s2f79 = this._driver.S2F79();

            //<L,4
            //    <U2,1 '101' [UNITID]>
            //    <A,16 'GLSID_001       ' [GLSID]>
            //    <U2,1 '10101' [GLSCODE]>
            //    <A,1 'a' [REQOPTION]> // case -> GLSSTATUS=0
            //>
            s2f79.UNITID = 101;
            s2f79.GLSID = "GLSID_001";
            s2f79.GLSCODE = 10101;
            s2f79.REQOPTION = "a";

            MessageError driverResult = s2f79.Request();

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0} Send : Result={1}", s2f79.Name, driverResult));
            }
        }

        private void btnS2F79_1_Click(object sender, RoutedEventArgs e)
        {
            CS2F79 s2f79 = this._driver.S2F79();

            //<L,4
            //    <U2,1 '101' [UNITID]>
            //    <A,16 'GLSID_001       ' [GLSID]>
            //    <U2,1 '10101' [GLSCODE]>
            //    <A,1 'b' [REQOPTION]> // case -> GLSSTATUS=1
            //>
            s2f79.UNITID = 101;
            s2f79.GLSID = "GLSID_001";
            s2f79.GLSCODE = 10101;
            s2f79.REQOPTION = "b";

            MessageError driverResult = s2f79.Request();

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0} Send : Result={1}", s2f79.Name, driverResult));
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void chkSecs2Log_Click(object sender, RoutedEventArgs e)
        {
            this._addSecs2Log = (sender as CheckBox).IsChecked.Value;
        }

        private void WriteLog(string logText)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                try
                {
                    ListBoxItem item = new ListBoxItem();

                    if (lsbLogViewer.Items.Count > MAX_LOG_LINES)
                    {
                        lsbLogViewer.Items.Clear();
                    }

                    item.Content = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logText);

                    lsbLogViewer.Items.Add(item);

                    if (this._autoScroll == true)
                    {
                        lsbLogViewer.ScrollIntoView(item);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }));
        }

        private void WriteSecs2Log(string logText)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                try
                {
                    ListBoxItem item = new ListBoxItem();

                    if (lsbLogViewer.Items.Count > MAX_LOG_LINES)
                    {
                        lsbLogViewer.Items.Clear();
                    }

                    item.Content = logText;

                    lsbLogViewer.Items.Add(item);

                    if (this._autoScroll == true)
                    {
                        lsbLogViewer.ScrollIntoView(item);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }));
        }

        private void mnuAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            if (this._autoScroll == true)
                mnuAutoScroll.IsChecked = false;
            else
                mnuAutoScroll.IsChecked = true;

            this._autoScroll = mnuAutoScroll.IsChecked;
        }

        private void mnuLogClear_Click(object sender, RoutedEventArgs e)
        {
            lsbLogViewer.Items.Clear();
        }
    }
}