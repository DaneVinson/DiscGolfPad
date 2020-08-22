using Dgp.Data.AzureTables;
using Dgp.Domain.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dgp.Service.Dispatcher.AzureFunctions
{
    public class CommandsQueueTrigger
    {
        public CommandsQueueTrigger(
            IDispatcher<ICommand<Course>> courseCommandDispatcher,
            IDispatcher<ICommand<Scorecard>> scorecardCommandDispatcher)
        {
            CourseCommandDispatcher = courseCommandDispatcher ?? throw new ArgumentNullException();
            ScorecardCommandDispatcher = scorecardCommandDispatcher ?? throw new ArgumentNullException();
        }


        [FunctionName(FunctionName)]
        public async Task Run([ServiceBusTrigger(QueueName, Connection = ConnectionName)]string commandMessage, ILogger log)
        {
            // Deserialize the PlayerMessage
            var playerMessage = JsonSerializer.Deserialize<PlayerMessage>(commandMessage);

            // Get the Type of the command from the message
            var commandType = Assembly.GetAssembly(typeof(Course)).GetType(playerMessage.Type);

            // Deserialize the command from the message and dispatch
            var command = JsonSerializer.Deserialize(playerMessage.Data, commandType);
            if (command is ICommand<Course>)
            {
                await CourseCommandDispatcher.DispatchAsync(command as ICommand<Course>);
            }
            else if (command is ICommand<Scorecard>)
            {
                await ScorecardCommandDispatcher.DispatchAsync(command as ICommand<Scorecard>);
            }
            else
            {
                log.LogError($"Function {FunctionName} received invalid command message: {commandMessage}");
                return;
            }

            log.LogInformation($"Function {FunctionName} received Service Bus Queue message: {commandMessage}");
        }


        private readonly IDispatcher<ICommand<Course>> CourseCommandDispatcher;
        private readonly IDispatcher<ICommand<Scorecard>> ScorecardCommandDispatcher;


        private const string ConnectionName = "AzureServiceBus:ConnectionString";
        private const string FunctionName = "DispatchCommand";
        private const string QueueName = "commands";
    }
}
