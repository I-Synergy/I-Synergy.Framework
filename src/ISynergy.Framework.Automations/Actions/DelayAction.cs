using ISynergy.Framework.Automations.Actions.Base;
using ISynergy.Framework.Automations.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Execute delay.
    /// </summary>
    public class DelayAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the Delay property value.
        /// </summary>
        public TimeSpan Delay
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        public DelayAction(Guid automationId, TimeSpan delay)
            : base(automationId)
        {
            Delay = delay;
        }
    }
}
