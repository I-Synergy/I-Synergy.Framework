Increasing timeout
==================

If you\'re not working on **TCP** but more likely on **HTTP** using a
web api to expose your sync process, you will probably have to face some
issues with timeout.

> ### Note
> Before increasing timeout, be sure you have already setup a
[snapshot](Snapshot.html) for all your new clients.

By default, `Timeout` is fixed to 2 minutes.

To increase the overall timeout, you will have to work on both side:

-   Your web server api project.
-   Your client application.

Server side
-----------

There is no way to increase the `Timeout` period on your web api using
code, with **.Net Core**.

The only solution is to provide a `web.config`, that you add manually to
your project.

> ### Note
> More information here :
> [increase-the-timeout-of-asp-net-core-application](https://medium.com/aspnetcore/increase-the-timeout-of-asp-net-core-application-9a7b4f6deebf)

Here is a `web.config` example where `requestTimeout` is fixed to **20**
minutes:

``` {.sourceCode .xml}
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" 
           modules="AspNetCoreModule" resourceType="Unspecified"/>
    </handlers>
    <aspNetCore requestTimeout="00:20:00"  processPath="%LAUNCHER_PATH%" 
                arguments="%LAUNCHER_ARGS%" stdoutLogEnabled="false" 
                stdoutLogFile=".\logs\stdout" forwardWindowsAuthToken="false"/>
  </system.webServer>
</configuration>
```

Client side
-----------

On the client side, the web orchestrator `WebClientOrchestrator`
instance uses its own `HttpClient` instance unless you specify your own
`HttpClient` instance.

So far, to increase the timeout, you can either:

-   Provide your own [HttpClient]{.title-ref} instance with the
    [Timeout]{.title-ref} property correctly set:

``` {.sourceCode .csharp}
var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };
var client = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(20) };
var clientProvider = new WebClientOrchestrator("http://my.syncapi.com:88/Sync", null, null, client);
```

-   Increase the existing [HttpClient]{.title-ref} instance, created by
    \`WebClientOrchestrator\`:

``` {.sourceCode .csharp}
var clientProvider = new WebClientOrchestrator("http://my.syncapi.com:88/Sync");
clientProvider.HttpClient.Timeout = TimeSpan.FromMinutes(20);
```