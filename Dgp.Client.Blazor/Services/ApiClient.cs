using Dgp.Domain.Core;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.Services
{
    public class ApiClient<TEntity, TInfoEntity> : IApiClient<TEntity, TInfoEntity> where TEntity : class
    {
        public ApiClient(HttpClient httpClient, ClientOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }


        public async Task<TEntity?> CreateEntityAsync(TEntity entity)
        {
            return await _httpClient.PostAsync<TEntity>(EntityRoute, entity);
        }

        public async Task<bool> DeleteEntityAsync(Guid id)
        {
            var result = await _httpClient.DeleteAsync(EntityRoute);
            return result?.IsSuccessStatusCode ?? false;
        }

        public async Task<IEnumerable<TInfoEntity>> GetEntitiesAsync(QueryBuilder? queryBuilder = null)
        {
            var uriBuilder = new StringBuilder(EntityRoute);
            if (queryBuilder != null) { uriBuilder.Append(queryBuilder.ToString()); }

            var result = await _httpClient.GetAsync<IEnumerable<TInfoEntity>>(uriBuilder.ToString());

            return result ?? Array.Empty<TInfoEntity>();
        }

        public async Task<TEntity?> GetEntityAsync(Guid id)
        {
            return await _httpClient.GetAsync<TEntity>($"{EntityRoute}/{id}");
        }

        public async Task<TEntity?> UpdateEntityAsync(TEntity entity)
        {
            return await _httpClient.PutAsync<TEntity>(EntityRoute, entity);
        }


        private string EntityRoute => $"{_options.ApiUri}/{EntityRoutes[typeof(TEntity)]}";
        private readonly ClientOptions _options;
        private readonly HttpClient _httpClient;

        private static readonly Dictionary<Type, string> EntityRoutes = new Dictionary<Type, string>()
        {
            { typeof(Course), "courses" },
            { typeof(Scorecard), "scorecards" }
        };
    }
}
