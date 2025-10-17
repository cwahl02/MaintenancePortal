using MaintenancePortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaintenancePortal.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
     : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite primary key on ParentGene and SelfGene
        modelBuilder.Entity<Issue>()
            .HasKey(i => new { i.ParentGene, i.SelfGene });

        // Index on ParentGene for faster lookups
        modelBuilder.Entity<Issue>()
            .HasIndex(i => i.ParentGene);

        // Index on SelfGene for faster lookups
        modelBuilder.Entity<Issue>()
            .HasIndex(i => i.SelfGene);

        // Unique constraint on Title and CreatedByUserId
        modelBuilder.Entity<Issue>()
            .HasIndex(i => new { i.Title, i.CreatedByUserId })
            .IsUnique();

        // Configure relationship between Issue and User (CreatedByUser)
        modelBuilder.Entity<Issue>()
            .HasOne(i => i.CreatedByUser)
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete on users
    }
}
