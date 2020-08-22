using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CourseValidator : AbstractValidator<Course>
    {
        public CourseValidator(IValidator<Hole> holeValidator)
        {
            if (holeValidator == null) { throw new ArgumentNullException(nameof(holeValidator)); }

            RuleFor(course => course.Name)
                .NotEmpty()
                .WithMessage("Name is required");

            RuleFor(course => course.PlayerId)
                .NotEmpty()
                .WithMessage("Player Id is requried");

            RuleFor(course => course.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Id is required");

            RuleFor(course => course.Holes)
                .NotEmpty()
                .WithMessage("At least one hole is required to define a course");

            RuleForEach(course => course.Holes)
                .SetValidator(holeValidator);
        }
    }
}
