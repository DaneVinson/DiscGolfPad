using Dgp.Client.Blazor.Services;
using Dgp.Domain.Core;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.ViewModels
{
    public class CourseViewModel : EditViewModel<Course>
    {
        public CourseViewModel(
            IApiClient<Course, CourseInfo> apiClient,
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

        public void GenerateHoles()
        {
            if (HoleGenerationCount > 0)
            {
                Entity.Holes = new Hole[HoleGenerationCount];
                for (int i = 0; i < HoleGenerationCount; i ++)
                {
                    Entity.Holes[i] = new Hole(3, 0);
                }
            }
        }

        public async Task InitializeAsync(string id)
        {
            Guid.TryParse(id, out var guid);
            IsNew = guid == Guid.Empty;
            if (IsNew)
            {
                Entity.Id = Guid.NewGuid();
                Entity.PlayerId = Bilbo.Id;
                Caption = "New Course";
            }
            else
            {
                _appStateService.IsBusy = true;
                var course = await _apiClient.GetEntityAsync(guid);
                if (course != null) 
                { 
                    Entity = course;
                    Caption = $"Edit {course.Name}";
                }
                _appStateService.IsBusy = false;
                if (course == null) { _navigation.NavigateTo("/courses"); }
            }
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
                        var courseInfo = _appStateService.Courses.FirstOrDefault(course => course.Id == Entity.Id);
                        if (courseInfo != null) { _appStateService.Courses.Remove(courseInfo); }
                        _navigation.NavigateTo("/courses");
                    }
                    else { Errors.Add($"Failed to complete the delete request for {Entity.Name}"); }
                }
                else
                {
                    var valid = Validate?.Invoke() ?? true;
                    if (valid)
                    {
                        Course? course = null;
                        if (IsNew) { course = await _apiClient.CreateEntityAsync(Entity); }
                        else { course = await _apiClient.UpdateEntityAsync(Entity); }
                        if (course != null)
                        {
                            var current = _appStateService.Courses.FirstOrDefault(c => c.Id == Entity.Id);
                            if (current != null) { current.CopyFrom(course); }
                        }
                        else { Errors.Add($"Failed to complete the update request for {Entity.Name}"); }
                    }
                    else { Errors.Add("The course is not in a valid state"); }
                }
            }
            finally { _appStateService.IsBusy = false; }
        }


        public int HoleGenerationCount { get; set; }

        private readonly IApiClient<Course, CourseInfo> _apiClient;
        private readonly IAppStateService _appStateService;
        private readonly NavigationManager _navigation;
    }
}
