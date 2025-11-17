using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenancePortal.Configurations;

public class TicketLabelConfiguration : IEntityTypeConfiguration<TicketLabels>
{
    public void Configure(EntityTypeBuilder<TicketLabels> builder)
    {
        // Composite Primary Key
        // Helps ensure uniqueness of IssueLabel entries
        builder.HasKey(ticketLabel => new { ticketLabel.Id, ticketLabel.LabelId });

        // Relationship: IssueLabel → Issue
        builder.HasOne(ticketLabel => ticketLabel.Ticket)
            .WithMany(ticket => ticket.TicketLabels)
            .HasForeignKey(ticketLabel => new { ticketLabel.Id })
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: IssueLabel → Label
        builder.HasOne(ticketLabel => ticketLabel.Label)
            .WithMany(ticket => ticket.TicketLabels)
            .HasForeignKey(ticketLabel => ticketLabel.LabelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}