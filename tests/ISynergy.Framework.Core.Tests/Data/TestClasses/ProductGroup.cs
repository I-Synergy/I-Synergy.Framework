using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Data.Tests.TestClasses
{
    /// <summary>
    /// Class ProductGroup.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ObservableClass" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ObservableClass" />
    public class ProductGroup : ObservableClass
    {
        /// <summary>
        /// Gets or sets the GroupId property value.
        /// </summary>
        /// <value>The group identifier.</value>
        public Guid GroupId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Products property value.
        /// </summary>
        /// <value>The products.</value>
        public List<Product> Products
        {
            get { return GetValue<List<Product>>(); }
            set { SetValue(value); }
        }
    }
}
