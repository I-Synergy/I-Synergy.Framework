using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Messaging;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations.Triggers.Base
{
    /// <summary>
    /// Base trigger.
    /// </summary>
    public abstract class BaseTrigger<T> : BaseTrigger, ITrigger<T>
    {
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
        /// <param name="function"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="callbackAsync"></param>
        /// <param name="for"></param>
        protected BaseTrigger(
            Guid automationId,
            Func<(IObservableClass Entity, IProperty<T> Property)> function,
            T from,
            T to,
            Func<T, Task> callbackAsync,
            TimeSpan @for)
            : base(automationId)
        {
            Argument.IsNotNull(nameof(from), from);
            Argument.IsNotNull(nameof(to), to);
            Argument.Equals(nameof(from), from, to);
            Argument.IsNotNull(nameof(@for), @for);

            From = from;
            To = to;
            For = @for;

            if (function.Invoke() is (IObservableClass Entity, IProperty<T> Property) result)
            {
                result.Property.BroadCastChanges = true;

                MessageService.Default.Register<PropertyChangedMessage<T>>(this, m =>
                {
                    var comparer = Comparer<T>.Default;
                    if (comparer.Compare(m.NewValue, to) == 0 && comparer.Compare(m.OldValue, from) == 0)
                        callbackAsync.Invoke(m.NewValue).Wait();
                });
            }
        }
    }
}
