using ISynergy.Framework.Documents.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Documents.Abstractions.Services
{
    /// <summary>
    /// Reporting service.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Generates a Microsoft Excel  stream.
        /// </summary>
        /// <param name="spreadsheetRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> GenerateExcelSheetAsync<T>(SpreadsheetRequest<T> spreadsheetRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a Microsoft Word or Pdf document stream.
        /// </summary>
        /// <param name="documentRequest"></param>
        /// <param name="exportAsPdf"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> GenerateDocumentAsync<TDocument, TDetails>(DocumentRequest<TDocument, TDetails> documentRequest, bool exportAsPdf = false, CancellationToken cancellationToken = default);
    }
}
