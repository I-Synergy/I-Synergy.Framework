using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Abstractions.Services;

namespace Sample.Services
{
    public class ReportingService : IReportingService
    {
        public Task GenerateExcelSheetAsync(IEnumerable table, string filename)
        {
            throw new NotImplementedException();
        }

        public Task GenerateWordDocumentAsync(string filename, string foldername, object dataset, List<DictionaryEntry> command, byte[] template, byte[] stationary)
        {
            throw new NotImplementedException();
        }
    }
}
