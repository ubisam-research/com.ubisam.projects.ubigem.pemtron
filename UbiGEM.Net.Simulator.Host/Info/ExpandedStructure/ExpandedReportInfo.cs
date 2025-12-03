using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class ExpandedReportInfo : Structure.ReportInfo
    {
        #region Property
        public bool IsInheritance { get; set; }
        public string DisplayStringForComboBox
        {
            get
            {
                string result;

                result = string.Format("{0}: {1}", this.ReportID, this.Description);

                return result;
            }
        }
        #endregion

        #region Constructor
        public ExpandedReportInfo()
        {
            this.IsInheritance = false;
        }
        #endregion
        // Public Method
        #region Clone
        public ExpandedReportInfo Clone()
        {
            ExpandedReportInfo result;

            result = new ExpandedReportInfo();
            result.ReportID = this.ReportID;
            result.IsInheritance = this.IsInheritance;
            result.Description = this.Description;
            if (result.Variables == null)
            {
                result.Variables = new Structure.VariableCollection();
            }

            if (this.Variables != null)
            {
                foreach(var child in this.Variables.Items)
                {
                    result.Variables.Add(child);
                }
            }
            return result;
        }
        #endregion
    }
}