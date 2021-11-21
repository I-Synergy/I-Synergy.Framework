using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Base;
using System;

namespace ISynergy.Framework.Automations.Triggers.Base
{
    /// <summary>
    /// Base trigger.
    /// </summary>
    public abstract class BaseTrigger : AutomationModel, ITrigger
    {
        /// <summary>
        /// Gets or sets the TriggerId property value.
        /// </summary>
        public Guid TriggerId
        {
            get { return GetValue<Guid>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// You can use For to have the trigger only fire if the state holds for some time.
        /// </summary>
        public TimeSpan For
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        protected BaseTrigger(
            Guid automationId)
            : base(automationId)
        {
            TriggerId = Guid.NewGuid();
        }
    }
}
