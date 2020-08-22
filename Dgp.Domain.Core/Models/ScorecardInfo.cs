using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ScorecardInfo : IEntity
    {
        public ScorecardInfo()
        { }

        public ScorecardInfo(ScorecardInfo scorecardInfo)
        {
            CopyFrom(scorecardInfo);
        }


        public Guid CourseId { get; set; }
        public DateTimeOffset Date { get; set; }
        public Guid Id { get; set; }
        public string PlayerId { get; set; }
        public HoleScore[] Scores { get; set; }

        public void CopyFrom(ScorecardInfo scorecardInfo)
        {
            if (scorecardInfo.Scores != null)
            {
                Scores = scorecardInfo.Scores
                                        .Select(s => new HoleScore(s.Par, s.Score))
                                        .ToArray();
            }
            CourseId = scorecardInfo.CourseId;
            Date = scorecardInfo.Date;
            Id = scorecardInfo.Id;
            PlayerId = scorecardInfo.PlayerId;
        }

        public string ToStringNetScore()
        {
            if (Scores.Nada()) { return string.Empty; }

            var score = Scores.Select(s => s.Score).Sum();
            var difference = score - Scores.Select(s => s.Par).Sum();
            if (difference == 0) { return $"{score} (even)"; }
            else if (difference > 0) { return $"{score} (+{difference})"; }
            else { return $"{score} ({difference})"; }
        }

        public override string ToString()
        {
            return $"{Date}: course {CourseId}, {Scores?.Length ?? 0} holes, score {ToStringNetScore()}";
        }
    }
}
