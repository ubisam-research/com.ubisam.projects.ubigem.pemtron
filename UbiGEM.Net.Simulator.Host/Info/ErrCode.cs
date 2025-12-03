using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiGEM.Net.Simulator.Host.Info
{
    public enum ErrCode
    {
        NoError = 0,
        UnknownObjectInSpecifier = 1,
        UnknownTargetObjectType = 2,
        UnknownObjectInstance = 3,
        UnknownAttributeName = 4,
        ReadOnlyAttribute = 5,
        UnknownObjectType = 6,
        InvalidAttributeValue = 7,
        SyntaxError = 8,
        VerificationError = 9,
        ValidationError = 10,
    }
}
