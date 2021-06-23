using ISynergy.Framework.Automations.Triggers.Base;
using System;

namespace ISynergy.Framework.Automations.Triggers
{
    /// <summary>
    /// State trigger based on a boolean.
    /// </summary>
    public class BooleanStateTrigger : BaseTrigger<bool>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        public BooleanStateTrigger(Guid automationId, bool from, bool to, TimeSpan @for)
            : base(automationId, from, to, @for)
        {
        }
    }

    /// <summary>
    /// State trigger based on a string.
    /// </summary>
    public class StringStateTrigger : BaseTrigger<string>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        public StringStateTrigger(Guid automationId, string from, string to, TimeSpan @for)
            : base(automationId, from, to, @for)
        {
        }
    }
}
