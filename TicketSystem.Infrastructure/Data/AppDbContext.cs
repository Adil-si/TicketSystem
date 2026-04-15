using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Models;

namespace TicketSystem.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<BlockedBy> BlockedBy { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<TicketAssignee> TicketAssignees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TicketAssignee>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.HasOne(a => a.Ticket)
                .WithMany(t => t.Assignees)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.User)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(a => new { a.TicketId, a.UserId }).IsUnique();
        });

        // TICKET 
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasOne(t => t.Category)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.ApplicationUser)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ATTACHMENT 
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FileName).IsRequired().HasMaxLength(255);
            entity.Property(a => a.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ApplicationUserId).IsRequired(false);

            entity.HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Uploader)
                .WithMany()
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict statt SetNull
        });

        //  COMMENT
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).IsRequired().HasMaxLength(2000);
            entity.Property(c => c.ApplicationUserId).IsRequired(false);

            entity.HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        // MESSAGE
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).IsRequired().HasMaxLength(2000);

            entity.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(m => m.SentAt);
            entity.HasIndex(m => m.IsRead);
        });



        //  BLOCKEDBY 
        modelBuilder.Entity<BlockedBy>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.HasIndex(b => new { b.TicketId, b.BlockedByTicketId })
                    .IsUnique();

                entity.HasOne(b => b.Ticket)
                    .WithMany(t => t.BlockedByTickets)
                    .HasForeignKey(b => b.TicketId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.BlockedByTicket)
                    .WithMany(t => t.BlocksTickets)
                    .HasForeignKey(b => b.BlockedByTicketId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
}