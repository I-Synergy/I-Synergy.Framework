using ISynergy.Framework.Models.Base;
using System.Collections;

namespace ISynergy.Framework.Models
{
    /// <summary>
    /// Class DocumentRequest.
    /// </summary>
    public class DocumentRequest<TDocument, TDetails> : BaseRequest
    {
        /// <summary>
        /// Gets and sets the template.
        /// </summary>
        public byte[] Template { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Gets or sets the stationary image bytes.
        /// </summary>
        public byte[] Stationary { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Document mail-merge fields
        /// </summary>
        public IEnumerable<TDocument> Document { get; set; } = Enumerable.Empty<TDocument>();

        /// <summary>
        /// Details mail-merge fields.
        /// </summary>
        public IEnumerable<TDetails>? DocumentDetails { get; set; }

        /// <summary>
        /// Alternatives mail-merge fields.
        /// </summary>
        public IEnumerable<TDetails>? DocumentAlternatives { get; set; }
    }
}
