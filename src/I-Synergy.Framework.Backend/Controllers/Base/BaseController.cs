using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace ISynergy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
        }

        public string GetCurrentUser { get => User.Claims.Where(q => q.Type == ClaimTypes.UserNameType).FirstOrDefault().Value; }
    }
}