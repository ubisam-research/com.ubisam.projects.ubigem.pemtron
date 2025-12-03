using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info.ExpandedStructure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// SettingVariableWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingVariableWindow : Window
    {
        #region Enum
        #region VariableSelectType
        public enum VariableSelectType
        {
            S1F3,
            S1F11,
            S1F21,
            S2F13,
            S2F15,
            S2F29,
            S2F47
        }
        #endregion
        #endregion
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

            public string VID { get; set; }

            public Structure.VariableType VIDType { get; set; }

            public string Name { get; set; }

            public bool IsUse { get; set; }

            public string Value { get; set; }

            public UbiCom.Net.Structure.SECSItemFormat Format { get; set; }
            public int Length { get; set; }

            public string Description { get; set; }
            #endregion

            // Public Method
            #region ToString
            public override string ToString()
            {
                return string.Format("ID={0}, Name={1}, Value={2}", this.VID, this.Name, this.Value);
            }
            #endregion

            // Protected Method
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
        public delegate void VariableSelectedEventHandler(VariableSelectType selectType, List<string> selectedItems);
        public delegate void VariableSelectedWithValueEventHandler(VariableSelectType selectType, Dictionary<string, dynamic> selectedItems);

        public event VariableSelectedEventHandler OnVariableSelected;
        public event VariableSelectedWithValueEventHandler OnVariableSelectedWithValue;
        #endregion

        #region MemberVariable
        private ObservableCollection<DisplayData> _displayData;
        private VariableSelectType _selectType;

        private MessageProcessor _messageProcessor;
        #endregion

        #region Constructor
        public SettingVariableWindow()
        {
            InitializeComponent();

            this._displayData = new ObservableCollection<DisplayData>();
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

            dgrVariable.ItemsSource = this._displayData;
        }
        #endregion

        // Button Event
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        // CheckBox Event
        #region chkAll_Click
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked;

            isChecked = (sender as CheckBox).IsChecked.GetValueOrDefault();

            foreach (DisplayData tempItem in this._displayData)
            {
                tempItem.IsSelected = isChecked;
            }
        }
        #endregion

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor, VariableSelectType selectType, List<string> selecteItems)
        {
            string title;

            ExpandedVariableInfo expandedVariableInfo;

            this._messageProcessor = messageProcessor;
            this._selectType = selectType;

            dgcIsUse.Visibility = Visibility.Collapsed;

            switch (selectType)
            {
                case VariableSelectType.S1F3:
                    dgcValue.Visibility = Visibility.Collapsed;
                    title = "Selected Equipment Status Request(S1F3)";
                    break;
                case VariableSelectType.S1F11:
                    dgcValue.Visibility = Visibility.Collapsed;
                    title = "Status Variable Namelist(S1F11)";
                    break;
                case VariableSelectType.S1F21:
                    dgcValue.Visibility = Visibility.Collapsed;
                    title = "Data Variable Namelist(S1F21)";
                    break;
                case VariableSelectType.S2F13:
                    dgcVIDType.Visibility = Visibility.Collapsed;
                    dgcValue.Visibility = Visibility.Collapsed;
                    title = "Equipment Constant(S2F13)";
                    break;
                case VariableSelectType.S2F15:
                    dgcVIDType.Visibility = Visibility.Collapsed;
                    title = "New Equipment Constant(S2F15)";
                    break;
                case VariableSelectType.S2F29:
                    dgcVIDType.Visibility = Visibility.Collapsed;
                    dgcValue.Visibility = Visibility.Collapsed;
                    title = "Equipment Constant Namelist(S2F29)";
                    break;
                case VariableSelectType.S2F47:
                    dgcValue.Visibility = Visibility.Collapsed;
                    title = "Variable Limit Attribute Request(S2F47)";
                    break;
                default:
                    title = string.Empty;
                    break;
            }

            grbTitle.Header = title;

            this._displayData = new ObservableCollection<DisplayData>();

            if (selectType == VariableSelectType.S2F13 ||
                selectType == VariableSelectType.S2F15 ||
                selectType == VariableSelectType.S2F29)
            {
                foreach (Structure.VariableInfo tempVariable in this._messageProcessor.VariableCollection.ECV.Items.Where(t => t.VID != t.Name && t.IsUse == true).OrderBy(t => t.VID))
                {
                    expandedVariableInfo = tempVariable as ExpandedVariableInfo;

                    this._displayData.Add(new DisplayData()
                    {
                        IsSelected = selecteItems.Contains(expandedVariableInfo.VID),
                        VID = expandedVariableInfo.VID,
                        VIDType = expandedVariableInfo.VIDType,
                        Name = expandedVariableInfo.Name,
                        Length = expandedVariableInfo.Length,
                        Value = expandedVariableInfo.Value,
                        IsUse = expandedVariableInfo.IsUse,
                        Format = expandedVariableInfo.Format,
                        Description = expandedVariableInfo.Description
                    });
                }
            }
            else
            {
                if (selectType != VariableSelectType.S1F21)
                {
                    foreach (Structure.VariableInfo tempVariable in this._messageProcessor.VariableCollection.SV.Items.Where(t => t.VID != t.Name && t.IsUse == true).OrderBy(t => t.VID))
                    {
                        expandedVariableInfo = tempVariable as ExpandedVariableInfo;

                        this._displayData.Add(new DisplayData()
                        {
                            IsSelected = selecteItems.Contains(expandedVariableInfo.VID),
                            VID = expandedVariableInfo.VID,
                            VIDType = expandedVariableInfo.VIDType,
                            Name = expandedVariableInfo.Name,
                            Length = expandedVariableInfo.Length,
                            Value = expandedVariableInfo.Value,
                            IsUse = expandedVariableInfo.IsUse,
                            Format = expandedVariableInfo.Format,
                            Description = expandedVariableInfo.Description
                        });
                    }
                }

                if (selectType != VariableSelectType.S1F11)
                {
                    foreach (Structure.VariableInfo tempVariable in this._messageProcessor.VariableCollection.DVVal.Items.Where(t => t.VID != t.Name && t.IsUse == true).OrderBy(t => t.VID))
                    {
                        expandedVariableInfo = tempVariable as ExpandedVariableInfo;

                        this._displayData.Add(new DisplayData()
                        {
                            IsSelected = selecteItems.Contains(expandedVariableInfo.VID),
                            VID = expandedVariableInfo.VID,
                            VIDType = expandedVariableInfo.VIDType,
                            Name = expandedVariableInfo.Name,
                            Length = expandedVariableInfo.Length,
                            Value = expandedVariableInfo.Value,
                            IsUse = expandedVariableInfo.IsUse,
                            Format = expandedVariableInfo.Format,
                            Description = expandedVariableInfo.Description
                        });
                    }
                }
            }
        }
        #endregion

        // Private Method
        #region GetValue
        private dynamic GetValue(DisplayData displayData)
        {
            dynamic result;
            sbyte sbyteValue;
            byte byteValue;
            short shortValue;
            ushort ushortValue;
            int intValue;
            uint uintValue;
            long longValue;
            ulong ulongValue;
            float floatValue;
            double doubleValue;
            bool boolValue;

            switch (displayData.Format)
            {
                case UbiCom.Net.Structure.SECSItemFormat.I1:
                    if (sbyte.TryParse(displayData.Value, out sbyteValue) == true)
                    {
                        result = sbyteValue;
                    }
                    else
                    {
                        result = (sbyte)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.I2:
                    if (short.TryParse(displayData.Value, out shortValue) == true)
                    {
                        result = shortValue;
                    }
                    else
                    {
                        result = (short)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.I4:
                    if (int.TryParse(displayData.Value, out intValue) == true)
                    {
                        result = intValue;
                    }
                    else
                    {
                        result = (int)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.I8:
                    if (long.TryParse(displayData.Value, out longValue) == true)
                    {
                        result = longValue;
                    }
                    else
                    {
                        result = (long)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.U1:
                    if (byte.TryParse(displayData.Value, out byteValue) == true)
                    {
                        result = byteValue;
                    }
                    else
                    {
                        result = (byte)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.U2:
                    if (ushort.TryParse(displayData.Value, out ushortValue) == true)
                    {
                        result = ushortValue;
                    }
                    else
                    {
                        result = (ushort)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.U4:
                    if (uint.TryParse(displayData.Value, out uintValue) == true)
                    {
                        result = uintValue;
                    }
                    else
                    {
                        result = (uint)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.U8:
                    if (ulong.TryParse(displayData.Value, out ulongValue) == true)
                    {
                        result = ulongValue;
                    }
                    else
                    {
                        result = (ulong)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.F4:
                    if (float.TryParse(displayData.Value, out floatValue) == true)
                    {
                        result = floatValue;
                    }
                    else
                    {
                        result = (float)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.F8:
                    if (double.TryParse(displayData.Value, out doubleValue) == true)
                    {
                        result = doubleValue;
                    }
                    else
                    {
                        result = (double)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.B:
                    if (byte.TryParse(displayData.Value, out byteValue) == true)
                    {
                        result = byteValue;
                    }
                    else
                    {
                        result = (byte)0;
                    }

                    break;
                case UbiCom.Net.Structure.SECSItemFormat.Boolean:
                    if (bool.TryParse(displayData.Value, out boolValue) == true)
                    {
                        result = boolValue;
                    }
                    else
                    {
                        result = false;
                    }

                    break;
                default:
                    result = displayData.Value;
                    break;
            }

            return result;
        }
        #endregion

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedItems;
            Dictionary<string, dynamic> selectedItemsWithValue;
            Structure.VariableInfo variableInfo;
            ExpandedVariableInfo expandedVariableInfo;

            if (this._selectType == VariableSelectType.S2F15)
            {
                selectedItemsWithValue = new Dictionary<string, dynamic>();

                foreach (DisplayData tempItem in this._displayData)
                {
                    if (tempItem.Format != UbiCom.Net.Structure.SECSItemFormat.L)
                    {
                        variableInfo = this._messageProcessor.VariableCollection[tempItem.VID];

                        if (variableInfo != null)
                        {
                            expandedVariableInfo = variableInfo as ExpandedVariableInfo;

                            if (tempItem.Value != null)
                            {
                                expandedVariableInfo.Value = tempItem.Value;
                            }
                            else
                            {
                                expandedVariableInfo.Value = string.Empty;
                            }

                            if (expandedVariableInfo.VIDType == Structure.VariableType.ECV)
                            {
                                this._messageProcessor.UpdateSystemConfig(expandedVariableInfo.Name, tempItem.Value);
                            }
                        }
                    }

                    if (tempItem.IsSelected == true)
                    {
                        selectedItemsWithValue[tempItem.VID] = GetValue(tempItem);
                    }
                }

                if (this.OnVariableSelectedWithValue != null)
                {
                    this.OnVariableSelectedWithValue(this._selectType, selectedItemsWithValue);
                }
            }
            else
            {
                selectedItems = new List<string>();

                selectedItems.AddRange(this._displayData.Where(t => t.IsSelected == true).Select(t => t.VID));

                if (this.OnVariableSelected != null)
                {
                    this.OnVariableSelected(this._selectType, selectedItems);
                }
            }

            this._messageProcessor.IsDirty = true;
            Close();
        }
    }
}