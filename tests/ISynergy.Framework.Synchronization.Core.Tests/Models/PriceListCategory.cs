using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Synchronization.Core.Tests.Models
{
    public class PriceListCategory
    {
        public PriceListCategory()
        {
            Details = new List<PriceListDetail>();
        }

        public PriceList PriceList { get; set; }

        public Guid PriceListId { get; set; }
        public string PriceCategoryId { get; set; }

        public IList<PriceListDetail> Details { get; set; }


    }
}
