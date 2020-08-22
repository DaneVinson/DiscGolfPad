using Dgp.Client.Blazor.Pages;
using Dgp.Domain.Core;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.Services
{
    public interface IApiClient<TEntity, TInfoEntity> where TEntity : class
    {
        // Command
        Task<TEntity?> CreateEntityAsync(TEntity entity);
        Task<bool> DeleteEntityAsync(Guid id);
        Task<TEntity?> UpdateEntityAsync(TEntity entity);

        // Query
        Task<IEnumerable<TInfoEntity>> GetEntitiesAsync(QueryBuilder? queryBuilder = null);
        Task<TEntity?> GetEntityAsync(Guid id);
    }
}
