using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReplyMessageControl : UserControl
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private ObservableCollection<UseReplyMessage> _displayInfo;
        #endregion
        #region Constructor
        public ReplyMessageControl()
        {
            InitializeComponent();
        }
        #endregion

        // UserControl Event
        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent;

            parent = this.Parent;

            if (parent != null)
            {
                while (LogicalTreeHelper.GetParent(parent) != null && parent is Window == false)
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }
            }
            InitializeCurrentReplyMessage();
        }
        #endregion

        // DataGrid CheckBox Event
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox;
            bool sendReply;

            if (_messageProcessor != null && e.Source != null)
            {
                checkBox = e.Source as CheckBox;

                if (checkBox != null)
                {
                    sendReply = checkBox.IsChecked.Value;
                    
                    foreach (var replyItem in this._messageProcessor.CurrentSetting.UseReplyCollection)
                    {
                        replyItem.SendReply = sendReply;
                    }
                }
            }
        }
        // Public Method
        #region InitializeCurrentReplyMessage
        public void InitializeCurrentReplyMessage()
        {
            if (this._messageProcessor != null)
            {
                this._displayInfo = new ObservableCollection<UseReplyMessage>(this._messageProcessor.CurrentSetting.UseReplyCollection);
                dgrReply.ItemsSource = this._displayInfo;
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion
    }
}
