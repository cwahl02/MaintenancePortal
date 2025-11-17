using MaintenancePortal.Data;
using MaintenancePortal.Interfaces;
using MaintenancePortal.Models;
using MaintenancePortal.Repository;
using System.Linq.Expressions;

namespace MaintenancePortal.Services;

public class TicketService : ITicketService
{
    private readonly DataAccessor _repo;

    public TicketService(IDataAccessor repo)
    {
        _repo = (DataAccessor?)repo!;
    }

    public Task<Ticket?> CreateTicketAsync(Ticket ticket) => _repo.CreateAsync<Ticket>(ticket);
    //public Task<Ticket?> GetTicketByIdAsync(ushort id);
    public Task<IEnumerable<Ticket>> GetAllTicketsAsync() => _repo.GetAllAsync<Ticket>();
    public Task<Ticket?> UpdateTicketAsync(ushort id, Ticket updatedTicket) => _repo.UpdateAsync<Ticket>(updatedTicket);
    //Task<bool> DeleteTicketAsync(ushort id) => _repo.DeleteAsync<Ticket>(id);
    //Task<IEnumerable<Ticket>> SerachTicketsAsync(Expression<Func<bool, Ticket>> predicate) => new List<Ticket>();
}
