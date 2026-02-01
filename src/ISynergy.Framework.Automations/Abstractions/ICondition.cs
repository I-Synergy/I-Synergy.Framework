using ISynergy.Framework.Automations.Enumerations;

namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Public interface of a condition
/// </summary>
public interface ICondition
{
    /// <summary>
    /// Gets or sets the ConditionId property value.
    /// </summary>
    public Guid ConditionId { get; }

    /// <summary>
    /// Gets or sets the AutomationId property value.
    /// </summary>
    public Guid AutomationId { get; }

    /// <summary>
    /// Gets or sets the ConditionType property value.
    /// </summary>
    OperatorTypes Operator { get; set; }

    /// <summary>
    /// ValidateAction with object.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool ValidateCondition(object entity);
}
