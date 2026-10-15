using Microsoft.Extensions.Hosting;
using Sample.EventSourcing.Api.Services;

namespace Sample.EventSourcing.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for minting development-only JWT tokens.
/// </summary>
public static class DevTokenEndpoints
{
    /// <summary>Registers development-only token endpoints on <paramref name="app"/>.</summary>
    public static IEndpointRouteBuilder MapDevTokenEndpoints(this IEndpointRouteBuilder app)
    {
        var env = app.ServiceProvider.GetRequiredService<IHostEnvironment>();

        // POST /dev/token — only exposed in Development.
        // This anonymous endpoint mints a JWT for ANY tenant on purpose: it is a local stand-in
        // for an identity provider so the sample can be exercised without a real IdP. It signs with
        // the same key the API validates against, so the returned token authenticates successfully.
        if (env.IsDevelopment())
        {
            app.MapPost("/dev/token", (
                DevTokenRequest request,
                DevTokenIssuer issuer) =>
            {
                var lifetime = TimeSpan.FromHours(1);
                var token = issuer.CreateToken(request.TenantId, request.Username, lifetime);
                return Results.Ok(new DevTokenResponse(token, "Bearer", (int)lifetime.TotalSeconds));
            })
            .AllowAnonymous()
            .WithName("DevToken")
            .WithTags("Dev")
            .WithSummary("Mint a development JWT (Development only)")
            .WithDescription(
                "Development-only stand-in for an identity provider. Mints a signed JWT for the " +
                "supplied tenant and user, intended solely for local testing. This endpoint is " +
                "anonymous by design and is not exposed in production builds.")
            .Produces<DevTokenResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem();
        }

        return app;
    }
}

/// <summary>Request body for <c>POST /dev/token</c>.</summary>
/// <param name="TenantId">Tenant to mint the token for (written to the <c>tenant</c> claim).</param>
/// <param name="Username">User name (written to the <c>username</c> and <c>sub</c> claims). Defaults to <c>demo-user</c>.</param>
public sealed record DevTokenRequest(Guid TenantId, string? Username);

/// <summary>Response body for <c>POST /dev/token</c>.</summary>
/// <param name="AccessToken">The signed JWT.</param>
/// <param name="TokenType">Token scheme, always <c>Bearer</c>.</param>
/// <param name="ExpiresIn">Token lifetime in seconds.</param>
public sealed record DevTokenResponse(string AccessToken, string TokenType, int ExpiresIn);
