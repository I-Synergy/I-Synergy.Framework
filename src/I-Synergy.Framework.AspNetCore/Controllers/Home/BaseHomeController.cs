using ISynergy.Framework.AspNetCore.ViewModels.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ISynergy.Framework.AspNetCore.Controllers.Base
{
    /// <summary>
    /// Class BaseHomeController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    [AllowAnonymous]
    public abstract class BaseHomeController : Controller
    {
        /// <summary>
        /// The environment
        /// </summary>
        protected IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHomeController"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        protected BaseHomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [AllowAnonymous]
        public virtual IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Errors this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public virtual IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
