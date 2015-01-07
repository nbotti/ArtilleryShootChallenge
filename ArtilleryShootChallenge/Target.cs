using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtilleryShootChallenge
{
    class Target
    {
        public int TargetNumber { get; set; }
        public int Distance { get; set; }
        public int PointValue { get; set; }

        public override string ToString()
        {
            return "Target " + TargetNumber + " at distance " + Distance + "m worth " + PointValue + " points";
        }
    }
}
