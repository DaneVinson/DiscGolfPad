using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public interface ICommand<TEntity>
    {
        Guid Id { get; }
        string PlayerId { get; }
    }
}
