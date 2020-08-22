using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CreateScorecard : Scorecard, ICommand<Scorecard>
    {
        public CreateScorecard()
        { }

        public CreateScorecard(Scorecard scorecard) : base(scorecard)
        {
            Id = Guid.NewGuid();
        }
    }
}
