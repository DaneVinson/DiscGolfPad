using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public interface ICreatedEvent<TEntity> : IEvent
    {
        Type EntityType { get; }
    }
}
