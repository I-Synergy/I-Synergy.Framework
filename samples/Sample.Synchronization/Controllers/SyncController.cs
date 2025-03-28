using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Dotmim.Sync.Web.Server;
using ISynergy.Framework.Core.Serializers;
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

        [HttpPost("v1/deprovision")]
        public async Task<ActionResult> DeprovisionScopeV1()
        {
            var remoteOrchestrator = _webServerAgent.RemoteOrchestrator;
            var connectionString = remoteOrchestrator.Provider.ConnectionString;
            remoteOrchestrator.Provider = new SqlSyncProvider(connectionString);

            // Deprovision everything
            var provisioning =
                SyncProvision.ScopeInfo |
                SyncProvision.ScopeInfoClient |
                SyncProvision.StoredProcedures |
                SyncProvision.TrackingTable |
                SyncProvision.Triggers;

            var scopeName = "DefaultScope";

            // Deprovision everything
            var result = await remoteOrchestrator.DeprovisionAsync(scopeName, provisioning);

            return result ? Ok() : BadRequest();
        }

        [HttpPost("v1/provision")]
        public async Task<ActionResult> ProvisionScopeV1()
        {
            var remoteOrchestrator = _webServerAgent.RemoteOrchestrator;
            var connectionString = remoteOrchestrator.Provider.ConnectionString;
            remoteOrchestrator.Provider = new SqlSyncProvider(connectionString);

            var setup = new SyncSetup(new[] {
            "Address",
            "Customer",
            "CustomerAddress",
            "ProductCategory",
            "ProductModel",
            "ProductDescription",
            "Product",
            "ProductModelProductDescription"
            });

            var result = await remoteOrchestrator.ProvisionAsync(setup);
            var options = DefaultJsonSerializers.Default;
            return result is not null ? new JsonResult(result, options) : BadRequest();
        }

        [HttpPost("v2/deprovision")]
        public async Task<ActionResult> DeprovisionScopeV2()
        {
            var remoteOrchestrator = _webServerAgent.RemoteOrchestrator;

            // Deprovision everything
            var provisioning =
                SyncProvision.ScopeInfo |
                SyncProvision.ScopeInfoClient |
                SyncProvision.StoredProcedures;

            var scopeName = "DefaultScope";

            // Deprovision everything
            var result = await remoteOrchestrator.DeprovisionAsync(scopeName, provisioning);

            return result ? Ok() : BadRequest();
        }

        [HttpPost("v2/provision")]
        public async Task<ActionResult> ProvisionScopeV2()
        {
            var remoteOrchestrator = _webServerAgent.RemoteOrchestrator;
            var setup = new SyncSetup(new[] {
            "Address",
            "Customer",
            "CustomerAddress",
            "ProductCategory",
            "ProductModel",
            "ProductDescription",
            "Product",
            "ProductModelProductDescription"
            });

            var result = await remoteOrchestrator.ProvisionAsync(setup);
            var options = DefaultJsonSerializers.Default;
            return result is not null ? new JsonResult(result, options) : BadRequest();
        }
    }
}