using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ScorecardInfoCreated : ScorecardInfo, ICreatedEvent<ScorecardInfo>
    {
        public ScorecardInfoCreated() : base()
        { }

        public ScorecardInfoCreated(ScorecardInfo scorecardInfo) : base(scorecardInfo)
        { }

        public Type EntityType
        {
            get { return typeof(ScorecardInfo); }
            set { }
        }
    }
}
