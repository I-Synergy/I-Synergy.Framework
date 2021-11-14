using Sample.Abstractions.Services;
using System;

namespace Sample.Services
{
    /// <summary>
    /// Class PrintingService.
    /// </summary>
    public class PrintingService : IPrintingService
    {
        /// <summary>
        /// Prints the dymo label.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void PrintDymoLabel(string content)
        {
            throw new NotImplementedException();
        }
    }
}
