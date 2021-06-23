using ISynergy.Framework.Automations.States.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ISynergy.Framework.Automations.States
{
    /// <summary>
    /// State trigger based on a boolean.
    /// </summary>
    public class BooleanState : BaseState<bool>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        public BooleanState(bool value, TimeSpan @for)
            : base(!value, value, @for)
        {
        }
    }
}
