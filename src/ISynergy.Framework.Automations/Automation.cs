using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Collections;
using System;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Automations
{
    /// <summary>
    /// Automation class.
    /// </summary>
    public class Automation : ObservableClass
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
        /// Gets or sets the Excecution Timeout property value.
        /// </summary>
        public TimeSpan ExecutionTimeout
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Triggers property value.
        /// </summary>
        public ObservableConcurrentCollection<object> Triggers
        {
            get { return GetValue<ObservableConcurrentCollection<object>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Conditions property value.
        /// </summary>
        public ObservableConcurrentCollection<ICondition> Conditions
        {
            get { return GetValue<ObservableConcurrentCollection<ICondition>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Actions property value.
        /// </summary>
        public ObservableConcurrentCollection<IAction> Actions
        {
            get { return GetValue<ObservableConcurrentCollection<IAction>>(); }
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
            ExecutionTimeout = TimeSpan.FromSeconds(30);
            Triggers = new ObservableConcurrentCollection<object>();
            Conditions = new ObservableConcurrentCollection<ICondition>();
            Actions = new ObservableConcurrentCollection<IAction>();
        }
    }
}
