using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaintenancePortal.Controllers;

[Authorize]
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
        IOrderedQueryable<Ticket> _tickets = _context.Tickets.AsQueryable().OrderByDescending(t => t.Id);

        var totalOpenItems = _tickets.Count(t => t.State == TicketState.Open);
        var totalInProgressItems = _tickets.Count(t => t.State == TicketState.InProgress);
        var totalClosedItems = _tickets.Count(t => t.State == TicketState.Closed);

        if (query != null && query.Contains("state"))
        {
            if (query.Contains("open"))
            {
                _tickets = (IOrderedQueryable<Ticket>)_tickets.Where(t => t.State == TicketState.Open);
            }

            if (query.Contains("inprogress"))
            {
                _tickets = (IOrderedQueryable<Ticket>)_tickets.Where(t => t.State == TicketState.InProgress);
            }

            if (query.Contains("closed"))
            {
                _tickets = (IOrderedQueryable<Ticket>)_tickets.Where(t => t.State == TicketState.Closed);
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
                .Select(ticket => new TicketIndexViewModel
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    State = ticket.State,
                    StateDate = (DateTime)(ticket.State == TicketState.Closed ? ticket.ClosedAt! : ticket.CreatedAt),
                    UserId = ticket.CreatedById,
                    Username = _context.Users
                        .Where(u => u.Id == ticket.CreatedById)
                        .Select(u => u.UserName)
                        .FirstOrDefault() ?? "Unknown",
                })
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
                CreatedById = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id
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

    [HttpPost]
    public async Task<IActionResult> Edit(TicketEditableViewModel model)
    {
        // Log to check if the model is received correctly
        Console.WriteLine($"Ticket ID: {model.Id}, Title: {model.Title}, Description: {model.Description}");
        if (ModelState.IsValid)
        {
            Ticket? ticket = _context.Tickets.Find(model.Id);
            if(ticket != null)
            {
                ticket.Title = model.Title;
                ticket.Description = model.Description;
                
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    updatedTicket = new
                    {
                        Id = ticket.Id,
                        Title = ticket.Title,
                        Description = ticket.Description
                    }
                });
            }
            else
            {
                return Json(new { success = false , message = "Ticket not found." });
            }
        }
        else
        {
            return Json(new { success = false, message = "Error updating ticket." });
        }
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

    [HttpPost]
    public IActionResult Edit4(TicketEditableViewModel model)
    {
        Ticket? ticket = _context.Tickets.Find(model.Id);
        if (ticket == null)
        {
            return NotFound();
        }

        ticket.State = model.State;
        _context.Tickets.Update(ticket);
        _context.SaveChanges();

        var updatedViewModel = new TicketEditableViewModel()
        {
            Id = ticket!.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            State = ticket.State,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.LastModifiedAt
        };
        return Json(ticket);
    }
}