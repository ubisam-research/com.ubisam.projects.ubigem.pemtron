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
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClientMenuControl : UserControl
    {
        #region Event
        public event EventHandler OnAdd;
        public event EventHandler OnRemove;
        #endregion
        #region Constructor
        public ClientMenuControl()
        {
            InitializeComponent();
        }
        #endregion
        // Button Click
        #region btnAdd_Click
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnAdd != null)
            {
                this.OnAdd(this, null);
            }
        }
        #endregion
        #region btnRemove_Click
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnRemove != null)
            {
                this.OnRemove(this, null);
            }
        }
        #endregion

        // Public Method
        #region ChangeButtonEnabled
        public void ChangeButtonEnabled(bool addEnabled, bool removeEnabled)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {
                btnAdd.IsEnabled = addEnabled;
                if (addEnabled == true)
                {
                    btnAdd.Visibility = Visibility.Visible;
                }
                else
                {
                    btnAdd.Visibility = Visibility.Collapsed;
                }

                btnRemove.IsEnabled = removeEnabled;
                if (removeEnabled == true)
                {
                    btnRemove.Visibility = Visibility.Visible;
                }
                else
                {
                    btnRemove.Visibility = Visibility.Collapsed;
                }
            }));
        }
        #endregion
    }
}
