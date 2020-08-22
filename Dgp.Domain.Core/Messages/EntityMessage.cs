using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class EntityMessage
    {
        public EntityMessage()
        { }

        public EntityMessage(string type, string data)
        {
            Data = data;
            Type = type;
        }

        public string Data { get; set; }

        public string Type { get; set; }
    }
}
