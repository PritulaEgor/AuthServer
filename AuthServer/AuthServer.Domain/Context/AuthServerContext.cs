using AuthServer.Domain.Data_Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AuthServerContext(DbContextOptions<AuthServerContext> options) : IdentityDbContext<ApplicationUser>(options)
{
}
