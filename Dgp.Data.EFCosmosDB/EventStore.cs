using Dgp.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.EFCosmosDB
{
    public class EventStore : IEventStore
    {
        public EventStore(DgpDbContext context)
        {
            Context = context ?? throw new ArgumentNullException();
        }


        public async Task<IEnumerable<IEvent>> GetEventsAsync(Guid entityId)
        {
            return await Context.Events
                                .Where(e => e.EntityId == entityId)
                                .Select(e => JsonConvert.DeserializeObject(e.Data, Type.GetType(e.Type)) as IEvent)
                                .ToArrayAsync();
        }

        public async Task<bool> PersistEventsAsync(IEnumerable<IEvent> events)
        {
            Context.Events.AddRange(events.Select(e => new EventDocument(e)));
            var saveCount = await Context.SaveChangesAsync();
            return saveCount == events.Count();
        }


        private readonly DgpDbContext Context;
    }
}
