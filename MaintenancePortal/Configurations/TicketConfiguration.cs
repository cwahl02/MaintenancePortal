using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net.Sockets;

namespace MaintenancePortal.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        //// Composite primary key on ParentGene and SelfGene
        //builder.HasKey(ticket => new { ticket.ParentId, ticket.Id });

        //// Index on ParentGene for faster lookups
        //builder.HasIndex(iticket => ticket.ParentId);

        //// Index on SelfGene for faster lookups
        //builder.HasIndex(ticket => ticket.Id);

        //// Unique constraint on Title and CreatedByUserId
        //builder
        //    .HasIndex(i => new { i.Title, i.CreatedByUserId })
        //    .IsUnique();

        //// Configure relationship between Issue and User (CreatedByUser)
        //builder
        //    .HasOne(ticket => ticket.CreatedByUser)
        //    .WithMany()
        //    .HasForeignKey(ticket => ticket.CreatedByUserId)
        //    .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete on users
    }
}
