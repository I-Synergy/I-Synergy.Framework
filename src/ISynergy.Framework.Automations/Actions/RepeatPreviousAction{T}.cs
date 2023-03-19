using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions.Base;
using ISynergy.Framework.Automations.Enumerations;

namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Repeats action.
    /// </summary>
    public class RepeatPreviousAction<TEntity> : BaseAction, IRepeatAction
         where TEntity : class, new()
    {
        /// <summary>
        /// Gets or sets the CountCircuitBreaker property value.
        /// </summary>
        public int CountCircuitBreaker
        {
            get { return GetValue<int>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RepeatType property value.
        /// </summary>
        public RepeatTypes RepeatType
        {
            get { return GetValue<RepeatTypes>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RepeatCondition property value.
        /// </summary>
        public Func<TEntity, bool> RepeatValidator
        {
            get { return GetValue<Func<TEntity, bool>>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="repeatType"></param>
        /// <param name="validator"></param>
        /// <param name="countCircuitBreaker"></param>
        public RepeatPreviousAction(Guid automationId, RepeatTypes repeatType, Func<TEntity, bool> validator, int countCircuitBreaker = 100)
            : base(automationId)
        {
            RepeatType = repeatType;
            RepeatValidator = validator;
            CountCircuitBreaker = countCircuitBreaker;
        }

        /// <summary>
        /// Validate object with given conditions.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Validate(object entity) => RepeatValidator?.Invoke(entity as TEntity) ?? false;
    }
}
