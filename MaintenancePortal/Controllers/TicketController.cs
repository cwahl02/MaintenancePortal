using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaintenancePortal.Controllers;

[Route("tickets")]
public class TicketController : Controller
{
    private readonly AppDbContext _context;

    public TicketController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index(string? query = "", int page = 0, int pageSize = 10)
    {
        var _tickets = _context.Tickets.AsQueryable();

        var totalOpenItems = _tickets.Count(t => t.State == "Open");
        var totalInProgressItems = _tickets.Count(t => t.State == "In Progress");
        var totalClosedItems = _tickets.Count(t => t.State == "Closed");

        if (query != null && query.Contains("state"))
        {
            if (query.Contains("open"))
            {
                _tickets = _tickets.Where(t => t.State == "Open");
            }

            if (query.Contains("inprogress"))
            {
                _tickets = _tickets.Where(t => t.State == "In Progress");
            }

            if (query.Contains("closed"))
            {
                _tickets = _tickets.Where(t => t.State == "Closed");
            }
        }

        var paginationMetadata = new PaginationMetadata()
        {
            PageSize = pageSize,
            Current = page,
            TotalItems = _tickets.Count(),
            TotalOpenItems = totalOpenItems,
            TotalInProgressItems = totalInProgressItems,
            TotalClosedItems = totalClosedItems
        };

        var pagination = paginationMetadata.GetPageList();

        TicketListViewModel tickets = new TicketListViewModel
        {
            PaginationMetadata = paginationMetadata,
            Pagination = pagination,
            Tickets = _tickets.Skip(page * pageSize)
                .Take(pageSize)
                .ToList()
        };

        return View(tickets);
    }

    [HttpGet("new")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("new")]
    public IActionResult Create(TicketModifyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Ticket newTicket = new Ticket()
        {
            Title = model.Title,
            Description = model.Description,
            State = "Open",
            CreatedAt = DateTime.UtcNow
        };

        _context.Tickets.Add(newTicket);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpGet("edit/{id}")]
    public IActionResult Edit(int id)
    {
        Ticket? ticket = _context.Tickets.Find(id);
        if (ticket is null)
        {
            return NotFound();
        }
        TicketModifyViewModel model = new TicketModifyViewModel
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description
        };
        return View(model);
    }

    [HttpPost("edit/{id}")]
    public IActionResult Edit(int id, TicketModifyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Ticket? ticket = _context.Tickets.Find(id);

        if (ticket is null)
        {
            return NotFound();
        }

        ticket.Title = model.Title ?? ticket.Title;
        ticket.Description = model.Description ?? ticket.Description;

        _context.Tickets.Update(ticket);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        Ticket? ticket = _context.Tickets.Find(id);
        if (ticket is null)
        {
            return NotFound();
        }

        _context.Tickets.Remove(ticket);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}