using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public class DispatchingMessenger<TMessage> : IMessenger<TMessage>
    {
        public DispatchingMessenger(IDispatcher<TMessage> dispatcher)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException();
        }


        public async Task SendAsync(TMessage message)
        {
            await Dispatcher.DispatchAsync(message);
        }

        public async Task SendAsync(IEnumerable<TMessage> messages)
        {
            await Task.WhenAll(messages.Select(m => Dispatcher.DispatchAsync(m)));
        }

        private readonly IDispatcher<TMessage> Dispatcher;
    }
}
