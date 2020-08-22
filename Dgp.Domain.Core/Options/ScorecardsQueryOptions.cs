using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ScorecardsQueryOptions
    {
        public ScorecardsQueryOptions()
        { }

        public ScorecardsQueryOptions(
            string playerId, 
            Guid? courseId = null, 
            DateTime? from = null, 
            DateTime? to = null)
        {
            CourseId = courseId;
            From = from;
            PlayerId = playerId;
            To = to;
        }

        public Guid? CourseId { get; set; }
        public DateTime? From { get; set; }
        public string PlayerId { get; set; }
        public DateTime? To { get; set; }
    }
}
