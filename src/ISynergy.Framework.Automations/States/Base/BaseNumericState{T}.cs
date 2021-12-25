using ISynergy.Framework.Core.Validation;
using System;

namespace ISynergy.Framework.Automations.States.Base
{
    /// <summary>
    /// Base generic numeric state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseNumericState<T> : BaseState
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
        /// 
        /// </summary>
        /// <param name="below"></param>
        /// <param name="above"></param>
        protected BaseNumericState(T below, T above)
            : base(TimeSpan.Zero)
        {
            Argument.IsNotNull(below);
            Argument.IsNotNull(above);

            Below = below;
            Above = above;
        }
    }
}
