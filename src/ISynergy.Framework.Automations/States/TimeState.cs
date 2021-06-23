using ISynergy.Framework.Automations.States.Base;
using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Automations.States
{
    /// <summary>
    /// Time state.
    /// </summary>
    public class TimeState : ObservableClass
    {
        /// <summary>
        /// Gets or sets the After property value.
        /// </summary>
        public TimeSpan After
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Before property value.
        /// </summary>
        public TimeSpan Before
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsFixedTime property value.
        /// </summary>
        public bool IsFixedTime
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Monday property value.
        /// </summary>
        public bool Monday
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Tuesday property value.
        /// </summary>
        public bool Tuesday
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Wednesday property value.
        /// </summary>
        public bool Wednesday
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Thursday property value.
        /// </summary>
        public bool Thursday
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Friday property value.
        /// </summary>
        public bool Friday
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Saturday property value.
        /// </summary>
        public bool Saturday
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Sunday property value.
        /// </summary>
        public bool Sunday
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="isFixedTime"></param>
        public TimeState(TimeSpan after, TimeSpan before, bool isFixedTime)
        {
            Argument.IsNotNull(nameof(after), after);
            Argument.IsNotNull(nameof(before), before);

            After = after;
            Before = before;
            Monday = true;
            Tuesday = true;
            Wednesday = true;
            Thursday = true;
            Friday = true;
            Saturday = true;
            Sunday = true;
            IsFixedTime = isFixedTime;
        }
    }
}
