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

namespace UbiCom.Sample
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EqpMainWindow : Window
    {
        private const int MAX_LOG_LINES = 5000;

        private HSMSDriver _driver;
        private bool _addSecs2Log;
        private bool _autoScroll;

        public EqpMainWindow()
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
                replyMessage = this._driver.Messages.GetMessageHeader(1, 2, DeviceType.Equipment);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                replyMessage.Body.Add("MDLN", SECSItemFormat.A, 6, "UbiSAM");
                replyMessage.Body.Add("SOFTREV", SECSItemFormat.A, 5, "1.0.0");

                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                if (driverResult != MessageError.Ok)
                {
                    WriteLog(string.Format("S1F2(OnLineData) Reply : Result={0}", driverResult));
                }
            }
            else if (message.Stream == 2 && message.Function == 41)
            {
                StringBuilder sb = new StringBuilder(1000);

                //<L,2                                      => Item[0]
                //    <A,8 'TEST_CMD' [RCMD]>               => Item[0].SubItem[0]
                //    <L,3 [CPCOUNT]                        => Item[0].SubItem[1]
                //        <L,2                              => Item[0].SubItem[1].SubItem[0]
                //            <A,11 'CONFIRMFLAG' [CPNAME]> => Item[0].SubItem[1].SubItem[0].SubItem[0]
                //            <A,1 'Y' [CPVAL]>             => Item[0].SubItem[1].SubItem[0].SubItem[1]
                //        >
                //        <L,2                              => Item[0].SubItem[1].SubItem[1]
                //            <A,7 'MODELID' [CPNAME]>      => Item[0].SubItem[1].SubItem[1].SubItem[0]
                //            <A,5 'MID11' [CPVAL]>         => Item[0].SubItem[1].SubItem[1].SubItem[1]
                //        >
                //        <L,2                              => Item[0].SubItem[1].SubItem[2]
                //            <A,3 'QTY'>                   => Item[0].SubItem[1].SubItem[2].SubItem[0]
                //            <U2,1 '123'>                  => Item[0].SubItem[1].SubItem[2].SubItem[1]
                //        >
                //    >
                //>
                sb.AppendFormat("    RCMD={0}\r\n", message.Body.Item[0].SubItem[0].Value.ToString());

                for (int i = 0; i < message.Body.Item[0].SubItem[1].SubItem.Count; i++)
                {
                    sb.AppendFormat("    CP Name={0}, CP Value={1}\r\n", message.Body.Item[0].SubItem[1].SubItem[i].SubItem[0].Value.ToString(), message.Body.Item[0].SubItem[1].SubItem[i].SubItem[1].Value.ToString());
                }

                WriteLog(string.Format("S2F41(S2F41) Received\r\n{0}", sb.ToString()));

                replyMessage = this._driver.Messages.GetMessageHeader(2, 42);

                replyMessage.Body.Add(SECSItemFormat.L, 2, null);
                replyMessage.Body.Add("HCACK", SECSItemFormat.B, 1, 0);
                replyMessage.Body.Add("CPCOUNT", SECSItemFormat.L, 3, null);
                replyMessage.Body.Add("CPINFO", SECSItemFormat.L, 2, null);
                replyMessage.Body.Add("CPNAME", SECSItemFormat.A, 11, "CONFIRMFLAG");
                replyMessage.Body.Add("CPACK", SECSItemFormat.B, 1, 0);
                replyMessage.Body.Add("CPINFO", SECSItemFormat.L, 2, null);
                replyMessage.Body.Add("CPNAME", SECSItemFormat.A, 7, "MODELID");
                replyMessage.Body.Add("CPACK", SECSItemFormat.B, 1, 1);
                replyMessage.Body.Add("CPINFO", SECSItemFormat.L, 2, null);
                replyMessage.Body.Add("CPNAME", SECSItemFormat.A, 3, "QTY");
                replyMessage.Body.Add("CPACK", SECSItemFormat.B, 1, 2);

                driverResult = this._driver.ReplySECSMessage(message, replyMessage);

                if (driverResult != MessageError.Ok)
                {
                    WriteLog(string.Format("S2F42(S2F42) Reply : Result={0}", driverResult));
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
                WriteLog(string.Format("{0}({1}) Send : Result={2}", message.Name, message.Description, driverResult));
            }
        }

        private void btnS6F11_Click(object sender, RoutedEventArgs e)
        {
            SECSMessage message;
            MessageError driverResult;

            message = this._driver.Messages.GetMessageHeader(6, 11);

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
            // Arguments
            // Name, Format, Length, Value : <Format,Length 'Value' [Name]> -> Logging 시 name 출력 함.
            //       Format, Length, Value : <Format,Length 'Value'>        -> Logging 시 name 없이 출력 함.
            message.Body.Add(SECSItemFormat.L, 3, null);
            message.Body.Add("DATAID", SECSItemFormat.I4, 1, 6750318);
            message.Body.Add("CEID", SECSItemFormat.I4, 1, 2097200);
            message.Body.Add("RPTIDCOUNT", SECSItemFormat.L, 1, null);
            message.Body.Add(SECSItemFormat.L, 2, null);
            message.Body.Add("RPTID", SECSItemFormat.I4, 1, 1);
            message.Body.Add("VCOUNT", SECSItemFormat.L, 2, null);
            message.Body.Add("V", SECSItemFormat.A, 5, "LOT11");
            message.Body.Add("V", SECSItemFormat.L, 3, null);
            message.Body.Add(SECSItemFormat.A, 2, "A1");
            message.Body.Add(SECSItemFormat.A, 2, "NG");
            message.Body.Add(SECSItemFormat.L, 2, null);
            message.Body.Add(SECSItemFormat.A, 2, "NG");
            message.Body.Add(SECSItemFormat.U1, 1, 3);

            driverResult = this._driver.SendSECSMessage(message);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0}({1}) Send : Result={2}", message.Name, message.Description, driverResult));
            }
        }

        private void btnS2F79_0_Click(object sender, RoutedEventArgs e)
        {
            SECSMessage message;
            MessageError driverResult;

            message = this._driver.Messages.GetMessageHeader(2, 79);

            //<L,4
            //    <U2,1 '101' [UNITID]>
            //    <A,16 'GLSID_001       ' [GLSID]>
            //    <U2,1 '10101' [GLSCODE]>
            //    <A,1 'a' [REQOPTION]> // case -> GLSSTATUS=0
            //>
            message.Body.Add(SECSItemFormat.L, 4, null);
            message.Body.Add("UNITID", SECSItemFormat.U2, 1, 101);
            message.Body.Add("GLSID", SECSItemFormat.A, 16, "GLSID_001");
            message.Body.Add("GLSCODE", SECSItemFormat.U2, 1, 10101);
            message.Body.Add(SECSItemFormat.A, 1, "a");

            driverResult = this._driver.SendSECSMessage(message);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0}({1}) Send : Result={2}", message.Name, message.Description, driverResult));
            }
        }

        private void btnS2F79_1_Click(object sender, RoutedEventArgs e)
        {
            SECSMessage message;
            MessageError driverResult;

            message = this._driver.Messages.GetMessageHeader(2, 79);

            //<L,4
            //    <U2,1 '101' [UNITID]>
            //    <A,16 'GLSID_001       ' [GLSID]>
            //    <U2,1 '10101' [GLSCODE]>
            //    <A,1 'b' [REQOPTION]> // case -> GLSSTATUS=1
            //>
            message.Body.Add(SECSItemFormat.L, 4, null);
            message.Body.Add("UNITID", SECSItemFormat.U2, 1, 101);
            message.Body.Add("GLSID", SECSItemFormat.A, 16, "GLSID_001");
            message.Body.Add("GLSCODE", SECSItemFormat.U2, 1, 10101);
            message.Body.Add(SECSItemFormat.A, 1, "b");

            driverResult = this._driver.SendSECSMessage(message);

            if (driverResult != MessageError.Ok)
            {
                WriteLog(string.Format("{0}({1}) Send : Result={2}", message.Name, message.Description, driverResult));
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