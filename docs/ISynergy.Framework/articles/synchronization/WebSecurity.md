ASP.NET Core Web Authentication
===============================

Overview
--------

The `I-Synergy.Framework.AspNetCore.Synchronization` package used to expose `the synchronization framework` through
**ASP.Net Core Web Api** is *just* a wrapper using the web `HttpContext`
object to figure out what should be done, internally.

> ### Hint
> You will find the auth sample here : [Web Authentication
Sample](https://github.com/I-Synergy/I-Synergy.Framework/blob/master/Samples/HelloWebAuthSync)

Just as a remember, the **Web Server** code looks like this:

``` {.sourceCode .csharp}
[Route("api/[controller]")]
[ApiController]
public class SyncController : ControllerBase
{
    private WebServerOrchestrator orchestrator;

    public SyncController(WebServerOrchestrator webServerOrchestrator) 
        => this.orchestrator = webServerOrchestrator;

    [HttpPost]
    public Task Post() 
        => orchestrator.HandleRequestAsync(this.HttpContext);
}
```

As you can see, we are completely integrated within the **ASP.Net Core**
architecture. So far, protecting our API is just like protecting any
kind of ASP.NET Core Api.

If you want to rely on a strong **OAUTH2** / **OpenID Connect**
provider, please read:

-   Microsoft : [Mobile application calling a secure Web Api, using
    Azure
    AD](https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-mobile-overview)
-   AWS : [Securing a Web API using AWS
    Cognito](https://referbruv.com/blog/posts/securing-aspnet-core-apis-with-jwt-bearer-using-aws-cognito)
-   Google : [OAUTH2 with Google
    APIS](https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth)
-   Identity Server : [Protecting an API using Identity
    Server](https://identityserver4.readthedocs.io/en/latest/topics/apis.html)

`the synchronization framework` relies on the ASP.NET Core Web Api architecture. So far, you can
secure [the synchronization framework]{.title-ref} like you\'re securing any kind of exposed Web
API:

-   Configuring the controller
-   Configuring the identity provider protocol
-   Calling the controller with an authenticated client, using a bearer
    token

> ### Note
> More information about ASP.Net Core Authentication here : [Overview of
ASP.NET Core
authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication)

Server side
-----------

We are going to use a **Bearer token** validation on the server side:

-   **Unsecure** but easier: Using an hard coded bearer token (Do not
    use this technic in production)
-   **Secured** but relying on an external token provider: Using for
    example [Azure Active Directory
    Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/azure-active-directory/).

### Configuration

You need to configure your Web API project to be able to secure any
controller.

> In your `Startup.cs`, you should add authentication services, with JWT
  Bearer protection.
> It involves using
  `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>{})`

Here is a quick sample, **without** relying on any external cloud
identity provider (once again, **DON\'T** do that in production, it\'s
**INSECURE** and just here for the sake of explanation)

``` {.sourceCode .csharp}
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddDistributedMemoryCache();
    services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

    // Adding a default authentication system
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                ValidIssuer = "I-Synergy.Framework.Synchronization.Bearer",
                ValidAudience = "I-Synergy.Framework.Synchronization.Bearer",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RANDOM_KEY"))
            });

    // [Required]: Get a connection string to your server data source
    var connectionString = Configuration.GetSection("ConnectionStrings")["SqlConnection"];

    // [Required] Tables involved in the sync process:
    var tables = new string[] {"ProductCategory", "ProductModel", "Product",
        "Address", "Customer", "CustomerAddress", "SalesOrderHeader", "SalesOrderDetail" };

    // [Required]: Add a SqlSyncProvider acting as the server hub.
    services.AddSyncServer<SqlSyncProvider>(connectionString, tables);
}
```

As an example, if you\'re using **Azure AD** authentication, your code
should be more like:

``` {.sourceCode .csharp}
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    // [Required]: Handling multiple sessions
    services.AddDistributedMemoryCache();
    services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

    // Using Azure AD Authentication
    services.AddMicrosoftIdentityWebApiAuthentication(Configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

    // [Required]: Get a connection string to your server data source
    var connectionString = Configuration.GetSection("ConnectionStrings")["SqlConnection"];

    // [Required] Tables involved in the sync process:
    var tables = new string[] {"ProductCategory", "ProductModel", "Product",
        "Address", "Customer", "CustomerAddress", "SalesOrderHeader", "SalesOrderDetail" };

    // [Required]: Add a SqlSyncProvider acting as the server hub.
    services.AddSyncServer<SqlSyncProvider>(connectionString, tables);
}
```

> ### Note
> More on Code Configuration
[Here](https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-app-configuration).

Finally, do not forget to add the **Authentication Middlewares** (and
Session Middleware) as well:

``` {.sourceCode .csharp}
// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

### Securing the controller

This part is the most easier one. Yo can choose to secure all the
controller, using the `[Authorize]` attribute on the class itself, or
you can use either `[Authorize]` / `[AllowAnonymous]` on each controller
methods:

The simplest controller could be written like this, using the
`[Authorize]` attribute:

``` {.sourceCode .csharp}
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private WebServerOrchestrator orchestrator;

    public SyncController(WebServerOrchestrator webServerOrchestrator) 
        => this.orchestrator = webServerOrchestrator;

    [HttpPost]
    public Task Post() 
        => orchestrator.HandleRequestAsync(this.HttpContext);
}
```

Maybe you\'ll need to expose the `GET` method to see the server
configuration. In that particular case, we can use both `[Authorize]`
and `[AllowAnonymous]`:

``` {.sourceCode .csharp}
[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private WebServerOrchestrator orchestrator;

    public SyncController(WebServerOrchestrator webServerOrchestrator) 
        => this.orchestrator = webServerOrchestrator;

    [HttpPost]
    [Authorize]
    public async Task Post() => orchestrator.HandleRequestAsync(this.HttpContext);

    [HttpGet]
    [AllowAnonymous]
    public Task Get() => WebServerOrchestrator.WriteHelloAsync(this.HttpContext, orchestrator);

}
```

And eventually, you can even have more control, using the `HttpContext`
instance, from within your `POST` handler:

``` {.sourceCode .csharp}
[HttpPost]
public async Task Post()
{
    // If you are using the [Authorize] attribute you don't need to check
    // the User.Identity.IsAuthenticated value
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        this.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    // using scope and even claims, you can have more grain control on your authenticated user
    string scope = (User.FindFirst("http://schemas.microsoft.com/identity/claims/scope"))?.Value;
    string user = (User.FindFirst(ClaimTypes.NameIdentifier))?.Value;
    if (scope != "access_as_user")
    {
        this.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    await orchestrator.HandleRequestAsync(this.HttpContext);
}
```

Client side
-----------

From you mobile / console / desktop application, you just need to send
your **Bearer Token** embedded into your [HttpClient]{.title-ref}
headers.

The `WebClientOrchestrator` object allows you to use your own
`HttpClient` instance. So far, create an instance and add your bearer
token to the `DefaultRequestHeaders.Authorization` property.

``` {.sourceCode .csharp}
// Getting a JWT token
// You should get a Jwt Token from an identity provider like Azure, Google, AWS or other.
var token = GenerateJwtToken(...);

HttpClient httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

// Adding the HttpClient instance to the web client orchestrator
var serverOrchestrator = new WebClientOrchestrator(
                "https://localhost:44342/api/sync", client:httpClient);

var clientProvider = new SqlSyncProvider(clientConnectionString);
var agent = new SyncAgent(clientProvider, serverOrchestrator);

var result = await agent.SynchronizeAsync();
```

### Xamaring sample

> ### Note
> More on mobile token acquisition : [Acquire token from mobile
application](https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-mobile-acquire-token)

> MSAL allows apps to acquire tokens silently and interactively.
> When you call `AcquireTokenSilent()` or `AcquireTokenInteractive()`,
  MSAL returns an access token for the requested scopes.
> The correct pattern is to make a silent request and then fall back to
  an interactive request.

``` {.sourceCode .csharp}
string[] scopes = new string[] {"user.read"};
var app = PublicClientApplicationBuilder.Create(clientId).Build();
var accounts = await app.GetAccountsAsync();

AuthenticationResult result;
try
{
    result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
}
catch(MsalUiRequiredException)
{
    result = await app.AcquireTokenInteractive(scopes)
                .ExecuteAsync();
}
```
