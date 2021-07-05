# Jaahas.AspNetCore.LocalhostRun

A library for simplifying integration between ASP.NET Core applications and [localhost.run](https://localhost.run/).


# Getting Started

Add the [Jaahas.AspNetCore.LocalhostRun](https://www.nuget.org/packages/Jaahas.AspNetCore.LocalhostRun) NuGet package to your application.

Add the localhost.run configuration to your application services:

```csharp
services.AddLocalhostRunIntegration();
```

Add the forwarded headers and HTTPS redirection middlewares to your application pipeline:

```
if (env.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseForwardedHeaders();
}
else {
    app.UseExceptionHandler("/Home/Error");
    app.UseForwardedHeaders();
    app.UseHsts();
}

app.UseHttpsRedirection();

// Remaining configuration removed
```


# Details

[localhost.run](https://localhost.run/) is an excellent service for creating internet-facing tunnels to web applications running on `localhost`, acting as a reverse proxy that can serve your application over HTTP/80 and HTTPS/443. However, due to the way that `localhost.run` adds the [X-Forwarded-* headers to proxied requests](https://localhost.run/docs/http-tunnels#proxy-headers), ASP.NET Core's forwarded headers middleware requires some additional configuration.

The `AddLocalhostRunIntegration` extension method used in the example above performs the following actions:

- Configures the [ForwardedHeadersOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.forwardedheadersoptions) for the application to configure ASP.NET Core to use proxy headers named `X_Forwarded_For`, `X_Forwarded_Host` and `X_Forwarded_Proto` (instead of the standard `X-Forwarded-For`, `X-Forwarded-Host` and `X-Forwarded-Proto` names).
- Configures `ForwardedHeadersOptions` so that ASP.NET Core will only process the `X_Forwarded_For` and `X_Forwarded_Proto` headers by default.
- Clears the `KnownNetworks` and `KnownProxies` lists on the `ForwardedHeadersOptions` as per [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#forward-the-scheme-for-linux-and-non-iis-reverse-proxies) to remove the default restrictions that only allow loopback proxies.
- Configures the application's HTTPS redirection policy to redirect non-HTTPS requests to port 443 via an HTTP 307/Temporary Redirect response.


## Automatic Forwarded Headers Configuration

When your application is started with the `ForwardedHeaders:Enabled` setting set to `true` (e.g. via the `ASPNETCORE_FORWARDEDHEADERS_ENABLED` environment variable) and you use `WebHost.CreateDefaultBuilder` to create your web host builder, it is not necessary to add the forwarded headers middleware to your application pipeline; ASP.NET Core will add it automatically.

However, the HTTPS redirection middleware must always be manually added to the application if you require it.



# Building the Solution

The repository uses [Cake](https://cakebuild.net/) for cross-platform build automation. The build script allows for metadata such as a build counter to be specified when called by a continuous integration system such as TeamCity.

A build can be run from the command line using the [build.ps1](/build.ps1) PowerShell script or the [build.sh](/build.sh) Bash script. For documentation about the available build script parameters, see [build.cake](/build.cake).
