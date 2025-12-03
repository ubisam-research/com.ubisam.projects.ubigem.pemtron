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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UbiGEM.Net.Simulator.Host.UserControls
{
    /// <summary>
    /// SecsFormatComboBoxControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SecsFormatComboBoxControl : UserControl
    {
        #region [Dependency Properties]
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                                        typeof(List<UbiCom.Net.Structure.SECSItemFormat>),
                                        typeof(SecsFormatComboBoxControl),
                                        new FrameworkPropertyMetadata(null,
                                                                      new PropertyChangedCallback(SecsFormatComboBoxControl.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems",
                                        typeof(List<UbiCom.Net.Structure.SECSItemFormat>),
                                        typeof(SecsFormatComboBoxControl),
                                        new FrameworkPropertyMetadata(null,
                                                                      new PropertyChangedCallback(SecsFormatComboBoxControl.OnSelectedItemsChanged)));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                                        typeof(string),
                                        typeof(SecsFormatComboBoxControl),
                                        new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText",
                                        typeof(string),
                                        typeof(SecsFormatComboBoxControl),
                                        new UIPropertyMetadata(string.Empty));
        #endregion

        #region [Variables]
        private ObservableCollection<Node> _nodeList;
        #endregion

        #region [Properties]
        public List<UbiCom.Net.Structure.SECSItemFormat> ItemsSource
        {
            get
            {
                return (List<UbiCom.Net.Structure.SECSItemFormat>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public List<UbiCom.Net.Structure.SECSItemFormat> SelectedItems
        {
            get
            {
                return (List<UbiCom.Net.Structure.SECSItemFormat>)GetValue(SelectedItemsProperty);
            }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }
        #endregion

        #region Constructor
        public SecsFormatComboBoxControl()
        {
            InitializeComponent();

            this._nodeList = new ObservableCollection<Node>();
        }
        #endregion

        #region Events
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SecsFormatComboBoxControl control = (SecsFormatComboBoxControl)d;

            control.DisplayInControl();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SecsFormatComboBoxControl control = (SecsFormatComboBoxControl)d;

            control.SelectNodes();
            control.SetText();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox clickedBox = (CheckBox)sender;
            int selectedCount;

            if (clickedBox.Content.ToString() == "All")
            {
                if (clickedBox.IsChecked.Value)
                {
                    foreach (Node node in this._nodeList)
                    {
                        node.IsSelected = true;
                    }
                }
                else
                {
                    foreach (Node node in this._nodeList)
                    {
                        node.IsSelected = false;
                    }
                }
            }
            else
            {
                selectedCount = 0;

                foreach (Node s in this._nodeList)
                {
                    if (s.IsSelected && s.Title != "All")
                    {
                        selectedCount++;
                    }
                }

                if (selectedCount == this._nodeList.Count - 1)
                {
                    this._nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = true;
                }
                else
                {
                    this._nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = false;
                }
            }

            SetSelectedItems();
            SetText();
        }
        #endregion

        // Method
        #region SelectNodes
        private void SelectNodes()
        {
            Node node;

            foreach (UbiCom.Net.Structure.SECSItemFormat tempItem in this.SelectedItems)
            {
                node = this._nodeList.FirstOrDefault(t => t.Title == tempItem.ToString());

                if (node != null)
                {
                    node.IsSelected = true;
                }
            }
        }
        #endregion
        #region SetSelectedItems
        private void SetSelectedItems()
        {
            if (this.SelectedItems == null)
            {
                this.SelectedItems = new List<UbiCom.Net.Structure.SECSItemFormat>();
            }

            SelectedItems.Clear();

            foreach (Node node in this._nodeList)
            {
                if (node.IsSelected && node.Title != "All")
                {
                    if (this.ItemsSource.Count > 0)
                    {
                        SelectedItems.Add((UbiCom.Net.Structure.SECSItemFormat)Enum.Parse(typeof(UbiCom.Net.Structure.SECSItemFormat), node.Title));
                    }
                }
            }
        }
        #endregion
        #region DisplayInControl
        private void DisplayInControl()
        {
            Node node;

            this._nodeList.Clear();

            if (this.ItemsSource.Count > 0)
            {
                this._nodeList.Add(new Node("All"));
            }

            foreach (object tempItem in this.ItemsSource)
            {
                node = new Node(tempItem.ToString());

                this._nodeList.Add(node);
            }

            cboMultiSelect.ItemsSource = this._nodeList;
        }
        #endregion
        #region SetText
        private void SetText()
        {
            StringBuilder displayText;

            if (this.SelectedItems != null)
            {
                displayText = new StringBuilder();

                foreach (Node s in this._nodeList)
                {
                    if (s.IsSelected == true && s.Title == "All")
                    {
                        displayText = new StringBuilder();
                        displayText.Append("All");

                        break;
                    }
                    else if (s.IsSelected == true && s.Title != "All")
                    {
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }

                this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
            }

            if (string.IsNullOrEmpty(this.Text) == true)
            {
                this.Text = this.DefaultText;
            }
        }
        #endregion
    }

    public class Node : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region [Variables]
        private string _title;
        private bool _isSelected;
        #endregion

        #region [Properties]
        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                this._title = value;

                NotifyPropertyChanged("Title");
            }
        }

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
        #endregion

        #region Constructor
        public Node(string title)
        {
            this.Title = title;
        }
        #endregion

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}