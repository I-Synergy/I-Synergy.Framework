namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Action that executes a command.
    /// </summary>
    public class CommandAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the Command property value.
        /// </summary>
        public ICommand Command
        {
            get { return GetValue<ICommand>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CommandParameter property value.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Timeout property value.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor for command action.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="command"></param>
        /// <param name="commandParameter"></param>
        public CommandAction(Guid automationId, ICommand command, object commandParameter = null)
            : base(automationId)
        {
            Command = command;
            CommandParameter = commandParameter;
            Timeout = TimeSpan.Zero;
        }
    }
}
