using Dgp.Domain.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.EFCosmosDB
{
    public class EventDocument
    {
        public EventDocument()
        { }

        public EventDocument(IEvent @event)
        {
            Data = JsonConvert.SerializeObject(@event);
            EntityId = @event.Id;
            Id = Guid.NewGuid();
            StampUtc = DateTime.UtcNow;
            Type = @event.GetType().ToString();
        }


        public string Data { get; set; }
        public Guid EntityId { get; set; }
        public Guid Id { get; set; }
        public DateTime StampUtc { get; set; }
        public string Type { get; set; }
    }
}
