using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Events;
using ISynergy.Framework.Core.Validation;
using System;

namespace ISynergy.Framework.Automations.Triggers.Base
{
    /// <summary>
    /// Base trigger.
    /// </summary>
    public abstract class BaseTrigger<T> : BaseTrigger, ITrigger<T>
    {
        public EventHandler<TriggerEventArgs<T>> Triggered;

        /// <summary>
        /// Gets or sets the From property value.
        /// </summary>
        public T From
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the To property value.
        /// </summary>
        public T To
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        protected BaseTrigger(Guid automationId, T from, T to, TimeSpan @for)
            : base(automationId)
        {
            Argument.IsNotNull(nameof(from), from);
            Argument.IsNotNull(nameof(to), to);
            Argument.IsNotNull(nameof(@for), @for);

            From = from;
            To = to;
            For = @for;
        }
    }
}
