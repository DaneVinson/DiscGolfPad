using Dgp.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Data.Seed
{
    public class BilbosSeedService
    {
        public static SeedPack NewSeedPack()
        {
            var courses = new Course[]
            {
                new Course()
                {
                    Holes = new Hole[]
                    {
                        new Hole(3, 210),
                        new Hole(3, 180),
                        new Hole(3, 245),
                        new Hole(3, 205),
                        new Hole(3, 310),
                        new Hole(3, 220),
                        new Hole(3, 200),
                        new Hole(3, 245),
                        new Hole(3, 270)
                    },
                    Id = OrchardParkId,
                    ImageUri = "https://images.com/orchardpark.jpg",
                    Location = "Portland, OR USA",
                    Name = "Orchard Park",
                    PlayerId = Bilbo.Id
                },
                new Course()
                {
                    Holes = new Hole[]
                    {
                        new Hole(3, 310),
                        new Hole(3, 240),
                        new Hole(3, 280),
                        new Hole(3, 205),
                        new Hole(3, 410),
                        new Hole(3, 345),
                        new Hole(3, 370),
                        new Hole(3, 400),
                        new Hole(3, 380),
                        new Hole(3, 416),
                        new Hole(3, 280),
                        new Hole(3, 300),
                        new Hole(3, 415),
                        new Hole(3, 420),
                        new Hole(3, 405),
                        new Hole(3, 320),
                        new Hole(3, 400),
                        new Hole(3, 450)
                    },
                    Id = PierParkId,
                    ImageUri = "https://images.com/pierpark.jpg",
                    Location = "Portland, OR USA",
                    Name = "Pier Park",
                    PlayerId = Bilbo.Id
                }
            };
            var scorecards = new Scorecard[]
            {
                new Scorecard()
                {
                    CourseId = OrchardParkId,
                    Date = new DateTimeOffset(DateTime.Now.AddDays(-5).Date),
                    Id = ScorecardIds[0],
                    Notes = "Not too bad.",
                    PlayerId = Bilbo.Id,
                    Scores = new HoleScore[]
                    {
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 2),
                        new HoleScore(3, 3),
                        new HoleScore(3, 2),
                        new HoleScore(3, 2),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3)
                    }
                },
                new Scorecard()
                {
                    CourseId = OrchardParkId,
                    Date = new DateTimeOffset(DateTime.Now.AddDays(-3).Date),
                    Id = ScorecardIds[1],
                    Notes = "Nice round!",
                    PlayerId = Bilbo.Id,
                    Scores = new HoleScore[]
                    {
                        new HoleScore(3, 2),
                        new HoleScore(3, 3),
                        new HoleScore(3, 2),
                        new HoleScore(3, 2),
                        new HoleScore(3, 4),
                        new HoleScore(3, 2),
                        new HoleScore(3, 2),
                        new HoleScore(3, 2),
                        new HoleScore(3, 1)
                    }
                },
                new Scorecard()
                {
                    CourseId = OrchardParkId,
                    Date = new DateTimeOffset(DateTime.Now.AddDays(-10).Date),
                    Id = ScorecardIds[2],
                    Notes = "Meh.",
                    PlayerId = Bilbo.Id,
                    Scores = new HoleScore[]
                    {
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 2),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3)
                    }
                },
                new Scorecard()
                {
                    CourseId = PierParkId,
                    Date = new DateTimeOffset(DateTime.Now.AddDays(-13).Date),
                    Id = ScorecardIds[3],
                    Notes = "I suck at disc golf!",
                    PlayerId = Bilbo.Id,
                    Scores = new HoleScore[]
                    {
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 4),
                        new HoleScore(3, 4),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 4),
                        new HoleScore(3, 4),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 4),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 3),
                        new HoleScore(3, 4),
                        new HoleScore(3, 4),
                        new HoleScore(3, 4)
                    }
                }
            };

            return new SeedPack()
            {
                Courses = courses,
                Scorecards = scorecards
            };
        }

        public static readonly Guid OrchardParkId = Guid.Parse("CC4EF389-1929-4D6E-B249-EB7DC5B71510");
        public static readonly Guid PierParkId = Guid.Parse("E48DFC2C-C8FD-47AE-821E-370F7DF51AA2");
        public static readonly Guid[] ScorecardIds = new Guid[]
        {
            Guid.Parse("37EFC0D1-8CDD-46D4-A8A0-B0A703295473"),
            Guid.Parse("10E2C7F3-DFE5-46F8-BB4F-A81457496488"),
            Guid.Parse("6A0C2D03-3BFC-47FE-852D-2EC331D51E00"),
            Guid.Parse("94513835-5D97-4ED2-9CFD-5052C42CD6E7")
        };
    }
}
