using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ScorecardCreated : Scorecard, ICreatedEvent<Scorecard>
    {
        public ScorecardCreated() : base()
        { }

        public ScorecardCreated(Scorecard scorecard) : base(scorecard)
        { }

        public Type EntityType
        {
            get { return typeof(Scorecard); }
            set { }
        }
    }
}
