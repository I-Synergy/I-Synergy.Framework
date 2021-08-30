using ISynergy.Framework.Automations.Enumerations;
using System;

namespace ISynergy.Framework.Automations.Abstractions
{
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
        /// Validate with object.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Validate(object entity);
    }
}
