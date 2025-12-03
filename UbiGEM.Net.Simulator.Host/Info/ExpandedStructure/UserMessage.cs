using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiCom.Net.Structure;

namespace UbiGEM.Net.Simulator.Host.Info.ExpandedStructure
{
    public class UserMessage
    {
        public string Name { get; set; }

        public SECSMessageDirection Direction { get; set; }

        public int Stream { get; set; }

        public int Function { get; set; }

        public bool WaitBit { get; set; }

        public string Data { get; set; }

        public string DirectionString
        {
            get
            {
                string result;
                result = "H↔E";

                switch (this.Direction)
                {
                    case SECSMessageDirection.ToEquipment:
                        result = "E←H";
                        break;
                    case SECSMessageDirection.ToHost:
                        result = "E→H";
                        break;
                }

                return result;
            }
        }

        public UserMessage()
        {
            this.Name = string.Empty;
            this.Direction = SECSMessageDirection.Both;
            this.Stream = 0;
            this.Function = 0;
            this.WaitBit = false;
            this.Data = string.Empty;
        }
    }
}