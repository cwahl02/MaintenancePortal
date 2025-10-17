using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace MaintenancePortal.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        // Composite primary key on ParentGene and SelfGene
        builder.HasKey(i => new { i.ParentGene, i.SelfGene });

        // Index on ParentGene for faster lookups
        builder.HasIndex(i => i.ParentGene);

        // Index on SelfGene for faster lookups
        builder.HasIndex(i => i.SelfGene);

        // Unique constraint on Title and CreatedByUserId
        builder
            .HasIndex(i => new { i.Title, i.CreatedByUserId })
            .IsUnique();

        // Configure relationship between Issue and User (CreatedByUser)
        builder
            .HasOne(i => i.CreatedByUser)
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete on users
    }
}
