using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public class EventDispatcher : IDispatcher<IEvent>
    {
        public EventDispatcher(
            IProjectionManager<Course> courseProjectionManager,
            IProjectionManager<CourseInfo> courseInfoProjectionManager,
            IProjectionManager<Scorecard> scorecardProjectionManager,
            IProjectionManager<ScorecardInfo> scorecardInfoProjectionManager)
        {
            CourseProjectionManager = courseProjectionManager ?? throw new NotImplementedException();
            CourseInfoProjectionManager = courseInfoProjectionManager ?? throw new NotImplementedException();
            ScorecardProjectionManager = scorecardProjectionManager ?? throw new NotImplementedException();
            ScorecardInfoProjectionManager = scorecardInfoProjectionManager ?? throw new NotImplementedException();
            EventHandlers = new Dictionary<Type, Func<IEvent, Task>>()
            {
                { typeof(CourseCreated), (e) => HandleCourseCreatedAsync((CourseCreated)e) },
                { typeof(CourseUpdated), (e) => HandleCourseUpdatedAsync((CourseUpdated)e) },
                { typeof(Deleted<Course>), (e) => HandleCourseDeletedAsync((Deleted<Course>)e) },
                { typeof(Deleted<Scorecard>), (e) => HandleScorecardDeletedAsync((Deleted<Scorecard>)e) },
                { typeof(Failed), (e) => HandleFailedAsync((Failed)e) },
                { typeof(ScorecardCreated), (e) => HandleScorecardCreatedAsync((ScorecardCreated)e) },
                { typeof(ScorecardUpdated), (e) => HandleScorecardUpdatedAsync((ScorecardUpdated)e) }
            };
        }


        public async Task DispatchAsync(IEvent @event)
        {
            Func<IEvent, Task> handler = null;
            if (!EventHandlers.TryGetValue(@event.GetType(), out handler)) { throw new ArgumentException($"{@event?.GetType()} is not a valid event."); }
            await handler.Invoke(@event);
        }

        #region Event Handlers

        private async Task HandleCourseCreatedAsync(CourseCreated @event)
        {
            await Task.WhenAll(
                        CourseProjectionManager.CreateProjectionAsync(@event),
                        CourseInfoProjectionManager.CreateProjectionAsync(new CourseInfoCreated(new CourseInfo(@event))));
        }

        private async Task HandleCourseDeletedAsync(Deleted<Course> @event)
        {
            await Task.WhenAll(
                        CourseProjectionManager.DeleteProjectionAsync(@event),
                        CourseInfoProjectionManager.DeleteProjectionAsync(new Deleted<CourseInfo>(@event.PlayerId, @event.Id)));
        }

        private async Task HandleCourseUpdatedAsync(CourseUpdated @event)
        {
            await Task.WhenAll(
                        CourseProjectionManager.UpdateProjectionAsync(@event),
                        CourseInfoProjectionManager.UpdateProjectionAsync(new CourseInfoUpdated(new CourseInfo(@event))));
        }

        private async Task HandleFailedAsync(Failed @event)
        {
            // TODO: Log failed events. Email?
            await Task.CompletedTask;
        }

        private async Task HandleScorecardCreatedAsync(ScorecardCreated @event)
        {
            await Task.WhenAll(
                        ScorecardProjectionManager.CreateProjectionAsync(@event),
                        ScorecardInfoProjectionManager.CreateProjectionAsync(new ScorecardInfoCreated(new ScorecardInfo(@event))));
        }

        private async Task HandleScorecardDeletedAsync(Deleted<Scorecard> @event)
        {
            await Task.WhenAll(
                        ScorecardProjectionManager.DeleteProjectionAsync(@event),
                        ScorecardInfoProjectionManager.DeleteProjectionAsync(new Deleted<ScorecardInfo>(@event.PlayerId, @event.Id)));
        }

        private async Task HandleScorecardUpdatedAsync(ScorecardUpdated @event)
        {
            await Task.WhenAll(
                        ScorecardProjectionManager.UpdateProjectionAsync(@event),
                        ScorecardInfoProjectionManager.UpdateProjectionAsync(new ScorecardInfoUpdated(new ScorecardInfo(@event))));
        }

        #endregion

        private readonly IProjectionManager<Course> CourseProjectionManager;
        private readonly IProjectionManager<CourseInfo> CourseInfoProjectionManager;
        private readonly Dictionary<Type, Func<IEvent, Task>> EventHandlers;
        private readonly IProjectionManager<Scorecard> ScorecardProjectionManager;
        private readonly IProjectionManager<ScorecardInfo> ScorecardInfoProjectionManager;
    }
}
