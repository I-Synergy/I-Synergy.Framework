﻿using ISynergy.Framework.Documents.Abstractions.Services;
using ISynergy.Framework.Documents.Models;
using System.Threading;

namespace Sample.Syncfusion
{
    public class Startup
    {
        private readonly IDocumentService _documentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        public Startup(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// run as an asynchronous operation.
        /// </summary>
        public async Task RunAsync()
        {
            Console.WriteLine("Syncfusion implementation started...");

            var data = new List<TestData>()
            {
                new TestData { Id = Guid.NewGuid(), Name = "Test1", Date = DateTimeOffset.Now, Quantity= 1, Price = 1m },
                new TestData { Id = Guid.NewGuid(), Name = "Test2", Date = DateTimeOffset.Now, Quantity= 2, Price = 2m },
                new TestData { Id = Guid.NewGuid(), Name = "Test3", Date = DateTimeOffset.Now, Quantity= 3, Price = 3m },
                new TestData { Id = Guid.NewGuid(), Name = "Test4", Date = DateTimeOffset.Now, Quantity= 4, Price = 4m },
                new TestData { Id = Guid.NewGuid(), Name = "Test5", Date = DateTimeOffset.Now, Quantity= 5, Price = 5m }
            };

            var request = new SpreadsheetRequest<TestData>()
            {
                DataSet = data,
                FileName = "test.xlsx"
            };

            if (await _documentService.GenerateExcelSheetAsync(request).ConfigureAwait(false) is Stream fileStream)
            {
                Console.WriteLine($"File size is {fileStream.Length}.");
            }
            else
            {
                Console.WriteLine("Failed to create file.");
            }

            Console.ReadLine();
        }
    }
}