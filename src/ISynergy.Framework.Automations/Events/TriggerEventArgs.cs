using ISynergy.Framework.Core.Validation;
using System;

namespace ISynergy.Framework.Automations.Events
{
    public class TriggerEventArgs<T> : EventArgs
    {
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }

        public TriggerEventArgs(T oldValue, T newValue)
            : base()
        {
            Argument.IsNotNull(nameof(oldValue), oldValue);
            Argument.IsNotNull(nameof(newValue), newValue);

            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
