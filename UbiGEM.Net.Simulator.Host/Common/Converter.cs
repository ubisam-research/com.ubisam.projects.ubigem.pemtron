using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UbiGEM.Net.Simulator.Host.Converter
{
    class VariableType2String : IValueConverter
    {
        #region Convert
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result;

            switch ((UbiGEM.Net.Structure.VariableType)value)
            {
                case Structure.VariableType.ECV:
                    result = "EC";
                    break;
                case Structure.VariableType.SV:
                    result = "SV";
                    break;
                case Structure.VariableType.DVVAL:
                    result = "DV";
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }
        #endregion
        #region ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}