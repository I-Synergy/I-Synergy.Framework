using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.UI.Sample.Abstractions.Services
{
    /// <summary>
    /// Interface IReportingService
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Generates the excel sheet asynchronous.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Task.</returns>
        Task GenerateExcelSheetAsync(IEnumerable table, string filename);
        /// <summary>
        /// Generates the word document asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="foldername">The foldername.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="command">The command.</param>
        /// <param name="template">The template.</param>
        /// <param name="stationary">The stationary.</param>
        /// <returns>Task.</returns>
        Task GenerateWordDocumentAsync(string filename, string foldername, object dataset, List<DictionaryEntry> command, byte[] template, byte[] stationary);
    }
}
