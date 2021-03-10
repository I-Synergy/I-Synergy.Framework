using ISynergy.Framework.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Models
{
    public class TestItem : ModelBase
    {
        /// <summary>
        /// Gets or sets the Id property value.
        /// </summary>
        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
