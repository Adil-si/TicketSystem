using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Infrastructure.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly AppDbContext _context;

        public AttachmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Attachment?> GetByIdAsync(int id)
            => await _context.Attachments
                .Include(a => a.Uploader)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IEnumerable<Attachment>> GetByTicketIdAsync(int ticketId)
            => await _context.Attachments
                .Include(a => a.Uploader)
                .Where(a => a.TicketId == ticketId)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();

        public async Task<Attachment> CreateAsync(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task DeleteAsync(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByTicketIdAsync(int ticketId)
        {
            var attachments = await _context.Attachments
                .Where(a => a.TicketId == ticketId)
                .ToListAsync();

            _context.Attachments.RemoveRange(attachments);
            await _context.SaveChangesAsync();
        }
    }
}