using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class Hole
    {
        public Hole()
        { }

        public Hole(int par, int distance)
        {
            Par = par;
            Distance = distance;
        }

        public int Par { get; set; }
        public int Distance { get; set; }
    }
}
