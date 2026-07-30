using AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

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
                    // Enable authorization endpoint 
                    options.SetAuthorizationEndpointUris("connect/authorize");
                    // Enable the token endpoint.
                    options.SetTokenEndpointUris("connect/token");

                    // Enable user credentials flow
                    options.AllowAuthorizationCodeFlow();
                    // Enable the client credentials flow.
                    options.AllowClientCredentialsFlow();
                    // Enable the refresh token flow.
                    options.AllowRefreshTokenFlow();

                    //PKCE
                    options.RequireProofKeyForCodeExchange();

                    options.RegisterScopes(
                            OpenIddictConstants.Scopes.OpenId,
                            OpenIddictConstants.Scopes.Profile,
                            OpenIddictConstants.Scopes.Email,
                            OpenIddictConstants.Scopes.OfflineAccess
                            );

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                            .EnableAuthorizationEndpointPassthrough()
                            .EnableTokenEndpointPassthrough();
                });
        }
    }
}
