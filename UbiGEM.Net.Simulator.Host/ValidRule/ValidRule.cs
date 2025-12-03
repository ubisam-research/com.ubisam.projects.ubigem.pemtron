using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace UbiGEM.Net.Simulator.Host.ValidRule
{
    public abstract class ValidRule : ValidationRule
    {
        #region Constructor
        public ValidRule()
            : base()
        {
        }
        #endregion

        // Protected Method
        #region GetBoundValue
        protected object GetBoundValue(object value)
        {
            if (value is BindingExpression)
            {
                BindingExpression binding = (BindingExpression)value;

                object dataItem = binding.DataItem;
                string propertyName = binding.ParentBinding.Path.Path;

                object propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);

                return propertyValue;
            }
            else
            {
                return value;
            }
        }
        #endregion
    }
}
