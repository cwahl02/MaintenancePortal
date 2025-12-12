using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MaintenancePortal.Controllers;

/// <summary>
/// Provides actions for creating, viewing, editing, and managing support tickets within the application. Only
/// authorized users can access these endpoints.
/// </summary>
/// <remarks>This controller supports listing tickets with pagination and filtering, viewing ticket details,
/// creating new tickets, editing existing tickets, deleting tickets, and toggling the open or closed state of a ticket.
/// All actions require the user to be authenticated. The controller relies on dependency injection for database access
/// and uses view models to transfer data between the controller and views.</remarks>
[Authorize]
public class TicketController : Controller
{
    private readonly AppDbContext _context;

    public TicketController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles HTTP GET requests to display a paginated list of tickets, optionally filtered by ticket state.
    /// </summary>
    /// <param name="ticketState">An optional value indicating the ticket state to filter by. If <see langword="true"/>, only open tickets are
    /// shown; if <see langword="false"/>, only closed tickets are shown; if <see langword="null"/>, all tickets are
    /// included.</param>
    /// <param name="page">The page number to display. Must be greater than or equal to 1.</param>
    /// <param name="pageSize">The number of tickets to display per page. Must be greater than 0.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> that
    /// renders the ticket list view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(bool? ticketState = null, int page = 1, int pageSize = 10)
    {
        return View(await GetPaginatedTickets(ticketState, page, pageSize));
    }

    /// <summary>
    /// Retrieves a paginated list of tickets, optionally filtered by ticket state, along with pagination and summary
    /// information.
    /// </summary>
    /// <param name="ticketState">An optional value indicating the ticket state to filter by. Specify <see langword="true"/> to include only open
    /// tickets, <see langword="false"/> to include only closed tickets, or <see langword="null"/> to include all
    /// tickets.</param>
    /// <param name="page">The page number to retrieve. Must be greater than or equal to 1.</param>
    /// <param name="pageSize">The number of tickets to include on each page. Must be greater than 0.</param>
    /// <returns>A <see cref="TicketPaginationViewModel"/> containing the paginated list of tickets, pagination details, and
    /// summary counts of open and closed tickets.</returns>
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

    /// <summary>
    /// Displays the details of a specific ticket identified by its ID.
    /// </summary>
    /// <remarks>If <paramref name="id"/> is null, the user is redirected to the ticket list. If the specified
    /// ticket does not exist, a 404 Not Found result is returned. The view model includes information about whether the
    /// current user can edit the ticket.</remarks>
    /// <param name="id">The unique identifier of the ticket to display. If null, the method redirects to the ticket list.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the ticket details view if the ticket is found; otherwise, a
    /// redirect to the ticket list or a not found result.</returns>
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

    /// <summary>
    /// Returns the view for creating a new resource.
    /// </summary>
    /// <returns>A view that enables the user to enter details for a new resource.</returns>
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handles HTTP POST requests to create a new ticket using the provided model.
    /// </summary>
    /// <remarks>If the model state is invalid, the method returns the view with validation errors to allow
    /// the user to correct the input.</remarks>
    /// <param name="model">The data used to create the new ticket. Must contain valid ticket information; otherwise, the creation will not
    /// proceed.</param>
    /// <returns>A redirect to the ticket list view if the ticket is created successfully; otherwise, the view displaying
    /// validation errors and the submitted model.</returns>
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

    /// <summary>
    /// Displays the edit form for the ticket with the specified identifier.
    /// </summary>
    /// <remarks>Use this action to retrieve the current details of a ticket for editing. If the ticket does
    /// not exist, a 404 Not Found response is returned.</remarks>
    /// <param name="id">The unique identifier of the ticket to edit.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the edit view for the specified ticket if found; otherwise, a
    /// NotFound result.</returns>
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

    /// <summary>
    /// Updates the details of an existing ticket using the provided model data.
    /// </summary>
    /// <remarks>This action requires a valid ticket ID in the model. If the ticket is not found, a 404 Not
    /// Found response is returned. If model validation fails, a 400 Bad Request response is returned.</remarks>
    /// <param name="model">The view model containing the updated ticket information. Must have a valid ticket ID and pass model validation.</param>
    /// <returns>A redirect to the ticket details view if the update is successful; otherwise, a Bad Request or Not Found result
    /// if the model is invalid or the ticket does not exist.</returns>
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

    /// <summary>
    /// Deletes the ticket with the specified identifier and redirects to the ticket list view.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to delete.</param>
    /// <returns>A redirect to the ticket list view if the ticket is deleted; otherwise, a NotFound result if the ticket does not
    /// exist.</returns>
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

    /// <summary>
    /// Toggles the open or closed state of the specified ticket and updates its status in the database.
    /// </summary>
    /// <remarks>This action inverts the ticket's open state. If the ticket is closed, the closure timestamp
    /// is set; if reopened, the closure timestamp is cleared. The ticket's last modified time is always
    /// updated.</remarks>
    /// <param name="id">The unique identifier of the ticket to update.</param>
    /// <returns>A redirect to the details view of the updated ticket if the operation succeeds; otherwise, a NotFound result if
    /// the ticket does not exist.</returns>
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

    /// <summary>
    /// Handles a POST request to cancel an operation and redirects to the details view for the specified item.
    /// </summary>
    /// <param name="id">The identifier of the item to display in the details view after cancellation.</param>
    /// <returns>A redirect to the details view for the specified item.</returns>
    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        return RedirectToAction("Details", new { id = id });
    }
}