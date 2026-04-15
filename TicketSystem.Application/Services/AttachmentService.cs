using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHostEnvironment _environment;

        // Limitierungen
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
        private static readonly string[] AllowedExtensions = {
            ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".txt", ".zip" };

        public AttachmentService(IAttachmentRepository attachmentRepository, IHostEnvironment environment)
        {
            _attachmentRepository = attachmentRepository;
            _environment = environment;
        }

        public async Task<IEnumerable<Attachment>> GetAttachmentsByTicketIdAsync(int ticketId)
        {
            return await _attachmentRepository.GetByTicketIdAsync(ticketId);
        }

        
        public async Task<Attachment?> GetAttachmentByIdAsync(int attachmentId)
        {
            return await _attachmentRepository.GetByIdAsync(attachmentId);
        }

        public async Task<Attachment?> UploadAttachmentAsync(int ticketId, IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                return null;

            if (!IsValidFile(file))
                throw new InvalidOperationException("Dateityp oder -größe ist nicht erlaubt.");

            
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads", "tickets", ticketId.ToString());

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

           
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            
            var relativePath = Path.Combine("uploads", "tickets", ticketId.ToString(), uniqueFileName).Replace('\\', '/');

            // Datei speichern
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                FileName = file.FileName,
                FilePath = relativePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                TicketId = ticketId,
                ApplicationUserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            return await _attachmentRepository.CreateAsync(attachment);
        }

        public async Task<byte[]?> DownloadAttachmentAsync(int attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null)
                return null;

            var fullPath = Path.Combine(_environment.ContentRootPath, attachment.FilePath.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(fullPath))
                return null;

            return await File.ReadAllBytesAsync(fullPath);
        }

        public async Task DeleteAttachmentAsync(int attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment != null)
            {
                // vom Server löschen
                var fullPath = Path.Combine(_environment.ContentRootPath, attachment.FilePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

               
                await _attachmentRepository.DeleteAsync(attachmentId);
            }
        }

        public bool IsValidFile(IFormFile file)
        {
            // Prüfe 
            if (file.Length > MaxFileSize)
                return false;

            // Prüfe 
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }
    }
}