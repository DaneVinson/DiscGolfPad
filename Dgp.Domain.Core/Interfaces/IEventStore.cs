using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> GetEventsAsync(Guid entityId);
        Task<bool> PersistEventsAsync(IEnumerable<IEvent> events);
    }
}
