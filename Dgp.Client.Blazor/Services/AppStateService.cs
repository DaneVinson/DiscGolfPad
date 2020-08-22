using Dgp.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.Services
{
    public class AppStateService : IAppStateService
    {
        public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();

        public bool IsBusy { get; set; }

        public List<ScorecardInfo> Scorecards { get; set; } = new List<ScorecardInfo>();

        public Action StateHasChanged { get; set; } = () => { };
    }
}
