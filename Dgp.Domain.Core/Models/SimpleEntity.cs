using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class SimpleEntity : IEntity
    {
        public SimpleEntity()
        { }

        public SimpleEntity(Guid id, string playerId)
        {
            Id = id;
            PlayerId = playerId;
        }


        public Guid Id { get; set; }

        public string PlayerId { get; set; }
    }
}
