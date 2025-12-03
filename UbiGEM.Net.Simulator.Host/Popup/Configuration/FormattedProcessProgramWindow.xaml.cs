using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UbiCom.Net.Structure;
using UbiGEM.Net.Simulator.Host.Common;
using UbiGEM.Net.Simulator.Host.Info;
using UbiGEM.Net.Structure;

namespace UbiGEM.Net.Simulator.Host.Popup
{
    /// <summary>
    /// CollectionEventWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormattedProcessProgramWindow : Window
    {
        #region MemberVariable
        private MessageProcessor _messageProcessor;

        private ObservableCollection<FormattedProcessProgramInfo> _fmtPPCollection;
        private ObservableCollection<FmtPPCCodeInfo> _fmtCCodeCollection;
        private ObservableCollection<FmtPPItem> _fmtPPItemCollection;
        private List<string> _removedPPIDs;
        #endregion

        #region Constructor
        public FormattedProcessProgramWindow()
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

            this._removedPPIDs = new List<string>();

            if (this._messageProcessor != null)
            {
                this._fmtPPCollection = new ObservableCollection<FormattedProcessProgramInfo>();

                foreach (var info in this._messageProcessor.FormattedProcessProgramCollection.Items)
                {
                    this._fmtPPCollection.Add(info.Clone());
                }
            }

            dgrFmtPP.ItemsSource = this._fmtPPCollection;
        }
        #endregion

        // DataGrid Event
        #region ctlFmtPP_OnAdd
        private void ctlFmtPP_OnAdd(object sender, EventArgs e)
        {
            FormattedProcessProgramInfo selectedInfo;

            selectedInfo = new FormattedProcessProgramInfo();
            this._fmtPPCollection.Add(selectedInfo);
            selectedInfo.IsLoaded = true;

            dgrCCode.ItemsSource = null;
            dgrPPItem.ItemsSource = null;

            dgrFmtPP.SelectedItem = selectedInfo;
            dgrFmtPP.ScrollIntoView(selectedInfo);
        }
        #endregion
        #region ctlFmtPP_OnRemove
        private void ctlFmtPP_OnRemove(object sender, EventArgs e)
        {
            FormattedProcessProgramInfo selectedInfo;

            selectedInfo = dgrFmtPP.SelectedItem as FormattedProcessProgramInfo;

            if (selectedInfo != null)
            {
                this._removedPPIDs.Add(selectedInfo.PPID);
                this._fmtPPCollection.Remove(selectedInfo);

                dgrCCode.ItemsSource = null;
                dgrPPItem.ItemsSource = null;
            }
        }
        #endregion
        #region dgrFmtPP_SelectionChanged
        private void dgrFmtPP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormattedProcessProgramInfo selectedInfo;

            selectedInfo = dgrFmtPP.SelectedItem as FormattedProcessProgramInfo;

