using System;
using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// ASP.NET Core extensions for integration with localhost.run.
    /// </summary>
    public static class LocalhostRunExtensions {

        /// <summary>
        /// Configures forwarded headers and HTTPS redirection options for use with localhost.run.
        /// </summary>
        /// <param name="services">
        ///   The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddLocalhostRunIntegration(this IServiceCollection services) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            services.Configure<ForwardedHeadersOptions>(options => {
                // Clear known networks and proxies since default behaviour only allows loopback
                // proxies.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();

                // Localhost.run uses underscores in its proxy headers rather than dashes.
                options.ForwardedForHeaderName = "X_Forwarded_For";
                options.ForwardedHostHeaderName = "X_Forwarded_Host";
                options.ForwardedProtoHeaderName = "X_Forwarded_Proto";

                // Do not use X-Forwarded-Host by default. See here for details: https://github.com/aspnet/Announcements/issues/295
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });


            services.AddHttpsRedirection(options => {
                // The localhost.run web server allows HTTP requests on port 80 and HTTPS requests
                // on port 443. If HTTPS redirection is required, we need to ensure that the
                // correct external port is used.
                options.RedirectStatusCode = (int) HttpStatusCode.TemporaryRedirect;
                options.HttpsPort = 443;
            });

            return services;
        }

    }
}
