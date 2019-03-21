using System;

namespace ISynergy.Data.Tests.TestClasses
{
    public class Product : ObservableClass
    {
        public Product()
        {
        }

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
        public Guid ProductId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Quantity property value.
        /// </summary>
        public int Quantity
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Price property value.
        /// </summary>
        public decimal Price
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Date property value.
        /// </summary>
        public DateTimeOffset? Date
        {
            get { return GetValue<DateTimeOffset?>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ProductGroup property value.
        /// </summary>
        public ProductGroup ProductGroup
        {
            get { return GetValue<ProductGroup>(); }
            set { SetValue(value); }
        }
    }
}
