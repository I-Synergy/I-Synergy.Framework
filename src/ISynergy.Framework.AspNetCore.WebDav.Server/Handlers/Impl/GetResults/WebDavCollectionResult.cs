using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers.Impl.GetResults
{
    internal class WebDavCollectionResult : WebDavResult
    {
        private readonly ICollection _collection;

        public WebDavCollectionResult(ICollection collection)
            : base(WebDavStatusCode.OK)
        {
            _collection = collection;
        }

        public override async Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            await base.ExecuteResultAsync(response, ct);
            response.Headers["Last-Modified"] = new[] { _collection.LastWriteTimeUtc.ToString("R") };
        }
    }
}
