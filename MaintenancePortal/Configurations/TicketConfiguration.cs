using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenancePortal.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        // Enum to string conversion for TicketState
        builder.Property(t => t.State)
            .HasConversion<string>();

        List<Ticket> tickets = new List<Ticket>();
        for (int i = 0; i < 30; i++)
        {
            tickets.Add(new Ticket
            {
                Id = i + 1,
                State = i % 3 == 0 ? TicketState.Open : (i % 3 == 1 ? TicketState.InProgress : TicketState.Closed),
                Title = $"Sample Ticket {i + 1}",
                Description = "This is a sample ticket.",
                CreatedAt = new DateTime(2000, 1, 1),
                CreatedById = "00000000-0000-0000-0000-00000000000"
            });
        }
        builder.HasData(tickets);
    }
}
