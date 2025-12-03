using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// SettingSpoolWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingSpoolWindow : Window
    {
        #region DisplayData 
        private class DisplayData : INotifyPropertyChanged
        {
            #region Event
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
            #region MemberVariable
            private bool _isSelected;
            #endregion

            #region Properties
            public bool IsSelected
            {
                get
                {
                    return this._isSelected;
                }
                set
                {
                    this._isSelected = value;

                    NotifyPropertyChanged("IsSelected");
                }
            }

            public string SF { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
            #endregion

            #region NotifyPropertyChanged
            protected void NotifyPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion
        }
        #endregion
        #region Delegate
        public delegate void SpoolSelectedEventHandler(List<string> selectedItems);

        public event SpoolSelectedEventHandler OnSpoolSelected;
        #endregion
        #region MemberVariable
        private List<DisplayData> _displayInfo;
        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public SettingSpoolWindow()
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

            dgrSpool.ItemsSource = this._displayInfo;
        }

        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedItem;

            selectedItem = new List<string>();

            foreach (DisplayData tempItem in this._displayInfo)
            {
                if (tempItem.IsSelected == true)
                {
                    selectedItem.Add(tempItem.SF);
                }
            }

            if (this.OnSpoolSelected != null)
            {
                this.OnSpoolSelected(selectedItem);
            }

            this._messageProcessor.IsDirty = true;
            Close();
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, SECSMessageCollection secsMessageCollection, List<string> selecteItems)
        {
            this._messageProcessor = messageProcessor;

            this._displayInfo = new List<DisplayData>();

            var varMessage = from tempMessage in secsMessageCollection.MessageInfo
                             where tempMessage.Value.Stream != 9 &&
                                   tempMessage.Value.Function % 2 != 0 &&
                                   (tempMessage.Value.Direction == SECSMessageDirection.Both ||
                                    tempMessage.Value.Direction == SECSMessageDirection.ToHost)
                             orderby tempMessage.Value.Stream, tempMessage.Value.Function, tempMessage.Key
                             select new
                             {
                                 SF = string.Format("S{0}F{1}", tempMessage.Value.Stream, tempMessage.Value.Function),
                                 Name = tempMessage.Value.Name,
                                 Description = tempMessage.Value.Description
                             };

            foreach (var tempMessage in varMessage)
            {
                this._displayInfo.Add(new DisplayData()
                {
                    SF = tempMessage.SF,
                    Name = tempMessage.Name,
                    Description = tempMessage.Description,
                    IsSelected = selecteItems.Exists(t => t == tempMessage.SF)
                });
            }
        }
        #endregion
    }
}