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
using System.Windows.Shapes;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingFormattedProcessProgram.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingFormattedProcessProgram : Window
    {
        #region enum
        public enum Type
        {
            S7F5,
            S7F25
        }
        #endregion
        #region Event
        public delegate void PPIDChanged(Type type, string ppid);
        public event PPIDChanged OnPPIDChanged;
        #endregion
        #region MemberVariable
        private MessageProcessor _messageProcessor;
        private Type _type;
        #endregion
        #region Constructor
        public SettingFormattedProcessProgram()
        {
            InitializeComponent();
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

            txtPPID.Text = this._messageProcessor.CurrentSetting.ProcessProgramIDS7F25;
        }
        #endregion
        #region Button Event
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            string ppid;

            ppid = txtPPID.Text;

            if (ppid.Length == 0)
            {
                MessageBox.Show(" ppid is blank ");
            }
            else
            {
                if (this.OnPPIDChanged != null)
                {
                    this.OnPPIDChanged(this._type, ppid);
                }

                Close();
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, Type type)
        {
            this._messageProcessor = messageProcessor;
            this._type = type;
        }
        #endregion
    }
}