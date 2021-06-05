using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;

namespace ISynergy.Framework.Core.Data.Tests.TestClasses
{
    /// <summary>
    /// Class Product.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ObservableClass" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ObservableClass" />
    public class Product : ObservableClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        public Product()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="price">The price.</param>
        public Product(Guid id, string name, int quantity, decimal price)
            : this()
        {
            ProductId = id;
            Name = name;
            Quantity = quantity;
            Price = price;
            Date = null;
            MarkAsClean();
        }

        /// <summary>
        /// Gets or sets the ProductId property value.
        /// </summary>
        /// <value>The product identifier.</value>
        [Identity]
        public Guid ProductId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Quantity property value.
        /// </summary>
        /// <value>The quantity.</value>
        public int Quantity
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Price property value.
        /// </summary>
        /// <value>The price.</value>
        public decimal Price
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Date property value.
        /// </summary>
        /// <value>The date.</value>
        public DateTimeOffset? Date
        {
            get { return GetValue<DateTimeOffset?>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ProductTypes property value.
        /// </summary>
        public List<ProductGroup> ProductGroups
        {
            get { return GetValue<List<ProductGroup>>(); }
            set { SetValue(value); }
        }
    }
}
