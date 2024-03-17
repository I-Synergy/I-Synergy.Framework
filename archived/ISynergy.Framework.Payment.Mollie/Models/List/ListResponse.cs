using System.Collections.Generic;
using ISynergy.Framework.Payment.Converters;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.List
{
    /// <summary>
    /// Class ListResponse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListResponse<T> where T : IResponseObject
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [JsonConverter(typeof(ListResponseConverter))]
        [JsonProperty("_embedded")]
        public List<T> Items { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public ListResponseLinks<T> Links { get; set; }
    }

    /// <summary>
    /// Class ListResponseSimple.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListResponseSimple<T>
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public List<T> Data { get; set; }
    }
}
