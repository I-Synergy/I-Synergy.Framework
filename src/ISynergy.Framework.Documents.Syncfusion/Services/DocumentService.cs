using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Documents.Abstractions.Services;
using ISynergy.Framework.Documents.Models;
using Microsoft.Extensions.Options;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.XlsIO;
using System.Collections;
using System.ComponentModel;

namespace ISynergy.Framework.Documents.Services;

/// <summary>
/// Reporting service.
/// </summary>
internal class DocumentService : IDocumentService
{
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="languageService"></param>
    /// <param name="options"></param>
    public DocumentService(ILanguageService languageService, IOptions<SyncfusionLicenseOptions> options)
    {
        _languageService = languageService;

        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(options.Value?.LicenseKey);
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
        IWorkbook workbook = null;

        var table = spreadsheetRequest.DataSet.ToDataTable(spreadsheetRequest.FileName);

        try
        {
            using var engine = new ExcelEngine();
            workbook = engine.Excel.Workbooks.Create(1);

            var importDataOptions = new ExcelImportDataOptions
            {
                FirstRow = 1,
                FirstColumn = 1,
                IncludeHeader = true,
                PreserveTypes = false
            };

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
        WordDocument document = null;

        try
        {
            if (documentRequest.Template is { } templateBytes && templateBytes.ToMemoryStream() is { } stream)
            {
                using (document = new WordDocument(stream, Syncfusion.DocIO.FormatType.Docx))
                {
                    if (documentRequest.Stationery is { } imageArray && imageArray.Length > 0)
                    {
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

                    document.MailMerge.MergeImageField += new WeakEventHandler<MergeImageFieldEventArgs>(MergeImageField).Handler;

                    document.MailMerge.RemoveEmptyParagraphs = true;
                    document.MailMerge.ExecuteNestedGroup(dataSet, commands);

                    foreach (var paragraph in mailMergeImageParagraph.EnsureNotNull())
                    {
                        if (paragraph.ChildEntities is not null)
                        {
                            foreach (ParagraphItem paraItem in paragraph.ChildEntities)
                            {
                                if (paraItem is WPicture picture && paragraph.OwnerTextBody is WTableCell cell)
                                {
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

                                    break;
                                }
                            }
                        }
                    }

                    if (exportAsPdf)
                    {
                        using (var iORenderer = new DocIORenderer())
                        {
                            iORenderer.Settings.ChartRenderingOptions.ImageFormat = Syncfusion.OfficeChart.ExportImageFormat.Jpeg;
                            iORenderer.Settings.EmbedFonts = true;

                            var pdfDocument = iORenderer.ConvertToPDF(document);
                            pdfDocument.Save(result);
                            pdfDocument.Close();
                        };
                    }
                    else
                    {
                        document.Save(result, Syncfusion.DocIO.FormatType.Docx);
                    }
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
    /// The mail merge imgae paragraph
    /// </summary>
    private readonly List<WParagraph> mailMergeImageParagraph = new List<WParagraph>();

    /// <summary>
    /// Handles the MergeImageField event of the MailMerge control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The <see cref="MergeImageFieldEventArgs" /> instance containing the event data.</param>
    private void MergeImageField(object sender, MergeImageFieldEventArgs args)
    {
        // Get the image from disk during Merge.
        if (args.FieldName == "Image")
            mailMergeImageParagraph.Add(args.CurrentMergeField.OwnerParagraph);
    }
}
