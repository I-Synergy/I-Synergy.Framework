namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Raises event.
    /// </summary>
    public class EventAction : BaseAction
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
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        public EventAction(Guid automationId)
            : base(automationId)
        {
        }
    }
}
