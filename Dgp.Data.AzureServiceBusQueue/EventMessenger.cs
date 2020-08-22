using Dgp.Domain.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.AzureServiceBusQueue
{
    public class EventMessenger<TEntity> : IMessenger<IEvent>
    {
        public EventMessenger(AzureServiceBusOptions options)
        {
            Options = options ?? throw new ArgumentNullException();
        }


        public Task SendAsync(IEvent @event)
        {
            return NewQueueClient().SendAsync(GetServiceBusMessage(@event));
        }

        public Task SendAsync(IEnumerable<IEvent> events)
        {
            return NewQueueClient().SendAsync(events.Select(c => GetServiceBusMessage(c)).ToArray());
        }


        private static Message GetServiceBusMessage(IEvent @event)
        {
            var entityMessage = new EntityMessage(
                                            @event.GetType().ToString(),
                                            JsonConvert.SerializeObject(@event));
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entityMessage)))
            {
                PartitionKey = @event.Id.ToString()
            };
            return message;
        }

        private QueueClient NewQueueClient()
        {
            return new QueueClient(Options.ConnectionString, QueueName, ReceiveMode.ReceiveAndDelete);
        }


        private const string QueueName = "events";
        private readonly AzureServiceBusOptions Options;
    }
}
