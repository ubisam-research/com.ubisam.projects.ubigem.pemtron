using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace UbiGEM.Net.Simulator.Host.ValidRule
{
    public class NumberOnlyValidationRule : ValidRule
    {
        // Public Method
        #region Validate
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result;
            string stringValue;
            int intVaule;

            stringValue = GetBoundValue(value) as string;

            if (string.IsNullOrWhiteSpace(stringValue) == true)
            {
                result = new ValidationResult(true, null);
            }
            else
            {
                if (int.TryParse(stringValue, out intVaule) == true)
                {
                    result = new ValidationResult(true, null);
                }
                else
                {
                    result = new ValidationResult(false, ValidationError.NotNumber);
                }
            }

            return result;
        }
        #endregion
    }
}