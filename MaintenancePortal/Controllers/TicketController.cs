using Microsoft.AspNetCore.Mvc;

namespace MaintenancePortal.Controllers;

public class TicketController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
