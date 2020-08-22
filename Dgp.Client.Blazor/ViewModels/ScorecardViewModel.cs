using Dgp.Client.Blazor.Services;
using Dgp.Domain.Core;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.ViewModels
{
    public class ScorecardViewModel : EditViewModel<Scorecard>
    {
        public ScorecardViewModel(
            IApiClient<Scorecard, ScorecardInfo> apiClient,
            IApiClient<Course, CourseInfo> courseApiClient,
            IAppStateService stateService,
            NavigationManager navigation)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _appStateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _courseApiClient = courseApiClient ?? throw new ArgumentNullException(nameof(courseApiClient));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }

        public void Cancel()
        {
            if (IsNew) { _navigation.NavigateTo("/courses"); }
            else { _navigation.NavigateTo($"/scorecards/{Entity.CourseId}"); }
        }

        public async Task InitializeAsync(string courseId, string id)
        {
            try
            {
                _appStateService.IsBusy = true;

                Guid.TryParse(id, out var guid);
                IsNew = guid == Guid.Empty;
                if (IsNew)
                {
                    Guid.TryParse(courseId, out var courseGuid);
                    var course = await _courseApiClient.GetEntityAsync(courseGuid) ?? new Course();
                    if (course == null)
                    {
                        _navigation.NavigateTo("/courses");
                        return;
                    }

                    Entity.Date = DateTimeOffset.Now;
                    Entity.Id = Guid.NewGuid();
                    Entity.PlayerId = Bilbo.Id;
                    Entity.Scores = new HoleScore[course.Holes.Length];
                    for (int i = 0; i < course.Holes.Length; i++)
                    {
                        Entity.Scores[i] = new HoleScore(course.Holes[i].Par, 0);
                    }
                    Caption = $"New Scorecard for {course.Name}";
                }
                else
                {
                    var scorecard = await _apiClient.GetEntityAsync(guid);
                    if (scorecard != null)
                    {
                        Entity = scorecard;
                        Caption = "Edit Scorecard";
                    }
                    _appStateService.IsBusy = false;
                    if (scorecard == null) { _navigation.NavigateTo("/courses"); }
                }
            }
            finally { _appStateService.IsBusy = false; }
        }

        public async Task SaveAsync()
        {
            try
            {
                _appStateService.IsBusy = true;
                Errors.Clear();

                if (IsDelete)
                {
                    var accepted = await _apiClient.DeleteEntityAsync(Entity.Id);
                    if (accepted)
                    {
                        var scorecardInfo = _appStateService.Scorecards.FirstOrDefault(scorecard => scorecard.Id == Entity.Id);
                        if (scorecardInfo != null) { _appStateService.Scorecards.Remove(scorecardInfo); }
                        _navigation.NavigateTo($"/scorecards/{Entity.CourseId}");
                    }
                    else { Errors.Add("Failed to complete the delete request for the scorecard"); }
                }
                else
                {
                    var valid = Validate?.Invoke() ?? true;
                    if (valid)
                    {
                        Scorecard? scorecard = null;
                        if (IsNew) { scorecard = await _apiClient.CreateEntityAsync(Entity); }
                        else { scorecard = await _apiClient.UpdateEntityAsync(Entity); }
                        if (scorecard != null)
                        {
                            var current = _appStateService.Scorecards.FirstOrDefault(scorecard => scorecard.Id == Entity.Id);
                            if (current != null) { current.CopyFrom(scorecard); }
                        }
                        else { Errors.Add("Failed to complete the update request for the scorecard"); }
                    }
                    else { Errors.Add("The scorecard is not in a valid state"); }
                }
            }
            finally { _appStateService.IsBusy = false; }
        }

        private readonly IApiClient<Scorecard, ScorecardInfo> _apiClient;
        private readonly IAppStateService _appStateService;
        private readonly IApiClient<Course, CourseInfo> _courseApiClient;
        private readonly NavigationManager _navigation;
    }
}