            if (selectedInfo != null)
            {
                if (selectedInfo.FmtPPCollection.Items.Count == 0)
                {
                    LoadINI(selectedInfo);
                }

                this._fmtCCodeCollection = new ObservableCollection<FmtPPCCodeInfo>();

                if (selectedInfo.FmtPPCollection.Items != null)
                {
                    foreach (var ccode in selectedInfo.FmtPPCollection.Items)
                    {
                        this._fmtCCodeCollection.Add(ccode);
                    }
                }

                dgrCCode.ItemsSource = this._fmtCCodeCollection;
                dgrPPItem.ItemsSource = null;

                if (this._fmtCCodeCollection.Count > 0)
                {
                    dgrCCode.SelectedItem = this._fmtCCodeCollection[0];
                }
            }
            else
            {
                dgrCCode.ItemsSource = null;
                dgrPPItem.ItemsSource = null;
            }
        }
        #endregion
        #region ctlCCode_OnAdd
        private void ctlCCode_OnAdd(object sender, EventArgs e)
        {
            FormattedProcessProgramInfo selectedInfo;
            FmtPPCCodeInfo ccodeInfo;

            selectedInfo = dgrFmtPP.SelectedItem as FormattedProcessProgramInfo;

            if (selectedInfo != null && this._fmtCCodeCollection != null)
            {
                ccodeInfo = new FmtPPCCodeInfo();
                this._fmtCCodeCollection.Add(ccodeInfo);
                selectedInfo.FmtPPCollection.Items.Add(ccodeInfo);

                dgrPPItem.ItemsSource = null;

                dgrCCode.SelectedItem = ccodeInfo;
                dgrCCode.ScrollIntoView(ccodeInfo);
            }
        }
        #endregion
        #region ctlCCode_OnRemove
        private void ctlCCode_OnRemove(object sender, EventArgs e)
        {
            FormattedProcessProgramInfo selectedInfo;
            FmtPPCCodeInfo ccodeInfo;

            selectedInfo = dgrFmtPP.SelectedItem as FormattedProcessProgramInfo;
            ccodeInfo = dgrCCode.SelectedItem as FmtPPCCodeInfo;

            if (selectedInfo != null && ccodeInfo != null)
            {
                this._fmtCCodeCollection.Remove(ccodeInfo);
                selectedInfo.FmtPPCollection.Items.Remove(ccodeInfo);

                dgrPPItem.ItemsSource = null;
            }
        }
        #endregion
        #region dgrCCode_SelectionChanged
        private void dgrCCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FmtPPCCodeInfo selectedCCodeInfo;

            selectedCCodeInfo = dgrCCode.SelectedItem as FmtPPCCodeInfo;

            if (selectedCCodeInfo != null)
            {
                this._fmtPPItemCollection = new ObservableCollection<FmtPPItem>();

                foreach (var ppItem in selectedCCodeInfo.Items)
                {
                    this._fmtPPItemCollection.Add(ppItem);
                }

                dgrPPItem.ItemsSource = this._fmtPPItemCollection;
            }
        }
        #endregion
        #region ctlPPItem_OnAdd
        private void ctlPPItem_OnAdd(object sender, EventArgs e)
        {
            FmtPPCCodeInfo selectedCCodeInfo;
            FmtPPItem ppItem;

            selectedCCodeInfo = dgrCCode.SelectedItem as FmtPPCCodeInfo;

            if (selectedCCodeInfo != null)
            {
                ppItem = new FmtPPItem();

                selectedCCodeInfo.Items.Add(ppItem);
                this._fmtPPItemCollection.Add(ppItem);

                dgrPPItem.SelectedItem = ppItem;
                dgrPPItem.ScrollIntoView(ppItem);
            }
        }
        #endregion
        #region ctlPPItem_OnRemove
        private void ctlPPItem_OnRemove(object sender, EventArgs e)
        {
            FmtPPCCodeInfo selectedCCodeInfo;
            FmtPPItem ppItem;

            selectedCCodeInfo = dgrCCode.SelectedItem as FmtPPCCodeInfo;
            ppItem = dgrPPItem.SelectedItem as FmtPPItem;

            if (selectedCCodeInfo != null && ppItem != null)
            {
                this._fmtPPItemCollection.Remove(ppItem);
                selectedCCodeInfo.Items.Remove(ppItem);
            }
        }
        #endregion

        // Button Event
        #region btnSave_Click
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GemDriverError error;
            RecipeManager recipeManager;
            string path;

            if (IsValid(out string errorText) == true)
            {
                foreach (string removedPPID in this._removedPPIDs)
                {
                    path = string.Format(@"{0}\{1}.rcp", this._messageProcessor.CurrentSetting.FormattedRecipeDirectory, removedPPID);
                    if (System.IO.File.Exists(path) == true)
                    {
                        System.IO.File.Delete(path);
                    }
                }

                this._messageProcessor.FormattedProcessProgramCollection.Clear();

                foreach (var info in this._fmtPPCollection)
                {
                    if (string.IsNullOrEmpty(errorText) == true)
                    {
                        this._messageProcessor.FormattedProcessProgramCollection.Add(info);

                        recipeManager = new RecipeManager(info.PPID)
                        {
                            RecipeDirectory = this._messageProcessor.CurrentSetting.FormattedRecipeDirectory
                        };
                        recipeManager.Save(info.IsLoaded, info.FmtPPCollection.Items, out errorText);
                    }
                }

                if (string.IsNullOrEmpty(errorText) == false)
                {
                    MessageBox.Show(errorText);
                }
                else
                {
                    if (string.IsNullOrEmpty(this._messageProcessor.ConfigFilepath) == false)
                    {
                        error = this._messageProcessor.SaveConfigFile(out errorText);

                        if (error != GemDriverError.Ok)
                        {
                            this._messageProcessor.IsDirty = true;
                            MessageBox.Show(errorText);
                        }
                        else
                        {
                            this._messageProcessor.IsDirty = false;
                        }
                    }
                    else
                    {
                        this._messageProcessor.IsDirty = true;
                    }
                }
            }
            else
            {
                MessageBox.Show(errorText);
            }
        }
        #endregion
        #region btnClose_Click
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        #region btnTriggerEdit_Click
        private void btnTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            Settings.TriggerWindow triggerWindow;
            FormattedProcessProgramInfo selectedInfo;

            selectedInfo = dgrFmtPP.SelectedItem as FormattedProcessProgramInfo;

            if (selectedInfo != null)
            {
                triggerWindow = new Settings.TriggerWindow();
                triggerWindow.Initialize(this._messageProcessor, "Fmt Process Program", selectedInfo.PPID, selectedInfo.TriggerCollection);
                triggerWindow.Owner = this;
                triggerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                triggerWindow.ShowDialog();
            }
        }
        #endregion
        // TextChanged Event

        // Public Method
        #region Initialize
        public void Initialize(MessageProcessor messageProcessor)
        {
            this._messageProcessor = messageProcessor;
        }
        #endregion

        // Private Method
        #region LoadINI
        private void LoadINI(FormattedProcessProgramInfo info)
        {
            RecipeManager recipeManager;
            List<FmtPPCCodeInfo> list;

            if (info != null)
            {
                if (string.IsNullOrEmpty(info.PPID) == false)
                {
                    recipeManager = new RecipeManager(info.PPID)
                    {
                        RecipeDirectory = this._messageProcessor.CurrentSetting.FormattedRecipeDirectory
                    };
                    list = recipeManager.Load(out _);
                    info.IsLoaded = true;

                    if (list != null)
                    {
                        info.FmtPPCollection.Items = list;
                    }
                    
                }
            }
        }
        #endregion
        #region IsValid
        private bool IsValid(out string errorText)
        {
            bool result;
            List<string> usedPPIDs;
            List<string> usedCCodes;
            List<string> usedNames;

            result = true;
            errorText = string.Empty;
            object converted;

            SECSItemFormat ppidFormat;
            SECSItemFormat mdlnFormat;
            SECSItemFormat softRevFormat;
            SECSItemFormat ccodeFormat;
            SECSItemFormat ppNameFormat;

            ppidFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.PPID, SECSItemFormat.A);
            mdlnFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.MDLN, SECSItemFormat.A);
            softRevFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.SOFTREV, SECSItemFormat.A);
            ccodeFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.CCODE, SECSItemFormat.A);
            ppNameFormat = this._messageProcessor.GetSECSFormat(DataDictinaryList.PARAMNAME, SECSItemFormat.A);

            usedPPIDs = new List<string>();

            foreach (var info in this._fmtPPCollection)
            {
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (string.IsNullOrEmpty(info.PPID) == true)
                    {
                        errorText = string.Format(" empty PPID exists ");
                        dgrFmtPP.SelectedItem = info;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    converted = this._messageProcessor.ConvertValue(ppidFormat, info.PPID);

                    if (converted == null)
                    {
                        errorText = string.Format(" PPID convert fail \r\n\r\n PPID: {0} \r\n\r\n Format: {1}", info.PPID, ppidFormat.ToString());
                        dgrFmtPP.SelectedItem = info;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    converted = this._messageProcessor.ConvertValue(mdlnFormat, info.MDLN);

                    if (converted == null)
                    {
                        errorText = string.Format(" MDLN convert fail \r\n\r\n PPID: {0} \r\n\r\n Format: {1}", info.PPID, mdlnFormat.ToString());
                        dgrFmtPP.SelectedItem = info;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    converted = this._messageProcessor.ConvertValue(softRevFormat, info.SOFTREV);

                    if (converted == null)
                    {
                        errorText = string.Format(" SOFTREV convert fail \r\n\r\n PPID: {0} \r\n\r\n Format: {1}", info.PPID, softRevFormat.ToString());
                        dgrFmtPP.SelectedItem = info;
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    if (usedPPIDs.Contains(info.PPID) == true)
                    {
                        errorText = string.Format(" dupelicated PPID \r\n\r\n PPID: {0} ", info.PPID);
                        dgrFmtPP.SelectedItem = info;

                    }
                    else
                    {
                        usedPPIDs.Add(info.PPID);
                    }
                }

                if (string.IsNullOrEmpty(errorText) == true)
                {
                    usedCCodes = new List<string>();

                    foreach (var ccodeInfo in info.FmtPPCollection.Items)
                    {
                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            if (string.IsNullOrEmpty(ccodeInfo.CommandCode) == true)
                            {
                                errorText = string.Format(" empty CommandCode exists ");
                                dgrFmtPP.SelectedItem = info;
                                dgrCCode.SelectedItem = ccodeInfo;
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            converted = this._messageProcessor.ConvertValue(ccodeFormat, ccodeInfo.CommandCode);

                            if (converted == null)
                            {
                                errorText = string.Format(" CommandCode convert fail \r\n\r\n CommandCode: {0} \r\n\r\n Format: {1}", ccodeInfo.CommandCode, ccodeFormat.ToString());
                                dgrFmtPP.SelectedItem = info;
                                dgrCCode.SelectedItem = ccodeInfo;
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            if (usedCCodes.Contains(ccodeInfo.CommandCode) == true)
                            {
                                errorText = string.Format(" dupelicated CommandCode \r\n\r\n CommandCode: {0} ", ccodeInfo.CommandCode);
                                dgrFmtPP.SelectedItem = info;
                                dgrCCode.SelectedItem = ccodeInfo;
                            }
                            else
                            {
                                usedCCodes.Add(ccodeInfo.CommandCode);
                            }
                        }

                        if (string.IsNullOrEmpty(errorText) == true)
                        {
                            usedNames = new List<string>();

                            foreach (var ppItem in ccodeInfo.Items)
                            {
                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    if (string.IsNullOrEmpty(ppItem.PPName) == true)
                                    {
                                        errorText = string.Format(" empty PPName exists ");
                                        dgrFmtPP.SelectedItem = info;
                                        dgrCCode.SelectedItem = ccodeInfo;
                                        dgrPPItem.SelectedItem = ppItem;
                                    }
                                }

                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    if (ppItem.PPName.IndexOf('=') >= 0)
                                    {
                                        errorText = string.Format(" invalid PPName: {0} ", ppItem.PPName);
                                        dgrFmtPP.SelectedItem = info;
                                        dgrCCode.SelectedItem = ccodeInfo;
                                        dgrPPItem.SelectedItem = ppItem;
                                    }
                                }

                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    converted = this._messageProcessor.ConvertValue(ppNameFormat, ppItem.PPName);

                                    if (converted == null)
                                    {
                                        errorText = string.Format(" PPName convert fail \r\n\r\n PPName: {0} \r\n\r\n Format: {1}", ppItem.PPName, ppNameFormat.ToString());
                                        dgrFmtPP.SelectedItem = info;
                                        dgrCCode.SelectedItem = ccodeInfo;
                                        dgrPPItem.SelectedItem = ppItem;
                                    }
                                }

                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    if (ppItem.Format != SECSItemFormat.A && ppItem.PPValue.IndexOf(" ") >= 0)
                                    {
                                        foreach (string split in ppItem.PPValue.Split(' '))
                                        {
                                            if (string.IsNullOrEmpty(errorText) == true)
                                            {
                                                converted = this._messageProcessor.ConvertValue(ppItem.Format, split);

                                                if (converted == null)
                                                {
                                                    errorText = string.Format(" PPValue convert fail \r\n\r\n PPValue: {0} \r\n\r\n Format: {1}", ppItem.PPValue, ppItem.Format.ToString());
                                                    dgrFmtPP.SelectedItem = info;
                                                    dgrCCode.SelectedItem = ccodeInfo;
                                                    dgrPPItem.SelectedItem = ppItem;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        converted = this._messageProcessor.ConvertValue(ppItem.Format, ppItem.PPValue);

                                        if (converted == null)
                                        {
                                            errorText = string.Format(" PPValue convert fail \r\n\r\n PPValue: {0} \r\n\r\n Format: {1}", ppItem.PPValue, ppItem.Format.ToString());
                                            dgrFmtPP.SelectedItem = info;
                                            dgrCCode.SelectedItem = ccodeInfo;
                                            dgrPPItem.SelectedItem = ppItem;
                                        }
                                    }
                                }

                                if (string.IsNullOrEmpty(errorText) == true)
                                {
                                    if (usedNames.Contains(ppItem.PPName) == true)
                                    {
                                        errorText = string.Format(" dupelicated PPName \r\n\r\n PPName: {0} ", ppItem.PPName);
                                        dgrFmtPP.SelectedItem = info;
                                        dgrCCode.SelectedItem = ccodeInfo;
                                        dgrPPItem.SelectedItem = ppItem;
                                    }
                                    else
                                    {
                                        usedNames.Add(ppItem.PPName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                result = false;
            }

            return result;
        }
        #endregion
    }
}