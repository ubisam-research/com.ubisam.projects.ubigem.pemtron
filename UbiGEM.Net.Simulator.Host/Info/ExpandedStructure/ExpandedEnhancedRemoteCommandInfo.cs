using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedEnhancedRemoteCommandInfo : Structure.EnhancedRemoteCommandInfo
    {
        #region Property
        public bool IsInheritance { get; set; }
        public new string DataID { get; set; }

        public bool AutoSend { get; set; }
        public AutoSendTriggerCollection TriggerCollection { get; set; }
        public ExpandedEnhancedRemoteCommandValueSetInfo SelectedValueSet { get; set; }
        public ExpandedEnhancedRemoteCommandValueSetCollection ValueSetCollection
        {
            get; set;
        }
        #endregion
        #region Constructor
        public ExpandedEnhancedRemoteCommandInfo()
        {
            this.IsInheritance = false;
            this.ObjSpec = string.Empty;
            this.TriggerCollection = new AutoSendTriggerCollection();

            this.ValueSetCollection = new ExpandedEnhancedRemoteCommandValueSetCollection();
        }
        #endregion
        #region Clone
        public ExpandedEnhancedRemoteCommandInfo Clone()
        {
            ExpandedEnhancedRemoteCommandInfo result;

            result = new ExpandedEnhancedRemoteCommandInfo()
            {
                RemoteCommand = this.RemoteCommand,
                Description = this.Description,
                AutoSend = this.AutoSend,
                IsInheritance = this.IsInheritance,
                DataID = this.DataID,
                ObjSpec = this.ObjSpec,
            };

            if (this.TriggerCollection != null)
            {
                result.TriggerCollection = this.TriggerCollection.Clone();
            }

            if (this.ValueSetCollection != null)
            {
                result.ValueSetCollection = this.ValueSetCollection.Clone();
            }

            return result;
        }
        #endregion
    }
}