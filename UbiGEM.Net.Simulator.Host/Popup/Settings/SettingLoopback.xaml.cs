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
    /// SettingLoopback.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingLoopback : Window
    {
        #region MemberVariable
        private List<byte> _loopbackDiagnostic;
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public SettingLoopback()
        {
            InitializeComponent();
        }
        #endregion

        // Window Event
        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string abs;

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

            abs = string.Empty;

            foreach (byte byteValue in this._loopbackDiagnostic)
            {
                abs += string.Format("{0} ", byteValue);
            }

            txtAbs.Text = abs.Trim();
        }
        #endregion
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, List<byte> loopbackDiagnostic)
        {
            this._messageProcessor = messageProcessor;
            this._loopbackDiagnostic = new List<byte>(loopbackDiagnostic);
        }
        #endregion

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            string[] splitData;
            byte byteValue;
            string errorMsg;

            errorMsg = string.Empty;

            splitData = txtAbs.Text.Split(' ');

            this._loopbackDiagnostic.Clear();

            if (splitData.Length == 0)
            {
                MessageBox.Show(" more than 1 integer seperated by blank");
            }
            else
            {
                if (splitData != null)
                {
                    foreach (string temp in splitData.Where(t => t.Length > 0))
                    {
                        if (byte.TryParse(temp, out byteValue) == true)
                        {
                            this._loopbackDiagnostic.Add(byteValue);
                        }
                        else
                        {
                            errorMsg = " Convert fail.";
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorMsg) == false)
                {
                    MessageBox.Show(errorMsg);
                }
                else
                {
                    this._messageProcessor.CurrentSetting.LoopbackDiagnostic.Clear();
                    this._messageProcessor.CurrentSetting.LoopbackDiagnostic.AddRange(this._loopbackDiagnostic);
                    this._messageProcessor.IsDirty = true;
                    Close();
                }
            }

        }
    }
}