using ISynergy.Framework.Core.Validation;
using Newtonsoft.Json;
using System;

namespace ISynergy.Framework.Core.Data
{
    /// <summary>
    /// Class ModelBase.
    /// Implements the <see cref="ObservableClass" />
    /// Implements the <see cref="IModelBase" />
    /// </summary>
    /// <seealso cref="ObservableClass" />
    /// <seealso cref="IModelBase" />
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class ModelBase : ObservableClass, IModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        /// <param name="validation">The validation.</param>
        protected ModelBase(ValidationTriggers validation = ValidationTriggers.Manual)
            : base(validation)
        {
        }

        /// <summary>
        /// Gets or sets the Memo property value.
        /// </summary>
        /// <value>The memo.</value>
        public string Memo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
        /// <value>The version.</value>
        public int Version
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CreatedDate property value.
        /// </summary>
        /// <value>The created date.</value>
        public DateTimeOffset CreatedDate
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CreatedBy property value.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ChangedDate property value.
        /// </summary>
        /// <value>The changed date.</value>
        public DateTimeOffset? ChangedDate
        {
            get { return GetValue<DateTimeOffset?>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ChangedBy property value.
        /// </summary>
        /// <value>The changed by.</value>
        public string ChangedBy
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
