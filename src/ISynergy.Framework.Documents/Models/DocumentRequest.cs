using ISynergy.Framework.Documents.Models.Base;

namespace ISynergy.Framework.Documents.Models;

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
    /// Gets or sets the stationery image bytes.
    /// </summary>
    public byte[] Stationery { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Document mail-merge fields
    /// </summary>
    public IEnumerable<TDocument> Document { get; set; } = Enumerable.Empty<TDocument>();

    /// <summary>
    /// Details mail-merge fields.
    /// </summary>
    public IEnumerable<TDetails> DocumentDetails { get; set; } = Enumerable.Empty<TDetails>();

    /// <summary>
    /// Alternatives mail-merge fields.
    /// </summary>
    public IEnumerable<TDetails> DocumentAlternatives { get; set; } = Enumerable.Empty<TDetails>();
}
