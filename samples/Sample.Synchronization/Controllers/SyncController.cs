using Dotmim.Sync;
using Dotmim.Sync.Web.Server;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Synchronization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly WebServerAgent _webServerAgent;
        private readonly ILogger<SyncController> _logger;

        public SyncController(WebServerAgent webServerAgent, ILogger<SyncController> logger)
        {
            _webServerAgent = webServerAgent;
            _logger = logger;
        }

        /// <summary>
        /// This POST handler is mandatory to handle all the sync process
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task Post() => _webServerAgent.HandleRequestAsync(HttpContext);

        /// <summary>
        /// This GET handler is optional. It allows you to see the configuration hosted on the server
        /// The configuration is shown only if Environmenent == Development
        /// </summary>
        [HttpGet]
        public Task Get() => HttpContext.WriteHelloAsync(_webServerAgent);
    }
}