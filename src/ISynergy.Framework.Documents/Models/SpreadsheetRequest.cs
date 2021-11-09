using ISynergy.Services.Models.Base;
using System.Collections;

namespace ISynergy.Services.Models
{
    /// <summary>
    /// Class SpreadsheetRequest.
    /// </summary>
    public class SpreadsheetRequest<T> : BaseRequest
    {
        /// <summary>
        /// Gets or sets the data set.
        /// </summary>
        /// <value>The data set.</value>
        public IEnumerable<T> DataSet { get; set; } = Enumerable.Empty<T>();
    }
}
