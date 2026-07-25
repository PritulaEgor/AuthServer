using AuthServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Extensions
{
    public static class OpenIddict
    {
        public static void AddOpenIddictConfigurations(this IServiceCollection services, string authConnectionString)
        {
            services.AddDbContext<AuthServerContext>(options =>
            {
                options.UseSqlServer(authConnectionString);

                options.UseOpenIddict();
            });

            services
                .AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                   .UseDbContext<AuthServerContext>();
                });

            // Register the OpenIddict server components.
            services
                .AddOpenIddict()
                .AddServer(options =>
                {
                    // Enable the token endpoint.
                    options.SetTokenEndpointUris("connect/token");

                    // Enable the client credentials flow.
                    options.AllowClientCredentialsFlow();

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough();
                });
        }
    }
}
