using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Services;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;
using TicketSystem.Infrastructure.Repositories;

namespace TicketSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Datenbank
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration
                    .GetConnectionString("DefaultConnection")));

            // Identity 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            // Cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // Repositories
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IBlockedByRepository, BlockedByRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<ITicketAssigneeRepository, TicketAssigneeRepository>();

            // Services
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IBlockedByService, BlockedByService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<ITicketAssigneeService, TicketAssigneeService>();

            // Dashboard Service
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            builder.Services.AddScoped<IRandomUserService, RandomUserService>();

            // MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // ABTEILUNGEN ERSTELLEN (VOR USERS!)
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!dbContext.Departments.Any())
                {
                    dbContext.Departments.AddRange(
                        new Department { Name = "IT", Description = "Information Technology" },
                        new Department { Name = "HR", Description = "Human Resources" },
                        new Department { Name = "Sales", Description = "Vertrieb" },
                        new Department { Name = "Marketing", Description = "Marketing" },
                        new Department { Name = "Finance", Description = "Finanzen" },
                        new Department { Name = "Legal", Description = "Rechtsabteilung" },
                        new Department { Name = "Customer Support", Description = "Kundensupport" },
                        new Department { Name = "Production", Description = "Produktion" },
                        new Department { Name = "Logistics", Description = "Logistik" },
                        new Department { Name = "Administration", Description = "Verwaltung" }
                        
                    );
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Abteilungen wurden erfolgreich hinzugefügt!");
                }
            }

            //  USERS + ADMIN ERSTELLEN (NACH ABTEILUNGEN!)
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Admin-Rolle
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Teamleiter-Rolle 
                if (!await roleManager.RoleExistsAsync("Teamleiter"))
                    await roleManager.CreateAsync(new IdentityRole("Teamleiter"));

                // IT Abteilung ID holen
                var itDepartment = await dbContext.Departments.FirstOrDefaultAsync(d => d.Name == "IT");
                var itDepartmentId = itDepartment?.Id;

                // Test-User
                var testEmail = "john@doe.de";
                var testUser = await userManager.FindByEmailAsync(testEmail);
                if (testUser == null)
                {
                    testUser = new ApplicationUser
                    {
                        UserName = testEmail,
                        Email = testEmail,
                        EmailConfirmed = true,
                        Name = "John Doe",
                        DepartmentId = itDepartmentId
                    };
                    await userManager.CreateAsync(testUser, "test123");
                }
                var hrDepartment = await dbContext.Departments.FirstOrDefaultAsync(d => d.Name == "HR");
                var lisaEmail = "lisa@schmidt.de";
                var lisaUser = await userManager.FindByEmailAsync(lisaEmail);
                if (lisaUser == null)
                {
                    lisaUser = new ApplicationUser
                    {
                        UserName = lisaEmail,
                        Email = lisaEmail,
                        EmailConfirmed = true,
                        Name = "Lisa Schmidt",
                        DepartmentId = hrDepartment?.Id
                    };
                    await userManager.CreateAsync(lisaUser, "test123");
                }
                // Teamleiter-User
                var teamleiterEmail = "teamleiter@ticket.de";
                var teamleiterUser = await userManager.FindByEmailAsync(teamleiterEmail);
                if (teamleiterUser == null)
                {
                    teamleiterUser = new ApplicationUser
                    {
                        UserName = teamleiterEmail,
                        Email = teamleiterEmail,
                        EmailConfirmed = true,
                        Name = "Teamleiter",
                        DepartmentId = itDepartmentId  
                    };
                    await userManager.CreateAsync(teamleiterUser, "Team123!");
                    await userManager.AddToRoleAsync(teamleiterUser, "Teamleiter");
                }
                // Admin-User
                var adminEmail = "admin@ticket.de";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        Name = "Admin User"
                    };
                    await userManager.CreateAsync(adminUser, "Admin123!");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            app.Run();
        }
    }
}