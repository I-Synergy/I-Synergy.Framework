using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IClipboardService
    {
        Task<byte[]> GetPngImageFromClipboardAsync();
    }
}
