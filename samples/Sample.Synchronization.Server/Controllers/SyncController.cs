using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Synchronization.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly WebServerOrchestrator _orchestrator;
        private readonly ILogger _logger;

        // Injected thanks to Dependency Injection
        public SyncController(WebServerOrchestrator webServerOrchestrator, ILogger<SyncController> logger)
        {
            _orchestrator = webServerOrchestrator;
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
    }
}
