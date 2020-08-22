using Dgp.Domain.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.AzureTables
{
    /// <summary>
    /// Implementation of <see cref="TableEntity"/> used to store complex objects as serialized data.
    /// </summary>
    public class ModelTableEntity<TEntity> : TableEntity
    {
        public ModelTableEntity()
        { }

        public ModelTableEntity(string playerId, Guid id, string data = null) : base(playerId, id.ToString())
        {
            Data = data;
            ETag = "*";
        }


        public string Data { get; set; }

        public string EntityType => typeof(TEntity).ToString();
    }
}
