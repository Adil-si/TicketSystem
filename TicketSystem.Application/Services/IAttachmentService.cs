using Microsoft.AspNetCore.Http;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public interface IAttachmentService
    {
        Task<IEnumerable<Attachment>> GetAttachmentsByTicketIdAsync(int ticketId);
        Task<Attachment?> UploadAttachmentAsync(int ticketId, IFormFile file, string userId);
        Task<byte[]?> DownloadAttachmentAsync(int attachmentId);
        Task DeleteAttachmentAsync(int attachmentId);
        Task<Attachment?> GetAttachmentByIdAsync(int attachmentId); // Download
        bool IsValidFile(IFormFile file);
    }
}