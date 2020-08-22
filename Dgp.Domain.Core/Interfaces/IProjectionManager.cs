using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public interface IProjectionManager<TEntity> where TEntity : IEntity
    {
        Task CreateProjectionAsync(ICreatedEvent<TEntity> @event);
        Task DeleteProjectionAsync(IDeletedEvent<TEntity> @event);
        Task UpdateProjectionAsync(IUpdatedEvent<TEntity> @event);
    }
}
