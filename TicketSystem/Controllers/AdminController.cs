using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketSystem.Application.Services;
using TicketSystem.Domain.Models;

namespace TicketSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDepartmentService _departmentService;
        private readonly ITicketAssigneeService _ticketAssigneeService;
        private readonly IMessageService _messageService;

        public AdminController(
            ITicketService ticketService,
            IUserService userService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            IDepartmentService departmentService,
            ITicketAssigneeService ticketAssigneeService,
            IMessageService messageService)
        {
            _ticketService = ticketService;
            _userService = userService;
            _categoryService = categoryService;
            _userManager = userManager;
            _departmentService = departmentService;
            _ticketAssigneeService = ticketAssigneeService;
            _messageService = messageService; //neu
            
        }


        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            if (tickets == null)
            {
                tickets = new List<Ticket>();
            }
            return View(tickets);
        }

        //  USER VERWALTEN 
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            var adminEmails = new List<string>();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    adminEmails.Add(user.Email);
                }
            }
            ViewBag.AdminEmails = adminEmails;
            return View(users);
        }

        // User zu Admin machen
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Success"] = $"{user.Email} wurde zum Admin gemacht.";
            }
            return RedirectToAction("Users");
        }

        //  USER SPERREN 
        [HttpPost]
        public async Task<IActionResult> LockUser(string id, string reason)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsLocked = true;
                user.LockReason = reason;
                user.LockedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"{user.Name} wurde gesperrt! Grund: {reason}";
            }
            return RedirectToAction("Users");
        }

        // User entsperren
        [HttpPost]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsLocked = false;
                user.LockReason = null;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"{user.Name} wurde entsperrt!";
            }
            return RedirectToAction("Users");
        }

        //TICKET 
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                await _ticketService.DeleteTicketAsync(id);
                TempData["Success"] = "Ticket wurde gelöscht!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Löschen: {ex.Message}";
            }
            return RedirectToAction("Index");
        }

        // (Team-Zusammenarbeit)
        [HttpPost]
        public async Task<IActionResult> AddAssignee(int ticketId, string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                // Mitarbeiter zuweisen
                await _ticketAssigneeService.AddAssigneeAsync(ticketId, userId);

                // Benachrichtigung senden
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                var currentUser = await _userManager.GetUserAsync(User);
                var assignedUser = await _userManager.FindByIdAsync(userId);

                if (ticket != null && assignedUser != null)
                {
                    var message = new Message
                    {
                        SenderId = currentUser.Id,
                        ReceiverId = assignedUser.Id,
                        Content = $"Sie wurden dem Ticket \"{ticket.Title}\" (ID: {ticket.Id}) als Mitarbeiter zugewiesen.",
                        SentAt = DateTime.UtcNow,
                        IsRead = false
                    };
                    await _messageService.SendMessageAsync(message);
                }
                ////

                TempData["Success"] = $"Mitarbeiter wurde zum Ticket hinzugefügt und benachrichtigt!";
            }
            return RedirectToAction("ManageAssignees", new { ticketId });
        }
        // mitarbeiter verwalten (Team-Zusammenarbeit)
        [HttpGet]
        public async Task<IActionResult> ManageAssignees(int ticketId)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            if (ticket == null) return NotFound();

            var currentAssignees = await _ticketAssigneeService.GetAssigneesByTicketIdAsync(ticketId);
            var allUsers = await _userService.GetAllUsersAsync();
            var availableUsers = allUsers.Where(u => !currentAssignees.Any(a => a.UserId == u.Id)).ToList();

            ViewBag.Ticket = ticket;
            ViewBag.CurrentAssignees = currentAssignees;
            ViewBag.AvailableUsers = new SelectList(availableUsers, "Id", "Name");

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAssignee(int assigneeId, int ticketId)
        {
            await _ticketAssigneeService.RemoveAssigneeAsync(assigneeId);
            TempData["Success"] = "Mitarbeiter wurde entfernt!";
            return RedirectToAction("ManageAssignees", new { ticketId });
        }


        //category
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory(string name = "")
        {
            if (!string.IsNullOrEmpty(name))
            {
                ViewBag.PreFilledName = name;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(category);
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction("Categories");
        }

        // ABTEILUNGEN 
        public async Task<IActionResult> Departments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        [HttpGet]
        public IActionResult CreateDepartment() => View();

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.CreateDepartmentAsync(department);
                return RedirectToAction("Departments");
            }
            return View(department);
        }

        [HttpGet]
        public async Task<IActionResult> EditDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            await _departmentService.UpdateDepartmentAsync(department);
            return RedirectToAction("Departments");
        }

        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return RedirectToAction("Departments");
        }
    }
}