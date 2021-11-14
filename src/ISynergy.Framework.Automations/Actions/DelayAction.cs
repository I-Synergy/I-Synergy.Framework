namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Execute delay.
    /// </summary>
    public class DelayAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the Delay property value.
        /// </summary>
        public TimeSpan Delay
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="delay"></param>
        public DelayAction(Guid automationId, TimeSpan delay)
            : base(automationId)
        {
            Delay = delay;
        }
    }
}
