using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IClipboardService
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// Gets the PNG image from clipboard asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        Task<byte[]> GetPngImageFromClipboardAsync();
    }
}
