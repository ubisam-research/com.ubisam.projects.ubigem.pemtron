using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedCollectionEventInfo : Structure.CollectionEventInfo
    {
        #region Property
        public bool IsInheritance { get; set; }
        public string DisplayStringForComboBox
        {
            get
            {
                string result;

                result = string.Format("{0}: {1}", this.CEID, this.Name);

                return result;
            }
        }
        #endregion
        #region Constructor
        public ExpandedCollectionEventInfo()
        {
            this.IsInheritance = false;
        }
        #endregion
        // Public Method
        #region Clone
        public ExpandedCollectionEventInfo Clone()
        {
            ExpandedCollectionEventInfo result;

            result = new ExpandedCollectionEventInfo
            {
                IsInheritance = this.IsInheritance,

                CEID = this.CEID,
                Name = this.Name,
                PreDefined = this.PreDefined,
                IsUse = this.IsUse,
                IsBase = this.IsBase,
                Enabled = this.Enabled,
                Description = this.Description
            };

            if (this.Reports != null)
            {
                foreach (ExpandedReportInfo reportInfo in this.Reports.Items.Values)
                {
                    result.Reports.Items[reportInfo.ReportID] = reportInfo;
                }
            }

            return result;
        }
        #endregion
    }
}
