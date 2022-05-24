using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.WebDav.Filters.Internal
{
    internal class WebDavExceptionFilterMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            options.Filters.AddService(typeof(WebDavExceptionFilter));
        }
    }
}
