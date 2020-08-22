using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class DeleteValidator<TEntity> : AbstractValidator<Delete<TEntity>> where TEntity : IEntity
    {
        public DeleteValidator()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("A valid Id is required for delete");

            RuleFor(c => c.PlayerId)
                .NotEmpty()
                .WithMessage("Player Id is required");
        }
    }
}
