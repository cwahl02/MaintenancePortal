using MaintenancePortal.Models;
using System.Linq.Expressions;

namespace MaintenancePortal.Interfaces;

public interface ITicketService
{
    Task<Ticket?> CreateTicketAsync(Ticket ticket);
    //Task<Ticket?> GetTicketByIdAsync(ushort id);
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
    Task<Ticket?> UpdateTicketAsync(ushort id, Ticket updatedTicket);
    //Task<bool> DeleteTicketAsync(ushort id);
    //Task<IEnumerable<Ticket>> SerachTicketsAsync(Expression<Func<bool, Ticket>> predicate);
}
