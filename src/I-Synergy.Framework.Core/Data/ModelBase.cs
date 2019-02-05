using Newtonsoft.Json;
using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ISynergy.Models.Base
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