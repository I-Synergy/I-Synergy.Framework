using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services
{
    /// <summary>
    /// Class OverviewClientBase.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OverviewClientBase<T> where T : IResponseObject {

        /// <summary>
        /// Maps a <see cref="ListResponse{T}"/> to an <see cref="OverviewModel{T}"/>.
        /// </summary>
        /// <param name="list">The list response to map.</param>
        /// <returns>OverviewModel&lt;T&gt;.</returns>
        protected OverviewModel<T> Map(ListResponse<T> list) {
            return new OverviewModel<T> {
                Items = list.Items,
                Navigation = new OverviewNavigationLinksModel(list.Links.Previous, list.Links.Next)
            };
        }

        /// <summary>
        /// Creates the URL object.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>UrlObjectLink&lt;ListResponse&lt;T&gt;&gt;.</returns>
        protected UrlObjectLink<ListResponse<T>> CreateUrlObject(string url) {
            return new UrlObjectLink<ListResponse<T>> {
                Href = url
            };
        }
    }
}
