using Newtonsoft.Json;
using System;

namespace ISynergy
{
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class ModelBase : ObservableClass, IModelBase
    {
        /// <summary>
        /// Gets or sets the Memo property value.
        /// </summary>
        public string Memo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
        public int Version
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CreatedDate property value.
        /// </summary>
        public DateTimeOffset CreatedDate
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CreatedBy property value.
        /// </summary>
        public string CreatedBy
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ChangedDate property value.
        /// </summary>
        public DateTimeOffset? ChangedDate
        {
            get { return GetValue<DateTimeOffset?>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ChangedBy property value.
        /// </summary>
        public string ChangedBy
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}