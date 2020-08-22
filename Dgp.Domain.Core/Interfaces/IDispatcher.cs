using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public interface IDispatcher<TMessage>
    {
        Task DispatchAsync(TMessage message);
    }
}
