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

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// UCUpDownButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCUpDownButton : UserControl
    {
        #region Event
        public event EventHandler OnFirst;
        public event EventHandler OnUp;
        public event EventHandler OnDown;
        public event EventHandler OnLast;
        #endregion

        #region Constructor
        public UCUpDownButton()
        {
            InitializeComponent();
        }
        #endregion

        // Button Event
        #region btnFirst_Click
        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnFirst != null)
            {
                this.OnFirst(sender, e);
            }
        }
        #endregion
        #region btnUp_Click
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnUp != null)
            {
                this.OnUp(sender, e);
            }
        }
        #endregion
        #region btnDown_Click
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnDown != null)
            {
                this.OnDown(sender, e);
            }
        }
        #endregion
        #region bntLast_Click
        private void bntLast_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnLast != null)
            {
                this.OnLast(sender, e);
            }
        }
        #endregion

        // Public Method
        #region ChangeButtonEnabled
        public void ChangeButtonEnabled(bool firstEnabled, bool upEnabled, bool downEnabled, bool lastEnabled)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {
                btnFirst.IsEnabled = firstEnabled;
                btnUp.IsEnabled = upEnabled;
                btnDown.IsEnabled = downEnabled;
                btnLast.IsEnabled = lastEnabled;
            }));
        }
        #endregion
        #region ChangeButtonEnabled
        public void ChangeButtonEnabledAll(bool enabled)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {
                btnFirst.IsEnabled = enabled;
                btnUp.IsEnabled = enabled;
                btnDown.IsEnabled = enabled;
                btnLast.IsEnabled = enabled;
            }));
        }
        #endregion
    }
}
