using ISynergy.Framework.Automations.Enumerations;

namespace ISynergy.Framework.Automations.Abstractions
{
    public interface IRepeatAction : IAction
    {
        RepeatTypes RepeatType { get; }
        int CountCircuitBreaker { get; }
        bool Validate(object entity);
    }
}
