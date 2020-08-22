using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ScorecardInfoUpdated : ScorecardInfo, IUpdatedEvent<ScorecardInfo>
    {
        public ScorecardInfoUpdated() : base()
        { }

        public ScorecardInfoUpdated(ScorecardInfo scorecardInfo) : base(scorecardInfo)
        { }

        public Type EntityType
        {
            get { return typeof(ScorecardInfo); }
            set { }
        }
    }
}
