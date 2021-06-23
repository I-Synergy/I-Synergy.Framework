using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions.Base;
using ISynergy.Framework.Automations.Enumerations;
using System;

namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Executes another automation.
    /// </summary>
    public class AutomationAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the Service property value.
        /// </summary>
        public IAutomation Automation
        {
            get { return GetValue<IAutomation>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        public AutomationAction(Guid automationId)
            : base(automationId)
        {
        }
    }
}
