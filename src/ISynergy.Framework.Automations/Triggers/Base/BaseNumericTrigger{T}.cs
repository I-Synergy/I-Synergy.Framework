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
    /// Base generic numeric trigger. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseNumericTrigger<T> : BaseTrigger, ITrigger
        where T : struct
    {
        /// <summary>
        /// Gets or sets the Below property value.
        /// </summary>
        public T Below
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Above property value.
        /// </summary>
        public T Above
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="for"></param>
        protected BaseNumericTrigger(Guid automationId, T below, T above, TimeSpan @for)
            : base(automationId)
        {
            Argument.IsNotNull(nameof(below), below);
            Argument.IsNotNull(nameof(above), above);
            Argument.IsNotNull(nameof(@for), @for);

            Below = below;
            Above = above;
            For = @for;
        }

        /// <summary>
        /// Trigger for T type properties.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="function"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="callbackAsync"></param>
        protected BaseNumericTrigger(
            Guid automationId, 
            Func<(IObservableClass Entity, IProperty<T> Property)> function, 
            T below, 
            T above, 
            Func<T, Task> callbackAsync)
            : this(automationId, below, above, TimeSpan.Zero)
        {
            if (function.Invoke() is (IObservableClass Entity, IProperty<T> Property) result)
            {
                result.Property.BroadCastChanges = true;

                MessageService.Default.Register<PropertyChangedMessage<T>>(this, m =>
                {
                    var comparer = Comparer<T>.Default;
                    if (comparer.Compare(m.NewValue, below) < 0 && comparer.Compare(m.NewValue, above) > 0)
                        callbackAsync.Invoke(m.NewValue).Wait();
                });
            }
        }
    }
}
