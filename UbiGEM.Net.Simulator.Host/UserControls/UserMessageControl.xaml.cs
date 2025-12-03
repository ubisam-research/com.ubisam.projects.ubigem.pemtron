using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UbiCom.Net.Automata.SECS2;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserMessageControl : UserControl
    {
        #region Delegate
        public delegate void WriteLog(MessageProcessor.DriverLogType logType, string logText);

        public WriteLog OnWriteLog { get; set; }
        #endregion
        #region MemberVariables
        private MessageProcessor _messageProcessor;
        private ObservableCollection<UserMessage> _displayCollection;
        private Automata _secs2Automata;
        #endregion
        #region Constructor
        public UserMessageControl()
        {
            InitializeComponent();

            this._displayCollection = new ObservableCollection<UserMessage>();
            this._secs2Automata = new Automata();
        }
        #endregion

        // UserControl Event
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.IsReadOnly = true;

            ctlFunctionUserMessage.ChangeButtonEnabled(true, true);

            InitializeUserMessages();
        }

        #region ctl_WriteLog
        private void ctl_WriteLog(MessageProcessor.DriverLogType logType, string logText)
        {
            RaiseWriteLog(logType, logText);
        }
        #endregion

        // Button Event
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            string errorLine;
            string errorText;
            SECSMessage message;
            UserMessage userMessage;
            string data;
            string name;

            userMessage = null;

            if (CheckUserMessage(out message, out errorLine, out errorText, out data) == true)
            {
                if (string.IsNullOrEmpty(errorText) == true && message != null)
                {
                    name = message.Name;

                    if (this._messageProcessor.UserMessageData.ContainsKey(name) == true)
                    {
                        userMessage = this._messageProcessor.UserMessageData[name];

                        userMessage.WaitBit = chkWaitBit.IsChecked.Value;
                        userMessage.Data = data;

                        if (this._messageProcessor.UserMessage[name] == null)
                        {
                            userMessage.Stream = message.Stream;
                            userMessage.Function = message.Function;
                        }
                    }
                    else
                    {
                        if (dgrUserMessages.SelectedItem != null)
                        {
                            userMessage = dgrUserMessages.SelectedItem as UserMessage;

                            if (userMessage != null)
                            {
                                this._messageProcessor.UserMessageData.Remove(userMessage.Name);
                            }
                        }

                        userMessage = new UserMessage()
                        {
                            Name = name,
                            Stream = message.Stream,
                            Function = message.Function,
                            Direction = message.Direction,
                            WaitBit = message.WaitBit,
                            Data = data
                        };

                        this._messageProcessor.UserMessageData[userMessage.Name] = userMessage;
                        this._messageProcessor.IsDirty = true;
                    }

                    MessageBox.Show("data is ok");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(errorText) == false)
                {
                    MessageBox.Show(string.Format(" {0} \n\n {1}", errorText, errorLine));
                }
            }

            if (userMessage != null)
            {
                AdjustUserMessages(userMessage);
            }
        }
        #region btnSend_Click
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            MessageError error;
            string errorLine;
            string errorText;
            SECSMessage message;
            UserMessage userMessage;
            string data;
            string name;

            userMessage = null;

            if (CheckUserMessage(out message, out errorLine, out errorText, out data) == true)
            {
                if (string.IsNullOrEmpty(errorText) == true && message != null && message.Direction != SECSMessageDirection.ToHost)
                {
                    name = message.Name;

                    error = this._messageProcessor.SendSECSMessage(message);

                    if (error != MessageError.Ok)
                    {
                        errorText = string.Format("Send Message Fail. Reason: {0}", error.ToString());
                    }
                    else
                    {
                        if (this._messageProcessor.UserMessageData.ContainsKey(name) == true)
                        {
                            userMessage = this._messageProcessor.UserMessageData[name];

                            userMessage.WaitBit = chkWaitBit.IsChecked.Value;
                            userMessage.Data = data;

                            if (this._messageProcessor.UserMessage[name] == null)
                            {
                                userMessage.Stream = message.Stream;
                                userMessage.Function = message.Function;
                            }
                        }
                        else
                        {
                            if (dgrUserMessages.SelectedItem != null)
                            {
                                userMessage = dgrUserMessages.SelectedItem as UserMessage;

                                if (userMessage != null)
                                {
                                    this._messageProcessor.UserMessageData.Remove(userMessage.Name);
                                }
                            }

                            userMessage = new UserMessage()
                            {
                                Name = name,
                                Stream = message.Stream,
                                Function = message.Function,
                                Direction = message.Direction,
                                WaitBit = message.WaitBit,
                                Data = data
                            };
                            this._messageProcessor.UserMessageData[userMessage.Name] = userMessage;
                            this._messageProcessor.IsDirty = true;
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(errorText) == false)
                {
                    MessageBox.Show(string.Format(" {0} \n\n {1}", errorText, errorLine));
                }
            }

            if (userMessage != null)
            {
                AdjustUserMessages(userMessage);
            }
        }
        #endregion

        // DataGrid Event
        private void UserMessage_OnAdd(object sender, EventArgs e)
        {
            UserMessage userMessage;

            userMessage = new UserMessage()
            {
                Direction = SECSMessageDirection.Both
            };
            this._displayCollection.Add(userMessage);
            this.dgrUserMessages.SelectedItem = userMessage;
            this.dgrUserMessages.ScrollIntoView(userMessage);
        }

        private void UserMessage_OnRemove(object sender, EventArgs e)
        {
            UserMessage userMessage;

            if (dgrUserMessages.SelectedItem != null)
            {
                userMessage = dgrUserMessages.SelectedItem as UserMessage;

                if (this._messageProcessor.UserMessage[userMessage.Name] != null)
                {
                    MessageBox.Show("Can not remove");
                }
                else
                {
                    this._messageProcessor.UserMessageData.Remove(userMessage.Name);
                    this._displayCollection.Remove(userMessage);

                    txtName.Text = string.Empty;
                    chkWaitBit.IsChecked = false;
                    txtStream.Text = string.Empty;
                    txtFunction.Text = string.Empty;
                    txtAbonormalMessage.Text = string.Empty;

                    txtName.IsReadOnly = true;

                    ctlFunctionUserMessage.ChangeButtonEnabled(true, false);
                }
            }
        }
        #region grdUserMessages_SelectionChanged
        private void grdUserMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserMessage userMessage;

            userMessage = dgrUserMessages.SelectedItem as UserMessage;

            if (userMessage != null)
            {
                txtName.Text = userMessage.Name;
                chkWaitBit.IsChecked = userMessage.WaitBit;
                txtStream.Text = userMessage.Stream.ToString();
                txtFunction.Text = userMessage.Function.ToString();
                txtAbonormalMessage.Text = userMessage.Data;

                if (string.IsNullOrEmpty(userMessage.Data) == true && this._messageProcessor.UserMessage[userMessage.Name] != null)
                {
                    txtAbonormalMessage.Text = this._messageProcessor.MakeSECS2Log(this._messageProcessor.UserMessage[userMessage.Name].Body);
                }

                if (this._messageProcessor.UserMessage[userMessage.Name] != null)
                {
                    txtName.IsReadOnly = true;
                    ctlFunctionUserMessage.ChangeButtonEnabled(true, false);
                }
                else
                {
                    txtName.IsReadOnly = false;
                    ctlFunctionUserMessage.ChangeButtonEnabled(true, true);
                }
            }
        }
        #endregion

        // Public Method
        #region AdjustUserMessages
        public void AdjustUserMessages(UserMessage userMessage)
        {
            UserMessage targetMessage;

            if (userMessage != null && dgrUserMessages.SelectedItem != null)
            {
                targetMessage = dgrUserMessages.SelectedItem as UserMessage;

                targetMessage.Name = userMessage.Name;
                targetMessage.Direction = userMessage.Direction;
                targetMessage.Stream = userMessage.Stream;
                targetMessage.Function = userMessage.Function;
                targetMessage.WaitBit = userMessage.WaitBit;
                targetMessage.Data = userMessage.Data;

                dgrUserMessages.Items.Refresh();
            }
        }
        #endregion
        #region InitializeUserMessages
        public void InitializeUserMessages()
        {
            if (this._messageProcessor != null)
            {
                this._displayCollection.Clear();

                foreach (var message in this._messageProcessor.UserMessageData.Values)
                {
                    this._displayCollection.Add(message);
                }

                dgrUserMessages.ItemsSource = null;
                dgrUserMessages.ItemsSource = this._displayCollection;
            }
        }
        #endregion
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion

        // Method
        #region CheckUserMessage
        private bool CheckUserMessage(out SECSMessage message, out string errorLine, out string errorText, out string data)
        {
            bool result;
            string stringValue;
            int stream;
            int function;
            bool isWait;
            string name;

            message = null;
            stream = 0;
            function = 0;
            isWait = false;
            errorLine = string.Empty;
            errorText = string.Empty;
            result = false;
            data = string.Empty;

            name = txtName.Text.Trim();

            if (string.IsNullOrEmpty(name) == true)
            {
                errorText = " Make message fail. \n\n Reason: Name is empty";
            }
            else
            {
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    stringValue = txtStream.Text.Trim();

                    if (int.TryParse(stringValue, out stream) == false)
                    {
                        errorText = " invalid stream";
                    }
                    else
                    {
                        if (stream < 1 || stream > 127)
                        {
                            errorText = " invalid stream";
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    stringValue = txtFunction.Text.Trim();

                    if (int.TryParse(stringValue, out function) == false)
                    {
                        errorText = " invalid function";
                    }
                    else
                    {
                        if (function < 1 || function > 255)
                        {
                            errorText = " invalid function";
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    isWait = chkWaitBit.IsChecked.Value;

                    data = txtAbonormalMessage.Text.Trim();

                    message = _secs2Automata.MakeSECSMessageUsingAutomata(name, SECSMessageDirection.Both, stream, function, isWait, data, out errorLine, out errorText);

                    if (message != null && string.IsNullOrEmpty(errorText) == true)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }
        #endregion
        #region RaiseWriteLog
        private void RaiseWriteLog(MessageProcessor.DriverLogType logType, string logText)
        {
            if (this.OnWriteLog != null)
            {
                this.OnWriteLog.BeginInvoke(logType, logText, null, null);
            }
        }
        #endregion
    }
}