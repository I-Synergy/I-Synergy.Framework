using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Base;
using ISynergy.Framework.Automations.Enumerations;
using System;

namespace ISynergy.Framework.Automations.Actions.Base
{
    /// <summary>
    /// Base action.
    /// </summary>
    public abstract class BaseAction : AutomationModel, IAction
    {
        /// <summary>
        /// Gets or sets the ActionId property value.
        /// </summary>
        public Guid ActionId
        {
            get { return GetValue<Guid>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Data property value.
        /// </summary>
        public object Data
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="actionType"></param>
        protected BaseAction(Guid automationId)
            : base(automationId)
        {
            ActionId = Guid.NewGuid();
        }
    }
}
