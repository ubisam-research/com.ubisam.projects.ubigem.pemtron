using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace UbiGEM.Net.Simulator.Host.ValidRule
{
    public class IpAddressValidationRule : ValidRule
    {
        // Public Method
        #region Validate
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result;
            IPAddress ip;
            string stringValue;

            result = null;
            stringValue = GetBoundValue(value) as string;

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                result = new ValidationResult(true, null);
            }
            else
            {
                if (IPAddress.TryParse(stringValue, out ip) == true)
                {
                    result = new ValidationResult(true, null);
                }
                else
                {
                    result = new ValidationResult(false, ValidationError.NotIpAddress);
                }

                return result;
            }

            return result;
        }
        #endregion
    }
}