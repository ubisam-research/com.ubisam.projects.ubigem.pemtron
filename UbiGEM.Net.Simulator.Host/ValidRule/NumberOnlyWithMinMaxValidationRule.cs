using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace UbiGEM.Net.Simulator.Host.ValidRule
{
    public class NumberOnlyWithMinMaxValidationRule : ValidRule
    {
        #region Properties
        public int Min { get; set; }

        public int Max { get; set; }
        #endregion

        #region Constructor
        public NumberOnlyWithMinMaxValidationRule()
            : base()
        {
            Min = 0;
            Max = 100;
        }

        public NumberOnlyWithMinMaxValidationRule(int min, int max)
            : base()
        {
            Min = min;
            Max = max;
        }
        #endregion

        // Public Method
        #region Validate
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result;
            string stringValue;
            int intValue;

            stringValue = GetBoundValue(value) as string;

            if (string.IsNullOrWhiteSpace(stringValue) == true)
            {
                result = new ValidationResult(true, null);
            }
            else
            {
                if (int.TryParse(stringValue, out intValue) == true)
                {
                    if (intValue < Min)
                    {
                        result = new ValidationResult(false, ValidationError.LesserThanMin);
                    }
                    else if (intValue > Max)
                    {
                        result = new ValidationResult(false, ValidationError.GreaterThanMax);
                    }
                    else
                    {
                        result = new ValidationResult(true, null);
                    }
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