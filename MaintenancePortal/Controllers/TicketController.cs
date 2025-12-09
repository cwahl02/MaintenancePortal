using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaintenancePortal.Controllers;

public class TicketController : Controller
{
    private readonly AppDbContext _context;

    public TicketController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(string? query = "", int page = 0, int pageSize = 10)
    {
        var _tickets = _context.Tickets.AsQueryable();

        var totalOpenItems = _tickets.Count(t => t.State == TicketState.Open);
        var totalInProgressItems = _tickets.Count(t => t.State == TicketState.InProgress);
        var totalClosedItems = _tickets.Count(t => t.State == TicketState.Closed);

        if (query != null && query.Contains("state"))
        {
            if (query.Contains("open"))
            {
                _tickets = _tickets.Where(t => t.State == TicketState.Open);
            }

            if (query.Contains("inprogress"))
            {
                _tickets = _tickets.Where(t => t.State == TicketState.InProgress);
            }

            if (query.Contains("closed"))
            {
                _tickets = _tickets.Where(t => t.State == TicketState.Closed);
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
            Query = query,
            PaginationMetadata = paginationMetadata,
            Pagination = pagination,
            Tickets = _tickets.Skip(page * pageSize)
                .Take(pageSize)
                .ToList()
        };

        return View(tickets);
    }

    [HttpGet]
    public IActionResult Details(int? id = null)
    {
        if (id == null)
        {
            return NotFound();
        }

        Ticket? ticket = _context.Tickets.Find(id);
        if (ticket == null)
        {
            return NotFound();
        }

        return View(new TicketEditableViewModel()
        {
            Id = ticket.Id,
            State = ticket.State,
            Title = ticket.Title,
            Description = ticket.Description,
            CreatedAt = ticket.CreatedAt,
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TicketModifyViewModel model)
    {
        if (ModelState.IsValid)
        {
            Ticket ticket = new Ticket()
            {
                Title = model.Title,
                Description = model.Description,
                State = TicketState.Open,
                CreatedAt = DateTime.Now,
                CreatedById = "system" // Placeholder for the user ID
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(model);
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
    public IActionResult Edit(int id, TicketEditableViewModel model)
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

        return RedirectToAction("Details", new { Id = model.Id });
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