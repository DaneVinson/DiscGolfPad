using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public interface IUpdatedEvent<TEntity> : IEvent
    {
        Type EntityType { get; }
    }
}
