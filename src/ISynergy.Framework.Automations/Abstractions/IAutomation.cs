using ISynergy.Framework.Automations.Conditions;
using ISynergy.Framework.Automations.Enumerations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations.Abstractions
{
    /// <summary>
    /// Automation interface.
    /// </summary>
    public interface IAutomation
    {
        /// <summary>
        /// Gets or sets the AutomationId property value.
        /// </summary>
        public Guid AutomationId { get;}

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the Mode property value.
        /// </summary>
        RunModes Mode { get; set; }

        /// <summary>
        /// Gets or sets the IsActive property value.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the Triggers property value.
        /// </summary>
        ObservableCollection<ITrigger> Triggers { get; set; }

        /// <summary>
        /// Gets or sets the Conditions property value.
        /// </summary>
        ObservableCollection<ICondition> Conditions { get; set; }

        /// <summary>
        /// Gets or sets the Actions property value.
        /// </summary>
        ObservableCollection<IAction> Actions { get; set; }

        Task<bool> ExecuteAsync(object value);
    }
}
