using System.Collections.Generic;

namespace ISynergy.Framework.Core.Abstractions
{
    public interface IResult
    {
        List<string> Messages { get; set; }
        bool Succeeded { get; set; }
    }
}
