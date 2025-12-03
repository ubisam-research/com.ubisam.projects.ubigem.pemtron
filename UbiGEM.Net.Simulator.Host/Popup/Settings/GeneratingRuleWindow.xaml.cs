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
using System.Windows.Shapes;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;
using UbiGEM.Net.Simulator.Host.Info;
using System.ComponentModel;

namespace UbiGEM.Net.Simulator.Host.Popup.Settings
{
    /// <summary>
    /// RemoteCommandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GeneratingRuleWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region [Properties]
        public string Header { get; set; }
        public string GeneratingRule
        {
            get
            {
                string result;

                result = string.Empty;

                if (this._generatingRule != null)
                {
                    result = this._generatingRule;
                }

                return result;
            }
            set
            {
                if (this._generatingRule != value)
                {
                    this._generatingRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GeneratingRule"));
                }
            }
        }
        #endregion
        #region [Member Variables]
        private MessageProcessor _processor;

        private string _generatingRule;
        #endregion
        #region [Constructors]
        public GeneratingRuleWindow()
        {
            InitializeComponent();

            this.Header = string.Empty;
            this.GeneratingRule = string.Empty;

            this._processor = null;
        }
        #endregion

        #region [Window Event Handlers]
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

            if (this._processor != null)
            {
                if (string.IsNullOrEmpty(this.Header) == true)
                {
                    gbHeader.Header = "Generating Rule Editor";
                }
                else
                {
                    gbHeader.Header = this.Header;
                }

                if (this.GeneratingRule == null)
                {
                    this.GeneratingRule = string.Empty;
                }

                if (string.IsNullOrEmpty(this.GeneratingRule) == false)
                {
                }

                txtRule.Text = this.GeneratingRule;

                dgrEC.ItemsSource = this._processor.VariableCollection.ECV.Items;
                dgrV.ItemsSource = this._processor.VariableCollection.Variables.Items;
            }
        }
        #endregion

        #region [Button Event Handlers]
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.GeneratingRule = string.Empty;
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        #endregion

        #region [Public Methods]
        public void Initialize(MessageProcessor processor)
        {
            this._processor = processor;
        }
        #endregion

        #region [DataGrid Event Handlers]
        private void dgrEC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void dgrV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion

        #region [Radio Button Event Handlers]
        private void radEC_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void radV_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void radRandom_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void radIncremental_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void HideOptions()
        {
            dgrEC.Visibility = Visibility.Collapsed;
            dgrV.Visibility = Visibility.Collapsed;
            
        }
        #endregion
    }
}