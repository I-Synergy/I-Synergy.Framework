using ISynergy.Framework.Automations.States.Base;
using System;

namespace ISynergy.Framework.Automations.States
{
    /// <summary>
    /// State trigger based on a string.
    /// </summary>
    public class StringState : BaseState<string>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        public StringState(string from, string to, TimeSpan @for)
            : base(from, to, @for)
        {
        }
    }
}
