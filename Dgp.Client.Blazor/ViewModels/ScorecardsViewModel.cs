using Dgp.Client.Blazor.Services;
using Dgp.Domain.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.ViewModels
{
    public class ScorecardsViewModel
    {
        public ScorecardsViewModel(
            IApiClient<Scorecard, ScorecardInfo> apiClient,
            IAppStateService stateService,
            NavigationManager navigation)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _appStateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }


        public void Cancel()
        {
            _navigation.NavigateTo("/courses");
        }

        public void EditScorecard(string id)
        {
            _navigation.NavigateTo($"/scorecard/{CourseId}/{id}");
        }

        public string GetScoreClass(HoleScore holeScore)
        {
            if (holeScore.Score < holeScore.Par) { return "badge badge-pill badge-success"; }
            else if (holeScore.Score > holeScore.Par) { return "badge badge-danger"; }
            else { return "badge"; }
        }

        public string GetTotal(ScorecardInfo scorecard)
        {
            var score = scorecard.Scores.Select(s => s.Score).Sum();
            var difference = score - scorecard.Scores.Select(s => s.Par).Sum();
            if (difference == 0) { return $"{score} (even)"; }
            else if (difference > 0) { return $"{score} (+{difference})"; }
            else { return $"{score} ({difference})"; }
        }

        public async Task InitializeScorecardsAsync(string courseId, bool forceReload = false)
        {
            if (!Guid.TryParse(courseId, out var courseGuid)) { return; }
            CourseId = courseId;

            try
            {
                _appStateService.IsBusy = true;
                Caption = $"{_appStateService.Courses.FirstOrDefault(c => c.Id == courseGuid)?.Name} Scorecards";
                if (!_appStateService.Scorecards.Any(s => s.CourseId == courseGuid) || forceReload)
                {
                    var scorecards = await _apiClient.GetEntitiesAsync(new QueryBuilder() { { "courseId", courseId } });
                    _appStateService.Scorecards
                                    .Where(s => s.CourseId == courseGuid)
                                    .ToArray()
                                    .Select(s => _appStateService.Scorecards.Remove(s));
                    _appStateService.Scorecards.AddRange(scorecards);
                }
                Scorecards = _appStateService.Scorecards.Where(s => s.CourseId == courseGuid).ToList();
            }
            finally { _appStateService.IsBusy = false; }
        }


        public string Caption { get; set; } = string.Empty;
        public string CourseId { get; private set; } = string.Empty;
        public bool IsBusy => _appStateService.IsBusy;
        public List<ScorecardInfo> Scorecards { get; set; } = new List<ScorecardInfo>();


        private readonly IApiClient<Scorecard, ScorecardInfo> _apiClient;
        private readonly IAppStateService _appStateService;
        private readonly NavigationManager _navigation;
    }
}
