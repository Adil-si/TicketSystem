using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
    public interface IAttachmentRepository
    {
        Task<Attachment?> GetByIdAsync(int id);
        Task<IEnumerable<Attachment>> GetByTicketIdAsync(int ticketId);
        Task<Attachment> CreateAsync(Attachment attachment);
        Task DeleteAsync(int id);
        Task DeleteByTicketIdAsync(int ticketId);
    }
}
