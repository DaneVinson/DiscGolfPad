using Dgp.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.AzureTables
{
    public class ScorecardInfoTableEntity : ModelTableEntity<ScorecardInfo>
    {
        public ScorecardInfoTableEntity()
        { }

        public ScorecardInfoTableEntity(string playerId, Guid id, string data = null) : base(playerId, id, data)
        { }


        public Guid CourseId { get; set; }
        public DateTime Date { get; set; }
    }
}
