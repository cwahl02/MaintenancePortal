using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return View(new TicketEditableViewModel()
        {
            Id = ticket.Id,
            State = ticket.State,
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

    public IActionResult Edit(int id)
    {
        var ticket = _context.Tickets.Find(id);
        if (ticket == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get logged-in user ID

        // If user is not the creator, redirect back to details
        if (ticket.CreatedById != currentUserId)
        {
            return Unauthorized();
        }

        var viewModel = new TicketEditableViewModel
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description
        };

        // Return a partial view with the ticket in edit mode
        return PartialView("_EditTicket", viewModel);
    }

    [HttpPost]
    public IActionResult Update(TicketEditableViewModel model)
    {
        if (ModelState.IsValid)
        {
            var ticket = _context.Tickets.Find(model.Id);
            if (ticket == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get logged-in user ID

            // If user is not the creator, reject the update
            if (ticket.CreatedById != currentUserId)
            {
                return Unauthorized();
            }

            // Update ticket with new values
            ticket.Title = model.Title;
            ticket.Description = model.Description;
            _context.SaveChanges();

            // Return the updated details view as a partial view
            var updatedViewModel = new TicketEditableViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                CanEdit = ticket.CreatedById == currentUserId
            };

            return PartialView("_TicketDetails", updatedViewModel);  // Return updated ticket details as a partial view
        }

        return BadRequest();  // Return an error if model validation fails
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        Ticket? ticket = _context.Tickets.Find(id);
        if (ticket is null)
        {
            return Json(new { success = false });
        }

        _context.Tickets.Remove(ticket);
        _context.SaveChanges();

        return Json(new { success = true, redirectUrl = Url.Action("Index", "Ticket") });
    }
}