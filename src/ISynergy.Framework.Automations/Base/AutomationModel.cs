using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Validation;
using System;

namespace ISynergy.Framework.Automations.Base
{
    /// <summary>
    /// Abstract class for automation objects.
    /// </summary>
    public abstract class AutomationModel : ObservableClass
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
        /// Default model constructor for automation objects.
        /// </summary>
        /// <param name="automationId"></param>
        protected AutomationModel(Guid automationId)
        {
            Argument.IsNotNullOrEmpty(nameof(automationId), automationId);
            AutomationId = automationId;
        }
    }
}
