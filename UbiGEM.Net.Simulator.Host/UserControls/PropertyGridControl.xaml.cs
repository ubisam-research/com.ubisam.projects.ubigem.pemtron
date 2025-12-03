using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbiCom.Net.Structure;


namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// ClientMenuControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PropertyGridControl : UserControl
    {
        #region Property
        public object SelectedObject
        {
            get
            {
                return pgr.SelectedObject;
            }
            set
            {
                pgr.SelectedObject = value;
            }
        }
        #endregion
        #region Constructor
        public PropertyGridControl()
        {
            InitializeComponent();

            pgr.SelectedGridItemChanged += pgr_SelectedGridItemChanged;
        }

        private void pgr_SelectedGridItemChanged(object sender, System.Windows.Forms.SelectedGridItemChangedEventArgs e)
        {
        }
        #endregion
    }
}