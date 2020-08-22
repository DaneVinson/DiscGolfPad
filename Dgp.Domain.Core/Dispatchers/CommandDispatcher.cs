using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public class CommandDispatcher<TEntity> : IDispatcher<ICommand<TEntity>> where TEntity : IEntity
    {
        public CommandDispatcher(
            IAggregate<TEntity> aggregate,
            IEventStore eventStore,
            IMessenger<IEvent> eventMessenger)
        {
            Aggregate = aggregate ?? throw new ArgumentNullException();
            EventMessenger = eventMessenger ?? throw new ArgumentNullException();
            EventStore = eventStore ?? throw new ArgumentNullException();
        }


        public async Task DispatchAsync(ICommand<TEntity> command)
        {
            // Retreive events
            IEnumerable<IEvent> events = await EventStore.GetEventsAsync(command.Id);

            // Apply events to hydrate the aggregate
            Aggregate.ApplyEvents(events);

            // Handle the command
            events = Aggregate.HandleCommand(command);
            if (events.Nada()) { return; }

            // Persist the events if there were no failed.
            if (!events.Any(e => e.GetType() == typeof(Failed)))
            {
                var persisted = await EventStore.PersistEventsAsync(events);
                if (!persisted) { events = new IEvent[] { new Failed("Failed to persist events.") }; }
            }
            else { events = events.Where(e => e.GetType() == typeof(Failed)); }

            // Forward resulting events
            await EventMessenger.SendAsync(events);
        }


        private readonly IAggregate<TEntity> Aggregate;
        private readonly IMessenger<IEvent> EventMessenger;
        private readonly IEventStore EventStore;
    }
}
