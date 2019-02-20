using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ISynergy.Controllers.Base
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
        }

        public string GetCurrentUser { get => User.Claims.Where(q => q.Type == ClaimTypes.UserNameType).FirstOrDefault().Value; }
    }
}