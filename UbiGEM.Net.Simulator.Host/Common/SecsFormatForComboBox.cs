using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UbiCom.Net.Structure;

namespace UbiGEM.Net.Simulator.Host.Common
{
    public class SECSItemFormatAll : List<SECSItemFormat>
    {
        public SECSItemFormatAll()
        {
            this.AddRange(Enum.GetValues(typeof(SECSItemFormat)).Cast<SECSItemFormat>());
        }
    }

    public class SECSItemFormatAllWithL : List<SECSItemFormat>
    {
        public SECSItemFormatAllWithL()
        {
            this.AddRange(Enum.GetValues(typeof(SECSItemFormat)).Cast<SECSItemFormat>());
            this.Remove(SECSItemFormat.None);
            this.Remove(SECSItemFormat.X);
        }
    }

    public class SECSItemFormatData : List<SECSItemFormat>
    {
        public SECSItemFormatData()
        {
            this.AddRange(Enum.GetValues(typeof(SECSItemFormat)).Cast<SECSItemFormat>());
            this.Remove(SECSItemFormat.None);
            this.Remove(SECSItemFormat.X);
            this.Remove(SECSItemFormat.L);
        }
    }
}