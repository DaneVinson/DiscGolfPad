using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class HoleScore
    {
        public HoleScore()
        { }

        public HoleScore(int par, int score)
        {
            Par = par;
            Score = score;
        }

        public int Par { get; set; }
        public int Score { get; set; }
    }
}
