using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiCom.Net.Automata.SECS2
{
    internal enum AutomataState
    {
        Start,
        InListFormatString,
        InListCountString,
        InListNameString,
        InAfterListNameString,
        InFormatString,
        InCountString,
        InValueString,
        InBeforeNameString,
        InNameString,
        InAfterNameString,
        InItemEnd,
        Error,
    }
}
