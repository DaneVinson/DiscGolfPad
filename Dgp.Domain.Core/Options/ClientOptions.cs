using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ClientOptions
    {
        public ClientOptions()
        { }

        public ClientOptions(string apiUri, string playerId)
        {
            ApiUri = apiUri;
            PlayerId = playerId;
        }

        public string ApiUri { get; set; }
        public string PlayerId { get; set; }
    }
}
