using ISynergy.Framework.Automations.States.Base;
using System;

namespace ISynergy.Framework.Automations.States
{
    /// <summary>
    /// Numeric trigger based on an integer.
    /// </summary>
    public class IntegerState : BaseState<int>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        public IntegerState(int from, int to, TimeSpan @for)
            : base(from, to, @for)
        {
        }
    }

    /// <summary>
    /// Numeric trigger based on a decimal.
    /// </summary>
    public class DecimalState : BaseState<decimal>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        public DecimalState(decimal from, decimal to, TimeSpan @for)
            : base(from, to, @for)
        {
        }
    }

    /// <summary>
    /// Numeric trigger based on a double.
    /// </summary>
    public class DoubleState : BaseState<double>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="for"></param>
        public DoubleState(double from, double to, TimeSpan @for)
            : base(from, to, @for)
        {
        }
    }
}
