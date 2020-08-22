using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public class CourseAggregate : EntityAggregate<Course>
    {
        public CourseAggregate(IValidator<Course> cousreValidator, IValidator<ICommand<Course>> deleteValidator)
        {
            CourseValidator = cousreValidator ?? throw new ArgumentNullException();
            DeleteValidator = deleteValidator ?? throw new ArgumentNullException();

            CommandHandlers = new Dictionary<Type, Func<ICommand<Course>, IEnumerable<IEvent>>>()
            {
                { typeof(CreateCourse), c => HandleCreateCourse((CreateCourse)c) },
                { typeof(Delete<Course>), c => HandleDeleteCourse((Delete<Course>)c) },
                { typeof(UpdateCourse), c => HandleUpdateCourse((UpdateCourse)c) }
            };
            EventHandlers = new Dictionary<Type, Action<IEvent>>()
            {
                { typeof(CourseCreated), (e) => HandleCourseCreated((CourseCreated)e) },
                { typeof(Deleted<Course>), (e) => HandleCourseDeleted((Deleted<Course>)e) },
                { typeof(CourseUpdated), (e) => HandleCourseUpdated((CourseUpdated)e) }
            };
        }

        #region Command Handlers

        public IEnumerable<IEvent> HandleCreateCourse(CreateCourse command)
        {
            // Validate the command.
            if (State != null) { return new IEvent[] { new Failed("Can't create an existing course.") }; }
            if (command == null) { return new IEvent[] { new Failed("Can handle a null command.") }; }
            var validationResult = CourseValidator.Validate(command);
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            return new IEvent[] { new CourseCreated(command) };
        }

        public IEnumerable<IEvent> HandleDeleteCourse(Delete<Course> command)
        {
            // Validate the command.
            if (State == null) { return new IEvent[] { new Failed("Course doesn't exist.") }; }
            if (Deleted) { return new IEvent[] { new Failed("The course was previously deleted.") }; }
            var validationResult = DeleteValidator.Validate(command);
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            return new IEvent[] { new Deleted<Course>(command) };
        }

        public IEnumerable<IEvent> HandleUpdateCourse(UpdateCourse command)
        {
            // Validate the command.
            if (State == null) { return new IEvent[] { new Failed("Course doesn't exist.") }; }
            if (Deleted) { return new IEvent[] { new Failed("The course was previously deleted.") }; }
            var validationResult = CourseValidator.Validate(command);
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            // Validate that the command doesn't conflict with the current state.
            if (command.Id != State.Id) { validationResult.Errors.Add(new ValidationFailure("Id", "Id mismatch.")); }
            if (command.Holes.Length != State.Holes.Length) { validationResult.Errors.Add(new ValidationFailure("Holes", "Cannot change the number of holes.")); }
            if (command.PlayerId != State.PlayerId) { validationResult.Errors.Add(new ValidationFailure("PlayerId", "Cannot change a course's player.")); }
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            return new IEvent[] { new CourseUpdated(command) };
        }

        #endregion

        #region Event Handlers

        private void HandleCourseCreated(CourseCreated @event)
        {
            State = new Course(@event);
        }

        private void HandleCourseDeleted(Deleted<Course> @event)
        {
            Deleted = true;
        }

        private void HandleCourseUpdated(CourseUpdated @event)
        {
            for (int i = 0; i < State.Holes.Length; i++)
            {
                State.Holes[i].Distance = @event.Holes[i].Distance;
                State.Holes[i].Par = @event.Holes[i].Par;
            }
            State.ImageUri = @event.ImageUri;
            State.Location = @event.Location;
            State.Name = @event.Name;
        }

        #endregion

        private readonly IValidator<Course> CourseValidator;
        private readonly IValidator<ICommand<Course>> DeleteValidator;
    }
}
