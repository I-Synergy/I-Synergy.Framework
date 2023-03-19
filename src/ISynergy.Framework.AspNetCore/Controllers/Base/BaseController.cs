using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISynergy.Framework.AspNetCore.Controllers.Base
{
    /// <summary>
    /// Class BaseController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        protected BaseController()
        {
        }

        /// <summary>
        /// Gets the get current user.
        /// </summary>
        /// <value>The get current user.</value>
        public string GetCurrentUser { get => User.Claims.FirstOrDefault(q => q.Type == Core.Constants.ClaimTypes.UserNameType)?.Value ?? string.Empty; }
    }
}
