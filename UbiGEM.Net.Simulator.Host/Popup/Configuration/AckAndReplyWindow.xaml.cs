using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AckAndReplyWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public AckAndReplyWindow()
        {
            InitializeComponent();
        }
        #endregion

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

            ctlACKs.InitializeCurrentAck();
            ctlReplyMessage.InitializeCurrentReplyMessage();
}
        #endregion
        #region Window_Closing
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
            ctlACKs.Initialize(this._messageProcessor);
            ctlReplyMessage.Initialize(this._messageProcessor);
        }
        public void InitializeData()
        {
            ctlACKs.InitializeCurrentAck();
            ctlReplyMessage.InitializeCurrentReplyMessage();
        }
        #endregion
    }
}