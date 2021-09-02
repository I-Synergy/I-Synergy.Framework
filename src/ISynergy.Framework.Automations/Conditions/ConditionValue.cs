using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.States;
using System;

namespace ISynergy.Framework.Automations.Conditions
{
    /// <summary>
    /// Value holder for a state.
    /// </summary>
    public class ConditionValue
    {
        /// <summary>
        /// Placeholder for IState.
        /// </summary>
        public Type State { get; }

        /// <summary>
        /// Placeholder for the value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value"></param>
        public ConditionValue(object value)
        {
            switch (value)
            {
                case int:
                    State = typeof(IntegerState);
                    break;
                case double:
                    State = typeof(DoubleState);
                    break;
                case decimal:
                    State = typeof(DecimalState);
                    break;
                case bool:
                    State = typeof(BooleanState);
                    break;
                case TimeSpan:
                    State = typeof(TimeState);
                    break;
                default:
                    State = typeof(StringState);
                    break;
            }

            Value = value;
        }

        /// <summary>
        /// Default constructor for events.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="value"></param>
        public ConditionValue(string @event, object value)
        {
            State = typeof(EventState);
            Value = value;
        }
    }
}
