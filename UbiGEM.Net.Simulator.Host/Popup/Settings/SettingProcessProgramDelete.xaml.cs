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
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingTerminalWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingProcessProgramDelete : Window
    {
        #region StringWrapper
        public class StringWrapper
        {
            public string Value { get; set; }
        }
        #endregion
        #region MemberVariable
        private ObservableCollection<StringWrapper> _ppidCollection;
        private MessageProcessor _messageProcessor;
        #endregion
        #region Constructor
        public SettingProcessProgramDelete()
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

            this._ppidCollection = new ObservableCollection<StringWrapper>();

            foreach (string ppid in this._messageProcessor.CurrentSetting.ProcessProgramDelete)
            {
                this._ppidCollection.Add(new StringWrapper()
                {
                    Value = ppid
                });
            }

            dgrPPID.ItemsSource = this._ppidCollection;
        }
        #endregion

        // DataGrid Event
        #region ClientMenuControl_OnAdd
        private void ClientMenuControl_OnAdd(object sender, EventArgs e)
        {
            this._ppidCollection.Add(new StringWrapper());
        }
        #endregion
        #region ClientMenuControl_OnRemove
        private void ClientMenuControl_OnRemove(object sender, EventArgs e)
        {
            int selectedIndex;

            selectedIndex = dgrPPID.SelectedIndex;
            if (selectedIndex >= 0)
            {
                this._ppidCollection.RemoveAt(selectedIndex);
            }
        }
        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this._messageProcessor.CurrentSetting.ProcessProgramDelete.Clear();

            foreach (StringWrapper item in this._ppidCollection)
            {
                if (string.IsNullOrEmpty(item.Value) == false)
                {
                    this._messageProcessor.CurrentSetting.ProcessProgramDelete.Add(item.Value);
                }
            }
            this._messageProcessor.IsDirty = true;
            Close();
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