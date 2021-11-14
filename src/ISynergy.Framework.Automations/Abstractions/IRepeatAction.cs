namespace ISynergy.Framework.Automations.Abstractions
{
    /// <summary>
    /// Repeatable action interface.
    /// </summary>
    public interface IRepeatAction : IAction
    {
        /// <summary>
        /// Repeat type.
        /// </summary>
        RepeatTypes RepeatType { get; }

        /// <summary>
        /// Circuitbreaker count.
        /// </summary>
        int CountCircuitBreaker { get; }

        /// <summary>
        /// Validate action.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Validate(object entity);
    }
}
