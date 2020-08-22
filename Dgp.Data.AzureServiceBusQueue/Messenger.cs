using Dgp.Domain.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.AzureServiceBusQueue
{
    public class Messenger<TMessage> : IMessenger<TMessage>
    {
        public Messenger(AzureServiceBusOptions options)
        {
            Options = options ?? throw new ArgumentNullException();
        }


        public Task SendAsync(TMessage message)
        {
            var queueClient = new QueueClient(Options.ConnectionString, "queuename", ReceiveMode.ReceiveAndDelete);
            var msg = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
            msg.PartitionKey = "";
            return queueClient.SendAsync(msg);
        }

        public Task SendAsync(IEnumerable<TMessage> messages)
        {
            throw new NotImplementedException();
        }


        private readonly AzureServiceBusOptions Options;
    }
}
