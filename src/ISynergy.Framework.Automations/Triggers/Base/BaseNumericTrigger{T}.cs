using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Core.Validation;
using System;

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
    }
}
