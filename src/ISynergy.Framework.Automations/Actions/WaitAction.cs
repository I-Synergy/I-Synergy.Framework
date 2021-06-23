using ISynergy.Framework.Automations.Actions.Base;
using ISynergy.Framework.Automations.Enumerations;
using System;

namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Wait action.
    /// </summary>
    public class WaitAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the WaitTime property value.
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Timeout property value.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContinueOnTimeout property value.
        /// </summary>
        public bool ContinueOnTimeout
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        public WaitAction(Guid automationId)
            : base(automationId)
        {
            WaitTime = TimeSpan.Zero;
            ContinueOnTimeout = false;
            Timeout = TimeSpan.Zero;
        }
    }
}
