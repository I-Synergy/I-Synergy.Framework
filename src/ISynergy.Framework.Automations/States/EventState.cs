using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Validation;
using System;
using System.Reflection;

namespace ISynergy.Framework.Automations.States
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public class EventState : ObservableClass, IState
    {
        /// <summary>
        /// Gets or sets the StateId property value.
        /// </summary>
        public Guid StateId
        {
            get { return GetValue<Guid>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// You can use For to have the trigger only fire if the state holds for some time.
        /// </summary>
        public TimeSpan For
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Event property value.
        /// </summary>
        public EventInfo Event
        {
            get { return GetValue<EventInfo>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the EventData property value.
        /// </summary>
        public object EventData
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="eventData"></param>
        /// <param name="for"></param>
        public EventState(EventInfo @event, object eventData, TimeSpan @for)
        {
            Argument.IsNotNull(nameof(@event), @event);
            Argument.IsNotNull(nameof(@for), @for);

            StateId = Guid.NewGuid();
            For = @for;
            Event = @event;
            EventData = eventData;
        }
    }
}
