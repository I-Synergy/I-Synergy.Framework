﻿using Newtonsoft.Json;
using ISynergy.Data;

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
        /// Gets or sets the InputFirst property value.
        /// </summary>
        public string InputFirst
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the InputLast property value.
        /// </summary>
        public string InputLast
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}