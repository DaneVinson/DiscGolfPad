using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class Failed : IEvent
    {
        public Failed()
        {
            Id = Guid.NewGuid();
        }

        public Failed(string message)
        {
            Id = Guid.NewGuid();
            Message = message;
        }


        public Guid Id { get; }
        public string Message { get; set; }
        public string PlayerId { get; }
    }
}
