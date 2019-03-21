using System;
using System.Collections.Generic;

namespace ISynergy.Data.Tests.TestClasses
{
    public class ProductGroup : ObservableClass
    {
        /// <summary>
        /// Gets or sets the GroupId property value.
        /// </summary>
        public Guid GroupId
        {
            get { return GetValue<Guid>(); }
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

        /// <summary>
        /// Gets or sets the Products property value.
        /// </summary>
        public List<Product> Products
        {
            get { return GetValue<List<Product>>(); }
            set { SetValue(value); }
        }
    }
}
