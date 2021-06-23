using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Conditions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Core.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations
{
    /// <summary>
    /// Automation class.
    /// </summary>
    public class Automation : ObservableClass, IAutomation
    {
        /// <summary>
        /// Gets or sets the AutomationId property value.
        /// </summary>
        public Guid AutomationId
        {
            get { return GetValue<Guid>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Mode property value.
        /// </summary>
        public RunModes Mode
        {
            get { return GetValue<RunModes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsActive property value.
        /// </summary>
        public bool IsActive
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Triggers property value.
        /// </summary>
        public ObservableCollection<ITrigger> Triggers
        {
            get { return GetValue<ObservableCollection<ITrigger>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Conditions property value.
        /// </summary>
        public ObservableCollection<ICondition> Conditions
        {
            get { return GetValue<ObservableCollection<ICondition>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Actions property value.
        /// </summary>
        public ObservableCollection<IAction> Actions
        {
            get { return GetValue<ObservableCollection<IAction>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor for Automation.
        /// </summary>
        public Automation()
        {
            AutomationId = Guid.NewGuid();
            Mode = RunModes.Single;
            IsActive = false;
            Triggers = new ObservableCollection<ITrigger>();
            Conditions = new ObservableCollection<ICondition>();
            Actions = new ObservableCollection<IAction>();
        }

        public async Task<bool> ExecuteAsync(object value)
        {
            if (IsActive)
            {
                var areAllConditionsValid = true;

                // Check if all conditions met.
                foreach (var condition in Conditions)
                {
                    if (condition.Operator == OperatorTypes.And)
                        areAllConditionsValid = areAllConditionsValid && condition.Validate(value);
                    else
                        areAllConditionsValid = areAllConditionsValid || condition.Validate(value);
                }

                if (areAllConditionsValid)
                {
                    var repeatCount = 0;

                    // Excecute all actions.
                    for (int i = 0; i < Actions.Count; i++)
                    {
                        if (Actions[i] is CommandAction commandAction && commandAction.Command.CanExecute(commandAction.CommandParameter))
                        {
                            commandAction.Command.Execute(commandAction.CommandParameter);
                        }
                        else if (Actions[i] is DelayAction delayAction)
                        {
                            await Task.Delay(delayAction.Delay);
                        }
                        else if (Actions[i] is AutomationAction automationAction)
                        {
                            await automationAction.Automation.ExecuteAsync(value);
                        }
                        else if (Actions[i] is RepeatPreviousAction repeatAction && repeatAction.Count > 0)
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
                        else if (Actions[i] is IRepeatAction untilRepeatAction && untilRepeatAction.RepeatType == RepeatTypes.Until)
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
                        else if (Actions[i] is IRepeatAction whileRepeatAction && whileRepeatAction.RepeatType == RepeatTypes.While)
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
                }

                return areAllConditionsValid;
            }

            return false;
        }
    }
}
