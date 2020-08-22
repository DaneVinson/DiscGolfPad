using Dgp.Domain.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.EFCosmosDB
{
    public class ScorecardDocument
    {
        public ScorecardDocument()
        { }

        public ScorecardDocument(Scorecard scorecard)
        {
            CourseId = scorecard.CourseId;
            Date = scorecard.Date;
            Id = scorecard.Id;
            PlayerId = scorecard.PlayerId;
            ScorecardData = JsonConvert.SerializeObject(scorecard);
        }

        public Guid CourseId { get; set; }
        public DateTimeOffset Date { get; set; }
        public Guid Id { get; set; }
        public string PlayerId { get; set; }
        public string ScorecardData { get; set; }
    }
}
