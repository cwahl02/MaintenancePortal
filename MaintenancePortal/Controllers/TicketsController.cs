using MaintenancePortal.Models;
using MaintenancePortal.Repository;
using Microsoft.AspNetCore.Mvc;

namespace MaintenancePortal.Controllers;

public class TicketsController : Controller
{
    private readonly DataAccessor _repo;

    public TicketsController(IDataAccessor factory)
    {
    }
    public IActionResult Index()
    {
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            UserName = "johndoe"
        };

        var tickets = new List<Ticket>
        {
            new Ticket
            {
                Title = "Leaky Faucet",
                Description = "The kitchen faucet is leaking.",
                CreatedBy = user,
                CreatedById = user.Id,
                CreatedAt = DateTime.Now.AddDays(-2),
                Status = true
            },
            new Ticket
            {
                Title = "Broken Window",
                Description = "The living room window is broken.",
                CreatedBy = user,
                CreatedById = user.Id,
                CreatedAt = DateTime.Now.AddDays(-1),
                Status = true
            }
        };

        return View(tickets);
    }

    public IActionResult New()
    {
        return View();
    }
}
