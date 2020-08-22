using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public interface IMessage
    {
        string Data { get; }
        string PlayerId { get; }
        string Type { get; }
    }
}
