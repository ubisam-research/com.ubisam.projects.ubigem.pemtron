using System;
using System.Collections.ObjectModel;
using System.Windows;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingTerminalWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingTerminalWindow : Window
    {
        #region Enum
        #region TerminalMessageSelectType
        public enum TerminalMessageSelectType
        {
            S10F3,
            S10F5
        }
        #endregion
        #endregion

        #region StringWrapper
        public class StringWrapper
        {
            public string Value { get; set; }
        }
        #endregion
        #region Delegate
        public delegate void TerminalMessageChangedEventHandler(TerminalMessageSelectType selectType, TerminalMessageInfo terminalMessageInfo);

        public event TerminalMessageChangedEventHandler OnTerminalMessageChanged;
        #endregion

        #region MemberVariable
        private TerminalMessageSelectType _selectType;
        private TerminalMessageInfo _terminalMessageInfo;
        private readonly ObservableCollection<StringWrapper> _terminalMessageCollection;

        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public SettingTerminalWindow()
        {
            InitializeComponent();

            this._terminalMessageCollection = new ObservableCollection<StringWrapper>();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MouseLeftButtonDown += delegate
            {
                try
                {
                    DragMove();
                }
                catch
                {
                }
            };

            if (this._selectType == TerminalMessageSelectType.S10F3)
            {
                txtTid.Text = this._terminalMessageInfo.S10F3TID;
                txtTerminamMessage.Text = this._terminalMessageInfo.S10F3TerminalMessage;
            }
            else
            {
                txtTid.Text = this._terminalMessageInfo.S10F5TID;

                foreach (var item in this._terminalMessageInfo.S10F5TerminalMessages)
                {
                    this._terminalMessageCollection.Add(new StringWrapper()
                    {
                        Value = item
                    });
                }

                dgrTerminalMessage.ItemsSource = this._terminalMessageCollection;
            }
        }
        #endregion

        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            this._terminalMessageCollection.Add(new StringWrapper()
            {
                Value = "Terminal Message"
            });
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrTerminalMessage.SelectedIndex;
            if (selectedIndex >= 0)
            {
                this._terminalMessageCollection.RemoveAt(selectedIndex);
            }
        }
        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, TerminalMessageSelectType selectType, TerminalMessageInfo terminalMessageInfo)
        {
            string title;

            this._messageProcessor = messageProcessor;
            this._selectType = selectType;

            switch (selectType)
            {
                case TerminalMessageSelectType.S10F3:
                    title = "Terminal Display, Single(S10F3)";
                    grdMultiMessage.Visibility = Visibility.Collapsed;
                    txtTerminamMessage.Visibility = Visibility.Visible;
                    break;
                case TerminalMessageSelectType.S10F5:
                    title = "Terminal Display, Multi-Block(S10F5)";
                    grdMultiMessage.Visibility = Visibility.Visible;
                    txtTerminamMessage.Visibility = Visibility.Collapsed;
                    break;
                default:
                    title = string.Empty;
                    break;
            }

            grbTitle.Header = title;

            this._terminalMessageInfo = terminalMessageInfo;
        }
        #endregion

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            string tid;
            SECSItemFormat tidFormat;
            dynamic converted;

            tid = txtTid.Text.Trim();

            if (string.IsNullOrEmpty(tid) == true)
            {
                MessageBox.Show("TID is empty");
                txtTid.Focus();
            }
            else
            {
                tidFormat = this._messageProcessor.GetSECSFormat(Info.DataDictinaryList.TID, SECSItemFormat.B);

                converted = this._messageProcessor.ConvertValue(tidFormat, tid);

                if (converted == null)
                {
                    MessageBox.Show(string.Format("invalid tid, format is {0}", tidFormat));
                    txtTid.Focus();
                }
                else
                {
                    if (this._selectType == TerminalMessageSelectType.S10F3)
                    {
                        this._terminalMessageInfo.S10F3TID = txtTid.Text.Trim();
                        this._terminalMessageInfo.S10F3TerminalMessage = txtTerminamMessage.Text;
                    }
                    else
                    {
                        this._terminalMessageInfo.S10F5TID = txtTid.Text.Trim();
                        this._terminalMessageInfo.S10F5TerminalMessages.Clear();

                        foreach (var item in this._terminalMessageCollection)
                        {
                            this._terminalMessageInfo.S10F5TerminalMessages.Add(item.Value);
                        }
                    }

                    this.OnTerminalMessageChanged?.Invoke(this._selectType, this._terminalMessageInfo);

                    this._messageProcessor.IsDirty = true;

                    Close();
                }
            }
        }
    }
}