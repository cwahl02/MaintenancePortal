using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net.Sockets;

namespace MaintenancePortal.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        List<Ticket> tickets = new List<Ticket>();
        for (int i = 0; i < 30; i++)
        {
            tickets.Add(new Ticket
            {
                Id = i + 1,
                State = i % 3 == 0 ? "Open" : (i % 3 == 1 ? "In Progress" : "Closed"),
                Title = $"Sample Ticket {i + 1}",
                Description = "This is a sample ticket.",
                CreatedAt = new DateTime(2000, 1, 1),
                UpdatedAt = new DateTime(2000, 1, 1)
            });
        }
        builder.HasData(tickets);
    }
}
