using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.ValidRule
{
    public class ValidationError
    {
        public const string NotEmpty = "Must not be empty";
        public const string NotNumber = "Not number";
        public const string NotIpAddress = "Not Ip Address";
        public const string LesserThanMin = "Lesser than min";
        public const string GreaterThanMax = "Greater than max";
    }
}
