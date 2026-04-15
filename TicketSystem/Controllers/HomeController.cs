using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Services;
using TicketSystem.Domain.Models;

namespace TicketSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IDepartmentService _departmentService;  

        public HomeController(
            IDashboardService dashboardService,
            ITicketService ticketService,
            IUserService userService,
            ICategoryService categoryService,
            IDepartmentService departmentService)  
        {
            _dashboardService = dashboardService;
            _ticketService = ticketService;
            _userService = userService;
            _categoryService = categoryService;
            _departmentService = departmentService;  
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchTerm, string category, string status, string sortBy, string department)
        {
            var dashboard = await _dashboardService.GetDashboardDataAsync();

            // Alle Tickets 
            var tickets = await _ticketService.GetAllTicketsAsync();
            var allTickets = tickets.ToList();

            // FILTER 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                allTickets = allTickets.Where(t =>
                    t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // FILTER nach Kategorie
            if (!string.IsNullOrEmpty(category) && category != "Alle")
            {
                allTickets = allTickets.Where(t => t.Category?.Name == category).ToList();
            }

            // FILTER nach Status
            if (!string.IsNullOrEmpty(status) && status != "Alle")
            {
                if (Enum.TryParse<TicketStatus>(status, out var statusEnum))
                {
                    allTickets = allTickets.Where(t => t.Status == statusEnum).ToList();
                }
            }

            // FILTER nach abteilung
            if (!string.IsNullOrEmpty(department) && int.TryParse(department, out int deptId))
            {
                allTickets = allTickets.Where(t => t.DepartmentId == deptId).ToList();
            }
            

            // Sortierung
            allTickets = sortBy switch
            {
                "name" => allTickets.OrderBy(t => t.Title).ToList(),
                "name_desc" => allTickets.OrderByDescending(t => t.Title).ToList(),
                "date_desc" => allTickets.OrderByDescending(t => t.CreatedAt).ToList(),
                _ => allTickets.OrderBy(t => t.CreatedAt).ToList()
            };

            // Ticket mit ersteller namen laden
            var ticketsWithCreator = new List<dynamic>();
            foreach (var ticket in allTickets)
            {
                string creatorName = "Unbekannt";
                if (!string.IsNullOrEmpty(ticket.ApplicationUserId))
                {
                    var user = await _userService.GetUserByIdAsync(ticket.ApplicationUserId);
                    creatorName = user?.Name ?? user?.Email ?? "Unbekannt";
                }
                ticketsWithCreator.Add(new { ticket, creatorName });
            }

            // Kategorien für Filter 
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

           
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = departments;
            // neu

            ViewBag.SelectedCategory = category;
            ViewBag.SelectedStatus = status;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.TicketsWithCreator = ticketsWithCreator;
            ViewBag.FilteredCount = allTickets.Count;

            return View(dashboard);
        }
    }
}