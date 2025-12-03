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

namespace UbiCom.Sample.Wrapper.Host
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HostWMainWindow : Window
    {
        private const int MAX_LOG_LINES = 5000;

        private HostDriver _driver;
        private bool _addSecs2Log;
        private bool _autoScroll;

        public HostWMainWindow()
        {
            InitializeComponent();

            this._driver = new HostDriver();

            this._driver.OnSECSConnected += _driver_OnSECSConnected;
            this._driver.OnSECSDisconnected += _driver_OnSECSDisconnected;
            this._driver.OnTimeout += _driver_OnTimeout;
            this._driver.OnT3Timeout += _driver_OnT3Timeout;
            this._driver.OnSECS2WriteLog += _driver_OnSECS2WriteLog;

            this._driver.OnS1F1Received+= _driver_OnS1F1Received;
            this._driver.OnS1F2_ToHostReceived += _driver_OnS1F2_ToHostReceived;
            this._driver.OnS2F42Received += _driver_OnS2F42Received;
            this._driver.OnS6F11Received += _driver_OnS6F11Received;

            this._driver.OnS2F79Received += _driver_OnS2F79Received;

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
            CS1F2_ToEqp s1f2 = this._driver.S1F2_ToEqp();

            driverResult = s1f2.Reply(S1F1);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("S1F2 Send : Result={0}", driverResult));
            }
        }

        private void _driver_OnS1F2_ToHostReceived(SECSMessage primaryMessage, CS1F2_ToHost S1F2_ToHost)
        {
        }

        private void _driver_OnS2F42Received(SECSMessage primaryMessage, CS2F42 S2F42)
        {
        }

        private void _driver_OnS6F11Received(CS6F11 S6F11)
        {
            MessageError driverResult;
            StringBuilder sb = new StringBuilder(1000);

            //<L,3 [L]                           =>
            //    <I4,1 '6750318' [DATAID]>      => DATAID
            //    <I4,1 '2097200' [CEID]>        => CEID
            //    <L,1 [RPTIDCOUNT]              => RPTIDCOUNT
            //        <L,2 [RPTINFO]             => 
            //            <I4,1 '1' [RPTID]>     => RPTIDCOUNT[n].RPTID
            //            <L,2 [VCOUNT]          => 
            //                <A,5 'LOT11' [V]>  => RPTIDCOUNT[n].V[m]
            //                <L,3 [V]           => 
            //                    <A,2 'A1'>     => RPTIDCOUNT[n].V[m].SubItem[0]
            //                    <A,2 'NG'>     => RPTIDCOUNT[n].V[m].SubItem[1]
            //                    <L,2           => RPTIDCOUNT[n].V[m].SubItem[2]
            //                        <A,2 'NG'> => RPTIDCOUNT[n].V[m].SubItem[2].SubItem[0]
            //                        <U1,1 '3'> => RPTIDCOUNT[n].V[m].SubItem[2].SubItem[1]
            //                    >
            //                >
            //            >
            //        >
            //    >
            //>
            sb.AppendFormat("    CEID={0}\r\n", S6F11.CEID);
            
            for (int i = 0; i < S6F11.RPTIDCOUNTCount; i++)
            {
                sb.AppendFormat("    RPTID={0}\r\n", S6F11.RPTIDCOUNT[i].RPTID);
                
                for (int j = 0; j < S6F11.RPTIDCOUNT[i].VCOUNTCount; j++)
                {
                    if (S6F11.RPTIDCOUNT[i].VCOUNT[j].V.Format == SECSItemFormat.L)
                    {
                        sb.AppendFormat("        V count={0}\r\n", S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem.Count);

                        for (int k = 0; k < S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem.Count; k++)
                        {
                            if (S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem[k].Format == SECSItemFormat.L)
                            {
                                sb.AppendFormat("            V count={0}\r\n", S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem[k].SubItem.Count);

                                for (int l = 0; l < S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem[k].SubItem.Count; l++)
                                {
                                    if (S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem[k].SubItem[l].Format == SECSItemFormat.L)
                                    {
                                        sb.AppendFormat("                V count={0}\r\n", S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem[k].SubItem[l].SubItem.Count);
                                    }
                                    else
                                    {
                                        sb.AppendFormat("                V={0}\r\n", S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem[k].SubItem[l].Value.ToString());
                                    }
                                }
                            }
                            else
                            {
                                sb.AppendFormat("            V={0}\r\n", S6F11.RPTIDCOUNT[i].VCOUNT[j].V.SubItem[k].Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        sb.AppendFormat("        V={0}\r\n", S6F11.RPTIDCOUNT[i].VCOUNT[j].V.Value.ToString());
                    }
                }
            }

            WriteLog(string.Format("{0} Received\r\n{1}", S6F11.Name, sb.ToString()));

            CS6F12 s6f12 = this._driver.S6F12();

            s6f12.ACKC6 = 0;

            driverResult = s6f12.Reply(S6F11);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0} Send : Result={1}", s6f12.Name, driverResult));
            }
        }

        private void _driver_OnS2F79Received(CS2F79 S2F79)
        {
            StringBuilder sb = new StringBuilder(1000);

            //<L,4
            //    <U2,1 '101' [UNITID]>
            //    <A,16 'GLSID_001       ' [GLSID]>
            //    <U2,1 '10101' [GLSCODE]>
            //    <A,1 'a' [REQOPTION]>
            //>

            sb.AppendFormat("    UNITID={0}\r\n", S2F79.UNITID);
            sb.AppendFormat("    GLSID={0}\r\n", S2F79.GLSID);
            sb.AppendFormat("    GLSCODE={0}\r\n", S2F79.GLSCODE);
            sb.AppendFormat("    REQOPTION={0}\r\n", S2F79.REQOPTION);

            WriteLog(string.Format("{0} Received\r\n{1}", S2F79.Name, sb.ToString()));

            if (S2F79.REQOPTION == "a")
            {
                CS2F80_L0 s2f80 = this._driver.S2F80_L0();

                s2f80.UNITID = S2F79.UNITID;
                s2f80.ACKC2 = 0;

                MessageError driverResult = s2f80.Reply(S2F79);

                if (driverResult != MessageError.Ok)
                {
                    WriteLog(string.Format("{0} Reply : Result={1}", s2f80.Name, driverResult));
                }
            }
            else
            {
                CS2F80_L21 s2f80 = this._driver.S2F80_L21();

                s2f80.UNITID = S2F79.UNITID;
                s2f80.GLSID = S2F79.GLSID;
                s2f80.GLSCODE = S2F79.GLSCODE;
                s2f80.ACKC2 = 0;
                s2f80.LOTID = "LOT_001";
                s2f80.CSTID = "CST_001";
                s2f80.SLOTID = "001";
                s2f80.RECIPEID = "PPID_001";
                s2f80.PRCID = "1000";
                s2f80.GLSODR = "G";
                s2f80.GLSJUDGE = "G";
                s2f80.PNLIF = "GGGGNNNGGGG";
                //s2f80.SUBMDLIF = "";
                s2f80.PORTID = 3;
                s2f80.REINPUT = 0;
                s2f80.GLSTHICK = "6";
                //s2f80.PARTNUM = "";
                //s2f80.PRODTYPE = "";
                //s2f80.ATTRIBUTE = "";
                s2f80.GLSTYPE = "P";
                s2f80.KEYID = "KEY_001";

                s2f80.LSDCOUNTCount = 3;
                s2f80.LSDCOUNT[0].LSDITEM = "LSDITEM1";
                s2f80.LSDCOUNT[0].LSDVALUE = "LSDVALUE1";
                s2f80.LSDCOUNT[1].LSDITEM = "LSDITEM2";
                s2f80.LSDCOUNT[1].LSDVALUE = "LSDVALUE2";
                s2f80.LSDCOUNT[2].LSDITEM = "LSDITEM3";
                s2f80.LSDCOUNT[2].LSDVALUE = "LSDVALUE3";

                s2f80.GSDCOUNTCount = 2;
                s2f80.GSDCOUNT[0].GSDITEM = "GSDITEM1";
                s2f80.GSDCOUNT[0].GSDVALUE = "GSDVALUE1";
                s2f80.GSDCOUNT[1].GSDITEM = "GSDITEM2";
                s2f80.GSDCOUNT[1].GSDVALUE = "GSDVALUE2";

                MessageError driverResult = s2f80.Reply(S2F79);

                if (driverResult != MessageError.Ok)
                {
                    WriteLog(string.Format("{0} Reply : Result={1}", s2f80.Name, driverResult));
                }
            }
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
                DeviceType = DeviceType.Host,
                DriverName = "HostTest",
                HSMSModeConfig = new Configurtion.HSMS()
                {
                    HSMSMode = HSMSMode.Active,
                    RemoteIPAddress = "127.0.0.1",
                    RemotePortNo = 7000
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
            SECSMessage message;

            message = new SECSMessage()
            {
                Name = "AreYouThere",
                Stream = 1,
                Function = 1,
                WaitBit = true
            };

            MessageError driverResult = this._driver.SendSECSMessage(message);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("S1F1 Send : Result={0}", driverResult));
            }
        }

        private void btnS2F41_Click(object sender, RoutedEventArgs e)
        {
            SECSMessage message;
            MessageError driverResult;

            message = this._driver.Messages.GetMessageHeader(2, 41);

            //<L,2
            //    <A,6 'PERMIT' [RCMD]>
            //    <L,3 [CPCOUNT]
            //        <L,2
            //            <A,11 'CONFIRMFLAG' [CPNAME]>
            //            <A,1 'Y' [CPVAL]>
            //        >
            //        <L,2
            //            <A,7 'MODELID' [CPNAME]>
            //            <A,5 'MID11' [CPVAL]>
            //        >
            //        <L,2
            //            <A,3 'QTY'>
            //            <U2,1 '123'>
            //        >
            //    >
            //>
            CS2F41 s2f41 = this._driver.S2F41();

            //s2f41.RCMD = new SECSItem("RCMD", SECSItemFormat.A, 6, "PERMIT");
            s2f41.RCMD = "TEST_CMD";
            s2f41.CPCOUNTCount = 3;
            s2f41.CPCOUNT[0].CPNAME = "CONFIRMFLAG";
            s2f41.CPCOUNT[0].CPVAL = new SECSItem("CPVAL", SECSItemFormat.A, 1, "Y");
            s2f41.CPCOUNT[1].CPNAME = "MODELID";
            s2f41.CPCOUNT[1].CPVAL = new SECSItem("CPNAME", SECSItemFormat.A, 5, "MID11");
            s2f41.CPCOUNT[2].CPNAME = "OPID";
            s2f41.CPCOUNT[2].CPVAL = new SECSItem("QTY", SECSItemFormat.U2, 1, 123);

            driverResult = s2f41.Request();

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("S2F41 Send : Result={0}", driverResult));
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