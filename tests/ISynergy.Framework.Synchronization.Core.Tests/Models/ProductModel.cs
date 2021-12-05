using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Synchronization.Core.Tests.Models
{
    public partial class ProductModel
    {
        public ProductModel()
        {
            Product = new HashSet<Product>();
        }

        public int ProductModelId { get; set; }
        public string Name { get; set; }
        public string CatalogDescription { get; set; }
        public Guid? Rowguid { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
