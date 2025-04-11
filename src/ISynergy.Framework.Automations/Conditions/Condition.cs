using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Base;
using ISynergy.Framework.Automations.Enumerations;

namespace ISynergy.Framework.Automations.Conditions;

/// <summary>
/// Condition.
/// </summary>
public class Condition<TEntity> : AutomationModel, ICondition
    where TEntity : class, new()
{
    /// <summary>
    /// Gets or sets the ConditionId property value.
    /// </summary>
    public Guid ConditionId
    {
        get { return GetValue<Guid>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the ConditionType property value.
    /// </summary>
    public OperatorTypes Operator
    {
        get { return GetValue<OperatorTypes>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Validator property value.
    /// </summary>
    public new Func<TEntity, bool> Validator
    {
        get { return GetValue<Func<TEntity, bool>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="validator"></param>
    public Condition(Guid automationId, Func<TEntity, bool> validator)
        : base(automationId)
    {
        ConditionId = Guid.NewGuid();
        Validator = validator;
        Operator = OperatorTypes.And;
    }

    /// <summary>
    /// ValidateAction object with given conditions.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool ValidateCondition(object entity)
    {
        if (entity is TEntity obj)
            return Validator?.Invoke(obj) ?? false;

        return false;
    }
}
