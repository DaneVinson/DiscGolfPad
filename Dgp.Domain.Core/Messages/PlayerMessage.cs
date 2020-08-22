using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class PlayerMessage : IMessage
    {
        public PlayerMessage()
        { }

        public PlayerMessage(string playerId, string type, string data)
        {
            Data = data;
            PlayerId = playerId;
            Type = type;
        }


        public string Data { get; set; }

        public string PlayerId { get; set; }

        public string Type { get; set; }
    }
}
