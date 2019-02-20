using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using ISynergy.Attributes;
using ISynergy.ViewModels.Authorization;
using ISynergy.ViewModels.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Controllers.Base
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseAuthorizationController : Controller
    {
        public IServiceProvider _serviceProvider { get; internal set; }
        public OpenIddictApplicationManager<OpenIddictApplication> _appManager { get; internal set; }
        public IOptions<IdentityOptions> _identityOptions { get; internal set; }

        protected BaseAuthorizationController(
            IServiceProvider services,
            OpenIddictApplicationManager<OpenIddictApplication> appManager,
            IOptions<IdentityOptions> identityOptions)
        {
            _serviceProvider = services;
            _appManager = appManager;
            _identityOptions = identityOptions;
        }

        #region Authorization code, implicit and implicit flows

        // Note: to support interactive flows like the code flow,
        // you must provide your own authorization endpoint action:

        [HttpGet("~/oauth/authorize")]
        public async Task<IActionResult> Authorize(OpenIdConnectRequest request, CancellationToken cancellationToken = default)
        {
            Debug.Assert(request.IsAuthorizationRequest(),
                "The OpenIddict binder for ASP.NET Core MVC is not registered. " +
                "Make sure services.AddOpenIddict().AddMvcBinders() is correctly called.");

            // Retrieve the application details from the database.
            var application = await _appManager.FindByClientIdAsync(request.ClientId, cancellationToken);
            if (application is null)
            {
                return View("Error", new ErrorViewModel
                {
                    Error = OpenIdConnectConstants.Errors.InvalidClient,
                    ErrorDescription = "Details concerning the calling client application cannot be found in the database"
                });
            }

            // Flow the request_id to allow OpenIddict to restore
            // the original authorization request from the cache.
            return View(new AuthorizeViewModel
            {
                ApplicationName = application.DisplayName,
                RequestId = request.RequestId,
                Scope = request.Scope
            });
        }

        [FormValueRequired("submit.Accept")]
        [HttpPost("~/oauth/authorize"), ValidateAntiForgeryToken]
        public abstract Task<IActionResult> Accept(OpenIdConnectRequest request, CancellationToken cancellationToken = default);

        [FormValueRequired("submit.Deny")]
        [HttpPost("~/oauth/authorize"), ValidateAntiForgeryToken]
        public IActionResult Deny(CancellationToken cancellationToken = default)
        {
            // Notify OpenIddict that the authorization grant has been denied by the resource owner
            // to redirect the user agent to the client application using the appropriate response_mode.
            return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
        }

        // Note: the logout action is only useful when implementing interactive
        // flows like the authorization code flow or the implicit flow.
        [AllowAnonymous]
        [HttpGet("~/oauth/logout")]
        public IActionResult Logout(OpenIdConnectRequest request, CancellationToken cancellationToken = default)
        {
            Debug.Assert(request.IsLogoutRequest(),
                "The OpenIddict binder for ASP.NET Core MVC is not registered. " +
                "Make sure services.AddOpenIddict().AddMvcBinders() is correctly called.");

            // Flow the request_id to allow OpenIddict to restore
            // the original logout request from the distributed cache.
            return View(new LogoutViewModel
            {
                RequestId = request.RequestId
            });
        }

        [AllowAnonymous]
        [HttpPost("~/oauth/logout"), ValidateAntiForgeryToken]
        public abstract Task<IActionResult> Logout(CancellationToken cancellationToken = default);
        #endregion Authorization code, implicit and implicit flows

        #region Password, authorization code and refresh token flows

        // Note: to support non-interactive flows like password,
        // you must provide your own token endpoint action:
        [AllowAnonymous]
        [HttpPost("~/oauth/token"), Produces("application/json")]
        public abstract Task<IActionResult> Exchange(OpenIdConnectRequest request, CancellationToken cancellationToken = default);
        #endregion Password, authorization code and refresh token flows

        [AllowAnonymous]
        [HttpGet("~/maintenance/pruneinvalidrecords")]
        public async Task PruneInvalidRecords(CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                // Note: this task is responsible of automatically removing orphaned tokens/authorizations
                // (i.e tokens that are no longer valid and ad-hoc authorizations that have no valid tokens associated).
                // Since ad-hoc authorizations and their associated tokens are removed as part of the same operation
                // when they no longer have any token attached, it's more efficient to remove the authorizations first.

                // Note: the authorization/token managers MUST be resolved from the scoped provider
                // as they depend on scoped stores that should be disposed as soon as possible.

                try
                {
                    var tokenManager = scope.ServiceProvider.GetRequiredService<OpenIddictTokenManager<OpenIddictToken>>();
                    if (tokenManager != null) await tokenManager.PruneAsync(cancellationToken);
                }
                catch (Exception) { }

                try
                {
                    var authorizationManager = scope.ServiceProvider.GetRequiredService<OpenIddictAuthorizationManager<OpenIddictAuthorization>>();
                    if (authorizationManager != null) await authorizationManager.PruneAsync(cancellationToken);
                }
                catch (Exception) { }
            }
        }
    }
}
