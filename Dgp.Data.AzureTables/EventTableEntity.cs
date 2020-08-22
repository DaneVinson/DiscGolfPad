using Dgp.Domain.Core;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.AzureTables
{
    public class EventTableEntity : TableEntity
    {
        public EventTableEntity()
        { }

        public EventTableEntity(IEvent @event) : base(@event.Id.ToString(), Guid.NewGuid().ToString())
        {
            Data = JsonConvert.SerializeObject(@event);
            Type = @event.GetType().ToString();
        }


        public string Data { get; set; }
        public string Type { get; set; }
    }
}
