using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class HoleScoreValidator : AbstractValidator<HoleScore>
    {
        public HoleScoreValidator()
        {
            RuleFor(hole => hole.Par)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Par must be at least 1");

            RuleFor(hole => hole.Score)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Score must be at least 1 (eagle)");
        }
    }
}
