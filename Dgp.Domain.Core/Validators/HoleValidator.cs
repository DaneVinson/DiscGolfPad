using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class HoleValidator : AbstractValidator<Hole>
    {
        public HoleValidator()
        {
            RuleFor(hole => hole.Distance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Distance cannot be less than zero");

            RuleFor(hole => hole.Par)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Par must be at least 1");
        }
    }
}
