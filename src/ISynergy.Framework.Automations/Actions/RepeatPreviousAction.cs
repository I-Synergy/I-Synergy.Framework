using ISynergy.Framework.Automations.Actions.Base;
using System;

namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Repeats action.
    /// </summary>
    public class RepeatPreviousAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the Count property value.
        /// </summary>
        public int Count
        {
            get { return GetValue<int>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="count"></param>
        public RepeatPreviousAction(Guid automationId, int count)
            : base(automationId)
        {
            Count = count;
        }
    }
}

