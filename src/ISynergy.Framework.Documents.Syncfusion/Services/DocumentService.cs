using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Documents.Abstractions.Services;
using ISynergy.Framework.Documents.Models;
using ISynergy.Framework.Documents.Syncfusion.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.XlsIO;
using System.Collections;

namespace ISynergy.Framework.Documents.Syncfusion.Services;

/// <summary>
/// Reporting service.
/// </summary>
/// <remarks>
/// <para>
/// <strong>AOT/Trimming notice:</strong> Syncfusion DocIO, DocIORenderer, and XlsIO libraries use reflection
/// internally for MailMerge, document templating, and Excel I/O. This service is not compatible with
/// Native AOT publishing. Applications targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> cannot use
/// this library and should implement a native document generation alternative.
/// </para>
/// </remarks>
internal class DocumentService : IDocumentService
{

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="options"></param>
    public DocumentService(
        IOptions<SyncfusionLicenseOptions> options)
    {
        global::Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(options.Value?.LicenseKey);
    }

    /// <summary>
    /// Generates the excel sheet asynchronous.
    /// </summary>
    /// <param name="spreadsheetRequest">The spreadsheet request.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    /// <exception cref="Exception"></exception>
    public Task<Stream> GenerateExcelSheetAsync<T>(SpreadsheetRequest<T> spreadsheetRequest, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNull(spreadsheetRequest);

        Stream result = new MemoryStream();
        IWorkbook? workbook = null;

        var table = spreadsheetRequest.DataSet.ToDataTable(spreadsheetRequest.FileName);

        try
        {
            using var engine = new ExcelEngine();
            workbook = engine.Excel.Workbooks.Create(1);

            workbook.Worksheets[0].ImportDataTable(table, true, 1, 1);
            workbook.Worksheets[0].UsedRange.AutofitColumns();
            workbook.Worksheets[0].UsedRange.WrapText = true;

            workbook.Version = ExcelVersion.Xlsx;
            workbook.SaveAs(result);
        }
        finally
        {
            if (workbook is not null)
                workbook.Close();

            result.Position = 0;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// generate word document as an asynchronous operation.
    /// </summary>
    /// <param name="documentRequest">The document request.</param>
    /// <param name="exportAsPdf">if set to <c>true</c> [export as PDF].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public Task<Stream> GenerateDocumentAsync<TDocument, TDetails>(DocumentRequest<TDocument, TDetails> documentRequest, bool exportAsPdf = false, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNull(documentRequest);

        Stream result = new MemoryStream();
        WordDocument? document = null;

        try
        {
            if (documentRequest.Template is { } templateBytes && templateBytes.ToMemoryStream() is { } stream)
            {
                using (document = new WordDocument(stream, global::Syncfusion.DocIO.FormatType.Docx))
                {
                    AddStationeryWatermark(document, documentRequest.Stationery);
                    MergeDocument(document, documentRequest);
                    ScaleMailMergeImages();
                    ExportDocument(document, result, exportAsPdf);
                }
            }
        }
        finally
        {
            result.Position = 0;

            if (document is not null)
                document.Close();
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Adds the stationery image as a page watermark, if a stationery image was supplied.
    /// </summary>
    /// <param name="document">The document to add the watermark to.</param>
    /// <param name="imageArray">The stationery image.</param>
    private static void AddStationeryWatermark(WordDocument document, byte[]? imageArray)
    {
        if (imageArray is null || imageArray.Length == 0)
            return;

        //Adds picture watermark to the document.
        var section = document.Sections[0];
        var paragraph = section.HeadersFooters.Header.AddParagraph();
        var picture = paragraph.AppendPicture(imageArray);
        picture.TextWrappingStyle = TextWrappingStyle.Behind;
        picture.Width = section.PageSetup.PageSize.Width;
        picture.Height = section.PageSetup.PageSize.Height;
        picture.HorizontalOrigin = HorizontalOrigin.Page;
        picture.VerticalOrigin = VerticalOrigin.Page;
    }

    /// <summary>
    /// Executes the nested mail merge of the document, document details and alternatives.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TDetails">The type of the document details.</typeparam>
    /// <param name="document">The document to merge into.</param>
    /// <param name="documentRequest">The document request containing the merge data.</param>
    private void MergeDocument<TDocument, TDetails>(WordDocument document, DocumentRequest<TDocument, TDetails> documentRequest)
    {
        var dataSet = new MailMergeDataSet();
        var commands = new List<DictionaryEntry>();

        var documentDataTable = new MailMergeDataTable(nameof(documentRequest.Document), documentRequest.Document);
        dataSet.Add(documentDataTable);
        commands.Add(new DictionaryEntry(documentDataTable.GroupName, string.Empty));

        var detailsDataTable = new MailMergeDataTable(nameof(documentRequest.DocumentDetails), documentRequest.DocumentDetails);
        dataSet.Add(detailsDataTable);
        commands.Add(new DictionaryEntry(detailsDataTable.GroupName, string.Empty));

        var alternativesDataTable = new MailMergeDataTable(nameof(documentRequest.DocumentAlternatives), documentRequest.DocumentAlternatives);
        dataSet.Add(alternativesDataTable);
        commands.Add(new DictionaryEntry(alternativesDataTable.GroupName, string.Empty));

        document.MailMerge.MergeImageField += (sender, args) =>
        {
            // Get the image from disk during Merge.
            if (args.FieldName == "Image")
                mailMergeImageParagraph.Add(args.CurrentMergeField.OwnerParagraph);
        };

        document.MailMerge.RemoveEmptyParagraphs = true;
        document.MailMerge.ExecuteNestedGroup(dataSet, commands);
    }

    /// <summary>
    /// Scales the merged images down to the width of the table cell that contains them.
    /// </summary>
    private void ScaleMailMergeImages()
    {
        foreach (var paragraph in mailMergeImageParagraph.EnsureNotNull())
        {
            if (paragraph.OwnerTextBody is not WTableCell cell)
                continue;

            var picture = paragraph.ChildEntities.EnsureNotNull().OfType<WPicture>().FirstOrDefault();
            if (picture is null)
                continue;

            //Get the cell width and reduce the 5 points from total width
            //to avoid the image overlapping on the cell borders.
            var newWidth = cell.Width - 10;

            //Check whether the image width exceeds the cell width.
            if (picture.Width > newWidth)
            {
                var originalWidth = picture.Width;
                var originalHeight = picture.Height;

                var nPercentH = newWidth / originalWidth;

                //Scaling the image to the cell width.
                picture.Height = Math.Max((int)Math.Round(originalHeight * nPercentH), 1) - 10; // Just in case;
                picture.Width = newWidth;
            }
        }
    }

    /// <summary>
    /// Writes the merged document to the result stream, either as PDF or as Docx.
    /// </summary>
    /// <param name="document">The merged document.</param>
    /// <param name="result">The stream to write to.</param>
    /// <param name="exportAsPdf">if set to <c>true</c> the document is exported as PDF.</param>
    private static void ExportDocument(WordDocument document, Stream result, bool exportAsPdf)
    {
        if (exportAsPdf)
        {
            using (var iORenderer = new DocIORenderer())
            {
                iORenderer.Settings.ChartRenderingOptions.ImageFormat = global::Syncfusion.OfficeChart.ExportImageFormat.Jpeg;
                iORenderer.Settings.EmbedFonts = true;

                var pdfDocument = iORenderer.ConvertToPDF(document);
                pdfDocument.Save(result);
                pdfDocument.Close();
            }
        }
        else
        {
            document.Save(result, global::Syncfusion.DocIO.FormatType.Docx);
        }
    }

    /// <summary>
    /// The mail merge imgae paragraph
    /// </summary>
    private readonly List<WParagraph> mailMergeImageParagraph = new List<WParagraph>();
}
