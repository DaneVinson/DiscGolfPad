using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public class ScorecardAggregate : EntityAggregate<Scorecard>
    {
        public ScorecardAggregate(IValidator<Scorecard> scorecardValidator, IValidator<ICommand<Scorecard>> deleteValidator)
        {
            DeleteValidator = deleteValidator ?? throw new ArgumentNullException();
            ScorecardValidator = scorecardValidator ?? throw new ArgumentNullException();

            CommandHandlers = new Dictionary<Type, Func<ICommand<Scorecard>, IEnumerable<IEvent>>>()
            {
                { typeof(CreateScorecard), c => HandleCreateScorecard((CreateScorecard)c) },
                { typeof(Delete<Scorecard>), c => HandleDeleteScorecard((Delete<Scorecard>)c) },
                { typeof(UpdateScorecard), c => HandleUpdateScorecard((UpdateScorecard)c) }
            };
            EventHandlers = new Dictionary<Type, Action<IEvent>>()
            {
                { typeof(ScorecardCreated), (e) => HandleScorecardCreated((ScorecardCreated)e) },
                { typeof(Deleted<Scorecard>), (e) => HandleScorecardDeleted((Deleted<Scorecard>)e) },
                { typeof(ScorecardUpdated), (e) => HandleScorecardUpdated((ScorecardUpdated)e) }
            };
        }

        #region Command Handlers

        public IEnumerable<IEvent> HandleCreateScorecard(CreateScorecard command)
        {
            // Validate the command.
            if (State != null) { return new IEvent[] { new Failed("Can't create an existing scorecard.") }; }
            if (command == null) { return new IEvent[] { new Failed("Can handle a null command.") }; }
            var validationResult = ScorecardValidator.Validate(command);
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            return new IEvent[] { new ScorecardCreated(command) };
        }

        public IEnumerable<IEvent> HandleDeleteScorecard(Delete<Scorecard> command)
        {
            // Validate the command.
            if (State == null) { return new IEvent[] { new Failed("Scorecard doesn't exist.") }; }
            if (Deleted) { return new IEvent[] { new Failed("The scorecard was previously deleted.") }; }
            var validationResult = DeleteValidator.Validate(command);
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            return new IEvent[] { new Deleted<Scorecard>(command) };
        }

        public IEnumerable<IEvent> HandleUpdateScorecard(UpdateScorecard command)
        {
            // Validate the command.
            if (State == null) { return new IEvent[] { new Failed("Scorecard doesn't exist.") }; }
            if (Deleted) { return new IEvent[] { new Failed("The scorecard was previously deleted.") }; }
            var validationResult = ScorecardValidator.Validate(command);
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            // Validate that the command doesn't conflict with the current state.
            if (command.Id != State.Id) { validationResult.Errors.Add(new ValidationFailure("Id", "Id mismatch.")); }
            if (command.Scores.Length != State.Scores.Length) { validationResult.Errors.Add(new ValidationFailure("HolePars", "Cannot change the number of holes.")); }
            if (command.PlayerId != State.PlayerId) { validationResult.Errors.Add(new ValidationFailure("PlayerId", "Cannot change a scorecard's player.")); }
            if (!validationResult.IsValid) { return new IEvent[] { new Failed(GetFormattedErrorMessage(validationResult)) }; }

            return new IEvent[] { new ScorecardUpdated(command) };
        }

        #endregion

        #region Event Handlers

        private void HandleScorecardCreated(ScorecardCreated @event)
        {
            State = new Scorecard(@event);
        }

        private void HandleScorecardDeleted(Deleted<Scorecard> @event)
        {
            Deleted = true;
        }

        private void HandleScorecardUpdated(ScorecardUpdated @event)
        {
            for (int i = 0; i < State.Scores.Length; i++)
            {
                State.Scores[i].Par = @event.Scores[i].Par;
                State.Scores[i].Score = @event.Scores[i].Score;
            }
            State.CourseId = @event.CourseId;
            State.Date = @event.Date;
            State.Notes = @event.Notes;
        }

        #endregion

        private readonly IValidator<Scorecard> ScorecardValidator;
        private readonly IValidator<ICommand<Scorecard>> DeleteValidator;
    }
}
