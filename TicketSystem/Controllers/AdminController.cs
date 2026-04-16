using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TicketSystem.Application.Services;
using TicketSystem.Domain.Models;

namespace TicketSystem.Controllers
{
    [Authorize(Roles = "Admin, Teamleiter")]
    public class AdminController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDepartmentService _departmentService;
        private readonly ITicketAssigneeService _ticketAssigneeService;
        private readonly IMessageService _messageService;
        private readonly IRandomUserService _randomUserService;

        public AdminController(
            ITicketService ticketService,
            IUserService userService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            IDepartmentService departmentService,
            ITicketAssigneeService ticketAssigneeService,
            IMessageService messageService, IRandomUserService randomUserService)
        {
            _ticketService = ticketService;
            _userService = userService;
            _categoryService = categoryService;
            _userManager = userManager;
            _departmentService = departmentService;
            _ticketAssigneeService = ticketAssigneeService;
            _messageService = messageService;
            _randomUserService = randomUserService;

        }

        // dashboard 
        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();

            return View(tickets);
        }

        // user management (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            var adminEmails = new List<string>();
            var teamleiterIds = new List<string>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    adminEmails.Add(user.Email);
                }
                if (await _userManager.IsInRoleAsync(user, "Teamleiter"))
                {
                    teamleiterIds.Add(user.Id);
                }
            }

            ViewBag.AdminEmails = adminEmails;
            ViewBag.TeamleiterIds = teamleiterIds;
            return View(users);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
            else
            {
                TempData["Error"] = "Benutzer nicht gefunden!";
            }
            return RedirectToAction("Users");
        }

        [Authorize(Roles = "Admin")]
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

        //Ticket löschen (Admin only)
        [Authorize(Roles = "Admin")]
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

        // Mitarbeiter zu Ticket hinzufügen (Admin + Teamleiter, aber Teamleiter nur für eigene Abteilung)
        [HttpPost]
        public async Task<IActionResult> AddAssignee(int ticketId, string userId)
        {
            // Teamleiter darf nur Tickets seiner Abteilung bearbeiten
            if (!User.IsInRole("Admin"))
            {
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                var currentUser = await _userManager.GetUserAsync(User);
                if (ticket != null && ticket.DepartmentId != currentUser.DepartmentId)
                {
                    TempData["Error"] = "Sie können nur Tickets Ihrer Abteilung bearbeiten!";
                    return RedirectToAction("Index");
                }
            }

            if (!string.IsNullOrEmpty(userId))
            {
                await _ticketAssigneeService.AddAssigneeAsync(ticketId, userId);

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

                TempData["Success"] = $"Mitarbeiter wurde zum Ticket hinzugefügt und benachrichtigt!";
            }
            return RedirectToAction("ManageAssignees", new { ticketId });
        }


        // Mitarbeiter von Ticket entfernen (Admin + Teamleiter, aber Teamleiter nur für eigene Abteilung)
        [HttpGet]
        public async Task<IActionResult> ManageAssignees(int ticketId)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            if (ticket == null) return NotFound();

            // Teamleiter darf nur Tickets seiner Abteilung sehen
            if (!User.IsInRole("Admin"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (ticket.DepartmentId != currentUser.DepartmentId)
                {
                    TempData["Error"] = "Sie können nur Tickets Ihrer Abteilung bearbeiten!";
                    return RedirectToAction("Index");
                }
            }

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

        //kategorien management
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

        // =abteilungsmanagement (Admin only) =
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Departments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateDepartment() => View();

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            return View(department);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            await _departmentService.UpdateDepartmentAsync(department);
            return RedirectToAction("Departments");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return RedirectToAction("Departments");
        }
        // Teamleiter machen (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeTeamleiter(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Prüfen ob User bereits Admin ist
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    TempData["Error"] = "Admin kann nicht zum Teamleiter gemacht werden!";
                    return RedirectToAction("Users");
                }

                // Teamleiter Rolle hinzufügen
                await _userManager.AddToRoleAsync(user, "Teamleiter");
                TempData["Success"] = $"{user.Name} wurde zum Teamleiter gemacht!";
            }
            return RedirectToAction("Users");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveTeamleiter(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Teamleiter Rolle entfernen
                if (await _userManager.IsInRoleAsync(user, "Teamleiter"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Teamleiter");
                    TempData["Success"] = $"{user.Name} ist kein Teamleiter mehr!";
                }
                else
                {
                    TempData["Error"] = $"{user.Name} ist kein Teamleiter!";
                }
            }
            return RedirectToAction("Users");
        }
        // Teamleiter sieht Benutzer (nur lesen, keine Aktionen)
        [Authorize(Roles = "Admin, Teamleiter")]
        public async Task<IActionResult> TeamUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            //nur user der eigenen Abteilung anzeigen, wenn Teamleiter
            if (!User.IsInRole("Admin"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                users = users.Where(u => u.DepartmentId == currentUser.DepartmentId).ToList();
            }

            return View(users);
        }
        // benutzer generieren (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateRandomUsers(int count = 5)
        {
            var randomUsers = await _randomUserService.GenerateRandomUsersAsync(count);
            int created = 0;

            foreach (var randomUser in randomUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(randomUser.Email);
                if (existingUser == null)
                {
                    var result = await _userManager.CreateAsync(randomUser, "Demo123!");
                    if (result.Succeeded)
                    {
                        created++;
                    }
                }
            }

            TempData["Success"] = $"{created} von {count} zufällige Benutzer wurden erstellt!";
            return RedirectToAction("Users");
        }

        // avatar
        public async Task<IActionResult> UserAvatar(string email)
        {
            var avatarUrl = _randomUserService.GetAvatarUrl(email);
            using var client = new HttpClient();
            try
            {
                var imageBytes = await client.GetByteArrayAsync(avatarUrl);
                return File(imageBytes, "image/jpeg");
            }
            catch
            {
                // Fallback Avatar
                return File(new byte[0], "image/jpeg");
            }
        }
    }
}