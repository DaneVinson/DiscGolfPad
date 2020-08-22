using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class EntityAggregate<TEntity> : IAggregate<TEntity> where TEntity : IEntity
    {
        public EntityAggregate()
        { }


        public void ApplyEvents(IEnumerable<IEvent> events)
        {
            if (events.Any(e => !EventHandlers.ContainsKey(e.GetType()))) { throw new Exception("Attempt to apply and unknown event type."); }

            foreach (var @event in events)
            {
                EventHandlers[@event.GetType()].Invoke(@event);
            }
        }

        public static string GetFormattedErrorMessage(ValidationResult validationResult) =>
            string.Join(";", validationResult.Errors.Select(e => $"{e.PropertyName},{e.ErrorMessage}"));

        public TEntity GetState()
        {
            return (TEntity)Activator.CreateInstance(typeof(TEntity), State);
        }

        public IEnumerable<IEvent> HandleCommand(ICommand<TEntity> command)
        {
            if (!CommandHandlers.ContainsKey(command.GetType())) { throw new Exception("Attempt to handle and unknown command type."); }

            return CommandHandlers[command.GetType()].Invoke(command);
        }


        protected bool Deleted { get; set; }

        protected TEntity State { get; set; }

        protected Dictionary<Type, Func<ICommand<TEntity>, IEnumerable<IEvent>>> CommandHandlers { get; set; }
        protected Dictionary<Type, Action<IEvent>> EventHandlers { get; set; }
    }
}
