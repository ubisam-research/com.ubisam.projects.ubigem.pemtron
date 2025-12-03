using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedAlarmInfo : Structure.AlarmInfo
    {
        public bool IsInheritance { get; set; }

        #region Constructor
        public ExpandedAlarmInfo()
        {
            this.IsInheritance = false;
        }
        #endregion
        // Public Method
        #region Clone
        public ExpandedAlarmInfo Clone()
        {
            ExpandedAlarmInfo result;

            result = new ExpandedAlarmInfo();

            result.IsInheritance = this.IsInheritance;
            result.ID = this.ID;
            result.Code = this.Code;
            result.Description = this.Description;

            return result;
        }
        #endregion
    }
}