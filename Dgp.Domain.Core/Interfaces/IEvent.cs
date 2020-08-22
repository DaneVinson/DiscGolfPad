using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public interface IEvent
    {
        Guid Id { get; }
        string PlayerId { get; }
    }
}
