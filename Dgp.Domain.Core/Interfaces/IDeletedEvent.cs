using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public interface IDeletedEvent<TEntity> : IEvent
    {
        Type EntityType { get; }
    }
}
