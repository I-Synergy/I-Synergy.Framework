using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.List
{
    /// <summary>
    /// Links to help navigate through the lists of objects, based on the given offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListResponseLinks<T> where T : IResponseObject
    {
        /// <summary>
        /// The URL to the current set of payments.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<ListResponse<T>> Self { get; set; }

        /// <summary>
        /// The previous set of objects, if available.
        /// </summary>
        /// <value>The previous.</value>
        public UrlObjectLink<ListResponse<T>> Previous { get; set; }

        /// <summary>
        /// The next set of objects, if available.
        /// </summary>
        /// <value>The next.</value>
        public UrlObjectLink<ListResponse<T>> Next { get; set; }

        /// <summary>
        /// The URL to the payments list endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
