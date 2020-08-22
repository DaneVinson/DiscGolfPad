using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class Scorecard : ScorecardInfo
    {
        public Scorecard()
        { }

        public Scorecard(Scorecard scorecard) : base(scorecard)
        {
            Notes = scorecard.Notes;
        }


        public string Notes { get; set; }
    }
}
