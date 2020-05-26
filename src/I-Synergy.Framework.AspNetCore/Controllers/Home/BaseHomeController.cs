using ISynergy.Framework.AspNetCore.ViewModels.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ISynergy.Framework.AspNetCore.Controllers.Base
{
    [AllowAnonymous]
    public abstract class BaseHomeController : Controller
    {
        protected IWebHostEnvironment _environment;

        protected BaseHomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public virtual IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
