using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Azure.Documents;
//using Nest;
using System.Net.Mail;
using System.Security.Claims;
using TicketSystem.Application.Services;
using TicketSystem.Domain.Models;

namespace TicketSystem.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ICategoryService _categoryService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;
        private readonly IBlockedByService _blockedByService;
        private readonly IDepartmentService _departmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(ITicketService ticketService,
            ICategoryService categoryService,
            IAttachmentService attachmentService,
            IUserService userService,
            ICommentService commentService,
            IBlockedByService blockedByService,
            IDepartmentService departmentService,
            UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _categoryService = categoryService;
            _attachmentService = attachmentService;
            _userService = userService;
            _commentService = commentService;
            _blockedByService = blockedByService;
            _departmentService = departmentService;
            _userManager = userManager;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index(string departmentFilter)
        {
            IEnumerable<Ticket> tickets;

            if (User.IsInRole("Admin"))
            {
                // Admin sieht alle Tickets
                tickets = await _ticketService.GetAllTicketsAsync();
            }
            else if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

                // User sieht: 
                // 1. Tickets seiner Abteilung
                // 2. Tickets die er erstellt hat
                // 3. Tickets die ihm zugewiesen sind neu
                var userTickets = await _ticketService.GetTicketsByUserDepartmentAsync(userId);
                var assignedTickets = await _ticketService.GetTicketsAssignedToUserAsync(userId);

                tickets = userTickets.Concat(assignedTickets).Distinct().ToList();
            }
            else
            {
                tickets = new List<Ticket>();
            }

            // Filter nach Abteilung
            if (!string.IsNullOrEmpty(departmentFilter) && int.TryParse(departmentFilter, out int depId))
            {
                tickets = tickets.Where(t => t.DepartmentId == depId);
            }

            // Ersteller Namen laden
            var creatorNames = new Dictionary<string, string>();
            foreach (var ticket in tickets)
            {
                if (!string.IsNullOrEmpty(ticket.ApplicationUserId) && !creatorNames.ContainsKey(ticket.ApplicationUserId))
                {
                    var user = await _userService.GetUserByIdAsync(ticket.ApplicationUserId);
                    creatorNames[ticket.ApplicationUserId] = user?.Name ?? user?.Email ?? "Unbekannt";
                }
            }

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = departments;
            ViewBag.CreatorNames = creatorNames;

            return View(tickets);
        }

        //  CREATE 
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Ticket ticket, IFormFile? file)
        {
            if (!ticket.CategoryId.HasValue || ticket.CategoryId == 0)
                ModelState.AddModelError("CategoryId", "Bitte wählen Sie eine Kategorie!");

            if (string.IsNullOrWhiteSpace(ticket.Title))
                ModelState.AddModelError("Title", "Bitte geben Sie einen Titel ein!");

            if (string.IsNullOrWhiteSpace(ticket.Description))
                ModelState.AddModelError("Description", "Bitte geben Sie eine Beschreibung ein!");

            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                var departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                return View(ticket);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var createdTicket = await _ticketService.CreateTicketAsync(ticket, userId);

            if (file != null && file.Length > 0 && createdTicket != null)
            {
                try
                {
                    await _attachmentService.UploadAttachmentAsync(createdTicket.Id, file, userId);
                    TempData["Success"] = "Ticket wurde erfolgreich erstellt und Datei hochgeladen!";
                }
                catch (Exception ex)
                {
                    TempData["Warning"] = $"Ticket erstellt, aber Datei konnte nicht hochgeladen werden: {ex.Message}";
                }
            }
            else
            {
                TempData["Success"] = "Ticket wurde erfolgreich erstellt!";
            }

            return RedirectToAction("Index");
        }

        // DETAILS 
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null) return NotFound();

            string creatorName = "Unbekannt";
            if (!string.IsNullOrEmpty(ticket.ApplicationUserId))
            {
                var creator = await _userService.GetUserByIdAsync(ticket.ApplicationUserId);
                creatorName = creator?.Name ?? creator?.Email ?? "Unbekannt";
            }
            ViewBag.CreatorName = creatorName;

            var attachments = await _attachmentService.GetAttachmentsByTicketIdAsync(id);
            ViewBag.Attachments = attachments;

            var comments = await _commentService.GetCommentsByTicketIdAsync(id);
            ViewBag.Comments = comments;

            var blockingTickets = await _blockedByService.GetBlockingTicketsAsync(id);
            ViewBag.BlockingTickets = blockingTickets;

            var availableTickets = await _blockedByService.GetAvailableTicketsForBlockingAsync(id);
            ViewBag.AvailableTickets = availableTickets;

            ViewBag.CanClose = await _blockedByService.CanCloseTicketAsync(id);

            return View(ticket);
        }

        // COmMENT
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int ticketId, string content)
        {
            Console.WriteLine($"AddComment wurde aufgerufen: ticketId={ticketId}, content={content}");

            System.Diagnostics.Debug.WriteLine($"ticketId: {ticketId}");
            System.Diagnostics.Debug.WriteLine($"content: {content}");
            System.Diagnostics.Debug.WriteLine($"User: {User.Identity?.Name}");

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Kommentar darf nicht leer sein.";
                return RedirectToAction("Details", new { id = ticketId });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _commentService.AddCommentAsync(ticketId, content, userId ?? "");
                TempData["Success"] = "Kommentar hinzugefügt!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FEHLER: {ex.Message}");
                TempData["Error"] = $"Fehler: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = ticketId });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int id, int ticketId)
        {
            try
            {
                await _commentService.DeleteCommentAsync(id);
                TempData["Success"] = "Kommentar wurde gelöscht!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Löschen: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = ticketId });
        }


        //edit
        [HttpGet]
        [Authorize(Roles = "Admin, Teamleiter")]
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null) return NotFound();

            // Teamleiter darf nur Tickets seiner Abteilung bearbeiten
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (ticket.DepartmentId != user.DepartmentId)
                {
                    TempData["Error"] = "Sie können nur Tickets Ihrer Abteilung bearbeiten!";
                    return RedirectToAction("Index");
                }
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", ticket.CategoryId);

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", ticket.DepartmentId);

            return View(ticket);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", ticket.CategoryId);
                var departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Departments = new SelectList(departments, "Id", "Name", ticket.DepartmentId);
                return View(ticket);
            }

            ticket.UpdatedAt = DateTime.UtcNow;
            await _ticketService.UpdateTicketAsync(ticket);

            TempData["Success"] = "Ticket wurde aktualisiert!";
            return RedirectToAction("Index");
        }

        // runterladen 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var fileBytes = await _attachmentService.DownloadAttachmentAsync(id);
            if (fileBytes == null)
                return NotFound("Datei konnte nicht geladen werden.");

            var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
            if (attachment == null)
                return NotFound("Attachment nicht gefunden.");

            return File(fileBytes, attachment.ContentType, attachment.FileName);
        }

        // atachment löschen
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            try
            {
                await _attachmentService.DeleteAttachmentAsync(id);
                TempData["Success"] = "Datei wurde gelöscht!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Löschen: {ex.Message}";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // add block
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBlock(int ticketId, int blockedByTicketId)
        {
            try
            {
                await _blockedByService.AddBlockAsync(ticketId, blockedByTicketId);
                TempData["Success"] = "Blockierung wurde hinzugefügt!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = ticketId });
        }

        // block entfernen
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveBlock(int blockId, int ticketId)
        {
            try
            {
                await _blockedByService.RemoveBlockAsync(blockId);
                TempData["Success"] = "Blockierung wurde entfernt!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = ticketId });
        }

        // ticket schließen
        [Authorize(Roles = "Admin, Teamleiter")]
        public async Task<IActionResult> Close(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null) return NotFound();

            // Teamleiter darf nur Tickets seiner Abteilung schließen
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (ticket.DepartmentId != user.DepartmentId)
                {
                    TempData["Error"] = "Sie können nur Tickets Ihrer Abteilung schließen!";
                    return RedirectToAction("Index");
                }
            }

            if (!await _blockedByService.CanCloseTicketAsync(id))
            {
                TempData["Error"] = "Dieses Ticket kann nicht geschlossen werden, weil noch blockierende Tickets offen sind!";
                return RedirectToAction("Details", new { id });
            }

            ticket.Status = TicketStatus.Closed;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _ticketService.UpdateTicketAsync(ticket);

            TempData["Success"] = "Ticket wurde geschlossen!";
            return RedirectToAction("Index");
        }
    }
}