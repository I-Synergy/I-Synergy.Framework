using System;
using System.Net.Http;

namespace ISynergy.Framework.AspNetCore.Tests.Internals
{
    internal class HttpResponseMessageWithTiming
    {
        internal HttpResponseMessage Response { get; set; }
        internal TimeSpan Timing { get; set; }
    }
}
