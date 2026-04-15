using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Domain.Models;
using TicketSystem.Application.Services;
using System.Linq;

namespace TicketSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDepartmentService _departmentService;  

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IDepartmentService departmentService) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _departmentService = departmentService;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var defaultDepartment = departments.FirstOrDefault(d => d.Name == "IT");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = name,
                DepartmentId = defaultDepartment?.Id
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Ticket");
            }

            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // Prüfen
            if (user != null && user.IsLocked)
            {
                ViewBag.Error = $"Ihr Account wurde gesperrt. Grund: {user.LockReason ?? "Bitte kontaktieren Sie den Admin."}";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded) return RedirectToAction("Index", "Ticket");

            ViewBag.Error = "Falsche Anmeldedaten!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}