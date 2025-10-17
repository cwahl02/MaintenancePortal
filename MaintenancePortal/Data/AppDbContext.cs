using MaintenancePortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaintenancePortal.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
     : IdentityDbContext<User>(options)
{
    
}
