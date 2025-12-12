using MaintenancePortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaintenancePortal.Data;

/// <summary>
/// Represents the Entity Framework Core database context for the application, providing access to user identity and
/// ticket data.
/// </summary>
/// <remarks>This context extends IdentityDbContext to integrate ASP.NET Core Identity with application-specific
/// entities. Use this context to query and save instances of user and ticket entities. The model is configured using
/// all IEntityTypeConfiguration implementations found in the assembly.</remarks>
/// <param name="options">The options to be used by the context. Must not be null.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options)
     : IdentityDbContext<User>(options)
{
    public DbSet<Ticket> Tickets { get; set; }
}
