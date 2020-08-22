using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ScorecardValidator : AbstractValidator<Scorecard>
    {
        public ScorecardValidator(IValidator<HoleScore> holeScoreValidator)
        {
            if (holeScoreValidator == null) { throw new ArgumentNullException(nameof(holeScoreValidator)); }

            RuleFor(scorecard => scorecard.CourseId)
                .NotEqual(Guid.Empty)
                .WithMessage("Course Id is required");

            RuleFor(scorecard => scorecard.PlayerId)
                .NotEmpty()
                .WithMessage("Player Id is requried");

            RuleFor(scorecard => scorecard.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Id is required");

            RuleFor(scorecard => scorecard.Scores)
                .NotEmpty()
                .WithMessage("A scorecard must have at least one score");

            RuleForEach(scorecard => scorecard.Scores)
                .SetValidator(holeScoreValidator);
        }
    }
}
