using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ScorecardUpdated : Scorecard, IUpdatedEvent<Scorecard>
    {
        public ScorecardUpdated() : base()
        { }

        public ScorecardUpdated(Scorecard scorecard) : base(scorecard)
        { }

        public Type EntityType
        {
            get { return typeof(Scorecard); }
            set { }
        }
    }
}
