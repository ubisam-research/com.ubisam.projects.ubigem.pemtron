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

namespace UbiCom.Sample.UseWrapper
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HostMainWindow : Window
    {
        private const int MAX_LOG_LINES = 5000;

        private HSMSDriver _driver;
        private bool _addSecs2Log;
        private bool _autoScroll;

        public HostMainWindow()
        {
            InitializeComponent();

            this._driver = new HSMSDriver();

            this._driver.OnSECSConnected += _driver_OnSECSConnected;
            this._driver.OnSECSDisconnected += _driver_OnSECSDisconnected;
            this._driver.OnReceivedPrimaryMessage += _driver_OnReceivedPrimaryMessage;
            this._driver.OnReceivedSecondaryMessage += _driver_OnReceivedSecondaryMessage;
            this._driver.OnTimeout += _driver_OnTimeout;
            this._driver.OnT3Timeout += _driver_OnT3Timeout;

            this._driver.OnSECS2WriteLog += _driver_OnSECS2WriteLog;

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

        private void _driver_OnReceivedPrimaryMessage(object sender, Net.Structure.SECSMessage message)
        {
            SECSMessage replyMessage;
            MessageError driverResult;

            if (message.Stream == 1 && message.Function == 1)
            {
                replyMessage = this._driver.Messages.GetMessageHeader(1, 2, DeviceType.Host);

                replyMessage.Body.Add(SECSItemFormat.L, 0, null);

                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                if (driverResult != MessageError.Ok)
                {
                    WriteLog(string.Format("{0}({1}) Reply : Result={2}", replyMessage.Name, replyMessage.Description, driverResult));
                }
            }
            else if (message.Stream == 6 && message.Function == 11)
            {
                StringBuilder sb = new StringBuilder(1000);

                //<L,3                               => Item[0]
                //    <I4,1 '6750318' [DATAID]>      => Item[0].SubItem[0]
                //    <I4,1 '2097200' [CEID]>        => Item[0].SubItem[1]
                //    <L,1 [RPTIDCOUNT]              => Item[0].SubItem[2]
                //        <L,2                       => Item[0].SubItem[2].SubItem[0]
                //            <I4,1 '1' [RPTID]>     => Item[0].SubItem[2].SubItem[0].SubItem[0]
                //            <L,2 [VCOUNT]          => Item[0].SubItem[2].SubItem[0].SubItem[1]
                //                <A,5 'LOT11' [V]>  => Item[0].SubItem[2].SubItem[0].SubItem[1].SubItem[0]
                //                <L,3 [V]           => Item[0].SubItem[2].SubItem[0].SubItem[1].SubItem[1]
                //                    <A,2 'A1'>     => Item[0].SubItem[2].SubItem[0].SubItem[1].SubItem[1].SubItem[0]
                //                    <A,2 'NG'>     => Item[0].SubItem[2].SubItem[0].SubItem[1].SubItem[1].SubItem[1]
                //                    <L,2           => Item[0].SubItem[2].SubItem[0].SubItem[1].SubItem[1].SubItem[2]
                //                        <A,2 'NG'> => Item[0].SubItem[2].SubItem[0].SubItem[1].SubItem[1].SubItem[2].SubItem[0]
                //                        <U1,1 '3'> => Item[0].SubItem[2].SubItem[0].SubItem[1].SubItem[1].SubItem[2].SubItem[1]
                //                    >
                //                >
                //            >
                //        >
                //    >
                //>
                sb.AppendFormat("    CEID={0}\r\n", message.Body.Item[0].SubItem[1].Value.ToString());

                for (int i = 0; i < message.Body.Item[0].SubItem[2].SubItem.Count; i++)
                {
                    sb.AppendFormat("    RPTID={0}\r\n", message.Body.Item[0].SubItem[2].SubItem[i].SubItem[0].Value.ToString());

                    for (int j = 0; j < message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem.Count; j++)
                    {
                        if (message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem[j].Format == SECSItemFormat.L)
                        {
                            sb.AppendFormat("        V count={0}\r\n", message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem[j].SubItem.Count);

                            for (int k = 0; k < message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem[j].SubItem.Count; k++)
                            {
                                if (message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem[j].SubItem[k].Format == SECSItemFormat.L)
                                {
                                    sb.AppendFormat("            V count={0}\r\n", message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem[j].SubItem[k].SubItem.Count);
                                }
                                else
                                {
                                    sb.AppendFormat("            V={0}\r\n", message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem[j].SubItem[k].Value.ToString());
                                }
                            }
                        }
                        else
                        {
                            sb.AppendFormat("        V={0}\r\n", message.Body.Item[0].SubItem[2].SubItem[i].SubItem[1].SubItem[j].Value.ToString());
                        }
                    }
                }

                WriteLog(string.Format("S6F11(S6F11) Received\r\n{0}", sb.ToString()));

                replyMessage = this._driver.Messages.GetMessageHeader(6, 12);

                replyMessage.Body.Add("ACKC6", SECSItemFormat.B, 1, 0);

                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                if (driverResult != MessageError.Ok)
                {
                    WriteLog(string.Format("{0}({1}) Reply : Result={2}", replyMessage.Name, replyMessage.Description, driverResult));
                }
            }
            else if (message.Stream == 2 && message.Function == 79)
            {
                StringBuilder sb = new StringBuilder(1000);

                //<L,4                                  => Item[0]
                //    <U2,1 '101' [UNITID]>             => Item[0].SubItem[0]
                //    <A,16 'GLSID_001       ' [GLSID]> => Item[0].SubItem[1]
                //    <U2,1 '10101' [GLSCODE]>          => Item[0].SubItem[2]
                //    <A,1 'a' [REQOPTION]>             => Item[0].SubItem[3]
                //>
                sb.AppendFormat("    UNITID={0}\r\n", message.Body.Item[0].SubItem[0].Value.ToString());
                sb.AppendFormat("    GLSID={0}\r\n", message.Body.Item[0].SubItem[1].Value.ToString());
                sb.AppendFormat("    GLSCODE={0}\r\n", message.Body.Item[0].SubItem[2].Value.ToString());
                sb.AppendFormat("    REQOPTION={0}\r\n", message.Body.Item[0].SubItem[3].Value.ToString());

                WriteLog(string.Format("S6F11(S6F11) Received\r\n{0}", sb.ToString()));

                if (message.Body.Item[0].SubItem[3].Value == "a")
                {
                    replyMessage = this._driver.Messages.GetMessageHeader("S2F80_L0");

                    replyMessage.Body.Add(SECSItemFormat.L, 3, null);
                    replyMessage.Body.Add("UNITID", SECSItemFormat.U2, 1, message.Body.Item[0].SubItem[0].Value);
                    replyMessage.Body.Add("ACKC2", SECSItemFormat.B, 1, 0);
                    replyMessage.Body.Add("GLSINFO", SECSItemFormat.L, 0, null);
                }
                else
                {
                    replyMessage = this._driver.Messages.GetMessageHeader("S2F80_L21");

                    replyMessage.Body.Add(SECSItemFormat.L, 3, null);
                    replyMessage.Body.Add("UNITID", SECSItemFormat.U2, 1, (short)message.Body.Item[0].SubItem[0].Value);
                    replyMessage.Body.Add("ACKC2", SECSItemFormat.B, 1, 0);
                    replyMessage.Body.Add("GLSINFO", SECSItemFormat.L, 21, null);
                    replyMessage.Body.Add("LOTID", SECSItemFormat.A, 16, "LOT_001");
                    replyMessage.Body.Add("CSTID", SECSItemFormat.A, 14, "CST_001");
                    replyMessage.Body.Add("SLOTID", SECSItemFormat.A, 3, "001");
                    replyMessage.Body.Add("RECIPEID", SECSItemFormat.A, 24, "PPID_001");
                    replyMessage.Body.Add("PRCID", SECSItemFormat.A, 8, "1000");
                    replyMessage.Body.Add("GLSODR", SECSItemFormat.A, 1, "G");
                    replyMessage.Body.Add("GLSID", SECSItemFormat.A, 16, message.Body.Item[0].SubItem[1].Value);
                    replyMessage.Body.Add("GLSJUDGE", SECSItemFormat.A, 2, "G");
                    replyMessage.Body.Add("PNLIF", SECSItemFormat.A, 400, "GGGGNNNGGGG");
                    replyMessage.Body.Add("SUBMDLIF", SECSItemFormat.A, 400, "");
                    replyMessage.Body.Add("PORTID", SECSItemFormat.B, 1, 3);
                    replyMessage.Body.Add("GLSCODE", SECSItemFormat.U2, 1, (short)message.Body.Item[0].SubItem[2].Value);
                    replyMessage.Body.Add("REINPUT", SECSItemFormat.B, 1, 0);
                    replyMessage.Body.Add("GLSTHICK", SECSItemFormat.A, 5, "6");
                    replyMessage.Body.Add("PARTNUM", SECSItemFormat.A, 14, "");
                    replyMessage.Body.Add("PRODTYPE", SECSItemFormat.A, 12, "");
                    replyMessage.Body.Add("ATTRIBUTE", SECSItemFormat.A, 12, "");
                    replyMessage.Body.Add("GLSTYPE", SECSItemFormat.A, 1, "P");
                    replyMessage.Body.Add("KEYID", SECSItemFormat.A, 32, "KEY_001");

                    replyMessage.Body.Add("LSDCOUNT", SECSItemFormat.L, 3, null);
                    replyMessage.Body.Add("LSDINFO", SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("LSDITEM", SECSItemFormat.A, 24, "LSDITEM1");
                    replyMessage.Body.Add("LSDVALUE", SECSItemFormat.A, 40, "LSDVALUE1");
                    replyMessage.Body.Add("LSDINFO", SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("LSDITEM", SECSItemFormat.A, 24, "LSDITEM2");
                    replyMessage.Body.Add("LSDVALUE", SECSItemFormat.A, 40, "LSDVALUE2");
                    replyMessage.Body.Add("LSDINFO", SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("LSDITEM", SECSItemFormat.A, 24, "LSDITEM3");
                    replyMessage.Body.Add("LSDVALUE", SECSItemFormat.A, 40, "LSDVALUE3");

                    replyMessage.Body.Add("GSDCOUNT", SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("GSDINFO", SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("GSDITEM", SECSItemFormat.A, 24, "GSDITEM1");
                    replyMessage.Body.Add("GSDVALUE", SECSItemFormat.A, 40, "GSDVALUE1");
                    replyMessage.Body.Add("GSDINFO", SECSItemFormat.L, 2, null);
                    replyMessage.Body.Add("GSDITEM", SECSItemFormat.A, 24, "GSDITEM2");
                    replyMessage.Body.Add("GSDVALUE", SECSItemFormat.A, 40, "GSDVALUE2");
                }

                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                if (driverResult != MessageError.Ok)
                {
                    WriteLog(string.Format("{0}({1}) Reply : Result={2}", replyMessage.Name, replyMessage.Description, driverResult));
                }
            }
        }

        private void _driver_OnReceivedSecondaryMessage(object sender, SECSMessage primaryMessage, SECSMessage secondaryMessage)
        {
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
                WriteLog(string.Format("S1F1(AreYouThere) Send : Result={0}", driverResult));
            }
        }

        private void btnS2F41_Click(object sender, RoutedEventArgs e)
        {
            SECSMessage message;
            MessageError driverResult;

            message = this._driver.Messages.GetMessageHeader(2, 41);

            //<L,2
            //    <A,8 'TEST_CMD' [RCMD]>
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
            // Arguments
            // Name, Format, Length, Value : <Format,Length 'Value' [Name]> -> Logging 시 name 출력 함.
            //       Format, Length, Value : <Format,Length 'Value'>        -> Logging 시 name 없이 출력 함.
            message.Body.Add(SECSItemFormat.L, 2, null);
            message.Body.Add("RCMD", SECSItemFormat.A, 8, "TEST_CMD");
            message.Body.Add("CPCOUNT", SECSItemFormat.L, 3, null);
            message.Body.Add(SECSItemFormat.L, 2, null);
            message.Body.Add("CPNAME", SECSItemFormat.A, 11, "CONFIRMFLAG");
            message.Body.Add("CPVAL", SECSItemFormat.A, 1, "Y");
            message.Body.Add(SECSItemFormat.L, 2, null);
            message.Body.Add("CPNAME", SECSItemFormat.A, 7, "MODELID");
            message.Body.Add("CPVAL", SECSItemFormat.A, 5, "MID11");
            message.Body.Add(SECSItemFormat.L, 2, null);
            message.Body.Add(SECSItemFormat.A, 3, "QTY");
            message.Body.Add(SECSItemFormat.U2, 1, 123);

            driverResult = this._driver.SendSECSMessage(message);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("S2F41(S2F41) Send : Result={0}", driverResult));
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