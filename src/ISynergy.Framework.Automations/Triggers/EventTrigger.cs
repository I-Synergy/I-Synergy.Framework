using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Validation;
using System;

namespace ISynergy.Framework.Automations.Triggers
{
    /// <summary>
    /// Trigger based on an event.
    /// </summary>
    public class EventTrigger : BaseTrigger
    {
        /// <summary>
        /// Gets or sets the Event property value.
        /// </summary>
        public string Event
        {
            get { return GetValue<string>(); }
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
        /// <param name="automationId"></param>
        /// <param name="event"></param>
        /// <param name="eventData"></param>
        public EventTrigger(Guid automationId, string @event, object eventData)
            : base(automationId)
        {
            Argument.IsNotNullOrEmpty(nameof(@event), @event);

            Event = @event;
            EventData = eventData;
        }
    }
}
