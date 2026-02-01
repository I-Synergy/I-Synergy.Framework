using ISynergy.Framework.AspNetCore.WebDav.Controllers;
using ISynergy.Framework.AspNetCore.WebDav.Result;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support
{
    [Route("{*path}")]
    /* [Authorize] */
    public class TestWebDavController : WebDavControllerBase
    {
        public TestWebDavController(IWebDavContext context, IWebDavDispatcher dispatcher, ILogger<WebDavIndirectResult> responseLogger = null)
            : base(context, dispatcher, responseLogger)
        {
        }
    }
}
