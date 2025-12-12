using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
    public async Task<IActionResult> Index(bool? ticketState = null, int page = 1, int pageSize = 10)
    {
        return View(await GetPaginatedTickets(ticketState, page, pageSize));
    }

    private async Task<TicketPaginationViewModel> GetPaginatedTickets(bool? ticketState = null, int page = 1, int pageSize = 10)
    {
        IQueryable<Ticket> _tickets = _context.Tickets.AsQueryable().OrderByDescending(t => t.Id);

        var totalOpenTickets = _tickets.Count(t => t.IsOpen == true);
        var totalClosedTickets = _tickets.Count(t => t.IsOpen == false);

        if (ticketState != null)
        {
            if (ticketState == true)
            {
                _tickets = (IOrderedQueryable<Ticket>)_tickets.Where(t => t.IsOpen == true);
            }
            if (ticketState == false)
            {
                _tickets = (IOrderedQueryable<Ticket>)_tickets.Where(t => t.IsOpen == false);
            }
        }

        var totalFilteredTickets = await _tickets.CountAsync();

        List<Ticket> tickets = await _tickets
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        TicketPaginationViewModel model = new TicketPaginationViewModel()
        {
            Tickets = tickets.Select(ticket => new TicketIndexViewModel()
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                IsOpen = ticket.IsOpen,
                CreatedAt = ticket.CreatedAt,
                ClosedAt = ticket.ClosedAt,
                UserId = ticket.CreatedById,
                Username = _context.Users.FirstOrDefault(u => u.Id == ticket.CreatedById)?.UserName!
            }).ToList(),
            TicketState = ticketState,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalFilteredTickets / (double)pageSize),
            TotalOpenTickets = totalOpenTickets,
            TotalClosedTickets = totalClosedTickets,
            TotalTickets = await _context.Tickets.CountAsync()
        };

        return model;
    }

    [HttpGet]
    public IActionResult Details(int? id = null)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }

        Ticket? ticket = _context.Tickets.Find(id);
        if (ticket == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return View(new TicketDetailsViewModel()
        {
            Id = ticket.Id,
            IsOpen = ticket.IsOpen,
            Title = ticket.Title,
            Description = ticket.Description,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.LastModifiedAt,
            CanEdit = ticket.CreatedById == currentUserId
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TicketCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            Ticket ticket = new Ticket()
            {
                Title = model.Title,
                Description = model.Description,
                IsOpen = true,
                CreatedAt = DateTime.Now,
                CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var ticket = _context.Tickets.Find(id);
        if (ticket == null)
        {
            return NotFound();
        }

        var viewModel = new TicketEditViewModel
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            IsOpen = ticket.IsOpen
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Update(TicketDetailsViewModel model)
    {
        if (ModelState.IsValid)
        {
            var ticket = _context.Tickets.Find(model.Id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Update ticket with new values
            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.LastModifiedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = ticket.Id });
        }

        return BadRequest();  // Return an error if model validation fails
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
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
    public async Task<IActionResult> CloseOrOpen(int id)
    {
        Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null)
        {
            return NotFound();
        }

        ticket.IsOpen = !ticket.IsOpen;
        ticket.LastModifiedAt = DateTime.Now;
        ticket.ClosedAt = ticket.IsOpen ? null : DateTime.Now;

        _context.Tickets.Update(ticket);

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = ticket.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        return RedirectToAction("Details", new { id = id });
    }
}