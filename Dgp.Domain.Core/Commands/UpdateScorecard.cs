using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class UpdateScorecard : Scorecard, ICommand<Scorecard>
    {
        public UpdateScorecard()
        { }

        public UpdateScorecard(Scorecard scorecard) : base(scorecard)
        { }
    }
}
