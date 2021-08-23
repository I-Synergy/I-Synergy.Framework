using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations.Services
{
    /// <summary>
    /// Automation service.
    /// </summary>
    public class AutomationService : IAutomationService
    {
        private readonly ILogger _logger;
        private readonly AutomationOptions _options;
        private readonly IAutomationManager _manager;
        private readonly List<Automation> _automations;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public AutomationService(
            IAutomationManager manager,
            IOptions<AutomationOptions> options,
            ILogger<AutomationService> logger)
        {
            _automations = new List<Automation>();
            _manager = manager;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Gets all automations.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAutomationsAsync() =>
            _automations.AddNewRange(await _manager.GetItemsAsync());

        /// <summary>
        /// Validates the conditions.
        /// </summary>
        /// <param name="automation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> ValidateConditionsAsync(Automation automation, object value)
        {
            var areAllConditionsValid = true;

            // Check if all conditions met.
            foreach (var condition in automation.Conditions)
            {
                if (condition.Operator == OperatorTypes.And)
                    areAllConditionsValid = areAllConditionsValid && condition.Validate(value);
                else
                    areAllConditionsValid = areAllConditionsValid || condition.Validate(value);
            }

            return Task.FromResult(areAllConditionsValid);
        }

        /// <summary>
        /// Starts executing the automation.
        /// </summary>
        /// <param name="automation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExecuteAsync(Automation automation, object value)
        {
            if (automation.IsActive && await ValidateConditionsAsync(automation, value))
            {
                if (await GetTasksFromActionsAsync(automation, value) is List<Func<Task>> jobs)
                {
                    using var cancellationTokenSource = new CancellationTokenSource();
                    var cancellationToken = cancellationTokenSource.Token;
                    var timeoutTimer = new Timer(
                        TimeoutElapsed,
                        cancellationTokenSource,
                        (int)automation.ExecutionTimeout.TotalMilliseconds, 
                        -1);

                    foreach (var job in jobs)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();

                        await job
                            .Invoke()
                            .ContinueWith(x => _logger.LogInformation("Task completed!"), cancellationToken);
                    }

                    return new ActionResult(true, value);
                }
            }

            return new ActionResult(false, value);
        }

        private void TimeoutElapsed(object state)
        {
            if (state is CancellationTokenSource cancellationTokenSource)
                cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Executes the automation and is cancelled after timeout is elapsed.
        /// </summary>
        /// <param name="automation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Task<List<Func<Task>>> GetTasksFromActionsAsync(Automation automation, object value)
        {
            var result = new List<Func<Task>>();
            var repeatCount = 0;

            // Excecute all actions.
            for (int i = 0; i < automation.Actions.Count; i++)
            {
                if (automation.Actions[i] is CommandAction commandAction && commandAction.Command.CanExecute(commandAction.CommandParameter))
                {
                    result.Add(new Func<Task>(() => Task.Run(() => commandAction.Command.Execute(commandAction.CommandParameter))));
                }
                else if (automation.Actions[i] is DelayAction delayAction)
                {
                    result.Add(new Func<Task>(() => Task.Delay(delayAction.Delay)));
                }
                else if (automation.Actions[i] is AutomationAction automationAction)
                {
                    result.Add(new Func<Task>(() => ExecuteAsync(automationAction.Automation, value)));
                }
                else if (automation.Actions[i] is RepeatPreviousAction repeatAction && repeatAction.Count > 0)
                {
                    if (repeatCount == repeatAction.Count)
                    {
                        repeatCount = 0;
                    }
                    else
                    {
                        i -= 2;
                        repeatCount += 1;
                    }
                }
                else if (automation.Actions[i] is IRepeatAction untilRepeatAction && untilRepeatAction.RepeatType == RepeatTypes.Until)
                {
                    if (repeatCount.Equals(untilRepeatAction.CountCircuitBreaker) || untilRepeatAction.Validate(value))
                    {
                        repeatCount = 0;
                    }
                    else
                    {
                        i -= 2;
                        repeatCount += 1;
                    }
                }
                else if (automation.Actions[i] is IRepeatAction whileRepeatAction && whileRepeatAction.RepeatType == RepeatTypes.While)
                {
                    if (repeatCount.Equals(whileRepeatAction.CountCircuitBreaker) || !whileRepeatAction.Validate(value))
                    {
                        repeatCount = 0;
                    }
                    else
                    {
                        i -= 2;
                        repeatCount += 1;
                    }
                }
            }

            return Task.FromResult(result);
        }
    }
}
