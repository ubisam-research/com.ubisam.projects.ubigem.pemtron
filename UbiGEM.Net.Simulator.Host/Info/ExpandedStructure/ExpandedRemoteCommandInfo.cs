using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UbiGEM.Net.Simulator.Host.Common;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedRemoteCommandInfo : Structure.RemoteCommandInfo
    {
        #region Property
        public bool IsInheritance { get; set; }

        public bool AutoSend { get; set; }
        public AutoSendTriggerCollection TriggerCollection { get; set; }
        public ExpandedRemoteCommandValueSetInfo SelectedValueSet { get; set; }

        public ExpandedRemoteCommandValueSetCollection ValueSetCollection
        {
            get; set;
        }
        #endregion

        #region Constructor
        public ExpandedRemoteCommandInfo()
        {
            this.IsInheritance = false;
            this.AutoSend = false;
            this.TriggerCollection = new AutoSendTriggerCollection();
            this.ValueSetCollection = new ExpandedRemoteCommandValueSetCollection();
        }
        #endregion
        #region Clone
        public ExpandedRemoteCommandInfo Clone()
        {
            ExpandedRemoteCommandInfo result;

            result = new ExpandedRemoteCommandInfo()
            {
                RemoteCommand = this.RemoteCommand,
                Description = this.Description,
                AutoSend = this.AutoSend,
                IsInheritance = this.IsInheritance,
            };

            if(this.TriggerCollection != null)
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
        #region CopyTo
        public new ExpandedRemoteCommandInfo CopyTo()
        {
            return Clone();
        }
        #endregion
    }
}