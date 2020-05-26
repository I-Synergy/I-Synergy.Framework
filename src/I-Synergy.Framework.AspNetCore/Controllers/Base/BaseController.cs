using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ISynergy.Framework.AspNetCore.Controllers.Base
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
        }

        public string GetCurrentUser { get => User.Claims.Where(q => q.Type == Core.Constants.ClaimTypes.UserNameType).FirstOrDefault()?.Value ?? string.Empty; }
    }
}