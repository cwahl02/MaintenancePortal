using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenancePortal.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        // Composite primary key on ParentGene and SelfGene
        //builder.HasKey(i => new { i.ParentId, i.Id });

        // Index on ParentId for faster lookups
        builder.HasIndex(i => i.ParentId);

        // Index on Id for faster lookups
        builder.HasIndex(i => i.Id);

        // Unique constraint on Title and CreatedByUserId
        builder
            .HasIndex(i => new { i.Title, i.CreatedById })
            .IsUnique();

        // Configure relationship between Ticket and User (CreatedByUser)
        builder
            .HasOne(i => i.CreatedBy)
            .WithMany()
            .HasForeignKey(i => i.CreatedById)
            .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete on users
    }
}
