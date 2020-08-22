using Dgp.Data.AzureTables;
using Dgp.Domain.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dgp.Service.Dispatcher.AzureFunctions
{
    public class EventsQueueTrigger
    {
        public EventsQueueTrigger(IDispatcher<IEvent> eventDispatcher)
        {
            EventDispatcher = eventDispatcher ?? throw new ArgumentNullException();
        }


        [FunctionName(FunctionName)]
        public async Task Run([ServiceBusTrigger(QueueName, Connection = ConnectionName)]string message, ILogger log)
        {
            // Deserialize the EntityMessage
            var entityMessage = JsonSerializer.Deserialize<EntityMessage>(message);

            // Get the Type of the event from the message
            var eventType = Assembly.GetAssembly(typeof(Course)).GetType(entityMessage.Type);

            // Deserialize the event from the message and dispatch
            var @event = JsonSerializer.Deserialize(entityMessage.Data, eventType);
            await EventDispatcher.DispatchAsync(@event as IEvent);

            log.LogInformation($"Function {FunctionName} received Service Bus Queue message: {message}");
        }


        private readonly IDispatcher<IEvent> EventDispatcher;


        private const string ConnectionName = "AzureServiceBus:ConnectionString";
        private const string FunctionName = "DispatchEvent";
        private const string QueueName = "events";
    }
}
