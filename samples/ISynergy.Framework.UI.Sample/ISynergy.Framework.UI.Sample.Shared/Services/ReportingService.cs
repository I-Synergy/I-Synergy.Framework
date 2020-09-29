using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISynergy.Framework.UI.Sample.Abstractions.Services;

namespace ISynergy.Framework.UI.Sample.Services
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
