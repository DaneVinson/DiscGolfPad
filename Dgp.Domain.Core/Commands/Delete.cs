using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class Delete<TEntity> : ICommand<TEntity>
    {
        public Delete()
        { }

        public Delete(string playerId, Guid id)
        {
            Id = id;
            PlayerId = playerId;
        }


        public Guid Id { get; set; }
        public string PlayerId { get; set; }
    }
}
