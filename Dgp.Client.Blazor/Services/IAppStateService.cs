using Dgp.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.Services
{
    public interface IAppStateService
    {
        List<CourseInfo> Courses { get; set; }

        bool IsBusy { get; set; }

        List<ScorecardInfo> Scorecards { get; set;  }

        Action StateHasChanged { get; set;  }
    }
}
