using Dgp.Client.Blazor.Services;
using Dgp.Domain.Core;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.ViewModels
{
    public class CoursesViewModel
    {
        public CoursesViewModel(
            IApiClient<Course, CourseInfo> apiClient,
            IAppStateService stateService,
            NavigationManager navigation)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _appStateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }


        public void EditCourse(string id)
        {
            _navigation.NavigateTo($"/course/{id}");
        }

        public async Task InitializeCoursesAsync(bool forceReload = false)
        {
            if (!_appStateService.Courses.Any() || forceReload)
            {
                _appStateService.IsBusy = true;
                var courses = await _apiClient.GetEntitiesAsync();
                _appStateService.Courses = courses.ToList();
                _appStateService.IsBusy = false;
            }
        }


        public List<CourseInfo> Courses => _appStateService.Courses;
        public bool IsBusy => _appStateService.IsBusy;


        private readonly IApiClient<Course, CourseInfo> _apiClient;
        private readonly IAppStateService _appStateService;
        private readonly NavigationManager _navigation;
    }
}
