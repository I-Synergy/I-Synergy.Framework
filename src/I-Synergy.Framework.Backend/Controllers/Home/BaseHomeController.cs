using ISynergy.ViewModels.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ISynergy.Controllers.Base
{
    [AllowAnonymous]
    public abstract class BaseHomeController : Controller
    {
        protected IHostingEnvironment _environment;

        public BaseHomeController(IHostingEnvironment environment)
            : base()
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
