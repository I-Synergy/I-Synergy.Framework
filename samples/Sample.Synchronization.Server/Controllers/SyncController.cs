using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sample.Synchronization.Common.Options;

namespace Sample.Synchronization.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly WebServerOrchestrator _orchestrator;
        private readonly ServerSynchronizationOptions _options;
        private readonly ILogger _logger;

        // Injected thanks to Dependency Injection
        public SyncController(
            WebServerOrchestrator webServerOrchestrator,
            IOptions<ServerSynchronizationOptions> options,
            ILogger<SyncController> logger)
        {
            _orchestrator = webServerOrchestrator;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// This POST handler is mandatory to handle all the sync process.
        /// The request size limit is set to 50Mb
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(52428800)]
        public Task Post()
            => _orchestrator.HandleRequestAsync(HttpContext);

        /// <summary>
        /// This GET handler is optional. It allows you to see the configuration hosted on the server
        /// The configuration is shown only if Environmenent == Development
        /// </summary>
        [HttpGet]
        public Task Get()
            => WebServerOrchestrator.WriteHelloAsync(HttpContext, _orchestrator);

        /// <summary>
        /// Retrieves the remote folder for the client.
        /// </summary>
        /// <returns></returns>
        [HttpGet("files/folder")]
        public string GetRemoteFolder() 
            => _orchestrator.GetRemoteFullPath(_options.SynchronizationFolderPath);

        /// <summary>
        /// Gets a list of files in the remote folder.
        /// </summary>
        /// <returns></returns>
        [HttpGet("files/list")]
        public List<FileInfoMetadata> GetRemoteFiles()
            => _orchestrator.GetRemoteFiles(_options.SynchronizationFolderPath);

        /// <summary>
        /// Downloads file by it's full name.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("files/download")]
        public IActionResult DownloadFile(string path) 
            => PhysicalFile(path, "application/octet-stream", Path.GetFileName(path));
    }
}
