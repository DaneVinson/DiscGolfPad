using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public interface IMessenger<TMessage>
    {
        Task SendAsync(TMessage message);
        Task SendAsync(IEnumerable<TMessage> messages);
    }
}
