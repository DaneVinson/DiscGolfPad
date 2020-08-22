using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class Deleted<TEntity> : IDeletedEvent<TEntity>
    {
        public Deleted()
        { }

        public Deleted(string playerId, Guid id)
        {
            Id = id;
            PlayerId = playerId;
        }

        public Deleted(Delete<TEntity> command)
        {
            if (command != null)
            {
                Id = command.Id;
                PlayerId = command.PlayerId;
            }
        }


        public Type EntityType
        {
            get { return typeof(TEntity); }
            set { }
        }
        public Guid Id { get; set; }
        public string PlayerId { get; set; }
    }
}
