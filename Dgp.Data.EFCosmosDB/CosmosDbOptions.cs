using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.EFCosmosDB
{
    public class CosmosDbOptions
    {
        public string AuthKey { get; set; }
        public string DatabaseName { get; set; }
        public string Endpoint { get; set; }
    }
}
