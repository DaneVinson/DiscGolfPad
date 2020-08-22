using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public interface IAggregate<TEntity> where TEntity : IEntity
    {
        void ApplyEvents(IEnumerable<IEvent> events);
        TEntity GetState();
        IEnumerable<IEvent> HandleCommand(ICommand<TEntity> command);
    }
}
