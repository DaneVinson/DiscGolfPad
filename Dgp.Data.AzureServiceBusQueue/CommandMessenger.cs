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
    public class CommandMessenger<TEntity> : IMessenger<ICommand<TEntity>>
    {
        public CommandMessenger(AzureServiceBusOptions options)
        {
            Options = options ?? throw new ArgumentNullException();
        }


        public Task SendAsync(ICommand<TEntity> command)
        {
            return NewQueueClient().SendAsync(GetServiceBusMessage(command));
        }

        public Task SendAsync(IEnumerable<ICommand<TEntity>> commands)
        {
            return NewQueueClient().SendAsync(commands.Select(c => GetServiceBusMessage(c)).ToArray());
        }


        private static Message GetServiceBusMessage(ICommand<TEntity> command)
        {
            var playerMessage = new PlayerMessage(
                                        command.PlayerId,
                                        command.GetType().ToString(),
                                        JsonConvert.SerializeObject(command));
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(playerMessage)))
            {
                PartitionKey = playerMessage.PlayerId
            };
            return message;
        }

        private QueueClient NewQueueClient()
        {
            return new QueueClient(Options.ConnectionString, QueueName, ReceiveMode.ReceiveAndDelete);
        }


        private const string QueueName = "commands";
        private readonly AzureServiceBusOptions Options;
    }
}
