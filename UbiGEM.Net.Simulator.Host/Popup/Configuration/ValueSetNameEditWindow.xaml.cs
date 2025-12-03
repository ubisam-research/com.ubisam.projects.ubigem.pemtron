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
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ValueSetNameEditWindow : Window
    {
        #region DisplayData
        public class DisplayData
        {
            #region Property
            public int OriginalIndex { set; get; }
            public string Name { set; get; }
            #endregion
        }
        #endregion
        #region Delegate
        public delegate void OnValueSetNameChanged(List<DisplayData> changedData);
        public event OnValueSetNameChanged ValueSetNameChanged;
        #endregion
        #region MemberVariable
        private ObservableCollection<DisplayData> _displayInfo;
        #endregion

        #region Constructor
        public ValueSetNameEditWindow()
        {
            InitializeComponent();
            this._displayInfo = new ObservableCollection<DisplayData>();
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
        }
        #endregion

        // User Control Event
        #region ClientMenu_OnAdd
        private void ClientMenu_OnAdd(object sender, EventArgs e)
        {
            DisplayData data;

            data = new DisplayData()
            {
                OriginalIndex = -1,
                Name = string.Empty
            };
            this._displayInfo.Add(data);
        }
        #endregion
        #region ClientMenu_OnRemove
        private void ClientMenu_OnRemove(object sender, EventArgs e)
        {
            DisplayData data;

            data = dgrValueSet.SelectedItem as DisplayData;

            if (data != null && data.Name != "Default")
            {
                this._displayInfo.Remove(data);
            }
        }
        #endregion

        // DataGrid Event

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorText;

            errorText = string.Empty;

            if (IsValid(out errorText) == false)
            {
                MessageBox.Show(errorText);
            }
            else
            {
                if (this.ValueSetNameChanged != null)
                {
                    this.ValueSetNameChanged(this._displayInfo.ToList());
                }
            }
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
        public void Initialize(List<DisplayData> originalNames)
        {
            this._displayInfo.Clear();

            foreach (DisplayData data in originalNames)
            {
                this._displayInfo.Add(data);
            }

            dgrValueSet.ItemsSource = this._displayInfo;
        }
        #endregion

        // Private Method
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            List<string> usedNames;

            errorText = string.Empty;
            result = true;

            usedNames = new List<string>();

            foreach (DisplayData data in this._displayInfo)
            {
                if (string.IsNullOrEmpty(data.Name.Trim()) == true)
                {
                    errorText = string.Format("blank name exists");
                    result = false;
                }
                else
                {
                    if (usedNames.Contains(data.Name.Trim()) == true)
                    {
                        errorText = string.Format("Dupelicated Name: {0}", data.Name.Trim());
                        result = false;
                    }
                    else
                    {
                        usedNames.Add(data.Name.Trim());
                    }
                }
            }

            return result;
        }
        #endregion
    }
}