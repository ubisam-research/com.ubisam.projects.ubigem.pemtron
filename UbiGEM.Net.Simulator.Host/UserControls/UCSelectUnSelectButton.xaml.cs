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
    /// UCAddRemoveVerticalButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCSelectUnSelectButton : UserControl
    {
        #region Event
        public event EventHandler OnSelect;
        public event EventHandler OnUnselect;
        #endregion

        #region Constructor
        public UCSelectUnSelectButton()
        {
            InitializeComponent();
        }
        #endregion

        // Button Event
        #region btnSelect_Click
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnSelect != null)
            {
                this.OnSelect(sender, e);
            }
        }
        #endregion
        #region btnUnselect_Click
        private void btnUnselect_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnUnselect != null)
            {
                this.OnUnselect(sender, e);
            }
        }
        #endregion

        // Public Method
        #region ChangeButtonEnabled
        public void ChangeButtonEnabled(bool selectEnabled, bool unselectEnabled)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {
                btnSelect.IsEnabled = selectEnabled;
                btnUnselect.IsEnabled = unselectEnabled;
            }));
        }
        #endregion
    }
}
