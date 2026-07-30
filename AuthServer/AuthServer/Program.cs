using AuthServer.Domain.Data_Models;
using AuthServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.Buffering;
using OpenIddict.Abstractions;
using Serilog;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Serilog config
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
//OpenIddict related code 
var authConnectionString = builder.Configuration.GetConnectionString("AuthDb") ?? throw new InvalidOperationException("Connection string 'AuthDb' not found.");

builder.Services.AddOpenIddictConfigurations(authConnectionString);
//OpenIddict related code 

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AuthServerContext>();

builder.Services.AddRazorPages();
builder.Services.AddRouting();

var app = builder.Build();

app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
//app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

//app.UseEndpoints(options =>
//{
//    options.MapControllers();
//    options.MapDefaultControllerRoute();
//});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

#region OpenIddict Test Setup
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthServerContext>();
    await context.Database.EnsureCreatedAsync();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
    if (await manager.FindByClientIdAsync("service-worker") is null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "service-worker",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Authorization,
                Permissions.GrantTypes.ClientCredentials,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.TokenExchange,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.ResponseTypes.Token,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Email,
                Permissions.Prefixes.Scope + "offline_access"
            },
            RedirectUris =
            {
                new Uri("https://localhost:5231")
            }
        });
    }
}
#endregion

app.Run();
